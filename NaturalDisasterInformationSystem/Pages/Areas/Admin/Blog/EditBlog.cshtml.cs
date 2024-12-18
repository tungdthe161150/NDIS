using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.Areas.Admin.Blog
{
    public class EditBlogModel : PageModel
    {
        private readonly DO_ANContext context;

        public EditBlogModel(DO_ANContext context)
        {
            this.context = context;
        }
        [BindProperty]
        public DisasterBlog Blog { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Blog = await context.DisasterBlogs.FindAsync(id);
            if (Blog == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            var blogToUpdate = await context.DisasterBlogs.FindAsync(Blog.BlogId);
            if (blogToUpdate == null)
            {
                return NotFound();
            }

            // Update the properties
            blogToUpdate.Title = Blog.Title;
            blogToUpdate.Content = Blog.Content;
            blogToUpdate.UpdatedAt = DateTime.Now;
            // Save changes to the database
            await context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Cap nhat thanh cong!";
            // Redirect to the ListBlog page
            return RedirectToPage("./ListBlogAdmin");
        }

    }
}
