using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GitMe.Schema;
using GitMe.Common;
using GitMe.Common.OctokitWrapper;

namespace GitMe.Storage
{
    #region interfaces

    public interface IUpdatableDictionary<Key,Value>
    {
        void UpdateOrInsert(Key id, Value item);
    }

    public interface INotificationStorage : IUpdatableDictionary<int,Notification>
    {
        void UpdateOrInsert(int id, Octokit.Notification note, Response details);
    }

    public interface IRepositoryStorage : IUpdatableDictionary<int,Repository>
    {
        void UpdateOrInsert(int id, Octokit.Notification note, Response details);
    }

    public interface IUserStorage : IUpdatableDictionary<int,User>
    {
        void UpdateOrInsert(int id, Octokit.Notification note, Response details);
    }

    #endregion

    #region implementations

    public class UpdatableDictionary<Key, Value> : Dictionary<Key, Value>, IUpdatableDictionary<Key, Value>
    {
        public void UpdateOrInsert(Key id, Value item)
        {
            if (!this.ContainsKey(id))
                this.Add(id, item);
            else
                this[id] = item;
        }
    }

    public class NotificationStorage : UpdatableDictionary<int,Notification>, INotificationStorage
    {
        public void UpdateOrInsert(int id, Octokit.Notification note, Response details)
        {

            if (this.ContainsKey(id))
            {
                this[id].HtmlUrl = details.HtmlUrl;
                this[id].RepoFullName = note.Repository.FullName;
                this[id].UserLogin = details.User.Login;
                this[id].UserId = details.User.Id;
                this[id].SubjectTitle = note.Subject.Title;
                this[id].Body = details.Body;
                //this[id].TimeStamp = details.UpdatedAt;
            }
            else
            {
                var notification = new Notification()
                {
                    Id = id,
                    HtmlUrl = details.HtmlUrl,
                    RepoFullName = note.Repository.FullName,
                    UserLogin = details.User.Login,
                    UserId = details.User.Id,
                    SubjectTitle = note.Subject.Title,
                    Body = details.Body,
                    TimeStamp = details.UpdatedAt
                };
                this.Add(id, notification);
            }
        }
    }

    public class RepositoryStorage : Dictionary<int,Repository>, IRepositoryStorage
    {
        public void UpdateOrInsert(int id, Repository item)
        {
            if (this.ContainsKey(id))
            {
                if (this[id].Notifications == null)
                    this[id].Notifications = new int[] { };
                if (!this[id].Notifications.Contains(id))
                    this[id].Notifications.Concat(new int[] { id });
            }
            else
                this.Add(id, item);
        }

        public void UpdateOrInsert(int id, Octokit.Notification note, Response details)
        {
            if (this.ContainsKey(note.Repository.Id))
            {
                if (this[note.Repository.Id].Notifications == null)
                    this[note.Repository.Id].Notifications = new int[] { };
                if (!this[note.Repository.Id].Notifications.Contains(id))
                    this[note.Repository.Id].Notifications.Concat(new int[] { id });
                this[note.Repository.Id].RepoFullName = note.Repository.FullName;
                //this[note.Repository.Id].LastReadAt = details.UpdatedAt;
            }
            else
            {
                var repository = new Repository()
                {
                    Id = note.Repository.Id,
                    RepoFullName = note.Repository.FullName,
                    LastReadAt = DateTimeHelper.ParsedOrDefaultDateTime(details.UpdatedAt),
                    Notifications = new int[] { id }
                };
                this.Add(note.Repository.Id, repository);
            }
        }
    }

    public class UserStorage : Dictionary<int,User>, IUserStorage
    {
        public void UpdateOrInsert(int id, User item)
        {
            if (this.ContainsKey(id))
            {
                if (this[id].Notifications == null)
                    this[id].Notifications = new int[] { };
                if (!this[id].Notifications.Contains(id))
                    this[id].Notifications.Concat(new int[] { id });
                this[id].Login = item.Login;
                this[id].AvatarUrl = item.AvatarUrl;
                //this[id].TimeStamp = details.UpdatedAt;
            }
            else
                this.Add(id, item);
            
        }

        public void UpdateOrInsert(int id, Octokit.Notification note, Response details)
        {
            if (this.ContainsKey(details.User.Id))
            {
                if (this[details.User.Id].Notifications == null)
                    this[details.User.Id].Notifications = new int[] { };
                if (!this[details.User.Id].Notifications.Contains(id))
                    this[details.User.Id].Notifications.Concat(new int[] { id });
                this[details.User.Id].Login = details.User.Login;
                this[details.User.Id].AvatarUrl = details.User.AvatarUrl;
                //this[details.User.Id].TimeStamp = details.UpdatedAt;
            }
            else
            {
                var user = new User()
                {
                    Id = details.User.Id,
                    Login = details.User.Login,
                    AvatarUrl = details.User.AvatarUrl,
                    Notifications = new int[] { id },
                    TimeStamp = DateTimeHelper.ParsedOrDefaultDateTime(details.UpdatedAt)
                };
                this.Add(details.User.Id, user);
            }
        }
    }

    #endregion

}
