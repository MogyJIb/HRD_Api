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
    [Route("api/fired_employees")]
    public class FiredEmployeesController : Controller
    {
        private readonly HRD_DbContext _context;

        public FiredEmployeesController(HRD_DbContext context)
        {
            _context = context;
        }

        // GET: api/fired_employees{?deleted=false}
        [HttpGet]
        public async Task<IActionResult> GetFiredEmployees(string session, bool deleted = false)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            return Json(_context.FiredEmployees.Where(firedEmployee => firedEmployee.Deleted == deleted));
        }

        // GET: api/fired_employees/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFiredEmployee(string session, [FromRoute] int id)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            var firedEmployee = await _context.FiredEmployees.SingleOrDefaultAsync(m => m.FiredEmployeeId == id);

            if (firedEmployee == null)
            {
                Response.StatusCode = 405;
                return Json(ErrorType.NotFoundObject);
            }

            return Json(firedEmployee);
        }

        // PUT: api/fired_employees/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFiredEmployee(string session, [FromRoute] int id, [FromBody] FiredEmployee firedEmployee)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            if (id != firedEmployee.FiredEmployeeId)
            {
                Response.StatusCode = 405;
                return Json(ErrorType.NotFoundObject);
            }

            _context.Entry(firedEmployee).State = EntityState.Modified;

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

        // POST: api/fired_employees
        [HttpPost]
        public async Task<IActionResult> PostFiredEmployee(string session, [FromBody] FiredEmployee firedEmployee)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            _context.FiredEmployees.Add(firedEmployee);
            await _context.SaveChangesAsync();

            return await GetFiredEmployee(session, firedEmployee.FiredEmployeeId);
        }

        // DELETE: api/fired_employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFiredEmployee(string session, [FromRoute] int id)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            var firedEmployee = await _context.FiredEmployees.SingleOrDefaultAsync(m => m.FiredEmployeeId == id);
            if (firedEmployee == null)
            {
                Response.StatusCode = 405;
                return Json(ErrorType.NotFoundObject);
            }

            _context.FiredEmployees.Remove(firedEmployee);
            await _context.SaveChangesAsync();

            return Json(firedEmployee);
        }

        private bool FiredEmployeeExists(int id)
        {
            return _context.FiredEmployees.Any(e => e.FiredEmployeeId == id);
        }
    }
}