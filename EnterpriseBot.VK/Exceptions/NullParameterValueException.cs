using System;

namespace EnterpriseBot.VK.Exceptions
{
    [Serializable]
    public class NullParameterValueException : Exception
    {
        public NullParameterValueException() { }
        public NullParameterValueException(string message) : base(message) { }
        public NullParameterValueException(string message, Exception inner) : base(message, inner) { }
        protected NullParameterValueException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
