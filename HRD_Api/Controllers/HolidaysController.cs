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
    [Route("api/holidays")]
    public class HolidaysController : Controller
    {
        private readonly HRD_DbContext _context;

        public HolidaysController(HRD_DbContext context)
        {
            _context = context;
        }

        // GET: api/holidays
        [HttpGet]
        public IEnumerable<Holiday> GetHolidays()
        {
            return _context.Holidays;
        }

        // GET: api/holidays/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHoliday([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var holiday = await _context.Holidays.SingleOrDefaultAsync(m => m.HolidayId == id);

            if (holiday == null)
            {
                return NotFound();
            }

            return Ok(holiday);
        }

        // PUT: api/holidays/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHoliday([FromRoute] int id, [FromBody] Holiday holiday)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != holiday.HolidayId)
            {
                return BadRequest();
            }

            _context.Entry(holiday).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HolidayExists(id))
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

        // POST: api/holidays
        [HttpPost]
        public async Task<IActionResult> PostHoliday([FromBody] Holiday holiday)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Holidays.Add(holiday);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHoliday", new { id = holiday.HolidayId }, holiday);
        }

        // DELETE: api/holidays/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHoliday([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var holiday = await _context.Holidays.SingleOrDefaultAsync(m => m.HolidayId == id);
            if (holiday == null)
            {
                return NotFound();
            }

            _context.Holidays.Remove(holiday);
            await _context.SaveChangesAsync();

            return Ok(holiday);
        }

        private bool HolidayExists(int id)
        {
            return _context.Holidays.Any(e => e.HolidayId == id);
        }
    }
}