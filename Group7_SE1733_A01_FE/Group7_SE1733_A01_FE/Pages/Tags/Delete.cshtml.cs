using System.Net.Http.Json;
using Group7_SE1733_A01_FE.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Group7_SE1733_A01_FE.Pages.Tag
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
        public TagDTO Tag { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = _httpClientFactory.CreateClient("MyApi");
            var response = await client.GetAsync($"api/Tag/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, error);
                return Page();
            }

            var tag = await response.Content.ReadFromJsonAsync<TagDTO>();
            if (tag == null)
            {
                return NotFound();
            }

            Tag = tag;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = _httpClientFactory.CreateClient("MyApi");
            var response = await client.DeleteAsync($"api/Tag/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, error);

                // Reload tag info to redisplay
                var getResponse = await client.GetAsync($"api/Tag/{id}");
                if (getResponse.IsSuccessStatusCode)
                {
                    var tag = await getResponse.Content.ReadFromJsonAsync<TagDTO>();
                    if (tag != null)
                    {
                        Tag = tag;
                    }
                }

                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
