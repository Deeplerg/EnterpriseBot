using System;

namespace EnterpriseBot.VK.Exceptions
{
    [Serializable]
    public class MenuNotFoundException : Exception
    {
        public MenuNotFoundException() { }
        public MenuNotFoundException(string message) : base(message) { }
        public MenuNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected MenuNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
