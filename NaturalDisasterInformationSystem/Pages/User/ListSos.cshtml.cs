using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;
using System.Linq;

namespace NaturalDisasterInformationSystem.Pages.User
{
    public class ListSosModel : PageModel
    {
        private readonly DO_ANContext _context;

        public ListSosModel(DO_ANContext context)
        {
            _context = context;
        }

        public List<So> Reports { get; set; } = new List<So>();
        public string? AddressFilter { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 9;
        public int TotalPages { get; set; }

        public async Task OnGetAsync(string? addressFilter, int pageNumber = 1, int pageSize = 9)
        {
            AddressFilter = addressFilter;
            PageNumber = pageNumber;
            PageSize = pageSize;

            IQueryable<So> query = _context.Sos.Include(u => u.User).OrderByDescending(s => s.CreateTime);

            if (!string.IsNullOrEmpty(AddressFilter))
            {
                query = query.Where(s => s.Address != null && s.Address.Contains(AddressFilter));
            }

            // Calculate total pages
            int totalRecords = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalRecords / (double)PageSize);

            // Apply pagination
            Reports = await query.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToListAsync();
        }
    }
}
