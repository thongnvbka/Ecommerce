using System;

namespace Library.ViewModels.Items
{
    public class SourceItem
    {
        public long ROW { get; set; }
        public long id { get; set; }
        public string code { get; set; }
        public DateTime CreateDate { get; set; }
        public string ImagePath { get; set; }
        public int Quantity { get; set; }
        public byte TypeService { get; set; }
        public string TypeServiceName { get; set; }
        public byte Status { get; set; }
    }
    public class SourceStatusItem
    {
        public int WaitProccess { get; set; }
        public int Proccess { get; set; }
        public int WaitingChoice { get; set; }
        public int Finish { get; set; }
        public int Cancel { get; set; }
    }
}
