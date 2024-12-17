using BilgiYonetimSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BilgiYonetimSistemi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options){}

        public DbSet<Advisors> Advisors { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Transcripts> Transcripts { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<CourseSelection> CourseSelection { get; set; }
        public DbSet<CourseSelectionHistory> CourseSelectionHistory { get; set; }
        public DbSet<CourseCapacity> CourseCapacity { get; set; }
        public DbSet<NonConfirmedSelections> NonConfirmedSelections { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CourseSelection>()
                .HasOne(scs => scs.Student)
                .WithMany(s => s.CourseSelection)
                .HasForeignKey(scs => scs.StudentID);

            modelBuilder.Entity<CourseSelection>()
                .HasOne(scs => scs.Course)
                .WithMany(c => c.CourseSelection)
                .HasForeignKey(scs => scs.CourseID);

            modelBuilder.Entity<Student>()
                 .HasOne(s => s.Advisors)
                 .WithMany(a => a.Students)
                 .HasForeignKey(s => s.AdvisorID)
                 .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Advisors>()
                 .HasMany(a => a.Students)
                 .WithOne(s => s.Advisors)
                 .HasForeignKey(s => s.AdvisorID)
                 .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CourseSelectionHistory>()
                 .HasOne(c => c.Student)
                 .WithMany()
                 .HasForeignKey(c => c.StudentID);

            modelBuilder.Entity<Student>()
                 .HasMany(s => s.CourseSelection)
                 .WithOne(scs => scs.Student)
                 .HasForeignKey(scs => scs.StudentID)
                 .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CourseCapacity>()
                 .HasOne(cc => cc.Course)
                 .WithMany()
                 .HasForeignKey(cc => cc.CourseID);  

            modelBuilder.Entity<CourseSelection>()
                .HasKey(cs => new { cs.CourseID, cs.StudentID }); // Composite key

            modelBuilder.Entity<CourseSelection>()
                .HasOne(cs => cs.Course)
                .WithMany(c => c.CourseSelection)
                .HasForeignKey(cs => cs.CourseID);

            modelBuilder.Entity<CourseSelection>()
                .HasOne(cs => cs.Student)
                .WithMany(s => s.CourseSelection)
                .HasForeignKey(cs => cs.StudentID);

        }

    }
}
