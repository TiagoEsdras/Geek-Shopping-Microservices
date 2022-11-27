using GeekShopping.MessageBus;
using GeekShopping.PaymentAPI.Messages;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace GeekShopping.PaymentAPI.RabbitMQSender;

public class RabbitMQMessageSender : IRabbitMQMessageSender
{
    private readonly string hostName;
    private readonly string password;
    private readonly string userName;
    private readonly string exchangeName;
    private IConnection connection;

    public RabbitMQMessageSender()
    {
        hostName = "localhost";
        password = "guest";
        userName = "guest";
        exchangeName = "FanoutPaymentUpdateExchange";
    }

    public void SendMessage(BaseMessage message)
    {
        if (ConnectionExists())
        {
            using var channel = connection.CreateModel();
            channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, durable: false);
            byte[] body = GetMessageAsByteArray(message);
            channel.BasicPublish(
                exchange: exchangeName, routingKey: "", basicProperties: null, body: body);
        }
    }

    private static byte[] GetMessageAsByteArray(BaseMessage message)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
        };
        var json = JsonSerializer.Serialize((UpdatePaymentResultMessage)message, options);
        var body = Encoding.UTF8.GetBytes(json);
        return body;
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

    private bool ConnectionExists()
    {
        if (connection != null) return true;
        CreateConnection();
        return connection != null;
    }
}