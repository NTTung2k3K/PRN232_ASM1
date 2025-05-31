using System.Net.Http.Json;
using Group7_SE1733_A01_FE.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Group7_SE1733_A01_FE.Pages.NewsArticle
{
    [Authorize(Roles = "1")]
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CreateModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public NewsArticleCreateDTO NewsArticle { get; set; } = new();

        public List<SelectListItem> CategoryOptions { get; set; } = new();

        public List<TagDTO> AllTags { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadCategoriesAsync();
            await LoadTagsAsync();

            // Set default values
            NewsArticle.CreatedDate = DateTime.Now;
            NewsArticle.NewsStatus = true;

            // Debug: Check if tags are loaded
            Console.WriteLine($"Loaded {AllTags.Count} tags");
            foreach (var tag in AllTags)
            {
                Console.WriteLine($"Tag: {tag.TagId} - {tag.TagName}");
            }

            return Page();
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

        public async Task<IActionResult> OnPostAsync()
        {
            // Reload data for dropdowns in case of validation errors
            await LoadCategoriesAsync();
            await LoadTagsAsync();

            // Get user ID from cookies
            if (!Request.Cookies.TryGetValue("Id", out string? userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                ModelState.AddModelError(string.Empty, "Cannot get UserId from cookies.");
                return Page();
            }

            NewsArticle.CreatedById = (short)userId;

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

            // Validate model
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Send to API
            var client = _httpClientFactory.CreateClient("MyApi");
            var response = await client.PostAsJsonAsync("api/NewsArticles/create", NewsArticle);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error creating news article: {error}");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
