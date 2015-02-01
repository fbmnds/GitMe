using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage.Streams;

using GitMe.Schema;
using GitMe.Storage;

namespace GitMe.Common
{
    public static class JsonHelper
    {
        public static string StorageToJsonString(string storageType, dynamic store)
        {
            var result = new StringBuilder();

            switch (storageType)
            {
                case "Notifications":
                    result.AppendLine("{ \"Notifications\" : [");
                    foreach (var note in store.Values)
                    {
                        result.Append(Serializer.SerializeNotification(note));
                        result.Append(",");
                    }
                    var result1 = result.ToString().TrimEnd(',');
                    result.Clear().Append(result1).AppendLine("\n] }");
                    break;
                case "Repositories":
                    result.AppendLine("{ \"Repositories\" : [");
                    foreach (var repo in store.Values)
                    {
                        result.Append(Serializer.SerializeRepository(repo));
                        result.Append(",");
                    }
                    var result2 = result.ToString().TrimEnd(',');
                    result.Clear().Append(result2).AppendLine("\n] }");
                    break;
                case "Users":
                    result.AppendLine("{ \"Users\" : [");
                    foreach (var user in store.Values)
                    {
                        result.Append(Serializer.SerializeUser(user));
                        result.Append(",");
                    }
                    var result3 = result.ToString().TrimEnd(',');
                    result.Clear().Append(result3).AppendLine("] }");
                    break;
                default:
                    throw new System.ArgumentException(storageType);
            }
            return result.ToString();
        }

        public static dynamic DeserializeToStorage(string storageType, string storageText)
        {
            JsonObject storageJsonObject;
            JsonArray storageJsonArray;

            dynamic jsonStore;
            switch (storageType)
            {
                case "Notifications":
                    jsonStore = new NotificationStorage();
                    break;
                case "Repositories":
                    jsonStore = new RepositoryStorage();
                    break;
                case "Users":
                    jsonStore = new UserStorage();
                    break;
                default:
                    throw new System.ArgumentException(storageType);
            }

            try
            {
                storageJsonObject = JsonObject.Parse(storageText);
                storageJsonArray = storageJsonObject[storageType].GetArray();
            }
            catch
            {
                return jsonStore;
            }

            foreach (JsonValue storageValue in storageJsonArray)
            {
                try
                {
                    dynamic storageItem;
                    
                    switch (storageType)
                    {
                        case "Notifications":
                            storageItem = Serializer.DeserializeToNotification(storageValue.Stringify());
                            storageItem.SubjectTitle = DecodeBase64(storageItem.SubjectTitle);
                            storageItem.Body = DecodeBase64(storageItem.Body);
                            jsonStore.UpdateOrInsert(storageItem.Id, storageItem);
                            break;
                        case "Repositories":
                            storageItem = Serializer.DeserializeToRepository(storageValue.Stringify());
                            jsonStore.UpdateOrInsert(storageItem.Id, storageItem);
                            break;
                        case "Users":
                            storageItem = Serializer.DeserializeToUser(storageValue.Stringify());
                            jsonStore.UpdateOrInsert(storageItem.Id, storageItem);
                            break;
                        default:
                            throw new System.ArgumentException(storageType);
                    }
                }
                catch
                {
                    throw;
                }
            }
            return jsonStore;
        }

        public static string DeserializeToOctokitNotification(Octokit.Notification note)
        {
            var contents = new StringBuilder();
            contents.Append("{");
            contents.Append(string.Format("\"{0}\": {1},", "id", note.Id));
            contents.Append(string.Format("\"{0}\": \"{1}\",", "lastreadat", note.LastReadAt));
            contents.Append(string.Format("\"{0}\": \"{1}\",", "reason", note.Reason));
            contents.Append("\"repository\": \n{");
            contents.Append(string.Format("\"{0}\": \"{1}\",", "cloneurl", note.Repository.CloneUrl));
            contents.Append(string.Format("\"{0}\": \"{1}\",", "createdat", note.Repository.CreatedAt));
            contents.Append(string.Format("\"{0}\": \"{1}\",", "defaultbranch", note.Repository.DefaultBranch));
            contents.Append(string.Format("\"{0}\": \"{1}\",", "description", note.Repository.Description));
            contents.Append(string.Format("\"{0}\": \"{1}\",", "fork", note.Repository.Fork));
            contents.Append(string.Format("\"{0}\": {1},", "forkscount", note.Repository.ForksCount));
            contents.Append(string.Format("\"{0}\": \"{1}\",", "fullname", note.Repository.FullName));
            contents.Append(string.Format("\"{0}\": \"{1}\",", "giturl", note.Repository.GitUrl));
            contents.Append(string.Format("\"{0}\": {1},", "hasdownloads", note.Repository.HasDownloads));
            contents.Append(string.Format("\"{0}\": {1},", "hasissues", note.Repository.HasIssues));
            contents.Append(string.Format("\"{0}\": {1},", "haswiki", note.Repository.HasWiki));
            contents.Append(string.Format("\"{0}\": \"{1}\",", "homepage", note.Repository.Homepage));
            contents.Append(string.Format("\"{0}\": \"{1}\",", "htmlurl", note.Repository.HtmlUrl));
            contents.Append(string.Format("\"{0}\": {1},", "id", note.Repository.Id));
            contents.Append(string.Format("\"{0}\": \"{1}\",", "language", note.Repository.Language));
            contents.Append(string.Format("\"{0}\": \"{1}\",", "mirrorurl", note.Repository.MirrorUrl));
            contents.Append(string.Format("\"{0}\": \"{1}\",", "name", note.Repository.Name));
            contents.Append(string.Format("\"{0}\": {1},", "openissuescount", note.Repository.OpenIssuesCount));
            contents.Append(string.Format("\"{0}\": \"{1}\",", "organization", note.Repository.Organization));
            contents.Append(string.Format("\"{0}\": \"{1}\",", "owner", note.Repository.Owner));
            contents.Append(string.Format("\"{0}\": \"{1}\",", "parent", note.Repository.Parent));
            contents.Append(string.Format("\"{0}\": \"{1}\",", "private", note.Repository.Private));
            contents.Append(string.Format("\"{0}\": \"{1}\",", "pushedat", note.Repository.PushedAt));
            contents.Append(string.Format("\"{0}\": \"{1}\",", "source", note.Repository.Source));
            contents.Append(string.Format("\"{0}\": \"{1}\",", "sshurl", note.Repository.SshUrl));
            contents.Append(string.Format("\"{0}\": \"{1}\",", "svnurl", note.Repository.SvnUrl));
            contents.Append(string.Format("\"{0}\": \"{1}\",", "updatedat", note.Repository.UpdatedAt));
            contents.Append(string.Format("\"{0}\": \"{1}\",", "url", note.Repository.Url));
            contents.Append(string.Format("\"{0}\": {1}", "watcherscount", note.Repository.WatchersCount));
            contents.Append("},");
            contents.Append("\"subject\": \n{");
            contents.Append(string.Format("\"{0}\": \"{1}\",", "latestcommenturl", note.Subject.LatestCommentUrl));
            contents.Append(string.Format("\"{0}\": \"{1}\",", "title", note.Subject.Title));
            contents.Append(string.Format("\"{0}\": \"{1}\",", "type", note.Subject.Type));
            contents.Append(string.Format("\"{0}\": \"{1}\",", "url", note.Subject.Url));
            contents.Append("},");
            contents.Append(string.Format("\"{0}\": \"{1}\",", "unread", note.Unread));
            contents.Append(string.Format("\"{0}\": \"{1}\",", "updatedat", note.UpdatedAt));
            contents.Append(string.Format("\"{0}\": \"{1}\"", "url", note.Url));
            contents.Append("}");

            return contents.ToString();
        }

        public static string DecodeBase64(string text)
        {
            if (text == null)
                return null;
            if (text.Length == 0)
                return "";
            Byte[] b = Convert.FromBase64String(text);
            return Encoding.UTF8.GetString(b, 0, b.Length);
        }

        public static string EncodeBase64(string text)
        {
            return Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(text));
        }

    }
}
