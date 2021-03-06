﻿using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace ARConsistency.Abstractions
{
    public class ConsistentApiResponse: ConsistentApiResponse<object>
    {
        public ConsistentApiResponse() { }

        public ConsistentApiResponse(object payload) :base(payload) { }

        public ConsistentApiResponse(string exceptionMessage, IEnumerable<ValidationError> validationErrors)
            : base(exceptionMessage, validationErrors) { }
        
    }    
    
    public class ConsistentApiResponse<TPayload>
    {
        public int? StatusCode { get; set; }
        public string Version { get; set; }
        public string Message { get; set; }
        public bool IsError => !string.IsNullOrEmpty(ExceptionMessage) || (ValidationErrors != null && ValidationErrors.Any());
        public string ExceptionMessage { get; set; }
        public string ExceptionDetails { get; set; } 
        public IEnumerable<ValidationError> ValidationErrors { get; set; }
        
        [JsonProperty(NullValueHandling =  NullValueHandling.Include)]
        public TPayload Payload { get; set; }

        public ConsistentApiResponse() { }

        public ConsistentApiResponse(TPayload payload)
            :this()
        {
            Payload = payload;
        }

        public ConsistentApiResponse(string exceptionMessage, IEnumerable<ValidationError> validationErrors)
        {
            ExceptionMessage = exceptionMessage;
            ValidationErrors = validationErrors;
            StatusCode = 400;
        }
    }
}
