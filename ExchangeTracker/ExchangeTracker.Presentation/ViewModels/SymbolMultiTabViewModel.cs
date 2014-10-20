using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using ExchangeTracker.Domain;
using ExchangeTracker.Presentation.Common;
using ExchangeTracker.Presentation.ViewModels;
using ExchangeTracker.Presentation.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;

namespace ExchangeTracker.Presentation.ViewModel
{
    public class SymbolMultiTabViewModel : MyViewModelBase
    {
        private MenuCommandObject _currentTab;

        public SymbolMultiTabViewModel()
        {
            MenuCommandObjects = new ObservableCollection<MenuCommandObject>();
            MessengerInstance.Register<TrackItem>(this, "SymbolMultiTab", trackItem =>
            {
                if (MenuCommandObjects.All(p => p.Key != trackItem.Company.Symbol))
                {
                    var companyHistoryViewModel = new CompanyHistoryViewModel { TrackItem = trackItem };
                    var menuCommandObject = new MenuCommandObject(new CompanyHistoryView
                    {
//                        Name = string.Format("CompanyHistoryUserControl{0}", trackItem.Company.StockCode),
                        DataContext = companyHistoryViewModel
                    }, trackItem.Company.Symbol);
                    companyHistoryViewModel.LoadData();
                    MenuCommandObjects.Add(menuCommandObject);
                    CurrentTab = menuCommandObject;
                }
                else
                    CurrentTab = MenuCommandObjects.First(p => p.Key == trackItem.Company.Symbol);
            });

            //                {
            //                    new MenuCommandObject(SimpleIoc.Default.GetInstance<INavigation>("EmptyView"), "ஃ"),//"ஃ※⁂∷╸─✣"፧
            //                    new MenuCommandObject(SimpleIoc.Default.GetInstance<INavigation>("OnlineTrackItemsView"), "OnlineTrackItems"),
            //                };
        }
        public ObservableCollection<MenuCommandObject> MenuCommandObjects { get; set; }

        public override string Title { get { return string.Empty; } }

        public MenuCommandObject CurrentTab
        {
            get { return _currentTab; }
            set { Set(() => CurrentTab, ref _currentTab, value); }
        }

        public void OnClose(MenuCommandObject menuCommandObject)
        {
            MenuCommandObjects.Remove(menuCommandObject);
            if (!MenuCommandObjects.Any())
                AppHelper.MainWindow.Navigate("OnlineTrackItemsView");
        }
    }
}