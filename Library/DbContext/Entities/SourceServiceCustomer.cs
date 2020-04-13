using System;

namespace Library.DbContext.Entities
{

    // SourceServiceCustomer

    public partial class SourceServiceCustomer
    {
        public long Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã khách hàng
        ///</summary>
        public int CustomerId { get; set; } // CustomerId
        public string CustomerName { get; set; } // CustomerName (length: 255)

        ///<summary>
        /// Mã dịch vụ tìm nguồn
        ///</summary>
        public int SourceServiceId { get; set; } // SourceServiceId
        public string SourceServiceName { get; set; } // SourceServiceName (length: 255)

        ///<summary>
        /// Ngày bắt đầu
        ///</summary>
        public DateTime? StartDate { get; set; } // StartDate

        ///<summary>
        /// Ngày kết thúc
        ///</summary>
        public DateTime? FinishDate { get; set; } // FinishDate

        ///<summary>
        /// Ngày tạo
        ///</summary>
        public DateTime? CreateDate { get; set; } // CreateDate

        ///<summary>
        /// Ngày cập nhật
        ///</summary>
        public DateTime? UpdateDate { get; set; } // UpdateDate
        public bool? IsActive { get; set; } // IsActive
        public int? CreateId { get; set; } // CreateId
        public string CreateName { get; set; } // CreateName (length: 255)
        public int? UpdateId { get; set; } // UpdateId
        public string UpdateName { get; set; } // UpdateName (length: 255)
        public SourceServiceCustomer()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
