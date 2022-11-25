using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CouponAPI.Model.Context;

public class MySQLContext : DbContext
{
    public MySQLContext()
    {
    }

    public MySQLContext(DbContextOptions<MySQLContext> options) : base(options)
    {
    }

    public DbSet<Coupon> Coupons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Coupon>().HasData(new Coupon
        {
            Id = 1,
            CouponCode = "COUPON_TESTE_10",
            DiscountAmount = 10.00m
        });

        modelBuilder.Entity<Coupon>().HasData(new Coupon
        {
            Id = 2,
            CouponCode = "COUPON_TESTE_20",
            DiscountAmount = 20.00m
        });
    }
}