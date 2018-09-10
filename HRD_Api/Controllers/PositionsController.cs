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
    [Route("api/positions")]
    public class PositionsController : Controller
    {
        private readonly HRD_DbContext _context;

        public PositionsController(HRD_DbContext context)
        {
            _context = context;
        }

        // GET: api/positions{?deleted=false}
        [HttpGet]
        public async Task<IActionResult> GetPositions(string session, bool deleted = false)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            return Json(_context.Positions
                .Where(position => position.Deleted == deleted)
                .Include(t => t.Department));
        }

        // GET: api/positions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPosition(string session, [FromRoute] int id)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            var position = await _context.Positions
                .Include(t => t.Department)
                .SingleOrDefaultAsync(m => m.PositionId == id);

            if (position == null)
            {
                Response.StatusCode = 405;
                return Json(ErrorType.NotFoundObject);
            }

            return Json(position);
        }

        // PUT: api/positions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPosition(string session, [FromRoute] int id, [FromBody] Position position)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            if (id != position.PositionId)
            {
                Response.StatusCode = 405;
                return Json(ErrorType.NotFoundObject);
            }

            _context.Entry(position).State = EntityState.Modified;

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

        // POST: api/positions
        [HttpPost]
        public async Task<IActionResult> PostPosition(string session, [FromBody] Position position)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            _context.Positions.Add(position);
            await _context.SaveChangesAsync();

            return await GetPosition(session, position.PositionId);
        }

        // DELETE: api/positions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePosition(string session, [FromRoute] int id)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            var position = await _context.Positions
                .Include(t => t.Department)
                .SingleOrDefaultAsync(m => m.PositionId == id);

            if (position == null)
            {
                Response.StatusCode = 405;
                return Json(ErrorType.NotFoundObject);
            }

            position.Deleted = true;
            _context.Entry(position).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Json(position);
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return Json(ErrorType.InternalError);
            }
        }

        private bool PositionExists(int id)
        {
            return _context.Positions.Any(e => e.PositionId == id);
        }
    }
}