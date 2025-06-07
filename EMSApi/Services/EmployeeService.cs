using EMSApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EMSApi.Services
{
    public class EmployeeService : IEntityService<Employee>
    {
        private readonly EMSdbContext _context;
        private readonly IAuditService _auditService;
        private readonly ILogger<EmployeeService> _logger;

        public event EventHandler<Employee>? EmployeeAdded;
        public EmployeeService(EMSdbContext context, IAuditService auditService, ILogger<EmployeeService> logger)
        {
            _context = context;
            _auditService = auditService;
            _logger = logger;
        }
        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all employees...");
            return await _context.Employees
                .Include(e => e.Department)
                .ToListAsync();
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching employee with ID {id}", id);
            return await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);
        }

        public async Task AddAsync(Employee employee, string performedBy)
        {
            employee.CreatedBy = performedBy;
            employee.CreatedAt = DateTime.UtcNow;

            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Added employee: {id}", employee.EmployeeId);

            await _auditService.LogAsync(
                entityName: nameof(Employee),
                entityId: employee.EmployeeId,
                action: "Add",
                performedBy: performedBy,
                changes: $"Added employee {employee.FirstName} {employee.LastName}");

            
            EmployeeAdded?.Invoke(this, employee);

        }

        public async Task UpdateAsync(Employee employee, string performedBy)
        {
            employee.UpdatedBy = performedBy;
            employee.UpdatedAt = DateTime.UtcNow;

            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated employee: {id}", employee.EmployeeId);

            await _auditService.LogAsync(
                nameof(Employee),
                employee.EmployeeId,
                "Update",
                performedBy,
                "Employee details updated");
        }

        public async Task DeleteAsync(int id, string performedBy)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                _logger.LogWarning("Attempted to delete non-existent employee with ID {id}", id);
                throw new KeyNotFoundException("Employee not found");
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted employee: {id}", id);

            await _auditService.LogAsync(
                nameof(Employee),
                id,
                "Delete",
                performedBy,
                $"Deleted employee {employee.FirstName} {employee.LastName}");
        }
    }
}
