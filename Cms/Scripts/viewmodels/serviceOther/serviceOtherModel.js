function ServiceOtherModel() {
    var self = this;
    self.isLoading = ko.observable(false);

    // Search
    self.warehouseIdPath = ko.observable("");
    self.fromDate = ko.observable(null);
    self.toDate = ko.observable(null);
    self.keyword = ko.observable("");
    self.orderType = ko.observable(null);
    self.mode = ko.observable(null);
    // list data
    self.items = ko.observableArray();

    // pagging
    self.currentPage = ko.observable(1);
    self.recordPerPage = ko.observable(20);
    self.totalPage = ko.observable(0);
    self.totalRecord = ko.observable(0);
    //self.addForm = ko.observable(null);

    // Form
    self.idOther = ko.observable(null);
    self.noteOther = ko.observable("");
    self.valueOther = ko.observable(null);
    self.isUpdating = ko.observable(false);

    self.orderId = ko.observable(null);

    self.token = $("#serviceOther input[name='__RequestVerificationToken']").val();

    self.showNoteUpdateModal = function(data) {
        self.noteOther(data.note);
        self.idOther(data.id);

        $("#noteOtherModal").modal("show");
    }

    self.showValueUpdateModal = function (data) {
        self.valueOther(formatNumberic(data.value, "N2"));
        self.idOther(data.id);

        self.initInputMark();

        $("#valueOtherModal").modal("show");
    }
    
    self.saveValue = function () {
        var data = {
            id: self.idOther(),
            value: self.valueOther()
        };

        data["__RequestVerificationToken"] = self.token;

        self.isUpdating(true);
        $.post("/ServiceOther/UpdateValue",
            data,
            function(rs) {
                self.isUpdating(false);

                if (rs.status < 0) {
                    toastr.error(rs.text);
                    return;
                }

                toastr.success(rs.text);
                $("#valueOtherModal").modal("hide");
                self.valueOther(null);
                self.idOther(null);
                self.search(1);
            });
    }

    self.saveNote = function () {
        var data = {
            id: self.idOther(),
            note: self.noteOther()
        };

        data["__RequestVerificationToken"] = self.token;

        self.isUpdating(true);
        $.post("/ServiceOther/UpdateNote",
            data,
            function (rs) {
                self.isUpdating(false);

                if (rs.status < 0) {
                    toastr.error(rs.text);
                    return;
                }

                toastr.success(rs.text);
                $("#noteOtherModal").modal("hide");
                self.noteOther(null);
                self.idOther(null);
                self.search(1);
            });
    }

    //Thêm ServiceOther
    self.showAddServiceOther = function (data) {
        $("#addServiceOtherModal").modal("show");
    }
    //self.orderCode = ko.observable("");
    self.value = ko.observable("");
    self.note = ko.observable("");
    self.mode = ko.observable(0);
    self.saveServiceOther = function () {
        if (self.orderId() == null) {
            toastr.error("The order is required to enter");
            return;
        }
        var model = {
            Value: self.value(),
            Note: self.note(),
            Mode: self.mode(),
            orderId: self.orderId()
        };
        model["__RequestVerificationToken"] = self.token;
        self.isLoading(true);
        $.post("/ServiceOther/Add", model, function (result) {
            self.isLoading(false);
            if (result.status < 0) {
                toastr.error(result.text);
                return;
            }
            else
            {
                toastr.success("Additional fees incurred successfully");
                self.orderId(null);
                self.value("");
                self.note("");
                self.mode(0);
                self.search(1);
            }
            
        });
    }

    self.chooseOrder = function (order) {
        if (order) {
            self.orderId(order.id);
            return;
        }
        self.orderId(null);
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

    var orderTypeFirst = true;
    self.orderType.subscribe(function () {
        if (orderTypeFirst) {
            orderTypeFirst = false;
            return;
        }
        self.search(1);
    });

    var modeFirst = true;
    self.mode.subscribe(function () {
        if (modeFirst) {
            modeFirst = false;
            return;
        }
        self.search(1);
    });

    self.renderPager = function () {
        self.totalPage(Math.ceil(self.totalRecord() / self.recordPerPage()));

        $("#sumaryPagerServiceOther").html(self.totalRecord() === 0 ? "There are no order" : "Show " + ((self.currentPage() - 1) * self.recordPerPage() + 1) + " to " + (self.currentPage() * self.recordPerPage() < self.totalRecord() ?
            self.currentPage() * self.recordPerPage() : self.totalRecord()) + " of " + self.totalRecord() + " order");

        $("#pagerServiceOther").pager({ pagenumber: self.currentPage(), pagecount: self.totalPage(), totalrecords: self.totalRecord(), buttonClickCallback: self.pageClick });
    };

    self.pageClick = function (currentPage) {
        $("#pagerServiceOther").pager({ pagenumber: currentPage, pagecount: self.totalPage(), buttonClickCallback: self.search });

        self.search(currentPage);
    };

    self.search = function (currentPage) {
        self.currentPage(currentPage);
        self.isLoading(true);
        $.get("/ServiceOther/search",
            {
                mode: self.mode(),
                warehouseIdPath: self.warehouseIdPath(),
                orderType: self.orderType(),
                fromDate: self.fromDate(),
                toDate: self.toDate(),
                keyword: self.keyword(),
                currentPage: self.currentPage(),
                recordPerPage: self.recordPerPage()
            }, function (data) {
                self.isLoading(false);

                self.items(data.items);
                self.totalRecord(data.totalRecord);
                self.renderPager();
                self.initInputMark();
            });
    }

    self.delete = function(data) {
        swal({
            title: 'Delete this incurred charge delete?',
            text: "After deletion will not be able to restore",
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            cancelButtonText: 'Do not delete',
            confirmButtonText: 'Delete'
        }).then(function() {
            $.post("/ServiceOther/Delete", { id: data.id, '__RequestVerificationToken': self.token }, function(rs) {
               if (rs.status < 0) {
                   toastr.warning(rs.text);
                   return;
                }

                toastr.success(rs.text);
            });
        }, function(){});
    }

    self.token = $("#serviceOther input[name='__RequestVerificationToken']").val();

    self.initInputMark = function () {
        $('#valueOtherModal input.decimal').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize.culture().numberFormat['.'], autoGroup: true, groupSeparator: Globalize.culture().numberFormat[','], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });
    }

    $(function () {
        $('#ServiceOther-date-btn').daterangepicker({
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
                $('#ServiceOther-date-btn span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
            });

        self.search(1);
    });
}

// Bind PackageDetail
var packageDetailModelView = new PackageDetail();
ko.applyBindings(packageDetailModelView, $("#packageDetail")[0]);

// Bind OrderDetail
var orderDetailViewModel = new OrderDetailViewModel();
ko.applyBindings(orderDetailViewModel, $("#orderDetailModal")[0]);

var depositDetailViewModel = new DepositDetailViewModel();
ko.applyBindings(depositDetailViewModel, $("#orderDepositDetail")[0]);

var orderCommerceDetailViewModel = new OrderCommerceDetailViewModel();
ko.applyBindings(orderCommerceDetailViewModel, $("#orderCommerceDetailModal")[0]);

var modelView = new ServiceOtherModel();
ko.applyBindings(modelView, $("#serviceOther")[0]);