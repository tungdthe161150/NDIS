using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NaturalDisasterInformationSystem.Pages.Areas.Charity
{
    public class Update_InformationModel : PageModel
    {
        private readonly DO_ANContext _context;

        [BindProperty]
        public ProjectInformation NewInformation { get; set; } = new ProjectInformation();

        public List<ProjectInformation> ProjectInformationListU { get; set; }
        public List<ProjectInformation> ProjectInformationList { get; set; }
        public List<ProjectInformation> ProjectInformationList1 { get; set; }
        public VolunteerHistory VolunteerHistory { get; set; }
        public FundraisingCampaign Campaign { get; set; }

        public Update_InformationModel(DO_ANContext context)
        {
            _context = context;
        }

        public void OnGet(int pi_id, int vh_id)
        {
            VolunteerHistory = _context.VolunteerHistories.FirstOrDefault(e => e.UserId == vh_id);
            Campaign = _context.FundraisingCampaigns.FirstOrDefault(c => c.CampaignId == pi_id);
            if (VolunteerHistory != null)
            {
                ProjectInformationListU = _context.ProjectInformations.Include(v=>v.Profile)
                    .Where(p => p.CampaignId == pi_id && p.ProfileId == VolunteerHistory.ProfileId)
                    .OrderByDescending(d => d.CreateDate)
                    .ToList();
            }
            else
            {
                // Handle the case where VolunteerHistory is null
                // Maybe return an empty list or log an error
                ProjectInformationListU = new List<ProjectInformation>();
            }

            ProjectInformationList = _context.ProjectInformations.Where(p => p.CampaignId == pi_id).Include(u=>u.Profile).ThenInclude(profile => profile.User).OrderByDescending(d=>d.CreateDate).ToList();
            ProjectInformationList1 = _context.ProjectInformations.Where(p => p.CampaignId == pi_id).Where(s=>s.Status=="1").Include(u=>u.Profile).ThenInclude(profile => profile.User).OrderByDescending(d => d.CreateDate).ToList();
        }

        [HttpPost]
        public async Task<IActionResult> OnPostAsync(List<IFormFile>? video, List<IFormFile>? images, string content, int profileId, int campaignId,int charityid,string Status)
        {
            if (string.IsNullOrEmpty(content) || profileId == 0 || campaignId == 0)
            {
                ModelState.AddModelError("", "Missing required fields.");
                return Page(); // Return to page with validation errors
            }
            var ca = _context.FundraisingCampaigns.FirstOrDefault(c=>c.CampaignId==campaignId);
            if (charityid == ca.CharityId)
            {
                Status = "1";
            }
            else
            {
                Status = "0";
            }
            var addInfo = new ProjectInformation
            {
                Content = content,
                Status = Status,
                ProfileId = profileId,
                CampaignId = campaignId,
                CreateDate = DateTime.Now
            };

           /* // Save videos if uploaded
            if (video != null && video.Count > 0)
            {
                var videoFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "projectInformation");
                Directory.CreateDirectory(videoFolder); // Ensure the directory exists

                var videoNames = new List<string>(); // List to store video names

                foreach (var vid in video)
                {
                    var videoPath = Path.Combine(videoFolder, vid.FileName);

                    using (var stream = new FileStream(videoPath, FileMode.Create))
                    {
                        await vid.CopyToAsync(stream);
                    }
                    videoNames.Add(vid.FileName); // Store only the file name
                }

                // Save all video names as a single comma-separated string
                addInfo.Video = string.Join(",", videoNames);
            }*/

            // Save images if uploaded
            if (images != null && images.Count > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "projectInformation");
                Directory.CreateDirectory(uploadsFolder); // Ensure the directory exists

                var imageNames = new List<string>();

                foreach (var image in images)
                {
                    var fileName = Path.GetFileName(image.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }

                    imageNames.Add(fileName);
                }

                // Save all image names as a single comma-separated string
                addInfo.Img = string.Join(",", imageNames);
            }

            // Save to database
            _context.ProjectInformations.Add(addInfo);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { pi_id = addInfo.CampaignId, vh_id = addInfo.ProfileId });
        }

        [HttpPost]
        public async Task<IActionResult> OnPostUpdateAsync(int projectId, string content, List<IFormFile>? video, List<IFormFile>? images,int uid)
        {
            var info = await _context.ProjectInformations.FindAsync(projectId);

            if (info == null)
            {
                ModelState.AddModelError("", "Project Information not found.");
                return Page();
            }

            // Update content and status
            info.Content = content;
            info.Status = "0"; // Modify status as needed
            info.CreateDate = DateTime.Now;

            // Handle video update
            if (video != null && video.Count > 0)
            {
                var videoFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "projectInformation");
                Directory.CreateDirectory(videoFolder); // Ensure the directory exists

                var videoNames = new List<string>();

                foreach (var vid in video)
                {
                    var videoPath = Path.Combine(videoFolder, vid.FileName);
                    using (var stream = new FileStream(videoPath, FileMode.Create))
                    {
                        await vid.CopyToAsync(stream);
                    }
                    videoNames.Add(vid.FileName);
                }

                info.Video = string.Join(",", videoNames); // Update the video field
            }

            // Handle image update
            if (images != null && images.Count > 0)
            {
                var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "projectInformation");
                Directory.CreateDirectory(imageFolder); // Ensure the directory exists

                var imageNames = new List<string>();

                foreach (var image in images)
                {
                    var imagePath = Path.Combine(imageFolder, image.FileName);
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                    imageNames.Add(image.FileName);
                }

                info.Img = string.Join(",", imageNames); // Update the image field
            }

            // Save changes
            _context.ProjectInformations.Update(info);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { pi_id = info.CampaignId, vh_id = uid }); // Redirect to the same page after deletion
        }


        // Delete a ProjectInformation entry
        [HttpPost]
        public async Task<IActionResult> OnPostDeleteAsync(int id,int uid)
        {
            var info = await _context.ProjectInformations.FindAsync(id);

            if (info == null)
            {
                ModelState.AddModelError("", "Project Information not found.");
                return Page();
            }

            _context.ProjectInformations.Remove(info);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { pi_id = info.CampaignId, vh_id = uid}); // Redirect to the same page after deletion
        } 
        [HttpPost]
        public async Task<IActionResult> OnPostApproveAsync(int projectId)
        {
            var info = await _context.ProjectInformations.FindAsync(projectId);

            if (info == null)
            {
                ModelState.AddModelError("", "Project Information not found.");
                return Page();
            }
            info.Status = "1";
            _context.ProjectInformations.Update(info);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { pi_id = info.CampaignId, vh_id = info.ProfileId }); 
        }
        [HttpPost]
        public async Task<IActionResult> OnPostUnApproveAsync(int projectId, int uid)
        {
            var info = await _context.ProjectInformations.FindAsync(projectId);

            if (info == null)
            {
                ModelState.AddModelError("", "Project Information not found.");
                return Page();
            }
            info.Status = "0";
            _context.ProjectInformations.Update(info);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { pi_id = info.CampaignId, vh_id = uid });
        }
    }
}
