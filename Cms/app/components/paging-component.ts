import {Component, Output, Input, EventEmitter, HostListener, HostBinding} from '@angular/core';

@Component({
    selector: 'pager',
    template:
    '<div class="row">\
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
    styles: [`
    .pagination {
      margin: 0;
    },
    .henry-paging{
        line-height: 35px;
    }
  `]
})

export class PagingComponent {
    @Input() totalRecord: number;
    @Input() recordPerPage: number = 20;
    @Input() pagePerSegment: number = 6;
    @Input() recordName: string = 'recordName';

    @Output() pageClick = new EventEmitter<number>();

    private totalPage: number;
    private listPage: Array<number> = [];
    private currentPage: number = 1;

    
    private currentSegment: number = 1;
    private totalSegment: number;

    private maxPageInSegment: number;
    private minPageInSegment: number;


    ngOnChanges(changes) {
        this.totalPage = Math.ceil(this.totalRecord / this.recordPerPage);
        this.totalSegment = Math.ceil(this.totalPage / this.pagePerSegment);

        this.renderSement();
    }

    renderSement() {
        this.minPageInSegment = (this.currentSegment - 1) * this.pagePerSegment + 1;
        this.maxPageInSegment = this.minPageInSegment + this.pagePerSegment;

        this.listPage = [];
        for (var i = this.minPageInSegment; i <= this.maxPageInSegment; i++) {
            if (i > this.totalPage) 
                break;

            this.listPage.push(i);
        }
    }

    previousSegment() {
        this.currentSegment -= 1;
        this.renderSement();
    }

    previous() {
        this.currentPage -= 1;
        if (this.currentPage < this.minPageInSegment) {
            this.previousSegment();
        }
        this.pageClick.emit(this.currentPage);
    }

    page(idx: number) {
        if (idx === this.currentPage)
            return;

        this.currentPage = idx;
        this.pageClick.emit(this.currentPage);
    }

    nextSegment() {
        this.currentSegment += 1;
        this.renderSement();
    }

    next() {
        this.currentPage += 1;
        if (this.currentPage > this.maxPageInSegment) {
            this.nextSegment();
        }
        this.pageClick.emit(this.currentPage);
    }
}