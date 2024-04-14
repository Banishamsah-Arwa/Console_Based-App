using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers
{
    [ApiController] 
    [Route("/category")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategory _categoryService;

        public CategoryController(ICategory categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategories();
            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] Category category)
        {
            await _categoryService.AddCategory(category);
            return Ok();
        }

        [HttpPut("{name}")]
        public async Task<IActionResult> UpdateCategory(string name, [FromBody] Category category)
        {
            await _categoryService.UpdateCategory(name, category);
            return Ok("Updated successfully");
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteCategory(string name)
        {
            await _categoryService.DeleteCategory(name);
            return Ok("Deleted");
        }
    }
}
