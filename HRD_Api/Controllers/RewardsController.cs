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
    [Route("api/rewards")]
    public class RewardsController : Controller
    {
        private readonly HRD_DbContext _context;

        public RewardsController(HRD_DbContext context)
        {
            _context = context;
        }

        // GET: api/rewards{?deleted=false}
        [HttpGet]
        public async Task<IActionResult> GetRewards(string session, bool deleted = false)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            return Json(_context.Rewards
                .Where(reward => reward.Deleted == deleted)
                .Include(t => t.Employee)
                .ThenInclude(e => e.Position)
                .ThenInclude(p => p.Department));
        }

        // GET: api/rewards/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReward(string session, [FromRoute] int id)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            var reward = await _context.Rewards
                .Include(t => t.Employee)
                .ThenInclude(e => e.Position)
                .ThenInclude(p => p.Department)
                .SingleOrDefaultAsync(m => m.RewardId == id);

            if (reward == null)
            {
                Response.StatusCode = 405;
                return Json(ErrorType.NotFoundObject);
            }

            return Json(reward);
        }

        // PUT: api/rewards/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReward(string session, [FromRoute] int id, [FromBody] Reward reward)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            if (id != reward.RewardId)
            {
                Response.StatusCode = 405;
                return Json(ErrorType.NotFoundObject);
            }

            _context.Entry(reward).State = EntityState.Modified;

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

        // POST: api/rewards
        [HttpPost]
        public async Task<IActionResult> PostReward(string session, [FromBody] Reward reward)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            _context.Rewards.Add(reward);
            await _context.SaveChangesAsync();

            return await GetReward(session, reward.RewardId);
        }

        // DELETE: api/rewards/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReward(string session, [FromRoute] int id)
        {
            if (!SessionLogic.Instance.Valid(session))
            {
                Response.StatusCode = 403;
                return Json(ErrorType.AuthanticationFaild);
            }

            var reward = await _context.Rewards
                .Include(t => t.Employee)
                .ThenInclude(e => e.Position)
                .ThenInclude(p => p.Department)
                .SingleOrDefaultAsync(m => m.RewardId == id);

            if (reward == null)
            {
                Response.StatusCode = 405;
                return Json(ErrorType.NotFoundObject);
            }

            reward.Deleted = true;
            _context.Entry(reward).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Json(reward);
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return Json(ErrorType.InternalError);
            }
        }

        private bool RewardExists(int id)
        {
            return _context.Rewards.Any(e => e.RewardId == id);
        }
    }
}