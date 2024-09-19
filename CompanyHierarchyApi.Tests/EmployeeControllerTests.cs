using CompanyHierarchyApi.Controllers;
using CompanyHierarchyApi.Domain;
using CompanyHierarchyApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace CompanyHierarchyApi.Tests
{
    public class EmployeeControllerTests
    {
        private readonly Mock<IEmployeeRepository> _mockRepo;
        private readonly EmployeeController _controller;

        public EmployeeControllerTests()
        {
            // Setup mock repository
            _mockRepo = new Mock<IEmployeeRepository>();

            // Inject mock repository into controller
            _controller = new EmployeeController(_mockRepo.Object);
        }

        // Test for GET /employee/{EmployeeId}
        [Fact]
        public async Task GetEmployeeById_ReturnsOkResult_WhenEmployeeExists()
        {
            var employee = new Employee { EmployeeId = 1, FullName = "John Doe", Title = "Manager" };
            _mockRepo.Setup(repo => repo.GetEmployeeByIdAsync(1))
                     .ReturnsAsync(employee);

            var result = await _controller.GetEmployeeById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedEmployee = Assert.IsType<Employee>(okResult.Value);
            Assert.Equal(1, returnedEmployee.EmployeeId);
        }

        // Test for GET /employee (Top-level managers)
        [Fact]
        public async Task GetTopLevelManagers_ReturnsOkResult_WithListOfManagers()
        {
            var managers = new List<Employee> {
                new Employee { EmployeeId = 1, FullName = "Alice", Title = "CEO" },
                new Employee { EmployeeId = 2, FullName = "Bob", Title = "CFO" }
            };
            _mockRepo.Setup(repo => repo.GetTopLevelManagersAsync())
                     .ReturnsAsync(managers);

            var result = await _controller.GetTopLevelManagers();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedManagers = Assert.IsType<List<Employee>>(okResult.Value);
            Assert.Equal(2, returnedManagers.Count);
        }

        // Test for PUT /employee (Add/Update Employee)
        [Fact]
        public async Task AddOrUpdateEmployee_ReturnsOkResult()
        {
            var employeeData = new EmployeeData { FullName = "John Doe", Title = "Manager", ManagerEmployeeId = null };

            var result = await _controller.AddOrUpdateEmployee(employeeData);

            Assert.IsType<OkResult>(result);
            _mockRepo.Verify(repo => repo.AddOrUpdateEmployeeAsync(employeeData), Times.Once);
        }

        // Test for DELETE /employee/{EmployeeId}
        [Fact]
        public async Task DeleteEmployee_ReturnsNoContent_WhenEmployeeDeleted()
        {
            var result = await _controller.DeleteEmployee(1);

            Assert.IsType<NoContentResult>(result);
            _mockRepo.Verify(repo => repo.DeleteEmployeeAsync(1), Times.Once);
        }
    }
}
