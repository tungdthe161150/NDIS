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
                ErrorMessage = "Vui lòng nh?p email.";
                return Page();
            }
            // Here, you would normally check if the email exists in your user database
            // For example:
            var user = _context.Users.FirstOrDefault(u => u.Email == Email);
            if (user == null) { ErrorMessage = "Email not found."; return Page(); }
           var  id = user.UserId;
            // Generate a unique OTP (or use a token)
           /* string otpCode = Guid.NewGuid().ToString(); // Simple example for demonstration*/

            // Send OTP email
            try
            {
                SendOtpEmail(Email, id);
                SuccessMessage = "Hãy check mail c?a b?n.";
                // Here, you can also save the OTP in a database or cache with an expiration time.
            }
            catch (Exception ex)
            {
                ErrorMessage = "?ã x?y ra l?i khi g?i email. Vui lòng th? l?i sau.";
                // Log the exception
            }

            return Page();
        }
        //Your OTP code is: {id}.It will expire in 10 minutes.
        private void SendOtpEmail(string recipientEmail, int id)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("YourApp", "yourapp@example.com"));
            message.To.Add(new MailboxAddress("", recipientEmail));
            message.Subject = "Your OTP Code";
            message.Body = new TextPart("plain")
            {
                Text = $" \n" +
                        $"Click the link to change your password: " +
                        $"http://localhost:5064/Account/reset_pass?uid={id}"
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
