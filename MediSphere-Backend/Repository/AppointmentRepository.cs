using MediSphere.Models;
using Microsoft.EntityFrameworkCore;

namespace MediSphere.Repository
{
    public class AppointmentRepository : IAppointmentServices
    {
        private readonly MediSphereDbContext _dbContext;

        public AppointmentRepository(MediSphereDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Get all appointments
        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
        {
            return await _dbContext.Appointments
                .Include(a => a.Patient) // Include related Patient data
                .Include(a => a.Doctor)  // Include related Doctor data
                .Where(a => !a.IsDeleted) // Exclude soft-deleted records
                .ToListAsync();
        }

        // Get a specific appointment by ID
        public async Task<Appointment> GetAppointmentByIdAsync(int id)
        {
            return await _dbContext.Appointments
                .Include(a => a.Patient) // Include related Patient data
                .Include(a => a.Doctor)  // Include related Doctor data
                .FirstOrDefaultAsync(a => a.AppointmentId == id);
        }

        // Add a new appointment
        public async Task<bool> AddAppointmentAsync(Appointment appointment)
        {
            await _dbContext.Appointments.AddAsync(appointment); // Add to DbContext
            return await _dbContext.SaveChangesAsync() > 0; // Save changes and return success
        }

        // Update an existing appointment
        public async Task<bool> UpdateAppointmentAsync(Appointment appointment)
        {
            // Mark the entity as modified
            _dbContext.Entry(appointment).State = EntityState.Modified;

            // Save changes and return success
            return await _dbContext.SaveChangesAsync() > 0;
        }

        // Delete an appointment by ID
        public async Task<bool> DeleteAppointmentAsync(int id)
        {
            // Find the appointment by ID
            var appointment = await _dbContext.Appointments.FindAsync(id);

            if (appointment == null) 
                return false; // Return false if not found

            // Remove the appointment and save changes
            //_dbContext.Appointments.Remove(appointment);
            appointment.IsDeleted = true; // Mark as deleted
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}