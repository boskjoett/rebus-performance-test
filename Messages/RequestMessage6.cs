using System;

namespace RebusPerformanceTest.Messages
{
    public class RequestMessage6
    {
        public Guid RequestId { get;  }

        public DateTime SendTime { get; }

        public string Message { get; }

        public RequestMessage6(Guid requestId, DateTime sendTime, string message)
        {
            RequestId = requestId;
            SendTime = sendTime;
            Message = message;
        }
    }
}
