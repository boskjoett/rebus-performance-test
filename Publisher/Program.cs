using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Logging;
using Rebus.Messages;
using Rebus.RabbitMq;
using Rebus.Routing.TypeBased;
using RebusPerformanceTest.Messages;

namespace RebusPerformanceTest.Publisher
{
    class Program
    {
        static string _inputQueueName;
        static Dictionary<string, string> _headers;
        static IBus _bus;
        static Random _randomgenerator;
        static int _instance;
        static int _longestRoundtripDelayMs;
        static int _shortestRoundtripDelayMs = int.MaxValue;
        static int _longestSendDelayMs;
        static int _totalResponsesReceived;
        static int _minPublishIntervalInMs;
        static int _maxPublishIntervalInMs;
        static Dictionary<int, int> _requestsSent;
        static Dictionary<int, int> _responsesReceived;

        static async Task Main(string[] args)
        {
            _randomgenerator = new Random();

            _requestsSent = new Dictionary<int, int>();
            _responsesReceived = new Dictionary<int, int>();

            _headers = new Dictionary<string, string> {
                { RabbitMqHeaders.DeliveryMode, "1" },     // 1 = non-persistent, i.e. messages are not persisted by RabbitMQ
                { Headers.TimeToBeReceived, "00:01:00" },  // Set messages to expire after 1 minute,
                { Headers.Express, "" }                    // Deliver messages as fast as possible
            };

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

            IConfigurationRoot configuration = builder
                .AddEnvironmentVariables()
                .Build();

            _instance = int.Parse(configuration["Instance"]);
            int messagesToSend = int.Parse(configuration["MessagesToSend"]);

            _minPublishIntervalInMs = int.Parse(configuration["MinPublishIntervalInMs"]);
            _maxPublishIntervalInMs = int.Parse(configuration["MaxPublishIntervalInMs"]);

            _inputQueueName = $"Publisher{_instance}";
            Console.WriteLine($"Publisher {_instance} started");

            Console.WriteLine("Waiting for RabbitMQ to start");
            Thread.Sleep(20000);

            string rabbitMqConnectionString = configuration.GetConnectionString("RabbitMq");
            Console.WriteLine($"Connecting to Rebus at {rabbitMqConnectionString}");

            BuiltinHandlerActivator activator = new BuiltinHandlerActivator();

            activator.Handle<ResponseMessage1>(async msg => await HandleResponseMessage1(msg));
            activator.Handle<ResponseMessage2>(async msg => await HandleResponseMessage2(msg));
            activator.Handle<ResponseMessage3>(async msg => await HandleResponseMessage3(msg));
            activator.Handle<ResponseMessage4>(async msg => await HandleResponseMessage4(msg));
            activator.Handle<ResponseMessage5>(async msg => await HandleResponseMessage5(msg));
            activator.Handle<ResponseMessage6>(async msg => await HandleResponseMessage6(msg));
            activator.Handle<ResponseMessage7>(async msg => await HandleResponseMessage7(msg));
            activator.Handle<ResponseMessage8>(async msg => await HandleResponseMessage8(msg));
            activator.Handle<ResponseMessage9>(async msg => await HandleResponseMessage9(msg));
            activator.Handle<ResponseMessage10>(async msg => await HandleResponseMessage10(msg));

            _bus = Configure.With(activator)
                .Options(o => {
                    // Rebus defaults are 1 worker thread and a max parallelism of 5.
                    o.SetNumberOfWorkers(1);
                    o.SetMaxParallelism(5);
                })
                .Logging(l => l.Console(LogLevel.Warn))
                .Transport(t => t.UseRabbitMq(rabbitMqConnectionString, _inputQueueName))
                .Routing(r => r.TypeBased()
                    .Map<ResponseMessage1>(_inputQueueName)
                    .Map<ResponseMessage2>(_inputQueueName)
                    .Map<ResponseMessage3>(_inputQueueName)
                    .Map<ResponseMessage4>(_inputQueueName)
                    .Map<ResponseMessage5>(_inputQueueName)
                    .Map<ResponseMessage6>(_inputQueueName)
                    .Map<ResponseMessage7>(_inputQueueName)
                    .Map<ResponseMessage8>(_inputQueueName)
                    .Map<ResponseMessage9>(_inputQueueName)
                    .Map<ResponseMessage10>(_inputQueueName))
                .Start();

            _bus.Subscribe<ResponseMessage1>().Wait();
            _bus.Subscribe<ResponseMessage2>().Wait();
            _bus.Subscribe<ResponseMessage3>().Wait();
            _bus.Subscribe<ResponseMessage4>().Wait();
            _bus.Subscribe<ResponseMessage5>().Wait();
            _bus.Subscribe<ResponseMessage6>().Wait();
            _bus.Subscribe<ResponseMessage7>().Wait();
            _bus.Subscribe<ResponseMessage8>().Wait();
            _bus.Subscribe<ResponseMessage9>().Wait();
            _bus.Subscribe<ResponseMessage10>().Wait();

            Console.WriteLine($"Sending {messagesToSend} request messages to random subscribers");

            for (int i=1; i <= messagesToSend; i++)
            {
                await SendRequestMessageAsync(i);

                if (_minPublishIntervalInMs > 0 && _maxPublishIntervalInMs > _minPublishIntervalInMs)
                {
                    // Wait a random delay before sending next message
                    Thread.Sleep(_randomgenerator.Next(_minPublishIntervalInMs, _maxPublishIntervalInMs));
                }
            }

            // Wait for all responses to arrive
            Thread.Sleep(5000);

            // Write stats
            Console.WriteLine();
            Console.WriteLine("------------------------------------------");
            Console.WriteLine($"Longest roundtrip delay: {_longestRoundtripDelayMs} ms");
            Console.WriteLine($"Shortest roundtrip delay: {_shortestRoundtripDelayMs} ms");
            Console.WriteLine($"Longest send delay: {_longestSendDelayMs} ms");
            Console.WriteLine();

            foreach (KeyValuePair<int, int> item in _requestsSent)
            {
                int responses = 0;

                if (_responsesReceived.ContainsKey(item.Key))
                    responses = _responsesReceived[item.Key];

                Console.WriteLine($"Sent {item.Value} requests to subscriber {item.Key}. Got {responses} responses.");
            }

            Console.WriteLine($"Total reponses received: {_totalResponsesReceived}");
            Console.WriteLine("------------------------------------------");

            if (IsRunningInContainer())
            {
                new ManualResetEvent(false).WaitOne();
            }
            else
            {
                Console.WriteLine("\nPress any key to exit");
                Console.ReadKey();
            }

            _bus.Dispose();
            activator.Dispose();
        }

        private static async Task SendRequestMessageAsync(int messageCounter)
        {
            object requestMessage;
            Guid id = Guid.NewGuid();

            // Pick a random subscriber from 1 to 10 to send message to
            int targetSubscriber = _randomgenerator.Next(1, 11);

            string message = $"Request number {messageCounter} to subscriber {targetSubscriber} from publisher {_instance}";

            if (!_requestsSent.ContainsKey(targetSubscriber))
                _requestsSent.Add(targetSubscriber, 1);
            else
                _requestsSent[targetSubscriber]++;

            Console.WriteLine($"Publishing request message with ID {id} to subscriber {targetSubscriber}");

            switch (targetSubscriber)
            {
                case 1: requestMessage = new RequestMessage1(id, DateTime.Now, message); break;
                case 2: requestMessage = new RequestMessage2(id, DateTime.Now, message); break;
                case 3: requestMessage = new RequestMessage3(id, DateTime.Now, message); break;
                case 4: requestMessage = new RequestMessage4(id, DateTime.Now, message); break;
                case 5: requestMessage = new RequestMessage5(id, DateTime.Now, message); break;
                case 6: requestMessage = new RequestMessage6(id, DateTime.Now, message); break;
                case 7: requestMessage = new RequestMessage7(id, DateTime.Now, message); break;
                case 8: requestMessage = new RequestMessage8(id, DateTime.Now, message); break;
                case 9: requestMessage = new RequestMessage9(id, DateTime.Now, message); break;
                case 10: requestMessage = new RequestMessage10(id, DateTime.Now, message); break;

                default:
                    return;
            }

            await _bus.Publish(requestMessage, _headers);
        }

        private static async Task HandleResponseMessage1(ResponseMessage1 msg)
        {
            DateTime receiveTime = DateTime.Now;
            int delayMs = (int)receiveTime.Subtract(msg.RequestSendTime).TotalMilliseconds;
            if (delayMs > _longestRoundtripDelayMs)
                _longestRoundtripDelayMs = delayMs;

            if (delayMs < _shortestRoundtripDelayMs)
                _shortestRoundtripDelayMs = delayMs;

            int sendDelayMs = (int)msg.ResponseReplyTime.Subtract(msg.RequestSendTime).TotalMilliseconds;
            if (sendDelayMs > _longestSendDelayMs)
                _longestSendDelayMs = sendDelayMs;

            Console.WriteLine($"ResponseMessage1 received at {receiveTime}. ID: {msg.RequestId}, SendTime: {msg.RequestSendTime}, ReplyTime: {msg.ResponseReplyTime}, Message: {msg.Message}, Delay in ms: {delayMs}");

            if (!_responsesReceived.ContainsKey(1))
                _responsesReceived.Add(1, 1);
            else
                _responsesReceived[1]++;

            Interlocked.Increment(ref _totalResponsesReceived);
            await Task.CompletedTask;
        }

        private static async Task HandleResponseMessage2(ResponseMessage2 msg)
        {
            DateTime receiveTime = DateTime.Now;
            int delayMs = (int)receiveTime.Subtract(msg.RequestSendTime).TotalMilliseconds;
            if (delayMs > _longestRoundtripDelayMs)
                _longestRoundtripDelayMs = delayMs;

            if (delayMs < _shortestRoundtripDelayMs)
                _shortestRoundtripDelayMs = delayMs;

            int sendDelayMs = (int)msg.ResponseReplyTime.Subtract(msg.RequestSendTime).TotalMilliseconds;
            if (sendDelayMs > _longestSendDelayMs)
                _longestSendDelayMs = sendDelayMs;

            Console.WriteLine($"ResponseMessage2 received at {receiveTime}. ID: {msg.RequestId}, SendTime: {msg.RequestSendTime}, ReplyTime: {msg.ResponseReplyTime}, Message: {msg.Message}, Delay in ms: {delayMs}");

            if (!_responsesReceived.ContainsKey(2))
                _responsesReceived.Add(2, 1);
            else
                _responsesReceived[2]++;

            Interlocked.Increment(ref _totalResponsesReceived);
            await Task.CompletedTask;
        }

        private static async Task HandleResponseMessage3(ResponseMessage3 msg)
        {
            DateTime receiveTime = DateTime.Now;
            int delayMs = (int)receiveTime.Subtract(msg.RequestSendTime).TotalMilliseconds;
            if (delayMs > _longestRoundtripDelayMs)
                _longestRoundtripDelayMs = delayMs;

            if (delayMs < _shortestRoundtripDelayMs)
                _shortestRoundtripDelayMs = delayMs;

            int sendDelayMs = (int)msg.ResponseReplyTime.Subtract(msg.RequestSendTime).TotalMilliseconds;
            if (sendDelayMs > _longestSendDelayMs)
                _longestSendDelayMs = sendDelayMs;

            Console.WriteLine($"ResponseMessage3 received at {receiveTime}. ID: {msg.RequestId}, SendTime: {msg.RequestSendTime}, ReplyTime: {msg.ResponseReplyTime}, Message: {msg.Message}, Delay in ms: {delayMs}");

            if (!_responsesReceived.ContainsKey(3))
                _responsesReceived.Add(3, 1);
            else
                _responsesReceived[3]++;

            Interlocked.Increment(ref _totalResponsesReceived);
            await Task.CompletedTask;
        }

        private static async Task HandleResponseMessage4(ResponseMessage4 msg)
        {
            DateTime receiveTime = DateTime.Now;
            int delayMs = (int)receiveTime.Subtract(msg.RequestSendTime).TotalMilliseconds;
            if (delayMs > _longestRoundtripDelayMs)
                _longestRoundtripDelayMs = delayMs;

            if (delayMs < _shortestRoundtripDelayMs)
                _shortestRoundtripDelayMs = delayMs;

            int sendDelayMs = (int)msg.ResponseReplyTime.Subtract(msg.RequestSendTime).TotalMilliseconds;
            if (sendDelayMs > _longestSendDelayMs)
                _longestSendDelayMs = sendDelayMs;

            Console.WriteLine($"ResponseMessage4 received at {receiveTime}. ID: {msg.RequestId}, SendTime: {msg.RequestSendTime}, ReplyTime: {msg.ResponseReplyTime}, Message: {msg.Message}, Delay in ms: {delayMs}");

            if (!_responsesReceived.ContainsKey(4))
                _responsesReceived.Add(4, 1);
            else
                _responsesReceived[4]++;

            Interlocked.Increment(ref _totalResponsesReceived);
            await Task.CompletedTask;
        }

        private static async Task HandleResponseMessage5(ResponseMessage5 msg)
        {
            DateTime receiveTime = DateTime.Now;
            int delayMs = (int)receiveTime.Subtract(msg.RequestSendTime).TotalMilliseconds;
            if (delayMs > _longestRoundtripDelayMs)
                _longestRoundtripDelayMs = delayMs;

            if (delayMs < _shortestRoundtripDelayMs)
                _shortestRoundtripDelayMs = delayMs;

            int sendDelayMs = (int)msg.ResponseReplyTime.Subtract(msg.RequestSendTime).TotalMilliseconds;
            if (sendDelayMs > _longestSendDelayMs)
                _longestSendDelayMs = sendDelayMs;

            Console.WriteLine($"ResponseMessage5 received at {receiveTime}. ID: {msg.RequestId}, SendTime: {msg.RequestSendTime}, ReplyTime: {msg.ResponseReplyTime}, Message: {msg.Message}, Delay in ms: {delayMs}");

            if (!_responsesReceived.ContainsKey(5))
                _responsesReceived.Add(5, 1);
            else
                _responsesReceived[5]++;

            Interlocked.Increment(ref _totalResponsesReceived);
            await Task.CompletedTask;
        }

        private static async Task HandleResponseMessage6(ResponseMessage6 msg)
        {
            DateTime receiveTime = DateTime.Now;
            int delayMs = (int)receiveTime.Subtract(msg.RequestSendTime).TotalMilliseconds;
            if (delayMs > _longestRoundtripDelayMs)
                _longestRoundtripDelayMs = delayMs;

            if (delayMs < _shortestRoundtripDelayMs)
                _shortestRoundtripDelayMs = delayMs;

            int sendDelayMs = (int)msg.ResponseReplyTime.Subtract(msg.RequestSendTime).TotalMilliseconds;
            if (sendDelayMs > _longestSendDelayMs)
                _longestSendDelayMs = sendDelayMs;

            Console.WriteLine($"ResponseMessage6 received at {receiveTime}. ID: {msg.RequestId}, SendTime: {msg.RequestSendTime}, ReplyTime: {msg.ResponseReplyTime}, Message: {msg.Message}, Delay in ms: {delayMs}");

            if (!_responsesReceived.ContainsKey(6))
                _responsesReceived.Add(6, 1);
            else
                _responsesReceived[6]++;

            Interlocked.Increment(ref _totalResponsesReceived);
            await Task.CompletedTask;
        }

        private static async Task HandleResponseMessage7(ResponseMessage7 msg)
        {
            DateTime receiveTime = DateTime.Now;
            int delayMs = (int)receiveTime.Subtract(msg.RequestSendTime).TotalMilliseconds;
            if (delayMs > _longestRoundtripDelayMs)
                _longestRoundtripDelayMs = delayMs;

            if (delayMs < _shortestRoundtripDelayMs)
                _shortestRoundtripDelayMs = delayMs;

            int sendDelayMs = (int)msg.ResponseReplyTime.Subtract(msg.RequestSendTime).TotalMilliseconds;
            if (sendDelayMs > _longestSendDelayMs)
                _longestSendDelayMs = sendDelayMs;

            Console.WriteLine($"ResponseMessage7 received at {receiveTime}. ID: {msg.RequestId}, SendTime: {msg.RequestSendTime}, ReplyTime: {msg.ResponseReplyTime}, Message: {msg.Message}, Delay in ms: {delayMs}");

            if (!_responsesReceived.ContainsKey(7))
                _responsesReceived.Add(7, 1);
            else
                _responsesReceived[7]++;

            Interlocked.Increment(ref _totalResponsesReceived);
            await Task.CompletedTask;
        }

        private static async Task HandleResponseMessage8(ResponseMessage8 msg)
        {
            DateTime receiveTime = DateTime.Now;
            int delayMs = (int)receiveTime.Subtract(msg.RequestSendTime).TotalMilliseconds;
            if (delayMs > _longestRoundtripDelayMs)
                _longestRoundtripDelayMs = delayMs;

            if (delayMs < _shortestRoundtripDelayMs)
                _shortestRoundtripDelayMs = delayMs;

            int sendDelayMs = (int)msg.ResponseReplyTime.Subtract(msg.RequestSendTime).TotalMilliseconds;
            if (sendDelayMs > _longestSendDelayMs)
                _longestSendDelayMs = sendDelayMs;

            Console.WriteLine($"ResponseMessage8 received at {receiveTime}. ID: {msg.RequestId}, SendTime: {msg.RequestSendTime}, ReplyTime: {msg.ResponseReplyTime}, Message: {msg.Message}, Delay in ms: {delayMs}");

            if (!_responsesReceived.ContainsKey(8))
                _responsesReceived.Add(8, 1);
            else
                _responsesReceived[8]++;

            Interlocked.Increment(ref _totalResponsesReceived);
            await Task.CompletedTask;
        }

        private static async Task HandleResponseMessage9(ResponseMessage9 msg)
        {
            DateTime receiveTime = DateTime.Now;
            int delayMs = (int)receiveTime.Subtract(msg.RequestSendTime).TotalMilliseconds;
            if (delayMs > _longestRoundtripDelayMs)
                _longestRoundtripDelayMs = delayMs;

            if (delayMs < _shortestRoundtripDelayMs)
                _shortestRoundtripDelayMs = delayMs;

            int sendDelayMs = (int)msg.ResponseReplyTime.Subtract(msg.RequestSendTime).TotalMilliseconds;
            if (sendDelayMs > _longestSendDelayMs)
                _longestSendDelayMs = sendDelayMs;

            Console.WriteLine($"ResponseMessage9 received at {receiveTime}. ID: {msg.RequestId}, SendTime: {msg.RequestSendTime}, ReplyTime: {msg.ResponseReplyTime}, Message: {msg.Message}, Delay in ms: {delayMs}");

            if (!_responsesReceived.ContainsKey(9))
                _responsesReceived.Add(9, 1);
            else
                _responsesReceived[9]++;

            Interlocked.Increment(ref _totalResponsesReceived);
            await Task.CompletedTask;
        }

        private static async Task HandleResponseMessage10(ResponseMessage10 msg)
        {
            DateTime receiveTime = DateTime.Now;
            int delayMs = (int)receiveTime.Subtract(msg.RequestSendTime).TotalMilliseconds;
            if (delayMs > _longestRoundtripDelayMs)
                _longestRoundtripDelayMs = delayMs;

            if (delayMs < _shortestRoundtripDelayMs)
                _shortestRoundtripDelayMs = delayMs;

            int sendDelayMs = (int)msg.ResponseReplyTime.Subtract(msg.RequestSendTime).TotalMilliseconds;
            if (sendDelayMs > _longestSendDelayMs)
                _longestSendDelayMs = sendDelayMs;

            Console.WriteLine($"ResponseMessage10 received at {receiveTime}. ID: {msg.RequestId}, SendTime: {msg.RequestSendTime}, ReplyTime: {msg.ResponseReplyTime}, Message: {msg.Message}, Delay in ms: {delayMs}");

            if (!_responsesReceived.ContainsKey(10))
                _responsesReceived.Add(10, 1);
            else
                _responsesReceived[10]++;

            Interlocked.Increment(ref _totalResponsesReceived);
            await Task.CompletedTask;
        }


        public static bool IsRunningInContainer()
        {
            string dotNetRunningInContainerEnvVariable = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER");
            if (!string.IsNullOrEmpty(dotNetRunningInContainerEnvVariable))
            {
                if (bool.TryParse(dotNetRunningInContainerEnvVariable, out bool runningInDocker))
                    return runningInDocker;
            }

            return false;
        }
    }
}