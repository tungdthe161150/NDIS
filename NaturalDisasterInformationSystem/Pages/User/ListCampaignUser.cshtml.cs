using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NaturalDisasterInformationSystem.Pages.User
{
    public class ListCampaignUserModel : PageModel
    {
        private readonly DO_ANContext _context;

        public List<FundraisingCampaign> FundraisingCampaignsU { get; set; }

        public ListCampaignUserModel(DO_ANContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Retrieve all campaigns without pagination
            FundraisingCampaignsU = await _context.FundraisingCampaigns.Where(h=>h.Hidden=="1").OrderByDescending(f=>f.CampaignId).Include(c => c.Charity).ToListAsync();
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(int UserId, int EventId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Ki?m tra xem b?n ghi ?ã t?n t?i ch?a
            var existingHistory = await _context.VolunteerHistories
                .FirstOrDefaultAsync(vh => vh.UserId == UserId && vh.EventId == EventId);

            if (existingHistory != null)
            {
                // N?u ?ã t?n t?i, có th? tr? v? thông báo ho?c ch? chuy?n h??ng
                ModelState.AddModelError(string.Empty, "B?n ?ã tham gia s? ki?n này tr??c ?ó.");
                return RedirectToPage("/user/DetailCampaignUser", new { dcu_id = existingHistory.EventId });
            }

            // N?u ch?a t?n t?i, thêm m?i
            var volunteerHistory = new VolunteerHistory
            {
                UserId = UserId,
                EventId = EventId,
                Status = "tham gia",
                JoinedAt = DateTime.Now // C?p nh?t th?i gian tham gia
            };

            _context.VolunteerHistories.Add(volunteerHistory);
            await _context.SaveChangesAsync();

            // Chuy?n h??ng ??n trang chi ti?t chi?n d?ch
            return RedirectToPage("/user/DetailCampaignUser", new { dcu_id = volunteerHistory.EventId });
        }
    }
}
