using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;
using Windows.Networking.BackgroundTransfer;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;
using Windows.Security.Credentials;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using GitMe.Common;
using GitMe.Common.OctokitWrapper;
using GitMe.Schema;
using GitMe.Storage;

// Das von dieser Datei definierte Datenmodell dient als repräsentatives Beispiel für ein Modell
// unterstützt.  Die gewählten Eigenschaftennamen stimmen mit Datenbindungen in den Standardelementvorlagen überein.
//
// Anwendungen können dieses Modell als Startpunkt verwenden und darauf aufbauen. Es kann jedoch auch komplett verworfen und
// durch ein anderes den Anforderungen entsprechendes Modell ersetzt werden Bei Verwendung dieses Modells verbessern Sie möglicherweise 
// Reaktionsfähigkeit durch Initiieren der Datenladeaufgabe im hinteren Code für App.xaml, wenn die App 
// zuerst gestartet wird.

namespace GitMe.Data
{
    /// <summary>
    /// Generisches Elementdatenmodell
    /// </summary>
    //public class DataItem : IDataItem
    //{
    //    public DataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content)
    //    {
    //        this.UniqueId = uniqueId;
    //        this.Title = title;
    //        this.Subtitle = subtitle;
    //        this.Description = description;
    //        this.ImagePath = imagePath;
    //        this.Content = content;
    //    }
         
    //    public string UniqueId { get; private set; }
    //    public string Title { get; private set; }
    //    public string Subtitle { get; private set; }
    //    public string Description { get; private set; }
    //    public string ImagePath { get; private set; }
    //    public string Content { get; private set; }

    //    public override string ToString()
    //    {
    //        return this.Title;
    //    }
    //}


    /// <summary>
    /// Generisches Gruppendatenmodell
    /// </summary>
    public class DataGroup : IDataGroup
    {
        public DataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.Subtitle = subtitle;
            this.Description = description;
            this.ImagePath = imagePath;
            this.Items = new ObservableCollection<IDataItem>();
        }

        public string UniqueId { get; private set; }
        public string Title { get; private set; }
        public string Subtitle { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public ObservableCollection<IDataItem> Items { get; set; }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Erstellt eine Auflistung von Gruppen und Elementen mit Inhalten, die aus einer statischen JSON-Datei gelesen werden.
    /// 
    /// SampleDataSource wird mit Daten initialisiert, die aus einer statischen JSON-Datei gelesen werden, die 
    /// Projekt.  Dadurch werden Beispieldaten zur Entwurfszeit und zur Laufzeit bereitgestellt.
    /// </summary>
    public sealed class DataSource : IDisposable
    {
        #region internals

        private static NotificationStorage notificationStore = new NotificationStorage();
        private static RepositoryStorage repositoryStore = new RepositoryStorage();
        private static UserStorage userStore = new UserStorage();

        private ObservableCollection<IDataGroup> _groups = new ObservableCollection<IDataGroup>();

        //private static DataState _dataState = new DataState();

        private static IAppCredentials credentials = AppCredentials.Builder;


        private static HttpClient httpClient;
        private static CancellationTokenSource cts;

        private static ObservableCollection<IDataItem> _notes = new ObservableCollection<IDataItem>();
        public static ObservableCollection<IDataItem> Notes
        {
            get { return _notes; }
            private set { _notes = value; }
        }

        private static DataSource _dataSource = new DataSource();

        #endregion

        #region properties

        public ObservableCollection<IDataGroup> Groups
        {
            get { return this._groups; }
        }

        public static void SetUserLogin(string value)
        {
            credentials.Login = value;
        }

        #endregion

        public static async Task<IDataGroup> GetGroupAsync(INotifyPage page, string uniqueId)
        {
            if(uniqueId != Constants.NotesGroupName)
            {
                throw (new NotImplementedException());
            }

            var now = DateTime.UtcNow;

            if (DataState.IsDataLoadDue())
            {
                DataState.LastDataLoadAt = now;
                try
                {
                    //page.NotifyUser(Constants.PivotLocalData, NotifyType.StatusMessage);
                    await DataSource.LoadDenormalizeNotificationsAsync();
                    DataState.LastDataLoadSuccess = true;
                    page.NotifyUser(Constants.PivotLocalDataSuccess, NotifyType.StatusMessage);
                }
                catch
                {
                    DataState.LastDataLoadSuccess = false;
                    page.NotifyUser(Constants.PivotLocalDataError, NotifyType.ErrorMessage);
                }
            }

            if (DataState.IsDataFetchDue())
            {
                DataState.LastDataFetchAt = now;
                try
                {
                    page.NotifyUser(Constants.PivotFetchData, NotifyType.StatusMessage);
                    await DataSource.FetchDenormalizedNotificationsAsync(page);
                    DataState.LastDataFetchSuccess = true;
                    page.NotifyUser(Constants.PivotFetchDataSuccess, NotifyType.StatusMessage);
                }
                catch
                {
                    DataState.LastDataFetchSuccess = false;
                    page.NotifyUser(Constants.PivotFetchDataError, NotifyType.ErrorMessage);
                }
            }  
            
            return GetDenormalizedNotificationGroup();
        }

        public static async Task LoadDenormalizeNotificationsAsync()
        {
            if (notificationStore == null)
                notificationStore = new NotificationStorage();
            if (repositoryStore == null)
                repositoryStore = new RepositoryStorage();
            if (userStore == null)
                userStore = new UserStorage();

            //await GetStorageFileAsync("Repositories");
            //await GetStorageFileAsync("Users");
            await GetStorageFileAsync("Notifications");

            if (notificationStore == null)
            {
                notificationStore = new NotificationStorage();
                return;
            }

            if (notificationStore.Count == 0)
            {
                return;
            }

            var dict = new UpdatableDictionary<int, DenormalizedNotification>();
            if (_notes.Count > 0)
            {
                foreach (var item in _notes)
                {
                    var _item = item as DenormalizedNotification;
                    dict.UpdateOrInsert(_item.Id, _item);
                }
            }

            foreach (var note in notificationStore.Values)
            {
                string imagePath = await GetImagePath(note.UserId.ToString(), Constants.AvatarSize);
                try
                {
                    var denormalizedNote = new DenormalizedNotification(note.Id,
                                                    note.HtmlUrl,
                                                    note.UserLogin,
                                                    note.RepoFullName,
                                                    note.SubjectTitle,
                                                    imagePath,
                                                    DateTimeHelper.ParsedOrDefaultDateTime(note.TimeStamp),
                                                    note.Body);
                    dict.UpdateOrInsert(denormalizedNote.Id, denormalizedNote);
                }
                catch
                {
                    //ignore
                }
            }
        
            _notes = new ObservableCollection<IDataItem>();
            var list = dict.Values.OrderByDescending(i => i.UpdatedAt).Take(Constants.NotificationStorageMaxSize);
            foreach (var value in list)
                _notes.Add(value);
        }

        public static async Task FetchDenormalizedNotificationsAsync(INotifyPage page)
        {
            if (credentials.Login != "" && credentials.Password != "")
                try
                {
                    if (page != null)
                        page.NotifyUser(Constants.FetchingGithubNotifications, NotifyType.StatusMessage);
                    await FetchDenormalizedNotificationsAsync(page, credentials.Login, credentials.Password);
                }
                catch (Exception Error)
                {
                    if (page != null)
                        page.NotifyUser(String.Format("{0}\n{1}", Constants.FetchingGithubNotificationsError, Error.Message), 
                                        NotifyType.ErrorMessage);
                    throw (Error);
                }
        }

        private static async Task FetchDenormalizedNotificationsAsync(INotifyPage page, string login, string password)
        {
            var client = new GitHubWrapper(login, password);
            var notes = client.GetNotifications();
            if (page != null)
                page.NotifyUser(Constants.FetchingGithubNotificationDetails, NotifyType.StatusMessage);
            await _dataSource.ProcessFetchedDataAsync(notes);
        }

        /// <summary>
        /// throws exception
        /// </summary>
        /// <param name="page"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        private async Task ProcessFetchedDataAsync(List<Octokit.Notification> notes)
        {
            if (notificationStore == null)
                notificationStore = new NotificationStorage();
            if (repositoryStore == null)
                repositoryStore = new RepositoryStorage();
            if (userStore == null)
                userStore = new UserStorage();

            var dict = new UpdatableDictionary<int, DenormalizedNotification>();
            if (_notes != null)
            {
                foreach (var item in _notes)
                {
                    var _item = item as DenormalizedNotification;
                    dict.UpdateOrInsert(_item.Id, _item);
                }
            }

            foreach (var note in notes)
            {
                try
                {
                    string responseBuffer = await GetDetailsResponse(note.Subject.LatestCommentUrl);

                    Response details = Serializer.DeserializeToResponse(responseBuffer);

                    int id;
                    if (!Int32.TryParse(note.Id, out id))
                        throw new ArgumentException();
                    string userimagepath = await GetImagePath(details.User.Id.ToString(), Constants.AvatarSize);

                    var newNote = new DenormalizedNotification(id,
                                                               details.HtmlUrl,
                                                               details.User.Login,
                                                               note.Repository.FullName,
                                                               note.Subject.Title,
                                                               userimagepath,
                                                               DateTimeHelper.ParsedOrDefaultDateTime(note.UpdatedAt),
                                                               details.Body);

                    dict.UpdateOrInsert(newNote.Id, newNote);

                    notificationStore.UpdateOrInsert(id, note, details);
                    repositoryStore.UpdateOrInsert(id, note, details);
                    userStore.UpdateOrInsert(id, note, details);
                }
                catch
                {
                    // ignore
#if DEBUG
                    throw;
#endif
                }
            }
            _notes = new ObservableCollection<IDataItem>();
            var list = dict.Values.OrderByDescending(i => i.UpdatedAt).Take(Constants.NotificationStorageMaxSize);
            foreach (var value in list)
                _notes.Add(value);

        }

        private static IDataGroup GetDenormalizedNotificationGroup()
        {
            DataGroup group = new DataGroup("AllNotifications",
                                "Benachrichtigungen",
                                "Subtitle",
                                Constants.DefaultImagePath,
                                "Description");
            if (_notes == null)
                return group;
            else
            {
                foreach (var note in _notes)
                {
                    try
                    {
                        group.Items.Add(note);
                    }
                    catch
                    {
#if DEBUG
                        throw;
#endif
                    }
                }
            }

            return group;
        }

        private static async Task<string> GetDetailsResponse(string latestCommentUrl)
        {
            CreateHttpClient(ref httpClient);
            cts = new CancellationTokenSource();
            var uri = new Uri(latestCommentUrl);

            HttpResponseMessage response = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseContentRead).AsTask(cts.Token);

            var responseBuffer = await response.Content.ReadAsStringAsync().AsTask(cts.Token);
            response.EnsureSuccessStatusCode();

            cts.Token.ThrowIfCancellationRequested();

            return responseBuffer;
        }

        public static async Task WriteAllStorageFilesAsync(bool condition)
        {
            if (condition)
            {
                DataState.LastDataSaveAt = DateTime.UtcNow;
                try
                {
                    await WriteToStorageFile("Notifications");
                    DataState.LastDataSaveSuccess = true;
                }
                catch
                {
                    DataState.LastDataSaveSuccess = false;
                }

                //try
                //{
                //    await WriteToStorageFile("Users");
                //    await WriteToStorageFile("Repositories");
                //}
                //catch
                //{
                //    // ignore
                //}
            }
        }

        public static async Task WriteToStorageFile(string storageType)
        { 
            string fileName = String.Format("{0}.json", storageType);
            string result;

            switch (storageType)
            {
                case "Notifications":
                    result = JsonHelper.StorageToJsonString(storageType, notificationStore);
                    break;
                case "Repositories":
                    result = JsonHelper.StorageToJsonString(storageType, repositoryStore);
                    break;
                case "Users":
                    result = JsonHelper.StorageToJsonString(storageType, userStore);
                    break;
                default:
                    throw new ArgumentException();
            }

            await FileIOHelper.WriteUtf8ToLocalFileAsync("Data", fileName, result);
#if DEBUG
            await FileIOHelper.WriteUtf8ToSDAsync("GitMe", fileName, result);
#endif
        }

        private static async Task GetStorageFileAsync(string fileType)
        {
            dynamic jsonStore = await FileIOHelper.GetStorageFromLocalFileAsync("Data", fileType);

            if (jsonStore == null || jsonStore.Count == 0)
                return;

            foreach (var key in jsonStore.Keys)
            {
                switch (fileType)
                {
                    case "Notifications":
                        notificationStore.UpdateOrInsert(key, jsonStore[key]);
                        break;
                    case "Repositories":
                        repositoryStore.UpdateOrInsert(key, jsonStore[key]);
                        break;
                    case "Users":
                        userStore.UpdateOrInsert(key, jsonStore[key]);
                        break;
                    default:
                        throw new ArgumentException();
                }
            }
        }

        //private static string FindRepository(int notificationId)
        //{
        //    foreach (var repo in repositoryStore.Values)
        //    {
        //        try
        //        {
        //            if (repo.Notifications.Contains(notificationId))
        //                return repo.RepoFullName;    
        //        }
        //        catch 
        //        {
        //            // ignore
        //        }
        //    }
        //    return Constants.UnknownRepository;
        //}

        private static async Task<string> GetImagePath(string id, string size)
        {
            string prefix = Constants.LocalPrefix;
            var imageName = String.Format("{0}.png", id);
            var imagePath = String.Format("{0}/{1}", Constants.ImageFolder, imageName); 
            
            var localFolder = ApplicationData.Current.LocalFolder;
            var imageFolder = await localFolder.CreateFolderAsync(Constants.ImageFolder, CreationCollisionOption.OpenIfExists);
            StorageFile imageFile = null;
            try
            {
                imageFile = await imageFolder.CreateFileAsync(imageName, CreationCollisionOption.FailIfExists);
            }
            catch // take existing image path
            {
                return prefix + imagePath;
            }

            var url = String.Format("https://avatars.githubusercontent.com/u/{0}?v=2&s={1}", id, size);

            // Validating the URI is required since it was received from an untrusted source (external input).
            // The URI is validated by calling Uri.TryCreate() that will return 'false' for strings that are not valid URIs.
            // Note that when enabling the text box users may provide URIs to machines on the intrAnet that require
            // the "Home or Work Networking" capability.
            Uri resourceAddress;
            if (!Uri.TryCreate(url.Trim(), UriKind.Absolute, out resourceAddress))
            {
                // this is the unlikely event that Github provides a wrong avatar link
                return Constants.DefaultImagePath;
            }

            try
            {
                CreateHttpClient(ref httpClient);
                cts = new CancellationTokenSource();
                HttpResponseMessage response = await httpClient.GetAsync(resourceAddress).AsTask(cts.Token);
                var responseBuffer = await response.Content.ReadAsBufferAsync().AsTask(cts.Token);

                await FileIO.WriteBufferAsync(imageFile,responseBuffer);

                return prefix + imagePath;
            }
            catch
            {
                // default user image
                return Constants.DefaultImagePath;
            }
        }

        #region dispose

        internal static void CreateHttpClient(ref HttpClient httpClient)
        {
            if (httpClient != null)
            {
                httpClient.Dispose();
            }

            // HttpClient functionality can be extended by plugging multiple filters together and providing
            // HttpClient with the configured filter pipeline.
            IHttpFilter filter = new HttpBaseProtocolFilter();
            filter = new PlugInFilter(filter); // Adds a custom header to every request and response message.
            httpClient = new HttpClient(filter);

            // The following line sets a "User-Agent" request header as a default header on the HttpClient instance.
            // Default headers will be sent with every request sent from this HttpClient instance.
            httpClient.DefaultRequestHeaders.UserAgent.Add(new HttpProductInfoHeaderValue("gitme"));
            
        }

        public void Dispose()
        {
            if (httpClient != null)
            {
                httpClient.Dispose();
                httpClient = null;
            }

            if (cts != null)
            {
                cts.Dispose();
                cts = null;
            }
        }

        #endregion

    }
}