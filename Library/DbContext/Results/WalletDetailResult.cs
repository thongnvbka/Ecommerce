using Library.DbContext.Entities;
using System;
using System.Collections.Generic;

namespace Library.DbContext.Results
{
    public class WalletDetailResult
    {
        public int Id { get; set; } // Id (Primary key)
        public int WalletId { get; set; } // WalletId
        public string WalletCode { get; set; } // WalletCode (length: 30)

        ///<summary>
        /// Id kiện hàng hoặc Id của bao hàng
        ///</summary>
        public int PackageId { get; set; } // PackageId

        ///<summary>
        /// Mã kiện hàng hoặc là mã bao hàng
        ///</summary>
        public string PackageCode { get; set; } // packageCode (length: 50)

        ///<summary>
        /// Id đơn hàng
        ///</summary>
        public int? OrderId { get; set; } // OrderId

        ///<summary>
        /// Mã đơn hàng
        ///</summary>
        public string OrderCode { get; set; } // OrderCode (length: 50)

        public byte OrderType { get; set; } // OrderType

        ///<summary>
        /// Ghi chú các dịch vụ trong đơn hàng: (Kiểm đếm, Đóng kiện,..)
        ///</summary>
        public string OrderServices { get; set; } // OrderServices (length: 500)

        public int? OrderPackageNo { get; set; } // OrderPackageNo
        public decimal? Amount { get; set; } // Amount

        ///<summary>
        /// Mã vận đơn
        ///</summary>
        public string TransportCode { get; set; } // TransportCode (length: 50)

        ///<summary>
        /// Ghi chú đơn hàng
        ///</summary>
        public string Note { get; set; } // Note (length: 500)

        ///<summary>
        /// 1: Bình thường, 2: Hỏng vỡ
        ///</summary>
        public byte Status { get; set; } // Status

        public decimal? Weight { get; set; } // Weight

        ///<summary>
        /// Thể tích m3 của kiện hàng
        ///</summary>
        public decimal? Volume { get; set; } // Volume

        ///<summary>
        /// Tổng cân nặng quy đổi
        ///</summary>
        public decimal? ConvertedWeight { get; set; } // ConvertedWeight

        ///<summary>
        /// Tổng cân nặng thực tế
        ///</summary>
        public decimal? ActualWeight { get; set; } // ActualWeight

        public DateTime Created { get; set; } // Created
        public DateTime Updated { get; set; } // Updated
        public string OrderCodes { get; set; } // OrderCodes (length: 1000)
        public string PackageCodes { get; set; } // PackageCodes (length: 1000)
        public string Customers { get; set; } // Customers (length: 1000)
        public string OrderCodesUnsigned { get; set; } // OrderCodesUnsigned (length: 1000)
        public string PackageCodesUnsigned { get; set; } // PackageCodesUnsigned (length: 1000)
        public string CustomersUnsigned { get; set; } // CustomersUnsigned (length: 1000)

        #region Kiện hàng

        ///<summary>
        /// Tên khách hàng
        ///</summary>
        public string CustomerName { get; set; } // CustomerName (length: 300)

        ///<summary>
        /// Tài khoản khách hàng
        ///</summary>
        public string CustomerUserName { get; set; } // CustomerUserName (length: 300)

        ///<summary>
        /// Độ Vip của khách hàng
        ///</summary>
        public byte CustomerLevelId { get; set; } // CustomerLevelId

        ///<summary>
        /// Tên độ Vip của khách hàng
        ///</summary>
        public string CustomerLevelName { get; set; } // CustomerLevelName (length: 300)

        ///<summary>
        /// Kho hàng
        ///</summary>
        public int CustomerWarehouseId { get; set; } // CustomerWarehouseId

        ///<summary>
        /// Tên kho hàng
        ///</summary>
        public string CustomerWarehouseAddress { get; set; } // CustomerWarehouseAddress (length: 500)

        ///<summary>
        /// Tên kho hàng
        ///</summary>
        public string CustomerWarehouseName { get; set; } // CustomerWarehouseName (length: 300)

        public string CustomerWarehouseIdPath { get; set; } // CustomerWarehouseIdPath (length: 300)

        ///<summary>
        /// Cân nặng bì gỗ khi kiện đóng kiện gỗ
        ///</summary>
        public decimal? WeightWapper { get; set; } // WeightWapper

        #endregion Kiện hàng

        public List<PackageNote> PackageNotes { get; set; }

        public Dictionary<byte, string> PackageNoteMode { get; set; }
    }
}