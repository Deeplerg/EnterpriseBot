using System;

namespace EnterpriseBot.VK.Exceptions
{
    [Serializable]
    public class InvalidGroupIdException : Exception
    {
        public InvalidGroupIdException() { }

        public InvalidGroupIdException(string message) : base(message) { }

        public InvalidGroupIdException(string message, Exception inner) : base(message, inner) { }

        protected InvalidGroupIdException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}