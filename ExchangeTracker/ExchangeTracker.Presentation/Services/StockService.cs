using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using ExchangeTracker.Domain;
using ExchangeTracker.Presentation.Common;
using FarsiLibrary.FX.Utils;

namespace ExchangeTracker.Presentation.Services
{
    public static class StockService
    {
        static StockService()
        {
            OfflineStaticData = File.Exists(StaticDataFilePath) ? File.ReadAllLines(StaticDataFilePath).ToList() : Enumerable.Empty<string>().ToList();
        }

        private static readonly List<string> OfflineStaticData;
        private static readonly string StaticDataFilePath = Path.Combine(AppHelper.AppDataPath, "staticdata.txt");

        public static void SaveStaticData()
        {
            File.WriteAllLines(StaticDataFilePath, OfflineStaticData);
        }
        public static IEnumerable<TrackItem> GetInitDataCompanies()
        {
            //            var namadha = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "Assets\\MarketWatchPlus.txt"));
            var namadha = ReadAllText("http://www.tsetmc.com/tsev2/data/MarketWatchPlus.aspx");
            //            File.WriteAllText(Path.Combine(Environment.CurrentDirectory, string.Format("{0}.txt", Guid.NewGuid())), namadha, Encoding.UTF8);
            var companiesArray =
                namadha.Split(';').Select(p => p.Split(',')).Where(p =>
                {
                    bool b = p.GetSafe(1).StartsWith("IR");
                    return b;
                }).ToArray();
            return companiesArray.Select(p => new TrackItem
            {
                Id = Guid.NewGuid(),
                Company = new Company
                {
                    Id = Guid.NewGuid(),
                    Caption = p.GetSafe(3),
                    //                    StockCode = p.GetSafe(1],
                    StockId = p.GetSafe(0),
                    Symbol = p.GetSafe(2),
                    Cid = p.GetSafe(18).ToType<int>(),
                    GroupId = p.GetSafe(17).ToType<int>(),
                },
                TransactionCount = p.GetSafe(8).ToType<int>(),
                TransactionVolume = p.GetSafe(9).ToType<int>(),
                TransactionValue = p.GetSafe(10).ToType<decimal>(),
                YesterdayPrice = p.GetSafe(13).ToType<decimal>(),
                //                FirstPrice = p.GetSafe(5).ToType<decimal>(),
                LastTransactionPrice = p.GetSafe(7).ToType<decimal>(),
                FinalPrice = p.GetSafe(6).ToType<decimal>(),
                //                MinDayPrice = p.GetSafe(11).ToType<decimal>(),
                //                MaxDayPrice = p.GetSafe(12).ToType<decimal>(),
                Eps = p.GetSafe(14).ToType<int>(),
            }).ToArray();
        }

        private static string ReadAllText(string url)
        {
            string html;
            using (var wc = new GZipWebClient())
            {
                //wc.Headers.Add("User-Agent","Mozilla/5.0 (Windows NT 6.3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/34.0.1847.11 Safari/537.36");
                var bytes = wc.DownloadData(url);
                html = Encoding.UTF8.GetString(bytes);
            }

            return html;
        }

        public static string GetInfoData(string stockId, int cid)
        {
            // Grab the content of the first script element
            string url = "http://www.tsetmc.com/tsev2/data/instinfodata.aspx?i=" + stockId + "&c=" + cid;
            string html;
            using (var wc = new GZipWebClient())
                html = wc.DownloadString(url);
            return html;
        }

        public static IEnumerable<TrackItem> GetCompanyHistory(TrackItem trackItem)
        {
            var namadha = ReadAllText("http://www.tsetmc.com/tsev2/data/clienttype.aspx?i=" + trackItem.Company.StockId + "&c=" + trackItem.Company.Cid);
            var companiesArray =
                namadha.Split(';').Select(p => p.Split(',')).Where(p =>
                {
                    return true;
                }).ToArray();
            return companiesArray.OrderByDescending(p => p.GetSafe(0)).Select(p => new TrackItem
            {
                Id = Guid.NewGuid(),
                Company = new Company
                {
                    Id = Guid.NewGuid(),
                    Caption = trackItem.Company.Caption,
                    StockId = trackItem.Company.StockId,
                    Symbol = trackItem.Company.Symbol,
                    Cid = trackItem.Company.Cid,
                    GroupId = trackItem.Company.GroupId,
                },
                LastTransactionDateTime = new PersianDate(DateTime.Parse(string.Format("{0}/{1}/{2}",
                    p.GetSafe(0).Substring(0, 4),
                    p.GetSafe(0).Substring(4, 2),
                    p.GetSafe(0).Substring(6, 2)))).ToString(""),
                BuyLegalVolume = p.GetSafe(6).ToType<int>(),
                BuyRealVolume = p.GetSafe(5).ToType<int>(),
                SellLegalVolume = p.GetSafe(8).ToType<int>(),
                SellRealVolume = p.GetSafe(7).ToType<int>(),
                BuyLegalCount = p.GetSafe(2).ToType<int>(),
                BuyRealCount = p.GetSafe(1).ToType<int>(),
                SellLegalCount = p.GetSafe(4).ToType<int>(),
                SellRealCount = p.GetSafe(3).ToType<int>(),
            }).ToArray();
        }


        public static List<Task> AsyncRefreshTrackItems(IEnumerable<TrackItem> items)
        {
            //            Parallel.ForEach(items, item =>
            //            {
            //                var infoData = GetInfoData(item.Company.StockId, item.Company.Cid);
            //                var infoDataArray = infoData == null
            //                    ? null
            //                    : infoData.Split(';').Select(p => p.Split(',')).ToArray();
            //                var eData = infoDataArray == null ? null : infoDataArray[4];
            //                var oV = infoDataArray == null ? null : infoDataArray[0];
            //                SetTrackItemValues(item, eData, oV);
            //            });
            var trackItems = items as TrackItem[] ?? items.ToArray();
            var tasks = trackItems.Select(item => Task<string[][]>.Factory.StartNew(() =>
            {
                var infoData = GetInfoData(item.Company.StockId, item.Company.Cid);
                var infoDataArray = infoData == null
                    ? null
                    : infoData.Split(';').Select(p => p.Split(',')).ToArray();
                return infoDataArray;
            }).ContinueWith(result =>
            {
                var infoDataArray = result.Result;
                var eData = infoDataArray == null ? null : infoDataArray[4];
                var oV = infoDataArray == null ? null : infoDataArray[0];

                string staticValues = GetOfflineStaticValues(item.Company.StockId);
                if (string.IsNullOrEmpty(staticValues))
                {
                    staticValues = GetOnlineStaticValues(item.Company.StockId);
                    OfflineStaticData.Add(string.Format("{0},{1}", item.Company.StockId, staticValues));
                }
                var staticData = string.IsNullOrEmpty(staticValues) ? null : staticValues.Split(',');

                SetTrackItemValues(item, eData, oV, staticData);
            })).ToList();
            return tasks;
        }

        public static void RefreshTrackItem(TrackItem item)
        {
            if (item == null)
                return;
            var infoData = GetInfoData(item.Company.StockId, item.Company.Cid);
            var infoDataArray = infoData == null ? null : infoData.Split(';').Select(p => p.Split(',')).ToArray();
            var eData = infoDataArray == null ? null : infoDataArray[4];
            var oV = infoDataArray == null ? null : infoDataArray[0];

            string staticValues = GetOfflineStaticValues(item.Company.StockId);
            if (string.IsNullOrEmpty(staticValues))
            {
                staticValues = GetOnlineStaticValues(item.Company.StockId);
                OfflineStaticData.Add(string.Format("{0},{1}", item.Company.StockId, staticValues));
            }
            var staticData = string.IsNullOrEmpty(staticValues) ? null : staticValues.Split(',');

            SetTrackItemValues(item, eData, oV, staticData);
        }

        public static void UpdateStaticData(string stockId)
        {
            var staticValues = GetOnlineStaticValues(stockId);
            var index = OfflineStaticData.FindIndex(p => p.Split(new[] { ',' })[0] == stockId);
            if (index < 0)
                OfflineStaticData.Add(string.Format("{0},{1}", stockId, staticValues));
            else
                OfflineStaticData[index] = string.Format("{0},{1}", stockId, staticValues);
        }

        private static void SetTrackItemValues(TrackItem item, string[] eData, string[] oV, string[] staticData)
        {
            item.RegisterDateTime = PersianDateConverter.ToPersianDate(DateTime.Now).ToString("G");
            item.FloatingStocks = item.Company.DefaultFloatingStocks;

            if (eData != null && eData.Count() > 8)
            {
                item.BuyLegalCount = eData[6].ToType<int>();
                item.BuyLegalVolume = eData[1].ToType<int>();
                item.BuyRealCount = eData[5].ToType<int>();
                item.BuyRealVolume = eData[0].ToType<int>();
                item.SellLegalCount = eData[6].ToType<int>();
                item.SellLegalVolume = eData[4].ToType<int>();
                item.SellRealCount = eData[8].ToType<int>();
                item.SellRealVolume = eData[3].ToType<int>();
            }

            if (oV != null && oV.Count() > 10)
            {
                item.FinalPrice = oV.GetSafe(3).ToType<decimal>();
                //                item.FirstPrice = oV.GetSafe(4).ToType<decimal>();
                item.LastTransactionDateTime = oV.GetSafe(0);
                item.LastTransactionPrice = oV.GetSafe(2).ToType<decimal>();
                //                item.MarketValue = staticData != null && staticData.Count() > 1
                //                    ? oV.GetSafe(3).ToType<int>() * staticData.GetSafe(1).ToType<int>()
                //                    : 0;
                //                item.MaxDayPrice = oV.GetSafe(7).ToType<decimal>();
                //                item.MinDayPrice = oV.GetSafe(6).ToType<decimal>();
                item.StatusId = (oV.GetSafe(1) ?? string.Empty).ToCharArray().GetSafe(0);
                item.TransactionCount = oV.GetSafe(8).ToType<int>();
                item.TransactionValue = oV.GetSafe(10).ToType<decimal>();
                item.TransactionVolume = oV.GetSafe(9).ToType<int>();
                item.YesterdayPrice = oV.GetSafe(5).ToType<decimal>();
            }

            SetStaticData(item, staticData);
        }

        private static void SetStaticData(TrackItem item, string[] staticData)
        {
            if (staticData != null && staticData.Count() > 9)
            {
                item.BaseVolume = staticData.GetSafe(0).ToType<int>();
                item.FloatingStocks = staticData.GetSafe(8).ToType<int>();
                //                item.MaxValidPrice = staticData.GetSafe(2).ToType<decimal>();
                //                item.MaxWeekPrice = staticData.GetSafe(4).ToType<decimal>();
                //                item.MaxYearPrice = staticData.GetSafe(6).ToType<decimal>();
                //                item.MinValidPrice = staticData.GetSafe(3).ToType<decimal>();
                //                item.MinWeekPrice = staticData.GetSafe(5).ToType<decimal>();
                //                item.MinYearPrice = staticData.GetSafe(7).ToType<decimal>();
                item.MonthVolumeAvg = staticData.GetSafe(9).ToType<int>();
                item.StocksCount = staticData.GetSafe(1).ToType<int>();
            }
        }

        public static string GetOfflineStaticValues(string stockId)
        {
            var str = OfflineStaticData.FirstOrDefault(p => p.Split(new[] { ',' })[0] == stockId);
            if (string.IsNullOrEmpty(str))
                return str;
            return string.Join(",", str.Split(new[] { ',' }).Skip(1));
        }

        public static string GetOnlineStaticValues(string stockId)
        {
            // Grab the content of the first script element
            string url = "http://www.tsetmc.com/Loader.aspx?ParTree=151311&i=" + stockId;
            string html;
            using (var wc = new GZipWebClient())
                html = wc.DownloadString(url);
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            var script = doc
                .DocumentNode
                .Descendants()
                .First(n => n.Name == "script" && n.InnerText.Contains("LVal18AFC"))
                .InnerText;

            // Return the data of spect and stringify it into a proper JSON object
            var engine = new Jurassic.ScriptEngine();
            var fileds = new[]
            {
                "BaseVol.toString()",           //0
                "ZTitad.toString()",            //1
                "PSGelStaMax.toString()",       //2
                "PSGelStaMin.toString()",       //3
                "MaxWeek.toString()",           //4
                "MinWeek.toString()",           //5
                "MaxYear.toString()",           //6
                "MinYear.toString()",           //7
                "KAjCapValCpsIdx.toString()",   //8
                "QTotTran5JAvg.toString()"      //9
            };
            string code = "(function() { " + script + " var jarray=[" + string.Join(",", fileds) + "]; return jarray.join();})()";
            return engine.Evaluate(code).ToString();
            //            var json = JSONObject.Stringify(engine, result);
        }


    }
    public class GZipWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = (HttpWebRequest)base.GetWebRequest(address);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            return request;
        }
    }

}
