using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly DO_ANContext _context;

        public ForgotPasswordModel(DO_ANContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Email { get; set; }

        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Vui long nhap email.";
                return Page();
            }
            // Here, you would normally check if the email exists in your user database
            // For example:
            var user = _context.Users.FirstOrDefault(u => u.Email == Email);
            if (user == null) { ErrorMessage = "Khong tim thay email."; return Page(); }
           var  id = user.UserId;
            // Generate a unique OTP (or use a token)
           /* string otpCode = Guid.NewGuid().ToString(); // Simple example for demonstration*/

            // Send OTP email
            try
            {
                SendOtpEmail(Email, id);
                SuccessMessage = "Hay kiem tra mail cua ban";
                // Here, you can also save the OTP in a database or cache with an expiration time.
            }
            catch (Exception ex)
            {
                ErrorMessage = "Da xay ra loi .Vui long thu lai sau.";
                // Log the exception
            }

            return Page();
        }
        //Your OTP code is: {id}.It will expire in 10 minutes.
        private void SendOtpEmail(string recipientEmail, int id)
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); // Current time in seconds
            var link = $"https://ndis.cloud/Account/reset_pass?uid={id}&ts={timestamp}";
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("NDIS", "yourapp@example.com"));
            message.To.Add(new MailboxAddress("", recipientEmail));
            message.Subject = "Your OTP Code";
            message.Body = new TextPart("plain")
            {
                Text = $"Nhan vao lien ket de thay doi mat khau: {link}\nLien ket nay se het han sau 10 phut."
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
