using ARConsistency.ResponseModels.Base;
using System.Collections.Generic;
using System.Linq;

namespace ARConsistency.ResponseModels
{
    public class ApiException : System.Exception, IConsistentable
    {
        public string Version { get; set; }
        public int StatusCode { get; set; }
        public override string StackTrace { get; }
        public bool IsModelValidatonError => ValidationErrors != null && ValidationErrors.Any();
        public IEnumerable<ValidationError> ValidationErrors { get; set; }

        internal ApiException() { }

        public ApiException(string message, int statusCode = 400)
            : base(message)
        {
            this.StatusCode = statusCode;
        }

        public ApiException(IEnumerable<ValidationError> errors, int statusCode = 400)
        {
            this.StatusCode = statusCode;
            this.ValidationErrors = errors;
        }

        public ApiException(System.Exception ex, int statusCode = 500)
            : base(ex.Message)
        {
            StatusCode = statusCode;
            StackTrace = ex.StackTrace;
        }

        ConsistentApiResponse IConsistentable.GetConsistentApiResponse()
        {
            return new ConsistentApiResponse
            {
                Version = this.Version,
                StatusCode = this.StatusCode,
                ExceptionDetails = this.StackTrace,
                ExceptionMessage = this.Message,
                ValidationErrors = this.ValidationErrors
            };
        }
    }
}
