using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MimeKit;
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
            var otpStored = HttpContext.Session.GetString("OTP");
            var otpExpiryString = HttpContext.Session.GetString("OTPExpiry");
            DateTime? otpExpiry = otpExpiryString != null ? DateTime.Parse(otpExpiryString) : null;

            if (otpStored == null || otpExpiry == null || otpExpiry < DateTime.UtcNow)
            {
                // OTP không hợp lệ hoặc đã hết hạn
                TempData["ErrorMessage"] = "OTP không hợp lệ hoặc đã hết hạn.";
                return Page();
            }

            if (OTPEntered != otpStored)
            {
                TempData["ErrorMessage"] = "OTP không chính xác. Vui lòng nhập lại.";
                return Page(); // Trả về trang hiện tại để nhập lại OTP
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
            HttpContext.Session.Remove("OTP");
            HttpContext.Session.Remove("OTPExpiry");

            // Điều hướng đến trang đăng nhập sau khi xác thực thành công
            return RedirectToPage("/Account/Login");
        }

        public async Task<IActionResult> OnPostResendOtp()
        {
            // Tạo mã OTP mới
            var newOtp = new Random().Next(100000, 999999).ToString();
            HttpContext.Session.SetString("OTP", newOtp);
            HttpContext.Session.SetString("OTPExpiry", DateTime.UtcNow.AddMinutes(10).ToString());

            // Gửi OTP qua email (giả lập)
            var email = HttpContext.Session.GetString("Email");

            if (email != null)
            {
                SendOtpEmail(email, newOtp);

            }
            else
            {
                TempData["ErrorMessage"] = "Không thể gửi lại OTP. Vui lòng thử lại.";
            }
            return RedirectToPage("/Account/VerifyOTP");
        }

        private void SendOtpEmail(string recipientEmail, string otpCode)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("YourApp", "yourapp@example.com"));
            message.To.Add(new MailboxAddress("", recipientEmail));
            message.Subject = "Your OTP Code";
            message.Body = new TextPart("plain")
            {
                Text = $"Your OTP code is: {otpCode}. It will expire in 10 minutes."
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("ledungb12509@gmail.com", "bdog vxel qcwk bbqh");
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}

