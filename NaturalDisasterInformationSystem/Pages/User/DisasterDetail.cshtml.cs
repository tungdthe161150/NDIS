using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NaturalDisasterInformationSystem.Pages.User
{
    public class DisasterDetailModel : PageModel
    {
        private readonly DO_ANContext _context; // Your DbContext

        public DisasterDetailModel(DO_ANContext context)
        {
            _context = context;
        }

        public Disaster Disaster { get; set; } = null!;
        public List<DisasterStory> DisasterStories { get; set; } = new List<DisasterStory>();
        public List<DisasterStory> DisasterStoriesU { get; set; } = new List<DisasterStory>();
        public List<FundraisingCampaign> FundraisingCampaignsU { get; set; }

        public async Task OnGetAsync(int dd_id)
        {
            Disaster = await _context.Disasters.FindAsync(dd_id);
            DisasterStories = await _context.DisasterStories
                                 .Where(ds => ds.DisasterId == dd_id)
                                 .Include(ds => ds.User)
                                 .OrderByDescending(ds => ds.Createdate) 
                                 .ToListAsync();
            DisasterStoriesU = await _context.DisasterStories.ToListAsync();
            FundraisingCampaignsU = await _context.FundraisingCampaigns.Include(c => c.Charity).OrderByDescending(f => f.CampaignId).Where(h=>h.Hidden=="1").Where(u=>u.DisasterId==dd_id).ToListAsync();


        }

        public async Task<IActionResult> OnPostAsync(string content, int disasterId, string userId, IFormFile? video, List<IFormFile>? Images)
        {
            if (!ModelState.IsValid)
            {
                // Log the validation errors for debugging
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                return Page(); // Return with errors
            }

            var disasterStory = new DisasterStory
            {
                Content = content,
                DisasterId = disasterId,
                UserId = int.Parse(userId),
                Createdate = DateTime.Now
            };

            // Save video if uploaded
            if (video != null)
            {
                var videoFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "videos");
                Directory.CreateDirectory(videoFolder); // Ensure the directory exists

                var videoPath = Path.Combine(videoFolder, video.FileName); // Create the full video path

                using (var stream = new FileStream(videoPath, FileMode.Create))
                {
                    await video.CopyToAsync(stream);
                }
                disasterStory.Video = video.FileName; // Store only the file name or path as needed
            }

            // Save multiple images if uploaded
            if (Images != null && Images.Count > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "images");
                Directory.CreateDirectory(uploadsFolder); // Ensure the directory exists

                var imageNames = new List<string>(); // Store all image names

                foreach (var image in Images)
                {
                    var fileName = Path.GetFileName(image.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }

                    imageNames.Add(fileName); // Add the file name to the list
                }

                // Save all image names as a single comma-separated string
                disasterStory.Img = string.Join(",", imageNames);
            }



            await _context.DisasterStories.AddAsync(disasterStory);
            await _context.SaveChangesAsync();

            // Redirect to the same page or wherever you want
            return RedirectToPage(new { dd_id = disasterId });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int storyId)
        {
            var story = await _context.DisasterStories.FindAsync(storyId);

            if (story == null || story.UserId.ToString() != HttpContext.Session.GetString("UserId"))
            {
                return NotFound(); // Không tìm th?y ho?c không có quy?n
            }

            _context.DisasterStories.Remove(story);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { dd_id = story.DisasterId });
        }

        public async Task<IActionResult> OnPostUpdateStoryAsync(int storyId, string content, IFormFile? video, List<IFormFile>? images)
        {
            var story = await _context.DisasterStories.FindAsync(storyId);

            if (story == null || story.UserId.ToString() != HttpContext.Session.GetString("UserId"))
            {
                return NotFound(); // Không tìm th?y ho?c không có quy?n
            }

            // C?p nh?t n?i dung
            story.Content = content;

            // N?u có video m?i, l?u l?i
            if (video != null)
            {
                var videoPath = Path.Combine("wwwroot/uploads/videos", video.FileName);
                using (var stream = new FileStream(videoPath, FileMode.Create))
                {
                    await video.CopyToAsync(stream);
                }
                story.Video = video.FileName;
            }

            // N?u có hình ?nh m?i, l?u l?i
            if (images != null && images.Count > 0)
            {
                var imageNames = new List<string>();
                foreach (var image in images)
                {
                    var imagePath = Path.Combine("wwwroot/uploads/images", image.FileName);
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                    imageNames.Add(image.FileName);
                }
                story.Img = string.Join(",", imageNames);
            }

            _context.DisasterStories.Update(story);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { dd_id = story.DisasterId });
        }


    }
}
