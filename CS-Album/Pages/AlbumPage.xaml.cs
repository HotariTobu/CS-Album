using SharedWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// AlbumPage.xaml の相互作用ロジック
    /// </summary>
    public partial class AlbumPage : Page
    {
        private MainWindowViewModel MainWindowViewModel { set; get; }
        private AlbumPageViewModel ViewModel { get; }

        public AlbumPage()
        {
            InitializeComponent();
            ViewModel = (Content as FrameworkElement)?.DataContext as AlbumPageViewModel;
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            DataContext = MainWindowViewModel = visualAdded?.FindAncestor<Window>()?.DataContext as MainWindowViewModel;
            if (MainWindowViewModel != null)
            {
                MainWindowViewModel.AlbumPageViewModel = ViewModel;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainWindowViewModel != null)
            {
                MainWindowViewModel.RefreshAlbumList();
                int index = MainWindowViewModel.AlbumList.IndexOf(MainWindowViewModel.SelectedAlbum);
                if (AlbumComboBox.SelectedIndex != index)
                {
                    AlbumComboBox.SelectedIndex = index;
                }

                MainWindowViewModel.AddImageItems(ViewModel.ImageItemList, MainWindowViewModel.GetPathsFromAlbum()?.Except(ViewModel.ImageItemList.Select(imageItem => imageItem.PathText)));
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void AlbumComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainWindowViewModel != null)
            {
                MainWindowViewModel.SelectedAlbum = AlbumComboBox.SelectedItem as string;

                System.Collections.ObjectModel.ObservableCollection<ImageItem> imageItemList = ViewModel.ImageItemList;
                MainWindowViewModel.ClearImageItemList(imageItemList);
                MainWindowViewModel.AddImageItems(imageItemList, MainWindowViewModel.GetPathsFromAlbum());
            }
        }

        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindowViewModel?.ImageTab is TabItem imageTab && ((sender as FrameworkElement)?.Tag as ImageItem)?.PathText is string path)
            {
                IEnumerable<string> pathList = ViewModel.ImageItemList.Select(imageItem => imageItem.PathText);
#if NET45 || NET451 || NET452 || NET46 || NET461 || NET462
                imageTab.Tag = new ImagePage.ListData() { list = pathList.ToArray(), index = pathList.ToList().IndexOf(path), tab = MainWindowViewModel?.AlbumTab };
#else
                imageTab.Tag = (pathList.ToArray(), pathList.ToList().IndexOf(path), MainWindowViewModel?.AlbumTab);
#endif
                imageTab.IsSelected = true;
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (((sender as FrameworkElement)?.Tag as ImageItem)?.PathText is string path)
            {
                MainWindowViewModel?.RemovePathFromAlbum(path);
            }

            e.Handled = true;
        }
    }
}
