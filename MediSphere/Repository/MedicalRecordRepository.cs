using MediSphere.Models;
using Microsoft.EntityFrameworkCore;

namespace MediSphere.Repository
{
    public class MedicalRecordRepository : IMedicalRecordServices
    {
        private readonly MediSphereDbContext _context;

        public MedicalRecordRepository(MediSphereDbContext context)
        {
            _context = context;
        }

        // Get medical record by ID
        public async Task<MedicalRecord> GetByIdAsync(int id)
        {
            return await _context.MedicalRecords
                .Include(r => r.Patient)
                .Include(r => r.Doctor)
                .Include(r => r.Appointment)
                .Where(r => !r.IsDeleted) // Exclude soft-deleted records
                .FirstOrDefaultAsync(r => r.RecordId == id); 
        }

        // Add a new medical record
        public async Task<bool> AddAsync(MedicalRecord medicalRecord)
        {
            _context.MedicalRecords.Add(medicalRecord);
            return await _context.SaveChangesAsync() > 0;
        }

        // Update an existing medical record
        public async Task<bool> UpdateAsync(MedicalRecord medicalRecord)
        {
            var existingRecord = await _context.MedicalRecords.FindAsync(medicalRecord.RecordId);
            if (existingRecord == null)
            {
                return false; // Record not found or is soft-deleted
            }

            _context.Entry(existingRecord).CurrentValues.SetValues(medicalRecord);
            return await _context.SaveChangesAsync() > 0;
        }

        // Delete a medical record by ID (soft delete)
        public async Task<bool> DeleteAsync(int id)
        {
            var medicalRecord = await _context.MedicalRecords.FindAsync(id);
            if (medicalRecord == null )
            {
                return false; // Record not found or already deleted
            }

            medicalRecord.IsDeleted = true; // Perform a soft delete

            return await _context.SaveChangesAsync() > 0;
        }
    }
}

