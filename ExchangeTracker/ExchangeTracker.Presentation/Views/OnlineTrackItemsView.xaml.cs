
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
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
    public partial class OnlineTrackItemsView
    {
        public OnlineTrackItemsView()
        {
            InitializeComponent();
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
            //GridHelper.RestoreLayout(dataGrid);
        }

        public override void OnUnloaded()
        {
            base.OnUnloaded();
            //GridHelper.SaveLayout(dataGrid);
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
            if (e.IsGetData)
                e.Value = e.ListSourceRowIndex + 1;
        }
    }
}
