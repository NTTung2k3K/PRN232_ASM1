using System.Net.Http.Json;
using Group7_SE1733_A01_FE.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Group7_SE1733_A01_FE.Pages.Tag
{
    [Authorize(Roles = "1")]
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EditModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public TagUpdateDTO Tag { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient("MyApi");

            var response = await client.GetAsync($"api/Tag/{Id}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, error);
                return Page();
            }

            var tagFromApi = await response.Content.ReadFromJsonAsync<TagUpdateDTO>();
            if (tagFromApi == null)
            {
                return NotFound();
            }

            Tag = tagFromApi;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var client = _httpClientFactory.CreateClient("MyApi");

            var response = await client.PutAsJsonAsync($"api/Tag/{Id}", Tag);
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
