using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace NaturalDisasterInformationSystem.Pages.User
{
    public class ListCampaignUserModel : PageModel
    {
        private readonly DO_ANContext _context;

        public List<FundraisingCampaign> FundraisingCampaignsU { get; set; }
        public List<Disaster> Disasters { get; set; }  // Add this line to hold the disasters

        public ListCampaignUserModel(DO_ANContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(string? filter, string? disasterName, string? location)
        {
            // Get list of disasters to populate the dropdown
            ViewData["Disasters"] = await _context.Disasters.ToListAsync();
            Disasters = await _context.Disasters.ToListAsync();

            IQueryable<FundraisingCampaign> campaignsQuery = _context.FundraisingCampaigns
                .Where(h => h.Hidden == "1")
                .OrderByDescending(f => f.CampaignId)
                .Include(c => c.Charity)
                .Include(c => c.Disaster);  // Include Disaster table to filter by DisasterName

            // Filter by Ended Campaigns
            if (filter == "ended")
            {
                campaignsQuery = campaignsQuery.Where(c => c.EndDate < DateTime.Now);
            }
            if (filter == "ongoing")
            {
                campaignsQuery = campaignsQuery.Where(c => c.StartDate <= DateTime.Now && c.EndDate > DateTime.Now);
            }


            // Filter by Disaster Name
            if (!string.IsNullOrEmpty(disasterName))
            {
                campaignsQuery = campaignsQuery.Where(c => c.Disaster.DisasterName.Contains(disasterName));
            }
            // Apply "Location" filter
            if (!string.IsNullOrEmpty(location))
            {
                var lowerLocation = location.ToLower();
                campaignsQuery = campaignsQuery.Where(campaign => campaign.Disaster.Location != null && campaign.Disaster.Location.ToLower().Contains(lowerLocation) );
            }
            // Execute the query and return the results
            FundraisingCampaignsU = await campaignsQuery.ToListAsync();
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
                ModelState.AddModelError(string.Empty, "Ban da tham gia su kien nay truoc do.");
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
