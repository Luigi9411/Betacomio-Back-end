using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Betacomio.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Betacomio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VOrderHistoriesController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;

        public VOrderHistoriesController(AdventureWorksLt2019Context context)
        {
            _context = context;
        }

        // GET: api/VOrderHistories
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<VOrderHistory>>> GetVOrderHistories()
        {
          if (_context.VOrderHistories == null)
          {
              return NotFound();
          }
            return await _context.VOrderHistories.ToListAsync();
        }

        // GET: api/VOrderHistories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VOrderHistory>> GetVOrderHistory(int id)
        {
          if (_context.VOrderHistories == null)
          {
              return NotFound();
          }
            var vOrderHistory = await _context.VOrderHistories.FindAsync(id);

            if (vOrderHistory == null)
            {
                return NotFound();
            }

            return vOrderHistory;
        }

        // PUT: api/VOrderHistories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVOrderHistory(int id, VOrderHistory vOrderHistory)
        {
            if (id != vOrderHistory.SalesOrderId)
            {
                return BadRequest();
            }

            _context.Entry(vOrderHistory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VOrderHistoryExists(id))
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

        // POST: api/VOrderHistories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<VOrderHistory>> PostVOrderHistory(VOrderHistory vOrderHistory)
        {
          if (_context.VOrderHistories == null)
          {
              return Problem("Entity set 'AdventureWorksLt2019Context.VOrderHistories'  is null.");
          }
            _context.VOrderHistories.Add(vOrderHistory);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (VOrderHistoryExists(vOrderHistory.SalesOrderId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetVOrderHistory", new { id = vOrderHistory.SalesOrderId }, vOrderHistory);
        }

        // DELETE: api/VOrderHistories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVOrderHistory(int id)
        {
            if (_context.VOrderHistories == null)
            {
                return NotFound();
            }
            var vOrderHistory = await _context.VOrderHistories.FindAsync(id);
            if (vOrderHistory == null)
            {
                return NotFound();
            }

            _context.VOrderHistories.Remove(vOrderHistory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VOrderHistoryExists(int id)
        {
            return (_context.VOrderHistories?.Any(e => e.SalesOrderId == id)).GetValueOrDefault();
        }
    }
}
