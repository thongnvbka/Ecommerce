function OrderAddViewModel() {
    var self = this;
    self.active = ko.observable('');
    self.listOrderDetail = ko.observableArray([]);
    self.listOrder = ko.observableArray([]);

    self.checkDuplicateLink = ko.observable(false);
    self.isSubmit = ko.observable(true);
    self.isSuccess = ko.observable(true);

    self.url = ko.observable("");
    self.dataHTML = ko.observable("");
    self.product = ko.observable(null);
    self.listImg = ko.observableArray([]);
    self.listProperty = ko.observableArray([]);
    self.listRange = ko.observableArray([]);
    self.dataJson = ko.observable();
    self.website = ko.observable();
    self.countShop = ko.observable(0);
    self.countLink = ko.observable(0);
    self.mes = ko.observable("");
    self.listWardDelivery = ko.observableArray([]);
    self.WardDeliveryId = ko.observable('');
    self.listService = ko.observableArray([]);

    //reset model
    self.reset = function () {
        self.url("");
        self.dataHTML("");
        self.product(null);
        self.listImg([]);
        self.listProperty([]);
        self.listRange([]);
        self.listWardDelivery([]);
        self.dataJson();
        self.website();
        self.countShop(0);
        self.countLink(0);
        self.mes("");
    }

    //hàm khởi tạo dữ liệu
    $(function () {
        var arr = _.split(window.location.href, "url=");
        if (arr.length > 1) {
            self.url(arr[1]);
            self.getHTML();
        }
        self.listWardDelivery(window.listWardDelivery);
        self.listService(window.listService);
        self.initInputMark();
        self.WardDeliveryId(window.warehouseId);
        //Lấy dữ liệu từ cookie cho vào giỏ hàng
        var cartCookie = $.cookie('cart-hand-' + window.customerId);
        if (cartCookie != undefined) {
            var cartJson = $.parseJSON(cartCookie);
            _.each(cartJson, function (item) {
                var oderDetail = self.addDetail();

                oderDetail.Id = item.Id;
                oderDetail.Name(item.Name);
                oderDetail.Image(item.Image);
                oderDetail.Link(item.Link);
                oderDetail.Size(item.Size);
                oderDetail.Color(item.Color);
                oderDetail.Price(item.Price);
                oderDetail.Quantity(item.Quantity);
                oderDetail.Note(item.Note);
                oderDetail.ShopName(item.ShopName);
                oderDetail.ShopLink(item.ShopLink);
                oderDetail.BeginAmount(item.BeginAmount);
                oderDetail.EndAmount(item.EndAmount);
                oderDetail.Properties(item.Properties);
                oderDetail.TypeObj(item.TypeObj);

                self.listOrder.push(oderDetail);
            });

            //tính lại giá trị
            var arrayLink = $.map(_.countBy(ko.mapping.toJS(self.listOrder()), 'Link'), function (value, index) {
                return [value];
            });

            var arrayShop = $.map(_.countBy(ko.mapping.toJS(self.listOrder()), 'ShopLink'), function (value, index) {
                return [value];
            });

            self.countLink(arrayLink.length);
            self.countShop(arrayShop.length);

            self.initInputMark();
        }
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
    }

    self.backToList = function () {
        window.location.href = "/" + window.culture + "/CMS/Order/BuyOrder";
    };
    //tạo chi tiết đơn hàng
    self.addDetail = function () {
        var oderDetail = {
            Id: (new Date()).getTime(),
            Name: ko.observable(""),
            Image: ko.observable(""),
            Link: ko.observable(""),
            Size: ko.observable(""),
            Color: ko.observable(""),
            Price: ko.observable(""),
            Quantity: ko.observable(""),
            Note: ko.observable(""),
            TotalPrice: ko.observable(""),
            BeginPrice: ko.observable(""),
            EndPrice: ko.observable(""),
            BeginAmount: ko.observable(1),
            EndAmount: ko.observable(999999999),
            Properties: ko.observableArray([]),
            TypeObj: ko.observable("hand"),
            ShopName: ko.observable("NULL"),
            ShopLink: ko.observable("NULL")
        };

        //oderDetail.ImageCss = ko.observable("");
        //oderDetail.Image.subscribe(function (newValue) {
        //    if (newValue == undefined || newValue == null || newValue == '') {
        //        oderDetail.ImageCss("border: 1px solid red");
        //        toastr.error('Thêm link ảnh sản phẩm');
        //    } else {
        //        oderDetail.ImageCss("");
        //    }
        //});

        oderDetail.LinkCss = ko.observable("");
        oderDetail.LinkFocus = ko.observable(false);
        oderDetail.Link.subscribe(function (newValue) {
            if (newValue == undefined || newValue == null || newValue == '') {
                oderDetail.LinkCss("error");
                oderDetail.LinkFocus(true);
                toastr.error(window.messager.error.linkProduct);
            } else {
                var checkLink = _.filter(self.listOrderDetail(), function (item) {
                    return (oderDetail.Link() === item.Link() && oderDetail.Size() === item.Size() && oderDetail.Color() === item.Color() && oderDetail.Id != item.Id && oderDetail.Properties() === item.Properties());
                });

                if (checkLink.length > 0) {
                    oderDetail.LinkCss("error");
                    oderDetail.SizeCss("error");
                    oderDetail.ColorCss("error");
                    oderDetail.LinkFocus(true);

                    toastr.error(window.messager.error.duplicateSize);
                    self.checkDuplicateLink(true);
                } else {
                    oderDetail.LinkCss("");
                    oderDetail.SizeCss("");
                    oderDetail.ColorCss("");
                    oderDetail.LinkFocus(false);
                    self.checkDuplicateLink(false);
                }
            }
        });

        oderDetail.SizeCss = ko.observable("");
        oderDetail.SizeFocus = ko.observable(false);
        //oderDetail.Size.subscribe(function (newValue) {
        //    if (newValue == undefined || newValue == null || newValue == '') {
        //        oderDetail.SizeCss("error");
        //        oderDetail.SizeFocus(true);
        //        toastr.error('Nhập size');
        //    } else {
        //        var checkLink = _.filter(self.listOrderDetail(), function (item) {
        //            return (oderDetail.Link() === item.Link() && oderDetail.Size() === item.Size() && oderDetail.Color() === item.Color() && oderDetail.Id != item.Id && oderDetail.Properties() === item.Properties());
        //        });

        //        if (checkLink.length > 0) {
        //            oderDetail.LinkCss("error");
        //            oderDetail.SizeCss("error");
        //            oderDetail.ColorCss("error");
        //            oderDetail.SizeFocus(true);

        //            toastr.error(window.messager.error.linkProduct);
        //            self.checkDuplicateLink(true);
        //        } else {
        //            oderDetail.LinkCss("");
        //            oderDetail.SizeCss("");
        //            oderDetail.ColorCss("");
        //            oderDetail.SizeFocus(false);
        //            self.checkDuplicateLink(false);
        //        }
        //    }
        //});

        oderDetail.ColorCss = ko.observable("");
        oderDetail.ColorFocus = ko.observable(false);
        //oderDetail.Color.subscribe(function (newValue) {
        //    if (newValue == undefined || newValue == null || newValue == '') {
        //        oderDetail.ColorCss("error");
        //        oderDetail.ColorFocus(true);
        //        toastr.error('Nhập màu sắc');
        //    } else {
        //        var checkLink = _.filter(self.listOrderDetail(), function (item) {
        //            return (oderDetail.Link() === item.Link() && oderDetail.Size() === item.Size() && oderDetail.Color() === item.Color() && oderDetail.Id != item.Id && oderDetail.Properties() === item.Properties());
        //        });

        //        if (checkLink.length > 0) {
        //            oderDetail.LinkCss("error");
        //            oderDetail.SizeCss("error");
        //            oderDetail.ColorCss("error");
        //            oderDetail.ColorFocus(true);

        //            toastr.error(window.messager.error.linkProduct);
        //            self.checkDuplicateLink(true);
        //        } else {
        //            oderDetail.LinkCss("");
        //            oderDetail.SizeCss("");
        //            oderDetail.ColorCss("");
        //            oderDetail.ColorFocus(false);
        //            self.checkDuplicateLink(false);
        //        }
        //    }
        //});

        oderDetail.QuantityCss = ko.observable("");
        oderDetail.QuantityFocus = ko.observable(false);
        oderDetail.Quantity.subscribe(function (newValue) {
            if (newValue == undefined || newValue == null || newValue == '' || newValue == 0) {
                oderDetail.QuantityCss("error");
                oderDetail.QuantityFocus(true);
                toastr.error(window.messager.error.numberProduct);
            } else {
                //var quantity = Globalize.parseFloat(oderDetail.Quantity() + '');
                //var endAmount = Globalize.parseFloat(oderDetail.EndAmount() + '');
                //var beginAmount = Globalize.parseFloat(oderDetail.BeginAmount() + '');

                //if (quantity > endAmount) {
                //    toastr.error('Số lượng đặt vượt quá số lượng shop có!');
                //    oderDetail.Quantity(endAmount);
                //}
                //if (quantity < beginAmount) {
                //    toastr.error('Số lượng đặt nhỏ hơn số lượng shop quy định!');
                //    oderDetail.Quantity(beginAmount);
                //}

                oderDetail.QuantityCss("");
                oderDetail.QuantityFocus(false);
                oderDetail.TotalPrice(Globalize.parseFloat(oderDetail.Price()) * Globalize.parseFloat(oderDetail.Quantity()));
                oderDetail.TotalPrice(formatNumberic(oderDetail.TotalPrice(), 'N2'));

                self.getTotalDetailHand();
            }
        });

        oderDetail.Price.subscribe(function (newValue) {
            oderDetail.TotalPrice(Globalize.parseFloat(oderDetail.Price()) * Globalize.parseFloat(oderDetail.Quantity()));
            oderDetail.TotalPrice(formatNumberic(oderDetail.TotalPrice(), 'N2'));
            self.getTotalDetailHand();
        });

        return oderDetail;
    }

    //tạo editer thêm link hình ảnh
    self.renderedHandler = function (elements, data) {
        $('#editableBtn' + data.Id).click(function (e) {
            e.stopPropagation();
            $('#editableImg' + data.Id).editable('toggle');
        });

        data.TotalPrice(Globalize.parseFloat(data.Price()) * Globalize.parseFloat(data.Quantity()));
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
            toastr.error(window.messager.error.linkProduct);
        }

        _.each(self.listOrderDetail(), function (oderDetail) {
            //if (oderDetail.Image() == undefined || oderDetail.Image() == null || oderDetail.Image() == '') {
            //    toastr.error('Thêm link ảnh sản phẩm');
            //    oderDetail.ImageCss("border: 1px solid red");
            //    check = false;
            //    return false;
            //} else {
            //    oderDetail.ImageCss("");
            //}

            if (oderDetail.Link() == undefined || oderDetail.Link() == null || oderDetail.Link() == '') {
                oderDetail.LinkCss("error");
                oderDetail.LinkFocus(true);
                toastr.error(window.messager.error.linkProduct);
                check = false;
                return false;
            }

            //if (oderDetail.Color() == undefined || oderDetail.Color() == null || oderDetail.Color() == '') {
            //    oderDetail.ColorCss("error");
            //    oderDetail.ColorFocus(true);
            //    toastr.error('Nhập màu sắc');
            //    check = false;
            //    return false;
            //}

            //if (oderDetail.Size() == undefined || oderDetail.Size() == null || oderDetail.Size() == '') {
            //    oderDetail.SizeCss("error");
            //    oderDetail.SizeFocus(true);
            //    toastr.error('Nhập size');
            //    check = false;
            //    return false;
            //}

            if (oderDetail.Quantity() == undefined || oderDetail.Quantity() == null || oderDetail.Quantity() == '') {
                oderDetail.QuantityCss("error");
                oderDetail.QuantityFocus(true);
                toastr.error(window.messager.error.numberProduct);
                check = false;
                return false;
            }
        });
        return check;
    }

    //bắt lỗi order
    self.checkOrder = function () {
        var check = true;
        _.each(self.listOrder(), function (oderDetail) {
            var checkLink = _.filter(self.listOrder(), function (item) {
                return (oderDetail.Link() === item.Link() && oderDetail.Size() === item.Size() && oderDetail.Color() === item.Color() && oderDetail.Id != item.Id && oderDetail.Properties() === item.Properties());
            });

            if (checkLink.length > 0) {
                oderDetail.LinkCss("error");
                oderDetail.SizeCss("error");
                oderDetail.ColorCss("error");
                oderDetail.SizeFocus(true);

                toastr.error(window.messager.error.linkProduct);
                check = false;
            }

            if (oderDetail.Link() == undefined || oderDetail.Link() == null || oderDetail.Link() == '') {
                oderDetail.LinkCss("error");
                oderDetail.LinkFocus(true);
                toastr.error(window.messager.error.linkProduct);
                check = false;
                return false;
            }

            //if (oderDetail.Color() == undefined || oderDetail.Color() == null || oderDetail.Color() == '') {
            //    oderDetail.ColorCss("error");
            //    oderDetail.ColorFocus(true);
            //    toastr.error('Nhập màu sắc');
            //    check = false;
            //    return false;
            //}

            //if (oderDetail.Size() == undefined || oderDetail.Size() == null || oderDetail.Size() == '') {
            //    oderDetail.SizeCss("error");
            //    oderDetail.SizeFocus(true);
            //    toastr.error('Nhập size');
            //    check = false;
            //    return false;
            //}

            if (oderDetail.Quantity() == undefined || oderDetail.Quantity() == null || oderDetail.Quantity() == '' || oderDetail.Quantity() == 0) {
                oderDetail.QuantityCss("error");
                oderDetail.QuantityFocus(true);
                toastr.error(window.messager.error.numberProduct);
                check = false;
                return false;
            }
        });
        return check;
    }

    //thêm chi tiết đơn hàng trên view
    self.addOrderDetail = function () {
        if (self.checkOrderDetail()) {
            if (self.listOrderDetail().length >= 50) {
                toastr.error(window.messager.error.maxLinkProduct);
            } else {
                self.listOrderDetail.push(self.addDetail());
                self.initInputMark();
            }
        }
    }

    //xóa chi tiết đơn hàng trên view
    self.removeOrderDetail = function (item) {
        self.listOrderDetail.remove(item);
        self.getTotalDetailHand();
    }

    //xóa chi tiết đơn hàng trên order
    self.removeOrder = function (item) {
        self.listOrder.remove(item);

        //tính lại giá trị
        var arrayLink = $.map(_.countBy(ko.mapping.toJS(self.listOrder()), 'Link'), function (value, index) {
            return [value];
        });

        var arrayShop = $.map(_.countBy(ko.mapping.toJS(self.listOrder()), 'ShopLink'), function (value, index) {
            return [value];
        });

        self.countLink(arrayLink.length);
        self.countShop(arrayShop.length);

        var cartCookie = ko.mapping.toJS(self.listOrder());
        //lưu giỏ hàng vào cookie
        $.cookie('cart-hand-' + window.customerId, JSON.stringify(cartCookie), { expires: 7 });
    }

    //thêm vào đơn hàng vào csdl
    self.saveOrder = function () {
        self.hideNotifile();
        if (self.listOrder().length == 0) {
            toastr.error(window.messager.error.notProduct);
        } else {
            if (self.checkOrder()) {
                self.isSubmit(false);

                _.each(self.listOrder(),
                    function (item) {
                        item.Properties = JSON.stringify(item.Properties());
                        item.Price = Globalize.parseFloat(item.Price());
                        item.Quantity = Globalize.parseFloat(item.Quantity());
                        item.Note = item.Note() == undefined ? '' : item.Note();
                    });

                $.post("/" + window.culture + "/CMS/Order/SaveOrder",
                    {
                        listOrderDetails: self.listOrder(),
                        listService: self.listService(),
                        wardDeliveryId: self.WardDeliveryId(),
                        __RequestVerifiCationToken: $("#orderAdd input[name='__RequestVerificationToken']").val()
                    },
                    function (result) {
                        if (result.status == msgType.error) {
                            toastr.error(result.msg);
                            self.isSubmit(true);
                        } else {
                            //xóa cookie giỏ hàng
                            $.cookie('cart-hand-' + window.customerId, undefined, { expires: 0 });

                            toastr.success(result.msg);
                            setTimeout(function () {
                                window.location.replace("/" + window.culture + "/CMS/Order/BuyOrder");
                            },
                                1000);
                        }
                    });
            }
        }
    }

    //Lấy link url
    self.getHTML = function () {
        self.isSubmit(false);
        self.isSuccess(true);
        self.product([]);
        self.listImg([]);
        self.listProperty([]);
        self.dataJson('');
        self.mes("");
        $.post("/" + window.culture + "/CMS/Order/AddLink",
        {
            url: self.url()
        },
            function (result) {
                if (result.status == msgType.error) {
                    //toastr.error(result.msg);
                    self.listOrderDetail([]);

                    var orderDetail = self.addDetail();
                    orderDetail.Link(self.url());

                    self.listOrderDetail([orderDetail]);

                    self.isSuccess(false);
                    self.initInputMark();
                } else {
                    try {
                        self.isSuccess(true);
                        result.pro.Price = ko.observable(result.pro.Price);
                        result.pro.PriceExchange = ko.observable(result.pro.Price() * window.rate);
                        self.product(result.pro);

                        self.listRange(JSON.parse(result.pro.ListRange));

                        self.dataJson(JSON.parse(JSON.stringify(eval("(" + result.pro.DataJson + ")"))));
                        window.dataJson = self.dataJson();
                        self.listImg(JSON.parse(result.pro.ListImage));

                        var listProperty = JSON.parse(result.pro.ListProperty);
                        self.website(result.website);

                        var index = 0;
                        //website world taobao
                        if (result.website == 0 && self.url().indexOf('world.taobao.com') !== -1) {
                            _.each(listProperty, function (item) {
                                if (listProperty.length == 1) {
                                    item.IsDetail = true;
                                    _.each(item.ChilProperty,
                                        function (chil) {
                                            chil.Quantity = ko.observable(0);

                                            chil.QuantityBook = ko.observable(formatNumberic(self.dataJson().skuInfo.skuMap[';' + chil.Properties + ';'].stock, 'N0'));
                                            chil.Price = ko.observable(formatNumberic(self.dataJson().skuInfo.skuMap[';' + chil.Properties + ';'].price, 'N2'));
                                            chil.CSS = ko.observable('size-chil');
                                            chil.CheckQuantity = function () {
                                                chil.CSS('size-select');
                                                //Xóa bỏ chọn
                                                _.each(item.ChilProperty, function (chilcheck) {
                                                    chilcheck.Quantity(0);
                                                    if (chilcheck.Id != chil.Id) {
                                                        chil.CSS('size-chil');
                                                    }
                                                });

                                                self.getTotalDetail();
                                            }
                                            chil.Note = ko.observable();
                                            chil.TotalPrice = ko.observable();
                                            chil.BeginAmount = ko.observable(self.listRange() == null ? 1 : self.listRange()[0].BeginAmount);
                                            chil.EndAmount = ko.observable(Globalize.parseFloat(chil.QuantityBook()));

                                            //check lại số lượng
                                            chil.UpdateQuantity = function () {
                                                if (Globalize.parseFloat(chil.Quantity()) < chil.BeginAmount() || Globalize.parseFloat(chil.Quantity()) > chil.EndAmount()) {
                                                    //chil.Quantity(formatNumberic(chil.BeginAmount(), 'N0'));
                                                    //toastr.error('');
                                                }

                                                self.getTotalDetail();
                                            };
                                        });
                                } else {
                                    if (index == 0) {
                                        item.IsDetail = false;
                                    } else {
                                        item.IsDetail = true;
                                    }
                                    var check = false;

                                    _.each(item.ChilProperty, function (chil) {
                                        chil.Quantity = ko.observable(0);
                                        chil.QuantityBook = ko.observable(0);
                                        chil.Price = ko.observable(0);
                                        if (index == 0) {
                                            chil.CSS = ko.observable(check == true ? 'size-chil' : 'size-select');
                                        }
                                        check = true;
                                        chil.CheckQuantity = function () {
                                            chil.CSS('size-select');
                                            //Xóa bỏ chọn
                                            _.each(item.ChilProperty, function (chilcheck) {
                                                chilcheck.Quantity(0);
                                                if (chilcheck.Properties != chil.Properties) {
                                                    chilcheck.CSS('size-chil');
                                                }
                                            });

                                            //tính lại số lượng
                                            _.each(listProperty, function (prop) {
                                                if (prop.IsDetail == true) {
                                                    _.each(prop.ChilProperty, function (chilProp) {
                                                        var quanty = self.dataJson().skuInfo.skuMap[';' + chil.Properties + ';' + chilProp.Properties + ';'];
                                                        var price = self.dataJson().skuInfo.skuMap[';' + chil.Properties + ';' + chilProp.Properties + ';'];
                                                        quanty = quanty == null ? 0 : quanty.stock;
                                                        price = price == null ? 0 : price.price;

                                                        chilProp.QuantityBook(formatNumberic(quanty, 'N0'));
                                                        chilProp.Price(formatNumberic(price, 'N2'));
                                                        chilProp.EndAmount(Globalize.parseFloat(chilProp.QuantityBook()));
                                                        chilProp.Quantity(0);
                                                    });
                                                }
                                            });

                                            self.getTotalDetail();
                                        }
                                        chil.Note = ko.observable();
                                        chil.TotalPrice = ko.observable();
                                        chil.BeginAmount = ko.observable(self.listRange() == null ? 1 : self.listRange()[0].BeginAmount);
                                        chil.EndAmount = ko.observable(0);

                                        //check lại số lượng
                                        chil.UpdateQuantity = function () {
                                            if (Globalize.parseFloat(chil.Quantity()) < chil.BeginAmount() || Globalize.parseFloat(chil.Quantity()) > chil.EndAmount()) {
                                                //chil.Quantity(formatNumberic(chil.BeginAmount(), 'N0'));
                                            }

                                            self.getTotalDetail();
                                        };
                                    });
                                }
                                index++;
                            });
                        }

                        //website item taobao
                        if (result.website == 0 && self.url().indexOf('world.taobao.com') === -1) {
                            _.each(listProperty, function (item) {
                                if (listProperty.length == 1) {
                                    item.IsDetail = true;
                                    _.each(item.ChilProperty,
                                        function (chil) {
                                            chil.Quantity = ko.observable(0);
                                            chil.QuantityBook = ko.observable('--');
                                            chil.Price = ko.observable(formatNumberic(self.dataJson().valItemInfo.skuMap[';' + chil.Properties + ';'].price, 'N2'));
                                            chil.CSS = ko.observable('size-chil');
                                            chil.CheckQuantity = function () {
                                                chil.CSS('size-select');
                                                //Xóa bỏ chọn
                                                _.each(item.ChilProperty, function (chilcheck) {
                                                    chilcheck.Quantity(0);
                                                    if (chilcheck.Id != chil.Id) {
                                                        chil.CSS('size-chil');
                                                    }
                                                });

                                                self.getTotalDetail();
                                            }
                                            chil.Note = ko.observable();
                                            chil.TotalPrice = ko.observable();


                                            chil.BeginAmount = ko.observable(self.listRange() == null ? 1 : self.listRange()[0].BeginAmount);
                                            chil.EndAmount = ko.observable(0);

                                            //check lại số lượng
                                            chil.UpdateQuantity = function () {
                                                if (Globalize.parseFloat(chil.Quantity()) < chil.BeginAmount()) {
                                                    //chil.Quantity(formatNumberic(chil.BeginAmount(), 'N0'));
                                                }

                                                self.getTotalDetail();
                                            };
                                        });
                                } else {
                                    if (index == 0) {
                                        item.IsDetail = false;
                                    } else {
                                        item.IsDetail = true;
                                    }
                                    var check = false;

                                    _.each(item.ChilProperty, function (chil) {
                                        chil.Quantity = ko.observable(0);
                                        chil.QuantityBook = ko.observable(0);
                                        chil.Price = ko.observable(0);
                                        if (index == 0) {
                                            chil.CSS = ko.observable(check == true ? 'size-chil' : 'size-select');
                                        }
                                        check = true;
                                        chil.CheckQuantity = function () {
                                            chil.CSS('size-select');
                                            //Xóa bỏ chọn
                                            _.each(item.ChilProperty, function (chilcheck) {
                                                chilcheck.Quantity(0);
                                                if (chilcheck.Properties != chil.Properties) {
                                                    chilcheck.CSS('size-chil');
                                                }
                                            });

                                            //tính lại số lượng
                                            _.each(listProperty, function (prop) {
                                                if (prop.IsDetail == true) {
                                                    _.each(prop.ChilProperty, function (chilProp) {
                                                        var quanty = self.dataJson().valItemInfo.skuMap[';' + chil.Properties + ';' + chilProp.Properties + ';'];
                                                        var price = self.dataJson().valItemInfo.skuMap[';' + chil.Properties + ';' + chilProp.Properties + ';'];
                                                        quanty = quanty == null ? 0 : quanty.stock;
                                                        price = price == null ? 0 : price.price;

                                                        //chilProp.QuantityBook(formatNumberic(quanty, 'N0'));
                                                        chil.QuantityBook('--');
                                                        chilProp.Price(formatNumberic(price, 'N2'));
                                                        chilProp.Quantity(0);
                                                    });
                                                }
                                            });

                                            self.getTotalDetail();
                                        }
                                        chil.Note = ko.observable();
                                        chil.TotalPrice = ko.observable();
                                        chil.BeginAmount = ko.observable(self.listRange() == null ? 1 : self.listRange()[0].BeginAmount);
                                        chil.EndAmount = ko.observable(0);

                                        //check lại số lượng
                                        chil.UpdateQuantity = function () {
                                            if (Globalize.parseFloat(chil.Quantity()) < chil.BeginAmount()) {
                                                //chil.Quantity(formatNumberic(chil.BeginAmount(), 'N0'));
                                            }

                                            self.getTotalDetail();
                                        };
                                    });
                                }
                                index++;
                            });
                        }

                        //website tmall
                        if (result.website == 2) {
                            self.product().Price(self.dataJson().detail.defaultItemPrice);
                            self.product().PriceExchange(self.product().Price() * window.rate);

                            _.each(listProperty, function (item) {
                                if (listProperty.length == 1) {
                                    item.IsDetail = true;
                                    _.each(item.ChilProperty,
                                        function (chil) {
                                            chil.Quantity = ko.observable(0);
                                            chil.QuantityBook = ko.observable(formatNumberic(self.dataJson().valItemInfo.skuMap[';' + chil.Properties + ';'].stock, 'N0'));
                                            chil.Price = ko.observable(formatNumberic(self.dataJson().valItemInfo.skuMap[';' + chil.Properties + ';'].price, 'N2'));
                                            chil.CSS = ko.observable('size-chil');
                                            chil.CheckQuantity = function () {
                                                chil.CSS('size-select');
                                                //Xóa bỏ chọn
                                                _.each(item.ChilProperty, function (chilcheck) {
                                                    chilcheck.Quantity(0);
                                                    if (chilcheck.Id != chil.Id) {
                                                        chil.CSS('size-chil');
                                                    }
                                                });

                                                self.getTotalDetail();
                                            }
                                            chil.Note = ko.observable();
                                            chil.TotalPrice = ko.observable();
                                            chil.BeginAmount = ko.observable(self.listRange() == null ? 1 : self.listRange()[0].BeginAmount);
                                            chil.EndAmount = ko.observable(Globalize.parseFloat(chil.QuantityBook()));

                                            //check lại số lượng
                                            chil.UpdateQuantity = function () {
                                                if (Globalize.parseFloat(chil.Quantity()) < chil.BeginAmount() || Globalize.parseFloat(chil.Quantity()) > chil.EndAmount()) {
                                                    //chil.Quantity(formatNumberic(chil.BeginAmount(), 'N0'));
                                                }

                                                self.getTotalDetail();
                                            };
                                        });
                                } else {
                                    if (index == 0) {
                                        item.IsDetail = false;
                                    } else {
                                        item.IsDetail = true;
                                    }
                                    var check = false;

                                    _.each(item.ChilProperty, function (chil) {
                                        chil.Quantity = ko.observable(0);
                                        chil.QuantityBook = ko.observable(0);
                                        chil.Price = ko.observable(0);
                                        if (index == 0) {
                                            chil.CSS = ko.observable(check == true ? 'size-chil' : 'size-select');
                                        }
                                        check = true;
                                        chil.CheckQuantity = function () {
                                            chil.CSS('size-select');
                                            //Xóa bỏ chọn
                                            _.each(item.ChilProperty, function (chilcheck) {
                                                chilcheck.Quantity(0);
                                                if (chilcheck.Properties != chil.Properties) {
                                                    chilcheck.CSS('size-chil');
                                                }
                                            });

                                            //tính lại số lượng
                                            _.each(listProperty, function (prop) {
                                                if (prop.IsDetail == true) {
                                                    _.each(prop.ChilProperty, function (chilProp) {
                                                        var quanty = self.dataJson().valItemInfo.skuMap[';' + chil.Properties + ';' + chilProp.Properties + ';'];
                                                        var price = self.dataJson().valItemInfo.skuMap[';' + chil.Properties + ';' + chilProp.Properties + ';'];
                                                        quanty = quanty == null ? 0 : quanty.stock;
                                                        price = price == null ? 0 : price.price;

                                                        chilProp.QuantityBook(formatNumberic(quanty, 'N0'));
                                                        chilProp.Price(formatNumberic(price, 'N2'));
                                                        chilProp.EndAmount(Globalize.parseFloat(chilProp.QuantityBook()));
                                                        chilProp.Quantity(0);
                                                    });
                                                }
                                            });

                                            self.getTotalDetail();
                                        }
                                        chil.Note = ko.observable();
                                        chil.TotalPrice = ko.observable();
                                        chil.BeginAmount = ko.observable(self.listRange() == null ? 1 : self.listRange()[0].BeginAmount);
                                        chil.EndAmount = ko.observable(0);

                                        //check lại số lượng
                                        chil.UpdateQuantity = function () {
                                            if (Globalize.parseFloat(chil.Quantity()) < chil.BeginAmount() || Globalize.parseFloat(chil.Quantity()) > chil.EndAmount()) {
                                                //chil.Quantity(formatNumberic(chil.BeginAmount(), 'N0'));
                                            }

                                            self.getTotalDetail();
                                        };
                                    });
                                }
                                index++;
                            });
                        }

                        //website 1688
                        if (result.website == 1) {
                            _.each(listProperty, function (item) {
                                if (listProperty.length == 1) {
                                    item.IsDetail = true;
                                    _.each(item.ChilProperty,
                                        function (chil) {
                                            chil.Quantity = ko.observable(0);
                                            chil.QuantityBook = ko.observable(formatNumberic(self.dataJson().sku.skuMap[chil.Properties].canBookCount, 'N0'));
                                            var price = self.dataJson().sku.discountPrice;
                                            if (price == null || price == '' || price == undefined) {
                                                if (self.dataJson().sku.priceRange == null || self.dataJson().sku.priceRange == '' || self.dataJson().sku.priceRange == undefined) {
                                                    //check map
                                                    if (self.dataJson().sku.skuMap == null || self.dataJson().sku.skuMap == '' || self.dataJson().sku.skuMap == undefined) {
                                                        price = self.listRange()[0].Price;
                                                    } else {
                                                        //check price
                                                        if (self.dataJson().sku.skuMap[chil.Properties].price == null || self.dataJson().sku.skuMap[chil.Properties].price == '' || self.dataJson().sku.skuMap[chil.Properties].price == undefined) {
                                                            price = self.listRange()[0].Price;
                                                        } else {
                                                            price = self.dataJson().sku.skuMap[chil.Properties].price;
                                                        }
                                                    }
                                                } else {
                                                    price = self.dataJson().sku.priceRange[0][1];
                                                }
                                            }
                                            chil.Price = ko.observable(formatNumberic(price, 'N2'));
                                            chil.CSS = ko.observable('size-chil');
                                            chil.CheckQuantity = function () {
                                                chil.CSS('size-select');
                                                //Xóa bỏ chọn
                                                _.each(item.ChilProperty, function (chilcheck) {
                                                    chilcheck.Quantity(0);
                                                    if (chilcheck.Id != chil.Id) {
                                                        chil.CSS('size-chil');
                                                    }
                                                });

                                                self.getTotalDetail();
                                            }
                                            chil.Note = ko.observable();
                                            chil.TotalPrice = ko.observable();
                                            chil.BeginAmount = ko.observable(self.listRange() == null ? 1 : self.listRange()[0].BeginAmount);
                                            chil.EndAmount = ko.observable(Globalize.parseFloat(chil.QuantityBook()));

                                            //check lại số lượng
                                            chil.UpdateQuantity = function () {
                                                if (Globalize.parseFloat(chil.Quantity()) < chil.BeginAmount() || Globalize.parseFloat(chil.Quantity()) > chil.EndAmount()) {
                                                    //chil.Quantity(formatNumberic(chil.BeginAmount(), 'N0'));
                                                }

                                                //lấy lại giá theo khoảng giá
                                                var quantity = Globalize.parseFloat(chil.Quantity());
                                                _.each(self.listRange(), function (range) {
                                                    if (quantity >= range.BeginAmount && quantity <= range.EndAmount) {
                                                        chil.Price(formatNumberic(range.BeginPrice, 'N2'));
                                                    }
                                                });

                                                self.getTotalDetail();
                                            };
                                        });
                                } else {
                                    if (index == 0) {
                                        item.IsDetail = false;
                                    } else {
                                        item.IsDetail = true;
                                    }
                                    var check = false;

                                    _.each(item.ChilProperty, function (chil) {
                                        chil.Quantity = ko.observable(0);
                                        chil.QuantityBook = ko.observable(0);
                                        chil.Price = ko.observable(0);
                                        if (index == 0) {
                                            chil.CSS = ko.observable(check == true ? 'size-chil' : 'size-select');
                                        }
                                        check = true;
                                        chil.CheckQuantity = function () {
                                            chil.CSS('size-select');
                                            //Xóa bỏ chọn
                                            _.each(item.ChilProperty, function (chilcheck) {
                                                if (chilcheck.Properties != chil.Properties) {
                                                    chilcheck.CSS('size-chil');
                                                }
                                            });

                                            //tính lại số lượng
                                            _.each(listProperty, function (prop) {
                                                if (prop.IsDetail == true) {
                                                    _.each(prop.ChilProperty, function (chilProp) {
                                                        var obj = self.dataJson().sku.skuMap[chil.Properties + '&gt' + chilProp.Properties];
                                                        var quanty = obj == null ? 0 : obj.canBookCount;

                                                        var price = self.dataJson().sku.discountPrice;
                                                        if (price == null || price == '' || price == undefined) {
                                                            if (self.dataJson().sku.priceRange == null || self.dataJson().sku.priceRange == '' || self.dataJson().sku.priceRange == undefined) {
                                                                price = obj == null ? 0 : obj.price;
                                                            } else {
                                                                price = self.dataJson().sku.priceRange[0][1];
                                                            }
                                                        }

                                                        chilProp.QuantityBook(formatNumberic(quanty, 'N0'));
                                                        chilProp.Price(formatNumberic(price, 'N2'));
                                                        chilProp.EndAmount(Globalize.parseFloat(chilProp.QuantityBook()));
                                                        chilProp.Quantity(0);
                                                    });
                                                }
                                            });

                                            self.getTotalDetail();
                                        }
                                        chil.Note = ko.observable();
                                        chil.TotalPrice = ko.observable();
                                        chil.BeginAmount = ko.observable(self.listRange() == null ? 1 : self.listRange()[0].BeginAmount);
                                        chil.EndAmount = ko.observable(0);

                                        //check lại số lượng
                                        chil.UpdateQuantity = function () {
                                            if (Globalize.parseFloat(chil.Quantity()) < chil.BeginAmount() || Globalize.parseFloat(chil.Quantity()) > chil.EndAmount()) {
                                                //chil.Quantity(formatNumberic(chil.BeginAmount(), 'N0'));
                                            }

                                            //lấy lại giá theo khoảng giá
                                            var quantity = Globalize.parseFloat(chil.Quantity());
                                            _.each(self.listRange(), function (range) {
                                                if (quantity >= range.BeginAmount && quantity <= range.EndAmount) {
                                                    chil.Price(formatNumberic(range.BeginPrice, 'N2'));
                                                }
                                            });

                                            self.getTotalDetail();
                                        };
                                    });
                                }
                                index++;
                            });

                            if (listProperty.length == 0) {
                                var property;
                                if (self.dataJson().sku == '' || self.dataJson().sku == undefined) {

                                    var json = JSON.parse(self.product().SingerDataJson);

                                    var quantityBook = json.max;
                                    var price = self.listRange()[0].BeginPrice;
                                    property = {
                                        IsDetail: true,
                                        Title: "其他",
                                        Type: "other",
                                        ChilProperty: [
                                            {
                                                Quantity: ko.observable(0),
                                                QuantityBook: ko.observable(formatNumberic(quantityBook, 'N0')),
                                                Price: ko.observable(formatNumberic(price, 'N2')),
                                                CSS: ko.observable('size-chil'),
                                                Title: '',
                                                Type: 'img',
                                                Img: self.product().Image,
                                                Note: ko.observable(""),
                                                TotalPrice: ko.observable(""),
                                                BeginPrice: ko.observable(""),
                                                EndPrice: ko.observable(""),
                                                BeginAmount: ko.observable(json.min),
                                                EndAmount: ko.observable(quantityBook)
                                            }
                                        ]
                                    };
                                } else {
                                    var priceRange = self.dataJson().sku.priceRange;
                                    if (priceRange != null && priceRange != '' && priceRange != undefined) {
                                        priceRange = priceRange[0][1];
                                    } else {
                                        priceRange = self.dataJson().sku.discountPrice;
                                    }
                                    property = {
                                        IsDetail: true,
                                        Title: "其他",
                                        Type: "other",
                                        ChilProperty: [
                                            {
                                                Quantity: ko.observable(0),
                                                QuantityBook: ko.observable(formatNumberic(self.dataJson().sku.canBookCount, 'N0')),
                                                Price: ko.observable(formatNumberic(priceRange, 'N2')),
                                                CSS: ko.observable('size-chil'),
                                                Title: '',
                                                Type: 'img',
                                                Img: self.product().Image,
                                                Note: ko.observable(""),
                                                TotalPrice: ko.observable(""),
                                                BeginPrice: ko.observable(""),
                                                EndPrice: ko.observable(""),
                                                BeginAmount: ko.observable(self.listRange() == null ? 1 : self.listRange()[0].BeginAmount),
                                                EndAmount: ko.observable(self.dataJson().sku.canBookCount)
                                            }
                                        ]
                                    };
                                }

                                //check lại số lượng
                                property.ChilProperty[0].UpdateQuantity = function () {
                                    if (Globalize.parseFloat(property.ChilProperty[0].Quantity()) < property.ChilProperty[0].BeginAmount() || Globalize.parseFloat(property.ChilProperty[0].Quantity()) > property.ChilProperty[0].EndAmount()) {
                                        //property.ChilProperty[0].Quantity(formatNumberic(property.ChilProperty[0].BeginAmount(), 'N0'));
                                    }

                                    //lấy lại giá theo khoảng giá
                                    var quantity = Globalize.parseFloat(property.ChilProperty[0].Quantity());
                                    _.each(self.listRange(), function (range) {
                                        if (quantity >= range.BeginAmount && quantity <= range.EndAmount) {
                                            property.ChilProperty[0].Price(formatNumberic(range.BeginPrice, 'N2'));
                                        }
                                    });

                                    self.getTotalDetail();
                                };

                                listProperty.push(property);
                            }
                        }

                        self.product().Price(formatNumberic(self.product().Price(), 'N2'));
                        self.product().PriceExchange(formatNumberic(self.product().PriceExchange(), 'N2'));

                        self.listProperty(listProperty);
                        self.initInputMark();
                    }
                    catch (err) {
                        self.listOrderDetail([]);

                        var orderDetail = self.addDetail();
                        orderDetail.Link(self.url());

                        self.listOrderDetail([orderDetail]);

                        self.isSuccess(false);
                        self.initInputMark();
                    }
                }
                self.isSubmit(true);
            });
    }

    // thêm chi tiết đơn hàng bằng tay
    self.addDetaiOrder = function () {
        self.mes("");
        self.isSubmit(false);
        if (self.checkOrderDetail()) {
            _.each(self.listOrderDetail(),
                function (item) {
                    if (self.listOrder().length >= 50) {
                        toastr.error(window.messager.error.maxLinkProduct);
                        return false;
                    } else {
                        self.listOrder.push(item);
                    }
                });

            var arrayLink = $.map(_.countBy(ko.mapping.toJS(self.listOrder()), 'Link'), function (value, index) {
                return [value];
            });

            var arrayShop = $.map(_.countBy(ko.mapping.toJS(self.listOrder()), 'ShopLink'), function (value, index) {
                return [value];
            });

            self.countLink(arrayLink.length);
            self.countShop(arrayShop.length);

            self.listOrderDetail([self.addDetail()]);
            self.initInputMark();
            window.flyToCartHand();

            var cartCookie = ko.mapping.toJS(self.listOrder());
            //lưu vào cookie
            $.cookie('cart-hand-' + window.customerId, JSON.stringify(cartCookie), { expires: 7 });
        }
        self.isSubmit(true);
    }

    //thêm chi tiết đơn hàng bằng link
    self.addDetaiOrderToUrl = function () {
        self.isSubmit(false);

        var init = 0;
        var check = false;
        //Thêm vào đơn hàng
        var img = "";
        var listPropertiesOther = [];
        var listProperties = [];

        _.each(ko.mapping.toJS(self.listProperty()), function (item) {
            _.each(item.ChilProperty, function (o) {
                //Thêm thuộc tính
                var propertie;
                if (item.IsDetail == false && o.CSS == 'size-select') {
                    propertie = {
                        Label: (item.Type == 'color' ? window.messager.infor.color: item.Type == 'size' ? 'Size' : window.messager.infor.orther) + ' (' + item.Title + ')',
                        Text: o.Title
                    };
                    listPropertiesOther.push(propertie);

                    if (o.Type == 'img') {
                        img = o.Img;
                    }
                }

                if (Globalize.parseFloat(o.Quantity) > 0 && item.IsDetail == true) {
                    //Thêm thuộc tính
                    propertie = {
                        Label: (item.Type == 'color' ? window.messager.infor.color : item.Type == 'size' ? 'Size' : window.messager.infor.orther) + ' (' + item.Title + ')',
                        Text: o.Title
                    };
                    listProperties.push(propertie);

                    if (o.Type == 'img') {
                        img = o.Img;
                    }

                    var oderDetail = self.addDetail();

                    oderDetail.Id = (new Date()).getTime() + '_' + init;
                    oderDetail.Name(self.product().Name);
                    oderDetail.Image(img == '' ? self.product().Image : img);
                    oderDetail.Link(self.product().ProLink);
                    oderDetail.Size(window.messager.infor.defaultValue);
                    oderDetail.Color(window.messager.infor.defaultValue);
                    oderDetail.Price(o.Price);
                    oderDetail.Quantity(o.Quantity);
                    oderDetail.Note(o.Note);
                    oderDetail.ShopName(self.product().ShopNick);
                    oderDetail.ShopLink(self.product().ShopLink);
                    oderDetail.BeginAmount(o.BeginAmount);
                    oderDetail.EndAmount(o.QuantityBook);
                    oderDetail.Properties(listPropertiesOther.concat(listProperties));
                    oderDetail.TypeObj("link");

                    //kiểm tra đơn có vượt quá 50 link không
                    if (self.listOrder().length >= 50) {
                        toastr.error(window.messager.error.maxLinkProduct);
                        return false;
                    } else {

                        //kiểm tra có trùng link và thuộc tính
                        var checkLink = _.filter(self.listOrder(), function (it) {
                            var p1 = JSON.stringify(oderDetail.Properties());
                            var p2 = JSON.stringify(it.Properties());
                            return (oderDetail.Link() === it.Link() && p1 === p2);
                        });

                        //trùng thì cộng số lượng lên
                        if (checkLink.length > 0) {
                            _.each(self.listOrder(), function (od) {
                                var p1 = JSON.stringify(oderDetail.Properties());
                                var p2 = JSON.stringify(od.Properties());

                                if (oderDetail.Link() === od.Link() && p1 === p2) {
                                    var total = Globalize.parseFloat(od.Quantity()) + Globalize.parseFloat(oderDetail.Quantity());

                                    if (total > Globalize.parseFloat(od.EndAmount())) {
                                        toastr.error('The number of orders exceeds the number of shops available!');
                                        return false;
                                    } else {
                                        od.Quantity(formatNumberic(total, 'N0'));
                                    }
                                }
                            });
                        } else {
                            self.listOrder.push(oderDetail);
                        }
                    }

                    //tính lại giá trị
                    var arrayLink = $.map(_.countBy(ko.mapping.toJS(self.listOrder()), 'Link'), function (value, index) {
                        return [value];
                    });

                    var arrayShop = $.map(_.countBy(ko.mapping.toJS(self.listOrder()), 'ShopLink'), function (value, index) {
                        return [value];
                    });

                    self.countLink(arrayLink.length);
                    self.countShop(arrayShop.length);

                    //khởi tạo lại giá trị
                    listProperties = [];
                    init++;

                    self.initInputMark();

                    if (!check) {
                        window.flyToCart();
                        check = true;
                    }
                }
            });
        });

        self.getTotalDetail();

        var cartCookie = ko.mapping.toJS(self.listOrder());
        //lưu vào cookie
        $.cookie('cart-hand-' + window.customerId, JSON.stringify(cartCookie), { expires: 7 });

        self.isSubmit(true);
    }

    self.getTotalDetail = function () {
        var sum = 0;
        var total = 0;

        _.each(ko.mapping.toJS(self.listProperty()),
               function (item) {
                   sum += _.sumBy(item.ChilProperty, function (o) { return Globalize.parseFloat(o.Quantity); });
                   total += _.sumBy(item.ChilProperty, function (o) { return Globalize.parseFloat(o.Quantity) * Globalize.parseFloat(o.Price); });
               });

        if (sum == 0) {
            self.mes(window.messager.infor.selectProductInOrder);
        } else {
            self.mes(window.messager.infor.totalProduct + formatNumberic(sum, 'N2') + window.messager.infor.totalMoney + formatNumberic(total, 'N2') + " ¥" + window.messager.infor.totalMoneyExcharge + formatNumberic(total * window.rate, 'N2') + window.messager.infor.curruncy);
        }
        self.initInputMark();
    };

    self.getTotalDetailHand = function () {
        var sum = 0;
        var total = 0;

        _.each(ko.mapping.toJS(self.listOrderDetail()),
               function (item) {
                   sum += Globalize.parseFloat(item.Quantity);
                   total += Globalize.parseFloat(item.Quantity) * Globalize.parseFloat(item.Price);
               });

        if (sum == 0) {
            //self.mes(window.messager.infor.selectProductInOrder);
        } else {
            self.mes(window.messager.infor.totalProduct + formatNumberic(sum, 'N2') + window.messager.infor.totalMoney + formatNumberic(total, 'N2') + " ¥" + window.messager.infor.totalMoneyExcharge + formatNumberic(total * window.rate, 'N2') + window.messager.infor.curruncy);
        }
        self.initInputMark();
    };

    self.showNotifile = function () {
        self.saveOrder();
    }

    self.hideNotifile = function () {
        $('#delete-box').fadeOut(300);
    }
}

var viewModel = new OrderAddViewModel();
ko.applyBindings(viewModel, $("#orderAdd")[0]);