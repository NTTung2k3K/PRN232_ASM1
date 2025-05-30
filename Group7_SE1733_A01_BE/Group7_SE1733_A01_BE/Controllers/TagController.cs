using Microsoft.AspNetCore.Mvc;
using Repositories.Models;
using Services.DTOs;
using Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Group7_SE1733_A01_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        // GET: api/Tag/get-all
        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<Tag>>> GetAll()
        {
            try
            {
                return Ok(await _tagService.GetAll());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET: api/Tag/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tag>> GetById(int id)
        {
            try
            {
                var tag = await _tagService.GetById(id);
                if (tag == null) return NotFound();
                return Ok(tag);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: api/Tag/create
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] TagCreateDTO dto)
        {
            try
            {
                var result = await _tagService.Create(dto);
                if (result == 0) return BadRequest("Failed to create tag.");
                return Ok("Tag created successfully.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // PUT: api/Tag/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TagUpdateDTO dto)
        {
            try
            {
                var existing = await _tagService.GetById(id);
                if (existing == null) return NotFound("Tag not found.");

                var result = await _tagService.Update(id, dto);
                if (result == 0) return BadRequest("Failed to update tag.");
                return Ok("Tag updated successfully.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE: api/Tag/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var existing = await _tagService.GetById(id);
                if (existing == null) return NotFound();

                var result = await _tagService.Delete(id);
                if (!result) return BadRequest("Failed to delete tag.");
                return Ok("Tag deleted successfully.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET: api/Tag/search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Tag>>> Search(
            [FromQuery] string? tagName,
            [FromQuery] string? note)
        {
            try
            {
                var result = await _tagService.Search(tagName, note);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
