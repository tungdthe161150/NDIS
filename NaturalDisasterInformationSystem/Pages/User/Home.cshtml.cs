using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace NaturalDisasterInformationSystem.Pages.User
{
    public class HomeModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly DO_ANContext _context;

        public HomeModel(HttpClient httpClient, DO_ANContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }

        public List<DisasterDTO> Disasters { get; set; } = new List<DisasterDTO>(); 
        public List<So> Reports { get; set; } = new List<So>();

        // Fetch disasters and reports on GET request
        public async Task OnGetAsync()
        {
            Disasters = await CallApiAsync();
            Reports = await _context.Sos.Include(u => u.User).Where(s => s.CreateTime >= DateTime.Now.AddDays(-20)).OrderByDescending(s => s.CreateTime).ToListAsync();
        }

        private async Task<List<DisasterDTO>> CallApiAsync()
        {
            var apiUrl = "http://vndms.dmc.gov.vn/EventDisaster/Searching?keyword=&year=2024";
            var response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var disasters = await response.Content.ReadFromJsonAsync<List<DisasterDTO>>();

                // L?c ra các s? ki?n không có id là 3052
                return disasters?.Where(d => d.Id != 3052).ToList() ?? new List<DisasterDTO>();
            }
            else
            {
                return new List<DisasterDTO>();
            }
           /* if (response.IsSuccessStatusCode)
            {
                var disasters = await response.Content.ReadFromJsonAsync<List<DisasterDTO>>();
                return disasters ?? new List<DisasterDTO>();
            }
            else
            {
                return new List<DisasterDTO>();
            }*/
        }

        [HttpPost]
        public async Task<IActionResult> OnPostAddDisasterAsync(
    string disasterName, string? location, string? description, int? severityLevel, int? idjson)
        {
            if (string.IsNullOrWhiteSpace(disasterName))
            {
                ModelState.AddModelError("", "Disaster name is required.");
                return Page();
            }

            if (!ModelState.IsValid)
            {
                return Page(); // Return with errors displayed
            }

            // Check if a disaster with the same IdJson already exists
            var existingDisaster = await _context.Disasters
                .FirstOrDefaultAsync(d => d.IdJson == idjson);

            Disaster disaster; // Declare disaster here

            if (existingDisaster != null)
            {
                // Update the existing disaster's fields
                existingDisaster.DisasterName = disasterName;
                existingDisaster.Location = location;
                existingDisaster.Description = description;
                existingDisaster.SeverityLevel = severityLevel;
                existingDisaster.ReportedAt = DateTime.Now;

                // Mark the entity as modified
                _context.Disasters.Update(existingDisaster);

                // Set disaster to the existing disaster for redirection
                disaster = existingDisaster;
            }
            else
            {
                // Create a new disaster if no existing one is found
                disaster = new Disaster
                {
                    DisasterName = disasterName,
                    Location = location,
                    Description = description,
                    SeverityLevel = severityLevel,
                    IdJson = idjson,
                    ReportedAt = DateTime.Now
                };

                await _context.Disasters.AddAsync(disaster);
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = existingDisaster != null
                ? "Disaster updated successfully."
                : "Disaster added successfully.";

            // Redirect to DisasterDetail with the correct DisasterId
            return RedirectToPage("/User/DisasterDetail", new { dd_id = disaster.DisasterId });
        }



        // POST method for adding a disaster report
        [HttpPost]
        public async Task<IActionResult> OnPostAddReportAsync(
      string content, double longitude, double latitude, int userId,
      List<IFormFile>? imgSos, string address)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                ModelState.AddModelError("", "Report content cannot be empty.");
                return Page();
            }

            List<string> imgSosPaths = new List<string>();

            // Define the upload folder path
            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "sos");

            // Check if the directory exists; if not, create it
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            if (imgSos != null && imgSos.Count > 0)
            {
                foreach (var img in imgSos)
                {
                    if (img.Length > 0)
                    {
                        var filePath = Path.Combine(uploadFolder, img.FileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await img.CopyToAsync(stream);
                        }
                        imgSosPaths.Add(img.FileName);
                    }
                }
            }

            try
            {
                var newReport = new So
                {
                    Content = content,
                    Longitude = longitude,
                    Latitude = latitude,
                    UserId = userId,
                    CreateTime = DateTime.Now,
                    ImgSos = string.Join(",", imgSosPaths), // Store paths as comma-separated string
                    Address = address
                };

                await SaveReportAsync(newReport);

                TempData["SuccessMessage"] = "Report submitted successfully.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return Page();
            }
        }


        private async Task SaveReportAsync(So report)
        {
            var userExists = await _context.Users.AnyAsync(u => u.UserId == report.UserId);

            if (!userExists)
            {
                throw new Exception($"User with ID {report.UserId} does not exist.");
            }

            await _context.Sos.AddAsync(report);
            await _context.SaveChangesAsync();
        }
    }
}
