using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Betacomio.Models;

namespace Betacomio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VProductDescriptionPricesController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;

        public VProductDescriptionPricesController(AdventureWorksLt2019Context context)
        {
            _context = context;
        }

        // GET: api/VProductDescriptionPrices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VProductDescriptionPrice>>> GetVProductDescriptionPrices()
        {
          if (_context.VProductDescriptionPrices == null)
          {
              return NotFound();
          }
            return await _context.VProductDescriptionPrices.ToListAsync();
        }

        // GET: api/VProductDescriptionPrices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VProductDescriptionPrice>> GetVProductDescriptionPrice(int id)
        {
          if (_context.VProductDescriptionPrices == null)
          {
              return NotFound();
          }
            var vProductDescriptionPrice = await _context.VProductDescriptionPrices.FindAsync(id);

            if (vProductDescriptionPrice == null)
            {
                return NotFound();
            }

            return vProductDescriptionPrice;
        }

        //Get nome e categoria
        //api/VProductDescriptionPrices/GetProductsByCategoryAndName

        [Route("GetProductsByCategoryAndName/{searchValue}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VProductDescriptionPrice>>> GetVProductsByCategoryAndName(string searchvalue)
        {

            if (_context.VProductDescriptionPrices == null)
            {
                return NotFound();
            }
            var products = await _context.VProductDescriptionPrices
                .Where(p => p.Name.ToUpper().Contains(searchvalue.ToUpper()) || p.CategoryName.ToUpper().Contains(searchvalue.ToUpper()))
                .ToListAsync();

            if (products == null)
            {
                return NotFound();
            }

            return products;
        }



        // PUT: api/VProductDescriptionPrices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVProductDescriptionPrice(int id, VProductDescriptionPrice vProductDescriptionPrice)
        {
            if (id != vProductDescriptionPrice.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(vProductDescriptionPrice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VProductDescriptionPriceExists(id))
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

        // POST: api/VProductDescriptionPrices
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<VProductDescriptionPrice>> PostVProductDescriptionPrice(VProductDescriptionPrice vProductDescriptionPrice)
        {
          if (_context.VProductDescriptionPrices == null)
          {
              return Problem("Entity set 'AdventureWorksLt2019Context.VProductDescriptionPrices'  is null.");
          }
            _context.VProductDescriptionPrices.Add(vProductDescriptionPrice);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (VProductDescriptionPriceExists(vProductDescriptionPrice.ProductId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetVProductDescriptionPrice", new { id = vProductDescriptionPrice.ProductId }, vProductDescriptionPrice);
        }

        // DELETE: api/VProductDescriptionPrices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVProductDescriptionPrice(int id)
        {
            if (_context.VProductDescriptionPrices == null)
            {
                return NotFound();
            }
            var vProductDescriptionPrice = await _context.VProductDescriptionPrices.FindAsync(id);
            if (vProductDescriptionPrice == null)
            {
                return NotFound();
            }

            _context.VProductDescriptionPrices.Remove(vProductDescriptionPrice);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VProductDescriptionPriceExists(int id)
        {
            return (_context.VProductDescriptionPrices?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}
