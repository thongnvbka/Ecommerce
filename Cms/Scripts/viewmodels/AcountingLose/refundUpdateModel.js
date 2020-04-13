function RefundUpdateModel() {
    var self = this;

    self.isLoading = ko.observable(false);
    self.isSubmit = ko.observable(false);

    self.id = ko.observable(0);
    self.orderId = ko.observable(null);
    self.code = ko.observable(null);
    self.orderId = ko.observable(null);
    self.linkNo = ko.observable(null);
    self.productNo = ko.observable(null);
    self.status = ko.observable(null);
    self.mode = ko.observable(0);
    self.note = ko.observable(null);
    self.createUserId = ko.observable(window.currentUser.UserId);
    self.createUserFullName = ko.observable(window.currentUser.FullName);
    self.createUserName = ko.observable(window.currentUser.UserName);
    self.createOfficeId = ko.observable(window.currentUser.OfficeId);
    self.createOfficeName = ko.observable(window.currentUser.OfficeName);
    self.createOfficeIdPath = ko.observable(window.currentUser.OfficeIdPath);
    self.updateUserId = ko.observable(null);
    self.updateUserFullName = ko.observable(null);
    self.updateUserName = ko.observable(null);
    self.updateOfficeId = ko.observable(null);
    self.updateOfficeName = ko.observable(null);
    self.updateOfficeIdPath = ko.observable(null);
    self.commentNo = ko.observable(null);
    self.amount = ko.observable(null);
    self.amountActual = ko.observable(null);
    self.totalAcount = ko.observable(null);
    self.percent = ko.observable(null);
    self.created = ko.observable(moment());
    self.updated = ko.observable(moment());

    self.items = ko.observableArray([]);

    self.amountActual.subscribe(function() {
        self.countRefund();
    });
    
    self.resetValue = function () {
        self.isLoading(false);
        self.isSubmit(false);
        self.id(0);
        self.orderId(null);
        self.code(null);
        self.orderId(null);
        self.linkNo(null);
        self.productNo(null);
        self.status(null);
        self.mode(0);
        self.note(null);
        self.createUserId(window.currentUser.UserId);
        self.createUserFullName(window.currentUser.FullName);
        self.createUserName(window.currentUser.UserName);
        self.createOfficeId(window.currentUser.OfficeId);
        self.createOfficeName(window.currentUser.OfficeName);
        self.createOfficeIdPath(window.currentUser.OfficeIdPath);
        self.updateUserId(null);
        self.updateUserFullName(null);
        self.updateUserName(null);
        self.updateOfficeId(null);
        self.updateOfficeName(null);
        self.updateOfficeIdPath(null);
        self.commentNo(null);
        self.amount(null);
        self.amountActual(null);
        self.totalAcount(null);
        self.percent(null);
        self.created(moment());
        self.updated(moment());
        self.items([]);

        self.initInputMark();
    }

    self.setValue = function(data) {
        self.id(data.id);
        self.code(data.code);
        self.orderId(data.orderId);
        self.linkNo(data.linkNo);
        self.productNo(data.productNo);
        self.status(data.status);
        self.mode(data.mode);
        self.note(data.note);
        self.createUserId(data.createUserId);
        self.createUserFullName(data.createUserFullName);
        self.createUserName(data.createUserName);
        self.createOfficeId(data.createOfficeId);
        self.createOfficeName(data.createOfficeName);
        self.createOfficeIdPath(data.createOfficeIdPath);
        self.updateUserId(data.updateUserId);
        self.updateUserFullName(data.updateUserFullName);
        self.updateUserName(data.updateUserName);
        self.updateOfficeId(data.updateOfficeId);
        self.updateOfficeName(data.updateOfficeName);
        self.updateOfficeIdPath(data.updateOfficeIdPath);
        self.commentNo(data.commentNo);
        self.amount(data.amount);
        self.amountActual(data.amountActual);
        self.totalAcount(data.totalAcount);
        self.percent(data.percent);
        self.created(moment(data.created));
        self.updated(moment(data.updated));
    }

    /**
     * Lấy dữ liệu form
     * @returns {object} 
     */
    self.getValue = function () {
        return {
            id: self.id(),
            code: self.code(),
            orderId: self.orderId(),
            linkNo: self.linkNo(),
            productNo: self.productNo(),
            status: self.status(),
            mode: self.mode(),
            note: self.note(),
            createUserId: self.createUserId(),
            createUserFullName: self.createUserFullName(),
            createUserName: self.createUserName(),
            createOfficeId: self.createOfficeId(),
            createOfficeName: self.createOfficeName(),
            createOfficeIdPath: self.createOfficeIdPath(),
            updateUserId: self.updateUserId(),
            updateUserFullName: self.updateUserFullName(),
            updateUserName: self.updateUserName(),
            updateOfficeId: self.updateOfficeId(),
            updateOfficeName: self.updateOfficeName(),
            updateOfficeIdPath: self.updateOfficeIdPath(),
            commentNo: self.commentNo(),
            amount: self.amount(),
            amountActual: self.amountActual(),
            totalAcount: self.totalAcount(),
            percent: self.percent(),
            created: self.created().format(),
            updated: self.updated().format()
        };
    }

    self.showAdd = function (orderId, mode) {
        self.resetValue();
        self.mode(mode);
        self.orderId(orderId);

        self.isLoading(true);
        $.get("/OrderRefund/GetOrderCoutingLose",
            { orderId: orderId },
            function (data) {
                self.isLoading(false);

                if (data.length === 0) {
                    toastr.warning("There is no longer any wrong counting to create refund slip!");
                    return;
                }

                _.each(data,
                    function(it) {
                        it.orderDetailCountingId = it.id;
                        it.id = 0;
                    });

                self.items(data);

                self.countRefund();

                if(self.mode() === 0)
                    self.amountActual(formatNumberic(self.amount(), "N4"));

                $("#refundUpdateModal").modal("show");
            });
    }

    self.showDetail = function(refundId, mode) {
        self.resetValue();
        self.mode(mode);

        self.isLoading(true);

        $.get("/OrderRefund/GetDetail",
            { refundId: refundId },
            function(data) {
                self.isLoading(false);
                if (data == null) {
                    toastr.warning("This slip does no exist or has been deleted.");
                    return;
                }

                self.setValue(data.refund);
                self.orderId(data.refund.orderId);
                self.items(data.refundDetails);

                self.countRefund();

                //if (self.mode() === 0)
                //    self.amountActual(formatNumberic(self.amountActual(), "N4"));

                $("#refundUpdateModal").modal("show");
            });
    }

    /**
     * Khởi tạo Input nhập decimal
     * @returns {void} 
     */
    self.initInputMark = function () {
        $('#refundUpdateModal input.decimal').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", {
                    radixPoint: Globalize.culture().numberFormat['.'], autoGroup: true,
                    groupSeparator: Globalize.culture().numberFormat[','], digits: 2,
                    digitsOptional: true, allowPlus: false, allowMinus: false
                });
            }
        });
    }

    /**
     * Tính Total money hàng và % hoàn trả
     * @returns {void} 
     */
    self.countRefund = function() {
        var amount = _.sumBy(self.items(), "totalPriceLose");
        self.amount(amount);

        if (self.mode() !== 0 || isNaN(Globalize.parseFloat(self.amountActual()))) {
            self.percent(null);
            return;
        }

        self.percent(Globalize.parseFloat(self.amountActual()) * 100 / amount);
    }

    self.remove = function (data) {
        self.items.remove(data);

        self.countRefund();
    }

    self.save = function() {
        if (isNaN(Globalize.parseFloat(self.amountActual()))) {
            var text = self.mode() === 0 ? 'Refund amount (CYN)' : 'Tran(CYN)';
            toastr.warning(text + " Not in correct format");
            return;
        }

        //if (self.mode() === 0 && Globalize.parseFloat(self.amountActual()) > self.amount()) {
        //    toastr.warning("Số tiền hoàn trả không được lớn hơn Total money hoàn trả");
        //    return;
        //}

        var data = self.getValue();

        data.items = self.items();
        data["__RequestVerificationToken"] = self.token;

        self.isSubmit(true);
        $.post("/OrderRefund/Add",
            data,
            function(rs) {
                self.isSubmit(false);

                if (rs && rs.status <= 0) {
                    toastr.warning(rs.text);
                    return;
                }

                toastr.success(rs.text);
                $("#refundUpdateModal").modal("hide");
            });
    }
}