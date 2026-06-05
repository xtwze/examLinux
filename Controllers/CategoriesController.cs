using Microsoft.AspNetCore.Mvc;
using ExamApi.Models;

namespace ExamApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private static readonly List<Category> _categories = new()
    {
        new Category { Id = 1, Title = "Electronics",  Description = "Electronic devices and accessories" },
        new Category { Id = 2, Title = "Peripherals",  Description = "Computer peripherals" },
        new Category { Id = 3, Title = "Accessories",  Description = "Various accessories" },
    };

    // GET api/categories
    [HttpGet]
    public ActionResult<IEnumerable<Category>> GetAll() => Ok(_categories);

    // GET api/categories/{id}
    [HttpGet("{id}")]
    public ActionResult<Category> GetById(int id)
    {
        var category = _categories.FirstOrDefault(c => c.Id == id);
        return category is null ? NotFound() : Ok(category);
    }

    // POST api/categories
    [HttpPost]
    public ActionResult<Category> Create(Category category)
    {
        category.Id = _categories.Count > 0 ? _categories.Max(c => c.Id) + 1 : 1;
        _categories.Add(category);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    // PUT api/categories/{id}
    [HttpPut("{id}")]
    public IActionResult Update(int id, Category updated)
    {
        var existing = _categories.FirstOrDefault(c => c.Id == id);
        if (existing is null) return NotFound();

        existing.Title       = updated.Title;
        existing.Description = updated.Description;
        return NoContent();
    }

    // DELETE api/categories/{id}
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var category = _categories.FirstOrDefault(c => c.Id == id);
        if (category is null) return NotFound();

        _categories.Remove(category);
        return NoContent();
    }
}
