using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Group7_SE1733_A01_FE.DTOs;
using Group7_SE1733_A01_FE.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Group7_SE1733_A01_FE.Pages.NewsArticle
{
    [Authorize(Roles = "1")]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IList<NewsArticleDTO> NewsArticles { get; set; } = new List<NewsArticleDTO>();

        public async Task OnGetAsync(string NewsTitle, string Headline, string NewsSource)
        {
            var client = _httpClientFactory.CreateClient("MyApi");

            var query = $"api/NewsArticles/search?NewsTitle={NewsTitle}&Headline={Headline}&NewsSource={NewsSource}";

            var response = await client.GetAsync(query);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<List<NewsArticleDTO>>();
                if (result != null)
                {
                    NewsArticles = result;
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
