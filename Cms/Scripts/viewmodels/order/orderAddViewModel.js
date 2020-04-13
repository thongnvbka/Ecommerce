function OrderAddViewModel() {
    var self = this;

    self.customer = ko.observableArray([]);
    self.isDetailRending = ko.observable(true);
    self.listDetail = ko.observableArray([]);
    self.exchangeRate = ko.observable();
    self.isSubmit = ko.observable(true);
    self.isAdd = ko.observable(true);
    self.isModelAdd = ko.observable(true);
    self.isUpload = ko.observable(true);
    self.listWarehouseOrder =  ko.observableArray([]);
    //model
    //khách hàng
    self.CustomerId = ko.observable();
    self.CustomerName = ko.observable("");
    self.CustomerPhone = ko.observable("");
    self.CustomerEmail = ko.observable("");
    self.CustomerAddress = ko.observable("");
    self.ContactName = ko.observable("");
    self.ContactPhone = ko.observable("");
    self.ContactAddress = ko.observable("");
    self.ContactEmail = ko.observable("");
    //thông tin thêm
    self.WebsiteName = ko.observable("");
    self.ShopId = ko.observable();
    self.ShopName = ko.observable("");
    self.ShopLink = ko.observable("");
    self.ExchangeRate = ko.observable(0);

    self.WarehouseId = ko.observable();
    self.WarehouseName = ko.observable("");
    self.TotalPrice = ko.observable(0);
    //note
    self.Note = ko.observable();
    self.UserNote = ko.observable();

    self.resetModel = function() {
        self.CustomerId(0);
        self.CustomerName("");
        self.CustomerPhone("");
        self.CustomerEmail("");
        self.CustomerAddress("");
        self.ContactName("");
        self.ContactPhone("");
        self.ContactAddress("");
        self.ContactEmail("");
        self.WebsiteName("");
        self.ShopId(0);
        self.ShopName("");
        self.ShopLink("");
        self.WarehouseId(0);
        self.WarehouseName("");
        self.TotalPrice(0);
        self.Note("");
        self.UserNote("");
        self.listDetail([]);
    }

    //Detail sản phẩm

    self.DetailId = ko.observable("");
    self.DetailName = ko.observable("");
    self.DetailImage = ko.observable("");
    self.DetailQuantity = ko.observable("");
    self.DetailPrice = ko.observable("");
    self.DetailTotalPrice = ko.observable("");
    self.DetailNote = ko.observable("");
    self.DetailLink = ko.observable("");
    self.DetailCategoryId = ko.observable(0);
    self.DetailCategoryName = ko.observable("");
    self.DetailSize = ko.observable("");
    self.DetailColor = ko.observable("");
    self.DetailStatus = ko.observable("");
    self.DetailUserNote = ko.observable("");

    self.setDetail = function (data) {
        self.DetailId(data.Id);
        self.DetailName(data.Name);
        self.DetailImage(data.Image);
        self.DetailQuantity(data.Quantity);
        self.DetailPrice(data.Price);
        self.DetailTotalPrice(data.TotalPrice);
        self.DetailNote(data.Note);
        self.DetailLink(data.Link);
        self.DetailCategoryId(data.CategoryId);
        self.DetailCategoryName(data.CategoryName);
        self.DetailSize(data.Size);
        self.DetailColor(data.Color);
        self.DetailStatus(data.Status);
        self.DetailUserNote(data.UserNote);
    }

    self.resetDetail = function () {
        self.DetailId("");
        self.DetailName("");
        self.DetailImage("");
        self.DetailQuantity("");
        self.DetailPrice("");
        self.DetailTotalPrice("");
        self.DetailNote("");
        self.DetailLink("");
        self.DetailCategoryId(0);
        self.DetailCategoryName("");
        self.DetailSize("");
        self.DetailColor("");
        self.DetailStatus("");
        self.DetailUserNote("");
    }

    self.viewOrderAdd = function () {
        self.isDetailRending(false);
        $('#orderAddModal').modal();

        self.searchCustomer();
        self.searchShop();
        self.listWarehouseOrder(window.listWarehouse);

        self.isDetailRending(true);
        self.initInputMark();
        self.ExchangeRate(formatNumberic(window.exchangeRate, 'N2'));
    }


    //hàm lấy thông tin shop
    self.searchShop = function () {
        $(".shop-search")
            .select2({
                ajax: {
                    url: "Shop/GetShopSearch",
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
                                        <div>\
                                            <b>" + repo.text + "</b><br/>\
                                            " + repo.url + "<br/>\
                                        </div>\
                                    </div>\
                                    <div class='clear-fix'></div>\
                                </div>";
                    return markup;
                },
                templateSelection: function (repo) {
                    if (self.ShopName() !== repo.text) {
                        self.ShopName(repo.text);
                        self.ShopLink(repo.url);
                    }

                    return repo.text;
                },
                placeholder: "",
                allowClear: true,
                language: 'en'
            });
    };

    //Thêm mới shop
    self.showAddShop = function () {
        $('#shopAdd').modal();
    }

    self.submitShop = function () {
        if (self.ShopName() === "") {
            toastr.error("Shop name can not be empty!");
            return false;
        }

        if (self.ShopLink() === "") {
            toastr.error("Link shop receipt cannot be empty!");
            return false;
        }

        self.isSubmit(false);

        $.post("/Shop/AddFash", {
            name: self.ShopName(),
            link: self.ShopLink()
        }, function (result) {
            if (result.status === msgType.success) {
                toastr.success(result.msg);
                $("#shopAdd").modal('hide');

                self.ShopId(result.shop.Id);
                self.ShopName(result.shop.Name);
                self.ShopLink(result.shop.Url);

                $(".shop-search")
                    .empty()
                    .append($("<option/>").val(result.shop.Id).text(result.shop.Name))
                    .val(result.shop.Id)
                    .trigger("change");

            } else if (result.status === msgType.error) {
                toastr.error(result.msg);
                self.ShopId("");
                self.ShopName("");
                self.ShopLink("");
            } else {
                toastr.warning(result.msg);
                $("#shopAdd").modal('hide');

                self.ShopId(result.shop.Id);
                self.ShopName(result.shop.Name);
                self.ShopLink(result.shop.Url);

                $(".shop-search")
                    .empty()
                    .append($("<option/>").val(result.shop.Id).text(result.shop.Name))
                    .val(result.shop.Id)
                    .trigger("change");
            }

            self.isSubmit(true);
        });
    }

    //hàm show form Detail

    self.showAddDetail = function () {
        self.resetDetail();
        self.isAdd(true);
        $('#detailAdd').modal();
        self.initInputMark();

        $('.dropdownjstree').remove();
        $("#category_tree_order_detail").dropdownjstree({
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
        $("#detailAdd").modal();
        self.initInputMark();

        $('.dropdownjstree').remove();
        $("#category_tree_order_detail").dropdownjstree({
            source: window.categoryJsTree,
            selectedNode: self.DetailCategoryId(),
            selectNote: (node, selected) => {
                self.DetailCategoryId(selected.node.id);
                self.DetailCategoryName(selected.node.text);
            }
        });
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
            self.listDetail.remove(data);
        }, function () { });
    }

    self.saveDetail = function () {
        if (!self.checkDetail()) {
            return;
        };

        if (self.isAdd()) {
            var detail = {
                Id: (new Date()).getTime(),
                Name: self.DetailName(),
                Image: self.DetailImage(),
                Quantity: self.DetailQuantity(),
                Price: self.DetailPrice(),
                TotalPrice: formatNumberic(Globalize.parseFloat(self.DetailQuantity()) * Globalize.parseFloat(self.DetailPrice()), 'N2'),
                Note: self.DetailNote(),
                Link: self.DetailLink(),
                CategoryId: self.DetailCategoryId(),
                CategoryName: self.DetailCategoryName(),
                Size: self.DetailSize(),
                Color: self.DetailColor(),
                Status: 0,
                UserNote: self.DetailUserNote()
            };

            self.listDetail.push(detail);

            $("#detailAdd").modal('hide');

        } else {
            var detail = _.find(self.listDetail(), function (it) {
                if (it.Id === self.DetailId()) {
                    it.Name = self.DetailName();
                    it.Image = self.DetailImage();
                    it.Quantity = self.DetailQuantity();
                    it.Price = self.DetailPrice();
                    it.TotalPrice = formatNumberic(Globalize.parseFloat(self.DetailQuantity()) * Globalize.parseFloat(self.DetailPrice()), 'N2');
                    it.Note = self.DetailNote();
                    it.Link = self.DetailLink();
                    it.CategoryId = self.DetailCategoryId();
                    it.CategoryName = self.DetailCategoryName();
                    it.Size = self.DetailSize();
                    it.Color = self.DetailColor();
                    it.Status = 0;
                    it.UserNote = self.DetailUserNote();
                };
                return it.Id === self.DetailId();
            });

            var list = self.listDetail();
            self.listDetail([]);
            self.listDetail(list);

            $("#detailAdd").modal('hide');

        }
        self.initInputMark();
    }

    self.saveOrder = function() {
         if (!self.checkModel()) {
            return;
        };

         $.post("/Order/Save", {
                order: {
                  CustomerId : self.CustomerId(),
                  CustomerName : self.CustomerName(),
                  CustomerPhone : self.CustomerPhone(),
                  CustomerEmail : self.CustomerEmail(),
                  CustomerAddress : self.CustomerAddress(),
                  ContactName : self.ContactName(),
                  ContactPhone : self.ContactPhone(),
                  ContactAddress : self.ContactAddress(),
                  ContactEmail : self.ContactEmail(),
                  WebsiteName : self.WebsiteName(),
                  ShopId : self.ShopId(),
                  ShopName : self.ShopName(),
                  ShopLink : self.ShopLink(),
                  ExchangeRate : self.ExchangeRate(),
                  WarehouseId : self.WarehouseId(),
                  WarehouseName : self.WarehouseName(),
                  TotalPrice : self.TotalPrice(),
                  Note : self.Note(),
                  UserNote : self.UserNote()
                },
                listDetails: self.listDetail()
            }, function (result) {
                if (result.status === msgType.success) {
                    toastr.success(result.msg);
                    $("#orderAddModal").modal('hide');

                    $(".search-list").trigger('click');
                } else {
                    toastr.error(result.msg);
                }

                self.isSubmit(true);
            });
    }

    self.checkDetail = function () {
        if (self.DetailName() === "") {
            toastr.error("Product name can not be empty!");
            return false;
        }
        if (self.DetailLink() === "") {
            toastr.error("Link can not be empty!");
            return false;
        }
        if (self.DetailCategoryName() === "") {
            toastr.error("Branch is not empty!");
            return false;
        }
        if (self.DetailPrice() === "") {
            toastr.error("unit price cannot be empty!");
            return false;
        }
        if (self.DetailQuantity() === "") {
            toastr.error("quantity cannot be empty!");
            return false;
        }
        if (self.DetailImage() === "") {
            toastr.error("Image cannot be empty!");
            return false;
        }

        return true;
    }

 self.checkModel = function () {
        if (self.CustomerName() === "") {
            toastr.error("no customer selected!");
            return false;
        }
        if (self.ContactName() === "") {
            toastr.error("Enter the recipient name!");
            return false;
        }
        if (self.ContactPhone() === "") {
            toastr.error("Enter the recipient phone!");
            return false;
        }
        if (self.ContactAddress() === "") {
            toastr.error("Enter the recipient address!");
            return false;
        }
        if (self.WebsiteName() === "") {
            toastr.error("Website cannot be empty!");
            return false;
        }
        if (self.ShopName() === "") {
            toastr.error("Shop cannot be empty!");
            return false;
        }

        if (self.WarehouseName() === "") {
            toastr.error("CN warehouse cannot be empty!");
            return false;
        }

        if (self.listDetail().length === 0) {
            toastr.error("Details have not yet record!");
            return false;
        }


        return true;
    }

    // Hàm lấy thông tin kho khi đã chọn
    self.WarehouseId.subscribe(function (newId) {
        var warehouse = _.find(self.listWarehouseOrder(), function (item) { return item.Id === newId; });

        if (warehouse !== undefined) {
            self.WarehouseName(warehouse.Name);
        }
    });

    //Hàm lấy thông tin khách hàng
    self.searchCustomer = function () {
        $(".customer-search-add-order")
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
                    toastr.error("The file is not permitted");
                    return;
                }

                self.DetailImage(window.location.origin + data.result[0].path);

                $("div").removeClass("hover");
            }
        });
        return true;
    }

    var validateBlackListExtensions = function (file) {
        var ext = file.substring(file.lastIndexOf(".")).toLowerCase();
        return !_.some([".jpg", ".jpeg", ".gif", ".png"], function (item) { return item === ext; });
    };

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
}