
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Grid;
using ExchangeTracker.Domain;
using ExchangeTracker.Presentation.Common;

namespace ExchangeTracker.Presentation.Views
{
    /// <summary>
    /// Interaction logic for EmptyView.xaml
    /// </summary>
    public partial class SymbolGroupView
    {
        public SymbolGroupView()
        {
            InitializeComponent();
        }

        private void UIElement_OnKeyUp(object sender, KeyEventArgs e)
        {
            var listBox = (sender as ListBox);
            if (e.Key == Key.Delete && listBox != null)
            {
                var observableCollection = listBox.ItemsSource as ObservableCollection<Company>;
                if (observableCollection != null && listBox.SelectedItems.Count > 0)
                    observableCollection.Remove(listBox.SelectedItems[0] as Company);
                e.Handled = true;
            }
        }

        private void GridControl_OnKeyUp(object sender, KeyEventArgs e)
        {
            var grid = (sender as GridControl);
            if (e.Key == Key.Delete && grid != null)
            {
                var observableCollection = grid.ItemsSource as ObservableCollection<SymbolGroup>;
                if (observableCollection != null && grid.SelectedItems.Count > 0)
                {
                    var symbolGroup = grid.SelectedItems[0] as SymbolGroup;
                    if (symbolGroup != null && !symbolGroup.Companies.Any())
                        observableCollection.Remove(symbolGroup);
                    else
                    {
                        MessageBoxHelper.Show("لطفا ابتدا نمادهای گروه را حذف نمایید");
                    }
                }
            }

        }
    }
}
