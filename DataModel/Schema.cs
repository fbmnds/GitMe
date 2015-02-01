using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using GitMe.Common;

namespace GitMe.Schema
{
    public interface IDataItem
    {
        int Id { get; }
        string UniqueId { get; }
        Uri HtmlUrl { get; }
        string HtmlUrlString { get; }
        string UserLogin { get; }
        string RepoFullName { get; }
        string SubjectTitle { get; }
        string UserImagePath { get; }
        DateTime UpdatedAt { get; }
        string Body { get; }
        string BodySnippet(int count);
        string Since { get; }



    }

    public interface IDataGroup
    {
        string UniqueId { get; }
        string Title { get; }
        string Subtitle { get; }
        string Description { get; }
        string ImagePath { get; }
        ObservableCollection<IDataItem> Items { get; set; }
    }




    public static class Serializer
    {
        public static string SerializeNotification(Notification note)
        {
            var result = new StringBuilder();
            result.Append("{ ");
            result.Append(String.Format("\"id\" : {0}, ", note.Id));
            result.Append(String.Format("\"htmlurl\" : \"{0}\", ", note.HtmlUrl));
            result.Append(String.Format("\"repofullname\" : \"{0}\", ", note.RepoFullName));
            result.Append(String.Format("\"userlogin\" : \"{0}\", ", note.UserLogin));
            result.Append(String.Format("\"userid\" : \"{0}\", ", note.UserId));
            result.Append(String.Format("\"subjecttitle\" : \"{0}\", ", JsonHelper.EncodeBase64(note.SubjectTitle)));
            result.Append(String.Format("\"body\" : \"{0}\", ", JsonHelper.EncodeBase64(note.Body)));
            result.Append(String.Format("\"timestamp\" : \"{0}\"", note.TimeStamp));
            result.Append(" }");
            return result.ToString();
        }

        public static Notification DeserializeToNotification(string jsonString)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString)))
            {
                DataContractJsonSerializer serializer =
                        new DataContractJsonSerializer(typeof(Notification));

                return (Notification)serializer.ReadObject(ms);
            }
        }

        public static string SerializeRepository(Repository repo)
        {
            var result = new StringBuilder();
            result.Append("{ ");
            result.Append(String.Format("\"id\" : {0}, ", repo.Id));
            result.Append(String.Format("\"repofullname\" : \"{0}\", ", repo.RepoFullName));
            result.Append(String.Format("\"lastreadat\" : \"{0}\", ", (repo.LastReadAt.ToUniversalTime()).ToString(Constants.DateTimeFormat)));
            result.Append("\"notifications\" : [");
            foreach (int i in repo.Notifications)
            {
                result.Append(String.Format("{0},", i));
            }
            var result2 = result.ToString().TrimEnd(',');
            result.Clear().Append(result2);
            result.Append("] }");
            return result.ToString();
        }

        public static Repository DeserializeToRepository(string jsonString)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString)))
            {
                DataContractJsonSerializer serializer =
                        new DataContractJsonSerializer(typeof(Repository));

                return (Repository)serializer.ReadObject(ms);
            }
        }

        public static string SerializeUser(User user)
        {
            var result = new StringBuilder();
            result.Append("{ ");
            result.Append(String.Format("\"id\" : {0}, ", user.Id));
            result.Append(String.Format("\"login\" : \"{0}\", ", user.Login));
            result.Append(String.Format("\"avatar_url\" : \"{0}\", ", user.AvatarUrl));
            result.Append(String.Format("\"notifications\" : ["));
            foreach (int i in user.Notifications)
            {
                result.Append(String.Format("{0},", i));
            }
            var result2 = result.ToString().TrimEnd(',');
            result.Clear().Append(result2);
            result.Append("], \"timestamp\" : ");
            result.Append(String.Format("\"{0}\"", (user.TimeStamp.ToUniversalTime()).ToString(Constants.DateTimeFormat)));
            result.Append(" }");
            return result.ToString();
        }

        public static User DeserializeToUser(string jsonString)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString)))
            {
                DataContractJsonSerializer serializer =
                        new DataContractJsonSerializer(typeof(User));

                return (User)serializer.ReadObject(ms);
            }
        }

        public static UserDetails DeserializeToUserDetails(string jsonString)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString)))
            {
                DataContractJsonSerializer serializer =
                        new DataContractJsonSerializer(typeof(UserDetails));

                return (UserDetails)serializer.ReadObject(ms);
            }
        }

        public static Response DeserializeToResponse(string jsonString)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString)))
            {
                DataContractJsonSerializer serializer =
                        new DataContractJsonSerializer(typeof(Response));

                return (Response)serializer.ReadObject(ms);
            }
        }
    }


    [DataContract]
    public class DenormalizedNotification : IDataItem
    {
        public DenormalizedNotification()
        {
            new DenormalizedNotification(Constants.DefaultDenormalizedNotificationId,
                                         Constants.DefaultDenormalizedNotificationIUri,
                                         "",
                                         Constants.DefaultDenormalizedNotificationRepoFullName,
                                         "",
                                         Constants.DefaultDenormalizedNotificationImagePath,
                                         DateTime.UtcNow,
                                         Constants.DefaultDenormalizedNotificationBody);
        }

        public DenormalizedNotification(int id, Uri htmlurl, String userlogin, String repofullname, String subjecttitle, String userimagepath, DateTime updatedat, String body)
        {
            this.Id = id;
            this.HtmlUrl = htmlurl;
            this.UserLogin = userlogin;
            this.RepoFullName = repofullname;
            this.SubjectTitle = subjecttitle;
            this.UserImagePath = userimagepath;
            this.UpdatedAt = updatedat;
            this.Body = body;
        }

        [DataMember(Name = "id")]
        public int Id { get; private set; }
        public string UniqueId { get { return Id.ToString(); } }
        [DataMember(Name = "htmlurl")]
        public Uri HtmlUrl { get; private set; }
        public string HtmlUrlString { get { return String.Format(" → {0}", HtmlUrl.ToString()); } }
        [DataMember(Name = "userlogin")]
        public string UserLogin { get; private set; }
        public string AtUserLogin { get { return String.Format("@{0}", UserLogin); } }
        [DataMember(Name = "repofullname")]
        public string RepoFullName { get; private set; }
        public string Title { get { return RepoFullName; } }
        [DataMember(Name = "subjecttitle")]
        public string SubjectTitle { get; private set; }
        public string Description { get { return SubjectTitle; } }
        public string Subtitle { get { return SubjectTitle; } }
        [DataMember(Name = "userimagepath")]
        public string UserImagePath { get; private set; }
        public string ImagePath { get { return UserImagePath; } }
        [DataMember(Name = "updatedat")]
        public DateTime UpdatedAt { get; private set; }
        [DataMember(Name = "body")]
        public string Body { get; private set; }
        public string Content { get { return Body; } }

        public string BodySnippet(int count)
        {
            if (this.Body.Length > count)
                return this.Body.Substring(0, count);
            else
                return this.Body;
        }

        public override string ToString()
        {
            return this.BodySnippet(20);
        }

        public string Since 
        {
            get { return DateTimeHelper.GetFormatedTimeSpan(DateTime.UtcNow, UpdatedAt); }
        }

    }


    [DataContract]
    public class Notification
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }
        [DataMember(Name = "htmlurl")]
        public Uri HtmlUrl { get; set; }
        [DataMember(Name = "repofullname")]
        public string RepoFullName { get; set; }
        [DataMember(Name = "userlogin")]
        public string UserLogin { get; set; }
        [DataMember(Name = "userid")]
        public int UserId { get; set; }
        [DataMember(Name = "subjecttitle")]
        public string SubjectTitle { get; set; }
        [DataMember(Name = "body")]
        public string Body { get; set; }
        [DataMember(Name = "timestamp")]
        public string TimeStamp { get; set; }

        public string since { get { return DateTimeHelper.GetFormatedTimeSpan(DateTime.UtcNow, 
                                                                              DateTimeHelper.ParsedOrDefaultDateTime(TimeStamp)); } }
    }


    [DataContract]
    public class Repository
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }
        [DataMember(Name = "repofullname")]
        public string RepoFullName { get; set; }
        [DataMember(Name = "lastreadat")]
        public DateTime LastReadAt { get; set; }
        [DataMember(Name = "notifications")]
        public int[] Notifications { get; set; }

        public string since { get { return DateTimeHelper.GetFormatedTimeSpan(DateTime.UtcNow,LastReadAt); } }
    }


    [DataContract]
    public class User
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }
        [DataMember(Name = "login")]
        public string Login { get; set; }
        [DataMember(Name = "avatar_url")]
        public Uri AvatarUrl { get; set; }
        [DataMember(Name = "notifications")]
        public int[] Notifications { get; set; }
        [DataMember(Name = "timestamp")]
        public DateTime TimeStamp { get; set; }

        public string Name { get { return "@" + this.Login; } }
        public string ImagePath { get { return "Images/" + this.Id + ".png"; } }
    }

    [DataContract]
    public class UserDetails
    {
        [DataMember(Name = "login")]
        public string Login { get; set; }
        [DataMember(Name = "id")]
        public int Id { get; set; }
        [DataMember(Name = "avatar_url")]
        public Uri AvatarUrl { get; set; }
        [DataMember(Name = "gravatar_id")]
        public string GravatarId { get; set; }
        [DataMember(Name = "url")]
        public Uri Url { get; set; }
        [DataMember(Name = "html_url")]
        public Uri HtmlUrl { get; set; }
        [DataMember(Name = "followers_url")]
        public Uri FollowersUrl { get; set; }
        [DataMember(Name = "following_url")]
        public Uri FollowingUrl { get; set; }
        [DataMember(Name = "gists_url")]
        public Uri GistsUrl { get; set; }
        [DataMember(Name = "starred_url")]
        public Uri StarredUrl { get; set; }
        [DataMember(Name = "subscriptions_url")]
        public Uri SubscriptionsUrl { get; set; }
        [DataMember(Name = "organizations_url")]
        public Uri OrganizationsUrl { get; set; }
        [DataMember(Name = "repos_url")]
        public Uri ReposUrl { get; set; }
        [DataMember(Name = "events_url")]
        public Uri EventsUrl { get; set; }
        [DataMember(Name = "received_events_url")]
        public Uri ReceivedEventsUrl { get; set; }
        [DataMember(Name = "type")]
        public string Type { get; set; }
        [DataMember(Name = "site_admin")]
        public bool SiteAdmin { get; set; }
    }

    [DataContract]
    public class Response
    {
        [DataMember(Name = "url")]
        public Uri Url { get; set; }
        [DataMember(Name = "labels_url")]
        public Uri LabelsUrl { get; set; }
        [DataMember(Name = "comments_url")]
        public Uri CommentsUrl { get; set; }
        [DataMember(Name = "events_url")]
        public Uri EventsUrl { get; set; }
        [DataMember(Name = "html_url")]
        public Uri HtmlUrl { get; set; }
        [DataMember(Name = "id")]
        public int Id { get; set; }
        [DataMember(Name = "number")]
        public int Number { get; set; }
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "user")]
        public UserDetails User { get; set; }
        [DataMember(Name = "labels")]
        public string[] Labels { get; set; }
        [DataMember(Name = "state")]
        public string State { get; set; }
        [DataMember(Name = "locked")]
        public bool Locked { get; set; }
        [DataMember(Name = "assignee")]
        public UserDetails Assignee { get; set; }      //TODO: verify type
        [DataMember(Name = "milestone")]
        public string Milestone { get; set; }  // TODO: verify type
        [DataMember(Name = "comments")]
        public int Comments { get; set; }
        [DataMember(Name = "created_at")]
        public DateTime CreatedAt { get; set; }
        [DataMember(Name = "updated_at")]
        public string UpdatedAt { get; set; }
        [DataMember(Name = "closed_at")]
        public DateTime ClosedAt { get; set; }
        [DataMember(Name = "body")]
        public string Body { get; set; }
        [DataMember(Name = "closed_by")]
        public UserDetails ClosedBy { get; set; }      // TODO: verify type
    }
}
