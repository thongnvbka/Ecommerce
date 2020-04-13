System.register(["@angular/core", "../../services/permission-action.service", "../../services/group-permission.service"], function (exports_1, context_1) {
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
    var core_1, permission_action_service_1, group_permission_service_1, PermissionDetailModalComponent;
    return {
        setters: [
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (permission_action_service_1_1) {
                permission_action_service_1 = permission_action_service_1_1;
            },
            function (group_permission_service_1_1) {
                group_permission_service_1 = group_permission_service_1_1;
            }
        ],
        execute: function () {
            PermissionDetailModalComponent = (function () {
                function PermissionDetailModalComponent(permissionActionService, groupPermissionService) {
                    this.permissionActionService = permissionActionService;
                    this.groupPermissionService = groupPermissionService;
                    this.recordPerPage = 20;
                    this.currentPage = 1;
                    this.totalRecord = 0;
                    this.pageIds = [];
                    this.isSearch = false;
                    this.pages = [];
                    this.actions = [];
                    this.keyword = "";
                }
                PermissionDetailModalComponent.prototype.checkFirst = function (page, idx) {
                    console.log(page, idx);
                    if (idx === 0)
                        this.moduleIdFirst = null;
                    var check = page.moduleId === this.moduleIdFirst;
                    this.moduleIdFirst = page.moduleId;
                    return check;
                };
                PermissionDetailModalComponent.prototype.search = function () {
                    var _this = this;
                    this.permissionActionService.getByPermissionId(this.permission.id, this.keyword)
                        .subscribe(function (data) {
                        var pagesCache = [];
                        var groupPages = _.groupBy(data.items, "pageId");
                        _this.pageIds = _.keys(groupPages);
                        _.forEach(_this.pageIds, function (pageId) {
                            var actions = groupPages[pageId + ""];
                            var p = actions[0];
                            var page = {
                                appId: p.appId,
                                appName: p.appName,
                                moduleId: p.moduleId,
                                moduleName: p.moduleName,
                                pageId: p.pageId,
                                pageName: p.pageName,
                                actions: actions
                            };
                            _.forEach(actions, function (action) {
                                var a = action;
                                page["action_" + a.roleActionId] = a.checked;
                            });
                            pagesCache.push(page);
                        });
                        pagesCache = _.orderBy(pagesCache, ['moduleId', 'pageId'], ['asc', 'asc']);
                        var moduleIdFist;
                        _.forEach(pagesCache, function (item) {
                            item.firstRecord = moduleIdFist !== item.moduleId;
                            item.pathName = item.appName + " / " + item.moduleName;
                            moduleIdFist = item.moduleId;
                        });
                        _this.pages = pagesCache;
                        _this.actions = data.actions;
                    }, function () { }, function () { return _this.isSearch = false; });
                };
                PermissionDetailModalComponent.prototype.show = function (permission) {
                    this.permission = permission;
                    this.search();
                    $("#permission-detail-modal").modal('show');
                };
                PermissionDetailModalComponent.prototype.hide = function () {
                    $("#permission-detail-modal").modal('hide');
                };
                PermissionDetailModalComponent.prototype.update = function (page, actionId) {
                    this.groupPermissionService.updatePermission(this.permission.id, page.pageId, actionId, !page['action_' + actionId]).subscribe(function (data) {
                        if (data.status > 0) {
                            page['action_' + actionId] = !page['action_' + actionId];
                            return;
                        }
                    }, function () { });
                };
                PermissionDetailModalComponent.prototype.delete = function (page) {
                    var _this = this;
                    swal({
                        title: "Bạn có chắc chắn muốn Trang này này?",
                        showCancelButton: true,
                        confirmButtonText: 'Xóa',
                        cancelButtonText: 'Không'
                    })
                        .then(function () {
                        _this.groupPermissionService.deletePermission(_this.permission.id, page.pageId)
                            .subscribe(function (data) {
                            if (data.status > 0) {
                                toastr.success(data.content);
                                _.remove(_this.pages, function (item) { return item.pageId === page.pageId; });
                                // ReSharper disable once CoercedEqualsUsing
                                _.remove(_this.pageIds, function (item) { return item == page.pageId; });
                                return;
                            }
                            toastr.error(data.content);
                        }, function () { });
                    });
                };
                PermissionDetailModalComponent.prototype.ngAfterContentInit = function () {
                    $(function () {
                    });
                };
                return PermissionDetailModalComponent;
            }());
            PermissionDetailModalComponent = __decorate([
                core_1.Component({
                    selector: 'permission-detail-modal',
                    templateUrl: '/app/components/permission/permission-detail-modal.html',
                    providers: [permission_action_service_1.PermissionActionService]
                }),
                __metadata("design:paramtypes", [permission_action_service_1.PermissionActionService, group_permission_service_1.GroupPermissionService])
            ], PermissionDetailModalComponent);
            exports_1("PermissionDetailModalComponent", PermissionDetailModalComponent);
        }
    };
});
//# sourceMappingURL=permission-detail-modal.component.js.map