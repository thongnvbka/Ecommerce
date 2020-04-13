using Library.DbContext.Entities;
using System;
using System.Collections.Generic;
namespace Library.ViewModels.Items
{
    public class DebitItem
    {
        public long ROW { get; set; }
        public int DebitId { get; set; }
        public byte DebitType { get; set; }
        public string DebitCode { get; set; }
        public decimal Money { get; set; }
        public int OrderId { get; set; }
        public byte OrderType { get; set; }
        public string OrderCode { get; set; }
        public string PayReceivableIName { get; set; }
        public int SubjectId { get; set; }
        public DateTime Created { get; set; }
    }

    public class DebitItemV2
    {
        public long ROW { get; set; }
        public int DebitId { get; set; }
        public byte DebitType { get; set; }
        public string DebitCode { get; set; }
        public decimal Money { get; set; }
        public int OrderId { get; set; }
        public byte OrderType { get; set; }
        public string OrderCode { get; set; }
        public string PayReceivableIName { get; set; }
        public int SubjectId { get; set; }
        public DateTime Created { get; set; }
        public List<DebitHistory> ListDebitHistory { get; set; }
    }
}
