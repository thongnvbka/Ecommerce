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
    var PageService;
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
            PageService = (function () {
                function PageService(http) {
                    this.http = http;
                    this.apiUrl = "/page";
                    ;
                }
                PageService.prototype.setHeaders = function () {
                    this.headers = new http_1.Headers();
                    this.headers.append('Content-Type', 'application/json');
                    this.headers.append('Accept', 'application/json');
                    this.headers.append('Authorization', 'Bearer ' + localStorage.getItem('access_token'));
                };
                PageService.prototype.getPageAllTree = function () {
                    var url = this.apiUrl + "/page-all-tree";
                    this.setHeaders();
                    var options = new http_1.RequestOptions({ headers: this.headers, body: '' });
                    return this.http.get(url, options).map(function (x) { return x.json(); });
                };
                PageService.prototype.getAll = function () {
                    var url = this.apiUrl + "/all";
                    this.setHeaders();
                    var options = new http_1.RequestOptions({ headers: this.headers, body: '' });
                    return this.http.get(url, options).map(function (x) { return x.json(); });
                };
                PageService.prototype.getByAppId = function (appId) {
                    var url = this.apiUrl + "/app-id" + appId;
                    this.setHeaders();
                    var options = new http_1.RequestOptions({ headers: this.headers, body: '' });
                    return this.http.get(url, options).map(function (x) { return x.json(); });
                };
                PageService.prototype.getByModuleId = function (appId) {
                    var url = this.apiUrl + "/module-id" + appId;
                    this.setHeaders();
                    var options = new http_1.RequestOptions({ headers: this.headers, body: '' });
                    return this.http.get(url, options).map(function (x) { return x.json(); });
                };
                PageService.prototype.search = function (keyword, appId, moduleId, recordPerPage, currentPage) {
                    if (keyword === void 0) { keyword = ''; }
                    var params = new http_1.URLSearchParams();
                    params.set('keyword', keyword);
                    if (appId)
                        params.set('appId', appId.toString());
                    if (moduleId)
                        params.set('moduleId', moduleId.toString());
                    if (recordPerPage)
                        params.set('recordPerPage', recordPerPage.toString());
                    if (currentPage)
                        params.set('currentPage', currentPage.toString());
                    var url = this.apiUrl + "/search";
                    this.setHeaders();
                    var options = new http_1.RequestOptions({ headers: this.headers, body: '', search: params });
                    return this.http.get(url, options).map(function (x) { return x.json(); });
                };
                PageService.prototype.add = function (page) {
                    var url = this.apiUrl + "/add";
                    this.setHeaders();
                    var options = new http_1.RequestOptions({ headers: this.headers });
                    return this.http.post(url, JSON.stringify(page), options).map(function (res) { return res.json(); });
                };
                PageService.prototype.update = function (page) {
                    var url = this.apiUrl + "/update";
                    this.setHeaders();
                    var options = new http_1.RequestOptions({ headers: this.headers });
                    return this.http.post(url, JSON.stringify(page), options).map(function (res) { return res.json(); });
                };
                PageService.prototype.delete = function (id) {
                    var url = this.apiUrl + "/delete/?id=" + id;
                    this.setHeaders();
                    var options = new http_1.RequestOptions({ headers: this.headers, body: '' });
                    return this.http.post(url, options).map(function (res) { return res.json(); });
                };
                PageService = __decorate([
                    core_1.Injectable(), 
                    __metadata('design:paramtypes', [http_1.Http])
                ], PageService);
                return PageService;
            }());
            exports_1("PageService", PageService);
        }
    }
});
//# sourceMappingURL=page.service.js.map