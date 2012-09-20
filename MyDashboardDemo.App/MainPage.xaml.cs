using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace MyDashboardDemo.App
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : MyDashboardDemo.App.Common.LayoutAwarePage
    {
        private IPropertySet appSettings;
        private static MainPage _current;
        private const string userNameKey = "username";
        private const string pictureKey = "photo";
        
        public MainPage()
        {
            this.InitializeComponent();
            appSettings = ApplicationData.Current.RoamingSettings.Values;

            _current = this;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected async override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            if (pageState != null && pageState.ContainsKey("greetingOutputText"))
            {
                greetingOutput.Text = pageState["greetingOutputText"].ToString();
            }

            if (appSettings.ContainsKey(userNameKey))
            {
                nameInput.Text = appSettings[userNameKey].ToString();
            }

            if (appSettings.ContainsKey(pictureKey))
            {
                object filePath;
                if (appSettings.TryGetValue(pictureKey, out filePath) && filePath.ToString() != "")
                {
                    await ReloadPictureFromAppSettings(filePath.ToString());
                }
            }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            pageState["greetingOutputText"] = greetingOutput.Text;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            greetingOutput.Text = string.Format("Hello, {0}!", nameInput.Text);
        }

        private void nameInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Our applicationdata needs to be stored as it changes.
            appSettings[userNameKey] = nameInput.Text;
        }

        private void StartPreview(object sender, RoutedEventArgs e)
        {
        }


        private async void SelectPicture(object sender, RoutedEventArgs e)
        {
            StorageFile picture = await SelectPicture();
            if (picture != null)
            {
                await UsePicture(picture);
            }
        }

        private async Task<StorageFile> SelectPicture()
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            return await openPicker.PickSingleFileAsync();
        }

        private async void TakePicture(object sender, RoutedEventArgs e)
        {
            try
            {
                StorageFile picture = await CapturePicture();
                if (picture != null)
                {
                    await UsePicture(picture);
                }
                else
                {
                    NotifyUser("No photo taken.");
                }
            }
            catch (Exception ex)
            {
                NotifyUser(ex.Message);
            }
        }

        private async Task UsePicture(StorageFile file)
        {
            BitmapImage bitmapImage = new BitmapImage();
            using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
            {
                bitmapImage.SetSource(fileStream);
            }
            CapturedPhoto.Source = bitmapImage;

            // TODO: Store the file path in Application Data
            appSettings[pictureKey] = file.Path;
        }

        private async Task<StorageFile> CapturePicture()
        {
            // Using Windows.Media.Capture.CameraCaptureUI API to capture a photo
            CameraCaptureUI dialog = new CameraCaptureUI();
            Size aspectRatio = new Size(16, 9);
            dialog.PhotoSettings.CroppedAspectRatio = aspectRatio;

            return await dialog.CaptureFileAsync(CameraCaptureUIMode.Photo);
        }

        private async Task ReloadPictureFromAppSettings(String filePath)
        {
            try
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(filePath);
                BitmapImage bitmapImage = new BitmapImage();
                using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
                {
                    bitmapImage.SetSource(fileStream);
                }
                CapturedPhoto.Source = bitmapImage;
            }
            catch (Exception ex)
            {
                appSettings.Remove(pictureKey);
                NotifyUser(ex.Message);
            }
        }

        private async void NotifyUser(string message)
        {
            MessageDialog md = new MessageDialog(message, "Information");
            
            //md.Commands.Add( new UICommand("OK", new UICommandInvokedHandler((cmd) => result = true)));
            
            await md.ShowAsync();

        }

        public void ProcessQueryText(string queryText)
        {
            var message = string.Format("Someone is searching this app for : \"{0}\"", queryText);
            NotifyUser(message);
        }


        public static MainPage Current
        {
            get { return _current; }
        }
    }
}
