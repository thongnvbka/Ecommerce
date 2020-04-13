var orderPopupModel = function () {
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
        $("#" + id).fadeOut(300, function () {
            $("#over").remove();
        });
        return false;
    }
    //show dialog
    self.showMessagerDeposit = function (id) {
        $("#hfOrderId").val(id);
        $.ajax({
            url: "/" + window.culture + "/CMS/Order/GetMoneyLevel",
            type: 'POST',
            data: { "orderId": id },
            async: false,
            success: function (data) {
                $('#deposit-level').html("(" + data.TotalLevel + " baht)");
                $('#deposit-full').html("(" + data.Total + " baht)");
                $('#deposit-level-percent').html("(" + data.Percent + " %)");
            },
            beforeSend: function () {
                ShowLoading();
            },
            complete: function () {
                HideLoading();
            }
        });
        self.showDialog('dialog_confirm_deposit');
        return false;
    }
    self.showMessagerSuccess = function () {
        $("#mess_success").text("เกิดปัญหาในการลบฐานข้อมูลกรุณาลองใหม่!");
        self.showDialog('dialog_success');
        return false;
    }
    self.showMessagerCancel = function (id) {
        console.log(id);
        $("#mess_cancel").text("คุณอยากยกเลิกออเดอร์นี้จริงๆหรอไม!");
        $("#hfOrderId").val(id);
        self.showDialog('dialog_cancel');
        return false;
    }
    self.cancelOrderConfig = function () {
        $.ajax({
            url: "/" + window.culture + "/CMS/Order/UpdateCancelOrder",
            type: 'POST',
            data: { "orderId": $("#hfOrderId").val() },
            success: function (data) {
                if (data > 0) {
                    if (typeof self.parent.GetAll === "function") {
                        self.closeDialog('dialog_cancel');
                        self.parent.GetAll();
                    }
                    else {
                        location.reload();
                    }
                }
                if (data == -1) {
                    self.showMessagerSuccess();;
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
    self.depositConfig = function () {
        $.ajax({
            url: "/" + window.culture + "/CMS/Order/ConfirmUpdateBalance",
            type: 'POST',
            data: { "orderId": $("#hfOrderId").val(), 'type': $('#hfTypeDeposit').val() },
            success: function (data) {
                
                self.closeDialog('dialog_confirm_deposit');
                if (data > 0) {
                    self.showDialog('dialog_deposit_ok');

                }
                else {
                    self.showDialog('dialog_deposit_not_ok');
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
    self.updateBalanceConfig = function () {
        $.ajax({
            url: "/" + window.culture + "/CMS/Order/UpdateBalance",
            type: 'POST',
            data: { "orderId": $("#hfOrderId").val(), 'pass': $('#dialog_deposit_ok #Password').val(), 'type': $('#hfTypeDeposit').val() },
            success: function (data) {
                if (data == 1) {
                    self.closeDialog('dialog_deposit_ok');
                    $('#mess_deposit').html("มัดจำสำเร็จ");
                    self.showDialog('dialog_deposit_not_ok');
                    if (typeof self.parent.GetAll === "function") {
                        self.closeDialog('dialog_cancel');
                        self.parent.GetAll();
                    }
                    else {
                        location.reload();
                    }
                }
                else {
                    if (data == -1) {
                        $('#dialog_deposit_ok .field-validation-valid').addClass('field-validation-error').removeClass('field-validation-valid');
                        $('#dialog_deposit_ok .field-validation-error').html('<span for="Password" class="">รหัสผ่านไม่ถูกต้อง</span>');
                    }
                    else {
                        self.closeDialog('dialog_deposit_ok');
                        $('#mess_deposit').html("เกิดปัญหาในการลบฐานข้อมูลกรุณาลองใหม่.");
                        self.showDialog('dialog_deposit_not_ok');
                    }
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

    $(document).ready(function () {
        $('#dialog_confirm_deposit .select-deposit').change(function () {
            $('#hfTypeDeposit').val($(this).val());
        })
        
    })

}