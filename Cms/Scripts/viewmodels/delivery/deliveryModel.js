function DeliveryModel(deliveryAddModal) {
    var self = this;
    self.isLoading = ko.observable(false);

    // Search
    self.warehouses = ko.observableArray(window["warehouses"] ? window.warehouses : []);
    self.states = ko.observableArray(window["deliveryStates"] ? window.deliveryStates : []);
    self.statesGroupId = _.groupBy(window["deliveryStates"] ? window.deliveryStates : [], 'id');

    self.warehouseIdPath = ko.observable("");
    self.userId = ko.observable(null);
    self.status = ko.observable(null);
    self.fromDate = ko.observable(null);
    self.toDate = ko.observable(null);
    self.keyword = ko.observable("");
    self.moneys = ko.observableArray([]);

    // list data
    self.items = ko.observableArray();

    // pagging
    self.currentPage = ko.observable(1);
    self.recordPerPage = ko.observable(20);
    self.totalPage = ko.observable(0);
    self.totalRecord = ko.observable(0);

    self.addForm = ko.observable(null);

    self.changeCheck = function (data) {
        data.checked(!data.checked());
        self.search(1);
    }

    self.changeCheckMoney = function (data) {
        data.checked(!data.checked());

        // Đang nợ tiền khách
        if (data.id === 1 && data.checked()) {
            self.moneys()[0].checked(false);
            self.moneys()[2].checked(false);
        } else if (data.id === 0 && data.checked() || data.id === 2 && data.checked()) {
            self.moneys()[1].checked(false);
        }

        self.search(1);
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

        $("#sumaryPagerDelivery").html(self.totalRecord() === 0 ? "There is not any transport ticket" : "Show " + ((self.currentPage() - 1) * self.recordPerPage() + 1) + " to " + (self.currentPage() * self.recordPerPage() < self.totalRecord() ?
            self.currentPage() * self.recordPerPage() : self.totalRecord()) + " of " + self.totalRecord() + " transport ticket");

        $("#pagerDelivery").pager({ pagenumber: self.currentPage(), pagecount: self.totalPage(), totalrecords: self.totalRecord(), buttonClickCallback: self.pageClick });
    };

    self.pageClick = function (currentPage) {
        $("#pagerDelivery").pager({ pagenumber: currentPage, pagecount: self.totalPage(), buttonClickCallback: self.search });

        self.search(currentPage);
    };

    self.search = function (currentPage) {
        self.currentPage(currentPage);
        self.isLoading(true);

        var moneyTexts = _.map(_.filter(self.moneys(), function (it) { return it.checked() }), "id");
        var moneyText = moneyTexts.length === 0 ? "" : ';' + _.join(moneyTexts, ';') + ';';

        $.get("/delivery/search",
            {
                moneyText: moneyText,
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

                var statesGroup = _.groupBy(window.deliveryStates, "id");

                _.each(data.items,
                    function (item) {
                        item.statusText = statesGroup[item.status + ''][0].name;
                        item.createdText = moment(item.forcastDate).format("DD/MM/YYYY HH:mm:ss");
                        item.statusClass = item.status === 0
                            ? 'label label-warning'
                            : item.status === 1
                            ? 'label label-info'
                            : item.status === 2
                            ? 'label label-warning'
                            : item.status === 3
                            ? 'label label-success'
                            : item.status === 4 ? 'label label-success' : 'label label-danger';
                    });

                self.items(data.items);
                self.totalRecord(data.totalRecord);
                self.renderPager();
            });
    }

    self.callback = function () {
        self.search(self.currentPage());
    }

    //self.addBill = function () {
    //    if (self.addForm() == null) {
    //        self.addForm(new DeliveryAddModel(self.callback));
    //        ko.applyBindings(self.addForm(), $("#DeliveryAddModel")[0]);
    //    }

    //    self.addForm().showAddForm();
    //}

    self.detail = function (data) {
        deliveryAddModal.showDetail(data.id);
        deliveryAddModal.callback = self.callback;
    }

    self.token = $("#delivery input[name='__RequestVerificationToken']").val();

    self.delete = function(data) {
        swal({
            title: 'Are you sure you want to delete this item?',
            text: 'Dispatch note "D' + data.code + '"',
            type: 'warning',
            showCancelButton: true,
            //confirmButtonColor: '#3085d6',
            //cancelButtonColor: '#d33',
           cancelButtonText: 'Cancel',
            confirmButtonText: 'Delete'
        }).then(function () {
                var submitData = { deliveryId: data.id };

                submitData["__RequestVerificationToken"] = self.token;
                $.post("/delivery/delete", submitData,
                    function (rs) {
                        if (rs.status < 0) {
                            toastr.error(rs.text);
                            return;
                        }

                        toastr.success(rs.text);
                        self.search(self.currentPage());
                    });
            }, function () { });
    }

    $(function () {
        var arr = _.split(window.location.href, "#delivery");
        if (arr.length > 1) {
            deliveryAddModal.showDetail(arr[1]);
            deliveryAddModal.callback = self.callback;
        }

        self.moneys([
            {
                id: 0,
                name: "Note with receivables uncollected",
                checked: ko.observable(false)
            },
            {
                id: 1,
                name: "Note with receivables fully collected ",
                checked: ko.observable(false)
            },
            {
                id: 2,
                name: "Note due from last period",
                checked: ko.observable(false)
            }
        ]);

        //var drop = null;
        $('.dropdown-toggle-custome').click(function () {
            var $this = $(this);
            var drop = $this.parent().find(".dropdown-menu");
            drop.slideToggle(10);

            $(document).click(function (e) {
                if (drop && $(e.target).parents().filter($this).length === 0 && $(e.toElement).filter($this).length === 0
                    && $(e.target).parents().filter('.size18.isCheckbox').length === 0 && $(e.toElement).filter(('.size18.isCheckbox')).length === 0) {
                    drop.slideUp(10);
                }
            });
        });
        
        $('#Delivery-date-btn').daterangepicker({
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
                $('#Delivery-date-btn span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
            });

        self.search(1);
    });
}

var deliveryAddModelView = new DeliveryAddModel();

ko.applyBindings(deliveryAddModelView, $("#DeliveryUpdateModalBinding")[0]);

var modelView = new DeliveryModel(deliveryAddModelView);
ko.applyBindings(modelView, $("#delivery")[0])