using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AiTech.Domains;
using AITechDATA.Domain;
using AITechDATA.Tools;

namespace AITechDATA.DataLayer
{
    public class AiITechContext : DbContext
    {
        public DbSet<Address> Addresses { get; set; }
        public DbSet<AdminReport> AdminReports { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<FileUpload> FileUploads { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<LoginMethod> LoginMethods { get; set; }
        public DbSet<PermissionRole> PermissionRoles { get; set; }
        public DbSet<UserCourse> UserCourses { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<PaymentHistory> PaymentHistories { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PreRegistration> PreRegistrations { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<SessionAssignment> SessionAssignments { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<StudentDetails> StudentDetails { get; set; }
        public DbSet<TeacherResume> TeacherResumes { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketMessage> TicketMessages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Token> Tokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configHelper = new ConfigurationHelper();
            string _connectionString = configHelper.GetConnectionString("publicdb");
            optionsBuilder.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentDetails>()
                .HasOne(sd => sd.User) // یک StudentDetails به یک User تعلق دارد
                .WithOne(u => u.StudentDetails) // یک User می‌تواند یک StudentDetails داشته باشد
                .HasForeignKey<StudentDetails>(sd => sd.UserId); // کلید خارجی UserId

            modelBuilder.Entity<Group>()
                .HasOne(g => g.Teacher)
                .WithMany()
                .HasForeignKey(g => g.TeacherId)
                .OnDelete(DeleteBehavior.Restrict); // یا DeleteBehavior.NoAction

            modelBuilder.Entity<FileUpload>()
               .HasOne(x => x.Assignment)
               .WithMany(x => x.Files)
               .HasForeignKey(x => x.AssignmentId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Course>()
             .HasOne(x => x.Category)
             .WithMany(x => x.Courses)
             .HasForeignKey(x => x.CategoryId)
             .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Address>()
             .HasOne(x => x.City)
             .WithMany(x => x.Addresses)
             .HasForeignKey(x => x.CityID)
             .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserCourse>()
            .HasOne(x => x.User)
            .WithMany(x => x.UserCourses)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserCourse>()
           .HasOne(x => x.Course)
           .WithMany(x => x.UserCourses)
           .HasForeignKey(x => x.CourseId)
           .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserGroup>()
         .HasOne(x => x.User)
         .WithMany(x => x.UserGroups)
         .HasForeignKey(x => x.UserId)
         .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserGroup>()
           .HasOne(x => x.Group)
           .WithMany(x => x.Students)
           .HasForeignKey(x => x.GroupId)
           .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PermissionRole>()
          .HasOne(x => x.Permission)
          .WithMany(x => x.PermissionRoles)
          .HasForeignKey(x => x.PerrmissionId)
          .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PermissionRole>()
          .HasOne(x => x.Role)
          .WithMany(x => x.PermissionRoles)
          .HasForeignKey(x => x.RoleId)
          .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Assignment>()
          .HasOne(x => x.User)
          .WithMany(x => x.Assignments)
          .HasForeignKey(x => x.UserId)
          .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Assignment>()
          .HasOne(x => x.SessionAssignment)
          .WithMany(x => x.Assignments)
          .HasForeignKey(x => x.SessionAssignmentId)
          .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Attendance>()
          .HasOne(x => x.User)
          .WithMany(x => x.Attendances)
          .HasForeignKey(x => x.UserId)
          .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Attendance>()
          .HasOne(x => x.Session)
          .WithMany(x => x.Attendances)
          .HasForeignKey(x => x.SessionId)
          .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Group>()
          .HasOne(x => x.Course)
          .WithMany(x => x.Groups)
          .HasForeignKey(x => x.CourseId)
          .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Session>()
         .HasOne(x => x.Group)
         .WithMany(x => x.Sessions)
         .HasForeignKey(x => x.GroupId)
         .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PreRegistration>()
        .HasOne(x => x.Group)
        .WithMany(x => x.PreRegistrations)
        .HasForeignKey(x => x.GroupId)
        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PaymentHistory>()
   .HasOne(x => x.Group)
   .WithMany(x => x.PaymentHistories)
   .HasForeignKey(x => x.GroupId)
   .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PaymentHistory>()
   .HasOne(x => x.User)
   .WithMany(x => x.PaymentHistories)
   .HasForeignKey(x => x.UserId)
   .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
   .HasOne(x => x.Role)
   .WithMany(x => x.Users)
   .HasForeignKey(x => x.RoleId)
   .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SessionAssignment>()
   .HasOne(x => x.Session)
   .WithMany(x => x.SessionAssignments)
   .HasForeignKey(x => x.SessionId)
   .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Setting>()
   .HasOne(x => x.Parent)
   .WithMany(x => x.Children)
   .HasForeignKey(x => x.ParentId)
   .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Parent>()
.HasOne(x => x.StudentDetails)
.WithMany(x => x.Parents)
.HasForeignKey(x => x.StudentDetailsId)
.OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TicketMessage>()
.HasOne(x => x.Ticket)
.WithMany(x => x.Messages)
.HasForeignKey(x => x.TicketId)
.OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TicketMessage>()
.HasOne(x => x.Admin)
.WithMany(x => x.TicketMessages)
.HasForeignKey(x => x.AdminId)
.OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TeacherResume>()
.HasOne(x => x.User)
.WithMany(x => x.TeacherResumes)
.HasForeignKey(x => x.UserId)
.OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LoginMethod>()
.HasOne(x => x.User)
.WithMany(x => x.LoginMethods)
.HasForeignKey(x => x.UserId)
.OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Event>()
.HasOne(x => x.User)
.WithMany(x => x.Events)
.HasForeignKey(x => x.UserId)
.OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<News>()
.HasOne(x => x.User)
.WithMany(x => x.News)
.HasForeignKey(x => x.UserId)
.OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
.HasOne(x => x.User)
.WithMany(x => x.Notifications)
.HasForeignKey(x => x.UserId)
.OnDelete(DeleteBehavior.Cascade);

        }
    }
}