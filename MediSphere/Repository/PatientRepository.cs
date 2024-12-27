using MediSphere.Models;

namespace MediSphere.Repository
{
    public class PatientRepository : IPatientServices
    {
        private readonly MediSphereDbContext _context;

        public PatientRepository(MediSphereDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Patient> GetAllPatients() => _context.Patients.ToList();

        public void AddPatient(Patient patient) => _context.Patients.Add(patient);
    }
}
