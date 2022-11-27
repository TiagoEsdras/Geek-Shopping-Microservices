using GeekShopping.Email.Messages;
using GeekShopping.Email.Model;
using GeekShopping.Email.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.Email.Repository;

public class EmailRepository : IEmailRepository
{
    private readonly DbContextOptions<MySQLContext> context;

    public EmailRepository(DbContextOptions<MySQLContext> context)
    {
        this.context = context;
    }

    public async Task LogEmail(UpdatePaymentResultMessage message)
    {
        EmailLog email = new()
        {
            Email = message.Email,
            SentDate = DateTime.Now,
            Log = $"Order - {message.OrderId} has been created successfully!"
        };
        await using var db = new MySQLContext(context);
        db.Emails.Add(email);
        await db.SaveChangesAsync();
    }
}