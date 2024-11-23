using System;
using System.Collections.Generic;

namespace NaturalDisasterInformationSystem.Models
{
    public partial class VolunteerHistory
    {
        public VolunteerHistory()
        {
            ProjectInformations = new HashSet<ProjectInformation>();
        }

        public int ProfileId { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
        public string? Status { get; set; }
        public DateTime? JoinedAt { get; set; }
        public string? Evaluate { get; set; }
        public string? Reasons { get; set; }
        public string? Describe { get; set; }

        public virtual FundraisingCampaign Event { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual ICollection<ProjectInformation> ProjectInformations { get; set; }
    }
}
