statusApp = {

    // Trạng thái Orders Order
    order: [
        { Name: "Mới", Class: "label label-default", Ifa: "fa fa-file-o bg-gray" },
        { Name: "Chờ báo giá", Class: "label label-warning", Ifa: "fa fa-clock-o bg-yellow" },
        { Name: "Đang báo giá", Class: "label label-warning", Ifa: "fa fa-calendar-check-o bg-blue" },
        { Name: "Chờ Deposit", Class: "label label-info", Ifa: "fa fa-exclamation-triangle bg-yellow" },
        { Name: "Chờ đặt hàng", Class: "label label-warning", Ifa: "fa fa-cart-plus bg-gray" },
        { Name: "Đang đặt hàng", Class: "label label-warning", Ifa: "fa fa-cart-arrow-down bg-yellow" },
        { Name: "Chờ kế toán thanh toán", Class: "label label-warning", Ifa: "fa fa-exclamation-triangle bg-yellow" },
        { Name: "Kế toán đang thanh toán", Class: "label label-warning", Ifa: "fa fa-cc-visa bg-yellow" },
        { Name: "Đặt hàng xong", Class: "label label-success", Ifa: "fa fa-check-circle-o bg-green" },
        { Name: "Shop phát hàng", Class: "label label-primary", Ifa: "fa fa-archive bg-blue" },
        { Name: "Nhận hàng", Class: "label label-info", Ifa: "fa fa-cubes bg-yellow" },
        { Name: "Vận chuyển", Class: "label label-info", Ifa: "fa fa-truck bg-aqua" },
        { Name: "Wait for delivery", Class: "label label-info", Ifa: "fa fa-truck bg-yellow" },
        { Name: "Đang giao hàng", Class: "label label-info", Ifa: "fa fa-home bg-green" },
        { Name: "Hoàn thành", Class: "label label-success", Ifa: "fa fa-check bg-aqua" },
        { Name: "Hủy", Class: "label label-danger", Ifa: "fa fa-times-circle bg-red" },
        { Name: "Hỏng mất", Class: "label label-danger", Ifa: "fa user-times bg-red" }
    ],
    // Tạng thái Orders ký gửi
    deposit: [
        { Name: "Chờ báo giá", Class: "label label-default" },
        { Name: "Đang xử lý", Class: "label label-default" },
        { Name: "Chờ duyệt giá", Class: "label label-danger" },
        { Name: "Chờ kết đơn", Class: "label label-success" },
        { Name: "Chờ nhận hàng", Class: "label label-warning" },
        { Name: "Nhận hàng", Class: "label label-info" },
        { Name: "Vận chuyển", Class: "label label-info" },
        { Name: "Wait for delivery", Class: "label label-info" },
        { Name: "Đang giao hàng", Class: "label label-info" },
        { Name: "Hoàn thành", Class: "label label-primary" },
        { Name: "Hủy", Class: "label label-default" }
    ],
    //trạng thái của Detail Orders
    orderDetail: [
        { Name: "Đặt được hàng", Class: "label label-success" },
        { Name: "Hủy", Class: "label label-warning" },
        { Name: "Chờ báo giá", Class: "label label-warning" }
    ],
    //trạng thái package
    orderPackage: [
        { Name: "Shop TQ phát hàng", Class: "label label-default" },
        { Name: "Kho TQ nhận hàng", Class: "label label-info" },
        { Name: "Đang trong kho TQ", Class: "label label-primary" },
        { Name: "Xuất kho TQ", Class: "label label-warning" },
        { Name: "Trên đường về VN", Class: "label label-warning" },
        { Name: "Nhận hàng", Class: "label label-warning" },
        { Name: "Trong kho", Class: "label label-warning" },
        { Name: "Đang điều chuyển", Class: "label label-warning" },
        { Name: "Wait for delivery", Class: "label label-warning" },
        { Name: "Đang đi giao hàng", Class: "label label-warning" },
        { Name: "Đã trả hàng", Class: "label label-warning" },
        { Name: "Hàng bị mất", Class: "label label-warning" },
        { Name: "Hàng bị mất mã", Class: "label label-warning" },
        { Name: "Hàng bị mất Processed", Class: "label label-warning" }
    ],
    //Trạng thái đơn tìm nguồn
    source: [
        { Name: "Waiting", Class: "label label-default" },
        { Name: "Đang xử lý", Class: "label label-info" },
        { Name: "Chờ khách chọn NCC", Class: "label label-warning" },
        { Name: "Hoàn thành", Class: "label label-primary" },
        { Name: "Cancelled", Class: "label label-danger" },
    ],
    sourceDetail:[
        { Name: "Đặt được", Class: "label label-success" },
        { Name: "Hủy", Class: "label label-warning" }
    ],

    // Trạng thái phiếu nhập kho
    importwarehouse: [
        { Name: "Mới khởi tạo", Class: "label label-default" },
        { Name: "Đã duyệt", Class: "label label-success" } 
    ],

    wallet: [
         { Name: "Mới khởi tạo", Class: "label label-default" },
        { Name: "Đã duyệt", Class: "label label-success" },
        { Name: "Trong kho", Class: "label label-default" },
        { Name: "Đang vận chuyển", Class: "label label-default" },
        { Name: "Mất", Class: "label label-default" },
        { Name: "Hoàn thành", Class: "label label-success" } 
    ],

    exportwarehouse: [
        { Name: "Mới khởi tạo", Class: "label label-default" },
        { Name: "Đã duyệt", Class: "label label-success" } 
    ],

    exportwarehouseType: [
        { Name: "Bao hàng", Class: "label label-default" },
        { Name: "package", Class: "label label-success" },
    ],

    expectedtransport: [
        { Name: "Tiểu ngạch", Class: "label label-danger" },
        { Name: "Chính ngạch", Class: "label label-success" } 
    ],

    packinglist: [
        { Name: "Mới khởi tạo", Class: "label label-default" },
        { Name: "Đã duyệt", Class: "label label-success" } 
    ],

    delivery: [
        { Name: "Chờ duyệt", Class: "label label-default" },
        { Name: "Đã duyệt", Class: "label label-default" },
        { Name: "Đã xuất giao", Class: "label label-default" },
        { Name: "Đã xuất giao", Class: "label label-default" },
        { Name: "Hoàn thành", Class: "label label-success" },
        { Name: "Hủy", Class: "label label-danger" },
        { Name: "Hoàn phiếu", Class: "label label-success" } 
    ],

    // Trạng thái khách hàng
    customerOfStaffStatus: [
        { Name: "Đang mở", Class: "label label-success" },
        { Name: "Tạm ngưng", Class: "label label-warning" },
        { Name: "Deleted", Class: "label label-default" },
    ],

    //ten he thong 
    systemConfig: [
        { Name: "orderhangkinhdoanh.com", Class: "" },
        { Name: "nhaphangkinhdoanh.com", Class: "" },
        { Name: "nhaphangkinhdoanh.com", Class: "" },
        { Name: "timnguonhanggiare.com", Class: "" },
        { Name: "timnguonhangtrungquoc.com", Class: "" },
        { Name: "dathangweb.com", Class: "" } 

    ],

    //Trạng thái của đơn khiếu nại 
    statusComplain: [
        { Name: "Waiting", Class: "label label-default" },
        { Name: "Đang xử lý", Class: "label label-warning" },
        { Name: "Chờ đặt hàng xử lý", Class: "label label-order" },
        { Name: "Chờ CSKH xử lý", Class: "label label-care" },
        { Name: "Chờ phê duyệt", Class: "label btn-info" },
        { Name: "Chờ kế toán xử lý", Class: "label label-account" },
        { Name: "Kế toán đã Refund", Class: "label label-success" },
        { Name: "Hoàn thành", Class: "label label-success" },
        { Name: "Cancelled", Class: "label label-danger" }
    ],


    // Type FundBill 
    typeFundBill: [
        { Name: "Charge", Class: "label label-success" },
        { Name: "spend money funds", Class: "label label-danger" },
    ],
    // Status FundBill 
    statusFundBill: [
        { Name: "Chờ duyệt", Class: "label label-danger" },
        { Name: "Đã duyệt", Class: "label label-success" } 
    ],

    // Type RechargeBill 
    typeRechargeBill: [
        { Name: "Nạp tiền ví", Class: "label label-success" },
        { Name: "Trừ tiền ví", Class: "label label-danger" } 
    ],

    // Status RechargeBill 
    statusRechargeBill: [
        { Name: "Chờ duyệt", Class: "label label-danger" },
        { Name: "Đã duyệt", Class: "label label-success" } 
    ],

    // Status MustConllect 
    statusMustCollect: [
        { Name: "Chờ thu", Class: "label label-danger" },
        { Name: "Hoàn tất thu", Class: "label label-success" } 
    ],

    // Status MustReturn 
    statusMustReturn: [
        { Name: "Chờ trả", Class: "label label-danger" },
        { Name: "Hoàn tất trả", Class: "label label-success" } 
    ],

    statusWithDrawal: [
        { Name: "Waiting", Class: "label label-danger" },
        { Name: "Processed", Class: "label label-success" },
    ],

    //Trạng thái Contract code
    statusContractCode: [
       { Name: "Mới", Class: "label label-default" },
       { Name: "Đặt hàng kiểm tra lại", Class: "label label-danger" },
       { Name: "Chờ thanh toán", Class: "label label-warning" },
       { Name: "Đã thanh toán", Class: "label label-success" }
    ],

    //Trạng thái phiếu Refund
    statusClaimForRefund: [
        { Name: "Chờ đặt hàng xử lý", Class: "label label-order" },
        { Name: "Chờ CSKH xử lý", Class: "label label-care" },
        { Name: "Chờ phê duyệt", Class: "label btn-info" },
        { Name: "Chờ kế toán xử lý", Class: "label label-account" },
        { Name: "Hoàn tất", Class: "label label-success" },
        { Name: "Hủy hoàn bồi", Class: "label label-danger" }
    ],

    //Trạng thái đối tác vận chuyển
    statusPartner: [
        { Name: "Đối tác mới"},
        { Name: "Đối tác hiện tại"},
        { Name: "Đối tác cũ"}
    ],

    //Trạng thái của khách hàng tiêm năng
    statusPotentialCustomer: [
        { Name: "Kích hoạt", Class: "label label-success" },
        { Name: "Chưa kích hoạt", Class: "label label-warning" },
        { Name: "Chờ chuyển chính thức", Class: "label label-default" }  
    ],

    //Trạng thái yêu cầu ship
    statusRequestShip: [
        { Name: "Chưa xử lý", Class: "label label-warning" },
        { Name: "Processed", Class: "label label-success" },
    ],

    // Loai cong no
    typeDebit: [
        { Name: "Phải thu", Class: "label label-warning" },
        { Name: "Phải trả", Class: "label label-success" } 
    ],

// Trang thai cong no
    StatusDebitHistory: [
        { Name: "Chưa hoàn thành", Class: "label label-warning" },
        { Name: "Hoàn thành", Class: "label label-success" } 
    ]
}