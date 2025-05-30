using System.Net.Http.Json;
using Group7_SE1733_A01_FE.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Group7_SE1733_A01_FE.Pages.Category
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
        public CategoryCreateDTO Category { get; set; } = new CategoryCreateDTO();

        public List<SelectListItem> CategoryOptions { get; set; } = new List<SelectListItem>();

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadCategoriesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadCategoriesAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var client = _httpClientFactory.CreateClient("MyApi");
            var response = await client.PostAsJsonAsync("api/Category/create", Category);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, error);
                return Page();
            }

            return RedirectToPage("./Index");
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
    }
}
