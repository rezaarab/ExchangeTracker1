using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExchangeTracker.Domain;
using ExchangeTracker.Presentation.Common;
using ExchangeTracker.Presentation.Services;
using GalaSoft.MvvmLight.Command;

namespace ExchangeTracker.Presentation.ViewModels
{
    public class SymbolGroupViewModel : MyViewModelBase
    {
        public SymbolGroupViewModel()
        {
            Task.Factory.StartNew(() => { SymbolGroups = LoadGroups(); });
            CommandObjects.Add(new CommandObject(AddGroupCommand, "AddGroup"));
            CommandObjects.Add(new CommandObject(SaveCommand, "Save"));
            Task.Factory.StartNew(
                () => { Companies = StockService.GetInitDataCompanies().Select(p => p.Company).ToList(); });
        }

        private ObservableCollection<SymbolGroup> LoadGroups()
        {
            return StockService.GetSymbolGroups() ??
            new ObservableCollection<SymbolGroup>();
        }


        private ObservableCollection<SymbolGroup> _symbolGroups;

        private List<Company> _companies;
        private Company _selectedCompany;
        private RelayCommand _addCompanyCommand;
        private SymbolGroup _selectedSymbolGroup;
        private RelayCommand _addGroupCommand;
        private string _newGroupCaption;
        private RelayCommand _saveCommand;

        public ObservableCollection<SymbolGroup> SymbolGroups
        {
            get { return _symbolGroups; }
            set { Set(() => SymbolGroups, ref _symbolGroups, value); }
        }

        public List<Company> Companies
        {
            get { return _companies; }
            set { Set(() => Companies, ref _companies, value); }
        }

        public Company SelectedCompany
        {
            get { return _selectedCompany; }
            set { Set(() => SelectedCompany, ref _selectedCompany, value); }
        }

        public SymbolGroup SelectedSymbolGroup
        {
            get { return _selectedSymbolGroup; }
            set { Set(() => SelectedSymbolGroup, ref _selectedSymbolGroup, value); }
        }

        public RelayCommand AddCompanyCommand
        {
            get { return _addCompanyCommand ?? (_addCompanyCommand = new RelayCommand(AddCompany)); }
        }

        public RelayCommand AddGroupCommand
        {
            get { return _addGroupCommand ?? (_addGroupCommand = new RelayCommand(AddGroup, () => !string.IsNullOrWhiteSpace(NewGroupCaption))); }
        }

        public RelayCommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new RelayCommand(Save)); }
        }

        private void Save()
        {
            SerializeHelper.SerializeObject(SymbolGroups, StockService.GetFileName());
        }

        public string NewGroupCaption
        {
            get { return _newGroupCaption; }
            set { Set(() => NewGroupCaption, ref _newGroupCaption, value); AddGroupCommand.RaiseCanExecuteChanged(); }
        }

        private void AddGroup()
        {
            if (!string.IsNullOrWhiteSpace(NewGroupCaption))
                if (SymbolGroups.All(p => p.Caption != NewGroupCaption.Trim()))
                    SymbolGroups.Add(new SymbolGroup { Caption = NewGroupCaption });
            NewGroupCaption = string.Empty;
        }

        private void AddCompany()
        {
            if (SelectedCompany != null && SelectedSymbolGroup != null && SelectedSymbolGroup.Companies.All(p => p.StockId != SelectedCompany.StockId))
            {
                SelectedSymbolGroup.Companies.Add(SelectedCompany);
            }
        }
    }
}
