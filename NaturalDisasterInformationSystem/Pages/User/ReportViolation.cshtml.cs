using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;
using Org.BouncyCastle.Asn1.X509;
using System.Composition;

namespace NaturalDisasterInformationSystem.Pages.User
{
    public class ReportViolationModel : PageModel
    {
        private readonly DO_ANContext context;

        public ReportViolationModel(DO_ANContext context)
        {
            this.context = context;
        }

        [BindProperty]
        public FundraisingCampaign campaign { get; set; }

        [BindProperty]
        public Report reports { get; set; }

        [BindProperty]
        public string Reason { get; set; }

        public async Task<IActionResult> OnGet(int id)
        {
            campaign = await context.FundraisingCampaigns
                .Include(c => c.Charity)
                .ThenInclude(c => c.User)
                .Include(c => c.Disaster)
                .FirstOrDefaultAsync(fc => fc.CampaignId == id);

            // Ki?m tra n?u campaign không t?n t?i
            if (campaign == null)
            {
                return NotFound("Chi?n d?ch không t?n t?i.");
            }

            ViewData["Reports"] = campaign;

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                return Page();
            }

            reports.CreatedAt = DateTime.Now;
            reports.Status = "pending";
            reports.Reason = Reason;
            try
            {
                context.Reports.Add(reports);
                context.SaveChanges();

                TempData["SuccessMessage"] = "Báo cáo c?a b?n ?ã ???c g?i thành công.";
                return RedirectToPage("./ListCampaignUser");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"L?i khi g?i báo cáo: {ex.Message}");
                return Page();
            }
        }
    }
}
