var CreateDepositViewModel = function () {
    var self = this;
    self.active = ko.observable('');
    self.listDepositDetail = ko.observableArray([]);
    self.checkDuplicateLink = ko.observable(false);
    self.isSubmit = ko.observable(true);
    self.listWard = ko.observableArray([]);
    self.listWardDelivery = ko.observableArray([]);
    self.CategoryId = ko.observable('');
    self.WardId = ko.observable('');
    self.WardDeliveryId = ko.observable('');
    self.listService = ko.observableArray([]);

    //hàm khởi tạo dữ liệu
    $(function () {
        self.listDepositDetail.push(self.addDetail());
        self.listWard(listWard);
        self.listWardDelivery(listWardDelivery);
        self.WardDeliveryId(window.warehouseId);
        $('.table-product tbody tr').each(function () {
            $(this).find('.dropdownjstree').remove();
            $(this).find(".category_tree_deposit").dropdownjstree({
                source: window.listcategoryJsTree,
                selectedNode: "",
                selectNote: (node, selected) => {
                    self.CategoryId(selected.node.id);
                }
            });
        })
        self.listService([]);
        self.listService(window.listService);
        self.initInputMark();
    });


    //Cập nhật thông tin phí dịch vụ
    self.updateService = function (data) {
        var list = _.filter(self.listService(),
            function (item) {
                if (item.ServiceId === data.Checked) {
                    item.Checked = !item.Checked;
                }
                return item;
            });
        self.listService([]);
        self.listService(list);
        console.log(self.listService());
    }

    var maxFileLength = 5120000;;
    self.addImage = function (data1) {
        $(".flieuploadImg").fileupload({
            url: "/" + window.culture + "/CMS/Ticket/UploadImages",
            sequentialUploads: true,
            dataType: "json",
            add: function (e, data) {
                var file = data.files[0];
                var msg = "";
                if (maxFileLength && file.size > maxFileLength) {
                    if (msg) {
                        msg += "<br/>";
                    }
                    msg += file.name + window.messager.errorTicket.errorWidthImg;
                } else if (validateBlackListExtensions(file.name)) {
                    if (msg) {
                        msg += "<br/>";
                    }
                    msg += file.name + window.messager.errorTicket.errorImgNotFormat;
                }
                if (msg !== "") {
                    toastr.error(msg);
                } else {
                    data.submit();
                }
            },
            done: function (e, data) {
                if (data.result === -5) {
                    toastr.error(window.messager.errorTicket.errorFileError);
                    return;
                }

                data1.Image(window.location.origin + data.result[0].path);

                $("div").removeClass("hover");
            }
        });
        return true;
    }

    var validateBlackListExtensions = function (file) {
        var ext = file.substring(file.lastIndexOf(".")).toLowerCase();
        return !_.some([".jpg", ".jpeg", ".gif", ".png"], function (item) { return item === ext; });
    };

    self.addContractCode = function (id) {
        var contractCode = {
            Id: (new Date()).getTime(),
            ParentId: id,
            Code: ko.observable("")
        };
        return contractCode;
    }

    self.backToList = function () {
        window.location.href = "/" + window.culture + "/CMS/Order/DepositOrder";
    };


    //tạo chi tiết đơn hàng ký gửi
    self.addDetail = function () {
        var depositDetail = {
            Id: (new Date()).getTime(),
            ProductName: ko.observable(""),
            CategoryId: ko.observable(""),
            ContractCode: ko.observable(""),
            Image: ko.observable(""),
            Size: ko.observable("0x0x0"),
            High: ko.observable(""),
            Long: ko.observable(""),
            Wide: ko.observable(""),
            Price: ko.observable(""),
            Quantity: ko.observable(""),
            Weight: ko.observable(""),
            Ward: ko.observable(""),
            PacketNumber: ko.observable(""),
            Note: ko.observable(""),
            ShipTq: ko.observable(""),
            ListContractCode: ko.observableArray([])
        };
        depositDetail.Long.subscribe(function (newValue) {
            depositDetail.Size(newValue + "x" + depositDetail.Wide() + "x" + depositDetail.High());
        });
        depositDetail.Wide.subscribe(function (newValue) {
            depositDetail.Size(depositDetail.Long() + "x" + newValue + "x" + depositDetail.High());
        });
        depositDetail.High.subscribe(function (newValue) {
            depositDetail.Size(depositDetail.Long() + "x" + depositDetail.Wide() + "x" + newValue);
        });
        self.CategoryId.subscribe(function (newValue) {
            depositDetail.CategoryId(newValue);
        });


        depositDetail.ListContractCode.push(self.addContractCode(depositDetail.Id));

        //depositDetail.ImageCss = ko.observable("");
        //depositDetail.Image.subscribe(function (newValue) {
        //    if (newValue == undefined || newValue == null || newValue == '') {
        //        depositDetail.ImageCss("border: 1px solid red");
        //        toastr.error('Thêm link ảnh sản phẩm');
        //    } else {
        //        depositDetail.ImageCss("");
        //    }
        //});

        //depositDetail.ImageCss = ko.observable("");
        //depositDetail.ImageFocus = ko.observable(false);
        //depositDetail.Image.subscribe(function (newValue) {
        //    if (newValue == undefined || newValue == null || newValue == '') {
        //        depositDetail.ImageCss("error");
        //        depositDetail.ImageFocus(true);
        //        toastr.error('Nhập link sản phẩm');
        //    } else {
        //        var checkImage = _.filter(self.listDepositDetail(), function (item) {
        //            return (depositDetail.Image() === item.Image() && depositDetail.High() === item.High() && depositDetail.Long() === item.Long() && depositDetail.Wide() === item.Wide() && depositDetail.Id != item.Id);
        //        });

        //        if (checkImage.length > 0) {
        //            depositDetail.ImageCss("error");
        //            depositDetail.ImageFocus(true);

        //            toastr.error('Image, màu sắc, size sản phẩm bị trùng');
        //            self.checkDuplicateLink(true);
        //        } else {
        //            depositDetail.ImageCss("");
        //            depositDetail.ImageFocus(false);
        //            self.checkDuplicateLink(false);
        //        }
        //    }
        //});
        depositDetail.ProductNameCss = ko.observable("");
        depositDetail.ProductNameFocus = ko.observable(false);
        depositDetail.ProductName.subscribe(function (newValue) {
            if (newValue == undefined || newValue == null || newValue == '') {
                depositDetail.ProductNameCss("error");
                depositDetail.ProductNameFocus(true);
                toastr.error('กรุณากรอกชื่อสินค้า');
            } else {
                //var checkImage = _.filter(self.listDepositDetail(), function (item) {
                //    return (depositDetail.Id != item.Id && depositDetail.ProductName() == item.ProductName());
                //});
                //console.log(checkImage.length);
                //if (checkImage.length > 0) {
                //    depositDetail.ProductNameCss("error");
                //    depositDetail.ProductNameFocus(true);

                //    toastr.error('รูปภาพของสินค้าซ้ำกัน');
                //    self.checkDuplicateLink(true);
                //} else {
                //    depositDetail.ProductNameCss("");
                //    depositDetail.ProductNameFocus(false);
                //    self.checkDuplicateLink(false);
                //}
            }
        });

        depositDetail.PacketNumberCss = ko.observable("");
        depositDetail.PacketNumberFocus = ko.observable(false);
        depositDetail.PacketNumber.subscribe(function (newValue) {
            if (newValue == undefined || newValue == null || newValue == '' || newValue == 0) {
                depositDetail.PacketNumberCss("error");
                depositDetail.PacketNumberFocus(true);
                toastr.error('กรอกจำนวนลังสินค้า');
            } else {
                depositDetail.PacketNumberCss("");
                depositDetail.PacketNumberFocus(false);
            }
        });

        depositDetail.WeightCss = ko.observable("");
        depositDetail.WeightFocus = ko.observable(false);
        //depositDetail.Weight.subscribe(function (newValue) {
        //    if (newValue == undefined || newValue == null || newValue == '' || newValue == 0) {
        //        depositDetail.WeightCss("error");
        //        depositDetail.WeightFocus(true);
        //        toastr.error('Nhập cân nặng');
        //    } else {
        //        depositDetail.WeightCss("");
        //        depositDetail.PacketNumberFocus(false);
        //    }
        //});


        depositDetail.LongFocus = ko.observable(false);
        depositDetail.HighFocus = ko.observable(false);
        depositDetail.WideFocus = ko.observable(false);

        depositDetail.ShipTqCss = ko.observable("");
        depositDetail.ShipTqFocus = ko.observable(false);
        //depositDetail.ShipTq.subscribe(function (newValue) {
        //    if (newValue == undefined || newValue == null || newValue == '' || newValue == 0) {
        //        depositDetail.ShipTqCss("error");
        //        depositDetail.ShipTqFocus(true);
        //        toastr.error('Nhập ship Trung Quốc');
        //    } else {
        //        depositDetail.ShipTqCss("");
        //        depositDetail.PacketNumberFocus(false);
        //    }
        //});

        depositDetail.QuantityCss = ko.observable("");
        depositDetail.QuantityFocus = ko.observable(false);
        //depositDetail.Quantity.subscribe(function (newValue) {
        //    if (newValue == undefined || newValue == null || newValue == '' || newValue == 0) {
        //        depositDetail.QuantityCss("error");
        //        depositDetail.QuantityFocus(true);
        //        toastr.error('Nhập số lượng');
        //    } else {
        //        depositDetail.QuantityCss("");
        //        depositDetail.QuantityFocus(false);
        //    }
        //});

        return depositDetail;
    }

    //format số cho input
    self.initInputMark = function () {
        $('input.decimal').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize.culture().numberFormat['.'], autoGroup: true, groupSeparator: Globalize.culture().numberFormat[','], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });
        $('input.decimal').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize.culture().numberFormat['.'], autoGroup: true, groupSeparator: Globalize.culture().numberFormat[','], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });
    }

    //bắt lỗi nhập chi tiết đơn hàng
    self.checkOrderDetail = function () {
        var check = true;

        if (self.checkDuplicateLink()) {
            check = false;
            toastr.error('รูปภาพของสินค้าซ้ำกัน');
        }
        if (self.WardId() == undefined || self.WardId() == null || self.WardId() == -1) {
            toastr.error('เลือกโกดังส่งสินค้า');
            check = false;
            return false;
        }
        if (self.WardDeliveryId() == undefined || self.WardDeliveryId() == null || self.WardDeliveryId() == -1) {
            toastr.error('เลือกโกดังรับสินค้า');
            check = false;
            return false;
        }

        _.each(self.listDepositDetail(), function (depositDetail) {

            //if (depositDetail.Image() == undefined || depositDetail.Image() == null || depositDetail.Image() == '') {
            //    depositDetail.ImageCss("error");
            //    depositDetail.ImageFocus(true);
            //    toastr.error('Chọn ảnh sản phẩm');
            //    check = false;
            //    return false;
            //}

            if (depositDetail.ProductName() == undefined || depositDetail.ProductName() == null || depositDetail.ProductName() == '') {
                depositDetail.ProductNameCss("error");
                depositDetail.ProductNameFocus(true);
                toastr.error('กรุณากรอกชื่อสินค้า');
                check = false;
                return false;
            }

            if (depositDetail.CategoryId() == undefined || depositDetail.CategoryId() == null || depositDetail.CategoryId() == '' || depositDetail.CategoryId() == -1) {

                toastr.error('เลือกประเภทสินค้า');
                check = false;
                return false;
            }


            //if (depositDetail.Quantity() == undefined || depositDetail.Quantity() == null || depositDetail.Quantity() == '') {
            //    depositDetail.QuantityCss("error");
            //    depositDetail.QuantityFocus(true);
            //    toastr.error('Nhập số lượng');
            //    check = false;
            //    return false;
            //}
            //if (depositDetail.Weight() == undefined || depositDetail.Weight() == null || depositDetail.Weight() == '') {
            //    depositDetail.WeightCss("error");
            //    depositDetail.WeightFocus(true);
            //    toastr.error('Nhập cân nặng');
            //    check = false;
            //    return false;
            //}
            if (depositDetail.PacketNumber() == undefined || depositDetail.PacketNumber() == null || depositDetail.PacketNumber() == '') {
                depositDetail.PacketNumberCss("error");
                depositDetail.PacketNumberFocus(true);
                toastr.error('กรอกจำนวนลังสินค้า');
                check = false;
                return false;
            }

            //if (depositDetail.ShipTq() == undefined || depositDetail.ShipTq() == null || depositDetail.ShipTq() == '') {
            //    depositDetail.ShipTqCss("error");
            //    depositDetail.ShipTqFocus(true);
            //    toastr.error('Nhập ship Trung Quốc');
            //    check = false;
            //    return false;
            //}

            if (depositDetail.High() == undefined || depositDetail.High() == null || depositDetail.High() == '') {
                depositDetail.High(0);
            }
            if (depositDetail.Wide() == undefined || depositDetail.Wide() == null || depositDetail.Wide() == '') {
                depositDetail.Wide(0);
            }
            if (depositDetail.Long() == undefined || depositDetail.Long() == null || depositDetail.Long() == '') {
                depositDetail.Long(0);
            }

            //_.each(depositDetail.ListContractCode(), function (item) {

            //    if (item.Code() == "") {
            //        toastr.error('Nhập mã vận đơn');
            //        check = false;
            //        return false;
            //    }
            //})


        });
        return check;
    }

    //tạo editer thêm link hình ảnh
    self.renderedHandler = function (elements, data) {
        $('#editableBtn' + data.Id).click(function (e) {
            e.stopPropagation();
            $('#editableImg' + data.Id).editable('toggle');
        });
    }

    //thêm chi tiết đơn hàng trên view
    self.addDepositDetail = function () {
        if (self.checkOrderDetail()) {
            if (self.listDepositDetail().length >= 50) {
                toastr.error("ออเดอร์ไม่สามารถใส่เกิน 50 ลิงค์สินค้า !");
                //toastr.error('Đơn hàng không thể vượt quá 50 link sản phẩm!');
            } else {
                self.listDepositDetail.push(self.addDetail());
                self.initInputMark();
            }
        }
    }

    //xóa chi tiết đơn hàng trên view
    self.removeOrderDetail = function (item) {
        self.listDepositDetail.remove(item);
    }

    //thêm vào đơn hàng vào csdl
    self.saveDeposit = function () {

        if (self.checkOrderDetail()) {
            if (self.listDepositDetail() != null) {
                _.each(self.listDepositDetail(), function (item) {
                    item.ListContractCode = item.ListContractCode();
                })
            }

            self.isSubmit(false);
            $.post("/" + window.culture + "/CMS/Order/AddDeposit",
            {
                listProduct: self.listDepositDetail(), wardId: self.WardId(), listService: self.listService(), wardDeliveryId: self.WardDeliveryId(),
                __RequestVerifiCationToken: $("#depositAdd input[name='__RequestVerificationToken']").val()
            },
                function (result) {
                    if (result.status == msgType.error) {
                        toastr.error(result.msg);
                    } else {
                        toastr.success(result.msg);
                        setTimeout(function () {
                            window.location.replace("/" + window.culture + "/CMS/Order/DepositOrder");
                        }, 3000);
                    }
                    self.isSubmit(true);
                });
        }
    }

    self.AddInput = function (data) {
        data.ListContractCode.push(self.addContractCode(data.Id));
    };

    self.RemoveInput = function (data) {
        _.each(self.listDepositDetail(), function (item) {
            if (item.id == item.ParentId) {
                item.ListContractCode.remove(data);
            }
        });

        //data.ListContractCode.remove(data);
    };

    self.renderedHandler = function (elements, data) {
        $("#cate" + data.Id).dropdownjstree({
            source: window.listcategoryJsTree,
            selectedNode: "",
            selectNote: (node, selected) => {
                data.CategoryId(selected.node.id);
            }
        });
    }
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
}
var createDepositViewModel = new CreateDepositViewModel();
ko.applyBindings(createDepositViewModel, $("#depositAdd")[0]);
