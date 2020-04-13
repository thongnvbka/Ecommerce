using System;

namespace Library.DbContext.Entities
{
    // Dispatcher

    public partial class Dispatcher
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã phiếu điều vận
        ///</summary>
        public string Code { get; set; } // Code (length: 20)

        ///<summary>
        /// Điều vận từ kho Id
        ///</summary>
        public int? FromWarehouseId { get; set; } // FromWarehouseId

        ///<summary>
        /// Tên kho bao hàng hiên tại đang ở đó
        ///</summary>
        public string FromWarehouseIdPath { get; set; } // FromWarehouseIdPath (length: 300)

        ///<summary>
        /// Tên kho bao hàng hiên tại đang ở đó
        ///</summary>
        public string FromWarehouseName { get; set; } // FromWarehouseName (length: 300)

        ///<summary>
        /// Tên kho bao hàng hiên tại đang ở đó
        ///</summary>
        public string FromWarehouseAddress { get; set; } // FromWarehouseAddress (length: 500)

        ///<summary>
        /// Trạng thái bao hàng (0: Mới khởi tạo, 1: Đã duyệt, 2: Trong kho, 3: Đang vận chuyển, 4: Mất, 5: Hoàn thành)
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Tổng giá trị tiền hàng trong bao hàng
        ///</summary>
        public decimal? Amount { get; set; } // Amount

        ///<summary>
        /// Tổng cân nặng hàng hóa trong bao hàng
        ///</summary>
        public decimal? TotalWeight { get; set; } // TotalWeight

        ///<summary>
        /// Tổng cân nặng thực tế
        ///</summary>
        public decimal? TotalWeightActual { get; set; } // TotalWeightActual

        ///<summary>
        /// Tổng cân nặng quy đổi
        ///</summary>
        public decimal? TotalWeightConverted { get; set; } // TotalWeightConverted

        ///<summary>
        /// Tổng thể tích của kiện hàng trong bao hàng
        ///</summary>
        public decimal? TotalVolume { get; set; } // TotalVolume

        ///<summary>
        /// Tổng số kiện hàng nằm trong bao
        ///</summary>
        public int TotalPackageNo { get; set; } // TotalPackageNo

        ///<summary>
        /// Tổng số kiện hàng nằm trong bao
        ///</summary>
        public int WalletNo { get; set; } // WalletNo

        ///<summary>
        /// Hình thức tính tiền (Theo khối lượng, Theo thể tích)
        ///</summary>
        public byte PriceType { get; set; } // PriceType

        ///<summary>
        /// Giá vốn / 1kg hoặc 1m3
        ///</summary>
        public decimal Price { get; set; } // Price

        ///<summary>
        /// Giá trị cân nặng hoặc thể tích chốt với nhà vận chuyển
        ///</summary>
        public decimal Value { get; set; } // Value

        ///<summary>
        /// Id Nhân viên tạo phiếu nhập bao
        ///</summary>
        public int UserId { get; set; } // UserId

        ///<summary>
        /// Tên nhân viên thực hiện tạo bao
        ///</summary>
        public string UserName { get; set; } // UserName (length: 50)

        ///<summary>
        /// Mã Code nhân viên thực hiện tạo phiếu
        ///</summary>
        public string UserFullName { get; set; } // UserFullName (length: 300)

        ///<summary>
        /// Ngày bao được tạo
        ///</summary>
        public DateTime Created { get; set; } // Created

        ///<summary>
        /// Thời gian sửa - Cập nhật bao hàng
        ///</summary>
        public DateTime Updated { get; set; } // Updated
        public string Note { get; set; } // Note (length: 500)
        public string UnsignedText { get; set; } // UnsignedText (length: 500)
        public bool IsDelete { get; set; } // IsDelete

        ///<summary>
        /// Ngày dự kiến về kho
        ///</summary>
        public DateTime? ForcastDate { get; set; } // ForcastDate

        ///<summary>
        /// Id kho hàng hiện tại bao đang ở đó
        ///</summary>
        public int? ToWarehouseId { get; set; } // ToWarehouseId

        ///<summary>
        /// Tên kho bao hàng hiên tại đang ở đó
        ///</summary>
        public string ToWarehouseIdPath { get; set; } // ToWarehouseIdPath (length: 300)

        ///<summary>
        /// Tên kho bao hàng hiên tại đang ở đó
        ///</summary>
        public string ToWarehouseName { get; set; } // ToWarehouseName (length: 300)

        ///<summary>
        /// Tên kho bao hàng hiên tại đang ở đó
        ///</summary>
        public string ToWarehouseAddress { get; set; } // ToWarehouseAddress (length: 500)
        public int TransportPartnerId { get; set; } // TransportPartnerId
        public string TransportPartnerName { get; set; } // TransportPartnerName (length: 300)
        public int TransportMethodId { get; set; } // TransportMethodId
        public string TransportMethodName { get; set; } // TransportMethodName (length: 300)

        ///<summary>
        /// Tên người liên hệ
        ///</summary>
        public string ContactName { get; set; } // ContactName (length: 300)

        ///<summary>
        /// Số điện thoại người liên hệ
        ///</summary>
        public string ContactPhone { get; set; } // ContactPhone (length: 20)

        public int? EntrepotId { get; set; }

        public string EntrepotName { get; set; }

        public Dispatcher()
        {
            TotalPackageNo = 0;
            WalletNo = 0;
            UnsignedText = "";
            IsDelete = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
