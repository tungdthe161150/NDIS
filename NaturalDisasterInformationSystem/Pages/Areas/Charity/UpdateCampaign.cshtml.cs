using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NaturalDisasterInformationSystem.Models;
using Org.BouncyCastle.Utilities.Collections;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace NaturalDisasterInformationSystem.Pages.Areas.Charity
{
    [Authorize(Policy = "Charity")]

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

        public async Task<IActionResult> OnPostAsync(
    int CampaignId,
    string CampaignName,
    string Description,
    double GoalAmount,
    double RaisedAmount,
    DateTime EndDate,
    List<IFormFile>? Images,
    IFormFile? Images_QR)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                // Log l?i n?u c?n
                return Page();
            }

            var campaignToUpdate = _context.FundraisingCampaigns.FirstOrDefault(c => c.CampaignId == CampaignId);
            if (campaignToUpdate == null)
            {
                ModelState.AddModelError(string.Empty, "Không tìm th?y chi?n d?ch.");
                return Page();
            }

            // C?p nh?t các tr??ng d? li?u
            campaignToUpdate.CampaignName = CampaignName;
            campaignToUpdate.Description = Description;
            campaignToUpdate.GoalAmount = GoalAmount;
            campaignToUpdate.RaisedAmount = RaisedAmount;
            campaignToUpdate.EndDate = EndDate;

            // X? lý upload ?nh danh sách
            if (Images != null && Images.Count > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "campaigns");
                Directory.CreateDirectory(uploadsFolder);

                var imageNames = new List<string>();
                foreach (var image in Images)
                {
                    var fileName = Path.GetFileName(image.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    if (!System.IO.File.Exists(filePath))
                    {
                        await using var fileStream = new FileStream(filePath, FileMode.Create);
                        await image.CopyToAsync(fileStream);
                    }

                    imageNames.Add(fileName);
                }

                campaignToUpdate.ImgCam = string.Join(",", imageNames);
            }

            // X? lý upload ?nh QR
            if (Images_QR != null)
            {
                var uploadsFolderQR = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "QR");
                Directory.CreateDirectory(uploadsFolderQR);

                var imagePathQR = Path.Combine(uploadsFolderQR, Images_QR.FileName);
                if (!System.IO.File.Exists(imagePathQR))
                {
                    await using var stream = new FileStream(imagePathQR, FileMode.Create);
                    await Images_QR.CopyToAsync(stream);
                }

                campaignToUpdate.ImgQr = Images_QR.FileName;
            }

            // C?p nh?t d? li?u vào c? s? d? li?u
            _context.FundraisingCampaigns.Update(campaignToUpdate);
            await _context.SaveChangesAsync();

            TempData["UpdateCamMessage"] = "Cap nhat thanh cong.";
            return RedirectToPage("/Areas/Charity/DetailCampaign", new { dc_id = campaignToUpdate.CampaignId });
        }


    }
}
