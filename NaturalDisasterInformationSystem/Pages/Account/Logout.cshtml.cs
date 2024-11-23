using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NaturalDisasterInformationSystem.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Xóa t?t c? d? li?u trong Session
            HttpContext.Session.Clear();

            // Xóa Cookie 'RoleId' (n?u ?ã dùng Cookie ?? l?u)
            if (Request.Cookies.ContainsKey("RoleId"))
            {
                Response.Cookies.Delete("RoleId");
            }

            // Chuy?n h??ng v? trang Login (ho?c b?t k? trang nào b?n mu?n)
            return RedirectToPage("/Account/Login");
        }
    }
}
