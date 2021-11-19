using Confluent.Kafka;
using MQ.Models;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace MQ.MQServices
{
    public class MQService
    {
        public async Task ProduceEmailMessage(QueueEmailMessage message)
        {
            var config = new ProducerConfig { BootstrapServers = "localhost:9092" };
            using var p = new ProducerBuilder<Null, string>(config).Build();
            try
            {
                var jsonString = JsonSerializer.Serialize(message);
                var deliveryResult = await p.ProduceAsync("send-email-test-topic", new Message<Null, string> { Value = jsonString });
                Console.WriteLine($"Delivered '{deliveryResult.Value}' to '{deliveryResult.TopicPartitionOffset}'");
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"Delivery failed: {e.Error.Reason}");
            }
        }
        public async Task ConsumeEmailMessage(Func<QueueEmailMessage, Task> onRecieve)
        {
            var config = new ConsumerConfig
            {
                AllowAutoCreateTopics = true,
                BootstrapServers = "localhost:9092",
                GroupId = "test-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };
            using var c = new ConsumerBuilder<Ignore, string>(config).Build();
            c.Subscribe("send-email-test-topic");

            try
            {
                while (true)
                {
                    try
                    {
                        var cr = c.Consume();
                        var jsonString = cr.Message.Value;

                        var value = JsonSerializer.Deserialize<QueueEmailMessage>(jsonString);

                        try
                        {
                            await onRecieve(value);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error occured processing kafka message : {exception}", e);
                        }

                        c.Commit();
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
                c.Close();
            }
        }
    }
}
