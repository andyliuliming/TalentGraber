using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GithubGraberLib.Domain
{
    public class AuthorSub
    {
        public string name { get; set; }
        public string email { get; set; }
        public string date { get; set; }
    }
    public class Author
    {
        public string login { get; set; }
        public string id { get; set; }
        public string avatar_url { get; set; }
        public string gravatar_id { get; set; }
        public string url { get; set; }
        public string html_url { get; set; }
        public string followers_url { get; set; }
        public string following_url { get; set; }
        public string gists_url { get; set; }
        public string starred_url { get; set; }
        public string subscriptions_url { get; set; }
        public string organizations_url { get; set; }
        public string repos_url { get; set; }
        public string events_url { get; set; }
        public string received_events_url { get; set; }
        public string type { get; set; }
        public string site_admin { get; set; }
    }
    public class Commit
    {
        public AuthorSub author { get; set; }
        public AuthorSub committer { get; set; }
    }
    public class CommitDetail
    {
        public string sha { get; set; }
        public Author author { get; set; }
        public Author committer { get; set; }
    }
}
