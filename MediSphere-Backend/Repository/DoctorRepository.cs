using MediSphere.Models;

namespace MediSphere.Repository
{
    public class DoctorRepository : IDoctorServices
    {
        private readonly MediSphereDbContext _context;

        public DoctorRepository(MediSphereDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Doctor> GetAllDoctors() => _context.Doctors.ToList();

        public void AddDoctor(Doctor doctor) => _context.Doctors.Add(doctor);
    }
}
