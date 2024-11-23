using System;
using System.Collections.Generic;

namespace NaturalDisasterInformationSystem.Models
{
    public partial class DisasterType
    {
        public DisasterType()
        {
            DisasterBlogs = new HashSet<DisasterBlog>();
            Disasters = new HashSet<Disaster>();
        }

        public int TypeId { get; set; }
        public string TypeName { get; set; } = null!;

        public virtual ICollection<DisasterBlog> DisasterBlogs { get; set; }
        public virtual ICollection<Disaster> Disasters { get; set; }
    }
}
