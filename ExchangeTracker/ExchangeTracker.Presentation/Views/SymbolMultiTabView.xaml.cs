
using System.Windows;
using DevExpress.Xpf.Editors;
using ExchangeTracker.Domain;
using ExchangeTracker.Presentation.Common;
using ExchangeTracker.Presentation.ViewModel;

namespace ExchangeTracker.Presentation.Views
{
    /// <summary>
    /// Interaction logic for EmptyView.xaml
    /// </summary>
    public partial class SymbolMultiTabView
    {
        public SymbolMultiTabView()
        {
            InitializeComponent();
        }

        public SymbolMultiTabViewModel ViewModel
        {
            get { return DataContext as SymbolMultiTabViewModel; }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var menuCommandObject = (sender as FrameworkElement).DataContext as MenuCommandObject;
            (menuCommandObject.Navigator as UserControlBase).OnUnloaded();
            ViewModel.OnClose(menuCommandObject);
        }

        public override void OnUnloaded()
        {
            base.OnUnloaded();

        }
    }
}
