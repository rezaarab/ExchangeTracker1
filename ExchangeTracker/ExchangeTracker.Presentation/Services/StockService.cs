using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Media.Animation;
using System.Xml;
using DevExpress.Data.PLinq.Helpers;
using DevExpress.Xpf.Editors.Helpers;
using ExchangeTracker.Domain;
using ExchangeTracker.Presentation.Common;
using ExchangeTracker.Presentation.Model;
using ExchangeTracker.Presentation.ViewModels;
using FarsiLibrary.FX.Utils;
using HtmlAgilityPack;
using Jurassic;

namespace ExchangeTracker.Presentation.Services
{
    public static class StockService
    {
        static StockService()
        {
            _trackItemHistories = new ConcurrentBag<TrackItemModel>(DataService.GetTodaySavedItems());
            OfflineStaticData = File.Exists(StaticDataFilePath) ? File.ReadAllLines(StaticDataFilePath).ToList() : Enumerable.Empty<string>().ToList();
        }

        private static readonly List<string> OfflineStaticData;
        private static readonly string StaticDataFilePath = Path.Combine(AppHelper.AppDataPath, "staticdata" + AppHelper.AppStaticDataVersion + ".txt");

        public static void SaveStaticData()
        {
            File.WriteAllLines(StaticDataFilePath, OfflineStaticData);
        }
        static readonly object Lck = new object();
        private static List<TrackItem> _initDataCompanies;
        private static readonly ConcurrentBag<TrackItemModel> _trackItemHistories = new ConcurrentBag<TrackItemModel>();

        public static IEnumerable<TrackItem> GetInitDataCompanies()
        {
            lock (Lck)
            {
                if (_initDataCompanies != null)
                    return _initDataCompanies;
                //            var namadha = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "Assets\\MarketWatchPlus.txt"));
                var namadha = ReadAllText("http://www.tsetmc.com/tsev2/data/MarketWatchPlus.aspx");
                //            File.WriteAllText(Path.Combine(Environment.CurrentDirectory, string.Format("{0}.txt", Guid.NewGuid())), namadha, Encoding.UTF8);
                var companiesArray =
                    namadha.Split(';').Select(p => p.Split(',')).Where(p =>
                    {
                        bool b = p.GetSafe(1).StartsWith("IR");
                        return b;
                    }).ToArray();
                var items = companiesArray.Select(p => new TrackItem
                {
                    Id = Guid.NewGuid(),
                    Company = new Company
                    {
                        Id = Guid.NewGuid(),
                        Caption = p.GetSafe(3),
                        StockCode = p.GetSafe(1),
                        StockId = p.GetSafe(0),
                        Symbol = p.GetSafe(2),
                        Cid = p.GetSafe(18).ToType<int>(),
                        GroupId = p.GetSafe(17).ToType<int>(),
                    },
                    TransactionCount = p.GetSafe(8).ToType<decimal>(),
                    TransactionVolume = p.GetSafe(9).ToType<decimal>(),
                    TransactionValue = p.GetSafe(10).ToType<decimal>(),
                    YesterdayPrice = p.GetSafe(13).ToType<decimal>(),
                    //                FirstPrice = p.GetSafe(5).ToType<decimal>(),
                    LastTransactionPrice = p.GetSafe(7).ToType<decimal>(),
                    FinalPrice = p.GetSafe(6).ToType<decimal>(),
                    //                MinDayPrice = p.GetSafe(11).ToType<decimal>(),
                    //                MaxDayPrice = p.GetSafe(12).ToType<decimal>(),
                    Eps = p.GetSafe(14).ToType<decimal>(),
                }).ToList();
                try
                {
                    var otherHtml = ReadAllText("http://www.tsetmc.com/Loader.aspx?ParTree=111C1411")
                        .Replace("<tr\">", "<tr>");
                    var doc = new HtmlDocument();
                    doc.LoadHtml(otherHtml);
                    var trs = doc.GetElementbyId("tblToGrid").SelectNodes("tr").Skip(1).Select(
                        p => new Tuple<string, string, string>(
                            p.SelectNodes("td")[0].InnerText.Split(new[] { ',' })[0],
                            p.SelectNodes("td")[0].InnerText.Split(new[] { ',' })[1],
                            p.SelectNodes("td")[3].InnerText)).ToArray();
                    var enumerable = items.Select(q => q.Company.StockId).ToArray();
                    var otherItems = trs.Where(p => !enumerable.Contains(p.Item1)).ToList();
                    otherItems.ForEach(p => items.Add(new TrackItem
                    {
                        Id = Guid.NewGuid(),
                        Company = new Company
                        {
                            Id = Guid.NewGuid(),
                            StockId = p.Item1,
                            Symbol = p.Item2,
                        },
                        StatusId = p.Item3,
                    }));
                }
                catch (Exception ex) { ExceptionHelper.ReportException(ex, "Load status of symbols"); }
                return _initDataCompanies = items;
            }
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
            var hahoHistories = ReadAllText("http://www.tsetmc.com/tsev2/data/clienttype.aspx?i=" + trackItem.Company.StockId + "&c=" + trackItem.Company.Cid);
            var hahoHistoryArray =
                hahoHistories.Split(';').Select(p => p.Split(',')).Where(p =>
                {
                    return true;
                }).ToArray();
            var trackItems = hahoHistoryArray.OrderByDescending(p => p.GetSafe(0)).Where(p => !String.IsNullOrWhiteSpace(p.GetSafe(0))).Select(p => new TrackItem
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
                LastTransactionTime = new PersianDate(DateTime.Parse(String.Format("{0}/{1}/{2}",
                    p.GetSafe(0).Substring(0, 4),
                    p.GetSafe(0).Substring(4, 2),
                    p.GetSafe(0).Substring(6, 2)))).ToString(""),
                BuyLegalVolume = p.GetSafe(6).ToType<decimal>(),
                BuyRealVolume = p.GetSafe(5).ToType<decimal>(),
                SellLegalVolume = p.GetSafe(8).ToType<decimal>(),
                SellRealVolume = p.GetSafe(7).ToType<decimal>(),
                BuyLegalCount = p.GetSafe(2).ToType<decimal>(),
                BuyRealCount = p.GetSafe(1).ToType<decimal>(),
                SellLegalCount = p.GetSafe(4).ToType<decimal>(),
                SellRealCount = p.GetSafe(3).ToType<decimal>(),
            }).ToArray();
            var histories = ReadAllText("http://www.tsetmc.com/tsev2/data/InstTradeHistory.aspx?i=" + trackItem.Company.StockId + "&Top=" + 999 + "&A=1");
            var nodes = histories.Split(new[] { ';' }).Select(p => p.Split(new[] { '@' })).ToArray();
            var telorance = nodes.GroupBy(p => p.Count()).Max(p => p.Key);
            foreach (var node in nodes.Where(p => p.Count() == telorance).ToArray())
            {
                var date = node[0];
                var pDate = new PersianDate(DateTime.Parse(String.Format("{0}/{1}/{2}",
                    date.Substring(0, 4),
                    date.Substring(4, 2),
                    date.Substring(6, 2)))).ToString("");
                var track = trackItems.FirstOrDefault(p => p.LastTransactionTime == pDate);
                if (track != null)
                {
                    track.FinalPrice = node[3].ToType<decimal>();
                    track.YesterdayPrice = node[6].ToType<decimal>();
                    track.LastTransactionPrice = node[4].ToType<decimal>();
                }
            }

            //            var xmlDoc = new XmlDocument();
            //            xmlDoc.LoadXml(histories);
            //            var nodes = xmlDoc.SelectNodes("rows/row");
            //            foreach (XmlNode node in nodes)
            //            {
            //                var date = node.Attributes["id"].Value;
            //                var pDate = new PersianDate(DateTime.Parse(String.Format("{0}/{1}/{2}",
            //                    date.Substring(0, 4),
            //                    date.Substring(4, 2),
            //                    date.Substring(6, 2)))).ToString("");
            //                var track = trackItems.FirstOrDefault(p => p.LastTransactionDateTime == pDate);
            //                if (track != null)
            //                {
            //                    track.FinalPrice = node.ChildNodes[5].InnerText.ToType<decimal>();//4
            //                    track.YesterdayPrice = node.ChildNodes[10].InnerText.ToType<decimal>();//9
            //                    track.LastTransactionPrice = node.ChildNodes[8].InnerText.ToType<decimal>();//7
            //                }
            //                else
            //                {
            //
            //                }
            //            }
            return trackItems.ToArray();
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
                if (infoDataArray.Length <= 4)
                    return;
                var eData = infoDataArray == null ? null : infoDataArray[4];
                var oV = infoDataArray == null ? null : infoDataArray[0];

                string staticValues = GetOfflineStaticValues(item.Company.StockId);
                if (String.IsNullOrEmpty(staticValues))
                {
                    staticValues = GetOnlineStaticValues(item.Company.StockId);
                    AddToOfflineData(String.Format("{0},{1}", item.Company.StockId, staticValues));
                }
                var staticData = String.IsNullOrEmpty(staticValues) ? null : staticValues.Split(',');

                SetTrackItemValues(item, eData, oV, staticData);
            })).ToList();
            return tasks;
        }

        private static void AddToOfflineData(string value)
        {
            lock (Lck)
            {
                OfflineStaticData.Add(value);
            }
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
            if (String.IsNullOrEmpty(staticValues))
            {
                staticValues = GetOnlineStaticValues(item.Company.StockId);
                AddToOfflineData(String.Format("{0},{1}", item.Company.StockId, staticValues));
            }
            var staticData = String.IsNullOrEmpty(staticValues) ? null : staticValues.Split(',');

            SetTrackItemValues(item, eData, oV, staticData);
        }

        public static void UpdateStaticData(string stockId)
        {
            var staticValues = GetOnlineStaticValues(stockId);
            var index = OfflineStaticData.FindIndex(p => p.Split(new[] { ',' })[0] == stockId);
            if (index < 0)
                AddToOfflineData(String.Format("{0},{1}", stockId, staticValues));
            else
                OfflineStaticData[index] = String.Format("{0},{1}", stockId, staticValues);
        }

        private static void SetTrackItemValues(TrackItem item, string[] eData, string[] oV, string[] staticData)
        {
            var now = DateTime.Now;
            item.RegisterDateTime = now;
            item.RegisterDateTimeStr = PersianDateConverter.ToPersianDate(now).ToString("G");
            item.FloatingStocks = item.Company.DefaultFloatingStocks;

            if (eData != null && eData.Count() > 8)
            {
                item.BuyLegalCount = eData[6].ToType<decimal>();
                item.BuyLegalVolume = eData[1].ToType<decimal>();
                item.BuyRealCount = eData[5].ToType<decimal>();
                item.BuyRealVolume = eData[0].ToType<decimal>();
                item.SellLegalCount = eData[9].ToType<decimal>();
                item.SellLegalVolume = eData[4].ToType<decimal>();
                item.SellRealCount = eData[8].ToType<decimal>();
                item.SellRealVolume = eData[3].ToType<decimal>();
            }

            if (oV != null && oV.Count() > 10)
            {
                item.FinalPrice = oV.GetSafe(3).ToType<decimal>();
                //                item.FirstPrice = oV.GetSafe(4).ToType<decimal>();
                item.LastTransactionTime = oV.GetSafe(0);
                item.LastTransactionPrice = oV.GetSafe(2).ToType<decimal>();
                TimeSpan ouTimeSpan = TimeSpan.Zero;
                TimeSpan.TryParse(oV.GetSafe(0), out ouTimeSpan);
                item.LastTransactionDateTime = oV.GetSafe(12).Insert(4, "/").Insert(7, "/").ToType<DateTime>().Add(ouTimeSpan);
                //                item.MarketValue = staticData != null && staticData.Count() > 1
                //                    ? oV.GetSafe(3).ToType<decimal>() * staticData.GetSafe(1).ToType<decimal>()
                //                    : 0;
                //                item.MaxDayPrice = oV.GetSafe(7).ToType<decimal>();
                //                item.MinDayPrice = oV.GetSafe(6).ToType<decimal>();
                item.StatusId = (oV.GetSafe(1) ?? String.Empty);
                item.TransactionCount = oV.GetSafe(8).ToType<decimal>();
                item.TransactionValue = oV.GetSafe(10).ToType<decimal>();
                item.TransactionVolume = oV.GetSafe(9).ToType<decimal>();
                item.YesterdayPrice = oV.GetSafe(5).ToType<decimal>();
                if (staticData != null && staticData.Length >= 10 && staticData.GetSafe(10).ToType<decimal>() > 0)
                    item.Pe = oV.GetSafe(3).ToType<decimal>() / staticData.GetSafe(10).ToType<decimal>();
            }

            SetStaticData(item, staticData);
            AddToHistories(item);
        }

        public static ConcurrentBag<TrackItemModel> TrackItemHistories
        {
            get { return _trackItemHistories; }
        }

        private static void AddToHistories(TrackItem item)
        {
            var trackItemModel = StaticReference.FromTrackItem(item);
            if (!TrackItemHistories.Any(p => p.Equals(trackItemModel)))
            {
                TrackItemHistories.Add(trackItemModel);
                DataService.SaveTrackItem(trackItemModel);
            }
        }

        private static void SetStaticData(TrackItem item, string[] staticData)
        {
            if (staticData != null && staticData.Count() > 9)
            {
                item.BaseVolume = staticData.GetSafe(0).ToType<decimal>();
                item.FloatingStocks = staticData.GetSafe(8).ToType<decimal>();
                //                item.MaxValidPrice = staticData.GetSafe(2).ToType<decimal>();
                //                item.MaxWeekPrice = staticData.GetSafe(4).ToType<decimal>();
                //                item.MaxYearPrice = staticData.GetSafe(6).ToType<decimal>();
                //                item.MinValidPrice = staticData.GetSafe(3).ToType<decimal>();
                //                item.MinWeekPrice = staticData.GetSafe(5).ToType<decimal>();
                //                item.MinYearPrice = staticData.GetSafe(7).ToType<decimal>();
                item.MonthVolumeAvg = staticData.GetSafe(9).ToType<decimal>();
                item.StocksCount = staticData.GetSafe(1).ToType<decimal>();
            }
        }

        public static string GetOfflineStaticValues(string stockId)
        {
            lock (Lck)
            {
                var str = OfflineStaticData.FirstOrDefault(p => p.Split(new[] { ',' })[0] == stockId);
                if (String.IsNullOrEmpty(str))
                    return str;
                return String.Join(",", str.Split(new[] { ',' }).Skip(1));
            }
        }

        public static string GetOnlineStaticValues(string stockId)
        {
            // Grab the content of the first script element
            string url = "http://www.tsetmc.com/Loader.aspx?ParTree=151311&i=" + stockId;
            string html;
            using (var wc = new GZipWebClient())
                html = wc.DownloadString(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var script = doc
                .DocumentNode
                .Descendants()
                .First(n => n.Name == "script" && n.InnerText.Contains("LVal18AFC"))
                .InnerText;

            // Return the data of spect and stringify it into a proper JSON object
            var engine = new ScriptEngine();
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
                "QTotTran5JAvg.toString()",     //9
                "EstimatedEPS.toString()"       //10
            };
            string code = "(function() { " + script + " var jarray=[" + String.Join(",", fileds) + "]; return jarray.join();})()";
            return engine.Evaluate(code).ToString();
            //            var json = JSONObject.Stringify(engine, result);
        }

        public static string GetFileName()
        {
            return Path.Combine(AppHelper.AppDataPath, "SymbolGroups.xml");
        }

        public static ObservableCollection<SymbolGroup> GetSymbolGroups()
        {
            return SerializeHelper.DeSerializeObject<ObservableCollection<SymbolGroup>>(
                GetFileName()) ?? new ObservableCollection<SymbolGroup>();
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
