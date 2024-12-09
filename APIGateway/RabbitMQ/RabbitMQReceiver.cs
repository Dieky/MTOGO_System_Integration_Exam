using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace APIGateway.RabbitMQ
{
    public class RabbitMQReceiver
    {

        /// <summary>
        /// Accepts an order based on the given order ID. 
        /// Processes the messages in the queue until the desired order is found, acknowledges it, and cancels the consumer.
        /// </summary>
        /// <param name="orderId">The ID of the order to be accepted and processed.</param>
        public static async void AcceptOrder(string orderId)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();
            await channel.QueueDeclareAsync(queue: "place_order", durable: false, exclusive: false, autoDelete: false);
            var consumer = new AsyncEventingBasicConsumer(channel);
            string consumerTag = await channel.BasicConsumeAsync("place_order", autoAck: false, consumer: consumer);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var response = Encoding.UTF8.GetString(ea.Body.ToArray());
                JsonDocument jsonDocument = JsonDocument.Parse(response);

                if (jsonDocument.RootElement.GetProperty("id").ToString() == orderId)
                {
                    Console.WriteLine($"Order : {orderId} has been accepted");
                    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                    await channel.BasicCancelAsync(consumerTag);
                }
                else
                {
                    Console.WriteLine($"Message with DeliveryTag {ea.DeliveryTag} does not match, requeuing...");
                    await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };
        }

    }
}
