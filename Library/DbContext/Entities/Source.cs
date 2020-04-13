using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.DbContext.Entities
{

    // Source

    public partial class Source
    {
        public long Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã yêu cầu tìm nguồn
        ///</summary>
        public string Code { get; set; } // Code (length: 30)

        ///<summary>
        /// Id hệ thống
        ///</summary>
        public int SystemId { get; set; } // SystemId

        ///<summary>
        /// Đơn hàng phát sinh từ hệ thống
        ///</summary>
        public string SystemName { get; set; } // SystemName (length: 100)

        ///<summary>
        /// Id kho đến khách hàng chọn
        ///</summary>
        public int WarehouseId { get; set; } // WarehouseId

        ///<summary>
        /// Tên kho đến khách hàng chọn
        ///</summary>
        public string WarehouseName { get; set; } // WarehouseName (length: 500)

        ///<summary>
        /// Id khách hàng
        ///</summary>
        public int? CustomerId { get; set; } // CustomerId
        ///<summary>
        /// Tên khách hàng
        ///</summary>
        public string CustomerName { get; set; } // CustomerName (length: 300)

        ///<summary>
        /// Email khách hàng
        ///</summary>
        public string CustomerEmail { get; set; } // CustomerEmail (length: 300)

        ///<summary>
        /// Điện thoại khách hàng
        ///</summary>
        public string CustomerPhone { get; set; } // CustomerPhone (length: 300)
        public string CustomerAddress { get; set; } // CustomerPhone (length: 500)

        ///<summary>
        /// Trạng thái yêu cầu tìm nguồn: 0: chờ xử lý, 1: đang xử lý, 2: hoàn thành, 3: đã hủy
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Id nhân viên xử lý
        ///</summary>
        public int? UserId { get; set; } // UserId

        ///<summary>
        /// Tên nhân viên xử lý
        ///</summary>
        public string UserFullName { get; set; } // UserFullName (length: 150)
                                                 ///<summary>
                                                 /// Đơn vị nhân viên tạo
                                                 ///</summary>
        public int? OfficeId { get; set; } // OfficeId

        ///<summary>
        /// Tên đơn vị nhân viên xử lý
        ///</summary>
        public string OfficeName { get; set; } // OfficeName (length: 300)

        ///<summary>
        /// Id Path đơn vị nhân viên
        ///</summary>
        public string OfficeIdPath { get; set; } // OfficeIdPath (length: 300)

        ///<summary>
        /// Id Path đơn vị nhân viên
        ///</summary>
        public string CreatedOfficeIdPath { get; set; } // CreatedOfficeIdPath (length: 300)

        ///<summary>
        /// Id nhân viên xử lý
        ///</summary>
        public int? CreatedUserId { get; set; } // CreatedUserId

        ///<summary>
        /// Tên nhân viên xử lý
        ///</summary>
        public string CreatedUserFullName { get; set; } // CreatedUserFullName (length: 150)

        ///<summary>
        /// Đơn vị nhân viên tạo
        ///</summary>
        public int? CreatedOfficeId { get; set; } // CreatedOfficeId

        ///<summary>
        /// Tên đơn vị nhân viên xử lý
        ///</summary>
        public string CreatedOfficeName { get; set; } // CreatedOfficeName (length: 300)

        ///<summary>
        /// Ngày tạo
        ///</summary>
        public DateTime CreateDate { get; set; } // CreateDate

        ///<summary>
        /// Ngày cập nhật
        ///</summary>
        public DateTime? UpdateDate { get; set; } // UpdateDate

        ///<summary>
        /// Gói dịch vụ, 0: gói cơ bản, 1: gói vip, 2: dùng thử
        ///</summary>
        public int TypeService { get; set; } // TypeService
        public string TypeServiceName { get; set; } // TypeServiceName
        ///<summary>
        /// Tiền dịch vụ trên 1 sản phẩm tìm nguồn
        ///</summary>
        public decimal ServiceMoney { get; set; } // ServiceMoney

        ///<summary>
        /// Bản ghi đã xóa hay chưa
        ///</summary>
        public bool IsDelete { get; set; } // IsDelete
        public string AnalyticSupplier { get; set; } // AnalyticSupplier

        ///<summary>
        /// Tiền phí vận chuyển
        ///</summary>
        public decimal ShipMoney { get; set; } // ShipMoney
        public long? SourceSupplierId { get; set; } // SourceSupplierId
        ///<summary>
        /// Loại đơn hàng order, ký gửi, tìm nguồn
        ///</summary>
        public byte Type { get; set; } // Status
        ///<summary>
        /// Nhân viên ghi chú
        ///</summary>
        public string UserNote { get; set; } // UserNote (length: 500)
        ///<summary>
        /// Phục vụ tìm kiếm
        ///</summary>
        public string UnsignName { get; set; } // UnsignName (length: 500)
        /// <summary>
        /// Lý do hủy đơn hàng
        /// </summary>
        public string ReasonCancel { get; set; }

        [NotMapped]
        public int? Chat { get; set; }
        [NotMapped]
        public string ChatContent { get; set; }

        public Source()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
