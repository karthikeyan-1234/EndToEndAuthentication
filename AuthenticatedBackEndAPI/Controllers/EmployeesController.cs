using AuthenticatedBackEndAPI.Contexts;
using AuthenticatedBackEndAPI.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticatedBackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        EmployeeContext context;

        public EmployeesController(EmployeeContext context)
        {
            this.context = context;
        }


        [HttpGet("GetAllEmployees")]
        [Authorize(Roles = "manager,operator")]
        public IActionResult GetAllEmployees()
        {
            var employees = context.Employees.ToList();
            return Ok(employees);
        }

        [HttpGet("GetEmployeeById/{id}")]
        public IActionResult GetEmployees()
        {
            var id = HttpContext.Request.RouteValues["id"];
            if (id == null)
            {
                return BadRequest("Employee ID is required.");
            }
            var employee = context.Employees.FirstOrDefault(e => e.Id == Convert.ToInt32(id));
            if (employee == null)
            {
                return NotFound($"Employee with ID {id} not found.");
            }
            return Ok(employee);
        }

        [HttpPost("AddEmployee")]
        public IActionResult AddEmployee([FromBody] Employee employee)
        {
            if (employee == null)
            {
                return BadRequest("Employee data is required.");
            }
            context.Employees.Add(employee);
            context.SaveChanges();
            return CreatedAtAction(nameof(GetEmployees), new { id = employee.Id }, employee);
        }

        [HttpPut("UpdateEmployee/{id}")]
        public IActionResult UpdateEmployee(int id, [FromBody] Employee employee)
        {
            if (employee == null || id != employee.Id)
            {
                return BadRequest("Employee data is invalid.");
            }
            
            var existingEmployee = context.Employees.FirstOrDefault(e => e.Id == id);
            
            if (existingEmployee == null)
            {
                return NotFound($"Employee with ID {id} not found.");
            }
            
            existingEmployee.Name = employee.Name;
            existingEmployee.Phone = employee.Phone;
            existingEmployee.Email = employee.Email;
            context.Employees.Update(existingEmployee);

            context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("DeleteEmployee/{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            var employee = context.Employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
            {
                return NotFound($"Employee with ID {id} not found.");
            }
            context.Employees.Remove(employee);
            context.SaveChanges();
            return NoContent();
        }
    }
}
