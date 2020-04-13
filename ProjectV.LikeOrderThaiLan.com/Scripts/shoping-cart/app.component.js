"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var core_1 = require("@angular/core");
var http_1 = require("@angular/http");
var _ = require("lodash");
var AppComponent = (function () {
    function AppComponent(http) {
        var _this = this;
        this.http = http;
        this.orders = [];
        this.totalPrice = 0;
        this.orderNo = 0;
        this.linkNo = 0;
        this.productNo = 0;
        this.resource = resource;
        this.checkedAll = false;
        this.token = "";
        this.isLoading = false;
        if (window["orderDetails"])
            this.orderDetails = orderDetails;
        var self = this;
        this.token = $("input[name='__RequestVerificationToken']").val();
        var noMesssage;
        // Callback sự kiện bên Extension
        window.addEventListener("message", function (request) {
            if (request.data['data'] && request.data['data']['likeorder_products']) {
                //self.loadData(request.data);
                var data = self.loadData(request.data);
                _this.requestData(data, function (rs) {
                    if (!rs.hasOwnProperty('status'))
                        window.postMessage({ type: "CLEAR_DATA" }, 'https://likeorder.com');
                    // Xóa dữ liệu bên Extension
                });
                clearTimeout(noMesssage);
            }
        }, false);
        // Gọi request lấy dữ liệu từ Extension
        setTimeout(function () {
            window.postMessage({ type: "REQUEST_DATA" }, 'https://likeorder.com');
            noMesssage = setTimeout(function () {
                _this.requestData(null, null);
            }, 300);
        }, 300);
    }
    AppComponent.prototype.ngAfterViewInit = function () {
        // Sản phẩm Shop yêu cầu mua tối thiểu
        if (this.orderDetails) {
            var names = _.map(this.orderDetails, function (d) {
                return "- Shop \"" + d.shopName + "\" \u0E04\u0E27\u0E32\u0E21\u0E15\u0E49\u0E2D\u0E07\u0E01\u0E32\u0E23\u0E0B\u0E37\u0E49\u0E2D\u0E02\u0E31\u0E49\u0E19\u0E15\u0E48\u0E33 \"" + d.name + "\" l\u00E0 \"" + d.beginAmount + "\" \u0E2A\u0E34\u0E19\u0E04\u0E49\u0E32";
                // return `- Shop "${d.shopName}" yêu cầu mua tổi thiếu sản phẩm "${d.name}" là "${d.beginAmount}" sản phẩm`;
            });
            var htmlContent = _.join(names, '<br />');
            swal({
                type: 'warning',
                html: htmlContent,
                showCloseButton: true,
                showCancelButton: false,
                confirmButtonText: 'ยกเลิก'
            });
        }
    };
    AppComponent.prototype.requestData = function (data, callback) {
        var _this = this;
        var objData = {
            __RequestVerificationToken: this.token
        };
        if (data) {
            objData.shopingCarts = data;
        }
        this.isLoading = true;
        var self = this;
        $.post("/" + window.culture + "/Product/AddOrder", objData, function (result) {
            self.isLoading = false;
            if (!result["status"])
                // Cache giá trị số lượng
                _.each(result, function (o) {
                    _.each(o.products, function (p) {
                        p.cacheQuantity = p.quantity;
                    });
                });
            _this.orders = result;
            if (callback)
                callback(result);
        });
    };
    // Cập nhật thông tin sản phẩm trong giỏ hàng
    AppComponent.prototype.updateProduct = function (product) {
        $.post("/" + window.culture + "/Product/UpdateProduct", {
            __RequestVerificationToken: this.token,
            model: {
                id: product.id,
                note: product.note,
                quantity: product.quantity
            }
        }, function (result) {
            if (result && result.status < 0) {
                toastr.warning(result.text);
            }
        });
    };
    // Cập nhật thông tin đơn hàng
    AppComponent.prototype.updateOrder = function (order) {
        $.post("/" + window.culture + "/Product/UpdateOrder", {
            __RequestVerificationToken: this.token,
            model: {
                id: order.id,
                note: order.note,
                privateNote: order.privateNote,
                serviceType: order.serviceType
            }
        }, function (result) {
            if (result && result.status < 0) {
                toastr.warning(result.text);
            }
        });
    };
    // Cập nhật tùy chọn dịch vụ của đơn hàng
    AppComponent.prototype.updateService = function (id, orderId, checked) {
        $.post("/" + window.culture + "/Product/UpdateOrderService", {
            __RequestVerificationToken: this.token,
            model: {
                id: id, orderId: orderId, checked: checked
            }
        }, function (result) {
            if (result && result.status < 0) {
                toastr.warning(result.text);
            }
        });
    };
    // Xóa sản phẩm trong giỏ hàng
    AppComponent.prototype.deleteProduct = function (product, order) {
        var _this = this;
        swal({
            title: window.ShoppingCart.error.TitleDelete,
            //title: 'คุณแน่ใจหรือว่าต้องการลบเมนูประจำวัน?',//title: 'Bạn có chắc chắn muốn xóa sản phẩm này?',
            text: window.ShoppingCart.error.ConfirmDelete,
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            cancelButtonText: window.ShoppingCart.error.Huy,
            confirmButtonText: window.ShoppingCart.error.DongY
        })
            .then(function () {
            if (product.proId != null) {
                var products = _.filter(order.products, function (x) { return x.link === product.link; });
                var totalQuantity = _.sumBy(products, "quantity");
                // Số sản phẩm nhỏ hơn số sản phẩm yêu cầu của Shop
                if (products.length > 1 && (totalQuantity - product.quantity) < product.beginAmount) {
                    toastr.warning("Shop" + window.ShoppingCart.error.MuaToiThieu + product.beginAmount + window.ShoppingCart.error.SanPham);
                    // toastr.warning("Shop " + window.ShoppingCart.error.MuaToiThieu + product.beginAmount + window.ShoppingCart.error.SanPham);;
                    return;
                }
            }
            $.post("/" + window.culture + "/Product/DeleteProduct", {
                __RequestVerificationToken: _this.token,
                id: product.id
            }, function (result) {
                if (result && result.status < 0) {
                    toastr.warning(result.text);
                }
                else {
                    toastr.success(result.text);
                    if (order.products.length === 1) {
                        _.remove(_this.orders, function (o) {
                            return o.id === order.id;
                        });
                        // Tính lại summary
                        if (_this.orderNo > 0)
                            _this.summary();
                        return;
                    }
                    // Tính lại giá tiền
                    if (product.proId != null) {
                        var products = _.filter(order.products, function (x) { return x.link === product.link; });
                        var totalQuantity_1 = _.sumBy(products, "quantity");
                        totalQuantity_1 -= product.quantity;
                        // Tính lại giá cho sản phẩm
                        if (product.prices.length > 0) {
                            var price_1 = _.find(product.prices, function (x) { return (x.begin <= totalQuantity_1 && x.end >= totalQuantity_1) ||
                                (x.begin <= totalQuantity_1 && x.end == null); });
                            if (price_1) {
                                var totalProductQuantity_1 = _.sumBy(order.products, "quantity");
                                // Cập nhật lại tiền sản phẩm
                                _.each(products, function (pd) {
                                    pd.price = price_1.price;
                                    pd.exchangePrice = pd.price * pd.exchangeRate;
                                    pd.totalPrice = pd.price * pd.quantity;
                                    pd.totalExchange = pd.price * pd.exchangeRate * pd.quantity;
                                    pd.auditPrice = _this.orderAudit(totalProductQuantity_1, pd.price);
                                });
                            }
                        }
                    }
                    order.linkNo -= 1;
                    order.productNo -= product.quantity;
                    order.totalPrice -= product.totalPrice;
                    order.totalExchange -= product.totalExchange;
                    _.remove(order.products, function (p) {
                        return p.id === product.id;
                    });
                    // Tính lại phí đặt hàng theo gói dịch vụ
                    //this.orderPrice(order);
                    // Cập nhật lại giá tính sản phẩm
                    _this.orderCountPrice(order);
                }
            });
        }, function () { });
    };
    AppComponent.prototype.finishOne = function (order) {
        this.processFinish([order]);
    };
    AppComponent.prototype.finishMutiple = function () {
        if (this.orderNo === 0)
            return;
        var orders = _.filter(this.orders, function (x) { return x.checked; });
        this.processFinish(orders);
    };
    AppComponent.prototype.processFinish = function (orders) {
        if (this.checkMinQuantityOrders(orders) === false)
            return;
        var codes = _.map(orders, "id");
        var strCodes = '%3B' + _.join(codes, '%3B') + '%3B';
        window.location.href = "/" + window.culture + "/Product/Deposit/" + strCodes;
    };
    // Kiểm tra các đơn hàng có sản phẩm mua nhỏ hơn số lượng yêu cầu của shop
    AppComponent.prototype.checkMinQuantityOrders = function (orders) {
        var allFinish = true;
        _.each(orders, function (o) {
            _.each(o.products, function (product) {
                var products = _.filter(o.products, function (x) { return x.link === product.link; });
                var totalQuantity = _.sumBy(products, "quantity");
                // Số sản phẩm nhỏ hơn số sản phẩm yêu cầu của Shop
                if (totalQuantity < product.beginAmount) {
                    // toastr.warning(`Shop "${product.shopName}" yêu cầu mua thối thiểu là "${product
                    toastr.warning("Shop \"" + product.shopName + "\" " + window.ShoppingCart.error.MuaToiThieu + (" \"" + product
                        .beginAmount + "\""), window.ShoppingCart.error.SanPham + ("\"" + product.name + "\""));
                    // `Sản phẩm "${product.name}"`);
                    allFinish = false;
                    return false;
                }
            });
        });
        return allFinish;
    };
    AppComponent.prototype.changeCheckAllOrder = function () {
        var _this = this;
        this.checkedAll = !this.checkedAll;
        _.each(this.orders, function (order) {
            order.checked = _this.checkedAll;
        });
        // Tính lại summary
        this.summary();
    };
    AppComponent.prototype.changeCheckOrder = function (order) {
        order.checked = !order.checked;
        var count = _.countBy(this.orders, "checked");
        if (count["true"] && count["true"] === this.orders.length) {
            this.checkedAll = true;
        }
        else {
            this.checkedAll = false;
        }
        // Tính lại summary
        this.summary();
    };
    // Tính thông số order đã chọn
    AppComponent.prototype.summary = function () {
        this.orderNo = _.sumBy(this.orders, function (o) {
            return o.checked ? 1 : 0;
        });
        this.linkNo = _.sumBy(this.orders, function (o) {
            return o.checked ? o.linkNo : 0;
        });
        this.productNo = _.sumBy(this.orders, function (o) {
            return o.checked ? o.productNo : 0;
        });
        this.totalPrice = _.sumBy(this.orders, function (o) {
            return o.checked ? o.total : 0;
        });
    };
    AppComponent.prototype.removeOrder = function (order) {
        var _this = this;
        swal({
            title: window.ShoppingCart.error.TitleDelete,
            //title: 'คุณแน่ใจหรือว่าต้องการลบเมนูประจำวัน?',// title: 'Bạn có chắc chắn muốn xóa đơn hàng này?',
            text: window.ShoppingCart.error.ConfirmDelete,
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            cancelButtonText: window.ShoppingCart.error.Huy,
            confirmButtonText: window.ShoppingCart.error.DongY
        })
            .then(function () {
            $.post("/" + window.culture + "/Product/DeleteOrder", {
                __RequestVerificationToken: _this.token,
                orderModels: [{ id: order.id, shopName: order.shopName }]
            }, function (result) {
                if (result && result.status < 0) {
                    toastr.warning(result.text);
                }
                else {
                    toastr.success(result.text);
                    _.remove(_this.orders, function (o) {
                        return o.id === order.id;
                    });
                    // Tính lại summary
                    if (_this.orderNo > 0)
                        _this.summary();
                }
            });
        }, function () { });
    };
    AppComponent.prototype.removeOrderSelected = function () {
        var _this = this;
        swal({
            title: window.ShoppingCart.error.TitleDelete,
            //title: 'คุณแน่ใจหรือว่าต้องการลบเมนูประจำวัน?',// title: 'Bạn có chắc chắn muốn xóa đơn hàng này?',
            text: window.ShoppingCart.error.ConfirmDelete,
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            cancelButtonText: window.ShoppingCart.error.Huy,
            confirmButtonText: window.ShoppingCart.error.DongY
        })
            .then(function () {
            var model = _.map(_.filter(_this.orders, function (x) { return x.checked === true; }), function (x) { return { id: x.id, shopName: x.shopName }; });
            var orderIds = _.map(model, "id");
            $.post("/" + window.culture + "/Product/DeleteOrder", {
                __RequestVerificationToken: _this.token,
                orderModels: model
            }, function (result) {
                if (result && result.status < 0) {
                    toastr.warning(result.text);
                }
                else {
                    toastr.success(result.text);
                    _.remove(_this.orders, function (o) {
                        return orderIds.indexOf(o.id) >= 0;
                    });
                    _this.orderNo = 0;
                    _this.summary();
                }
            });
        }, function () { });
    };
    // Thay đổi số lượng sản phẩm
    AppComponent.prototype.blurQuantity = function (product, order) {
        var _this = this;
        // Không sửa số lượng
        if (parseInt(product.quantity) === product.cacheQuantity)
            return;
        if (product.quantity > product.max) {
            // Cancel event
            product.quantity = product.max;
            product.cacheQuantity = product.max;
        }
        if (product.proId != null) {
            var products = _.filter(order.products, function (x) { return x.link === product.link; });
            var totalQuantity_2 = _.sumBy(products, "quantity");
            // Số sản phẩm nhỏ hơn số sản phẩm yêu cầu của Shop
            if (totalQuantity_2 < product.beginAmount) {
                product.quantity = product.cacheQuantity;
                toastr.warning("Shop " + window.ShoppingCart.error.MuaToiThieu + product.beginAmount + window.ShoppingCart.error.SanPham);
            }
            // Tính lại giá cho sản phẩm
            if (product.prices.length > 0) {
                var price_2 = _.find(product.prices, function (x) { return (x.begin <= totalQuantity_2 && x.end >= totalQuantity_2) ||
                    (x.begin <= totalQuantity_2 && x.end == null); });
                if (price_2) {
                    var totalProductQuantity_2 = _.sumBy(order.products, "quantity");
                    // Cập nhật lại tiền sản phẩm
                    _.each(products, function (pd) {
                        pd.price = price_2.price;
                        pd.exchangePrice = pd.price * pd.exchangeRate;
                        pd.totalPrice = pd.price * pd.quantity;
                        pd.totalExchange = pd.price * pd.exchangeRate * pd.quantity;
                        pd.auditPrice = _this.orderAudit(totalProductQuantity_2, pd.price);
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
    };
    // Thay đổi dịch vụ tùy chọn
    AppComponent.prototype.changeService = function (order, service) {
        service.checked = !service.checked;
        // Thay đổi dịch vụ kiểm đếm
        if (service.serviceId === 0) {
            this.orderCountPrice(order);
        }
        // Cập nhật dịch vụ lên database
        this.updateService(service.serviceId, order.id, service.checked);
    };
    // Thay đổi gói dịch vụ
    AppComponent.prototype.changeServiceType = function (order, serviceType) {
        order.serviceType = serviceType;
        // Tính lại phí đặt hàng theo gói dịch vụ
        //this.orderPrice(order);
        // Cập nhật thông tin đơn hàng trên database
        this.updateOrder(order);
    };
    // Tính phí kiểm đếm
    AppComponent.prototype.orderCountPrice = function (order) {
        var _this = this;
        var service = _.find(order.services, function (s) { return s.serviceId === 0; });
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
        var totalProductQuantity = _.sumBy(order.products, "quantity");
        _.each(order.products, function (p) {
            p.auditPrice = _this.orderAudit(totalProductQuantity, p.price);
        });
        service.totalPrice = _.sumBy(order.products, function (p) {
            return p.quantity * p.auditPrice;
        });
        // Tổng tiền với các dịch vụ đi kèm
        order.total = _.sumBy(order.services, "totalPrice") + order.totalExchange;
        // Tính lại summary
        if (this.orderNo > 0) {
            this.summary();
        }
    };
    // Tính phí đặt  hàng theo gói dịch vụ
    AppComponent.prototype.orderPrice = function (order) {
        var totalPrice = order.totalExchange;
        var orderPrice;
        if (totalPrice < 1000000) {
            orderPrice = totalPrice * 12 / 100;
        }
        else if (totalPrice >= 1000000 && totalPrice < 2000000) {
            orderPrice = totalPrice * 8 / 100;
        }
        else if (totalPrice >= 2000000 && totalPrice < 30000000) {
            orderPrice = totalPrice * 4 / 100;
        }
        else if (totalPrice >= 30000000 && totalPrice < 50000000) {
            orderPrice = totalPrice * 3.5 / 100;
        }
        else if (totalPrice >= 50000000 && totalPrice < 100000000) {
            orderPrice = totalPrice * 3 / 100;
        }
        else if (totalPrice >= 100000000 && totalPrice < 200000000) {
            orderPrice = totalPrice * 2.5 / 100;
        }
        else {
            orderPrice = totalPrice * 2 / 100;
        }
        // Đơn hàng nhỏ hơn 2 triệu bị tính 150.000 vnđ
        if (totalPrice < 2000000) {
            orderPrice = 150000;
        }
        var service = _.find(order.services, function (s) {
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
    };
    /// <summary>
    /// Tính chi phí kiểm đếm
    /// </summary>
    /// <param name="quantity">Số lượng sản phẩm/phụ kiện kiểm đếm</param>
    /// <param name="productPrice">Giá sản phẩm đơn vị tiền CNY</param>
    /// <returns>Số tiền kiểm đếm / 1 sản phẩm</returns>
    AppComponent.prototype.orderAudit = function (quantity, productPrice) {
        //            1 - 2 sản phẩm         8 bath
        //            3 - 10 sản phẩm       5 bath
        //            11 - 100 sản phẩm    3 bath
        //            101 - 500 sản phẩm  2 bath
        //            > 500 sản phẩm      1 bath
        var price;
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
    };
    /// <summary>
    /// Tính chi phí kiểm đếm sản phẩm
    /// </summary>
    /// <param name="quantity">Số lượng sản phẩm kiểm đếm</param>
    /// <returns>Số tiền kiểm đếm / 1 sản phẩm</returns>
    AppComponent.prototype.orderAuditProduct = function (quantity) {
        var price = 0;
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
    };
    /// <summary>
    /// Tính chi phí kiểm đếm phụ kiện
    /// </summary>
    /// <param name="quantity">Số lượng phụ kiện kiểm đếm</param>
    /// <returns>Số tiền kiểm đếm / 1 sản phẩm</returns>
    AppComponent.prototype.orderAuditExtension = function (quantity) {
        var price = 0;
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
    };
    AppComponent.prototype.loadData = function (data) {
        var products = data['data']['likeorder_products'];
        var groupByShop = _.groupBy(products, "shop_link");
        var orders = [];
        // Each orders
        _.each(_.keys(groupByShop), function (shipLink) {
            var order = {};
            order.shopName = groupByShop[shipLink][0]["shop_nick"];
            order.shopLink = groupByShop[shipLink][0]["shop_link"];
            order.products = [];
            order.serviceType = 0; // 0: Kinh doanh, 1: Tiêu dùng
            order.note = "";
            order.privateNote = "";
            // Each products of order
            _.each(groupByShop[shipLink], function (p) {
                var product = {};
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
                }
                else {
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
                    product.propeties.push({ name: 'color', label: window.ShoppingCart.error.MauSac, text: p["colortxt"] });
                }
                if (p["size"]) {
                    product.propeties.push({ name: 'size', label: 'Size', text: p["sizetxt"] });
                }
                order.products.push(product);
            });
            orders.push(order);
        });
        return orders;
    };
    return AppComponent;
}());
AppComponent = __decorate([
    core_1.Component({
        selector: 'shoping-cart',
        templateUrl: 'scripts/shoping-cart/app.html'
    }),
    __metadata("design:paramtypes", [http_1.Http])
], AppComponent);
exports.AppComponent = AppComponent;
//# sourceMappingURL=app.component.js.map