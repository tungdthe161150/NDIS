using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.Areas.Admin.News
{
    public class DetailModel : PageModel
    {
        private readonly DO_ANContext context;

        public DetailModel(DO_ANContext context)
        {
            this.context = context;
        }

        [BindProperty]
        public Models.News news { get; set; }

        public async Task<IActionResult> OnGet(int id)
        {
            news = context.News.FirstOrDefault(n => n.NewsId == id);
            ViewData["News"] = news;
            return Page();
        }
    }
}
