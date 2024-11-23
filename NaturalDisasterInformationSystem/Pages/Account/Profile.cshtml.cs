using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.Account
{
    public class ProfileModel : PageModel
    {
        private readonly DO_ANContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment; // Add this field

        public ProfileModel(DO_ANContext context, IWebHostEnvironment webHostEnvironment) // Update constructor
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }


        // Property to hold user profile
        public NaturalDisasterInformationSystem.Models.User UserProfile { get; set; }

        public IActionResult OnGet(int uid)
        {
            UserProfile = _context.Users.FirstOrDefault(u=>u.UserId == uid);

            if (UserProfile == null)
            {
                return NotFound();
            }

            return Page();
        }

        public IActionResult OnPost(int uid, string email,  string username,string phonenumber,string pass,string fullname,bool gender,DateTime birthday,string careers,string address, IFormFile profileImage)
        {
            // Retrieve the existing user
            var user = _context.Users.FirstOrDefault(u => u.UserId == uid);
            if (user == null)
            {
                return NotFound();
            }

            // Update user properties
            user.Email = email;
            user.Username = username;
            user.PhoneNumber = phonenumber;
            user.FullName = fullname;
            user.Gender = gender;
            user.Address = address;
            user.Birthday = birthday;
            user.Careers = careers;
            if (profileImage != null && profileImage.Length > 0)
            {
                try
                {
                    // Define the path to save the image
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(profileImage.FileName); // Create a unique file name
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    // Ensure the uploads folder exists
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        profileImage.CopyTo(stream);
                    }

                    // Update the user's image path in the database
                    user.Img = "/uploads/" + fileName; // Store the path to the image
                }
                catch (Exception ex)
                {
                    // Log the exception or display an error message
                    ModelState.AddModelError("", $"Error uploading image: {ex.Message}");
                }
            }

            // Save changes to the database
            _context.SaveChanges();

            // Redirect to the profile page (or wherever you want)
            return RedirectToPage("Profile", new { uid = user.UserId });
        }
    }
}