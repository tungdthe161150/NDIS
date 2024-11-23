using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;
using NaturalDisasterInformationSystem.Services;

namespace NaturalDisasterInformationSystem.Pages.Areas.Reports
{
    public class DetailModel : PageModel
    {
        private readonly DO_ANContext context;
        private readonly EmailService emailService;

        public DetailModel(DO_ANContext context,EmailService emailService)
        {
            this.context = context;
            this.emailService = emailService;
        }

        [BindProperty]
        public Report report{  get; set; }

        [BindProperty]
        public List<Report> reports { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? Stt { get; set; }


        public async Task<IActionResult> OnGet(int id)
        {
            report = context.Reports.Include(u => u.User)
                .Include(u => u.Campaign)
                .ThenInclude(u => u.Charity)
                .FirstOrDefault(r => r.CampaignId == id);

            if (report == null)
            {
                return NotFound("Báo cáo không tồn tại.");
            }

            // Lấy danh sách các báo cáo liên quan đến cùng CampaignId
            reports = context.Reports.Where(r => r.CampaignId == id).ToList();

            ViewData["Report"] = report;
            ViewData["ListReport"] = reports;

            return Page();
        }

        //SendNotificationAsync
        public async Task<IActionResult> OnPost(int id)
        {
            report = context.Reports
                .Include(u => u.User)
                .Include(u => u.Campaign)
                .ThenInclude(u => u.Charity)
                .FirstOrDefault(r => r.CampaignId == id);

            reports = context.Reports.Where(r => r.CampaignId == id).ToList();

            if (report == null || report.User == null || string.IsNullOrEmpty(report.User.Email))
            {
                return NotFound();
            }

            // Gửi email thông báo
            string emailSubject = "Thông báo xử lý báo cáo vi phạm";
            string emailBody = $"Chúng tôi đã nhận và xử lý báo cáo của bạn về chiến dịch: {(report.Campaign?.CampaignName ?? "Không có tên chiến dịch")}" +
                $". Trạng thái báo cáo hiện tại: Xử lý thành công.";

            try
            {
                // Kiểm tra xem _emailService có phải là null không
                if (emailService == null)
                {
                    TempData["ErrorMessage"] = "Dịch vụ gửi email không khả dụng.";
                    return Page();
                }

                Console.WriteLine(report.User.Email);

                // Gửi email
                await emailService.SendEmailAsync(report.User.Email, emailSubject, emailBody);

                // Cập nhật trạng thái của báo cáo thành "Xử lý thành công"
                report.Status = "Xử lý thành công";
                context.Reports.Update(report);
                await context.SaveChangesAsync();

                // Hiển thị thông báo thành công hoặc redirect
                TempData["SuccessMessage"] = "Gửi thông báo thành công và trạng thái đã được cập nhật.";

                return RedirectToPage("/Areas/Reports/Index");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                TempData["ErrorMessage"] = "Đã có lỗi xảy ra khi gửi thông báo.";
                return Page();
            }
        }
    }
}
