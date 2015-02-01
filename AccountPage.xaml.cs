using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups; 
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.Security.Credentials;

using GitMe.Common;
using GitMe.Common.OctokitWrapper;
using GitMe.Schema;
using GitMe.Data;


// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkID=390556 dokumentiert.

namespace GitMe
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class AccountPage : Page, INotifyPage
    {
        private readonly NavigationHelper navigationHelper;
        private AppCredentials cred = AppCredentials.Builder;

        public AccountPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Wird aufgerufen, wenn diese Seite in einem Frame angezeigt werden soll.
        /// </summary>
        /// <param name="e">Ereignisdaten, die beschreiben, wie diese Seite erreicht wurde.
        /// Dieser Parameter wird normalerweise zum Konfigurieren der Seite verwendet.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                this.AccountName.Text = cred.Login;
                this.AccountPassword.Password = cred.Password;
            }
            catch
            {
                this.AccountName.Text = "";
                this.AccountPassword.Password = "";
            }
        }


        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        //private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Ein geeignetes Datenmodell für die problematische Domäne erstellen, um die Beispieldaten auszutauschen
            //var item = await SampleDataSource.GetItemAsync((string)e.NavigationParameter);
            //this.DefaultViewModel["Item"] = item;

        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: den eindeutigen Zustand der Seite hier speichern.
        }

        private async void TestAccount_Click(object sender, RoutedEventArgs e)
        {
            this.StatusBorder.Visibility = Visibility.Collapsed;

            if (this.AccountName.Text == "")
            {
                NotifyUser(Constants.AccountTestError, NotifyType.ErrorMessage);
                return;
            }

            if (this.AccountPassword.Password != "")
            {
                var test1 = await new GitHubWrapper(this.AccountName.Text, this.AccountPassword.Password).TestCredentials();
                if (test1)
                {
                    NotifyUser(Constants.AccountTest, NotifyType.StatusMessage);
                    return;
                }
                else
                {
                    if (cred.Password == this.AccountPassword.Password)
                        NotifyUser(Constants.AccountTestError2, NotifyType.ErrorMessage);
                    else
                        NotifyUser(Constants.AccountTestError1, NotifyType.ErrorMessage);
                    return;
                }
            }

            if (cred.Password != "")
            {
                var test2 = await new GitHubWrapper(this.AccountName.Text, cred.Password).TestCredentials();
                if (test2)
                {
                    NotifyUser(Constants.AccountTest, NotifyType.StatusMessage);
                    return;
                }
                else
                {
                    NotifyUser(Constants.AccountTestError2, NotifyType.ErrorMessage);
                    return;
                }
            }
            NotifyUser(Constants.AccountTestError3, NotifyType.ErrorMessage);
        }

        private void SaveAccount_Click(object sender, RoutedEventArgs e)
        {
            this.StatusBorder.Visibility = Visibility.Collapsed;

            if (this.AccountName.Text != "" && this.AccountPassword.Password != "")
            {
                try
                {
                    cred.Login = this.AccountName.Text;
                    cred.Password = this.AccountPassword.Password;
                    NotifyUser(Constants.AccountSave, NotifyType.StatusMessage);
                }
                catch (Exception Error)
                {
                    NotifyUser(String.Format("{0}\n{1}", Constants.AccountSaveError, Error.Message), NotifyType.ErrorMessage);
                }
            }
        }

        private void AccountDeleteInvokedHandler(IUICommand command)
        {
            // Display message showing the label of the command that was invoked 
            switch ((int) command.Id)
            {
                case 0:
                    try
                    {
                        this.cred.RemoveCredentials(this.AccountName.Text);
                        NotifyUser(Constants.AccountDeleteSuccess, NotifyType.StatusMessage);
                        return;
                    }
                    catch
                    {
                        // ignore
                    }
                    break;
                default:
                    break;
            }
            NotifyUser(Constants.AccountDeleteSuccess2, NotifyType.StatusMessage);
        } 

        private async void DeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            StatusBorder.Visibility = Visibility.Collapsed;

            var messageDialog = new MessageDialog(Constants.AccountDeleteMessage);

            // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers 
            messageDialog.Commands.Add(new UICommand(Constants.AccountDeleteMessageOK, 
                                                     new UICommandInvokedHandler(this.AccountDeleteInvokedHandler), (object) 0));
            messageDialog.Commands.Add(new UICommand(Constants.AccountDeleteMessageCancel, 
                                                     new UICommandInvokedHandler(this.AccountDeleteInvokedHandler), (object) 1));

            // Set the command that will be invoked by default 
            messageDialog.DefaultCommandIndex = 1;

            // Set the command to be invoked when escape is pressed 
            messageDialog.CancelCommandIndex = 1;

            // Show the message dialog 
            await messageDialog.ShowAsync(); 



            this.AccountName.Text = "";
            this.AccountPassword.Password = "";
            //navigationHelper.GoBack();
        }

        public void NotifyUser(string strMessage, NotifyType type)
        {
            if (this.StatusBlock != null && this.StatusBorder != null)
            {
                switch (type)
                {
                    case NotifyType.StatusMessage:
                        this.StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Green);
                        break;
                    case NotifyType.ErrorMessage:
                        this.StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                        break;
                }
                this.StatusBlock.Text = strMessage;

                // Collapse the StatusBlock if it has no text to conserve real estate.
                if (this.StatusBlock.Text != String.Empty)
                {
                    this.StatusBorder.Visibility = Visibility.Visible;
                }
                else
                {
                    this.StatusBorder.Visibility = Visibility.Collapsed;
                }
            }
        }


        private async Task<string> DumpRawDataAsync(List<Octokit.Notification> notes)
        {
            var devices = Windows.Storage.KnownFolders.RemovableDevices;
            var sdCards = await devices.GetFoldersAsync();
            if (sdCards.Count == 0) return "";
            var firstCard = sdCards[0];
            StorageFolder notesFolder = await firstCard.CreateFolderAsync("GitMe", CreationCollisionOption.OpenIfExists);
            StorageFile file = await notesFolder.CreateFileAsync("RawData.json", CreationCollisionOption.ReplaceExisting);

            var contents = new StringBuilder();
            contents.AppendLine("{\"RawData\":[");
            if (notes.Count == 0)
            {
                contents.AppendLine("]\n}");
            }
            else
            {
                foreach (var note in notes)
                {
                    contents.Append(JsonHelper.DeserializeToOctokitNotification(note));
                    contents.Append(",");
                }
                var contents2 = contents.ToString().TrimEnd(',');
                contents.Clear().Append(contents2).AppendLine("\n]\n}");
            }
            var utf8 = new Windows.Storage.Streams.UnicodeEncoding();
            await FileIO.WriteTextAsync(file,contents.ToString(),utf8);
            
            return contents.ToString();
        }

        private async void AccountDataDeleteInvokedHandler(IUICommand command)
        {
            // Display message showing the label of the command that was invoked 
            switch ((int)command.Id)
            {
                case 0:
                    try
                    {
                        await FileIOHelper.WriteUtf8ToLocalFileAsync("Data", "Notifications.json", "");
                        await FileIOHelper.WriteUtf8ToLocalFileAsync("Data", "Repositories.json", "");
                        await FileIOHelper.WriteUtf8ToLocalFileAsync("Data", "Users.json", "");
                        DataState.LastDataFetchSuccess = false;
                        DataState.LastDataFetchAt = DateTime.UtcNow;
                        NotifyUser(Constants.AccountDataDeleteSuccess, NotifyType.StatusMessage);
                        return;
                    }
                    catch
                    {
                        // ignore
                    }
                    break;
                default:
                    break;
            }
            NotifyUser(Constants.AccountDataDeleteSuccess2, NotifyType.StatusMessage);
        } 

        private async void RemoveAccountDataButton_Click(object sender, RoutedEventArgs e)
        {
            StatusBorder.Visibility = Visibility.Collapsed;

            var messageDialog = new MessageDialog(Constants.AccountDataDeleteMessage);

            // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers 
            messageDialog.Commands.Add(new UICommand(Constants.AccountDataDeleteMessageOK,
                                                     new UICommandInvokedHandler(this.AccountDataDeleteInvokedHandler), (object)0));
            messageDialog.Commands.Add(new UICommand(Constants.AccountDataDeleteMessageCancel,
                                                     new UICommandInvokedHandler(this.AccountDataDeleteInvokedHandler), (object)1));

            // Set the command that will be invoked by default 
            messageDialog.DefaultCommandIndex = 1;

            // Set the command to be invoked when escape is pressed 
            messageDialog.CancelCommandIndex = 1;

            // Show the message dialog 
            await messageDialog.ShowAsync();
        }

        #region template_artefacts

        private void UnitTestAccountButton_Click(object sender, RoutedEventArgs e)
        { }

        private void UndoAccount_Click(object sender, RoutedEventArgs e)
        { }

        #endregion

    }
}
