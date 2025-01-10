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
        public DbSet<Course> Courses { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<FileUpload> FileUploads { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<LoginMethod> LoginMethods { get; set; }
        public DbSet<PermissionRole> PermissionRoles { get; set; }
        public DbSet<UserCourse> UserCourses { get; set; }
        public DbSet<News> News { get; set; }
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
        }
    }
}