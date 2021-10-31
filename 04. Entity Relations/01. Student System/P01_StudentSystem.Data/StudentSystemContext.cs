using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace P01_StudentSystem.Data
{
    public class StudentSystemContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Homework> HomeworkSubmissions { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public StudentSystemContext(DbContextOptions options) : base(options)
        {
        }

        public StudentSystemContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.CONNECTION_STRING);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>(e =>
            {
                e.Property(s => s.Name)
                .IsUnicode();

                e.Property(s => s.PhoneNumber)
                .IsUnicode(false);
            });

            modelBuilder.Entity<Course>(e =>
            {
                e.Property(c => c.Name)
                .IsUnicode();

                e.Property(c => c.Description)
                .IsUnicode();
            });

            modelBuilder.Entity<Resource>(e =>
            {
                e.Property(r => r.Name)
                .IsUnicode();

                e.Property(r => r.Url)
                .IsUnicode(false);
            });

            modelBuilder.Entity<Homework>(e =>
            {
                e.Property(h => h.Content)
                .IsUnicode(false);
            });

            modelBuilder.Entity<StudentCourse>(e =>
            {
                e.HasKey(sc => new { sc.StudentId, sc.CourseId });
            });
        }
    }
}
