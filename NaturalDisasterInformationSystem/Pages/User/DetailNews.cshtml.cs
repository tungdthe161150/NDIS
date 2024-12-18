using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.User
{
    public class DetailNewsModel : PageModel
    {
        private readonly DO_ANContext context;

        public DetailNewsModel(DO_ANContext context)
        {
            this.context = context;
        }

        [BindProperty]
        public News news { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            news = await context.News.FirstOrDefaultAsync(n => n.NewsId == id);

            if (news == null)
            {
                return NotFound();
            }
            ViewData["News"] = news;

            return Page();
        }
    }
}
