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

namespace RebusPerformanceTest.Subscriber
{
    class Program
    {
        static string _inputQueueName;
        static Dictionary<string, string> _headers;
        static IBus _bus;

        static void Main(string[] args)
        {
            _headers = new Dictionary<string, string> {
                { RabbitMqHeaders.DeliveryMode, "1" },     // 1 = non-persistent, i.e. messages are not persisted by RabbitMQ
                { Headers.TimeToBeReceived, "00:01:00" },  // Set messages to expire after 1 minute
                { Headers.Express, "" }                    // Deliver messages as fast as possible
            };

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

            IConfigurationRoot configuration = builder
                .AddEnvironmentVariables()
                .Build();

            int instance = int.Parse(configuration["Instance"]);
            _inputQueueName = $"Subscriber{instance}";
            Console.WriteLine($"Subscriber {instance} started");

            Console.WriteLine("Waiting for RabbitMQ to start");
            Thread.Sleep(20000);

            string rabbitMqConnectionString = configuration.GetConnectionString("RabbitMq");
            Console.WriteLine($"Connecting to Rebus at {rabbitMqConnectionString}");

            BuiltinHandlerActivator activator = new BuiltinHandlerActivator();

            // Rebus defaults are 1 worker thread and a max parallelism of 5.
            int numberOfWorkers = 1;
            int maxParallelism = 5;
            bool publisherConfirms = false;

            switch (instance)
            {
                case 1:
                    {
                        activator.Handle<RequestMessage1>(async msg => await HandleRequestMessage1(msg));

                        _bus = Configure.With(activator)
                            .Options(o => {
                                o.SetNumberOfWorkers(numberOfWorkers);
                                o.SetMaxParallelism(maxParallelism);
                            })
                            .Logging(l => l.Console(LogLevel.Warn))
                            .Transport(t => t.UseRabbitMq(rabbitMqConnectionString, _inputQueueName)
                                            .SetPublisherConfirms(publisherConfirms))
                            .Routing(r => r.TypeBased()
                                .Map<ResponseMessage1>("Publisher1"))
                            .Start();

                        _bus.Subscribe<RequestMessage1>().Wait();
                    }
                    break;

                case 2:
                    {
                        activator.Handle<RequestMessage2>(async msg => await HandleRequestMessage2(msg));

                        _bus = Configure.With(activator)
                            .Options(o => {
                                o.SetNumberOfWorkers(numberOfWorkers);
                                o.SetMaxParallelism(maxParallelism);
                            })
                            .Logging(l => l.Console(LogLevel.Warn))
                            .Transport(t => t.UseRabbitMq(rabbitMqConnectionString, _inputQueueName)
                                            .SetPublisherConfirms(publisherConfirms))
                            .Routing(r => r.TypeBased()
                                .Map<ResponseMessage2>("Publisher2"))
                            .Start();

                        _bus.Subscribe<RequestMessage2>().Wait();
                    }
                    break;

                case 3:
                    {
                        activator.Handle<RequestMessage3>(async msg => await HandleRequestMessage3(msg));

                        _bus = Configure.With(activator)
                            .Options(o => {
                                o.SetNumberOfWorkers(numberOfWorkers);
                                o.SetMaxParallelism(maxParallelism);
                            })
                            .Logging(l => l.Console(LogLevel.Warn))
                            .Transport(t => t.UseRabbitMq(rabbitMqConnectionString, _inputQueueName)
                                            .SetPublisherConfirms(publisherConfirms))
                            .Routing(r => r.TypeBased()
                                .Map<ResponseMessage3>("Publisher3"))
                            .Start();

                        _bus.Subscribe<RequestMessage3>().Wait();
                    }
                    break;

                case 4:
                    {
                        activator.Handle<RequestMessage4>(async msg => await HandleRequestMessage4(msg));

                        _bus = Configure.With(activator)
                            .Options(o => {
                                o.SetNumberOfWorkers(numberOfWorkers);
                                o.SetMaxParallelism(maxParallelism);
                            })
                            .Logging(l => l.Console(LogLevel.Warn))
                            .Transport(t => t.UseRabbitMq(rabbitMqConnectionString, _inputQueueName)
                                            .SetPublisherConfirms(publisherConfirms))
                            .Routing(r => r.TypeBased()
                                .Map<ResponseMessage4>("Publisher4"))
                            .Start();

                        _bus.Subscribe<RequestMessage4>().Wait();
                    }
                    break;

                case 5:
                    {
                        activator.Handle<RequestMessage5>(async msg => await HandleRequestMessage5(msg));

                        _bus = Configure.With(activator)
                            .Options(o => {
                                o.SetNumberOfWorkers(numberOfWorkers);
                                o.SetMaxParallelism(maxParallelism);
                            })
                            .Logging(l => l.Console(LogLevel.Warn))
                            .Transport(t => t.UseRabbitMq(rabbitMqConnectionString, _inputQueueName)
                                            .SetPublisherConfirms(publisherConfirms))
                            .Routing(r => r.TypeBased()
                                .Map<ResponseMessage5>("Publisher5"))
                            .Start();

                        _bus.Subscribe<RequestMessage5>().Wait();
                    }
                    break;

                case 6:
                    {
                        activator.Handle<RequestMessage6>(async msg => await HandleRequestMessage6(msg));

                        _bus = Configure.With(activator)
                            .Options(o => {
                                o.SetNumberOfWorkers(numberOfWorkers);
                                o.SetMaxParallelism(maxParallelism);
                            })
                            .Logging(l => l.Console(LogLevel.Warn))
                            .Transport(t => t.UseRabbitMq(rabbitMqConnectionString, _inputQueueName)
                                            .SetPublisherConfirms(publisherConfirms))
                            .Routing(r => r.TypeBased()
                                .Map<ResponseMessage6>("Publisher6"))
                            .Start();

                        _bus.Subscribe<RequestMessage6>().Wait();
                    }
                    break;

                case 7:
                    {
                        activator.Handle<RequestMessage7>(async msg => await HandleRequestMessage7(msg));

                        _bus = Configure.With(activator)
                            .Options(o => {
                                o.SetNumberOfWorkers(numberOfWorkers);
                                o.SetMaxParallelism(maxParallelism);
                            })
                            .Logging(l => l.Console(LogLevel.Warn))
                            .Transport(t => t.UseRabbitMq(rabbitMqConnectionString, _inputQueueName)
                                            .SetPublisherConfirms(publisherConfirms))
                            .Routing(r => r.TypeBased()
                                .Map<ResponseMessage7>("Publisher7"))
                            .Start();

                        _bus.Subscribe<RequestMessage7>().Wait();
                    }
                    break;

                case 8:
                    {
                        activator.Handle<RequestMessage8>(async msg => await HandleRequestMessage8(msg));

                        _bus = Configure.With(activator)
                            .Options(o => {
                                o.SetNumberOfWorkers(numberOfWorkers);
                                o.SetMaxParallelism(maxParallelism);
                            })
                            .Logging(l => l.Console(LogLevel.Warn))
                            .Transport(t => t.UseRabbitMq(rabbitMqConnectionString, _inputQueueName)
                                            .SetPublisherConfirms(publisherConfirms))
                            .Routing(r => r.TypeBased()
                                .Map<ResponseMessage8>("Publisher8"))
                            .Start();

                        _bus.Subscribe<RequestMessage8>().Wait();
                    }
                    break;

                case 9:
                    {
                        activator.Handle<RequestMessage9>(async msg => await HandleRequestMessage9(msg));

                        _bus = Configure.With(activator)
                            .Options(o => {
                                o.SetNumberOfWorkers(numberOfWorkers);
                                o.SetMaxParallelism(maxParallelism);
                            })
                            .Logging(l => l.Console(LogLevel.Warn))
                            .Transport(t => t.UseRabbitMq(rabbitMqConnectionString, _inputQueueName)
                                            .SetPublisherConfirms(publisherConfirms))
                            .Routing(r => r.TypeBased()
                                .Map<ResponseMessage9>("Publisher9"))
                            .Start();

                        _bus.Subscribe<RequestMessage9>().Wait();
                    }
                    break;

                case 10:
                    {
                        activator.Handle<RequestMessage10>(async msg => await HandleRequestMessage10(msg));

                        _bus = Configure.With(activator)
                            .Options(o => {
                                o.SetNumberOfWorkers(numberOfWorkers);
                                o.SetMaxParallelism(maxParallelism);
                            })
                            .Logging(l => l.Console(LogLevel.Warn))
                            .Transport(t => t.UseRabbitMq(rabbitMqConnectionString, _inputQueueName)
                                            .SetPublisherConfirms(publisherConfirms))
                            .Routing(r => r.TypeBased()
                                .Map<ResponseMessage10>("Publisher10"))
                            .Start();

                        _bus.Subscribe<RequestMessage10>().Wait();
                    }
                    break;

                default:
                    Console.WriteLine("Instance must be in the range [1, 10]");
                    return;
            }

            Console.WriteLine("Connected to Rebus");

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

            Console.WriteLine("Test completed");
        }

        private static async Task HandleRequestMessage1(RequestMessage1 msg)
        {
            DateTime receiveTime = DateTime.Now;
//            Console.WriteLine($"RequestMessage1 received at {receiveTime}. ID: {msg.RequestId}, SendTime: {msg.SendTime}, Message: {msg.Message}, Delay in ms: {receiveTime.Subtract(msg.SendTime).TotalMilliseconds}");

            var responseMessage = new ResponseMessage1(msg.RequestId, msg.SendTime, receiveTime, msg.Message + " reply");
            await _bus.Reply(responseMessage, _headers);
        }

        private static async Task HandleRequestMessage2(RequestMessage2 msg)
        {
            DateTime receiveTime = DateTime.Now;
//            Console.WriteLine($"RequestMessage2 received at {receiveTime}. ID: {msg.RequestId}, SendTime: {msg.SendTime}, Message: {msg.Message}, Delay in ms: {receiveTime.Subtract(msg.SendTime).TotalMilliseconds}");

            var responseMessage = new ResponseMessage2(msg.RequestId, msg.SendTime, receiveTime, msg.Message + " reply");
            await _bus.Reply(responseMessage, _headers);
        }

        private static async Task HandleRequestMessage3(RequestMessage3 msg)
        {
            DateTime receiveTime = DateTime.Now;
//            Console.WriteLine($"RequestMessage3 received at {receiveTime}. ID: {msg.RequestId}, SendTime: {msg.SendTime}, Message: {msg.Message}, Delay in ms: {receiveTime.Subtract(msg.SendTime).TotalMilliseconds}");

            var responseMessage = new ResponseMessage3(msg.RequestId, msg.SendTime, receiveTime, msg.Message + " reply");
            await _bus.Reply(responseMessage, _headers);
        }

        private static async Task HandleRequestMessage4(RequestMessage4 msg)
        {
            DateTime receiveTime = DateTime.Now;
//            Console.WriteLine($"RequestMessage4 received at {receiveTime}. ID: {msg.RequestId}, SendTime: {msg.SendTime}, Message: {msg.Message}, Delay in ms: {receiveTime.Subtract(msg.SendTime).TotalMilliseconds}");

            var responseMessage = new ResponseMessage4(msg.RequestId, msg.SendTime, receiveTime, msg.Message + " reply");
            await _bus.Reply(responseMessage, _headers);
        }

        private static async Task HandleRequestMessage5(RequestMessage5 msg)
        {
            DateTime receiveTime = DateTime.Now;
//            Console.WriteLine($"RequestMessage5 received at {receiveTime}. ID: {msg.RequestId}, SendTime: {msg.SendTime}, Message: {msg.Message}, Delay in ms: {receiveTime.Subtract(msg.SendTime).TotalMilliseconds}");

            var responseMessage = new ResponseMessage5(msg.RequestId, msg.SendTime, receiveTime, msg.Message + " reply");
            await _bus.Reply(responseMessage, _headers);
        }

        private static async Task HandleRequestMessage6(RequestMessage6 msg)
        {
            DateTime receiveTime = DateTime.Now;
//            Console.WriteLine($"RequestMessage6 received at {receiveTime}. ID: {msg.RequestId}, SendTime: {msg.SendTime}, Message: {msg.Message}, Delay in ms: {receiveTime.Subtract(msg.SendTime).TotalMilliseconds}");

            var responseMessage = new ResponseMessage6(msg.RequestId, msg.SendTime, receiveTime, msg.Message + " reply");
            await _bus.Reply(responseMessage, _headers);
        }

        private static async Task HandleRequestMessage7(RequestMessage7 msg)
        {
            DateTime receiveTime = DateTime.Now;
//            Console.WriteLine($"RequestMessage7 received at {receiveTime}. ID: {msg.RequestId}, SendTime: {msg.SendTime}, Message: {msg.Message}, Delay in ms: {receiveTime.Subtract(msg.SendTime).TotalMilliseconds}");

            var responseMessage = new ResponseMessage7(msg.RequestId, msg.SendTime, receiveTime, msg.Message + " reply");
            await _bus.Reply(responseMessage, _headers);
        }

        private static async Task HandleRequestMessage8(RequestMessage8 msg)
        {
            DateTime receiveTime = DateTime.Now;
//            Console.WriteLine($"RequestMessage8 received at {receiveTime}. ID: {msg.RequestId}, SendTime: {msg.SendTime}, Message: {msg.Message}, Delay in ms: {receiveTime.Subtract(msg.SendTime).TotalMilliseconds}");

            var responseMessage = new ResponseMessage8(msg.RequestId, msg.SendTime, receiveTime, msg.Message + " reply");
            await _bus.Reply(responseMessage, _headers);
        }

        private static async Task HandleRequestMessage9(RequestMessage9 msg)
        {
            DateTime receiveTime = DateTime.Now;
//            Console.WriteLine($"RequestMessage9 received at {receiveTime}. ID: {msg.RequestId}, SendTime: {msg.SendTime}, Message: {msg.Message}, Delay in ms: {receiveTime.Subtract(msg.SendTime).TotalMilliseconds}");

            var responseMessage = new ResponseMessage9(msg.RequestId, msg.SendTime, receiveTime, msg.Message + " reply");
            await _bus.Reply(responseMessage, _headers);
        }

        private static async Task HandleRequestMessage10(RequestMessage10 msg)
        {
            DateTime receiveTime = DateTime.Now;
//            Console.WriteLine($"RequestMessage10 received at {receiveTime}. ID: {msg.RequestId}, SendTime: {msg.SendTime}, Message: {msg.Message}, Delay in ms: {receiveTime.Subtract(msg.SendTime).TotalMilliseconds}");

            var responseMessage = new ResponseMessage10(msg.RequestId, msg.SendTime, receiveTime, msg.Message + " reply");
            await _bus.Reply(responseMessage, _headers);
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