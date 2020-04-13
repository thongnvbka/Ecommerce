using System;
using System.Collections.Generic;

namespace Library.DbContext.Entities
{

    // DepositDetail

    public partial class DepositDetail
    {
        public long Id { get; set; } // Id (Primary key)
        public DateTime CreateDate { get; set; } // CreateDate
        public DateTime UpdateDate { get; set; } // UpdateDate
        public int DepositId { get; set; } // DepositId
        public string LadingCode { get; set; } // LadingCode (length: 50)
        public double Weight { get; set; } // Weight

        ///<summary>
        /// Id nhóm sản phẩm
        ///</summary>
        public int CategoryId { get; set; } // CategoryId

        ///<summary>
        /// Tên nhóm sản phẩm
        ///</summary>
        public string CategoryName { get; set; } // CategoryName (length: 300)

        ///<summary>
        /// Tên sản phẩm
        ///</summary>
        public string ProductName { get; set; } // ProductName (length: 300)

        ///<summary>
        /// Số lượng sản phẩm
        ///</summary>
        public int Quantity { get; set; } // Quantity

        ///<summary>
        /// Kích cỡ sản phẩm
        ///</summary>
        public string Size { get; set; } // Size (length: 50)

        ///<summary>
        /// Ảnh
        ///</summary>
        public string Image { get; set; } // Image

        ///<summary>
        /// Khách hàng ghi chú cho sản phẩm
        ///</summary>
        public string Note { get; set; } // Note (length: 600)
        public int PacketNumber { get; set; } // PacketNumber
        public bool IsDelete { get; set; } // IsDelete
        /// <summary>
        /// chiều dài
        /// </summary>
        public double Long { get; set; } // Long
        /// <summary>
        /// chiều rộng
        /// </summary>
        public double Wide { get; set; } // Wide
        /// <summary>
        /// chiều cao
        /// </summary>
        public double High { get; set; } // High
        ///<summary>
        /// Danh sách mã vận đơn
        ///</summary>
        public string ListCode { get; set; } // ListCode (length: 1000)

        public List<ContractCode> ListContractCode // danh sách vận đơn
        {
            get; set;
        }
        public decimal? ShipTq { get; set; }
        public DepositDetail()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

    public partial class ContractCode
    {
        public int Id { get; set; } 
       
        public int ParentId { get; set; } 
       
        public string Code { get; set; }
    }
}
