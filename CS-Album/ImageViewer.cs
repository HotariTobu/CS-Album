using System;
using System.Collections.Generic;
using System.IO;
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
    public class ImageViewer : Control
    {
        static ImageViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageViewer), new FrameworkPropertyMetadata(typeof(ImageViewer)));
        }

        #region == ImageSource ==

        public ImageSource ImageSource 
        { 
            get => GetValue(ImageSourceProperty) as ImageSource;
            set
            {
                if (value != null)
                {
                    SetValue(ImageSourceProperty, value);
                    ImageSize = new Size(value.Width, value.Height);
                    GetMinScale();
                    ApplyRange();
                    Reset();
                }
            }
        }
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageViewer));

        #endregion

        #region == Scale ==

        public double Scale { get => (double)GetValue(ScaleProperty); set => SetValue(ScaleProperty, value < MinScale ? MinScale : value > MaxScale ? MaxScale : value); }
        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", typeof(double), typeof(ImageViewer), new PropertyMetadata(1d));

        private static double MaxScale => 100d;
        private double MinScale { get; set; }

        #endregion

        #region == X ==

        public double X { get => (double)GetValue(XProperty); set => SetValue(XProperty, value < MinX ? MinX : value > MaxX ? MaxX : value); }
        public static readonly DependencyProperty XProperty = DependencyProperty.Register("X", typeof(double), typeof(ImageViewer), new PropertyMetadata(0d));

        private double MaxX { get; set; }
        private double MinX { get; set; }

        #endregion

        #region == Y ==

        public double Y { get => (double)GetValue(YProperty); set => SetValue(YProperty, value < MinY ? MinY : value > MaxY ? MaxY : value); }
        public static readonly DependencyProperty YProperty = DependencyProperty.Register("Y", typeof(double), typeof(ImageViewer), new PropertyMetadata(0d));

        private double MaxY { get; set; }
        private double MinY { get; set; }

        #endregion

        #region == Updated ==

        public event EventHandler Updated;
        private void RaiseUpdatedEvent() => Updated?.Invoke(this, new EventArgs());

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Canvas = GetTemplateChild("Canvas") as Canvas;
            if (Canvas != null)
            {
                Canvas.SizeChanged += Content_SizeChanged;
                Canvas.PreviewMouseWheel += Canvas_PreviewMouseWheel;
                Canvas.MouseDown += Canvas_MouseDown;
                Canvas.MouseMove += Canvas_MouseMove;
            }

            Image = GetTemplateChild("Image") as Image;
            if (Image != null)
            {
                Image.SizeChanged += Content_SizeChanged;
            }
        }

        private Canvas Canvas { get; set; }
        private Image Image { get; set; }

        private Size ImageSize { get; set; }

        private void GetMinScale() => MinScale = Math.Min(ActualWidth / ImageSize.Width, ActualHeight / ImageSize.Height);

        private void Content_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ImageSize.Width != 0 && ImageSize.Height != 0)
            {
                GetMinScale();
                ApplyRange();
                if (Scale < MinScale)
                {
                    Reset();
                    RaiseUpdatedEvent();
                }
            }
        }

        #region == Mouse ==

        private Point StartMousePosition { get; set; }
        private Point StartCornerPosition { get; set; }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Canvas != null && e.MiddleButton == MouseButtonState.Pressed)
            {
                StartMousePosition = e.GetPosition(Canvas);
                StartCornerPosition = new Point(X, Y);
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (Canvas != null && e.MiddleButton == MouseButtonState.Pressed)
            {
                Point point = e.GetPosition(Canvas);
                Point sub = new Point(point.X - StartMousePosition.X, point.Y - StartMousePosition.Y);
                X = StartCornerPosition.X + sub.X;
                Y = StartCornerPosition.Y + sub.Y;

                RaiseUpdatedEvent();
            }
        }

        private static double MoveFactor => 4d;
        private static double ZoomFactor => 100d;

        private void Canvas_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                if (Image != null)
                {
                    Point point1 = e.GetPosition(Image);
                    double scale = Scale *= Math.Pow(Math.Abs(e.Delta / ZoomFactor), Math.Sign(e.Delta));

                    ApplyRange();

                    Point point2 = e.GetPosition(Image);
                    X += (point2.X - point1.X) * scale;
                    Y += (point2.Y - point1.Y) * scale;
                }
            }
            else if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                X += e.Delta / MoveFactor;
            }
            else
            {
                Y += e.Delta / MoveFactor;
            }

            RaiseUpdatedEvent();
        }

        #endregion

        private void ApplyRange()
        {
            double scale = Scale;
            MaxX = ActualWidth;
            MinX = -ImageSize.Width * scale;
            MaxY = ActualHeight;
            MinY = -ImageSize.Height * scale;
        }

        public void Reset()
        {
            double scale = Scale = MinScale;
            X = (ActualWidth - ImageSize.Width * scale) / 2;
            Y = (ActualHeight - ImageSize.Height * scale) / 2;
        }
    }
}
