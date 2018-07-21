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
    [Route("api/rewards")]
    public class RewardsController : Controller
    {
        private readonly HRD_DbContext _context;

        public RewardsController(HRD_DbContext context)
        {
            _context = context;
        }

        // GET: api/rewards
        [HttpGet]
        public IEnumerable<Reward> GetRewards()
        {
            return _context.Rewards;
        }

        // GET: api/rewards/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReward([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reward = await _context.Rewards.SingleOrDefaultAsync(m => m.RewardId == id);

            if (reward == null)
            {
                return NotFound();
            }

            return Ok(reward);
        }

        // PUT: api/rewards/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReward([FromRoute] int id, [FromBody] Reward reward)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != reward.RewardId)
            {
                return BadRequest();
            }

            _context.Entry(reward).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RewardExists(id))
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

        // POST: api/rewards
        [HttpPost]
        public async Task<IActionResult> PostReward([FromBody] Reward reward)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Rewards.Add(reward);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReward", new { id = reward.RewardId }, reward);
        }

        // DELETE: api/rewards/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReward([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reward = await _context.Rewards.SingleOrDefaultAsync(m => m.RewardId == id);
            if (reward == null)
            {
                return NotFound();
            }

            _context.Rewards.Remove(reward);
            await _context.SaveChangesAsync();

            return Ok(reward);
        }

        private bool RewardExists(int id)
        {
            return _context.Rewards.Any(e => e.RewardId == id);
        }
    }
}