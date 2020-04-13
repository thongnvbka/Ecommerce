function ExportWarehouseApprovelModel() {
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
    self.orders = ko.observableArray();

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

        $("#sumaryPagerExportWarehouseApprovel").html(self.totalRecord() === 0 ? "Ther is not any goods transfer note" : "Show " + ((self.currentPage() - 1) * self.recordPerPage() + 1) + " to " + (self.currentPage() * self.recordPerPage() < self.totalRecord() ?
            self.currentPage() * self.recordPerPage() : self.totalRecord()) + " of " + self.totalRecord() + "goods transfer note");

        $("#pagerExportWarehouseApprovel").pager({ pagenumber: self.currentPage(), pagecount: self.totalPage(), totalrecords: self.totalRecord(), buttonClickCallback: self.pageClick });
    };

    self.pageClick = function (currentPage) {
        $("#pagerExportWarehouseApprovel").pager({ pagenumber: currentPage, pagecount: self.totalPage(), buttonClickCallback: self.search });

        self.search(currentPage);
    };

    self.search = function (currentPage) {
        self.currentPage(currentPage);
        self.isLoading(true);
        $.get("/exportwarehouseapprovel/search",
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

                //var statesGroup = _.groupBy(window.states, "id");

                //_.each(data.items,
                //    function (item) {
                //        item.statusText = statesGroup[item.status + ''][0].name;
                //        item.createdText = moment(item.forcastDate).format("DD/MM/YYYY HH:mm:ss");
                //    });


                var groupCustomer = _.groupBy(data.customers, "customerId");
                var groupOrder = _.groupBy(data.packages, "orderId");

                var orders = [];

                _.each(_.keys(groupOrder),
                function(orderId) {
                    var order = _.clone(groupOrder[orderId][0]);

                    var customer = groupCustomer[order.customerId + ""][0];

                    order.customerFullName = customer.customerFullName;
                    order.customerAddress = customer.customerAddress;
                    order.customerBalanceAvalible = customer.customerBalanceAvalible;
                    order.customerEmail = customer.customerEmail;
                    order.customerPhone = customer.customerPhone;
                    order.packages = groupOrder[orderId];
                    order.isShowDetail = ko.observable(false);
                    order.isSubmit = ko.observable(false);
                    order.showDetailNo = ko.observable(0);
                    order.changeShowDetail = function() {
                        order.isShowDetail(!order.isShowDetail());

                        var orderFist = _.find(self.orders(), function (o) {
                            return o.customerId == order.customerId && o.isFirst;
                        });

                        if (orderFist) {
                            var countOrder = _.countBy(self.orders(), function (o) {
                                return o.customerId == order.customerId && o.isShowDetail();
                            });
                            orderFist.showDetailNo(countOrder["true"] ? countOrder["true"] : 0);
                        }
                    }
                    order.orderTotal = order.total;//_.sumBy(order.packages, function (p) { return p.total });
                    order.orderPayedPrice = order.payedPrice; //_.sumBy(order.packages, function (p) { return p.payedPrice });
                    order.orderTotalProductPrice = order.totalProductPrice;//_.sumBy(order.packages, function (p) { return p.totalProductPrice });
                    order.orderTotalServicePrice = order.totalServicePrice;//_.sumBy(order.packages, function (p) { return p.totalServicePrice });

                    orders.push(order);
                });
            
                orders = _.orderBy(orders, ['customerId'], ['asc']);

                var firstCustomerId = null;
            _.each(orders,
                function (o) {
                    o.isFirst = firstCustomerId !== o.customerId;

                    if (o.isFirst) {
                        o.customerTotal = _.sumBy(orders, function (p) { return p.orderTotal });
                        o.customerPayedPrice = _.sumBy(orders, function (p) { return p.orderPayedPrice });
                        o.customerTotalProductPrice = _
                            .sumBy(orders, function (p) { return p.orderTotalProductPrice });
                        o.customerTotalServicePrice = _
                            .sumBy(orders, function (p) { return p.orderTotalServicePrice });

                        var countCustomer = _.countBy(orders, "customerId");
                        o.customerOrderNo = countCustomer[o.customerId + ""];
                    }
                    firstCustomerId = o.customerId;
                });
            self.orders(orders);
            //console.log(orders);

            //_.each(data.customers,
            //    function (c) {
            //        var packages = _.filter(data.packages, function (p) { return p.customerId === c.customerId; });


            //    });

            //self.items(data.items);
            //self.totalRecord(data.totalRecord);
            //self.renderPager();
        });
    }

    self.token = $("#exportWarehouseApprovel input[name='__RequestVerificationToken']").val();

    self.approvel = function (order) {
        order.isSubmit(true);
        $.post("/exportwarehouseapprovel/paydebit",
            { orderId: order.orderId, exportWarehouseId: order.exportWarehouseId, "__RequestVerificationToken": self.token }, function(rs) {
                order.isSubmit(false);
                if (rs.status < 0) {
                    toastr.error(rs.text);
                    return;
                }

                toastr.success(rs.text);
                self.search(1);
            });
    }

    $(function () {
        $('#ExportWarehouseApprovel-date-btn').daterangepicker({
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
                $('#ExportWarehouseApprovel-date-btn span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
            });

        self.search(1);
    });
}

var modelView = new ExportWarehouseApprovelModel();
ko.applyBindings(modelView, $("#exportWarehouseApprovel")[0])