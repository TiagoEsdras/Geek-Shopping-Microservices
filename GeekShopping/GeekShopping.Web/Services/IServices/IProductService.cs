using GeekShopping.Web.Models;

namespace GeekShopping.Web.Services.IServices;

public interface IProductService
{
    Task<IEnumerable<ProductModel>> FindAll();

    Task<ProductModel> FindById(long id);

    Task<ProductModel> Create(ProductModel productModel);

    Task<ProductModel> Update(ProductModel productModel);

    Task<bool> DeleteById(long id);
}