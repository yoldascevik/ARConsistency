using ARConsistency.Extensions;
using ARConsistency.Helpers;
using ARConsistency.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ARConsistency.Abstractions;
using ARConsistency.Configuration;

namespace ARConsistency
{
    public class ConsistencyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ResponseOptions _options;
        private readonly ResponseHelper _responseHelper;
        private readonly ILogger<ConsistencyMiddleware> _logger;
        private readonly ExceptionStatusCodeHandler _exceptionStatusCodeHandler;

        public ConsistencyMiddleware(
            RequestDelegate next,
            ResponseOptions options,
            ILogger<ConsistencyMiddleware> logger, 
            ExceptionStatusCodeHandler exceptionStatusCodeHandler)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _exceptionStatusCodeHandler = exceptionStatusCodeHandler ?? throw new ArgumentNullException(nameof(exceptionStatusCodeHandler));
            _responseHelper = new ResponseHelper(_options);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!_responseHelper.ConsistentlyContentTypes.Contains(context.Response.ContentType))
            {
                await _next.Invoke(context);
                return;
            }
            
            Stream originalResponseBodyStream = context.Response.Body;
            await using MemoryStream memoryStream = new MemoryStream();
            {
                context.Response.Body = memoryStream;

                try
                {
                    await _next.Invoke(context);

                    context.Response.Body = originalResponseBodyStream;
                    await HandleRequestAsync(context, await _responseHelper.ReadResponseBodyStreamAsync(memoryStream));
                }
                catch (ApiException apiException)
                {
                    context.Response.Body = originalResponseBodyStream;
                    if (!_options.EnableExceptionLogging) throw;

                    await HandleExceptionAsync(context, apiException);
                }
                catch (Exception ex)
                {
                    context.Response.Body = originalResponseBodyStream;
                    if (!_options.EnableExceptionLogging) throw;

                    var apiException = new ApiException(ex);
                    apiException.StatusCode = _exceptionStatusCodeHandler.GetStatusCodeFromException(ex) ?? apiException.StatusCode;
                    
                    await HandleExceptionAsync(context, apiException);
                }
            }
        }

        private async Task HandleRequestAsync(HttpContext context, string body)
        {
            ConsistentApiResponse response = null;
            int httpStatusCode = context.Response.StatusCode == StatusCodes.Status204NoContent
                ? StatusCodes.Status200OK
                : context.Response.StatusCode;
            
            if (!string.IsNullOrEmpty(body))
            {
                string bodyText = !body.IsValidJson() ? JsonHelper.ConvertToJsonString(body) : body;
                JToken deserializedResponse = JsonConvert.DeserializeObject<JToken>(bodyText);

                if (deserializedResponse is JObject responseObj)
                {
                    Type type = responseObj["$type"]?.ToObject<Type>();

                    if (type != null && typeof(IConsistentable).IsAssignableFrom(type))
                    {
                        response = deserializedResponse.ToObject(type)
                            .As<IConsistentable>()
                            .GetConsistentApiResponse();
                    }
                }

                response ??= JsonHelper.GetConsistentApiResponseFromJsonToken(deserializedResponse);
            }
            else
            {
                response = new ConsistentApiResponse();
            }

            _responseHelper.FormatResponseAccordingToOptions(ref response, httpStatusCode);

            string serializedResponse = JsonHelper.ConvertResponseToJsonString(response, _options);
            await _responseHelper.WriteFormattedResponseToHttpContextAsync(context, response.StatusCode ?? httpStatusCode, serializedResponse);
        }

        private async Task HandleExceptionAsync(HttpContext context, ApiException exception)
        {
            ConsistentApiResponse response = exception.As<IConsistentable>().GetConsistentApiResponse();
            _responseHelper.FormatResponseAccordingToOptions(ref response, exception.StatusCode);
            _logger.LogError(exception, ResponseMessage.GetResponseMessageByStatusCode(response.StatusCode));
            
            string serializedResponse = JsonHelper.ConvertResponseToJsonString(response, _options);
            await _responseHelper.WriteFormattedResponseToHttpContextAsync(context, exception.StatusCode, serializedResponse);
        }
    }
}