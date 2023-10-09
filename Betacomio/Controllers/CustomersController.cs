using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Betacomio.Models;
using DBConnectionLibrary;
using ErrorLogLibrary.BusinessLogic;
using Microsoft.Extensions.Configuration;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Betacomio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;

        ErrorManager errManager;

        //Istanzio il context + la classe per effettuare il log degli errori
        public CustomersController(AdventureWorksLt2019Context context)
        {
            _context = context;

            var errorDB = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["ErrorDB"];
            var logPath = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["LogPath"];

            errManager = new(errorDB.ToString(), logPath.ToString());
           
        }

        // GET: api/Customers/GetCustomers
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

        //Questo metodo ritorna il customer con tutte le tabelle figlie per avere i dati completi
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

        //Trova il customer per id con tutte le tabelle figlie
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

        //Trova Customer per Email
        
        //api/Customers/GetCustomerByEmail
        [Route("GetCustomerByEmail/{Email}")]
        [HttpGet]
        public async Task<ActionResult<Customer>> GetCustomerByEmail(string Email)
        {


            if (_context.Customers == null)
            {
                return NotFound();
            }
            var Customer = await _context.Customers.FirstOrDefaultAsync(c => c.EmailAddress.Equals(Email));

            if (Customer == null)
            {
                return NotFound();
            }


            return Customer;

        }
        //Trova il customer semplicemente per id
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

        //Aggiorna un nuovo Customer tramite ID
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
                errManager.SaveException("dbo.Errors", ex, "CustomersController", "PutCustomer", DateTime.Now, "");
                return Problem("Errore nell'aggiornamento dati.", statusCode: 500);
            
            }

            return NoContent();
        }
        //api/Customers/PutCustomerByEmail
        [Route("PutCustomerByEmail/{Email}")]
        [HttpPut]
        public async Task<IActionResult> PutCustomerByEmail(string Email, Customer customer)
        {
            //if (Email != customer.EmailAddress)
            //{
            //    return BadRequest("Invalid customer Email.");
            //}

            try
            {
                var existingCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.EmailAddress.Equals(Email));
                if (existingCustomer == null)
                {
                    return NotFound();
                }

                existingCustomer.FirstName = customer.FirstName;
                existingCustomer.LastName = customer.LastName;
                existingCustomer.NameStyle = customer.NameStyle;
                existingCustomer.ModifiedDate = DateTime.Now;
                existingCustomer.PasswordHash = "";
                existingCustomer.PasswordSalt = "";
                existingCustomer.Phone = customer.Phone;



                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                errManager.SaveException("dbo.Errors", ex, "CustomersController", "PutCustomer", DateTime.Now, "");
                return Problem("Errore nell'aggiornamento dati.", statusCode: 500);

            }

            return NoContent();
        }
        //Aggiunge un Customer nuovo
        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
          if (_context.Customers == null)
          {
              return Problem("Entity set 'AdventureWorksLt2019Context.Customers'  is null.");
          }
            try
            {
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

            } catch(Exception ex)
            {
                errManager.SaveException("dbo.Errors", ex, "CustomersController", "PostCustomer", DateTime.Now, "");
            }
            

            return CreatedAtAction("GetCustomer", new { id = customer.CustomerId }, customer);
        }

        //Elimina Customer (Da aggiungere l'eliminazione a cascata)
        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
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

            try
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();

            } catch(Exception ex)
            {
                errManager.SaveException("dbo.Errors", ex, "CustomersController", "DeleteCustomer", DateTime.Now, "");

            }
            

            return NoContent();
        }

        private bool CustomerExists(int id)
        {
            return (_context.Customers?.Any(e => e.CustomerId == id)).GetValueOrDefault();
        }
    }
}
