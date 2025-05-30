using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Group7_SE1733_A01_FE.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Group7_SE1733_A01_FE.Pages.Category
{
    [Authorize(Roles = "1")]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IList<CategoryDTO> Categories { get; set; } = new List<CategoryDTO>();

        public async Task OnGetAsync(string CategoryName, string CategoryDesciption)
        {
            var client = _httpClientFactory.CreateClient("MyApi");

            var query = $"api/Category/search?CategoryName={CategoryName}&CategoryDesciption={CategoryDesciption}";
            var response = await client.GetAsync(query);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<List<CategoryDTO>>();
                if (result != null)
                {
                    Categories = result;
                }
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, error);
            }
        }
    }
}
