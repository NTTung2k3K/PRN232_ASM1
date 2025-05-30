using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Group7_SE1733_A01_FE.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace Group7_SE1733_A01_FE.Pages.NewsArticle
{
    [Authorize(Roles = "1")]
    public class HistoryModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HistoryModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IList<NewsArticleDTO> NewsArticles { get; set; } = new List<NewsArticleDTO>();

        public async Task OnGetAsync(string NewsTitle, string Headline, string NewsSource)
        {
            var client = _httpClientFactory.CreateClient("MyApi");

            var createdById = Request.Cookies["Id"];

            if (string.IsNullOrEmpty(createdById))
            {
                ModelState.AddModelError(string.Empty, "You must be logged in to view your history.");
                return;
            }

            var query = $"api/NewsArticles/view-history?createdById={createdById}&NewsTitle={NewsTitle}&Headline={Headline}&NewsSource={NewsSource}";

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
