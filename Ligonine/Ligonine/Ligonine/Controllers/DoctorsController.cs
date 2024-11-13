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
        public async Task<IActionResult> PutDoctor(int departmentId, int id, Doctor doctor)
        {
            // Find the department by ID
            var department = await _context.Departments.FindAsync(departmentId);
            if (department == null)
            {
                return BadRequest("Invalid Department ID");
            }

            // Find the doctor by ID and ensure they belong to the specified department
            var existingDoctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == id && d.DepartmentId == departmentId);
            if (existingDoctor == null)
            {
                return NotFound("Doctor not found in this department");
            }

            // Update the doctor's properties with the new data
            existingDoctor.Name = doctor.Name;
            existingDoctor.Age = doctor.Age;
            existingDoctor.BloodType = doctor.BloodType;
            // Update other properties as needed, e.g., existingDoctor.YearsOfExperience = doctor.YearsOfExperience;

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
        public async Task<ActionResult<Doctor>> PostDoctor(int departmentId, Doctor doctor)
        {
            var department = await _context.Departments.FindAsync(departmentId);
            if (department == null)
            {
                return BadRequest("Invalid Department ID");
            }

            doctor.DepartmentId = departmentId; // Ensure doctor is assigned to the department
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDoctor), new { departmentId = departmentId, id = doctor.Id }, doctor);
        }

        // DELETE: api/departments/{departmentId}/doctors/5
        [HttpDelete("{id}")]
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

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Check if a doctor exists in the given department
        private bool DoctorExists(int departmentId, int id)
        {
            return _context.Doctors.Any(e => e.Id == id && e.DepartmentId == departmentId);
        }
    }
}
