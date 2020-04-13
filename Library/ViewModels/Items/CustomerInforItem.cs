namespace Library.ViewModels.Items
{
    public class CustomerInforItem
    {
        public decimal Balance { get; set; }
        public decimal BalanceAvalible { get; set; }
        public string LevelName { get; set; }
        public decimal StartNext { get; set; }
        public decimal MaxMoney { get; set; }
        public decimal StartMoney { get; set; }

    }

    public class CusInforOrderItem
    {
        public string CreateDate { get; set; }
        public int TotalOrder { get; set; }
    }
}
