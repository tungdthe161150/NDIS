using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.Areas.Admin.Alerts
{
    [Authorize(Policy = "Admin")]

    public class DeleteModel : PageModel
    {
        private readonly DO_ANContext context;

        public DeleteModel(DO_ANContext context)
        {
            this.context = context;
        }

        [BindProperty]
        public Models.EmergencyAlert Alert { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alert = await context.EmergencyAlerts.FirstOrDefaultAsync(m => m.AlertId == id);

            if (alert == null)
            {
                return NotFound();
            }
            else
            {
                Alert = alert;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || context.EmergencyAlerts == null)
            {
                return BadRequest("ID không hợp lệ.");
            }

            var alert = await context.EmergencyAlerts.FindAsync(id);

            if (alert == null)
            {
                return NotFound("Không tìm thấy bản tin.");
            }
            else
            {
                try
                {
                    Alert = alert;
                    context.EmergencyAlerts.Remove(Alert);
                    await context.SaveChangesAsync();

                    //return new JsonResult(new { success = true });
                    return RedirectToPage("./Index");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Lỗi khi xóa bản tin: {ex.Message}");
                }
            }
        }
    }
}
