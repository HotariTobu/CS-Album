using SharedWPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// ImagePage.xaml の相互作用ロジック
    /// </summary>
    public partial class ImagePage : Page
    {
        private MainWindowViewModel MainWindowViewModel { set; get; }
        private ImagePageViewModel ViewModel { get; }

        public ImagePage()
        {
            InitializeComponent();
            ViewModel = (Content as FrameworkElement)?.DataContext as ImagePageViewModel;
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            DataContext = MainWindowViewModel = visualAdded?.FindAncestor<Window>()?.DataContext as MainWindowViewModel;
            if (MainWindowViewModel != null)
            {
                MainWindowViewModel.ImagePageViewModel = ViewModel;
                Dictionary<string, IconItem[]> rosters = MainWindowViewModel.Rosters;
                System.Collections.ObjectModel.ObservableCollection<GroupItem> groupItemList = ViewModel.GroupItemList;
                foreach (string group in rosters.Keys)
                {
                    GroupItem groupItem = new GroupItem() { Name = MainWindowViewModel.GetGroupName(group) };
                    groupItem.IconItemList.AddRange(rosters[group]);
                    groupItemList.Add(groupItem);
                }
#if NET45 || NET451 || NET452 || NET46 || NET461 || NET462 || NET47
                ViewModel.ClassList = new string[] { MainWindowViewModel.AllClassLiteral }.Union(groupItemList.Select(groupItem => groupItem.Name));
#else
                ViewModel.ClassList = groupItemList.Select(groupItem => groupItem.Name).Prepend(MainWindowViewModel.AllClassLiteral);
#endif
            }
        }

        private string[] PathList { get; set; }

        private Task<ImageSource> LoadImageSourceTask;
        private Task<ImageSource> LoadNextImageSourceTask;
        private Task<ImageSource> LoadPrevImageSourceTask;

        private int _PathIndex;
        private int PathIndex
        {
            get => _PathIndex;
            set
            {
                if (PathList == null)
                {
                    return;
                }

                if (value < PathList.Length && value >= 0)
                {
                    if (_PathIndex + 1 == value && LoadNextImageSourceTask != null)
                    {
                        LoadPrevImageSourceTask = LoadImageSourceTask;
                        LoadImageSourceTask = LoadNextImageSourceTask;
                        updateTask(ref LoadNextImageSourceTask, value + 1);
                    }
                    else if (_PathIndex - 1 == value && LoadPrevImageSourceTask != null)
                    {
                        LoadNextImageSourceTask = LoadImageSourceTask;
                        LoadImageSourceTask = LoadPrevImageSourceTask;
                        updateTask(ref LoadPrevImageSourceTask, value - 1);
                    }
                    else
                    {
                        updateTask(ref LoadImageSourceTask, value);
                        updateTask(ref LoadNextImageSourceTask, value + 1);
                        updateTask(ref LoadPrevImageSourceTask, value - 1);
                    }
                    LoadImageSourceTask?.Wait(5000);
                    ImageViewer.ImageSource = LoadImageSourceTask?.Result;
                    _PathIndex = value;
                    string path = PathList[value];
                    ViewModel.AddButtonIsVisible = !(ViewModel.RemoveButtonIsVisible = MainWindowViewModel?.GetPathsFromAlbum()?.Contains(path, EqualityComparer<string>.Default) ?? false);
                    ResetSelection();
                }

                void updateTask(ref Task<ImageSource> task, int index)
                {
                    if (index < PathList.Length && index >= 0)
                    {
                        task = MainWindowViewModel.GetImageSourceAsync(PathList[index]);
                    }
                }
            }
        }

        private TabItem BackTabItem { get; set; }

#if NET45 || NET451 || NET452 || NET46 || NET461 || NET462

public struct ListData
{
public string[] list;
public int index;
public TabItem tab;
}

#endif

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel?.RefreshIconList();

            if (MainWindowViewModel?.ImageTab is TabItem imageTab)
            {
                LoadImageSourceTask = null;
                LoadNextImageSourceTask = null;
                LoadPrevImageSourceTask = null;

#if NET45 || NET451 || NET452 || NET46 || NET461 || NET462
                if (imageTab?.Tag != null)
                {
                    ListData tuple = (ListData)imageTab.Tag;
                    PathList = tuple.list;
                    PathIndex = tuple.index;
                    BackTabItem = tuple.tab;
                    imageTab.Tag = null;
                }
#else
                (string[], int, TabItem)? tuple = imageTab.Tag as (string[], int, TabItem)?;
                if (tuple?.Item1 is string[] pathList && tuple?.Item2 is int pathIndex && tuple?.Item3 is TabItem backTabItem)
                {
                    PathList = pathList;
                    PathIndex = pathIndex;
                    BackTabItem = backTabItem;
                    imageTab.Tag = null;
                }
#endif
            }

            ImageViewer.Focus();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        #region == LeftTop ==

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindowViewModel?.AddPathToAlbum(PathList[PathIndex]) ?? false)
            {
                ViewModel.AddButtonIsVisible = !(ViewModel.RemoveButtonIsVisible = true);
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindowViewModel?.RemovePathFromAlbum(PathList[PathIndex]) ?? false)
            {
                ViewModel.AddButtonIsVisible = !(ViewModel.RemoveButtonIsVisible = false);
            }
        }

        #endregion

        #region == ImageViewer ==

        private void ImageViewer_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A:
                case Key.Left:
                    PathIndex--;
                    e.Handled = true;
                    break;
                case Key.S:
                case Key.Enter:
                    ResetSelection();
                    e.Handled = true;
                    break;
                case Key.D:
                case Key.Right:
                    PathIndex++;
                    e.Handled = true;
                    break;
            }
        }

        private void ImageViewer_MouseEnter(object sender, MouseEventArgs e)
        {
            ImageViewer.Focus();
        }

        private Point StartMousePosition { get; set; }
        private Point SelectionCorner { get; set; }
        private Size SelectionSize { get; set; }

        private void ResetSelection()
        {
            ViewModel.SelectionX = 0;
            ViewModel.SelectionY = 0;
            ViewModel.SelectionWidth = 0;
            ViewModel.SelectionHeight = 0;

            SelectionCorner = new Point();
            SelectionSize = new Size();
        }

        private void ImageViewer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StartMousePosition = e.GetPosition(sender as UIElement);
        }

        private void ImageViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point point = e.GetPosition(sender as UIElement);
                ViewModel.SelectionX = Math.Min(StartMousePosition.X, point.X);
                ViewModel.SelectionY = Math.Min(StartMousePosition.Y, point.Y);
                ViewModel.SelectionWidth = Math.Abs(StartMousePosition.X - point.X);
                ViewModel.SelectionHeight = Math.Abs(StartMousePosition.Y - point.Y);

                SelectionCorner = new Point((ViewModel.SelectionX - ImageViewer.X) / ImageViewer.Scale, (ViewModel.SelectionY - ImageViewer.Y) / ImageViewer.Scale);
                SelectionSize = new Size(ViewModel.SelectionWidth / ImageViewer.Scale, ViewModel.SelectionHeight / ImageViewer.Scale);
            }
        }

        private void ImageViewer_Updated(object sender, EventArgs e)
        {
            ViewModel.SelectionX = SelectionCorner.X * ImageViewer.Scale + ImageViewer.X;
            ViewModel.SelectionY = SelectionCorner.Y * ImageViewer.Scale + ImageViewer.Y;
            ViewModel.SelectionWidth = SelectionSize.Width * ImageViewer.Scale;
            ViewModel.SelectionHeight = SelectionSize.Height * ImageViewer.Scale;
        }

        private void ImageViewer_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (BackTabItem != null)
            {
                BackTabItem.IsSelected = true;
            }
        }

        #endregion

        #region == ClassComboBox ==

        private void ClassComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox)?.SelectedItem is string name)
            {
                if (name.Equals(MainWindowViewModel.AllClassLiteral))
                {
                    ViewModel.GroupIsVisible = true;
                    ViewModel.IconIsVisible = false;
                }
                else
                {
                    ViewModel.IconItemList.Clear();
                    ViewModel.IconItemList.AddRange(ViewModel.GroupItemList.FirstOrDefault(groupItem => groupItem.Name.Equals(name)).IconItemList);

                    ViewModel.GroupIsVisible = false;
                    ViewModel.IconIsVisible = true;
                }
            }
        }

        #endregion

        #region == IconList ==

        private void GroupGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is UIElement uIElement && uIElement.IsVisible && (Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                ViewModel.GroupItemWidth += e.Delta / 20;
                e.Handled = true;
            }
        }

        private void GroupGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is Grid grid)
            {
                ViewModel.GroupMaxItemWidth = grid.ActualWidth - 30;
            }
        }

        private void IconButton_Click(object sender, RoutedEventArgs e)
        {
            if (ImageViewer.ImageSource is BitmapSource bitmapSource && (sender as FrameworkElement)?.Tag is IconItem iconItem)
            {
                Vector vector1 = new Vector(SelectionCorner.X, SelectionCorner.Y);
                Vector vector2 = vector1 + new Vector(SelectionSize.Width, SelectionSize.Height);

                void coerceVector(ref Vector vector)
                {
                    if (vector.X < 0)
                    {
                        vector.X = 0;
                    }
                    else if (vector.X > bitmapSource.Width)
                    {
                        vector.X = bitmapSource.Width;
                    }

                    if (vector.Y < 0)
                    {
                        vector.Y = 0;
                    }
                    else if (vector.Y > bitmapSource.Height)
                    {
                        vector.Y = bitmapSource.Height;
                    }
                }

                coerceVector(ref vector1);
                coerceVector(ref vector2);

                Vector vector3 = vector2 - vector1;
                double raito = bitmapSource.PixelWidth / bitmapSource.Width;

                vector1 *= raito;
                vector3 *= raito;

                if (vector3.X > 0d && vector3.Y > 0d && vector3.Length > 0d)
                {
                    CroppedBitmap croppedBitmap = new CroppedBitmap(bitmapSource, new Int32Rect((int)vector1.X, (int)vector1.Y, (int)vector3.X, (int)vector3.Y));
                    if (MainWindowViewModel.SaveIcon(iconItem, croppedBitmap) is string path)
                    {
                        iconItem.IconList.TryAdd(path, croppedBitmap);
                    }
                }

                MainWindowViewModel?.BindIcon(PathList[PathIndex], iconItem);

                ResetSelection();
            }
        }

        #endregion
    }
}
