using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRD_Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRD_DataLibrary.Models;
using Microsoft.EntityFrameworkCore.Internal;
using HRD_DataLibrary.Errors;

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

        // GET: api/vacancies{?deleted=false}
        [HttpGet]
        public async Task<IActionResult> GetVacancies(string session, bool deleted = false)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            return Json(_context.Vacancies.Where(vacancy => vacancy.Deleted == deleted));
        }

        // GET: api/vacancies/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVacancy(string session, [FromRoute] int id)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            var vacancy = await _context.Vacancies.SingleOrDefaultAsync(m => m.VacancyId == id);

            if (vacancy == null)
            {
                Response.StatusCode = 405;
                return Json(ErrorType.NotFoundObject);
            }

            return Json(vacancy);
        }

        // PUT: api/vacancies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVacancy(string session, [FromRoute] int id, [FromBody] Vacancy vacancy)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            if (id != vacancy.VacancyId)
            {
                Response.StatusCode = 405;
                return Json(ErrorType.NotFoundObject);
            }

            _context.Entry(vacancy).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return Json(ErrorType.InternalError);
            }
        }

        // POST: api/vacancies
        [HttpPost]
        public async Task<IActionResult> PostVacancy(string session, [FromBody] Vacancy vacancy)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            _context.Vacancies.Add(vacancy);
            await _context.SaveChangesAsync();

            return await GetVacancy(session, vacancy.VacancyId);
        }

        // DELETE: api/vacancies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVacancy(string session, [FromRoute] int id)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            var vacancy = await _context.Vacancies.SingleOrDefaultAsync(m => m.VacancyId == id);
            if (vacancy == null)
            {
                Response.StatusCode = 405;
                return Json(ErrorType.NotFoundObject);
            }

            vacancy.Deleted = true;
            _context.Entry(vacancy).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Json(vacancy);
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return Json(ErrorType.InternalError);
            }
        }

        private bool VacancyExists(int id)
        {
            return _context.Vacancies.Any(e => e.VacancyId == id);
        }
    }
}