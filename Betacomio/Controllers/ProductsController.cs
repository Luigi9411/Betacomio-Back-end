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
    public class ProductsController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;

        ErrorManager errManager;

        public ProductsController(AdventureWorksLt2019Context context)
        {
            _context = context;

            var errorDB = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["ErrorDB"];
            var logPath = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["LogPath"];

            errManager = new(errorDB.ToString(), logPath.ToString());
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
          if (_context.Products == null)
          {
              return NotFound();
          }
            return await _context.Products.ToListAsync();
        }

        // GET: api/Products/5
        [Route("GetProductById/{id}")]
        [HttpGet]
        public async Task<ActionResult<Product>> GetProductbyId(int id)
        {
          if (_context.Products == null)
          {
              return NotFound();
          }
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        [Route("GetProductsByName/{name}")]
        [HttpGet("{name}")]
        public async Task<ActionResult<List<Product>>> GetProductbyName(string name)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            //var product = await _context.Products.FindAsync(name);

            //List<Product> products = new List<Product>();

            //products = _context.Products.Where(_context => _context.Name == name).ToList();

            try
            {
                var products = await _context.Products
              .Where(e => e.Name.ToUpper().Contains(name.ToUpper()))
              .ToListAsync();

                if (products == null)
                {
                    return NotFound();
                }

                return products;

            } catch(Exception ex)
            {
                errManager.SaveException("dbo.Errors", ex, "ProductsController", "GetProductByName", DateTime.Now, "");
            }

            return NotFound();
            
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ProductExists(id))
                {
                    errManager.SaveException("dbo.Errors", ex, "ProductsController", "GetProductByName", DateTime.Now, "Not found");
                    return NotFound();
                }
                else
                {

                    errManager.SaveException("dbo.Errors", ex, "ProductsController", "GetProductByName", DateTime.Now, "");
                }
            }

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
          if (_context.Products == null)
          {
              return Problem("Entity set 'AdventureWorksLt2019Context.Products'  is null.");
          }

            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

            } catch(Exception ex)
            {
                errManager.SaveException("dbo.Errors", ex, "ProductsController", "PostProduct", DateTime.Now, "");
            }
            

            return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

            } catch(Exception ex)
            {
                errManager.SaveException("dbo.Errors", ex, "ProductsController", "PostProduct", DateTime.Now, "");
            }
            

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}
