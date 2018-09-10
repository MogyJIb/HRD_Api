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
    [Route("api/worked_times")]
    public class WorkedTimesController : Controller
    {
        private readonly HRD_DbContext _context;

        public WorkedTimesController(HRD_DbContext context)
        {
            _context = context;
        }

        // GET: api/worked_times{?deleted=false}
        [HttpGet]
        public async Task<IActionResult> GetWorkedTimes(string session, bool deleted = false)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            return Json(_context.WorkedTimes
                .Where(workedTime => workedTime.Deleted == deleted)
                .Include(t => t.Employee));
        }

        // GET: api/worked_times/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkedTime(string session, [FromRoute] int id)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            var workedTime = await _context.WorkedTimes
                .Include(t => t.Employee)
                .SingleOrDefaultAsync(m => m.WorkedTimeId == id);

            if (workedTime == null)
            {
                Response.StatusCode = 405;
                return Json(ErrorType.NotFoundObject);
            }

            return Json(workedTime);
        }

        // PUT: api/worked_times/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWorkedTime(string session, [FromRoute] int id, [FromBody] WorkedTime workedTime)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            if (id != workedTime.WorkedTimeId)
            {
                Response.StatusCode = 405;
                return Json(ErrorType.NotFoundObject);
            }

            _context.Entry(workedTime).State = EntityState.Modified;

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

        // POST: api/worked_times
        [HttpPost]
        public async Task<IActionResult> PostWorkedTime(string session, [FromBody] WorkedTime workedTime)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            _context.WorkedTimes.Add(workedTime);
            await _context.SaveChangesAsync();

            return await GetWorkedTime(session, workedTime.WorkedTimeId);
        }

        // DELETE: api/worked_times/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkedTime(string session, [FromRoute] int id)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            var workedTime = await _context.WorkedTimes
                .Include(t => t.Employee)
                .SingleOrDefaultAsync(m => m.WorkedTimeId == id);

            if (workedTime == null)
            {
                Response.StatusCode = 405;
                return Json(ErrorType.NotFoundObject);
            }

            workedTime.Deleted = true;
            _context.Entry(workedTime).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Json(workedTime);
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return Json(ErrorType.InternalError);
            }
        }

        private bool WorkedTimeExists(int id)
        {
            return _context.WorkedTimes.Any(e => e.WorkedTimeId == id);
        }
    }
}