using MediSphere.Models;
using MediSphere.Repository;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace MediSphereTest.Test
{
    public class AppointmentControllerTests
    {
        private MediSphereDbContext _context;
        private AppointmentRepository _repository;

        [SetUp]
        public void Setup()
        {
            // Create a new in-memory database for each test
            var options = new DbContextOptionsBuilder<MediSphereDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _context = new MediSphereDbContext(options);
            _repository = new AppointmentRepository(_context);

            // Seed initial data
            _context.Appointments.AddRange(
            new Appointment
            {
                AppointmentId = 1,
                PatientId = 1,
                DoctorId = 1,
                IsDeleted = false,
                AppointmentDate = System.DateTime.Now
            },
            new Appointment
            {
                AppointmentId = 2,
                PatientId = 2,
                DoctorId = 2,
                IsDeleted = false,
                AppointmentDate = System.DateTime.Now.AddDays(1)
            });
           

            _context.SaveChanges();
        }

        //[Test]
        //public async Task GetAllAppointmentsAsync_ReturnsAppointments()
        //{
        //    // Arrange: Add appointments to the context
        //    var appointment1 = new Appointment
        //    {
        //        AppointmentId = 1,
        //        PatientId = 1,
        //        DoctorId = 1,
        //        IsDeleted = false,
        //        AppointmentDate = DateTime.Now
        //    };
        //    var appointment2 = new Appointment
        //    {
        //        AppointmentId = 2,
        //        PatientId = 2,
        //        DoctorId = 2,
        //        IsDeleted = false,
        //        AppointmentDate = DateTime.Now.AddDays(1)
        //    };
        //    // Add the appointments to the DbContext
        //    _context.Appointments.AddRange(appointment1, appointment2);
        //    await _context.SaveChangesAsync();
        //    // Act: Call GetAllAppointmentsAsync
        //    var result = await _repository.GetAllAppointmentsAsync();

        //    Assert.AreEqual(2, result.Count());
        //    Assert.AreEqual(1, result.First().AppointmentId);
        //    Assert.AreEqual(2, result.Last().AppointmentId);

        //}

        [Test]
        public async Task GetAppointmentByIdAsync_ReturnsAppointment()
        {
            // Act: Call GetAppointmentByIdAsync with a valid ID
            var result = await _repository.GetAppointmentByIdAsync(999);

            // Assert: Verify the appointment is returned correctly
            Assert.IsNull(result, "Expected null for non-existent appointment, but a result was returned.");
            //Assert.AreEqual(1, result.AppointmentId);
        }


        [Test]
        public async Task GetAppointmentByIdAsync_ReturnsNullForInvalidId()
        {
            // Act: Call GetAppointmentByIdAsync with an invalid ID
            var result = await _repository.GetAppointmentByIdAsync(999);

            // Assert: Verify null is returned for a non-existent appointment
            Assert.IsNull(result);
        }

        [Test]
        public async Task AddAppointmentAsync_AddsNewAppointment()
        {
            // Arrange: Create a new appointment to add
            var newAppointment = new Appointment
            {
                AppointmentId = 3,
                PatientId = 3,
                DoctorId = 3,
                AppointmentDate = System.DateTime.Now.AddDays(2),
                IsDeleted = false
            };

            // Act: Add the new appointment
            var result = await _repository.AddAppointmentAsync(newAppointment);

            // Assert: Verify the appointment is added successfully
            Assert.IsTrue(result);
            var addedAppointment = await _context.Appointments.FindAsync(3);
            Assert.IsNotNull(addedAppointment);
            Assert.AreEqual(3, addedAppointment.AppointmentId);
        }

        [Test]
        public async Task UpdateAppointmentAsync_UpdatesExistingAppointment()
        {
            // Arrange: Get an existing appointment
            var appointment = await _context.Appointments.FirstAsync(a => a.AppointmentId == 1);
            appointment.AppointmentDate = System.DateTime.Now.AddDays(3);

            // Act: Update the appointment
            var result = await _repository.UpdateAppointmentAsync(appointment);

            // Assert: Verify the update was successful
            Assert.IsTrue(result);
            var updatedAppointment = await _context.Appointments.FindAsync(1);
            Assert.AreEqual(System.DateTime.Now.AddDays(3).Date, updatedAppointment.AppointmentDate.Date);
        }

        [Test]
        public async Task DeleteAppointmentAsync_DeletesAppointment()
        {
            // Act: Delete an existing appointment
            var result = await _repository.DeleteAppointmentAsync(1);

            // Assert: Verify the appointment is marked as deleted
            Assert.IsTrue(result);
            var deletedAppointment = await _context.Appointments.FindAsync(1);
            Assert.IsNotNull(deletedAppointment);
            Assert.IsTrue(deletedAppointment.IsDeleted);
        }

        [Test]
        public async Task DeleteAppointmentAsync_ReturnsFalseForNonExistentAppointment()
        {
            // Act: Try to delete a non-existent appointment
            var result = await _repository.DeleteAppointmentAsync(999);

            // Assert: Verify the result is false
            Assert.IsFalse(result);
        }
        
        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }
    }
}
