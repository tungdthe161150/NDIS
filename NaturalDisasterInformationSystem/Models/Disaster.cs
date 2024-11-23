using System;
using System.Collections.Generic;

namespace NaturalDisasterInformationSystem.Models
{
    public partial class Disaster
    {
        public Disaster()
        {
            DisasterStories = new HashSet<DisasterStory>();
            FundraisingCampaigns = new HashSet<FundraisingCampaign>();
            VolunteersRegistrations = new HashSet<VolunteersRegistration>();
        }

        public int DisasterId { get; set; }
        public string DisasterName { get; set; } = null!;
        public string? Location { get; set; }
        public string? Description { get; set; }
        public int? SeverityLevel { get; set; }
        public DateTime ReportedAt { get; set; }
        public int? IdJson { get; set; }

        public virtual ICollection<DisasterStory> DisasterStories { get; set; }
        public virtual ICollection<FundraisingCampaign> FundraisingCampaigns { get; set; }
        public virtual ICollection<VolunteersRegistration> VolunteersRegistrations { get; set; }
    }
}
