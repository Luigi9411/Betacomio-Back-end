using Betacomio.Models;
using DBConnectionLibrary;
using ErrorLogLibrary.BusinessLogic;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Betacomio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        private readonly AdventureLoginContext _context;
        private readonly AdventureWorksLt2019Context _context2;

        private ErrorManager errManager;
        public LoginController(AdventureLoginContext context, AdventureWorksLt2019Context context2)
        {
            _context = context;
            _context2 = context2;

            var errorDB = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["ErrorDB"];
            var logPath = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["LogPath"];


            errManager = new(errorDB.ToString(), logPath.ToString());
        }

        //api/Login
        [HttpPost]
        public IActionResult Auth(Login login)
        {
            try
            {

                if (login == null) { return BadRequest(string.Empty); }
                else if (string.IsNullOrEmpty(login.Username) ||
                    string.IsNullOrEmpty(login.Password)) { return BadRequest(string.Empty); }

                Encryption encryption = new Encryption();

                foreach(NewCustomer cus in _context.NewCustomers)
                {
                    if(cus.EmailAddress == login.Username && encryption.checkPassword(login.Password, cus.PasswordHash, cus.PasswordSalt))
                    {

                        if(cus.Role == "Admin")
                        {
                            return NoContent();
                        }
                        else
                        {
                            return Ok();

                        }
                    
                    }
                }

                
                foreach(Customer? cus in _context2.Customers)
                {
                    if(cus == null)
                    {
                        continue;
                    }

                    if(cus.EmailAddress == null)
                    {
                        continue;
                    }
                    
                    if (cus.EmailAddress.Equals(login.Username) && encryption.checkPassword(login.Password, cus.PasswordHash, cus.PasswordSalt))
                    {
                        return NotFound();
                    }  
                }

              

            }
            catch (Exception ex)
            {
                errManager.SaveException("dbo.Errors", ex, "LoginController", "Auth", DateTime.Now, "Problema con il login");
                return BadRequest(string.Empty);
            }

            return BadRequest("Username o password not matching");
        }
    }

}
