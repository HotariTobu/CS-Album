using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CS_Album
{
    public class ImageItem : ListItemBase
    {
        public string PathText { get; set; }

        #region == Image ==

        private ImageSource _Image;
        public ImageSource Image
        {
            get => _Image;
            set
            {
                if (_Image != value)
                {
                    _Image = value;
                    if (_Image == null)
                    {
                        SetError(nameof(Image), "Error: Null Reference");
                    }
                    else
                    {
                        ClearErrror(nameof(Image));
                    }
                    RaisePropertyChanged(nameof(Image));
                }
            }
        }

        #endregion

        public string Name => System.IO.Path.GetFileName(PathText);

        #region == IconItemList ==

        private readonly ObservableCollection<IconItem> _IconItemList = new ObservableCollection<IconItem>();
        public ObservableCollection<IconItem> IconItemList => _IconItemList;

        #endregion
    }
}
