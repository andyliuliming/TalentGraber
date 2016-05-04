using GithubGraberLib;
using Macrodeek.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole
{
    public class AccountResource
    {
        private GoldModelContainer db = new GoldModelContainer();
        private List<GithubAccount> githubAccounts;
        private int accountIndex = 0;
        public AccountResource()
        {
            accountIndex = 0;
            githubAccounts = new List<GithubAccount>();
        }

        public string GetNextAccessToken()
        {
            if (accountIndex < this.githubAccounts.Count && this.githubAccounts.Count != 1)
            {
                accountIndex++;
                string accessToken = AuthorizeUtil.GetToken(githubAccounts[accountIndex].UserName, githubAccounts[accountIndex].Password);
                return accessToken;
            }
            else
            {
                githubAccounts = db.GithubAccounts.ToList<GithubAccount>();
                accountIndex = 0;
                return this.GetCurrentAccessToken();
            }
        }

        public string GetCurrentAccessToken()
        {
            if (githubAccounts == null || githubAccounts.Count == 0)
            {
                githubAccounts = db.GithubAccounts.ToList<GithubAccount>();
            }
            string accessToken = AuthorizeUtil.GetToken(githubAccounts[accountIndex].UserName, githubAccounts[accountIndex].Password);
            return accessToken;
        }
    }
}
