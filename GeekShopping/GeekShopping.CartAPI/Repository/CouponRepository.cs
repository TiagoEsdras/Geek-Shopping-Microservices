using GeekShopping.CartAPI.Data.ValueObjects;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace GeekShopping.CartAPI.Repository;

public class CouponRepository : ICouponRepository
{
    private readonly HttpClient httpClient;

    public CouponRepository(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<CouponVO> GetCouponByCouponCode(string couponCode, string token)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await httpClient.GetAsync($"/api/v1/coupon/{couponCode}");
        var content = await response.Content.ReadAsStringAsync();
        if (response.StatusCode != HttpStatusCode.OK) return new CouponVO();
        return JsonSerializer.Deserialize<CouponVO>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}