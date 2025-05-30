using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Group7_SE1733_A01_FE.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace Group7_SE1733_A01_FE.Pages.NewsArticle
{
    [Authorize(Roles = "1")]  // hoặc vai trò phù hợp
    public class DetailsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DetailsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public NewsArticleDTO NewsArticle { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = _httpClientFactory.CreateClient("MyApi");
            var response = await client.GetAsync($"api/NewsArticles/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, errorMessage);
                return Page();
            }

            var newsArticle = await response.Content.ReadFromJsonAsync<NewsArticleDTO>();

            if (newsArticle == null)
            {
                return NotFound();
            }

            NewsArticle = newsArticle;

            return Page();
        }
    }
}
