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
    [Route("api/vacancies")]
    public class VacanciesController : Controller
    {
        private readonly HRD_DbContext _context;

        public VacanciesController(HRD_DbContext context)
        {
            _context = context;
        }

        // GET: api/vacancies
        [HttpGet]
        public IEnumerable<Vacancy> GetVacancies()
        {
            return _context.Vacancies;
        }

        // GET: api/vacancies/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVacancy([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var vacancy = await _context.Vacancies.SingleOrDefaultAsync(m => m.VacancyId == id);

            if (vacancy == null)
            {
                return NotFound();
            }

            return Ok(vacancy);
        }

        // PUT: api/vacancies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVacancy([FromRoute] int id, [FromBody] Vacancy vacancy)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != vacancy.VacancyId)
            {
                return BadRequest();
            }

            _context.Entry(vacancy).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VacancyExists(id))
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

        // POST: api/vacancies
        [HttpPost]
        public async Task<IActionResult> PostVacancy([FromBody] Vacancy vacancy)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Vacancies.Add(vacancy);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVacancy", new { id = vacancy.VacancyId }, vacancy);
        }

        // DELETE: api/vacancies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVacancy([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var vacancy = await _context.Vacancies.SingleOrDefaultAsync(m => m.VacancyId == id);
            if (vacancy == null)
            {
                return NotFound();
            }

            _context.Vacancies.Remove(vacancy);
            await _context.SaveChangesAsync();

            return Ok(vacancy);
        }

        private bool VacancyExists(int id)
        {
            return _context.Vacancies.Any(e => e.VacancyId == id);
        }
    }
}