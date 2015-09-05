using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExchangeTracker.Domain;

namespace ExchangeTracker.Presentation.Model
{
    public static class StaticReference
    {
        private const string FileSettingPath = @"C:\ExchangeTracker\Settings.txt";
        private const string IntervalString = "Interval";

        static StaticReference()
        {
            _interval = int.Parse(GetSetting(IntervalString) ?? "3");
        }

        private static string GetSetting(string key)
        {
            var settings = ReadSettingsToDictionary();
            string result;
            settings.TryGetValue(key, out result);
            return result;
        }
        private static Dictionary<string, string> ReadSettingsToDictionary()
        {
            if (!File.Exists(FileSettingPath))
                return new Dictionary<string, string>();
            return File.ReadAllLines(FileSettingPath).Select(p => p.Split(new[] { @"@@#$%@@" }, StringSplitOptions.None))
                .Where(p => p.Length == 2)
                .ToDictionary(p => p[0], p => p[1]);
        }

        static void WriteSettings(Dictionary<string, string> dic)
        {
            File.WriteAllLines(FileSettingPath, dic.Select(p => p.Key + @"@@#$%@@" + p.Value).ToArray());
        }

        private static int _interval;
        public static int Interval
        {
            get { return _interval; }
            set
            {
                _interval = value;
                SetInterval(value);
            }
        }

        private static void SetInterval(int value)
        {
            var settings = ReadSettingsToDictionary();
            settings[IntervalString] = value.ToString(CultureInfo.InvariantCulture);
            WriteSettings(settings);
        }

        public static TrackItemModel FromTrackItem(TrackItem item)
        {
            return new TrackItemModel
            {
                Id = Guid.NewGuid(),
                BuyLegalCount = item.BuyLegalCount,
                BuyLegalVolume = item.BuyLegalVolume,
                BuyRealCount = item.BuyRealCount,
                BuyRealVolume = item.BuyRealVolume,
                FinalPrice = item.FinalPrice,
                LastTransactionPrice = item.LastTransactionPrice,
                RegisterDateTime = item.RegisterDateTime,
                SellLegalCount = item.SellLegalCount,
                SellLegalVolume = item.SellLegalVolume,
                SellRealCount = item.SellRealCount,
                SellRealVolume = item.SellRealVolume,
                StockId = item.Company.StockId,
                TransactionCount = item.TransactionCount,
                TransactionValue = item.TransactionValue,
                TransactionVolume = item.TransactionVolume,
                LastTransactionDateTime = item.LastTransactionDateTime
            };
        }

    }
}
