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
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Betacomio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesOrderDetailsController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;

        private ErrorManager errManager;

        public SalesOrderDetailsController(AdventureWorksLt2019Context context)
        {
            _context = context;

            var errorDB = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["ErrorDB"];
            var logPath = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["LogPath"];

            errManager = new(errorDB.ToString(), logPath.ToString());
        }

        // GET: api/SalesOrderDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalesOrderDetail>>> GetSalesOrderDetails()
        {
          if (_context.SalesOrderDetails == null)
          {
              return NotFound();
          }
            return await _context.SalesOrderDetails.ToListAsync();
        }

        // GET: api/SalesOrderDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SalesOrderDetail>> GetSalesOrderDetail(int id)
        {
          if (_context.SalesOrderDetails == null)
          {
              return NotFound();
          }
            var salesOrderDetail = await _context.SalesOrderDetails.FindAsync(id);

            if (salesOrderDetail == null)
            {
                return NotFound();
            }



            return salesOrderDetail;
        }

        // PUT: api/SalesOrderDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSalesOrderDetail(int id, SalesOrderDetail salesOrderDetail)
        {
            if (id != salesOrderDetail.SalesOrderId)
            {
                return BadRequest();
            }

            _context.Entry(salesOrderDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!SalesOrderDetailExists(id))
                {
                    errManager.SaveException("dbo.Errors", ex, "SalesOrderDetailController", "PutSalesOrderDetail", DateTime.Now, "");
                    return NotFound();
                }
                else
                {
                    errManager.SaveException("dbo.Errors", ex, "SalesOrderDetailController", "PutSalesOrderDetail", DateTime.Now, "");
                }
            }

            return NoContent();
        }

        // POST: api/SalesOrderDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult<SalesOrderDetail>> PostSalesOrderDetail(SalesOrderDetail salesOrderDetail)
        {
          if (_context.SalesOrderDetails == null)
          {
              return Problem("Entity set 'AdventureWorksLt2019Context.SalesOrderDetails'  is null.");
          }
            _context.SalesOrderDetails.Add(salesOrderDetail);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (SalesOrderDetailExists(salesOrderDetail.SalesOrderId))
                {
                    errManager.SaveException("dbo.Errors", ex, "SalesOrderDetailController", "PostSalesOrderDetail", DateTime.Now, "");
                    return BadRequest();
                }
                else
                {
                    errManager.SaveException("dbo.Errors", ex, "SalesOrderDetailController", "PostSalesOrderDetail", DateTime.Now, "");
                    return BadRequest();
                }
            }

            return CreatedAtAction("GetSalesOrderDetail", new { id = salesOrderDetail.SalesOrderId }, salesOrderDetail);
        }

        // DELETE: api/SalesOrderDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalesOrderDetail(int id)
        {
            if (_context.SalesOrderDetails == null)
            {
                return NotFound();
            }
            var salesOrderDetail = await _context.SalesOrderDetails.FindAsync(id);
            if (salesOrderDetail == null)
            {
                return NotFound();
            }

            try
            {
                _context.SalesOrderDetails.Remove(salesOrderDetail);
                await _context.SaveChangesAsync();

            } 
            catch(Exception ex)
            {
                errManager.SaveException("dbo.Errors", ex, "SalesOrderDetailController", "DeleteSalesOrderDetail", DateTime.Now, "");
            }
           

            return NoContent();
        }

        private bool SalesOrderDetailExists(int id)
        {
            return (_context.SalesOrderDetails?.Any(e => e.SalesOrderId == id)).GetValueOrDefault();
        }
    }
}
