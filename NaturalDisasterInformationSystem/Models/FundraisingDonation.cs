using System;
using System.Collections.Generic;

namespace NaturalDisasterInformationSystem.Models
{
    public partial class FundraisingDonation
    {
        public int DonationId { get; set; }
        public int CampaignId { get; set; }
        public int UserId { get; set; }
        public double? Amount { get; set; }
        public DateTime? DonationDate { get; set; }
        public string Description { get; set; }

        public virtual FundraisingCampaign Campaign { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
