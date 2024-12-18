using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using NaturalDisasterInformationSystem.Models;
using System.Threading.Tasks;

namespace NaturalDisasterInformationSystem.Pages.Areas.Admin.approved
{
    [Authorize(Policy = "Admin")]

    public class DetailApprovedCharityModel : PageModel
    {
        private readonly DO_ANContext _context;

        public DetailApprovedCharityModel(DO_ANContext context)
        {
            _context = context;
        }

        public Models.Charity CharityDetail { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int dac_id)
        {
            CharityDetail = await _context.Charities
                .Include(d => d.DocumentImgs)
                .FirstOrDefaultAsync(d => d.CharityId == dac_id);

            if (CharityDetail == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostApproveAsync(int dac_id, string? Comment)
        {
            var charity = await _context.Charities
                .Include(u => u.User)
                .FirstOrDefaultAsync(d => d.CharityId == dac_id);

            if (charity == null)
            {
                return NotFound();
            }

            charity.Reliability = true;
            charity.Comment = Comment;  // Save the comment
            SendConfirmationEmail(charity.ContactEmail, Comment);
            // Ensure that RoleId exists on the User model
            if (charity.User != null)
            {
                charity.User.RoleId = 2; // Example: Set to 2 for approved users
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("ApprovedCharity");
        }
        private void SendConfirmationEmail(string email,string content)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Natural Disaster System", "no-reply@nds.com"));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Charity Registration Confirmation";

            message.Body = new TextPart("plain")
            {
                Text = content
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("ledungb12509@gmail.com", "bdog vxel qcwk bbqh");
                client.Send(message);
                client.Disconnect(true);
            }
        }
        public async Task<IActionResult> OnPostHidAsync(int dac_id, string? Comment)
        {
            var charity = await _context.Charities
                .Include(u => u.User)
                .FirstOrDefaultAsync(d => d.CharityId == dac_id);

            if (charity == null)
            {
                return NotFound();
            }

            charity.Reliability = false;
            charity.Comment = Comment;  // Save the comment
            SendConfirmationEmail(charity.ContactEmail, Comment);

            // Ensure that RoleId exists on the User model
            if (charity.User != null)
            {
                charity.User.RoleId = 3; // Example: Set to 3 for rejected users
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("ApprovedCharity");
        }



        public async Task<IActionResult> OnPostDeleteAsync(int dac_id)
        {
            var charity = await _context.Charities
                .Include(c => c.DocumentImgs)  // Include the related DocumentImgs
                .FirstOrDefaultAsync(c => c.CharityId == dac_id);

            if (charity == null)
            {
                return NotFound();
            }

            // Remove related DocumentImgs first
            _context.DocumentImgs.RemoveRange(charity.DocumentImgs);

            // Then remove the charity
            _context.Charities.Remove(charity);

            await _context.SaveChangesAsync();

            return RedirectToPage("ApprovedCharity");
        }

    }
}
