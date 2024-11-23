using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MailKit.Net.Smtp;
using MimeKit;
using NaturalDisasterInformationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace NaturalDisasterInformationSystem.Pages.User
{
    public class EmailAlertsModel : PageModel
    {
        private readonly DO_ANContext _context;

        [BindProperty]
        public string Message { get; set; } = string.Empty;

        public EmailAlertsModel(DO_ANContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            // Optionally: Retrieve users if you need to display them on the page.
            ViewData["UserCount"] = await _context.Users.CountAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Message))
            {
                ModelState.AddModelError("", "Message cannot be empty.");
                return Page();
            }

            // Retrieve all users from the database
            var users = await _context.Users.Select(u => u.Email).ToListAsync();

            // Send emails asynchronously to all users
            var emailTasks = users.Select(email => SendEmailAsync(email, Message)).ToList();
            await Task.WhenAll(emailTasks); // Wait for all emails to be sent

            // Optionally: Provide a success message or redirect
            TempData["SuccessMessage"] = "Emails have been sent to all users!";
            return RedirectToPage(); // Reload the page or redirect elsewhere
        }

        private async Task SendEmailAsync(string email, string messageBody)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Natural Disaster System", "no-reply@nds.com"));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Emergency Alert";

            message.Body = new TextPart("plain")
            {
                Text = messageBody
            };

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, false);
                    await client.AuthenticateAsync("ledungb12509@gmail.com", "bdog vxel qcwk bbqh"); // Use App Password
                    await client.SendAsync(message);
                    Console.WriteLine($"Email sent to {email}");
                }
                catch (SmtpCommandException ex)
                {
                    Console.WriteLine($"SMTP Command Exception: {ex.Message} - {ex.StatusCode}");
                }
                catch (MailKit.Security.AuthenticationException ex)
                {
                    Console.WriteLine($"Authentication Exception: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"General Exception: {ex.Message}");
                }
                finally
                {
                    await client.DisconnectAsync(true);
                }
            }
        }
    }
}
