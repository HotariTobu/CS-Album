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
    /// PathPage.xaml の相互作用ロジック
    /// </summary>
    public partial class PathPage : Page
    {
        private MainWindowViewModel MainWindowViewModel { set; get; }
        private PathPageViewModel ViewModel { get; }

        public PathPage()
        {
            InitializeComponent();
            ViewModel = (Content as FrameworkElement)?.DataContext as PathPageViewModel;
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            DataContext = MainWindowViewModel = visualAdded?.FindAncestor<Window>()?.DataContext as MainWindowViewModel;
            if (MainWindowViewModel != null)
            {
                MainWindowViewModel.PathPageViewModel = ViewModel;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void PathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            System.Collections.ObjectModel.ObservableCollection<ImageItem> imageItemList = ViewModel.ImageItemList;
            MainWindowViewModel?.ClearImageItemList(imageItemList);

            string pathText = (sender as TextBox).Text;
            if (!(ViewModel.IsInvalidPathText = string.IsNullOrWhiteSpace(pathText)))
            {
                if (MainWindowViewModel.EnumerateAllFiles(pathText, MainWindowViewModel.ImagePattern) is IEnumerable<string> paths)
                {
                    MainWindowViewModel?.AddImageItems(imageItemList, paths);
                }
                else
                {
                    ViewModel.IsInvalidPathText = true;
                }
            }
        }

        private void ImageList_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void ImageList_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string path = (e.Data.GetData(DataFormats.FileDrop) as string[])[0];
                if (!string.IsNullOrWhiteSpace(path) && System.IO.Directory.Exists(path))
                {
                    ViewModel.PathText = path;
                }
            }
        }

        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindowViewModel?.ImageTab is TabItem imageTab && ((sender as FrameworkElement)?.Tag as ImageItem)?.PathText is string path)
            {
                IEnumerable<string> pathList = ViewModel.ImageItemList.Select(imageItem => imageItem.PathText);
#if NET45 || NET451 || NET452 || NET46 || NET461 || NET462
                imageTab.Tag = new ImagePage.ListData() { list = pathList.ToArray(), index = pathList.ToList().IndexOf(path), tab = MainWindowViewModel?.PathTab };
#else
                imageTab.Tag = (pathList.ToArray(), pathList.ToList().IndexOf(path), MainWindowViewModel?.PathTab);
#endif
                imageTab.IsSelected = true;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (((sender as FrameworkElement)?.Tag as ImageItem)?.PathText is string path)
            {
                MainWindowViewModel?.AddPathToAlbum(path);
            }

            e.Handled = true;
        }
    }
}
