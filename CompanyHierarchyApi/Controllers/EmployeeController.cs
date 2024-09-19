using Microsoft.AspNetCore.Mvc;
using CompanyHierarchyApi.Domain;
using CompanyHierarchyApi.Infrastructure;
using System.Threading.Tasks;

namespace CompanyHierarchyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        // GET /employee/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            try
            {
                var employee = await _employeeRepository.GetEmployeeByIdAsync(id);

                if (employee == null)
                {
                    return NotFound(new { message = $"Employee with ID {id} not found." });
                }

                return Ok(employee);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET /employee
        [HttpGet]
        public async Task<IActionResult> GetTopLevelManagers()
        {
            try
            {
                var employees = await _employeeRepository.GetTopLevelManagersAsync();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT /employee
        [HttpPut]
        public async Task<IActionResult> AddOrUpdateEmployee([FromBody] EmployeeData employeeData)
        {
            try
            {
                await _employeeRepository.AddOrUpdateEmployeeAsync(employeeData);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE /employee/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                await _employeeRepository.DeleteEmployeeAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
