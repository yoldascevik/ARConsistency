using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ARConsistency.Configuration
{
    public class ExceptionStatusCodeHandler
    {
        private Type _exceptionBaseType;
        private MemberTypes _memberType;
        private string _statusCodeMemberName;
        
        public void RegisterStatusCodedExceptionBaseType<TException>(
            Expression<Func<TException, int>> statusCodeExpression)
        {
            if (statusCodeExpression == null)
                throw new ArgumentNullException(nameof(statusCodeExpression));

            if (!(statusCodeExpression.Body is MemberExpression memberExpression))
                throw new ArgumentException("Unable to resolve StatusCode member in expression.");

            var memberType = memberExpression.Member.MemberType;
            if (memberType != MemberTypes.Field && memberType != MemberTypes.Property)
                throw new ArgumentException("StatusCode expression can be only Property or Field member.");

            _memberType = memberType;
            _statusCodeMemberName = memberExpression.Member.Name;
            _exceptionBaseType = memberExpression.Member.DeclaringType;
        }
        
        internal int? GetStatusCodeFromException(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));
            
            if (!IsStatusCodedException(exception))
                return null;

            object memberValue = _memberType switch
            {
                MemberTypes.Field => exception.GetType()
                    .GetField(_statusCodeMemberName)?
                    .GetValue(exception),
                MemberTypes.Property =>exception.GetType()
                    .GetProperty(_statusCodeMemberName)?
                    .GetValue(exception),
                _ => throw new NotImplementedException()
            };
                
            if (memberValue is int statusCode)
                return statusCode;

            return null;
        }
        
        private bool IsStatusCodedException(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));
            
            var exceptionType = exception.GetType();
            return _exceptionBaseType.IsInterface
                ? _exceptionBaseType.IsAssignableFrom(exceptionType)
                : _exceptionBaseType == exceptionType || exceptionType.IsSubclassOf(_exceptionBaseType);
        }
    }
}