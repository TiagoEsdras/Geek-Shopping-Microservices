using GeekShopping.CartAPI.Messages;
using GeekShopping.MessageBus;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace GeekShopping.CartAPI.RabbitMQSender;

public class RabbitMQMessageSender : IRabbitMQMessageSender
{
    private readonly string hostName;
    private readonly string password;
    private readonly string userName;
    private IConnection connection;

    public RabbitMQMessageSender()
    {
        hostName = "localhost";
        password = "guest";
        userName = "guest";
    }

    public void SendMessage(BaseMessage baseMessage, string queueName)
    {
        var factory = new ConnectionFactory
        {
            HostName = hostName,
            UserName = userName,
            Password = password,
        };
        connection = factory.CreateConnection();

        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: queueName, false, false, false, arguments: null);
        byte[] body = GetMessageAsByteArray(baseMessage);
        channel.BasicPublish(exchange: "", routingKey: queueName, mandatory: false, basicProperties: null, body: body);
        throw new NotImplementedException();
    }

    private static byte[] GetMessageAsByteArray(BaseMessage baseMessage)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
        };

        var json = JsonSerializer.Serialize<CheckoutHeaderVO>((CheckoutHeaderVO)baseMessage, options);

        return Encoding.UTF8.GetBytes(json);
    }
}