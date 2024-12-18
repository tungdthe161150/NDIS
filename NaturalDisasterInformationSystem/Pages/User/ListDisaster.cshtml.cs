using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.User
{
    public class ListDisasterModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly DO_ANContext _context;
        public List<DisasterDTO> Disasters { get; set; } = new List<DisasterDTO>();

        public ListDisasterModel(HttpClient httpClient, DO_ANContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }
        public async Task OnGetAsync()
        {
            Disasters = await CallApiAsync();
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
    }
}
