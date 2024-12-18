using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.Areas.Admin.SOS
{
    [Authorize(Policy = "Admin")]

    public class ListSosAdminModel : PageModel
    {
        private readonly DO_ANContext _context;

        public ListSosAdminModel(DO_ANContext context)
        {
            _context = context;
        }
        public List<So> Reports { get; set; } = new List<So>();
        public string? AddressFilter { get; set; }

        public async Task OnGetAsync(string? addressFilter)
        {
            AddressFilter = addressFilter;

            IQueryable<So> query = _context.Sos.Include(u => u.User).OrderByDescending(s => s.CreateTime);

            if (!string.IsNullOrEmpty(AddressFilter))
            {
                query = query.Where(s => s.Address != null && s.Address.Contains(AddressFilter));
            }

            Reports = await query.ToListAsync();
        }
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var sos = await _context.Sos.FindAsync(id);
            if (sos == null)
            {
                return NotFound();
            }

            _context.Sos.Remove(sos);
            await _context.SaveChangesAsync();

            return RedirectToPage("./ListSosAdmin");
        }


    }
}
