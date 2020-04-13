using System;

namespace Library.DbContext.Entities
{
    // ExportWarehouseDetail

    public partial class ExportWarehouseDetail
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Id phiếu xuất kho
        ///</summary>
        public int ExportWarehouseId { get; set; } // ExportWarehouseId

        ///<summary>
        /// Mã phiếu xuất kho
        ///</summary>
        public string ExportWarehouseCode { get; set; } // ExportWarehouseCode (length: 50)

        ///<summary>
        /// Id kiện xuất kho
        ///</summary>
        public int PackageId { get; set; } // PackageId

        ///<summary>
        /// Mã kiện xuất kho
        ///</summary>
        public string PackageCode { get; set; } // PackageCode (length: 50)

        ///<summary>
        /// Cân nặng kiện xuất kho
        ///</summary>
        public decimal PackageWeight { get; set; } // PackageWeight

        ///<summary>
        /// Cân nặng chuyển đổi kiện xuất kho
        ///</summary>
        public decimal PackageWeightConverted { get; set; } // PackageWeightConverted

        ///<summary>
        /// Cân nặng thực kiện xuất kho
        ///</summary>
        public decimal PackageWeightActual { get; set; } // PackageWeightActual

        ///<summary>
        /// Cân nặng thực kiện xuất kho
        ///</summary>
        public string PackageTransportCode { get; set; } // PackageTransportCode (length: 50)

        ///<summary>
        /// Ghi chú của kiện hàng
        ///</summary>
        public string Note { get; set; } // Note (length: 500)

        ///<summary>
        /// Cân nặng thực kiện xuất kho
        ///</summary>
        public string PackageSize { get; set; } // PackageSize (length: 500)

        ///<summary>
        /// Id đơn hàng kiện xuất kho
        ///</summary>
        public int OrderId { get; set; } // OrderId

        ///<summary>
        /// Mã đơn hàng của kiện xuất kho
        ///</summary>
        public string OrderCode { get; set; } // OrderCode (length: 50)
        public byte OrderType { get; set; } // OrderType

        ///<summary>
        /// Cân nặng đơn hàng kiện xuất kho
        ///</summary>
        public decimal OrderWeight { get; set; } // OrderWeight

        ///<summary>
        /// Cân nặng chuyển đổi đơn hàng kiện xuất kho
        ///</summary>
        public decimal OrderWeightConverted { get; set; } // OrderWeightConverted

        ///<summary>
        /// Cân nặng thực của đơn hàng kiện xuất kho
        ///</summary>
        public decimal OrderWeightActual { get; set; } // OrderWeightActual

        ///<summary>
        /// Tiền ship của đơn trong phiếu xuất kho này
        ///</summary>
        public decimal OrderShip { get; set; } // OrderShip

        ///<summary>
        /// Tiền ship thực thu của đơn trong phiếu xuất kho này
        ///</summary>
        public decimal OrderShipActual { get; set; } // OrderShipActual

        ///<summary>
        /// Số kiện của order trong phiếu xuất kho này
        ///</summary>
        public int OrderPackageNo { get; set; } // OrderPackageNo

        ///<summary>
        /// Tổng số kiện trong đơn hàng
        ///</summary>
        public int OrderTotalPackageNo { get; set; } // OrderTotalPackageNo

        ///<summary>
        /// Tiền ship thực thu của đơn trong phiếu xuất kho này
        ///</summary>
        public string OrderNote { get; set; } // OrderNote

        ///<summary>
        /// Id khách hàng của đơn hàng
        ///</summary>
        public int CustomerId { get; set; } // CustomerId

        ///<summary>
        /// Tài khoản khách hàng của đơn hàng
        ///</summary>
        public string CustomerUserName { get; set; } // CustomerUserName (length: 300)

        ///<summary>
        /// HỌ tên đầy đủ của khách hàng
        ///</summary>
        public string CustomerFullName { get; set; } // CustomerFullName (length: 300)
        public string CustomerPhone { get; set; } // CustomerPhone (length: 300)
        public string CustomerAddress { get; set; } // CustomerAddress (length: 500)
        public int CustomerOrderNo { get; set; } // CustomerOrderNo

        ///<summary>
        /// Khỏng cách giao hàng cho khách hàng
        ///</summary>
        public decimal CustomerDistance { get; set; } // CustomerDistance

        ///<summary>
        /// Tổng cân nặng giao cho khách trong phiếu xuất này
        ///</summary>
        public decimal CustomerWeight { get; set; } // CustomerWeight

        ///<summary>
        /// Tổng cân nặng chuyển đổi giao cho khách trong phiếu xuất này
        ///</summary>
        public decimal CustomerWeightConverted { get; set; } // CustomerWeightConverted

        ///<summary>
        /// Tổng cân nặng thực giao cho khách trong phiếu xuất này
        ///</summary>
        public decimal CustomerWeightActual { get; set; } // CustomerWeightActual
        public bool IsDelete { get; set; } // IsDelete
        public DateTime Created { get; set; } // Created
        public DateTime Updated { get; set; } // Updated

        ///<summary>
        /// 0: Chờ dyệt, 1: Đã duyệt
        ///</summary>
        public byte Status { get; set; } // Status

        public ExportWarehouseDetail()
        {
            OrderPackageNo = 0;
            OrderTotalPackageNo = 0;
            IsDelete = false;
            Status = 0;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
