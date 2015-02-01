using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GitMe.Common;
using GitMe.Schema;
using GitMe.Data;

namespace GitMe.UnitTests
{
    public static class UnitTest
    {
        public static async void JsonUnitTest()
        {
            Dictionary<int, Notification> notificationStore = new Dictionary<int, Notification>();
            var dataSource = new DataSource();

            for (int i = 0; i < 5; i++)
            {
                var notification = new Notification()
                {
                    Id = i,
                    HtmlUrl = new Uri("http://details.HtmlUrl"),
                    UserId = i * 10,
                    SubjectTitle = String.Format("note.Subject.Title {0}", i.ToString()),
                    Body = String.Format("details.Body {0}", i.ToString()),
                    TimeStamp = DateTimeHelper.DateTimeToUtcString(DateTime.UtcNow)
                };
                notificationStore.Add(i, notification);
            }

            var text = JsonHelper.StorageToJsonString("Notifications", notificationStore);
            await FileIOHelper.WriteUtf8ToLocalFileAsync("GitMeTests", "Notifications", text);
            var text2 = await FileIOHelper.ReadLocalFileAsync("GitMeTests", "Notifications");

            await FileIOHelper.WriteUtf8ToSDAsync("GitMeTests", "JsonUnitTesttext.json", text);
            await FileIOHelper.WriteUtf8ToSDAsync("GitMeTests", "JsonUnitTesttext2.json", text2);

            Dictionary<int, Notification> notes = JsonHelper.DeserializeToStorage("Notifications", text);
            Dictionary<int, Notification> notes2 = JsonHelper.DeserializeToStorage("Notifications", text2);

            bool b = String.Equals(notes[0].Body, notes2[0].Body);
                
            return;
            
        }

        public static void DateTimeTest()
        {
            var now = DateTime.UtcNow;
            var now2 = DateTimeHelper.DateTimeToUtcString(now);
            var now3 = DateTimeHelper.ParsedOrDefaultDateTime(now2);

            bool b1 = (now - now3).TotalMilliseconds < 1000;

            var now4 = DateTimeHelper.DateTimeToUtcString(now3);

            bool b2 = String.Equals(now2, now4);

            return;
        }
    }
}
