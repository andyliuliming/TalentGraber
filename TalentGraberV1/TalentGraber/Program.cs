using GithubGraberLib;
using GithubGraberLib.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentGraber
{
    class Program
    {
        static void Main(string[] args)
        {
            List<GithubAccount> githubAccounts = GithubAccountsLoader.Load();

            string githubAddress = args[0];
            GithubExtractor githubExtractor = new GithubExtractor();

            ExtractRequest extractRequest = new ExtractRequest();
            extractRequest.URL = githubAddress;
            extractRequest.StartPage = 0;
            extractRequest.PerPage = 100;

            int accountIndex = 0;
            HashSet<string> userLogins = new HashSet<string>();
            for (; accountIndex < githubAccounts.Count;)
            {
                extractRequest.Account = githubAccounts[accountIndex];

                string accessToken = AuthorizeUtil.GetToken(extractRequest.Account.UserName, extractRequest.Account.Password);
                extractRequest.AccessToken = accessToken;
                extractRequest.Left = 5000;
                HashSet<string> user_logins = githubExtractor.ExtractUserLogin(extractRequest);
                if (user_logins.Count == 0)
                {
                    accountIndex++;
                    break;
                }
                foreach (string ul in user_logins)
                {
                    userLogins.Add(ul);
                }
                if (extractRequest.Left > 0)
                {
                    accountIndex++;
                    break;
                }
            }

            List<string> userLoginList = new List<string>();
            foreach (string ul in userLogins)
            {
                userLoginList.Add(ul);
            }


            string exportFile = githubAddress + ".csv";

            exportFile = exportFile.Replace("/", "_");
            exportFile = exportFile.Replace(":", "_");

            if (File.Exists(exportFile))
            {
                File.Move(exportFile, exportFile + "." + Guid.NewGuid());
            }

            extractRequest.StartIndex = 0;

            using (StreamWriter streamWriter = new StreamWriter(exportFile, true, Encoding.Default))
            {
                //write the header line
                StringBuilder sb = new StringBuilder();
                sb.Append("Name").Append(","); //没一个字段后面都加逗号，表示是一列，因为这是第一行    因此也是列标题
                sb.Append("Followers").Append(",");
                sb.Append("Email").Append(",");
                sb.Append("Company").Append(",");
                sb.Append("Location").Append(",");
                streamWriter.WriteLine(sb.ToString());
            }
            // use the List to get the user info instead.
            for (; accountIndex < githubAccounts.Count; accountIndex++)
            {
                extractRequest.Account = githubAccounts[accountIndex];

                string accessToken = AuthorizeUtil.GetToken(extractRequest.Account.UserName, extractRequest.Account.Password);
                extractRequest.AccessToken = accessToken;
                extractRequest.Left = 5000;
                List<User> users = githubExtractor.Extract(extractRequest, userLoginList);

                // calculate the initial page of next.

                using (StreamWriter streamWriter = new StreamWriter(exportFile, true, Encoding.Default))
                {
                    foreach (User user in users)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(user.name).Append(",");
                        sb.Append(user.followers).Append(",");
                        sb.Append(user.email).Append(",");
                        sb.Append(user.company).Append(",");
                        if (user.location != null)
                        {
                            sb.Append(user.location.Replace(',', '/')).Append(",");
                        }
                        else
                        {
                            sb.Append(user.location).Append(",");
                        }
                        streamWriter.WriteLine(sb.ToString());
                    }
                }
                if (extractRequest.Left > 0)
                {
                    break;
                }
            }
        }
    }
}
