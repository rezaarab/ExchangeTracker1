using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ExchangeTracker.Presentation.Common;
using Path = System.IO.Path;

namespace ExchangeTracker.Presentation.Views
{
    /// <summary>
    /// Interaction logic for ChangeValueView.xaml
    /// </summary>
    public partial class ChangeValueView
    {
        private string _oldValue;

        public ChangeValueView()
        {
            InitializeComponent();
        }

        public bool IsAccept { get; set; }

        public string NewValue { get { return NewValueTextBox.Text; } }

        public string OldValue
        {
            get { return _oldValue; }
            set
            {
                _oldValue = value;
                if (string.IsNullOrEmpty(NewValueTextBox.Text))
                    NewValueTextBox.Text = value;
            }
        }

        private void ButtonOk_OnClick(object sender, RoutedEventArgs e)
        {
            OnOk();
        }

        private void OnOk()
        {
            if (!string.IsNullOrWhiteSpace(NewValue))
            {
                IsAccept = true;
                this.Close();
            }
            else
            {
                MessageBoxHelper.Show("لطفا عنوان جدید را وارد نمایید");
            }
        }

        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        {
            IsAccept = false;
            this.Close();
        }

        private void ChangeValueView_OnLoaded(object sender, RoutedEventArgs e)
        {
            NewValueTextBox.Focus();
        }

        private void PasswordTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                OnOk();
        }
    }
}
