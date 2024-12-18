using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.Areas.Admin.Blog
{
    [Authorize(Policy = "Admin")]
    public class DeleteBlogModel : PageModel
    {
        private readonly DO_ANContext context;

        public DeleteBlogModel(DO_ANContext context)
        {
            this.context = context;
        }
        [BindProperty]
        public Models.DisasterBlog DisasterBlog { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogs = await context.DisasterBlogs.FirstOrDefaultAsync(m => m.BlogId == id);

            if (blogs == null)
            {
                return NotFound();
            }
            else
            {
                DisasterBlog = blogs;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || context.DisasterBlogs == null)
            {
                return BadRequest("ID không hợp lệ.");
            }

            var blogs = await context.DisasterBlogs.FindAsync(id);

            if (blogs == null)
            {
                return NotFound("Không tìm thấy bản tin.");
            }
            else
            {
                try
                {
                    DisasterBlog = blogs;
                    context.DisasterBlogs.Remove(DisasterBlog);
                    await context.SaveChangesAsync();

                    TempData["DeleteSuccess"] = "Tin tức đã được xóa thành công!";
                    return RedirectToPage("./ListBlogAdmin");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Lỗi khi xóa bản tin: {ex.Message}");
                }
            }
        }
    }
}
