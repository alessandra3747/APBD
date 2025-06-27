using Microsoft.EntityFrameworkCore;
using PrescriptionApplication.Models;

namespace PrescriptionApplication.Data;

public class AppDbContext : DbContext
{
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }
    
    public AppDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Doctor>().HasData(
            new Doctor { IdDoctor = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@hospital.com" },
            new Doctor { IdDoctor = 2, FirstName = "Emily", LastName = "Smith", Email = "emily.smith@hospital.com" }
        );
        
        modelBuilder.Entity<Patient>().HasData(
            new Patient { IdPatient = 1, FirstName = "Michael", LastName = "Brown", Birthdate = new DateTime(1985, 7, 12) },
            new Patient { IdPatient = 2, FirstName = "Sophia", LastName = "Wilson", Birthdate = new DateTime(1992, 3, 28) }
        );
        
        modelBuilder.Entity<Medicament>().HasData(
            new Medicament { IdMedicament = 1, Name = "Paracetamol", Description = "Pain relief and fever reducer", Type = "Tablet" },
            new Medicament { IdMedicament = 2, Name = "Ibuprofen", Description = "Anti-inflammatory", Type = "Tablet" }
        );
        
        modelBuilder.Entity<Prescription>().HasData(
            new Prescription { IdPrescription = 1, Date = new DateTime(2025, 5, 30), DueDate = new DateTime(2025, 6, 30), IdPatient = 1, IdDoctor = 1 },
            new Prescription { IdPrescription = 2, Date = new DateTime(2025, 5, 28), DueDate = new DateTime(2025, 6, 15), IdPatient = 2, IdDoctor = 2 }
        );
        
        modelBuilder.Entity<PrescriptionMedicament>().HasData(
            new PrescriptionMedicament { IdPrescription = 1, IdMedicament = 1, Dose = 2, Details = "Take two tablets daily" },
            new PrescriptionMedicament { IdPrescription = 1, IdMedicament = 2, Dose = 1, Details = "Take one tablet in the evening" },
            new PrescriptionMedicament { IdPrescription = 2, IdMedicament = 2, Dose = 3, Details = "Take three tablets after meals" }
        );
    }
    
}