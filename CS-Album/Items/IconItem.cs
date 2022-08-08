using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CS_Album
{
    public class IconItem : ListItemBase
    {
        public string Group { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }

        #region == Icon ==

        private ImageSource _Icon;
        public ImageSource Icon
        {
            get => _Icon;
            set
            {
                if (_Icon != value)
                {
                    _Icon = value;
                    if (_Icon == null)
                    {
                        SetError(nameof(Icon), "Error: Null Reference");
                    }
                    else
                    {
                        ClearErrror(nameof(Icon));
                    }
                    RaisePropertyChanged(nameof(Icon));
                }
            }
        }

        #endregion


        #region == IconList ==

        private readonly ConcurrentDictionary<string, ImageSource> _IconList = new ConcurrentDictionary<string, ImageSource>();
        public ConcurrentDictionary<string, ImageSource> IconList => _IconList;

        #endregion

        public void ChangeIcon(int count)
        {
            if (IconList == null || IconList.IsEmpty)
            {
                return;
            }

            Icon = IconList.Values.ElementAtOrDefault(count % IconList.Count);
        }
    }
}
