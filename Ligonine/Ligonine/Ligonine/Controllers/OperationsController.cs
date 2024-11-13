using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ligonine.Data;
using Ligonine.Data.Models;

namespace Ligonine.Controllers
{
    [Route("api/departments/{departmentId}/doctors/{doctorId}/[controller]")]
    [ApiController]
    public class OperationsController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public OperationsController(HospitalDbContext context)
        {
            _context = context;
        }

        // GET: api/departments/{departmentId}/doctors/{doctorId}/operations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Operation>>> GetOperations(int departmentId, int doctorId)
        {
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Id == doctorId && d.DepartmentId == departmentId);
            if (doctor == null)
            {
                return NotFound("Doctor not found in this department");
            }

            return await _context.Operations
                                 .Where(op => op.DoctorId == doctorId)
                                 .ToListAsync();
        }

        // GET: api/departments/{departmentId}/doctors/{doctorId}/operations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Operation>> GetOperation(int departmentId, int doctorId, int id)
        {
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Id == doctorId && d.DepartmentId == departmentId);
            if (doctor == null)
            {
                return NotFound("Doctor not found in this department");
            }

            var operation = await _context.Operations
                                          .FirstOrDefaultAsync(op => op.Id == id && op.DoctorId == doctorId);

            if (operation == null)
            {
                return NotFound("Operation not found for this doctor");
            }

            return operation;
        }

        // PUT: api/departments/{departmentId}/doctors/{doctorId}/operations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOperation(int departmentId, int doctorId, int id, Operation operation)
        {
            // Find the doctor by ID and ensure they belong to the specified department
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Id == doctorId && d.DepartmentId == departmentId);
            if (doctor == null)
            {
                return BadRequest("Invalid Doctor ID or Doctor not in this department");
            }

            // Find the operation by ID and ensure it belongs to the correct doctor
            var existingOperation = await _context.Operations
                .FirstOrDefaultAsync(o => o.Id == id && o.DoctorId == doctorId);
            if (existingOperation == null)
            {
                return NotFound("Operation not found for this doctor");
            }

            // Update the operation's properties with the new data
            existingOperation.Name = operation.Name;
            existingOperation.Description = operation.Description;
            // Update other properties as needed, e.g., existingOperation.Duration = operation.Duration;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OperationExists(departmentId, doctorId, id))
                {
                    return NotFound("Operation not found for this doctor");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // POST: api/departments/{departmentId}/doctors/{doctorId}/operations
        [HttpPost]
        public async Task<ActionResult<Operation>> PostOperation(int departmentId, int doctorId, Operation operation)
        {
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Id == doctorId && d.DepartmentId == departmentId);
            if (doctor == null)
            {
                return BadRequest("Invalid Doctor ID or Doctor not in this department");
            }

            operation.DoctorId = doctorId;
            _context.Operations.Add(operation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOperation), new { departmentId = departmentId, doctorId = doctorId, id = operation.Id }, operation);
        }

        // DELETE: api/departments/{departmentId}/doctors/{doctorId}/operations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOperation(int departmentId, int doctorId, int id)
        {
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Id == doctorId && d.DepartmentId == departmentId);
            if (doctor == null)
            {
                return NotFound("Doctor not found in this department");
            }

            var operation = await _context.Operations
                                          .FirstOrDefaultAsync(op => op.Id == id && op.DoctorId == doctorId);
            if (operation == null)
            {
                return NotFound("Operation not found for this doctor");
            }

            _context.Operations.Remove(operation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OperationExists(int departmentId, int doctorId, int id)
        {
            return _context.Operations.Any(e => e.Id == id && e.DoctorId == doctorId &&
                                                _context.Doctors.Any(d => d.Id == doctorId && d.DepartmentId == departmentId));
        }
    }
}
