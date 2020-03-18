﻿using ARConsistency.Extensions;
using ARConsistency.Helpers;
using ARConsistency.ResponseModels;
using ARConsistency.ResponseModels.Consistent;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ARConsistency
{
    public class ConsistencyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ResponseOptions _options;
        private readonly ResponseHelper _responseHelper;
        private readonly ILogger<ConsistencyMiddleware> _logger;

        public ConsistencyMiddleware(
            RequestDelegate next,
            IOptions<ResponseOptions> options,
            ILogger<ConsistencyMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
            _responseHelper = new ResponseHelper(_options);
        }

        public async Task InvokeAsync(HttpContext context)
        {
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

                    await HandleExceptionAsync(context, new ApiException(ex));
                }
            }
        }

        private async Task HandleRequestAsync(HttpContext context, string body)
        {
            int httpStatusCode = context.Response.StatusCode;
            if (string.IsNullOrEmpty(body) || httpStatusCode == StatusCodes.Status304NotModified)
                return;

            string bodyText = !body.IsValidJson() ? JsonHelper.ConvertToJsonString(body) : body;

            JToken deserializedResponse = JsonConvert.DeserializeObject<JToken>(bodyText);

            ConsistentApiResponse response = null;
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

            _responseHelper.FormatResponseAccordingToOptions(ref response, httpStatusCode);

            string serializedResponse = JsonHelper.ConvertResponseToJsonString(response, _options);
            await _responseHelper.WriteFormattedResponseToHttpContextAsync(context, httpStatusCode, serializedResponse);
        }

        private async Task HandleExceptionAsync(HttpContext context, ApiException exception)
        {
            ConsistentApiResponse response = exception.As<IConsistentable>().GetConsistentApiResponse();

            _logger.LogError(exception, null);
            _responseHelper.FormatResponseAccordingToOptions(ref response, exception.StatusCode);

            string serializedResponse = JsonHelper.ConvertResponseToJsonString(response, _options);
            await _responseHelper.WriteFormattedResponseToHttpContextAsync(context, exception.StatusCode, serializedResponse);
        }
    }
}