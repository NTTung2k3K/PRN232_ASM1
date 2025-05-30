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
        public NewsArticleCreateDTO NewsArticle { get; set; } = new NewsArticleCreateDTO();

        [BindProperty]
        public List<TagCreateDTO> TagInputs { get; set; } = new List<TagCreateDTO>();

        public List<SelectListItem> CategoryOptions { get; set; } = new List<SelectListItem>();

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadCategoriesAsync();
            return Page();
        }

        private async Task LoadCategoriesAsync()
        {
            var client = _httpClientFactory.CreateClient("MyApi");
            var response = await client.GetAsync("api/Category/get-all");
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
            // Lấy lại danh sách category để render lại nếu có lỗi
            await LoadCategoriesAsync();

            // Gán createdById từ cookie
            if (Request.Cookies.TryGetValue("Id", out string? userIdStr) && int.TryParse(userIdStr, out int userId))
            {
                NewsArticle.CreatedById = (short)userId;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Cannot get UserId from cookies.");
                return Page();
            }

            // Gán danh sách tags
            if (TagInputs != null && TagInputs.Count > 0)
            {
                NewsArticle.Tags = TagInputs;
            }

            var client = _httpClientFactory.CreateClient("MyApi");
            var response = await client.PostAsJsonAsync("api/NewsArticles/create", NewsArticle);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, error);
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
