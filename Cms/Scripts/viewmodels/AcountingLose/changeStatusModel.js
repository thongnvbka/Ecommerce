/**
 * Cập nhật trạng thái của xử Tally wrong của Orders
 * @param {function} callback : Callback sau khi cập nhật thành công
 * @returns {} 
 */
function ChangeStatusModel(callback) {
    var self = this;

    self.isLoading = ko.observable(false);
    self.noteProcess = ko.observable("");
    self.status = ko.observable(null);
    self.orderId = ko.observable(null);
    self.orderCode = ko.observable("");
    self.callback = callback;


    self.show = function(orderId, orderCode, status, noteProcess) {
        self.resetValue();
        self.orderId(orderId);
        self.orderCode(orderCode);
        self.status(status);
        self.noteProcess(noteProcess);

        $("#changeStatusModal").modal("show");
    }

    self.resetValue = function() {
        self.isLoading(false);
        self.noteProcess("");
        self.status(null);
        self.orderId(null);
        self.orderCode("");
    }

    self.hide = function() {
        self.resetValue();
        $("#changeStatusModal").modal("hide");
    }

    self.save = function() {
        if ($.trim(self.noteProcess()).length === 0 && (self.status() == 1 || self.status() == 2)) {
            toastr.warning("Settling the processing method is compulsory!");
        }

        self.isLoading(true);

        $.post("/AcountingLose/UpdateStatus",
            { orderId: self.orderId(), status: self.status(), note: self.noteProcess() },
            function(rs) {
                if (rs && rs.status < 0) {
                    toastr.warning(rs.text);
                    return;
                }
                
                toastr.success(rs.text);
                self.resetValue();
                $("#changeStatusModal").modal("hide");

                if (self.callback)
                    self.callback();
            });
    }
}