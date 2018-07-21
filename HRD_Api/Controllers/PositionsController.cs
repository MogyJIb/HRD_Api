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
    [Route("api/positions")]
    public class PositionsController : Controller
    {
        private readonly HRD_DbContext _context;

        public PositionsController(HRD_DbContext context)
        {
            _context = context;
        }

        // GET: api/positions
        [HttpGet]
        public IEnumerable<Position> GetPositions()
        {
            return _context.Positions;
        }

        // GET: api/positions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPosition([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var position = await _context.Positions.SingleOrDefaultAsync(m => m.PositionId == id);

            if (position == null)
            {
                return NotFound();
            }

            return Ok(position);
        }

        // PUT: api/positions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPosition([FromRoute] int id, [FromBody] Position position)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != position.PositionId)
            {
                return BadRequest();
            }

            _context.Entry(position).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PositionExists(id))
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

        // POST: api/positions
        [HttpPost]
        public async Task<IActionResult> PostPosition([FromBody] Position position)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Positions.Add(position);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPosition", new { id = position.PositionId }, position);
        }

        // DELETE: api/positions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePosition([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var position = await _context.Positions.SingleOrDefaultAsync(m => m.PositionId == id);
            if (position == null)
            {
                return NotFound();
            }

            _context.Positions.Remove(position);
            await _context.SaveChangesAsync();

            return Ok(position);
        }

        private bool PositionExists(int id)
        {
            return _context.Positions.Any(e => e.PositionId == id);
        }
    }
}