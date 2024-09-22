using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Services
{
    public class PasswordHasher
    {
        public (string hashedPassword, string salt) HashPassword(string password)
        {
            var salt = GenerateSalt();
            var hashedPassword = Hash(password, salt);
            return (hashedPassword, salt);
        }

        public bool VerifyPassword(string password, string hashedPassword, string salt)
        {
            var hashedInputPassword = Hash(password, salt);
            return hashedInputPassword == hashedPassword;
        }

        private string GenerateSalt()
        {
            var saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        private string Hash(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = password + salt;
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
