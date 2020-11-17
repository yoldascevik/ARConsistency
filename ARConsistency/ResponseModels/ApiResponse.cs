using ARConsistency.Abstractions;
using ARConsistency.ResponseModels.Base;
using Newtonsoft.Json;

namespace ARConsistency.ResponseModels
{
    public class ApiResponse : ArcObjectResult, IConsistentable
    {
        public string Version { get; set; }
        public string Message { get; set; }

        [JsonProperty(ItemTypeNameHandling = TypeNameHandling.None, TypeNameHandling = TypeNameHandling.None)]
        public object Result { get; set; }

        internal ApiResponse() { }

        public ApiResponse(object result, int statusCode = 200)
            : this()
        {
            StatusCode = statusCode;
            Result = result;
        }

        public ApiResponse(string message, object result = null, int statusCode = 200, string apiVersion = "1.0.0.0")
            : this(message, statusCode)
        {
            StatusCode = statusCode;
            Message = message;
            Result = result;
            Version = apiVersion;
        }

        ConsistentApiResponse IConsistentable.GetConsistentApiResponse()
        {
            return new ConsistentApiResponse
            {
                Message = this.Message,
                Version = this.Version,
                StatusCode = this.StatusCode,
                Payload = this.Result
            };
        }
    }
}
