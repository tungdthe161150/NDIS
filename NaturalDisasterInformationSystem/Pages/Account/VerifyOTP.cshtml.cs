using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.Account
{
    public class VerifyOTPModel : PageModel
    {
        private readonly DO_ANContext _context;

        public VerifyOTPModel(DO_ANContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string OTPEntered { get; set; } = null!;

        [BindProperty(SupportsGet = true)]
        public string Email { get; set; } = null!; // Nhận email từ query string

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Kiểm tra mã OTP
            var otpStored = TempData["OTP"]?.ToString();
            var otpExpiry = (DateTime?)TempData["OTPExpiry"];

            if (otpStored == null || otpExpiry == null || OTPEntered != otpStored || otpExpiry < DateTime.UtcNow)
            {
                // OTP không hợp lệ hoặc đã hết hạn
                TempData["ErrorMessage"] = "OTP không hợp lệ hoặc đã hết hạn.";
                return Page();
            }

            // OTP hợp lệ => lưu thông tin người dùng vào cơ sở dữ liệu
            var user = new NaturalDisasterInformationSystem.Models.User
            {
                Username = TempData["Username"].ToString(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(TempData["Password"].ToString()),
                Email = TempData["Email"].ToString(),
                PhoneNumber = TempData["PhoneNumber"]?.ToString(),
                RoleId = 3, // Giả sử RoleId 1 là cho người dùng thường
                Active = true, // Người dùng đã được kích hoạt
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Xóa OTP và các thông tin tạm thời sau khi đã lưu vào cơ sở dữ liệu
            TempData.Clear();

            // Điều hướng đến trang đăng nhập sau khi xác thực thành công
            return RedirectToPage("/Account/Login");
        }

    }
}
