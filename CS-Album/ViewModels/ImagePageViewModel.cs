using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CS_Album
{
    public class ImagePageViewModel : SharedWPF.ViewModelBase
    {
        #region == AddButtonIsVisible ==

        private bool _AddButtonIsVisible;
        public bool AddButtonIsVisible
        {
            get => _AddButtonIsVisible;
            set
            {
                if (_AddButtonIsVisible != value)
                {
                    _AddButtonIsVisible = value;
                    RaisePropertyChanged(nameof(AddButtonIsVisible));
                }
            }
        }

        #endregion

        #region == RemoveButtonIsVisible ==

        private bool _RemoveButtonIsVisible;
        public bool RemoveButtonIsVisible
        {
            get => _RemoveButtonIsVisible;
            set
            {
                if (_RemoveButtonIsVisible != value)
                {
                    _RemoveButtonIsVisible = value;
                    RaisePropertyChanged(nameof(RemoveButtonIsVisible));
                }
            }
        }

        #endregion


        #region == SelectionX ==

        private double _SelectionX;
        public double SelectionX
        {
            get => _SelectionX;
            set
            {
                if (_SelectionX != value)
                {
                    _SelectionX = value;
                    RaisePropertyChanged(nameof(SelectionX));
                }
            }
        }

        #endregion

        #region == SelectionY ==

        private double _SelectionY;
        public double SelectionY
        {
            get => _SelectionY;
            set
            {
                if (_SelectionY != value)
                {
                    _SelectionY = value;
                    RaisePropertyChanged(nameof(SelectionY));
                }
            }
        }

        #endregion

        #region == SelectionWidth ==

        private double _SelectionWidth;
        public double SelectionWidth
        {
            get => _SelectionWidth;
            set
            {
                if (_SelectionWidth != value)
                {
                    _SelectionWidth = value;
                    RaisePropertyChanged(nameof(SelectionWidth));
                }
            }
        }

        #endregion

        #region == SelectionHeight ==

        private double _SelectionHeight;
        public double SelectionHeight
        {
            get => _SelectionHeight;
            set
            {
                if (_SelectionHeight != value)
                {
                    _SelectionHeight = value;
                    RaisePropertyChanged(nameof(SelectionHeight));
                }
            }
        }

        #endregion


        #region == ClassList ==

        private IEnumerable<string> _ClassList;
        public IEnumerable<string> ClassList
        {
            get => _ClassList;
            set
            {
                if (_ClassList != value)
                {
                    _ClassList = value;
                    if (_ClassList == null)
                    {
                        SetError(nameof(ClassList), "Error: Null Reference");
                    }
                    else
                    {
                        ClearErrror(nameof(ClassList));
                    }
                    RaisePropertyChanged(nameof(ClassList));
                }
            }
        }

        #endregion


        #region == GroupItemWidth ==

        private double _GroupItemWidth;
        public double GroupItemWidth
        {
            get => _GroupItemWidth;
            set
            {
                if (_GroupItemWidth != value)
                {
                    _GroupItemWidth = value < 0 ? 0 : value > GroupMaxItemWidth ? GroupMaxItemWidth : value;
                    RaisePropertyChanged(nameof(GroupItemWidth));
                }
            }
        }

        #endregion

        #region == GroupMaxItemWidth ==

        private double _GroupMaxItemWidth;
        public double GroupMaxItemWidth
        {
            get => _GroupMaxItemWidth;
            set
            {
                if (_GroupMaxItemWidth != value)
                {
                    _GroupMaxItemWidth = value;
                    RaisePropertyChanged(nameof(GroupMaxItemWidth));
                }
            }
        }

        #endregion

        #region == GroupItemList ==

        private ObservableCollection<GroupItem> _GroupItemList = new ObservableCollection<GroupItem>();
        public ObservableCollection<GroupItem> GroupItemList => _GroupItemList;

        #endregion

        #region == IconItemList ==

        private ObservableCollection<IconItem> _IconItemList = new ObservableCollection<IconItem>();
        public ObservableCollection<IconItem> IconItemList => _IconItemList;

        #endregion

        #region == GroupIsVisible ==

        private bool _GroupIsVisible = true;
        public bool GroupIsVisible
        {
            get => _GroupIsVisible;
            set
            {
                if (_GroupIsVisible != value)
                {
                    _GroupIsVisible = value;
                    RaisePropertyChanged(nameof(GroupIsVisible));
                }
            }
        }

        #endregion

        #region == IconIsVisible ==

        private bool _IconIsVisible = false;
        public bool IconIsVisible
        {
            get => _IconIsVisible;
            set
            {
                if (_IconIsVisible != value)
                {
                    _IconIsVisible = value;
                    RaisePropertyChanged(nameof(IconIsVisible));
                }
            }
        }

        #endregion

        public ImagePageViewModel()
        {
            GroupMaxItemWidth = 70;
            GroupItemWidth = 70;
        }
    }
}
