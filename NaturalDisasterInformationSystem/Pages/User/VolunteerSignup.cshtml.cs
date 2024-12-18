using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NaturalDisasterInformationSystem.Models;
using System;
using System.Threading.Tasks;

namespace NaturalDisasterInformationSystem.Pages.User
{
    [Authorize]

    public class VolunteerSignupModel : PageModel
    {
        private readonly DO_ANContext _context;

        public VolunteerSignupModel(DO_ANContext context)
        {
            _context = context;
        }
        public FundraisingCampaign Campaign { get; set; }

        [BindProperty]
        public int UserId { get; set; }
        [BindProperty]
        public int EventId { get; set; }
        [BindProperty]
        public string? Status { get; set; }
        [BindProperty]
        public string? Evaluate { get; set; }
        [BindProperty]
        public string? Reasons { get; set; }
        [BindProperty]
        public string? Des { get; set; }
        public string? Message { get; set; }

        public void OnGet(int vr_id)
        {
            Campaign = _context.FundraisingCampaigns.FirstOrDefault(c => c.CampaignId == vr_id);

            if (Campaign == null)
            {
                // N?u không tìm th?y Campaign, ??t m?t thông báo ho?c chuy?n h??ng ng??i dùng
                Message = "Khong tim thay chien dich gay quy!";
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var volunteerHistory = new VolunteerHistory
            {
                UserId = UserId,
                EventId = EventId,
                Status = "tham gia",
                Reasons = Reasons,
                Describe = Des,
                JoinedAt = DateTime.Now // C?p nh?t th?i gian tham gia
            };

            _context.VolunteerHistories.Add(volunteerHistory);
            await _context.SaveChangesAsync();

            Message = " ";
            return RedirectToPage("/User/DetailCampaignUser", new { dcu_id = volunteerHistory.EventId });
        }

    }
}
