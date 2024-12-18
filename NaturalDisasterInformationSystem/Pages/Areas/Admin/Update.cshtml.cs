using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;
using System.Data;

namespace NaturalDisasterInformationSystem.Pages.Areas.Admin
{
    [Authorize(Policy = "Admin")]

    public class UpdateModel : PageModel
    {
        private readonly DO_ANContext context;

        public UpdateModel(DO_ANContext context)
        {
            this.context = context;
        }

        [BindProperty]
        public NaturalDisasterInformationSystem.Models.User User { get; set; } = default!;


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Tìm người dùng theo ID và kèm theo thông tin Role
            var user = await context.Users
                .Include(r => r.Role) // Lấy thông tin role của người dùng
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound();
            }
            User = user;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("User.PasswordHash");
            ModelState.Remove("User.Role");

            if (!ModelState.IsValid)
            {
                var errors = ModelState
        .Where(x => x.Value.Errors.Count > 0)
        .Select(x => new { x.Key, x.Value.Errors })
        .ToList();

                foreach (var error in errors)
                {
                    Console.WriteLine($"Key: {error.Key}, Error: {string.Join(", ", error.Errors.Select(e => e.ErrorMessage))}");
                }

                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                return Page();
            }

            var userInDb = await context.Users.FindAsync(User.UserId);
            if (userInDb == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản cần cập nhật.";
                return RedirectToPage("./List");
            }

            try
            {
                // Chỉ cập nhật trường RoleId
                userInDb.RoleId = User.RoleId;
                userInDb.Active = User.Active;
                context.Users.Update(userInDb);
                await context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật quyền tài khoản thành công!";
                return RedirectToPage("./List");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Đã xảy ra lỗi: {ex.Message}";
                return Page();
            }
        }

        private bool UserExists(int id)
        {
            return (context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
