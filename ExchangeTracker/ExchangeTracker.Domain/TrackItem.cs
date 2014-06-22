using System;

namespace ExchangeTracker.Domain
{
    public class TrackItem : Entity
    {
        private Company _company;
        private int _buyRealVolume;
        private int _sellRealVolume;
        private int _buyLegalVolume;
        private int _sellLegalVolume;
        private int _buyRealCount;
        private int _sellRealCount;
        private int _buyLegalCount;
        private int _sellLegalCount;
        private decimal _lastTransactionPrice;
        private decimal _finalPrice;
        //        private decimal _firstPrice;
        private decimal _yesterdayPrice;
        private int _transactionCount;
        private int _transactionVolume;
        private decimal _transactionValue;
        //        private decimal _marketValue;
        //        private decimal _minDayPrice;
        //        private decimal _minWeekPrice;
        //        private decimal _minValidPrice;
        //        private decimal _minYearPrice;
        //        private decimal _maxDayPrice;
        //        private decimal _maxWeekPrice;
        //        private decimal _maxValidPrice;
        //        private decimal _maxYearPrice;
        private int _stocksCount;
        private int _baseVolume;
        private double _floatingStocks;
        private int _monthVolumeAvg;
        private string _lastTransactionDateTime;
        private char _statusId;
        private decimal _eps;
        private double _pe;
        //        private float _groupPe;
        private string _registerDateTime;

        public Company Company
        {
            get { return _company; }
            set { SetProperty(ref _company, value); }
        }

        /// <summary>
        /// حجم خرید CO
        /// </summary>
        public int BuyRealVolume
        {
            get { return _buyRealVolume; }
            set { SetProperty(ref _buyRealVolume, value); }
        }

        /// <summary>
        /// حجم فروش حقیقی
        /// </summary>
        public int SellRealVolume
        {
            get { return _sellRealVolume; }
            set { SetProperty(ref _sellRealVolume, value); }
        }

        /// <summary>
        /// حجم خرید حقوقی
        /// </summary>
        public int BuyLegalVolume
        {
            get { return _buyLegalVolume; }
            set { SetProperty(ref _buyLegalVolume, value); }
        }

        /// <summary>
        /// حجم فروش حقوقی
        /// </summary>
        public int SellLegalVolume
        {
            get { return _sellLegalVolume; }
            set { SetProperty(ref _sellLegalVolume, value); }
        }

        /// <summary>
        /// تعداد خرید حقیقی
        /// </summary>
        public int BuyRealCount
        {
            get { return _buyRealCount; }
            set { SetProperty(ref _buyRealCount, value); RaisePropertyChanged(() => SumBuyCount); }
        }

        /// <summary>
        /// تعداد فروش حقیقی
        /// </summary>
        public int SellRealCount
        {
            get { return _sellRealCount; }
            set { SetProperty(ref _sellRealCount, value); RaisePropertyChanged(() => SumSellCount); }
        }

        /// <summary>
        /// تعداد خرید حقوقی
        /// </summary>
        public int BuyLegalCount
        {
            get { return _buyLegalCount; }
            set { SetProperty(ref _buyLegalCount, value); RaisePropertyChanged(() => SumBuyCount); }
        }

        /// <summary>
        /// تعداد فروش حقوقی
        /// </summary>
        public int SellLegalCount
        {
            get { return _sellLegalCount; }
            set { SetProperty(ref _sellLegalCount, value); RaisePropertyChanged(() => SumSellCount); }
        }

        /// <summary>
        /// مجموع خرید
        /// </summary>
        public int SumBuyCount { get { return BuyRealCount + BuyLegalCount; } }

        /// <summary>
        /// مجموع فروش
        /// </summary>
        public int SumSellCount { get { return SellRealCount + SellLegalCount; } }


        /// <summary>
        /// قیمت آخرین معامله
        /// </summary>
        public decimal LastTransactionPrice
        {
            get { return _lastTransactionPrice; }
            set { SetProperty(ref _lastTransactionPrice, value); RaisePropertyChanged(() => LastTransactionPricePercent); }
        }
        /// <summary>
        /// درصد قیمت آخرین معامله
        /// </summary>
        public decimal LastTransactionPricePercent
        {
            get { return YesterdayPrice == 0 ? 0 : (LastTransactionPrice - YesterdayPrice) / YesterdayPrice; }
        }


        /// <summary>
        /// قیمت پایانی
        /// </summary>
        public decimal FinalPrice
        {
            get { return _finalPrice; }
            set { SetProperty(ref _finalPrice, value); RaisePropertyChanged(() => FinalPricePercent); }
        }

        /// <summary>
        /// قیمت پایانی
        /// </summary>
        public decimal FinalPricePercent
        {
            get { return YesterdayPrice == 0 ? 0 : (FinalPrice - YesterdayPrice) / YesterdayPrice; }
        }

        /// <summary>
        /// اولین قیمت
        /// </summary>
        //        public decimal FirstPrice
        //        {
        //            get { return _firstPrice; }
        //            set { SetProperty(ref _firstPrice, value); }
        //        }


        /// <summary>
        /// قیمت دیروز
        /// </summary>
        public decimal YesterdayPrice
        {
            get { return _yesterdayPrice; }
            set { SetProperty(ref _yesterdayPrice, value); RaisePropertyChanged(() => LastTransactionPricePercent); RaisePropertyChanged(() => FinalPricePercent); }
        }


        /// <summary>
        /// تعداد معاملات
        /// </summary>
        public int TransactionCount
        {
            get { return _transactionCount; }
            set { SetProperty(ref _transactionCount, value); }
        }


        /// <summary>
        /// حجم معاملات
        /// </summary>
        public int TransactionVolume
        {
            get { return _transactionVolume; }
            set { SetProperty(ref _transactionVolume, value); }
        }

        /// <summary>
        /// ارزش معاملات
        /// </summary>
        public decimal TransactionValue
        {
            get { return _transactionValue; }
            set { SetProperty(ref _transactionValue, value); }
        }

        /// <summary>
        /// ارزش بازار
        /// </summary>
        //        public decimal MarketValue
        //        {
        //            get { return _marketValue; }
        //            set { SetProperty(ref _marketValue, value); }
        //        }

        /// <summary>
        /// کمترین قیمت روز
        /// </summary>
        //        public decimal MinDayPrice
        //        {
        //            get { return _minDayPrice; }
        //            set { SetProperty(ref _minDayPrice, value); }
        //        }

        /// <summary>
        /// کمترین قیمت هفته
        /// </summary>
        //        public decimal MinWeekPrice
        //        {
        //            get { return _minWeekPrice; }
        //            set { SetProperty(ref  _minWeekPrice, value); }
        //        }

        /// <summary>
        /// کمترین قیمت سال
        /// </summary>
        //        public decimal MinYearPrice
        //        {
        //            get { return _minYearPrice; }
        //            set { SetProperty(ref _minYearPrice, value); }
        //        }

        /// <summary>
        /// کمترین قیمت مجاز
        /// </summary>
        //        public decimal MinValidPrice
        //        {
        //            get { return _minValidPrice; }
        //            set { SetProperty(ref _minValidPrice, value); }
        //        }

        /// <summary>
        /// بیشترین قیمت روز
        /// </summary>
        //        public decimal MaxDayPrice
        //        {
        //            get { return _maxDayPrice; }
        //            set { SetProperty(ref _maxDayPrice, value); }
        //        }

        /// <summary>
        /// بیشترین قیمت هفته
        /// </summary>
        //        public decimal MaxWeekPrice
        //        {
        //            get { return _maxWeekPrice; }
        //            set { SetProperty(ref  _maxWeekPrice, value); }
        //        }

        /// <summary>
        /// بیشترین قیمت سال
        /// </summary>
        //        public decimal MaxYearPrice
        //        {
        //            get { return _maxYearPrice; }
        //            set { SetProperty(ref _maxYearPrice, value); }
        //        }

        /// <summary>
        /// بیشترین قیمت مجاز
        /// </summary>
        //        public decimal MaxValidPrice
        //        {
        //            get { return _maxValidPrice; }
        //            set { SetProperty(ref _maxValidPrice, value); }
        //        }

        /// <summary>
        /// تعداد سهام
        /// </summary>
        public int StocksCount
        {
            get { return _stocksCount; }
            set { SetProperty(ref _stocksCount, value); }
        }

        /// <summary>
        /// حجم مبنا
        /// </summary>
        public int BaseVolume
        {
            get { return _baseVolume; }
            set { SetProperty(ref _baseVolume, value); }
        }

        /// <summary>
        /// سهام شناور
        /// </summary>
        public double FloatingStocks
        {
            get { return _floatingStocks; }
            set { SetProperty(ref _floatingStocks, value); }
        }

        /// <summary>
        /// میانگین حجم ماه
        /// </summary>
        public int MonthVolumeAvg
        {
            get { return _monthVolumeAvg; }
            set { SetProperty(ref _monthVolumeAvg, value); }
        }

        /// <summary>
        /// زمان آخرین معامله
        /// </summary>
        public string LastTransactionDateTime
        {
            get { return _lastTransactionDateTime; }
            set { SetProperty(ref  _lastTransactionDateTime, value); }
        }

        /// <summary>
        /// وضعیت
        /// </summary>
        public char StatusId
        {
            get { return _statusId; }
            set { SetProperty(ref  _statusId, value); }
        }

        /// <summary>
        /// EPS(پیش بینی سود)
        /// </summary>
        public decimal Eps
        {
            get { return _eps; }
            set { SetProperty(ref _eps, value); }
        }

        /// <summary>
        /// P/E(امید بازگشت سرمایه به سال)
        /// </summary>
        public double Pe
        {
            get { return _pe; }
            set { SetProperty(ref _pe, value); }
        }

        /// <summary>
        /// گروه P/E (امید بازگشت سرمایه گروه به سال)
        /// </summary>
        //        public float GroupPe
        //        {
        //            get { return _groupPe; }
        //            set { SetProperty(ref _groupPe, value); }
        //        }

        /// <summary>
        /// زمان ثبت آیتم
        /// </summary>
        public string RegisterDateTime
        {
            get { return _registerDateTime; }
            set { SetProperty(ref _registerDateTime, value); }
        }
    }
}
