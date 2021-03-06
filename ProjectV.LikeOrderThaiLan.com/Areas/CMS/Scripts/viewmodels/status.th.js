﻿statusApp = {

    // Trạng thái đơn hàng Order
    orderPackage: [
        { Name: "ร้านค้าจีนส่งสินค้า", Class: "label label-default", Ifa: "fa fa-file-o bg-gray" },
        { Name: "โกดังที่จีนรับสินค้า", Class: "label label-warning", Ifa: "fa fa-clock-o bg-yellow" },
        { Name: "อยู่ในโกดังที่จีน", Class: "label label-warning", Ifa: "fa fa-calendar-check-o bg-blue" },
        { Name: "ออกจากโกดังที่จีน", Class: "label label-info", Ifa: "fa fa-exclamation-triangle bg-yellow" },
        { Name: "กำลัังส่งมาเมืองไทย", Class: "label label-warning", Ifa: "fa fa-cart-plus bg-gray" },
        { Name: "รับสินค้า", Class: "label label-warning", Ifa: "fa fa-cart-arrow-down bg-yellow" },
        { Name: "สินค้าในโกดัง", Class: "label label-warning", Ifa: "fa fa-exclamation-triangle bg-yellow" },
        { Name: "กำลังจัดการ", Class: "label label-warning", Ifa: "fa fa-cc-visa bg-yellow" },
        { Name: "รอจัดส่ง", Class: "label label-success", Ifa: "fa fa-check-circle-o bg-green" },
        { Name: "กำำลังจัดส่ง", Class: "label label-primary", Ifa: "fa fa-archive bg-blue" },
        { Name: "ส่งสินค้าแล้ว", Class: "label label-success", Ifa: "fa fa-cubes bg-yellow" },
        { Name: "สินค้้าหาย", Class: "label label-danger", Ifa: "fa fa-truck bg-aqua" },
        { Name: "สินค้าหายรหัส", Class: "label label-danger", Ifa: "fa fa-truck bg-yellow" },
        { Name: "สินค้าหายได้จัดการเเล้ว", Class: "label label-danger", Ifa: "fa fa-home bg-green" }
    ],
    // Trạng thái đơn hàng Order
    orderPackageView: [
        { Name: "ร้านค้าจีนส่งสินค้า", Class: "label label-default", Ifa: "fa fa-file-o bg-gray" },
        { Name: "โกดังที่จีนรับสินค้า", Class: "label label-warning", Ifa: "fa fa-clock-o bg-yellow" },
        { Name: "อยู่ในโกดังที่จีน", Class: "label label-warning", Ifa: "fa fa-calendar-check-o bg-blue" },
        { Name: "ออกจากโกดังที่จีน", Class: "label label-warning", Ifa: "fa fa-exclamation-triangle bg-yellow" },
        { Name: "กำลัังส่งมาเมืองไทย", Class: "label label-info", Ifa: "fa fa-cart-plus bg-gray" },
        { Name: "กำลัังส่งมาเมืองไทย", Class: "label label-info", Ifa: "fa fa-cart-arrow-down bg-yellow" },
        { Name: "อยูุ่ในโกดังไทย", Class: "label label-info", Ifa: "fa fa-exclamation-triangle bg-yellow" },
        { Name: "กำลังจัดการ", Class: "label label-info", Ifa: "fa fa-cc-visa bg-yellow" },
        { Name: "รอจัดส่ง", Class: "label label-primary", Ifa: "fa fa-check-circle-o bg-green" },
        { Name: "กำำลังจัดส่ง", Class: "label label-primary", Ifa: "fa fa-archive bg-blue" },
        { Name: "ส่งสินค้าแล้ว", Class: "label label-success", Ifa: "fa fa-cubes bg-yellow" },
        { Name: "สินค้้าหาย", Class: "label label-danger", Ifa: "fa fa-truck bg-aqua" },
        { Name: "สินค้้าหาย", Class: "label label-danger", Ifa: "fa fa-truck bg-yellow" },
        { Name: "สินค้้าหาย", Class: "label label-danger", Ifa: "fa fa-home bg-green" }
    ],
    complain: [
        { Name: "รอจัดการ", Key: "", Class: "label label-default" },
        { Name: "กำลังจัดการ", Key: "", Class: "label label-warning" },
        { Name: "รอสั่งซื้อ xử lý", Key: "", Class: "label label-warning" },
        { Name: "รอฝ่ายดูเเลลูกค้าจัดการ", Key: "", Class: "label label-warning" },
        { Name: "รอคอนเฟิร์ม", Key: "", Class: "label label-warning" },
        { Name: "รอฝ่ายบัญชีจัดการ", Key: "", Class: "label label-warning" },
        { Name: "บัญชีรับเงินคืน", Class: "label label-success" },
        { Name: "สำเร็จ", Key: "", Class: "label label-success" },
        { Name: "ยกเลิกแล้ว", Key: "", Class: "label label-danger" },
    ],
    complainView: [
        { Name: "รอจัดการ", Key: "", Class: "label label-default" },
        { Name: "กำลังจัดการ", Key: "", Class: "label label-warning" },
        { Name: "กำลังจัดการ", Key: "", Class: "label label-warning" },
        { Name: "กำลังจัดการ", Key: "", Class: "label label-warning" },
        { Name: "กำลังจัดการ", Key: "", Class: "label label-warning" },
        { Name: "กำลังจัดการ", Key: "", Class: "label label-warning" },
        { Name: "บัญชีรับเงินคืน", Class: "label label-success" },
        { Name: "สำเร็จ", Key: "", Class: "label label-success" },
        { Name: "ยกเลิกแล้ว", Key: "", Class: "label label-danger" },
    ],
    source: [
        { Name: "รอจัดการ", Key: "", Class: "label label-default" },
        { Name: "กำลังจัดการ", Key: "", Class: "label label-warning" },
        { Name: "รอลูกค้าเลือกเเหน่งให้สินค้า", Key: "", Class: "label label-warning" },
        { Name: "สำเร็จ", Key: "", Class: "label label-success" },
        { Name: "ยกเลิกแล้ว", Key: "", Class: "label label-danger" },
    ],
    deposit: [
        { Name: "รอประเมินราคา", Key: "WaitDeposit", Value: 0 },
        { Name: "รอจัดการ", Key: "Processing", Value: 1 },
        { Name: "รอจัดการ", Key: "PendingPrice", Value: 2 },
        { Name: "รอสรุปออเดอร์", Key: "WaitOrder", Value: 3 },
        { Name: "รอสินค้าเข้าโกดัง", Key: "Order", Value: 4 },
        { Name: "สินค้าในโกดัง", Key: "InWarehouse", Value: 5 },
        { Name: "กำลังขนส่ง", Key: "Shipping", Value: 6 },
        { Name: "รอจัดส่ง", Key: "Pending", Value: 7 },
        { Name: "ส่งสินค้าสำเร็จ", Key: "PendingSuccess", Value: 8 },
        { Name: "สำเร็จ", Key: "Finish", Value: 9 },
        { Name: "ยกเลิก", Key: "Cancel", Value: 10 },
    ],
    order: [
        { Name: "ใหม่", Key: "New", Value: 0 },
        { Name: "รอประเมินราคา", Key: "WaitPrice", Value: 1 },
        { Name: "กำลังประเมินราคา", Key: "AreQuotes", Value: 2 },
        { Name: "รอมัดจำ", Key: "WaitDeposit", Value: 3 },
        { Name: "รอสั่งซื้อ", Key: "WaitOrder", Value: 4 },
        { Name: "กำลังซื้อเสร็จ", Key: "Order", Value: 5 },
        { Name: "รอฝ่ายบัญชีชำระ", Key: "WaitAccountant", Value: 6 },
        { Name: "ฝ่ายบัญชีกำลังชำระ", Key: "AccountantProcessing", Value: 7 },
        { Name: "สั่งซื้อเสร็จ", Key: "OrderSuccess", Value: 8 },
        { Name: "ร้านค้าส่งสินค้า", Key: "DeliveryShop", Value: 9 },
        { Name: "รับสินค้า", Key: "InWarehouse", Value: 10 },
        { Name: "กำลังขนส่ง", Key: "Shipping", Value: 11 },
        { Name: "รอจัดส่ง", Key: "Pending", Value: 12 },
        { Name: "Đang thành công", Key: "PendingSuccess", Value: 13 },
        { Name: "สำเร็จ", Key: "Finish", Value: 14 },
        { Name: "ยกเลิก", Key: "Cancel", Value: 15 },
        { Name: "หาย", Key: "Lost", Value: 16 },
    ],
    OrderServices: [
        { Name: "การซื้อสินค้าค่าบริการ:", Key: "Order", Value: 0 },
        { Name: "ค่าบริการตรวจนับ", Key: "Audit", Value: 1 },
        { Name: "ค่าบริการตีลังไม้", Key: "Packing", Value: 2 },
        { Name: "ค่าบริการร้านค้าขนส่ง", Key: "ShopShipping", Value: 3 },
        { Name: "ค่าบริการขนส่งมาเมืองไทย", Key: "OutSideShipping", Value: 4 },
        { Name: "ค่าบริการขนส่งมาถึงบ้าน", Key: "InSideShipping", Value: 5 },
        { Name: "ค่าบริการขนส่งสินค้าด่วน (ทางอากาศ)", Key: "FastDelivery", Value: 6 },
        { Name: "ที่เกิดขึ้น", Key: "Other", Value: 7 },
        { Name: "ค่าใส่กระสอบ", Key: "PackingCharge", Value: 8 },
    ],
    // Trạng thái đơn hàng Order
    orderView: [
        { Name: "", Class: "label label-default", Ifa: "fa fa-file-o bg-gray" },
        { Name: "รอประเมินราคา", Class: "label label-default", Ifa: "fa fa-clock-o bg-yellow" },
        { Name: "รอประเมินราคา", Class: "label label-default", Ifa: "fa fa-calendar-check-o bg-blue" },
        { Name: "รอมัดจำ", Class: "label label-chodatcoc", Ifa: "fa fa-exclamation-triangle bg-yellow" },
        { Name: "รอสั่งซื้อ", Class: "label label-orangered", Ifa: "fa fa-cart-plus bg-gray" },
        { Name: "กำลังซื้อเสร็จ", Class: "label label-warning", Ifa: "fa fa-cart-arrow-down bg-yellow" },
        { Name: "กำลังซื้อเสร็จ", Class: "label label-warning", Ifa: "fa fa-exclamation-triangle bg-yellow" },
        { Name: "กำลังซื้อเสร็จ", Class: "label label-warning", Ifa: "fa fa-cc-visa bg-yellow" },
        { Name: "สั่งซื้อเสร็จ", Class: "label label-success", Ifa: "fa fa-check-circle-o bg-green" },
        { Name: "ร้านค้าส่งสินค้า", Class: "label label-primary", Ifa: "fa fa-archive bg-blue" },
        { Name: "LikeOrder รับสินค้า", Class: "label label-nhkd", Ifa: "fa fa-cubes bg-yellow" },
        { Name: "กำลังขนส่ง", Class: "label label-info", Ifa: "fa fa-truck bg-aqua" },
        { Name: "รอจัดส่ง", Class: "label label-info", Ifa: "fa fa-truck bg-yellow" },
        { Name: "กำลังส่งสินค้า", Class: "label label-dgh", Ifa: "fa fa-home bg-green" },
        { Name: "สำเร็จ", Class: "label label-success", Ifa: "fa fa-check bg-aqua" },
        { Name: "ยกเลิก", Class: "label label-danger", Ifa: "fa fa-times-circle bg-red" },
        { Name: "หาย", Class: "label label-danger2", Ifa: "fa user-times bg-red" }
    ],
    // Tạng thái đơn hàng ký gửi
    depositView: [
        { Name: "รอประเมินราคา", Class: "label label-default" },
        { Name: "รอจัดการ", Class: "label label-warning" },
        { Name: "รอจัดการ", Class: "label label-warning" },
        { Name: "รอสรุปออเดอร์", Class: "label label-primary" },
        { Name: "รอสินค้าเข้าโกดัง", Class: "label label-orangered" },
        { Name: "สินค้าในโกดัง", Class: "label label-info" },
        { Name: "กำลังขนส่ง", Class: "label label-transport" },
        { Name: "รอจัดส่ง", Class: "label label-info" },
        { Name: "กำลังส่งสินค้า", Class: "label label-green" },
        { Name: "สำเร็จ", Class: "label label-success" },
        { Name: "ยกเลิก", Class: "label label-danger" }
    ],
}