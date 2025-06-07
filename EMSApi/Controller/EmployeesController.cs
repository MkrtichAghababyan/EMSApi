using EMSApi.Models;
using EMSApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EMSApi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin,Manager")]
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeService _employeeService;

        public EmployeesController(EmployeeService employeeService)
        {
            _employeeService = employeeService;

            // ✅ Subscribe to the event (demonstrates delegate usage)
            _employeeService.EmployeeAdded += (sender, employee) =>
            {
                Console.WriteLine($"[Event] Employee added: {employee.FirstName} {employee.LastName}");
            };
        }

        // ✅ GET /api/employees?department=HR&sortBy=salary&page=1&pageSize=5
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string? department,
            [FromQuery] string? sortBy = "LastName",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var employees = await _employeeService.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(department))
            {
                employees = employees.Where(e => e.Department?.Name == department);
            }

            employees = sortBy?.ToLower() switch
            {
                "salary" => employees.OrderByDescending(e => e.Salary),
                "firstname" => employees.OrderBy(e => e.FirstName),
                _ => employees.OrderBy(e => e.LastName)
            };

            var paged = employees
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return Ok(paged);
        }

        // ✅ GET /api/employees/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        // ✅ POST /api/employees
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Employee employee)
        {
            var username = User.Identity?.Name ?? "system"; // pulled from JWT
            await _employeeService.AddAsync(employee, username);
            return CreatedAtAction(nameof(GetById), new { id = employee.EmployeeId }, employee);
        }

        // ✅ PUT /api/employees/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Employee updated)
        {
            if (id != updated.EmployeeId)
                return BadRequest("Mismatched employee ID");

            var username = User.Identity?.Name ?? "system";
            await _employeeService.UpdateAsync(updated, username);
            return NoContent();
        }

        // ✅ DELETE /api/employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var username = User.Identity?.Name ?? "system";
            await _employeeService.DeleteAsync(id, username);
            return NoContent();
        }
    }
}
