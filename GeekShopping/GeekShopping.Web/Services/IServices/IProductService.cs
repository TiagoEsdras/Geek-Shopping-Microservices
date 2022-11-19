using GeekShopping.Web.Models;

namespace GeekShopping.Web.Services.IServices;

public interface IProductService
{
    Task<IEnumerable<ProductViewModel>> FindAll(string token);

    Task<ProductViewModel> FindById(long id, string token);

    Task<ProductViewModel> Create(ProductViewModel productModel, string token);

    Task<ProductViewModel> Update(ProductViewModel productModel, string token);

    Task<bool> DeleteById(long id, string token);
}