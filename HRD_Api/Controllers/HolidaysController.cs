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
    [Route("api/holidays")]
    public class HolidaysController : Controller
    {
        private readonly HRD_DbContext _context;

        public HolidaysController(HRD_DbContext context)
        {
            _context = context;
        }

        // GET: api/holidays{?deleted=false}
        [HttpGet]
        public async Task<IActionResult> GetHolidays(string session, bool deleted = false)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            return Json(_context.Holidays.Where(holiday => holiday.Deleted == deleted));
        }

        // GET: api/holidays/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHoliday(string session, [FromRoute] int id)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            var holiday = await _context.Holidays.SingleOrDefaultAsync(m => m.HolidayId == id);

            if (holiday == null)
            {
                Response.StatusCode = 405;
                return Json(ErrorType.NotFoundObject);
            }

            return Json(holiday);
        }

        // PUT: api/holidays/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHoliday(string session, [FromRoute] int id, [FromBody] Holiday holiday)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            if (id != holiday.HolidayId)
            {
                Response.StatusCode = 405;
                return Json(ErrorType.NotFoundObject);
            }

            _context.Entry(holiday).State = EntityState.Modified;

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

        // POST: api/holidays
        [HttpPost]
        public async Task<IActionResult> PostHoliday(string session, [FromBody] Holiday holiday)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            _context.Holidays.Add(holiday);
            await _context.SaveChangesAsync();

            return await GetHoliday(session, holiday.HolidayId);
        }

        // DELETE: api/holidays/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHoliday(string session, [FromRoute] int id)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            var holiday = await _context.Holidays.SingleOrDefaultAsync(m => m.HolidayId == id);
            if (holiday == null)
            {
                Response.StatusCode = 405;
                return Json(ErrorType.NotFoundObject);
            }

            holiday.Deleted = true;
            _context.Entry(holiday).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Json(holiday);
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return Json(ErrorType.InternalError);
            }
        }

        private bool HolidayExists(int id)
        {
            return _context.Holidays.Any(e => e.HolidayId == id);
        }
    }
}