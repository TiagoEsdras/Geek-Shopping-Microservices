using AutoMapper;
using GeekShopping.ProductAPI.Data.ValueObjects;
using GeekShopping.ProductAPI.Model;
using GeekShopping.ProductAPI.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.ProductAPI.Repository;

public class ProductRepository : IProductRepository
{
    private readonly MySQLContext context;
    private readonly IMapper mapper;

    public ProductRepository(MySQLContext context, IMapper mapper)
    {
        this.mapper = mapper;
        this.context = context;
    }

    public async Task<IEnumerable<ProductVO>> FindAll()
    {
        var products = await context.Products.ToListAsync();
        return mapper.Map<IEnumerable<ProductVO>>(products);
    }

    public async Task<ProductVO> FindById(long id)
    {
        var product = await context.Products.Where(p => p.Id == id).FirstOrDefaultAsync();
        return mapper.Map<ProductVO>(product);
    }

    public async Task<ProductVO> Create(ProductVO product)
    {
        var productEntity = mapper.Map<Product>(product);
        context.Add(productEntity);
        await context.SaveChangesAsync();
        return mapper.Map<ProductVO>(productEntity);
    }

    public async Task<ProductVO> Update(ProductVO product)
    {
        var productEntity = mapper.Map<Product>(product);
        context.Update(productEntity);
        await context.SaveChangesAsync();
        return mapper.Map<ProductVO>(productEntity);
    }

    public async Task<bool> Delete(long id)
    {
        try
        {
            var product = await context.Products.Where(p => p.Id == id).FirstOrDefaultAsync();
            if (product is null) return false;
            context.Products.Remove(product);
            await context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}