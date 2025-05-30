using System.Net.Http.Json;
using Group7_SE1733_A01_FE.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Group7_SE1733_A01_FE.Pages.Category
{
    [Authorize(Roles = "1")]
    public class DeleteModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DeleteModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public CategoryDTO Category { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = _httpClientFactory.CreateClient("MyApi");
            var response = await client.GetAsync($"api/Category/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, error);
                return Page();
            }

            var category = await response.Content.ReadFromJsonAsync<CategoryDTO>();
            if (category == null)
            {
                return NotFound();
            }

            Category = category;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = _httpClientFactory.CreateClient("MyApi");
            var response = await client.DeleteAsync($"api/Category/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, error);

                // Reload category info to redisplay
                var getResponse = await client.GetAsync($"api/Category/{id}");
                if (getResponse.IsSuccessStatusCode)
                {
                    var category = await getResponse.Content.ReadFromJsonAsync<CategoryDTO>();
                    if (category != null)
                    {
                        Category = category;
                    }
                }

                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
