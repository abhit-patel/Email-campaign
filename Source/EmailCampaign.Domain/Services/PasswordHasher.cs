using EmailCampaign.Domain.Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Services
{
    public class PasswordHasher
    {

        public HashedPassVM HashPassword(string password)
        {
            var saltkey = GenerateSalt();
            var hashedPassword = Hash(password, saltkey);
            
            return new HashedPassVM
            {
                saltKey = saltkey,
                hashedPassword = hashedPassword
            };
        }

        public bool VerifyPassword(string password, string hashedPassword, string salt)
        {
            var hashedInputPassword = Hash(password, salt);
            return hashedInputPassword == hashedPassword;
        }

        private string GenerateSalt()
        {
            var saltBytes = new byte[16];
            using (var item = new RNGCryptoServiceProvider())
            {
                item.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        private string Hash(string password, string salt)
        {
            using (var item = SHA256.Create())
            {
                var saltedPassword = password + salt;
                var hashedBytes = item.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
