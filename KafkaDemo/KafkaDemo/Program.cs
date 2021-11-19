using Confluent.Kafka;
using System;
using System.Threading.Tasks;

namespace KafkaDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Producing..");
            ProduceMessage().Wait();

            Console.WriteLine("Consuming..");
            ConsumeMessage();
        }

        public static async Task ProduceMessage()
        {
            var config = new ProducerConfig { BootstrapServers = "localhost:9092" }; //comma-separated list of host:port brokers
            using var p = new ProducerBuilder<Null, string>(config).Build();
            try
            {
                for (int i = 1; i < 5; i++)
                {
                    var deliveryResult = await p.ProduceAsync("test-topic", new Message<Null, string> { Value = $"test kafka #{i}" });
                    Console.WriteLine($"Delivered '{deliveryResult.Value}' to '{deliveryResult.TopicPartitionOffset}'");
                }
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"Delivery failed: {e.Error.Reason}");
            }
        }

        public static void ConsumeMessage()
        {
            var config = new ConsumerConfig
            {
                AllowAutoCreateTopics = true,
                BootstrapServers = "localhost:9092",
                GroupId = "test-group",
                // The AutoOffsetReset property determines the start offset in case no offsets are committed yet for the consumer group.
                // By default, offsets are committed automatically, so in this example, 
                // consumption will only start from the earliest message in the topic 'test-topic' the first time you run the program.
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };
            using var c = new ConsumerBuilder<Ignore, string>(config).Build();

            //Before we can consume messages, we need to subscribe to the topics we wish to receive messages from.
            c.Subscribe("test-topic");

            try
            {
                while (true)
                {
                    try
                    {
                        var cr = c.Consume();
                        Console.WriteLine($"Consumed message '{cr.Message.Value}' at: '{cr.TopicPartitionOffset}'.");
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Error occured: {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                //Final offsets are committed, and all resources used by this consumer are released.
                c.Close();
            }
        }
    }
}
