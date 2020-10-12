using System;

namespace GitHubApi.Models.Commit
{
    public class Author
    {
        public string name { get; set; }
        public string email { get; set; }
        public DateTime date { get; set; }
    }
}
