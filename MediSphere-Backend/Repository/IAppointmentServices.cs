using MediSphere.Models;

namespace MediSphere.Repository
{
    public interface IAppointmentServices
    {
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();  // Fetch all appointments
        Task<Appointment> GetAppointmentByIdAsync(int id);         // Fetch an appointment by ID
        Task<bool> AddAppointmentAsync(Appointment appointment);   // Add a new appointment
        Task<bool> UpdateAppointmentAsync(Appointment appointment); // Update an existing appointment
        Task<bool> DeleteAppointmentAsync(int id);


    }
}
