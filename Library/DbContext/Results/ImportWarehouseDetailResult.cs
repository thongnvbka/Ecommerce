using System;
using System.Collections.Generic;
using Library.DbContext.Entities;

namespace Library.DbContext.Results
{
    public class ImportWarehouseDetailResult
    {
        public int Id { get; set; } // Id (Primary key)
        public int ImportWarehouseId { get; set; } // ImportWarehouseId
        public string ImportWarehouseCode { get; set; } // ImportWarehouseCode (length: 30)

        ///<summary>
        /// Id khách hàng
        ///</summary>
        public int? CustomerId { get; set; } // CustomerId

        ///<summary>
        /// Tên khách hàng
        ///</summary>
        public string CustomerName { get; set; } // CustomerName (length: 300)

        ///<summary>
        /// Tài khoản khách hàng
        ///</summary>
        public string CustomerUserName { get; set; } // CustomerUserName (length: 300)

        ///<summary>
        /// Loại package: 0: kiện hàng, 1: Bao hàng
        ///</summary>
        public byte Type { get; set; } // Type

        ///<summary>
        /// Id kiện hàng hoặc Id của bao hàng
        ///</summary>
        public int PackageId { get; set; } // PackageId

        ///<summary>
        /// Mã kiện hàng hoặc là mã bao hàng
        ///</summary>
        public string PackageCode { get; set; } // packageCode (length: 50)
        public int OrderPackageNo { get; set; } // OrderPackageNo

        ///<summary>
        /// Ghi chú các dịch vụ trong đơn hàng: (Kiểm đếm, Đóng kiện,..)
        ///</summary>
        public string OrderServices { get; set; } // OrderServices (length: 500)

        ///<summary>
        /// Ghi chú đơn hàng
        ///</summary>
        public string Note { get; set; } // Note (length: 500)

        ///<summary>
        /// Mã đơn hàng
        ///</summary>
        public string OrderCode { get; set; } // OrderCode (length: 50)

        ///<summary>
        /// Id đơn hàng
        ///</summary>
        public int? OrderId { get; set; } // OrderId
        public byte OrderType { get; set; } // OrderType

        ///<summary>
        /// Mã vận đơn
        ///</summary>
        public string TransportCode { get; set; } // TransportCode (length: 50)

        ///<summary>
        /// 1: Bình thường, 2: Hỏng vỡ
        ///</summary>
        public byte Status { get; set; } // Status
        public string WarehouseIdPath { get; set; } // WarehouseIdPath (length: 300)

        ///<summary>
        /// Tên kho nhập hàng
        ///</summary>
        public string WarehouseName { get; set; } // WarehouseName (length: 300)
        public string WarehouseAddress { get; set; } // WarehouseAddress (length: 300)
        public int WarehouseId { get; set; } // WarehouseId
        public bool IsDelete { get; set; } // IsDelete
        public DateTime Created { get; set; } // Created
        public DateTime Updated { get; set; } // Updated

        public List<PackageNote> PackageNotes { get; set; }

        public Dictionary<byte, string> PackageNoteMode { get; set; }
    }
}
