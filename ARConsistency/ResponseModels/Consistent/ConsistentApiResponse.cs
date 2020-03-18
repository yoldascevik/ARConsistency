using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace ARConsistency.ResponseModels.Consistent
{
    public class ConsistentApiResponse
    {
        public int? StatusCode { get; set; }
        public string Version { get; set; }
        public string Message { get; set; }
        public bool IsError => !string.IsNullOrEmpty(ExceptionMessage) || (ValidationErrors != null && ValidationErrors.Any());
        public string ExceptionMessage { get; set; }
        public string ExceptionDetails { get; set; }
        public IEnumerable<ValidationError> ValidationErrors { get; set; }
        public object Payload { get; set; }

        public ConsistentApiResponse()
        {
          
        }

        public ConsistentApiResponse(object payload)
            :this()
        {
            Payload = payload;
        }

        public ConsistentApiResponse(string exceptionMessage, IEnumerable<ValidationError> validationErrors)
        {
            ExceptionMessage = exceptionMessage;
            ValidationErrors = validationErrors;
            StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}
