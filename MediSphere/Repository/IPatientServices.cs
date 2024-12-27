using MediSphere.Models;

namespace MediSphere.Repository
{
    public interface IPatientServices
    {
        IEnumerable<Patient> GetAllPatients();
        void AddPatient(Patient patient);
    }
}
