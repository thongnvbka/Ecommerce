System.register(["@angular/core"], function (exports_1, context_1) {
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
    var core_1, PagingComponent;
    return {
        setters: [
            function (core_1_1) {
                core_1 = core_1_1;
            }
        ],
        execute: function () {
            PagingComponent = (function () {
                function PagingComponent() {
                    this.recordPerPage = 20;
                    this.pagePerSegment = 6;
                    this.recordName = 'recordName';
                    this.pageClick = new core_1.EventEmitter();
                    this.listPage = [];
                    this.currentPage = 1;
                    this.currentSegment = 1;
                }
                PagingComponent.prototype.ngOnChanges = function (changes) {
                    this.totalPage = Math.ceil(this.totalRecord / this.recordPerPage);
                    this.totalSegment = Math.ceil(this.totalPage / this.pagePerSegment);
                    this.renderSement();
                };
                PagingComponent.prototype.renderSement = function () {
                    this.minPageInSegment = (this.currentSegment - 1) * this.pagePerSegment + 1;
                    this.maxPageInSegment = this.minPageInSegment + this.pagePerSegment;
                    this.listPage = [];
                    for (var i = this.minPageInSegment; i <= this.maxPageInSegment; i++) {
                        if (i > this.totalPage)
                            break;
                        this.listPage.push(i);
                    }
                };
                PagingComponent.prototype.previousSegment = function () {
                    this.currentSegment -= 1;
                    this.renderSement();
                };
                PagingComponent.prototype.previous = function () {
                    this.currentPage -= 1;
                    if (this.currentPage < this.minPageInSegment) {
                        this.previousSegment();
                    }
                    this.pageClick.emit(this.currentPage);
                };
                PagingComponent.prototype.page = function (idx) {
                    if (idx === this.currentPage)
                        return;
                    this.currentPage = idx;
                    this.pageClick.emit(this.currentPage);
                };
                PagingComponent.prototype.nextSegment = function () {
                    this.currentSegment += 1;
                    this.renderSement();
                };
                PagingComponent.prototype.next = function () {
                    this.currentPage += 1;
                    if (this.currentPage > this.maxPageInSegment) {
                        this.nextSegment();
                    }
                    this.pageClick.emit(this.currentPage);
                };
                return PagingComponent;
            }());
            __decorate([
                core_1.Input(),
                __metadata("design:type", Number)
            ], PagingComponent.prototype, "totalRecord", void 0);
            __decorate([
                core_1.Input(),
                __metadata("design:type", Number)
            ], PagingComponent.prototype, "recordPerPage", void 0);
            __decorate([
                core_1.Input(),
                __metadata("design:type", Number)
            ], PagingComponent.prototype, "pagePerSegment", void 0);
            __decorate([
                core_1.Input(),
                __metadata("design:type", String)
            ], PagingComponent.prototype, "recordName", void 0);
            __decorate([
                core_1.Output(),
                __metadata("design:type", Object)
            ], PagingComponent.prototype, "pageClick", void 0);
            PagingComponent = __decorate([
                core_1.Component({
                    selector: 'pager',
                    template: '<div class="row">\
        <div class="col-sm-6">\
            <div class="henry-paging">\
                <span *ngIf="totalPage === 0">No data {{recordName}}</span>\
                <span *ngIf="totalPage !== 0">Show from {{(currentPage - 1) * recordPerPage + 1}} to {{totalRecord <= recordPerPage ? totalRecord : (currentPage - 1) * recordPerPage + recordPerPage}} of {{totalRecord}} {{recordName}}</span>\
            </div>\
        </div>\
        <div *ngIf="totalPage > 1" class="col-sm-6">\
            <nav class="pull-right">\
              <ul class="pagination">\
                <li *ngIf="currentPage !== 1">\
                  <a (click)="previous($event)" href="javascript:;" aria-label="Previous">\
                    <span aria-hidden="true">&laquo;</span>\
                  </a>\
                </li>\
                <li *ngIf="totalSegment > 1 && currentSegment > 1">\
                  <a (click)="previousSegment($event)" href="javascript:;" aria-label="Previous">\
                    <span aria-hidden="true">...</span>\
                  </a>\
                </li>\
                <li *ngFor="let idx of listPage" [class.active]="currentPage === idx" ><a (click)="page(idx)" href="javascript:;">{{idx}}</a></li>\
                <li *ngIf="totalSegment > 1 && currentSegment < totalSegment">\
                  <a (click)="nextSegment($event)" href="javascript:;" aria-label="Next">\
                    <span aria-hidden="true">...</span>\
                  </a>\
                </li>\
                <li *ngIf="currentPage !== totalPage && totalPage > 0">\
                  <a (click)="next($event)" href="javascript:;" aria-label="Next">\
                    <span aria-hidden="true">&raquo;</span>\
                  </a>\
                </li>\
              </ul>\
            </nav>\
        </div>\
    </div>',
                    styles: ["\n    .pagination {\n      margin: 0;\n    },\n    .henry-paging{\n        line-height: 35px;\n    }\n  "]
                })
            ], PagingComponent);
            exports_1("PagingComponent", PagingComponent);
        }
    };
});
//# sourceMappingURL=paging-component.js.map