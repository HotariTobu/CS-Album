using SharedWPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
    /// CheckPage.xaml の相互作用ロジック
    /// </summary>
    public partial class CheckPage : Page
    {
        private MainWindowViewModel MainWindowViewModel { set; get; }
        private CheckPageViewModel ViewModel { get; }

        public CheckPage()
        {
            InitializeComponent();
            ViewModel = (Content as FrameworkElement)?.DataContext as CheckPageViewModel;
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            DataContext = MainWindowViewModel = visualAdded?.FindAncestor<Window>()?.DataContext as MainWindowViewModel;
            if (MainWindowViewModel != null)
            {
                MainWindowViewModel.CheckPageViewModel = ViewModel;
                Dictionary<string, IconItem[]> from = MainWindowViewModel.Rosters;
                System.Collections.ObjectModel.ObservableCollection<RosterItem> to = ViewModel.RosterItemList;
                foreach (string group in from.Keys)
                {
                    RosterItem rosterItem = new RosterItem() { Name = MainWindowViewModel.GetGroupName(group) };
                    rosterItem.PersonItemList.AddRange(from[group].Select((iconItem) => new PersonItem() { Name = iconItem.Name, Number = iconItem.Number + 1 }));
                    to.Add(rosterItem);
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainWindowViewModel?.SelectedAlbum?.Equals(MainWindowViewModel.NewAlbumLiteral) ?? false)
            {
                AlbumRadioButton.IsEnabled = false;
                ExternalRadioButton.IsChecked = true;
            }
            else
            {
                AlbumRadioButton.IsEnabled = true;
                AlbumRadioButton.IsChecked = true;
            }

            RadioButton_Click(null, null);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<string> paths = null;
            if (AlbumRadioButton.IsChecked ?? false)
            {
                paths = MainWindowViewModel?.GetPathsFromAlbum();
            }
            else if ((ExternalRadioButton.IsChecked ?? false) && MainWindowViewModel?.PathPageViewModel?.PathText is string path)
            {
                paths = MainWindowViewModel.EnumerateAllFiles(path, MainWindowViewModel.ImagePattern);
            }

            if (ViewModel != null)
            {
                foreach (RosterItem rosterItem in ViewModel.RosterItemList)
                {
                    foreach (PersonItem personItem in rosterItem.PersonItemList)
                    {
                        personItem.Count = 0;
                        for (int i = 0; i < personItem.Images.Count; i++)
                        {
                            personItem.Images[i] = null;
                        }
                        personItem.Images.Clear();
                        personItem.Paths.Clear();
                    }
                }
            }
#if NET45 || NET451 || NET452 || NET46 || NET461 || NET462
            if (paths != null && MainWindowViewModel?.GetIconBonds() is IEnumerable<MainWindowViewModel.Bond> iconBonds)
            {
                foreach (string path in paths)
                {
                    List<PersonItem> matches = new List<PersonItem>();
                    List<MainWindowViewModel.Bond> newBonds = new List<MainWindowViewModel.Bond>();
                    foreach (MainWindowViewModel.Bond iconBond in iconBonds)
                    {
                        if (iconBond.path.Equals(path))
                        {
                            string groupName = MainWindowViewModel.GetGroupName(iconBond.group);
                            PersonItem personItem = ViewModel.RosterItemList.FirstOrDefault(rosterItem => rosterItem.Name.Equals(groupName)).PersonItemList.ElementAtOrDefault(iconBond.number);
                            personItem.Count++;
                            personItem.Paths.Add(path);
                            matches.Add(personItem);
                        }
                        else
                        {
                            newBonds.Add(iconBond);
                        }
                    }
                    
                    if (matches.Count > 0)
                    {
                        MainWindowViewModel.GetImageSourceAsync(path, imageSource => Dispatcher.Invoke(() =>
                        {
                            foreach (PersonItem personItem in matches)
                            {
                                personItem.Images.Add(imageSource);
                            }
                        }), 128);
                    }

                    iconBonds = newBonds;
                }
            }
#else
            if (paths != null && MainWindowViewModel?.GetIconBonds() is IEnumerable<(string, string, int)> iconBonds)
            {
                foreach (string path in paths)
                {
                    List<PersonItem> matches = new List<PersonItem>();
                    List<(string, string, int)> newBonds = new List<(string, string, int)>();
                    foreach ((string, string, int) iconBond in iconBonds)
                    {
                        if (iconBond.Item1.Equals(path))
                        {
                            string groupName = MainWindowViewModel.GetGroupName(iconBond.Item2);
                            PersonItem personItem = ViewModel.RosterItemList.FirstOrDefault(rosterItem => rosterItem.Name.Equals(groupName)).PersonItemList.ElementAtOrDefault(iconBond.Item3);
                            personItem.Count++;
                            personItem.Paths.Add(path);
                            matches.Add(personItem);
                        }
                        else
                        {
                            newBonds.Add(iconBond);
                        }
                    }

                    if (matches.Count > 0)
                    {
                        MainWindowViewModel.GetImageSourceAsync(path, imageSource => Dispatcher.Invoke(() =>
                        {
                            foreach (PersonItem personItem in matches)
                            {
                                personItem.Images.Add(imageSource);
                            }
                        }), 128);
                    }

                    iconBonds = newBonds;
                }
            }
#endif   
        }

        private void AlbumBorder_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is UIElement uIElement && (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed) && MainWindowViewModel?.GetPathsFromAlbum() is IEnumerable<string> paths && paths.Any())
            {
                DragDrop.DoDragDrop(uIElement, new DataObject(DataFormats.FileDrop, paths.ToArray(), true), DragDropEffects.Copy);
            }
        }

        private void CSVBorder_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is UIElement uIElement && (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed) && MainWindowViewModel?.SaveCSV(AlbumRadioButton.IsChecked ?? false) is string path)
            {
                DragDrop.DoDragDrop(uIElement, new DataObject(DataFormats.FileDrop, new string[] { path, }, true), DragDropEffects.Move);
            }
        }
    }
}
