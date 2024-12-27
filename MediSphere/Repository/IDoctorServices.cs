using MediSphere.Models;

namespace MediSphere.Repository
{
    public interface IDoctorServices
    {
        IEnumerable<Doctor> GetAllDoctors();
        void AddDoctor(Doctor doctor);
    }
}
