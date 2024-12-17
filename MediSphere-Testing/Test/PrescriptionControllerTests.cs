using MediSphere.Models;
using MediSphere.Repository;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace MediSphereTest.Test
{
    public class PrescriptionControllerTests
    {
        private MediSphereDbContext _context;
        private PrescriptionRepository _repository;

        [SetUp]
        public void Setup()
        {
            // Set up an in-memory database for each test
            var options = new DbContextOptionsBuilder<MediSphereDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}") // Unique DB for isolation
                .Options;

            _context = new MediSphereDbContext(options);
            _repository = new PrescriptionRepository(_context);

            // Seed initial data
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            _context.Prescriptions.RemoveRange(_context.Prescriptions); // Clear data before seeding
            _context.SaveChanges();

            _context.Prescriptions.AddRange(
                new Prescription
                {
                    PrescriptionId = 1,
                    RecordId = 1,
                    MedicineName = "Medication A",
                    Dosage = "1 tablet daily",
                    IsDeleted = false
                },
                new Prescription
                {
                    PrescriptionId = 2,
                    RecordId = 2,
                    MedicineName = "Medication B",
                    Dosage = "2 tablets daily",
                    IsDeleted = false
                }
            );
            _context.SaveChanges();
        }

        [Test]
        public async Task GetAllPrescriptionsAsync_ReturnsAllPrescriptions()
        {
            // Act
            var result = await _repository.GetAllPrescriptionsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetPrescriptionByIdAsync_ReturnsPrescription()
        {
            // Act
            var result = await _repository.GetPrescriptionByIdAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.PrescriptionId);
            Assert.AreEqual("Medication A", result.MedicineName);
        }

        [Test]
        public async Task CreatePrescriptionAsync_AddsNewPrescription()
        {
            // Arrange
            var newPrescription = new Prescription
            {
                RecordId = 3,
                MedicineName = "Medication C",
                Dosage = "3 tablets daily",
                IsDeleted = false
            };

            // Act
            var result = await _repository.CreatePrescriptionAsync(newPrescription);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.PrescriptionId);
            Assert.AreEqual(3, _context.Prescriptions.Count());
        }

        [Test]
        public async Task UpdatePrescriptionAsync_UpdatesExistingPrescription()
        {
            // Arrange
            var existingPrescription = await _context.Prescriptions.FindAsync(1);
            existingPrescription.MedicineName = "Updated Medication A";

            // Act
            var result = await _repository.UpdatePrescriptionAsync(existingPrescription);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Updated Medication A", result.MedicineName);
        }

        [Test]
        public async Task DeletePrescriptionAsync_SoftDeletesPrescription()
        {
            // Act
            var result = await _repository.DeletePrescriptionAsync(1);

            // Assert
            Assert.IsTrue(result);
            var deletedPrescription = await _context.Prescriptions.FindAsync(1);
            Assert.IsTrue(deletedPrescription.IsDeleted);
        }

        [Test]
        public async Task DeletePrescriptionAsync_RecordNotFound_ReturnsFalse()
        {
            // Act
            var result = await _repository.DeletePrescriptionAsync(999); // Non-existent ID

            // Assert
            Assert.IsFalse(result);
        }
        [Test]
        public async Task GetPrescriptionByIdAsync_ReturnsNullForNonExistentPrescription()
        {
            // Act
            var result = await _repository.GetPrescriptionByIdAsync(999); // Non-existent ID

            // Assert
            Assert.IsNull(result);
        }
        [Test]
        public async Task GetAllPrescriptionsAsync_ReturnsEmptyListWhenNoPrescriptionsExist()
        {
            // Arrange: Remove all prescriptions from the database
            _context.Prescriptions.RemoveRange(_context.Prescriptions);
            _context.SaveChanges();

            // Act
            var result = await _repository.GetAllPrescriptionsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }
        [Test]
        public async Task DeletePrescriptionAsync_ReturnsTrueIfAlreadyDeleted()
        {
            // Arrange: Soft delete a prescription
            var prescription = await _context.Prescriptions.FindAsync(1);
            prescription.IsDeleted = true;
            await _context.SaveChangesAsync();

            // Act: Try to delete the already deleted prescription
            var result = await _repository.DeletePrescriptionAsync(1);

            // Assert: Verify that the result is false
            Assert.IsTrue(result);
        }

        [TearDown]
        public void TearDown()
        {
            // Dispose context after each test
            _context?.Dispose();
        }
    }
}
