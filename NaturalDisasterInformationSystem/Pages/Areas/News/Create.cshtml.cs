using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.Areas.News
{
    public class CreateModel : PageModel
    {
        private readonly DO_ANContext context;

        public CreateModel(DO_ANContext context)
        {
            this.context = context;
        }

        [BindProperty]
        public Models.News News { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || context.News == null || News == null)
            {
                return Page();
            }

            context.News.Add(News);
            await context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        [HttpPost]
        public async Task<IActionResult> OnPostCreateNewsAsync([FromBody] Models.News news)
        {
            if (!ModelState.IsValid || context.News == null || news == null)
            {

                return new JsonResult(new
                {
                    success = false,
                    message = "Invalid data"
                });
            }

            context.News.Add(news);
            await context.SaveChangesAsync();

            return new JsonResult(new{success = true});
        }
    }
}
