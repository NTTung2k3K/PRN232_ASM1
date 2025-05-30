using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTOs;
using Repositories.Models;
using Group7_SE1733_A01_FE.DTOs;

namespace Group7_SE1733_A01_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsArticlesController : ControllerBase
    {
        private readonly INewsArticleService _newsArticleService;

        public NewsArticlesController(INewsArticleService newsArticleService)
        {
            _newsArticleService = newsArticleService;
        }

        // GET: api/NewsArticles/get-all
        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<NewsArticle>>> GetAll()
        {
            try
            {
                return Ok(await _newsArticleService.GetAll());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET: api/NewsArticles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NewsArticle>> GetById(string id)
        {
            try
            {
                var article = await _newsArticleService.GetById(id);
                if (article == null) return NotFound();
                return Ok(article);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: api/NewsArticles/create
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] NewsArticleCreateDTO dto)
        {
            try
            {
                var result = await _newsArticleService.Create(dto);
                if (result == 0) return BadRequest("Failed to create news article.");
                return Ok("News article created successfully.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // PUT: api/NewsArticles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] NewsArticleUpdateDTO dto)
        {
            try
            {
                var existing = await _newsArticleService.GetById(id);
                if (existing == null) return NotFound("Article not found.");

                var result = await _newsArticleService.Update(id, dto);
                if (result == 0) return BadRequest("Failed to update article.");
                return Ok("Article updated successfully.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE: api/NewsArticles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var existing = await _newsArticleService.GetById(id);
                if (existing == null) return NotFound();

                var result = await _newsArticleService.Delete(id);
                if (!result) return BadRequest("Failed to delete article.");
                return Ok("Article deleted successfully.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET: api/NewsArticles/search?newsTitle=abc&headline=xyz&newsSource=bbc
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<NewsArticle>>> Search(
            [FromQuery] string? newsTitle,
            [FromQuery] string? headline,
            [FromQuery] string? newsSource)
        {
            try
            {
                var result = await _newsArticleService.Search(newsTitle, headline, newsSource);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET: api/NewsArticles/view-history?createdById=1&NewsTitle=abc&Headline=xyz&NewsSource=bbc
        [HttpGet("view-history")]
        public async Task<ActionResult<IEnumerable<NewsArticle>>> GetHistory(
            [FromQuery] short createdById,
            [FromQuery] string? NewsTitle,
            [FromQuery] string? Headline,
            [FromQuery] string? NewsSource)
        {
            try
            {
                var result = await _newsArticleService.GetNewsHistoryByUser(
                    createdById,
                    NewsTitle ?? string.Empty,
                    Headline ?? string.Empty,
                    NewsSource ?? string.Empty
                );
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }



        [HttpGet("report")]
        public async Task<ActionResult<IEnumerable<NewsArticle>>> GetReportAsync([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                if (startDate == null && endDate == null)
                {
                    return BadRequest("You must provide at least one of startDate or endDate.");
                }
                var result = await _newsArticleService.GetReportAsync(startDate, endDate);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
