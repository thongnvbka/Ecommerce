System.register(['@angular/core', 'rxjs/Subject'], function(exports_1, context_1) {
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
    var core_1, Subject_1;
    var AppState;
    return {
        setters:[
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (Subject_1_1) {
                Subject_1 = Subject_1_1;
            }],
        execute: function() {
            AppState = (function () {
                function AppState() {
                    var _this = this;
                    this.data = new Subject_1.Subject();
                    this.dataStream$ = this.data.asObservable();
                    this.subscriptions = new Map();
                    this.dataStream$.subscribe(function (data) {
                        _this.onEvent(data);
                    });
                }
                AppState.prototype.notifyDataChanged = function (event, value) {
                    var current = this.data[event];
                    // ReSharper disable once CoercedEqualsUsing
                    if (current != value) {
                        this.data[event] = value;
                        this.data.next({
                            event: event,
                            data: this.data[event]
                        });
                    }
                };
                AppState.prototype.subscribe = function (event, callback) {
                    var subscribers = this.subscriptions.get(event) || [];
                    subscribers.push(callback);
                    this.subscriptions.set(event, subscribers);
                };
                AppState.prototype.onEvent = function (data) {
                    var subscribers = this.subscriptions.get(data['event']) || [];
                    subscribers.forEach(function (callback) {
                        callback(data['data']);
                    });
                };
                AppState = __decorate([
                    core_1.Injectable(), 
                    __metadata('design:paramtypes', [])
                ], AppState);
                return AppState;
            }());
            exports_1("AppState", AppState);
        }
    }
});
//# sourceMappingURL=app.state.js.map