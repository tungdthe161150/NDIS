using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.Areas.Admin.Story
{
    [Authorize(Policy = "Admin")]

    public class ListStoryAdminModel : PageModel
    {
        private readonly DO_ANContext _context;

        public ListStoryAdminModel(DO_ANContext context)
        {
            _context = context;
        }

        public List<DisasterStory> DisasterStory_ { get; set; } = new List<DisasterStory>();
        public int? DisasterIdFilter { get; set; }
        public List<int?> AvailableDisasterIds { get; set; } = new List<int?>();
        public List<(int? DisasterId, string DisasterName)> AvailableDisasters { get; set; } = new List<(int?, string)>();

        public async Task OnGetAsync(int? disasterIdFilter)
        {
            DisasterIdFilter = disasterIdFilter;

            // Populate available DisasterIds
            AvailableDisasters = await _context.Disasters
        .Select(d => new { DisasterId = (int?)d.DisasterId, d.DisasterName }) // Anonymous type for EF compatibility
        .Distinct()
        .ToListAsync()
        .ContinueWith(task => task.Result.Select(d => (d.DisasterId, d.DisasterName)).ToList()); // Convert to tuple list


            // Query DisasterStory with optional filtering
            IQueryable<DisasterStory> query = _context.DisasterStories
                .Include(s => s.User)
                .Include(s => s.Disaster)
                .OrderByDescending(s => s.Createdate);

            if (DisasterIdFilter.HasValue)
            {
                query = query.Where(s => s.DisasterId == DisasterIdFilter.Value);
            }

            DisasterStory_ = await query.ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var story = await _context.DisasterStories.FindAsync(id);
            if (story == null)
            {
                return NotFound();
            }

            _context.DisasterStories.Remove(story);
            await _context.SaveChangesAsync();

            return RedirectToPage("./ListStoryAdmin");
        }
    }
}
