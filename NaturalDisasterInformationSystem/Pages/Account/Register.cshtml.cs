using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NaturalDisasterInformationSystem.Models;
using MailKit.Net.Smtp;
using MimeKit;

namespace NaturalDisasterInformationSystem.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly DO_ANContext _context;

        // Properties for registration
        [BindProperty]
        public string Username { get; set; } = null!;
        [BindProperty]
        public string Password { get; set; } = null!;
        [BindProperty]
        public string Email { get; set; } = null!;
        [BindProperty]
        public string? PhoneNumber { get; set; }

        public RegisterModel(DO_ANContext context)
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

            // Kiểm tra username hoặc email đã tồn tại
            if (_context.Users.Any(u => u.Username == Username || u.Email == Email))
            {
                ModelState.AddModelError(string.Empty, "Tên người dùng hoặc Email đã tồn tại.");
                return Page();
            }
            if (string.IsNullOrWhiteSpace(Password) || Password.Length < 8)
            {
                ModelState.AddModelError(nameof(Password), "Mật khẩu phải có ít nhất 8 ký tự.");
                return Page();
            }
            if (string.IsNullOrWhiteSpace(PhoneNumber) || PhoneNumber.Length != 10 || !PhoneNumber.All(char.IsDigit))
            {
                ModelState.AddModelError(nameof(PhoneNumber), "Số điện thoại phải có 10 chữ số.");
                return Page();
            }

            // Tạo mã OTP
            Random random = new Random();
            string otpCode = random.Next(100000, 999999).ToString();

            // Lưu thông tin người dùng vào TempData (thông tin sẽ lưu tạm trong session)
            TempData["Username"] = Username;
            TempData["Password"] = Password;
            TempData["Email"] = Email;
            TempData["PhoneNumber"] = PhoneNumber;
            // Lưu OTP và thời gian hết hạn
            var generatedOtp = otpCode;
            HttpContext.Session.SetString("OTP", generatedOtp);
            HttpContext.Session.SetString("OTPExpiry", DateTime.UtcNow.AddMinutes(5).ToString());
            HttpContext.Session.SetString("Email", Email);


            // Gửi OTP qua email
            SendOtpEmail(Email, otpCode);

            // Điều hướng đến trang VerifyOTP
            return RedirectToPage("/Account/VerifyOTP");
        }

        private void SendOtpEmail(string recipientEmail, string otpCode)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("YourApp", "yourapp@example.com"));
            message.To.Add(new MailboxAddress("",recipientEmail));
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
