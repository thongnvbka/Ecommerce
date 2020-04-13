using System;

namespace Library.ViewModels.Account
{
    public class DepositItem
    {
        public long ROW { get; set; }
        public int Id { get; set; }
        public string code { get; set; }
        public DateTime CreateDate { get; set; }
        public int PacketNumber { get; set; }
        public decimal ProvisionalMoney { get; set; }
        public double TotalWeight { get; set; }
        public long IsComplain { get; set; }
        public byte Status { get; set; }
    }

    public class DepositStatusItem
    {
        public int choBaoGia { get; set; }
        public int choXuLy { get; set; }
        public int choKetDon { get; set; }
        public int choXuatKho { get; set; }
        public int hangTrongKho { get; set; }
        public int dangVanChuyen { get; set; }
        public int choGiaoHang { get; set; }
        public int daGiaoHang { get; set; }
        public int hoanThanh { get; set; }
        public int huy { get; set; }
    }
}
