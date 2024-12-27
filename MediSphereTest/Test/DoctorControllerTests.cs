using MediSphere.Controllers;
using MediSphere.Models;
using MediSphere.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Linq;

namespace MediSphereTest.Test
{
    public class DoctorControllerTests
    {
        private DoctorRepository _repository;
        private MediSphereDbContext _context;

        [SetUp]
        public void Setup()
        {
            // Create a new in-memory database for each test
            var options = new DbContextOptionsBuilder<MediSphereDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _context = new MediSphereDbContext(options);
            _repository = new DoctorRepository(_context);

            // Seed the database with initial data (if needed)
            _context.Doctors.AddRange(new List<Doctor>
            {
                new Doctor { DoctorId = 1, FullName = "Dr. Smith", Specialty = "Cardiology" },
                new Doctor {DoctorId = 2, FullName = "Dr. Jones", Specialty = "Neurology"}
            });
            _context.SaveChanges();
        }

        [Test]
        public void GetAllDoctors_ReturnsDoctorsList()
        {
            // Act: Call GetAllDoctors
            var result = _repository.GetAllDoctors().ToList();

            // Assert: Verify the correct number of doctors is returned
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Dr. Smith", result[0].FullName);
            Assert.AreEqual("Cardiology", result[0].Specialty);
            Assert.AreEqual("Dr. Jones", result[1].FullName);
            Assert.AreEqual("Neurology", result[1].Specialty);
        }
        [Test]
        public void GetAllDoctors_ReturnsTrueWhenDoctorsExist()
        {
            // Act: Call GetAllDoctors
            var result = _repository.GetAllDoctors().ToList();

            // Assert: Verify that the result is not empty
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0, "Expected doctors to be returned.");
        }
        [Test]
        public void GetAllDoctors_ReturnsFalseWhenNoDoctorsExist()
        {
            // Arrange: Clear the doctors from the database
            _context.Doctors.RemoveRange(_context.Doctors);
            _context.SaveChanges();

            // Act: Call GetAllDoctors
            var result = _repository.GetAllDoctors().ToList();

            // Assert: Verify that the result is empty
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count, "Expected no doctors to be returned.");
        }
        [Test]
        public void AddDoctor_SavesDoctorToDbContext()
        {
            // Arrange: Create a new doctor to add
            var doctor = new Doctor { DoctorId = 3, FullName = "Dr. Brown", Specialty = "Orthopedics" };

            // Act: Add the doctor using the repository
            _repository.AddDoctor(doctor);

            // Assert: Verify the doctor was added by checking the count in the database
            var doctorFromDb = _context.Doctors.Find(3);
            Assert.IsNotNull(doctorFromDb);
            Assert.AreEqual("Dr. Brown", doctorFromDb.FullName);
            Assert.AreEqual("Orthopedics", doctorFromDb.Specialty);
        }
    }
}

