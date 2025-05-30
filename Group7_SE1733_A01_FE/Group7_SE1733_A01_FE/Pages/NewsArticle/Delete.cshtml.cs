using System.Threading.Tasks;
using Group7_SE1733_A01_FE.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace Group7_SE1733_A01_FE.Pages.NewsArticle
{
    [Authorize(Roles = "1")] // Hoặc role phù hợp
    public class DeleteModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DeleteModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
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

            var article = await response.Content.ReadFromJsonAsync<NewsArticleDTO>();

            if (article == null)
            {
                return NotFound();
            }

            NewsArticle = article;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = _httpClientFactory.CreateClient("MyApi");
            var response = await client.DeleteAsync($"api/NewsArticles/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, errorMessage);

                // Reload the article to show again
                var responseGet = await client.GetAsync($"api/NewsArticles/{id}");
                if (responseGet.IsSuccessStatusCode)
                {
                    var article = await responseGet.Content.ReadFromJsonAsync<NewsArticleDTO>();
                    if (article != null)
                    {
                        NewsArticle = article;
                    }
                }

                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
