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
    [Route("api/resumes")]
    public class ResumesController : Controller
    {
        private readonly HRD_DbContext _context;

        public ResumesController(HRD_DbContext context)
        {
            _context = context;
        }

        // GET: api/resumes
        [HttpGet]
        public IEnumerable<Resume> GetResume()
        {
            return _context.Resume;
        }

        // GET: api/resumes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetResume([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resume = await _context.Resume.SingleOrDefaultAsync(m => m.ResumeId == id);

            if (resume == null)
            {
                return NotFound();
            }

            return Ok(resume);
        }

        // PUT: api/resumes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutResume([FromRoute] int id, [FromBody] Resume resume)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != resume.ResumeId)
            {
                return BadRequest();
            }

            _context.Entry(resume).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResumeExists(id))
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

        // POST: api/resumes
        [HttpPost]
        public async Task<IActionResult> PostResume([FromBody] Resume resume)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Resume.Add(resume);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetResume", new { id = resume.ResumeId }, resume);
        }

        // DELETE: api/resumes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResume([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resume = await _context.Resume.SingleOrDefaultAsync(m => m.ResumeId == id);
            if (resume == null)
            {
                return NotFound();
            }

            _context.Resume.Remove(resume);
            await _context.SaveChangesAsync();

            return Ok(resume);
        }

        private bool ResumeExists(int id)
        {
            return _context.Resume.Any(e => e.ResumeId == id);
        }
    }
}