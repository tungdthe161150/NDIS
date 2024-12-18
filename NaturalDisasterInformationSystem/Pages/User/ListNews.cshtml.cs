using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.User
{
    public class ListNewsModel : PageModel
    {
        private readonly DO_ANContext context;

        public ListNewsModel(DO_ANContext context)
        {
            this.context = context;
        }

        public List<News> List { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        private const int PageSize = 6;

        public async Task<IActionResult> OnGetAsync(string? searchNews, int pageNumber = 1)
        {
            IQueryable<News> newsQuery = context.News
                .OrderByDescending(f => f.NewsId);

            if (!string.IsNullOrEmpty(searchNews))
            {
                newsQuery = newsQuery.Where(b => b.Title.ToLower().Contains(searchNews.ToLower()));
            }

            int totalItems = await newsQuery.CountAsync();

            // Tính tổng số trang
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
            CurrentPage = pageNumber;

            var newList = await newsQuery.OrderByDescending(n => n.PublishedAt) // Sắp xếp theo ngày đăng (mới nhất trước)
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            List = newList;

            return Page();
        }
    }
}
