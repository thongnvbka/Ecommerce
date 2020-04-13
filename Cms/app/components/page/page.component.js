System.register(["@angular/core", "../../services/app.service", "../../services/module.service", "../../services/page.service", "../../services/permission-action.service", "../../services/role-action.service"], function (exports_1, context_1) {
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
    var core_1, app_service_1, module_service_1, page_service_1, permission_action_service_1, role_action_service_1, PageComponent;
    return {
        setters: [
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (app_service_1_1) {
                app_service_1 = app_service_1_1;
            },
            function (module_service_1_1) {
                module_service_1 = module_service_1_1;
            },
            function (page_service_1_1) {
                page_service_1 = page_service_1_1;
            },
            function (permission_action_service_1_1) {
                permission_action_service_1 = permission_action_service_1_1;
            },
            function (role_action_service_1_1) {
                role_action_service_1 = role_action_service_1_1;
            }
        ],
        execute: function () {
            PageComponent = (function () {
                function PageComponent(appService, moduleService, pageService, roleActionService) {
                    var _this = this;
                    this.appService = appService;
                    this.moduleService = moduleService;
                    this.pageService = pageService;
                    this.roleActionService = roleActionService;
                    this.recordPerPage = 20;
                    this.currentPage = 1;
                    this.pages = [];
                    this.totalRecord = 0;
                    this.isSearch = false;
                    this.isError = false;
                    this.modules = [];
                    this.apps = [];
                    this.roleActions = [];
                    this.pageFilter = {
                        keyword: '',
                        appId: null,
                        moduleId: null,
                        modulesByApId: []
                    };
                    // Lấy ra hết các App
                    this.appService.getAll()
                        .subscribe(function (data) {
                        _this.apps = data;
                    });
                    // Lấy ra hết các module
                    this.moduleService.getAll()
                        .subscribe(function (data) {
                        _this.modules = data;
                    });
                    // Lấy ra hết roleAction
                    this.roleActionService.getAll()
                        .subscribe(function (data) {
                        _this.roleActions = data;
                    });
                    this.search(1);
                }
                PageComponent.prototype.changeApp = function (appId) {
                    // ReSharper disable once CoercedEqualsUsing
                    this.pageFilter.modulesByApId = this.modules.filter(function (x) { return x.appId == appId; });
                    this.search(1);
                };
                PageComponent.prototype.changeModule = function (moduleId) {
                    this.search(1);
                };
                PageComponent.prototype.search = function (currentPage) {
                    var _this = this;
                    this.currentPage = currentPage;
                    this.isSearch = true;
                    this.pageService.search(this.pageFilter.keyword, this.pageFilter.appId, this.pageFilter.moduleId, this.recordPerPage, this.currentPage)
                        .subscribe(function (data) {
                        var groupActions = _.groupBy(data.permissionActions, "pageId");
                        data.items.forEach(function (i) {
                            i.actions = groupActions[i.id + ""];
                            i.actions.forEach(function (ac) {
                                i["action_" + ac.roleActionId] = ac.checked;
                            });
                        });
                        _this.pages = data.items;
                        _this.totalRecord = data.totalRecord;
                    }, function () { }, function () { return _this.isSearch = false; });
                };
                PageComponent.prototype.afterHideModal = function (refresh) {
                    if (refresh)
                        this.search(1);
                };
                PageComponent.prototype.delete = function (page) {
                    var _this = this;
                    swal({
                        title: "Bạn có chắc chắn muốn xóa page này?",
                        showCancelButton: true,
                        confirmButtonText: 'Delete',
                        cancelButtonText: 'Không xóa'
                    })
                        .then(function () {
                        _this.pageService.delete(page.id)
                            .subscribe(function (data) {
                            if (data.status > 0) {
                                toastr.success(data.content);
                                _.remove(_this.pages, function (item) { return item.id === page.id; });
                                return;
                            }
                            toastr.error(data.content);
                        }, function () { });
                    });
                };
                return PageComponent;
            }());
            PageComponent = __decorate([
                core_1.Component({
                    selector: 'page',
                    templateUrl: '/app/components/page/page.html',
                    providers: [app_service_1.AppService, module_service_1.ModuleService, role_action_service_1.RoleActionService, page_service_1.PageService, permission_action_service_1.PermissionActionService]
                }),
                __metadata("design:paramtypes", [app_service_1.AppService,
                    module_service_1.ModuleService, page_service_1.PageService, role_action_service_1.RoleActionService])
            ], PageComponent);
            exports_1("PageComponent", PageComponent);
        }
    };
});
//# sourceMappingURL=page.component.js.map