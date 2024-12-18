using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.User
{
    public class ListBlogModel : PageModel
    {
        private readonly DO_ANContext context;

        public ListBlogModel(DO_ANContext context)
        {
            this.context = context;
        }

        public List<DisasterBlog> List { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        private const int PageSize = 6;

        public async Task<IActionResult> OnGetAsync(string? search, int pageNumber = 1)
        {
            IQueryable<DisasterBlog> blogsQuery = context.DisasterBlogs
                .OrderByDescending(f => f.BlogId);

            if (!string.IsNullOrEmpty(search))
            {
                blogsQuery = blogsQuery.Where(b => b.Title.ToLower().Contains(search.ToLower()));
            }
            int totalItems = await blogsQuery.CountAsync();

            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
            CurrentPage = pageNumber;

            var blogList = await blogsQuery.OrderByDescending(n => n.CreatedAt) 
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync(); 

            List = blogList;

            return Page();
        }
    }
}
