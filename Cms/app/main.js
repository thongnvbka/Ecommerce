System.register(["@angular/platform-browser-dynamic", "./page.module"], function (exports_1, context_1) {
    "use strict";
    var __moduleName = context_1 && context_1.id;
    var platform_browser_dynamic_1, page_module_1, platform;
    return {
        setters: [
            function (platform_browser_dynamic_1_1) {
                platform_browser_dynamic_1 = platform_browser_dynamic_1_1;
            },
            function (page_module_1_1) {
                page_module_1 = page_module_1_1;
            }
        ],
        execute: function () {
            platform = platform_browser_dynamic_1.platformBrowserDynamic();
            platform.bootstrapModule(page_module_1.PageModule);
        }
    };
});
//# sourceMappingURL=main.js.map