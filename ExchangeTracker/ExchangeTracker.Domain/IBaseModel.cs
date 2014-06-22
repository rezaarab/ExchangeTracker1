using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeTracker.Domain
{
    public interface IBaseModel : IDataErrorInfo
    {
        Guid Id { get; set; }

        Dictionary<string, string> GetErrors();
    }
}
