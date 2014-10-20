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
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private string FilePath
        {
            get { return System.IO.Path.Combine(AppHelper.AppDataPath, "settings.sys"); }
        }

        public bool IsAccept { get; set; }

        private void ButtonOk_OnClick(object sender, RoutedEventArgs e)
        {
            OnOk();
        }

        private void OnOk()
        {
            const string share = "444";
            var decpass = File.Exists(FilePath) ? CryptoHelper.DecryptStringAES(File.ReadAllText(FilePath), share) : "444";

            if (decpass == PasswordTextBox.Password)
            {
                if (!string.IsNullOrWhiteSpace(NewPassTextBox.Text))
                {
                    if (
                        MessageBoxHelper.Show("آیا می خواهید پسورد را عوض نمایید", "", MessageBoxButton.YesNo,
                            MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        var newpassenc = CryptoHelper.EncryptStringAES(NewPassTextBox.Text, share);
                        File.WriteAllText(FilePath, newpassenc);
                    }
                }
                IsAccept = true;
                this.Close();
            }
            else
                MessageBoxHelper.Show("پسورد اشتباه است");
        }

        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        {
            IsAccept = false;
            this.Close();
        }

        private void LoginView_OnLoaded(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.Focus();
        }

        private void PasswordTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Enter)
                OnOk();
        }
    }
}
