using Group7_SE1733_A01_FE.DTOs;
using Group7_SE1733_A01_FE.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Group7_SE1733_A01_FE.Pages.NewsArticle
{
    [Authorize(Roles = "0")]
    public class ReportModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ReportModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public List<NewsArticleDTO> ReportData { get; set; }
        public async Task<IActionResult> OnGet(DateTime? StartDate, DateTime? EndDate)
        {

            if (StartDate == null && EndDate == null)
            {
                ModelState.AddModelError(string.Empty, "Start Date or End Date is required to generate report");

                return Page();
            }
            if (StartDate != null && EndDate != null && StartDate > EndDate)
            {
                ModelState.AddModelError(string.Empty, "Start date must be before end date");
                return Page();
            }
            var client = _httpClientFactory.CreateClient("MyApi");
            var url = "api/NewsArticles/report?";
            if (StartDate != null)
            {
                url += $"startDate={StartDate.Value}&";
            }
            if (EndDate != null)
            {
                url += $"endDate={EndDate.Value}&";
            }
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();

                ModelState.AddModelError(string.Empty, errorMessage);
                return Page();
            }
            var newsArticleDTO = await response.Content.ReadFromJsonAsync<List<NewsArticleDTO>>();

                ReportData = newsArticleDTO;
            return Page();


        }
    }
}
