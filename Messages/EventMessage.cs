using System;

namespace RebusPerformanceTest.Messages
{
    public class EventMessage
    {
        public Guid MessageId { get; }

        public DateTime PublishTime { get; }

        public string Event { get; }

        public EventMessage(Guid messageId, DateTime publishTime, string evt)
        {
            MessageId = messageId;
            PublishTime = publishTime;
            Event = evt;
        }
    }
}
