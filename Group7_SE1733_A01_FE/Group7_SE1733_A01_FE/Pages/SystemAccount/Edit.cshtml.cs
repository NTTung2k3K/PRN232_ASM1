using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Group7_SE1733_A01_FE.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Group7_SE1733_A01_FE.Pages.SystemAccount
{
    [Authorize(Roles = "0")]

    public class EditModel : PageModel
    {

        private readonly IHttpClientFactory _httpClientFactory;
        public EditModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [BindProperty]
        public SystemAccountUpdateDTO SystemAccount { get; set; } = default!;

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
            var systemaccount = await response.Content.ReadFromJsonAsync<SystemAccountUpdateDTO>();

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

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var client = _httpClientFactory.CreateClient("MyApi");
            var response = await client.PutAsJsonAsync($"api/SystemAccounts/{SystemAccount.AccountId}", SystemAccount);
            if(!response.IsSuccessStatusCode) {
                var errorMessage = await response.Content.ReadAsStringAsync();

                ModelState.AddModelError(string.Empty, errorMessage);
                return Page();
            }


            return RedirectToPage("./Index");
        }

     
    }
}
