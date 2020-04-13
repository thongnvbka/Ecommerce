System.register(["@angular/core", "../../services/group-permission.service"], function (exports_1, context_1) {
    "use strict";
    var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
        var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
        if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
        else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
        return c > 3 && r && Object.defineProperty(target, key, r), r;
    };
    var __metadata = (this && this.__metadata) || function (k, v) {
        if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
    };
    var __moduleName = context_1 && context_1.id;
    var core_1, group_permission_service_1, PermissionComponent;
    return {
        setters: [
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (group_permission_service_1_1) {
                group_permission_service_1 = group_permission_service_1_1;
            }
        ],
        execute: function () {
            PermissionComponent = (function () {
                function PermissionComponent(permissionService) {
                    this.permissionService = permissionService;
                    this.permissions = [];
                    this.recordPerPage = 20;
                    this.currentPage = 1;
                    this.totalRecord = 0;
                    this.isSearch = false;
                    this.keyword = "";
                    this.search(1);
                }
                PermissionComponent.prototype.search = function (currentPage) {
                    var _this = this;
                    this.currentPage = currentPage;
                    this.isSearch = true;
                    this.permissionService.search(this.keyword, this.recordPerPage, this.currentPage)
                        .subscribe(function (data) {
                        _this.permissions = data.items;
                        _this.totalRecord = data.totalRecord;
                    }, function () { }, function () { return _this.isSearch = false; });
                };
                PermissionComponent.prototype.afterHideModal = function (refresh) {
                    if (refresh)
                        this.search(this.currentPage);
                };
                PermissionComponent.prototype.delete = function (permission) {
                    var _this = this;
                    swal({
                        title: "Bạn có chắc chắn muốn xóa nhóm quyền này?",
                        showCancelButton: true,
                        confirmButtonText: 'Xóa',
                        cancelButtonText: 'Không'
                    })
                        .then(function () {
                        _this.permissionService.delete(permission.id)
                            .subscribe(function (data) {
                            if (data.status > 0) {
                                toastr.success(data.content);
                                _.remove(_this.permissions, function (item) { return item.id === permission.id; });
                                return;
                            }
                            toastr.error(data.content);
                        }, function () { });
                    });
                };
                PermissionComponent.prototype.ngAfterContentInit = function () {
                    //jQuery(document).ready(() => {
                    //    App.init(); // init metronic core componets
                    //    Layout.init();
                    //});
                };
                return PermissionComponent;
            }());
            PermissionComponent = __decorate([
                core_1.Component({
                    selector: 'permissions',
                    templateUrl: '/app/components/permission/permission.html',
                    providers: [group_permission_service_1.GroupPermissionService]
                }),
                __metadata("design:paramtypes", [group_permission_service_1.GroupPermissionService])
            ], PermissionComponent);
            exports_1("PermissionComponent", PermissionComponent);
        }
    };
});
//# sourceMappingURL=permission.component.js.map