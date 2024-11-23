using System;
using System.Collections.Generic;

namespace NaturalDisasterInformationSystem.Models
{
    public partial class ProjectInformation
    {
        public int InformationId { get; set; }
        public string? Content { get; set; }
        public string? Video { get; set; }
        public int ProfileId { get; set; }
        public int CampaignId { get; set; }
        public string? Status { get; set; }
        public DateTime CreateDate { get; set; }
        public string? Img { get; set; }

        public virtual FundraisingCampaign Campaign { get; set; } = null!;
        public virtual VolunteerHistory Profile { get; set; } = null!;
    }
}
