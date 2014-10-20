/*
  In App.xaml:
  <Application.Resources>
      <vm:MvvmViewModelLocator xmlns:vm="clr-namespace:ExchangeTracker.Presentation.ViewModels"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using ExchangeTracker.Presentation.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using ExchangeTracker.Presentation.Common;
using ExchangeTracker.Presentation.Views;

namespace ExchangeTracker.Presentation.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// Use the <strong>mvvmlocatorproperty</strong> snippet to add ViewModels
    /// to this locator.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class MvvmViewModelLocator
    {
        static MvvmViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
            }
            else
            {
            }

            SimpleIoc.Default.Register<MainWindowViewModel>();
            SimpleIoc.Default.Register<OnlineTrackItemsViewModel>();
            SimpleIoc.Default.Register<SymbolMultiTabViewModel>();
            SimpleIoc.Default.Register<SymbolGroupViewModel>();

            SimpleIoc.Default.Register<INavigation>(() => new EmptyView(), "EmptyView");
            SimpleIoc.Default.Register<INavigation>(() => new OnlineTrackItemsView(), "OnlineTrackItemsView");
            SimpleIoc.Default.Register<INavigation>(() => new SymbolMultiTabView(), "SymbolMultiTabView");
            SimpleIoc.Default.Register<INavigation>(() => new SymbolGroupView(), "SymbolGroupView");
        }

        public MainWindowViewModel MainWindowViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainWindowViewModel>();
            }
        }
        public OnlineTrackItemsViewModel OnlineTrackItemsViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<OnlineTrackItemsViewModel>();
            }
        }

        public SymbolMultiTabViewModel SymbolMultiTabViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SymbolMultiTabViewModel>();
            }
        }
        public SymbolGroupViewModel SymbolGroupViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SymbolGroupViewModel>();
            }
        }
    }
}