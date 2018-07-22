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
    [Route("api/worked_times")]
    public class WorkedTimesController : Controller
    {
        private readonly HRD_DbContext _context;

        public WorkedTimesController(HRD_DbContext context)
        {
            _context = context;
        }

        // GET: api/worked_times
        [HttpGet]
        public IEnumerable<WorkedTime> GetWorkedTime()
        {
            return _context.WorkedTimes;
        }

        // GET: api/worked_times/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkedTime([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var workedTime = await _context.WorkedTimes.SingleOrDefaultAsync(m => m.WorkedTimeId == id);

            if (workedTime == null)
            {
                return NotFound();
            }

            return Ok(workedTime);
        }

        // PUT: api/worked_times/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWorkedTime([FromRoute] int id, [FromBody] WorkedTime workedTime)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != workedTime.WorkedTimeId)
            {
                return BadRequest();
            }

            _context.Entry(workedTime).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkedTimeExists(id))
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

        // POST: api/worked_times
        [HttpPost]
        public async Task<IActionResult> PostWorkedTime([FromBody] WorkedTime workedTime)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.WorkedTimes.Add(workedTime);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWorkedTime", new { id = workedTime.WorkedTimeId }, workedTime);
        }

        // DELETE: api/worked_times/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkedTime([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var workedTime = await _context.WorkedTimes.SingleOrDefaultAsync(m => m.WorkedTimeId == id);
            if (workedTime == null)
            {
                return NotFound();
            }

            _context.WorkedTimes.Remove(workedTime);
            await _context.SaveChangesAsync();

            return Ok(workedTime);
        }

        private bool WorkedTimeExists(int id)
        {
            return _context.WorkedTimes.Any(e => e.WorkedTimeId == id);
        }
    }
}