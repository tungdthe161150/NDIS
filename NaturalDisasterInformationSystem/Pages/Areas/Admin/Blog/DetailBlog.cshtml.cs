using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.Areas.Admin.Blog
{
    public class DetailBlogModel : PageModel
    {
        private readonly DO_ANContext context;

        public DetailBlogModel(DO_ANContext context)
        {
            this.context = context;
        }
        public DisasterBlog Blog { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Fetch the blog details by id from the database
            Blog = await context.DisasterBlogs.FindAsync(id);
            if (Blog == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
