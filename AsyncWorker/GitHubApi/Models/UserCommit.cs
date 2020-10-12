using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitHubApi.Models
{
    public class UserCommit
    {
        public string UserName { get; set; }
        public string Date { get; set; }
        public int Amount { get; set; }
        public int TotalCommits { get; set; }
    }
}
