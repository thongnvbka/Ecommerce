function DispatcherAddModel(callback, walletDetailModal) {
    var self = this;

    self.allWarehouse = ko.observableArray(window.allWarehouse);
    self.isLoading = ko.observable(false);
    self.isLoadingItems = ko.observable(false);
    self.suggetionType = ko.observable(0); // 0: package, 1: Bao hàng
    self.statesGroupId = _.groupBy(window.states, 'id');

    self.items = ko.observableArray([]);

    // model
    self.id = ko.observable(0);
    self.note = ko.observable("");
    self.code = ko.observable("");
    self.status = ko.observable(1);
    self.transportPartnerId = ko.observable(null);
    self.entrepotId = ko.observable(null);
    self.entrepotName = ko.observable("");
    self.transportPartnerName = ko.observable("");
    self.transportMethodId = ko.observable(null);
    self.transportMethodName = ko.observable("");
    self.toWarehouseId = ko.observable(null);
    self.toWarehouseIdPath = ko.observable("");
    self.toWarehouseName = ko.observable("");
    self.toWarehouseAddress = ko.observable("");
    self.userId = ko.observable(0);
    self.userName = ko.observable("");
    self.userFullName = ko.observable("");
    self.created = ko.observable();
    self.updated = ko.observable();
    self.walletNo = ko.observable(0);
    self.amount = ko.observable(null);
    self.totalWeight = ko.observable(null);
    self.totalWeightConverted = ko.observable(null);
    self.totalWeightActual = ko.observable(null);
    self.totalVolume = ko.observable(null);
    self.totalValue = ko.observable(null);
    self.totalPackageNo = ko.observable(null);
    self.fromWarehouseId = ko.observable(0);
    self.fromWarehouseIdPath = ko.observable("");
    self.fromWarehouseName = ko.observable("");
    self.fromWarehouseAddress = ko.observable("");
    self.priceType = ko.observable(0);
    self.priceTypes = ko.observableArray([{id: 0, name: "Calculated by weight"}, {id: 1, name: "Calculated by volume"}]);
    self.priceTypeName = ko.observable("Calculated by weight");
    self.price = ko.observable(null);
    self.value = ko.observable(null);
    self.contactName = ko.observable(null);
    self.contactPhone = ko.observable(null);

    self.toWarehouseId.subscribe(function (warehouseId) {
        var warehouse = _.find(self.allWarehouse(), function (w) { return w.id === warehouseId; });

        if (warehouse) {
            self.toWarehouseId(warehouse.id);
            self.toWarehouseIdPath(warehouse.idPath);
            self.toWarehouseName(warehouse.name);
            self.toWarehouseAddress(warehouse.address);
        } else {
            self.toWarehouseId(null);
            self.toWarehouseIdPath(null);
            self.toWarehouseName(null);
            self.toWarehouseAddress(null);
        }
    });

    self.priceType.subscribe(function() {
        if (self.items().length > 0) {
            _.each(self.items(),
                function(item) {
                    item.value(formatNumberic(self.priceType() === 0 ? item.weight : item.volume, 'N2'));
                });
            self.refreshDispatcher();
        }
    });

    self.resetForm = function () {
        self.id(0);
        self.note("");
        self.code("");
        self.status(1);
        self.entrepotId(null);
        self.entrepotName("");
        self.transportPartnerId(null);
        self.transportPartnerName("");
        self.transportMethodId(null);
        self.transportMethodName("");
        self.toWarehouseId(null);
        self.toWarehouseIdPath("");
        self.toWarehouseName("");
        self.toWarehouseAddress("");
        self.userId(0);
        self.userName("");
        self.userFullName("");
        self.created();
        self.updated();
        self.walletNo(0);
        self.amount(null);
        self.totalWeight(null);
        self.totalWeightConverted(null);
        self.totalWeightActual(null);
        self.totalVolume(null);
        self.totalValue(null);
        self.totalPackageNo(null);
        self.fromWarehouseId(0);
        self.fromWarehouseIdPath("");
        self.fromWarehouseName("");
        self.fromWarehouseAddress("");
        self.priceType(0);
        self.priceTypeName("Calculated by weight");
        self.value(null);
        self.price(null);
        self.contactName(null);
        self.contactPhone(null);

        self.isLoadingItems(false);
        self.isLoading(false);

        self.items.removeAll();
        self.initInputMark();
    }

    self.setForm = function (data) {
        self.showAddForm();

        self.id(data.id);
        self.note(data.note);
        self.code(data.code);
        self.status(data.status);
        self.transportPartnerId(data.transportPartnerId);
        self.entrepotId(data.entrepotId);
        self.entrepotName(data.entrepotName);
        self.transportPartnerName(data.transportPartnerName);
        self.transportMethodId(data.transportMethodId);
        self.transportMethodName(data.transportMethodName);
        self.toWarehouseId(data.toWarehouseId);
        self.toWarehouseIdPath(data.toWarehouseIdPath);
        self.toWarehouseName(data.toWarehouseName);
        self.toWarehouseAddress(data.toWarehouseAddress);
        self.userId(data.userId);
        self.userName(data.userName);
        self.userFullName(data.userFullName);
        self.created(data.created);
        self.updated(data.updated);
        self.walletNo(data.walletNo);
        self.amount(data.amount);
        self.totalWeight(data.totalWeight);
        self.totalWeightConverted(data.totalWeightConverted);
        self.totalWeightActual(data.totalWeightActual);
        self.totalVolume(data.totalVolume);
        self.totalValue(data.totalValue);
        self.totalPackageNo(data.totalPackageNo);
        self.fromWarehouseId(data.fromWarehouseId);
        self.fromWarehouseIdPath(data.fromWarehouseIdPath);
        self.fromWarehouseName(data.fromWarehouseName);
        self.fromWarehouseAddress(data.fromWarehouseAddress);
        self.priceType(data.priceType);
        self.priceTypeName(data.priceType === 0 ? "Calculated by weight" : "Calculated by volume");
        self.price(data.price);
        self.value(data.value);
        self.contactName(data.contactName);
        self.contactPhone(data.contactPhone);
        // Get detail của phiếu nhập kho
        self.getDetail();
    }

    // Tính lại cân nặng,...
    self.refreshDispatcher = function () {
        self.walletNo(formatNumberic(self.items().length));

        self.totalWeight(_.sumBy(self.items(), function (i) {
            return i.weight !== null ? i.weight : 0;
        }));
        self.totalPackageNo(_.sumBy(self.items(), function (i) {
            return i.packageNo !== null ? i.packageNo : 0;
        }));
        self.totalWeightConverted(_.sumBy(self.items(), function(i) {
            return i.weightConverted !== null ? i.weightConverted : 0;
        }));
        self.totalWeightActual(_.sumBy(self.items(), function (i) {
            return i.weightActual !== null ? i.weightActual : 0;
        }));
        self.totalVolume(_.sumBy(self.items(), function (i) {
            return i.volume !== null ? i.volume : 0;
        }));
        self.totalValue(_.sumBy(self.items(), function (i) {
            return isNaN(Globalize.parseFloat(i.value())) ? 0 : Globalize.parseFloat(i.value());
        }));
    }

    self.getDetail = function () {
        self.isLoadingItems(true);
        $.get("/dispatcher/getwallets", { id: self.id() }, function (data) {
            self.isLoadingItems(false);

            _.each(data, function (it) {
                // Ghi chu

                it.cacheDescription = it.description;
                it.description = ko.observable(it.description);
                it.description.subscribe(function(newValue) {
                    if (it.cacheDescription !== newValue) {
                        self.updateWallet(it);
                        it.cacheDescription = newValue;
                    }
                });

                it.cacheNote = it.note;
                it.note = ko.observable(it.note);
                it.note.subscribe(function (newValue) {
                    if (it.cacheNote !== newValue) {
                        self.updateWallet(it);
                        it.cacheNote = newValue;
                    }
                });


                it.cacheContact = it.contact;
                it.contact = ko.observable(it.contact);
                it.contact.subscribe(function (newValue) {
                    if (it.cacheContact !== newValue) {
                        self.updateWallet(it);
                        it.cacheContact = newValue;
                    }
                });

                it.cacheAddress = it.address;
                it.address = ko.observable(it.address);
                it.address.subscribe(function (newValue) {
                    if (it.cacheAddress !== newValue) {
                        self.updateWallet(it);
                        it.cacheAddress = newValue;
                    }
                });
            });
            self.items(data);
            self.initInputMark();
        });
    }

    self.getFormData = function () {
        return {
            id: self.id(),
            note: self.note(),
            code: self.code(),
            status: self.status(),
            toWarehouseId: self.toWarehouseId(),
            toWarehouseIdPath: self.toWarehouseIdPath(),
            toWarehouseName: self.toWarehouseName(),
            toWarehouseAddress: self.toWarehouseAddress(),
            walletNo: self.walletNo(),
            userId: self.userId(),
            userName: self.userName(),
            userFullName: self.userFullName(),
            created: self.created(),
            updated: self.updated(),
            fromWarehouseId: self.fromWarehouseId(),
            fromWarehouseIdPath: self.fromWarehouseIdPath(),
            fromWarehouseName: self.fromWarehouseName(),
            fromWarehouseAddress: self.fromWarehouseAddress(),
            transportPartnerId: self.transportPartnerId(),
            entrepotId: self.entrepotId(),
            transportPartnerName: self.transportPartnerName(),
            transportMethodId: self.transportMethodId(),
            transportMethodName: self.transportMethodName(),
            priceType: self.priceType(),
            price: self.price(),
            value: self.value(),
            contactName: self.contactName(),
            contactPhone: self.contactPhone()
        }
    }

    self.showAddForm = function (walletCodes) {
        self.resetForm();
        resetForm("#dispatcherForm");
        $("#DispatcherAddModel").modal("show");

        if (walletCodes !== undefined) {
            self.isLoadingItems(true);
            $.get("/wallet/getforwallettracker",
                { walletCodes: walletCodes },
                function(data) {
                    self.isLoadingItems(false);
                    _.each(data,
                        function(item) {
                            item.dispatcherId = 0;
                            item.dispatcherCode = 0;
                            item.walletId = item.id;
                            item.id = 0;
                            item.walletCode = item.code;
                            item.status = 1;
                            item.amount = item.totalValue;
                            item.weight = item.weight;
                            item.weightActual = item.weightActual;
                            item.weightConverted = item.weightConverted;
                            item.volume = item.volume;
                            item.size = item.size;
                            item.value = ko.observable(self.priceType() === 0 ? item.weight : item.volume);
                            item.description = ko.observable("");
                            item.note = ko.observable(item.note);
                            item.contact = ko.observable("");
                            item.address = ko.observable("");

                            item.value.subscribe(function() {
                                self.refreshDispatcher();
                            });
                        });
                    self.items(data);
                    self.refreshDispatcher();
                    self.initInputMark();
                });
        }
    }

    self.showUpdateForm = function (data) {
        self.setForm(data);
        resetForm("#dispatcherForm");
        $("#DispatcherAddModel").modal("show");
    }

    self.loadingItems = function (dispatcherId) {
        $.get("/dispatcher/getwallets", { id: dispatcherId }, function (data) {
            self.items(data);
        });
    }

    self.updateWallet = function (wallet) {
        var data = ko.mapping.toJS(wallet);

        data["__RequestVerificationToken"] = self.token;
        self.isLoading(true);
        $.post("/dispatcher/updatewallet", data, function (rs) {
            self.isLoading(false);
            if (rs.status < 0) {
                toastr.error(rs.text);
                return;
            }
        });
    }

    self.removeItem = function (item) {
        if (self.id() === 0) {
            self.items.remove(item);
            self.refreshDispatcher();
            return;
        }
        swal({
            title: 'Are you sure you want to delete this item?',
            text: 'Goods sack "' + item.walletCode + '"',
            type: 'warning',
            showCancelButton: true,
            //confirmButtonColor: '#3085d6',
            //cancelButtonColor: '#d33',
           cancelButtonText: 'Cancel',
            confirmButtonText: 'Delete'
        })
            .then(function () {
                self.isLoading(true);

                item["__RequestVerificationToken"] = self.token;
                $.post("/dispatcher/deletewallet", item,
                    function (rs) {
                        self.isLoading(false);
                        if (rs.status < 0) {
                            toastr.error(rs.text);
                            return;
                        }

                        toastr.success(rs.text);
                        self.getDetail();
                    });
            }, function () { });
    }

    self.token = $("#dispatcherForm input[name='__RequestVerificationToken']").val();

    self.save = function () {
        if (!$("#dispatcherForm").valid()) {
            toastr.error("Check the fields entered!");
            $(".input-validation-error:first").focus();
            return;
        }

        if (self.items().length === 0) {
            toastr.error("Requisition for warehousing does not have any goods packages/sacks!");
            return;
        }

        var data = self.getFormData();
        data.wallets = ko.mapping.toJS(self.items());
        data["__RequestVerificationToken"] = self.token;

        // Cập nhật
        if (self.id() > 0) {
            self.isLoading(true);
            $.post("/dispatcher/update", data, function (rs) {
                self.isLoading(false);

                if (rs.status < 0) {
                    toastr.error(rs.text);
                    return;
                }

                toastr.success(rs.text);
                // reset form
                self.resetForm();
                resetForm("#dispatcherForm");
                $("#DispatcherAddModel").modal("hide");

                if (callback)
                    callback();
            });
        } else { // Thêm mới
            self.isLoading(true);
            $.post("/dispatcher/add", data, function (rs) {
                self.isLoading(false);
                if (rs.status < 0) {
                    toastr.error(rs.text);
                    return;
                }

                toastr.success(rs.text);
                // reset form
                self.resetForm();
                resetForm("#dispatcherForm");
                $("#DispatcherAddModel").modal("hide");

                if (callback)
                    callback();
            });
        }
    }

    self.initInputMark = function() {
        $('#DispatcherAddModel input.decimal').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize.culture().numberFormat['.'], autoGroup: true, groupSeparator: Globalize.culture().numberFormat[','], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });
    }

    self.showWalletDetail = function (data) {
        if (walletDetailModal) {
            walletDetailModal.showModel(data);
            return;
        }
    }

    self.initSuggetion = function () {
        // Đăng ký suggetion đơn vị
        $('#suggetion').autocomplete({
            delay: 100,
            autoFocus: true,
            source: function (request, response) {
                // ReSharper disable once CoercedEqualsUsing
                var codes = _.map(self.items(), "walletCode");

                var strCodes = codes.length === 0 ? "" : ';' + _.join(codes, ';') + ';';

                $.post('/wallet/SuggetionToDispatcher', { term: request.term, walletCodes: strCodes }, function (result) {
                    return response(result);
                });
            },
            select: function (event, ui) {
                var item = {
                    id: 0,
                    dispatcherId: 0,
                    dispatcherCode: "",
                    walletId: ui.item.id,
                    walletCode: ui.item.code,
                    status: 1,
                    amount: ui.item.totalValue,
                    weight: ui.item.weight,
                    weightActual: ui.item.weightActual,
                    weightConverted: ui.item.weightConverted,
                    volume: ui.item.volume,
                    size: ui.item.size,
                    value: ko.observable(self.priceType() === 0 ? ui.item.weight : ui.item.volume),
                    packageNo: ui.item.packageNo,
                    description: ko.observable(""),
                    note: ko.observable(ui.item.note),
                    contact: ko.observable(""),
                    address: ko.observable("")
                };

                item.value.subscribe(function() {
                    self.refreshDispatcher();
                });

                if (self.id() > 0) {
                    var wallet = ko.mapping.toJS(item);

                    wallet.dispatcherId = self.id();
                    wallet.dispatcherCode = self.code();
                    wallet["__RequestVerificationToken"] = self.token;

                    self.isLoading(true);
                    $.post("/dispatcher/addwallet", wallet, function (rs) {
                        self.isLoading(false);
                        if (rs.status < 0) {
                            toastr.error(rs.text);
                            return;
                        }
                        self.getDetail();
                    });
                    return;
                }

                if (!_.find(self.items(), function (it) { return it.walletCode === ui.item.code })) {
                    self.items.push(item);

                    self.initInputMark();
                    self.refreshDispatcher();
                }
            }
        }).autocomplete("instance")._renderItem = function (ul, item) {
            var name = item.code;

            return $("<li>").addClass('media media-line').append('<div>' +
                '<h4 class="media-heading bold size-16 pr-mgb-0">' + name +
                '</h4></div>').appendTo(ul).addClass('automember media-list');
        };
    }

    $(function () {
        self.initSuggetion();
    });
}