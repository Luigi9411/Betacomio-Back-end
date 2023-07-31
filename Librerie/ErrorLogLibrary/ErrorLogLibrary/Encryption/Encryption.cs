using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ErrorLogLibrary.Encryption
{
    public class Encryption
    {

        public KeyValuePair<string, string> EncryptPassword(string pass)
        {
            KeyValuePair<string, string> encrPass = new KeyValuePair<string, string>();

            byte[] byteSalt = new byte[32];

            try
            {
                byteSalt = RandomNumberGenerator.GetBytes(16);
                string sHashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: pass,
                    salt: byteSalt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 32
                    ));

                encrPass = new KeyValuePair<string, string>(sHashed, Convert.ToBase64String(byteSalt));


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }

            return encrPass;
        }

        public bool checkPassword(string pass, string passHash, string Salt)
        {
            byte[] byteSalt = Convert.FromBase64String(Salt);
           

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: pass,
                salt: byteSalt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 32));

            Console.WriteLine(hashed);
            Console.WriteLine(passHash);

            return (hashed == passHash);

        }
    }
}
