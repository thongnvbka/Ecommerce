function PutAwayAddModel(callback, packageDetailModal, orderDetailModal) {
    var self = this;

    self.allWarehouse = ko.observableArray(window.allWarehouse);
    self.isLoading = ko.observable(false);
    self.isLoadingItems = ko.observable(false);
    self.statesPutAwayGroupById = _.groupBy(window.statesPutAway, 'id');
    self.hasUpdatePutAway = window.hasUpdatePutAway;

    self.suggetionType = ko.observable(0); // 0: package, 1: Bao hàng
    self.changeSuggetionType = function(s) {
        self.suggetionType(s);
    };
    self.items = ko.observableArray([]);

    // model
    self.id = ko.observable(0);
    self.note = ko.observable("");
    self.code = ko.observable("");
    self.status = ko.observable(1);
    self.userId = ko.observable(0);
    self.userName = ko.observable("");
    self.userFullName = ko.observable("");
    self.created = ko.observable();
    self.updated = ko.observable();
    self.packageNo = ko.observable(0);
    self.totalWeight = ko.observable(null);
    self.totalConversionWeight = ko.observable(null);
    self.totalActualWeight = ko.observable(null);
    self.warehouseId = ko.observable(0);
    self.warehouseIdPath = ko.observable("");
    self.warehouseName = ko.observable("");
    self.warehouseAddress = ko.observable("");
    self.layoutJstree = ko.observable(window.warehouseLayouts);
    self.isShowInfo = ko.observable(false);
    self.lastMessage = ko.observable("");
    self.lastCodeCheck = ko.observable("");

    self.chageIsShowInfo = function() {
        self.isShowInfo(!self.isShowInfo());
    };

    self.isScanCode = ko.observable(true);

    self.changeIsScanCode = function () {
        self.isScanCode(!self.isScanCode());
    }

    self.resetForm = function () {
        self.items.removeAll();
        self.id(0);
        self.note("");
        self.code("");
        self.lastCodeCheck("");
        self.lastMessage("");
        self.status(1);
        self.userId(0);
        self.userName("");
        self.userFullName("");
        self.created();
        self.updated();
        self.packageNo(0);
        self.totalWeight(null);
        self.totalConversionWeight(null);
        self.totalActualWeight(null);
        self.warehouseId(0);
        self.warehouseIdPath("");
        self.warehouseName("");
        self.warehouseAddress("");

        self.layoutJstree(window.warehouseLayouts);
        self.isLoadingItems(false);
        self.isLoading(false);
    };
    self.setForm = function (data) {
        self.showAddForm();

        self.id(data.id);
        self.note(data.note);
        self.code(data.code);
        self.status(data.status);
        self.userId(data.userId);
        self.userName(data.userName);
        self.userFullName(data.userFullName);
        self.created(data.created);
        self.updated(data.updated);
        self.packageNo(data.packageNo);
        self.totalWeight(data.totalWeight);
        self.totalConversionWeight(data.totalConversionWeight);
        self.totalActualWeight(data.totalConversionWeight);
        self.warehouseId(data.warehouseId);
        self.warehouseIdPath(data.warehouseIdPath);
        self.warehouseName(data.warehouseName);
        self.warehouseAddress(data.warehouseAddress);

        // Get detail của phiếu nhập kho
        self.getDetail();
    };
    self.changeLose = function(data) {
        data.isLose(!data.isLose());

        if (!data.isLose() && data.isLastChecked()) {
            data.isLastChecked(false);
        }
    };

    // Tính lại cân nặng,...
    self.refreshWallet = function () {
        self.packageNo(formatNumberic(self.items().length));

        self.totalWeight(formatNumberic(_.sumBy(self.items(), function (i) {
            return i.weight() !== "" ? Globalize.parseFloat(i.weight()) : 0;
        }), 'N2'));
        self.totalConversionWeight(formatNumberic(_.sumBy(self.items(), function (i) {
            return i.convertedWeight() !== "" ? Globalize.parseFloat(i.convertedWeight()) : 0;
        }), 'N2'));
        self.totalActualWeight(formatNumberic(_.sumBy(self.items(), function (i) {
            return i.actualWeight() !== "" ? Globalize.parseFloat(i.actualWeight()) : 0;
        }), 'N2'));
    };
    self.reOrderBy = function (packageIds) {
        if (packageIds) {
            _.each(self.items(), function (it) {
                it.isHighline = packageIds != undefined && packageIds != null
                    && packageIds.indexOf(it.packageId) >= 0 ? true : false;
            });
        }

        var list = _.orderBy(self.items(), ['orderCode', 'isHighline'], ['asc', 'desc']);

        // Đẩy lại toàn bộ kiện của Orders mới thêm lên đầu danh sách
        if (packageIds) {
            var orderIds = _.uniq(_.map(_.filter(self.items(), function (it) { return packageIds.indexOf(it.packageId) >= 0; }), "orderId"));

            _.each(orderIds,
                function(orderId) {
                    var listPackage = _.filter(list, function (it) { return it.orderId === orderId; });
                    _.remove(list, function (it) { return it.orderId === orderId; });
                    list = _.concat(listPackage, list);
                });
        }

        //var list = _.orderBy(self.items(), ['orderCode'], ['asc']);

        var firstOrderCode;
        _.each(list, function (it) {
            it.isFirst = ko.observable(firstOrderCode !== it.orderCode);
            firstOrderCode = it.orderCode;

            it.isHighline = packageIds != undefined && packageIds != null
                && packageIds.indexOf(it.packageId) >= 0 ? true : false;

            if (it.isFirst) {
                it.packageNoInWallet = _.filter(list, function (d) {
                    return d.orderCode === it.orderCode;
                }).length;
            }
        });
        self.items.removeAll();
        self.items(list);
    };
    self.getDetail = function (packageIds) {
        self.isLoadingItems(true);
        $.get("/putaway/getpackages", { id: self.id() }, function (data) {
            self.isLoadingItems(false);
            self.layoutJstree(data.layoutJstree);

            if (packageIds) {
                _.each(data.items, function (it) {
                    it.isHighline = packageIds != undefined && packageIds != null
                        && packageIds.indexOf(it.packageId) >= 0 ? true : false;
                });
            }

            var list = _.orderBy(data.items, ['orderCode', 'isHighline'], ['asc', 'desc']);

            // Đẩy lại toàn bộ kiện của Orders mới thêm lên đầu danh sách
            if (packageIds) {
                var orderIds = _.uniq(_.map(_.filter(self.items(), function (it) { return packageIds.indexOf(it.packageId) >= 0; }), "orderId"));

                _.each(orderIds,
                    function (orderId) {
                        var listPackage = _.filter(list, function (it) { return it.orderId === orderId; });
                        _.remove(list, function (it) { return it.orderId === orderId; });
                        list = _.concat(listPackage, list);
                    });
            }

            var firstOrderCode;
            _.each(list, function (it) {
                it.isFirst = ko.observable(firstOrderCode !== it.orderCode);
                firstOrderCode = it.orderCode;

                it.isHighline = packageIds != undefined && packageIds != null
                    && packageIds.indexOf(it.packageId) >= 0 ? true : false;

                if (it.isFirst) {
                    it.packageNoInWallet = _.filter(list, function (d) {
                        return d.orderCode === it.orderCode;
                    }).length;
                }

                // Ghi chu
                it.cacheNote = it.note;
                it.note = ko.observable(it.note);
                it.note.subscribe(function (newValue) {
                    if (it.cacheNote !== newValue) {
                        self.updatePackage(it);
                        it.cacheNote = newValue;
                    }
                });

                // Chiều dài
                it.cacheLength = it.length;
                it.length = ko.observable(formatNumberic(it.length, 'N2'));
                it.length.subscribe(function (newValue) {
                    if (it.cacheLength !== Globalize.parseFloat(newValue)) {
                        self.updatePackage(it);
                        it.cacheLength = Globalize.parseFloat(newValue);
                    }
                });
                
                // Chiều rộng
                it.cacheWidth = it.width;
                it.width = ko.observable(formatNumberic(it.width, 'N2'));
                it.width.subscribe(function (newValue) {
                    if (it.cacheWidth !== Globalize.parseFloat(newValue)) {
                        self.updatePackage(it);
                        it.cacheWidth = Globalize.parseFloat(newValue);
                    }
                });

                // Chiều cao
                it.cacheHeight = it.height;
                it.height = ko.observable(formatNumberic(it.height, 'N2'));
                it.height.subscribe(function (newValue) {
                    if (it.cacheHeight !== Globalize.parseFloat(newValue)) {
                        self.updatePackage(it);
                        it.cacheHeight = Globalize.parseFloat(newValue);
                    }
                });

                // Can nang thuc te
                it.cacheWeight = it.weight;
                it.weight = ko.observable(formatNumberic(it.weight, 'N2'));
                it.weight.subscribe(function (newValue) {
                    if (it.cacheWeight !== Globalize.parseFloat(newValue)) {
                        self.updatePackage(it);
                        it.cacheWeight = Globalize.parseFloat(newValue);
                    }
                });

                it.convertedWeight = ko.computed(function () {
                    var lenth = Globalize.parseFloat(it.length());
                    var width = Globalize.parseFloat(it.width());
                    var height = Globalize.parseFloat(it.height());

                    if (!isNaN(lenth) && !isNaN(width) && !isNaN(height)) {
                        return formatNumberic(lenth * width * height / 5000, 'N2');
                    }
                    return "";
                }, it);

                it.actualWeight = ko.computed(function () {
                    var weight = Globalize.parseFloat(it.weight());
                    var convertedWeight = Globalize.parseFloat(it.convertedWeight());

                    if (isNaN(weight) && isNaN(convertedWeight))
                        return "";

                    if (!isNaN(weight) && isNaN(convertedWeight))
                        return formatNumberic(weight, 'N2');

                    if (isNaN(weight) && !isNaN(convertedWeight))
                        return formatNumberic(convertedWeight, 'N2');

                    return weight > convertedWeight ? formatNumberic(weight, 'N2') : formatNumberic(convertedWeight, 'N2');
                }, it);

                it.layoutId = ko.observable(it.layoutId);
                it.layoutName = ko.observable(it.layoutName);
                it.layoutNamePath = ko.observable(it.layoutNamePath);
                it.keyword = ko.observable(null);
            });

            self.items(list);
            self.initInputMark();
            self.refreshWallet();
            self.initJsTree();
        });
    };
    self.clearKeyword = function (item) {
        item.layoutId(0);
        item.layoutName("");
        item.layoutNamePath("");
        item.keyword("");
    };
    self.getFormData = function () {

        self.id(0);
        //self.note("");
        self.code("");
        self.status(1);
        self.userId(0);
        self.userName("");
        self.userFullName("");
        self.created();
        self.updated();
        self.packageNo(0);
        self.totalWeight(null);
        self.totalConversionWeight(null);
        self.totalActualWeight(null);
        self.warehouseId(0);
        self.warehouseIdPath("");
        self.warehouseName("");
        self.warehouseAddress("");

        return {
            id: self.id(),
            note: self.note(),
            code: self.code(),
            status: self.status(),
            userId: self.userId(),
            userName: self.userName(),
            userFullName: self.userFullName(),
            created: self.created(),
            updated: self.updated(),
            packageNo: self.packageNo(),
            totalWeight: self.totalWeight(),
            totalConversionWeight: self.totalConversionWeight(),
            totalActualWeight: self.totalActualWeight(),
            warehouseId: self.warehouseId(),
            warehouseIdPath: self.warehouseIdPath(),
            warehouseName: self.warehouseName(),
            warehouseAddress: self.warehouseAddress()
        };
    };
    self.showAddForm = function () {
        self.resetForm();
        resetForm("#putAwayForm");
        self.initSuggetion();
        self.initSuggetionToCheckPackage();
        $("#putAwayAddOrEdit").modal("show");
    };
    self.showUpdateForm = function (data) {
        self.resetForm();
        self.setForm(data);
        resetForm("#putAwayForm");
        self.initSuggetion();
        self.initSuggetionToCheckPackage();
        $("#putAwayAddOrEdit").modal("show");
    };
    self.loadingItems = function (putawayId) {
        $.get("/putaway/getpackages", { id: putawayId }, function (data) {
            self.items(data.items);
            self.layoutJstree(data.layoutJstree);
        });
    };
    self.updatePackage = function (package) {
        var data = ko.mapping.toJS(package);

        data["__RequestVerificationToken"] = self.token;
        self.isLoading(true);
        $.post("/putaway/updatepackage", data, function (rs) {
            self.isLoading(false);
            if (rs.status < 0) {
                toastr.error(rs.text);
                return;
            }
            self.refreshWallet();
        });
    };
    self.removeItem = function (item) {
        if (self.id() === 0) {
            self.items.remove(item);
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
                $.post("/putaway/deletepackage", item,
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
    };
    self.token = $("#putAwayForm input[name='__RequestVerificationToken']").val();

    self.save = function () {
        if (!$("#putAwayForm").valid()) {
            toastr.error("Check the entered fields!");
            $(".input-validation-error:first").focus();
            return;
        }

        if (self.items().length === 0) {
            toastr.error("Requisition for warehousing is not available!");
            return;
        }

        var data = self.getFormData();
        data.packages = ko.mapping.toJS(self.items());
        data["__RequestVerificationToken"] = self.token;

        // Kiểm tra nhập kho quên chưa check kiểm tra
        var losePackages = _.filter(data.packages,
            function (p) { return p.customerWarehouseId == window.currentUser.OfficeId && p.isLose == false; });

        var losePackages2 = _.filter(data.packages,
            function (p) {
                return p.customerWarehouseId == window.currentUser.OfficeId &&
                    p.isLose == false &&
                    $.trim(p.note).length === 0;
            });

        var confirm2 = function () {
            swal({
                title: "Do you want to make PutAway?",
                text: "Have a lost package Not yet noted",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Performing",
                cancelButtonText: "No",
                closeOnConfirm: false,
                closeOnCancel: false
            }).then(function () {
                self.saveAction(data);
            }, function() {
                
            });
        }

        var confirm1 = function () {
            swal({
                title: "Do you want to make PutAway?",
                text: "All package being put by putAway will go to the lost line item",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Performing",
                cancelButtonText: "No ",
                closeOnConfirm: false,
                closeOnCancel: false
            }).then(function () {
                    if (losePackages2.length > 0) {
                        confirm2();
                        return;
                    }

                    self.saveAction(data);
                },
                function() {
                });
        };

        // Kiểm tra quên chưa check kiểm tra hàng
        if (losePackages.length > 0 && losePackages.length === data.packages.length) {
            confirm1();
        } else if (losePackages2.length > 0) {
            confirm2();
        } else {
            self.saveAction(data);
        } 
    };
    self.saveAction = function(data) {
        // Cập nhật
        if (self.id() > 0) {
            self.isLoading(true);
            $.post("/putaway/update", data, function (rs) {
                self.isLoading(false);

                if (rs.status < 0) {
                    toastr.error(rs.text);
                    return;
                }

                toastr.success(rs.text);
                // reset form
                self.resetForm();
                resetForm("#putAwayForm");
                $("#putAwayAddOrEdit").modal("hide");

                if (callback)
                    callback();
            });
        } else { // Thêm mới
            self.isLoading(true);
            $.post("/putaway/add", data, function (rs) {
                self.isLoading(false);
                if (rs.status < 0) {
                    toastr.error(rs.text);
                    return;
                }

                toastr.success(rs.text);
                // reset form
                self.resetForm();
                resetForm("#putAwayForm");
                $("#putAwayAddOrEdit").modal("hide");

                if (callback)
                    callback();
            });
        }
    };
    self.initInputMark = function () {
        $('#putAwayAddOrEdit input.decimal').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize.culture().numberFormat['.'], autoGroup: true, groupSeparator: Globalize.culture().numberFormat[','], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });
    };
    self.initJsTree = function () {
        $("#putAwayAddOrEdit .suggestion")
            .each(function () {
                var isPaste = false;
                $(this).on('paste', function () {
                    isPaste = true;
                });

                var idx = $(this).data("index");

                var item = self.items()[idx];

                $(this).autocomplete({
                        minLength: 0,
                        autoFocus: true,
                        //source: _.filter(self.layoutJstree(), function (l) { return l.childNo === 0 }),
                        source: function (request, response) {
                            var resource = _.filter(self.layoutJstree(), function(l) { return l.childNo === 0 });

                            if (isPaste) {
                                var it = _.find(resource, function (i) { return _.includes(i, request.term); });
                                if (item) {
                                    item.layoutId(it.id);
                                    item.layoutName(it.text);
                                    item.layoutNamePath(it.namePath);
                                    item.keyword(it.text);
                                    // ReSharper disable once InconsistentFunctionReturns
                                    isPaste = false;
                                    return;
                                }
                            } 

                            var result = _.filter(resource, function (i) { return _.includes(_.toLower(i.text), _.toLower(request.term)); });
                            isPaste = false;
                            return response(result);
                        },
                        select: function(event, ui) {
                            item.layoutId(ui.item.id);
                            item.layoutName(ui.item.text);
                            item.layoutNamePath(ui.item.namePath);
                            item.keyword(ui.item.text);
                            isPaste = false;
                            return false;
                        }
                    })
                    .autocomplete("instance")._renderItem = function(ul, item) {
                        return $("<li>")
                            .append("<div>" + item.text + "</div>")
                            .appendTo(ul);
                    };
            });

        //$("#putAwayAddOrEdit .jstree")
        //    .each(function () {
        //        var idx = $(this).data("index");
        //        var item = self.items()[idx];

        //        $(this).dropdownjstree({
        //                source: self.layoutJstree(),
        //                dropdownLabel: 'Chọn layout',
        //                dropdownLabelClick: () => {
        //                    item.layoutId = null;
        //                    item.layoutName = null;
        //                },
        //                selectedNode: item.layoutId,
        //                selectNote: (node, selected) => {
        //                    item.layoutId = selected.selected[0];
        //                }
        //            });
        //    });
    };

    // Xem Detail Kiện
    self.showDetailPackage = function (data) {
        if (packageDetailModal) {
            packageDetailModal.showModel(data);
            return;
        }
    };

    // Xem Detail Orders
    self.showOrderDetail = function (orderId) {
        if (orderDetailModal) {
            orderDetailModal.viewOrderDetail(orderId);
            return;
        }
    };
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

            return ($("#suggetion").val().length > 4 && ((inputStop - inputStart) / $("#suggetion").val().length) < 15);
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
                var url = self.suggetionType() == 0 ? '/package/suggetionforputaway' : '/wallet/suggetionforputaway';

                var codes = _.map(self.items(), "packageCode");

                var strCodes = codes.length === 0 ? "" : ';' + _.join(codes, ';') + ';';

                isPaste = self.isScanCode() || isPaste || isScannerInput() ? true : false;

                $.post(url, { term: request.term, packageCodes: strCodes, isPaste: isPaste }, function (result) {
                    if (isPaste) {
                        if (result.length > 0)
                            self.processItemSelected(result[0]);

                        isPaste = false;
                        $('#suggetion').val("");
                        resetValues();
                    } else {
                        return response(result);
                    }
                });
            },
            select: function (event, ui) {
                self.processItemSelected(ui.item);
                resetValues();
            }
        }).autocomplete("instance")._renderItem = function (ul, item) {
            var name = self.suggetionType() == 0 ? item.transportCode + ' - P' + item.code + ' - ' + 'ODR' + item.orderCode + ' - destination warehouse: ' + item.customerWarehouseName
                : item.code + ' - destination warehouse: ' + item.targetWarehouseName;

            return $("<li>").addClass('media media-line').append('<div>' +
                '<h4 class="media-heading bold size-16 pr-mgb-0">' + name +
                '</h4></div>').appendTo(ul).addClass('automember media-list');
        };
    };
    self.initSuggetionToCheckPackage = function () {
        var isPaste = false;
        $('#suggetionToCheckPackage').on('paste', function () {
            isPaste = true;
        });

        var inputStart, inputStop, firstKey, lastKey, timing, userFinishedEntering;
        var minChars = 3;

        // handle a key value being entered by either keyboard or scanner
        $("#suggetionToCheckPackage").keypress(function (e) {
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
                if ($("#suggetionToCheckPackage").val().length >= minChars) {
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
                    $("body").on("blur", "#suggetionToCheckPackage", inputBlur);
                }

                // start the timer again
                timing = setTimeout(inputTimeoutHandler, 500);
            }
        });

        // Assume that a loss of focus means the value has finished being entered
        function inputBlur() {
            clearTimeout(timing);
            if ($("#suggetionToCheckPackage").val().length >= minChars) {
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
            if ($("#suggetionToCheckPackage").val().length <= 2)
                return false;

            return (((inputStop - inputStart) / $("#suggetionToCheckPackage").val().length) < 15);
        }

        // Determine if the user is just typing slowly
        function isUserFinishedEntering() {
            return !isScannerInput() && userFinishedEntering;
        }

        function inputTimeoutHandler() {
            // stop listening for a timer event
            clearTimeout(timing);
            // if the value is being entered manually and hasn't finished being entered
            if (!isUserFinishedEntering() || $("#suggetionToCheckPackage").val().length < 3) {
                // keep waiting for input
                return;
            }
        }

        // here we decide what to do now that we know a value has been completely entered
        function inputComplete() {
            // stop listening for the input to lose focus
            $("body").off("blur", "#suggetionToCheckPackage", inputBlur);
        }

        // Đăng ký suggetion đơn vị
        $('#suggetionToCheckPackage').autocomplete({
            delay: 100,
            autoFocus: true,
            source: function (request, response) {
                // ReSharper disable once CoercedEqualsUsing
                var url = '/package/suggetionforputaway';

                isPaste = isPaste || isScannerInput() ? true : false;

                $.post(url, { term: request.term, packageCodes: "", isPaste: isPaste }, function (result) {
                    if (isPaste) {
                        if (result.length > 0)
                            self.checkPackage(result[0]);

                        isPaste = false;
                        $('#suggetionToCheckPackage').val("");
                        resetValues();
                    } else {
                        return response(result);
                    }
                });
            },
            select: function (event, ui) {
                self.checkPackage(ui.item);
                resetValues();
            }
        }).autocomplete("instance")._renderItem = function (ul, item) {
            var name = item.transportCode + ' - P' + item.code + ' - ' + 'ODR' + item.orderCode + ' - destination warehouse: ' + item.customerWarehouseName;

            return $("<li>").addClass('media media-line').append('<div>' +
                '<h4 class="media-heading bold size-16 pr-mgb-0">' + name +
                '</h4></div>').appendTo(ul).addClass('automember media-list');
        };
    };
    self.checkPackage = function (uiItem) {
        // Kiểm tra tồn tại trong ds chưa
        var item = _.find(self.items(), function(it) { return it.packageCode === uiItem.code });

        if (item) {
            var lastItem = _.find(self.items(), function (it) { return it.isLastChecked() === true });

            if (lastItem)
                lastItem.isLastChecked(false);

            item.isLose(true);
            item.isLastChecked(true);

            self.lastCodeCheck("P" + uiItem.code);

            //// Sắp xếp lại
            //var cache = item;
            //self.items.remove(item);
            //self.items.unshift(cache);

            //var firstOrderCode;
            //_.each(self.items(), function (it) {
            //    it.isFirst(firstOrderCode !== it.orderCode);
            //    firstOrderCode = it.orderCode;

            //    if (it.isFirst()) {
            //        it.packageNoInWallet = _.filter(self.items(), function (d) {
            //            return d.orderCode === it.orderCode;
            //        }).length;
            //    }
            //});
        }
    };
    self.processItemSelected = function(uiItem) {
        var items = [];

        // Nhập mã bao
        if (self.suggetionType() !== 0) {
            $.ajax({
                url: "/package/SuggetionForPutAwayByWalletId",
                data: { walletId: uiItem.id },
                type: "get",
                async: false,
                success: function (res) {
                    items = res;
                }
            });
            self.lastMessage(uiItem.code);
        } else { // package
            self.lastMessage("P" + uiItem.code + ", MVĐ: " + uiItem.transportCode );

            items = [uiItem];
        }
        var msg;
        var packageIdsAdded = [];
        _.each(items,
            function (item) {
                item.isFirst = ko.observable(false);
                item.orderPackageNo = item.packageNo;
                item.putawayId = 0;
                item.putawayCode = "";
                item.packageId = item.id;
                item.packageCode = item.code;
                item.id = 0;
                item.note = ko.observable(item.note);
                item.weight = ko.observable(item.weight ? formatNumberic(item.weight, 'N2') : null);
                item.length = ko.observable(item.length ? formatNumberic(item.length, 'N2') : null);
                item.width = ko.observable(item.width ? formatNumberic(item.width, 'N2') : null);
                item.height = ko.observable(item.height ? formatNumberic(item.height, 'N2') : null);
                item.status = 1;

                item.isHighline = false;
                item.isLose = ko.observable(false);
                item.isLastChecked = ko.observable(false);
                item.convertedWeight = ko.computed(function () {
                    var lenth = Globalize.parseFloat(item.length());
                    var width = Globalize.parseFloat(item.width());
                    var height = Globalize.parseFloat(item.height());

                    if (!isNaN(lenth) && !isNaN(width) && !isNaN(height)) {
                        return formatNumberic(lenth * width * height / 5000, 'N2');
                    }
                    return "";
                },
                    item);

                item.actualWeight = ko.computed(function () {
                    var weight = Globalize.parseFloat(item.weight());
                    var convertedWeight = Globalize.parseFloat(item.convertedWeight());

                    if (isNaN(weight) && isNaN(convertedWeight))
                        return "";

                    if (!isNaN(weight) && isNaN(convertedWeight))
                        return formatNumberic(weight, 'N2');

                    if (isNaN(weight) && !isNaN(convertedWeight))
                        return formatNumberic(convertedWeight, 'N2');

                    return weight > convertedWeight
                        ? formatNumberic(weight, 'N2')
                        : formatNumberic(convertedWeight, 'N2');
                },
                    item);

                item.layoutId = ko.observable(0);
                item.layoutName = ko.observable("");
                item.layoutNamePath = ko.observable("");
                item.keyword = ko.observable("");

                // Kiểm tra tồn tại trong ds chưa
                if (!_.find(self.items(), function(it) { return it.packageCode === item.packageCode })) {
                    var isError = false;
                    if (item.mode != null && item.sameCodeStatus === 0) {
                        isError = true;
                        msg = 'Bill of Lading "' + item.transportCode + '" being unresolved infection';
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

                    self.items.push(item);
                    packageIdsAdded.push(item.packageId);

                    msg = 'Successfully added bill of lading "' + item.transportCode + '"';
                    if (self.isScanCode()) {
                        swal({
                            title: 'Successfully!',
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
                    msg = 'Included in the bill of lading list "' + item.transportCode + '"';
                    if (self.isScanCode()) {
                        swal({
                            title: 'Overlap code!',
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
            });

        if (self.id() > 0) {
            var data = {};
            data["__RequestVerificationToken"] = self.token;
            data.putawayId = self.id();
            data.putawayCode = self.code();
            data.packages = ko.mapping.toJS(items);

            self.isLoading(true);
            $.post("/putaway/addpackage", data, function (rs) {
                self.isLoading(false);
                if (rs.status < 0) {
                    toastr.error(rs.text);
                    return;
                }
                self.getDetail(packageIdsAdded);
            });
            return;
        }

        self.reOrderBy(packageIdsAdded);
        self.initInputMark();
        self.initJsTree();
    };
    //$(function () {
    //    self.initSuggetion();
    //    self.initSuggetionToCheckPackage();
    //});
}


function PutAwayModel(packageDetailModal, orderDetailModal, walletDetailModal) {
    var self = this;
    self.isLoading = ko.observable(false);

    // Search
    self.warehouses = ko.observableArray(window["warehouses"] ? window.warehouses : []);
    self.states = ko.observableArray(window["states"] ? window.states : []);


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

        $("#sumaryPagerPutAway").html(self.totalRecord() === 0 ? "There is no order/package" : "Show " + ((self.currentPage() - 1) * self.recordPerPage() + 1) + " to " + (self.currentPage() * self.recordPerPage() < self.totalRecord() ?
            self.currentPage() * self.recordPerPage() : self.totalRecord()) + "of " + self.totalRecord() + " package");

        $("#pagerPutAway").pager({ pagenumber: self.currentPage(), pagecount: self.totalPage(), totalrecords: self.totalRecord(), buttonClickCallback: self.pageClick });
    };

    self.pageClick = function (currentPage) {
        $("#pagerPutAway").pager({ pagenumber: currentPage, pagecount: self.totalPage(), buttonClickCallback: self.search });

        self.search(currentPage);
    };

    self.search = function (currentPage) {
        self.currentPage(currentPage);
        self.isLoading(true);
        $.get("/putaway/search",
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

                var statesGroup = _.groupBy(window.statesPutAway, "id");
                var first = null;
                _.each(data.items,
                    function (item) {
                        item.isFirst = first !== item.id;
                        first = item.id;
                        item.packageForcastDateText = moment(item.packageForcastDate).format("DD/MM/YYYY");
                        item.statusText = statesGroup[item.status + ''][0].name;
                        item.createdText = moment(item.forcastDate).format("DD/MM/YYYY HH:mm:ss");
                    });

                self.items(data.items);
                self.totalRecord(data.totalRecord);
                self.renderPager();
            });
    };
    self.callback = function () {
        self.search(1);
    };
    self.addBill = function () {
        if (self.addForm() == null) {
            self.addForm(new PutAwayAddModel(self.callback, packageDetailModal, orderDetailModal, walletDetailModal));
            ko.applyBindings(self.addForm(), $("#putAwayAddOrEdit")[0]);
        }

        self.addForm().showAddForm();
    };
    self.update = function (data) {
        if (self.addForm() == null) {
            self.addForm(new PutAwayAddModel(self.callback, packageDetailModal, orderDetailModal, walletDetailModal));
            ko.applyBindings(self.addForm(), $("#putAwayAddOrEdit")[0]);
        }

        self.addForm().setForm(data);
    };
    self.showDetail = function (data) {
        if (packageDetailModal) {
            packageDetailModal.showModel(data.packageId);
            return;
        }
    };
    $(function () {
        $('#PutAway-date-btn').daterangepicker({
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
                $('#PutAway-date-btn span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
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

// Bind WalletDetail
//allWarehouse, walletStates, orderPackageStates, packageDetailModal, orderDetailModal
var walletDetailModelView = new WalletDetailModel(window.allWarehouse, window.walletStates, window.orderPackageStates, packageDetailModelView, orderDetailViewModel);
ko.applyBindings(walletDetailModelView, $("#walletDetailModal")[0]);

//var modelView = new PutAwayModel(packageDetailModelView, orderDetailViewModel, walletDetailModelView);
var modelView = new PutAwayModel(packageDetailModelView, orderDetailViewModel, walletDetailModelView);
ko.applyBindings(modelView, $("#putAway")[0]);