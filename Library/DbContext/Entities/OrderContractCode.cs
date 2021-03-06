﻿using System;

namespace Library.DbContext.Entities
{
    // OrderContractCode
    public partial class OrderContractCode
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã đơn hàng
        ///</summary>
        public int OrderId { get; set; } // OrderId

        ///<summary>
        /// Loại đơn hàng order, tìm nguồn
        ///</summary>
        public byte OrderType { get; set; } // OrderType

        ///<summary>
        /// Mã hợp đồng
        ///</summary>
        public string ContractCode { get; set; } // ContractCode (length: 50)

        ///<summary>
        /// Số tiền
        ///</summary>
        public decimal? TotalPrice { get; set; } // TotalPrice
        public bool IsDelete { get; set; } // IsDelete
        public DateTime CreateDate { get; set; } // CreateDate
        public DateTime UpdateDate { get; set; } // UpdateDate

        ///<summary>
        /// Trạng thái
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Ngày thanh toán
        ///</summary>
        public DateTime? AccountantDate { get; set; } // AccountantDate

        ///<summary>
        /// Mã nhân viên kế toán
        ///</summary>
        public int? AccountantId { get; set; } // AccountantId

        ///<summary>
        /// Họ tên nhân viên kế toán
        ///</summary>
        public string AccountantFullName { get; set; } // AccountantFullName (length: 50)

        ///<summary>
        /// Đơn vị nhân viên kế toán
        ///</summary>
        public int? AccountantOfficeId { get; set; } // AccountantOfficeId

        ///<summary>
        /// Tên đơn vị nhân viên kế toán
        ///</summary>
        public string AccountantOfficeName { get; set; } // AccountantOfficeName (length: 300)

        ///<summary>
        /// Id Path đơn vị nhân viên kế toán
        ///</summary>
        public string AccountantOfficeIdPath { get; set; } // AccountantOfficeIdPath (length: 300)

        /// <summary>
        /// 0: Hợp đồng cần thanh toán 1: Phí ship trung quốc
        /// </summary>
        public bool? IsShip { get; set; }
        public OrderContractCode()
        {
            AccountantFullName = "";
            IsShip = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
