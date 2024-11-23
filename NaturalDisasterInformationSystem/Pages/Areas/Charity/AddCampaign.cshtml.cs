using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeKit;
using NaturalDisasterInformationSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NaturalDisasterInformationSystem.Pages.Areas.Charity
{
    public class AddCampaignModel : PageModel
    {
        private readonly DO_ANContext _context;
        [BindProperty]
        public int CampaignId { get; set; }
        [BindProperty]
        public int CharityId { get; set; }
        [BindProperty]
        public int DisasterId { get; set; }
        [BindProperty]
        public string? CampaignName { get; set; }
        [BindProperty]
        public string? Description { get; set; }
        [BindProperty]
        public double? GoalAmount { get; set; }
        [BindProperty]
        public double? RaisedAmount { get; set; }
        [BindProperty]
        public DateTime? StartDate { get; set; }
        [BindProperty]
        public DateTime? EndDate { get; set; } 
        [BindProperty]
        public List<IFormFile>? Images { get; set; }
        public IFormFile? Images_QR { get; set; }

        public List<Disaster> Disasters { get; set; } = new List<Disaster>();

        public AddCampaignModel(DO_ANContext context)
        {
            _context = context;
        }

        public void OnGet(int? DisasterId = null)
        {
            Disasters = _context.Disasters.ToList();

            if (DisasterId.HasValue)
            {
                this.DisasterId = DisasterId.Value;
            }
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Disasters = _context.Disasters.ToList();
                return Page();
            }

            var Campaign = new FundraisingCampaign
            {
                CharityId = CharityId,
                DisasterId = DisasterId,
                Description = Description,
                CampaignName = CampaignName,
                GoalAmount = GoalAmount,
                RaisedAmount = RaisedAmount,
                StartDate = StartDate,
                EndDate = EndDate,
                Hidden="1"
            };

            if (Images != null && Images.Count > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "campaigns");
                Directory.CreateDirectory(uploadsFolder);

                var imageNames = new List<string>();

                foreach (var image in Images)
                {
                    var fileName = Path.GetFileName(image.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }

                    imageNames.Add(fileName);
                }

                Campaign.ImgCam = string.Join(",", imageNames);
            }
            if (Images_QR != null)
            {
                var uploadsFolderQR = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "QR");
                Directory.CreateDirectory(uploadsFolderQR);

                var fileName1 = Path.GetFileName(Images_QR.FileName);
                var filePath1 = Path.Combine(uploadsFolderQR, fileName1);

                using (var fileStream = new FileStream(filePath1, FileMode.Create))
                {
                    await Images_QR.CopyToAsync(fileStream);
                }

                // Only save the path of the first image
                Campaign.ImgQr = fileName1;
            }
            _context.FundraisingCampaigns.Add(Campaign);
            await _context.SaveChangesAsync();

            // Send notification emails to all users
            var users = _context.Users.Select(u => u.Email).ToList();
            var campaignLink = $"http://localhost:5064/User/DetailCampaignUser?dcu_id={Campaign.CampaignId}";
            var emailBody = $"Chào b?n chúng tôi v?a có m?t chi?n d?ch m?i mang tên là:'{CampaignName}'.Chi?n d?ch ?ang c?n s? ?ng h? c?a các nhà h?o tâm và ?ang trong quá trình tuy?n tình nguy?n viên cho d? án. ?? bi?t thêm chi ti?t vui lòng ?n vào ?ây: {campaignLink}";

            foreach (var userEmail in users)
            {
                await SendEmailAsync(userEmail, emailBody);
            }

            return RedirectToPage("/Areas/Charity/ListCampaign", new { cid = Campaign.CharityId });
        }


        private async Task SendEmailAsync(string email, string messageBody)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Natural Disaster System", "no-reply@nds.com"));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Thông Báo M?i";

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
