using Microsoft.AspNetCore.Mvc;
using MediSphere.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using log4net;

namespace MediSphere.Controllers
{
    [Authorize]  // Applied globally to require authentication for all actions
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly MediSphereDbContext _context;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(AppointmentsController));

        public AppointmentsController(MediSphereDbContext context)
        {
            _context = context;
        }

        // GET: api/Appointments
        // Only Doctors can view all appointments (their own patients' appointments)
        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
        {
            _logger.Info("Executing get apointments function");
            return await _context.Appointments
               // .Include(a => a.Patient)
                //.Include(a => a.Doctor)
                .ToListAsync();
        }


        // GET: api/Appointments/5
        // Doctors can view their own patients' appointments, and patients can view their own appointments
        [Authorize(Roles = "Doctor,Patient")]
        [HttpGet("{id}")] 
        public async Task<ActionResult<Appointment>> GetAppointment(int id)
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

                // Fetch the appointment that matches the ID and belongs to the authenticated user
                var appointment = await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .FirstOrDefaultAsync(a =>
                        a.AppointmentId == id &&
                        ((doctor != null && a.DoctorId == doctor.DoctorId) || (patient != null && a.PatientId == patient.PatientId)));

                if (appointment == null)
                {
                    return NotFound(new { error = "Appointment not found or you are not authorized to access it." });
                }

                return Ok(appointment);
            }


            // POST: api/Appointments
            // Only Doctors can create new appointments
            [Authorize(Roles = "Doctor,Patient")]
        [HttpPost]
        public async Task<ActionResult<Appointment>> PostAppointment(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetAppointment", new { id = appointment.AppointmentId }, appointment);
        }
        


        // PUT: api/Appointments/5
        // Doctors can modify their own appointments, Patients can only modify their own appointments
        [Authorize(Roles = "Doctor,Patient")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppointment(int id, Appointment appointment)
        {
            if (id != appointment.AppointmentId)
                return BadRequest("Appointment ID mismatch.");

            var existingAppointment = await _context.Appointments.FindAsync(id);
            if (existingAppointment == null)
                return NotFound("Appointment not found.");

            // Authorization checks
            if (User.IsInRole("Doctor"))
            {
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.FullName == User.Identity.Name);
                if (doctor == null)
                    //return Forbid("Doctor not found.");
                    return Unauthorized(new { message = "Doctor not found." });

                if (existingAppointment.DoctorId != doctor.DoctorId)
                    return Forbid();
            }

            if (User.IsInRole("Patient"))
            {
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.FullName == User.Identity.Name);
                if (patient == null)
                    return Unauthorized(new { message = "Patient not found." });

                if (existingAppointment.PatientId != patient.PatientId)
                    return Forbid();
            }

            // Set the entity state to Modified
            _context.Entry(existingAppointment).CurrentValues.SetValues(appointment);

            try
            {
                var affectedRows = await _context.SaveChangesAsync();
                if (affectedRows == 0)
                {
                    return StatusCode(500, "Update failed. No changes were made.");
                }
            }
            catch (DbUpdateConcurrencyException) when (!_context.Appointments.Any(e => e.AppointmentId == id))
            {
                return NotFound("Appointment not found.");
            }

            // Return the updated appointment data
            var updatedAppointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            return Ok(updatedAppointment);  // This returns the updated appointment
        }




        // DELETE: api/Appointments/5
        // Doctors can delete their own appointments, Patients can delete their own appointments
        // [Authorize(Roles = "Doctor,Patient")]
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteAppointment(int id)
        //{
        //    var appointment = await _context.Appointments.FindAsync(id);

        //    if (appointment == null) 
        //        return NotFound();

        //    // Ensure that the user is allowed to delete the appointment
        //    if (User.IsInRole("Doctor") && appointment.DoctorId.ToString() != User.Identity.Name)
        //    {
        //        return Forbid();  // Doctor cannot delete another doctor's appointment
        //    }

        //    if (User.IsInRole("Patient") && appointment.PatientId.ToString() != User.Identity.Name)
        //    {
        //        return Forbid();  // Patient cannot delete another patient's appointment
        //    }

        //    // _context.Appointments.Remove(appointment);
        //    // Soft delete
        //    appointment.IsDeleted = true;
        //    await _context.SaveChangesAsync();
        //    return NoContent();
        //}
        [Authorize(Roles = "Doctor,Patient")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound(new { error = "Appointment not found." });
            }

            // Check if the user is a Doctor
            if (User.IsInRole("Doctor"))
            {
                // Ensure the doctor can only delete their own appointments
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.FullName == User.Identity.Name);
                if (doctor == null || appointment.DoctorId != doctor.DoctorId)
                {
                    return Unauthorized(new { error = "Doctor cannot delete another doctor's appointment." });
                }
            }

            // Check if the user is a Patient
            if (User.IsInRole("Patient"))
            {
                // Ensure the patient can only delete their own appointments
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.FullName == User.Identity.Name);
                if (patient == null || appointment.PatientId != patient.PatientId)
                {
                    return Unauthorized(new { error = "Patient cannot delete another patient's appointment." });
                }
            }

            // Soft delete the appointment
            appointment.IsDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent(); // Return NoContent for successful soft delete
        }



    }
}
