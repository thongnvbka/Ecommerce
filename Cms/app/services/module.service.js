System.register(['@angular/core', '@angular/http', 'rxjs/Rx'], function(exports_1, context_1) {
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
    var core_1, http_1;
    var ModuleService;
    return {
        setters:[
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (http_1_1) {
                http_1 = http_1_1;
            },
            function (_1) {}],
        execute: function() {
            //import { erpApiDomain } from '../../shared/constants/app.constant';
            ModuleService = (function () {
                function ModuleService(http) {
                    this.http = http;
                    //this.apiDomain = erpApiDomain;
                }
                ModuleService.prototype.setHeaders = function () {
                    this.headers = new http_1.Headers();
                    this.headers.append('Content-Type', 'application/json');
                    this.headers.append('Accept', 'application/json');
                    this.headers.append('Authorization', 'Bearer ' + localStorage.getItem('access_token'));
                };
                ModuleService.prototype.getAll = function () {
                    var url = "/module/all";
                    this.setHeaders();
                    var options = new http_1.RequestOptions({ headers: this.headers, body: '' });
                    return this.http.get(url, options).map(function (x) { return x.json(); });
                };
                ModuleService.prototype.getByAppId = function (appId) {
                    var url = "/module/app-id" + appId;
                    this.setHeaders();
                    var options = new http_1.RequestOptions({ headers: this.headers });
                    return this.http.get(url, options).map(function (x) { return x.json(); });
                };
                ModuleService = __decorate([
                    core_1.Injectable(), 
                    __metadata('design:paramtypes', [http_1.Http])
                ], ModuleService);
                return ModuleService;
            }());
            exports_1("ModuleService", ModuleService);
        }
    }
});
//# sourceMappingURL=module.service.js.map