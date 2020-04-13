function WalletAddModel(callback, packageDetailModal, orderDetailModal, orderServiceOtherModal) {
    var self = this;

    self.allWarehouse = ko.observableArray(window.allWarehouse);
    self.isLoading = ko.observable(false);
    self.isLoadingItems = ko.observable(false);
    self.suggetionType = ko.observable(0); // 0: package, 1: Bao hàng
    self.statesGroupId = _.groupBy(window.states, 'id');

    self.items = ko.observableArray([]);

    // model
    self.id = ko.observable(0);
    self.note = ko.observable("");
    self.code = ko.observable("");
    self.mode = ko.observable(0);
    self.status = ko.observable(1);
    self.entrepotId = ko.observable(null);
    self.isSameWallet = ko.observable(false);
    self.modeDelivery = ko.observable(0); // 0: Thường, 1: Tối ưu, 2: Nhanh
    self.targetWarehouseId = ko.observable(null);
    self.targetWarehouseIdPath = ko.observable("");
    self.targetWarehouseName = ko.observable("");
    self.targetWarehouseAddress = ko.observable("");
    self.userId = ko.observable(0);
    self.userName = ko.observable("");
    self.userFullName = ko.observable("");
    self.created = ko.observable();
    self.updated = ko.observable();
    self.packageNo = ko.observable(0);
    self.totalValue = ko.observable(null);
    self.weight = ko.observable(null);
    self.totalConversionWeight = ko.observable(null);
    self.totalConversionWeightValue = ko.observable(null);
    self.totalActualWeight = ko.observable(null);
    self.currentWarehouseId = ko.observable(0);
    self.currentWarehouseIdPath = ko.observable("");
    self.currentWarehouseName = ko.observable("");
    self.currentWarehouseAddress = ko.observable("");
    self.createdWarehouseId = ko.observable(0);
    self.createdWarehouseIdPath = ko.observable("");
    self.createdWarehouseName = ko.observable("");
    self.createdWarehouseAddress = ko.observable("");

    self.width = ko.observable(null);
    self.length = ko.observable(null);
    self.height = ko.observable(null);
    self.size = ko.observable(null);
    self.totalSumWeight = ko.observable(null);
    self.totalSumVolume = ko.observable(null);

    self.widthConverted = ko.computed(function () {
        var width = Globalize.parseFloat(self.width());
        var length = Globalize.parseFloat(self.length());
        var height = Globalize.parseFloat(self.height());

        return width && length && height ? width * length * height / 5000 : 0;
    }, this);

    self.isScanCode = ko.observable(true);
    self.lastMessage = ko.observable("");

    self.changeIsScanCode = function () {
        self.isScanCode(!self.isScanCode());
    }

    self.targetWarehouseId.subscribe(function (warehouseId) {
        var warehouse = _.find(self.allWarehouse(), function (w) { return w.id === warehouseId; });

        if (warehouse) {
            self.targetWarehouseId(warehouse.id);
            self.targetWarehouseIdPath(warehouse.idPath);
            self.targetWarehouseName(warehouse.name);
            self.targetWarehouseAddress(warehouse.address);
        } else {
            self.targetWarehouseId(null);
            self.targetWarehouseIdPath(null);
            self.targetWarehouseName(null);
            self.targetWarehouseAddress(null);
        }
    });

    self.changeIsSameWallet = function () {
        self.isSameWallet(!self.isSameWallet());
    }

    self.resetForm = function () {
        self.id(0);
        self.note("");
        self.code("");
        self.mode(0);
        self.status(1);
        self.targetWarehouseId(null);
        self.entrepotId(null);
        self.isSameWallet(null);
        self.targetWarehouseIdPath("");
        self.targetWarehouseName("");
        self.targetWarehouseAddress("");
        self.userId(0);
        self.userName("");
        self.userFullName("");
        self.created();
        self.updated();
        self.packageNo(0);
        self.totalValue(null);
        self.weight(null);
        self.totalConversionWeight(null);
        self.totalConversionWeightValue(null);
        self.totalActualWeight(null);
        self.currentWarehouseId(0);
        self.currentWarehouseIdPath("");
        self.currentWarehouseName("");
        self.currentWarehouseAddress("");
        self.createdWarehouseId(0);
        self.createdWarehouseIdPath("");
        self.createdWarehouseName("");
        self.createdWarehouseAddress("");

        self.isLoadingItems(false);
        self.isLoading(false);

        self.items.removeAll();

        self.width(null);
        self.length(null);
        self.height(null);
        self.size(null);

        self.initInputMark();
        self.removeRules();
    }

    self.removeRules = function () {
        $("#walletForm #Width").rules("remove", "number");
        $("#walletForm #Length").rules("remove", "number");
        $("#walletForm #Height").rules("remove", "number");
        $("#walletForm #Weight").rules("remove", "number");
    }

    self.setForm = function (data) {
        self.showAddForm();

        self.id(data.id);
        self.note(data.note);
        self.code(data.code);
        self.mode(data.mode);
        self.status(data.status);
        self.targetWarehouseId(data.targetWarehouseId);
        self.isSameWallet(data.isSameWallet);
        self.targetWarehouseIdPath(data.targetWarehouseIdPath);
        self.targetWarehouseName(data.targetWarehouseName);
        self.targetWarehouseAddress(data.targetWarehouseAddress);
        self.userId(data.userId);
        self.userName(data.userName);
        self.userFullName(data.userFullName);
        self.created(data.created);
        self.updated(data.updated);
        self.packageNo(data.packageNo);
        self.totalValue(data.totalValue);
        self.totalConversionWeight(data.totalConversionWeight);
        self.totalActualWeight(data.totalActualWeight);
        self.currentWarehouseId(data.currentWarehouseId);
        self.currentWarehouseIdPath(data.currentWarehouseIdPath);
        self.currentWarehouseName(data.currentWarehouseName);
        self.currentWarehouseAddress(data.currentWarehouseAddress);
        self.createdWarehouseId(data.createdWarehouseId);
        self.createdWarehouseIdPath(data.createdWarehouseIdPath);
        self.createdWarehouseName(data.createdWarehouseName);
        self.createdWarehouseAddress(data.createdWarehouseAddress);

        self.weight(formatNumberic(data.weight, 'N2'));
        self.width(formatNumberic(data.width, 'N2'));
        self.length(formatNumberic(data.length, 'N2'));
        self.height(formatNumberic(data.height, 'N2'));
        self.size(data.size);

        self.initInputMark();
        // Get detail của phiếu nhập kho
        self.getDetail();
    }

    //self.totalActualWeightComputed = ko.computed(function () {
    //    var width = Globalize.parseFloat(self.width());
    //    var length = Globalize.parseFloat(self.length());
    //    var height = Globalize.parseFloat(self.height());

    //    if (isNaN(width) || isNaN(length) || isNaN(height))
    //        return null;

    //    return width * length * height / 5000;
    //}, this);

    // Tính lại cân nặng,...
    self.refreshWallet = function () {
        self.packageNo(formatNumberic(self.items().length));
        self.totalValue(formatNumberic(_.sumBy(self.items(), "totalValue"), 'N2'));
        self.totalSumWeight(formatNumberic(_.sumBy(self.items(), "weight"), 'N2'));
        self.totalSumVolume(formatNumberic(_.sumBy(self.items(), "volume"), 'N2'));
        self.totalConversionWeight(formatNumberic(_.sumBy(self.items(), "convertedWeight"), 'N2'));
        self.totalActualWeight(formatNumberic(_.sumBy(self.items(), "actualWeight"), 'N2'));

        self.totalConversionWeightValue(_.sumBy(self.items(), "convertedWeight"));
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
                it.packageNoInWallet = _.filter(list, function (d) {
                    return d.orderCode === it.orderCode;
                }).length;
            }
        });
        self.items.removeAll();
        self.items(list);

        self.refreshWallet();
    }

    self.getDetail = function () {
        self.isLoadingItems(true);
        $.get("/wallet/getpackages", { id: self.id() }, function (data) {
            self.isLoadingItems(false);

            var firstOrderCode;
            _.each(data, function (it) {
                it.isFirst = ko.observable(firstOrderCode !== it.orderCode);
                firstOrderCode = it.orderCode;

                if (it.isFirst) {
                    it.packageNoInWallet = _.filter(data, function (d) {
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

            self.refreshWallet();
        });
    }

    self.getFormData = function () {
        return {
            id: self.id(),
            note: self.note(),
            code: self.code(),
            mode: self.mode(),
            status: self.status(),
            targetWarehouseId: self.targetWarehouseId(),
            entrepotId: self.entrepotId(),
            isSameWallet: self.isSameWallet(),
            targetWarehouseIdPath: self.targetWarehouseIdPath(),
            targetWarehouseName: self.targetWarehouseName(),
            targetWarehouseAddress: self.targetWarehouseAddress(),
            packageNo: self.packageNo(),
            userId: self.userId(),
            userName: self.userName(),
            userFullName: self.userFullName(),
            created: self.created(),
            updated: self.updated(),
            currentWarehouseId: self.currentWarehouseId(),
            currentWarehouseIdPath: self.currentWarehouseIdPath(),
            currentWarehouseName: self.currentWarehouseName(),
            currentWarehouseAddress: self.currentWarehouseAddress(),
            createdWarehouseId: self.createdWarehouseId(),
            createdWarehouseIdPath: self.createdWarehouseIdPath(),
            createdWarehouseName: self.createdWarehouseName(),
            createdWarehouseAddress: self.createdWarehouseAddress(),
            width: self.width(),
            length: self.length(),
            weight: self.weight(),
            height: self.height()
        }
    }

    self.showAddForm = function (packageCodes) {
        self.resetForm();
        resetForm("#walletForm");
        orderServiceOtherModal.resetValue();

        $("#walletAddOrEdit").modal("show");

        if (packageCodes !== undefined) {
            self.mode(1);
            self.isLoadingItems(true);
            $.get("/package/getforpacking",
                { packageCodes: packageCodes },
                function (data) {
                    self.isLoadingItems(false);
                    _.each(data,
                        function (item, idx) {
                            if (idx === 0) {
                                self.targetWarehouseId(item.customerWarehouseId);
                                self.targetWarehouseIdPath(item.customerWarehouseIdPath);
                                self.targetWarehouseName(item.customerWarehouseName);
                                self.targetWarehouseAddress(item.customerWarehouseAddress);
                            }

                            item.orderPackageNo = item.packageNo;
                            item.isFirst = ko.observable(false);
                            item.walletId = 0;
                            item.walletCode = "";
                            item.packageId = item.id;
                            item.packageCode = item.code;
                            item.id = 0;
                            item.note = ko.observable(item.note);
                            item.convertedWeight = item.weightConverted;
                            item.actualWeight = item.weightActual;
                            item.status = 1;
                            item.isHighline = false;
                        });

                    self.items(data);
                    self.reOrderBy();
                    self.initInputMark();
                });
        }
    }

    self.showUpdateForm = function (data) {
        self.setForm(data);
        resetForm("#walletForm");
        $("#walletAddOrEdit").modal("show");
    }

    self.loadingItems = function (walletId) {
        $.get("/wallet/getpackages", { id: walletId }, function (data) {
            self.items(data);
        });
    }

    self.updatePackage = function (package) {
        var data = ko.mapping.toJS(package);

        data["__RequestVerificationToken"] = self.token;
        self.isLoading(true);
        $.post("/wallet/updatepackage", data, function (rs) {
            self.isLoading(false);
            if (rs.status < 0) {
                toastr.error(rs.text);
                return;
            }

            self.refreshWallet();
        });
    }

    self.removeItem = function (item) {
        if (self.id() === 0) {
            self.items.remove(item);

            self.refreshWallet();

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
                $.post("/wallet/deletepackage", item,
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

    self.token = $("#walletForm input[name='__RequestVerificationToken']").val();

    self.save = function () {
        if (!$("#walletForm").valid()) {
            toastr.error("Check the entered fields!");
            $(".input-validation-error:first").focus();
            return;
        }

        if (self.items().length === 0) {
            toastr.error("Requisition for warehousing is not available.");
            return;
        }

        var data = self.getFormData();
        data.packages = ko.mapping.toJS(self.items());
        data.OrderServiceOthers = orderServiceOtherModal.getData();
        data["__RequestVerificationToken"] = self.token;

        var hasError;
        // Validate phí phát sinh
        _.each(data.OrderServiceOthers, function (s) {
            s.orderCode = _.trim(s.orderCode);
            s.value = _.trim(s.value);
            if (s.orderCode.length === 0 || s.value.length === 0) {
                hasError = true;
                toastr.warning("Orders code and the value of money in items incurred can not be blank");
                return false;
            }

            if (_.isNil(_.find(data.packages, function (p) { return p.orderCode === s.orderCode; }))) {
                hasError = true;
                toastr.warning('Code orders "' + s.orderCode + '" not included in this entry');
                return false;
            }
        });

        if (hasError)
            return;

        // Hàng tối ưu :Line = 2: Đông Hưng
        if (self.modeDelivery() === 1 && (self.mode() === 0 || self.mode() === 1
            && self.isSameWallet()) && self.entrepotId() !== 2) {
            swal({
                title: 'Confirm the optimal shipping cover does not go "Dongxing"?',
                text: 'By default the system requires the optimal transport needs to follow the line "Dongxing"',
                type: 'warning',
                showCancelButton: true,
                //confirmButtonColor: '#3085d6',
                //cancelButtonColor: '#d33',
               cancelButtonText: 'Cancel',
               confirmButtonText: 'Confirm'
            }).then(function () {
                self.saveSubmit(data);
            }, function () { });
            return;
        }

        self.saveSubmit(data);
    }

    self.saveSubmit = function(data) {
        // Cập nhật
        if (self.id() > 0) {
            self.isLoading(true);
            $.post("/wallet/update", data, function (rs) {
                self.isLoading(false);

                if (rs.status < 0) {
                    toastr.error(rs.text);
                    return;
                }

                toastr.success(rs.text);
                // reset form
                self.resetForm();
                resetForm("#walletForm");
                $("#walletAddOrEdit").modal("hide");

                if (callback)
                    callback();
            });
        } else { // Thêm mới
            self.isLoading(true);
            $.post("/wallet/add", data, function (rs) {
                self.isLoading(false);

                // Confirm khi là bao hàng vc tối ưu không đi theo line Đông Hưng
                if (rs.status === -1000) {
                    swal({
                        title: 'Confirm the optimal shipping cover does not go "Dongxing"?',
                        text: 'By default the system requires the optimal transport needs to follow the line "Dongxing"',
                        type: 'warning',
                        showCancelButton: true,
                        //confirmButtonColor: '#3085d6',
                        //cancelButtonColor: '#d33',
                       cancelButtonText: 'Cancel',
                       confirmButtonText: 'Confirm'
                    }).then(function () {
                        data["isConfirm"] = true;
                        self.saveSubmit(data);
                    },
                        function () { });
                    return;
                }

                if (rs.status < 0) {
                    toastr.error(rs.text);
                    return;
                }

                if (rs["wallet"] && rs["data"] == null) {
                    swal(
                        'Create ' + (data.mode === 1 ? 'package' : 'sacks') + ' success',
                        'Code ' + (data.mode === 1 ? 'package' : 'sacks')
                        + ': ' + rs["wallet"].code
                        + ', shallow: ' + formatNumberic(rs["wallet"].weight, "N2")
                        + ', exchange: ' + formatNumberic(rs["wallet"].weightConverted, "N2")
                        + (data.mode === 0 ? '(kg), the go: ' + rs["wallet"].entrepotName : ''),
                        'success'
                    );
                } else if (rs["wallet"] && rs["data"]) {
                    swal(
                        'Make wood bales & sacks successfully',
                        'Sacks code: ' + rs["wallet"].code
                        + ', shallow: ' + formatNumberic(rs["wallet"].weight, "N2")
                        + ', exchange: ' + formatNumberic(rs["wallet"].weightConverted, "N2")
                        + '(kg), the go: ' + rs["wallet"].entrepotName
                        + ', the code package: ' + rs['data'].code,
                        'success'
                    );

                    toastr.success(rs.text);
                }

                // reset form
                self.resetForm();
                resetForm("#walletForm");
                $("#walletAddOrEdit").modal("hide");

                if (callback)
                    callback();
            });
        }
    }

    self.initInputMark = function () {
        $('#walletAddOrEdit input.decimal').each(function () {
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

    self.showOrderServiceOther = function () {
        orderServiceOtherModal.show();
    }

    self.orderCodesNoAudit = [];
    self.orderCodesAuditLose = [];
    self.orderCodesNoPacking = [];
    self.orderCodesNoPackingService = [];
    self.orderCodesFastDelivery = [];
    self.orderCodesOptimal = [];

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

                isPaste = self.isScanCode() || isPaste || isScannerInput() ? true : false;

                $.post('/package/Suggetion2', { term: request.term, packageCodes: strCodes, targetWarehouseId: self.targetWarehouseId(), isPaste: isPaste }, function (result) {
                    self.orderCodesNoAudit = result.orderCodesNoAudit;
                    self.orderCodesAuditLose = result.orderCodesAuditLose;
                    self.orderCodesNoPacking = result.orderCodesNoPacking;
                    self.orderCodesNoPackingService = result.orderCodesNoPackingService;
                    self.orderCodesFastDelivery = result.orderCodesFastDelivery;
                    self.orderCodesOptimal = result.orderCodesOptimal;

                    if (isPaste) {
                        if (result.packages.length === 0) {
                            var msg = 'Cannot find the event with the tag "' + request.term + '"';
                            if (self.isScanCode()) {
                                swal({
                                    title: 'Error!',
                                    text: msg,
                                    type: 'error',
                                    timer: 2000
                                }).then(function () {
                                }, function () { });

                                self.lastMessage(msg);
                                return;
                            }
                            toastr.error(msg);
                            self.lastMessage(msg);
                            return;
                        }

                        self.processItemSelected(result.packages[0]);

                        isPaste = false;
                        $('#suggetion').val("");
                        resetValues();
                    } else {
                        return response(result.packages);
                    }
                });
            },
            select: function (event, ui) {
                self.processItemSelected(ui.item);
                resetValues();
            }
        }).autocomplete("instance")._renderItem = function (ul, item) {
            var name = self.suggetionType() == 0 ? item.transportCode + ' - P' + item.code + ' - ' + 'ODR' + item.orderCode + ' - destination warehouse: ' + item.customerWarehouseName
                : item.code + ' - destination warehouse: ' + item.customerWarehouseName;

            return $("<li>").addClass('media media-line').append('<div>' +
                '<h4 class="media-heading bold size-16 pr-mgb-0">' + name +
                '</h4></div>').appendTo(ul).addClass('automember media-list');
        };
    }

    self.processItemSelected = function (item) {
        item.orderPackageNo = item.packageNo;
        item.isFirst = ko.observable(false);
        item.walletId = 0;
        item.walletCode = "";
        item.packageId = item.id;
        item.packageCode = item.code;
        item.id = 0;
        item.note = ko.observable(item.note);
        item.convertedWeight = item.weightConverted;
        item.actualWeight = item.weightActual;
        item.status = 1;

        if (self.id() > 0) {
            var package = ko.mapping.toJS(item);

            package.walletId = self.id();
            package.walletCode = self.code();
            package["__RequestVerificationToken"] = self.token;

            self.isLoading(true);
            $.post("/wallet/addpackage", package, function (rs) {
                self.isLoading(false);
                if (rs.status < 0) {
                    toastr.error(rs.text);
                    return;
                }
                self.getDetail();
            });
            return;
        }
        var msg;
        if (!_.find(self.items(), function (it) { return it.packageCode === item.code })) {
            if (self.items().length === 0) {
                self.targetWarehouseId(item.customerWarehouseId);
                self.targetWarehouseIdPath(item.customerWarehouseIdPath);
                self.targetWarehouseName(item.customerWarehouseName);
                self.targetWarehouseAddress(item.customerWarehouseAddress);

                if (_.find(self.orderCodesFastDelivery, function (orderCode) { return orderCode === item.orderCode; })) {
                    // VC nhanh
                    self.modeDelivery(2);
                } else if (_.find(self.orderCodesOptimal, function (orderCode) { return orderCode === item.orderCode; })) {
                    // VC tối ưu
                    self.modeDelivery(1);
                } else {
                    // VC thông thường
                    self.modeDelivery(0);
                }
            }

            var isError = false;
            if (_.find(self.orderCodesNoAudit, function (orderCode) { return orderCode === item.orderCode; })) {
                isError = true;
                msg = '"' + window.ReturnCode(item.orderCode) + '"' + ' MVĐ "' + item.transportCode + '" not yet tally';
            } else if (_.find(self.orderCodesAuditLose, function (orderCode) { return orderCode === item.orderCode; })) {
                isError = true;
                msg = '"' + window.ReturnCode(item.orderCode) + '"' + ' MVĐ "' + item.transportCode + '" the wrong tally is not yet processed';
            } else if (self.mode() === 0 && _.find(self.orderCodesNoPacking, function (orderCode) { return orderCode === item.orderCode; })) {
                isError = true;
                msg = '"' + window.ReturnCode(item.orderCode) + '"' + ' MVĐ "' + item.transportCode + '" not yet packing wood';
            } else if (self.mode() === 1 && _.find(self.orderCodesNoPackingService, function (orderCode) { return orderCode === item.orderCode; })) {
                isError = true;
                msg = '"' + window.ReturnCode(item.orderCode) + '"' + ' MVĐ "' + item.transportCode + '" do not use the services close wood';
            } else if (self.mode() === 1 && _.find(self.items(), function (it) { return it.customerId === item.customerId }) == null) {
                isError = true;
                msg = '"' + window.ReturnCode(item.orderCode) + '"' + ' MVĐ "' + item.transportCode + '" not the same guests are packing wood';
            } else if (self.items().length > 0 && self.modeDelivery() !== 2 && _.find(self.orderCodesFastDelivery, function (orderCode) { return orderCode === item.orderCode; })) {
                isError = true;
                msg = ' MVĐ "' + item.transportCode + '" is not possible to go fast in this cover';
            } else if (self.items().length > 0 && self.modeDelivery() !== 1 && _.find(self.orderCodesOptimal, function (orderCode) { return orderCode === item.orderCode; })) {
                isError = true;
                msg = ' MVĐ "' + item.transportCode + '" is now shipping the optimization could not close out cover this';
            } else if (self.items().length > 0 && (self.modeDelivery() === 2 &&
                _.find(self.orderCodesFastDelivery, function (orderCode) { return orderCode === item.orderCode; }) === null
                || self.modeDelivery() === 1 && _.find(self.orderCodesOptimal, function (orderCode) { return orderCode === item.orderCode; }) === null)) {
                isError = true;
                msg = 'MVĐ "' + item.transportCode + '" is going slow could not close out cover this';
            } else if (self.items().length > 0 && item.customerWarehouseId !== self.targetWarehouseId()) {
                isError = true;
                msg = 'MVĐ "' + item.transportCode + '" do not select the repository with the same event are in the bag';
            } else if (item.mode != null && item.sameCodeStatus === 0) {
                isError = true;
                msg = 'MVĐ "' + item.transportCode + '" being unresolved infection';
            }

            if (isError) {
                if (self.isScanCode()) {
                    swal({
                        title: 'Error!',
                        text: msg,
                        type: 'error',
                        timer: 2000
                    }).then(function () {
                    }, function () { });

                    self.lastMessage(msg);
                    return;
                }

                toastr.success(msg);
                self.lastMessage(msg);
                return;
            }

            item.isHighline = true;
            self.items.push(item);

            self.reOrderBy(item.packageId);
            self.initInputMark();
            msg = 'Successfully added tracking code "' + item.transportCode + '"';
            if (self.isScanCode()) {
                swal({
                    title: 'Success!',
                    text: msg,
                    type: 'success',
                    timer: 2000
                }).then(function () {
                }, function () { });

                self.lastMessage(msg);
                return;
            }
            toastr.success(msg);
            self.lastMessage(msg);
        } else {
            msg = 'Already in list of tracking code "' + item.transportCode + '"';
            if (self.isScanCode()) {
                swal({
                    title: 'Infection code!',
                    text: msg,
                    type: 'warning',
                    timer: 2000
                }).then(function () {
                }, function () { });

                self.lastMessage(msg);
                return;
            }
            toastr.warning(msg);
            self.lastMessage(msg);
        }
    }

    $(function () {
        self.initSuggetion();
    });
}
