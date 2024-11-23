using System;
using System.Collections.Generic;

namespace NaturalDisasterInformationSystem.Models
{
    public partial class ImgDonation
    {
        public int ImgDoId { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? File { get; set; }
        public int? CamId { get; set; }
        public string? Content { get; set; }

        public virtual FundraisingCampaign? Cam { get; set; }
    }
}
