using GeekShopping.PaymentAPI.Messages;
using GeekShopping.PaymentAPI.RabbitMQSender;
using GeekShopping.PaymentProcessor;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeekShopping.PaymentAPI.MessageConsumer;

public class RabbitMQPaymentConsumer : BackgroundService
{
    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly IProcessPayment processPayment;
    private IRabbitMQMessageSender rabbitMQMessageSender;

    public RabbitMQPaymentConsumer(IProcessPayment processPayment, IRabbitMQMessageSender rabbitMQMessageSender)
    {
        this.processPayment = processPayment;

        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };
        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        channel.QueueDeclare(queue: "orderpaymentprocessqueue", false, false, false, arguments: null);
        this.rabbitMQMessageSender = rabbitMQMessageSender;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (chanel, evt) =>
        {
            var content = Encoding.UTF8.GetString(evt.Body.ToArray());
            PaymentMessage vo = JsonSerializer.Deserialize<PaymentMessage>(content);
            ProcessPayment(vo).GetAwaiter().GetResult();
            channel.BasicAck(evt.DeliveryTag, false);
        };
        channel.BasicConsume("orderpaymentprocessqueue", false, consumer);
        return Task.CompletedTask;
    }

    private async Task ProcessPayment(PaymentMessage vo)
    {
        var result = processPayment.PaymentProcessor();

        UpdatePaymentResultMessage paymentResult = new()
        {
            Status = result,
            OrderId = vo.OrderId,
            Email = vo.Email
        };

        try
        {
            rabbitMQMessageSender.SendMessage(paymentResult, "orderpaymentresultqueue");
        }
        catch (Exception)
        {
            //Log
            throw;
        }
    }
}