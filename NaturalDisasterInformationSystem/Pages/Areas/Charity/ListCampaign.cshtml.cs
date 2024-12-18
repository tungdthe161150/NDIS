using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;
using System.Linq;

namespace NaturalDisasterInformationSystem.Pages.Areas.Charity
{ 
    [Authorize(Policy = "Charity")]
    public class ListCampaignModel : PageModel
    {
        private readonly DO_ANContext context;


        public ListCampaignModel(DO_ANContext context)
        {
            this.context = context;
        }

        public List<Disaster> Disasters { get; set; } = new();
        public List<FundraisingCampaign> FundraisingCampaigns { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int cid, string? filter, string? disasterName, string? location)
        {
            // Fetch available disasters for the filter dropdown
            Disasters = await context.Disasters.ToListAsync();

            // Query the fundraising campaigns
            var query = context.FundraisingCampaigns
                .Include(c => c.Charity.User)
                .ThenInclude(c => c.VolunteerHistories)
                .Where(campaign => campaign.CharityId == cid);

            // Apply "Status" filter
            if (!string.IsNullOrEmpty(filter))
            {
                if (filter == "ended")
                {
                    query = query.Where(campaign => campaign.EndDate.HasValue && campaign.EndDate.Value <= DateTime.Now&& campaign.CharityId == cid);
                }
                else if (filter == "ongoing")
                {
                    query = query.Where(campaign => campaign.StartDate.HasValue && campaign.StartDate.Value <= DateTime.Now &&
                                                    (!campaign.EndDate.HasValue || campaign.EndDate.Value > DateTime.Now)&& campaign.CharityId == cid);
                }
            }

            // Apply "Disaster" filter
            if (!string.IsNullOrEmpty(disasterName))
            {
                query = query.Where(campaign => campaign.Disaster.DisasterName == disasterName&& campaign.CharityId == cid);
            }
            // Apply "Location" filter
            if (!string.IsNullOrEmpty(location))
            {
                var lowerLocation = location.ToLower();
                query = query.Where(campaign => campaign.Disaster.Location != null && campaign.Disaster.Location.ToLower().Contains(lowerLocation) && campaign.CharityId == cid);
            }
            // Execute the query
            FundraisingCampaigns = await query.OrderByDescending(f => f.CampaignId).ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int UserId, int EventId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Ki?m tra xem b?n ghi ?ã t?n t?i ch?a
            var existingHistory = await context.VolunteerHistories
                .FirstOrDefaultAsync(vh => vh.UserId == UserId && vh.EventId == EventId);

            if (existingHistory != null)
            {
                // N?u ?ã t?n t?i, có th? tr? v? thông báo ho?c ch? chuy?n h??ng
                ModelState.AddModelError(string.Empty, "Ban da tham gia su kien nay truoc do.");
                return RedirectToPage("/Areas/Charity/DetailCampaign", new { dc_id = existingHistory.EventId });
            }

            // N?u ch?a t?n t?i, thêm m?i
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
