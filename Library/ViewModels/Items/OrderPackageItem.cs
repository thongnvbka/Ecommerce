using System;

namespace Library.ViewModels.Items
{
    public class OrderPackageListItem
    {
        public long ROW { get; set; }
        public string Code { get; set; }
        public int Id { get; set; }

        ///<summary>
        /// Mã vận đơn của kiện hàng
        ///</summary>
        public string TransportCode { get; set; } // TransportCode (length: 50)

        public DateTime Created { get; set; }
        public decimal? WeightActual { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public decimal? Length { get; set; }
        public string OrderCode { get; set; }
        public int OrderId { get; set; }
        public byte Status { get; set; }
        public string Size { get; set; }
        public string CurrentWarehouseName { get; set; }
        public byte OrderType { get; set; }
        public byte IsGross { get; set; }
        public decimal? ActualMoney { get; set; } // ActualMoney
        public decimal? Weight { get; set; }
    }

    public class OrderPackageStatusItem
    {
        public int shopTqPhatHang { get; set; }
        public int khoTqNhanHang { get; set; }
        public int trongKhoTq { get; set; }
        public int xuatKhoTq { get; set; }
        public int trenDuongVe { get; set; }
        public int trongKhoVn { get; set; }
        public int dangVanChuyen { get; set; }
        public int choGiaoHang { get; set; }
        public int dangGiaoHang { get; set; }
        public int daTraHang { get; set; }
        public int hangBiMat { get; set; }
    }
}