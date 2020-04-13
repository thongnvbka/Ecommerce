ko.components.register("suggetion-staff-input-component", {
    viewModel: function (params) {
        var self = this;

        self.userId = ko.observable(undefined);
        self.userName = params.userName;
        self.fullName = params.fullName;
        self.prefix = ko.observable(params.prefix === undefined ? "prefix_" : params.prefix);
        self.text = ko.observable(params.text === undefined ? "Staff: " : params.text);
        self.callback = params.callback;

        self.currentPage = ko.observable(1);
        self.recordPerPage = ko.observable(10);
        self.totalRecord = ko.observable(0);
        self.totalPage = ko.observable(0);

        self.keyword = ko.observable("");
        self.fcKeyWord = ko.observable(false);
        self.listUserSelected = ko.observableArray([]);
        self.listUserSearch = ko.observableArray([]);
        self.listUserRecent = ko.observableArray([]);
        self.isLoadingRecent = ko.observable(false);
        self.isActiveSearchTab = ko.observable(true);
        self.isLoadingSearch = ko.observable(false);

        self.position = function () {
            $("#" + self.prefix() + "formChooseUserShipper").position({ my: "center top", at: "center top", of: $("#" + self.prefix() + "userShipperInput"), collision: "flipfit" });
        }

        self.showSelectCreator = function () {
            self.getRecentUser();
            $("#" + self.prefix() + "formChooseUserShipper").css("display", "block");

            self.position();
            self.fcKeyWord(true);
        }

        self.clearValue = function () {
            self.userId(undefined);
            self.userName(undefined);
            self.fullName(undefined);
            self.callback(undefined);
        }

        self.getRecentUser = function () {
            self.isActiveSearchTab(false);

            if (self.listUserRecent().length === 0) {
                self.isLoadingRecent(true);

                $.post("/User/SuggettionRecentSearch", function (result) {
                    self.isLoadingRecent(false);

                if (!$("#" + self.prefix() + "creatorTab a[href='#" + self.prefix() + "recentCreatorSearch']").parent().hasClass("active"))
                    $("#" + self.prefix() + "creatorTab a[href='#" + self.prefix() + "recentCreatorSearch']").tab("show");

                    if (result != undefined) {
                        var arr = [];
                        _.each(result, function (item) {
                            item.isSelect = ko.observable(false);

                            arr.push(item);
                        });

                        self.listUserRecent(arr);
                    }
                });
            } else {
                _.each(self.listUserRecent(), function (item) {
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
                self.searchUser(1);
            }, 500);
        });

        self.searchUser = function (pageIndex) {
            self.isActiveSearchTab(true);
            self.isLoadingSearch(true);
            self.currentPage(pageIndex);

            $.post("/User/SuggettionSearch", {
                keyword: self.keyword(),
                page: self.currentPage(),
                pageSize: self.recordPerPage()
            }, function (result) {
                self.isLoadingSearch(false);

                if (!$("#" + self.prefix() + "creatorTab a[href='#" + self.prefix() + "searchCreatorSearch']").parent().hasClass("active"))
                    $("#" + self.prefix() + "creatorTab a[href='#" + self.prefix() + "searchCreatorSearch']").tab("show");

                if (result != undefined) {
                    var arr = [];
                    var isSelected = false;
                    _.each(result.items, function (item) {
                        var exists = _.find(self.listUserSelected(), function (selected) {
                            return selected.UserId === item.ID;
                        });

                        isSelected = exists == undefined ? false : true;

                        item.isSelect = ko.observable(isSelected);

                        arr.push(item);
                        //arr.push({
                        //    userId: item.id,
                        //    FullName: item.fullName,
                        //    OfficeId: item.officeId,
                        //    OfficeName: item.officeName,
                        //    OfficeIdPath: item.fficeIdPath,
                        //    TitleId: item.TitleId,
                        //    TitleName: item.TitleName,
                        //    IsSelect: ko.observable(isSelected),
                        //    IsLeader: item.IsLeader
                        //});
                    });
                    self.listUserSearch(arr);

                    self.totalRecord(result.totalRecord);
                    self.genPager();

                    self.position();
                }
            });
        }

        self.genPager = function () {
            self.totalPage(Math.ceil(self.totalRecord() / self.recordPerPage()));
            $("#" + self.prefix() + "pagerUser").pager({ pagenumber: self.currentPage(), pagecount: self.totalPage(), totalrecords: self.totalRecord(), buttonClickCallback: self.pageClickSearch });
        }

        self.pageClickSearch = function (pageclickednumber) {
            $("#" + self.prefix() + "pagerUser").pager({ pagenumber: pageclickednumber, pagecount: self.totalPage(), buttonClickCallback: self.pageClickSearch });
            self.searchUser(pageclickednumber);
        };

        self.hiddenForm = function () {
            if (!$("#" + self.prefix() + "creatorTab a[href='#" + self.prefix() + "recentCreatorSearch']").parent().hasClass("active"))
                $("#" + self.prefix() + "creatorTab a[href='#" + self.prefix() + "recentCreatorSearch']").tab("show");

            $("#" + self.prefix() + "formChooseUserShipper").fadeOut();
            self.listUserSearch.removeAll();
            self.keyword("");
        }

        self.selectCreator = function (data) {
            self.userId(data.id);
            self.userName(data.userName);
            self.fullName(data.fullName);

            self.callback(data);

            self.hiddenForm();
            $.post("/User/SuggettionRecentSave", {
                userId: data.id
            });
        }
        $(document).click(function (e) {
            if ($("#" + self.prefix() + "userShipperInput").is(":focus") || $(e.target).parents("#" + self.prefix() + "formChooseUserShipper").length > 0
                || $(e.target).parents(".pagination").length > 0 || $("#" + self.prefix() + "formChooseUserShipper input").is(":focus") || $(e.toElement).hasClass("jstree-icon jstree-ocl"))
                return;

            self.hiddenForm();
        });
    },
    template: //'<a href="javascript://" class="editable" data-bind="click: showSelectCreator, text: text, attr: {id: prefix() + \'userShipperInput\'}"></a>\
    '<div style="position: relative;"><input type="text" readonly class="form-control" data-bind="click: showSelectCreator, value: fullName()  && userName() ?  fullName() + \' (\' + userName()+ \')\' : \'\', attr: {id: prefix() + \'userShipperInput\', placeholder: text}" />\
                <a href="javascript:;" style="position: absolute; right: 8px; top: 8px; color: rgb(0, 0, 0); display: none;" data-bind="click: clearValue, visible: userName"><i class="fa fa-times" style="font-size:15px"></i></a></div>\
                <div class="saleCall wrapper-user-search wrapper-user-single-search"  data-bind="attr: {id:  prefix() + \'formChooseUserShipper\'}">\
                    <div class="block heightNone">\
                        <input type="text" class="form-control pr-mgb-10" placeholder="Select Shipper" value="" data-bind="value: keyword, attr: {placeholder: text}, hasFocus: fcKeyWord, valueUpdate: \'afterkeydown\'" />\
                        <!-- Nav tabs -->\
                        <ul class="nav nav-tabs tab-bricky mt10" role="tablist" data-bind="attr: {id:  prefix() + \'creatorTab\'}">\
                            <li role="presentation" class="active"><a data-bind="attr: {href: \'#\'+ prefix() + \'recentCreatorSearch\', \'aria-controls\': prefix() + \'recentCreatorSearch\'}" role="tab" data-toggle="tab">Recent</a></li>\
                            <li role="presentation"><a data-bind="attr: {href: \'#\'+ prefix() + \'searchCreatorSearch\', \'aria-controls\': prefix() + \'searchCreatorSearch\'}" role="tab" data-toggle="tab">Search</a></li>\
                        </ul>\
                        <!-- Tab panes -->\
                        <div class="tab-content pb5 pt5">\
                            <div role="tabpanel" class="tab-pane active" data-bind="attr: {id:  prefix() + \'recentCreatorSearch\'}">\
                                <!-- ko if: !isLoadingRecent() -->\
                                <!-- ko if: listUserRecent().length === 0 -->\
                                    <div>No data shipper</div>\
                                <!-- /ko -->\
                                <div data-bind="visible: listUserRecent().length > 0">\
                                    <div data-bind="foreach: listUserRecent">\
                                        <div class="user-search-item">\
                                            <a href="javascript:;" class="size24 isCheckbox" data-bind="click: $parent.selectCreator"><i class="clip-user-2"></i> </a>\
                                            <a href="javascript:;" data-bind="click: $parent.selectCreator">\
                                                <span data-bind="text: fullName"></span>\
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
                            <div role="tabpanel" class="tab-pane" data-bind="attr: {id:  prefix() + \'companyCreatorSearch\'}">\
                                <div data-bind="attr: {id:  prefix() + \'userSearchOffices\'}"></div>\
                            </div>\
                            <div role="tabpanel" class="tab-pane" data-bind="attr: {id:  prefix() + \'searchCreatorSearch\'}">\
                                <!-- ko if: !isLoadingSearch() -->\
                                    <!-- ko if: listUserSearch().length === 0 && keyword().length === 0 -->\
                                        <div>Please enter search keywords</div>\
                                    <!-- /ko -->\
                                    <!-- ko if: listUserSearch().length === 0  && keyword().length > 0 -->\
                                        <div>Không tìm thấy nhân viên với từ khóa "<b data-bind="text: keyword"></b>"</div>\
                                    <!-- /ko -->\
                                <div data-bind="foreach: listUserSearch">\
                                    <div class="user-search-item">\
                                        <a href="javascript:;" class="size24 isCheckbox" data-bind="click: $parent.selectCreator"><i class="clip-user-2"></i> </a>\
                                        <a href="javascript:;" data-bind="click: $parent.selectCreator">\
                                            <span data-bind="text: fullName"></span>\
                                        </a>\
                                    </div>\
                                </div>\
                                <!-- /ko -->\
                                <div data-bind="visible: isLoadingSearch()" style="text-align: center;">\
                                    <i class="fa fa-spinner fa-pulse"></i>\
                                </div>\
                                <div class="pull-right" data-bind="attr: {id:prefix() + \'pagerUser\'}, visible: !isLoadingSearch() && isActiveSearchTab() && listUserSearch().length > 0"></div>\
                                <div class="clear"></div>\
                            </div>\
                        </div>\
                    </div>\
                </div><!-- end wrapper recent search -->'
});