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
using static Ligonine.Controllers.DepartmentsController;
using System.IdentityModel.Tokens.Jwt;

namespace Ligonine.Controllers
{
    [Route("api/departments/{departmentId}/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public DoctorsController(HospitalDbContext context)
        {
            _context = context;
        }

        // GET: api/departments/{departmentId}/doctors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctors(int departmentId)
        {
            var department = await _context.Departments.FindAsync(departmentId);
            if (department == null)
            {
                return NotFound("Department not found");
            }

            return await _context.Doctors
                                 .Where(d => d.DepartmentId == departmentId) // Fetch doctors for the given department
                                 .ToListAsync();
        }

        // GET: api/departments/{departmentId}/doctors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Doctor>> GetDoctor(int departmentId, int id)
        {
            var department = await _context.Departments.FindAsync(departmentId);
            if (department == null)
            {
                return NotFound("Department not found");
            }

            var doctor = await _context.Doctors
                                       .FirstOrDefaultAsync(d => d.Id == id && d.DepartmentId == departmentId);

            if (doctor == null)
            {
                return NotFound("Doctor not found in this department");
            }

            return doctor;
        }

        // PUT: api/departments/{departmentId}/doctors/5
        [HttpPut("{id}")]
        [Authorize(Roles = $"{ForumRoles.Admin},{ForumRoles.Doctor}")]
        public async Task<IActionResult> PutDoctor(int departmentId, int id, DoctorDto doctorDto)
        {
            var department = await _context.Departments.FindAsync(departmentId);
            if (department == null)
            {
                return BadRequest("Invalid Department ID");
            }

            var existingDoctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == id && d.DepartmentId == departmentId);
            if (existingDoctor == null)
            {
                return NotFound("Doctor not found in this department");
            }

            var isAdmin = User.IsInRole(ForumRoles.Admin);
            var userId = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if(!isAdmin && userId != existingDoctor.UserId)
            {
                return BadRequest("You can not change other doctors");
            }

            existingDoctor.Name = doctorDto.Name;
            existingDoctor.Age = doctorDto.Age;
            existingDoctor.BloodType = doctorDto.BloodType;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoctorExists(departmentId, id))
                {
                    return NotFound("Doctor not found in this department");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // POST: api/departments/{departmentId}/doctors
        [HttpPost]
        [Authorize(Roles = $"{ForumRoles.Admin},{ForumRoles.Doctor}")]
        public async Task<ActionResult<Doctor>> PostDoctor(int departmentId, DoctorDto doctorDto)
        {
            var department = await _context.Departments.FindAsync(departmentId);
            if (department == null)
            {
                return BadRequest("Invalid Department ID");
            }

            var userId = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in the token.");
            }

            var existingDoctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);

            var isAdmin = User.IsInRole(ForumRoles.Admin);
            if (existingDoctor != null && !isAdmin)
            {
                return BadRequest("You already have a doctor associated with your account. Only Admins can create additional doctors.");
            }

            var doctor = new Doctor
            {
                Name = doctorDto.Name,
                Age = doctorDto.Age,
                BloodType = doctorDto.BloodType,
                DepartmentId = departmentId,
                UserId = userId,
            };
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDoctor), new { departmentId = departmentId, id = doctor.Id }, doctor);
        }

        // DELETE: api/departments/{departmentId}/doctors/5
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{ForumRoles.Admin},{ForumRoles.Doctor}")]
        public async Task<IActionResult> DeleteDoctor(int departmentId, int id)
        {
            var department = await _context.Departments.FindAsync(departmentId);
            if (department == null)
            {
                return NotFound("Department not found");
            }

            var doctor = await _context.Doctors
                                       .FirstOrDefaultAsync(d => d.Id == id && d.DepartmentId == departmentId);
            if (doctor == null)
            {
                return NotFound("Doctor not found in this department");
            }

            var userId = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in the token.");
            }

            var isAdmin = User.IsInRole(ForumRoles.Admin);
            if (doctor.UserId != userId && !isAdmin)
            {
                return BadRequest("Can not delete other doctor");
            }

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Check if a doctor exists in the given department
        private bool DoctorExists(int departmentId, int id)
        {
            return _context.Doctors.Any(e => e.Id == id && e.DepartmentId == departmentId);
        }

        public record DoctorDto(string Name, int Age, string BloodType);
    }
}
