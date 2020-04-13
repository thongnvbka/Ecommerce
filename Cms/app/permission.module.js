System.register(["@angular/core", "@angular/common", "@angular/forms", "@angular/platform-browser", "@angular/http", "./components/paging-component", "./components/permission/permission.component", "./components/permission/permission-users-modal.component", "./components/permission/permission-page-modal.component", "./components/permission/permission-modal.component", "./components/permission/permission-detail-modal.component", "./components/control-messages.component", "./pipes/permission-detail.pipe"], function (exports_1, context_1) {
    "use strict";
    var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
        var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
        if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
        else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
        return c > 3 && r && Object.defineProperty(target, key, r), r;
    };
    var __moduleName = context_1 && context_1.id;
    var core_1, common_1, forms_1, forms_2, platform_browser_1, http_1, paging_component_1, permission_component_1, permission_users_modal_component_1, permission_page_modal_component_1, permission_modal_component_1, permission_detail_modal_component_1, control_messages_component_1, permission_detail_pipe_1, PermissionModule;
    return {
        setters: [
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (common_1_1) {
                common_1 = common_1_1;
            },
            function (forms_1_1) {
                forms_1 = forms_1_1;
                forms_2 = forms_1_1;
            },
            function (platform_browser_1_1) {
                platform_browser_1 = platform_browser_1_1;
            },
            function (http_1_1) {
                http_1 = http_1_1;
            },
            function (paging_component_1_1) {
                paging_component_1 = paging_component_1_1;
            },
            function (permission_component_1_1) {
                permission_component_1 = permission_component_1_1;
            },
            function (permission_users_modal_component_1_1) {
                permission_users_modal_component_1 = permission_users_modal_component_1_1;
            },
            function (permission_page_modal_component_1_1) {
                permission_page_modal_component_1 = permission_page_modal_component_1_1;
            },
            function (permission_modal_component_1_1) {
                permission_modal_component_1 = permission_modal_component_1_1;
            },
            function (permission_detail_modal_component_1_1) {
                permission_detail_modal_component_1 = permission_detail_modal_component_1_1;
            },
            function (control_messages_component_1_1) {
                control_messages_component_1 = control_messages_component_1_1;
            },
            function (permission_detail_pipe_1_1) {
                permission_detail_pipe_1 = permission_detail_pipe_1_1;
            }
        ],
        execute: function () {
            PermissionModule = (function () {
                function PermissionModule() {
                }
                return PermissionModule;
            }());
            PermissionModule = __decorate([
                core_1.NgModule({
                    imports: [platform_browser_1.BrowserModule, common_1.CommonModule, forms_1.FormsModule, forms_2.ReactiveFormsModule, http_1.HttpModule],
                    //declarations: [PermissionComponent, PagingComponent, PermissionUsersModalComponent, PermissionPageModalComponent,
                    //    PermissionModalComponent, PermissionDetailModalComponent, , ControlMessages],
                    declarations: [permission_component_1.PermissionComponent, paging_component_1.PagingComponent, permission_modal_component_1.PermissionModalComponent,
                        permission_users_modal_component_1.PermissionUsersModalComponent, permission_detail_modal_component_1.PermissionDetailModalComponent, control_messages_component_1.ControlMessages,
                        permission_detail_pipe_1.PermissionDetailPipe, permission_page_modal_component_1.PermissionPageModalComponent],
                    bootstrap: [permission_component_1.PermissionComponent]
                })
            ], PermissionModule);
            exports_1("PermissionModule", PermissionModule);
        }
    };
});
//# sourceMappingURL=permission.module.js.map