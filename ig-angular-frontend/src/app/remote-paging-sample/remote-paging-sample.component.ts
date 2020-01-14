import { AfterViewInit, Component, OnDestroy, OnInit, TemplateRef, ViewChild, ViewEncapsulation } from "@angular/core";
import { IgxGridComponent, IgxColumnComponent, NoopFilteringStrategy, NoopSortingStrategy, IGridEditEventArgs } from "igniteui-angular";
import { Observable, Subject } from "rxjs";
import { RemotePagingService } from "../services/remotePagingService";
import { debounceTime, takeUntil } from "rxjs/operators";
import { FormGroup, FormControl } from '@angular/forms';

const DEBOUNCE_TIME = 300;
@Component({
    encapsulation: ViewEncapsulation.None,
    providers: [RemotePagingService],
    selector: "remote-paging-grid-sample",
    styleUrls: ["./remote-paging-sample.component.scss"],
    templateUrl: "./remote-paging-sample.component.html"
})
export class RemotePagingGridSample implements OnInit, AfterViewInit, OnDestroy {

    public page = 0;
    public totalCount = 0;
    public pages = [];
    public data: Observable<any[]>;
    public selectOptions = [5, 10, 15, 25, 50];
    public noopFilterStrategy = NoopFilteringStrategy.instance();
    public noopSortStrategy = NoopSortingStrategy.instance();

    @ViewChild("customPager", { read: TemplateRef, static: true }) public remotePager: TemplateRef<any>;
    @ViewChild("grid1", { read: IgxGridComponent, static: true }) public grid1: IgxGridComponent;

    private _perPage = 10;
    private _prevRequest: any;
    private destroy$ = new Subject<boolean>();
    private _dataLengthSubscriber;
    private _prevBool = true;
    sortingExpr: any;
    filteringExpr: any;
    searchText: string;

    public contextmenu = false;
    public contextmenuX = 0;
    public contextmenuY = 0;
    public clickedCell = null;
    public copiedData;
    public multiCellSelection: { data: any[]} = { data: []};
    public multiCellArgs;
    
    public hColumns = [];
    
    constructor(private remoteService: RemotePagingService) {
    }

    public get perPage(): number {
        return this._perPage;
    }

    public set perPage(val: number) {
        this._perPage = val;
        this.paginate(0);
    }

    public ngOnInit() {
        this.data = this.remoteService.remoteData.asObservable();

        this._dataLengthSubscriber = this.remoteService.getDataLength(this.filteringExpr, this.sortingExpr, this.searchText).subscribe((data) => {
            this.totalCount = data;
            this.grid1.isLoading = false;
        });
    }

    public ngOnDestroy() {
        if (this._dataLengthSubscriber) {
            this._dataLengthSubscriber.unsubscribe();
        }
        if (this._prevRequest) {
            this._prevRequest.unsubscribe();
        }
        this.destroy$.next();
        this.destroy$.complete();
    }
    
    public onColumnInit(column: IgxColumnComponent) {
        this.hColumns.push({index:column.index, label: column.field, checked: column.hidden});
        column.movable = true;
        column.sortable = true;
        this._prevBool = column.editable = !this._prevBool;
    }

    public onChecklistChange(value:any,column:any){
        this.grid1.columns[column.index].hidden = value;
    }

    public editDone(event: IGridEditEventArgs) {
        let editEvent = {originalRowObj : event.oldValue,
             updatedRowObj : event.newValue,
             cancelValue : event.cancel,
             rowID : event.rowID,} 
        console.log('editDone',editEvent)
    }

    public ngAfterViewInit() {
        this.filteringExpr = this.grid1.filteringExpressionsTree.filteringOperands;
        this.sortingExpr = this.grid1.sortingExpressions[0];
        this.grid1.isLoading = true;
        this.remoteService.getData(0, this.perPage, this.filteringExpr, this.sortingExpr, this.searchText);

        this.grid1.filteringExpressionsTreeChange.pipe(
            debounceTime(DEBOUNCE_TIME),
            takeUntil(this.destroy$)
        ).subscribe(() => {
            this.paginate(0);
        });
        this.grid1.sortingExpressionsChange.pipe(
            debounceTime(DEBOUNCE_TIME),
            takeUntil(this.destroy$)
        ).subscribe(() => {
            this.paginate(0);
        });
    }

    public processData(skip: number, top: number) {
        if (this._prevRequest) {
            this._prevRequest.unsubscribe();
        }

        if (!this.grid1.isLoading) {
            this.grid1.isLoading = true;
        }

        this.filteringExpr = this.grid1.filteringExpressionsTree.filteringOperands;
        this.sortingExpr = this.grid1.sortingExpressions[0];

        this._dataLengthSubscriber = this.remoteService.getDataLength(this.filteringExpr, this.sortingExpr, this.searchText).subscribe((data) => {
            this.totalCount = data;
            this.grid1.isLoading = false;
        });

        this._prevRequest =  this.remoteService.getData(skip, top, this.filteringExpr, this.sortingExpr, this.searchText);
    }

    public paginate(page: number) {
        this.page = page;
        const skip = this.page * this.perPage;
        const top = this.perPage;
        this.processData(skip, top);
    }

    public fulltextSearch(searchText: string){
        this.searchText = searchText;
        this.paginate(0);
    }

    public rightClick(eventArgs: any) {
        eventArgs.event.preventDefault();
        this.multiCellArgs = {};
        if (this.multiCellSelection) {
            const node = eventArgs.cell.selectionNode;
            const isCellWithinRange = this.grid1.getSelectedRanges().some((range) => {
                if (node.column >= range.columnStart &&
                    node.column <= range.columnEnd &&
                    node.row >= range.rowStart &&
                    node.row <= range.rowEnd) {
                    return true;
                }
                return false;
            });
            if (isCellWithinRange) {
                this.multiCellArgs = { data: this.multiCellSelection.data };
            }
        }
        this.contextmenuX = eventArgs.event.clientX;
        this.contextmenuY = eventArgs.event.clientY;
        this.clickedCell = eventArgs.cell;
        this.contextmenu = true;
    }

    public disableContextMenu() {
        if (this.contextmenu) {
            this.multiCellSelection = undefined;
            this.multiCellArgs = undefined;
            this.contextmenu = false;
        }
    }

    public getCells(event) {
        this.multiCellSelection = {
            data: this.grid1.getSelectedData()
        };
    }

    public copy(event) {
        this.copiedData = JSON.stringify(event.data, null, 2);
        if (this.multiCellSelection) {
            this.multiCellSelection = undefined;
            this.multiCellArgs = undefined;
            this.grid1.clearCellSelection();
        }
    }

    public cellSelection(event) {
        this.contextmenu = false;
    }
}
