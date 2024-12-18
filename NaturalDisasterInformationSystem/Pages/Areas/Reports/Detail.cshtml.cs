using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;
using NaturalDisasterInformationSystem.Services;

namespace NaturalDisasterInformationSystem.Pages.Areas.Reports
{
    [Authorize(Policy = "Admin")]

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
            var reason = reports.Select(x => x.Reason).Distinct().ToList();
            string reasonList = string.Join("<li>", reason);
            reasonList = $"<ul><li>{reasonList}</li></ul>";


            if (report == null || report.User == null || string.IsNullOrEmpty(report.User.Email))
            {
                return NotFound();
            }

            // Gửi email thông báo
            string emailSubject = "Thông báo xử lý báo cáo vi phạm";
            string emailBody = $@"
<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f9f9f9;
            color: #333;
            line-height: 1.6;
            padding: 20px;
        }}
        .container {{
            max-width: 600px;
            margin: 0 auto;
            background: white;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
            padding: 20px;
        }}
        h2 {{
            color: #4CAF50;
            text-align: center;
        }}
        p {{
            margin: 15px 0;
        }}
        .footer {{
            margin-top: 20px;
            font-size: 0.9em;
            color: #777;
            text-align: center;
        }}
        .highlight {{
            font-weight: bold;
            color: #4CAF50;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <h2>Thông báo về báo cáo vi phạm</h2>
        <p>Kính gửi <span class=""highlight"">{report.User.FullName}</span>,</p>


        <p>Chúng tôi muốn thông báo rằng chúng tôi đã nhận được một báo cáo vi phạm liên quan đến bạn trong hệ thống của chúng tôi. Chi tiết về báo cáo như sau:</p>

        <ul>
            <li><span class=""highlight"">Tên chiến dịch:</span> {report.Campaign?.CampaignName ?? "Không có tên chiến dịch"}</li>
            <li><span class=""highlight"">Ngày báo cáo:</span> {report.CreatedAt}</li>
        </ul>

        <p>Chiến dịch của bạn đã bị báo cáo với nội dung: <span class=""highlight"">{reasonList}</span>. Chúng tôi rất coi trọng sự công bằng và minh bạch trong quy trình này và đang tiến hành xem xét báo cáo một cách nghiêm túc.</p>

        <p>Nếu bạn có bất kỳ thông tin hoặc giải thích nào liên quan đến tình huống này, xin vui lòng phản hồi lại email này 
để chúng tôi có thể xem xét ý kiến của bạn. Chúng tôi cam kết sẽ xem xét tất cả các ý kiến một cách công bằng và khách quan.</p>

        <p>Chúng tôi rất cảm ơn bạn đã hợp tác và chờ đợi phản hồi từ bạn.</p>

        <div class=""footer"">
            <p>Trân trọng,<br>
            <span class=""highlight"">NDIS</span></p>
        </div>
    </div>
</body>
</html>
";


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
                foreach (Report report in reports)
                {
                    // Cập nhật trạng thái của báo cáo thành "Xử lý thành công"
                    report.Status = "xy ly thanh cong";
                    context.Reports.Update(report);
                }


            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                TempData["ErrorMessage"] = "Đã có lỗi xảy ra khi gửi thông báo.";
                return Page();
            }
            await context.SaveChangesAsync();

            // Hiển thị thông báo thành công hoặc redirect
            TempData["SuccessMessage"] = "Gửi thông báo thành công và trạng thái đã được cập nhật.";

            return RedirectToPage("/Areas/Reports/Index");


        }
    }
}
