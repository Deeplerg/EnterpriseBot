using System;

namespace EnterpriseBot.ApiWrapper.Exceptions
{

    [Serializable]
    public class ApiCriticalException : ApiException
    {
        public ApiCriticalException() { }
        public ApiCriticalException(string message) : base(message) { }
        public ApiCriticalException(string message, Exception inner) : base(message, inner) { }
        protected ApiCriticalException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
