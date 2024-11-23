using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.Areas.Admin.News
{
    public class UpdateModel : PageModel
    {
        private readonly DO_ANContext context;

        public UpdateModel(DO_ANContext context)
        {
            this.context = context;
        }

        [BindProperty]
        public Models.News News { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || context.News == null)
            {
                return NotFound();
            }

            var newList = await context.News.FirstOrDefaultAsync(u => u.NewsId == id);

            if (newList == null)
            {
                return NotFound();
            }

            News = newList;
            ViewData["News"] = News;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToPage("./Index");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var newInDb = context.News.Find(id);

            if (newInDb == null)
            {
                return NotFound();
            }

            // Cập nhật các thuộc tính
            newInDb.Title = News.Title;
            newInDb.Content = News.Content;
            newInDb.Source = News.Source;
            newInDb.PublishedAt = News.PublishedAt;

            News = newInDb;

            ViewData["News"] = News;

            context.News.Update(newInDb);
            context.SaveChanges();

            TempData["UpdateSuccess"] = "Cập nhật thành công!";

            return RedirectToPage("./Index");
        }
    }
}
