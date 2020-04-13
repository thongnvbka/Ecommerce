function ExportWarehouseAddModel(callback) {
    var self = this;

    self.statesGroupBy = _.groupBy(window["states"] ? window.states : [], "id");
    self.isLoading = ko.observable(false);
    self.isLoadingItems = ko.observable(false);

    self.orders = ko.observableArray([]);

    // model
    self.id = ko.observable(0);
    self.shipperName = ko.observable("");
    self.code = ko.observable("");
    self.status = ko.observable(1);
    self.warehouseId = ko.observable(0);
    self.warehouseIdPath = ko.observable("");
    self.warehouseName = ko.observable("");
    self.warehouseAddress = ko.observable("");
    self.orderNo = ko.observable(0);
    self.userId = ko.observable(0);
    self.userName = ko.observable("");
    self.userFullName = ko.observable("");
    self.created = ko.observable();
    self.lastUpdated = ko.observable();

    self.resetForm = function () {
        self.id(0);
        self.code("");
        self.status(0);
        self.warehouseId(0);
        self.warehouseIdPath("");
        self.warehouseName("");
        self.warehouseAddress("");
        self.orderNo(0);
        self.userId(0);
        self.userName("");
        self.userFullName("");
        self.created(null);
        self.lastUpdated(null);
        self.isLoadingItems(false);
        self.isLoading(false);

        self.orders.removeAll();
        self.initSuggetion();self.initSuggetion();
    }

    self.setForm = function (data) {
        self.showAddForm();

        self.id(data.id);
        self.code(data.code);
        self.status(data.status);
        self.warehouseId(data.warehouseId);
        self.warehouseIdPath(data.warehouseIdPath);
        self.warehouseName(data.warehouseName);
        self.warehouseAddress(data.warehouseAddress);
        self.orderNo(data.orderNo);
        self.userId(data.userId);
        self.userName(data.userName);
        self.userFullName(data.userFullName);
        self.created(data.created);
        self.lastUpdated(data.lastUpdated);
        
        // Get detail của phiếu nhập kho
        self.getDetail();
    }

    self.getDetail = function () {
        self.isLoadingItems(true);
        $.get("/exportwarehouse/getdetail", { id: self.id() }, function (data) {
            self.isLoadingItems(false);
            var groupByOrderCode = _.groupBy(data, 'orderCode');
            var ordersCode = _.keys(groupByOrderCode);

            var orders = [];
            _.each(ordersCode,
                function (orderCode) {
                    var order = groupByOrderCode[orderCode][0];
                    order.isShowDetail = ko.observable(false);
                    order.packages = _.filter(data, function (i) { return i.orderCode === orderCode; });

                    order.changeShowDetail = function () {
                        order.isShowDetail(!order.isShowDetail());
                    }

                    //order.packageWeight = order.weight;
                    //order.packageWeightActual = order.weightActual;
                    //order.packageWeightConverted = order.weightConverted;

                    order.orderPackageNo = order.packages.length;
                    order.orderWeight = _.sumBy(order.packages, "packageWeight");
                    order.orderWeightActual = _.sumBy(order.packages, "packageWeightActual");
                    order.orderWeightConverted = _.sumBy(order.packages, "packageWeightConverted");

                    order.customerOrderNo = ordersCode.length;

                    var packageOfCustomer = _.filter(data,
                        function (item) { return item.customerId === order.customerId; });

                    order.customerWeight = _.sumBy(packageOfCustomer, "packageWeight");
                    order.customerWeightActual = _.sumBy(packageOfCustomer, "packageWeightActual");
                    order.customerWeightConverted = _.sumBy(packageOfCustomer, "packageWeightConverted");

                    orders.push(order);
                });

            self.orders.removeAll();
            self.orders(orders);
            self.initInputMark();
        });
    }

    //self.getFormData = function () {
    //    return {
    //        id: self.id(),
    //        shipperName: self.shipperName(),
    //        shipperPhone: self.shipperPhone(),
    //        shipperAddress: self.shipperAddress(),
    //        shipperEmail: self.shipperEmail(),
    //        note: self.note(),
    //        code: self.code(),
    //        status: self.status(),
    //        warehouseId: self.warehouseId(),
    //        warehouseIdPath: self.warehouseIdPath(),
    //        warehouseName: self.warehouseName(),
    //        warehouseAddress: self.warehouseAddress(),
    //        packageNumber: self.packageNumber(),
    //        userId: self.userId(),
    //        userName: self.userName(),
    //        userFullName: self.userFullName(),
    //        created: self.created(),
    //        lastUpdated: self.lastUpdated(),
    //        warehouseManagerId: self.warehouseManagerId(),
    //        warehouseManagerCode: self.warehouseManagerCode(),
    //        warehouseManagerFullName: self.warehouseManagerFullName(),
    //        warehouseAccountantId: self.warehouseAccountantId(),
    //        warehouseAccountantCode: self.warehouseAccountantCode(),
    //        warehouseAccountantFullName: self.warehouseAccountantFullName(),
    //        walletNumber: self.walletNumber()
    //    }
    //}

    self.showAddForm = function () {
        self.resetForm();
        resetForm("#ExportWarehouseAddModelForm");
        $("#ExportWarehouseAddModel").modal("show");
    }

    self.showUpdateForm = function (data) {
        self.setForm(data);
        resetForm("#ExportWarehouseAddModelForm");
        $("#ExportWarehouseAddModel").modal("show");
    }

    //self.updatePackage = function (package) {
    //    var data = ko.mapping.toJS(package);

    //    data["__RequestVerificationToken"] = self.token;
    //    self.isLoading(true);
    //    $.post("/importwarehouse/updatepackage", data, function (rs) {
    //        self.isLoading(false);
    //        if (rs.status < 0) {
    //            toastr.error(rs.text);
    //            return;
    //        }
    //    });
    //}

    self.removeItem = function (item) {
        if (self.id() === 0) {
            self.orders.remove(item);
            return;
        }
    }

    self.token = $("#ExportWarehouseAddModelForm input[name='__RequestVerificationToken']").val();

    self.save = function () {
        if (!$("#ExportWarehouseAddModelForm").valid()) {
            toastr.error("Check the fields entered!");
            $(".input-validation-error:first").focus();
            return;
        }

        if (self.orders().length === 0) {
            toastr.error("Dispatch note does not have any order yet!");
            return;
        }

        var packages = [];
        _.each(self.orders(),
            function(order) {
                _.each(order.packages,
                    function(package) {
                        var p = ko.mapping.toJS(package);

                        p.customerDistance = order.customerDistance();
                        p.orderShip = order.orderShip();
                        p.orderShipActual = order.orderShipActual();
                        p.orderNote = order.orderNote();
                        p.orderPackageNo = order.orderPackageNo;
                        p.orderWeight = formatNumberic(order.orderWeight, 'N2');
                        p.orderWeightActual = formatNumberic(order.orderWeightActual, 'N2');
                        p.orderWeightConverted = formatNumberic(order.orderWeightConverted, 'N2');
                        p.customerOrderNo = order.customerOrderNo;
                        p.customerWeight = formatNumberic(order.customerWeight, 'N2');
                        p.customerWeightActual = formatNumberic(order.customerWeightActual, 'N2');
                        p.customerWeightConverted = formatNumberic(order.customerWeightConverted, 'N2');
                        p.packageWeight = formatNumberic(p.packageWeight, 'N2');
                        p.packageWeightActual = formatNumberic(p.packageWeightActual, 'N2');
                        p.packageWeightConverted = formatNumberic(p.packageWeightConverted, 'N2');

                        packages.push(p);
                    });
            });
        var data = {};
        data.model = packages;
        data["__RequestVerificationToken"] = self.token;

        // Cập nhật
        if (self.id() > 0) {
            self.isLoading(true);
            $.post("/exportwarehouse/update", data, function (rs) {
                self.isLoading(false);

                if (rs.status < 0) {
                    toastr.error(rs.text);
                    return;
                }

                toastr.success(rs.text);
                // reset form
                self.resetForm();
                resetForm("#ExportWarehouseAddModelForm");
                $("#ExportWarehouseAddModel").modal("hide");

                if (callback)
                    callback();
            });
        } else { // Thêm mới
            self.isLoading(true);
            $.post("/exportwarehouse/add", data, function (rs) {
                self.isLoading(false);
                if (rs.status < 0) {
                    toastr.error(rs.text);
                    return;
                }

                toastr.success(rs.text);
                // reset form
                self.resetForm();
                resetForm("#ExportWarehouseAddModelForm");
                $("#ExportWarehouseAddModel").modal("hide");

                if (callback)
                    callback();
            });
        }
    }

    self.showDetail = function (data) {
        if (packageDetailModal) {
            packageDetailModal.showModel(data);
            return;
        }
    }

    self.showOrderDetail = function (orderId) {
        if (orderDetailModal) {
            orderDetailModal.viewOrderDetail(orderId);
            return;
        }
    }

    self.showWalletDetail = function (walletId) {
        if (walletDetailModal) {
            walletDetailModal.showModel(walletId);
        }
    }

    self.initInputMark = function () {
        $('#ExportWarehouseAddModel input.decimal').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize.culture().numberFormat['.'], autoGroup: true, groupSeparator: Globalize.culture().numberFormat[','], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });
    }

    self.initSuggetion = function () {
        // Đăng ký suggetion đơn vị
        $('#suggetion').autocomplete({
            delay: 100,
            autoFocus: true,
            source: function (request, response) {
                var codes = _.map(self.orders(), "orderCode");

                var strCodes = codes.length === 0 ? "" : ';' + _.join(codes, ';') + ';';

                $.post("/exportWarehouse/searchorder", { term: request.term, codeOrders: strCodes }, function (result) {
                    return response(result);
                });
            },
            select: function (event, ui) {
                $.get("/package/getByordercodeforexportwarehouse",
                    { codeOrder: ui.item.code },
                    function (items) {

                        if (!items || items.length === 0)
                            return;

                        _.each(items,
                            function (it) {
                                it.exportWarehouseCode = 0;
                                it.exportWarehouseId = "";
                                it.customerDistance = null;
                                it.customerOrderNo = 0;
                                it.customerWeight = 0;
                                it.customerWeightActual = 0;
                                it.customerWeightConverted = 0;
                                it.orderTotalPackageNo = it.packageNo;
                                it.orderPackageNo = 0;
                                it.orderShip = null;
                                it.orderShipActual = null;
                                it.orderWeight = 0;
                                it.orderWeightActual = 0;
                                it.orderWeightConverted = 0;
                                it.orderNote = "";
                                it.packageCode = it.code;
                                it.packageId = it.id;
                                it.packageWeight = it.weight;
                                it.packageWeightActual = it.weightActual;
                                it.packageWeightConverted = it.weightConverted;
                                it.packageSize = it.size;
                                it.packageTransportCode = it.transportCode;
                                it.status = 0;
                                it.id = 0;
                            });

                        var order = _.clone(items[0]);    

                        order.customerDistance = ko.observable(formatNumberic(order.customerDistance, 'N2'));
                        order.orderShip = ko.observable(formatNumberic(order.orderShip, 'N2'));
                        order.orderShipActual = ko.observable(formatNumberic(order.orderShipActual, 'N2'));
                        order.orderNote = ko.observable(order.orderNote);

                        order.orderShip.subscribe(function (newValue) {
                            if (!order.orderShipActual() && newValue) {
                                order.orderShipActual(newValue);
                            }
                        });

                        order.isShowDetail = ko.observable(false);
                        order.packages = items;

                        order.changeShowDetail = function () {
                            order.isShowDetail(!order.isShowDetail());
                        }

                        //order.packageWeight = order.weight;
                        //order.packageWeightActual = order.weightActual;
                        //order.packageWeightConverted = order.weightConverted;

                        order.orderPackageNo = items.length;
                        order.orderWeight = _.sumBy(items, "packageWeight");
                        order.orderWeightActual = _.sumBy(items, "packageWeightActual");
                        order.orderWeightConverted = _.sumBy(items, "packageWeightConverted");
                        order.customerOrderNo = order.orderPackageNo;
                        order.customerWeight = order.orderWeight;
                        order.customerWeightActual = order.orderWeightActual;
                        order.customerWeightConverted = order.orderWeightConverted;

                        self.orders.push(order);

                        self.initInputMark();
                    });
            }
        }).autocomplete("instance")._renderItem = function (ul, item) {
            var name = 'Order code: ODR' + item.code + ' Customer: ' + item.customerName + '(' + item.customerEmail + ')' + ' - Phone: ' + item.customerPhone;

            return $("<li>").addClass('media media-line').append('<div>' +
                '<h4 class="media-heading bold size-16 pr-mgb-0">' + name +
                '</h4></div>').appendTo(ul).addClass('automember media-list');
        };
    }

    $(function () {
        self.initSuggetion();
    });
}

function ExportWarehouseModel() {
    var self = this;
    self.isLoading = ko.observable(false);

    // Search
    self.states = ko.observableArray(window["states"] ? window.states : []);
    self.statesGroupId = _.groupBy(window["states"] ? window.states : [], 'id');

    self.warehouseIdPath = ko.observable("");
    self.userId = ko.observable(null);
    self.status = ko.observable(null);
    self.fromDate = ko.observable(null);
    self.toDate = ko.observable(null);
    self.keyword = ko.observable("");

    // list data
    self.items = ko.observableArray();

    // pagging
    self.currentPage = ko.observable(1);
    self.recordPerPage = ko.observable(20);
    self.totalPage = ko.observable(0);
    self.totalRecord = ko.observable(0);

    self.addForm = ko.observable(null);

    // Subscribe
    var warehouseIdPathFirst = true;
    self.warehouseIdPath.subscribe(function () {
        if (warehouseIdPathFirst) {
            warehouseIdPathFirst = false;
            return;
        }
        self.search(1);
    });

    var statusFirst = true;
    self.status.subscribe(function () {
        if (statusFirst) {
            statusFirst = false;
            return;
        }
        self.search(1);
    });

    self.renderPager = function () {
        self.totalPage(Math.ceil(self.totalRecord() / self.recordPerPage()));

        $("#sumaryPagerExportWarehouse").html(self.totalRecord() === 0 ? "There is not any transfer notes" : "Show " + ((self.currentPage() - 1) * self.recordPerPage() + 1) + " to " + (self.currentPage() * self.recordPerPage() < self.totalRecord() ?
            self.currentPage() * self.recordPerPage() : self.totalRecord()) + " of " + self.totalRecord() + " transfer note");

        $("#pagerExportWarehouse").pager({ pagenumber: self.currentPage(), pagecount: self.totalPage(), totalrecords: self.totalRecord(), buttonClickCallback: self.pageClick });
    };

    self.pageClick = function (currentPage) {
        $("#pagerExportWarehouse").pager({ pagenumber: currentPage, pagecount: self.totalPage(), buttonClickCallback: self.search });

        self.search(currentPage);
    };

    self.search = function (currentPage) {
        self.currentPage(currentPage);
        self.isLoading(true);
        $.get("/exportwarehouse/search",
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

                var statesGroup = _.groupBy(window.states, "id");

                _.each(data.items,
                    function (item) {
                        item.statusText = statesGroup[item.status + ''][0].name;
                        item.createdText = moment(item.forcastDate).format("DD/MM/YYYY HH:mm:ss");
                    });

                self.items(data.items);
                self.totalRecord(data.totalRecord);
                self.renderPager();
            });
    }

    self.callback = function () {
        self.search(1);
    }

    self.addBill = function () {
        if (self.addForm() == null) {
            self.addForm(new ExportWarehouseAddModel(self.callback));
            ko.applyBindings(self.addForm(), $("#ExportWarehouseAddModel")[0]);
        }

        self.addForm().showAddForm();
    }

    self.update = function (data) {
        if (self.addForm() == null) {
            self.addForm(new ExportWarehouseAddModel(self.callback));
            ko.applyBindings(self.addForm(), $("#ExportWarehouseAddModel")[0]);
        }

        self.addForm().setForm(data);
    }

    $(function () {
        $('#ExportWarehouse-date-btn').daterangepicker({
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
                $('#ExportWarehouse-date-btn span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
            });

        self.search(1);
    });
}

var modelView = new ExportWarehouseModel();
ko.applyBindings(modelView, $("#exportWarehouse")[0])