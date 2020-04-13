function AcountingModel(imageViewModal) {
    var self = this;

    self.isLoading = ko.observable(false);
    self.isUploading = ko.observable(false);

    self.id = ko.observable(null);
    self.beginAmount = ko.observable(null);
    self.customerAddress = ko.observable(null);
    self.customerEmail = ko.observable(null);
    self.customerId = ko.observable(null);
    self.customerName = ko.observable(null);
    self.customerPhone = ko.observable(null);
    self.exchangePrice = ko.observable(null);
    self.exchangeRate = ko.observable(null);
    self.image = ko.observable(null);
    self.link = ko.observable(null);
    self.mode = ko.observable(null);
    self.name = ko.observable(null);
    self.note = ko.observable(null);
    self.notePrivate = ko.observable(null);
    self.officeId = ko.observable(null);
    self.officeIdPath = ko.observable(null);
    self.officeName = ko.observable(null);
    self.orderCode = ko.observable(null);
    self.orderDetailId = ko.observable(null);
    self.orderId = ko.observable(null);
    self.price = ko.observable(null);
    self.productNo = ko.observable(null);
    self.properties = ko.observableArray([]);
    self.quantity = ko.observable(null);
    self.quantityActual = ko.observable(null);
    self.quantityLose = ko.observable(null);
    self.shopId = ko.observable(null);
    self.shopLink = ko.observable(null);
    self.shopName = ko.observable(null);
    self.status = ko.observable(null);
    self.totalExchange = ko.observable(null);
    self.totalExchangeLose = ko.observable(null);
    self.totalExchangeShop = ko.observable(null);
    self.totalPrice = ko.observable(null);
    self.totalPriceCustomer = ko.observable(null);
    self.totalPriceLose = ko.observable(null);
    self.totalPriceShop = ko.observable(null);
    self.updated = ko.observable(null);
    self.userFullName = ko.observable(null);
    self.userId = ko.observable(null);
    self.warehouseId = ko.observable(null);
    self.warehouseName = ko.observable(null);
    self.websiteName = ko.observable(null);
    self.images = ko.observableArray([]);

    self.isShowInfo = ko.observable(false);

    self.changeIsShowInfo = function () {
        self.isShowInfo(!self.isShowInfo());
    }

    self.resetForm = function() {
        self.id(0);
        self.beginAmount(null);
        self.customerAddress("");
        self.customerEmail("");
        self.customerId(null);
        self.customerName("");
        self.customerPhone("");
        self.exchangePrice(null);
        self.exchangeRate(null);
        self.image(null);
        self.link(null);
        self.mode(null);
        self.name(null);
        self.note("");
        self.notePrivate("");
        self.officeId(null);
        self.officeIdPath(null);
        self.officeName(null);
        self.orderCode(null);
        self.orderDetailId(null);
        self.orderId(null);
        self.price(null);
        self.productNo(null);
        self.properties([]);
        self.quantity(null);
        self.quantityActual(null);
        self.quantityLose(null);
        self.shopId(null);
        self.shopLink(null);
        self.shopName(null);
        self.status(null);
        self.totalExchange(null);
        self.totalExchangeLose(null);
        self.totalExchangeShop(null);
        self.totalPrice(null);
        self.totalPriceCustomer(null);
        self.totalPriceLose(null);
        self.totalPriceShop(null);
        self.updated(null);
        self.userFullName(null);
        self.userId(null);
        self.warehouseId(null);
        self.warehouseName(null);
        self.websiteName(null);
        self.images([]);
    }

    self.setValue = function(data) {
        self.id(data.id);
        self.beginAmount(data.beginAmount);
        self.customerAddress(data.customerAddress);
        self.customerEmail(data.customerEmail);
        self.customerId(data.customerId);
        self.customerName(data.customerName);
        self.customerPhone(data.customerPhone);
        self.exchangePrice(data.exchangePrice);
        self.exchangeRate(data.exchangeRate);
        self.image(data.image);
        self.link(data.link);
        self.mode(data.mode);
        self.name(data.name);
        self.note(data.note);
        self.notePrivate(data.notePrivate());
        self.officeId(data.officeId);
        self.officeIdPath(data.officeIdPath);
        self.officeName(data.officeName);
        self.orderCode(data.orderCode);
        self.orderDetailId(data.orderDetailId);
        self.orderId(data.orderId);
        self.price(data.price);
        self.productNo(data.productNo);
        self.quantity(data.quantity);
        self.quantityActual(data.quantityActual);
        self.quantityLose(data.quantityLose);
        self.shopId(data.shopId);
        self.shopLink(data.shopLink);
        self.shopName(data.shopName);
        self.status(data.status);
        self.totalExchange(data.totalExchange);
        self.totalExchangeLose(data.totalExchangeLose);
        self.totalExchangeShop(data.totalExchangeShop);
        self.totalPrice(data.totalPrice);
        self.totalPriceCustomer(data.totalPriceCustomer);
        self.totalPriceLose(data.totalPriceLose);
        self.totalPriceShop(data.totalPriceShop);
        self.updated(data.updated);
        self.userFullName(data.userFullName);
        self.userId(data.userId);
        self.warehouseId(data.userId);
        self.warehouseName(data.warehouseName);
        self.websiteName(data.websiteName);
        self.properties(data.properties ? data.properties : []);
        self.totalPriceLose(self.price() * self.quantityLose());
        self.totalExchangeLose(self.totalPriceLose() * self.exchangeRate());

        if (self.id() <= 0) {
            self.quantityLose(data.quantityIncorrect());
        }

        self.initInputMark();
    }

    self.quantityLose.subscribe(function (newValue) {
        if (newValue && !isNaN(Globalize.parseFloat(newValue))) {
            self.totalPriceLose(self.price() * Globalize.parseFloat(newValue));
            self.totalExchangeLose(self.totalPriceLose() * self.exchangeRate());
            return;
        }
        self.totalPriceLose(null);
        self.totalExchangeLose(null);
    });

    self.show = function (mode, data) {
        self.resetForm();
        self.setValue(data);
        self.mode(mode);

        $("#acountingModal").modal("show");
    }

    self.token = $("#acountingModal input[name='__RequestVerificationToken']").val();
    
    self.save = function () {
        if (!$("#acountingForm").valid()) {
            toastr.error("Check the fields entered!");
            $(".input-validation-error:first").focus();
            return;
        }

        var data = {
            orderId: self.orderId(),
            orderDetailId: self.orderDetailId(),
            mode: self.mode(),
            quantityLose: Globalize.parseFloat(self.quantityLose()),
            note: self.notePrivate(),
            imageJson: JSON.stringify(self.images())
        }

        data["__RequestVerificationToken"] = self.token;

        self.isLoading(true);
        $.post("/orderdetailacounting/add",
            data, function(rs) {
                self.isLoading(false);
                if (rs && rs.status <= 0) {
                    toastr.error(rs.text);
                    return;
                }

                toastr.success(rs.text);
                var id = self.orderDetailId();
                self.resetForm();
                $("#acountingModal").modal("hide");
                modelView.getDetail(id);
            });
    }

    self.initInputMark = function () {
        $('#acountingForm input.decimal').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize.culture().numberFormat['.'], autoGroup: true, groupSeparator: Globalize.culture().numberFormat[','], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });
    }

    self.showImage = function (image) {
        imageViewModal.show(self.images(), image);
    }

    $(function () {
        // Init jquery upload ajax
        $("#addImageForAcounting").fileupload({
            url: "/Upload/UploadImages",
            sequentialUploads: true,
            dataType: "json",
            add: function (e, data) {
                var file = data.files[0];
                var msg = "";
                if (window.maxFileLength && file.size > window.maxFileLength) {
                    msg += file.name + ": Size is too large";
                } else if (window.validateBlackListExtensions(file.name)) {
                    msg += file.name + ": Incorrect formatting";
                }
                if (msg !== "") {
                    toastr.error(msg);
                } else {
                    data.submit();
                }
            },
            done: function (e, data) {
                if (data.result === -5) {
                    toastr.error("The file is not allowed");
                    return;
                }
                self.images.push(data.result[0].url);
                self.isUploading(false);
            },
            send: function () {
                self.isUploading(true);
            }, fail: function () {
                self.isUploading(false);
            }
        });
    });
}
function OrderDetailAcountingModel(acoutingModal, imageViewModal) {
    var self = this;
    self.isLoading = ko.observable(false);

    self.acoutingModal = ko.observable(acoutingModal);

    // Search
    self.states = ko.observableArray(window["states"] ? window.states : []);
    self.statesGroupId = _.groupBy(window["states"] ? window.states : [], 'id');
    self.orderDetailCountingStatusGroupId = _.groupBy(window["orderDetailCountingStatus"] ? window.orderDetailCountingStatus : [], 'id');
    self.orderDetailCountingModeGroupId = _.groupBy(window["orderDetailCountingMode"] ? window.orderDetailCountingMode : [], 'id');

    self.warehouseIdPath = ko.observable("");
    self.userId = ko.observable(null);
    self.status = ko.observable(window.initStatus != null && window.initStatus != undefined ? window.initStatus : null);
    self.fromDate = ko.observable(null);
    self.toDate = ko.observable(null);
    self.keyword = ko.observable("");

    // list data
    self.items = ko.observableArray();
    self.orders = ko.observableArray();

    // pagging
    self.currentPage = ko.observable(1);
    self.recordPerPage = ko.observable(20);
    self.totalPage = ko.observable(0);
    self.totalRecord = ko.observable(0);
    //self.addForm = ko.observable(null);

    // Subscribe
    var warehouseIdPathFirst = true;
    self.warehouseIdPath.subscribe(function () {
        if (warehouseIdPathFirst) {
            warehouseIdPathFirst = false;
            return;
        }
        self.search(1);
    });

    self.status.subscribe(function () {
        self.search(1);
    });

    self.renderPager = function () {
        self.totalPage(Math.ceil(self.totalRecord() / self.recordPerPage()));

        $("#sumaryPagerOrderDetailAcounting").html(self.totalRecord() === 0 ? "No Orders" : "Show " + ((self.currentPage() - 1) * self.recordPerPage() + 1) + " to " + (self.currentPage() * self.recordPerPage() < self.totalRecord() ?
            self.currentPage() * self.recordPerPage() : self.totalRecord()) + " of " + self.totalRecord() + " orders");

        $("#pagerOrderDetailAcounting").pager({ pagenumber: self.currentPage(), pagecount: self.totalPage(), totalrecords: self.totalRecord(), buttonClickCallback: self.pageClick });
    };

    self.pageClick = function (currentPage) {
        $("#pagerOrderDetailAcounting").pager({ pagenumber: currentPage, pagecount: self.totalPage(), buttonClickCallback: self.search });

        self.search(currentPage);
    };

    self.search = function (currentPage) {
        self.currentPage(currentPage);
        self.isLoading(true);
        $.get("/orderdetailacounting/search",
            {
                warehouseIdPath: self.warehouseIdPath(),
                userId: self.userId(),
                status: self.status(),
                fromDate: self.fromDate(),
                toDate: self.toDate(),
                keyword: self.keyword(),
                currentPage: self.currentPage(),
                recordPerPage: self.recordPerPage()
            }, function (data) {
                self.isLoading(false);

            var firstOrderCode = null;
            _.each(data.items,
                function(item) {
                    item.isFirst = firstOrderCode !== item.orderCode;
                    firstOrderCode = item.orderCode;

                    if (item.isFirst) {
                        var count = _.countBy(data.items,
                            function(it) { return it.orderId === item.orderId });
                        item.productNoInHere = count["true"];
                    }

                    item.cacheQuantityActual = item.quantityActual;
                    item.quantityRecived = ko.observable(item.quantityRecived != null ? formatNumberic(item.quantityRecived) : formatNumberic(item.quantityBooked));
                    item.quantityIncorrect = ko.observable(formatNumberic(item.quantityIncorrect));
                    item.quantityActual = ko.observable(item.quantityActual);

                    item.quantityMissing = ko.computed(function() {
                        var quantityRecived = Globalize.parseInt(item.quantityRecived());

                        if (quantityRecived === 0) {
                            return item.quantityBooked;
                        }

                        return item.quantityBooked - quantityRecived;
                    }, self);

                    item.quantityActualCache = ko.computed(function () {
                        var quantityRecived = Globalize.parseInt(item.quantityRecived());
                        var quantityIncorrect = Globalize.parseInt(item.quantityIncorrect());

                        if (quantityRecived === 0) {
                            return 0;
                        }

                        if (quantityIncorrect === 0) {
                            return quantityRecived;
                        }

                        return quantityRecived - quantityIncorrect;
                    }, self);

                    item.locked = ko.computed(function () {
                        var quantityIncorrect = Globalize.parseInt(item.quantityIncorrect());

                        return quantityIncorrect === 0 || item.quantityActual() === null;
                    }, self);

                    item.cacheNote = item.notePrivate;
                    item.notePrivate = ko.observable(item.notePrivate);

                    if (item.properties && item.properties != "null") {
                        item.properties = JSON.parse(item.properties);
                    } else {
                        item.properties = [];
                    }
                    
                    item.isShowDetail = ko.observable(false);
                    item.changeShowDetail = function() {
                        item.isShowDetail(!item.isShowDetail());
                    }
                    item.linkAcountings = ko.observableArray(_.filter(data.linkAcountings,
                        function (i) { return i.orderDetailId === item.orderDetailId }));
                    item.requestLose = ko.observable(item.requestLose);
                });
            
            self.items(data.items);
            self.totalRecord(data.totalRecord);
            self.renderPager();
            self.initInputMark();
        });
    }

    self.token = $("#orderDetailAcounting input[name='__RequestVerificationToken']").val();

    self.initInputMark = function () {
        $('#orderDetailAcounting input.integer').each(function () {
            if (!$(this).data()._inputmask) {
               $(this).inputmask("decimal", { radixPoint: Globalize.culture().numberFormat['.'], autoGroup: true, groupSeparator: Globalize.culture().numberFormat[','], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });
    }
    self.updateOrderDetail = function (data) {
        if ($.trim(data.quantityRecived()).length === 0) {
            toastr.warning("Amount received is required to enter");
            return;
        }

        $.post("/orderdetailacounting/updateorderdetail",
            {
                "__RequestVerificationToken": self.token,
                orderDetailId: data.orderDetailId,
                note: data.notePrivate(),
                quantityRecived: data.quantityRecived(),
                quantityIncorrect: data.quantityIncorrect()
            },
            function(rs) {
                if (rs && rs.status <= 0) {
                    toastr.error(rs.text);
                    return;
                }

                data.quantityActual(data.quantityActualCache());
                data.linkAcountings(rs.linkAcountings);
                data.requestLose(rs.linkAcountings.length);
            });
    }

    self.getDetail = function (id) {
        var data = _.find(self.items(), function (it) { return it.orderDetailId === id });
        $.get("/orderdetailacounting/GetLinkCountings",
            {
                id: data.orderDetailId
            },
            function (linkAcountings) {
                data.linkAcountings(linkAcountings);
                data.requestLose(linkAcountings.length);
            });
    }

    self.showImage = function (data) {
        imageViewModal.show([data.image], data.image, true);
    }

    $(function () {
        $('#OrderDetailAcounting-date-btn').daterangepicker({
            ranges: {
                'Today' : [moment(), moment()],
                'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                '7 days ago': [moment().subtract(6, 'days'), moment()],
                '30 days ago': [moment().subtract(29, 'days'), moment()],
                'This month': [moment().startOf('month'), moment().endOf('month')],
                'Last month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
            },
            startDate: moment().subtract(29, 'days'),
            endDate: moment()
        },
            function (start, end) {
                self.fromDate(start.format());
                self.toDate(end.format());
                self.search(1);
                $('#OrderDetailAcounting-date-btn span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
            });

        self.search(1);
    });
}

var imageViewModel = new ViewImageModel();
ko.applyBindings(imageViewModel, $("#viewImageModal")[0]);

var modelViewAcouting = new AcountingModel(imageViewModel);
ko.applyBindings(modelViewAcouting, $("#acountingModal")[0]);

var packageDetailModelView = new PackageDetail();
ko.applyBindings(packageDetailModelView, $("#packageDetail")[0]);

// Bind OrderDetail
var orderDetailViewModel = new OrderDetailViewModel();
ko.applyBindings(orderDetailViewModel, $("#orderDetailModal")[0]);

var depositDetailViewModel = new DepositDetailViewModel();
ko.applyBindings(depositDetailViewModel, $("#orderDepositDetail")[0]);

var orderCommerceDetailViewModel = new OrderCommerceDetailViewModel();
ko.applyBindings(orderCommerceDetailViewModel, $("#orderCommerceDetailModal")[0]);

var modelView = new OrderDetailAcountingModel(modelViewAcouting, imageViewModel);
ko.applyBindings(modelView, $("#orderDetailAcounting")[0]);