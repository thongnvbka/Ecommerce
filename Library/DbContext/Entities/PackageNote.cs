using System;

namespace Library.DbContext.Entities
{
    // PackageNote

    public partial class PackageNote
    {
        public long Id { get; set; } // Id
        public int OrderId { get; set; } // OrderId
        public string OrderCode { get; set; } // OrderCode (length: 50)
        public int? PackageId { get; set; } // PackageId
        public string PackageCode { get; set; } // PackageCode (length: 50)

        ///<summary>
        /// Mode ghi chú: Nhập kho, Putaway, Đóng bao, Xuất kho,...
        ///</summary>
        public byte Mode { get; set; } // Mode

        ///<summary>
        /// Thời gian tạo ghi chú
        ///</summary>
        public DateTime Time { get; set; } // Time

        ///<summary>
        /// Object: Id phiếu nhập, bao, kiện gỗ,...
        ///</summary>
        public int? ObjectId { get; set; } // ObjectId
        public string ObjectCode { get; set; } // ObjectCode (length: 50)

        ///<summary>
        /// Nhân viên thực hiện
        ///</summary>
        public int? UserId { get; set; } // UserId
        public string UserFullName { get; set; } // UserFullName (length: 300)
        public string DataJson { get; set; } // DataJson
        public string Content { get; set; } // DataJson
        public PackageNote()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
