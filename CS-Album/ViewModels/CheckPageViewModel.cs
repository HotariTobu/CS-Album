using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CS_Album
{
    public class CheckPageViewModel : SharedWPF.ViewModelBase
    {
        #region == RosterItemList ==

        private readonly ObservableCollection<RosterItem> _RosterItemList = new ObservableCollection<RosterItem>();
        public ObservableCollection<RosterItem> RosterItemList => _RosterItemList;

        #endregion
    }
}
