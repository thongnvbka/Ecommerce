function DeliveryAddModel() {
    var self = this;

    self.isLoading = ko.observable(false);
    self.isSubmiting = ko.observable(false);
    self.deliveryStatusGroup = _.groupBy(window["deliveryStates"] ? window.deliveryStates : [], 'id');
    self.callback = null;

    self.id = ko.observable(0);
    self.code = ko.observable("");
    self.status = ko.observable(null);
    self.statusClass = ko.observable(null);
    self.orderNo = ko.observable(null);
    self.packageNo = ko.observable(null);
    self.warehouseId = ko.observable(null);
    self.warehouseIdPath = ko.observable(null);
    self.warehouseName = ko.observable(null);
    self.warehouseAddress = ko.observable(null);
    self.createdUserId = ko.observable(null);
    self.createdUserFullName = ko.observable(null);
    self.createdUserUserName = ko.observable(null);
    self.createdUserTitleId = ko.observable(null);
    self.createdUserTitleName = ko.observable(null);
    self.createdOfficeId = ko.observable(null);
    self.createdOfficeName = ko.observable(null);
    self.createdOfficeIdPath = ko.observable(null);
    self.createdTime = ko.observable(null);
    self.expertiseUserId = ko.observable(null);
    self.expertiseUserFullName = ko.observable(null);
    self.expertiseUserUserName = ko.observable(null);
    self.expertiseUserTitleId = ko.observable(null);
    self.expertiseUserTitleName = ko.observable(null);
    self.expertiseOfficeId = ko.observable(null);
    self.expertiseOfficeName = ko.observable(null);
    self.expertiseOfficeIdPath = ko.observable(null);
    self.expertiseTime = ko.observable(null);
    self.shipperUserId = ko.observable(null);
    self.shipperFullName = ko.observable(null);
    self.shipperUserUserName = ko.observable(null);
    self.shipperUserTitleId = ko.observable(null);
    self.shipperUserTitleName = ko.observable(null);
    self.shipperOfficeId = ko.observable(null);
    self.shipperOfficeName = ko.observable(null);
    self.shipperOfficeIdPath = ko.observable(null);
    self.shipperTime = ko.observable(null);
    self.approvelUserId = ko.observable(null);
    self.approvelFullName = ko.observable(null);
    self.approvelUserUserName = ko.observable(null);
    self.approvelUserTitleId = ko.observable(null);
    self.approvelUserTitleName = ko.observable(null);
    self.approvelOfficeId = ko.observable(null);
    self.approvelOfficeName = ko.observable(null);
    self.approvelOfficeIdPath = ko.observable(null);
    self.approvelTime = ko.observable(null);
    self.accountantUserId = ko.observable(null);
    self.accountantFullName = ko.observable(null);
    self.accountantUserUserName = ko.observable(null);
    self.accountantUserTitleId = ko.observable(null);
    self.accountantUserTitleName = ko.observable(null);
    self.accountantOfficeId = ko.observable(null);
    self.accountantOfficeName = ko.observable(null);
    self.accountantOfficeIdPath = ko.observable(null);
    self.accountantTime = ko.observable(null);
    self.note = ko.observable(null);
    self.carNumber = ko.observable(null);
    self.carDescription = ko.observable(null);
    self.isLast = ko.observable(null);
    self.customerId = ko.observable(null);
    self.customerCode = ko.observable(null);
    self.customerFullName = ko.observable(null);
    self.customerEmail = ko.observable(null);
    self.customerPhone = ko.observable(null);
    self.customerAddress = ko.observable(null);
    self.customerVipId = ko.observable(null);
    self.customerVipName = ko.observable(null);
    self.weight = ko.observable(null);
    self.weightConverted = ko.observable(null);
    self.weightActual = ko.observable(null);
    self.priceWeight = ko.observable(null);
    self.pricePacking = ko.observable(null);
    self.priceOrder = ko.observable(null);
    self.priceOther = ko.observable(null);
    self.priceStored = ko.observable(null);
    self.priceShip = ko.observable(null);
    self.total = ko.observable(null);
    self.debit = ko.observable(null);
    self.debitPre = ko.observable(null);
    self.pricePayed = ko.observable(null);
    self.receivable = ko.observable(null);
    self.blanceBefo = ko.observable(null);
    self.blanceAfter = ko.observable(null);
    self.debitAfter = ko.observable(null);
    self.rechargeMoney = ko.observable(null);
    self.modePrint = ko.observable(1);

    self.suggettionFullName = ko.computed(function () {
        if (self.shipperFullName() == null || self.shipperUserUserName() == null)
            return "";

        return this.shipperFullName() + " (" + this.shipperUserUserName() + ")";
    }, self);

    self.packages = ko.observableArray([]);

    self.resetForm = function () {
        self.id(0);
        self.code("");
        self.status(null);
        self.statusClass(null);
        self.orderNo(null);
        self.packageNo(null);
        self.warehouseId(null);
        self.warehouseIdPath(null);
        self.warehouseName(null);
        self.warehouseAddress(null);
        self.createdUserId(null);
        self.createdUserFullName(null);
        self.createdUserUserName(null);
        self.createdUserTitleId(null);
        self.createdUserTitleName(null);
        self.createdOfficeId(null);
        self.createdOfficeName(null);
        self.createdOfficeIdPath(null);
        self.createdTime(null);
        self.expertiseUserId(null);
        self.expertiseUserFullName(null);
        self.expertiseUserUserName(null);
        self.expertiseUserTitleId(null);
        self.expertiseUserTitleName(null);
        self.expertiseOfficeId(null);
        self.expertiseOfficeName(null);
        self.expertiseOfficeIdPath(null);
        self.expertiseTime(null);
        self.shipperUserId(null);
        self.shipperFullName(null);
        self.shipperUserUserName(null);
        self.shipperUserTitleId(null);
        self.shipperUserTitleName(null);
        self.shipperOfficeId(null);
        self.shipperOfficeName(null);
        self.shipperOfficeIdPath(null);
        self.shipperTime(null);
        self.approvelUserId(null);
        self.approvelFullName(null);
        self.approvelUserUserName(null);
        self.approvelUserTitleId(null);
        self.approvelUserTitleName(null);
        self.approvelOfficeId(null);
        self.approvelOfficeName(null);
        self.approvelOfficeIdPath(null);
        self.approvelTime(null);
        self.accountantUserId(null);
        self.accountantFullName(null);
        self.accountantUserUserName(null);
        self.accountantUserTitleId(null);
        self.accountantUserTitleName(null);
        self.accountantOfficeId(null);
        self.accountantOfficeName(null);
        self.accountantOfficeIdPath(null);
        self.accountantTime(null);
        self.note(null);
        self.carNumber(null);
        self.carDescription(null);
        self.isLast(null);
        self.customerId(null);
        self.customerCode(null);
        self.customerFullName(null);
        self.customerEmail(null);
        self.customerPhone(null);
        self.customerAddress(null);
        self.customerVipId(null);
        self.customerVipName(null);
        self.weight(null);
        self.weightConverted(null);
        self.weightActual(null);
        self.priceWeight(null);
        self.pricePacking(null);
        self.priceOrder(null);
        self.priceOther(null);
        self.priceStored(null);
        self.priceShip(null);
        self.total(null);
        self.debit(null);
        self.debitPre(null);
        self.pricePayed(null);
        self.receivable(null);
        self.blanceBefo(null);
        self.blanceAfter(null);
        self.debitAfter(null);

        self.packages([]);
    }

    self.setForm = function (data) {
        self.id(data.id);
        self.code(data.code);


        self.status(data.status);
        // 0: Mới khởi tạo/Chờ duyệt, 1: Approved, 2: Đã xuất giao, 3: Giao thành công, 4: Complete note, 5: Hủy/Hoàn phiếu)
        self.statusClass(data.status === 0 ? 'label label-warning' : data.status === 1
            ? 'label label-info' : data.status === 2 ? 'label label-warning' : data.status === 3 ? 'label label-success' : data.status === 4 ? 'label label-success' : 'label label-danger'); 

        self.orderNo(data.orderNo);
        self.packageNo(data.packageNo);
        self.warehouseId(data.warehouseId);
        self.warehouseIdPath(data.warehouseIdPath);
        self.warehouseName(data.warehouseName);
        self.warehouseAddress(data.warehouseAddress);
        self.createdUserId(data.createdUserId);
        self.createdUserFullName(data.createdUserFullName);
        self.createdUserUserName(data.createdUserUserName);
        self.createdUserTitleId(data.createdUserTitleId);
        self.createdUserTitleName(data.createdUserTitleName);
        self.createdOfficeId(data.createdOfficeId);
        self.createdOfficeName(data.createdOfficeName);
        self.createdOfficeIdPath(data.createdOfficeIdPath);
        self.createdTime(data.createdTime);
        self.expertiseUserId(data.expertiseUserId);
        self.expertiseUserFullName(data.expertiseUserFullName);
        self.expertiseUserUserName(data.expertiseUserUserName);
        self.expertiseUserTitleId(data.expertiseUserTitleId);
        self.expertiseUserTitleName(data.expertiseUserTitleName);
        self.expertiseOfficeId(data.expertiseOfficeId);
        self.expertiseOfficeName(data.expertiseOfficeName);
        self.expertiseOfficeIdPath(data.expertiseOfficeIdPath);
        self.expertiseTime(data.expertiseTime);
        self.shipperUserId(data.shipperUserId);
        self.shipperFullName(data.shipperFullName);
        self.shipperUserUserName(data.shipperUserUserName);
        self.shipperUserTitleId(data.shipperUserTitleId);
        self.shipperUserTitleName(data.shipperUserTitleName);
        self.shipperOfficeId(data.shipperOfficeId);
        self.shipperOfficeName(data.shipperOfficeName);
        self.shipperOfficeIdPath(data.shipperOfficeIdPath);
        self.shipperTime(data.shipperTime);
        self.approvelUserId(data.approvelUserId);
        self.approvelFullName(data.approvelFullName);
        self.approvelUserUserName(data.approvelUserUserName);
        self.approvelUserTitleId(data.approvelUserTitleId);
        self.approvelUserTitleName(data.approvelUserTitleName);
        self.approvelOfficeId(data.approvelOfficeId);
        self.approvelOfficeName(data.approvelOfficeName);
        self.approvelOfficeIdPath(data.approvelOfficeIdPath);
        self.approvelTime(data.approvelTime);
        self.accountantUserId(data.accountantUserId);
        self.accountantFullName(data.accountantFullName);
        self.accountantUserUserName(data.accountantUserUserName);
        self.accountantUserTitleId(data.accountantUserTitleId);
        self.accountantUserTitleName(data.accountantUserTitleName);
        self.accountantOfficeId(data.accountantOfficeId);
        self.accountantOfficeName(data.accountantOfficeName);
        self.accountantOfficeIdPath(data.accountantOfficeIdPath);
        self.accountantTime(data.accountantTime);
        self.note(data.note);
        self.carNumber(data.carNumber);
        self.carDescription(data.carDescription);
        self.isLast(data.isLast);
        self.customerId(data.customerId);
        self.customerCode(data.customerCode);
        self.customerFullName(data.customerFullName);
        self.customerEmail(data.customerEmail);
        self.customerPhone(data.customerPhone);
        self.customerAddress(data.customerAddress);
        self.customerVipId(data.customerVipId);
        self.customerVipName(data.customerVipName);
        self.weight(data.weight);
        self.weightConverted(data.weightConverted);
        self.weightActual(data.weightActual);
        self.priceWeight(data.priceWeight);
        self.pricePacking(data.pricePacking);
        self.priceOrder(data.priceOrder);
        self.priceOther(data.priceOther);
        self.priceStored(data.priceStored);
        self.priceShip(data.priceShip);
        self.total(data.total);
        self.debit(data.debit);
        self.debitPre(data.debitPre);
        self.pricePayed(data.pricePayed);
        self.receivable(data.receivable);
        self.blanceBefo(data.blanceBefo);
        self.blanceAfter(data.blanceAfter);
        self.debitAfter(data.debitAfter);
    }

    self.getDetail = function (id) {
        self.isLoading(true);
        $.get("/delivery/getdetail", { id: id }, function (data) {
            self.isLoading(false);

            self.setForm(data.delivery);

            var firstOrderId = null;
            _.each(data.packages,
                function (p) {
                    p.isFirst = firstOrderId !== p.orderId;
                    firstOrderId = p.orderId;
                    p.totalPackageNo = p.isFirst ? _.filter(data.packages,
                        function (p1) { return p1.orderId === p.orderId; }).length : 0;
                });

            self.packages(data.packages);
        });
    }

    self.showDetail = function (id) {
        self.resetForm();
        self.getDetail(id);
        $("#DeliveryUpdateModal").modal("show");
    }

    self.approvel = function () {
        swal({
            title: 'Do you agree to approve this note??',
            text: 'System automatically deduct due amount of dispatch note from customer account!',
            type: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Approve',
            cancelButtonText: 'Do not apporve!'
        }).then(function () {
            var data = {
                id: self.id()
            }

            data["__RequestVerificationToken"] = self.token;

            self.isSubmiting(true);
            $.post("/Delivery/Approvel", data,
                function (rs) {
                    self.isSubmiting(false);
                    if (rs.status <= 0) {
                        toastr.warning(rs.text);
                        return;
                    }

                    self.getDetail(self.id());
                    toastr.success(rs.text);
                    if (self.callback)
                        self.callback();
                });
        }, function(){});
    }

    self.rechargeForComplete = function () {
        self.rechargeMoney(null);
        $("#moneyRechargeModal").modal("show");
        self.initInputMark();
    }

    self.submitMoneyRecharge = function() {
        if ($.trim(self.rechargeMoney()).length === 0) {
            toastr.warning("The 'add money' field may not be blank");
            return;
        };

        var data = {
            id: self.id(),
            money: self.rechargeMoney()
        }

        data["__RequestVerificationToken"] = self.token;

        self.isSubmiting(true);
        $.post("/Delivery/RechargeForComplete", data,
            function (rs) {
                self.isSubmiting(false);
                if (rs.status <= 0) {
                    toastr.warning(rs.text);
                    return;
                }

                self.getDetail(self.id());
                toastr.success(rs.text);
                if (self.callback)
                    self.callback();

                $("#moneyRechargeModal").modal("hide");
            });
    }

    self.initInputMark = function () {
        $('#moneyRechargeModal input.decimal').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize.culture().numberFormat['.'], autoGroup: true, groupSeparator: Globalize.culture().numberFormat[','], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });
    }

    self.completed = function () {
        swal({
            title: 'Do you agree to complete this note?',
            type: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Complete note',
            cancelButtonText: 'No!'
        }).then(function () {
            var data = {
                id: self.id()
            }

            data["__RequestVerificationToken"] = self.token;

            self.isSubmiting(true);
            $.post("/Delivery/ForComplete", data,
                function (rs) {
                    self.isSubmiting(false);
                    if (rs.status <= 0) {
                        toastr.warning(rs.text);
                        return;
                    }

                    self.getDetail(self.id());
                    toastr.success(rs.text);
                    if (self.callback)
                        self.callback();
                });
        }, function () { });
    }

    self.deductForComplete = function () {
        swal({
            title: 'Do you agree to subtract customer account to complete the note?',
            text: 'he system automatically deducts the due amount of the note from the customer account and completes the note',
            type: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Deduct money & Complete note',
            cancelButtonText: 'No'
        }).then(function () {
            var data = {
                id: self.id()
            }

            data["__RequestVerificationToken"] = self.token;

            self.isSubmiting(true);
            $.post("/Delivery/DeductForComplete", data,
                function (rs) {
                    self.isSubmiting(false);
                    if (rs.status <= 0) {
                        toastr.warning(rs.text);
                        return;
                    }

                    self.getDetail(self.id());
                    toastr.success(rs.text);
                    if (self.callback)
                        self.callback();
                });
        }, function(){});
    }

    self.addShipper = function (user) {
        if (user) {
            self.shipperUserId(user.id);
            self.shipperFullName(user.fullName);
            self.shipperUserUserName(user.userName);
            self.shipperUserTitleId(user.titleId);
            self.shipperUserTitleName(user.titleName);
            self.shipperOfficeId(user.officeId);
            self.shipperOfficeName(user.officeName);
            self.shipperOfficeIdPath(user.officeIdPath);

            self.assignToShipper();
            return;
        }

        self.shipperUserId(null);
        self.shipperFullName(null);
        self.shipperUserUserName(null);
        self.shipperUserTitleId(null);
        self.shipperUserTitleName(null);
        self.shipperOfficeId(null);
        self.shipperOfficeName(null);
        self.shipperOfficeIdPath(null);

        self.assignToShipper();
    }

    self.assignToShipper = function () {
        var data = {
            deliveryId: self.id()
        };

        if (self.shipperUserId() !== null) {
            data = {
                deliveryId: self.id(),
                userId: self.shipperUserId(),
                titleId: self.shipperUserTitleId(),
                officeId: self.shipperOfficeId(),
                userName: self.shipperUserUserName(),
                fullName: self.shipperFullName()
            };
        }

        data["__RequestVerificationToken"] = self.token;

        $.post("/delivery/assignToShipper",
            data,
            function(rs) {
                if (rs.status <= 0) {
                    toastr.warning(rs.text);
                    return;
                }

                self.getDetail(self.id());
                toastr.success(rs.text);
                if (self.callback)
                    self.callback();
            });
    }

    self.print = function (mode) {
        self.modePrint(mode);
        $("#DeliveryPrintModal").modal("show");
    }

    self.token = $("#DeliveryUpdateModal input[name='__RequestVerificationToken']").val();

    $(function() {
        $('#DeliveryPrintModal').on('shown',
            function() {
                $("body").addClass("printModal");
                $("#DeliveryPrintModal").addClass('modalPrint');
                window.print();
                $("body").removeClass("printModal");
                $("#DeliveryPrintModal").removeClass('modalPrint');
                $("#DeliveryPrintModal").modal("hide");
            });
    });
}