function StockQuotesAddOrEditViewModel() {
    var self = this;

    self.customer = ko.observableArray([]);
    self.isDetailRending = ko.observable(true);
    self.listDetail = ko.observableArray([]);
    self.listSupplier = ko.observableArray([]);
    self.exchangeRate = ko.observable();
    self.isSubmit = ko.observable(true);
    self.isAdd = ko.observable(true);
    self.isModelAdd = ko.observable(true);
    self.isUpload = ko.observable(true);

    //model
    self.Id = ko.observable();
    self.Code = ko.observable();
    self.SystemId = ko.observable();
    self.SystemName = ko.observable();
    self.WarehouseId = ko.observable();
    self.WarehouseName = ko.observable();
    self.CustomerId = ko.observable();
    self.CustomerName = ko.observable("");
    self.CustomerEmail = ko.observable();
    self.CustomerPhone = ko.observable();
    self.CustomerAddress = ko.observable();
    self.Status = ko.observable(0);
    self.UserId = ko.observable();
    self.UserFullName = ko.observable();
    self.OfficeId = ko.observable();
    self.OfficeName = ko.observable();
    self.OfficeIdPath = ko.observable();
    self.CreateDate = ko.observable();
    self.UpdateDate = ko.observable();
    self.TypeService = ko.observable();
    self.ServiceMoney = ko.observable();
    self.AnalyticSupplier = ko.observable("");
    self.ShipMoney = ko.observable();
    self.SourceSupplierId = ko.observable();
    self.Type = ko.observable();
    self.ReasonCancel = ko.observable();
    self.UnsignName = ko.observable();
    self.UserNote = ko.observable();

    //rest form
    self.restForm = function () {
        self.Id("");
        self.Code("");
        self.SystemId("");
        self.SystemName("");
        self.WarehouseId("");
        self.WarehouseName("");
        self.CustomerId("");
        self.CustomerName("");
        self.CustomerEmail("");
        self.CustomerPhone("");
        self.CustomerAddress("");
        self.Status(0);
        self.UserId("");
        self.UserFullName("");
        self.CreateDate("");
        self.UpdateDate("");
        self.TypeService("");
        self.ServiceMoney("");
        self.AnalyticSupplier("");
        self.ShipMoney("");
        self.SourceSupplierId("");
        self.Type("");
        self.ReasonCancel("");
        self.UnsignName("");
        self.UserNote("");
    }

    self.setData = function (data) {
        self.Id(data.Id);
        self.Code(data.Code);
        self.SystemId(data.SystemId);
        self.SystemName(data.SystemName);
        self.WarehouseId(data.WarehouseId);
        self.WarehouseName(data.WarehouseName);
        self.CustomerId(data.CustomerId);
        self.CustomerName(data.CustomerName);
        self.CustomerEmail(data.CustomerEmail);
        self.CustomerPhone(data.CustomerPhone);
        self.CustomerAddress(data.CustomerAddress);
        self.Status(data.Status);
        self.UserId(data.UserId);
        self.UserFullName(data.UserFullName);
        self.CreateDate(data.CreateDate);
        self.UpdateDate(data.UpdateDate);
        self.TypeService(data.TypeService);
        self.ServiceMoney(data.ServiceMoney);
        self.AnalyticSupplier(data.AnalyticSupplier);
        self.ShipMoney(data.ShipMoney);
        self.SourceSupplierId(data.SourceSupplierId);
        self.Type(data.Type);
        self.ReasonCancel(data.ReasonCancel);
        self.UnsignName(data.UnsignName);
        self.UserNote(data.UserNote);
    }

    //nhà cung cấp
    self.SupplierId = ko.observable();
    self.SupplierPrice = ko.observable();
    self.SupplierExchangeRate = ko.observable();
    self.SupplierExchangePrice = ko.observable();
    self.SupplierTotalPrice = ko.observable();
    self.SupplierTotalExchange = ko.observable();
    self.SupplierQuantity = ko.observable();
    self.SupplierName = ko.observable();
    self.SupplierStatus = ko.observable();
    self.SupplierLink = ko.observable();
    self.SupplierDescription = ko.observable();
    self.SupplierCreated = ko.observable();
    self.SupplierShipMoney = ko.observable();

    self.resetSupplier = function () {
        self.SupplierId("");
        self.SupplierPrice("");
        self.SupplierExchangeRate("");
        self.SupplierExchangePrice("");
        self.SupplierTotalPrice("");
        self.SupplierTotalExchange("");
        self.SupplierQuantity("");
        self.SupplierName("");
        self.SupplierStatus("");
        self.SupplierLink("");
        self.SupplierDescription("");
        self.SupplierCreated("");
        self.SupplierShipMoney("");
    }

    self.setDataSupplier = function (data) {
        self.SupplierId(data.Id);
        self.SupplierPrice(data.Price);
        self.SupplierExchangeRate(data.ExchangeRate);
        self.SupplierExchangePrice(data.ExchangePrice);
        self.SupplierTotalPrice(data.TotalPrice);
        self.SupplierTotalExchange(data.TotalExchange);
        self.SupplierQuantity(data.Quantity);
        self.SupplierName(data.Name);
        self.SupplierStatus(data.Status);
        self.SupplierLink(data.Link);
        self.SupplierDescription(data.Description);
        self.SupplierCreated(data.Created);
        self.SupplierShipMoney(data.ShipMoney);
    }

    //Detail Orders
    self.DetailId = ko.observable();
    self.DetailName = ko.observable();
    self.DetailLink = ko.observable();
    self.DetailNote = ko.observable();
    self.DetailProperties = ko.observable();
    self.DetailCategoryId = ko.observable();
    self.DetailCategoryName = ko.observable();
    self.DetailQuantity = ko.observable();
    self.DetailImagePath1 = ko.observable("");
    self.DetailImagePath2 = ko.observable("");
    self.DetailImagePath3 = ko.observable("");
    self.DetailImagePath4 = ko.observable("");

    self.setDetail = function (data) {
        self.DetailId(data.Id);
        self.DetailName(data.Name);
        self.DetailLink(data.Link);
        self.DetailNote(data.Note);
        self.DetailProperties(data.Properties);
        self.DetailCategoryId(data.CategoryId);
        self.DetailCategoryName(data.CategoryName);
        self.DetailQuantity(data.Quantity);
        self.DetailImagePath1(data.ImagePath1);
        self.DetailImagePath2(data.ImagePath2);
        self.DetailImagePath3(data.ImagePath3);
        self.DetailImagePath4(data.ImagePath4);
    }

    self.resetDetail = function () {
        self.DetailId("");
        self.DetailName("");
        self.DetailLink("");
        self.DetailNote("");
        self.DetailProperties("");
        self.DetailCategoryId("");
        self.DetailCategoryName("");
        self.DetailQuantity("");
        self.DetailImagePath1("");
        self.DetailImagePath2("");
        self.DetailImagePath3("");
        self.DetailImagePath4("");
    }


    self.showModalDialog = function (id) {
        self.restForm();
        self.resetDetail();
        self.resetSupplier();

        self.listDetail([]);
        self.listSupplier([]);
        self.userOrder([]);

        self.isDetailRending(false);
        self.isShowHistory(false);

        $.post("/Source/GetData", function (result) {
            if (result.status === msgType.success) {
                self.exchangeRate(result.exchangeRate);
            } else {
                toastr.error(result.msg);
                self.isDetailRending(true);
            }
        });

        if (id > 0) {
            self.isModelAdd(false);
            $(".view-chat-box").show();

            $.post("/Source/GetSourceDetail", { id: id }, function (result) {
                if (result.status === msgType.success) {
                    self.setData(result.source);
                    self.customer(result.customer);
                    self.listDetail(result.listDetail);
                    self.listSupplier(result.listSupplier);
                    self.userOrder(result.userOrder);

                    self.isDetailRending(true);
                    $("#stockQuotesAddOrEdit").modal();

                    $(".customer-search-add-stock")
                        .empty()
                        .append($("<option/>").val(result.source.CustomerId).text(result.source.CustomerName))
                        .val(result.source.CustomerId)
                        .trigger("change");

                    self.viewBoxChat.showChat(self.Id(), self.Code(), self.Type(), 1);

                } else {
                    toastr.error(result.msg);
                    self.isDetailRending(true);
                }
            });
        } else {
            self.isDetailRending(true);
            self.isModelAdd(true);
            $("#stockQuotesAddOrEdit").modal();
            $(".customer-search-add-stock").empty();
            $(".view-chat-box").hide();
        }
    }

    // Nhà cung cấp
    self.showAddSupplier = function () {
        self.resetSupplier();
        self.isAdd(true);
        $("#addOrEditStockDetailModal").modal();
        self.initInputMark();
    }

    self.showEditSupplier = function (data) {
        self.resetSupplier();
        self.isAdd(false);
        self.setDataSupplier(data);
        $("#addOrEditStockDetailModal").modal();
        self.initInputMark();
    }

    self.saveSupplier = function () {
        if (!self.checkSupplier()) {
            return;
        };

        if (self.isAdd()) {
            var supplier = {
                Id: (new Date()).getTime(),
                Price: self.SupplierPrice(),
                ExchangeRate: self.SupplierExchangeRate(),
                ExchangePrice: self.SupplierExchangePrice(),
                TotalPrice: Globalize.parseFloat(self.SupplierPrice()) * Globalize.parseFloat(self.SupplierQuantity()),
                TotalExchange: self.SupplierTotalExchange(),
                Quantity: self.SupplierQuantity(),
                Name: self.SupplierName(),
                Status: self.SupplierStatus(),
                Link: self.SupplierLink(),
                Description: self.SupplierDescription(),
                ShipMoney: self.SupplierShipMoney()
            };

            if (!self.isModelAdd()) {
                $.post("/Source/SaveSupplier", { id: self.Id(), sourceSupplier: supplier }, function (result) {
                    if (result.status === msgType.success) {

                        self.listSupplier(result.list);
                        $("#addOrEditStockDetailModal").modal('hide');

                        toastr.success(result.msg);
                    } else {
                        toastr.error(result.msg);
                    }
                });
            } else {
                self.listSupplier.push(supplier);
                $("#addOrEditStockDetailModal").modal('hide');
            }
        } else {
            var supplier = _.find(self.listSupplier(), function (it) {
                if (it.Id === self.SupplierId()) {
                    it.Price = self.SupplierPrice();
                    it.ExchangeRate = self.SupplierExchangeRate();
                    it.ExchangePrice = self.SupplierExchangePrice();
                    it.TotalPrice = Globalize.parseFloat(self.SupplierPrice()) * Globalize.parseFloat(self.SupplierQuantity());
                    it.TotalExchange = self.SupplierTotalExchange();
                    it.Quantity = self.SupplierQuantity();
                    it.Name = self.SupplierName();
                    it.Status = self.SupplierStatus();
                    it.Link = self.SupplierLink();
                    it.Description = self.SupplierDescription();
                    it.ShipMoney = self.SupplierShipMoney();
                };
                return it.Id === self.SupplierId();
            });

            if (!self.isModelAdd()) {
                $.post("/Source/UpdateSupplier", { sourceSupplier: supplier }, function (result) {
                    if (result.status === msgType.success) {
                        var list = self.listSupplier();
                        self.listSupplier([]);
                        self.listSupplier(list);
                        $("#addOrEditStockDetailModal").modal('hide');

                        toastr.success(result.msg);
                    } else {
                        toastr.error(result.msg);
                    }
                });
            } else {
                var list = self.listSupplier();
                self.listSupplier([]);
                self.listSupplier(list);
                $("#addOrEditStockDetailModal").modal('hide');
            }
        }

        //self.updateGeneral();
    }

    self.removeSupplier = function (data) {
        swal({
            title: 'Are you sure you want to delete this item?',
            text: '',
            type: 'warning',
            showCancelButton: true,
           cancelButtonText: 'Cancel',
            confirmButtonText: 'Delete'
        }).then(function () {
            if (self.isModelAdd()) {
                self.listSupplier.remove(data);
            } else {
                $.post("/Source/DeleteSupplier", { id: data.Id }, function (result) {
                    if (result.status === msgType.success) {
                        self.listSupplier.remove(data);
                        toastr.success(result.msg);
                        //self.updateGeneral();
                    } else {
                        toastr.error(result.msg);
                    }
                });
            }
            //self.updateGeneral();
        }, function () { });
    }

    //Detail Orders
    self.showAddDetail = function () {
        self.resetDetail();
        self.isAdd(true);
        $("#addOrEditSourceDetailModal").modal();
        self.initInputMark();

        $('.dropdownjstree').remove();
        $("#category_tree_source").dropdownjstree({
            source: window.categoryJsTree,
            selectedNode: self.DetailCategoryId(),
            selectNote: (node, selected) => {
                self.DetailCategoryId(selected.node.id);
                self.DetailCategoryName(selected.node.text);
            }
        });
    }

    self.showEditDetail = function (data) {
        self.resetDetail();
        self.isAdd(false);
        self.setDetail(data);
        $("#addOrEditSourceDetailModal").modal();
        self.initInputMark();

        $('.dropdownjstree').remove();
        $("#category_tree_source").dropdownjstree({
            source: window.categoryJsTree,
            selectedNode: self.DetailCategoryId(),
            selectNote: (node, selected) => {
                self.DetailCategoryId(selected.node.id);
                self.DetailCategoryName(selected.node.text);
            }
        });
    }

    self.saveDetail = function () {
        if (!self.checkDetail()) {
            return;
        };

        if (self.isAdd()) {
            var detail = {
                Id: (new Date()).getTime(),
                Name: self.DetailName(),
                Link: self.DetailLink(),
                Note: self.DetailNote(),
                Properties: self.DetailProperties(),
                CategoryId: self.DetailCategoryId(),
                CategoryName: self.DetailCategoryName(),
                Quantity: self.DetailQuantity(),
                ImagePath1: self.DetailImagePath1(),
                ImagePath2: self.DetailImagePath2(),
                ImagePath3: self.DetailImagePath3(),
                ImagePath4: self.DetailImagePath4()
            };

            if (!self.isModelAdd()) {
                $.post("/Source/SaveDetail", { id: self.Id(), sourceDetail: detail }, function (result) {
                    if (result.status === msgType.success) {

                        self.listDetail.push(detail);
                        $("#addOrEditSourceDetailModal").modal('hide');

                        toastr.success(result.msg);
                    } else {
                        toastr.error(result.msg);
                    }
                });
            } else {
                self.listDetail.push(detail);
                $("#addOrEditSourceDetailModal").modal('hide');
            }
        } else {
            var detail = _.find(self.listDetail(), function (it) {
                if (it.Id === self.DetailId()) {
                    it.Name = self.DetailName();
                    it.Link = self.DetailLink();
                    it.Note = self.DetailNote();
                    it.Properties = self.DetailProperties();
                    it.CategoryId = self.DetailCategoryId();
                    it.CategoryName = self.DetailCategoryName();
                    it.Quantity = self.DetailQuantity();
                    it.ImagePath1 = self.DetailImagePath1();
                    it.ImagePath2 = self.DetailImagePath2();
                    it.ImagePath3 = self.DetailImagePath3();
                    it.ImagePath4 = self.DetailImagePath4();
                };
                return it.Id === self.DetailId();
            });

            if (!self.isModelAdd()) {
                $.post("/Source/UpdateDetail", { sourceDetail: detail }, function (result) {
                    if (result.status === msgType.success) {
                        var list = self.listDetail();
                        self.listDetail([]);
                        self.listDetail(list);
                        $("#addOrEditSourceDetailModal").modal('hide');

                        toastr.success(result.msg);
                    } else {
                        toastr.error(result.msg);
                    }
                });
            } else {
                var list = self.listDetail();
                self.listDetail([]);
                self.listDetail(list);
                $("#addOrEditSourceDetailModal").modal('hide');
            }
        }

        //self.updateGeneral();
    }

    self.removeDetail = function (data) {
        swal({
            title: 'Are you sure you want to delete this item?',
            text: '',
            type: 'warning',
            showCancelButton: true,
           cancelButtonText: 'Cancel',
            confirmButtonText: 'Delete'
        }).then(function () {
            if (self.isModelAdd()) {
                self.listDetail.remove(data);
            } else {
                $.post("/Source/DeleteDetail", { id: data.Id }, function (result) {
                    if (result.status === msgType.success) {
                        self.listDetail.remove(data);
                        toastr.success(result.msg);
                    } else {
                        toastr.error(result.msg);
                    }
                });
            }
        }, function () { });
    }

    // type: 1- Lưu lại, 2- Gửi cho khách luôn
    self.save = function (type) {
        if (!self.checkModel(type)) {
            return;
        }
        self.isSubmit(false);
        if (self.isModelAdd()) {
            //Thêm Ticket find source
            $.post("/Source/Save", {
                source: {
                    Id: self.Id(),
                    Code: self.Code(),
                    CustomerId: self.CustomerId(),
                    CustomerName: self.CustomerName(),
                    CustomerEmail: self.CustomerEmail(),
                    CustomerPhone: self.CustomerPhone(),
                    CustomerAddress: self.CustomerAddress(),
                    AnalyticSupplier: self.AnalyticSupplier(),
                    UserNote: self.UserNote()
                },
                listDetails: self.listDetail(),
                listsSuppliers: self.listSupplier(),
                type: type
            }, function (result) {
                if (result.status === msgType.success) {
                    toastr.success(result.msg);
                    $("#stockQuotesAddOrEdit").modal('hide');

                    $(".search-list").trigger('click');
                } else {
                    toastr.error(result.msg);
                }

                self.isSubmit(true);
            });

        } else {
            //Edit Ticket find source

            $.post("/Source/Update", {
                source: {
                    Id: self.Id(),
                    Code: self.Code(),
                    CustomerId: self.CustomerId(),
                    CustomerName: self.CustomerName(),
                    CustomerEmail: self.CustomerEmail(),
                    CustomerPhone: self.CustomerPhone(),
                    CustomerAddress: self.CustomerAddress(),
                    AnalyticSupplier: self.AnalyticSupplier(),
                    UserNote: self.UserNote()
                },
                listDetails: self.listDetail(),
                listsSuppliers: self.listSupplier(),
                type: type
            }, function (result) {
                if (result.status === msgType.success) {
                    toastr.success(result.msg);
                    $("#stockQuotesAddOrEdit").modal('hide');

                    $(".search-list").trigger('click');
                } else {
                    toastr.error(result.msg);
                }

                self.isSubmit(true);
            });
        }
    }

    self.checkDetail = function () {
        if (self.DetailName() === "") {
            toastr.error("Product name can not be empty!");
            return false;
        }
        if (self.DetailLink() === "") {
            toastr.error("Link cannot be empty!");
            return false;
        }
        if (self.DetailCategoryName() === "") {
            toastr.error("Branch is not empty!");
            return false;
        }
        if (self.DetailQuantity() === "") {
            toastr.error("Quantity cannot be empty!");
            return false;
        }
        if (self.DetailImagePath1() === "" && self.DetailImagePath2() === "" && self.DetailImagePath3() === "" && self.DetailImagePath4() === "") {
            toastr.error("Image cannot be empty!");
            return false;
        }

        return true;
    }

    self.checkSupplier = function () {
        if (self.SupplierName() === "") {
            toastr.error("Name of supplier cannot be empty!");
            return false;
        }
        if (self.SupplierLink() === "") {
            toastr.error("Link cannot be empty!");
            return false;
        }
        if (self.SupplierQuantity() === "") {
            toastr.error("Quanlity cannot be empty!");
            return false;
        }
        if (self.SupplierPrice() === "") {
            toastr.error("Unit price cannot be empty!");
            return false;
        }
        if (self.SupplierDescription() === "") {
            toastr.error("Describe cannot be empty!");
            return false;
        }

        return true;
    }

    self.checkModel = function (type) {
        if (self.listDetail().length === 0) {
            toastr.error("Detail quotation find the source cannot be empty!");
            return false;
        }

        if (type === 2) {
            if (self.listSupplier().length < 3) {
                toastr.error("Must have 3 supplier!");
                return false;
            }
        }
        if (self.CustomerName() === "") {
            toastr.error("Customer cannot be empty!");
            return false;
        }
        if (self.AnalyticSupplier() === "") {
            toastr.error("Analyze the suppliercannot be empty!");
            return false;
        }

        return true;
    }

    $(function () {
        self.searchCustomer();

        if (Modernizr.touch) {
            // show the close overlay button
            $(".close-overlay").removeClass("hidden");
            // handle the adding of hover class when clicked
            $(".img").click(function (e) {
                if (!$(this).hasClass("hover")) {
                    $(this).addClass("hover");
                }
            });
            // handle the closing of the overlay
            $(".close-overlay").click(function (e) {
                e.preventDefault();
                e.stopPropagation();
                if ($(this).closest(".img").hasClass("hover")) {
                    $(this).closest(".img").removeClass("hover");
                }
            });
        } else {
            // handle the mouseenter functionality
            $(".img").mouseenter(function () {
                $(this).addClass("hover");
            })
            // handle the mouseleave functionality
            .mouseleave(function () {
                $(this).removeClass("hover");
            });
        }
    });

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

    //Hàm lấy thông tin khách hàng
    self.searchCustomer = function () {
        $(".customer-search-add-stock")
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
                    if (self.CustomerName() !== repo.text) {
                        self.CustomerName(repo.text);
                        self.CustomerEmail(repo.email);
                        self.CustomerPhone(repo.phone);
                        self.CustomerAddress(repo.address);
                    }

                    return repo.text;
                },
                placeholder: "",
                allowClear: true,
                language: 'en'
            });
    };

    var maxFileLength = 5120000;;
    self.addImage = function () {
        $(".flieuploadImg").fileupload({
            url: "/Upload/UploadImages",
            sequentialUploads: true,
            dataType: "json",
            add: function (e, data) {
                var file = data.files[0];
                var msg = "";
                if (maxFileLength && file.size > maxFileLength) {
                    if (msg) {
                        msg += "<br/>";
                    }
                    msg += file.name + ": Size is too large";
                } else if (validateBlackListExtensions(file.name)) {
                    if (msg) {
                        msg += "<br/>";
                    }
                    msg += file.name + ": Not in correct format";
                }
                if (msg !== "") {
                    toastr.error(msg);
                } else {
                    data.submit();
                }
            },
            done: function (e, data) {
                if (data.result === -5) {
                    toastr.error("Files are not allowed");
                    return;
                }

                if (self.DetailImagePath1() !== "" &&
                    self.DetailImagePath2() !== "" &&
                    self.DetailImagePath3() !== "" &&
                    self.DetailImagePath4() !== "") {
                    self.DetailImagePath1(window.location.origin + data.result[0].path);
                } else {
                    if (self.DetailImagePath1() === "") {
                        self.DetailImagePath1(window.location.origin + data.result[0].path);
                    } else {
                        if (self.DetailImagePath2() === "") {
                            self.DetailImagePath2(window.location.origin + data.result[0].path);
                        } else {
                            if (self.DetailImagePath3() === "") {
                                self.DetailImagePath3(window.location.origin + data.result[0].path);
                            } else {
                                if (self.DetailImagePath4() === "") {
                                    self.DetailImagePath4(window.location.origin + data.result[0].path);
                                }
                            }
                        }
                    }
                }

                $("div").removeClass("hover");
            }
        });
        return true;
    }

    self.listHistory  = ko.observableArray([]);
    self.isShowHistory = ko.observable(false);
    self.checkShowHistory = function() {
        self.isShowHistory(!self.isShowHistory());
    }

    var validateBlackListExtensions = function (file) {
        var ext = file.substring(file.lastIndexOf(".")).toLowerCase();
        return !_.some([".jpg", ".jpeg", ".gif", ".png"], function (item) { return item === ext; });
    };

    self.userOrder = ko.observableArray([]);
}