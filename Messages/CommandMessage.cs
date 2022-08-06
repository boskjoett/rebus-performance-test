using System;

namespace RebusPerformanceTest.Messages
{
    public class CommandMessage
    {
        public Guid MessageId { get; }

        public DateTime PublishTime { get; }

        public string Command { get; }

        public CommandMessage(Guid messageId, DateTime publishTime, string command)
        {
            MessageId = messageId;
            PublishTime = publishTime;
            Command = command;
        }
    }
}
