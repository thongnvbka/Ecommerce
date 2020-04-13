using System;

namespace Library.DbContext.Entities
{
    // OrderProcessItem
    
    public partial class OrderProcessItem
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Id OrderProcess - Phiếu xử lý đơn hàng
        ///</summary>
        public int? OrderProcessId { get; set; } // OrderProcessId

        ///<summary>
        /// Mã Code OrderProcess - Phiếu xử lý đơn hàng
        ///</summary>
        public string OrderProcessCode { get; set; } // OrderProcessCode (length: 20)

        ///<summary>
        /// Id Đơn hàng của khách hàng
        ///</summary>
        public int OrderId { get; set; } // OrderId

        ///<summary>
        /// Mã Code đơn hàng của khách hàng
        ///</summary>
        public int OrderCode { get; set; } // OrderCode

        ///<summary>
        /// Mã đơn hàng của Shop
        ///</summary>
        public string OrderShopCode { get; set; } // OrderShopCode (length: 100)

        ///<summary>
        /// Mã hợp đồng của Shop
        ///</summary>
        public string OrderShopContract { get; set; } // OrderShopContract (length: 100)

        ///<summary>
        /// Mã vận đơn (Mã kiện) Shop báo về cho nhân viên
        ///</summary>
        public string LadingCode { get; set; } // LadingCode (length: 100)

        ///<summary>
        /// Id Khách hàng đặt hàng
        ///</summary>
        public int CustomerId { get; set; } // CustomerId

        ///<summary>
        /// Mã Code khách hàng đặt hàng
        ///</summary>
        public string CustomerCode { get; set; } // CustomerCode (length: 20)

        ///<summary>
        /// Tên khách hàng đặt hàng
        ///</summary>
        public string CustomerName { get; set; } // CustomerName (length: 300)

        ///<summary>
        /// Điện thoại khách hàng đặt hàng
        ///</summary>
        public string CustomerPhone { get; set; } // CustomerPhone (length: 50)

        ///<summary>
        /// Email khách hàng đặt hàng
        ///</summary>
        public string CustomerEmail { get; set; } // CustomerEmail (length: 300)

        ///<summary>
        /// Địa chỉ khách hàng đặt hàng
        ///</summary>
        public string CustomerAddress { get; set; } // CustomerAddress (length: 300)

        ///<summary>
        /// Id kho hàng kiện hàng dự kiến về
        ///</summary>
        public int? WarehouseDesId { get; set; } // WarehouseDesId

        ///<summary>
        /// Mã kho hàng kiện hàng này dự kiến về
        ///</summary>
        public string WarehouseCode { get; set; } // WarehouseCode (length: 20)

        ///<summary>
        /// Tên kho hàng kiện hàng này dự kiến về
        ///</summary>
        public string WarehouseName { get; set; } // WarehouseName (length: 300)

        ///<summary>
        /// Địa chỉ kho hàng kiện hàng này dự kiến về
        ///</summary>
        public string WarehouseAddress { get; set; } // WarehouseAddress (length: 300)
        public bool IsDelete { get; set; } // IsDelete
        public DateTime? Created { get; set; } // Created
        public DateTime? LastUpdated { get; set; } // LastUpdated

        public OrderProcessItem()
        {
            Created = DateTime.Now;
            LastUpdated = DateTime.Now;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
