
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Grid;
using ExchangeTracker.Presentation.Common;
using ExchangeTracker.Presentation.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Win32;

namespace ExchangeTracker.Presentation.Views
{
    /// <summary>
    /// Interaction logic for OnlineTrackItems.xaml
    /// </summary>
    public partial class CompanyHistoryView
    {
        public CompanyHistoryView()
        {
            InitializeComponent();
            GridHelper.RestoreLayout(dataGrid);
        }

        public CompanyHistoryViewModel ViewModel
        {
            get
            {
                return DataContext as CompanyHistoryViewModel;
            }
        }

        private void ExcellButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog { Filter = "Excel File(*.xlsx)|*.xlsx" };
            if (dialog.ShowDialog() == true)
            {
                View.ExportToXlsx(dialog.FileName);
            }
        }

        private void CompanyHistoryView_OnLoaded(object sender, RoutedEventArgs e)
        {
        }

        public override void OnUnloaded()
        {
            base.OnUnloaded();
            GridHelper.SaveLayout(dataGrid);
        }

        private void View_OnFilterEditorCreated(object sender, FilterEditorEventArgs e)
        {
            e.FilterControl.Foreground = Brushes.Black;
        }

        private void View_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Left && dataGrid.CurrentColumn.IsLast) || (e.Key == Key.Right && dataGrid.CurrentColumn.IsFirst))
                e.Handled = true;
        }

        private void View_OnShowGridMenu(object sender, GridMenuEventArgs e)
        {
            if (e.MenuType == GridMenuType.Column && e.MenuInfo.Column.AllowUnboundExpressionEditor)
            {
                var menu = e.MenuInfo.Menu;
                var barButtonItem = new BarButtonItem { Content = "Rename Header" };
                barButtonItem.ItemClick += (ss, ee) => ChangeColumnHeader(e.MenuInfo.Column);
                menu.ItemLinks.Add(barButtonItem);
            }
        }

        private void ChangeColumnHeader(ColumnBase column)
        {
            var changeForm = new ChangeValueView() { OldValue = column.Header.ToString() };
            changeForm.ShowDialog();
            if (changeForm.IsAccept)
                column.Header = changeForm.NewValue;
        }

    }
}
