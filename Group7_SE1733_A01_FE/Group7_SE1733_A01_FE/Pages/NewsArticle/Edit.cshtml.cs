using System.Net.Http.Json;
using Group7_SE1733_A01_FE.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public NewsArticleUpdateDTO NewsArticle { get; set; } = new();

        public List<SelectListItem> CategoryOptions { get; set; } = new();

        public List<TagDTO> AllTags { get; set; } = new();

        public List<int> ExistingTagIds { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var client = _httpClientFactory.CreateClient("MyApi");

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

            // Get existing tag IDs
            ExistingTagIds = NewsArticle.Tags?.Select(t => t.TagId).ToList() ?? new List<int>();

            // Set UpdatedById from cookies
            if (Request.Cookies.TryGetValue("Id", out string? userIdStr) && int.TryParse(userIdStr, out int userId))
            {
                NewsArticle.UpdatedById = (short)userId;
            }

            // Set ModifiedDate to current time
            NewsArticle.ModifiedDate = DateTime.Now;

            await LoadCategoriesAsync();
            await LoadTagsAsync();

            return Page();
        }

        private async Task LoadCategoriesAsync()
        {
            var client = _httpClientFactory.CreateClient("MyApi");
            var response = await client.GetAsync("api/Category/get-all-active");
            if (response.IsSuccessStatusCode)
            {
                var categories = await response.Content.ReadFromJsonAsync<List<CategoryDTO>>();
                if (categories != null)
                {
                    CategoryOptions = categories.Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.CategoryName
                    }).ToList();
                }
            }
        }

        private async Task LoadTagsAsync()
        {
            var client = _httpClientFactory.CreateClient("MyApi");
            var response = await client.GetAsync("api/Tag/get-all");
            if (response.IsSuccessStatusCode)
            {
                var tags = await response.Content.ReadFromJsonAsync<List<TagDTO>>();
                if (tags != null)
                {
                    AllTags = tags;
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Get user ID from cookies FIRST
            if (!Request.Cookies.TryGetValue("Id", out string? userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                ModelState.AddModelError(string.Empty, "Cannot get UserId from cookies.");
                await LoadCategoriesAsync();
                await LoadTagsAsync();
                return Page();
            }

            // Set UpdatedById and ModifiedDate BEFORE validation
            NewsArticle.UpdatedById = (short)userId;
            NewsArticle.ModifiedDate = DateTime.Now;

            // Get selected tag IDs from form
            var selectedTagIds = new List<int>();
            foreach (var key in Request.Form.Keys.Where(k => k.StartsWith("TagIds")))
            {
                if (int.TryParse(Request.Form[key], out int tagId) && tagId > 0)
                {
                    selectedTagIds.Add(tagId);
                }
            }
            NewsArticle.TagIds = selectedTagIds;

            // Manual validation for UpdatedById since we removed [Required] attribute
            if (NewsArticle.UpdatedById == null || NewsArticle.UpdatedById <= 0)
            {
                ModelState.AddModelError(nameof(NewsArticle.UpdatedById), "UpdatedById is required.");
            }

            // Validate model
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                await LoadTagsAsync();
                return Page();
            }

            // Send to API
            var client = _httpClientFactory.CreateClient("MyApi");
            var response = await client.PutAsJsonAsync($"api/NewsArticles/{NewsArticle.NewsArticleId}", NewsArticle);

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error updating news article: {err}");
                await LoadCategoriesAsync();
                await LoadTagsAsync();
                return Page();
            }

            return RedirectToPage("./Details", new { id = NewsArticle.NewsArticleId });
        }
    }
}
