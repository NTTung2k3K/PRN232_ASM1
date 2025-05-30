using System.Net.Http.Json;
using Group7_SE1733_A01_FE.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Group7_SE1733_A01_FE.Pages.NewsArticle
{
    [Authorize(Roles = "1")]
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EditModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public NewsArticleUpdateDTO NewsArticle { get; set; } = default!;

        public List<CategoryDTO> Categories { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var client = _httpClientFactory.CreateClient("MyApi");

            // Get the NewsArticle
            var articleResponse = await client.GetAsync($"api/NewsArticles/{id}");
            if (!articleResponse.IsSuccessStatusCode)
            {
                var err = await articleResponse.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, err);
                return Page();
            }

            var article = await articleResponse.Content.ReadFromJsonAsync<NewsArticleUpdateDTO>();
            if (article == null) return NotFound();
            NewsArticle = article;

            // Get categories for dropdown
            var catResponse = await client.GetAsync("api/Category/get-all");
            if (catResponse.IsSuccessStatusCode)
            {
                var categoryList = await catResponse.Content.ReadFromJsonAsync<List<CategoryDTO>>();
                if (categoryList != null)
                    Categories = categoryList;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            // Gán createdById từ cookie
            if (Request.Cookies.TryGetValue("Id", out string? userIdStr) && int.TryParse(userIdStr, out int userId))
            {
                NewsArticle.UpdatedById = (short)userId;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Cannot get UserId from cookies.");
                return Page();
            }

            var client = _httpClientFactory.CreateClient("MyApi");
            var response = await client.PutAsJsonAsync($"api/NewsArticles/{NewsArticle.NewsArticleId}", NewsArticle);

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, err);
                return Page();
            }

            return RedirectToPage("./Details", new { id = NewsArticle.NewsArticleId });
        }
    }
}
