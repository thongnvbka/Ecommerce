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
        { Name: "รอเเจ้งราคา", Class: "label label-default" },
        { Name: "กำลังจัดการ", Class: "label label-default" },
        { Name: "รอคอนเฟิร์มราคา", Class: "label label-danger" },
        { Name: "รอสร้างออเดอร์", Class: "label label-success" },
        { Name: "รอรับสินค้า", Class: "label label-warning" },
        { Name: "โกดังรับสินค้า", Class: "label label-info" },
        { Name: "ขนส่ง", Class: "label label-info" },
        { Name: "รอส่งของ", Class: "label label-info" },
        { Name: "กำลังส่งของ", Class: "label label-info" },
        { Name: "สำเร็จ", Class: "label label-primary" },
        { Name: "Cancel", Class: "label label-default" }
    ],
    //trạng thái của Detail Orders
    orderDetail: [
        { Name: "สั่งของเรียบร้อย", Class: "label label-success" },
        { Name: "Cancel", Class: "label label-warning" },
        { Name: "รอเเจ้งราคา", Class: "label label-warning" }
    ],
    //trạng thái package
    orderPackage: [
        { Name: "Shop จีน ส่งสินค้า", Class: "label label-default" },
        { Name: "โกดังที่จีนรับสินค้า", Class: "label label-info" },
        { Name: "กำลังอยู่ในโกดังที่จีน", Class: "label label-primary" },
        { Name: "สินค้าออกจากโกดังจีน", Class: "label label-warning" },
        { Name: "สินค้ากำลังขนส่งกลับไทย", Class: "label label-warning" },
        { Name: "โกดังรับสินค้า", Class: "label label-warning" },
        { Name: "สินค้าในโกดัง", Class: "label label-warning" },
        { Name: "กำลังสั่งขนส่ง", Class: "label label-warning" },
        { Name: "รอส่งของ", Class: "label label-warning" },
        { Name: "กำลังส่งสินค้า", Class: "label label-warning" },
        { Name: "สินค้าถึงลูกค้าเรียบร้อย", Class: "label label-warning" },
        { Name: "สินค้าหาย", Class: "label label-warning" },
        { Name: "สินค้าหายรหัส", Class: "label label-warning" },
        { Name: "สินค้าหายได้จัดการเเล้ว", Class: "label label-warning" }
    ],
    //Trạng thái đơn tìm nguồn
    source: [
        { Name: "รอจัดการ", Class: "label label-default" },
        { Name: "กำลังจัดการ", Class: "label label-info" },
        { Name: "รอลูกค้าเลือกเเหน่งให้สินค้า", Class: "label label-warning" },
        { Name: "สำเร็จ", Class: "label label-primary" },
        { Name: "ยกเลิกเเล้ว", Class: "label label-danger" } 
    ],
    sourceDetail: [
        { Name: "สั่งได้", Class: "label label-success" },
        { Name: "Cancel", Class: "label label-warning" }
    ],

    // Trạng thái phiếu nhập kho
    importwarehouse: [
        { Name: "เพิ่งเปิด", Class: "label label-default" },
        { Name: "ตรวจสอบเเล้ว", Class: "label label-success" } 
    ],

    wallet: [
        { Name: "เพิ่งเปิด", Class: "label label-default" },
        { Name: "ตรวจสอบเเล้ว", Class: "label label-success" },
        { Name: "สินค้าในโกดัง", Class: "label label-default" },
        { Name: "กำลังขนส่ง", Class: "label label-default" },
        { Name: "หาย", Class: "label label-default" },
        { Name: "สำเร็จ", Class: "label label-success" } 
    ],

    exportwarehouse: [
        { Name: "เพิ่งเปิด", Class: "label label-default" },
        { Name: "ตรวจสอบเเล้ว", Class: "label label-success" } 
    ],

    exportwarehouseType: [
        { Name: "กระสอบสินค้า", Class: "label label-default" },
        { Name: "ลังสินค้า", Class: "label label-success" } 
    ],

    expectedtransport: [
        { Name: "Tiểu ngạch", Class: "label label-danger" },
        { Name: "Chính ngạch", Class: "label label-success" } 
    ],

    packinglist: [
        { Name: "เพิ่งเปิด", Class: "label label-default" },
        { Name: "ตรวจสอบเเล้ว", Class: "label label-success" } 
    ],

    delivery: [
        { Name: "รอตรวจสอบ", Class: "label label-default" },
        { Name: "ตรวจสอบเเล้ว", Class: "label label-default" },
        { Name: "ส่งของเรียบร้อย", Class: "label label-default" },
        { Name: "ส่งของเรียบร้อย", Class: "label label-default" },
        { Name: "สำเร็จ", Class: "label label-success" },
        { Name: "Cancel", Class: "label label-danger" },
        { Name: "Hoàn phiếu", Class: "label label-success" } 
    ],

    // Trạng thái khách hàng
    customerOfStaffStatus: [
        { Name: "กำลังเปิด", Class: "label label-success" },
        { Name: "ถูกลบ", Class: "label label-warning" },
        { Name: "ถูกลบ", Class: "label label-default" } 
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
        { Name: "รอจัดการ", Class: "label label-default" },
        { Name: "กำลังจัดการ", Class: "label label-warning" },
        { Name: "รอสั่งสินค้าจัดการ", Class: "label label-order" },
        { Name: "รอฝ่ายดูเเลลูกค้าจัดการ", Class: "label label-care" },
        { Name: "รอคอนเฟิร์ม", Class: "label btn-info" },
        { Name: "รอฝ่ายบัญชีจัดการ", Class: "label label-account" },
        { Name: "ฝ่ายบัญชีคืนเงินเเล้ว", Class: "label label-success" },
        { Name: "สำเร็จ", Class: "label label-success" },
        { Name: "ยกเลิกเเล้ว", Class: "label label-danger" }
    ],


    // Type FundBill 
    typeFundBill: [
        { Name: "เพิ่มเงินเข้ากองทุน", Class: "label label-success" },
        { Name: "จ่ายจากกองทุน", Class: "label label-danger" } 
    ],
    // Status FundBill 
    statusFundBill: [
        { Name: "รอตรวจสอบ", Class: "label label-danger" },
        { Name: "ตรวจสอบเเล้ว", Class: "label label-success" } 
    ],

    // Type RechargeBill 
    typeRechargeBill: [
        { Name: "เติมเงินเข้าบัญชี", Class: "label label-success" },
        { Name: "จ่ายเงินจากบัญชี", Class: "label label-danger" } 
    ],

    // Status RechargeBill 
    statusRechargeBill: [
        { Name: "รอตรวจสอบ", Class: "label label-danger" },
        { Name: "ตรวจสอบเเล้ว", Class: "label label-success" } 
    ],

    // Status MustConllect 
    statusMustCollect: [
        { Name: "รอชำระเงิน", Class: "label label-danger" },
        { Name: "ชำระเงินเรียบร้อย", Class: "label label-success" } 
    ],

    // Status MustReturn 
    statusMustReturn: [
        { Name: "รอคืนเงิน", Class: "label label-danger" },
        { Name: "คืนเงินเรียบร้อย", Class: "label label-success"} 
    ],

    statusWithDrawal: [
        { Name: "Chờ xử lý", Class: "label label-danger" },
        { Name: "Đã xử lý", Class: "label label-success" },
    ],

    //Trạng thái Contract code
    statusContractCode: [
        { Name: "New", Class: "label label-default" },
        { Name: "สั่งของ ตรวจของ", Class: "label label-danger" },
        { Name: "รอชำระ", Class: "label label-warning" },
        { Name: "ชำระเเล้ว", Class: "label label-success" }
    ],

    //Trạng thái phiếu Refund
    statusClaimForRefund: [
        { Name: "รอสั่งสินค้าจัดการ", Class: "label label-order" },
        { Name: "รอฝ่ายดูเเลลูกค้าจัดการ", Class: "label label-care" },
        { Name: "รอคอนเฟิร์ม", Class: "label btn-info" },
        { Name: "รอฝ่ายบัญชีจัดการ", Class: "label label-account" },
        { Name: "เสร็จสิ้น", Class: "label label-success" },
        { Name: "ยกเลิก คืนของ ชดใช้", Class: "label label-danger" }
    ],

    //Trạng thái đối tác vận chuyển
    statusPartner: [
        { Name: "Đối tác mới" },
        { Name: "Đối tác hiện tại" },
        { Name: "Đối tác cũ" }
    ],

    //Trạng thái của khách hàng tiêm năng
    statusPotentialCustomer: [
        { Name: "เปิดใช้งาน", Class: "label label-success" },
        { Name: "ปิดใช้งาน", Class: "label label-warning" },
        { Name: "รอเปลี่ยนเเปลง", Class: "label label-default" } 
    ],

    //Trạng thái yêu cầu ship
    statusRequestShip: [
        { Name: "Chưa xử lý", Class: "label label-warning" },
        { Name: "Đã xử lý", Class: "label label-success" },
    ],

    // Loai cong no
    typeDebit: [
        { Name: "จำเป็นต้องเก็บ", Class: "label label-warning" },
        { Name: "จำเป็นต้องคืน", Class: "label label-success" } 
    ],

    // Trang thai cong no
    StatusDebitHistory: [
        { Name: "ยังไม่เสร็จ", Class: "label label-warning" },
        { Name: "สำเร็จ", Class: "label label-success" } 
    ]
}