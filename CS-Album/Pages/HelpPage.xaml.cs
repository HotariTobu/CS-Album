using System;
using System.Windows;
using System.Windows.Controls;

namespace CS_Album
{
    /// <summary>
    /// HelpPage.xaml の相互作用ロジック
    /// </summary>
    public partial class HelpPage : Page
    {
        private HelpPageViewModel ViewModel { get; }

        public HelpPage()
        {
            InitializeComponent();
            ViewModel = DataContext as HelpPageViewModel;
        }

        private void Link_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement frameworkElement && frameworkElement.Tag is string uri)
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("cmd", $"/c start {uri.Replace("&", "^&")}") { CreateNoWindow = true });
                }
                catch (Exception ee)
                {
                    MainWindow.LogException(ee);
                }
            }
        }
    }
}
