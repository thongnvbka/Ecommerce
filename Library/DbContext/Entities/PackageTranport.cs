using System;

namespace Library.DbContext.Entities
{
    // PackageTranport
    
    public partial class PackageTranport
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Id kiện hàng được vận chuyển
        ///</summary>
        public int PackageId { get; set; } // PackageId

        ///<summary>
        /// Thời gian vận chuyển
        ///</summary>
        public DateTime Time { get; set; } // Time

        ///<summary>
        /// Id kho vận chuyển
        ///</summary>
        public int StoreId { get; set; } // StoreId

        ///<summary>
        /// Tên kho vận chuyển
        ///</summary>
        public string StoreName { get; set; } // StoreName (length: 300)

        ///<summary>
        /// Ghi chú vận chuyển
        ///</summary>
        public string Note { get; set; } // Note (length: 600)

        ///<summary>
        /// Địa chỉ vận chuyển
        ///</summary>
        public string Address { get; set; } // Address (length: 600)

        ///<summary>
        /// Loại hình vận chuyển
        ///</summary>
        public byte? Type { get; set; } // Type

        ///<summary>
        /// Tên loại hình vận chuyển
        ///</summary>
        public string TypeName { get; set; } // TypeName (length: 300)

        public PackageTranport()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
