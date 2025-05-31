using System.Net.Http.Json;
using Group7_SE1733_A01_FE.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Group7_SE1733_A01_FE.Pages.Category
{
    [Authorize(Roles = "1")]
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EditModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public CategoryUpdateDTO Category { get; set; } = new CategoryUpdateDTO();

        // Thêm property để lưu danh sách categories cho dropdown
        public List<SelectListItem> AvailableCategories { get; set; } = new List<SelectListItem>();

        // Thêm property để lưu thông tin category hiện tại (bao gồm ParentCategory)
        public CategoryDisplayDTO CurrentCategory { get; set; } = new CategoryDisplayDTO();

        public async Task<IActionResult> OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient("MyApi");

            // Lấy thông tin category hiện tại (bao gồm ParentCategory để hiển thị)
            var categoryResponse = await client.GetAsync($"api/Category/{Id}");
            if (!categoryResponse.IsSuccessStatusCode)
            {
                var error = await categoryResponse.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, error);
                return Page();
            }

            CurrentCategory = await categoryResponse.Content.ReadFromJsonAsync<CategoryDisplayDTO>();
            if (CurrentCategory == null)
            {
                return NotFound();
            }

            // Map sang CategoryUpdateDTO để bind với form
            Category = new CategoryUpdateDTO
            {
                CategoryName = CurrentCategory.CategoryName,
                CategoryDesciption = CurrentCategory.CategoryDesciption,
                ParentCategoryId = CurrentCategory.ParentCategoryId,
                IsActive = CurrentCategory.IsActive
            };

            // Lấy danh sách tất cả categories active để làm dropdown
            await LoadAvailableCategories();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Reload available categories nếu có lỗi validation
                await LoadAvailableCategories();
                return Page();
            }

            var client = _httpClientFactory.CreateClient("MyApi");

            var response = await client.PutAsJsonAsync($"api/Category/{Id}", Category);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, error);

                // Reload available categories nếu có lỗi từ API
                await LoadAvailableCategories();
                return Page();
            }

            return RedirectToPage("./Index");
        }

        private async Task LoadAvailableCategories()
        {
            var client = _httpClientFactory.CreateClient("MyApi");

            try
            {
                var response = await client.GetAsync("api/Category/get-all-active");
                if (response.IsSuccessStatusCode)
                {
                    var categories = await response.Content.ReadFromJsonAsync<List<CategoryDisplayDTO>>();

                    // Khởi tạo danh sách với option "No Parent"
                    AvailableCategories = new List<SelectListItem>
                    {
                        new SelectListItem { Value = "", Text = "-- No Parent Category --" }
                    };

                    if (categories != null)
                    {
                        // Loại bỏ category hiện tại khỏi danh sách để tránh tự reference
                        // TRỪ trường hợp category đang tự reference chính nó
                        var availableCategories = categories;

                        // Nếu không phải trường hợp self-reference, loại bỏ category hiện tại
                        if (CurrentCategory.ParentCategoryId != Id)
                        {
                            availableCategories = categories.Where(c => c.CategoryId != Id).ToList();
                        }

                        foreach (var category in availableCategories)
                        {
                            // Tạo SelectListItem và đánh dấu selected nếu trùng với ParentCategoryId hiện tại
                            var item = new SelectListItem
                            {
                                Value = category.CategoryId.ToString(),
                                Text = category.CategoryName,
                                Selected = category.CategoryId == CurrentCategory.ParentCategoryId
                            };
                            AvailableCategories.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error nếu cần
                ModelState.AddModelError(string.Empty, "Unable to load categories for selection.");
            }
        }
    }
}