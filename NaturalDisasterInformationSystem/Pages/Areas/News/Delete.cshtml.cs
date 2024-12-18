using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.Areas.Admin.News
{
    [Authorize(Policy = "Admin")]

    public class DeleteModel : PageModel
    {
        private readonly DO_ANContext context;

        public DeleteModel(DO_ANContext context)
        {
            this.context = context;
        }

        [BindProperty]
        public Models.News News { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await context.News.FirstOrDefaultAsync(m => m.NewsId == id);

            if (news == null)
            {
                return NotFound();
            }
            else
            {
                News = news;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || context.News == null)
            {
                return BadRequest("ID không hợp lệ.");
            }

            var news = await context.News.FindAsync(id);

            if (news == null)
            {
                return NotFound("Không tìm thấy bản tin.");
            }
            else
            {
                try
                {
                    News = news;
                    context.News.Remove(News);
                    await context.SaveChangesAsync();

                    TempData["DeleteSuccess"] = "Tin tức đã được xóa thành công!";
                    return RedirectToPage("./Index");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Lỗi khi xóa bản tin: {ex.Message}");
                }
            }
        }
    }
}
