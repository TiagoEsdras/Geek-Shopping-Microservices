using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using GeekShopping.Web.Utils;
using System.Net;
using System.Net.Http.Headers;

namespace GeekShopping.Web.Services;

public class CouponService : ICouponService
{
    private readonly HttpClient httpClient;
    public const string BasePath = "api/v1/coupon";

    public CouponService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<CouponViewModel> GetCoupon(string code, string token)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await httpClient.GetAsync($"{BasePath}/{code}");
        if (response.StatusCode != HttpStatusCode.OK) return new CouponViewModel();
        return await response.ReadContentAs<CouponViewModel>();
    }
}