using GitMe.Common;
using GitMe.Data;
using GitMe.Schema;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;

#if DEBUG
using GitMe.UnitTests;
#endif

// Die Vorlage "Pivotanwendung" ist unter http://go.microsoft.com/fwlink/?LinkID=391641 dokumentiert.

namespace GitMe
{
    public sealed partial class PivotPage : Page, INotifyPage
    {
        private const string FirstGroupName = "FirstGroup";
        private const string SecondGroupName = "SecondGroup";
        private readonly string NotesGroupname = Constants.NotesGroupName;

        private readonly NavigationHelper navigationHelper;
        private static ObservableDictionary defaultViewModel = new ObservableDictionary();

        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        private static AppCredentials credentials = AppCredentials.Builder;
        public PivotPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Ruft den <see cref="NavigationHelper"/> ab, der mit dieser <see cref="Page"/> verknüpft ist.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Ruft das Anzeigemodell für diese <see cref="Page"/> ab.
        /// Dies kann in ein stark typisiertes Anzeigemodell geändert werden.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return defaultViewModel; }
        }

        private async void MainPageUpdate()
        {
            this.NotifyUser(Constants.PivotInitialization, NotifyType.StatusMessage);
            bool _isValid = true; // await credentials.IsValid();
            if (!_isValid)
                this.NotifyUser(Constants.PivotNoAccount, NotifyType.StatusMessage);
            else
            {
                var dataGroup = await DataSource.GetGroupAsync(this, NotesGroupname);
                this.DefaultViewModel[NotesGroupname] = dataGroup;
                await DataSource.WriteAllStorageFilesAsync(DataState.IsDataSaveDue());
                if (dataGroup.Items == null || dataGroup.Items.Count == 0)
                    this.NotifyUser(Constants.PivotNoData, NotifyType.StatusMessage);
                else
                    this.StatusBorder.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Füllt die Seite mit Inhalt auf, der bei der Navigation übergeben wird. Gespeicherte Zustände werden ebenfalls
        /// bereitgestellt, wenn eine Seite aus einer vorherigen Sitzung neu erstellt wird.
        /// </summary>
        /// <param name="sender">
        /// Die Quelle des Ereignisses, normalerweise <see cref="NavigationHelper"/>.
        /// </param>
        /// <param name="e">Ereignisdaten, die die Navigationsparameter bereitstellen, die an
        /// <see cref="Frame.Navigate(Type, Object)"/> als diese Seite ursprünglich angefordert wurde und
        /// ein Wörterbuch des Zustands, der von dieser Seite während einer früheren
        /// beibehalten wurde. Der Zustand ist beim ersten Aufrufen einer Seite NULL.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
              //(int) this.ActualWidth;
#if DEBUG
            //UnitTest.JsonUnitTest();
            //UnitTest.DateTimeTest();

#endif
            MainPageUpdate();
        }

        /// <summary>
        /// Behält den dieser Seite zugeordneten Zustand bei, wenn die Anwendung angehalten oder
        /// die Seite im Navigationscache verworfen wird. Die Werte müssen den Serialisierungsanforderungen
        /// von <see cref="SuspensionManager.SessionState"/> entsprech
        /// en.
        /// </summary>
        /// <param name="sender">Die Quelle des Ereignisses, normalerweise <see cref="NavigationHelper"/>.</param>
        /// <param name="e">Ereignisdaten, die ein leeres Wörterbuch zum Auffüllen bereitstellen
        /// serialisierbarer Zustand.</param>
        private async void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            if (DataState.IsDataSaveEnforced())
                await DataSource.WriteAllStorageFilesAsync(true);
        }

        private void RefreshAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            MainPageUpdate();
        }


        #region template_artefacts

        /// <summary>
        /// Wird aufgerufen, wenn auf ein Element innerhalb eines Abschnitts geklickt wird.
        /// </summary>
        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Zur entsprechenden Zielseite navigieren und die neue Seite konfigurieren,
            // indem die erforderlichen Informationen als Navigationsparameter übergeben werden
            var itemId = ((IDataItem)e.ClickedItem).UniqueId;
            if (!Frame.Navigate(typeof(ItemPage), itemId))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        { }

        /// <summary>
        /// Lädt den Inhalt für das zweite Pivotelement, wenn es per Bildlauf in die Anzeige verschoben wird.
        /// </summary>
        private async void SecondPivot_Loaded(object sender, RoutedEventArgs e)
        {
            await DataSource.GetGroupAsync(null, "Group-2");
            this.DefaultViewModel[SecondGroupName] = null;
        }

        #endregion

        #region NavigationHelper-Registrierung

        /// <summary>
        /// Die in diesem Abschnitt bereitgestellten Methoden werden einfach verwendet, um
        /// damit NavigationHelper auf die Navigationsmethoden der Seite reagieren kann.
        /// <para>
        /// Platzieren Sie seitenspezifische Logik in Ereignishandlern für  
        /// <see cref="NavigationHelper.LoadState"/>
        /// und <see cref="NavigationHelper.SaveState"/>.
        /// Der Navigationsparameter ist in der LoadState-Methode verfügbar 
        /// zusätzlich zum Seitenzustand, der während einer früheren Sitzung beibehalten wurde.
        /// </para>
        /// </summary>
        /// <param name="e">Stellt Daten für Navigationsmethoden und -ereignisse bereit.
        /// Handler, bei denen die Navigationsanforderung nicht abgebrochen werden kann.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void GoToAccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Frame.Navigate(typeof(AccountPage)))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        private void SettingsAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            GoToAccountButton_Click(sender, e);
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

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Frame.Navigate(typeof(AboutPage)))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

    }
}
