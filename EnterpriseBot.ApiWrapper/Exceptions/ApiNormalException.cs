using System;

namespace EnterpriseBot.ApiWrapper.Exceptions
{

    [Serializable]
    public class ApiNormalException : ApiException
    {
        public ApiNormalException() { }
        public ApiNormalException(string message) : base(message) { }
        public ApiNormalException(string message, Exception inner) : base(message, inner) { }
        protected ApiNormalException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
