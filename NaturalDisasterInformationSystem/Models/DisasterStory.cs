using System;
using System.Collections.Generic;

namespace NaturalDisasterInformationSystem.Models
{
    public partial class DisasterStory
    {
        public DisasterStory()
        {
            DisasterStoryComments = new HashSet<DisasterStoryComment>();
        }

        public int StoryId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = null!;
        public string? Img { get; set; }
        public string? Video { get; set; }
        public DateTime? Createdate { get; set; }
        public int? DisasterId { get; set; }

        public virtual Disaster? Disaster { get; set; }
        public virtual User User { get; set; } = null!;
        public virtual ICollection<DisasterStoryComment> DisasterStoryComments { get; set; }
    }
}
