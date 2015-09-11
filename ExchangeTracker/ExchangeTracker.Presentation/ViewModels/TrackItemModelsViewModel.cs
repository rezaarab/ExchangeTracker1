using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeTracker.Domain;
using ExchangeTracker.Presentation.Common;
using ExchangeTracker.Presentation.Services;
using Timer = System.Timers.Timer;

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
    public class TrackItemModelsViewModel : MyViewModelBase
    {
        public TrackItemModelsViewModel()
        {
        }

        public override string Title
        {
            get { return string.Empty; }
        }

        private string _stockId;
        public string StockId
        {
            get { return _stockId; }
            set { Set(() => StockId, ref _stockId, value); }
        }

        public DateTime Date
        {
            get { return _date; }
            set { Set(() => Date, ref _date, value); }
        }

        Timer _timer;
        public void LoadInitialData()
        {
            _timer = new Timer(Interval * 60 * 1000);
            _timer.Elapsed += (s, e) => UpdateItems();
            UpdateItems();
            _timer.Start();
        }

        public ObservableCollection<TrackItemModel> TrackItemModels
        {
            get { return _trackItemModels; }
            set { Set(() => TrackItemModels, ref _trackItemModels, value); }
        }

        public TrackItemModel CurrentTrackItemModel
        {
            get { return _currentTrackItemModel; }
            set { Set(() => CurrentTrackItemModel, ref _currentTrackItemModel, value); }
        }

        private ObservableCollection<TrackItemModel> _trackItemModels = new ObservableCollection<TrackItemModel>();
        private TrackItemModel _currentTrackItemModel;
        private DateTime _date;

        public int Interval { get; set; }
        public TrackItem InputTrackItem { get; set; }

        private void UpdateItems()
        {
            var current = CurrentTrackItemModel == null ? TimeSpan.Zero : CurrentTrackItemModel.TimeSpan;
            var isReturn = false;
            if (InputTrackItem != null)
                Task.Factory.StartNew(() => StockService.RefreshTrackItem(InputTrackItem)).ContinueWith(p => isReturn = true);
            if (!isReturn)
                Thread.Sleep(TimeSpan.FromMilliseconds(50));
            var companyTrackItemModels = DataService.GetCompanyTrackItemModels(StockId, Date, Interval);
            TrackItemModels.Clear();
            companyTrackItemModels
                //                .Where(p => p.TimeSpan > TrackItemModels.DefaultIfEmpty(new TrackItemModel { TimeSpan = TimeSpan.Zero }).Max(q => q.TimeSpan))
                .ToList()
                .ForEach(p => TrackItemModels.Add(p));
            CurrentTrackItemModel = TrackItemModels.FirstOrDefault(p => p.TimeSpan == current);
        }

        public override bool NavigateExit()
        {
            _timer.Stop();
            return true;
        }
    }
}