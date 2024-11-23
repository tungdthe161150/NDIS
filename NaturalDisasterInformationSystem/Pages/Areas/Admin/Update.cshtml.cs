using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.Areas.Admin
{
    public class UpdateModel : PageModel
    {
        private readonly DO_ANContext context;

        public UpdateModel(DO_ANContext context)
        {
            this.context = context;
        }

        [BindProperty]
        public NaturalDisasterInformationSystem.Models.User User { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            User = await context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (User == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userInDb = await context.Users.FirstOrDefaultAsync(u => u.UserId == User.UserId);
            if (userInDb == null)
            {
                return NotFound();
            }

            // Cập nhật các thuộc tính
            userInDb.Username = User.Username;
            userInDb.Email = User.Email;
            userInDb.FullName = User.FullName;
            userInDb.PhoneNumber = User.PhoneNumber;
            userInDb.RoleId = User.RoleId;
            userInDb.Active = User.Active;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(User.UserId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("List");
        }

        private bool UserExists(int id)
        {
            return context.Users.Any(e => e.UserId == id);
        }
    }
}
