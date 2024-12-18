using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.Areas.Admin
{
    [Authorize(Policy = "Admin")]

    public class ListModel : PageModel
    {
        private readonly DO_ANContext context;

        public List<NaturalDisasterInformationSystem.Models.User> Users { get; set; }

        public ListModel(DO_ANContext context)
        {
            this.context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Users = context.Users.Where(u => u.RoleId != 1).ToList();
            return Page();
        }
    }
}
