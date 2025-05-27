using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Group7_SE1733_A01_FE.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace Group7_SE1733_A01_FE.Pages.SystemAccount
{
    [Authorize(Roles = "0")]

    public class DeleteModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public DeleteModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public SystemAccountDTO SystemAccount { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = _httpClientFactory.CreateClient("MyApi");
            var response = await client.GetAsync($"api/SystemAccounts/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();

                ModelState.AddModelError(string.Empty, errorMessage);
                return Page();
            }
            var systemaccount = await response.Content.ReadFromJsonAsync<SystemAccountDTO>();

            if (systemaccount == null)
            {
                return NotFound();
            }
            else
            {
                SystemAccount = systemaccount;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var client = _httpClientFactory.CreateClient("MyApi");
            var response = await client.DeleteAsync($"api/SystemAccounts/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();

                ModelState.AddModelError(string.Empty, errorMessage);
                var responseGet = await client.GetAsync($"api/SystemAccounts/{id}");
                if (!responseGet.IsSuccessStatusCode)
                {
                    var errorMessageGet = await responseGet.Content.ReadAsStringAsync();

                    ModelState.AddModelError(string.Empty, errorMessageGet);
                    return Page();
                }
                var systemaccount = await responseGet.Content.ReadFromJsonAsync<SystemAccountDTO>();

                if (systemaccount == null)
                {
                    return NotFound();
                }
                else
                {
                    SystemAccount = systemaccount;
                }
                return Page();
            }


            return RedirectToPage("./Index");
        }
    }
}
