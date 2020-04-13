var AccountViewModel = function () {
    var self = this;
    //Todo Khai báo
    self.customer = ko.observable();
    self.orderAwait = ko.observable();
    self.orderNew = ko.observable();
    self.notification = ko.observable();


    self.isRending = ko.observable(false);

    self.customerModel = ko.observable(new CustomerModel());
    self.GetCustomer = function () {
        self.isRending(false);

        self.customerModel(new CustomerModel());
        //self.notificationModel(new NotifiCommonModel());

        $.post("/" + window.culture + "/CMS/AccountCMS/SelectInforCustomer",
            function (result) {
                if (!result.status) {
                    toastr.error(result.msg);
                } else {
                    self.mapCustomerModel(result.userDetail);
                    self.orderNew(result.orderNew);
                    self.orderAwait(result.orderAwait);
                    self.notification(result.notification);

                    self.isRending(true);
                }
            });
    };

    // Object chi tiết khách hàng
    self.mapCustomerModel = function (data) {
        self.customerModel(new CustomerModel());

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
        self.customerModel().CardName(data.CardName);
        self.customerModel().CardId(data.CardId);
        self.customerModel().CardBank(data.CardBank);
        self.customerModel().CardBranch(data.CardBranch);
        self.customerModel().WarehouseId(data.WarehouseId);
        self.customerModel().WarehouseName(data.WarehouseName);
        self.customerModel().DepositPrice(data.DepositPrice);
    };

    //Todo khai bao bien model show du lieu tren view cua bang notification
    self.notificationModel = ko.observable(new NotifiCommonModel());
    //Todo==================== Object Map dữ liệu trả về View =========================================
    //Todo Object chi tiết notificommon
    self.mapnotificationModel = function (data) {
        self.notificationModel(new NotifiCommonModel());

        self.notificationModel().Id(data.Id);
        self.notificationModel().SystemId(data.SystemId);
        self.notificationModel().SystemName(data.SystemName);
        self.notificationModel().Description(data.Description);
        self.notificationModel().CreateDate(data.CreateDate);
        self.notificationModel().UpdateDate(data.UpdateDate);
        self.notificationModel().IsRead(data.IsRead);
        self.notificationModel().Title(data.Title);
        self.notificationModel().UserId(data.UserId);
        self.notificationModel().UserName(data.UserName);
        self.notificationModel().Url(data.Url);
        self.notificationModel().Status(data.Status);
        self.notificationModel().PublishDate(data.PublishDate);
        self.notificationModel().ImagePath(data.ImagePath);
    };

    $(function () {
        self.GetCustomer();
    });
}

var viewModeAccount = new AccountViewModel();
ko.applyBindings(viewModeAccount);