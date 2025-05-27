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

    public class DetailsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public DetailsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

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
    }
}
