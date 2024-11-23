using System;
using System.Collections.Generic;

namespace NaturalDisasterInformationSystem.Models
{
    public partial class User
    {
        public User()
        {
            Charities = new HashSet<Charity>();
            DisasterBlogs = new HashSet<DisasterBlog>();
            DisasterStories = new HashSet<DisasterStory>();
            DisasterStoryComments = new HashSet<DisasterStoryComment>();
            ForumComments = new HashSet<ForumComment>();
            ForumPosts = new HashSet<ForumPost>();
            FundraisingDonations = new HashSet<FundraisingDonation>();
            Reports = new HashSet<Report>();
            Sos = new HashSet<So>();
            VolunteerHistories = new HashSet<VolunteerHistory>();
            VolunteersRegistrations = new HashSet<VolunteersRegistration>();
        }

        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public int RoleId { get; set; }
        public bool? Active { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Img { get; set; }
        public DateTime? Birthday { get; set; }
        public bool? Gender { get; set; }
        public string? Address { get; set; }
        public string? Careers { get; set; }

        public virtual Role Role { get; set; } = null!;
        public virtual ICollection<Charity> Charities { get; set; }
        public virtual ICollection<DisasterBlog> DisasterBlogs { get; set; }
        public virtual ICollection<DisasterStory> DisasterStories { get; set; }
        public virtual ICollection<DisasterStoryComment> DisasterStoryComments { get; set; }
        public virtual ICollection<ForumComment> ForumComments { get; set; }
        public virtual ICollection<ForumPost> ForumPosts { get; set; }
        public virtual ICollection<FundraisingDonation> FundraisingDonations { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
        public virtual ICollection<So> Sos { get; set; }
        public virtual ICollection<VolunteerHistory> VolunteerHistories { get; set; }
        public virtual ICollection<VolunteersRegistration> VolunteersRegistrations { get; set; }
    }
}
