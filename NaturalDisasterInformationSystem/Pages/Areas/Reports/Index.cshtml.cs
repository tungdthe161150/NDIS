using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.Areas.Reports
{
    [Authorize(Policy = "Admin")]

    public class IndexModel : PageModel
    {
        private readonly DO_ANContext context;

        public IndexModel(DO_ANContext context)
        {
            this.context = context;
        }

        [BindProperty]
        public List<Report> reports { get; set; }

        public async Task<IActionResult> OnGet()
        {
            /*reports = context.Reports.
                ToList();*/

            reports = context.Reports.Include(r => r.Campaign)
                        .GroupBy(r => r.CampaignId).Select(g => g.First()) 
                        .ToList();

            return Page();
        }
    }
}
