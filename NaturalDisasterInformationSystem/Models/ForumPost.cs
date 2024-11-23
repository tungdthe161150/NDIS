using System;
using System.Collections.Generic;

namespace NaturalDisasterInformationSystem.Models
{
    public partial class ForumPost
    {
        public ForumPost()
        {
            ForumComments = new HashSet<ForumComment>();
        }

        public int PostId { get; set; }
        public int UserId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public bool? IsApprove { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Img { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual ICollection<ForumComment> ForumComments { get; set; }
    }
}
