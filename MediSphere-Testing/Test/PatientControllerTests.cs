using MediSphere.Models;
using MediSphere.Repository;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace MediSphereTest.Test
{
    public class PatientControllerTests
    {
        private PatientRepository _repository;
        private MediSphereDbContext _context;

        [SetUp]
        public void Setup()
        {
            // Create a new in-memory database for each test
            var options = new DbContextOptionsBuilder<MediSphereDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _context = new MediSphereDbContext(options);
            _repository = new PatientRepository(_context);

            // Seed the database with initial data (if needed)
            _context.Patients.AddRange(new List<Patient>
            {
                new Patient { PatientId = 1, FullName = "John Doe", DateOfBirth= DateTime.Parse("1988-04-12"), Gender= char.Parse("M") },
                new Patient { PatientId = 2, FullName = "Mary Smith", DateOfBirth = DateTime.Parse("2024-12-12"), Gender = char.Parse("F") }
            });
            _context.SaveChanges();
        }

        [Test]
        public void GetAllPatients_ReturnsPatientsList()
        {
            // Act: Call GetAllPatients
            var result = _repository.GetAllPatients().ToList();

            // Assert: Verify the correct number of patients is returned
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("John Doe", result[0].FullName);
            Assert.AreEqual( 'M', result[0].Gender);
            Assert.AreEqual("Mary Smith", result[1].FullName);
            Assert.AreEqual('F', result[1].Gender);
        }
        [Test]
        public void GetAllPatients_ReturnsTrueWhenPatientsExist()
        {
            // Act: Call GetAllPatients
            var result = _repository.GetAllPatients().ToList();

            // Assert: Verify that the result is not empty
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0, "Expected patients to be returned.");
        }
        [Test]
        public void AddPatient_SavesPatientToDbContext()
        {
            // Arrange: Create a new patient to add
            var patient = new Patient { PatientId = 3, FullName = "Alice Johnson", DateOfBirth = DateTime.Parse("2023-06-20"), Gender = char.Parse("M") };

            // Act: Add the patient using the repository
            _repository.AddPatient(patient);
            _context.SaveChanges();  // Save the changes to the in-memory database

            // Assert: Verify the patient was added by checking the count in the database
            var patientFromDb = _context.Patients.Find(3);
            Assert.IsNotNull(patientFromDb);
            Assert.AreEqual("Alice Johnson", patientFromDb.FullName);
            Assert.AreEqual('M', patientFromDb.Gender);
        }
    }
}
