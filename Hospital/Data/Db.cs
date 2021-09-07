using Hospital.Models;
using Hospital.Models.Identity;
using Microsoft.EntityFrameworkCore;
using System;

namespace Hospital.Data
{
    public class Db : DbContext
    {
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Record> Records { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Specialty> Specialties { get; set; }

        public DbSet<Student> Students { get; set; }
        public DbSet<User> Users { get; set; }

        public Db()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Hospital;Trusted_Connection=True;");
        }
    }
}
