using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;
using System.Threading.Tasks;

namespace NaturalDisasterInformationSystem.Pages.Areas.Admin.approved
{
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

            // Ensure that RoleId exists on the User model
            if (charity.User != null)
            {
                charity.User.RoleId = 2; // Example: Set to 2 for approved users
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("ApprovedCharity");
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
