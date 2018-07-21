using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRD_Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRD_DataLibrary.Models;

namespace HRD_Api.Controllers
{
    [Produces("application/json")]
    [Route("api/fired_employees")]
    public class FiredEmployeesController : Controller
    {
        private readonly HRD_DbContext _context;

        public FiredEmployeesController(HRD_DbContext context)
        {
            _context = context;
        }

        // GET: api/fired_employees
        [HttpGet]
        public IEnumerable<FiredEmployee> GetFiredEmployees()
        {
            return _context.FiredEmployees;
        }

        // GET: api/fired_employees/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFiredEmployee([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var firedEmployee = await _context.FiredEmployees.SingleOrDefaultAsync(m => m.FiredEmployeeId == id);

            if (firedEmployee == null)
            {
                return NotFound();
            }

            return Ok(firedEmployee);
        }

        // PUT: api/fired_employees/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFiredEmployee([FromRoute] int id, [FromBody] FiredEmployee firedEmployee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != firedEmployee.FiredEmployeeId)
            {
                return BadRequest();
            }

            _context.Entry(firedEmployee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FiredEmployeeExists(id))
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

        // POST: api/fired_employees
        [HttpPost]
        public async Task<IActionResult> PostFiredEmployee([FromBody] FiredEmployee firedEmployee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.FiredEmployees.Add(firedEmployee);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFiredEmployee", new { id = firedEmployee.FiredEmployeeId }, firedEmployee);
        }

        // DELETE: api/fired_employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFiredEmployee([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var firedEmployee = await _context.FiredEmployees.SingleOrDefaultAsync(m => m.FiredEmployeeId == id);
            if (firedEmployee == null)
            {
                return NotFound();
            }

            _context.FiredEmployees.Remove(firedEmployee);
            await _context.SaveChangesAsync();

            return Ok(firedEmployee);
        }

        private bool FiredEmployeeExists(int id)
        {
            return _context.FiredEmployees.Any(e => e.FiredEmployeeId == id);
        }
    }
}