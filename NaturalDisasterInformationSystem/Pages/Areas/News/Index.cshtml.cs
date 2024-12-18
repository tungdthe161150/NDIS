using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.Areas.Admin.News
{
    [Authorize(Policy = "Admin")]

    public class IndexModel : PageModel
    {
        private readonly DO_ANContext context;

        public IndexModel(DO_ANContext context)
        {
            this.context = context;
        }

        [BindProperty]
        public List<Models.News> News { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var news = await context.News.ToListAsync();
            News = news;
            return Page();
        }
    }
}
