System.register(['@angular/core', '../../services/page.service', '../../services/group-permission.service'], function(exports_1, context_1) {
    "use strict";
    var __moduleName = context_1 && context_1.id;
    var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
        var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
        if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
        else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
        return c > 3 && r && Object.defineProperty(target, key, r), r;
    };
    var __metadata = (this && this.__metadata) || function (k, v) {
        if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
    };
    var core_1, page_service_1, group_permission_service_1;
    var PermissionPageModalComponent;
    return {
        setters:[
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (page_service_1_1) {
                page_service_1 = page_service_1_1;
            },
            function (group_permission_service_1_1) {
                group_permission_service_1 = group_permission_service_1_1;
            }],
        execute: function() {
            PermissionPageModalComponent = (function () {
                function PermissionPageModalComponent(pageService, permissionService) {
                    var _this = this;
                    this.pageService = pageService;
                    this.permissionService = permissionService;
                    this.afterHideModal = new core_1.EventEmitter();
                    this.apps = [];
                    this.modules = [];
                    this.pages = [];
                    this.actions = [];
                    this.pageTree = [];
                    this.isSubmit = false;
                    this.hasUpdate = false;
                    var idx = 1;
                    this.pageTree.push({
                        id: 1,
                        text: 'Erp System',
                        icon: 'fa fa-desktop icon-state-info',
                        parent: '#',
                        state: {
                            disabled: true,
                            opened: true
                        }
                    });
                    this.pageService.getPageAllTree()
                        .subscribe(function (data) {
                        _this.apps = data.apps;
                        _this.modules = data.modules;
                        _this.pages = data.pages;
                        _this.actions = data.actions;
                        _this.actionsGroupModule = _.groupBy(data.actions, "pageId");
                        var moduleGroup = _.groupBy(data.modules, "appId");
                        var pageGroup = _.groupBy(data.pages, "moduleId");
                        // foreach Apps
                        data.apps.forEach(function (app) {
                            idx += 1;
                            _this.pageTree.push({
                                id: idx,
                                text: app.name,
                                icon: app.icon + ' icon-state-info',
                                parent: '1',
                                state: {
                                    disabled: true,
                                    opened: false
                                }
                            });
                            var modules = moduleGroup[app.id + ""];
                            if (modules) {
                                var appId_1 = idx;
                                // foreach Modules level 1
                                _.filter(modules, function (m) {
                                    return m.parentId === null;
                                }).forEach(function (module) {
                                    idx += 1;
                                    _this.pageTree.push({
                                        id: idx,
                                        text: module.name,
                                        icon: 'fa fa-folder icon-state-info',
                                        parent: appId_1 + '',
                                        state: {
                                            disabled: true,
                                            opened: true
                                        }
                                    });
                                    var moduleId = idx;
                                    // foreach Modules level 2
                                    _.filter(modules, function (m) {
                                        return m.parentId === module.id;
                                    }).forEach(function (module) {
                                        idx += 1;
                                        _this.pageTree.push({
                                            id: idx,
                                            text: module.name,
                                            icon: 'fa fa-folder icon-state-info',
                                            parent: moduleId + '',
                                            state: {
                                                disabled: true,
                                                opened: false
                                            }
                                        });
                                        var moduleId2 = idx;
                                        var pages2 = pageGroup[module.id + ""];
                                        if (pages2) {
                                            // foreach page
                                            pages2.forEach(function (page) {
                                                idx += 1;
                                                _this.pageTree.push({
                                                    id: idx,
                                                    text: page.name,
                                                    icon: 'fa fa-file icon-state-success',
                                                    parent: moduleId2 + '',
                                                    pageId: page.id,
                                                    state: {
                                                        disabled: false,
                                                        opened: false
                                                    }
                                                });
                                            });
                                        }
                                    });
                                    // foreach page of Module level 1
                                    var pages1 = pageGroup[module.id + ""];
                                    if (pages1) {
                                        // foreach page
                                        pages1.forEach(function (page) {
                                            idx += 1;
                                            _this.pageTree.push({
                                                id: idx,
                                                text: page.name,
                                                icon: 'fa fa-file icon-state-success',
                                                parent: moduleId + '',
                                                pageId: page.id,
                                                state: {
                                                    disabled: false,
                                                    opened: false
                                                }
                                            });
                                        });
                                    }
                                });
                            }
                        });
                        _this.initJsTree();
                    }, function () { });
                }
                PermissionPageModalComponent.prototype.show = function (pageIds) {
                    // Disable page
                    _.forEach(_.filter(this.pageTree, function (pageTree) {
                        return pageTree.hasOwnProperty("pageId");
                    }), function (page) {
                        if (pageIds.indexOf(page.pageId + "") >= 0) {
                            $('#page-tree').jstree(true).disable_node(page.id);
                            return;
                        }
                        $('#page-tree').jstree(true).enable_node(page.id);
                    });
                    $("#permission-page-modal").modal('show');
                    this.hasUpdate = false;
                    this.isSubmit = false;
                };
                PermissionPageModalComponent.prototype.hide = function () {
                    $("#permission-page-modal").modal('hide');
                };
                PermissionPageModalComponent.prototype.save = function () {
                    var _this = this;
                    if (this.page === null || this.page.actions === null || this.page.actions.length === 0) {
                        toastr.error("Chưa có trang nào được chọn");
                        return;
                    }
                    this.isSubmit = true;
                    this.permissionService.addPermission(this.page)
                        .subscribe(function (data) {
                        if (data.status > 0) {
                            toastr.success(data.content);
                            _this.refreshJsTree(_this.page.nodeId);
                            _this.hasUpdate = true;
                            return;
                        }
                        toastr.error(data.content);
                    }, function () { }, function () { _this.isSubmit = false; });
                };
                PermissionPageModalComponent.prototype.refreshJsTree = function (nodeId) {
                    this.page.actions = [];
                    this.page.nodeId = null;
                    this.page.pageId = null;
                    $('#page-tree').jstree(true).disable_node(nodeId);
                    $('#page-tree').jstree(true).deselect_node(nodeId);
                };
                PermissionPageModalComponent.prototype.initJsTree = function () {
                    var _this = this;
                    $('#page-tree').jstree({
                        'plugins': ["types"],
                        'core': {
                            "themes": {
                                "responsive": false
                            },
                            'data': this.pageTree
                        }
                    }).on('select_node.jstree', function (e, data) {
                        if (data.node.state.disabled)
                            return;
                        var actions = [];
                        _.each(_this.actionsGroupModule[data.node.original.pageId + ""], function (ac) {
                            actions.push({ id: ac.roleActionId, name: ac.roleName, checked: true });
                        });
                        _this.page = {
                            nodeId: data.selected,
                            actions: actions,
                            groupPermissionId: _this.permission.id,
                            pageId: data.node.original.pageId
                        };
                    });
                };
                PermissionPageModalComponent.prototype.ngAfterContentInit = function () {
                    var _this = this;
                    $(function () {
                        $('#permission-page-modal').on("hide", function () {
                            _this.afterHideModal.emit(_this.hasUpdate);
                        });
                    });
                };
                __decorate([
                    core_1.Input(), 
                    __metadata('design:type', Object)
                ], PermissionPageModalComponent.prototype, "permission", void 0);
                __decorate([
                    core_1.Output(), 
                    __metadata('design:type', Object)
                ], PermissionPageModalComponent.prototype, "afterHideModal", void 0);
                PermissionPageModalComponent = __decorate([
                    core_1.Component({
                        selector: 'permission-page-modal',
                        templateUrl: '/app/components/permission/permission-page-modal.html',
                        providers: [page_service_1.PageService, group_permission_service_1.GroupPermissionService]
                    }), 
                    __metadata('design:paramtypes', [page_service_1.PageService, group_permission_service_1.GroupPermissionService])
                ], PermissionPageModalComponent);
                return PermissionPageModalComponent;
            }());
            exports_1("PermissionPageModalComponent", PermissionPageModalComponent);
        }
    }
});
//# sourceMappingURL=permission-page-modal.component.js.map