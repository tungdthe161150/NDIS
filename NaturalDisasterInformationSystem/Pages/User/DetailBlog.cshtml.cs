using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.User
{
    public class DetailBlogModel : PageModel
    {
        private readonly DO_ANContext context;

        public DetailBlogModel(DO_ANContext context)
        {
            this.context = context;
        }

        [BindProperty]
        public DisasterBlog blog { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            blog = await context.DisasterBlogs.FirstOrDefaultAsync(n => n.BlogId == id);

            if (blog == null)
            {
                return NotFound();
            }
            ViewData["DisasterBlogs"] = blog;

            return Page();
        }
    }
}
