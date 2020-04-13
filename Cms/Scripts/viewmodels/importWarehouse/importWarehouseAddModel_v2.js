function ImportWarehouseAddModel(callback, packageDetailModal, orderDetailModal, walletDetailModal, orderServiceOtherModal) {
    var self = this;

    self.statesGroupBy = _.groupBy(window["states"] ? window.states : [], "id");
    self.isLoading = ko.observable(false);
    self.isLoadingItems = ko.observable(false);
    self.suggetionType = ko.observable(window.viewMode); // 0: package, 1: Bao hàng

    self.changeSuggetionType = function (s) {
        self.suggetionType(s);
    }

    self.items = ko.observableArray([]);

    // model
    self.id = ko.observable(0);
    self.shipperName = ko.observable("");
    self.shipperPhone = ko.observable("");
    self.shipperAddress = ko.observable("");
    self.shipperEmail = ko.observable("");
    self.note = ko.observable("");
    self.code = ko.observable("");
    self.status = ko.observable(1);
    self.warehouseId = ko.observable(0);
    self.warehouseIdPath = ko.observable("");
    self.warehouseName = ko.observable("");
    self.warehouseAddress = ko.observable("");
    self.packageNumber = ko.observable(0);
    self.userId = ko.observable(0);
    self.userName = ko.observable("");
    self.userFullName = ko.observable("");
    self.created = ko.observable();
    self.lastUpdated = ko.observable();
    self.warehouseManagerId = ko.observable(0);
    self.warehouseManagerCode = ko.observable("");
    self.warehouseManagerFullName = ko.observable("");
    self.warehouseAccountantId = ko.observable(0);
    self.warehouseAccountantCode = ko.observable("");
    self.warehouseAccountantFullName = ko.observable("");
    self.walletNumber = ko.observable(0);
    self.isShowInfo = ko.observable(false);
    self.transportCodes = ko.observableArray([]);
    self.lastMessage = ko.observable("");
    self.isScanCode = ko.observable(true);

    self.changeIsScanCode = function () {
        self.isScanCode(!self.isScanCode());
    }

    self.removeTransportCode = function (data) {
        swal({
            title: 'Are you sure you want to delete this item?',
            text: 'The waybill code has lost this information',
            type: 'question',
            showCancelButton: true,
           cancelButtonText: 'Cancel',
            confirmButtonText: 'Delete'
        })
            .then(function () {
                self.isLoading(true);

                var packageData = {
                    transportCode: data.transportCode
                }
                packageData["__RequestVerificationToken"] = self.token;

                $.post("/ImportWarehouse/PackageNoCodeDelete",
                    packageData,
                    function (rs) {
                        if (rs.status < 0) {
                            toastr.error(rs.text);
                            return;
                        }

                        toastr.success(rs.text);
                        self.transportCodes.remove(data);
                    });
            }, function () { });
    }

    self.showTransportCodesModal = function () {
        $("#transportCodesModal").modal("show");
    }

    self.chageIsShowInfo = function () {
        self.isShowInfo(!self.isShowInfo());
    }

    self.resetForm = function () {
        self.id(0);
        self.shipperName("");
        self.shipperPhone("");
        self.shipperAddress("");
        self.shipperEmail("");
        self.note("");
        self.code("");
        self.lastMessage("");
        self.status(1);
        self.warehouseId(0);
        self.warehouseIdPath("");
        self.warehouseName("");
        self.warehouseAddress("");
        self.packageNumber(0);
        self.userId(0);
        self.userName("");
        self.userFullName("");
        self.created(null);
        self.lastUpdated(null);
        self.warehouseManagerId(0);
        self.warehouseManagerCode("");
        self.warehouseManagerFullName("");
        self.warehouseAccountantId(0);
        self.warehouseAccountantCode("");
        self.warehouseAccountantFullName("");
        self.walletNumber(0);
        self.isLoadingItems(false);
        self.isLoading(false);
        self.transportCodes.removeAll();
        self.items.removeAll();
    }

    self.setForm = function (data) {
        self.showAddForm();

        self.id(data.id);
        self.shipperName(data.shipperName);
        self.shipperPhone(data.shipperPhone);
        self.shipperAddress(data.shipperAddress);
        self.shipperEmail(data.shipperEmail);
        self.note(data.note);
        self.code(data.code);
        self.status(data.status);
        self.warehouseId(data.warehouseId);
        self.warehouseIdPath(data.warehouseIdPath);
        self.warehouseName(data.warehouseName);
        self.warehouseAddress(data.warehouseAddress);
        self.packageNumber(data.packageNumber);
        self.userId(data.userId);
        self.userName(data.userName);
        self.userFullName(data.userFullName);
        self.created(data.created);
        self.lastUpdated(data.lastUpdated);
        self.warehouseManagerId(data.warehouseManagerId);
        self.warehouseManagerCode(data.warehouseManagerCode);
        self.warehouseManagerFullName(data.warehouseManagerFullName);
        self.warehouseAccountantId(data.warehouseAccountantId);
        self.warehouseAccountantCode(data.warehouseAccountantCode);
        self.warehouseAccountantFullName(data.warehouseAccountantFullName);
        self.walletNumber(data.walletNumber);

        // Get detail của phiếu nhập kho
        self.getDetail();
    }

    self.reOrderBy = function (packageId) {
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

            it.isHighline = packageId != undefined && packageId != null
                && it.packageId === packageId ? true : false;

            if (it.isFirst) {
                it.packageNoInWallet = _.filter(list, function (d) {
                    return d.orderCode === it.orderCode;
                }).length;
            }
        });
        self.items.removeAll();
        self.items(list);
    }

    self.notePackageFocus = "";
    self.packageNoteFocus = function (data) {
        self.notePackageFocus = data.note();
    }


    self.addPackageForm = ko.observable();
    self.showAddPackage = function () {
        if (self.addPackageForm() == null) {
            self.addPackageForm(new AddPackageModel());
            ko.applyBindings(self.addPackageForm(), $("#addPackageModal")[0]);
        }

        self.addPackageForm().show();
    }

    self.showAddPackageCallback = function (data) {
        if (self.addPackageForm() == null) {
            self.addPackageForm(new AddPackageModel());
            ko.applyBindings(self.addPackageForm(), $("#addPackageModal")[0]);
        }

        self.addPackageForm().show(data.transportCode, function () {
            self.transportCodes.remove(data);
        }, data.packageId);
    }

    self.addPackageLoseForm = ko.observable();
    self.showAddPackageLose = function () {
        if (self.addPackageLoseForm() == null) {
            self.addPackageLoseForm(new AddPackageLoseModel());
            ko.applyBindings(self.addPackageLoseForm(), $("#addPackageLoseModal")[0]);
        }

        self.addPackageLoseForm().show();
    }

    self.showAddPackageLoseCallback = function (data) {
        if (self.addPackageLoseForm() == null) {
            self.addPackageLoseForm(new AddPackageLoseModel());
            ko.applyBindings(self.addPackageLoseForm(), $("#addPackageLoseModal")[0]);
        }

        self.addPackageLoseForm().show(data.transportCode, function () {
            self.transportCodes.remove(data);
        }, data.packageId);
    }

    self.getDetail = function (packageId) {
        self.isLoadingItems(true);
        $.get("/importwarehouse/getdetail", { id: self.id(), viewMode: window.viewMode }, function (data) {
            self.isLoadingItems(false);

            if (packageId) {
                _.each(data, function (it) {
                    it.isHighline = packageId != undefined && packageId != null
                        && it.packageId === packageId ? true : false;
                });
            }

            var list = _.orderBy(data, ['orderCode', 'isHighline'], ['asc', 'desc']);

            // Đẩy lại toàn bộ kiện của Orders mới thêm lên đầu danh sách
            if (packageId) {
                var order = _.find(list, function (it) { return it.packageId === packageId; });
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

                if (packageId == undefined || packageId == null) {
                    it.isHighline = false;
                }

                if (it.isFirst) {
                    it.packageNoInWallet = _.filter(list, function (d) {
                        return d.orderCode === it.orderCode;
                    }).length;
                }
                it.note = ko.observable(it.note);
                it.note.subscribe(function (newValue) {
                    if (self.notePackageFocus !== newValue)
                        self.updatePackage(it);
                });
            });

            //_.each(data, function (it) {
            //    it.note = ko.observable(it.note);
            //    it.note.subscribe(function (newValue) {
            //        if (self.notePackageFocus !== newValue)
            //            self.updatePackage(it);
            //    });
            //});
            self.items(list);
        });
    }

    self.getFormData = function () {
        return {
            id: self.id(),
            shipperName: self.shipperName(),
            shipperPhone: self.shipperPhone(),
            shipperAddress: self.shipperAddress(),
            shipperEmail: self.shipperEmail(),
            note: self.note(),
            code: self.code(),
            status: self.status(),
            warehouseId: self.warehouseId(),
            warehouseIdPath: self.warehouseIdPath(),
            warehouseName: self.warehouseName(),
            warehouseAddress: self.warehouseAddress(),
            packageNumber: self.packageNumber(),
            userId: self.userId(),
            userName: self.userName(),
            userFullName: self.userFullName(),
            created: self.created(),
            lastUpdated: self.lastUpdated(),
            warehouseManagerId: self.warehouseManagerId(),
            warehouseManagerCode: self.warehouseManagerCode(),
            warehouseManagerFullName: self.warehouseManagerFullName(),
            warehouseAccountantId: self.warehouseAccountantId(),
            warehouseAccountantCode: self.warehouseAccountantCode(),
            warehouseAccountantFullName: self.warehouseAccountantFullName(),
            walletNumber: self.walletNumber()
        }
    }

    self.showAddForm = function () {
        self.resetForm();
        resetForm("#importWarehouseForm");
        $("#importWarehouseAddOrEdit").modal("show");

        if (orderServiceOtherModal)
            orderServiceOtherModal.resetValue();
    }

    self.showUpdateForm = function (data) {
        self.setForm(data);
        resetForm("#importWarehouseForm");
        $("#importWarehouseAddOrEdit").modal("show");
    }

    self.loadingItems = function (importWarehouseId) {
        $.get("/importwarehouse/getdetail", { id: importWarehouseId }, function (data) {
            self.items(data);
        });
    }

    self.updatePackage = function (package) {
        var data = ko.mapping.toJS(package);

        data["__RequestVerificationToken"] = self.token;
        self.isLoading(true);
        $.post("/importwarehouse/updatepackage", data, function (rs) {
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
            return;
        }

        var typeText = item.type === 0 ? "Goods package" : "Goods sack";
        swal({
            title: 'Are you sure you want to delete this item?',
            text: typeText + ' "' + item.packageCode + '"',
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
                $.post("/importwarehouse/deletepackage", item,
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

    self.token = $("#importWarehouseForm input[name='__RequestVerificationToken']").val();

    self.save = function () {
        if (!$("#importWarehouseForm").valid()) {
            toastr.error("Check the fields entered!");
            $(".input-validation-error:first").focus();
            return;
        }

        if (self.items().length === 0) {
            toastr.error("Requisition for warehousing does not have any goods packages/sacks!");
            return;
        }

        var data = self.getFormData();
        data.packages = ko.mapping.toJS(self.items());
        data.transportCodes = self.transportCodes();
        data.OrderServiceOthers = orderServiceOtherModal.getData();
        data["__RequestVerificationToken"] = self.token;

        var hasError;
        // Validate phí phát sinh
        _.each(data.OrderServiceOthers, function (s) {
            s.orderCode = _.trim(s.orderCode);
            s.value = _.trim(s.value);

            if (s.orderCode.length === 0 || s.value.length === 0) {
                hasError = true;
                toastr.warning("Order code and amount of money in incurred expenses section may not be left blank");
                return false;
            }

            if (_.isNil(_.find(data.packages, function (p) { return p.orderCode === s.orderCode; }))) {
                hasError = true;
                toastr.warning('Order code "' + s.orderCode + '" is not included in this entry');
                return false;
            }
        });

        if (hasError)
            return;

        // Cập nhật
        if (self.id() > 0) {
            self.isLoading(true);
            $.post("/importwarehouse/update", data, function (rs) {
                self.isLoading(false);

                if (rs.status < 0) {
                    toastr.error(rs.text);
                    return;
                }

                toastr.success(rs.text);
                // reset form
                self.resetForm();
                resetForm("#importWarehouseForm");
                $("#importWarehouseAddOrEdit").modal("hide");

                if (callback)
                    callback();
            });
        } else { // Thêm mới
            self.isLoading(true);
            $.post("/importwarehouse/add", data, function (rs) {
                self.isLoading(false);
                if (rs.status < 0) {
                    toastr.error(rs.text);
                    return;
                }

                toastr.success(rs.text);
                // reset form
                self.resetForm();
                resetForm("#importWarehouseForm");
                $("#importWarehouseAddOrEdit").modal("hide");

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

    self.showOrderServiceOther = function () {
        orderServiceOtherModal.show(self.id(), self.code(), self.items());
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
            if (e.which == 13) {
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
                var url = self.suggetionType() == 0 ? '/package/Suggetion' : '/wallet/Suggetion';

                var codes = _.map(self.items(), "packageCode");

                var strCodes = codes.length === 0 ? "" : ';' + _.join(codes, ';') + ';';

                isPaste = self.isScanCode() || isPaste || isScannerInput() ? true : false;

                //isPaste = self.suggetionType() == 0 ? isPaste : false;

                $.post(url, { term: request.term, packageCodes: strCodes, isPaste: isPaste }, function (result) {
                    if (isPaste) {
                        if (result.length > 0) {
                            self.processItemSelected(result[0]);
                        } else {
                            if (_.trim($('#suggetion').val("")).length > 0) {

                                var packageData = {
                                    transportCode: _.trim(request.term)
                                }
                                packageData["__RequestVerificationToken"] = self.token;

                                $.post("/ImportWarehouse/AddPackageNoCode",
                                    packageData,
                                    function(rs) {
                                        self.lastMessage(rs.text);
                                        if (rs.status < 0) {
                                            if (self.isScanCode()) {
                                                swal({
                                                    title: 'Error!',
                                                    text: rs.text,
                                                    type: 'error',
                                                    timer: 2000
                                                }).then(function() {
                                                    },
                                                    function () { });
                                                return;
                                            }
                                            toastr.error(rs.text);
                                            return;
                                        }
                                        if (self.isScanCode()) {
                                            swal({
                                                title: 'Successful!',
                                                text: rs.text,
                                                type: 'warning',
                                                timer: 2000
                                            }).then(function () {
                                            }, function () { });
                                        } else {
                                            toastr.success(rs.text);
                                        }
                                        self.transportCodes.push({ transportCode: _.trim(request.term), packageId: rs.data.id });
                                    });
                            }
                        }

                        isPaste = false;
                        $('#suggetion').val("");
                        resetValues();
                    } else {
                        return response(result);
                    }
                });
            },
            select: function (event, ui) {
                var item = ui.item;

                self.processItemSelected(item);
                resetValues();
            }
        }).autocomplete("instance")._renderItem = function (ul, item) {
            var name = self.suggetionType() == 0 ? item.transportCode + ' - P' + item.code + ' - ' + 'ODR' + item.orderCode + ' - Customer: ' + item.customerName + '(' + item.customerUserName + ')' + ' - destination warehouse: ' + item.customerWarehouseName
                : item.code + ' - Customer: ' + item.customers + ' - Warehouse:' + item.targetWarehouseName;

            return $("<li>").addClass('media media-line').append('<div>' +
                '<h4 class="media-heading bold size-16 pr-mgb-0">' + name +
                '</h4></div>').appendTo(ul).addClass('automember media-list');
        };
    }

    self.processItemSelected = function (item) {
        item.isFirst = ko.observable(false);
        item.orderPackageNo = item.packageNo;
        item.type = self.suggetionType();
        item.packageId = item.id;
        item.packageCode = item.code;
        item.note = ko.observable(item.note);
        item.status = 1;
        item.id = 0;

        if (self.suggetionType() == 1) {
            item.orderCode = item.orderCodes;
            item.warehouseIdPath = item.targetWarehouseIdPath;
            item.warehouseName = item.targetWarehouseName;
            item.warehouseAddress = item.targetWarehouseAddress;
            item.warehouseId = item.targetWarehouseId;
        } else if (self.suggetionType() == 0) {
            item.warehouseIdPath = item.customerWarehouseIdPath;
            item.warehouseName = item.customerWarehouseName;
            item.warehouseAddress = item.customerWarehouseAddress;
            item.warehouseId = item.customerWarehouseId;
        }

        if (self.id() > 0) {
            var package = ko.mapping.toJS(item);

            var packageData = {
                packageId: package.packageId,
                importWarehouseId: self.id(),
                packageCode: package.packageCode,
                importWarehouseCode: self.code(),
                note: package.note
            }
            packageData["__RequestVerificationToken"] = self.token;

            self.isLoading(true);
            $.post("/importwarehouse/AddPackage", packageData, function (rs) {
                self.isLoading(false);
                self.lastMessage(rs.text);
                if (rs.status < 0) {    
                    if (self.isScanCode()) {
                        swal({
                            title: 'Error!',
                            text: rs.text,
                            type: 'error',
                            timer: 2000
                        }).then(function () {
                            }, function () { });
                        return;
                    }

                    toastr.error(rs.text);
                    return;
                }

                if (self.isScanCode()) {
                    swal({
                        title: 'Successful!',
                        text: rs.text,
                        type: 'success',
                        timer: 2000
                    }).then(function () {
                    }, function () { });
                } else {
                    toastr.success(rs.text);
                }

                self.getDetail(packageData.packageId);

                if (callback)
                    callback();
            });
        }

        if (_.findIndex(self.items(), function (it) { return it.packageCode === item.packageCode }) < 0) {
            item.isHighline = true;
            self.items.push(item);
            self.reOrderBy(item.packageId);
        }
    }

    $(function () {
        self.initSuggetion();
    });
}
