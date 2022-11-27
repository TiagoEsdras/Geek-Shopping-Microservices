using GeekShopping.Email.Messages;
using GeekShopping.Email.Repository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeekShopping.Email.MessageConsumer;

public class RabbitMQPaymentConsumer : BackgroundService
{
    private readonly EmailRepository emailRepository;
    private readonly IConnection connection;
    private readonly IModel channel;
    private const string ExchangeName = "FanoutPaymentUpdateExchange";
    private readonly string queueName = "";

    public RabbitMQPaymentConsumer(EmailRepository emailRepository)
    {
        this.emailRepository = emailRepository;
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };
        connection = factory.CreateConnection();
        channel = connection.CreateModel();

        channel.ExchangeDeclare(ExchangeName, ExchangeType.Fanout);
        queueName = channel.QueueDeclare().QueueName;
        channel.QueueBind(queueName, ExchangeName, "");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (chanel, evt) =>
        {
            var content = Encoding.UTF8.GetString(evt.Body.ToArray());
            UpdatePaymentResultMessage message = JsonSerializer.Deserialize<UpdatePaymentResultMessage>(content);
            ProcessLogs(message).GetAwaiter().GetResult();
            channel.BasicAck(evt.DeliveryTag, false);
        };
        channel.BasicConsume(queueName, false, consumer);
        return Task.CompletedTask;
    }

    private async Task ProcessLogs(UpdatePaymentResultMessage message)
    {
        try
        {
            await emailRepository.LogEmail(message);
        }
        catch (Exception)
        {
            //Log
            throw;
        }
    }
}