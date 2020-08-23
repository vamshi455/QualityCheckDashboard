import * as React from 'react';
import { AgGridReact } from 'ag-grid-react';
import 'ag-grid-community/dist/styles/ag-grid.css';
import 'ag-grid-community/dist/styles/ag-theme-alpine.css';
// import 'ag-grid-community/dist/styles/ag-theme-fresh.css';
import './styles.scss';
import { Constants } from '../../Utility/Constants'
import { IState } from './IState';
import axios from 'axios';
import { parse } from 'path';
import { ColumnAnimationService } from 'ag-grid-community/dist/lib/rendering/columnAnimationService';
import { Column, ColDef, _ } from 'ag-grid-community';
import { ColumnDefinition } from '../../Models/ColumnDefinition';
import '../../Utility/gbStyles.scss';
import 'lodash';
import FormModal from '../FormModal';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faCoffee, faPlus, faArchive, faSave } from '@fortawesome/free-solid-svg-icons'
import { Utilities } from './../../Utility/Util';
import { debug } from 'console';







export default class GridLayout extends React.PureComponent<any, any> {
    public util = new Utilities();
    constructor(props: any) {
        super(props);
        this.state = {
            gridOptions: {
                api: {},
                columnApi: {},
                defaultColDef: {
                    flex: 1,
                    minWidth: 180,
                    resizable: true,
                    headerCheckboxSelection: this.isFirstColumn,
                    checkboxSelection: this.isFirstColumn,
                    sortable: true,
                    filter: true,
                    editable: true,
                    headerClass: "class-make",
                    //   newValueHandler: this.compareValues.bind(this),
                    cellClass: this.onCellHigh.bind(this)
                },
                columnDefs: [],
                rowData: null,
                enableSorting: true,
                pagination: true,
                paginationPageSize: 30,
                animateRows: true,
                rowSelection: "multiple",
                onGridReady: this.onGridReady.bind(this),
                onCellValueChanged: this.onCellValueChanged.bind(this),
                rowClassRules: {
                    'row-even': function (params: any) {
                        return params.node.rowIndex % 2 == 0;
                    },
                    'row-odd': function (params: any) {
                        return params.node.rowIndex % 2 != 0;
                    },
                },
            },
            dBData: []
        }
    }

    onCellHigh = (params: any) => {

        return (params.value === 'something' ? 'my-class-3' : 'my-class-3');
    }

    prepareCollDefs() {
        let varColDef: ColDef = {
            headerName: "test"
        };
        var objects = [{ 'a': 1 }, { 'b': 2 }];

        var deep = _.cloneObject(this.state);
        //console.log(deep[0] === objects[0]);
    }

    getTables = () => {
        let tablesURL = Constants.hostURL + "/" + Constants.controller.dynamicData + "/" + Constants.actions.getTables;
        debugger
        let tables = this.util.getDataFromDB(tablesURL);

    }
    componentDidMount() {
        this.getTables();
        axios.get(Constants.jsonSampleUrl).then(res => {
            console.log(res.data);
            let ss = res.data.map((s: any) => {
                s.Id = Math.random();
                return s;
            }) || [];

            this.setState({ dBData: res.data }, () => {
                let clnObj = _.cloneObject(this.state);
                let colArr: ColDef[] = [];
                res.data && res.data.length > 0 && Object.keys(res.data[0]).forEach(key => {
                    // colArr.push({
                    //     headerName: '',
                    //     field: ''
                    // });
                    colArr.push(new ColumnDefinition(key));
                });
                clnObj.gridOptions.columnDefs = colArr || [];


                clnObj.gridOptions.rowData = res.data;


                console.log(clnObj.gridOptions.rowData);
                this.setState({ clnObj }, () => {

                    this.setState({ dBData: this.state.dBData });
                });
            });
        });
    }



    public render() {
        return (
            <div className="gridLayoutContainer">
                <div className="row mb-1">
                    <div className="col-md-6 form-group">
                        <div className="input-group">
                            <div className="input-group-prepend">
                                <span className="input-group-text" >Tables</span>
                            </div>
                            <select className=" form-control col-md-6" placeholder="Select Table">
                                <option>1</option>
                                <option>2</option>
                                <option>3</option>
                                <option>4</option>
                            </select>
                        </div>
                    </div>
                    <div className="col-md-6 form-group search-txtbox-container ext-right">
                        <div className="input-group col-md-6 px-0">
                            <input type="text" onInput={this.onQuickFilterChanged} className="form-control" placeholder="Search..." aria-label="Recipient's username" aria-describedby="basic-addon2" />
                            <div className="input-group-append d-block">
                                <span className="input-group-text" >Search</span>
                            </div>
                        </div>
                    </div>
                </div>

                <div className="row mb-2">
                    <div className="col-md-6 px-0">
                        <div className="d-inline-block col-md-2">
                            <button className="btn btn-primary btn-action col"><FontAwesomeIcon icon={faPlus} /> New</button>
                        </div>
                        <div className="d-inline-block col-md-2">
                            <button className="btn btn-primary btn-action col"><FontAwesomeIcon icon={faArchive} /> Delete</button>
                        </div>
                    </div>
                    <div className="col-md-6 px-0 ext-right">
                        <div className="d-inline-block col-md-2">
                            <button className="btn btn-primary btn-action col"><FontAwesomeIcon icon={faSave} /> Save</button>
                        </div>
                    </div>
                </div>


                <div className="ag-theme-alpine gridLayoutInnerContainer">

                    <AgGridReact
                        columnDefs={this.state.gridOptions.columnDefs}
                        rowData={this.state.gridOptions.rowData}
                        gridOptions={this.state.gridOptions}
                    >
                    </AgGridReact>

                </div>
                {/* <FormModal></FormModal> */}
            </div>
        );
    }

    getRowStyle(params: any) {
        if (params.node.rowIndex % 2 === 0) {
            return 'my-shaded-effect';
        }
    }

    private test = () => {
        let table: any[] = this.state.gridOptions.rowData;
        let obj: any = {};
        let keys = Object.keys(this.state.gridOptions.rowData[0]);
        let mainObj = {};
        table.forEach(element => {
            let ss = [];
            keys.forEach(ele => {


                ss.push(Object.defineProperties({}, {
                    [ele]: {
                        value: 42
                    },
                    dataType: {
                        value: "string"
                    }
                }));


            });



        });

    }

    checkState = () => {

        let ss = this.state;
    }

    private onBtAdd = () => {
        let table = this.state.gridOptions.rowData;

        table.unshift({
            "athlete": "Enter New Item",
            "age": null,
            "date": null,
            "country": null,
            "year": null,
            "sport": null,
            "gold": null,
            "silve": 2,
            "asd": "Supper",
            "bronze": null,
            "Total": null
        });
        this.state.gridOptions.api.setRowData(table);
    };

    onCellValueChanged = (eve: any) => {

        let ss = this.state;
    }



    onGridReady(eve: any) {
        console.log('Sai Teja' + eve);
        this.state.gridOptions.api = eve.api;
        this.state.gridOptions.columnApi = eve.columnApi;
        //  this.state.gridOptions.api.selectAll();
        this.setState({ gridOptions: this.state.gridOptions });
    }


    onQuickFilterChanged = (eve: any) => {
        const { value } = eve.target;
        this.state.gridOptions.api.setQuickFilter(value);
    };

    isFirstColumn(params: any) {
        var displayedColumns = params.columnApi.getAllDisplayedColumns();
        var thisIsFirstColumn = displayedColumns[0] === params.column;
        return thisIsFirstColumn;
    }

    compareValues(params: any) {

        if (params.oldValue.toString() > params.newValue.toString()) {
            return { headerClass: "class-make" }
        }
        if (params.oldValue.toString() < params.newValue.toString()) {
            return { color: 'red', backgroundColor: 'black' };
        }
    }
};


