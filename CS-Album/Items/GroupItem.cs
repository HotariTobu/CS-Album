using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CS_Album
{
    public class GroupItem : SharedWPF.ViewModelBase
    {
        public string Name { get; set; }

        #region == IconItemList ==

        private ObservableCollection<IconItem> _IconItemList = new ObservableCollection<IconItem>();
        public ObservableCollection<IconItem> IconItemList => _IconItemList;

        #endregion
    }
}
