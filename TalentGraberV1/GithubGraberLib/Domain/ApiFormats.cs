using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GithubGraberLib
{
    public class ApiFormats
    {
        public const string BaseUri = "https://api.github.com";
        // {0} is the "login" of the User
        public const string UserApi = "/users/{0}";
        // {0} is owner {1} is repository {2} is branch
        public const string CommitRelativePathPattern = "/repos/{0}/{1}/commits";

    }
}
