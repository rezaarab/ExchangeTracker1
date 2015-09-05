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
using FarsiLibrary.FX.Utils;
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
            //            TestTrackItemModel();
        }

        private void TestTrackItemModel()
        {
            using (var dbContext = new ExchangeDbCeEntities())
            {
                dbContext.TrackItemModel.RemoveRange(dbContext.TrackItemModel.ToList());
                dbContext.SaveChanges();
            }

            var stockId = "2135047529277416";
            var trackItemModels = new List<TrackItemModel>
            {
                new TrackItemModel
                {
                    LastTransactionDateTime = DateTime.Now.Date.Add(TimeSpan.FromMinutes(9*60 + 00)),
                    BuyLegalVolume = 2,
                    BuyRealVolume = 1,
                    BuyRealCount = 1,
                    BuyLegalCount = 1,
                    TransactionValue = -25
                },
                new TrackItemModel
                {
                    LastTransactionDateTime = DateTime.Now.Date.Add(TimeSpan.FromMinutes(9*60 + 00)),
                    BuyLegalVolume = 2,
                    BuyRealVolume = 1,
                    BuyRealCount = 1,
                    BuyLegalCount = 1,
                    TransactionValue = -30
                },
                new TrackItemModel
                {
                    LastTransactionDateTime = DateTime.Now.Date.Add(TimeSpan.FromMinutes(9*60 + 01)),
                    BuyLegalVolume = 4,
                    BuyRealVolume = 2,
                    BuyRealCount = 2,
                    BuyLegalCount = 2,
                    TransactionValue = 25
                },
                new TrackItemModel
                {
                    LastTransactionDateTime = DateTime.Now.Date.Add(TimeSpan.FromMinutes(9*60 + 03)),
                    BuyLegalVolume = 6,
                    BuyRealVolume = 3,
                    BuyRealCount = 3,
                    BuyLegalCount = 3,
                    TransactionValue = 15
                },
                new TrackItemModel
                {
                    LastTransactionDateTime = DateTime.Now.Date.Add(TimeSpan.FromMinutes(9*60 + 05)),
                    BuyLegalVolume = 2,
                    BuyRealVolume = 4,
                    BuyRealCount = 4,
                    BuyLegalCount = 4,
                    TransactionValue = 10
                },
                new TrackItemModel
                {
                    LastTransactionDateTime = DateTime.Now.Date.Add(TimeSpan.FromMinutes(9*60 + 10)),
                    BuyLegalVolume = 10,
                    BuyRealVolume = 5,
                    BuyRealCount = 5,
                    BuyLegalCount = 5
                },
                new TrackItemModel
                {
                    LastTransactionDateTime = DateTime.Now.Date.Add(TimeSpan.FromMinutes(9*60 + 11)),
                    BuyLegalVolume = 12,
                    BuyRealVolume = 6,
                    BuyRealCount = 6,
                    BuyLegalCount = 6
                },
                new TrackItemModel
                {
                    LastTransactionDateTime = DateTime.Now.Date.Add(TimeSpan.FromMinutes(9*60 + 15)),
                    BuyLegalVolume = 14,
                    BuyRealVolume = 7,
                    BuyRealCount = 7,
                    BuyLegalCount = 7
                },
                new TrackItemModel
                {
                    LastTransactionDateTime = DateTime.Now.Date.Add(TimeSpan.FromMinutes(9*60 + 30)),
                    BuyLegalVolume = 16,
                    BuyRealVolume = 8,
                    BuyRealCount = 8,
                    BuyLegalCount = 8
                },
                new TrackItemModel
                {
                    LastTransactionDateTime = DateTime.Now.Date.Add(TimeSpan.FromMinutes(9*60 + 32)),
                    BuyLegalVolume = 18,
                    BuyRealVolume = 9,
                    BuyRealCount = 9,
                    BuyLegalCount = 9
                },
            };
            trackItemModels.ForEach(p =>
            {
                p.Id = Guid.NewGuid();
                p.RegisterDateTime = DateTime.Now;
                p.StockId = stockId;
            });
            DataService.SaveTrackItems(trackItemModels);
            var viewModel = new TrackItemModelsViewModel
            {
                StockId = stockId,
                Date = DateTime.Now,
                Interval = 3,
            };
            var menuCommandObject = new MenuCommandObject(new TrackItemModelsView
            {
                DataContext = viewModel
            }, "s");
            viewModel.LoadInitialData();
            ModalDialogHelper.Show(menuCommandObject, true);
            ((UserControlBase)menuCommandObject.Navigator).OnUnloaded();
            Close();
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
