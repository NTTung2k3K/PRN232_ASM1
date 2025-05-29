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

    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;


        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IList<SystemAccountDTO> SystemAccount { get;set; } = default!;

        public async Task OnGetAsync(string AccountName, string AccountEmail, bool IsSortDescByAccountName)
        {
            var client = _httpClientFactory.CreateClient("MyApi");
            var response = await client.GetAsync($"api/SystemAccounts/search?AccountName={AccountName}&AccountEmail={AccountEmail}&IsSortDescByAccountName={IsSortDescByAccountName}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<List<SystemAccountDTO>>();
                if (result != null)
                {
                    SystemAccount = result;
                }
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();

                ModelState.AddModelError(string.Empty, errorMessage);
            }
        }
    }
}
