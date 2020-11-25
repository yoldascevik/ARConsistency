using Microsoft.AspNetCore.Http;

namespace ARConsistency.Helpers
{
    internal class ResponseMessage
    {
        internal const string Success = "Request successful.";
        internal const string Created = "Entity created successful. Please see payload for more information.";
        internal const string NotFound = "Request not found. The specified uri or entity does not exist.";
        internal const string BadRequest = "Request invalid.";
        internal const string MethodNotAllowed = "Request responded with 'Method Not Allowed'.";
        internal const string NotContent = "Request no content. The specified uri does not contain any content.";
        internal const string Exception = "Request responded with exceptions.";
        internal const string UnAuthorized = "Access denied.";
        internal const string ValidationError = "Request responded with validation error(s). Please correct the specified validation errors and try again.";
        internal const string Unhandled = "An unhandled exception was thrown by the application.";

        internal static string GetResponseMessageByStatusCode(int? statusCode) =>
            statusCode switch
            {
                StatusCodes.Status200OK => ResponseMessage.Success,
                StatusCodes.Status201Created => ResponseMessage.Created,
                StatusCodes.Status204NoContent => ResponseMessage.NotContent,
                StatusCodes.Status400BadRequest => ResponseMessage.BadRequest,
                StatusCodes.Status401Unauthorized => ResponseMessage.UnAuthorized,
                StatusCodes.Status404NotFound => ResponseMessage.NotFound,
                StatusCodes.Status405MethodNotAllowed => ResponseMessage.MethodNotAllowed,
                StatusCodes.Status500InternalServerError => ResponseMessage.Unhandled,
                _ => ResponseMessage.Unhandled
            };
    }
}
