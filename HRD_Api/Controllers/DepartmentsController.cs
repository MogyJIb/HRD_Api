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
    [Route("api/departments")]
    public class DepartmentsController : Controller
    {
        private readonly HRD_DbContext _context;

        public DepartmentsController(HRD_DbContext context)
        {
            _context = context;
        }

        // GET: api/departments{?deleted=false}
        [HttpGet]
        public async Task<IActionResult> GetDepartments(string session, bool deleted = false)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }
            
            return Json(_context.Departments.Where(department => department.Deleted == deleted));
        }

        // GET: api/departments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartment(string session, [FromRoute] int id)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            var department = await _context.Departments.SingleOrDefaultAsync(m => m.DepartmentId == id);

            if (department == null)
            {
                Response.StatusCode = 405;
                return Json(ErrorType.NotFoundObject);
            }

            return Json(department);
        }

        // PUT: api/departments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartment(string session, [FromRoute] int id, [FromBody] Department department)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }
            
            if (id != department.DepartmentId)
            {
                Response.StatusCode = 405;
                return Json(ErrorType.NotFoundObject);
            }

            _context.Entry(department).State = EntityState.Modified;

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

        // POST: api/departments
        [HttpPost]
        public async Task<IActionResult> PostDepartment(string session, [FromBody] Department department)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return await GetDepartment(session, department.DepartmentId);
        }

        // DELETE: api/departments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(string session, [FromRoute] int id)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            var department = await _context.Departments.SingleOrDefaultAsync(m => m.DepartmentId == id);
            if (department == null)
            {
                Response.StatusCode = 405;
                return Json(ErrorType.NotFoundObject);
            }

            department.Deleted = true;
            _context.Entry(department).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Json(department);
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return Json(ErrorType.InternalError);
            }
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.DepartmentId == id);
        }
    }
}