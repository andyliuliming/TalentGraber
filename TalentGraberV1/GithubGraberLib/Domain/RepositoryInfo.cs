using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GithubGraberLib.Domain
{
    public class RepositoryInfo
    {
        public long id { get; set; }
        public string repository { get; set; }
        public string branch { get; set; }
        public Author owner { get; set; }
        public string language { get; set; }
    }
}
