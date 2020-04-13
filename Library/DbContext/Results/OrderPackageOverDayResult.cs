namespace Library.DbContext.Results
{
    public class OrderPackageOverDayResult
    {
        public int OrderId { get; set; }
        public byte OrderType { get; set; }
        public string OrderCode { get; set; }
        public string PackageCode { get; set; }
        public int PackageId { get; set; }
        public string UserFullName { get; set; }
        public int UserId { get; set; }
    }
}
