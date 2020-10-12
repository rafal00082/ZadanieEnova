using System.Collections.Generic;
using System.Linq;
using GitHubApi.Api;
using GitHubApi.Models;
using GitHubApi.Models.Commit;
using Soneta.Business;
using Soneta.Business.App;
using Soneta.CRM;
using Soneta.Example.AsyncWorker;

[assembly: Worker(typeof(TablesExtender))]

namespace Soneta.Example.AsyncWorker
{
    [AsyncWorker(IsConcurrent = true)]
    public sealed class TablesExtender : IAsyncWorker, IAsyncIsReady
    {
        private Kontrahent _kontrahent;
        private TableRecords[] _value;

        [Context]
        public Kontrahent Kontrahent
        {
            get { return _kontrahent; }
            set
            {
                if (_kontrahent != value) _value = null;
                _kontrahent = value;
            }
        }

        public TableRecords[] Value
        {
            get
            {
                if (_value == null) throw new InProgressException();
                return _value;
            }
        }

        public bool IsActionReady(IAsyncContext acx) => IsNotLoaded();

        public void Action(IAsyncContext acx) =>
            _value = LoadTablesRecordCounts(Kontrahent.Session.Login, acx)
                .OrderBy(_ => _.Count)
                .Reverse()
                .Take(5)
                .ToArray();

        public bool IsLoaded() => _value != null;

        public bool IsNotLoaded() => !IsLoaded();

        public static List<UserCommit> GetCommitsByUser(List<CommitDate> dateCommits, string user)
        {
            return dateCommits.Select(x => new UserCommit
            {
                UserName = user,
                Date = x.Date,
                Amount = x.AuthorList.Where(y => y.name == user).Count(),
                TotalCommits = x.AuthorList.Count
            }).OrderBy(x => x.Date).ToList();
        }

        private static IEnumerable<TableRecords> LoadTablesRecordCounts(Login login, IAsyncContext acx)
        {
            GitHubClient gitHubClient = new GitHubClient("https://api.github.com", "rafal00082", "enova");
            var branches =  gitHubClient.GetBranches().GetAwaiter().GetResult();
            
            var commitsList = new List<CommitInfo>();
            foreach (var branch in branches)
            {
                commitsList.AddRange(gitHubClient.GetCommits(branch.Commit.sha).GetAwaiter().GetResult());
            }
            var commitsDate = commitsList.Select(y => y.commit.author).GroupBy(
                x => x.date.ToShortDateString(),
                (key, g) => new CommitDate
                {
                    Date = key,
                    AuthorList = g.ToList()
                }).ToList();

            var dateStat = GetCommitsByUser(commitsDate, "rafalk82");

            
            var queue = new Queue<TableRecords>();
            /*
            using (var session = login.CreateSession(true, false, $"{nameof(TablesExtender)}"))
            {
                foreach (Table table in session.Tables.GetAllTables())
                {
                    if (acx.IsCancellationRequested) break;

                    //to work a bit slower ;)
                    Thread.Sleep(5);

                    var query = new Query.Table(table.TableName);
                    query.Add(new Query.Count("ID") {PropertyName = "Count"});

                    var count = session
                        .Execute<Counter>(query)
                        .First()
                        .Count;

                    queue.Enqueue(new TableRecords {TableName = table.TableName, Count = count});
                }
            }
            */
            return queue;
        }

        internal sealed class Counter
        {
            public int Count { get; set; }
        }
    }
}