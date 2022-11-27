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
        if (ConnectionExists())
        {
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: queueName, false, false, false, arguments: null);
            byte[] body = GetMessageAsByteArray(baseMessage);
            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
        }
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

    private bool ConnectionExists()
    {
        if (connection is not null) return true;
        CreateConnection();
        return connection is not null;
    }

    private void CreateConnection()
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = hostName,
                UserName = userName,
                Password = password
            };
            connection = factory.CreateConnection();
        }
        catch (Exception)
        {
            //Log exception
            throw;
        }
    }
}