
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
    /// Interaction logic for TrackItemModels.xaml
    /// </summary>
    public partial class TrackItemModelsView
    {
        public TrackItemModelsView()
        {
            InitializeComponent();
            GridHelper.RestoreLayout(ModelGridControl);
        }

        public TrackItemModelsViewModel ViewModel
        {
            get
            {
                return DataContext as TrackItemModelsViewModel;
            }
        }


        private void TrackItemModelsView_OnLoaded(object sender, RoutedEventArgs e)
        {
        }

        public override void OnUnloaded()
        {
            base.OnUnloaded();
            GridHelper.SaveLayout(ModelGridControl);
        }

        private void ExcellButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog { Filter = "Excel File(*.xlsx)|*.xlsx" };
            if (dialog.ShowDialog() == true)
            {
                View.ExportToXlsx(dialog.FileName);
            }
        }

        private void DataGrid_OnCustomUnboundColumnData(object sender, GridColumnDataEventArgs e)
        {
        }

        private void View_OnFilterEditorCreated(object sender, FilterEditorEventArgs e)
        {
            e.FilterControl.Foreground = Brushes.DarkSlateBlue;
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
            if ((e.Key == Key.Left && ModelGridControl.CurrentColumn.IsLast) || (e.Key == Key.Right && ModelGridControl.CurrentColumn.IsFirst))
                e.Handled = true;
        }
    }
}
