using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NaturalDisasterInformationSystem.Models;

namespace NaturalDisasterInformationSystem.Pages.Map
{
    public class Map_disasterModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public Map_disasterModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public List<DisasterDTO> Disasters { get; set; } = new List<DisasterDTO>();

        public async Task OnGetAsync()
        {
            Disasters = await CallApiAsync();
        }

        private async Task<List<DisasterDTO>> CallApiAsync()
        {
            var apiUrl = "http://vndms.dmc.gov.vn/EventDisaster/Searching?keyword=&year=2024&limit=1";
            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var disasters = await response.Content.ReadFromJsonAsync<List<DisasterDTO>>();
                return disasters ?? new List<DisasterDTO>();
            }
            else
            {
                return new List<DisasterDTO>();
            }
        }
    }
}
