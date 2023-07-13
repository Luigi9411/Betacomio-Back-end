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
    public class NewCustomersController : ControllerBase
    {
        private readonly AdventureLoginContext _context;
        private readonly AdventureWorksLt2019Context _context2;

        public NewCustomersController(AdventureLoginContext context, AdventureWorksLt2019Context context2)
        {
            _context = context;
            _context2 = context2;
        }

        // GET: api/NewCustomers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NewCustomer>>> GetNewCustomers()
        {
          if (_context.NewCustomers == null)
          {
              return NotFound();
          }
            return await _context.NewCustomers.ToListAsync();
        }

        // GET: api/NewCustomers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NewCustomer>> GetNewCustomer(int id)
        {
          if (_context.NewCustomers == null)
          {
              return NotFound();
          }
            var newCustomer = await _context.NewCustomers.FindAsync(id);

            if (newCustomer == null)
            {
                return NotFound();
            }

            return newCustomer;
        }

        // PUT: api/NewCustomers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNewCustomer(int id, NewCustomer newCustomer)
        {
            if (id != newCustomer.CustomerId)
            {
                return BadRequest();
            }

            _context.Entry(newCustomer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NewCustomerExists(id))
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

        // POST: api/NewCustomers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NewCustomer>> PostNewCustomer(NewCustomer newCustomer)
        {
            if (_context.NewCustomers == null)
            {
                return Problem("Entity set 'AdventureLoginContext.NewCustomers'  is null.");
            }

            //Qui si cerca tra i customer nuovi se l'indirizzo email è già in uso
            if (_context.NewCustomers.Any(cl => cl.EmailAddress == newCustomer.EmailAddress))
            {
                return Conflict("Un utente con lo stesso nome utente esiste già.");
            }


            string passHashNew;
            string passSaltNew;

            try
            {
                //Qui encripto la password e ottengo la password criptata con il sale
                Encryption encryption = new Encryption();
                KeyValuePair<string, string> passPair = encryption.EncryptPassword(newCustomer.PasswordHash);

                passHashNew = passPair.Key;
                passSaltNew = passPair.Value;

            }
            catch (Exception ex)
            {
                return Problem("Problema con l'encriptazione della password");
            }

            //Qui si cerca nella tabella customer vecchia se l'email è già in uso
            //il risultato lo metto nel booleano newCus
            Customer retrievedData = new();
            bool newMail = true;
            bool newPass = true;
           

            foreach (Customer cus in _context2.Customers)
            {
                if (cus.EmailAddress == newCustomer.EmailAddress)
                {
                    retrievedData = cus;
                    retrievedData.ModifiedDate = DateTime.Now;
                    newMail = false;

                    try
                    {
                        Encryption encr = new Encryption();

                        if(encr.checkPassword(newCustomer.PasswordHash, cus.PasswordHash, cus.PasswordSalt))
                        {
                            passHashNew = cus.PasswordHash;
                            passSaltNew = cus.PasswordSalt;

                            newPass = false;

                            cus.PasswordHash = "";
                            cus.PasswordSalt = "";
                        }

                    }
                    catch(Exception ex)
                    {
                        return Problem("Encriptazione password non riuscita");
                    }

                }

                
            }

            newCustomer.PasswordHash = passHashNew;
            newCustomer.PasswordSalt = passSaltNew;

            retrievedData.CustomerId = 0;

            if (newMail)
            {

                Guid guid = Guid.NewGuid();

                retrievedData.EmailAddress = newCustomer.EmailAddress;
                retrievedData.FirstName = "";
                retrievedData.LastName = "";
                retrievedData.NameStyle = false;
                retrievedData.PasswordHash = "";
                retrievedData.PasswordSalt = "";
                retrievedData.ModifiedDate = DateTime.Now;
                retrievedData.Rowguid = guid;

                _context2.Customers.Add(retrievedData);
                _context.NewCustomers.Add(newCustomer);

                _context2.SaveChanges();

            }
            else
            {
                if (!newPass)
                {
                    
                    _context.NewCustomers.Add(newCustomer);
                }
                else
                {
                    return Problem("Email e password non corrispondono");
                }
               
            }
            
            
            await _context.SaveChangesAsync();
            


            return CreatedAtAction("GetNewCustomer", new { id = newCustomer.CustomerId }, newCustomer);
        }


        // DELETE: api/NewCustomers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNewCustomer(int id)
        {
            if (_context.NewCustomers == null)
            {
                return NotFound();
            }
            var newCustomer = await _context.NewCustomers.FindAsync(id);
            if (newCustomer == null)
            {
                return NotFound();
            }

            _context.NewCustomers.Remove(newCustomer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NewCustomerExists(int id)
        {
            return (_context.NewCustomers?.Any(e => e.CustomerId == id)).GetValueOrDefault();
        }
    }
}
