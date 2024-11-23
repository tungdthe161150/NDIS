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

        public async Task<IActionResult> OnGetAsync(int pageNumber = 1)
        {
            // Đếm tổng số bản ghi
            int totalItems = await context.News.CountAsync();

            // Tính tổng số trang
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
            CurrentPage = pageNumber;

            var newList = context.News.OrderByDescending(n => n.PublishedAt) // Sắp xếp theo ngày đăng (mới nhất trước)
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            List = newList;

            return Page();
        }
    }
}
