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
using AppBase.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

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
            if (args.InvokedItemContainer is NavigationViewItem item)
            {
                switch (item.Tag)
                {
                    case "page1":
                        ContentFrame.Navigate(typeof(Page1));
                        break;
                    case "page2":
                        ContentFrame.Navigate(typeof(Page2));
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
