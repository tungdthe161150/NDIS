using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;
using System.Threading.Tasks;

namespace NaturalDisasterInformationSystem.Pages.User
{
    public class ProfileCharityModel : PageModel
    {
        private readonly DO_ANContext _context;

        public ProfileCharityModel(DO_ANContext context)
        {
            _context = context;
        }

        public Charity? Charity { get; set; }

        public async Task<IActionResult> OnGetAsync(int pc_id)
        {
            Charity = await _context.Charities.Include(c => c.User) 
                                    .FirstOrDefaultAsync(c => c.CharityId == pc_id);

            if (Charity == null)
            {
                return NotFound(); 
            }

            return Page();
        }
    }
}
