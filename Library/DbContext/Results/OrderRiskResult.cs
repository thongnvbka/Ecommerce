using Library.DbContext.Entities;
using System;
using System.Collections.Generic;

namespace Library.DbContext.Results
{
    public class OrderRiskResult
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã đơn hàng
        ///</summary>
        public string Code { get; set; } // Code (length: 30)

        ///<summary>
        /// Loại đơn hàng: Ký gửi, Order,..
        ///</summary>
        public byte Type { get; set; } // Type

        ///<summary>
        /// Trạng thái đơn hàng
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Id hệ thống
        ///</summary>
        public int SystemId { get; set; } // SystemId

        ///<summary>
        /// Đơn hàng phát sinh từ hệ thống
        ///</summary>
        public string SystemName { get; set; } // SystemName (length: 100)

        ///<summary>
        /// Thời gian tạo đơn hàng
        ///</summary>
        public DateTime Created { get; set; } // Created

        ///<summary>
        /// Thời gian cập nhật gần đây
        ///</summary>
        public DateTime LastUpdate { get; set; } // LastUpdate

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

        public int? UserId { get; set; }

        ///<summary>
        /// Email nhân viên
        ///</summary>
        public string UserEmail { get; set; } // CustomerEmail (length: 300)

        ///<summary>
        /// Điện thoại khách hàng
        ///</summary>
        public string CustomerPhone { get; set; } // CustomerPhone (length: 300)

        public string CustomerAddress { get; set; } // CustomerPhone (length: 500)

        ///<summary>
        /// Số lượng kiện hàng đã về kho
        ///</summary>
        public int? PackageNoInStock { get; set; } // PackageNoInStock

        ///<summary>
        /// Số kiện
        ///</summary>
        public int? PackageNo { get; set; } // PackageNo

        //danh sách kiện
        public string PackageCodes { get; set; }

        public List<OrderPackage> Packages { get; set; }

        ///<summary>
        /// Số mã vận đơn
        ///</summary>
        public int? ContractCodeNo { get; set; } // ContractCode (length: 50)

        /// <summary>
        /// Ghi chú cho đơn hàng
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Ghi chú của đặt hàng
        /// </summary>
        public string UserNote { get; set; }

        public byte? BargainType { get; set; } // BargainType

                                               ///<summary>
                                               /// Nguồn Order từ website
                                               ///</summary>
        public string WebsiteName { get; set; } // WebsiteName (length: 300)
    }
}