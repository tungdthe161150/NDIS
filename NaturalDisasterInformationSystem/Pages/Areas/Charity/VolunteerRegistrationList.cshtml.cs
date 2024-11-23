using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace NaturalDisasterInformationSystem.Pages.Areas.Charity
{
    public class VolunteerRegistrationListModel : PageModel
    {
        private readonly DO_ANContext _context;

        // Property to hold the list of volunteer registrations
        public List<VolunteerHistory> VolunteerRegistrations { get; set; }

        public VolunteerRegistrationListModel(DO_ANContext context)
        {
            _context = context;
        }

        public void OnGet(int cv_id)
        {
            VolunteerRegistrations = _context.VolunteerHistories.Include(u=>u.User).Include(c=>c.Event)
                .Where(c => c.EventId == cv_id)
                .ToList();
        }
        public IActionResult OnPost(string status ,int ProfileId)
        {

            var Volun = _context.VolunteerHistories.FirstOrDefault(c => c.ProfileId == ProfileId);
            if (Volun != null)
            {
                // Update fields
                Volun.Status = "hoàn thành";
            

                // **Mark entity as updated**
                _context.VolunteerHistories.Update(Volun);

                // Save changes to the database
                _context.SaveChanges();
            }

            return RedirectToPage( new { cv_id = Volun.EventId });
        }
    }
}
