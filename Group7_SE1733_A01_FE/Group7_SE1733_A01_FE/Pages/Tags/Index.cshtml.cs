using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Group7_SE1733_A01_FE.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Group7_SE1733_A01_FE.Pages.Tag
{
    [Authorize(Roles = "1")]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IList<TagDTO> Tags { get; set; } = new List<TagDTO>();

        public async Task OnGetAsync(string TagName, string Note)
        {
            var client = _httpClientFactory.CreateClient("MyApi");

            // Tạo query string theo filter (nếu backend hỗ trợ)
            var query = $"api/Tag/search?TagName={TagName}&Note={Note}";
            var response = await client.GetAsync(query);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<List<TagDTO>>();
                if (result != null)
                {
                    Tags = result;
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
