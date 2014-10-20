using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using ExchangeTracker.Domain;
using ExchangeTracker.Presentation.Common;
using ExchangeTracker.Presentation.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;

namespace ExchangeTracker.Presentation.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class OnlineTrackItemsViewModel : MyViewModelBase
    {
        public OnlineTrackItemsViewModel()
        {
            CommandObjects.Add(new CommandObject(RefreshRowCommand, "RefreshRow"));
            //            CommandObjects.Add(new CommandObject(RefreshStaticDataCommand, "RefreshStaticDataCommand"));
            LoadInitialData();
            _timer.Elapsed += (s, e) => { if (AutoRefresh) UpdateItems(); };
        }

        public ObservableCollection<SymbolGroup> SymbolGroups
        {
            get { return _symbolGroups; }
            set
            {
                var temp = value.FirstOrDefault(p => p != null && SelectedSymbolGroup != null && p.Caption == SelectedSymbolGroup.Caption);
                Set(() => SymbolGroups, ref _symbolGroups, value);
                SelectedSymbolGroup = temp;
            }
        }

        public SymbolGroup SelectedSymbolGroup
        {
            get { return _selectedSymbolGroup; }
            set
            {
                Set(() => SelectedSymbolGroup, ref _selectedSymbolGroup, value);
                SetGroupFilter();
            }
        }

        private void SetGroupFilter()
        {
            if (SelectedSymbolGroup != null)
                TrackItems =
                    new ObservableCollection<TrackItem>(
                        OrginalTrackItems.Where(
                            p => SelectedSymbolGroup.Companies.Select(q => q.StockId).Contains(p.Company.StockId)));
            else
                TrackItems = OrginalTrackItems;
        }

        public bool AutoRefresh
        {
            get { return _autoRefresh; }
            set { Set(() => AutoRefresh, ref _autoRefresh, value); }
        }

        private List<Task> updateListTasks;
        public override string Title
        {
            get { return string.Empty; }
        }

        readonly Timer _timer = new Timer(60000);
        private void LoadInitialData()
        {
            Task.Factory.StartNew(() =>
            {
                OrginalTrackItems = TrackItems = new ObservableCollection<TrackItem>(StockService.GetInitDataCompanies());
            }).ContinueWith(p => { UpdateItems(); _timer.Start(); });
        }

        public ObservableCollection<TrackItem> TrackItems
        {
            get { return _trackItems; }
            set { Set(() => TrackItems, ref _trackItems, value); }
        }

        public ObservableCollection<TrackItem> OrginalTrackItems
        {
            get { return _orginalTrackItems; }
            set { Set(() => OrginalTrackItems, ref _orginalTrackItems, value); }
        }

        public TrackItem CurrentTrackItem
        {
            get { return _currentTrackItem; }
            set { Set(() => CurrentTrackItem, ref _currentTrackItem, value); RefreshRowCommand.RaiseCanExecuteChanged(); }
        }

        private RelayCommand _refreshRowCommand;
        private RelayCommand _refreshStaticDataCommand;
        private ObservableCollection<TrackItem> _trackItems;
        private TrackItem _currentTrackItem;
        private bool _autoRefresh = true;
        private ObservableCollection<SymbolGroup> _symbolGroups;
        private SymbolGroup _selectedSymbolGroup;
        private ObservableCollection<TrackItem> _orginalTrackItems;

        public RelayCommand RefreshRowCommand
        {
            get
            {
                return _refreshRowCommand
                       ?? (_refreshRowCommand = new RelayCommand(() => StockService.RefreshTrackItem(CurrentTrackItem), () => CurrentTrackItem != null));
            }
        }
        public RelayCommand RefreshStaticDataCommand
        {
            get
            {
                return _refreshStaticDataCommand
                       ?? (_refreshStaticDataCommand = new RelayCommand(() => UpdateStaticItems()));
            }
        }

        private void UpdateStaticItems()
        {
            foreach (var trackItem in TrackItems)
            {
                StockService.UpdateStaticData(trackItem.Company.StockId);
            }
        }

        private void UpdateItems()
        {
            if (updateListTasks == null || updateListTasks.All(p => p.IsCompleted))
                updateListTasks = StockService.AsyncRefreshTrackItems(TrackItems);
        }

        public void NavigateCompanyHistory()
        {
            if (CurrentTrackItem != null)
            {
                NavigationManagert.NavigateTo(
                    SimpleIoc.Default.GetInstance<INavigation>("SymbolMultiTabView"));
                MessengerInstance.Send(CurrentTrackItem, "SymbolMultiTab");
            }
        }
    }
}