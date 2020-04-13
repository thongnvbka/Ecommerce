using System;

namespace Library.DbContext.Results
{
    public class PackageNoCodeForReportResult
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Id kiện hàng mất mã hoặc kiện hàng kho yêu cầu xác nhận khi kho thêm kiện vào đơn hàng
        ///</summary>
        public int PackageId { get; set; } // PackageId

        ///<summary>
        /// Mã kiện hàng
        ///</summary>
        public string PackageCode { get; set; } // PackageCode (length: 30)


        public string TransportCode { get; set; }

        ///<summary>
        /// Kho ghi chú cho kiện hàng mất mã
        ///</summary>
        public string Note { get; set; } // Note

        ///<summary>
        /// Text cho tìm kiềm kiện hàng mất mã, kiện hàng kho thêm
        ///</summary>
        public string UnsignText { get; set; } // UnsignText

        ///<summary>
        /// 0: Mới tạo, 1: Đã đóng
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Loại hàng mất mã: 0: Hàng mất mã, 1: Hàng kho yêu cầu đặt hàng xác nhận
        ///</summary>
        public byte Mode { get; set; } // Mode
        public string ImageJson { get; set; } // ImageJson
        public DateTime Created { get; set; } // Created
        public DateTime Updated { get; set; } // Updated
        public int? CreateUserId { get; set; } // CreateUserId
        public string CreateUserFullName { get; set; } // CreateUserFullName (length: 300)
        public string CreateUserName { get; set; } // CreateUserName (length: 50)
        public int? CreateOfficeId { get; set; } // CreateOfficeId
        public string CreateOfficeName { get; set; } // CreateOfficeName (length: 300)
        public string CreateOfficeIdPath { get; set; } // CreateOfficeIdPath (length: 300)
        public int? UpdateUserId { get; set; } // UpdateUserId
        public string UpdateUserFullName { get; set; } // UpdateUserFullName (length: 300)
        public string UpdateUserName { get; set; } // UpdateUserName (length: 50)
        public int? UpdateOfficeId { get; set; } // UpdateOfficeId
        public int CommentNo { get; set; } // CommentNo
        public string UpdateOfficeName { get; set; } // UpdateOfficeName (length: 300)
        public string UpdateOfficeIdPath { get; set; } // UpdateOfficeIdPath (length: 300)
    }
}
