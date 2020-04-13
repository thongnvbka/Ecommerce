function PackageNoCodePrcessModel() {
    var self = this;
    self.isLoading = ko.observable(false);

    // Search
    self.warehouses = ko.observableArray(window["warehouses"] ? window.warehouses : []);
    self.states = ko.observableArray(window["states"] ? window.states : []);

    self.warehouseIdPath = ko.observable("");
    self.systemId = ko.observable(null);
    self.userId = ko.observable(null);
    self.status = ko.observable(null);
    self.day = ko.observable(null);
    self.fromDate = ko.observable(null);
    self.toDate = ko.observable(null);
    self.keyword = ko.observable("");

    // list data
    self.packages = ko.observableArray();
    self.isCheckedAll = ko.observable(false);
    self.hasItemChecked = ko.observable(false);

    // pagging
    self.currentPage = ko.observable(1);
    self.recordPerPage = ko.observable(20);
    self.totalPage = ko.observable(0);
    self.totalRecord = ko.observable(0);

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

    var dayFirst = true;
    self.day.subscribe(function () {
        if (dayFirst) {
            dayFirst = false;
            return;
        }
        self.search(1);
    });

    self.changeIsCheckedAll = function () {
        self.isCheckedAll(!self.isCheckedAll());

        _.each(self.packages(), function (it) {
            it.isChecked(self.isCheckedAll());
        });

        self.hasItemChecked(self.isCheckedAll());
    }

    self.changeIsChecked = function (item) {
        item.isChecked(!item.isChecked());

        var count = _.countBy(self.packages(), function (it) { return it.isChecked(); });

        if (count["true"] === undefined) {
            self.isCheckedAll(false);
        } else if (count["false"] === undefined) {
            self.isCheckedAll(true);
        } else if (count["true"] && count["false"]) {
            self.isCheckedAll(false);
        } else {
            self.isCheckedAll(true);
        }

        self.hasItemChecked(count["true"] !== undefined);
    }

    self.renderPager = function () {
        self.totalPage(Math.ceil(self.totalRecord() / self.recordPerPage()));

        $("#sumaryPagerPackageNoCodeProcess").html(self.totalRecord() === 0 ? "No package" : "Show " + ((self.currentPage() - 1) * self.recordPerPage() + 1) + " to " + (self.currentPage() * self.recordPerPage() < self.totalRecord() ?
            self.currentPage() * self.recordPerPage() : self.totalRecord()) + " of " + self.totalRecord() + " package");

        $("#pagerPackageNoCodeProcess").pager({ pagenumber: self.currentPage(), pagecount: self.totalPage(), totalrecords: self.totalRecord(), buttonClickCallback: self.pageClick });
    };

    self.pageClick = function (currentPage) {
        $("#pagerPackageNoCodeProcess").pager({ pagenumber: currentPage, pagecount: self.totalPage(), buttonClickCallback: self.search });

        self.search(currentPage);
    };

    self.search = function (currentPage) {
        self.currentPage(currentPage);
        self.isLoading(true);
        $.get("/PackageNoCodeProcess/search",
            {
                warehouseIdPath: self.warehouseIdPath(),
                systemId: self.systemId(),
                userId: self.userId(),
                status: self.status(),
                day: self.day(),
                fromDate: self.fromDate(),
                toDate: self.toDate(),
                keyword: self.keyword(),
                currentPage: self.currentPage(),
                recordPerPage: self.recordPerPage()
            }, function (data) {
                self.isLoading(false);

                //var statesGroup = _.groupBy(window.states, "id");

                //var orderFirst = null;
                //_.each(data.items,
                //    function (item) {
                //        item.isFirst = item.orderCode !== orderFirst;
                //        orderFirst = item.orderCode;

                //        item.statusText = statesGroup[item.status + ''][0].name;

                //        item.statusClass = item.status === 0
                //            ? 'label label-warning'
                //            : item.status === 1
                //                ? 'label label-success'
                //                : item.status === 2
                //                    ? 'label label-info'
                //                    : item.status === 4 ? 'label label-primary' : 'label label-danger';

                //        item.forcastDateText = item.forcastDate ? moment(item.forcastDate).fromNow() : '';
                //    });

            _.each(data.packages,
                function(it) {
                    it.isChecked = ko.observable(false);
                });

                self.packages(data.packages);
                self.totalRecord(data.totalRecord);
                self.renderPager();
            });
    }

    // Thêm vào hàng mất mã
    self.addPackageLoseForm = ko.observable();
    self.showAddPackageLoseCallback = function (data) {
        if (self.addPackageLoseForm() == null) {
            self.addPackageLoseForm(new AddPackageLoseModel());
            ko.applyBindings(self.addPackageLoseForm(), $("#addPackageLoseModal")[0]);
        }

        self.addPackageLoseForm().show(data.transportCode, function () {
            self.search(1);
        }, data.id);
    }

    // Tìm kiếm Orders cho MVĐ
    self.addPackageForm = ko.observable();
    self.showAddPackageCallback = function (data) {
        if (self.addPackageForm() == null) {
            self.addPackageForm(new AddPackageModel());
            ko.applyBindings(self.addPackageForm(), $("#addPackageModal")[0]);
        }

        self.addPackageForm().show(data.transportCode,
            function() {
                self.search(1);
            },
            data.id);
    }

    self.remove = function(data) {
        swal({
            title: 'Are you sure you want to delete this code?',
            text: "After deletion will not be able to restore the data",
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Delete',
            cancelButtonText: 'No'
        }).then(function() {
            $.post("/PackageNoCodeProcess/Delete",
                { packageId: data.id },
                function(rs) {
                    if (rs && rs.status < 0) {
                        toastr.warning(rs.text);
                        return;
                    }

                    toastr.success(rs.text);
                    self.search(1);
                });
        }, function(){});
    }

    self.removeAll = function () {
        var packages = _.map(_.filter(self.packages(), function (it) { return it.isChecked(); }), "code");

        swal({
            title: 'Are you sure you want to delete "' + packages.length + '"this code missing information?',
            text: "After deletion will not be able to restore the data",
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Delete',
            cancelButtonText: 'No'
        }).then(function () {

            var packageCodes = ';'+_.join(packages, ';') + ';';

            $.post("/PackageNoCodeProcess/DeleteAll",
                { packageCodes: packageCodes },
                function (rs) {
                    if (rs && rs.status < 0) {
                        toastr.warning(rs.text);
                        return;
                    }
                    self.hasItemChecked(false);
                    toastr.success(rs.text);
                    self.search(1);
                });
        }, function () { });
    }

    $(function () {
        $('#forcastDate-btn').daterangepicker({
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
                $('#forcastDate-btn span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
            });

        self.search(1);
    });
}

// Bind PackageModel
var modelView = new PackageNoCodePrcessModel();
ko.applyBindings(modelView, $("#packageNoCodeProcess")[0]);