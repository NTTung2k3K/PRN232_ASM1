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

    }
}
