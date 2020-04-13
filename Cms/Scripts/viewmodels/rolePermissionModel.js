function RolePermissionModel() {
    var self = this;

    self.isSubmit = ko.observable(false);
    self.isLoading = ko.observable(false);
    self.officeId = ko.observable(null);
    self.titleId = ko.observable(null);
    self.userId = ko.observable(null);
    self.type = ko.observable(null);
    self.groupPermissionId = ko.observable(null);
    self.userPositions = ko.observableArray(null);
    self.groupPermissionJson = ko.observableArray(window.groupPermissionJson);

    self.quickAddChangeOfficeId = function(officeId) {
        $("#rolePermission-TitleId").attr('disabled', 'disabled');

        $.get("/Position/PossitionByOffice?officeId=" + officeId,
            function(data) {
                var options = '<option value="" selected>Select postion</option>';
                $.each(data,
                    function(idx, item) {
                        options = options +
                            '<option value="' +
                            item.id +
                            '">' +
                            item.name +
                            '</option>';
                    });
                $("#rolePermission-TitleId").html(options);
                $("#rolePermission-TitleId").removeAttr('disabled');
            });
    }

    self.search = function() {
        self.isLoading(true);
        $.get("/User/GetUserPosition",
            { userId: self.userId() },
            function(data) {
                self.isLoading(false);

                _.each(data,
                    function(item) {
                        item.groupPermisionId = ko.observable(item.groupPermisionId);
                    });

                self.userPositions(data);
            });
    }

    self.token = $("#rolePermissionTab input[name='__RequestVerificationToken']").val();

    self.addUserPosstion = function() {
        var data = {
            userId: self.userId(),
            officeId: self.officeId(),
            titleId: self.titleId(),
            type: self.type(),
            groupPermissionId: self.groupPermissionId()
        };

        if (data.officeId === "" || data.officeId == null) {
            toastr.warning("Unit is required to choose");
            return;
        }

        if (data.titleId  === "" || data.titleId == null) {
            toastr.warning("Position is mandatory");
            return;
        }

        data["__RequestVerificationToken"] = self.token;

        self.isSubmit(true);
        $.post("/User/AddUserPosition",
            data,
            function(rs) {
                self.isSubmit(false);
                if (rs && rs.status <= 0) {
                    toastr.warning(rs.text);
                    return;
                }
                toastr.success(rs.text);
                self.search();
            });
    }

    self.removeUserPosition = function(data) {
        swal({
            title: 'Are you sure you want to take part?',
                text: 'Position"' + data.titleName + '" in unit "' + data.officeName + '"' ,
                type: 'warning',
                showCancelButton: true,
                //confirmButtonColor: '#3085d6',
                //cancelButtonColor: '#d33',
               cancelButtonText: 'Cancel',
                confirmButtonText: 'Delete'
            })
            .then(function() {
                    var dataValue = { id: data.id };
                    dataValue["__RequestVerificationToken"] = self.token;

                    $.post("/User/RemoveUserPosition",
                        dataValue,
                        function(rs) {
                            if (rs && rs <= 0) {
                                toastr.warning(rs.text);
                                return;
                            }
                            toastr.success(rs.text);
                            self.search();
                        });
                },
                function() {});
    }

    self.updateGroupPermisison = function(params) {
        var d = new $.Deferred();

        var data = { groupPermissionId: params.value, id: params.pk };
        data["__RequestVerificationToken"] = self.token;

        $.post("/User/UpdateUserPosition",
            data,
            function(rs) {
                if (rs && rs.status > 0) {
                    toastr.success(rs.text);
                    d.resolve();
                    self.search();
                    return d.promise();
                } else {
                    toastr.warning(rs.text);
                    d.reject(rs.text);
                }
            });
    }

    $(function() {
        self.userId($("#accountInfo input#Id").val());
        
        self.search();

        $("#rolePermission-office-tree")
            .dropdownjstree({
                source: window.officesTree,
                dropdownLabel: 'Choose unit',
                dropdownLabelClick: () => {
                    self.officeId("1");
                    self.quickAddChangeOfficeId(1);
                },
                selectedNode: '1',
                selectNote: (node, selected) => {
                    self.officeId(selected.selected[0]);
                    self.quickAddChangeOfficeId(selected.selected[0]);
                }
            });

        $.validator.setDefaults({
            ignore: []
        });
    });
}

var rolePermissionModelView = new RolePermissionModel();

ko.applyBindings(rolePermissionModelView, $("#rolePermissionTab")[0]);