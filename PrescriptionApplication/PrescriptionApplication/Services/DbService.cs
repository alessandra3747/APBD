using Microsoft.EntityFrameworkCore;
using PrescriptionApplication.Data;
using PrescriptionApplication.DTOs;
using PrescriptionApplication.Exceptions;
using PrescriptionApplication.Models;

namespace PrescriptionApplication.Services;

public interface IDbService
{
    Task<PrescriptionResponseDto> CreatePrescriptionAsync(PrescriptionCreateDto dto);
    Task<PrescriptionResponseDto> GetPrescriptionByIdAsync(int id);
    Task<PatientDetailsDto> GetPatientDetailsAsync(int id);

}

public class DbService(AppDbContext data) : IDbService
{
    
    public async Task<PrescriptionResponseDto> CreatePrescriptionAsync(PrescriptionCreateDto dto)
    {
        
        // CHECK IF MEDS EXIST
        var medicaments = dto.Medicaments.Select(m => m.IdMedicament).Distinct();
        
        var existingMedicaments = await data.Medicaments
            .Where(m => medicaments.Contains(m.IdMedicament))
            .Select(m => m.IdMedicament)
            .ToListAsync();

        var missingMedicaments = medicaments
            .Except(existingMedicaments)
            .ToList();

        if (missingMedicaments.Any())
            throw new NotFoundException($"Following medicaments does not exist: {string.Join(", ", missingMedicaments)}");

        
        //CHECK IF MEDS COUNT IS LESS THAN 10 AND HIGHER THAN 0 (it doesnt make sense to not have any meds on prescription)
        if (dto.Medicaments == null || dto.Medicaments.Count == 0)
            throw new ValidationException("Prescription does not have any medicaments.");
        if (dto.Medicaments.Count > 10)
            throw new ValidationException("Prescription cannot have more than 10 medicaments.");

        
        // CHECK IF DueDate>=Date
        if (dto.DueDate < dto.Date)
            throw new ValidationException("DueDate must be >= Date.");
        
        // CHECK IF DOCTOR EXISTS
        var doctor = await data.Doctors.FirstOrDefaultAsync(d => d.IdDoctor == dto.IdDoctor);
        if (doctor == null)
            throw new NotFoundException($"Doctor with id {dto.IdDoctor} does not exist.");
        
        
        // ADDING PATIENT IF HE DOESN'T EXIST, ADDING PRESCRIPTION
        using var transaction = await data.Database.BeginTransactionAsync();
        try
        {
            Patient? patient = null;

            if (dto.Patient.IdPatient.HasValue)
            {
                patient = await data.Patients.FirstOrDefaultAsync(p => p.IdPatient == dto.Patient.IdPatient);
            }

            if (patient == null)
            {
                patient = new Patient
                {
                    FirstName = dto.Patient.FirstName,
                    LastName = dto.Patient.LastName,
                    Birthdate = dto.Patient.Birthdate
                };
                await data.Patients.AddAsync(patient);
                await data.SaveChangesAsync();
            }

            var prescription = new Prescription
            {
                IdPatient = patient.IdPatient,
                IdDoctor = doctor.IdDoctor,
                Date = dto.Date,
                DueDate = dto.DueDate
            };
            await data.Prescriptions.AddAsync(prescription);
            await data.SaveChangesAsync();

            var prescriptionMedicaments = dto.Medicaments.Select(m => new PrescriptionMedicament
            {
                IdPrescription = prescription.IdPrescription,
                IdMedicament = m.IdMedicament,
                Dose = m.Dose,
                Details = m.Description
            }).ToList();

            await data.PrescriptionMedicaments.AddRangeAsync(prescriptionMedicaments);
            await data.SaveChangesAsync();

            await transaction.CommitAsync();
            
            var result = await data.Prescriptions
                .Where(p => p.IdPrescription == prescription.IdPrescription)
                .Select(p => new PrescriptionResponseDto
                {
                    IdPrescription = p.IdPrescription,
                    Date = p.Date,
                    DueDate = p.DueDate,
                    Doctor = new DoctorDto
                    {
                        IdDoctor = p.Doctor.IdDoctor,
                        FirstName = p.Doctor.FirstName,
                        LastName = p.Doctor.LastName,
                        Email = p.Doctor.Email
                    },
                    Medicaments = p.PrescriptionMedicaments.Select(pm => new MedicamentDto
                    {
                        IdMedicament = pm.IdMedicament,
                        Name = pm.Medicament.Name,
                        Description = pm.Medicament.Description,
                        Dose = pm.Dose,
                        Details = pm.Details
                    }).ToList()
                }).FirstAsync();

            return result;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    
    public async Task<PrescriptionResponseDto> GetPrescriptionByIdAsync(int id)
    {
        var prescription = await data.Prescriptions
            .Where(p => p.IdPrescription == id)
            .Select(p => new PrescriptionResponseDto
            {
                IdPrescription = p.IdPrescription,
                Date = p.Date,
                DueDate = p.DueDate,
                Doctor = new DoctorDto
                {
                    IdDoctor = p.Doctor.IdDoctor,
                    FirstName = p.Doctor.FirstName,
                    LastName = p.Doctor.LastName,
                    Email = p.Doctor.Email
                },
                Medicaments = p.PrescriptionMedicaments.Select(pm => new MedicamentDto
                {
                    IdMedicament = pm.IdMedicament,
                    Name = pm.Medicament.Name,
                    Description = pm.Medicament.Description,
                    Dose = pm.Dose,
                    Details = pm.Details
                }).ToList()
            }).FirstOrDefaultAsync();

        if (prescription is null)
            throw new NotFoundException("Prescription does not exist.");

        return prescription;
    }

    public async Task<PatientDetailsDto> GetPatientDetailsAsync(int id)
    {
        var patient = await data.Patients
            .Where(p => p.IdPatient == id)
            .Select(p => new PatientDetailsDto
            {
                IdPatient = p.IdPatient,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Birthdate = p.Birthdate,
                Prescriptions = p.Prescriptions
                    .OrderBy(pr => pr.DueDate)
                    .Select(pr => new PrescriptionResponseDto
                    {
                        IdPrescription = pr.IdPrescription,
                        Date = pr.Date,
                        DueDate = pr.DueDate,
                        Doctor = new DoctorDto
                        {
                            IdDoctor = pr.Doctor.IdDoctor,
                            FirstName = pr.Doctor.FirstName,
                            LastName = pr.Doctor.LastName,
                            Email = pr.Doctor.Email
                        },
                        Medicaments = pr.PrescriptionMedicaments.Select(pm => new MedicamentDto
                        {
                            IdMedicament = pm.IdMedicament,
                            Name = pm.Medicament.Name,
                            Description = pm.Medicament.Description,
                            Dose = pm.Dose,
                            Details = pm.Details
                        }).ToList()
                    }).ToList()
            }).FirstOrDefaultAsync();

        if (patient is null)
            throw new NotFoundException("Patient does not exist.");

        return patient;
    }
    
        
}