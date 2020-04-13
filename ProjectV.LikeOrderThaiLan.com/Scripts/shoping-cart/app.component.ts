import { Component } from '@angular/core';
import { Http } from '@angular/http';
import * as _ from 'lodash';

declare var $;
declare var toastr;
declare var swal;
declare var orderDetails;
declare var window;
declare var resource;

@Component({
    selector: 'shoping-cart',
    templateUrl: 'scripts/shoping-cart/app.html'
})

export class AppComponent {
    orders = [];
    totalPrice = 0;
    orderNo = 0;
    linkNo = 0;
    productNo = 0;
    resource = resource;
    orderDetails;

    checkedAll = false;
    token = "";
    isLoading = false;
    constructor(private http: Http) {
        if (window["orderDetails"])
            this.orderDetails = orderDetails;

        let self = this;
        this.token = $("input[name='__RequestVerificationToken']").val();
        let noMesssage;
        // Callback sự kiện bên Extension
        window.addEventListener("message",
            (request) => {
                if (request.data['data'] && request.data['data']['likeorder_products']) {
                    //self.loadData(request.data);
                    let data = self.loadData(request.data);

                    this.requestData(data, (rs) => {
                        if (!rs.hasOwnProperty('status'))
                            window.postMessage({ type: "CLEAR_DATA" }, 'https://likeorder.com');
                        // Xóa dữ liệu bên Extension
                    });
                    clearTimeout(noMesssage);
                }
            }, false);

        // Gọi request lấy dữ liệu từ Extension
        setTimeout(() => {
            window.postMessage({ type: "REQUEST_DATA" }, 'https://likeorder.com');
            noMesssage = setTimeout(() => {
                this.requestData(null, null);
            }, 300);
        }, 300);
    }

    ngAfterViewInit() {
        // Sản phẩm Shop yêu cầu mua tối thiểu
        if (this.orderDetails) {
            let names = _.map(this.orderDetails, (d: any) => {
                return `- Shop "${d.shopName}" ความต้องการซื้อขั้นต่ำ "${d.name}" là "${d.beginAmount}" สินค้า`;
               // return `- Shop "${d.shopName}" yêu cầu mua tổi thiếu sản phẩm "${d.name}" là "${d.beginAmount}" sản phẩm`;
            });

            let htmlContent = _.join(names, '<br />');

            swal({
                type: 'warning',
                html: htmlContent,
                showCloseButton: true,
                showCancelButton: false,
                confirmButtonText: 'ยกเลิก'
                //confirmButtonText: 'Đóng lại'
            });
        }
    }

    requestData(data, callback) {
        let objData = <any>{
            __RequestVerificationToken: this.token
        }

        if (data) {
            objData.shopingCarts = data;

            // format lại kiểu dữ liệu decimal

        }

        this.isLoading = true;
        let self = this;
        $.post("/" + window.culture + "/Product/AddOrder", objData, (result) => {
            self.isLoading = false;

            if (!result["status"])
                // Cache giá trị số lượng
                _.each(result,
                    (o: any[]) => {
                        _.each(o.products,
                            (p: any) => {
                                p.cacheQuantity = p.quantity;
                            });
                    });

            this.orders = result;

            if (callback)
                callback(result);

        });
    }

    // Cập nhật thông tin sản phẩm trong giỏ hàng
    updateProduct(product) {
        $.post("/" + window.culture + "/Product/UpdateProduct",
            {
                __RequestVerificationToken: this.token,
                model: {
                    id: product.id,
                    note: product.note,
                    quantity: product.quantity
                }
            },
            (result) => {
                if (result && result.status < 0) {
                    toastr.warning(result.text);
                }
            });
    }

    // Cập nhật thông tin đơn hàng
    updateOrder(order) {
        $.post("/" + window.culture + "/Product/UpdateOrder",
            {
                __RequestVerificationToken: this.token,
                model: {
                    id: order.id,
                    note: order.note,
                    privateNote: order.privateNote,
                    serviceType: order.serviceType
                }
            },
            (result) => {
                if (result && result.status < 0) {
                    toastr.warning(result.text);
                }
            });
    }

    // Cập nhật tùy chọn dịch vụ của đơn hàng
    updateService(id, orderId, checked) {
        $.post("/" + window.culture + "/Product/UpdateOrderService",
            {
                __RequestVerificationToken: this.token,
                model: {
                    id, orderId, checked
                }
            },
            (result) => {
                if (result && result.status < 0) {
                    toastr.warning(result.text);
                }
            });
    }

    // Xóa sản phẩm trong giỏ hàng
    deleteProduct(product, order) {
        swal({
            title:window.ShoppingCart.error.TitleDelete,//title: 'Bạn có chắc chắn muốn xóa sản phẩm này?',
            //title: 'คุณแน่ใจหรือว่าต้องการลบเมนูประจำวัน?',//title: 'Bạn có chắc chắn muốn xóa sản phẩm này?',
           text: window.ShoppingCart.error.ConfirmDelete,//text: "Sau khi xóa là không thể phục hồi lại!",
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            cancelButtonText: window.ShoppingCart.error.Huy,
            confirmButtonText: window.ShoppingCart.error.DongY
        })
            .then(() => {
                if (product.proId != null) {
                    let products = _.filter(order.products, (x: any) => x.link === product.link);
                    let totalQuantity = _.sumBy(products, "quantity");

                    // Số sản phẩm nhỏ hơn số sản phẩm yêu cầu của Shop
                    if (products.length > 1 && (totalQuantity - product.quantity) < product.beginAmount) {
                        toastr.warning("Shop" + window.ShoppingCart.error.MuaToiThieu + product.beginAmount + window.ShoppingCart.error.SanPham);
                       // toastr.warning("Shop " + window.ShoppingCart.error.MuaToiThieu + product.beginAmount + window.ShoppingCart.error.SanPham);;
                        return;
                    }
                }

                $.post("/" + window.culture + "/Product/DeleteProduct",
                    {
                        __RequestVerificationToken: this.token,
                        id: product.id
                    },
                    (result) => {
                        if (result && result.status < 0) {
                            toastr.warning(result.text);
                        } else {
                            toastr.success(result.text);
                            if (order.products.length === 1) {
                                _.remove(this.orders,
                                    (o: any) => {
                                        return o.id === order.id;
                                    });

                                // Tính lại summary
                                if (this.orderNo > 0)
                                    this.summary();
                                return;
                            }

                            // Tính lại giá tiền
                            if (product.proId != null) {
                                let products = _.filter(order.products, (x: any) => x.link === product.link);
                                let totalQuantity = _.sumBy(products, "quantity");
                                totalQuantity -= product.quantity;

                                // Tính lại giá cho sản phẩm
                                if (product.prices.length > 0) {
                                    let price = _.find(product.prices,
                                        (x: any) => (x.begin <= totalQuantity && x.end >= totalQuantity) ||
                                            (x.begin <= totalQuantity && x.end == null));

                                    if (price) {
                                        let totalProductQuantity = _.sumBy(order.products, "quantity");
                                        // Cập nhật lại tiền sản phẩm
                                        _.each(products,
                                            pd => {
                                                pd.price = price.price;
                                                pd.exchangePrice = pd.price * pd.exchangeRate;
                                                pd.totalPrice = pd.price * pd.quantity;
                                                pd.totalExchange = pd.price * pd.exchangeRate * pd.quantity;
                                                pd.auditPrice = this.orderAudit(totalProductQuantity, pd.price);
                                            });
                                    }
                                }
                            }

                            order.linkNo -= 1;
                            order.productNo -= product.quantity;
                            order.totalPrice -= product.totalPrice;
                            order.totalExchange -= product.totalExchange;

                            _.remove(order.products,
                                (p: any) => {
                                    return p.id === product.id;
                                });

                            // Tính lại phí đặt hàng theo gói dịch vụ
                            //this.orderPrice(order);

                            // Cập nhật lại giá tính sản phẩm
                            this.orderCountPrice(order);
                        }
                    });
            }, () => { });
    }

    finishOne(order) {
        this.processFinish([order]);
    }

    finishMutiple() {
        if (this.orderNo === 0)
            return;

        let orders = _.filter(this.orders, (x) => { return x.checked; });

        this.processFinish(orders);
    }

    processFinish(orders: any[]) {
        if (this.checkMinQuantityOrders(orders) === false)
            return;

        let codes = _.map(orders, "id");

        let strCodes = '%3B' + _.join(codes, '%3B') + '%3B';

        window.location.href = "/" + window.culture + "/Product/Deposit/" + strCodes;
    }

    // Kiểm tra các đơn hàng có sản phẩm mua nhỏ hơn số lượng yêu cầu của shop
    checkMinQuantityOrders(orders: any[]): boolean {
        let allFinish = true;

        _.each(orders,
            (o) => {
                _.each(o.products,
                    (product) => {
                        let products = _.filter(o.products, (x: any) => x.link === product.link);
                        let totalQuantity = _.sumBy(products, "quantity");

                        // Số sản phẩm nhỏ hơn số sản phẩm yêu cầu của Shop
                        if (totalQuantity < product.beginAmount) {
                           // toastr.warning(`Shop "${product.shopName}" yêu cầu mua thối thiểu là "${product
                            toastr.warning(`Shop "${product.shopName}" `+ window.ShoppingCart.error.MuaToiThieu +` "${product
                                .beginAmount}"`,
                                window.ShoppingCart.error.SanPham+ `"${product.name}"`);
                               // `Sản phẩm "${product.name}"`);

                            allFinish = false;
                            return false;
                        }
                    });
            });

        return allFinish;
    }


    changeCheckAllOrder() {
        this.checkedAll = !this.checkedAll;

        _.each(this.orders, (order) => {
            order.checked = this.checkedAll;
        });

        // Tính lại summary
        this.summary();
    }

    changeCheckOrder(order) {
        order.checked = !order.checked;

        let count = _.countBy(this.orders, "checked");

        if (count["true"] && count["true"] === this.orders.length) {
            this.checkedAll = true;
        } else {
            this.checkedAll = false;
        }

        // Tính lại summary
        this.summary();
    }

    // Tính thông số order đã chọn
    summary() {
        this.orderNo = _.sumBy(this.orders,
            (o) => {
                return o.checked ? 1 : 0;
            });

        this.linkNo = _.sumBy(this.orders,
            (o) => {
                return o.checked ? o.linkNo : 0;
            });

        this.productNo = _.sumBy(this.orders,
            (o) => {
                return o.checked ? o.productNo : 0;
            });

        this.totalPrice = _.sumBy(this.orders,
            (o) => {
                return o.checked ? o.total : 0;
            });
    }

    removeOrder(order) {
        swal({
            title:window.ShoppingCart.error.TitleDelete,//title: 'Bạn có chắc chắn muốn xóa sản phẩm này?',
            //title: 'คุณแน่ใจหรือว่าต้องการลบเมนูประจำวัน?',// title: 'Bạn có chắc chắn muốn xóa đơn hàng này?',
           text: window.ShoppingCart.error.ConfirmDelete,//text: "Sau khi xóa là không thể phục hồi lại!",
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            cancelButtonText: window.ShoppingCart.error.Huy, 
            confirmButtonText: window.ShoppingCart.error.DongY 
        })
            .then(() => {
                $.post("/" + window.culture + "/Product/DeleteOrder",
                    {
                        __RequestVerificationToken: this.token,
                        orderModels: [{ id: order.id, shopName: order.shopName }]
                    },
                    (result) => {
                        if (result && result.status < 0) {
                            toastr.warning(result.text);
                        } else {
                            toastr.success(result.text);

                            _.remove(this.orders,
                                (o: any) => {
                                    return o.id === order.id;
                                });

                            // Tính lại summary
                            if (this.orderNo > 0)
                                this.summary();
                        }
                    });
            }, () => { });
    }

    removeOrderSelected() {
        swal({
            title:window.ShoppingCart.error.TitleDelete,//title: 'Bạn có chắc chắn muốn xóa sản phẩm này?',
            //title: 'คุณแน่ใจหรือว่าต้องการลบเมนูประจำวัน?',// title: 'Bạn có chắc chắn muốn xóa đơn hàng này?',
            text: window.ShoppingCart.error.ConfirmDelete,// text: "Sau khi xóa là không thể phục hồi lại!",
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            cancelButtonText: window.ShoppingCart.error.Huy,
            confirmButtonText: window.ShoppingCart.error.DongY
        })
            .then(() => {
                let model = _.map(_.filter(this.orders, x => x.checked === true),
                    (x) => { return { id: x.id, shopName: x.shopName } });

                let orderIds = _.map(model, "id");

                $.post("/" + window.culture + "/Product/DeleteOrder",
                    {
                        __RequestVerificationToken: this.token,
                        orderModels: model
                    },
                    (result) => {
                        if (result && result.status < 0) {
                            toastr.warning(result.text);
                        } else {
                            toastr.success(result.text);

                            _.remove(this.orders,
                                (o: any) => {
                                    return orderIds.indexOf(o.id) >= 0;
                                });
                            this.orderNo = 0;
                            this.summary();
                        }
                    });
            }, () => { });
    }

    // Thay đổi số lượng sản phẩm
    blurQuantity(product: any, order) {
        // Không sửa số lượng
        if (parseInt(product.quantity) === product.cacheQuantity)
            return;

        if (product.quantity > product.max) {
            // Cancel event
            product.quantity = product.max;
            product.cacheQuantity = product.max;
        }

        if (product.proId != null) {
            let products = _.filter(order.products, (x: any) => x.link === product.link);
            let totalQuantity = _.sumBy(products, "quantity");

            // Số sản phẩm nhỏ hơn số sản phẩm yêu cầu của Shop
            if (totalQuantity < product.beginAmount) {
                product.quantity = product.cacheQuantity;
                toastr.warning("Shop " + window.ShoppingCart.error.MuaToiThieu + product.beginAmount + window.ShoppingCart.error.SanPham);
                //toastr.warning("Shop " + window.ShoppingCart.error.MuaToiThieu + product.beginAmount + window.ShoppingCart.error.SanPham);;
            }

            // Tính lại giá cho sản phẩm
            if (product.prices.length > 0) {
                let price = _.find(product.prices,
                    (x: any) => (x.begin <= totalQuantity && x.end >= totalQuantity) ||
                        (x.begin <= totalQuantity && x.end == null));

                if (price) {
                    let totalProductQuantity = _.sumBy(order.products, "quantity");
                    // Cập nhật lại tiền sản phẩm
                    _.each(products,
                        pd => {
                            pd.price = price.price;
                            pd.exchangePrice = pd.price * pd.exchangeRate;
                            pd.totalPrice = pd.price * pd.quantity;
                            pd.totalExchange = pd.price * pd.exchangeRate * pd.quantity;
                            pd.auditPrice = this.orderAudit(totalProductQuantity, pd.price);
                        });
                }
            }
        }
        // Cập nhật số tiền của sản phẩm
        product.totalPrice = product.price * product.quantity;
        product.totalExchange = product.totalPrice * product.exchangeRate;

        // Cập nhật số tiền của đơn hàng
        order.totalExchange = _.sumBy(order.products, "totalExchange");
        // Cập nhật số lượng sản phẩm
        order.productNo = _.sumBy(order.products, "quantity");

        // Tính lại phí đặt hàng
        //this.orderPrice(order);

        // Tính lại phí kiểm đếm
        this.orderCountPrice(order);

        // Update sản phẩm trên database
        this.updateProduct(product);

        product.cacheQuantity = product.quantity;
    }

    // Thay đổi dịch vụ tùy chọn
    changeService(order, service) {
        service.checked = !service.checked;

        // Thay đổi dịch vụ kiểm đếm
        if (service.serviceId === 0) {
            this.orderCountPrice(order);
        }

        // Cập nhật dịch vụ lên database
        this.updateService(service.serviceId, order.id, service.checked);
    }

    // Thay đổi gói dịch vụ
    changeServiceType(order, serviceType) {
        order.serviceType = serviceType;

        // Tính lại phí đặt hàng theo gói dịch vụ
        //this.orderPrice(order);

        // Cập nhật thông tin đơn hàng trên database
        this.updateOrder(order);
    }

    // Tính phí kiểm đếm
    orderCountPrice(order) {
        let service = _.find(order.services, (s: any) => { return s.serviceId === 0; });

        if (service && !service.checked) {
            service.totalPrice = 0;
            // Tổng tiền với các dịch vụ đi kèm
            order.total = _.sumBy(order.services, "totalPrice") + order.totalExchange;
            // Tính lại summary
            if (this.orderNo > 0) {
                this.summary();
            }
            return;
        }

        let totalProductQuantity = _.sumBy(order.products, "quantity");
        _.each(order.products, (p: any) => {
            p.auditPrice = this.orderAudit(totalProductQuantity, p.price);
        });

        service.totalPrice = _.sumBy(order.products,
            (p: any) => {
                return p.quantity * p.auditPrice;
            });

        // Tổng tiền với các dịch vụ đi kèm
        order.total = _.sumBy(order.services, "totalPrice") + order.totalExchange;

        // Tính lại summary
        if (this.orderNo > 0) {
            this.summary();
        }
    }

    // Tính phí đặt  hàng theo gói dịch vụ
    orderPrice(order) {
        let totalPrice = order.totalExchange;
        let orderPrice;

        if (totalPrice < 1000000) {
            orderPrice = totalPrice * 12 / 100;
        } else if (totalPrice >= 1000000 && totalPrice < 2000000) {
            orderPrice = totalPrice * 8 / 100;
        } else if (totalPrice >= 2000000 && totalPrice < 30000000) {
            orderPrice = totalPrice * 4 / 100;
        } else if (totalPrice >= 30000000 && totalPrice < 50000000) {
            orderPrice = totalPrice * 3.5 / 100;
        } else if (totalPrice >= 50000000 && totalPrice < 100000000) {
            orderPrice = totalPrice * 3 / 100;
        } else if (totalPrice >= 100000000 && totalPrice < 200000000) {
            orderPrice = totalPrice * 2.5 / 100;
        } else {
            orderPrice = totalPrice * 2 / 100;
        }

        // Đơn hàng nhỏ hơn 2 triệu bị tính 150.000 vnđ
        if (totalPrice < 2000000) {
            orderPrice = 150000;
        }

        let service = _.find(order.services,
            (s: any) => {
                return s.serviceId === 0;
            });

        orderPrice = orderPrice < 5000 ? 5000 : orderPrice;

        // Triết khấu cấp độ VIP
        if (window.vipLevel.order > 0) {
            orderPrice = orderPrice - orderPrice * window.vipLevel.order / 100;
        }

        if (service)
            service.totalPrice = orderPrice;
        // Tổng tiền với các dịch vụ đi kèm
        order.total = _.sumBy(order.services, "totalPrice") + totalPrice;

        // Tính lại summary
        if (this.orderNo > 0) {
            this.summary();
        }
    }

    /// <summary>
    /// Tính chi phí kiểm đếm
    /// </summary>
    /// <param name="quantity">Số lượng sản phẩm/phụ kiện kiểm đếm</param>
    /// <param name="productPrice">Giá sản phẩm đơn vị tiền CNY</param>
    /// <returns>Số tiền kiểm đếm / 1 sản phẩm</returns>
    orderAudit(quantity, productPrice) {
        //            1 - 2 sản phẩm         8 bath
        //            3 - 10 sản phẩm       5 bath
        //            11 - 100 sản phẩm    3 bath
        //            101 - 500 sản phẩm  2 bath
        //            > 500 sản phẩm      1 bath

        let price;
        if (quantity <= 2) {
            price = 8;
        }
        else if (quantity <= 10) {
            price = 5;
        }
        else if (quantity <= 100) {
            price = 3;
        }
        else if (quantity <= 500) {
            price = 2;
        }
        else {
            price = 1;
        }

        return price;

        // Giá sản phẩm nhỏ hơn 10 tệ tính là phụ kiện
        //return productPrice < 10 ? this.orderAuditExtension(quantity) : this.orderAuditProduct(quantity);
    }

    /// <summary>
    /// Tính chi phí kiểm đếm sản phẩm
    /// </summary>
    /// <param name="quantity">Số lượng sản phẩm kiểm đếm</param>
    /// <returns>Số tiền kiểm đếm / 1 sản phẩm</returns>
    orderAuditProduct(quantity) {
        let price = 0;
        if (quantity <= 2) {
            price = 5000;
        }
        else if (quantity >= 3 && quantity <= 10) {
            price = 3500;
        }
        else if (quantity >= 11 && quantity <= 100) {
            price = 2000;
        }
        else if (quantity >= 101 && quantity <= 500) {
            price = 1500;
        }
        else if (quantity > 500) {
            price = 1000;
        }

        return price;
    }

    /// <summary>
    /// Tính chi phí kiểm đếm phụ kiện
    /// </summary>
    /// <param name="quantity">Số lượng phụ kiện kiểm đếm</param>
    /// <returns>Số tiền kiểm đếm / 1 sản phẩm</returns>
    orderAuditExtension(quantity) {
        let price = 0;
        if (quantity <= 2) {
            price = 1500;
        }
        else if (quantity >= 3 && quantity <= 10) {
            price = 1000;
        }
        else if (quantity >= 11 && quantity <= 100) {
            price = 700;
        }
        else if (quantity >= 101 && quantity <= 500) {
            price = 700;
        }
        else if (quantity > 500) {
            price = 700;
        }

        return price;
    }

    loadData(data) {
        let products = data['data']['likeorder_products'];

        let groupByShop = <any>_.groupBy(products, "shop_link");

        let orders = [];
        // Each orders
        _.each(_.keys(groupByShop), (shipLink) => {
            let order = <any>{};
            order.shopName = groupByShop[shipLink][0]["shop_nick"];
            order.shopLink = groupByShop[shipLink][0]["shop_link"];
            order.products = [];
            order.serviceType = 0; // 0: Kinh doanh, 1: Tiêu dùng
            order.note = "";
            order.privateNote = "";

            // Each products of order
            _.each(groupByShop[shipLink],
                (p) => {
                    let product = <any>{};

                    product.name = p.name;
                    product.image = p.image;
                    product.note = p.note;
                    product.link = p.pro_link;
                    product.price = p.price;
                    product.quantity = p.amount;
                    product.shopName = p.shop_nick;
                    product.shopLink = p.shop_link;
                    product.exchangeRate = p.rate;
                    product.max = p.max;

                    if (isNaN(parseInt(p["min"]))) {
                        product.min = 1;
                    } else {
                        product.min = parseInt(p["min"]);
                    }

                    // Options propertis
                    if (p["price_arr"] && _.isArray(p["price_arr"]))
                        product.prices = p["price_arr"];

                    if (p["proId"])
                        product.proId = p["proId"];

                    if (p["skullId"])
                        product.proId = p["skullId"];

                    if (p["beginAmount"])
                        product.beginAmount = p["beginAmount"];

                    // Array Properties
                    product.propeties = [];
                    if (p["color"]) {
                        product.propeties.push({ name: 'color', label: window.ShoppingCart.error.MauSac , text: p["colortxt"] });
                       // product.propeties.push({ name: 'color', label: 'Màu', text: p["colortxt"] });
                    }
                    if (p["size"]) {
                        product.propeties.push({ name: 'size', label: 'Size', text: p["sizetxt"] });
                    }
                    order.products.push(product);
                });
            orders.push(order);
        });

        return orders;
    }
}
