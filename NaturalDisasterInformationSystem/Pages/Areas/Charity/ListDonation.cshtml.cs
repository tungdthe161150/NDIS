using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;
using OfficeOpenXml;
using System.Globalization;


namespace NaturalDisasterInformationSystem.Pages.Areas.Charity
{
    public class ListDonationModel : PageModel
    {
        private readonly DO_ANContext context;

        public ListDonationModel(DO_ANContext context)
        {
            this.context = context;
        }

        [BindProperty]
        public List<FundraisingDonation> Donations { get; set; }
        public FundraisingCampaign FundraisingCampaigns { get; set; }


        [BindProperty]
        public List<DonationViewModel> PreviewData { get; set; }
        public List<RevExpDonation> RevExpDonations { get; set; }
        [BindProperty]
        public RevExpDonation NewRevExpDonation { get; set; }
        public List<ImgDonation> ImgDonations { get; set; }

        public void OnGet(int c_id)
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (int.TryParse(userId, out int id))
            {
                // Th?c hi?n truy v?n n?u chuy?n ??i thành công
                var donations = context.FundraisingDonations
                    .Where(x => x.CampaignId == c_id)
                    .ToList();
                Donations = donations;
            }
            else
            {
                // X? lý khi không l?y ???c UserId ho?c UserId không h?p l?
                Console.WriteLine("UserId không t?n t?i ho?c không h?p l?.");
            }

            ViewData["Donations"] = Donations;
            FundraisingCampaigns = context.FundraisingCampaigns.FirstOrDefault(c=>c.CampaignId==c_id);
            RevExpDonations = context.RevExpDonations
                .Where(c => c.CamId == c_id)
                .Include(c => c.Cam)
                .ToList();

            ImgDonations =  context.ImgDonations
            .Where(img => img.CamId == c_id)
            .OrderByDescending(img => img.CreateDate) // Optional: Order by the latest uploaded
            .ToList();
        }

        //Khoi 3
        public async Task<IActionResult> OnPostAddImgFileAsync(IFormFile file, int CamId,string Content)
        {
            if (file != null && file.Length > 0)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "donation", file.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Create ImgDonation object and save to the database
                var imgDonation = new ImgDonation
                {
                    CreateDate = DateTime.Now,
                    File = file.FileName,  // You can store the file name or the full path, depending on your needs
                    CamId = CamId,
                    Content= Content
                };

                context.ImgDonations.Add(imgDonation);
                await context.SaveChangesAsync();
            }

            return RedirectToPage(new { c_id = CamId }); // Redirect after saving the image
        }

        //khoi 2
        // Delete handler(same as before)
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var donation = await context.RevExpDonations.FindAsync(id);
            if (donation != null)
            {
                context.RevExpDonations.Remove(donation);
                await context.SaveChangesAsync();
            }

            return RedirectToPage(new { c_id = donation.CamId }); // Refresh the page with the same c_id
        }
        public async Task<IActionResult> OnPostEditDonationAsync(int RevExpId, string Des, double Revenue, double Expenditure, int CamId)
        {
            var donation = await context.RevExpDonations.FindAsync(RevExpId);
            if (donation != null)
            {
                donation.Des = Des;
                donation.Revenue = Revenue;
                donation.Expenditure = Expenditure;
                donation.CamId = CamId;

                await context.SaveChangesAsync(); // Save the changes
            }

            return RedirectToPage(new { c_id = donation.CamId }); // Refresh the page with the same c_id
        }

        public IActionResult OnGetExportToExcel(int c_id)
        {
            // Kiểm tra nếu không có dữ liệu
            var revExpDonations = context.RevExpDonations
                .Where(c => c.CamId == c_id)
                .Include(c => c.Cam)
                .ToList();

            // Tạo Workbook và Worksheet
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("RevExpDonations");

                // Tiêu đề cột
                worksheet.Cells[1, 1].Value = "Nội Dung";
                worksheet.Cells[1, 2].Value = "Tiền Thu Được";
                worksheet.Cells[1, 3].Value = "Chi Tiêu";
                worksheet.Cells[1, 4].Value = "Chiến Dịch";

                // Nếu không có dữ liệu, chỉ ghi tiêu đề cột
                if (!revExpDonations.Any())
                {
                    // Ghi thông báo "Chưa có dữ liệu" vào dòng thứ 2
                    worksheet.Cells[2, 1].Value = "Chưa có dữ liệu.";
                }
                else
                {
                    // Ghi dữ liệu vào Excel
                    int row = 2;

                    foreach (var donation in revExpDonations)
                    {
                        worksheet.Cells[row, 1].Value = donation.Des;
                        worksheet.Cells[row, 2].Value = donation.Revenue?.ToString("C", new System.Globalization.CultureInfo("vi-VN"));
                        worksheet.Cells[row, 3].Value = donation.Expenditure?.ToString("C", new System.Globalization.CultureInfo("vi-VN"));
                        worksheet.Cells[row, 4].Value = donation.Cam?.CampaignName;
                        row++;
                    }
                }

                // Định dạng bảng
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Trả về file Excel
                var fileName = $"RevExpDonations_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        public IActionResult OnPostAdd()
        {
            if (ModelState.IsValid)
            {
                // Add the new record to the database
                context.RevExpDonations.Add(NewRevExpDonation);
                context.SaveChanges();

                // Redirect back to the same page or any other desired page
                return RedirectToPage("/Areas/Charity/ListDonation", new { c_id = NewRevExpDonation.CamId });
            }

            // If the model state is invalid, reload data and show the modal again
            var c_id = NewRevExpDonation.CamId;
            FundraisingCampaigns = context.FundraisingCampaigns.FirstOrDefault(c => c.CampaignId == c_id);
            RevExpDonations = context.RevExpDonations
                .Where(c => c.CamId == c_id)
                .Include(c => c.Cam)
                .ToList();

            return Page();
        }
        public async Task<IActionResult> OnPostAddExcelAsync(IFormFile ExcelFile, int CamId)
        {
            if (ExcelFile == null || ExcelFile.Length == 0)
            {
                ModelState.AddModelError("", "Vui lòng chọn file Excel.");
                return Page();
            }
            var donations = new List<RevExpDonation>();

            using (var stream = new MemoryStream())
            {
                await ExcelFile.CopyToAsync(stream);

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var des = worksheet.Cells[row, 1].Text.Trim();
                        var revenueText = worksheet.Cells[row, 2].Text.Trim();
                        var expenditureText = worksheet.Cells[row, 3].Text.Trim();

                        // Kiểm tra và chuyển đổi Revenue
                        double? revenue = string.IsNullOrEmpty(revenueText)
                            ? (double?)null
                            : double.TryParse(revenueText, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedRevenue)
                                ? parsedRevenue
                                : (double?)null;

                        // Kiểm tra và chuyển đổi Expenditure
                        double? expenditure = string.IsNullOrEmpty(expenditureText)
                            ? (double?)null
                            : double.TryParse(expenditureText, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedExpenditure)
                                ? parsedExpenditure
                                : (double?)null;

                        // Thêm vào danh sách, không cần kiểm tra giá trị null vì mô hình hỗ trợ nullable
                        var donation = new RevExpDonation
                        {
                            CamId = CamId,
                            Revenue = revenue,
                            Expenditure = expenditure,
                            Des = des
                        };

                        donations.Add(donation);
                    }

                }
            }

            if (donations.Any())
            {
                context.RevExpDonations.AddRange(donations);
                await context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Dữ liệu đã được thêm vào cơ sở dữ liệu thành công!";
            }
            else
            {
                ModelState.AddModelError("", "Không có dữ liệu hợp lệ để lưu.");
            }

            return RedirectToPage(new { c_id = CamId });
        }

        //khoi 1
        public async Task<IActionResult> OnPostUploadAndSaveAsync(IFormFile ExcelFile, int CamId)
        {
            if (ExcelFile == null || ExcelFile.Length == 0)
            {
                ModelState.AddModelError("", "Vui lòng chọn file Excel.");
                return Page();
            }

            var userId = HttpContext.Session.GetString("UserId");

            if (!int.TryParse(userId, out int parsedUserId))
            {
                ModelState.AddModelError("", "Không thể xác định người dùng.");
                return Page();
            }

            var donations = new List<FundraisingDonation>();

            using (var stream = new MemoryStream())
            {
                await ExcelFile.CopyToAsync(stream);

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var dateString = worksheet.Cells[row, 2].Text;
                        var amountText = worksheet.Cells[row, 3].Text;
                        var message = worksheet.Cells[row, 5].Text;

                        if (DateTime.TryParseExact(dateString, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate)
                            && double.TryParse(amountText, out double amount))
                        {
                            var donation = new FundraisingDonation
                            {
                                CampaignId = CamId,
                                UserId = parsedUserId,
                                DonationDate = parsedDate,
                                Amount = amount,
                                Description = message
                            };
                            donations.Add(donation);
                        }
                        else
                        {
                            Console.WriteLine($"Invalid data on row {row}: Date='{dateString}', Amount='{amountText}'");
                        }
                    }
                }
            }

            if (donations.Any())
            {
                context.FundraisingDonations.AddRange(donations);
                await context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Dữ liệu đã được thêm vào cơ sở dữ liệu thành công!";
            }
            else
            {
                ModelState.AddModelError("", "Không có dữ liệu hợp lệ để lưu.");
            }

            return RedirectToPage(new { c_id = CamId });
        }

    }
}
