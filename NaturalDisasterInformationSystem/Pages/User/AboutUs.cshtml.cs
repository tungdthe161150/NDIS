using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.User
{
    public class AboutUsModel : PageModel
    {
        private readonly DO_ANContext context;

        public AboutUsModel(DO_ANContext context)
        {
            this.context = context;
        }
        public void OnGet()
        {
        }
    }
}
