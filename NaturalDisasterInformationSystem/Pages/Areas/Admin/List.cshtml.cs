using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.Areas.Admin
{
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
            Users = context.Users.ToList();
            return Page();
        }
    }
}
