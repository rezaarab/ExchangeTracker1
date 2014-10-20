using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeTracker.Domain
{
    [Serializable]
    public class SymbolGroup : Entity
    {
        private string _caption;
        private ObservableCollection<Company> _companies = new ObservableCollection<Company>();

        public string Caption
        {
            get { return _caption; }
            set { SetProperty(ref _caption, value); }
        }

        public ObservableCollection<Company> Companies
        {
            get { return _companies; }
            set { SetProperty(ref _companies, value); }
        }
    }
}
