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
using System.Windows.Shapes;

namespace CS_Album
{
    /// <summary>
    /// InputDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class InputDialog : Window
    {
        public InputDialog(string title, string message)
        {
            InitializeComponent();
            Title = title;
            TextBlock.Text = message;
        }

        #region == Message ==

        public string Message { get => GetValue(MessageProperty) as string; set => SetValue(MessageProperty, value); }
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(InputDialog), new PropertyMetadata(string.Empty));

        #endregion

        #region == Text ==

        public string Text { get => GetValue(TextProperty) as string; set => SetValue(TextProperty, value); }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(InputDialog), new PropertyMetadata(string.Empty));

        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox.Focus();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Text = TextBox.Text;
            DialogResult = true;

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

        }
    }
}
