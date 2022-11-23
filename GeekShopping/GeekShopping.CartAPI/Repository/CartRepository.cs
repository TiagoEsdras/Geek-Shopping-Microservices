using AutoMapper;
using GeekShopping.CartAPI.Data.ValueObjects;
using GeekShopping.CartAPI.Model;
using GeekShopping.CartAPI.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CartAPI.Repository;

public class CartRepository : ICartRepository
{
    private readonly MySQLContext context;
    private readonly IMapper mapper;

    public CartRepository(MySQLContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public async Task<bool> ClearCart(string userId)
    {
        var cartHeader = await context.CartHeaders.FirstOrDefaultAsync(c => c.UserId == userId);
        if (cartHeader is null) return false;
        context.CartDetails.RemoveRange(context.CartDetails.Where(c => c.CartHeaderId == cartHeader.Id));
        context.Remove(cartHeader);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<CartVO> FindCartByUserId(string userId)
    {
        var cartHeader = await context.CartHeaders.FirstOrDefaultAsync(c => c.UserId == userId) ?? new CartHeader();
        var cartDetails = context.CartDetails
            .Where(c => c.CartHeaderId == cartHeader.Id)
            .Include(c => c.Product);

        Cart cart = new()
        {
            CartHeader = cartHeader,
            CartDetails = cartDetails
        };

        return mapper.Map<CartVO>(cart);
    }

    public async Task<bool> RemoveFromCart(long cartDetaisId)
    {
        try
        {
            CartDetail cartDetail = await context.CartDetails.FirstOrDefaultAsync(c => c.Id == cartDetaisId);

            int total = context.CartDetails
                .Where(c => c.CartHeaderId == cartDetail.CartHeaderId)
                .Count();

            context.CartDetails.Remove(cartDetail);
            if (total == 1)
            {
                var cartHeaderToRemove = await context.CartHeaders.FirstOrDefaultAsync(c => c.Id == cartDetail.CartHeaderId);
                context.CartHeaders.Remove(cartHeaderToRemove);
            }
            await context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<CartVO> SaveOrUpdateCart(CartVO cartVO)
    {
        Cart cart = mapper.Map<Cart>(cartVO);
        var product = await context.Products.FirstOrDefaultAsync(p => p.Id == cartVO.CartDetails.FirstOrDefault().ProductId);
        if (product is null)
        {
            context.Products.Add(cart.CartDetails.FirstOrDefault().Product);
            await context.SaveChangesAsync();
        }

        var cartHeader = await context.CartHeaders.AsNoTracking().FirstOrDefaultAsync(c => c.UserId == cart.CartHeader.UserId);

        if (cartHeader is null)
        {
            context.Add(cart.CartHeader);
            await context.SaveChangesAsync();
            cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.Id;
            cart.CartDetails.FirstOrDefault().Product = null;
            context.CartDetails.Add(cart.CartDetails.FirstOrDefault());
            await context.SaveChangesAsync();
        }
        else
        {
            var cartDetail = await context.CartDetails.AsNoTracking().FirstOrDefaultAsync(cd =>
                cd.ProductId == cart.CartDetails.FirstOrDefault().ProductId &&
                cd.CartHeaderId == cartHeader.Id);

            if (cartDetail is null)
            {
                cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeader.Id;
                cart.CartDetails.FirstOrDefault().Product = null;
                context.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                await context.SaveChangesAsync();
            }
            else
            {
                cart.CartDetails.FirstOrDefault().Product = null;
                cart.CartDetails.FirstOrDefault().Count += cartDetail.Count;
                cart.CartDetails.FirstOrDefault().Id = cartDetail.Id;
                cart.CartDetails.FirstOrDefault().CartHeaderId = cartDetail.CartHeaderId;
                context.CartDetails.Update(cart.CartDetails.FirstOrDefault());
                await context.SaveChangesAsync();
            }
        }

        return mapper.Map<CartVO>(cart);
    }

    public async Task<bool> ApplyCoupon(string userId, string couponCode)
    {
        var header = await context.CartHeaders.FirstOrDefaultAsync(c => c.UserId == userId);
        if (header is null) return false;

        header.CouponCode = couponCode;
        context.CartHeaders.Update(header);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveCoupon(string userId)
    {
        var header = await context.CartHeaders.FirstOrDefaultAsync(c => c.UserId == userId);
        if (header is null) return false;

        header.CouponCode = "";
        context.CartHeaders.Update(header);
        await context.SaveChangesAsync();
        return true;
    }
}