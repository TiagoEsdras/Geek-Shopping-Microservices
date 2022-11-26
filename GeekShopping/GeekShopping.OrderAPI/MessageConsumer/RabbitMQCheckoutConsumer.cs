using GeekShopping.OrderAPI.Messages;
using GeekShopping.OrderAPI.Model;
using GeekShopping.OrderAPI.Repository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeekShopping.OrderAPI.MessageConsumer;

public class RabbitMQCheckoutConsumer : BackgroundService
{
    private readonly OrderRepository orderRepository;
    private readonly IConnection connection;
    private readonly IModel channel;

    public RabbitMQCheckoutConsumer(OrderRepository orderRepository)
    {
        this.orderRepository = orderRepository;
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };
        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        channel.QueueDeclare(queue: "checkoutqueue", false, false, false, arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (chanel, evt) =>
        {
            var content = Encoding.UTF8.GetString(evt.Body.ToArray());
            CheckoutHeaderVO vo = JsonSerializer.Deserialize<CheckoutHeaderVO>(content);
            ProcessOrder(vo).GetAwaiter().GetResult();
            channel.BasicAck(evt.DeliveryTag, false);
        };
        channel.BasicConsume("checkoutqueue", false, consumer);
        return Task.CompletedTask;
    }

    private async Task ProcessOrder(CheckoutHeaderVO vo)
    {
        OrderHeader order = new()
        {
            UserId = vo.UserId,
            FirstName = vo.FirstName,
            LastName = vo.LastName,
            OrderDetails = new List<OrderDetail>(),
            CardNumber = vo.CardNumber,
            CouponCode = vo.CouponCode,
            CVV = vo.CVV,
            DiscountAmount = vo.DiscountAmount,
            Email = vo.Email,
            ExpiryMonthYear = vo.ExpiryMonthYear,
            OrderTime = DateTime.Now,
            PurchaseAmount = vo.PurchaseAmount,
            PaymentStatus = false,
            Phone = vo.Phone,
            DateTime = vo.DateTime
        };

        foreach (var details in vo.CartDetails)
        {
            OrderDetail detail = new()
            {
                ProductId = details.ProductId,
                ProductName = details.Product.Name,
                Price = details.Product.Price,
                Count = details.Count,
            };
            order.CartTotalItens += details.Count;
            order.OrderDetails.Add(detail);
        }

        await orderRepository.AddOrder(order);
    }
}