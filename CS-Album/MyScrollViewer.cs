using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    public class MyScrollViewer : ScrollViewer
    {
        static MyScrollViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MyScrollViewer), new FrameworkPropertyMetadata(typeof(MyScrollViewer)));
        }

        #region == CanContentScale ==

        public bool CanContentScale { get => (bool)GetValue(CanContentScaleProperty); set => SetValue(CanContentScaleProperty, value); }
        public static readonly DependencyProperty CanContentScaleProperty = DependencyProperty.Register("CanContentScale", typeof(bool), typeof(MyScrollViewer), new PropertyMetadata(true));

        #endregion

        #region == Scale ==

        public double Scale { get => (double)GetValue(ScaleProperty); set => SetValue(ScaleProperty, value); }
        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", typeof(double), typeof(MyScrollViewer), new PropertyMetadata(1d));

        #endregion

        #region == MoveFactor ==

        public double MoveFactor { get => (double)GetValue(MoveFactorProperty); set => SetValue(MoveFactorProperty, value); }
        public static readonly DependencyProperty MoveFactorProperty = DependencyProperty.Register("MoveFactor", typeof(double), typeof(MyScrollViewer), new PropertyMetadata(2d));

        #endregion

        #region == ZoomFactor ==

        public double ZoomFactor { get => (double)GetValue(ZoomFactorProperty); set => SetValue(ZoomFactorProperty, value); }
        public static readonly DependencyProperty ZoomFactorProperty = DependencyProperty.Register("ZoomFactor", typeof(double), typeof(MyScrollViewer), new PropertyMetadata(100d));

        #endregion

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            if (CanContentScale && (Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                Scale *= Math.Pow(Math.Abs(e.Delta / ZoomFactor), Math.Sign(e.Delta));
                e.Handled = true;
            }
            else if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                ScrollToHorizontalOffset(HorizontalOffset - e.Delta / MoveFactor);
                e.Handled = true;
            }
            else
            {
                ScrollToVerticalOffset(VerticalOffset - e.Delta / MoveFactor);
                e.Handled = true;
            }
        }

        private Point StartMousePosition { get; set; }
        private Point StartScrollOffset { get; set; }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                StartMousePosition = e.GetPosition(this);
                StartScrollOffset = new Point(HorizontalOffset, VerticalOffset);
            }
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                double scale = Scale;
                Point point = e.GetPosition(this);
                ScrollToHorizontalOffset((StartMousePosition.X - point.X) / scale + StartScrollOffset.X);
                ScrollToVerticalOffset((StartMousePosition.Y - point.Y) / scale + StartScrollOffset.Y);
            }
        }
    }
}
