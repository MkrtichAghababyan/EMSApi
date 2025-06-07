using System.Threading.Tasks;
using EMSApi.Models;
using EMSApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using FluentAssertions;
namespace EMSApi.Tests
{
    public class EmployeeServiceTests
    {
        private EMSdbContext _context = null!;
        private EmployeeService _employeeService = null!;
        private Mock<IAuditService> _mockAuditService = null!;
        private List<Employee> _eventRaised = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<EMSdbContext>()
                    .UseInMemoryDatabase(databaseName: "EMS_TestDb")
                    .Options;

            _context = new EMSdbContext(options);

            _mockAuditService = new Mock<IAuditService>();
            var logger = new LoggerFactory().CreateLogger<EmployeeService>();

            _employeeService = new EmployeeService(_context, _mockAuditService.Object, logger);

            // Track raised events
            _eventRaised = new List<Employee>();
            _employeeService.EmployeeAdded += (s, e) => _eventRaised.Add(e);
        }

        [Test]
        public async Task AddAsync_ShouldAddEmployee_AndLogAudit_AndRaiseEvent()
        {
            // Arrange
            var newEmp = new Employee
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                Position = "QA",
                Salary = 50000,
                DepartmentId = 1
            };

            // Act
            await _employeeService.AddAsync(newEmp, "testUser");

            // Assert
            var savedEmp = await _context.Employees.FirstOrDefaultAsync(e => e.Email == "test@example.com");

            savedEmp.Should().NotBeNull();
            savedEmp!.CreatedBy.Should().Be("testUser");

            _mockAuditService.Verify(a => a.LogAsync(
                nameof(Employee),
                savedEmp.EmployeeId,
                "Add",
                "testUser",
                It.IsAny<string>()), Times.Once);

            _eventRaised.Should().ContainSingle();
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllEmployees()
        {
            // Arrange
            await _context.Employees.AddAsync(new Employee
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Position = "Dev",
                Salary = 60000,
                DepartmentId = 1,
                CreatedBy = "test"
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _employeeService.GetAllAsync();

            // Assert
            result.Should().HaveCount(1);
            result.First().Email.Should().Be("john@example.com");
        }

        [Test]
        public void DeleteAsync_ShouldThrow_WhenEmployeeNotFound()
        {
            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await _employeeService.DeleteAsync(999, "admin");
            });
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted(); // wipes the in-memory DB
            _context.Dispose();
        }
    }
}
