using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.Areas.Admin.Blog
{
    [Authorize(Policy = "Admin")]
    public class CreateBlogModel : PageModel
    {
        private readonly DO_ANContext context;

        public CreateBlogModel(DO_ANContext context)
        {
            this.context = context;
        }
        [BindProperty]
        public Models.DisasterBlog DisasterBlog { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            DisasterBlog.CreatedAt = DateTime.UtcNow;


            context.DisasterBlogs.Add(DisasterBlog);
            await context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Them thanh cong!";

            return RedirectToPage("./ListBlogAdmin");
        }

        [HttpPost]
        public async Task<IActionResult> OnPostCreateBlogsAsync([FromBody] DisasterBlog blogs)
        {
            if (!ModelState.IsValid || context.DisasterBlogs == null || blogs == null)
            {

                return new JsonResult(new
                {
                    success = false,
                    message = "Invalid data"
                });
            }

            context.DisasterBlogs.Add(blogs);
            await context.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }
    }
}
