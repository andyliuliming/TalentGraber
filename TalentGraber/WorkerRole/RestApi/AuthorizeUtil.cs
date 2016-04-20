using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GithubGraberLib
{
    public class AuthorizeUtil
    {
        public const string TokenHeaderName = "Authorization";
        public static string GetToken(string userName,string passWord)
        {
            string origin = userName + ":" + passWord;
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(origin);
            string basicAuthorize = Convert.ToBase64String(byteArray);
            return basicAuthorize;
        }
    }
}
