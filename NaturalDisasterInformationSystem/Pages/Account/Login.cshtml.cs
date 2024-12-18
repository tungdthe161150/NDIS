using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;
using System.Security.Claims;

namespace NaturalDisasterInformationSystem.Pages.Account
{
  
    public class LoginModel : PageModel
    {
        private readonly DO_ANContext _context;

        [BindProperty]
        public string Email { get; set; } = string.Empty; // Change to Email

        [BindProperty]
        public string Password { get; set; } = string.Empty; // Keep Password

        public LoginModel(DO_ANContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Check if the user exists by email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);

            if (user == null || !VerifyPassword(user.PasswordHash, Password))
            {
                ModelState.AddModelError(string.Empty, "Đăng nhập không hợp lệ.");
                return Page();
            }
            // L?u RoleId vào Session
            HttpContext.Session.SetString("RoleId", user.RoleId.ToString());
            HttpContext.Session.SetString("UserId", user.UserId.ToString());
            if (user.PhoneNumber != null)
            {
                HttpContext.Session.SetString("SDT", user.PhoneNumber);

            }
           

            // Tiến hành xác thực người dùng và lưu thông tin vào cookie
            var claims = new List<Claim>
{
    new Claim(ClaimTypes.Name, user.Username),
    new Claim(ClaimTypes.Role, user.RoleId.ToString()) // Lưu RoleId vào claim
};

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(claimsPrincipal);
            if (user.Active == false)
            {
                TempData["ErrorMessage"] = "Tài khoản này đã bị vô hiệu hóa.";

                return RedirectToPage("/Account/logout");
            }
            else
            {
                // Điều hướng dựa trên RoleId
                if (user.RoleId == 1) // Admin
                {
                    var chari = await _context.Charities.FirstOrDefaultAsync(c => c.UserId == user.UserId);
                    if (chari != null) { HttpContext.Session.SetString("CharityId", chari.CharityId.ToString()); }

                    return RedirectToPage("/Areas/Admin/Index"); // Điều chỉnh trang admin
                }
                else if (user.RoleId == 2) // Người dùng thông thường
                {
                    var chari = await _context.Charities.FirstOrDefaultAsync(c => c.UserId == user.UserId);
                    if (chari != null) { HttpContext.Session.SetString("CharityId", chari.CharityId.ToString()); }
                    return RedirectToPage("/User/Home"); // Điều chỉnh trang cho người dùng
                }
                else if (user.RoleId == 3) // Người dùng thông thường
                {

                    return RedirectToPage("/User/Home"); // Điều chỉnh trang cho người dùng
                }
            }
            
            // Successful login logic (e.g., setting a cookie or session)
            return RedirectToPage("/Index"); // Change to your desired page
        }

        private bool VerifyPassword(string hashedPassword, string password)
        {
            // Implement your password verification logic here (e.g., using BCrypt)
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword); // Simplified, replace with real logic
        }
    }

}
