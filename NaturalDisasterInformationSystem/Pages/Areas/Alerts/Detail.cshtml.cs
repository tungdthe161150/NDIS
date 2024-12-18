using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.Areas.Admin.Alerts
{
    [Authorize(Policy = "Admin")]

    public class DetailModel : PageModel
    {
        private readonly DO_ANContext context;

        public DetailModel(DO_ANContext context)
        {
            this.context = context;
        }

        [BindProperty]
        public Models.EmergencyAlert news { get; set; }
        [BindProperty]
        public Models.User user { get; set; }

        public async Task<IActionResult> OnGet(int? id)
        {
            news = context.EmergencyAlerts
                .FirstOrDefault(n => n.AlertId == id);

            /*ViewData["Phone"] = context.Users
                .Select(x => x.PhoneNumber)
                .Distinct()
                .ToList();*/

            ViewData["News"] = news;
            return Page();
        }
    }
}
