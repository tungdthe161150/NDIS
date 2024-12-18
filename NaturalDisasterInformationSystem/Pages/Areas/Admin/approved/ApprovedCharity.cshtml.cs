using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NaturalDisasterInformationSystem.Pages.Areas.Admin.approved
{
    [Authorize(Policy = "Admin")]

    public class ApprovedCharityModel : PageModel
    {
        private readonly DO_ANContext _context;

        public ApprovedCharityModel(DO_ANContext context)
        {
            _context = context;
        }

        public List<Models.Charity> ListCharity { get; set; } = new List<Models.Charity>();

        public async Task OnGetAsync()
        {
            ListCharity = await _context.Charities
                .Include(d => d.DocumentImgs)
                .OrderByDescending(c=>c.CreatedAt)
                .ToListAsync();
        }
    }
}
