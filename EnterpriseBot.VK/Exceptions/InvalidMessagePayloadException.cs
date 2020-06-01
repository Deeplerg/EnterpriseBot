using System;

namespace EnterpriseBot.VK.Exceptions
{
    [Serializable]
    public class InvalidMessagePayloadException : Exception
    {
        public InvalidMessagePayloadException() { }
        public InvalidMessagePayloadException(string message) : base(message) { }
        public InvalidMessagePayloadException(string message, Exception inner) : base(message, inner) { }
        protected InvalidMessagePayloadException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
