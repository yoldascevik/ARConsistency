using ARConsistency.ResponseModels.Base;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace ARConsistency.ResponseModels
{
    public class ApiError : ArcObjectResult, IConsistentable
    {
        public string Version { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionDetails { get; set; }
        public IEnumerable<ValidationError> ValidationErrors { get; set; }

        internal ApiError() { }

        public ApiError(string message)
            : this()
        {
            this.StatusCode = StatusCodes.Status400BadRequest;
            this.ExceptionMessage = message;
        }

        public ApiError(string message, string details)
            : this(message)
        {
            this.ExceptionDetails = details;
        }

        public ApiError(string message, int statusCode, string apiVersion = "1.0.0.0")
            : this(message)
        {
            this.StatusCode = statusCode;
            this.Version = apiVersion;
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
