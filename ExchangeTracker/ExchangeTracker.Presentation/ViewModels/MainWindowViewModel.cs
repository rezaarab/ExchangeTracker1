using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Ioc;
using ExchangeTracker.Presentation.Common;

namespace ExchangeTracker.Presentation.ViewModels
{
    /// <summary>
    /// MainWindow view model.
    /// </summary>
    public class MainWindowViewModel : MyViewModelBase
    {
        public MainWindowViewModel()
        {
            MenuCommandObjects = new ObservableCollection<MenuCommandObject>
                {
                    new MenuCommandObject(SimpleIoc.Default.GetInstance<INavigation>("EmptyView"), "ஃ"),//"ஃ※⁂∷╸─✣"፧
                    new MenuCommandObject(SimpleIoc.Default.GetInstance<INavigation>("OnlineTrackItemsView"), "OnlineTrackItems"),
                    new MenuCommandObject(SimpleIoc.Default.GetInstance<INavigation>("SymbolGroupView"), "SymbolGroup"),
                    new MenuCommandObject(SimpleIoc.Default.GetInstance<INavigation>("SettingView"), "Setting"),
                };
        }
        public ObservableCollection<MenuCommandObject> MenuCommandObjects { get; set; }

        public override string Title { get { return ResourceHelper.GetResource("MainWindow"); } }
    }
}
