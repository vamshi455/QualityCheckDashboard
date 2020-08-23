﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GridLayoutReact.IServices;
using GridLayoutReact.Models.DB;
using GridLayoutReact.Models.MiddleWare;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GridLayoutReact.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DynamicDataController : ControllerBase
    {


        private IDynamicData _dynamicData;

        public DynamicDataController(IDynamicData dynamicData)
        {
            _dynamicData = dynamicData;
        }

        [HttpGet]
        public List<Table> GetTables()
        {
            return _dynamicData.GetAllDBTables();
        }

        [HttpGet]
        public dynamic GetTableData(string tableName)
        {
            return _dynamicData.GetTableData(tableName);
        }



        [HttpGet]
        public List<TableSchema> GetTableSchema(string tableName)
        {
            return _dynamicData.GetTableSchema(tableName);
        }

        [HttpPost]
        public Response InsertItemInDB(NewRow newRowObj)
        {
            return _dynamicData.InsertItemInDB(newRowObj);
        }

        [HttpPut]
        public Response UpdateItemInDB(EditRow editRowObj)
        {
            return _dynamicData.UpdateItemInDB(editRowObj);
        }

        [HttpDelete]
        public Response DeleteItemFromDB(DeleteRow delRowObj)
        {
            return _dynamicData.DeleteItemFromDB(delRowObj);
        }

        public void GroupByTable(dynamic dataTable)
        {
            var s = 0;
            string output = JsonConvert.SerializeObject(dataTable);
            dynamic deserializedProduct = JsonConvert.DeserializeObject<dynamic>(output);
        }

    }
}
