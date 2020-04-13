//================================================= HỖ TRỢ TRA CỨU THÔNG TIN KHÁCH HÀNG
function CustomerFindViewModel(modelId) {
    var self = this;
    self.titleCustomer = ko.observable();
    self.complainModel = ko.observable(new complainModel());
    self.customerModel = ko.observable(new customerOfStaffModel());
    self.complainDetailModel = ko.observable(new complainDetailModel());
    self.customerId = ko.observable();

    self.customerId.subscribe(function (newValue) {
        if (self.customerId() > 0 || self.customerId() != null || self.customerId() != undefined) {
            $.post("/Ticket/GetCustomerInfo", { customerId: self.customerId() }, function (data) {
                self.mapCustomerModel(data.customer);
            });
        }
    });

    //Hàm lấy thông tin khách hàng
    self.searchCustomer = function () {
        $(".customer-search")
            .select2({
                ajax: {
                    url: "Customer/GetCustomerSearch",
                    dataType: 'json',
                    delay: 250,
                    data: function (params) {
                        return {
                            keyword: params.term,
                            page: params.page
                        };
                    },
                    processResults: function (data, params) {
                        params.page = params.page || 1;

                        return {
                            results: data.items,
                            pagination: {
                                more: (params.page * 10) < data.total_count
                            }
                        };
                    },
                    cache: true
                },
                escapeMarkup: function (markup) { return markup; },
                minimumInputLength: 1,
                templateResult: function (repo) {
                    if (repo.loading) return repo.text;
                    var markup = "<div class='select2-result-repository clearfix'>\
                                    <div class='pull-left'>\
                                        <img class='w-40 mr10 mt5' src='" + repo.avatar + "'/>\
                                    </div>\
                                    <div class='pull-left'>\
                                        <div>\
                                            <b>" + repo.text + "</b><br/>\
                                            <i class='fa fa-envelope-o'></i> " + repo.email + "<br/>\
                                            <i class='fa fa-phone'></i> " + repo.phone + "<br />\
                                            <i class='fa fa-globe'></i> " + repo.systemName + "<br />\
                                        </div>\
                                    </div>\
                                    <div class='clear-fix'></div>\
                                </div>";
                    return markup;
                },
                templateSelection: function (repo) {
                    return repo.text;
                },
                placeholder: "All",
                allowClear: true,
                language: 'en'
            });
    };

    self.active2 = ko.observable('customerDetail');
    self.templateId2 = ko.observable('customerDetail');
    //click menu tab detail  
    self.clickMenuDetail = function (name) {
        if (name !== self.active2()) {
            //self.init();
            self.active2(name);
            self.templateId2(name);
            if (name === 'customerDetail') {
                self.GetCustomerInfo();
            }

            if (name === 'OrderHistory') {
                self.OrderHistory();
            }

            if (name === 'SupportHistory') {
                self.SupportHistory();
            }

            if (name === 'OrderMoney') {
                self.OrderMoney();
            }
        }
    }

    // Account khách hàng
    self.GetCustomerInfo = function () {
        $.post("/Ticket/GetCustomerInfo", { customerId: self.customerId() }, function (data) {
            if (data.customer != null) {
                self.mapCustomerModel(data.customer);
            }

        });
    }
    ///SupportHistory
    self.listComplainByCustomer = ko.observable([]);
    self.listComplainCustomer = ko.observable([]);
    self.SupportHistory = function () {
        $.post("/Ticket/SupportHistory", { customerId: self.customerId() }, function (data) {
            self.listComplainCustomer(data.customer);
            self.listComplainByCustomer(data.customerinfo);
        });
    }
    ///OrderHistory
    self.listOrderByCustomer = ko.observable([]);

    self.OrderHistory = function () {
        $.post("/Ticket/OrderHistory", { customerId: self.customerId(), page: page, pageSize: pagesize }, function (data) {
            self.listOrderByCustomer(data);
        });
    }
    ///OrderMoney
    self.listOrderMoneyByCustomer = ko.observable([]);
    self.OrderMoney = function () {
        $.post("/Ticket/OrderMoney", { customerId: self.customerId(), page: page, pageSize: pagesize }, function (data) {
            self.listOrderMoneyByCustomer(data);
        });
    }

    // Object Detail khách hàng
    self.mapCustomerModel = function (data) {
        self.customerModel(new customerOfStaffModel());

        self.customerModel().Id(data.Id);
        self.customerModel().Email(data.Email);
        self.customerModel().FirstName(data.FirstName);
        self.customerModel().LastName(data.LastName);
        self.customerModel().MidleName(data.MidleName);
        self.customerModel().FullName(data.FullName);
        self.customerModel().Password(data.Password);
        self.customerModel().SystemId(data.SystemId);
        self.customerModel().SystemName(data.SystemName);
        self.customerModel().Phone(data.Phone);
        self.customerModel().Avatar(data.Avatar);
        self.customerModel().Nickname(data.Nickname);
        self.customerModel().LevelId(data.LevelId);
        self.customerModel().LevelName(data.LevelName);
        self.customerModel().Point(data.Point);
        self.customerModel().GenderId(data.GenderId);
        self.customerModel().GenderName(data.GenderName);
        self.customerModel().DistrictId(data.DistrictId);
        self.customerModel().DistrictName(data.DistrictName);
        self.customerModel().ProvinceId(data.ProvinceId);
        self.customerModel().ProvinceName(data.ProvinceName);
        self.customerModel().WardId(data.WardId);
        self.customerModel().WardsName(data.WardsName);
        self.customerModel().Address(data.Address);
        self.customerModel().UserId(data.UserId);
        self.customerModel().UserFullName(data.UserFullName);
        self.customerModel().Created(data.Created);
        self.customerModel().Updated(data.Updated);
        self.customerModel().LastLockoutDate(data.LastLockoutDate);
        self.customerModel().LockoutToDate(data.LockoutToDate);
        self.customerModel().FirstLoginFailureDate(data.FirstLoginFailureDate);
        self.customerModel().LoginFailureCount(data.LoginFailureCount);
        self.customerModel().HashTag(data.HashTag);
        self.customerModel().Balance(data.Balance);
        self.customerModel().BalanceAvalible(data.BalanceAvalible);
        self.customerModel().IsActive(data.IsActive);
        self.customerModel().IsLockout(data.IsLockout);
        self.customerModel().CodeActive(data.CodeActive);
        self.customerModel().CreateDateActive(data.CreateDateActive);
        self.customerModel().DateActive(data.DateActive);
        self.customerModel().CountryId(data.CountryId);
        self.customerModel().Code(data.Code);
        self.customerModel().Status(data.Status);
        self.customerModel().IsDelete(data.IsDelete);

    };
}