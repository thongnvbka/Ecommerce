function TransferAddModel(callback, packageDetailModal, orderDetailModal) {
    var self = this;

    self.allWarehouse = ko.observableArray(window.allWarehouse);
    self.isLoading = ko.observable(false);
    self.isLoadingItems = ko.observable(false);
    self.statesGroupId = _.groupBy(window.states, 'id');

    self.items = ko.observableArray([]);

    // model
    self.id = ko.observable(0);
    self.note = ko.observable("");
    self.toWarehouseId = ko.observable(null);
    self.toWarehouseIdPath = ko.observable("");
    self.toWarehouseName = ko.observable("");
    self.toWarehouseAddress = ko.observable("");
    self.priceShip = ko.observable(null);

    self.toWarehouseId.subscribe(function (warehouseId) {
        var warehouse = _.find(self.allWarehouse(), function (w) { return w.id === warehouseId; });

        if (warehouse) {
            self.toWarehouseId(warehouse.id);
            self.toWarehouseIdPath(warehouse.idPath);
            self.toWarehouseName(warehouse.name);
            self.toWarehouseAddress(warehouse.address);
        } else {
            self.toWarehouseId(null);
            self.toWarehouseIdPath(null);
            self.toWarehouseName(null);
            self.toWarehouseAddress(null);
        }
    });

    self.resetForm = function () {
        self.id(0);
        self.note("");
        self.toWarehouseId(null);
        self.toWarehouseIdPath("");
        self.toWarehouseName("");
        self.toWarehouseAddress("");
        self.priceShip(null);

        self.initInputMark();
        self.removeRules();
    }

    self.removeRules = function () {
        $("#transferAddOrEdit #PriceShip").rules("remove", "number");
    }

    self.reOrderBy = function (packageId) {
        //var list = _.orderBy(self.items(), ['orderCode'], ['asc']);

        if (packageId) {
            _.each(self.items(), function (it) {
                it.isHighline = packageId != undefined && packageId != null
                    && it.packageId === packageId ? true : false;
            });
        }

        var list = _.orderBy(self.items(), ['orderCode', 'isHighline'], ['asc', 'desc']);

        // Đẩy lại toàn bộ kiện của Orders mới thêm lên đầu danh sách
        if (packageId) {
            var order = _.find(self.items(), function (it) { return it.packageId === packageId; });
            if (order) {
                var listPackage = _.filter(list, function (it) { return it.orderId === order.orderId; });

                _.remove(list, function (it) { return it.orderId === order.orderId; });

                list = _.concat(listPackage, list);
            }
        }

        var firstOrderCode;
        _.each(list, function (it) {
            it.isFirst = ko.observable(firstOrderCode !== it.orderCode);
            firstOrderCode = it.orderCode;

            it.isHighline = packageId != undefined && packageId != null && it.packageId === packageId ? true : false;

            if (it.isFirst) {
                it.packageNoInTransfer = _.filter(list, function (d) {
                    return d.orderCode === it.orderCode;
                }).length;
            }
        });
        self.items.removeAll();
        self.items(list);
    }

    self.getDetail = function () {
        self.isLoadingItems(true);
        $.get("/transfer/getpackages", { id: self.id() }, function (data) {
            self.isLoadingItems(false);

            var firstOrderCode;
            _.each(data, function (it) {
                it.isFirst = ko.observable(firstOrderCode !== it.orderCode);
                firstOrderCode = it.orderCode;

                if (it.isFirst) {
                    it.packageNoInTransfer = _.filter(data, function (d) {
                        return d.orderCode === it.orderCode;
                    }).length;
                }
                it.isHighline = false;
                // Ghi chu
                it.cacheNote = it.note;
                it.note = ko.observable(it.note);
                it.note.subscribe(function (newValue) {
                    if (it.cacheNote !== newValue) {
                        self.updatePackage(it);
                        it.cacheNote = newValue;
                    }
                });
                it.totalValue = ko.observable(formatNumberic(it.totalValue, 'N2'));
            });
            self.items(data);
            self.initInputMark();
        });
    }

    self.getFormData = function () {
        return {
            id: self.id(),
            note: self.note(),
            toWarehouseId: self.toWarehouseId(),
            toWarehouseIdPath: self.toWarehouseIdPath(),
            toWarehouseName: self.toWarehouseName(),
            toWarehouseAddress: self.toWarehouseAddress(),
            priceShip: self.priceShip()
        }
    }

    self.showAddForm = function (packageCodes) {
        self.resetForm();
        resetForm("#transferForm");

        $("#transferAddOrEdit").modal("show");
    }

    self.loadingItems = function (transferId) {
        $.get("/transfer/getpackages", { id: transferId }, function (data) {
            self.items(data);
        });
    }

    self.updatePackage = function (package) {
        var data = ko.mapping.toJS(package);

        data["__RequestVerificationToken"] = self.token;
        self.isLoading(true);
        $.post("/transfer/updatepackage", data, function (rs) {
            self.isLoading(false);
            if (rs.status < 0) {
                toastr.error(rs.text);
                return;
            }
        });
    }

    self.removeItem = function (item) {
        if (self.id() === 0) {
            self.items.remove(item);

            if (self.items().length === 0) {
                self.resetForm();
            }
            return;
        }
        swal({
            title: 'Are you sure you want to delete this item?',
            text: 'package "' + item.packageCode + '"',
            type: 'warning',
            showCancelButton: true,
            //confirmButtonColor: '#3085d6',
            //cancelButtonColor: '#d33',
           cancelButtonText: 'Cancel',
            confirmButtonText: 'Delete'
        })
            .then(function () {
                self.isLoading(true);

                item["__RequestVerificationToken"] = self.token;
                $.post("/transfer/deletepackage", item,
                    function (rs) {
                        self.isLoading(false);
                        if (rs.status < 0) {
                            toastr.error(rs.text);
                            return;
                        }

                        toastr.success(rs.text);
                        self.getDetail();
                    });
            }, function () { });
    }

    self.token = $("#transferForm input[name='__RequestVerificationToken']").val();

    self.save = function () {
        if (!$("#transferForm").valid()) {
            toastr.error("Check the entered fields!");
            $(".input-validation-error:first").focus();
            return;
        }

        if (self.items().length === 0) {
            toastr.error("Requisition for warehousing is not available.");
            return;
        }

        var data = self.getFormData();
        data.TransferDetails = ko.mapping.toJS(self.items());
        data["__RequestVerificationToken"] = self.token;

        var hasError;
        // Validate phí phát sinh
        _.each(data.OrderServiceOthers, function (s) {
            s.orderCode = _.trim(s.orderCode);
            s.value = _.trim(s.value);
            if (s.orderCode.length === 0 || s.value.length === 0) {
                hasError = true;
                toastr.warning("The order number and the value of money in the resulting charge may not be left blank");
                return false;
            }

            if (_.isNil(_.find(data.packages, function (p) { return p.orderCode === s.orderCode; }))) {
                hasError = true;
                toastr.warning('Code orders "' + s.orderCode + '" not included in this entry');
                return false;
            }
            // ReSharper disable once NotAllPathsReturnValue
        });

        if (hasError)
            return;

        // Cập nhật
        if (self.id() > 0) {
            self.isLoading(true);
            $.post("/transfer/update", data, function (rs) {
                self.isLoading(false);

                if (rs.status < 0) {
                    toastr.error(rs.text);
                    return;
                }

                toastr.success(rs.text);
                // reset form
                self.resetForm();
                resetForm("#transferForm");
                $("#transferAddOrEdit").modal("hide");

                if (callback)
                    callback();
            });
        } else { // Thêm mới
            self.isLoading(true);
            $.post("/transfer/add", data, function (rs) {
                self.isLoading(false);
                if (rs.status < 0) {
                    toastr.error(rs.text);
                    return;
                }

                if (rs["data"]) {
                    swal(
                        'Create successful wood case',
                        'The code is just created: ' + rs["data"].code,
                        'success'
                    );
                } else {
                    toastr.success(rs.text);
                }

                // reset form
                self.resetForm();
                resetForm("#transferForm");
                $("#transferAddOrEdit").modal("hide");

                if (callback)
                    callback();
            });
        }
    }

    self.initInputMark = function () {
        $('#transferAddOrEdit input.decimal').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize.culture().numberFormat['.'], autoGroup: true, groupSeparator: Globalize.culture().numberFormat[','], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });
    }

    // Xem Detail Kiện
    self.showDetailPackage = function (data) {
        if (packageDetailModal) {
            packageDetailModal.showModel(data);
            return;
        }
    }

    // Xem Detail Orders
    self.showOrderDetail = function (orderId) {
        if (orderDetailModal) {
            orderDetailModal.viewOrderDetail(orderId);
            return;
        }
    }

    self.showOrderDispositDetail = function (orderId) {
        if (window.hasOwnProperty("depositDetailViewModel")) {
            window.depositDetailViewModel.showModalDialog(orderId);
            return;
        }
    }

    self.showOrderCareDetail = function (orderId) {
        if (window.hasOwnProperty("orderCommerce")) {
            window.orderCommerce.viewOrderAdd(orderId);
            return;
        }
    }

    self.showOrderDetail = function (orderId) {
        if (orderDetailModal) {
            orderDetailModal.viewOrderDetail(orderId);
            return;
        }
    }

    self.initSuggetion = function () {
        var isPaste = false;
        $('#suggetion').on('paste', function () {
            isPaste = true;
        });

        var inputStart, inputStop, firstKey, lastKey, timing, userFinishedEntering;
        var minChars = 3;

        // handle a key value being entered by either keyboard or scanner
        $("#suggetion").keypress(function (e) {
            // restart the timer
            if (timing) {
                clearTimeout(timing);
            }

            // handle the key event
            if (e.which === 13) {
                // Enter key was entered

                // don't submit the form
                e.preventDefault();

                // has the user finished entering manually?
                if ($("#suggetion").val().length >= minChars) {
                    userFinishedEntering = true; // incase the user pressed the enter key
                    inputComplete();
                }
            }
            else {
                // some other key value was entered

                // could be the last character
                inputStop = performance.now();
                lastKey = e.which;

                // don't assume it's finished just yet
                userFinishedEntering = false;

                // is this the first character?
                if (!inputStart) {
                    firstKey = e.which;
                    inputStart = inputStop;

                    // watch for a loss of focus
                    $("body").on("blur", "#suggetion", inputBlur);
                }

                // start the timer again
                timing = setTimeout(inputTimeoutHandler, 500);
            }
        });

        // Assume that a loss of focus means the value has finished being entered
        function inputBlur() {
            clearTimeout(timing);
            if ($("#suggetion").val().length >= minChars) {
                userFinishedEntering = true;
                inputComplete();
            }
        };

        function resetValues() {
            // clear the variables
            inputStart = null;
            inputStop = null;
            firstKey = null;
            lastKey = null;
            // clear the results
            inputComplete();
        }

        // Assume that it is from the scanner if it was entered really fast
        function isScannerInput() {
            if ($("#suggetion").val().length <= 2)
                return false;

            return (((inputStop - inputStart) / $("#suggetion").val().length) < 15);
        }

        // Determine if the user is just typing slowly
        function isUserFinishedEntering() {
            return !isScannerInput() && userFinishedEntering;
        }

        function inputTimeoutHandler() {
            // stop listening for a timer event
            clearTimeout(timing);
            // if the value is being entered manually and hasn't finished being entered
            if (!isUserFinishedEntering() || $("#suggetion").val().length < 3) {
                // keep waiting for input
                return;
            }
        }

        // here we decide what to do now that we know a value has been completely entered
        function inputComplete() {
            // stop listening for the input to lose focus
            $("body").off("blur", "#suggetion", inputBlur);
        }

        // Đăng ký suggetion đơn vị
        $('#suggetion').autocomplete({
            delay: 100,
            autoFocus: true,
            source: function (request, response) {
                // ReSharper disable once CoercedEqualsUsing
                var codes = _.map(self.items(), "packageCode");

                var strCodes = codes.length === 0 ? "" : ';' + _.join(codes, ';') + ';';

                isPaste = isPaste || isScannerInput() ? true : false;

                $.post('/package/SuggetionForTransfer', { term: request.term, packageCodes: strCodes, toWarehouseId: self.toWarehouseId(), isPaste: isPaste }, function (result) {
                    if (isPaste) {
                        if (result.length > 0)
                            self.processItemSelected(result[0]);

                        isPaste = false;
                        $('#suggetion').val("");
                        resetValues();
                    } else {
                        return response(result);
                    }
                    // ReSharper disable once NotAllPathsReturnValue
                });
            },
            select: function (event, ui) {
                self.processItemSelected(ui.item);
                resetValues();
            }
        }).autocomplete("instance")._renderItem = function (ul, item) {
            var name = item.transportCode + ' - P' + item.code + ' - ' + 'ODR' + item.orderCode + ' - destination warehouse: ' + item.customerWarehouseName;

            return $("<li>").addClass('media media-line').append('<div>' +
                '<h4 class="media-heading bold size-16 pr-mgb-0">' + name +
                '</h4></div>').appendTo(ul).addClass('automember media-list');
        };
    }

    self.processItemSelected = function (item) {
        item.orderPackageNo = item.packageNo;
        item.isFirst = ko.observable(false);
        item.transferId = 0;
        item.transferCode = "";
        item.packageId = item.id;
        item.packageCode = item.code;
        item.id = 0;
        item.note = ko.observable(item.note);
        item.convertedWeight = item.weightConverted;
        item.actualWeight = item.weightActual;
        item.status = 1;

        if (self.id() > 0) {
            var package = ko.mapping.toJS(item);

            package.transferId = self.id();
            package.transferCode = self.code();
            package["__RequestVerificationToken"] = self.token;

            self.isLoading(true);
            $.post("/transfer/addpackage", package, function (rs) {
                self.isLoading(false);
                if (rs.status < 0) {
                    toastr.error(rs.text);
                    return;
                }
                self.getDetail();
            });
            return;
        }

        if (!_.find(self.items(), function (it) { return it.packageCode === item.code })) {
            item.isHighline = true;
            self.items.push(item);

            //if (self.items().length === 1) {
            //    self.toWarehouseId(item.customerWarehouseId);
            //    self.toWarehouseIdPath(item.customerWarehouseIdPath);
            //    self.toWarehouseName(item.customerWarehouseName);
            //    self.toWarehouseAddress(item.customerWarehouseAddress);
            //}

            self.reOrderBy(item.packageId);
            self.initInputMark();
        }
    }

    $(function () {
        self.initSuggetion();
    });
}


function TransferModel(packageDetailModal, orderDetailModal, depositDetailViewModel, orderCommerceDetailViewModel) {
    var self = this;
    self.isLoading = ko.observable(false);

    // Search
    self.warehouses = ko.observableArray(window["warehouses"] ? window.warehouses : []);
    self.states = ko.observableArray([{ id: 0, name: 'New' }, { id: 1, name: 'Completed' }]);

    self.warehouseIdPath = ko.observable("");
    self.userId = ko.observable(null);
    self.status = ko.observable(null);
    self.fromDate = ko.observable(null);
    self.toDate = ko.observable(null);
    self.keyword = ko.observable("");

    self.mode = ko.observable(0);
    self.createdNo = ko.observable(0);
    self.inStockNo = ko.observable(0);
    self.allNo = ko.observable(0);

    // list data
    self.items = ko.observableArray();

    // pagging
    self.currentPage = ko.observable(1);
    self.recordPerPage = ko.observable(20);
    self.totalPage = ko.observable(0);
    self.totalRecord = ko.observable(0);

    self.addForm = ko.observable(null);

    self.changeMode = function (mode) {
        if (mode !== self.mode()) {
            self.mode(mode);
            self.search(1);
        }
    }

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

        $("#sumaryPagerTransfer").html(self.totalRecord() === 0 ? "No votes yet" : "Show " + ((self.currentPage() - 1) * self.recordPerPage() + 1) + " to " + (self.currentPage() * self.recordPerPage() < self.totalRecord() ?
            self.currentPage() * self.recordPerPage() : self.totalRecord()) + " of " + self.totalRecord() + " the vote moved the repository");

        $("#pagerTransfer").pager({ pagenumber: self.currentPage(), pagecount: self.totalPage(), totalrecords: self.totalRecord(), buttonClickCallback: self.pageClick });
    };

    self.pageClick = function (currentPage) {
        $("#pagerTransfer").pager({ pagenumber: currentPage, pagecount: self.totalPage(), buttonClickCallback: self.search });

        self.search(currentPage);
    };

    self.search = function (currentPage) {
        self.currentPage(currentPage);
        self.isLoading(true);
        $.get("/transfer/search",
            {
                mode: self.mode(),
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

                self.createdNo(formatNumberic(data.mode["createdNo"]));
                self.inStockNo(formatNumberic(data.mode["inStockNo"]));
                self.allNo(formatNumberic(data.mode["allNo"]));

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
            self.addForm(new TransferAddModel(self.callback, packageDetailModal, orderDetailModal));
            ko.applyBindings(self.addForm(), $("#transferAddOrEdit")[0]);
        }

        self.addForm().showAddForm();
    }

    self.update = function (data) {
        if (self.addForm() == null) {
            self.addForm(new TransferAddModel(self.callback, packageDetailModal, orderDetailModal));
            ko.applyBindings(self.addForm(), $("#transferAddOrEdit")[0]);
        }

        self.addForm().setForm(data);
    }

    self.approvel = function(id) {
        swal({
            title: 'You want to confirm this ticket?',
            text: 'The system automatically updates the packages in the coupon into the warehouse!',
            type: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Confirm',
            cancelButtonText: 'Not confirmed!'
        }).then(function () {
            var data = {
                transferId: id
            }
            data["__RequestVerificationToken"] = self.token;
            $.post("/Transfer/Approvel", data,
                function (rs) {
                    if (rs.status <= 0) {
                        toastr.warning(rs.text);
                        return;
                    }
                    self.search(1);
                    toastr.success(rs.text);
                });
        }, function () { });
    }

    self.token = $("#transferForm input[name='__RequestVerificationToken']").val();

    self.showTransferDetail = function (transferId) {
        if (window.hasOwnProperty("transferDetailModalView")) {
            transferDetailModalView.showModel(transferId);
            return;
        }
    }

    $(function () {
        $('#Transfer-date-btn').daterangepicker({
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
                $('#Transfer-date-btn span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
            });

        self.search(1);
    });
}

// Bind PackageDetail
var packageDetailModelView = new PackageDetail(window.orderPackageStates);
ko.applyBindings(packageDetailModelView, $("#packageDetail")[0]);

// Bind OrderDetail
var orderDetailViewModel = new OrderDetailViewModel();
ko.applyBindings(orderDetailViewModel, $("#orderDetailModal")[0]);

var depositDetailViewModel = new DepositDetailViewModel();
ko.applyBindings(depositDetailViewModel, $("#orderDepositDetail")[0]);

var orderCommerceDetailViewModel = new OrderCommerceDetailViewModel();
ko.applyBindings(orderCommerceDetailViewModel, $("#orderCommerceDetailModal")[0]);

// allWarehouse, walletStates, orderPackageStates, packageDetailModal, orderDetailModal
var transferDetailModalView = new TransferDetailModel(window.allWarehouse, window.states);
ko.applyBindings(transferDetailModalView, $("#transferDetailModal")[0]);

var modelView = new TransferModel(packageDetailModelView, orderDetailViewModel, depositDetailViewModel, orderCommerceDetailViewModel);
ko.applyBindings(modelView, $("#transfer")[0])