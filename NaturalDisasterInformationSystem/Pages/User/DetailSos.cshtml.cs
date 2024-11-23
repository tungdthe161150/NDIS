using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.User
{
    public class DetailSosModel : PageModel
    {
        private readonly DO_ANContext _context;

        public DetailSosModel(DO_ANContext context)
        {
            _context = context;
        }

        public So Report { get; set; }

        public async Task<IActionResult> OnGetAsync(int s_id)
        {
            Report = await _context.Sos.Include(u => u.User).FirstOrDefaultAsync(s => s.SosId == s_id);

            if (Report == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
