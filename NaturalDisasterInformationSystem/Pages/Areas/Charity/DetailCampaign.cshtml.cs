using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.Areas.Charity
{
    [Authorize(Policy = "Charity")]

    public class DetailCampaignModel : PageModel
    {
        private readonly DO_ANContext _context;

        public DetailCampaignModel(DO_ANContext context)
        {
            _context = context;
        }

        public FundraisingCampaign Campaign { get; set; }

        public IActionResult OnGet(int dc_id)
        {
            Campaign = _context.FundraisingCampaigns.FirstOrDefault(c => c.CampaignId == dc_id);

            if (Campaign == null)
            {
                return NotFound();
            }

            return Page();
        }
        public async Task<IActionResult> OnPostHiddenAsync(int id,string Hidden)
        {
            var fun = await _context.FundraisingCampaigns.FindAsync(id);
            if (fun != null)
            {
                fun.Hidden = "0";

                _context.FundraisingCampaigns.Update(fun);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { dc_id = fun.CampaignId }); // Refresh the page with the same c_id
        }
        public async Task<IActionResult> OnPostDisAsync(int id, string Hidden)
        {
            var fun = await _context.FundraisingCampaigns.FindAsync(id);
            if (fun != null)
            {
                fun.Hidden = "1";

                _context.FundraisingCampaigns.Update(fun);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { dc_id = fun.CampaignId }); // Refresh the page with the same c_id
        }
    }
}
