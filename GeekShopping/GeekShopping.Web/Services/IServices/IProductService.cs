using GeekShopping.Web.Models;

namespace GeekShopping.Web.Services.IServices;

public interface IProductService
{
    Task<IEnumerable<ProductModel>> FindAll(string token);

    Task<ProductModel> FindById(long id, string token);

    Task<ProductModel> Create(ProductModel productModel, string token);

    Task<ProductModel> Update(ProductModel productModel, string token);

    Task<bool> DeleteById(long id, string token);
}