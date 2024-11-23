using System;
using System.Collections.Generic;

namespace NaturalDisasterInformationSystem.Models
{
    public partial class CharityEvent
    {
        public int EventId { get; set; }
        public int CharityId { get; set; }
        public int? CampaignId { get; set; }
        public string? EventName { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime? EventCreateDate { get; set; }

        public virtual FundraisingCampaign? Campaign { get; set; }
        public virtual Charity Charity { get; set; } = null!;
    }
}
