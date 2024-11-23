using System;
using System.Collections.Generic;

namespace NaturalDisasterInformationSystem.Models
{
    public partial class DocumentImg
    {
        public int PhotoId { get; set; }
        public int CharityId { get; set; }
        public string? Img { get; set; }

        public virtual Charity Charity { get; set; } = null!;
    }
}
