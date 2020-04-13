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
    var PermissionActionService;
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
            PermissionActionService = (function () {
                function PermissionActionService(http) {
                    this.http = http;
                    this.apiUrl = "/permission-action";
                }
                PermissionActionService.prototype.setHeaders = function () {
                    this.headers = new http_1.Headers();
                    this.headers.append('Content-Type', 'application/json');
                    this.headers.append('Accept', 'application/json');
                    this.headers.append('Authorization', 'Bearer ' + localStorage.getItem('access_token'));
                };
                PermissionActionService.prototype.getByPageId = function (pageId) {
                    var url = this.apiUrl + "/by-page-" + pageId;
                    this.setHeaders();
                    var options = new http_1.RequestOptions({ headers: this.headers, body: '' });
                    return this.http.get(url, options).map(function (x) { return x.json(); });
                };
                PermissionActionService.prototype.getByPermissionId = function (permissionId, keyword) {
                    if (keyword === void 0) { keyword = ''; }
                    var params = new http_1.URLSearchParams();
                    params.set('keyword', keyword);
                    params.set('permissionId', permissionId.toString());
                    var url = this.apiUrl + "/get-by-permission";
                    this.setHeaders();
                    var options = new http_1.RequestOptions({ headers: this.headers, body: '', search: params });
                    return this.http.get(url, options).map(function (x) { return x.json(); });
                };
                PermissionActionService = __decorate([
                    core_1.Injectable(), 
                    __metadata('design:paramtypes', [http_1.Http])
                ], PermissionActionService);
                return PermissionActionService;
            }());
            exports_1("PermissionActionService", PermissionActionService);
        }
    }
});
//# sourceMappingURL=permission-action.service.js.map