var CreateSourcingViewModel = function () {
    var self = this;
    self.active = ko.observable('');
    self.listSourcingDetail = ko.observableArray([]);
    self.checkDuplicateLink = ko.observable(false);
    self.isSubmit = ko.observable(true);
    self.CategoryId = ko.observable('');
    //hàm khởi tạo dữ liệu
    $(function () {
        self.listSourcingDetail.push(self.addDetail());
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
                    msg += file.name + ": Có kích thước quá lớn";
                } else if (validateBlackListExtensions(file.name)) {
                    if (msg) {
                        msg += "<br/>";
                    }
                    msg += file.name + ": Không đúng định dạng";
                }
                if (msg !== "") {
                    toastr.error(msg);
                } else {
                    data.submit();
                }
            },
            done: function (e, data) {
                if (data.result === -5) {
                    toastr.error("Tệp tin là không được phép");
                    return;
                }

                data1.ImagePath1(window.location.origin + data.result[0].path);

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
        window.location.href = "/" + window.culture + "/CMS/Order/SourcingOrder";
    };

    //tạo chi tiết đơn hàng ký gửi
    self.addDetail = function () {
        var depositDetail = {
            Id: (new Date()).getTime(),
            Name: ko.observable(""),
            CategoryId: ko.observable(""),
            ImagePath1: ko.observable(""),
            Link: ko.observable(""),
            Color: ko.observable(""),
            Quantity: ko.observable(""),
            Note: ko.observable(""),
        };
        self.CategoryId.subscribe(function (newValue) {
            depositDetail.CategoryId(newValue);
        });

        depositDetail.NameCss = ko.observable("");
        depositDetail.NameFocus = ko.observable(false);
        depositDetail.Name.subscribe(function (newValue) {
            if (newValue == undefined || newValue == null || newValue == '') {
                depositDetail.NameCss("error");
                depositDetail.NameFocus(true);
                toastr.error('Nhập tên sản phẩm');
            } else {
                //var checkImage = _.filter(self.listSourcingDetail(), function (item) {
                //    return (depositDetail.Id != item.Id && depositDetail.ProductName() == item.ProductName());
                //});
                //console.log(checkImage.length);
                //if (checkImage.length > 0) {
                //    depositDetail.ProductNameCss("error");
                //    depositDetail.ProductNameFocus(true);

                //    toastr.error('Image sản phẩm bị trùng');
                //    self.checkDuplicateLink(true);
                //} else {
                //    depositDetail.ProductNameCss("");
                //    depositDetail.ProductNameFocus(false);
                //    self.checkDuplicateLink(false);
                //}
            }
        });

        depositDetail.LinkCss = ko.observable("");
        depositDetail.LinkFocus = ko.observable(false);
        depositDetail.Link.subscribe(function (newValue) {
            if (newValue == undefined || newValue == null || newValue == '' || newValue == 0) {
                depositDetail.LinkCss("error");
                depositDetail.LinkFocus(true);
                toastr.error('Nhập Link sản phẩm');
            } else {
                depositDetail.ColorCss("");
                depositDetail.ColorFocus(false);
            }
        });
        depositDetail.ColorCss = ko.observable("");
        depositDetail.ColorFocus = ko.observable(false);
        depositDetail.Color.subscribe(function (newValue) {
            if (newValue == undefined || newValue == null || newValue == '' || newValue == 0) {
                depositDetail.ColorCss("error");
                depositDetail.ColorFocus(true);
                toastr.error('Nhập màu sắc');
            } else {
                depositDetail.ColorCss("");
                depositDetail.ColorFocus(false);
            }
        });

        depositDetail.QuantityCss = ko.observable("");
        depositDetail.QuantityFocus = ko.observable(false);
        depositDetail.Quantity.subscribe(function (newValue) {
            if (newValue == undefined || newValue == null || newValue == '' || newValue == 0) {
                depositDetail.QuantityCss("error");
                depositDetail.QuantityFocus(true);
                toastr.error('Nhập số lượng');
            } else {
                depositDetail.QuantityCss("");
                depositDetail.QuantityFocus(false);
            }
        });

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
            toastr.error('Image sản phẩm bị trùng');
        }
       
        _.each(self.listSourcingDetail(), function (depositDetail) {

            if (depositDetail.Name() == undefined || depositDetail.Name() == null || depositDetail.Name() == '') {
                depositDetail.NameCss("error");
                depositDetail.NameFocus(true);
                toastr.error('Nhập tên sản phẩm');
                check = false;
                return false;
            }

            if (depositDetail.Quantity() == undefined || depositDetail.Quantity() == null || depositDetail.Quantity() == '') {
                depositDetail.QuantityCss("error");
                depositDetail.QuantityFocus(true);
                toastr.error('Nhập số lượng');
                check = false;
                return false;
            }

            if (depositDetail.Link() == undefined || depositDetail.Link() == null || depositDetail.Link() == '') {
                depositDetail.LinkCss("error");
                depositDetail.LinkFocus(true);
                toastr.error('Nhập Link sản phẩm');
                check = false;
                return false;
            }

            if (depositDetail.Color() == undefined || depositDetail.Color() == null || depositDetail.Color() == '') {
                depositDetail.ColorCss("error");
                depositDetail.ColorFocus(true);
                toastr.error('Nhập màu sản phẩm');
                check = false;
                return false;
            }
            if (depositDetail.CategoryId() == undefined || depositDetail.CategoryId() == null || depositDetail.CategoryId() == '' || depositDetail.CategoryId() == -1) {

                toastr.error('เลือกประเภทสินค้า');
                check = false;
                return false;
            }
           


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
    self.addSourcingDetail = function () {
        if (self.checkOrderDetail()) {
            if (self.listSourcingDetail().length >= 50) {
                toastr.error("ออเดอร์ไม่สามารถใส่เกิน 50 ลิงค์สินค้า !");
                //toastr.error("Đơn hàng không thể vượt quá 50 link sản phẩm !");
            } else {
                self.listSourcingDetail.push(self.addDetail());
                self.initInputMark();
            }
        }
    }

    //xóa chi tiết đơn hàng trên view
    self.removeOrderDetail = function (item) {
        self.listSourcingDetail.remove(item);
    }

    //thêm vào đơn hàng vào csdl
    self.saveSourcing = function () {

        if (self.checkOrderDetail()) {
            //if (self.listSourcingDetail() != null) {
            //    _.each(self.listSourcingDetail(), function (item) {
            //        item.ListContractCode = item.ListContractCode();
            //    })
            //}

            self.isSubmit(false);
            $.post("/" + window.culture + "/CMS/Order/SaveSourcing",
            {
                sourceDetail: self.listSourcingDetail(),
                __RequestVerifiCationToken: $("#depositAdd input[name='__RequestVerificationToken']").val()
            },
                function (result) {
                    if (result.status == msgType.error) {
                        toastr.error(result.msg);
                    } else {
                        toastr.success(result.msg);
                        setTimeout(function () {
                            window.location.replace("/" + window.culture + "/CMS/Order/Sourcing");
                        }, 3000);
                    }
                    self.isSubmit(true);
                });
        }
    }
    self.renderedHandler = function (elements, data) {
        $("#cate" + data.Id).dropdownjstree({
            source: window.listcategoryJsTree,
            selectedNode: "",
            selectNote: (node, selected) => {
                data.CategoryId(selected.node.id);
            }
        });
    }

}
var createSourcingViewModel = new CreateSourcingViewModel();
ko.applyBindings(createSourcingViewModel, $("#sourcingAdd")[0]);
