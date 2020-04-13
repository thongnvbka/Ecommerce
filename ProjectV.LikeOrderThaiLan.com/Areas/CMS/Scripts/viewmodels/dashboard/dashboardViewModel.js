var dashboardViewModel = function () {
    var self = this;
    self.active = ko.observable('');
    self.templateId = ko.observable('report-index');
    self.isRending = ko.observable(true);
    //Todo khai bao bien model show du lieu tren view cua bang dashboard
    self.dashboardModel = ko.observable(new dashboardModel());
    //Todo Object chi tiết dashboard
    self.mapdashboardModel = function (data) {
        self.dashboardModel(new dashboardModel());
        self.dashboardModel().CountComplate(data.CountComplate);
        self.dashboardModel().CountDeposit(data.CountDeposit);
        self.dashboardModel().CountOrder(data.CountOrder);
        self.dashboardModel().CountSource(data.CountSource);
        self.isRending(true);
    };
    self.viewDashboardModal = function (type) {
        self.isRending(true);
        $.ajax({
            type: 'GET',
            url: "/"+ window.culture+"/AccountCMS/CustomerReport",
            data: {typeSearch : type},
            success: function (response) {
                self.mapdashboardModel(response);
                
            },
            async: false
        });
        $(".nav-tabs li").each(function(idx) {
            if (idx === type) {
                $(this).find("a").addClass("active");
            } else {
                $(this).find("a").removeClass("active");
            }
        });
        self.isRending(true);
    }
    
    $(function () {
        self.viewDashboardModal(0);
        self.isRending(true);

    });
}

var dashboardModelView = new dashboardViewModel();

ko.applyBindings(dashboardModelView, $('#tabDash')[0]);