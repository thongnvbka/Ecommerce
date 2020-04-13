using System;

namespace Library.ViewModels.Account
{
    public class OrderItem
    {
        public long ROW { get; set; }
        public int id { get; set; }
        public string code { get; set; }
        public DateTime created { get; set; }
        public string ImagePath { get; set; }
        public int ProductCount { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalExchange { get; set; }
        public decimal TotalMiss { get; set; }
        public long IsComplain { get; set; }
        public byte Status { get; set; }
        public string ShopLink { get; set; }
    }

    public class OrderAdvanceMoneyItem
    {
        public string TotalLevel { get; set; }
        public string Total { get; set; }
        public string Percent { get; set; }
    }

    public class OrderAdvanceItem
    {
        public decimal TotalLevel { get; set; }
        public decimal Total { get; set; }
        public decimal Percent { get; set; }
    }

    public class OrderAutoItem
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; }
        public string created { get; set; }
        public string ImagePath { get; set; }
        public int ProductCount { get; set; }
        public string TotalPrice { get; set; }
        public string TotalExchange { get; set; }
        public byte Status { get; set; }
    }

    public class OrderAutoItemV2
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; }
        public DateTime Created { get; set; }
        public string ImagePath { get; set; }
        public int ProductCount { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalExchange { get; set; }
        public byte Status { get; set; }
    }

    public class OrderStatusItem
    {
        public int dhChoBaoGia { get; set; }
        public int dhChoDatCoc { get; set; }
        public int dhChoDatHang { get; set; }
        public int dhDangDatHang { get; set; }
        public int dhShopPhatHang { get; set; }
        public int dhHangTrongKho { get; set; }
        public int dhDangVanChuyen { get; set; }
        public int dhChoGiaoHang { get; set; }
        public int dhDaGiaoHang { get; set; }
        public int dhHoanThanh { get; set; }
        public int dhDaHuy { get; set; }
        public int dhMatHong { get; set; }
    }

    public class OrderDetailCountItem
    {
        public int ProductCount { get; set; }
        public int QuantityActually { get; set; }
        public int QuantityBooked { get; set; }
    }
}