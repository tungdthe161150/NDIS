using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.Account
{
    public class reset_passModel : PageModel
    {
        private readonly DO_ANContext _context;

        public reset_passModel(DO_ANContext context)
        {
            _context = context;
        }

        public NaturalDisasterInformationSystem.Models.User UserProfile { get; set; }

        [BindProperty]
        public string NewPassword { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost(int uid, string newPassword, string confirmPassword, long ts)
        {
            // Check if the timestamp is valid
            var currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (currentTimestamp - ts > 600) // 30 seconds validity
            {
                ModelState.AddModelError(string.Empty, "Lien ket doi mat khau da het han.Vui long thuc hien lai.");
                return Page();
            }
            // Retrieve the existing user
            var user = _context.Users.FirstOrDefault(u => u.UserId == uid);
            if (user == null)
            {
                return NotFound();
            }
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8)
            {
                ModelState.AddModelError(nameof(newPassword), "Mat khau phai co 8 ki tu..");
                return Page();
            }
            // Check if the new passwords match
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Hai mat khau khong trung nhau .Vui long nhap lai.");
                return Page();
            }

            // Hash the new password before saving
            user.PasswordHash = HashPassword(newPassword);

            // Save changes to the database
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "?ã x?y ra l?i khi c?p nh?t m?t kh?u: " + ex.Message);
                return Page();
            }

            // Redirect to the profile page (or wherever you want)
            return RedirectToPage("/user/Home");
        }

        private string HashPassword(string password)
        {
            // Hash the password with a work factor (e.g., 10)
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(10));
        }
    }
}
