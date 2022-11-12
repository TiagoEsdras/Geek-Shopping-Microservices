using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using GeekShopping.Web.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService productService;

        public ProductController(IProductService productService)
        {
            this.productService = productService;
        }

        [Authorize]
        public async Task<IActionResult> ProductIndex()
        {
            var products = await productService.FindAll();
            return View(products);
        }

        public IActionResult ProductCreate()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ProductCreate(ProductModel productModel)
        {
            if (ModelState.IsValid)
            {
                var response = await productService.Create(productModel);
                if (response is not null) return RedirectToAction(nameof(ProductIndex));
            }
            return View(productModel);
        }

        public async Task<IActionResult> ProductUpdate(long id)
        {
            var product = await productService.FindById(id);
            if (product is not null) return View(product);
            return NotFound();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ProductUpdate(ProductModel productModel)
        {
            if (ModelState.IsValid)
            {
                var response = await productService.Update(productModel);
                if (response != null) return RedirectToAction(nameof(ProductIndex));
            }
            return View(productModel);
        }

        [Authorize]
        public async Task<IActionResult> ProductDelete(long id)
        {
            var product = await productService.FindById(id);
            if (product is not null) return View(product);
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> ProductDelete(ProductModel productModel)
        {
            var response = await productService.DeleteById(productModel.Id);
            if (response) return RedirectToAction(nameof(ProductIndex));
            return View(productModel);
        }
    }
}