namespace ExchangeTracker.Domain
{
    public class Company : Entity
    {
        private string _stockId;
        private string _stockCode;
        private string _symbol;
        private string _caption;
        private int _cid;
        private int _groupId;
        private double _defaultFloatingStocks;

        public string StockId
        {
            get { return _stockId; }
            set { SetProperty(ref _stockId, value); }
        }

        public string Symbol
        {
            get { return _symbol; }
            set { SetProperty(ref _symbol, value); }
        }

        public string Caption
        {
            get { return _caption; }
            set { SetProperty(ref  _caption, value); }
        }

        public int Cid
        {
            get { return _cid; }
            set { SetProperty(ref _cid, value); }
        }

        public int GroupId
        {
            get { return _groupId; }
            set { SetProperty(ref _groupId, value); }
        }

        public double DefaultFloatingStocks
        {
            get { return _defaultFloatingStocks; }
            set { SetProperty(ref  _defaultFloatingStocks, value); }
        }
    }
}
