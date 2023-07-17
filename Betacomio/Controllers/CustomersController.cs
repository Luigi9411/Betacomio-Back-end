using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Betacomio.Models;
using DBConnectionLibrary;

namespace Betacomio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;

        public CustomersController(AdventureWorksLt2019Context context)
        {
            _context = context;
        }

        // GET: api/Customers
        [Route("GetCustomers")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
          if (_context.Customers == null)
          {
              return NotFound();
          } 
       
            return await _context.Customers.ToListAsync();
        }

        [Route("GetCustomersComplete")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomersComplete()
        {
            if (_context.Customers == null)
            {
                return NotFound();
            }

            return await _context.Customers
                .Include(c => c.CustomerAddresses)
                .ThenInclude(a => a.Address)
                .Include(c => c.SalesOrderHeaders)
                .ThenInclude(s => s.SalesOrderDetails)
                .ThenInclude(p => p.Product)
                .ToListAsync();
        }

        [Route("GetCustomersCompleteById/{id}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomersCompleteById(int id)
        {
            if (_context.Customers == null)
            {
                return NotFound();
            }

            return await _context.Customers
                .Include(c => c.CustomerAddresses)
                .ThenInclude(a => a.Address)
                .Include(c => c.SalesOrderHeaders)
                .ThenInclude(s => s.SalesOrderDetails)
                .ThenInclude(p => p.Product)
                .Where(c => c.CustomerId == id)
                .ToListAsync();
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
          if (_context.Customers == null)
          {
              return NotFound();
          }
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return BadRequest("Invalid customer ID.");
            }

            try
            {
                var existingCustomer = await _context.Customers.FindAsync(id);
                if (existingCustomer == null)
                {
                    return NotFound();
                }

                existingCustomer.FirstName = customer.FirstName;
                existingCustomer.LastName = customer.LastName;
                existingCustomer.NameStyle = customer.NameStyle;
                existingCustomer.ModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Problem("Errore nell'aggiornamento dati.", statusCode: 500);
            }

            return NoContent();
        }

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
          if (_context.Customers == null)
          {
              return Problem("Entity set 'AdventureWorksLt2019Context.Customers'  is null.");
          }
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCustomer", new { id = customer.CustomerId }, customer);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            if (_context.Customers == null)
            {
                return NotFound();
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerExists(int id)
        {
            return (_context.Customers?.Any(e => e.CustomerId == id)).GetValueOrDefault();
        }
    }
}
