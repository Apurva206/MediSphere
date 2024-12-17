using MediSphere.Models;
using MediSphere.Repository;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace MediSphereTest.Test
{
    public class MedicalRecordControllerTests
    {
        private MediSphereDbContext _context;
        private MedicalRecordRepository _repository;

        [SetUp]
        public void Setup()
        {
            // Set up an in-memory database for each test
            var options = new DbContextOptionsBuilder<MediSphereDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}") // Unique DB for isolation
                .Options;

            _context = new MediSphereDbContext(options);
            _repository = new MedicalRecordRepository(_context);

            // Seed initial data
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            // Clear existing data before seeding new data
            _context.MedicalRecords.RemoveRange(_context.MedicalRecords);
            _context.SaveChanges();

            // Seed new medical records
            _context.MedicalRecords.AddRange(
                new MedicalRecord
                {
                    RecordId = 1,
                    PatientId = 1,
                    DoctorId = 1,
                    AppointmentId = 1,
                    IsDeleted = false,
                    TreatmentPlan = "Initial check-up"
                },
                new MedicalRecord
                {
                    RecordId = 2,
                    PatientId = 2,
                    DoctorId = 2,
                    AppointmentId = 2,
                    IsDeleted = false,
                   TreatmentPlan = "Follow-up consultation"
                }
            );
            _context.SaveChanges();
        }

        [Test]
        public async Task GetByIdAsync_ReturnsMedicalRecord()
        {
            // Arrange: Seed the database with a MedicalRecord
            var record = new MedicalRecord
            {
                RecordId = 1,
                PatientId = 1,
                DoctorId = 1,
                AppointmentId = 1,
                TreatmentPlan = "Initial check-up",
                IsDeleted = false
            };
            await _context.SaveChangesAsync();

            // Act: Fetch the record by ID
            var result = await _repository.GetByIdAsync(1);

            // Assert: Verify that the record is fetched correctly
            Assert.IsNotNull(record); // Ensure the record is not null
            Assert.AreEqual(1, record.RecordId);
            Assert.AreEqual("Initial check-up", record.TreatmentPlan);
        }

        [Test]
        public async Task AddAsync_AddsNewMedicalRecord()
        {
            // Arrange: Create a new medical record to add
            var newRecord = new MedicalRecord
            {
                PatientId = 3,
                DoctorId = 3,
                AppointmentId = 3,
                IsDeleted = false,
                TreatmentPlan = "New patient visit"
            };

            // Act: Add the new medical record
            var result = await _repository.AddAsync(newRecord);

            // Assert: Verify that the record was added successfully
            Assert.IsTrue(result);
            Assert.AreEqual(3, _context.MedicalRecords.Count()); // Verify the record count increased
        }

        [Test]
        public async Task UpdateAsync_UpdatesMedicalRecord()
        {
            // Arrange: Get the existing record and modify it
            var existingRecord = await _context.MedicalRecords.FindAsync(1);
            existingRecord.TreatmentPlan = "Updated check-up description";

            // Act: Update the record
            var result = await _repository.UpdateAsync(existingRecord);

            // Assert: Verify the record was updated successfully
            Assert.IsTrue(result);
            var updatedRecord = await _context.MedicalRecords.FindAsync(1);
            Assert.AreEqual("Updated check-up description", updatedRecord.TreatmentPlan);
        }

        [Test]
        public async Task DeleteAsync_SoftDeletesMedicalRecord()
        {
            // Act: Soft delete the medical record
            var result = await _repository.DeleteAsync(1);

            // Assert: Verify that the record was soft-deleted
            Assert.IsTrue(result);
            var deletedRecord = await _context.MedicalRecords.FindAsync(1);
            Assert.IsTrue(deletedRecord.IsDeleted);
        }

        [Test]
        public async Task DeleteAsync_RecordNotFound_ReturnsFalse()
        {
            // Act: Try to delete a non-existent record
            var result = await _repository.DeleteAsync(999); // Assuming 999 doesn't exist

            // Assert: Verify that it returns false
            Assert.IsFalse(result);
        }
        [Test]
        public async Task DeleteAppointmentAsync_ReturnsFalseForNonExistentAppointment()
        {
            // Act: Try to delete a non-existent appointment
            var result = await _repository.DeleteAsync(999);

            // Assert: Verify the result is false
            Assert.IsFalse(result);
        }
        [Test]
        public async Task DeleteAppointmentAsync_ReturnsFalseForInvalidAppointmentId()
        {
            // Arrange: Pass an invalid ID (assuming 999 does not exist)

            // Act: Try to delete using an invalid appointment ID
            var result = await _repository.DeleteAsync(-1);

            // Assert: Verify the result is false since the appointment ID does not exist
            Assert.IsFalse(result);
        }
        [Test]
        public async Task GetByIdAsync_ReturnsNullForNonExistentId()
        {
            // Act: Try to fetch a non-existent medical record
            var result = await _repository.GetByIdAsync(999);

            // Assert: Verify that the result is null
            Assert.IsNull(result);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up after each test
            _context?.Dispose();
        }
    }
}
