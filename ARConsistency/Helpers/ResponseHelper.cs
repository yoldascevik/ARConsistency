using ARConsistency.ResponseModels.Base;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ARConsistency.Helpers
{
    internal class ResponseHelper
    {
        private readonly ResponseOptions _options;
        public ResponseHelper(ResponseOptions options)
        {
            _options = options;
        }

        internal void FormatResponseAccordingToOptions(ref ConsistentApiResponse response, int statusCode)
        {
            if (statusCode != StatusCodes.Status200OK && string.IsNullOrEmpty(response.Message))
                response.Message = ResponseMessage.GetResponseMessageByStatusCode(statusCode);

            response.Version = !_options.ShowApiVersion ? null : response.Version ?? _options.ApiVersion; 
            response.StatusCode = _options.ShowStatusCode ? statusCode : default;
            response.ExceptionDetails = _options.IsDebug ? response.ExceptionDetails : null;
        }

        internal async Task<string> ReadResponseBodyStreamAsync(Stream bodyStream)
        {
            bodyStream.Seek(0, SeekOrigin.Begin);
            string responseBody = await new StreamReader(bodyStream).ReadToEndAsync();
            bodyStream.Seek(0, SeekOrigin.Begin);

            return responseBody;
        }

        internal async Task WriteFormattedResponseToHttpContextAsync(HttpContext context, int code, string jsonString)
        {
            context.Response.StatusCode = code;
            context.Response.ContentType = "application/json";
            context.Response.ContentLength = jsonString != null ? Encoding.UTF8.GetByteCount(jsonString) : 0;

            await context.Response.WriteAsync(jsonString);
        }
    }
}
