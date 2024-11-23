using System;
using System.Collections.Generic;

namespace NaturalDisasterInformationSystem.Models
{
    public partial class EmergencyAlert
    {
        public int AlertId { get; set; }
        public string AlertMessage { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
    }
}
