using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NaturalDisasterInformationSystem.Pages.User
{
    public class FinanceReportModel : PageModel
    {
        private readonly DO_ANContext _context;

        public FinanceReportModel(DO_ANContext context)
        {
            _context = context;
        }

        public List<FundraisingDonation> Donations { get; set; } = new List<FundraisingDonation>();

        public async Task<IActionResult> OnGetAsync(int fr_id)
        {
            Donations = await _context.FundraisingDonations
                .Include(d => d.Campaign)
                .Include(d => d.User)
                .Where(d => d.UserId == fr_id)
                .OrderByDescending(d => d.DonationDate)
                .ToListAsync();

           /* if (Donations.Count == 0)
                return NotFound("Không tìm thấy dữ liệu quyên góp cho tổ chức này.");*/

            return Page();
        }
    }
}
