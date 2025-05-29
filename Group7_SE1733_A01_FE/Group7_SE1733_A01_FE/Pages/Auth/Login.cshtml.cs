using Group7_SE1733_A01_FE.Request;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Group7_SE1733_A01_FE.Response;

namespace Group7_SE1733_A01_FE.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [BindProperty]
        public LoginRequest LoginRequest { get; set; } = default!;


        public IActionResult OnGet()
        {
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            // Gọi API để đăng nhập
            var client = _httpClientFactory.CreateClient("MyApi");
            var response = await client.PostAsJsonAsync("api/SystemAccounts/login", LoginRequest);
            if (response.IsSuccessStatusCode)
            {

                if (response.Content != null)
                {
                    var item = await response.Content.ReadFromJsonAsync<SystemAccountDTO>();
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, item.AccountEmail),
                new Claim(ClaimTypes.Role, item.AccountRole.ToString())
            };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                    Response.Cookies.Append("Email", item.AccountEmail);
                    Response.Cookies.Append("Role", item.AccountRole.ToString());

                    return RedirectToPage("/Index");
                }

                return RedirectToPage("/Auth/Login");
            }
            else
            {
                // Xử lý lỗi đăng nhập
                ModelState.AddModelError(string.Empty, "Đăng nhập không thành công. Vui lòng kiểm tra lại thông tin đăng nhập.");
                return Page();
            }
        }

    }
}
