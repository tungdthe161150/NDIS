using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;
using System.Collections.Generic;

namespace NaturalDisasterInformationSystem.Pages.Areas.Charity
{
    public class VolunteerRegistrationDetailModel : PageModel
    {
        private readonly DO_ANContext _context;

        public VolunteerRegistrationDetailModel(DO_ANContext context)
        {
            _context = context;
        }

        public NaturalDisasterInformationSystem.Models.User UserDetail { get; set; }
        public VolunteerHistory? VolunteerDetail { get; set; }
        public List<VolunteerHistory> VolunteerHistoryList { get; set; } = new List<VolunteerHistory>();

        public void OnGet(int vrd_id)
        {
            VolunteerDetail = _context.VolunteerHistories
                .Include(v => v.User)
                .FirstOrDefault(v => v.ProfileId == vrd_id);

            if (VolunteerDetail != null)
            {
                UserDetail = VolunteerDetail.User;
                VolunteerHistoryList = _context.VolunteerHistories.Include(v => v.Event)
                    .Where(v => v.UserId == UserDetail.UserId)
                    .ToList();
            }
        }
        public IActionResult OnPost(string status, int ProfileId)
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

            return RedirectToPage("VolunteerRegistrationList",new { cv_id = Volun.EventId });
        }
        public IActionResult OnPostTuChoi(string status, int ProfileId)
        {

            var Volun = _context.VolunteerHistories.FirstOrDefault(c => c.ProfileId == ProfileId);
            if (Volun != null)
            {
                // Update fields
                Volun.Status = "tham gia";


                // **Mark entity as updated**
                _context.VolunteerHistories.Update(Volun);

                // Save changes to the database
                _context.SaveChanges();
            }

            return RedirectToPage("VolunteerRegistrationList",new { cv_id = Volun.EventId });
        }
    }
}
