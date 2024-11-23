using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.Areas.Charity
{
    public class ListCampaignModel : PageModel
    {
        private readonly DO_ANContext context;

        public List<NaturalDisasterInformationSystem.Models.FundraisingCampaign> FundraisingCampaigns { get; set; }

        public ListCampaignModel(DO_ANContext context)
        {
            this.context = context;
        }

        public async Task<IActionResult> OnGetAsync(int cid)
        {
            FundraisingCampaigns = await context.FundraisingCampaigns.Include(c=>c.Charity.User).ThenInclude(c=>c.VolunteerHistories).OrderByDescending(f => f.CampaignId)
                .Where(campaign => campaign.CharityId == cid)
                .ToListAsync();

            return Page();
        }
        public async Task<IActionResult> OnPostAsync(int UserId, int EventId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Ki?m tra xem b?n ghi ?� t?n t?i ch?a
            var existingHistory = await context.VolunteerHistories
                .FirstOrDefaultAsync(vh => vh.UserId == UserId && vh.EventId == EventId);

            if (existingHistory != null)
            {
                // N?u ?� t?n t?i, c� th? tr? v? th�ng b�o ho?c ch? chuy?n h??ng
                ModelState.AddModelError(string.Empty, "B?n ?� tham gia s? ki?n n�y tr??c ?�.");
                return RedirectToPage("/Areas/Charity/DetailCampaign", new { dc_id = existingHistory.EventId });
            }

            // N?u ch?a t?n t?i, th�m m?i
            var volunteerHistory = new VolunteerHistory
            {
                UserId = UserId,
                EventId = EventId,
                Status = "tham gia",
                JoinedAt = DateTime.Now // C?p nh?t th?i gian tham gia
            };

            context.VolunteerHistories.Add(volunteerHistory);
            await context.SaveChangesAsync();

            // Chuy?n h??ng ??n trang chi ti?t chi?n d?ch
            return RedirectToPage("/Areas/Charity/DetailCampaign", new { dc_id = volunteerHistory.EventId });
        }

    }
}
