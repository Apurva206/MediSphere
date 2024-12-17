using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MediSphere.Models
{
    public class MediSphereDbContext : DbContext
    {
        public MediSphereDbContext() { }

        public MediSphereDbContext(DbContextOptions<MediSphereDbContext> options) : base(options)
        {

        }

        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-5VEHB15;Database=MediSphere;Trusted_Connection=True;" + "TrustServerCertificate=True;Integrated Security=True;");
            base.OnConfiguring(optionsBuilder);
        }*/
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Appointment>()
        .HasQueryFilter(a => !a.IsDeleted);

            //     modelBuilder.Entity<Doctor>()
            // .HasQueryFilter(a => !a.IsDeleted);

            modelBuilder.Entity<MedicalRecord>()
        .HasQueryFilter(a => !a.IsDeleted);

            //     modelBuilder.Entity<Patient>()
            //.HasQueryFilter(a => !a.IsDeleted);

            modelBuilder.Entity<Prescription>()
        .HasQueryFilter(a => !a.IsDeleted);


            modelBuilder.Entity<Appointment>()
       .HasOne<Doctor>(a => a.Doctor) 
       .WithMany(d => d.Appointments)
       .HasForeignKey(a => a.DoctorId)
       .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Appointment>()
                .HasOne<Patient>(a => a.Patient) 
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MedicalRecord>()
                .HasOne<Patient>(m => m.Patient) 
                .WithMany(p => p.MedicalRecords)
                .HasForeignKey(m => m.PatientId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MedicalRecord>()
                .HasOne<Doctor>(m => m.Doctor) 
                .WithMany(d => d.MedicalRecords)
                .HasForeignKey(m => m.DoctorId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MedicalRecord>()
                .HasOne<Appointment>(m => m.Appointment) 
                .WithMany(a => a.MedicalRecords)
                .HasForeignKey(m => m.AppointmentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Prescription>()
                .HasOne<MedicalRecord>(p => p.MedicalRecord) 
                .WithMany(m => m.Prescriptions)
                .HasForeignKey(p => p.RecordId)
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
    }
}
