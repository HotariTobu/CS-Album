using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CS_Album
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel ViewModel { get; }

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = DataContext as MainWindowViewModel;
            ViewModel.PathTab = PathPage;
            ViewModel.AlbumTab = AlbumPage;
            ViewModel.ImageTab = ImagePage;
            ViewModel.CheckTab = CheckPage;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<ImageItem> imageItemList1 = ViewModel?.PathPageViewModel?.ImageItemList?.ToList();
            List<ImageItem> imageItemList2 = ViewModel?.AlbumPageViewModel?.ImageItemList?.ToList();
            List<RosterItem> rosterItemList = ViewModel?.CheckPageViewModel?.RosterItemList?.ToList();
            List<PersonItem> personItemList = new List<PersonItem>();
            rosterItemList?.ForEach(rosterItem => personItemList.AddRange(rosterItem.PersonItemList));

            if (ViewModel.SearchText is string text && ViewModel?.Rosters is Dictionary<string, IconItem[]> rosters)
            {
                if (text.Length == 0)
                {
                    rosters.Values.ToList().ForEach(iconItemList => iconItemList.ToList().ForEach(iconItem => iconItem.IsVisible = true));
                    imageItemList1?.ForEach(imageItem => imageItem.IsVisible = true);
                    imageItemList2?.ForEach(imageItem => imageItem.IsVisible = true);
                    rosterItemList?.ForEach(rosterItem => rosterItem.IsVisible = true);
                    personItemList?.ForEach(personItem => personItem.IsVisible = true);
                }
                else
                {
                    List<IconItem> candidates = new List<IconItem>();
                    foreach (IEnumerable<IconItem> iconItemList in rosters.Values)
                    {
                        candidates.AddRange(iconItemList.Where(iconItem => iconItem.IsVisible = iconItem.Name.Contains(text)));
                    }
                    if (candidates.Count == 0)
                    {
                        imageItemList1?.ForEach(imageItem => imageItem.IsVisible = false);
                        imageItemList2?.ForEach(imageItem => imageItem.IsVisible = false);
                        rosterItemList?.ForEach(rosterItem => rosterItem.IsVisible = false);
                        personItemList?.ForEach(personItem => personItem.IsVisible = false);
                    }
                    else
                    {
                        imageItemList1?.ForEach(imageItem => imageItem.IsVisible = imageItem.IconItemList.Any(iconItem => candidates.Contains(iconItem)));
                        imageItemList2?.ForEach(imageItem => imageItem.IsVisible = imageItem.IconItemList.Any(iconItem => candidates.Contains(iconItem)));
                        IEnumerable<string> names = candidates.Select(candidate => candidate.Name);
                        personItemList?.ForEach(personItem => personItem.IsVisible = names.Contains(personItem.Name, EqualityComparer<string>.Default));
                        rosterItemList?.ForEach(rosterItem => rosterItem.IsVisible = rosterItem.PersonItemList.Any(personItem => personItem.IsVisible));
                    }
                }
            }
        }

        private static string LogFileName => "Exception.log";

        public static void  LogException(Exception e)
        {
            File.AppendAllText(LogFileName,
                $"{DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}\n" +
                $"{e.Message}\n" +
                $"{e.StackTrace}\n\n");

            MessageBox.Show($"お手数ですが、実行ファイルと同じ階層にある{LogFileName}をメールで送信するようお願いします。", "例外が発生しました。", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
