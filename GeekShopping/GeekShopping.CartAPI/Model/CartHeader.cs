using GeekShopping.CartAPI.Model.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeekShopping.CartAPI.Model;

[Table("cart_header")]
public class CartHeader : BaseEntity
{
    [Column("user_id")]
    public string UserId { get; set; }
    [Column("coupn_code")]
    public string CouponCode { get; set; }
}