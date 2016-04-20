using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Macrodeek.StarDustServiceBase
{
    public class PasswordHashGenerator
    {
        public static string Gernate(string plainPassword)
        {
            string salt = plainPassword;
            SHA256 mySHA256 = SHA256Managed.Create();
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(plainPassword + salt);
            byte[] hashValue = mySHA256.ComputeHash(byteArray);
            string hashString = Convert.ToBase64String(hashValue);
            return hashString;
        }
    }
}
