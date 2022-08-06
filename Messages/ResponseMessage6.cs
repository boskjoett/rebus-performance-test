using System;

namespace RebusPerformanceTest.Messages
{
    public class ResponseMessage6
    {
        public Guid RequestId { get; }

        public DateTime RequestSendTime { get; }

        public DateTime ResponseReplyTime { get; }

        public string Message { get; }

        public ResponseMessage6(Guid requestId, DateTime requestSendTime, DateTime responseReplyTime, string message)
        {
            RequestId = requestId;
            RequestSendTime = requestSendTime;
            ResponseReplyTime = responseReplyTime;
            Message = message;
        }
    }
}
