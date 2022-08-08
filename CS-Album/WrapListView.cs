using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CS_Album
{
    public class WrapListView : ItemsControl
    {
        static WrapListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WrapListView), new FrameworkPropertyMetadata(typeof(WrapListView)));
        }

        #region == ItemWidth ==

        public double ItemWidth { get => (double)GetValue(ItemWidthProperty); set => SetValue(ItemWidthProperty, value < 0 ? 0 : value > MaxItemWidth ? MaxItemWidth : value); }
        public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register("ItemWidth", typeof(double), typeof(WrapListView), new PropertyMetadata(0d));

        #endregion

        #region == MaxItemWidth ==

        public double MaxItemWidth { get => (double)GetValue(MaxItemWidthProperty); set => SetValue(MaxItemWidthProperty, value); }
        public static readonly DependencyProperty MaxItemWidthProperty = DependencyProperty.Register("MaxItemWidth", typeof(double), typeof(WrapListView));

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("Grid") is Grid grid)
            {
                grid.PreviewMouseWheel += Grid_PreviewMouseWheel;
                grid.SizeChanged += Grid_SizeChanged;
            }
        }

        private void Grid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is UIElement uIElement && uIElement.IsVisible && (Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                ItemWidth += e.Delta / 20;
                e.Handled = true;
            }
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is Grid grid)
            {
                MaxItemWidth = grid.ActualWidth - 30;
            }
        }
    }
}
