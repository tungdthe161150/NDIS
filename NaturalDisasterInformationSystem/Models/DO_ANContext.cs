using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace NaturalDisasterInformationSystem.Models
{
    public partial class DO_ANContext : DbContext
    {
        public DO_ANContext()
        {
        }

        public DO_ANContext(DbContextOptions<DO_ANContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Charity> Charities { get; set; } = null!;
        public virtual DbSet<CharityEvent> CharityEvents { get; set; } = null!;
        public virtual DbSet<Disaster> Disasters { get; set; } = null!;
        public virtual DbSet<DisasterBlog> DisasterBlogs { get; set; } = null!;
        public virtual DbSet<DisasterStory> DisasterStories { get; set; } = null!;
        public virtual DbSet<DisasterStoryComment> DisasterStoryComments { get; set; } = null!;
        public virtual DbSet<DocumentImg> DocumentImgs { get; set; } = null!;
        public virtual DbSet<EmergencyAlert> EmergencyAlerts { get; set; } = null!;
        public virtual DbSet<ForumComment> ForumComments { get; set; } = null!;
        public virtual DbSet<ForumPost> ForumPosts { get; set; } = null!;
        public virtual DbSet<FundraisingCampaign> FundraisingCampaigns { get; set; } = null!;
        public virtual DbSet<FundraisingDonation> FundraisingDonations { get; set; } = null!;
        public virtual DbSet<ImgDonation> ImgDonations { get; set; } = null!;
        public virtual DbSet<News> News { get; set; } = null!;
        public virtual DbSet<ProjectInformation> ProjectInformations { get; set; } = null!;
        public virtual DbSet<Report> Reports { get; set; } = null!;
        public virtual DbSet<RevExpDonation> RevExpDonations { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<So> Sos { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<VolunteerHistory> VolunteerHistories { get; set; } = null!;
        public virtual DbSet<VolunteersRegistration> VolunteersRegistrations { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server=ADMIN-PC;database=DO_AN;uid=sa;pwd=123;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Charity>(entity =>
            {
                entity.ToTable("charities");

                entity.Property(e => e.CharityId).HasColumnName("charity_id");

                entity.Property(e => e.CharityName)
                    .HasMaxLength(100)
                    .HasColumnName("charity_name");

                entity.Property(e => e.ContactEmail)
                    .HasMaxLength(100)
                    .HasColumnName("contact_email");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Img)
                    .HasMaxLength(255)
                    .HasColumnName("img");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(15)
                    .HasColumnName("phone_number");

                entity.Property(e => e.Reliability)
                    .HasColumnName("reliability")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.Website)
                    .HasMaxLength(255)
                    .HasColumnName("website");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Charities)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__charities__user___5FB337D6");
            });

            modelBuilder.Entity<CharityEvent>(entity =>
            {
                entity.HasKey(e => e.EventId)
                    .HasName("PK__charity___2370F727D9408A9B");

                entity.ToTable("charity_events");

                entity.Property(e => e.EventId).HasColumnName("event_id");

                entity.Property(e => e.CampaignId).HasColumnName("campaign_id");

                entity.Property(e => e.CharityId).HasColumnName("charity_id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.EventCreateDate)
                    .HasColumnType("date")
                    .HasColumnName("event_create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EventName)
                    .HasMaxLength(255)
                    .HasColumnName("event_name");

                entity.Property(e => e.Location)
                    .HasMaxLength(255)
                    .HasColumnName("location");

                entity.HasOne(d => d.Campaign)
                    .WithMany(p => p.CharityEvents)
                    .HasForeignKey(d => d.CampaignId)
                    .HasConstraintName("FK__charity_e__campa__60A75C0F");

                entity.HasOne(d => d.Charity)
                    .WithMany(p => p.CharityEvents)
                    .HasForeignKey(d => d.CharityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__charity_e__chari__619B8048");
            });

            modelBuilder.Entity<Disaster>(entity =>
            {
                entity.ToTable("disasters");

                entity.Property(e => e.DisasterId).HasColumnName("disaster_id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.DisasterName)
                    .HasMaxLength(100)
                    .HasColumnName("disaster_name");

                entity.Property(e => e.IdJson).HasColumnName("id_json");

                entity.Property(e => e.Location)
                    .HasMaxLength(255)
                    .HasColumnName("location");

                entity.Property(e => e.ReportedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("reported_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.SeverityLevel).HasColumnName("severity_level");
            });

            modelBuilder.Entity<DisasterBlog>(entity =>
            {
                entity.HasKey(e => e.BlogId)
                    .HasName("PK__disaster__2975AA28780536FB");

                entity.ToTable("disaster_blogs");

                entity.Property(e => e.BlogId).HasColumnName("blog_id");

                entity.Property(e => e.Content).HasColumnName("content");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .HasColumnName("title");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.DisasterBlogs)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__disaster___user___628FA481");
            });

            modelBuilder.Entity<DisasterStory>(entity =>
            {
                entity.HasKey(e => e.StoryId)
                    .HasName("PK__disaster__66339C562CA50300");

                entity.ToTable("disaster_story");

                entity.Property(e => e.StoryId).HasColumnName("story_id");

                entity.Property(e => e.Content).HasColumnName("content");

                entity.Property(e => e.Createdate)
                    .HasColumnType("datetime")
                    .HasColumnName("createdate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DisasterId).HasColumnName("disaster_id");

                entity.Property(e => e.Img).HasColumnName("img");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.Video).HasColumnName("video");

                entity.HasOne(d => d.Disaster)
                    .WithMany(p => p.DisasterStories)
                    .HasForeignKey(d => d.DisasterId)
                    .HasConstraintName("FK__disaster___disas__6383C8BA");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.DisasterStories)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__disaster___user___6477ECF3");
            });

            modelBuilder.Entity<DisasterStoryComment>(entity =>
            {
                entity.HasKey(e => e.CommentId)
                    .HasName("PK__disaster__E79576878BE336B3");

                entity.ToTable("disaster_story_comments");

                entity.Property(e => e.CommentId).HasColumnName("comment_id");

                entity.Property(e => e.CommentText).HasColumnName("comment_text");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Img).HasColumnName("img");

                entity.Property(e => e.StoryId).HasColumnName("story_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.Video).HasColumnName("video");

                entity.HasOne(d => d.Story)
                    .WithMany(p => p.DisasterStoryComments)
                    .HasForeignKey(d => d.StoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__disaster___story__656C112C");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.DisasterStoryComments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__disaster___user___66603565");
            });

            modelBuilder.Entity<DocumentImg>(entity =>
            {
                entity.HasKey(e => e.PhotoId)
                    .HasName("PK__document__CB48C83D6A8204AC");

                entity.ToTable("document_img");

                entity.Property(e => e.PhotoId).HasColumnName("photo_id");

                entity.Property(e => e.CharityId).HasColumnName("charity_id");

                entity.Property(e => e.Img)
                    .HasMaxLength(255)
                    .HasColumnName("img");

                entity.HasOne(d => d.Charity)
                    .WithMany(p => p.DocumentImgs)
                    .HasForeignKey(d => d.CharityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__document___chari__6754599E");
            });

            modelBuilder.Entity<EmergencyAlert>(entity =>
            {
                entity.HasKey(e => e.AlertId)
                    .HasName("PK__emergenc__4B8FB03A320C810F");

                entity.ToTable("emergency_alerts");

                entity.Property(e => e.AlertId).HasColumnName("alert_id");

                entity.Property(e => e.AlertMessage).HasColumnName("alert_message");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<ForumComment>(entity =>
            {
                entity.HasKey(e => e.CommentId)
                    .HasName("PK__forum_co__E7957687C4E3C0C4");

                entity.ToTable("forum_comments");

                entity.Property(e => e.CommentId).HasColumnName("comment_id");

                entity.Property(e => e.CommentText).HasColumnName("comment_text");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Img).HasColumnName("img");

                entity.Property(e => e.PostId).HasColumnName("post_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.ForumComments)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__forum_com__post___68487DD7");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ForumComments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__forum_com__user___693CA210");
            });

            modelBuilder.Entity<ForumPost>(entity =>
            {
                entity.HasKey(e => e.PostId)
                    .HasName("PK__forum_po__3ED78766B70E53FD");

                entity.ToTable("forum_posts");

                entity.Property(e => e.PostId).HasColumnName("post_id");

                entity.Property(e => e.Content).HasColumnName("content");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Img).HasColumnName("img");

                entity.Property(e => e.IsApprove)
                    .HasColumnName("isApprove")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .HasColumnName("title");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ForumPosts)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__forum_pos__user___6A30C649");
            });

            modelBuilder.Entity<FundraisingCampaign>(entity =>
            {
                entity.HasKey(e => e.CampaignId)
                    .HasName("PK__fundrais__905B681CE6F508FD");

                entity.ToTable("fundraising_campaigns");

                entity.Property(e => e.CampaignId).HasColumnName("campaign_id");

                entity.Property(e => e.CampaignName)
                    .HasMaxLength(255)
                    .HasColumnName("campaign_name");

                entity.Property(e => e.CharityId).HasColumnName("charity_id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.DisasterId).HasColumnName("disaster_id");

                entity.Property(e => e.EndDate)
                    .HasColumnType("date")
                    .HasColumnName("end_date");

                entity.Property(e => e.GoalAmount).HasColumnName("goal_amount");

                entity.Property(e => e.Hidden)
                    .HasMaxLength(50)
                    .HasColumnName("hidden");

                entity.Property(e => e.ImgCam).HasColumnName("img_cam");

                entity.Property(e => e.ImgQr).HasColumnName("img_QR");

                entity.Property(e => e.RaisedAmount)
                    .HasColumnName("raised_amount")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("start_date");

                entity.HasOne(d => d.Charity)
                    .WithMany(p => p.FundraisingCampaigns)
                    .HasForeignKey(d => d.CharityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__fundraisi__chari__6B24EA82");

                entity.HasOne(d => d.Disaster)
                    .WithMany(p => p.FundraisingCampaigns)
                    .HasForeignKey(d => d.DisasterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__fundraisi__disas__6C190EBB");
            });

            modelBuilder.Entity<FundraisingDonation>(entity =>
            {
                entity.HasKey(e => e.DonationId)
                    .HasName("PK__fundrais__296B91DC9E035E55");

                entity.ToTable("fundraising_donations");

                entity.Property(e => e.DonationId).HasColumnName("donation_id");

                entity.Property(e => e.Amount).HasColumnName("amount");

                entity.Property(e => e.CampaignId).HasColumnName("campaign_id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.DonationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("donation_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Campaign)
                    .WithMany(p => p.FundraisingDonations)
                    .HasForeignKey(d => d.CampaignId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__fundraisi__campa__6D0D32F4");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.FundraisingDonations)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_fundraising_donations_users");
            });

            modelBuilder.Entity<ImgDonation>(entity =>
            {
                entity.HasKey(e => e.ImgDoId);

                entity.ToTable("img_donation");

                entity.Property(e => e.ImgDoId).HasColumnName("img_do_id");

                entity.Property(e => e.CamId).HasColumnName("cam_id");

                entity.Property(e => e.Content).HasColumnName("content");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date");

                entity.Property(e => e.File).HasColumnName("file");

                entity.HasOne(d => d.Cam)
                    .WithMany(p => p.ImgDonations)
                    .HasForeignKey(d => d.CamId)
                    .HasConstraintName("FK_img_donation_fundraising_campaigns");
            });

            modelBuilder.Entity<News>(entity =>
            {
                entity.ToTable("news");

                entity.Property(e => e.NewsId).HasColumnName("news_id");

                entity.Property(e => e.Content).HasColumnName("content");

                entity.Property(e => e.PublishedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("published_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Source)
                    .HasMaxLength(255)
                    .HasColumnName("source");

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .HasColumnName("title");
            });

            modelBuilder.Entity<ProjectInformation>(entity =>
            {
                entity.HasKey(e => e.InformationId);

                entity.ToTable("project_information");

                entity.Property(e => e.InformationId).HasColumnName("information_id");

                entity.Property(e => e.CampaignId).HasColumnName("campaign_id");

                entity.Property(e => e.Content).HasColumnName("content");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date");

                entity.Property(e => e.Img).HasColumnName("img");

                entity.Property(e => e.ProfileId).HasColumnName("profile_id");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Video).HasColumnName("video");

                entity.HasOne(d => d.Campaign)
                    .WithMany(p => p.ProjectInformations)
                    .HasForeignKey(d => d.CampaignId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_project_information_fundraising_campaigns");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ProjectInformations)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_project_information_volunteer_history");
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("reports");

                entity.Property(e => e.ReportId).HasColumnName("report_id");

                entity.Property(e => e.CampaignId).HasColumnName("campaign_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.Reason)
                    .HasColumnType("text")
                    .HasColumnName("reason");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("status");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Campaign)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.CampaignId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_reports_fundraising_campaigns");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_reports_users");
            });

            modelBuilder.Entity<RevExpDonation>(entity =>
            {
                entity.HasKey(e => e.RevExpId);

                entity.ToTable("rev_exp_donation");

                entity.Property(e => e.RevExpId).HasColumnName("rev_exp_id");

                entity.Property(e => e.CamId).HasColumnName("cam_id");

                entity.Property(e => e.Des).HasColumnName("des");

                entity.Property(e => e.Expenditure).HasColumnName("expenditure");

                entity.Property(e => e.Revenue).HasColumnName("revenue");

                entity.HasOne(d => d.Cam)
                    .WithMany(p => p.RevExpDonations)
                    .HasForeignKey(d => d.CamId)
                    .HasConstraintName("FK_rev_exp_donation_fundraising_campaigns");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(50)
                    .HasColumnName("role_name");
            });

            modelBuilder.Entity<So>(entity =>
            {
                entity.HasKey(e => e.SosId)
                    .HasName("PK__sos__1B03286400D822C9");

                entity.ToTable("sos");

                entity.Property(e => e.SosId).HasColumnName("sos_id");

                entity.Property(e => e.Address).HasColumnName("address");

                entity.Property(e => e.Content)
                    .HasMaxLength(200)
                    .HasColumnName("content");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ImgSos).HasColumnName("img_sos");

                entity.Property(e => e.Latitude).HasColumnName("latitude");

                entity.Property(e => e.Longitude).HasColumnName("longitude");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Sos)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__sos__user_id__74AE54BC");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasIndex(e => e.Email, "UQ__users__AB6E61645C1C78F9")
                    .IsUnique();

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.Active)
                    .HasColumnName("active")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .HasColumnName("address");

                entity.Property(e => e.Birthday)
                    .HasColumnType("date")
                    .HasColumnName("birthday");

                entity.Property(e => e.Careers)
                    .HasMaxLength(50)
                    .HasColumnName("careers");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");

                entity.Property(e => e.FullName)
                    .HasMaxLength(100)
                    .HasColumnName("full_name");

                entity.Property(e => e.Gender).HasColumnName("gender");

                entity.Property(e => e.Img)
                    .HasMaxLength(255)
                    .HasColumnName("img");

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(255)
                    .HasColumnName("password_hash");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(15)
                    .HasColumnName("phone_number");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .HasColumnName("username");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_users_roles");
            });

            modelBuilder.Entity<VolunteerHistory>(entity =>
            {
                entity.HasKey(e => e.ProfileId)
                    .HasName("PK__voluntee__AEBB701F3FF2DF1A");

                entity.ToTable("volunteer_history");

                entity.Property(e => e.ProfileId).HasColumnName("profile_id");

                entity.Property(e => e.Describe).HasColumnName("describe");

                entity.Property(e => e.Evaluate)
                    .HasMaxLength(50)
                    .HasColumnName("evaluate");

                entity.Property(e => e.EventId).HasColumnName("event_id");

                entity.Property(e => e.JoinedAt)
                    .HasColumnType("date")
                    .HasColumnName("joined_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Reasons)
                    .HasMaxLength(500)
                    .HasColumnName("reasons");

                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasColumnName("status");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.VolunteerHistories)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_volunteer_history_fundraising_campaigns");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.VolunteerHistories)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__volunteer__user___76969D2E");
            });

            modelBuilder.Entity<VolunteersRegistration>(entity =>
            {
                entity.HasKey(e => e.VolunteerRegistrationId)
                    .HasName("PK__voluntee__A83983E86E867026");

                entity.ToTable("volunteers_registrations");

                entity.Property(e => e.VolunteerRegistrationId).HasColumnName("volunteer_registration_id");

                entity.Property(e => e.EventId).HasColumnName("event_id");

                entity.Property(e => e.JoinedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("joined_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasColumnName("status");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.VolunteersRegistrations)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__volunteer__event__787EE5A0");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.VolunteersRegistrations)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__volunteer__user___797309D9");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
