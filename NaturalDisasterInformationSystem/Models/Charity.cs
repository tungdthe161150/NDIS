using System;
using System.Collections.Generic;

namespace NaturalDisasterInformationSystem.Models
{
    public partial class Charity
    {
        public Charity()
        {
            CharityEvents = new HashSet<CharityEvent>();
            DocumentImgs = new HashSet<DocumentImg>();
            FundraisingCampaigns = new HashSet<FundraisingCampaign>();
        }

        public int CharityId { get; set; }
        public int UserId { get; set; }
        public string CharityName { get; set; } = null!;
        public string? Description { get; set; }
        public string? Website { get; set; }
        public string? ContactEmail { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? Reliability { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Img { get; set; }
        public string? Comment { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual ICollection<CharityEvent> CharityEvents { get; set; }
        public virtual ICollection<DocumentImg> DocumentImgs { get; set; }
        public virtual ICollection<FundraisingCampaign> FundraisingCampaigns { get; set; }
    }
}
