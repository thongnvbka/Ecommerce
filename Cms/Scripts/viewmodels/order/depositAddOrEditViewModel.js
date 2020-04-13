function DepositAddOrEditViewModel() {
    var self = this;

    self.titleDeposit = ko.observable();
    self.isDeposit = ko.observable(true);
    self.customer = ko.observableArray([]);
    self.listDetail = ko.observableArray([]);
    self.exchangeRate = ko.observable(0);
    self.isDetailRending = ko.observable(true);
    self.listWarehouseDeposit = ko.observableArray([]);
    self.listWarehouseDelivery = ko.observableArray([]);
    self.isSubmit = ko.observable(true);
    self.isUpload = ko.observable(true);
    self.isAdd = ko.observable(true);
    self.isModelAdd = ko.observable(true);
    self.codePackage = ko.observable();
    self.listPackageView = ko.observableArray([]);

    self.listOrderService = ko.observableArray([]);
    self.listOrderServiceOther = ko.observableArray([]);
    self.listOrderServiceCheck = ko.observableArray([]);

    //model
    self.Id = ko.observable();
    self.Code = ko.observable();
    self.CreateDate = ko.observable(new Date());
    self.UpdateDate = ko.observable();
    self.CustomerId = ko.observable();
    self.CustomerName = ko.observable();
    self.CustomerEmail = ko.observable();
    self.CustomerPhone = ko.observable();
    self.CustomerAddress = ko.observable();
    self.LevelId = ko.observable();
    self.LevelName = ko.observable();
    self.Note = ko.observable();
    self.ContactName = ko.observable();
    self.ContactPhone = ko.observable();
    self.ContactAddress = ko.observable();
    self.ContactEmail = ko.observable();
    self.UserId = ko.observable();
    self.UserName = ko.observable();
    self.Type = ko.observable();
    self.PacketNumber = ko.observable();
    self.Description = ko.observable();
    self.Status = ko.observable(0);
    self.SystemId = ko.observable();
    self.SystemName = ko.observable();
    self.WarehouseId = ko.observable();
    self.WarehouseName = ko.observable();
    self.WarehouseDeliveryId = ko.observable();
    self.WarehouseDeliveryName = ko.observable();
    self.ProvisionalMoney = ko.observable();
    self.TotalWeight = ko.observable();
    self.Currency = ko.observable();
    self.ExchangeRate = ko.observable();
    self.UnsignName = ko.observable();
    self.ReasonCancel = ko.observable();
    self.DepositType = ko.observable();
    self.ApprovelUnit = ko.observable('Weight');
    self.ApprovelPrice = ko.observable();
    self.OrderInfoId = ko.observable();
    self.FeeShip = ko.observable();

    self.setData = function (data) {
        self.Id(data.Id);
        self.Code(data.Code);
        self.CreateDate(data.CreateDate);
        self.UpdateDate(data.UpdateDate);
        self.CustomerId(data.CustomerId);
        self.CustomerName(data.CustomerName);
        self.CustomerEmail(data.CustomerEmail);
        self.CustomerPhone(data.CustomerPhone);
        self.CustomerAddress(data.CustomerAddress);
        self.LevelId(data.LevelId);
        self.LevelName(data.LevelName);
        self.Note(data.Note);
        self.ContactName(data.ContactName);
        self.ContactPhone(data.ContactPhone);
        self.ContactAddress(data.ContactAddress);
        self.ContactEmail(data.ContactEmail);
        self.UserId(data.UserId);
        self.UserName(data.UserName);
        self.Type(data.Type);
        self.PacketNumber(data.PacketNumber);
        self.Description(data.Description);
        self.Status(data.Status);
        self.SystemId(data.SystemId);
        self.SystemName(data.SystemName);
        self.WarehouseId(data.WarehouseId);
        self.WarehouseName(data.WarehouseName);
        self.WarehouseDeliveryId(data.WarehouseDeliveryId);
        self.WarehouseDeliveryName(data.WarehouseDeliveryName);
        self.ProvisionalMoney(data.ProvisionalMoney);
        self.TotalWeight(data.TotalWeight);
        self.Currency(data.Currency);
        self.ExchangeRate(data.ExchangeRate);
        self.UnsignName(data.UnsignName);
        self.ReasonCancel(data.ReasonCancel);
        self.DepositType(data.DepositType);
        self.ApprovelUnit(data.ApprovelUnit === null ? 'Weight' : data.ApprovelUnit);
        self.ApprovelPrice(data.ApprovelPrice);
        self.OrderInfoId(data.OrderInfoId);
        self.FeeShip(data.FeeShip);
    }

    self.resetModel = function () {
        self.Id("");
        self.Code("");
        self.CreateDate(new Date());
        self.UpdateDate("");
        self.CustomerId("");
        self.CustomerName("");
        self.CustomerEmail("");
        self.CustomerPhone("");
        self.CustomerAddress("");
        self.LevelId("");
        self.LevelName("");
        self.Note("");
        self.ContactName("");
        self.ContactPhone("");
        self.ContactAddress("");
        self.ContactEmail("");
        self.UserId("");
        self.UserName("");
        self.Type("");
        self.PacketNumber("");
        self.Description("");
        self.Status(0);
        self.SystemId("");
        self.SystemName("");
        self.WarehouseId("");
        self.WarehouseName("");
        self.WarehouseDeliveryId("");
        self.WarehouseDeliveryName("");
        self.ProvisionalMoney("");
        self.TotalWeight("");
        self.Currency("");
        self.ExchangeRate("");
        self.UnsignName("");
        self.ReasonCancel("");
        self.DepositType("");
        self.ApprovelUnit('Weight');
        self.ApprovelPrice("");
        self.OrderInfoId("");
        self.FeeShip("");

        self.customer([]);
        self.listDetail([]);
        self.codePackage();
        self.listPackageView([]);
        self.listOrderService([]);
        self.listHistory([]);
        self.listOrderServiceOther([]);
    }

    //Detail Orders

    self.depositDetailId = ko.observable();
    self.depositDetailCreateDate = ko.observable();
    self.depositDetailDepositId = ko.observable();
    self.depositDetailLadingCode = ko.observable();
    self.depositDetailWeight = ko.observable();
    self.depositDetailCategoryId = ko.observable();
    self.depositDetailCategoryName = ko.observable();
    self.depositDetailProductName = ko.observable();
    self.depositDetailQuantity = ko.observable();
    self.depositDetailSize = ko.observable();
    self.depositDetailImage = ko.observable();
    self.depositDetailNote = ko.observable();
    self.depositDetailPacketNumber = ko.observable();
    self.depositDetailLong = ko.observable();
    self.depositDetailHigh = ko.observable();
    self.depositDetailWide = ko.observable();
    self.depositDetailListCode = ko.observable();
    self.depositShipTq = ko.observable();

    self.setDepositDetail = function (data) {
        self.depositDetailId(data.Id);
        self.depositDetailCreateDate(data.CreateDate);
        self.depositDetailDepositId(data.depositId);
        self.depositDetailLadingCode(data.LadingCode);
        self.depositDetailWeight(data.Weight);
        self.depositDetailCategoryId(data.CategoryId);
        self.depositDetailCategoryName(data.CategoryName);
        self.depositDetailProductName(data.ProductName);
        self.depositDetailQuantity(data.Quantity);
        self.depositDetailSize(data.Size);
        self.depositDetailImage(data.Image);
        self.depositDetailNote(data.Note);
        self.depositDetailPacketNumber(data.PacketNumber);
        self.depositDetailLong(data.Long);
        self.depositDetailHigh(data.High);
        self.depositDetailWide(data.Wide);
        self.depositDetailListCode(data.ListCode);
        self.depositShipTq(data.ShipTq);
    }

    self.resetDepositDetail = function () {
        self.depositDetailId("");
        self.depositDetailCreateDate("");
        self.depositDetailDepositId("");
        self.depositDetailLadingCode("");
        self.depositDetailWeight("");
        self.depositDetailCategoryId("");
        self.depositDetailCategoryName("");
        self.depositDetailProductName("");
        self.depositDetailQuantity("");
        self.depositDetailSize("");
        self.depositDetailImage(undefined);
        self.depositDetailNote("");
        self.depositDetailPacketNumber("");
        self.depositDetailLong("");
        self.depositDetailHigh("");
        self.depositDetailWide("");
        self.depositDetailListCode("");
        self.depositShipTq("");
    }

    self.showModalDialog = function (id) {
        //$(".customer-search-add").empty().trigger("change");
        self.resetModel();
        self.initInputMark();
        self.isDetailRending(false);
        self.isShowHistory(false);
        self.userOrder([]);

        $.post("/Deposit/GetData", function (result) {
            if (result.status === msgType.success) {
                self.exchangeRate(result.exchangeRate);
                self.listWarehouseDeposit(result.listWarehouseDeposit);
                self.listWarehouseDelivery(result.listWarehouseDelivery);

                self.listOrderServiceCheck([]);
                _.each(result.listOrderServiceCheck,
                    function (item) {

                        item.Checked = ko.observable(item.Checked);

                        self.listOrderServiceCheck.push(item);
                    });
            } else {
                toastr.error(result.msg);
                self.isDetailRending(true);
            }
        });

        if (id > 0) {
            self.isModelAdd(false);
            $.post("/Deposit/GetDepositDetail", { id: id }, function (result) {
                if (result.status === msgType.success) {
                    self.setData(result.deposit);
                    self.customer(result.customer);
                    self.listDetail(result.listDetail);
                    self.listHistory(result.listHistory);
                    self.userOrder(result.userOrder);
                    self.listOrderService(result.listOrderService);
                    self.listOrderServiceOther(result.listOrderServiceOther);

                    self.listOrderServiceCheck([]);
                    _.each(result.listOrderServiceCheck,
                        function (item) {

                            item.Checked = ko.observable(item.Checked);

                            self.listOrderServiceCheck.push(item);
                        });

                    _.each(result.listPackageView,
                      function (it) {
                          it.cacheTransportCode = it.TransportCode;
                          it.cacheForcastDate = it.ForcastDate;
                          it.cacheWeight = it.Weight;
                          it.cacheWidth = it.Width;
                          it.cacheHeight = it.Height;
                          it.cacheLength = it.Length;

                          it.TransportCode = ko.observable(it.TransportCode);
                          it.TransportCode.subscribe(function (newValue) {
                              if (it.cacheTransportCode !== newValue) {
                                  self.updatePackage(ko.mapping.toJS(it));
                                  it.cacheTransportCode = newValue;
                              }
                          });

                          it.ForcastDate = ko.observable(it.ForcastDate ? moment(it.ForcastDate).format("L") : '');
                          it.ForcastDate.subscribe(function (newValue) {
                              if (it.cacheForcastDate !== newValue) {
                                  self.updatePackage(ko.mapping.toJS(it));
                                  it.cacheForcastDate = newValue;
                              }
                          });

                          it.Weight = ko.observable(formatNumberic(it.Weight, "N4"));
                          it.Weight.subscribe(function (newValue) {
                              if (it.cacheWeight !== newValue) {
                                  self.updatePackage(ko.mapping.toJS(it));
                                  it.cacheWeight = newValue;
                              }
                          });

                          it.Width = ko.observable(formatNumberic(it.Width, "N4"));
                          it.Width.subscribe(function (newValue) {
                              if (it.cacheWidth !== newValue) {
                                  self.updatePackage(ko.mapping.toJS(it));
                                  it.cacheWidth = newValue;
                              }
                          });

                          it.Height = ko.observable(formatNumberic(it.Height, "N4"));
                          it.Height.subscribe(function (newValue) {
                              if (it.cacheHeight !== newValue) {
                                  self.updatePackage(ko.mapping.toJS(it));
                                  it.cacheHeight = newValue;
                              }
                          });

                          it.Length = ko.observable(formatNumberic(it.Length, "N4"));
                          it.Length.subscribe(function (newValue) {
                              if (it.cacheLength !== newValue) {
                                  self.updatePackage(ko.mapping.toJS(it));
                                  it.cacheLength = newValue;
                              }
                          });
                      });
                    self.listPackageView(result.listPackageView);

                    self.isDetailRending(true);
                    $("#orderDepositAddOrEdit").modal();

                    $('#addOrEditDepositDetailModal')
                       .on('hide.bs.modal',
                       function (e) {
                           $(".search-list").trigger('click');
                       });

                    $(".select-view").select2();

                    self.viewBoxChat.showChat(self.Id(), self.Code(), self.Type(), 1);

                    $(".view-chat-box").show();

                    $('.datepicker')
                       .datepicker({
                           autoclose: true,
                           language: 'en',
                           format: 'dd/mm/yyyy',
                           startDate: new Date()
                       });

                } else {
                    toastr.error(result.msg);
                    self.isDetailRending(true);
                }

                $(".customer-search-add")
                    .empty()
                    .append($("<option/>").val(result.deposit.CustomerId).text(result.deposit.CustomerName))
                    .val(result.deposit.CustomerId)
                    .trigger("change");

                self.updateGeneral();

                self.orderCodeWarehouse(self.WarehouseDeliveryId(), self.Code());
            });
        } else {
            self.isDetailRending(true);
            self.isModelAdd(true);
            $("#orderDepositAddOrEdit").modal();
            $(".customer-search-add").empty();
            $(".view-chat-box").hide();
        }
    }

    self.showAddDetail = function () {
        self.isAdd(true);
        self.resetDepositDetail();
        $("#addOrEditDepositDetailModal").modal();

        $('#addOrEditDepositDetailModal')
                .on('hide.bs.modal',
                function (e) {
                    $(".search-list").trigger('click');
                });


        self.initInputMark();

        $('.dropdownjstree').remove();

        $("#category_tree").dropdownjstree({
            source: window.categoryJsTree,
            selectedNode: self.depositDetailCategoryId(),
            selectNote: (node, selected) => {
                self.depositDetailCategoryId(selected.node.id);
                self.depositDetailCategoryName(selected.node.text);
            }
        });
    }

    self.WarehouseDeliveryId.subscribe(function (value) {
        if (self.isModelAdd() === false) {
            if (value !== null && value !== undefined && value !== '') {
                self.orderCodeWarehouse(self.WarehouseDeliveryId(), self.Code());
            }
        }
    });

    self.showEditDetail = function (data) {
        self.isAdd(false);
        self.resetDepositDetail();
        self.setDepositDetail(data);
        $("#addOrEditDepositDetailModal").modal();
        self.initInputMark();

        $('.dropdownjstree').remove();

        $("#category_tree")
            .dropdownjstree({
                source: window.categoryJsTree,
                selectedNode: self.depositDetailCategoryId(),
                selectNote: (node, selected) => {
                    self.depositDetailCategoryId(selected.node.id);
                    self.depositDetailCategoryName(selected.node.text);
                }
            });
    };

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
                $.post("/Deposit/DeleteDetail", { id: data.Id }, function (result) {
                    if (result.status === msgType.success) {
                        self.listDetail.remove(data);
                        toastr.success(result.msg);
                        self.updateGeneral();
                    } else {
                        toastr.error(result.msg);
                    }
                });
            }
            self.updateGeneral();
        }, function () { });
    };

    self.saveDetail = function () {
        if (!self.checkDetail()) {
            return;
        };

        if (self.isAdd()) {
            var detail = {
                Id: (new Date()).getTime(),
                LadingCode: self.depositDetailLadingCode(),
                Weight: self.depositDetailWeight(),
                CategoryId: self.depositDetailCategoryId(),
                CategoryName: self.depositDetailCategoryName(),
                ProductName: self.depositDetailProductName(),
                Quantity: self.depositDetailQuantity(),
                Image: self.depositDetailImage(),
                Note: self.depositDetailNote(),
                PacketNumber: self.depositDetailPacketNumber(),
                Long: self.depositDetailLong(),
                High: self.depositDetailHigh(),
                Wide: self.depositDetailWide(),
                Size: self.depositDetailLong() + "x" + self.depositDetailWide() + "x" + self.depositDetailHigh(),
                ListCode: self.depositDetailListCode(),
                ShipTq: self.depositShipTq()
            };

            if (!self.isModelAdd()) {
                $.post("/Deposit/SaveDetail", { id: self.Id(), depositDetail: detail }, function (result) {
                    if (result.status === msgType.success) {

                        self.listDetail.push(detail);
                        $("#addOrEditDepositDetailModal").modal('hide');

                        toastr.success(result.msg);
                    } else {
                        toastr.error(result.msg);
                    }
                });
            } else {
                self.listDetail.push(detail);
                $("#addOrEditDepositDetailModal").modal('hide');
            }
        } else {
            var detail = _.find(self.listDetail(), function (it) {
                if (it.Id === self.depositDetailId()) {
                    it.LadingCode = self.depositDetailLadingCode();
                    it.Weight = self.depositDetailWeight();
                    it.CategoryId = self.depositDetailCategoryId();
                    it.CategoryName = self.depositDetailCategoryName();
                    it.ProductName = self.depositDetailProductName();
                    it.Quantity = self.depositDetailQuantity();
                    it.Size = self.depositDetailLong() + "x" + self.depositDetailWide() + "x" + self.depositDetailHigh();
                    it.Image = self.depositDetailImage();
                    it.Note = self.depositDetailNote();
                    it.PacketNumber = self.depositDetailPacketNumber();
                    it.Long = self.depositDetailLong();
                    it.High = self.depositDetailHigh();
                    it.Wide = self.depositDetailWide();
                    it.ListCode = self.depositDetailListCode();
                    it.ShipTq = self.depositShipTq();
                };
                return it.Id === self.depositDetailId();
            });

            if (!self.isModelAdd()) {
                $.post("/Deposit/UpdateDetail", { depositDetail: detail }, function (result) {
                    if (result.status === msgType.success) {
                        var list = self.listDetail();
                        self.listDetail([]);
                        self.listDetail(list);
                        $("#addOrEditDepositDetailModal").modal('hide');

                        toastr.success(result.msg);
                    } else {
                        toastr.error(result.msg);
                    }
                });
            } else {
                var list = self.listDetail();
                self.listDetail([]);
                self.listDetail(list);
                $("#addOrEditDepositDetailModal").modal('hide');
            }
        }

        self.updateGeneral();
    };

    self.save = function () {
        if (self.listDetail().length == 0) {
            toastr.error("No goods information yet!");
            return;
        };

        if (!self.check()) {
            return;
        }

        self.isSubmit(false);

        if (self.isModelAdd()) {
            $.post("/Deposit/Save",
            {
                deposit: {
                    Id: self.Id(),
                    Code: self.Code(),
                    CustomerId: self.CustomerId(),
                    CustomerName: self.CustomerName(),
                    CustomerEmail: self.CustomerEmail(),
                    CustomerPhone: self.CustomerPhone(),
                    CustomerAddress: self.CustomerAddress(),
                    LevelId: self.LevelId(),
                    LevelName: self.LevelName(),
                    Note: self.Note(),
                    ContactName: self.ContactName(),
                    ContactPhone: self.ContactPhone(),
                    ContactAddress: self.ContactAddress(),
                    ContactEmail: self.ContactEmail(),
                    UserId: self.UserId(),
                    UserName: self.UserName(),
                    Type: self.Type(),
                    PacketNumber: self.PacketNumber(),
                    Description: self.Description(),
                    Status: self.Status(),
                    SystemId: self.SystemId(),
                    SystemName: self.SystemName(),
                    WarehouseId: self.WarehouseId(),
                    WarehouseName: self.WarehouseName(),
                    WarehouseDeliveryId: self.WarehouseDeliveryId(),
                    WarehouseDeliveryName: self.WarehouseDeliveryName(),
                    ProvisionalMoney: self.ProvisionalMoney(),
                    TotalWeight: self.TotalWeight(),
                    Currency: self.Currency(),
                    ExchangeRate: self.ExchangeRate(),
                    UnsignName: self.UnsignName(),
                    ReasonCancel: self.ReasonCancel(),
                    DepositType: self.DepositType(),
                    ApprovelUnit: self.ApprovelUnit(),
                    ApprovelPrice: self.ApprovelPrice(),
                    OrderInfoId: self.OrderInfoId(),
                    FeeShip: self.FeeShip()
                },
                listDetails: self.listDetail(),
                orderInfoId: self.OrderInfoId(),
                depositType: self.DepositType(),
                listOrderServiceCheck: self.listOrderServiceCheck()
            }, function (result) {
                if (result.status === msgType.success) {
                    toastr.success(result.msg);
                    $("#orderDepositAddOrEdit").modal('hide');

                    $(".search-list").trigger('click');
                } else {
                    toastr.error(result.msg);
                }

                self.isSubmit(true);
            });
        } else {
            $.post("/Deposit/Update", {
                deposit: {
                    Id: self.Id(),
                    Code: self.Code(),
                    CustomerId: self.CustomerId(),
                    CustomerName: self.CustomerName(),
                    CustomerEmail: self.CustomerEmail(),
                    CustomerPhone: self.CustomerPhone(),
                    CustomerAddress: self.CustomerAddress(),
                    LevelId: self.LevelId(),
                    LevelName: self.LevelName(),
                    Note: self.Note(),
                    ContactName: self.ContactName(),
                    ContactPhone: self.ContactPhone(),
                    ContactAddress: self.ContactAddress(),
                    ContactEmail: self.ContactEmail(),
                    UserId: self.UserId(),
                    UserName: self.UserName(),
                    Type: self.Type(),
                    PacketNumber: self.PacketNumber(),
                    Description: self.Description(),
                    Status: self.Status(),
                    SystemId: self.SystemId(),
                    SystemName: self.SystemName(),
                    WarehouseId: self.WarehouseId(),
                    WarehouseName: self.WarehouseName(),
                    WarehouseDeliveryId: self.WarehouseDeliveryId(),
                    WarehouseDeliveryName: self.WarehouseDeliveryName(),
                    ProvisionalMoney: self.ProvisionalMoney(),
                    TotalWeight: self.TotalWeight(),
                    Currency: self.Currency(),
                    ExchangeRate: self.ExchangeRate(),
                    UnsignName: self.UnsignName(),
                    ReasonCancel: self.ReasonCancel(),
                    DepositType: self.DepositType(),
                    ApprovelUnit: self.ApprovelUnit(),
                    ApprovelPrice: self.ApprovelPrice(),
                    OrderInfoId: self.OrderInfoId(),
                    FeeShip: self.FeeShip()
                },
                orderInfoId: self.OrderInfoId(),
                depositType: self.DepositType(),
                listOrderServiceCheck: self.listOrderServiceCheck()
            }, function (result) {
                if (result.status === msgType.success) {
                    toastr.success(result.msg);
                    $("#orderDepositAddOrEdit").modal('hide');

                    $(".search-list").trigger('click');
                } else {
                    toastr.error(result.msg);
                }

                self.isSubmit(true);
            });
        }
    }

    //duyệt giá
    self.selectUnit = function (text) {
        self.ApprovelUnit(text);
    }

    //Hàm Add the tracking code
    self.addPackage = function () {
        self.isSubmit(false);

        if (self.codePackage() === '' || self.codePackage() === null || self.codePackage() === undefined) {
            toastr.error('Waybill code cannot be empty!');
            self.isSubmit(true);
        } else {
            $.post("/Deposit/AddContractCode", { id: self.Id(), codePackage: self.codePackage() }, function (result) {
                if (result.status === msgType.error) {
                    toastr.error(result.msg);
                    self.isSubmit(true);
                } else {
                    _.each(result.list,
                       function (it) {
                           it.cacheTransportCode = it.TransportCode;
                           it.cacheForcastDate = it.ForcastDate;
                           it.cacheWeight = it.Weight;
                           it.cacheWidth = it.Width;
                           it.cacheHeight = it.Height;
                           it.cacheLength = it.Length;

                           it.TransportCode = ko.observable(it.TransportCode);
                           it.TransportCode.subscribe(function (newValue) {
                               if (it.cacheTransportCode !== newValue) {
                                   self.updatePackage(ko.mapping.toJS(it));
                                   it.cacheTransportCode = newValue;
                               }
                           });

                           it.ForcastDate = ko.observable(it.ForcastDate ? moment(it.ForcastDate).format("L") : '');
                           it.ForcastDate.subscribe(function (newValue) {
                               if (it.cacheForcastDate !== newValue) {
                                   self.updatePackage(ko.mapping.toJS(it));
                                   it.cacheForcastDate = newValue;
                               }
                           });

                           it.Weight = ko.observable(formatNumberic(it.Weight, "N4"));
                           it.Weight.subscribe(function (newValue) {
                               if (it.cacheWeight !== newValue) {
                                   self.updatePackage(ko.mapping.toJS(it));
                                   it.cacheWeight = newValue;
                               }
                           });

                           it.Width = ko.observable(formatNumberic(it.Width, "N4"));
                           it.Width.subscribe(function (newValue) {
                               if (it.cacheWidth !== newValue) {
                                   self.updatePackage(ko.mapping.toJS(it));
                                   it.cacheWidth = newValue;
                               }
                           });

                           it.Height = ko.observable(formatNumberic(it.Height, "N4"));
                           it.Height.subscribe(function (newValue) {
                               if (it.cacheHeight !== newValue) {
                                   self.updatePackage(ko.mapping.toJS(it));
                                   it.cacheHeight = newValue;
                               }
                           });

                           it.Length = ko.observable(formatNumberic(it.Length, "N4"));
                           it.Length.subscribe(function (newValue) {
                               if (it.cacheLength !== newValue) {
                                   self.updatePackage(ko.mapping.toJS(it));
                                   it.cacheLength = newValue;
                               }
                           });
                       });
                    self.listPackageView(result.list);

                    $('.datepicker')
                        .datepicker({
                            autoclose: true,
                            language: 'en',
                            format: 'dd/mm/yyyy',
                            startDate: new Date()
                        });

                    self.isSubmit(true);
                }
            });
            self.codePackage('');
        }
    };

    //Hàm Edit mã vận đơn
    self.updatePackage = function (data) {
        $.post("/Deposit/EditContractCode", { packageId: data.Id, packageName: data.TransportCode, date: data.ForcastDate, weight: data.Weight, width: data.Width, height: data.Height, length: data.Length }, function (result) {
            if (result.status === msgType.error) {
                toastr.error(result.msg);
            } else {
                //_.each(result.list,
                //    function (it) {
                //        it.cacheTransportCode = it.TransportCode;
                //        it.cacheForcastDate = it.ForcastDate;
                //        it.cacheWeight = it.Weight;
                //        it.cacheWidth = it.Width;
                //        it.cacheHeight = it.Height;
                //        it.cacheLength = it.Length;

                //        it.TransportCode = ko.observable(it.TransportCode);
                //        it.TransportCode.subscribe(function (newValue) {
                //            if (it.cacheTransportCode !== newValue) {
                //                self.updatePackage(ko.mapping.toJS(it));
                //                it.cacheTransportCode = newValue;
                //            }
                //        });

                //        it.ForcastDate = ko.observable(it.ForcastDate ? moment(it.ForcastDate).format("L") : '');
                //        it.ForcastDate.subscribe(function (newValue) {
                //            if (it.cacheForcastDate !== newValue) {
                //                self.updatePackage(ko.mapping.toJS(it));
                //                it.cacheForcastDate = newValue;
                //            }
                //        });

                //        it.Weight = ko.observable(formatNumberic(it.Weight, "N4"));
                //        it.Weight.subscribe(function (newValue) {
                //            if (it.cacheWeight !== newValue) {
                //                self.updatePackage(ko.mapping.toJS(it));
                //                it.cacheWeight = newValue;
                //            }
                //        });

                //        it.Width = ko.observable(formatNumberic(it.Width, "N4"));
                //        it.Width.subscribe(function (newValue) {
                //            if (it.cacheWidth !== newValue) {
                //                self.updatePackage(ko.mapping.toJS(it));
                //                it.cacheWidth = newValue;
                //            }
                //        });

                //        it.Height = ko.observable(formatNumberic(it.Height, "N4"));
                //        it.Height.subscribe(function (newValue) {
                //            if (it.cacheHeight !== newValue) {
                //                self.updatePackage(ko.mapping.toJS(it));
                //                it.cacheHeight = newValue;
                //            }
                //        });

                //        it.Length = ko.observable(formatNumberic(it.Length, "N4"));
                //        it.Length.subscribe(function (newValue) {
                //            if (it.cacheLength !== newValue) {
                //                self.updatePackage(ko.mapping.toJS(it));
                //                it.cacheLength = newValue;
                //            }
                //        });
                //    });
                //self.listPackageView(result.list);
                toastr.success(result.msg);
            }
        });
    };

    //Hàm xóa mã vận đơn
    self.deletePackage = function (data) {
        swal({
            title: 'Are you sure you want to delete this item?',
            text: 'Transport code "' + data.TransportCode + '"',
            type: 'warning',
            showCancelButton: true,
           cancelButtonText: 'Cancel',
            confirmButtonText: 'Delete'
        }).then(function () {
            $.post("/Deposit/DeleteContractCode", { id: data.Id }, function (result) {
                if (result.status === msgType.error) {
                    toastr.error(result.msg);
                } else {
                    toastr.success('Successfully deleted transport code "' + data.TransportCode + '"');
                    _.each(result.list,
                       function (it) {
                           it.cacheTransportCode = it.TransportCode;
                           it.cacheForcastDate = it.ForcastDate;
                           it.cacheWeight = it.Weight;
                           it.cacheWidth = it.Width;
                           it.cacheHeight = it.Height;
                           it.cacheLength = it.Length;

                           it.TransportCode = ko.observable(it.TransportCode);
                           it.TransportCode.subscribe(function (newValue) {
                               if (it.cacheTransportCode !== newValue) {
                                   self.updatePackage(ko.mapping.toJS(it));
                                   it.cacheTransportCode = newValue;
                               }
                           });

                           it.ForcastDate = ko.observable(it.ForcastDate ? moment(it.ForcastDate).format("L") : '');
                           it.ForcastDate.subscribe(function (newValue) {
                               if (it.cacheForcastDate !== newValue) {
                                   self.updatePackage(ko.mapping.toJS(it));
                                   it.cacheForcastDate = newValue;
                               }
                           });

                           it.Weight = ko.observable(formatNumberic(it.Weight, "N4"));
                           it.Weight.subscribe(function (newValue) {
                               if (it.cacheWeight !== newValue) {
                                   self.updatePackage(ko.mapping.toJS(it));
                                   it.cacheWeight = newValue;
                               }
                           });

                           it.Width = ko.observable(formatNumberic(it.Width, "N4"));
                           it.Width.subscribe(function (newValue) {
                               if (it.cacheWidth !== newValue) {
                                   self.updatePackage(ko.mapping.toJS(it));
                                   it.cacheWidth = newValue;
                               }
                           });

                           it.Height = ko.observable(formatNumberic(it.Height, "N4"));
                           it.Height.subscribe(function (newValue) {
                               if (it.cacheHeight !== newValue) {
                                   self.updatePackage(ko.mapping.toJS(it));
                                   it.cacheHeight = newValue;
                               }
                           });

                           it.Length = ko.observable(formatNumberic(it.Length, "N4"));
                           it.Length.subscribe(function (newValue) {
                               if (it.cacheLength !== newValue) {
                                   self.updatePackage(ko.mapping.toJS(it));
                                   it.cacheLength = newValue;
                               }
                           });
                       });
                    self.listPackageView(result.list);
                    toastr.success(result.msg);
                }
            });
        }, function () { });
    };

    self.ApprovelPrice.subscribe(function (newValue) {
        if (newValue > 0) {
            self.updateGeneral();
        }
    });

    self.updateGeneral = function () {
        self.PacketNumber(_.sumBy(self.listDetail(), function (it) { return Globalize.parseFloat(it.PacketNumber); }));
        self.TotalWeight(_.sumBy(self.listDetail(), function (it) { return Globalize.parseFloat(it.Weight); }));
        if (Globalize.parseFloat(self.TotalWeight()) < 50) {
            self.ProvisionalMoney(Globalize.parseFloat(self.ApprovelPrice()) * (Globalize.parseFloat(self.TotalWeight()) - 1) + 100000);
        } else {
            self.ProvisionalMoney(Globalize.parseFloat(self.ApprovelPrice()) * Globalize.parseFloat(self.TotalWeight()));
        }
    }

    self.check = function () {
        if (!(self.CustomerId() > 0)) {
            toastr.error("Select customer!");
            return false;
        }
        if (!(self.WarehouseId() > 0)) {
            toastr.error("select consignee warehouse!");
            return false;
        }
        if (!(self.WarehouseDeliveryId() > 0)) {
            toastr.error("Choose the shipper!");
            return false;
        }
        if (self.ContactName() == "" || self.ContactName() == undefined || self.ContactName() == null) {
            toastr.error("Recipient name can not be empty!");
            return false;
        }
        if (self.ContactPhone() == "" || self.ContactPhone() == undefined || self.ContactPhone() == null) {
            toastr.error("Phone recipient is not empty!");
            return false;
        }
        if (self.ContactAddress() == "" || self.ContactAddress() == undefined || self.ContactAddress() == null) {
            toastr.error("The recipient address can not be empty!");
            return false;
        }
        if (self.listDetail().length == 0) {
            toastr.error("Line item can not be empty!");
            return false;
        }

        return true;
    }

    self.checkDetail = function () {
        if (self.depositDetailProductName() == "" || self.ContactAddress() == undefined || self.ContactAddress() == null) {
            toastr.error("Product name can not be empty!");
            return false;
        }
        if (self.depositDetailCategoryName() == "" || self.depositDetailCategoryName() == undefined || self.depositDetailCategoryName() == null) {
            toastr.error("Branch is not empty!");
            return false;
        }
        if (self.depositDetailImage() == "" || self.depositDetailImage() == undefined || self.depositDetailImage() == null) {
            toastr.error("Image can not be empty!");
            return false;
        }
        if (self.depositDetailPacketNumber() == "" || self.depositDetailPacketNumber() == undefined || self.depositDetailPacketNumber() == null) {
            toastr.error("Number of packages can not be empty!");
            return false;
        }
        //if (self.depositDetailWeight() == "" || self.depositDetailWeight() == undefined || self.depositDetailWeight() == null) {
        //    toastr.error("Sum Weight cannot be empty!");
        //    return false;
        //}
        //if (self.depositDetailLong() == "" || self.depositDetailLong() == undefined || self.depositDetailLong() == null) {
        //    toastr.error("Chiều dài cannot be empty!");
        //    return false;
        //}
        //if (self.depositDetailHigh() == "" || self.depositDetailHigh() == undefined || self.depositDetailHigh() == null) {
        //    toastr.error("Chiều cao cannot be empty!");
        //    return false;
        //}
        //if (self.depositDetailWide() == "" || self.depositDetailWide() == undefined || self.depositDetailWide() == null) {
        //    toastr.error("Chiều rộng cannot be empty!");
        //    return false;
        //}
        //if (self.depositDetailQuantity() == "" || self.depositDetailQuantity() == undefined || self.depositDetailQuantity() == null) {
        //    toastr.error("Số lượng cannot be empty!");
        //    return false;
        //}

        return true;
    }

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
                    toastr.error("The file is not allowed");
                    return;
                }
                self.depositDetailImage(window.location.origin + data.result[0].path);
            }
        });
        return true;
    }

    var validateBlackListExtensions = function (file) {
        var ext = file.substring(file.lastIndexOf(".")).toLowerCase();
        return !_.some([".jpg", ".jpeg", ".gif", ".png"], function (item) { return item === ext; });
    };

    $(function () {
        self.searchCustomer();
    });

    //Hàm lấy thông tin khách hàng
    self.searchCustomer = function () {
        $(".customer-search-add")
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
                        self.ApprovelPrice(repo.depositPrice);
                    }

                    return repo.text;
                },
                placeholder: "",
                allowClear: true,
                language: 'en'
            });
    };

    //Gửi duyệt giá
    self.sendPendingPrice = function () {
        if (self.ApprovelPrice() == null ||
            self.ApprovelPrice() == '0' ||
            self.ApprovelPrice() == '' ||
            self.ApprovelPrice() == undefined) {
            toastr.error("Price browsing must be greater 0!");

            return;
        }

        self.isSubmit(false);

        $.post("/Deposit/PendingPrice",
            {
                deposit: {
                    Id: self.Id(),
                    Code: self.Code(),
                    CustomerId: self.CustomerId(),
                    CustomerName: self.CustomerName(),
                    CustomerEmail: self.CustomerEmail(),
                    CustomerPhone: self.CustomerPhone(),
                    CustomerAddress: self.CustomerAddress(),
                    LevelId: self.LevelId(),
                    LevelName: self.LevelName(),
                    Note: self.Note(),
                    ContactName: self.ContactName(),
                    ContactPhone: self.ContactPhone(),
                    ContactAddress: self.ContactAddress(),
                    ContactEmail: self.ContactEmail(),
                    UserId: self.UserId(),
                    UserName: self.UserName(),
                    Type: self.Type(),
                    PacketNumber: self.PacketNumber(),
                    Description: self.Description(),
                    Status: self.Status(),
                    SystemId: self.SystemId(),
                    SystemName: self.SystemName(),
                    WarehouseId: self.WarehouseId(),
                    WarehouseName: self.WarehouseName(),
                    WarehouseDeliveryId: self.WarehouseDeliveryId(),
                    WarehouseDeliveryName: self.WarehouseDeliveryName(),
                    ProvisionalMoney: self.ProvisionalMoney(),
                    TotalWeight: self.TotalWeight(),
                    Currency: self.Currency(),
                    ExchangeRate: self.ExchangeRate(),
                    UnsignName: self.UnsignName(),
                    ReasonCancel: self.ReasonCancel(),
                    DepositType: self.DepositType(),
                    ApprovelUnit: self.ApprovelUnit(),
                    ApprovelPrice: self.ApprovelPrice(),
                    OrderInfoId: self.OrderInfoId()
                },
                orderInfoId: self.OrderInfoId(),
                depositType: self.DepositType()
            },
            function (result) {
                if (result.status === msgType.error) {
                    toastr.error(result.msg);
                } else {
                    toastr.success(result.msg);
                    $("#orderDepositAddOrEdit").modal('hide');

                    $(".search-list").trigger('click');
                }

                self.isSubmit(true);
            });


    }

    //Gửi báo giá
    self.sendQuotes = function () {
        self.isSubmit(false);
        $.post("/Deposit/Quotes",
            { id: self.Id() },
            function (result) {
                if (result.status === msgType.error) {
                    toastr.error(result.msg);
                } else {
                    toastr.success(result.msg);
                    $("#orderDepositAddOrEdit").modal('hide');

                    $(".search-list").trigger('click');
                }

                self.isSubmit(true);
            });
    }

    //Kết đơn hộ khách
    self.singles = function () {
        self.isSubmit(false);
        $.post("/Deposit/Singles",
            { id: self.Id() },
            function (result) {
                if (result.status === msgType.error) {
                    toastr.error(result.msg);
                } else {
                    toastr.success(result.msg);
                    $("#orderDepositAddOrEdit").modal('hide');

                    $(".search-list").trigger('click');
                }

                self.isSubmit(true);
            });
    }

    self.listHistory = ko.observableArray([]);
    self.isShowHistory = ko.observable(false);
    self.checkShowHistory = function () {
        self.isShowHistory(!self.isShowHistory());
    }

    self.userOrder = ko.observableArray([]);

    self.codeOw = ko.observable();
    self.orderCodeWarehouse = function (id, code) {
        $.post("/Purchase/OrderCodeWarehouse", { idWarehouse: id, code: code }, function (result) {
            self.codeOw(result.code);
        });
    }
}