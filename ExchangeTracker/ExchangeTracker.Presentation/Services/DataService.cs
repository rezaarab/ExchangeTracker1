using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ExchangeTracker.Presentation.Services
{
    public static class DataService
    {
        static DataService()
        {
        }
        public static void SaveTrackItem(TrackItemModel model)
        {
            SaveTrackItems(new List<TrackItemModel> { model });
        }

        public static void SaveTrackItems(List<TrackItemModel> models)
        {
            using (var dbContext = new ExchangeDbCeEntities())
            {
                dbContext.TrackItemModel.AddRange(models);
                dbContext.SaveChanges();
            }
        }

        public static IEnumerable<TrackItemModel> GetTodaySavedItems()
        {
            var dateTime = DateTime.Now.Date;
            using (var dbContext = new ExchangeDbCeEntities())
            {
                return dbContext.TrackItemModel.Where(p => DbFunctions.TruncateTime(p.LastTransactionDateTime) == dateTime).ToArray();
            }
        }

        public static List<TrackItemModel> GetCompanyTrackItemModels(string stockId, DateTime date, int interval)
        {
            var dateTime = date.Date;
            using (var dbContext = new ExchangeDbCeEntities())
            {
                var result = MergeShrink(dbContext.TrackItemModel
                    .Where(p => p.StockId == stockId && DbFunctions.TruncateTime(p.LastTransactionDateTime) == dateTime));
                var first = result.FirstOrDefault();
                if (first == null)
                    return result;
                var startTime = SetSeconds(first.LastTransactionDateTime.TimeOfDay, 0);
                var endTime = DateTime.Now.TimeOfDay.Hours >= 13 || date.Date != DateTime.Now.Date ? new TimeSpan(13, 0, 0) : (startTime > DateTime.Now.TimeOfDay ? startTime : DateTime.Now.TimeOfDay);
                var count = (int)((endTime - startTime).TotalMinutes / interval);
                var trackItemModelTimes = Enumerable.Range(1, count)
                    .Select(p => startTime + TimeSpan.FromMinutes(p * interval))
                    .Select(p => new { timeSpan = p, trackItemModel = FindLastTrackItemModelInRange(result.Except(new[] { first }), p + TimeSpan.FromMinutes(-interval), p) })
                    .ToList()
                    ;
                SetTrackItemModelPercents(first);
                var lastTrackItemModelProceed = first;
                first.TimeSpan = SetSeconds(first.LastTransactionDateTime.TimeOfDay, 1);
                var finalResult = new List<TrackItemModel> { first };
                trackItemModelTimes.ForEach(p =>
                {
                    if (p.trackItemModel == null)
                    {
                        var emptyTrackItemModel = GetEmptyTrackItemModel(p.timeSpan, lastTrackItemModelProceed);
                        emptyTrackItemModel.TimeSpan = SetSeconds(emptyTrackItemModel.TimeSpan, 0);
                        finalResult.Add(emptyTrackItemModel);
                    }
                    else
                    {
                        var diffTrackItemModel = GetDiffTrackItemModel(p.timeSpan, lastTrackItemModelProceed, p.trackItemModel);
                        diffTrackItemModel.TimeSpan = SetSeconds(diffTrackItemModel.TimeSpan, 2);
                        SetTrackItemModelPercents(diffTrackItemModel);
                        finalResult.Add(diffTrackItemModel);
                        lastTrackItemModelProceed = p.trackItemModel;
                    }
                });
                if (trackItemModelTimes.Count(p => p.trackItemModel != null) > 1)
                {
                    var item = trackItemModelTimes.Last(p => p.trackItemModel != null);
                    item.trackItemModel.TimeSpan = SetSeconds(trackItemModelTimes.Last().timeSpan, 3);
                    SetTrackItemModelPercents(item.trackItemModel);
                    finalResult.Add(item.trackItemModel);
                }
                return finalResult;
                // timespan seconds 
                // 0:No updated
                // 1:first record
                // 2:row updated
                // 3:last row
            }
        }

        public static TimeSpan SetSeconds(TimeSpan timespan, int second)
        {
            return timespan.Add(TimeSpan.FromSeconds(-timespan.Seconds + second));
        }

        private static List<TrackItemModel> MergeShrink(IEnumerable<TrackItemModel> result)
        {
            return result
                .GroupBy(p => p.LastTransactionDateTime)
                .Select(p => p.OrderByDescending(q => q.RegisterDateTime)
                    .FirstOrDefault(q => q.BuyLegalCount > 0 || q.BuyRealCount > 0 || q.SellLegalCount > 0 || q.SellRealCount > 0))
                .Where(p => p != null)
                .OrderBy(p => p.LastTransactionDateTime)
                .ToList();
        }

        private static TrackItemModel GetDiffTrackItemModel(TimeSpan timeSpan, TrackItemModel lastTrackItemModelProceed, TrackItemModel trackItemModel)
        {
            return new TrackItemModel
            {
                TimeSpan = timeSpan,
                BuyLegalCount = trackItemModel.BuyLegalCount - lastTrackItemModelProceed.BuyLegalCount,
                StockId = trackItemModel.StockId,
                Id = Guid.NewGuid(),
                LastTransactionDateTime = trackItemModel.LastTransactionDateTime,
                BuyLegalVolume = trackItemModel.BuyLegalVolume - lastTrackItemModelProceed.BuyLegalVolume,
                BuyRealCount = trackItemModel.BuyRealCount - lastTrackItemModelProceed.BuyRealCount,
                BuyRealVolume = trackItemModel.BuyRealVolume - lastTrackItemModelProceed.BuyRealVolume,
                FinalPrice = trackItemModel.FinalPrice,
                LastTransactionPrice = trackItemModel.LastTransactionPrice,
                RegisterDateTime = DateTime.Now,
                SellLegalCount = trackItemModel.SellLegalCount - lastTrackItemModelProceed.SellLegalCount,
                SellLegalVolume = trackItemModel.SellLegalVolume - lastTrackItemModelProceed.SellLegalVolume,
                SellRealCount = trackItemModel.SellRealCount - lastTrackItemModelProceed.SellRealCount,
                SellRealVolume = trackItemModel.SellRealVolume - lastTrackItemModelProceed.SellRealVolume,
                TransactionCount = trackItemModel.TransactionCount - lastTrackItemModelProceed.TransactionCount,
                TransactionValue = trackItemModel.TransactionValue - lastTrackItemModelProceed.TransactionValue,
                TransactionVolume = trackItemModel.TransactionVolume - lastTrackItemModelProceed.TransactionVolume,
            };
        }

        private static TrackItemModel GetEmptyTrackItemModel(TimeSpan timeSpan, TrackItemModel lastTrackItemModelProceed)
        {
            return new TrackItemModel
            {
                Id = Guid.NewGuid(),
                LastTransactionDateTime = DateTime.Now.Date.Add(timeSpan.Add(TimeSpan.FromSeconds(2))),
                TimeSpan = timeSpan.Add(TimeSpan.FromSeconds(2)),
            };
        }

        private static TrackItemModel FindLastTrackItemModelInRange(IEnumerable<TrackItemModel> result, TimeSpan from, TimeSpan to)
        {
            return result
                .Where(p => p.LastTransactionDateTime.TimeOfDay > from && p.LastTransactionDateTime.TimeOfDay <= to)
                .OrderByDescending(p => p.LastTransactionDateTime)
                .FirstOrDefault();
        }

        /// <summary>
        /// Percent calculate base on volume
        /// </summary>
        /// <param name="trackItemModel"></param>
        private static void SetTrackItemModelPercents(TrackItemModel trackItemModel)
        {
            var buyLegalPercent = trackItemModel.BuyLegalVolume /
                                  GetOneIfZero(trackItemModel.BuyLegalVolume + trackItemModel.BuyRealVolume);
            trackItemModel.BuyLegalPercent = buyLegalPercent * 100;
            var buyRealPercent = trackItemModel.BuyRealVolume /
                                  GetOneIfZero(trackItemModel.BuyLegalVolume + trackItemModel.BuyRealVolume);
            trackItemModel.BuyRealPercent = buyRealPercent * 100;

            var sellLegalPercent = trackItemModel.SellLegalVolume /
                                  GetOneIfZero(trackItemModel.SellLegalVolume + trackItemModel.SellRealVolume);
            trackItemModel.SellLegalPercent = sellLegalPercent * 100;
            var sellRealPercent = trackItemModel.SellRealVolume /
                                  GetOneIfZero(trackItemModel.SellLegalVolume + trackItemModel.SellRealVolume);
            trackItemModel.SellRealPercent = sellRealPercent * 100;
        }

        private static decimal GetOneIfZero(decimal num)
        {
            return num == 0 ? 1 : num;
        }

        public static void DeleteAllHistoryExcepts(string[] stockIds)
        {
            using (var dbContext = new ExchangeDbCeEntities())
            {
                var trackItemModels = dbContext.TrackItemModel.Where(p => !stockIds.Contains(p.StockId)).ToArray();
                dbContext.TrackItemModel.RemoveRange(trackItemModels);
                dbContext.SaveChanges();
            }
        }
    }
}
