using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.Areas.Alerts
{
    [Authorize(Policy = "Admin")]

    public class UpdateModel : PageModel
    {
        private readonly DO_ANContext context;

        public UpdateModel(DO_ANContext context)
        {
            this.context = context;
        }

        [BindProperty]
        public Models.EmergencyAlert Alert { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || context.EmergencyAlerts == null)
            {
                return NotFound();
            }

            var alert = await context.EmergencyAlerts.FirstOrDefaultAsync(u => u.AlertId == id);

            if (alert == null)
            {
                return NotFound();
            }

            Alert = alert;
            ViewData["News"] = Alert;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToPage("./Index");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var alertDb = context.EmergencyAlerts.Find(id);

            if (alertDb == null)
            {
                return NotFound();
            }

            // Cập nhật các thuộc tính
            alertDb.AlertMessage = Alert.AlertMessage;

            Alert = alertDb;

            ViewData["News"] = Alert;

            context.EmergencyAlerts.Update(alertDb);
            context.SaveChanges();

            TempData["UpdateSuccess"] = "Cập nhật thành công!";

            return RedirectToPage("./Index");
        }
    }
}
