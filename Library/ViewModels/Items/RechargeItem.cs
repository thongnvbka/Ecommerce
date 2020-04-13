using System;

namespace Library.ViewModels.Items
{
    public class RechargeItem
    {
        public long ROW { get; set; }
        public int Id { get; set; }
        public string Code { get; set; }
        public byte Status { get; set; }
        public string Note { get; set; }
        public string TreasureName { get; set; }
        public decimal CurrencyFluctuations { get; set; }
        public decimal? CurencyEnd { get; set; }
        public decimal? CurencyStart { get; set; }
        public byte Type { get; set; }
        public byte IsPlus { get; set; }
        public DateTime Created { get; set; }
        public string OrderCode { get; set; }
        public byte? OrderType { get; set; }
        public int? OrderId { get; set; }
    }
}