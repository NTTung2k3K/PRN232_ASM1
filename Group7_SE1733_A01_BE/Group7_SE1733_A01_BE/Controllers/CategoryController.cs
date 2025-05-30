using Microsoft.AspNetCore.Mvc;
using Repositories.Models;
using Services.DTOs;
using Services;

namespace Group7_SE1733_A01_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/Categorys/get-all
        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll()
        {
            try
            {
                return Ok(await _categoryService.GetAll());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET: api/Categorys/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetById(short id)
        {
            try
            {
                var article = await _categoryService.GetById(id);
                if (article == null) return NotFound();
                return Ok(article);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDTO dto)
        {
            try
            {
                var result = await _categoryService.Create(dto);
                if (result == 0) return BadRequest("Failed to create news category.");
                return Ok("News category created successfully.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(short id, [FromBody] CategoryUpdateDTO dto)
        {
            try
            {
                var existing = await _categoryService.GetById(id);
                if (existing == null) return NotFound("category not found.");

                var result = await _categoryService.Update(id, dto);
                if (result == 0) return BadRequest("Failed to update category.");
                return Ok("category updated successfully.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(short id)
        {
            try
            {
                var existing = await _categoryService.GetById(id);
                if (existing == null) return NotFound();

                var result = await _categoryService.Delete(id);
                if (!result) return BadRequest("Failed to delete article.");
                return Ok("Category deleted successfully.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<NewsArticle>>> Search(
           [FromQuery] string? CategoryName,
           [FromQuery] string? CategoryDesciption)
        {
            try
            {
                var result = await _categoryService.Search(CategoryName, CategoryDesciption);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
