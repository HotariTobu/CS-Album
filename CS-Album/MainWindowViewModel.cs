using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace CS_Album
{
    public partial class MainWindowViewModel
    {
        public string IPAddress { get; }
        public Dictionary<string, IconItem[]> Rosters { get; }

        #region == TabItems ==
        
        public TabItem PathTab { get; set; }
        public TabItem AlbumTab { get; set; }
        public TabItem ImageTab { get; set; }
        public TabItem CheckTab { get; set; }

        #endregion

        #region == ViewModels ==

        public PathPageViewModel PathPageViewModel { get; set; }
        public AlbumPageViewModel AlbumPageViewModel { get; set; }
        public ImagePageViewModel ImagePageViewModel { get; set; }
        public CheckPageViewModel CheckPageViewModel { get; set; }

        #endregion

        #region == Album ==

        #region == AlbumList ==

        private readonly ObservableCollection<string> _AlbumList = new ObservableCollection<string>();
        public ObservableCollection<string> AlbumList => _AlbumList;

        #endregion

        public string SelectedAlbum { get; set; }

        #endregion

        #region == Icon ==

        private readonly Timer IconTimer;
        private int IconCount { get; set; }

        #endregion

        public string SearchText { get; set; }

        public MainWindowViewModel()
        {
            try
            {
                IPAddress = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName())?.Select(x => x.ToString())?.FirstOrDefault(x => x.IndexOf(".") > 0 && !x.StartsWith("127."))?.Replace('.', '_');
            }
            catch (Exception e)
            {
                MainWindow.LogException(e);
            }
            finally
            {
                if (string.IsNullOrWhiteSpace(IPAddress))
                {
                    MessageBox.Show("LAN内のコンピュータを識別し、疑似的な共同編集を実現するためにIPアドレスを利用しています。\nネットワークを確認してください。\n一時的に起動時刻が識別のために使われますが、編集するうえで不便が生じる可能性があります。", "IPアドレスを取得できませんでした", MessageBoxButton.OK, MessageBoxImage.Information);
                    IPAddress = DateTime.Now.Ticks.ToString();
                }
            }

            try
            {
                DirectoryInfo directoryInfo = Directory.CreateDirectory(RostersPath);
                Dictionary<string, IconItem[]> rosters = Rosters = new Dictionary<string, IconItem[]>();
                foreach (FileInfo fileInfo in directoryInfo.EnumerateFiles(TextPattern))
                {
                    string group = System.IO.Path.GetFileNameWithoutExtension(fileInfo.Name);
                    rosters.Add(group, File.ReadAllLines(fileInfo.FullName, Encoding.UTF8).Where(line => line.Length > 0).Select((name, number) => new IconItem() { Name = name, Group = group, Number = number }).ToArray());
                }
            }
            catch (Exception e)
            {
                MainWindow.LogException(e);
            }
            finally
            {
                if (Rosters.Count == 0)
                {
                    MessageBox.Show($"実行ファイルと同じ階層にある\"{RostersPath}\"というフォルダに名簿を入れてください。\n名簿はクラスごとに、名前を改行で区切り、BOM付きのUTF-8エンコードのテキストファイルで作ってください。", "名簿がありませんでした", MessageBoxButton.OK, MessageBoxImage.Information);
                    Application.Current.Shutdown();
                }
            }

            AlbumList.Add(NewAlbumLiteral);
            SelectedAlbum = NewAlbumLiteral;

            IconTimer = new Timer(new TimerCallback(o =>
            {
                try
                {
                    foreach (IconItem[] iconItems in Rosters.Values)
                    {
                        foreach (IconItem iconItem in iconItems)
                        {
                            iconItem.ChangeIcon(IconCount);
                        }
                    }
                    IconCount++;
                    GC.Collect();
                }
                catch (Exception e)
                {
                    MainWindow.LogException(e);
                    return;
                }
            }), null, 0, 2000);
            IconCount = 0;
        }

        #region == ImageItemList ==

        private bool IsAddingImageItemListCanceled { get; set; }

        public void ClearImageItemList(ObservableCollection<ImageItem> imageItemList)
        {
            if (imageItemList == null)
            {
                return;
            }

            foreach (ImageItem imageItem in imageItemList)
            {
                imageItem.Image = null;
                imageItem.IconItemList.Clear();
            }
            imageItemList.Clear();
            IsAddingImageItemListCanceled = true;
        }

        public async void AddImageItems(ObservableCollection<ImageItem> imageItemList, IEnumerable<string> paths)
        {
            if (imageItemList == null || paths == null || !paths.Any())
            {
                return;
            }

            IsAddingImageItemListCanceled = false;

#if NET45 || NET451 || NET452 || NET46 || NET461 || NET462
if (Rosters is Dictionary<string, IconItem[]> rosters && GetIconBonds() is IEnumerable<MainWindowViewModel.Bond> iconBonds)
            {
                foreach (string path in paths)
                {
                    ImageItem imageItem = new ImageItem() { PathText = path };
                    imageItem.Image = await MainWindowViewModel.GetImageSourceAsync(path, 256);

                    List<MainWindowViewModel.Bond> newBonds = new List<MainWindowViewModel.Bond>();
                    foreach (MainWindowViewModel.Bond bond in iconBonds)
                    {
                        if (bond.path.Equals(path))
                        {
                            imageItem.IconItemList.Add(rosters[bond.group][bond.number]);
                        }
                        else
                        {
                            newBonds.Add(bond);
                        }
                    }
                    iconBonds = newBonds;

                    if (IsAddingImageItemListCanceled)
                    {
                        IsAddingImageItemListCanceled = false;
                        return;
                    }
                    else
                    {
                        imageItemList.Add(imageItem);
                    }
                }
            }
#else
            if (Rosters is Dictionary<string, IconItem[]> rosters && GetIconBonds() is IEnumerable<(string, string, int)> iconBonds)
            {
                foreach (string path in paths)
                {
                    ImageItem imageItem = new ImageItem() { PathText = path };
                    imageItem.Image = await MainWindowViewModel.GetImageSourceAsync(path, 256);

                    List<(string, string, int)> newBonds = new List<(string, string, int)>();
                    foreach ((string, string, int) bond in iconBonds)
                    {
                        if (bond.Item1.Equals(path))
                        {
                            imageItem.IconItemList.Add(rosters[bond.Item2][bond.Item3]);
                        }
                        else
                        {
                            newBonds.Add(bond);
                        }
                    }
                    iconBonds = newBonds;

                    if (IsAddingImageItemListCanceled)
                    {
                        IsAddingImageItemListCanceled = false;
                        return;
                    }
                    else
                    {
                        imageItemList.Add(imageItem);
                    }
                }
            }
#endif
        }

        #endregion
    }
}
