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
using static Ligonine.Controllers.DoctorsController;
using System.IdentityModel.Tokens.Jwt;

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
        [Authorize(Roles = $"{ForumRoles.Admin},{ForumRoles.Doctor}")]
        public async Task<IActionResult> PutOperation(int departmentId, int doctorId, int id, OperationDto operationDto)
        {
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Id == doctorId && d.DepartmentId == departmentId);
            if (doctor == null)
            {
                return BadRequest("Invalid Doctor ID or Doctor not in this department");
            }

            var existingOperation = await _context.Operations
                .FirstOrDefaultAsync(o => o.Id == id && o.DoctorId == doctorId);
            if (existingOperation == null)
            {
                return NotFound("Operation not found for this doctor");
            }

            var isAdmin = User.IsInRole(ForumRoles.Admin);
            var userId = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (!isAdmin && userId != doctor.UserId)
            {
                return BadRequest("You can not change other doctors operations");
            }

            existingOperation.Name = operationDto.Name;
            existingOperation.Description = operationDto.Description;

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
        [Authorize(Roles = $"{ForumRoles.Admin},{ForumRoles.Doctor}")]
        public async Task<ActionResult<Operation>> PostOperation(int departmentId, int doctorId, OperationDto operationDto)
        {
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Id == doctorId && d.DepartmentId == departmentId);
            if (doctor == null)
            {
                return BadRequest("Invalid Doctor ID or Doctor not in this department");
            }

            var userId = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in the token.");
            }

            var isAdmin = User.IsInRole(ForumRoles.Admin);
            if (doctor.UserId != userId && !isAdmin)
            {
                return BadRequest("You can not create opperations for other doctors. Only Admins can create other doctors operations.");
            }

            var operation = new Operation
            {
                Name = operationDto.Name,
                Description = operationDto.Description,
                DoctorId = doctorId,
                UserId = userId,
            };

            _context.Operations.Add(operation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOperation), new { departmentId = departmentId, doctorId = doctorId, id = operation.Id }, operation);
        }

        // DELETE: api/departments/{departmentId}/doctors/{doctorId}/operations/5
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{ForumRoles.Admin},{ForumRoles.Doctor}")]
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

            var userId = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in the token.");
            }

            var isAdmin = User.IsInRole(ForumRoles.Admin);
            if (doctor.UserId != userId && !isAdmin)
            {
                return BadRequest("Can not delete other doctors operations");
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

        public record OperationDto(string Name, string Description);
    }
}
