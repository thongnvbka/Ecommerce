var depositPopupModel = function () {
    var self = this;
    self.parent = null;
    //show dialog
    self.showDialog = function (id) {
        $('#' + id).fadeIn("slow");
        // thêm phần tử id="over" vào cuối thẻ body
        $('body').append('<div id="over"></div>');
        return false;
    }

    //show dialog
    self.closeDialog = function (id) {
        $('#' + id).fadeOut(300, function () {
            $('#over').remove();
        });
        $('.modal-content .modal-header .close, .modal-footer .btn-default').click(function () {
            $('#' + id).fadeOut(300, function () {
                $('#over').remove();
            });
            return false;
        })
        return false;
    }
    //show dialog
    self.showMessagerStop = function () {
        self.showDialog('dialog_stop_not_ok');
        return false;
    }
    self.showMessagerDeposit = function (id) {
        $("#hfOrderId").val(id);
        self.showDialog('dialog_confirm_deposit');
        return false;
    }
    self.showMessagerSuccess = function () {
        $("#mess_success").text("เกิดปัญหาในการลบฐานข้อมูลกรุณาลองใหม่ !");

        //$("#mess_success").text("Xảy ra lỗi trong quá trình xóa dữ liệu. Bạn vui lòng thử lại!");

        self.showDialog('dialog_success');
        return false;
    }
    self.showMessagerCancel = function (id, status, type) {
        if (type == 0) {
            $("#mess_cancel").text("คุณอยากยกเลิกออเดอร์นี้จริงๆหรอไม่!");
            //$("#mess_cancel").text("Bạn có chắc chắn muốn hủy đơn hàng đã chọn!");
            $('#hfTypeDeposit').val('0');
        }
        else {
            $("#mess_cancel").text("คุณอยากยกเลิกออเดอร์นี้จริงๆหรอไม่ !");
            //$("#mess_cancel").text("Bạn có chắc chắn muốn kết đơn ký gửi đã chọn!");
            $('#hfTypeDeposit').val('1');
        }
        $("#hfOrderId").val(id);
        $('#hfOrderStatus').val(status);
        self.showDialog('dialog_cancel');
        return false;
    }
    self.cancelOrderConfig = function () {
        var obj = {
            orderId: $('#hfOrderId').val(),
            status: parseInt($("#hfOrderStatus").val()),
            type: $('#hfTypeDeposit').val()
        };
        $.ajax({
            url: "/" + window.culture + "/CMS/Order/UpdateStatusDeposit",
            type: 'POST',
            data: obj,
            success: function (data) {
                if (data > 0) {
                    self.closeDialog('dialog_cancel');
                    if (typeof self.parent.GetAll === "function") {
                        self.closeDialog('dialog_cancel');
                        self.parent.GetAll();
                    }
                    else {
                        location.reload();
                    }
                }
                if (data == -1) {
                    self.showDialog('dialog_success');;
                }
            },
            beforeSend: function () {
                ShowLoading();
            },
            complete: function () {
                HideLoading();
            }
        });
    }
}