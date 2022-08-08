using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Media;

namespace CS_Album
{
    public class AlbumPageViewModel : SharedWPF.ViewModelBase
    {
        #region == ImageItemList ==

        private ObservableCollection<ImageItem> _ImageItemList = new ObservableCollection<ImageItem>();
        public ObservableCollection<ImageItem> ImageItemList => _ImageItemList;

        #endregion
    }
}
