using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NaturalDisasterInformationSystem.Models;
using MailKit.Net.Smtp;
using MimeKit;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using Microsoft.AspNetCore.Authorization;

namespace NaturalDisasterInformationSystem.Pages.User
{
    [Authorize]

    public class RegisterCharityModel : PageModel
    {
        private readonly DO_ANContext _context;
        private readonly IWebHostEnvironment _environment;

        [BindProperty]
        public string UserId { get; set; } = null!;
        [BindProperty]
        public string CharityName { get; set; } = null!;
        [BindProperty]
        public string Description { get; set; } = null!;
        [BindProperty]
        public string Website { get; set; } = null!;
        [BindProperty]
        public string ContactEmail { get; set; } = null!;
        [BindProperty]
        public string? PhoneNumber { get; set; }

        [BindProperty]
        public List<IFormFile> Images { get; set; } = new();

        public Models.Charity CharityDetail { get; set; } = null!;
        public RegisterCharityModel(DO_ANContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public void OnGet(int cid) {
            CharityDetail = _context.Charities.Include(d => d.DocumentImgs).FirstOrDefault(c=>c.UserId==cid);
        }
        public async Task<IActionResult> OnPostUpdateAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var charity = await _context.Charities.Include(c => c.DocumentImgs)
                                                  .FirstOrDefaultAsync(c => c.UserId == int.Parse(UserId));

            if (charity == null)
            {
                ModelState.AddModelError(string.Empty, "Charity not found.");
                return Page();
            }
            if (string.IsNullOrWhiteSpace(PhoneNumber) || PhoneNumber.Length != 10 || !PhoneNumber.All(char.IsDigit))
            {
                ModelState.AddModelError(nameof(PhoneNumber), "So dien thoai phai co 10 chu so.");
                return Page();
            }
            charity.CharityName = CharityName;
            charity.Description = Description;
            charity.Website = Website;
            charity.ContactEmail = ContactEmail;
            charity.PhoneNumber = PhoneNumber;
            charity.CreatedAt = DateTime.Now;

            if (Images != null && Images.Count > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                // Get existing images for the charity
                var existingImages = _context.DocumentImgs
                    .Where(img => img.CharityId == charity.CharityId)
                    .ToList();

                foreach (var image in Images)
                {
                    var filePath = Path.Combine(uploadsFolder, image.FileName);

                    // Check if the file already exists in the database
                    var existingImage = existingImages.FirstOrDefault(img => img.Img == image.FileName);

                    if (existingImage != null)
                    {
                        // Replace the file on disk
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath); // Remove the old file
                        }
                    }
                    else
                    {
                        // Add new record to the database
                        var documentImg = new DocumentImg
                        {
                            CharityId = charity.CharityId,
                            Img = image.FileName
                        };
                        _context.DocumentImgs.Add(documentImg);
                    }

                    // Save the file to disk
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }
                }

                await _context.SaveChangesAsync();
            }


            await _context.SaveChangesAsync();
            ModelState.AddModelError(string.Empty, "Charity information updated successfully.");
            return RedirectToPage(new { cid = charity.UserId }); // Redirect after saving the image
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            if (_context.Charities.Any(c => c.CharityName == CharityName || c.ContactEmail == ContactEmail || c.UserId.Equals(UserId)))
            {
                ModelState.AddModelError(string.Empty, "Charity Name or Email already exists.");
                return Page();
            }
            if (string.IsNullOrWhiteSpace(PhoneNumber) || PhoneNumber.Length != 10 || !PhoneNumber.All(char.IsDigit))
            {
                ModelState.AddModelError(nameof(PhoneNumber), "So dien thoai phai co 10 chu so.");
                return Page();
            }
            var charity = new Charity
            {
                UserId = int.Parse(UserId),
                CharityName = CharityName,
                Description = Description,
                Website = Website,
                ContactEmail = ContactEmail,
                PhoneNumber = PhoneNumber,
                Reliability = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Charities.Add(charity);
            await _context.SaveChangesAsync();

            if (Images != null && Images.Count > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                foreach (var image in Images)
                {
                    var filePath = Path.Combine(uploadsFolder, image.FileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }

                    var documentImg = new DocumentImg
                    {
                        CharityId = charity.CharityId,
                        Img = $"{image.FileName}"
                    };
                    _context.DocumentImgs.Add(documentImg);
                }

                await _context.SaveChangesAsync();
            }

            SendConfirmationEmail(ContactEmail);
            ModelState.AddModelError(string.Empty, "Ban da gui thông tin thành công.");
            return RedirectToPage(new { cid = UserId }); // Redirect after saving the image
        }

        private void SendConfirmationEmail(string email)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Natural Disaster System", "no-reply@nds.com"));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Charity Registration Confirmation";

            message.Body = new TextPart("plain")
            {
                Text = "Thank you for registering your charity with us. Please verify your email to complete the registration."
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("ledungb12509@gmail.com", "bdog vxel qcwk bbqh");
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
