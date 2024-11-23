using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;
using NaturalDisasterInformationSystem.Services;

namespace NaturalDisasterInformationSystem.Pages.Areas.Alerts
{
    public class CreateModel : PageModel
    {
        private readonly SMSService speedSMSAPI;

        private readonly DO_ANContext context;

        [BindProperty]
        public List<Models.User> Users { get; set; }

        [BindProperty]
        public List<string> SelectedPhoneNumbers { get; set; }

        public CreateModel(DO_ANContext context,SMSService service)
        {
            this.context = context;
            this.speedSMSAPI = service;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Users = context.Users.ToList();
            ViewData["User"] = Users;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string MessageContent)
        {
            if (SelectedPhoneNumbers == null || !SelectedPhoneNumbers.Any() || string.IsNullOrEmpty(MessageContent))
            {
                ViewData["ErrorMessage"] = "Vui lòng chọn ít nhất một số điện thoại và nhập nội dung tin nhắn.";
                return Page();
            }

            /*try
            {
                // Chuyển đổi danh sách số điện thoại thành mảng `string[]`
                string[] phoneNumberArray = SelectedPhoneNumbers.ToArray();

                // Gửi tin nhắn SMS
                string response = await speedSMSAPI.SendSMSAsync(phoneNumberArray,MessageContent, SmsType, Sender);

                // Xử lý phản hồi từ API
                dynamic jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(response);
                if (jsonResponse.status == "success")
                {
                    ViewData["SuccessMessage"] = "Tin nhắn đã được gửi thành công!";

                    //Lưu thông báo vào trong database
                    var notify = new EmergencyAlert
                    {
                        AlertMessage = jsonResponse.message,
                        CreatedAt = DateTime.Now,
                    };

                    context.EmergencyAlerts.Add(notify);
                    context.SaveChanges();
                }
                else
                {
                    ViewData["ErrorMessage"] = $"Lỗi gửi tin nhắn: {jsonResponse.message}";
                }
            }
            catch (Exception ex)
            {
                // Kiểm tra lỗi 401 Unauthorized
                if (ex.Message.Contains("401"))
                {
                    ViewData["Error"] = "Unauthorized: Kiểm tra lại Access Token.";
                }
                else
                {
                    ViewData["Error"] = $"Lỗi xảy ra: {ex.Message}";
                }
            }*/

            // Lấy danh sách số điện thoại từ database
            List<string> phoneNumbers = context.Users
                .Where(u => !string.IsNullOrEmpty(u.PhoneNumber))
                .Select(u => u.PhoneNumber)
                .ToList();

            if (!phoneNumbers.Any())
            {
                ViewData["ErrorMessage"] = "Không có số điện thoại hợp lệ trong danh sách.";
                return Page();
            }

            // Gửi SMS
            var result = await speedSMSAPI.SendSmsAsync(MessageContent);

            var notify = new EmergencyAlert
            {
                AlertMessage = MessageContent,
                CreatedAt = DateTime.Now,
            };

            context.EmergencyAlerts.Add(notify);
            context.SaveChanges();

            /*if (result.Contains("thành công"))
            {
                ViewData["SuccessMessage"] = result;
            }
            else
            {
                ViewData["ErrorMessage"] = result;
            }*/

            // Hiển thị kết quả gửi tin nhắn
            ViewData["Result"] = string.Join("<br>", result);

            return Page();
        }
    }
}
