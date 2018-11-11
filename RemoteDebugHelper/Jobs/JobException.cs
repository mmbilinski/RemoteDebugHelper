using System;
using System.Runtime.Serialization;

namespace RemoteDebugHelper
{
    [Serializable]
    internal class JobException : Exception
    {
        public JobException()
        {
        }

        public JobException(string message) : base(message)
        {
        }

        public JobException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected JobException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}