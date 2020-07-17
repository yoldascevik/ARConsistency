using System;

namespace TestApi.Exceptions
{
    public class ItemNotFoundException: Exception, IStatusCodedException
    {
        public ItemNotFoundException(int statusCode = 400)
        {
            StatusCode = statusCode;
        }
        
        public ItemNotFoundException(string message, int statusCode = 400)
            : base(message)
        {
           StatusCode = statusCode;
        }

        public int StatusCode { get; }
    }
}