using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.Areas.Admin.Blog
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
        public List<Models.DisasterBlog> DisasterBlogs { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var blogs = await context.DisasterBlogs.ToListAsync();
            DisasterBlogs = blogs;
            return Page();
        }
    }
}
