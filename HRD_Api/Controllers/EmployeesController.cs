﻿using System;
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
    [Route("api/employees")]
    public class EmployeesController : Controller
    {
        private readonly HRD_DbContext _context;

        public EmployeesController(HRD_DbContext context)
        {
            _context = context;
        }

        // GET: api/employees{?session=13asd1231ss&deleted=false}
        [HttpGet]
        public async Task<IActionResult> GetEmployees(string session, bool deleted = false)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            return Json(_context.Employees
                .Where(employee => employee.Deleted == deleted)
                .Where(employee => !_context.FiredEmployees.Any(fired => fired.EmployeeId == employee.EmployeeId))
                .Include(t => t.Position)
                .ThenInclude(p => p.Department));
        }

        // GET: api/employees/5?session=awewe
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(string session, [FromRoute] int id)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            var employee = await _context.Employees
                .Include(t => t.Position)
                .ThenInclude(p => p.Department)
                .SingleOrDefaultAsync(m => m.EmployeeId == id);

            if (employee == null)
            {
                Response.StatusCode = 405;
                return Json(ErrorType.NotFoundObject);
            }

            return Json(employee);
        }

        // PUT: api/employees/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(string session, [FromRoute] int id, [FromBody] Employee employee)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            if (id != employee.EmployeeId)
            {
                Response.StatusCode = 405;
                return Json(ErrorType.NotFoundObject);
            }

            _context.Entry(employee).State = EntityState.Modified;

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

        // POST: api/employees
        [HttpPost]
        public async Task<IActionResult> PostEmployee(string session, [FromBody] Employee employee)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return await GetEmployee(session, employee.EmployeeId);
        }

        // DELETE: api/employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(string session, [FromRoute] int id)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            var employee = await _context.Employees
                .Include(t => t.Position)
                .ThenInclude(p => p.Department)
                .SingleOrDefaultAsync(m => m.EmployeeId == id);

            if (employee == null)
            {
                Response.StatusCode = 405;
                return Json(ErrorType.NotFoundObject);
            }

            employee.Deleted = true;
            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Json(employee);
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return Json(ErrorType.InternalError);
            }
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}