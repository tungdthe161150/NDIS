using System;
using System.Collections.Generic;

namespace NaturalDisasterInformationSystem.Models
{
    public partial class VolunteersRegistration
    {
        public int VolunteerRegistrationId { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
        public string? Status { get; set; }
        public DateTime? JoinedAt { get; set; }

        public virtual Disaster Event { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
