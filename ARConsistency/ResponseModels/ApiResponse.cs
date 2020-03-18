using ARConsistency.ResponseModels.Consistent;
using Newtonsoft.Json;

namespace ARConsistency.ResponseModels
{
    public class ApiResponse : IConsistentable
    {
        public string Version { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }

        [JsonProperty(ItemTypeNameHandling = TypeNameHandling.None, TypeNameHandling = TypeNameHandling.None)]
        public object Result { get; set; }

        public ApiResponse(string message, object result = null, int statusCode = 200, string apiVersion = "1.0.0.0")
        {
            StatusCode = statusCode;
            Message = message;
            Result = result;
            Version = apiVersion;
        }

        public ApiResponse(object result, int statusCode = 200)
        {
            StatusCode = statusCode;
            Result = result;
        }

        public ApiResponse() { }

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
