import { Component, AfterContentInit, Input, Output, EventEmitter } from '@angular/core';
import { PageService } from '../../services/page.service'
import { GroupPermissionService } from '../../services/group-permission.service';

declare var _;
declare var $;
declare var toastr;

@Component({
    selector: 'permission-page-modal',
    templateUrl: '/app/components/permission/permission-page-modal.html',
    providers: [PageService, GroupPermissionService]
})
export class PermissionPageModalComponent implements AfterContentInit {
    @Input()
    permission;

    @Output()
    afterHideModal = new EventEmitter();

    apps = [];
    modules = [];
    pages = [];
    actions = [];
    actionsGroupModule;
    page: any;
    pageTree = [];

    private isSubmit = false;
    private hasUpdate = false;

    constructor(private pageService: PageService, private permissionService: GroupPermissionService) {
        let idx = 1;
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
            .subscribe(data => {
                this.apps = data.apps;
                this.modules = data.modules;
                this.pages = data.pages;
                this.actions = data.actions;
                this.actionsGroupModule = _.groupBy(data.actions, "pageId");

                let moduleGroup = _.groupBy(data.modules, "appId");
                let pageGroup = _.groupBy(data.pages, "moduleId");

                // foreach Apps
                data.apps.forEach(app => {
                    idx += 1;
                    this.pageTree.push({
                        id: idx,
                        text: app.name,
                        icon: app.icon + ' icon-state-info',
                        parent: '1',
                        state: {
                            disabled: true,
                            opened: false
                        }
                    });

                    let modules = <Array<any>>moduleGroup[app.id + ""];

                    if (modules) {
                        let appId = idx;

                        // foreach Modules level 1
                        _.filter(modules, m => {
                            return m.parentId === null;
                        }).forEach((module) => {
                            idx += 1;
                            this.pageTree.push({
                                id: idx,
                                text: module.name,
                                icon: 'fa fa-folder icon-state-info',
                                parent: appId + '',
                                state: {
                                    disabled: true,
                                    opened: true
                                }
                            });
                            let moduleId = idx;

                            // foreach Modules level 2
                            _.filter(modules, m => {
                                return m.parentId === module.id;
                            }).forEach((module) => {
                                idx += 1;
                                this.pageTree.push({
                                    id: idx,
                                    text: module.name,
                                    icon: 'fa fa-folder icon-state-info',
                                    parent: moduleId + '',
                                    state: {
                                        disabled: true,
                                        opened: false
                                    }
                                });

                                let moduleId2 = idx;

                                let pages2 = <Array<any>>pageGroup[module.id + ""];
                                if (pages2) {
                                    // foreach page
                                    pages2.forEach(page => {
                                        idx += 1;
                                        this.pageTree.push({
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
                            let pages1 = <Array<any>>pageGroup[module.id + ""];
                            if (pages1) {
                                // foreach page

                                pages1.forEach(page => {
                                    idx += 1;
                                    this.pageTree.push({
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
                this.initJsTree();
            },
            ()=>{});
    }

    show(pageIds) {
        // Disable page
        _.forEach(_.filter(this.pageTree, pageTree => {
            return pageTree.hasOwnProperty("pageId");
        }), page => {
            if (pageIds.indexOf(page.pageId + "") >= 0) {
                $('#page-tree').jstree(true).disable_node(page.id);
                return;
            }

            $('#page-tree').jstree(true).enable_node(page.id);
        });

        $("#permission-page-modal").modal('show');
        this.hasUpdate = false;
        this.isSubmit = false;
    }

    hide() {
        $("#permission-page-modal").modal('hide');
    }

    save() {
        if (this.page === null || this.page.actions === null || this.page.actions.length === 0) {
            toastr.error("Chưa có trang nào được chọn");
            return;
        }

        this.isSubmit = true;
        this.permissionService.addPermission(this.page)
            .subscribe(data => {
                if (data.status > 0) {
                    toastr.success(data.content);
                    this.refreshJsTree(this.page.nodeId);
                    this.hasUpdate = true;
                    return;
                }

                toastr.error(data.content);
            },
            ()=>{},
            () => { this.isSubmit = false });
    }

    refreshJsTree(nodeId) {
        this.page.actions = [];
        this.page.nodeId = null;
        this.page.pageId = null;

        $('#page-tree').jstree(true).disable_node(nodeId);
        $('#page-tree').jstree(true).deselect_node(nodeId);
    }

    initJsTree() {
        $('#page-tree').jstree({
            'plugins': ["types"],
            'core': {
                "themes": {
                    "responsive": false
                },
                'data': this.pageTree
            }
        }).on('select_node.jstree', (e, data) => {
            if (data.node.state.disabled)
                return;

            let actions = [];
            _.each(this.actionsGroupModule[data.node.original.pageId + ""],
                ac => {
                    actions.push({ id: ac.roleActionId, name: ac.roleName, checked: true });
                });

            this.page = {
                nodeId: data.selected,
                actions: actions,
                groupPermissionId: this.permission.id,
                pageId: data.node.original.pageId
            };
        });
    }

    ngAfterContentInit() {
        $(() => {
            $('#permission-page-modal').on("hide", () => {
                this.afterHideModal.emit(this.hasUpdate);
            });
        });
    }
}
