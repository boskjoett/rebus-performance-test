using System;

namespace RebusPerformanceTest.Messages
{
    public class ResponseMessage3
    {
        public Guid RequestId { get; }

        public DateTime RequestSendTime { get; }

        public DateTime ResponseReplyTime { get; }

        public string Message { get; }

        public ResponseMessage3(Guid requestId, DateTime requestSendTime, DateTime responseReplyTime, string message)
        {
            RequestId = requestId;
            RequestSendTime = requestSendTime;
            ResponseReplyTime = responseReplyTime;
            Message = message;
        }
    }
}
