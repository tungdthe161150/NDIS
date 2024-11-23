using System;
using System.Collections.Generic;

namespace NaturalDisasterInformationSystem.Models
{
    public partial class So
    {
        public int SosId { get; set; }
        public int UserId { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string? Content { get; set; }
        public DateTime? CreateTime { get; set; }
        public string? ImgSos { get; set; }
        public string? Address { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
