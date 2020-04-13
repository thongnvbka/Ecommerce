using Library.DbContext.Entities;
using Library.ViewModels.Items;
using System.Collections.Generic;


namespace Library.ViewModels.Account
{
    public class OrderDetailModel
    {
        //Danh sách kiện hàng trong đơn hàng
        public List<OrderPackageItem> ListOrderPackage { get; set; }

        //Danh sách chi tiết sản phẩm trong đơn hàng
        public List<ProductDetailItem> ListProductDetail { get; set; }

        //Danh sách dịch vụ được sử dụng trong đơn hàng
        public List<OrderServiceItem> ListOrderService { get; set; }

        //Chi tiết đơn hàng Order
        public OrderDetailItem OrderDetailItem { get; set; }

        //Chi tiết địa chỉ nhận hàng
        public OrderAddress OrderAddress { get; set; }

        //Chi tiết trao đổi trong đơn hàng
        public List<OrderComment> ListOrderComment { get; set; }

        //Chi tiết yêu cầu Ship hàng
        public List<RequestShipItem> ListRequestShip { get; set; }
    }
}
