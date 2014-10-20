
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Data;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.ExpressionEditor;
using DevExpress.Xpf.Editors.ExpressionEditor.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Grid;
using ExchangeTracker.Presentation.Common;
using ExchangeTracker.Presentation.Services;
using ExchangeTracker.Presentation.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Win32;

namespace ExchangeTracker.Presentation.Views
{
    /// <summary>
    /// Interaction logic for OnlineTrackItems.xaml
    /// </summary>
    public partial class OnlineTrackItemsView
    {
        public OnlineTrackItemsView()
        {
            InitializeComponent();
            GridHelper.RestoreLayout(dataGrid);
        }

        public OnlineTrackItemsViewModel ViewModel
        {
            get
            {
                return DataContext as OnlineTrackItemsViewModel;
            }
        }


        private void OnlineTrackItemsView_OnLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel.SymbolGroups = StockService.GetSymbolGroups();
        }

        public override void OnUnloaded()
        {
            base.OnUnloaded();
            GridHelper.SaveLayout(dataGrid);
        }

        private void ExcellButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog { Filter = "Excel File(*.xlsx)|*.xlsx" };
            if (dialog.ShowDialog() == true)
            {
                View.ExportToXlsx(dialog.FileName);
            }
        }

        private void View_OnRowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            if (e.HitInfo.InRow && e.HitInfo.RowHandle >= 0)
            {
                ViewModel.NavigateCompanyHistory();
            }
        }

        private void DataGrid_OnCustomUnboundColumnData(object sender, GridColumnDataEventArgs e)
        {
            //            if (e.IsGetData && e.Column.FieldName == "RowNumber")
            //                e.Value = e.ListSourceRowIndex + 1;
        }

        private void View_OnFilterEditorCreated(object sender, FilterEditorEventArgs e)
        {
            e.FilterControl.Foreground = Brushes.Black;
        }

        //        private void ShowExpressionEditor(ColumnBase column)
        //        {
        //            var expressionEditorControl = new ExpressionEditorControl(column)
        //            {
        //                FlowDirection = FlowDirection.LeftToRight,
        //                Foreground = Brushes.Red,
        //            };
        //            DialogClosedDelegate closedHandler = delegate(bool? dialogResult)
        //            {
        //                if (dialogResult == true)
        //                    column.UnboundExpression = expressionEditorControl.Expression;
        //            };
        //            ExpressionEditorHelper.ShowExpressionEditor(expressionEditorControl, this, closedHandler);
        //        }
        //
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

        private void UIElement_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Delete && e.Key != Key.Back)
                return;

            var edit = (ComboBoxEdit)sender;
            if (!edit.AllowNullInput || edit.SelectionLength != edit.Text.Length)
                return;

            edit.EditValue = null;
            e.Handled = true;
        }

        private void View_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Left && dataGrid.CurrentColumn.IsLast) || (e.Key == Key.Right && dataGrid.CurrentColumn.IsFirst))
                e.Handled = true;
        }
    }
}
