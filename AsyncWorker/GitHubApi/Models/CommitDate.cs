using GitHubApi.Models.Commit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitHubApi.Models
{
    public class CommitDate
    {
        public string Date { get; set; }
        public List<Author> AuthorList { get; set; }
    }
}
