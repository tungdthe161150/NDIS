using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.Areas.Admin
{
    [Authorize(Policy = "Admin")]

    public class DetailModel : PageModel
    {
        private readonly DO_ANContext context;

        public DetailModel(DO_ANContext context)
        {
            this.context = context;
        }

        public NaturalDisasterInformationSystem.Models.User User { get; set; }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            User = await context.Users
                           .Include(r => r.Role)
                           .FirstOrDefaultAsync(u => u.UserId == id); if (User == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
