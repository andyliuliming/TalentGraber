using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GithubGraberLib
{
    public class GithubException : Exception
    {
        public GithubException(string msg) : base(msg)
        {

        }
    }
}
