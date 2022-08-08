using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CS_Album
{
    public class PathPageViewModel : SharedWPF.ViewModelBase
    {
        #region == PathText ==

        private string _PathText;
        public string PathText
        {
            get => _PathText;
            set
            {
                if (_PathText != value)
                {
                    _PathText = value;
                    if (_PathText == null)
                    {
                        SetError(nameof(PathText), "Error: Null Reference");
                    }
                    else
                    {
                        ClearErrror(nameof(PathText));
                    }
                    RaisePropertyChanged(nameof(PathText));
                }
            }
        }

        #endregion

        #region == IsInvalidPathText ==

        private bool _IsInvalidPathText = true;
        public bool IsInvalidPathText
        {
            get => _IsInvalidPathText;
            set
            {
                if (_IsInvalidPathText != value)
                {
                    _IsInvalidPathText = value;
                    RaisePropertyChanged(nameof(IsInvalidPathText));
                }
            }
        }

        #endregion

        #region == ImageItemList ==

        private ObservableCollection<ImageItem> _ImageItemList = new ObservableCollection<ImageItem>();
        public ObservableCollection<ImageItem> ImageItemList => _ImageItemList;

        #endregion
    }
}
