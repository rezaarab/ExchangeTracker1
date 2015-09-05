using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExchangeTracker.Domain;

namespace ExchangeTracker.Presentation
{
    public partial class TrackItemModel
    {
        //        public Guid Id { get; set; }
        //
        //        /// <summary>
        //        /// شناسه نماد
        //        /// </summary>
        //        public string StockId { get; set; }
        //
        //        /// <summary>
        //        /// زمان ثبت آیتم
        //        /// </summary>
        //        public DateTime RegisterDateTime { get; set; }
        //
        //        /// <summary>
        //        ///  قیمت آخرین معامله
        //        /// </summary>
        //        public decimal LastTransactionPrice { get; set; }
        //
        //        /// <summary>
        //        /// قیمت پایانی
        //        /// </summary>
        //        public decimal FinalPrice { get; set; }
        //
        //        /// <summary>
        //        /// تعداد معاملات
        //        /// </summary>
        //        public decimal TransactionCount { get; set; }
        //
        //        /// <summary>
        //        /// ارزش معاملات
        //        /// </summary>
        //        public decimal TransactionValue { get; set; }
        //
        //        /// <summary>
        //        /// حجم معاملات
        //        /// </summary>
        //        public decimal TransactionVolume { get; set; }
        //
        //        /// <summary>
        //        /// تعداد خرید حقیقی
        //        /// </summary>
        //        public decimal BuyRealCount { get; set; }
        //
        //        /// <summary>
        //        /// تعداد خرید حقوقی
        //        /// </summary>
        //        public decimal BuyLegalCount { get; set; }
        //
        //        /// <summary>
        //        /// حجم خرید حقیقی
        //        /// </summary>
        //        public decimal BuyRealVolume { get; set; }
        //
        /// <summary>
        /// درصد خرید حقیقی
        /// </summary>
        public decimal BuyRealPercent { get; set; }
        //
        //        /// <summary>
        //        /// حجم خرید حقوقی
        //        /// </summary>
        //        public decimal BuyLegalVolume { get; set; }
        //
        /// <summary>
        /// درصد خرید حقوقی
        /// </summary>
        public decimal BuyLegalPercent { get; set; }
        //
        //        /// <summary>
        //        /// تعداد فروش حقیقی
        //        /// </summary>
        //        public decimal SellRealCount { get; set; }
        //
        //        /// <summary>
        //        /// تعداد فروش حقوقی
        //        /// </summary>
        //        public decimal SellLegalCount { get; set; }
        //
        //        /// <summary>
        //        /// حجم فروش حقیقی
        //        /// </summary>
        //        public decimal SellRealVolume { get; set; }

        /// <summary>
        /// درصد فروش حقیقی
        /// </summary>
        public decimal SellRealPercent { get; set; }

        //        /// <summary>
        //        /// حجم فروش حقوقی
        //        /// </summary>
        //        public decimal SellLegalVolume { get; set; }

        /// <summary>
        /// درصد فروش حقوقی
        /// </summary>
        public decimal SellLegalPercent { get; set; }

        public TimeSpan TimeSpan { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is TrackItemModel))
                return false;
            var item = (TrackItemModel)obj;
            if (item.Id == Id)
                return true;
            return this.StockId == item.StockId &&
            this.LastTransactionDateTime == item.LastTransactionDateTime &&
            this.BuyLegalCount == item.BuyLegalCount &&
            this.BuyLegalVolume == item.BuyLegalVolume &&
            this.BuyRealCount == item.BuyRealCount &&
            this.BuyRealVolume == item.BuyRealVolume &&
            this.FinalPrice == item.FinalPrice &&
            this.LastTransactionPrice == item.LastTransactionPrice &&
            this.SellLegalCount == item.SellLegalCount &&
            this.SellLegalVolume == item.SellLegalVolume &&
            this.SellRealCount == item.SellRealCount &&
            this.SellRealVolume == item.SellRealVolume &&
            this.TransactionCount == item.TransactionCount &&
            this.TransactionValue == item.TransactionValue &&
            this.TransactionVolume == item.TransactionVolume &&
            this.TimeSpan == item.TimeSpan;
        }
    }
}
