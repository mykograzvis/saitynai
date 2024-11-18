using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ligonine.Data;
using Ligonine.Data.Models;
using Ligonine.Auth.Model;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using static Ligonine.Controllers.DepartmentsController;

namespace Ligonine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public DepartmentsController(HospitalDbContext context)
        {
            _context = context;
        }

        // GET: api/Departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
        {
            return await _context.Departments.ToListAsync();
        }

        // GET: api/Departments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return department;
        }

        // PUT: api/Departments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = ForumRoles.Admin)]
        public async Task<IActionResult> PutDepartment(int id, DepartmentDto departmentDto)
        {
            // Find the existing department by ID
            var existingDepartment = await _context.Departments.FindAsync(id);

            if (existingDepartment == null)
            {
                return NotFound();
            }

            // Update the properties of the existing department with the new data
            existingDepartment.Name = departmentDto.Name;
            existingDepartment.Description = departmentDto.Description;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // POST: api/Departments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = ForumRoles.Admin)]
        public async Task<ActionResult<Department>> PostDepartment(DepartmentDto departmentDto)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in the token.");
            }
            var department = new Department
            {
                Name = departmentDto.Name,
                Description = departmentDto.Description,
                UserId = userId
            };
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDepartment", new { id = department.Id }, department);
        }

        // DELETE: api/Departments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }

        public record DepartmentDto(string Name, string Description);
    }
}
