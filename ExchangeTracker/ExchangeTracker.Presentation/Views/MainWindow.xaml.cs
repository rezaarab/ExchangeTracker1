using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpf.Editors.Helpers;
using ExchangeTracker.Presentation.Common;
using ExchangeTracker.Presentation.Services;
using ExchangeTracker.Presentation.ViewModels;
using ExchangeTracker.Presentation.Views;
using GalaSoft.MvvmLight.Ioc;
using MahApps.Metro.Controls;

namespace ExchangeTracker.Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            var login = new LoginView();
            login.ShowDialog();
            if (!login.IsAccept)
                Application.Current.Shutdown();

            InitializeComponent();
            SimpleIoc.Default.Register(() => this);
        }
        public MenuCommandObject Current { get; private set; }
        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var co = (sender as TabControl).SelectedItem as MenuCommandObject;
            if (co != Current)
            {
                Current = co;
                if (co != null && co.Navigator != null &&
                    (co.Navigator as FrameworkElement).DataContext is MyViewModelBase &&
                    ((co.Navigator as FrameworkElement).DataContext as MyViewModelBase).RefreshOnEnter())
                    co.Navigator.NavigateEnter();
            }
        }

        public MainWindowViewModel ViewModel
        {
            get
            {
                return DataContext as MainWindowViewModel;
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBoxHelper.Show("Design & Implementation: \"REZA ARAB\" \n (MINOOTA@GMAIL.COM <--> 09119792045)");
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            ExceptionHelper.DoAction(() => ViewModel.MenuCommandObjects.Select(p => p.Navigator).OfType<UserControlBase>().ToList().ForEach(p => p.OnUnloaded()));
            StockService.SaveStaticData();
        }

        public void Navigate(string key)
        {
            TabMenu.SelectedItem = ViewModel.MenuCommandObjects.First(p => p.Navigator == SimpleIoc.Default.GetInstance<INavigation>(key));
        }
    }
}
