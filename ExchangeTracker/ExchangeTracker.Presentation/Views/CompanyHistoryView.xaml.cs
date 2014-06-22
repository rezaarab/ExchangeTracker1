
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
    public partial class CompanyHistoryView
    {
        public CompanyHistoryView()
        {
            InitializeComponent();
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
            //GridHelper.RestoreLayout(dataGrid);
        }

        public override void OnUnloaded()
        {
            base.OnUnloaded();
            //GridHelper.SaveLayout(dataGrid);
        }
    }
}
