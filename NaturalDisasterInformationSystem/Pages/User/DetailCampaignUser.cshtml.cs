using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.User
{
    public class DetailCampaignUserModel : PageModel
    {
        private readonly DO_ANContext _context;

        public DetailCampaignUserModel(DO_ANContext context)
        {
            _context = context;
        }
        public NaturalDisasterInformationSystem.Models.FundraisingCampaign DetailFundraisingCampaign { get; set; }

        public async Task<IActionResult> OnGetAsync(int dcu_id)
        {
            DetailFundraisingCampaign = await _context.FundraisingCampaigns.Include(d=>d.Charity).Include(v=>v.VolunteerHistories)
          .FirstOrDefaultAsync(c => c.CampaignId == dcu_id);

            if (DetailFundraisingCampaign == null)
            {
                return NotFound(); 
            }

            return Page();
        }
    }
}
