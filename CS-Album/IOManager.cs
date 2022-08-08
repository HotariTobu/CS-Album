using SharedWPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CS_Album
{
    public partial class MainWindowViewModel
    {
        #region == Paths ==

        public static string AlbumsPath => "Albums";
        public static string BondsPath => "Bonds";
        public static string IconsPath => "Icons";
        public static string RostersPath => "Rosters";

        public static string TextExtension => ".txt";
        public static string TextPattern => "*.txt";
        public static string ImageExtension => ".jpg";
        public static string ImagePattern => "*.jpg";

        public static string GetIconPath(string group, int number) => $"{IconsPath}\\{group}\\{number:00}";

        #endregion

        #region == Literals ==

        public static string AllClassLiteral => "すべてのクラス";
        public static string NewAlbumLiteral => "<新規アルバム>";

        public static string GetGroupName(string group) => group.Contains('組') ? group : group + '組';

        #endregion

        #region == System.IO ==

        public static IEnumerable<string> EnumerateAllFiles(string path, string searchPattern)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                if (directoryInfo.Exists)
                {
                    return directoryInfo.EnumerateFiles(searchPattern, SearchOption.AllDirectories).Select(fileInfo => fileInfo.FullName);
                }
            }
            catch (Exception e)
            {
                MainWindow.LogException(e);
            }

            return null;
        }

        #endregion

        #region == Album ==

        public string AlbumPath => (string.IsNullOrWhiteSpace(SelectedAlbum) || SelectedAlbum.Equals(NewAlbumLiteral)) ? null : $"{AlbumsPath}\\{SelectedAlbum}";
        public string DeletedAlbumPath => (string.IsNullOrWhiteSpace(SelectedAlbum) || SelectedAlbum.Equals(NewAlbumLiteral)) ? null : $"{AlbumPath}\\Deleted";
   
        public void RefreshAlbumList()
        {
            try
            {
                AlbumList.AddRange(Directory.CreateDirectory(AlbumsPath).EnumerateDirectories().Select(directoryInfo => directoryInfo.Name).Except(AlbumList.Where(album => !album.Equals(NewAlbumLiteral))));
            }
            catch (Exception e)
            {
                MainWindow.LogException(e);
            }
        }

        private static readonly string UnusableChars = "\\/:*?\"<>|";

        public bool AddPathToAlbum(string path)
        {
            if (string.IsNullOrWhiteSpace(SelectedAlbum))
            {
                return false;
            }

            if (SelectedAlbum.Equals(NewAlbumLiteral))
            {
                InputDialog inputDialog = new InputDialog("新規アルバム", "アルバムの名前を入力してください。")
                {
                    Owner = Application.Current.MainWindow
                };
                if (inputDialog.ShowDialog() ?? false)
                {
                    string album = inputDialog.Text;
                    if (string.IsNullOrWhiteSpace(album))
                    {
                        MessageBox.Show($"アルバムの名前を空白にすることはできません。", "アルバムの名前が無効です", MessageBoxButton.OK, MessageBoxImage.Information);
                        return false;
                    }
                    else if (album.Any(c => UnusableChars.Contains(c)))
                    {
                        MessageBox.Show($"アルバムの名前に {string.Join<char>(" ", UnusableChars)} を含むことはできません。", "アルバムの名前が無効です", MessageBoxButton.OK, MessageBoxImage.Information);
                        return false;
                    }
                    else
                    {
                        SelectedAlbum = album;
                    }
                }
                else
                {
                    return false;
                }
            }

            try
            {
                if (AlbumPath is string albumPath)
                {
                    File.AppendAllText($"{Directory.CreateDirectory(albumPath).FullName}\\{IPAddress}{TextExtension}", $"{path}\n", Encoding.UTF8);
                    if (DeletedAlbumPath is string deletedAlbumPath)
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(deletedAlbumPath);
                        if (directoryInfo.Exists)
                        {
                            string filePath = $"{directoryInfo.FullName}\\{IPAddress}{TextExtension}";
                            File.WriteAllLines(filePath, File.ReadAllLines(filePath, Encoding.UTF8).Where(line => !line.Equals(path)), Encoding.UTF8);
                        }

                        if (AlbumPageViewModel?.ImageItemList is System.Collections.ObjectModel.ObservableCollection<ImageItem> imageItemList)
                        {
                            imageItemList.Remove(imageItemList.FirstOrDefault(ImageItem => ImageItem.PathText.Equals(path)));
                        }

                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                MainWindow.LogException(e);
            }

            return false;
        }

        public bool RemovePathFromAlbum(string path)
        {
            try
            {
                if (DeletedAlbumPath != null)
                {
                    File.AppendAllText($"{Directory.CreateDirectory(DeletedAlbumPath).FullName}\\{IPAddress}{TextExtension}", $"{path}\n", Encoding.UTF8);

                    if (AlbumPageViewModel?.ImageItemList is System.Collections.ObjectModel.ObservableCollection<ImageItem> imageItemList)
                    {
                        imageItemList.Remove(imageItemList.FirstOrDefault(ImageItem => ImageItem.PathText.Equals(path)));
                    }

                    return true;
                }
            }
            catch (Exception e)
            {
                MainWindow.LogException(e);
            }

            return false;
        }

        public IEnumerable<string> GetPathsFromAlbum()
        {
            if (SelectedAlbum.Equals(NewAlbumLiteral))
            {
                return new string[0];
            }

            try
            {
                if (AlbumPath != null && getPaths(AlbumPath) is IEnumerable<string> paths)
                {
#if NET5_0 || NETCOREAPP || NETCOREAPP3_1
                    if (DeletedAlbumPath != null && getPaths(DeletedAlbumPath) is IEnumerable<string> deletedPaths)
#else
                    IEnumerable<string> deletedPaths = getPaths(DeletedAlbumPath);
                    if (DeletedAlbumPath != null && deletedPaths != null)
#endif
                    {
                        return paths.Except(deletedPaths);
                    }
                    else
                    {
                        return paths;
                    }
                }
                else
                {
                    return null;
                }

                IEnumerable<string> getPaths(string path)
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(path);
                    if (directoryInfo.Exists)
                    {
                        List<string> lines = new List<string>();
                        foreach (FileInfo fileInfo in directoryInfo.EnumerateFiles(TextPattern))
                        {
                            IEnumerable<string> text = File.ReadAllLines(fileInfo.FullName, Encoding.UTF8);
                            if (fileInfo.Name.StartsWith(IPAddress))
                            {
                                text = coerceLines(text);
                                File.WriteAllLines(fileInfo.FullName, text, Encoding.UTF8);
                            }
                            lines.AddRange(text);
                        }
                        return coerceLines(lines);
                    }
                    else
                    {
                        return null;
                    }
                }

                IEnumerable<string> coerceLines(IEnumerable<string> lines) => lines.Where(line => line.Length > 0).Distinct(EqualityComparer<string>.Default).Where(line => File.Exists(line));
            }
            catch (Exception e)
            {
                MainWindow.LogException(e);
            }

            return null;
        }

        #endregion

        #region == ImageSource ==

        public static ImageSource GetImageSource(string path, int limitedWidth = 0)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            try
            {
                FileInfo fileInfo = new FileInfo(path);
                if (fileInfo.Exists)
                {
                    Rotation rotation = Rotation.Rotate0;
                    using (FileStream fileStream = fileInfo.OpenRead())
                    {
                        BitmapFrame bitmapFrame = BitmapFrame.Create(fileStream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
                        BitmapMetadata bitmapMetadata = bitmapFrame.Metadata as BitmapMetadata;

                        if (bitmapMetadata?.GetQuery("System.Photo.Orientation") is ushort query)
                        {
                            switch (query)
                            {
                                case 6:
                                    rotation = Rotation.Rotate90;
                                    break;
                                case 3:
                                    rotation = Rotation.Rotate180;
                                    break;
                                case 8:
                                    rotation = Rotation.Rotate270;
                                    break;
                            }
                        }
                    }

                    BitmapImage bitmatImage = new BitmapImage();
                    bitmatImage.BeginInit();
                    bitmatImage.UriSource = new Uri(fileInfo.FullName);
                    if (limitedWidth > 0)
                    {
                        bitmatImage.DecodePixelWidth = limitedWidth;
                    }
                    bitmatImage.Rotation = rotation;
                    bitmatImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmatImage.CreateOptions = BitmapCreateOptions.None;
                    bitmatImage.EndInit();
                    bitmatImage.Freeze();
                    return bitmatImage;
                }
            }
            catch (Exception e)
            {
                MainWindow.LogException(e);
            }

            return null;
        }

        public delegate void GetImageSourceCallBack(ImageSource imageSource);
        public static void GetImageSourceAsync(string path, GetImageSourceCallBack callBack, int limitedWidth = 0) => Task.Run(() =>
        {
            if (GetImageSource(path, limitedWidth) is ImageSource imageSource)
            {
                try
                {
                    callBack?.Invoke(imageSource);
                }
                catch (Exception e)
                {
                    MainWindow.LogException(e);
                }
            }
        });

        public static Task<ImageSource> GetImageSourceAsync(string path, int limitedWidth = 0) => Task.Run(() => GetImageSource(path, limitedWidth));

        #endregion

        #region == Icon ==

        public void RefreshIconList()
        {
            try
            {
                foreach (IconItem[] iconItems in Rosters.Values)
                {
                    foreach (IconItem iconItem in iconItems)
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(GetIconPath(iconItem.Group, iconItem.Number));
                        if (directoryInfo.Exists)
                        {
                            foreach (string path in directoryInfo.EnumerateFiles(ImagePattern).Select(fileInfo => fileInfo.FullName).Except(iconItem.IconList.Keys))
                            {
                                GetImageSourceAsync(path, imageSource => iconItem.IconList.TryAdd(path, imageSource));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MainWindow.LogException(e);
            }
        }

        public static string SaveIcon(IconItem iconItem, BitmapSource bitmapSource)
        {
            if (iconItem == null || bitmapSource == null)
            {
                return null;
            }

            try
            {
                DirectoryInfo directoryInfo = Directory.CreateDirectory(GetIconPath(iconItem.Group, iconItem.Number));
                JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();
                jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                string path = $"{directoryInfo.FullName}\\{DateTime.Now.Ticks}{ImageExtension}";
                using (FileStream fileStream = new FileStream(path, FileMode.Create))
                {
                    jpegBitmapEncoder.Save(fileStream);
                }
                return path;
            }
            catch (Exception e)
            {
                MainWindow.LogException(e);
            }

            return null;
        }

        public void BindIcon(string path, IconItem iconItem)
        {
            if (string.IsNullOrWhiteSpace(path) || iconItem == null)
            {
                return;
            }

            try
            {
                string group = iconItem.Group;
                int number = iconItem.Number;
                File.AppendAllText($"{Directory.CreateDirectory(GetIconPath(group, number)).FullName}\\{IPAddress}{TextExtension}", $"{path}\n", Encoding.UTF8);
                File.AppendAllText($"{Directory.CreateDirectory(BondsPath).FullName}\\{IPAddress}{TextExtension}", $"{path}<{group}<{number:00}\n", Encoding.UTF8);
            }
            catch (Exception e)
            {
                MainWindow.LogException(e);
            }
        }

#if NET45 || NET451 || NET452 || NET46 || NET461 || NET462

public struct Bond
        {
            public string path;
            public string group;
            public int number;
        }

        public IEnumerable<Bond> GetIconBonds()
        {
            try
            {
                {
                    DirectoryInfo directoryInfo = Directory.CreateDirectory(BondsPath);
                    List<string> lines = new List<string>();
                    foreach (FileInfo fileInfo in directoryInfo.EnumerateFiles(TextPattern))
                    {
                        IEnumerable<string> text = File.ReadAllLines(fileInfo.FullName, Encoding.UTF8);
                        if (fileInfo.Name.StartsWith(IPAddress))
                        {
                            text = coerceLines(text).Select(tuple => $"{tuple.path}<{tuple.group}<{tuple.number:00}");
                            File.WriteAllLines(fileInfo.FullName, text, Encoding.UTF8);
                        }
                        lines.AddRange(text);
                    }
                    return coerceLines(lines);
                }

                IEnumerable<Bond> coerceLines(IEnumerable<string> lines) => lines.Where(line => line.Length > 0).Distinct(EqualityComparer<string>.Default).Select(line => line.Split('<').Where(token => token.Length > 0).ToArray()).Where(tokens => tokens.Length == 3).Select(tokens => new Bond() { path = tokens[0], group = tokens[1], number = int.Parse(tokens[2]) });

            }
            catch (Exception e)
            {
            MainWindow.LogException(e);
            }

            return null;
        }

#else

        public IEnumerable<(string, string, int)> GetIconBonds()
        {
            try
            {
                {
                    DirectoryInfo directoryInfo = Directory.CreateDirectory(BondsPath);
                    List<string> lines = new List<string>();
                    foreach (FileInfo fileInfo in directoryInfo.EnumerateFiles(TextPattern))
                    {
                        IEnumerable<string> text = File.ReadAllLines(fileInfo.FullName, Encoding.UTF8);
                        if (fileInfo.Name.StartsWith(IPAddress))
                        {
                            text = coerceLines(text).Select(tuple => $"{tuple.Item1}<{tuple.Item2}<{tuple.Item3:00}");
                            File.WriteAllLines(fileInfo.FullName, text, Encoding.UTF8);
                        }
                        lines.AddRange(text);
                    }
                    return coerceLines(lines);
                }

#if NET5_0 || NETCOREAPP || NETCOREAPP3_1
                static
#endif
                    IEnumerable<(string, string, int)> coerceLines(IEnumerable<string> lines) => lines.Where(line => line.Length > 0).Distinct(EqualityComparer<string>.Default).Select(line => line.Split('<').Where(token => token.Length > 0).ToArray()).Where(tokens => tokens.Length == 3).Select(tokens => (tokens[0], tokens[1], int.Parse(tokens[2])));

            }
            catch (Exception e)
            {
                MainWindow.LogException(e);
            }

            return null;
        }

#endif

        #endregion

        #region == CheckResult ==

        public string SaveCSV(bool isAlbumChecked)
        {
            try
            {
                string fileName = null;
                if (isAlbumChecked)
                {
                    fileName = SelectedAlbum;
                }
                else if (PathPageViewModel?.PathText is string path)
                {
                    fileName = Path.GetFileName(path);
                }

                if (fileName != null && CheckPageViewModel?.RosterItemList is System.Collections.ObjectModel.ObservableCollection<RosterItem> rosterList)
                {
                    List<string> lines = new List<string>
                    {
                        "組\t番号\t名前\tパス"
                    };
                    CheckPageViewModel?.RosterItemList?.ToList().ForEach(rosterItem => lines.AddRange(rosterItem.PersonItemList.Where(personItem => personItem.IsVisible).Select(personItem => $"{rosterItem.Name.TrimEnd('組')}\t{personItem.Number:00}\t{personItem.Name}\t" + string.Join("\t", personItem.Paths))));

                    fileName = $"{fileName}.csv";
                    using (StreamWriter streamWriter = File.CreateText(fileName))
                    {
                        streamWriter.Write(string.Join("\n", lines));
                    }

                    return Path.GetFullPath(fileName);
                }
            }
            catch (Exception e)
            {
                MainWindow.LogException(e);
            }

            return null;
        }

        #endregion
    }
}
