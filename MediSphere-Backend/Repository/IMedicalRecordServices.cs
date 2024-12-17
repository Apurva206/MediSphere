using MediSphere.Models;

namespace MediSphere.Repository
{
    public interface IMedicalRecordServices
    {
        Task<MedicalRecord> GetByIdAsync(int id); // Get medical record by ID
        Task<bool> AddAsync(MedicalRecord medicalRecord); // Add a new medical record
        Task<bool> UpdateAsync(MedicalRecord medicalRecord); // Update an existing medical record
        Task<bool> DeleteAsync(int id); // Delete a medical record by ID
    }
}
