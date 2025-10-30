using EmployeeManagementAPI.Models;
using EmployeeManagementAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepository _repository;

        public EmployeesController(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IEnumerable<Employee>> GetEmployees()
        {
            return await _repository.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _repository.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return employee;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddEmployee(Employee employee)
        {
            await _repository.AddAsync(employee);
            return CreatedAtAction(nameof(GetEmployee), new { id = employee.EmployeeId }, employee);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateEmployee(int id, Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return BadRequest();
            }
            await _repository.UpdateAsync(employee);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteEmployee(int id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}