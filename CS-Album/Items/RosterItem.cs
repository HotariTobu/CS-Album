using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CS_Album
{
    public class RosterItem : ListItemBase
    {
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

        #region == PersonItemList ==

        private ObservableCollection<PersonItem> _PersonItemList = new ObservableCollection<PersonItem>();
        public ObservableCollection<PersonItem> PersonItemList => _PersonItemList;

        #endregion
    }
}
