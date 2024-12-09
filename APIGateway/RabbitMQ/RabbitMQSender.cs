using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace APIGateway.RabbitMQ
{

    public class RabbitMQSender
    {

        /// <summary>
        /// Puts an order on the queue.
        /// </summary>
        /// <param name="orderJSON">A string formatted into JSON of the order.</param>
        public static async void SendOrder(string orderJSON)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            await channel.QueueDeclareAsync(queue: "place_order", durable: false, exclusive: false, autoDelete: false);
            var body = Encoding.UTF8.GetBytes(orderJSON);
            await channel.BasicPublishAsync(exchange: "", routingKey: $"place_order", body: body);
        }

    }

}
