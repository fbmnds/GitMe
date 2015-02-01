using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitMe.Common.OctokitWrapper
{
    public class GitHubWrapper
    {

        public GitHubWrapper(string username, string password)
        {
            _credentials = new Credentials(username, password);
            _client = new GitHubClient(new ProductHeaderValue("gitme"))
            {
                Credentials = _credentials
            };
        }

        #region User actions (Github)

        public async Task<bool> TestCredentials()
        {
            try
            {
                var user = await _client.User.Get(_credentials.Login);
                if (user != null && user.Email != "")
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Repository actions (GitHub)

        public List<Octokit.Repository> GetRepositoriesForOrganization(string organizationName)
        {
            if (string.IsNullOrWhiteSpace(organizationName))
                return new List<Octokit.Repository>();
            Task<IReadOnlyList<Octokit.Repository>> repositories = _client.Repository.GetAllForOrg(organizationName);
            try
            {
                repositories.Wait();
            }
            catch
            {
                return new List<Octokit.Repository>();
            }
            if (repositories.Result != null)
            {
                return repositories.Result.ToList();
            }
            else
            {
                return new List<Octokit.Repository>();
            }
        }

        public Repository GetRepository(string owner, string repoName)
        {
            Task<Repository> repo = _client.Repository.Get(owner, repoName);
            try
            {
                repo.Wait();
            }
            catch
            {
                return null;
            }
            return repo.Result;
        }

        public List<Notification> GetNotifications()
        {
            Task<IReadOnlyList<Octokit.Notification>> notifications = _client.Notification.GetAllForCurrent();
            notifications.Wait();
            
            if (notifications.Result != null)
            {
                return notifications.Result.ToList();
            }
            return new List<Notification>();
        }

        public List<Notification> TryGetNotifications()
        {
            bool err = false;
            Task<IReadOnlyList<Octokit.Notification>> notifications = _client.Notification.GetAllForCurrent();
            try
            {
                notifications.Wait();
            }
            catch
            {
                err = true;
            }
            if ((!err) && (notifications.Result != null))
            {
                return notifications.Result.ToList();
            }
            return new List<Notification>();
        }

        public List<Issue> GetIssues(Octokit.Repository repo)
        {
            bool err = false;
            Task<IReadOnlyList<Octokit.Issue>> issues = _client.Issue.GetForRepository(repo.Owner.Login, repo.Name);
            try
            {
                issues.Wait();
            }
            catch
            {
                err = true;
            }
            if ((!err) && (issues.Result != null))
            {
                return issues.Result.ToList();
            }
            return new List<Issue>();
        }

        //private GitHubClient Client 
        //{
        //    get
        //    {
        //        if (_client == null)
        //        {
        //            _client = new GitHubClient(new ProductHeaderValue("GithubIssueNotifier"))
        //            {
        //                Credentials = _credentials
        //            };
        //        }
        //        return _client;
        //    }
        //}
        //private GitHubClient _client;

        //public static void DeleteRepo(Octokit.Repository repository)
        //{
        //    GitHubWrapper.DeleteRepo(repository.Owner.Login, repository.Name);
        //}

        //public static void DeleteRepo(string owner, string name)
        //{
        //    GitHubClient api = new GitHubClient(new ProductHeaderValue("OctokitTests")) { Credentials = Credentials };
        //    try
        //    {
        //        api.Repository.Delete(owner, name).Wait(TimeSpan.FromSeconds(15));
        //    }
        //    catch
        //    {
        //    }
        //}

        #endregion

        #region Utility methods

        public static string GetNameWithTimestamp(string name)
        {
            return string.Format("{0}-{1}",
                name,
                DateTime.UtcNow.ToString("yyyyMMddhhmmssfff"));
        }

        #endregion

        #region Private parts

        //private const string TokenName = "OCTOKIT_OAUTHTOKEN";
        private Credentials _credentials = null;
        private GitHubClient _client;

        #endregion
    }
}
