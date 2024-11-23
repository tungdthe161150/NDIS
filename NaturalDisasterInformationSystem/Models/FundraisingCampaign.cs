using System;
using System.Collections.Generic;

namespace NaturalDisasterInformationSystem.Models
{
    public partial class FundraisingCampaign
    {
        public FundraisingCampaign()
        {
            CharityEvents = new HashSet<CharityEvent>();
            FundraisingDonations = new HashSet<FundraisingDonation>();
            ImgDonations = new HashSet<ImgDonation>();
            ProjectInformations = new HashSet<ProjectInformation>();
            Reports = new HashSet<Report>();
            RevExpDonations = new HashSet<RevExpDonation>();
            VolunteerHistories = new HashSet<VolunteerHistory>();
        }

        public int CampaignId { get; set; }
        public int CharityId { get; set; }
        public int DisasterId { get; set; }
        public string? CampaignName { get; set; }
        public string? Description { get; set; }
        public double? GoalAmount { get; set; }
        public double? RaisedAmount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ImgCam { get; set; }
        public string? ImgQr { get; set; }
        public string? Hidden { get; set; }

        public virtual Charity Charity { get; set; } = null!;
        public virtual Disaster Disaster { get; set; } = null!;
        public virtual ICollection<CharityEvent> CharityEvents { get; set; }
        public virtual ICollection<FundraisingDonation> FundraisingDonations { get; set; }
        public virtual ICollection<ImgDonation> ImgDonations { get; set; }
        public virtual ICollection<ProjectInformation> ProjectInformations { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
        public virtual ICollection<RevExpDonation> RevExpDonations { get; set; }
        public virtual ICollection<VolunteerHistory> VolunteerHistories { get; set; }
    }
}
