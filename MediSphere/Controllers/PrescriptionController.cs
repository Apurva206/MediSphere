using Microsoft.AspNetCore.Mvc;
using MediSphere.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace MediSphere.Controllers
{
    [Authorize]  // Applied globally to require authentication for all actions
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionsController : ControllerBase
    {
        private readonly MediSphereDbContext _context;

        public PrescriptionsController(MediSphereDbContext context)
        {
            _context = context;
        }

        // GET: api/Prescriptions
        // Doctors can view prescriptions for their patients
        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Prescription>>> GetPrescriptions()
        {
            return await _context.Prescriptions
                //.Include(p => p.MedicalRecord)
                /*.Where(p => p.MedicalRecord.DoctorId == int.Parse(User.Identity.Name)) // O*/
                .ToListAsync();
        }

        // GET: api/Prescriptions/5
        // Doctors can view prescriptions for their patients, and patients can view their own prescriptions
        [Authorize(Roles = "Doctor,Patient")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Prescription>> GetPrescription(int id)
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

            // Fetch the prescription that matches the ID and belongs to the authenticated user
            var prescription = await _context.Prescriptions
                //.Include(p => p.MedicalRecord)
                //.ThenInclude(mr => mr.Patient)
                .FirstOrDefaultAsync(p =>
                    p.PrescriptionId == id &&
                    ((doctor != null && p.MedicalRecord.DoctorId == doctor.DoctorId) || (patient != null && p.MedicalRecord.PatientId == patient.PatientId)));

            if (prescription == null)
            {
                return NotFound(new { error = "Prescription not found or you are not authorized to access it." });
            }

            return Ok(prescription);
        }


        // POST: api/Prescriptions
        // Only Doctors can create prescriptions
        [Authorize(Roles = "Doctor")]
        [HttpPost]
        public async Task<ActionResult<Prescription>> PostPrescription(Prescription prescription)
        {
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.FullName == User.Identity.Name);
            if (doctor == null)
            {
                return NotFound("Doctor not found.");
            }

            // Ensure the MedicalRecord exists
            var medicalRecord = await _context.MedicalRecords.FirstOrDefaultAsync(mr => mr.RecordId == prescription.RecordId);
            if (medicalRecord == null)
            {
                return NotFound("Medical record not found.");
            }

            // Ensure that the doctor is associated with the medical record
            if (medicalRecord.DoctorId != doctor.DoctorId)
            {
                return Forbid("You are not authorized to add a prescription for this medical record.");
            }

            // Set the MedicalRecord for the prescription
            prescription.MedicalRecord = medicalRecord;

            // Add the prescription to the database
            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPrescription", new { id = prescription.PrescriptionId }, prescription);
        }

        // PUT: api/Prescriptions/5
        // Doctors can update prescriptions for their patients, and Patients can update their own prescriptions
        [Authorize(Roles = "Doctor,Patient")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrescription(int id, Prescription prescription)
        {
            if (id != prescription.PrescriptionId)
                return BadRequest("Prescription ID mismatch.");

            var existingPrescription = await _context.Prescriptions
                .Include(p => p.MedicalRecord) // Ensure MedicalRecord is included
        .FirstOrDefaultAsync(p => p.PrescriptionId == id);

            if (existingPrescription == null)
                return NotFound("Prescription not found.");

            // Authorization checks
            if (User.IsInRole("Doctor"))
            {
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.FullName == User.Identity.Name);
                if (doctor == null)
                    return Forbid("Doctor not found.");

                /*if (MedicalRecord.DoctorId != doctor.DoctorId)
                    return Forbid("You are not authorized to modify this prescription.");*/
            }

            if (User.IsInRole("Patient"))
            {
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.FullName == User.Identity.Name);
                if (patient == null)
                    return Forbid("Patient not found.");

                //if (existingPrescription.MedicalRecord.PatientId != patient.PatientId)
                //    return Forbid("You are not authorized to modify this prescription.");
            }

            // Set the entity state to Modified
            _context.Entry(existingPrescription).CurrentValues.SetValues(prescription);

            try
            {
                var affectedRows = await _context.SaveChangesAsync();
                if (affectedRows == 0)
                {
                    return StatusCode(500, "Update failed. No changes were made.");
                }
            }
            catch (DbUpdateConcurrencyException) when (!_context.Prescriptions.Any(e => e.PrescriptionId == id))
            {
                return NotFound("Prescription not found.");
            }

            return NoContent(); // Return no content to indicate a successful update
        }

        // DELETE: api/Prescriptions/5
        // Doctors can delete prescriptions for their patients, and Patients can delete their own prescriptions
        [Authorize(Roles = "Doctor,Patient")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrescription(int id)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null)
            {
                return NotFound(new { error = " Prescription not found."});
            }
            // Ensure that the user is allowed to delete the prescription
            if (User.IsInRole("Doctor"))
            {
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.FullName == User.Identity.Name);
                    if (doctor == null || prescription.RecordId != doctor.DoctorId)
                {
                    return Unauthorized(new { error = "Doctor cannot delete another doctor's prescription." });
                }
            }

            if (User.IsInRole("Patient"))
            {
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.FullName == User.Identity.Name);
                if (patient == null || prescription.RecordId != patient.PatientId)
                {
                    return Unauthorized(new { error = "Patient cannot delete another patient's prescription." });
                }
            }

            //_context.Prescriptions.Remove(prescription);
            prescription.IsDeleted = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
