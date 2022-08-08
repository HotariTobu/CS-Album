using System;
using System.Collections.Generic;
using System.Text;

namespace CS_Album
{
    public class ListItemBase : SharedWPF.ViewModelBase
    {
        #region == IsVisible ==

        private bool _IsVisible = true;
        public bool IsVisible
        {
            get => _IsVisible;
            set
            {
                if (_IsVisible != value)
                {
                    _IsVisible = value;
                    RaisePropertyChanged(nameof(IsVisible));
                }
            }
        }

        #endregion
    }
}
