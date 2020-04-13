function UserModel(userId, userName, userFullName, isNotity) {
    var self = this;

    self.userId = ko.observable(userId);
    self.userFullName = ko.observable(userFullName);
    self.userName = ko.observable(userName);
    self.isNotify = ko.observable(isNotity);

    self.elementId = null;
    self.getElementId = function () {
        if (self.elementId)
            return self.elementId;

        self.elementId = "elementId_" + Math.round(new Date().getTime() + (Math.random() * 100)) + "_";

        return self.elementId;
    }

    self.addUser = function (user) {
        if (user) {
            self.userId(user.id);
            self.userName(user.userName);
            self.userFullName(user.fullName);
            return;
        }

        self.userId(0);
        self.userName("");
        self.userFullName("");
    }
    
    self.changeIsNotify = function() {
        self.isNotify(!self.isNotify());
    }
}

function SettingModel() {
    var self = this;

    self.isSubmit = ko.observable(false);
    self.isLoading = ko.observable(false);
    self.mode = ko.observable(window.officeTypeOrder);
    self.isFollow = ko.observable(false);
    self.users = ko.observableArray([]);

    self.changeIsFollow = function () {
        self.isFollow(!self.isFollow());
    }

    self.changeTab = function (mode, isFirst) {
        if (isFirst == undefined && mode === self.mode())
            return;

        self.isLoading(true);

        self.mode(mode);
        $.get("/Setting/GetSetting",
            { officeType: self.mode() },
            function(data) {
                self.isLoading(false);
                self.isFollow(data.isFollow);

                var users = [];
                _.each(data.users,
                    function(u) {
                        users.push(new UserModel(u.userId, u.userName, u.userFullName, u.isNotify));
                    });

                self.users(users);
            });
    }

    self.addUser = function () {
        self.users
            .unshift(new UserModel(0, "", "", true));
    }

    self.removeUser = function(user) {
        self.users.remove(user);
    }

    self.getUsers = function() {
        return _.map(self.users(),
            function(u) {
                return { userId: u.userId(), userName: u.userName(), userFullName: u.userFullName(), isNotify: u.isNotify() };
            });
    }

    self.save = function () {

        var data = {
            isFollow: self.isFollow(),
            officeType: self.mode(),
            users: self.getUsers()
        };

        data["__RequestVerificationToken"] = self.token;

        self.isSubmit(true);
        $.post("/Setting/SaveNotifySetting",
            data,
            function(rs) {
                self.isSubmit(false);
                if (rs && rs.status <= 0) {
                    toastr.warning(rs.text);
                    return;
                }

                toastr.success(rs.text);
            });
    }

    self.token = $("#SettingToBindings input[name='__RequestVerificationToken']").val();

    $(function () {
        self.changeTab(self.mode(), true);
    });
}

var modelView = new SettingModel();
ko.applyBindings(modelView, $("#SettingToBindings")[0]);