System.register(["@angular/core", "@angular/platform-browser", "@angular/forms", "@angular/http", "@angular/common", "./components/paging-component", "./components/page/page.component", "./components/page/modal-page.component", "./components/control-messages.component"], function (exports_1, context_1) {
    "use strict";
    var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
        var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
        if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
        else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
        return c > 3 && r && Object.defineProperty(target, key, r), r;
    };
    var __moduleName = context_1 && context_1.id;
    var core_1, platform_browser_1, forms_1, forms_2, http_1, common_1, paging_component_1, page_component_1, modal_page_component_1, control_messages_component_1, PageModule;
    return {
        setters: [
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (platform_browser_1_1) {
                platform_browser_1 = platform_browser_1_1;
            },
            function (forms_1_1) {
                forms_1 = forms_1_1;
                forms_2 = forms_1_1;
            },
            function (http_1_1) {
                http_1 = http_1_1;
            },
            function (common_1_1) {
                common_1 = common_1_1;
            },
            function (paging_component_1_1) {
                paging_component_1 = paging_component_1_1;
            },
            function (page_component_1_1) {
                page_component_1 = page_component_1_1;
            },
            function (modal_page_component_1_1) {
                modal_page_component_1 = modal_page_component_1_1;
            },
            function (control_messages_component_1_1) {
                control_messages_component_1 = control_messages_component_1_1;
            }
        ],
        execute: function () {
            PageModule = (function () {
                function PageModule() {
                }
                return PageModule;
            }());
            PageModule = __decorate([
                core_1.NgModule({
                    imports: [platform_browser_1.BrowserModule, common_1.CommonModule, forms_1.FormsModule, forms_2.ReactiveFormsModule, http_1.HttpModule],
                    declarations: [page_component_1.PageComponent, paging_component_1.PagingComponent, modal_page_component_1.ModalPageComponent, control_messages_component_1.ControlMessages],
                    bootstrap: [page_component_1.PageComponent]
                })
            ], PageModule);
            exports_1("PageModule", PageModule);
        }
    };
});
//# sourceMappingURL=page.module.js.map