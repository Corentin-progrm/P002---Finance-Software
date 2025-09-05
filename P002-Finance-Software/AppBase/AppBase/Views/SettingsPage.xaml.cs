using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace AppBase.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            // Charger le chemin du dossier si défini
            var localSettings = ApplicationData.Current.LocalSettings;
            FolderPathTextBox.Text = localSettings.Values["DataFolder"] as string ?? "Aucun dossier défini";
        }

        private async void OnChooseFolderClick(object sender, RoutedEventArgs e)
        {
            var picker = new FolderPicker();
            var hwnd = WindowNative.GetWindowHandle(App.MainWindowInstance);
            InitializeWithWindow.Initialize(picker, hwnd);

            var folder = await picker.PickSingleFolderAsync();
            if (folder != null)
            {
                FolderPathTextBox.Text = folder.Path;
                ApplicationData.Current.LocalSettings.Values["DataFolder"] = folder.Path;
            }
        }
    }
}
