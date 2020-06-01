using System;

namespace EnterpriseBot.VK.Exceptions
{
    [Serializable]
    public class KeyboardMaxButtonsException : Exception
    {
        public KeyboardMaxButtonsException() { }
        public KeyboardMaxButtonsException(string message) : base(message) { }
        public KeyboardMaxButtonsException(string message, Exception inner) : base(message, inner) { }
        protected KeyboardMaxButtonsException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
