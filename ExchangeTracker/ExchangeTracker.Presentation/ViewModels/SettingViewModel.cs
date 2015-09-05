using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ExchangeTracker.Domain;
using ExchangeTracker.Presentation.Common;
using ExchangeTracker.Presentation.Model;
using ExchangeTracker.Presentation.Services;
using GalaSoft.MvvmLight.Command;

namespace ExchangeTracker.Presentation.ViewModels
{
    public class SettingViewModel : MyViewModelBase
    {
        public SettingViewModel()
        {
            Task.Factory.StartNew(
                () =>
                {
                    Companies = StockService.GetInitDataCompanies().Select(p => p.Company).ToList();
                });
            _selectedInterval = StaticReference.Interval;
        }

        private List<Company> _companies;
        private Company _selectedCompany;
        private Company _selectedLiveCompany;
        private int _selectedInterval;
        private RelayCommand _addCompanyCommand;
        private RelayCommand _removeCompanyCommand;
        private RelayCommand _deleteDataCompanyCommand;

        private ObservableCollection<Company> _liveCompanies = new ObservableCollection<Company>();
        private readonly int[] _intervals = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        public List<Company> Companies
        {
            get { return _companies; }
            set { Set(() => Companies, ref _companies, value); }
        }

        public ObservableCollection<Company> LiveCompanies
        {
            get { return _liveCompanies; }
            set { Set(() => LiveCompanies, ref _liveCompanies, value); }
        }

        public Company SelectedCompany
        {
            get { return _selectedCompany; }
            set { Set(() => SelectedCompany, ref _selectedCompany, value); }
        }

        public int SelectedInterval
        {
            get { return _selectedInterval; }
            set
            {
                Set(() => SelectedInterval, ref _selectedInterval, value);
                StaticReference.Interval = value;
            }
        }

        public Company SelectedLiveCompany
        {
            get { return _selectedLiveCompany; }
            set { Set(() => SelectedLiveCompany, ref _selectedLiveCompany, value); }
        }

        public int[] Intervals
        {
            get { return _intervals; }
        }

        public RelayCommand AddCompanyCommand
        {
            get { return _addCompanyCommand ?? (_addCompanyCommand = new RelayCommand(AddCompany)); }
        }
        public RelayCommand RemoveCompanyCommand
        {
            get { return _removeCompanyCommand ?? (_removeCompanyCommand = new RelayCommand(RemoveCompany, CanRemoveCompany)); }
        }

        public RelayCommand DeleteDataCompanyCommand
        {
            get { return _deleteDataCompanyCommand ?? (_deleteDataCompanyCommand = new RelayCommand(DeleteDataCompany)); }
        }

        private void DeleteDataCompany()
        {
            if (MessageBoxResult.Yes ==
                MessageBoxHelper.Show("آیا برای حذف همه داده ها به جز موارد انتخاب شده مطمئن هستید؟", "حذف",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning))
            {
                DataService.DeleteAllHistoryExcepts(LiveCompanies.Select(p => p.StockId).ToArray());
            }
        }

        private void AddCompany()
        {
            if (SelectedCompany != null && LiveCompanies.All(p => p.StockId != SelectedCompany.StockId))
            {
                LiveCompanies.Add(SelectedCompany);
            }
        }

        private void RemoveCompany()
        {
            if (SelectedLiveCompany != null)
                LiveCompanies.Remove(SelectedLiveCompany);
        }
        private bool CanRemoveCompany()
        {
            return SelectedLiveCompany != null;
        }

    }
}
