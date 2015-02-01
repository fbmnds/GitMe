using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.ApplicationModel.Resources;

namespace GitMe.Common
{
    public static class Constants
    {
        public static ResourceLoader Loader = new ResourceLoader();

        public static string NotesGroupName = "AllNotifications";

        public static string PivotInitialization = Loader.GetString("PivotInitialization");
        public static string PivotLocalData = Loader.GetString("PivotLocalData");
        public static string PivotLocalDataSuccess = Loader.GetString("PivotLocalDataSuccess");
        public static string PivotLocalDataError = Loader.GetString("PivotLocalDataError");
        public static string PivotFetchData = Loader.GetString("PivotFetchData");
        public static string PivotFetchDataSuccess = Loader.GetString("PivotFetchDataSuccess");
        public static string PivotFetchDataError = Loader.GetString("PivotFetchDataError");
        public static string PivotNoAccount = Loader.GetString("PivotNoAccount");
        public static string PivotNoData = Loader.GetString("PivotNoData");

        public static string LocalPrefix = "ms-appdata:///local/";
        public static string ImageFolder = "Images";
        public static string DefaultImagePath = "ms-appx:///Assets/github.png";
        public static string AvatarSize = "40";

        public static string DateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ";

        public static string UnknownUser = Loader.GetString("UnknownUser");
        public static string UnknownRepository = Loader.GetString("UnknownRepository");

        public static int NotificationStorageMaxSize = 50;

        public static int DefaultDenormalizedNotificationId = -1;
        public static Uri DefaultDenormalizedNotificationIUri = new Uri("http://github.com");
        public static string DefaultDenormalizedNotificationRepoFullName = Loader.GetString("DefaultDenormalizedNotificationRepoFullName");
        public static string DefaultDenormalizedNotificationImagePath = DefaultImagePath;
        public static string DefaultDenormalizedNotificationBody = Loader.GetString("DefaultDenormalizedNotificationBody");

        public static string FetchingGithubNotifications = Loader.GetString("FetchingGithubNotifications");
        public static string FetchingGithubNotificationDetails = Loader.GetString("FetchingGithubNotificationDetails");
        public static string FetchingGithubNotificationsError = Loader.GetString("FetchingGithubNotificationsError");

        public static string AccountTest = Loader.GetString("AccountTest");
        public static string AccountTestError = Loader.GetString("AccountTestError");
        public static string AccountTestError1 = Loader.GetString("AccountTestError1");
        public static string AccountTestError2 = Loader.GetString("AccountTestError2");
        public static string AccountTestError3 = Loader.GetString("AccountTestError3");

        public static string AccountDeleteMessage = Loader.GetString("AccountDeleteMessage");
        public static string AccountDeleteMessageOK = Loader.GetString("AccountDeleteMessageOK");
        public static string AccountDeleteMessageCancel = Loader.GetString("AccountDeleteMessageCancel");
        public static string AccountDeleteSuccess = Loader.GetString("AccountDeleteSuccess");
        public static string AccountDeleteSuccess2 = Loader.GetString("AccountDeleteSuccess2");

        public static string AccountDataDeleteMessage = Loader.GetString("AccountDataDeleteMessage");
        public static string AccountDataDeleteMessageOK = Loader.GetString("AccountDataDeleteMessageOK");
        public static string AccountDataDeleteMessageCancel = Loader.GetString("AccountDataDeleteMessageCancel");
        public static string AccountDataDeleteSuccess = Loader.GetString("AccountDataDeleteSuccess");
        public static string AccountDataDeleteSuccess2 = Loader.GetString("AccountDataDeleteSuccess2");

        public static string AccountSave = Loader.GetString("AccountSave");
        public static string AccountSaveError = Loader.GetString("AccountSaveError");

        public static string FormatASecondAgo = Loader.GetString("FormatASecondAgo");
        public static string FormatXSecondsAgo = Loader.GetString("FormatXSecondsAgo");
        public static string FormatAMinuteAgo = Loader.GetString("FormatAMinuteAgo");
        public static string FormatXMinutesAgo = Loader.GetString("FormatXMinutesAgo");
        public static string FormatAnHourAgo = Loader.GetString("FormatAnHourAgo");
        public static string FormatXHoursAgo = Loader.GetString("FormatXHoursAgo");
        public static string FormatADayAgo = Loader.GetString("FormatADayAgo");
        public static string FormatXDaysAgo = Loader.GetString("FormatXDaysAgo");
        public static string FormatAMonthAgo = Loader.GetString("FormatAMonthAgo");
        public static string FormatXMonthsAgo = Loader.GetString("FormatXMonthsAgo");
        public static string FormatAYearAgo = Loader.GetString("FormatAYearAgo");
        public static string FormatXYearsAgo = Loader.GetString("FormatXYearsAgo");
    }
}
