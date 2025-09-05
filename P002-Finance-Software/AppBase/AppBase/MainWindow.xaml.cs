using AppBase.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;

// Si un jour tu as cette merde d'erreur
// 'SettingsPage' does not contain a definition for 'InitializeComponent' and no accessible extension method 'InitializeComponent' accepting a first argument of type
// SettingsPage' could be found (are you missing a using directive or an assembly reference?)
// il suffit d'aller dans les propriete du fichier xaml et de mettre build action : Page

namespace AppBase
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            ContentFrame.Navigate(typeof(Page1));
        }



        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {

            if (args.IsSettingsInvoked)
            {
                // Clic sur le bouton Settings en bas
                ContentFrame.Navigate(typeof(SettingsPage));
                return;
            }

            if (args.InvokedItemContainer is NavigationViewItem item)
            {
                switch (item.Tag)
                {
                    case "page1":
                        ContentFrame.Navigate(typeof(Page1));
                        break;
                    case "ImportPage":
                        ContentFrame.Navigate(typeof(ImportPage));
                        break;
                    case "page3":
                        ContentFrame.Navigate(typeof(Page3));
                        break;
                    case "page4":
                        ContentFrame.Navigate(typeof(Page4));
                        break;
                    case "page5":
                        ContentFrame.Navigate(typeof(Page5));
                        break;
                }
            }
        }
    }
}
