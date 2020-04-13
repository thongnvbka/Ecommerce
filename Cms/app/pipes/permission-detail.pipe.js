System.register(["@angular/core", '../services/common.service'], function(exports_1, context_1) {
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
    var core_1, common_service_1;
    var PermissionDetailPipe;
    return {
        setters:[
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (common_service_1_1) {
                common_service_1 = common_service_1_1;
            }],
        execute: function() {
            PermissionDetailPipe = (function () {
                function PermissionDetailPipe() {
                }
                PermissionDetailPipe.prototype.transform = function (values, term) {
                    var keyword = common_service_1.CommonService.stripVietnameseChars(term).toLowerCase();
                    if (_.trim(keyword) === "")
                        return values;
                    var items = values.filter(function (i) {
                        var pageName = common_service_1.CommonService.stripVietnameseChars(i.pageName).toLowerCase();
                        return pageName.indexOf(keyword) >= 0;
                    });
                    var moduleIdFist;
                    _.forEach(items, function (item) {
                        item.firstRecord = moduleIdFist !== item.moduleId;
                        item.pathName = item.appName + " / " + item.moduleName;
                        moduleIdFist = item.moduleId;
                    });
                    return items;
                };
                PermissionDetailPipe = __decorate([
                    core_1.Pipe({
                        name: "search"
                    }), 
                    __metadata('design:paramtypes', [])
                ], PermissionDetailPipe);
                return PermissionDetailPipe;
            }());
            exports_1("PermissionDetailPipe", PermissionDetailPipe);
        }
    }
});
//# sourceMappingURL=permission-detail.pipe.js.map