using System;
using System.Collections.Generic;

namespace NaturalDisasterInformationSystem.Models
{
    public partial class DisasterStoryComment
    {
        public int CommentId { get; set; }
        public int StoryId { get; set; }
        public int UserId { get; set; }
        public string CommentText { get; set; } = null!;
        public string? Img { get; set; }
        public string? Video { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual DisasterStory Story { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
