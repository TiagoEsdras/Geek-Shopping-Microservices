using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using GeekShopping.Web.Utils;

namespace GeekShopping.Web.Services;

public class ProductService : IProductService
{
    private readonly HttpClient httpClient;
    public const string basePath = "api/v1/product";

    public ProductService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IEnumerable<ProductModel>> FindAll()
    {
        var response = await httpClient.GetAsync(basePath);
        return await response.ReadContentAs<IEnumerable<ProductModel>>();
    }

    public async Task<ProductModel> FindById(long id)
    {
        var response = await httpClient.GetAsync($"{basePath}/{id}");
        return await response.ReadContentAs<ProductModel>();
    }

    public async Task<ProductModel> Create(ProductModel productModel)
    {
        var response = await httpClient.PostAsJson(basePath, productModel);
        if (!response.IsSuccessStatusCode)
            throw new ApplicationException($"Something went wrong calling the API: {response.ReasonPhrase}");
        return await response.ReadContentAs<ProductModel>();
    }

    public async Task<ProductModel> Update(ProductModel productModel)
    {
        var response = await httpClient.PutAsJson(basePath, productModel);
        if (!response.IsSuccessStatusCode)
            throw new ApplicationException($"Something went wrong calling the API: {response.ReasonPhrase}");
        return await response.ReadContentAs<ProductModel>();
    }

    public async Task<bool> DeleteById(long id)
    {
        var response = await httpClient.DeleteAsync($"{basePath}/{id}");
        if (!response.IsSuccessStatusCode)
            throw new ApplicationException($"Something went wrong calling the API: {response.ReasonPhrase}");
        return await response.ReadContentAs<bool>();
    }
}