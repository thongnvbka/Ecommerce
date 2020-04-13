ko.components.register("suggetion-order-input-components", {
    viewModel: function (params) {
        var self = this;

        self.orderId = ko.observable(undefined);
        self.orderCode = ko.observable("");
        self.prefix = ko.observable(params.value.prefix === undefined ? "prefix_" : params.value.prefix);
        self.text = ko.observable(params.value.text === undefined ? "Orders... " : params.value.text);
        self.callback = params.value.callback;
        self.url = params.value.url ? params.value.url : "/order/Suggetion";

        self.currentPage = ko.observable(1);
        self.recordPerPage = ko.observable(params.value.pageSize ? params.value.pageSize : 10);
        self.totalRecord = ko.observable(0);
        self.totalPage = ko.observable(0);

        self.keyword = ko.observable("");
        self.fcKeyWord = ko.observable(false);
        self.listOrderSelected = ko.observableArray([]);
        self.listOrderSearch = ko.observableArray([]);
        self.listOrderRecent = ko.observableArray([]);
        self.isLoadingRecent = ko.observable(false);
        self.isActiveSearchTab = ko.observable(true);
        self.isLoadingSearch = ko.observable(false);

        self.position = function () {
            $("#" + self.prefix() + "addPackageChooseOrder").position({ my: "center top", at: "center top", of: $("#" + self.prefix() + "orderInput"), collision: "flipfit" });
        }

        self.showSelectCreator = function () {
            self.getRecentOrder();
            $("#" + self.prefix() + "addPackageChooseOrder").css("display", "block");

            self.position();
            self.fcKeyWord(true);
        }

        self.clearValue = function () {
            self.orderId(undefined);
            self.orderCode(undefined);
            self.callback(undefined);
        }

        self.getRecentOrder = function () {
            self.isActiveSearchTab(false);

            if (self.listOrderRecent().length === 0) {
                self.isLoadingRecent(true);

                $.post("/order/recentsuggettion", function (result) {
                    self.isLoadingRecent(false);

                if (!$("#" + self.prefix() + "creatorTab a[href='#" + self.prefix() + "recentCreatorSearch']").parent().hasClass("active"))
                    $("#" + self.prefix() + "creatorTab a[href='#" + self.prefix() + "recentCreatorSearch']").tab("show");

                    if (result != undefined) {
                        var arr = [];
                        _.each(result, function (item) {
                            item.isSelect = ko.observable(false);
                            arr.push(item);
                        });

                        self.listOrderRecent(arr);
                    }
                });
            } else {
                _.each(self.listOrderRecent(), function (item) {
                    item.isSelect(false);
                });
            }
        }

        var timer;
        self.keyword.subscribe(function (newValue) {
            if (newValue === "")
                return;

            window.clearTimeout(timer);

            timer = setTimeout(function () {
                self.searchOrder(1);
            }, 500);
        });

        self.searchOrder = function (pageIndex) {
            self.isActiveSearchTab(true);
            self.isLoadingSearch(true);
            self.currentPage(pageIndex);

            $.post(self.url, {
                term: self.keyword(),
                page: self.currentPage(),
                pageSize: self.recordPerPage()
            }, function (result) {
                self.isLoadingSearch(false);

                if (!$("#" + self.prefix() + "creatorTab a[href='#" + self.prefix() + "searchOrderSearch']").parent().hasClass("active"))
                    $("#" + self.prefix() + "creatorTab a[href='#" + self.prefix() + "searchOrderSearch']").tab("show");

                if (result != undefined) {
                    var arr = [];

                    _.each(result.items, function (item) {
                        var exists = _.find(self.listOrderSelected(), function (selected) {
                            return selected.id === item.id;
                        });

                        item.isSelected = ko.observable(exists == undefined ? false : true);
                        arr.push(item);
                    });
                    self.listOrderSearch(arr);

                    self.totalRecord(result.totalRecord);
                    self.genPager();

                    self.position();
                }
            });
        }

        self.genPager = function () {
            self.totalPage(Math.ceil(self.totalRecord() / self.recordPerPage()));
            $("#" + self.prefix() + "pagerOrder").pager({ pagenumber: self.currentPage(), pagecount: self.totalPage(), totalrecords: self.totalRecord(), buttonClickCallback: self.pageClickSearch });
        }

        self.pageClickSearch = function (pageclickednumber) {
            $("#" + self.prefix() + "pagerOrder").pager({ pagenumber: pageclickednumber, pagecount: self.totalPage(), buttonClickCallback: self.pageClickSearch });
            self.searchOrder(pageclickednumber);
        };

        self.hiddenForm = function () {
            if (!$("#" + self.prefix() + "creatorTab a[href='#" + self.prefix() + "recentCreatorSearch']").parent().hasClass("active"))
                $("#" + self.prefix() + "creatorTab a[href='#" + self.prefix() + "recentCreatorSearch']").tab("show");

            $("#" + self.prefix() + "addPackageChooseOrder").fadeOut();
            self.listOrderSearch.removeAll();
            self.keyword("");
        }

        self.selectOrder = function (data) {
            self.orderId(data.id);
            self.orderCode( ReturnCode(data.code) + " - " + data.customerName + "(" + data.customerEmail + ")");

            self.callback(data);

            self.hiddenForm();
            $.post("/order/savesuggetionrecent", {
                orderId: data.id
            });
        }

        $(document).click(function (e) {
            if ($("#" + self.prefix() + "orderInput").is(":focus") || $(e.target).parents("#" + self.prefix() + "addPackageChooseOrder").length > 0
                || $(e.target).parents(".pagination").length > 0 || $("#" + self.prefix() + "addPackageChooseOrder input").is(":focus") || $(e.toElement).hasClass("jstree-icon jstree-ocl"))
                return;

            self.hiddenForm();
        });
    },
    template: //'<a href="javascript://" class="editable" data-bind="click: showSelectCreator, text: text, attr: {id: prefix() + \'orderInput\'}"></a>\
                '<input type="text" readonly class="form-control" data-bind="click: showSelectCreator, value: orderCode, attr: {id: prefix() + \'orderInput\', placeholder: text}" />\
                <a href="javascript:;" style="position: absolute; right: 25px; top: 10px; color: rgb(0, 0, 0); display: none;" data-bind="click: clearValue, visible: orderCode"><i class="fa fa-times" style="font-size:15px"></i></a>\
                <div class="saleCall wrapper-user-search wrapper-user-single-search"  data-bind="attr: {id:  prefix() + \'addPackageChooseOrder\'}">\
                    <div class="block heightNone">\
                        <input type="text" class="form-control pr-mgb-10" placeholder="Tìm kiếm" value="" data-bind="value: keyword, hasFocus: fcKeyWord, valueUpdate: \'afterkeydown\'" />\
                        <!-- Nav tabs -->\
                        <ul class="nav nav-tabs tab-bricky mt10" role="tablist" data-bind="attr: {id:  prefix() + \'creatorTab\'}">\
                            <li role="presentation" class="active"><a data-bind="attr: {href: \'#\'+ prefix() + \'recentCreatorSearch\', \'aria-controls\': prefix() + \'recentCreatorSearch\'}" role="tab" data-toggle="tab">Gần đây</a></li>\
                            <li role="presentation"><a data-bind="attr: {href: \'#\'+ prefix() + \'searchOrderSearch\', \'aria-controls\': prefix() + \'searchOrderSearch\'}" role="tab" data-toggle="tab">Search</a></li>\
                        </ul>\
                        <!-- Tab panes -->\
                        <div class="tab-content pb5 pt5">\
                            <div role="tabpanel" class="tab-pane active" data-bind="attr: {id:  prefix() + \'recentCreatorSearch\'}">\
                                <!-- ko if: !isLoadingRecent() -->\
                                <!-- ko if: listOrderRecent().length === 0 -->\
                                    <div>Không có Orders gần đây</div>\
                                <!-- /ko -->\
                                <div data-bind="visible: listOrderRecent().length > 0">\
                                    <div data-bind="foreach: listOrderRecent">\
                                        <div class="user-search-item">\
                                            <a href="javascript:;" class="size24 isCheckbox" data-bind="click: $parent.selectOrder"><i class="clip-cart"></i> </a>\
                                             <a href="javascript:;" data-bind="click: $parent.selectOrder">\
                                            <span data-bind="text: window.ReturnCode(code)"></span> - <span data-bind="text: customerName"></span>(<span data-bind="text: customerEmail"></span>)\
                                            </a>\
                                        </div>\
                                    </div>\
                                </div>\
                                <!-- /ko -->\
                                <div data-bind="visible: isLoadingRecent()" style="text-align: center;">\
                                    <i class="fa fa-spinner fa-pulse"></i>\
                                </div>\
                                <div class="clear"></div>\
                            </div>\
                            <div role="tabpanel" class="tab-pane" data-bind="attr: {id:  prefix() + \'searchOrderSearch\'}">\
                                <!-- ko if: !isLoadingSearch() -->\
                                    <!-- ko if: listOrderSearch().length === 0 && keyword().length === 0 -->\
                                        <div>Please enter search keyword</div>\
                                    <!-- /ko -->\
                                    <!-- ko if: listOrderSearch().length === 0  && keyword().length > 0 -->\
                                        <div>Không tìm thấy Orders nào với từ khóa"<b data-bind="text: keyword"></b>"</div>\
                                    <!-- /ko -->\
                                <div data-bind="foreach: listOrderSearch">\
                                    <div class="user-search-item">\
                                        <a href="javascript:;" class="size24 isCheckbox" data-bind="click: $parent.selectOrder"><i class="clip-cart"></i> </a>\
                                        <a href="javascript:;" data-bind="click: $parent.selectOrder">\
                                            <span data-bind="text: window.ReturnCode(code)"></span> - <span data-bind="text: customerName"></span>(<span data-bind="text: customerEmail"></span>)\
                                        </a>\
                                    </div>\
                                </div>\
                                <!-- /ko -->\
                                <div data-bind="visible: isLoadingSearch()" style="text-align: center;">\
                                    <i class="fa fa-spinner fa-pulse"></i>\
                                </div>\
                                <div class="pull-right" data-bind="attr: {id:prefix() + \'pagerOrder\'}, visible: !isLoadingSearch() && isActiveSearchTab() && listOrderSearch().length > 0"></div>\
                                <div class="clear"></div>\
                            </div>\
                        </div>\
                    </div>\
                </div><!-- end wrapper recent search -->'
});