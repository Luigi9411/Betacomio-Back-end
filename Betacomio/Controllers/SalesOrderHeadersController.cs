using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Betacomio.Models;
using ErrorLogLibrary.BusinessLogic;
using System.Configuration;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace Betacomio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesOrderHeadersController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;

        private ErrorManager errManager;

        public SalesOrderHeadersController(AdventureWorksLt2019Context context)
        {
            _context = context;

            var errorDB = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["ErrorDB"];
            var logPath = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["LogPath"];

            errManager = new(errorDB.ToString(), logPath.ToString());
        }

        // GET: api/SalesOrderHeaders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalesOrderHeader>>> GetSalesOrderHeaders()
        {
          if (_context.SalesOrderHeaders == null)
          {
              return NotFound();
          }
            return await _context.SalesOrderHeaders.ToListAsync();
        }

        // GET: api/SalesOrderHeaders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SalesOrderHeader>> GetSalesOrderHeader(int id)
        {
          if (_context.SalesOrderHeaders == null)
          {
              return NotFound();
          }
            var salesOrderHeader = await _context.SalesOrderHeaders.FindAsync(id);

            if (salesOrderHeader == null)
            {
                return NotFound();
            }

            return salesOrderHeader;
        }

        // PUT: api/SalesOrderHeaders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSalesOrderHeader(int id, SalesOrderHeader salesOrderHeader)
        {
            if (id != salesOrderHeader.SalesOrderId)
            {
                return BadRequest();
            }

            _context.Entry(salesOrderHeader).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!SalesOrderHeaderExists(id))
                {
                    errManager.SaveException("dbo.Errors", ex, "SalesOrderheaderController", "PutSalesHeader", DateTime.Now, "");
                    return NotFound();
                }
                else
                {
                    errManager.SaveException("dbo.Errors", ex, "SalesOrderheaderController", "PutSalesHeader", DateTime.Now, "");
                }
            }

            return NoContent();
        }

        // POST: api/SalesOrderHeaders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SalesOrderHeader>> PostSalesOrderHeader(SalesOrderHeader salesOrderHeader)
        {
          if (_context.SalesOrderHeaders == null)
          {
              return Problem("Entity set 'AdventureWorksLt2019Context.SalesOrderHeaders'  is null.");
          }
            try
            {
                _context.SalesOrderHeaders.Add(salesOrderHeader);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                errManager.SaveException("dbo.Errors", ex, "SalesOrderheaderController", "PostSalesHeader", DateTime.Now, "");
            }
           

            return CreatedAtAction("GetSalesOrderHeader", new { id = salesOrderHeader.SalesOrderId }, salesOrderHeader);
        }

        // DELETE: api/SalesOrderHeaders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalesOrderHeader(int id)
        {
            if (_context.SalesOrderHeaders == null)
            {
                return NotFound();
            }
            var salesOrderHeader = await _context.SalesOrderHeaders.FindAsync(id);
            if (salesOrderHeader == null)
            {
                return NotFound();
            }

            try
            {
                _context.SalesOrderHeaders.Remove(salesOrderHeader);
                await _context.SaveChangesAsync();

            } catch(Exception ex)
            {
                errManager.SaveException("dbo.Errors", ex, "SalesOrderheaderController", "DeleteSalesHeader", DateTime.Now, "");
            }

           

            return NoContent();
        }

        private bool SalesOrderHeaderExists(int id)
        {
            return (_context.SalesOrderHeaders?.Any(e => e.SalesOrderId == id)).GetValueOrDefault();
        }
    }
}
