using Macrodeek.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GithubGraberLib.Domain
{
    public class ExtractRequest
    {
        public GithubAccount Account { get; set; }
        public string AccessToken { get; set; }
        public string Url { get; set; }
        public int StartPage { get; set; }
        public int PerPage { get; set; }
        public int StartIndex { get; set; }
    }
}
