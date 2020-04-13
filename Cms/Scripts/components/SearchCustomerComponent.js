ko.components.register('search-customer-widget', {
    viewModel: function (params) {
        var delayTimer;
        var self = this;
        self.total = ko.observable(0);
        self.page = ko.observable(1);
        self.pagesize = ko.observable(5);
        self.pageTotal = ko.observable(0);
        self.listCustomer = ko.observable([]);
        self.keyword = ko.observable('');
        self.customerName = params.customerName;
        self.isLoading = ko.observable(true);

        //==================== Tìm kiếm ===================================================
        self.search = function (page) {
            self.isLoading(false);
            clearTimeout(delayTimer);

            delayTimer = setTimeout(function () {
                $.post("/Customer/GetSearch", { page: self.page(), pageSize: self.pagesize(), keyword: self.keyword() }, function (data) {

                    self.total(data.totalRecord);
                    self.listCustomer(data.listCustomer);
                    self.paging();
                    self.isLoading(true);
                });
            }, 500);
        }.bind(this);

        self.searchCustomer = function () {
            self.search(self.page());
        }.bind(this);

        self.showForm = function () {
            $('#searchCustomerWidget').toggle();
        };

        self.selectCustomer = function (data) {
            self.customerName(data.FullName);
            params.customer(data);
            params.clickSelect();

            self.keyword('');
            self.listCustomer([]);
            self.showForm();
        };

        self.removeCustomer = function () {
            self.customerName("- Select subject - ");
            params.customer(null);
            params.removeSelect();
            self.showForm();
        };

        //$(function () {
        //    self.search(self.page());
        //});

        //==================== Phân trang ==================================================
        self.listPage = ko.observableArray([]);
        self.pageStart = ko.observable(false);
        self.pageEnd = ko.observable(false);
        self.pageNext = ko.observable(false);
        self.pagePrev = ko.observable(false);
        self.pageTitle = ko.observable("");

        //Hàm khởi tạo phân trang
        self.paging = function () {
            var listPage = [];

            self.page(self.page() <= 0 ? 1 : self.page());
            self.pageTotal(Math.ceil(self.total() / self.pagesize()));
            self.page() > 3 ? self.pageStart(true) : self.pageStart(false);
            self.page() > 4 ? self.pageNext(true) : self.pageNext(false);
            self.pageTotal() - 2 > self.page() ? self.pageEnd(true) : self.pageEnd(false);
            self.pageTotal() - 3 > self.page() ? self.pagePrev(true) : self.pagePrev(false);

            var start = (self.page() - 2) <= 0 ? 1 : (self.page() - 2);
            var end = (self.page() + 2) >= self.pageTotal() ? self.pageTotal() : (self.page() + 2);

            for (var i = start; i <= end; i++) {
                listPage.push({ Page: i });
            }

            self.listPage(listPage);
            self.pageTitle("Show <b>" + (((self.page() - 1) * self.pagesize()) + 1) + "</b> to <b>" + (((self.page()) * self.pagesize()) > self.total() ? self.total() : ((self.page()) * self.pagesize())) + "</b> of <b>" + self.total() + "</b> Record" );
        }.bind(this);

        //Sự kiện click vào nút phân trang
        self.setPage = function (data) {
            if (self.page() !== data.Page) {
                self.page(data.Page);
                self.search(self.page());
            }
        }.bind(this);
    },
    template:
            '<div id="viewCustomerSearch">\
                <button id="buttonShowForm" data-bind="click: showForm" type="button" class="btn btn-block btn-default">\
                    <span data-bind="text: customerName"></span>\
                    <!-- ko if: customerName() != "- Select customer -  " -->\
                        <span data-bind="click: removeCustomer" class="pull-right"><i class="fa fa-times"></i></span>\
                    <!-- /ko -->\
                </button>\
                <div id="searchCustomerWidget">\
                    <input class="form-control" data-bind="value: keyword, valueUpdate: \'keyup\', event: { keyup: searchCustomer}" autocomplete="off" />\
                    <div data-bind="css: isLoading() === true? \'display-none\' : \'\'">\
                        <div class="spinner">\
                            <div class="rect1"></div>\
                            <div class="rect2"></div>\
                            <div class="rect3"></div>\
                            <div class="rect4"></div>\
                            <div class="rect5"></div>\
                        </div>\
                    </div>\
                    <div data-bind="css: isLoading() === true? \'\' : \'display-none\'">\
                        <table  id="customerSearch">\
                            <tbody data-bind="foreach: listCustomer">\
                                <tr>\
                                    <td>\
                                        <a data-bind="click: $parent.selectCustomer" href="javascript:;">\
                                            <img data-bind="attr: { src: Avatar }" class="pull-left" style="width:40px; height:40px; margin-right:5px;" />\
                                            <b data-bind="text: FullName"></b><br />\
                                            <i data-bind="text: SystemName"></i>\
                                        </a>\
                                    </td>\
                                </tr>\
                            </tbody>\
                            <tfoot data-bind="if: listCustomer().length == 0">\
                                <tr>\
                                    <td>\
                                        No data to display\
                                    </td>\
                                </tr>\
                            </tfoot>\
                        </table>\
                    </div>\
                    <div data-bind="if: listCustomer().length > 0" class="mt15">\
                        <div data-bind="html: pageTitle" class="pull-left"></div>\
                            <ul class="pagination pagination-sm no-margin pull-right">\
                                <li data-bind="if: pageStart">\
                                    <a data-bind="click: function(){ setPage({Page: 1})}" href="javascript:;"><i class="fa fa-angle-double-left"></i></a>\
                                </li>\
                                <li data-bind="if: pageNext">\
                                    <a data-bind="click: function(){ setPage({Page: (page() - 3) < 1 ? 1 : (page() - 3) })}" href="javascript:;">...</a>\
                                </li>\
                                <!-- ko foreach: listPage -->\
                                <li data-bind="css: Page ===  $parent.page() ? \'active\' :\'\' ">\
                                    <a data-bind="text: Page, click: $parent.setPage" href="javascript:;"></a>\
                                </li>\
                                <!-- /ko -->\
                                <li data-bind="if: pagePrev">\
                                    <a data-bind="click: function(){ setPage({Page: (page() + 3) > pageTotal() ? pageTotal() : (page() + 3) })}" href="javascript:;">...</a>\
                                </li>\
                                <li data-bind="if: pageEnd">\
                                    <a data-bind="click: function(){ setPage({Page: pageTotal()})}" href="javascript:;"><i class="fa fa-angle-double-right"></i></a>\
                                </li>\
                            </ul>\
                        </div>\
                    </div>\
                </div>\
            </div>\
            <style>\
                #viewCustomerSearch{\
                    width:100%;\
                }\
                #buttonShowForm{\
                    width:300px;\
                    text-align:left;\
                }\
                #searchCustomerWidget{\
                    border: 1px solid #d2d6de;\
                    display: none;\
                    position: absolute;\
                    width: 300px;\
                    z-index: 9999999;\
                    background: rgb(255, 255, 255);\
                    padding: 10px;\
                }\
                #customerSearch {\
                    width: 100%;\
                    margin: 5px 0px;\
                }\
                #customerSearch td{\
                }\
                #customerSearch td a{\
                    padding: 5px;\
                    display: block;\
                    border-bottom:1px dotted #d2d6de;\
                }\
                #customerSearch td a:hover {\
                    background-color: #3c8dbc !important;\
                    color: #fff !important;\
                }\
            </style>'
});


ko.components.register('search-customerfundbill-widget', {
    viewModel: function (params) {
        var delayTimer;
        var self = this;
        self.total = ko.observable(0);
        self.page = ko.observable(1);
        self.pagesize = ko.observable(5);
        self.pageTotal = ko.observable(0);
        self.listCustomer = ko.observable([]);
        self.keyword = ko.observable('');
        self.customerName = params.customerName;
        self.isLoading = ko.observable(true);

        //==================== Tìm kiếm ===================================================
        self.search = function (page) {
            self.isLoading(false);
            clearTimeout(delayTimer);

            delayTimer = setTimeout(function () {
                $.post("/Customer/GetSearch", { page: self.page(), pageSize: self.pagesize(), keyword: self.keyword() }, function (data) {

                    self.total(data.totalRecord);
                    self.listCustomer(data.listCustomer);
                    self.paging();
                    self.isLoading(true);
                });
            }, 500);
        }.bind(this);

        self.searchCustomer = function () {
            self.search(self.page());
        }.bind(this);

        self.showForm = function () {
            $('#searchCustomerFundBillWidget').toggle();
        };

        self.selectCustomer = function (data) {
            self.customerName(data.FullName);

            params.customer(data);
            params.clickSelect();

            self.keyword('');
            self.listCustomer([]);
            self.showForm();
        };

        self.removeCustomer = function () {
            self.customerName("- Select customer - ");
            params.customer(null);
            params.removeSelect();
            self.showForm();
        };

        $(function () {
            //self.search(self.page());
        });

        //==================== Phân trang ==================================================
        self.listPage = ko.observableArray([]);
        self.pageStart = ko.observable(false);
        self.pageEnd = ko.observable(false);
        self.pageNext = ko.observable(false);
        self.pagePrev = ko.observable(false);
        self.pageTitle = ko.observable("");

        //Hàm khởi tạo phân trang
        self.paging = function () {
            var listPage = [];

            self.page(self.page() <= 0 ? 1 : self.page());
            self.pageTotal(Math.ceil(self.total() / self.pagesize()));
            self.page() > 3 ? self.pageStart(true) : self.pageStart(false);
            self.page() > 4 ? self.pageNext(true) : self.pageNext(false);
            self.pageTotal() - 2 > self.page() ? self.pageEnd(true) : self.pageEnd(false);
            self.pageTotal() - 3 > self.page() ? self.pagePrev(true) : self.pagePrev(false);

            var start = (self.page() - 2) <= 0 ? 1 : (self.page() - 2);
            var end = (self.page() + 2) >= self.pageTotal() ? self.pageTotal() : (self.page() + 2);

            for (var i = start; i <= end; i++) {
                listPage.push({ Page: i });
            }

            self.listPage(listPage);
            self.pageTitle("Show <b>" + (((self.page() - 1) * self.pagesize()) + 1) + "</b> to <b>" + (((self.page()) * self.pagesize()) > self.total() ? self.total() : ((self.page()) * self.pagesize())) + "</b> of <b>" + self.total() + "</b> Record" );
        }.bind(this);

        //Sự kiện click vào nút phân trang
        self.setPage = function (data) {
            if (self.page() !== data.Page) {
                self.page(data.Page);
                self.search(self.page());
            }
        }.bind(this);
    },
    template:
            '<div id="viewCustomerSearch">\
                <button id="buttonShowForm" data-bind="click: showForm" type="button" class="btn btn-block btn-default">\
                    <span data-bind="text: customerName"></span>\
                    <!-- ko if: customerName() != "- Select customer -  " -->\
                        <span data-bind="click: removeCustomer" class="pull-right"><i class="fa fa-times"></i></span>\
                    <!-- /ko -->\
                </button>\
                <div id="searchCustomerFundBillWidget">\
                    <input class="form-control" data-bind="value: keyword, valueUpdate: \'keyup\', event: { keyup: searchCustomer}" autocomplete="off" />\
                    <div data-bind="css: isLoading() === true? \'display-none\' : \'\'">\
                        <div class="spinner">\
                            <div class="rect1"></div>\
                            <div class="rect2"></div>\
                            <div class="rect3"></div>\
                            <div class="rect4"></div>\
                            <div class="rect5"></div>\
                        </div>\
                    </div>\
                    <div data-bind="css: isLoading() === true? \'\' : \'display-none\'">\
                        <table  id="customerSearch">\
                            <tbody data-bind="foreach: listCustomer">\
                                <tr>\
                                    <td>\
                                        <a data-bind="click: $parent.selectCustomer" href="javascript:;">\
                                            <img data-bind="attr: { src: Avatar }" class="pull-left" style="width:40px; height:40px; margin-right:5px;" />\
                                            <b data-bind="text: FullName"></b><br />\
                                            <i data-bind="text: SystemName"></i>\
                                        </a>\
                                    </td>\
                                </tr>\
                            </tbody>\
                            <tfoot data-bind="if: listCustomer().length == 0">\
                                <tr>\
                                    <td>\
                                        No data to display\
                                    </td>\
                                </tr>\
                            </tfoot>\
                        </table>\
                    </div>\
                    <div data-bind="if: listCustomer().length > 0" class="mt15">\
                        <div data-bind="html: pageTitle" class="pull-left"></div>\
                            <ul class="pagination pagination-sm no-margin pull-right">\
                                <li data-bind="if: pageStart">\
                                    <a data-bind="click: function(){ setPage({Page: 1})}" href="javascript:;"><i class="fa fa-angle-double-left"></i></a>\
                                </li>\
                                <li data-bind="if: pageNext">\
                                    <a data-bind="click: function(){ setPage({Page: (page() - 3) < 1 ? 1 : (page() - 3) })}" href="javascript:;">...</a>\
                                </li>\
                                <!-- ko foreach: listPage -->\
                                <li data-bind="css: Page ===  $parent.page() ? \'active\' :\'\' ">\
                                    <a data-bind="text: Page, click: $parent.setPage" href="javascript:;"></a>\
                                </li>\
                                <!-- /ko -->\
                                <li data-bind="if: pagePrev">\
                                    <a data-bind="click: function(){ setPage({Page: (page() + 3) > pageTotal() ? pageTotal() : (page() + 3) })}" href="javascript:;">...</a>\
                                </li>\
                                <li data-bind="if: pageEnd">\
                                    <a data-bind="click: function(){ setPage({Page: pageTotal()})}" href="javascript:;"><i class="fa fa-angle-double-right"></i></a>\
                                </li>\
                            </ul>\
                        </div>\
                    </div>\
                </div>\
            </div>\
            <style>\
                #viewCustomerSearch{\
                    width:100%;\
                }\
                #buttonShowForm{\
                    width:300px;\
                    text-align:left;\
                }\
                #searchCustomerFundBillWidget{\
                    border: 1px solid #d2d6de;\
                    display: none;\
                    position: absolute;\
                    width: 300px;\
                    z-index: 9999999;\
                    background: rgb(255, 255, 255);\
                    padding: 10px;\
                }\
                #customerSearch {\
                    width: 100%;\
                    margin: 5px 0px;\
                }\
                #customerSearch td{\
                }\
                #customerSearch td a{\
                    padding: 5px;\
                    display: block;\
                    border-bottom:1px dotted #d2d6de;\
                }\
                #customerSearch td a:hover {\
                    background-color: #3c8dbc !important;\
                    color: #fff !important;\
                }\
            </style>'
});


ko.components.register('search-customermustcollect-widget', {
    viewModel: function (params) {
        var delayTimer;
        var self = this;
        self.total = ko.observable(0);
        self.page = ko.observable(1);
        self.pagesize = ko.observable(5);
        self.pageTotal = ko.observable(0);
        self.listCustomer = ko.observable([]);
        self.keyword = ko.observable('');
        self.customerName = params.customerName;
        self.isLoading = ko.observable(true);

        //==================== Tìm kiếm ===================================================
        self.search = function (page) {
            self.isLoading(false);
            clearTimeout(delayTimer);

            delayTimer = setTimeout(function () {
                $.post("/Customer/GetSearch", { page: self.page(), pageSize: self.pagesize(), keyword: self.keyword() }, function (data) {

                    self.total(data.totalRecord);
                    self.listCustomer(data.listCustomer);
                    self.paging();
                    self.isLoading(true);
                });
            }, 500);
        }.bind(this);

        self.searchCustomer = function () {
            self.search(self.page());
        }.bind(this);

        self.showForm = function () {
            $('#searchCustomermustcollectWidget').toggle();
        };

        self.selectCustomer = function (data) {
            self.customerName(data.FullName);

            params.customer(data);
            params.clickSelect();

            self.keyword('');
            self.listCustomer([]);
            self.showForm();
        };

        self.removeCustomer = function () {
            self.customerName("- Select customer - ");
            params.customer(null);
            params.removeSelect();
            self.showForm();
        };

        $(function () {
            //self.search(self.page());
        });

        //==================== Phân trang ==================================================
        self.listPage = ko.observableArray([]);
        self.pageStart = ko.observable(false);
        self.pageEnd = ko.observable(false);
        self.pageNext = ko.observable(false);
        self.pagePrev = ko.observable(false);
        self.pageTitle = ko.observable("");

        //Hàm khởi tạo phân trang
        self.paging = function () {
            var listPage = [];

            self.page(self.page() <= 0 ? 1 : self.page());
            self.pageTotal(Math.ceil(self.total() / self.pagesize()));
            self.page() > 3 ? self.pageStart(true) : self.pageStart(false);
            self.page() > 4 ? self.pageNext(true) : self.pageNext(false);
            self.pageTotal() - 2 > self.page() ? self.pageEnd(true) : self.pageEnd(false);
            self.pageTotal() - 3 > self.page() ? self.pagePrev(true) : self.pagePrev(false);

            var start = (self.page() - 2) <= 0 ? 1 : (self.page() - 2);
            var end = (self.page() + 2) >= self.pageTotal() ? self.pageTotal() : (self.page() + 2);

            for (var i = start; i <= end; i++) {
                listPage.push({ Page: i });
            }

            self.listPage(listPage);
            self.pageTitle("Show <b>" + (((self.page() - 1) * self.pagesize()) + 1) + "</b> to <b>" + (((self.page()) * self.pagesize()) > self.total() ? self.total() : ((self.page()) * self.pagesize())) + "</b> of <b>" + self.total() + "</b> Record" );
        }.bind(this);

        //Sự kiện click vào nút phân trang
        self.setPage = function (data) {
            if (self.page() !== data.Page) {
                self.page(data.Page);
                self.search(self.page());
            }
        }.bind(this);
    },
    template:
            '<div id="viewCustomerSearch">\
                <button id="buttonShowForm" data-bind="click: showForm" type="button" class="btn btn-block btn-default">\
                    <span data-bind="text: customerName"></span>\
                    <!-- ko if: customerName() != "- Select customer -  " -->\
                        <span data-bind="click: removeCustomer" class="pull-right"><i class="fa fa-times"></i></span>\
                    <!-- /ko -->\
                </button>\
                <div id="searchCustomermustcollectWidget">\
                    <input class="form-control" data-bind="value: keyword, valueUpdate: \'keyup\', event: { keyup: searchCustomer}" autocomplete="off" />\
                    <div data-bind="css: isLoading() === true? \'display-none\' : \'\'">\
                        <div class="spinner">\
                            <div class="rect1"></div>\
                            <div class="rect2"></div>\
                            <div class="rect3"></div>\
                            <div class="rect4"></div>\
                            <div class="rect5"></div>\
                        </div>\
                    </div>\
                    <div data-bind="css: isLoading() === true? \'\' : \'display-none\'">\
                        <table  id="customerSearch">\
                            <tbody data-bind="foreach: listCustomer">\
                                <tr>\
                                    <td>\
                                        <a data-bind="click: $parent.selectCustomer" href="javascript:;">\
                                            <img data-bind="attr: { src: Avatar }" class="pull-left" style="width:40px; height:40px; margin-right:5px;" />\
                                            <b data-bind="text: FullName"></b><br />\
                                            <i data-bind="text: SystemName"></i>\
                                        </a>\
                                    </td>\
                                </tr>\
                            </tbody>\
                            <tfoot data-bind="if: listCustomer().length == 0">\
                                <tr>\
                                    <td>\
                                        No data to display\
                                    </td>\
                                </tr>\
                            </tfoot>\
                        </table>\
                    </div>\
                    <div data-bind="if: listCustomer().length > 0" class="mt15">\
                        <div data-bind="html: pageTitle" class="pull-left"></div>\
                            <ul class="pagination pagination-sm no-margin pull-right">\
                                <li data-bind="if: pageStart">\
                                    <a data-bind="click: function(){ setPage({Page: 1})}" href="javascript:;"><i class="fa fa-angle-double-left"></i></a>\
                                </li>\
                                <li data-bind="if: pageNext">\
                                    <a data-bind="click: function(){ setPage({Page: (page() - 3) < 1 ? 1 : (page() - 3) })}" href="javascript:;">...</a>\
                                </li>\
                                <!-- ko foreach: listPage -->\
                                <li data-bind="css: Page ===  $parent.page() ? \'active\' :\'\' ">\
                                    <a data-bind="text: Page, click: $parent.setPage" href="javascript:;"></a>\
                                </li>\
                                <!-- /ko -->\
                                <li data-bind="if: pagePrev">\
                                    <a data-bind="click: function(){ setPage({Page: (page() + 3) > pageTotal() ? pageTotal() : (page() + 3) })}" href="javascript:;">...</a>\
                                </li>\
                                <li data-bind="if: pageEnd">\
                                    <a data-bind="click: function(){ setPage({Page: pageTotal()})}" href="javascript:;"><i class="fa fa-angle-double-right"></i></a>\
                                </li>\
                            </ul>\
                        </div>\
                    </div>\
                </div>\
            </div>\
            <style>\
                #viewCustomerSearch{\
                    width:100%;\
                }\
                #buttonShowForm{\
                    width:300px;\
                    text-align:left;\
                }\
                #searchCustomermustcollectWidget{\
                    border: 1px solid #d2d6de;\
                    display: none;\
                    position: absolute;\
                    width: 300px;\
                    z-index: 9999999;\
                    background: rgb(255, 255, 255);\
                    padding: 10px;\
                }\
                #customerSearch {\
                    width: 100%;\
                    margin: 5px 0px;\
                }\
                #customerSearch td{\
                }\
                #customerSearch td a{\
                    padding: 5px;\
                    display: block;\
                    border-bottom:1px dotted #d2d6de;\
                }\
                #customerSearch td a:hover {\
                    background-color: #3c8dbc !important;\
                    color: #fff !important;\
                }\
            </style>'
});


ko.components.register('search-customermustreturn-widget', {
    viewModel: function (params) {
        var delayTimer;
        var self = this;
        self.total = ko.observable(0);
        self.page = ko.observable(1);
        self.pagesize = ko.observable(5);
        self.pageTotal = ko.observable(0);
        self.listCustomer = ko.observable([]);
        self.keyword = ko.observable('');
        self.customerName = params.customerName;
        self.isLoading = ko.observable(true);

        //==================== Tìm kiếm ===================================================
        self.search = function (page) {
            self.isLoading(false);
            clearTimeout(delayTimer);

            delayTimer = setTimeout(function () {
                $.post("/Customer/GetSearch", { page: self.page(), pageSize: self.pagesize(), keyword: self.keyword() }, function (data) {

                    self.total(data.totalRecord);
                    self.listCustomer(data.listCustomer);
                    self.paging();
                    self.isLoading(true);
                });
            }, 500);
        }.bind(this);

        self.searchCustomer = function () {
            self.search(self.page());
        }.bind(this);

        self.showForm = function () {
            $('#searchCustomermustreturnWidget').toggle();
        };

        self.selectCustomer = function (data) {
            self.customerName(data.FullName);

            params.customer(data);
            params.clickSelect();

            self.keyword('');
            self.listCustomer([]);
            self.showForm();
        };

        self.removeCustomer = function () {
            self.customerName("- Select customer - ");
            params.customer(null);
            params.removeSelect();
            self.showForm();
        };

        $(function () {
            //self.search(self.page());
        });

        //==================== Phân trang ==================================================
        self.listPage = ko.observableArray([]);
        self.pageStart = ko.observable(false);
        self.pageEnd = ko.observable(false);
        self.pageNext = ko.observable(false);
        self.pagePrev = ko.observable(false);
        self.pageTitle = ko.observable("");

        //Hàm khởi tạo phân trang
        self.paging = function () {
            var listPage = [];

            self.page(self.page() <= 0 ? 1 : self.page());
            self.pageTotal(Math.ceil(self.total() / self.pagesize()));
            self.page() > 3 ? self.pageStart(true) : self.pageStart(false);
            self.page() > 4 ? self.pageNext(true) : self.pageNext(false);
            self.pageTotal() - 2 > self.page() ? self.pageEnd(true) : self.pageEnd(false);
            self.pageTotal() - 3 > self.page() ? self.pagePrev(true) : self.pagePrev(false);

            var start = (self.page() - 2) <= 0 ? 1 : (self.page() - 2);
            var end = (self.page() + 2) >= self.pageTotal() ? self.pageTotal() : (self.page() + 2);

            for (var i = start; i <= end; i++) {
                listPage.push({ Page: i });
            }

            self.listPage(listPage);
            self.pageTitle("Show <b>" + (((self.page() - 1) * self.pagesize()) + 1) + "</b> to <b>" + (((self.page()) * self.pagesize()) > self.total() ? self.total() : ((self.page()) * self.pagesize())) + "</b> of <b>" + self.total() + "</b> Record" );
        }.bind(this);

        //Sự kiện click vào nút phân trang
        self.setPage = function (data) {
            if (self.page() !== data.Page) {
                self.page(data.Page);
                self.search(self.page());
            }
        }.bind(this);
    },
    template:
            '<div id="viewCustomerSearch">\
                <button id="buttonShowForm" data-bind="click: showForm" type="button" class="btn btn-block btn-default">\
                    <span data-bind="text: customerName"></span>\
                    <!-- ko if: customerName() != "- Select customer -  " -->\
                        <span data-bind="click: removeCustomer" class="pull-right"><i class="fa fa-times"></i></span>\
                    <!-- /ko -->\
                </button>\
                <div id="searchCustomermustreturnWidget">\
                    <input class="form-control" data-bind="value: keyword, valueUpdate: \'keyup\', event: { keyup: searchCustomer}" autocomplete="off" />\
                    <div data-bind="css: isLoading() === true? \'display-none\' : \'\'">\
                        <div class="spinner">\
                            <div class="rect1"></div>\
                            <div class="rect2"></div>\
                            <div class="rect3"></div>\
                            <div class="rect4"></div>\
                            <div class="rect5"></div>\
                        </div>\
                    </div>\
                    <div data-bind="css: isLoading() === true? \'\' : \'display-none\'">\
                        <table  id="customerSearch">\
                            <tbody data-bind="foreach: listCustomer">\
                                <tr>\
                                    <td>\
                                        <a data-bind="click: $parent.selectCustomer" href="javascript:;">\
                                            <img data-bind="attr: { src: Avatar }" class="pull-left" style="width:40px; height:40px; margin-right:5px;" />\
                                            <b data-bind="text: FullName"></b><br />\
                                            <i data-bind="text: SystemName"></i>\
                                        </a>\
                                    </td>\
                                </tr>\
                            </tbody>\
                            <tfoot data-bind="if: listCustomer().length == 0">\
                                <tr>\
                                    <td>\
                                        No data to display\
                                    </td>\
                                </tr>\
                            </tfoot>\
                        </table>\
                    </div>\
                    <div data-bind="if: listCustomer().length > 0" class="mt15">\
                        <div data-bind="html: pageTitle" class="pull-left"></div>\
                            <ul class="pagination pagination-sm no-margin pull-right">\
                                <li data-bind="if: pageStart">\
                                    <a data-bind="click: function(){ setPage({Page: 1})}" href="javascript:;"><i class="fa fa-angle-double-left"></i></a>\
                                </li>\
                                <li data-bind="if: pageNext">\
                                    <a data-bind="click: function(){ setPage({Page: (page() - 3) < 1 ? 1 : (page() - 3) })}" href="javascript:;">...</a>\
                                </li>\
                                <!-- ko foreach: listPage -->\
                                <li data-bind="css: Page ===  $parent.page() ? \'active\' :\'\' ">\
                                    <a data-bind="text: Page, click: $parent.setPage" href="javascript:;"></a>\
                                </li>\
                                <!-- /ko -->\
                                <li data-bind="if: pagePrev">\
                                    <a data-bind="click: function(){ setPage({Page: (page() + 3) > pageTotal() ? pageTotal() : (page() + 3) })}" href="javascript:;">...</a>\
                                </li>\
                                <li data-bind="if: pageEnd">\
                                    <a data-bind="click: function(){ setPage({Page: pageTotal()})}" href="javascript:;"><i class="fa fa-angle-double-right"></i></a>\
                                </li>\
                            </ul>\
                        </div>\
                    </div>\
                </div>\
            </div>\
            <style>\
                #viewCustomerSearch{\
                    width:100%;\
                }\
                #buttonShowForm{\
                    width:300px;\
                    text-align:left;\
                }\
                #searchCustomermustreturnWidget{\
                    border: 1px solid #d2d6de;\
                    display: none;\
                    position: absolute;\
                    width: 300px;\
                    z-index: 9999999;\
                    background: rgb(255, 255, 255);\
                    padding: 10px;\
                }\
                #customerSearch {\
                    width: 100%;\
                    margin: 5px 0px;\
                }\
                #customerSearch td{\
                }\
                #customerSearch td a{\
                    padding: 5px;\
                    display: block;\
                    border-bottom:1px dotted #d2d6de;\
                }\
                #customerSearch td a:hover {\
                    background-color: #3c8dbc !important;\
                    color: #fff !important;\
                }\
            </style>'
});