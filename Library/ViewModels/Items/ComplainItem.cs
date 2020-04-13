using System;

namespace Library.ViewModels.Items
{
    public class ComplainItem
    {
        public long ROW { get; set; }
        public long id { get; set; }
        public int orderId { get; set; }
        public string orderCode { get; set; }
        public string code { get; set; }
        public DateTime createDate { get; set; }
        public DateTime lastUpdateDate { get; set; }
        public string content { get; set; }
        public string lastReply { get; set; }
        public byte Status { get; set; }
        public byte OrderType { get; set; }
    }

    public class ComplainStatusItem
    {
        public int Wait { get; set; }
        public int Process { get; set; }
        public int Success { get; set; }
        public int Cancel { get; set; }
    }
}
