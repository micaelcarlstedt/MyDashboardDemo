using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace MyDashboardDemo.App
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : MyDashboardDemo.App.Common.LayoutAwarePage
    {
        public MainPage()
        {
            this.InitializeComponent();
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
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Restore session state
            if (pageState != null && pageState.ContainsKey("greetingOutputText"))
            {
                greetingOutput.Text = pageState["greetingOutputText"].ToString();
            }

            // TODO: Restore app data
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            if (roamingSettings.Values.ContainsKey("userName"))
            {
                nameInput.Text = roamingSettings.Values["userName"].ToString();
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
            Windows.Storage.ApplicationDataContainer roamingSettings =
                Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values["userName"] = nameInput.Text;

        }
    }
}
