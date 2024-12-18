using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using NaturalDisasterInformationSystem.Models;
using NaturalDisasterInformationSystem.Services;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalDisasterInformationSystem.Pages.Areas.Alerts
{
    [Authorize(Policy = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly SMSService speedSMSAPI;
        private readonly DO_ANContext context;

        [BindProperty]
        public List<Models.User> Users { get; set; }

        [BindProperty]
        public List<string> SelectedPhoneNumbers { get; set; }

        public CreateModel(DO_ANContext context, SMSService service)
        {
            this.context = context;
            this.speedSMSAPI = service;
        }

        // OnGetAsync loads the users to select from.
        public async Task<IActionResult> OnGetAsync()
        {
            Users = await context.Users.ToListAsync();
            ViewData["User"] = Users;
            return Page();
        }

        // OnPostAsync processes the message sending (SMS + Email).
        public async Task<IActionResult> OnPostAsync(string MessageContent, string AddressFilter)
        {
            if (string.IsNullOrEmpty(MessageContent))
            {
                ViewData["ErrorMessage"] = "Vui lòng nhập nội dung tin nhắn.";
                return Page();
            }

            // Fetch users with the address filter if specified
            IQueryable<Models.User> usersQuery = context.Users;

            if (!string.IsNullOrEmpty(AddressFilter))
            {
                // Use EF.Functions.Like for case-insensitive comparison
                usersQuery = usersQuery.Where(u => EF.Functions.Like(u.Address, "%" + AddressFilter + "%"));
            }

            // Get the list of phone numbers of users whose address matches the filter
            var phoneNumbers = await usersQuery
                .Where(u => !string.IsNullOrEmpty(u.PhoneNumber)) // Make sure phone number is not null or empty
                .Select(u => u.PhoneNumber)
                .ToListAsync();

            if (!phoneNumbers.Any())
            {
                ViewData["ErrorMessage"] = "Không có số điện thoại hợp lệ trong danh sách.";
                return Page();
            }

            // Send SMS (assuming SendSmsAsync returns a list of results)
            var result = await speedSMSAPI.SendSmsAsync(MessageContent, AddressFilter);

            // Save the emergency alert to the database
            var notify = new EmergencyAlert
            {
                AlertMessage = MessageContent,
                CreatedAt = DateTime.Now
            };
            context.EmergencyAlerts.Add(notify);
            await context.SaveChangesAsync();

            // Get the emails of the users whose address matches the filter
            var emailAddresses = await usersQuery
                .Where(u => !string.IsNullOrEmpty(u.Email)) // Ensure the email is not null or empty
                .Select(u => u.Email) // Get the email addresses of filtered users
                .ToListAsync();

            // Send emails separately to avoid using instance methods inside LINQ
            var emailTasks = emailAddresses
                .Select(email => SendEmailAsync(email, MessageContent)) // Send email to filtered users
                .ToList();

            await Task.WhenAll(emailTasks); // Execute all email tasks asynchronously

            // Display SMS sending results
            ViewData["Result"] = string.Join("<br>", result);
            StringBuilder resultMessage = new StringBuilder();

            // Iterate through each email address and construct the message
            foreach (var email in emailAddresses)
            {
                resultMessage.AppendLine($"Gửi email tới {email}: Thành công                            <br />"); // Append each message
            }

            // Store the result in ViewData
            ViewData["Result1"] = resultMessage.ToString();
            return Page();
        }

        // Helper method to send email asynchronously
        private async Task SendEmailAsync(string email, string messageBody)
        {
            var message = new MimeMessage
            {
                From = { new MailboxAddress("Natural Disaster System", "no-reply@nds.com") },
                To = { new MailboxAddress("", email) },
                Subject = "Emergency Alert",
                Body = new TextPart("plain") { Text = messageBody }
            };

            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, false);
                    await client.AuthenticateAsync("ledungb12509@gmail.com", "bdog vxel qcwk bbqh"); // Use App Password
                    await client.SendAsync(message);
                    Console.WriteLine($"Email sent to {email}");
                }
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
        }

    }
}