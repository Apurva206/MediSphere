using Microsoft.AspNetCore.Mvc;
using MediSphere.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace MediSphere.Controllers
{
    [Authorize]  // Applied globally to require authentication for all actions
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly MediSphereDbContext _context;

        public MedicalRecordsController(MediSphereDbContext context)
        {
            _context = context;
        }

        // GET: api/MedicalRecords
        // Doctors can view all medical records for their patients
        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicalRecord>>> GetMedicalRecords()
        {



            // Query medical records for the current doctor
            var result = await _context.MedicalRecords

           //.Include(r => r.Patient)
           //.Include(r => r.Doctor)
           //.Include(r => r.Appointment)
           .ToListAsync();
            return result;
        } 

        // GET: api/MedicalRecords/5
        // Doctors can view medical records of their patients, and patients can view their own records
        [Authorize(Roles = "Doctor,Patient")]
        [HttpGet("{id}")]
        public async Task<ActionResult<MedicalRecord>> GetMedicalRecord(int id)
        {
            // Get the authenticated user's username
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized(new { error = "User identity is not available." });
            }

            // Determine if the user is a Doctor or a Patient
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.FullName == username);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.FullName == username);

            if (doctor == null && patient == null)
            {
                return Unauthorized(new { error = "Authenticated user not found in the system." });
            }

            // Fetch the medical record that matches the ID and belongs to the authenticated user
            var medicalRecord = await _context.MedicalRecords
                .Include(r => r.Patient)
                //.Include(r => r.Appointment)
                .FirstOrDefaultAsync(r =>
                    r.RecordId == id &&
                    ((doctor != null && r.DoctorId == doctor.DoctorId) || (patient != null && r.PatientId == patient.PatientId)));

            if (medicalRecord == null)
            {
                return NotFound(new { error = "Medical record not found or you are not authorized to access it." });
            }

            return Ok(medicalRecord);
        }


        // POST: api/MedicalRecords
        // Only Doctors can create new medical records
        [Authorize(Roles = "Doctor")]
        [HttpPost]
        public async Task<ActionResult<MedicalRecord>> PostMedicalRecord(MedicalRecord medicalRecord)
        {
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.FullName == User.Identity.Name);
            if (doctor == null)
            {
                return NotFound("Doctor not found.");
            }

            // Set the DoctorId
            medicalRecord.DoctorId = doctor.DoctorId;

            _context.MedicalRecords.Add(medicalRecord);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMedicalRecord", new { id = medicalRecord.RecordId }, medicalRecord);
        }

        // PUT: api/MedicalRecords/5
        // Doctors can modify their patients' records, and Patients can modify their own record
        [Authorize(Roles = "Doctor,Patient")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMedicalRecord(int id, MedicalRecord medicalRecord)
        {
            if (id != medicalRecord.RecordId)
                return BadRequest("Medical record ID mismatch.");

            var existingRecord = await _context.MedicalRecords.FindAsync(id);
            if (existingRecord == null)
                return NotFound("Medical record not found.");

            // Authorization checks
            if (User.IsInRole("Doctor"))
            {
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.FullName == User.Identity.Name);
                if (doctor == null)
                    return Unauthorized(new { message = "Doctor not found." });

                if (existingRecord.DoctorId != doctor.DoctorId)
                    return Forbid();
            }

            if (User.IsInRole("Patient"))
            {
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.FullName == User.Identity.Name);
                if (patient == null)
                    return Unauthorized(new { message = "Patient not found." });

                if (existingRecord.PatientId != patient.PatientId)
                    return Forbid();
            }

            // Set the entity state to Modified
            _context.Entry(existingRecord).CurrentValues.SetValues(medicalRecord);

            try
            {
                var affectedRows = await _context.SaveChangesAsync();
                if (affectedRows == 0)
                {
                    return StatusCode(500, "Update failed. No changes were made.");
                }
            }
            catch (DbUpdateConcurrencyException) when (!_context.MedicalRecords.Any(e => e.RecordId == id))
            {
                return NotFound("Medical record not found.");
            }

            // Return the updated medical record data
            var updatedRecord = await _context.MedicalRecords
                .Include(mr => mr.Patient)
                .Include(mr => mr.Doctor)
                .FirstOrDefaultAsync(mr => mr.RecordId == id);

            return Ok(updatedRecord); // This returns the updated medical record
        }

        // DELETE: api/MedicalRecords/5
        // Doctors can delete their patients' medical records, and Patients can delete their own records
        [Authorize(Roles = "Doctor,Patient")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicalRecord(int id)
        {
            // Find the medical record
            var medicalRecord = await _context.MedicalRecords.FindAsync(id);
            if (medicalRecord == null)
            {
                return NotFound(new { error = " Prescription not found." });
            }
            // Role-based access control
            if (User.IsInRole("Doctor"))
            {
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.FullName == User.Identity.Name);
                if (doctor == null || medicalRecord.DoctorId != doctor.DoctorId)
                {
                    return Unauthorized(new { error = "Doctor cannot delete another doctor's medical record." });
                }
            }

            if (User.IsInRole("Patient"))
            {
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.FullName == User.Identity.Name);
                if (patient == null || medicalRecord.PatientId != patient.PatientId)
                {
                    return Unauthorized(new { error = "Patient cannot delete another patient's prescription." });
                }
            }

            //_context.Prescriptions.Remove(prescription);
            medicalRecord.IsDeleted = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
