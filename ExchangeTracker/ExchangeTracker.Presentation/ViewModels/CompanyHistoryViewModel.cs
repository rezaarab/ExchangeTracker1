using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using ExchangeTracker.Domain;
using ExchangeTracker.Presentation.Common;
using ExchangeTracker.Presentation.Model;
using ExchangeTracker.Presentation.Services;
using ExchangeTracker.Presentation.Views;
using FarsiLibrary.FX.Utils;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using PersianCalendar = FarsiLibrary.FX.Utils.PersianCalendar;

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
    public class CompanyHistoryViewModel : MyViewModelBase
    {
        public CompanyHistoryViewModel()
        {
            //            MessengerInstance.Register<TrackItem>(this, "CompanyHistory", trackItem =>
            //            {
            //                TrackItem = trackItem;
            //                Title = trackItem.Company.Symbol;
            //                LoadData();
            //            });
            _timer.Elapsed += (s, e) => StockService.RefreshTrackItem(TrackItem);
        }

        readonly Timer _timer = new Timer(10000);
        public void LoadData()
        {
            Task.Factory.StartNew(() =>
            {
                TrackItems = new ObservableCollection<TrackItem>(new[] { TrackItem }.Concat(StockService.GetCompanyHistory(TrackItem)));
            }).ContinueWith(p => _timer.Start());
        }

        public ObservableCollection<TrackItem> TrackItems
        {
            get { return _trackItems; }
            set { Set(() => TrackItems, ref _trackItems, value); }
        }

        public TrackItem CurrentTrackItem
        {
            get { return _currentTrackItem; }
            set { Set(() => CurrentTrackItem, ref _currentTrackItem, value); }
        }

        public TrackItem TrackItem
        {
            get { return _trackItem; }
            set { Set(() => TrackItem, ref _trackItem, value); }
        }

        private ObservableCollection<TrackItem> _trackItems;
        private TrackItem _currentTrackItem;
        private TrackItem _trackItem;
        private string _title;

        public override bool NavigateExit()
        {
            _timer.Stop();
            return true;
        }
        public void NavigateTrackHistory()
        {
            if (CurrentTrackItem != null)
            {
                if (CurrentTrackItem.LastTransactionDateTime == default(DateTime))
                {
                    CurrentTrackItem.LastTransactionDateTime = AppHelper.ConvertPersianToEnglish(CurrentTrackItem.LastTransactionTime).Date;
                }
                var viewModel = new TrackItemModelsViewModel
                {
                    StockId = CurrentTrackItem.Company.StockId,
                    Date = CurrentTrackItem.LastTransactionDateTime,
                    Interval = StaticReference.Interval,
                };
                var menuCommandObject = new MenuCommandObject(new TrackItemModelsView
                {
                    DataContext = viewModel
                }, CurrentTrackItem.Company.Symbol);
                viewModel.LoadInitialData();
                ModalDialogHelper.Show(menuCommandObject, true);
                ((UserControlBase)menuCommandObject.Navigator).OnUnloaded();
            }
        }
    }
}