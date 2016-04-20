using GithubGraberLib.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentGraber
{
    public class GithubAccountsLoader
    {
        public static List<GithubAccount> Load()
        {
            List<GithubAccount> githubAccounts = new List<GithubAccount>();
            using (var stream = new StreamReader("./accounts.txt"))
            {
                string accountLine = stream.ReadLine();
                while (accountLine != null)
                {
                    string[] accountItem = accountLine.Trim().Split(' ');
                    GithubAccount account = new GithubAccount();
                    account.UserName = accountItem[0];
                    account.Password = accountItem[1];
                    githubAccounts.Add(account);
                    accountLine = stream.ReadLine();
                }
            }
            return githubAccounts;
        }
    }
}

