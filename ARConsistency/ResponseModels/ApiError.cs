using System.Collections.Generic;
using ARConsistency.ResponseModels.Consistent;

namespace ARConsistency.ResponseModels
{
    public class ApiError : IConsistentable
    {
        public string Version { get; set; }
        public int StatusCode { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionDetails { get; set; }
        public IEnumerable<ValidationError> ValidationErrors { get; set; }

        public ApiError()
        {

        }

        public ApiError(string message)
        {
            this.ExceptionMessage = message;
        }

        public ApiError(string message, string details)
            : this(message)
        {
            this.ExceptionDetails = details;
        }

        public ApiError(string message, int statusCode)
            : this(message)
        {
            this.StatusCode = statusCode;
        }

        public ApiError(string message, int statusCode, string version)
            : this(message, statusCode)
        {
            this.Version = version;
        }

        public ApiError(string message, IEnumerable<ValidationError> validationErrors)
        {
            this.ExceptionMessage = message;
            this.ValidationErrors = validationErrors;
        }

        ConsistentApiResponse IConsistentable.GetConsistentApiResponse()
        {
            return new ConsistentApiResponse
            {
                Version = this.Version,
                StatusCode = this.StatusCode,
                ExceptionDetails = this.ExceptionDetails,
                ExceptionMessage = this.ExceptionMessage,
                ValidationErrors = this.ValidationErrors
            };
        }
    }
}
