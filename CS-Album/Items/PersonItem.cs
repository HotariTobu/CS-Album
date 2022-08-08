using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace CS_Album
{
    public class PersonItem : ListItemBase
    {
        #region == Number ==

        private int _Number;
        public int Number
        {
            get => _Number;
            set
            {
                if (_Number != value)
                {
                    _Number = value;
                    RaisePropertyChanged(nameof(Number));
                }
            }
        }

        #endregion

        #region == Name ==

        private string _Name;
        public string Name
        {
            get => _Name;
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    if (_Name == null)
                    {
                        SetError(nameof(Name), "Error: Null Reference");
                    }
                    else
                    {
                        ClearErrror(nameof(Name));
                    }
                    RaisePropertyChanged(nameof(Name));
                }
            }
        }

        #endregion

        #region == Count ==

        private int _Count;
        public int Count
        {
            get => _Count;
            set
            {
                if (_Count != value)
                {
                    _Count = value;
                    RaisePropertyChanged(nameof(Count));
                }
            }
        }

        #endregion

        #region == Images ==

        private readonly ObservableCollection<ImageSource> _Images = new ObservableCollection<ImageSource>();
        public ObservableCollection<ImageSource> Images => _Images;

        #endregion

        #region == Paths ==

        private readonly ObservableCollection<string> _Paths = new ObservableCollection<string>();
        public ObservableCollection<string> Paths => _Paths;

        #endregion
    }

    public class CountToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
#if NET5_0 || NETCOREAPP || NETCOREAPP3_1
            return ((value as int?) ?? 0) switch
            {
                0 => Colors.Transparent,
                1 => Colors.Pink,
                2 => Colors.Red,
                3 => Colors.Orange,
                4 => Colors.Yellow,
                5 => Colors.LightGreen,
                6 => Colors.Lime,
                7 => Colors.Cyan,
                8 => Colors.DeepSkyBlue,
                9 => Colors.Blue,
                10 => Colors.BlueViolet,
                _ => Colors.Gray,
            };
#else
switch ((value as int?) ?? 0)
            {
                case 0:
                    return Colors.Transparent;
                case 1:
                    return Colors.Pink;
                case 2:
                    return Colors.Red;
                case 3:
                    return Colors.Orange;
                case 4:
                    return Colors.Yellow;
                case 5:
                    return Colors.LightGreen;
                case 6:
                    return Colors.Lime;
                case 7:
                    return Colors.Cyan;
                case 8:
                    return Colors.DeepSkyBlue;
                case 9:
                    return Colors.Blue;
                case 10:
                    return Colors.BlueViolet;
                default:
                    return Colors.Gray;
            }
#endif
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
