using Microsoft.AspNetCore.Mvc;
using ExamApi.Models;

namespace ExamApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private static readonly List<Product> _products = new()
    {
        new Product { Id = 1, Name = "Laptop",     Price = 999.99m,  Stock = 10 },
        new Product { Id = 2, Name = "Mouse",      Price = 29.99m,   Stock = 50 },
        new Product { Id = 3, Name = "Keyboard",   Price = 49.99m,   Stock = 30 },
    };

    // GET api/products
    [HttpGet]
    public ActionResult<IEnumerable<Product>> GetAll() => Ok(_products);

    // GET api/products/{id}
    [HttpGet("{id}")]
    public ActionResult<Product> GetById(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        return product is null ? NotFound() : Ok(product);
    }

    // POST api/products
    [HttpPost]
    public ActionResult<Product> Create(Product product)
    {
        product.Id = _products.Count > 0 ? _products.Max(p => p.Id) + 1 : 1;
        _products.Add(product);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    // PUT api/products/{id}
    [HttpPut("{id}")]
    public IActionResult Update(int id, Product updated)
    {
        var existing = _products.FirstOrDefault(p => p.Id == id);
        if (existing is null) return NotFound();

        existing.Name  = updated.Name;
        existing.Price = updated.Price;
        existing.Stock = updated.Stock;
        return NoContent();
    }

    // DELETE api/products/{id}
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product is null) return NotFound();

        _products.Remove(product);
        return NoContent();
    }
}
