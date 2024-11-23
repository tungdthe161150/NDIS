using System;
using System.Collections.Generic;

namespace NaturalDisasterInformationSystem.Models
{
    public partial class RevExpDonation
    {
        public int RevExpId { get; set; }
        public string? Des { get; set; }
        public double? Revenue { get; set; }
        public double? Expenditure { get; set; }
        public int? CamId { get; set; }

        public virtual FundraisingCampaign? Cam { get; set; }
    }
}
