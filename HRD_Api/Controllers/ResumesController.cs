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
    [Route("api/resumes")]
    public class ResumesController : Controller
    {
        private readonly HRD_DbContext _context;

        public ResumesController(HRD_DbContext context)
        {
            _context = context;
        }

        // GET: api/resumes{?deleted=false}
        [HttpGet]
        public async Task<IActionResult> GetResumes(string session, bool deleted = false)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            return Json(_context.Resumes
                .Where(resume => resume.Deleted == deleted)
                .Include(t => t.Vacancy)
                .ThenInclude(v => v.Position)
                .ThenInclude(p => p.Department));
        }

        // GET: api/resumes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetResume(string session, [FromRoute] int id)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            var resume = await _context.Resumes
                .Include(t => t.Vacancy)
                .ThenInclude(v => v.Position)
                .ThenInclude(p => p.Department)
                .SingleOrDefaultAsync(m => m.ResumeId == id);

            if (resume == null)
            {
                Response.StatusCode = 405;
                return Json(ErrorType.NotFoundObject);
            }

            return Json(resume);
        }

        // PUT: api/resumes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutResume(string session, [FromRoute] int id, [FromBody] Resume resume)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            if (id != resume.ResumeId)
            {
                Response.StatusCode = 405;
                return Json(ErrorType.NotFoundObject);
            }

            _context.Entry(resume).State = EntityState.Modified;

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

        // POST: api/resumes
        [HttpPost]
        public async Task<IActionResult> PostResume(string session, [FromBody] Resume resume)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            _context.Resumes.Add(resume);
            await _context.SaveChangesAsync();

            return await GetResume(session, resume.ResumeId);
        }

        // DELETE: api/resumes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResume(string session, [FromRoute] int id)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            var resume = await _context.Resumes
                .Include(t => t.Vacancy)
                .ThenInclude(v => v.Position)
                .ThenInclude(p => p.Department)
                .SingleOrDefaultAsync(m => m.ResumeId == id);

            if (resume == null)
            {
                Response.StatusCode = 405;
                return Json(ErrorType.NotFoundObject);
            }

            resume.Deleted = true;
            _context.Entry(resume).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Json(resume);
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return Json(ErrorType.InternalError);
            }
        }

        private bool ResumeExists(int id)
        {
            return _context.Resumes.Any(e => e.ResumeId == id);
        }
    }
}