using System;
using System.Collections.Generic;

namespace NaturalDisasterInformationSystem.Models
{
    public partial class Report
    {
        public int ReportId { get; set; }
        public int CampaignId { get; set; }
        public int UserId { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual FundraisingCampaign Campaign { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
