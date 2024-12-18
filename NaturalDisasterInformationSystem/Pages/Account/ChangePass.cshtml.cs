using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization; // Ensure you have this using directive for password hashing

namespace NaturalDisasterInformationSystem.Pages.Account
{
    [Authorize]

    public class ChangePassModel : PageModel
    {
        private readonly DO_ANContext _context;

        public ChangePassModel(DO_ANContext context)
        {
            _context = context;
        }

        // Property to hold user profile
        public NaturalDisasterInformationSystem.Models.User UserProfile { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost(int uid, string pass_new, string pass_old)
        {
            // Retrieve the existing user
            var user = _context.Users.FirstOrDefault(u => u.UserId == uid);
            if (user == null)
            {
                return NotFound();
            }

            // Verify the old password
            if (!VerifyPassword(pass_old, user.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Mat khau cu không dung.");
                return Page();
            }
           
            if (string.IsNullOrWhiteSpace(pass_new) || pass_new.Length < 8)
            {
                ModelState.AddModelError(nameof(pass_new), "Mat khau phai co it nhat 8 ky tu.");
                return Page();
            }

            // Hash the new password before saving
            user.PasswordHash = HashPassword(pass_new);

            // Save changes to the database
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating the password: " + ex.Message);
                return Page();
            }

            // Redirect to the profile page (or wherever you want)
            return RedirectToPage("/user/Home"); 

        }

        private bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            // Ensure neither password is null or empty
            if (string.IsNullOrEmpty(plainPassword) || string.IsNullOrEmpty(hashedPassword))
            {
                throw new ArgumentException("Password cannot be null or empty.");
            }

            // Check if the hashed password matches the plain password
            return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
        }


        private string HashPassword(string password)
        {
            // Hash the password with a work factor (e.g., 10)
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(10));
        }
    }
}
