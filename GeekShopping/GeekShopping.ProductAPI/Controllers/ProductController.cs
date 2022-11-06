﻿using GeekShopping.ProductAPI.Data.ValueObjects;
using GeekShopping.ProductAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.ProductAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductRepository productRepository;

    public ProductController(IProductRepository productRepository)
    {
        this.productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductVO>>> GetAll()
    {
        var products = await productRepository.FindAll();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductVO>> GetById(long id)
    {
        var product = await productRepository.FindById(id);
        if (product is null) return NotFound();
        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductVO>> Create([FromBody] ProductVO productVO)
    {
        if (productVO is null) return BadRequest();
        var product = await productRepository.Create(productVO);
        return Ok(product);
    }

    [HttpPut]
    public async Task<ActionResult<ProductVO>> Update([FromBody] ProductVO productVO)
    {
        if (productVO is null) return BadRequest();
        var product = await productRepository.Update(productVO);
        return Ok(product);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ProductVO>> Delete(long id)
    {
        var success = await productRepository.Delete(id);
        if (success) return NoContent();
        return BadRequest();
    }
}