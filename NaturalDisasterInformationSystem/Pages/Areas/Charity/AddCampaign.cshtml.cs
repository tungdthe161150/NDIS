using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeKit;
using NaturalDisasterInformationSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NaturalDisasterInformationSystem.Pages.Areas.Charity
{
    [Authorize(Policy = "Charity")]

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
            var campaignLink = $"https://ndis.cloud/User/DetailCampaignUser?dcu_id={Campaign.CampaignId}";
            var emailBody = $@"
<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            color: #333;
            line-height: 1.6;
            margin: 0;
            padding: 20px;
        }}
        .container {{
            max-width: 600px;
            margin: 0 auto;
            background: white;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
            padding: 20px;
        }}
        h1 {{
            color: #4CAF50;
            text-align: center;
        }}
        p {{
            margin: 15px 0;
        }}
        .button {{
            display: inline-block;
            background-color: #4CAF50;
            color: white;
            padding: 10px 15px;
            text-decoration: none;
            border-radius: 5px;
            text-align: center;
        }}
        .footer {{
            margin-top: 20px;
            font-size: 0.9em;
            color: #777;
            text-align: center;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <h1>Thong bao tu to chuc</h1>
        <p>Chao ban,</p>
        <p>Chung toi vua co mot chien dich moi co ten la: <strong>{CampaignName}</strong>.</p>
        <p>Chien dich dang can su ung ho cua cac nha hao tam va hien dang trong qua trinh tuyen tinh nguyen vien cho du an.</p>
        <p>De biet them chi tiet, vui long nhan vao lien ket duoi day:</p>
        <p><a href=""{campaignLink}"" class=""button"">Xem Chi Tiet Chien Dich</a></p>
        <div class=""footer"">
            <p>Cam on ban da dong hanh cung chung toi!</p>
            <p>Tran trong,<br>To chuc</p>
        </div>
    </div>
</body>
</html>
";

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
            message.Subject = "Thong Bao Moi";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = messageBody
            };
            message.Body = bodyBuilder.ToMessageBody();


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
