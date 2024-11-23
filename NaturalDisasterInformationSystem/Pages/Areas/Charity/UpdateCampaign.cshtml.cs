using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NaturalDisasterInformationSystem.Models;
using Org.BouncyCastle.Utilities.Collections;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace NaturalDisasterInformationSystem.Pages.Areas.Charity
{
    public class UpdateCampaignModel : PageModel
    {
        private readonly DO_ANContext _context;

        public UpdateCampaignModel(DO_ANContext context)
        {
            _context = context;
        }

        public FundraisingCampaign Campaign { get; set; }
       

        public IActionResult OnGet(int ud_id)
        {
            Campaign = _context.FundraisingCampaigns.FirstOrDefault(c => c.CampaignId == ud_id);
            if (Campaign == null)
            {
                return NotFound();
            }
            return Page();
        }

        public IActionResult OnPost(int CampaignId,string CampaignName,string Description,double GoalAmount,double RaisedAmount,DateTime EndDate, List<IFormFile>? Images,IFormFile? Images_QR)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                // Log errors or inspect them
                return Page();
            }


            var campaignToUpdate = _context.FundraisingCampaigns.FirstOrDefault(c => c.CampaignId == CampaignId);
            if (campaignToUpdate != null)
            {
                // Update fields
                campaignToUpdate.CampaignName = CampaignName;
                campaignToUpdate.Description = Description;
                campaignToUpdate.GoalAmount = GoalAmount;
                campaignToUpdate.RaisedAmount = RaisedAmount;
                campaignToUpdate.EndDate = EndDate;
                if (Images != null && Images.Count > 0)
                {
                    var imageNames = new List<string>();
                    foreach (var image in Images)
                    {
                        var imagePath = Path.Combine("wwwroot/uploads/campaigns", image.FileName);
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                             image.CopyToAsync(stream);
                        }
                        imageNames.Add(image.FileName);
                    }
                    campaignToUpdate.ImgCam = string.Join(",", imageNames);
                }
                if (Images_QR != null)
                {
                    var uploadsFolderQR = Path.Combine("wwwroot/uploads/QR");
                    Directory.CreateDirectory(uploadsFolderQR); // Ensure the folder exists

                    var imagePathQR = Path.Combine(uploadsFolderQR, Images_QR.FileName);
                    using (var stream = new FileStream(imagePathQR, FileMode.Create))
                    {
                        Images_QR.CopyToAsync(stream).Wait(); // Wait for async completion
                    }

                    // Save the file name to the ImgQr field in the database
                    campaignToUpdate.ImgQr = Images_QR.FileName;
                }

                // **Mark entity as updated**
                _context.FundraisingCampaigns.Update(campaignToUpdate);

                // Save changes to the database
                _context.SaveChanges();
            }

            return RedirectToPage("/Areas/Charity/DetailCampaign", new { dc_id = campaignToUpdate.CampaignId });
        }

    }
}
