
import { BehaviorSubject } from "rxjs";
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { SortingDirection, FilteringLogic } from 'igniteui-angular';

const DATA_URL = "http://localhost:13489/api/projectconstruction";
//"http://localhost:13489/msodata/pbk/v_project_construction_msk";
const EMPTY_STRING = "";
const NULL_VALUE = null;
export enum FILTER_OPERATION {
    CONTAINS = "contains",
    STARTS_WITH = "startswith",
    ENDS_WITH = "endswith",
    EQUALS = "eq",
    DOES_NOT_EQUAL = "ne",
    DOES_NOT_CONTAIN = "not contains",
    GREATER_THAN = "gt",
    LESS_THAN = "lt",
    LESS_THAN_EQUAL = "le",
    GREATER_THAN_EQUAL = "ge"
}
@Injectable()
export class RemotePagingService {
    public remoteData: BehaviorSubject<any[]>;
    public dataLenght: BehaviorSubject<number> = new BehaviorSubject(0);
    //public url = "http://localhost:13489/api/projectconstruction"
    //"https://www.igniteui.com/api/products";

    constructor(private http: HttpClient) {
        this.remoteData = new BehaviorSubject([]);
    }

    public getData(index?: number, perPage?: number, filteringArgs?: any, sortingArgs?: any, searchText?: string): any {
        let qS = this.buildDataUrl(index,perPage,filteringArgs,sortingArgs,searchText)
        console.log('getData',qS)
        this.http
            .get(qS).pipe(
                map((data: any) => {
                    if (data && data.value)
                        data = data.value;
                    return data;
                })
            ).subscribe((data) => this.remoteData.next(data));
    }

    public getDataLength(filteringArgs?: any, sortingArgs?: any, searchText?: string): any {
        let qS = this.buildDataUrl(undefined,undefined,filteringArgs,sortingArgs,searchText)
        //console.log('getDataLength',qS)
        return this.http.get(qS).pipe(
            map((data: any) => {
                if (data && data.value)
                    data = data.value;
                return data.length;
            })
        );
    }
    private buildDataUrl(index?: number, perPage?: number, filteringArgs?: any, sortingArgs?: any, searchText?: string): string {
        let baseQueryString = `${DATA_URL}`
        if (perPage) {
            baseQueryString += `?$skip=${index}&$top=${perPage}&$count=true`;
        }
        else
        {
            baseQueryString += `?$count=true`;
        }

        let orderQuery = EMPTY_STRING;
        let filterQuery = EMPTY_STRING;
        let query = EMPTY_STRING;
        let filter = EMPTY_STRING;

        if (sortingArgs) {
            orderQuery = this._buildSortExpression(sortingArgs);
        }

        if (filteringArgs && filteringArgs.length > 0) {
            filteringArgs.forEach((columnFilter) => {
                if (filter !== EMPTY_STRING) {
                    filter += ` ${FilteringLogic[FilteringLogic.And].toLowerCase()} `;
                }
                filter += this._buildAdvancedFilterExpression(
                    columnFilter.filteringOperands,
                    columnFilter.operator);
            });

            filterQuery = `$filter=${filter}`;
        }

        query += (orderQuery !== EMPTY_STRING) ? `&${orderQuery}` : EMPTY_STRING;
        query += (filterQuery !== EMPTY_STRING) ? `&${filterQuery}` : EMPTY_STRING;

        baseQueryString += query;

        if (searchText)
        baseQueryString += `&text=${searchText}`;

        return baseQueryString;
    }

    private _buildAdvancedFilterExpression(operands, operator): string {
        let filterExpression = EMPTY_STRING;
        let nestedOperands = operands;
        if (operands[0] && operands[0].filteringOperands) {
            operator = operands[0].operator;
            nestedOperands = operands[0].filteringOperands;
        }

        nestedOperands.forEach((operand) => {
            const value = operand.searchVal;
            const isNumberValue = (typeof (value) === "number") ? true : false;
            const filterValue = (isNumberValue) ? value : `'${value}'`;
            const fieldName = operand.fieldName;
            let filterString;

            if (filterExpression !== EMPTY_STRING) {
                filterExpression += ` ${FilteringLogic[operator].toLowerCase()} `;
            }

            switch (operand.condition.name) {
                case "contains": {
                    filterString = `${FILTER_OPERATION.CONTAINS}(${fieldName}, ${filterValue})`;
                    break;
                }
                case "startsWith": {
                    filterString = `${FILTER_OPERATION.STARTS_WITH}(${fieldName},${filterValue})`;
                    break;
                }
                case "endsWith": {
                    filterString = `${FILTER_OPERATION.ENDS_WITH}(${fieldName},${filterValue})`;
                    break;
                }
                case "equals": {
                    filterString = `${fieldName} ${FILTER_OPERATION.EQUALS} ${filterValue} `;
                    break;
                }
                case "doesNotEqual": {
                    filterString = `${fieldName} ${FILTER_OPERATION.DOES_NOT_EQUAL} ${filterValue} `;
                    break;
                }
                case "doesNotContain": {
                    filterString = `${FILTER_OPERATION.DOES_NOT_CONTAIN}(${fieldName},${filterValue})`;
                    break;
                }
                case "greaterThan": {
                    filterString = `${fieldName} ${FILTER_OPERATION.GREATER_THAN} ${filterValue} `;
                    break;
                }
                case "greaterThanOrEqualTo": {
                    filterString = `${fieldName} ${FILTER_OPERATION.GREATER_THAN_EQUAL} ${filterValue} `;
                    break;
                }
                case "lessThan": {
                    filterString = `${fieldName} ${FILTER_OPERATION.LESS_THAN} ${filterValue} `;
                    break;
                }
                case "lessThanOrEqualTo": {
                    filterString = `${fieldName} ${FILTER_OPERATION.LESS_THAN_EQUAL} ${filterValue} `;
                    break;
                }
                case "empty": {
                    filterString = `length(${fieldName}) ${FILTER_OPERATION.EQUALS} 0`;
                    break;
                }
                case "notEmpty": {
                    filterString = `length(${fieldName}) ${FILTER_OPERATION.GREATER_THAN} 0`;
                    break;
                }
                case "null": {
                    filterString = `${fieldName} ${FILTER_OPERATION.EQUALS} ${NULL_VALUE}`;
                    break;
                }
                case "notNull": {
                    filterString = `${fieldName} ${FILTER_OPERATION.DOES_NOT_EQUAL} ${NULL_VALUE}`;
                    break;
                }
            }

            filterExpression += filterString;
        });

        return filterExpression;
    }

    private _buildSortExpression(sortingArgs): string {
        let sortingDirection: string;
        switch (sortingArgs.dir) {
            case SortingDirection.None: {
                sortingDirection = EMPTY_STRING;
                break;
            }
            default: {
                sortingDirection = SortingDirection[sortingArgs.dir].toLowerCase();
                break;
            }
        }

        return `$orderby=${sortingArgs.fieldName} ${sortingDirection}`;
    }

}