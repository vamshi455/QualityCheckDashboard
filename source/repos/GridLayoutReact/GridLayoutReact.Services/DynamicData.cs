﻿using GridLayoutReact.IServices;
using GridLayoutReact.Models.DB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GridLayoutReact.Models.MiddleWare;

namespace GridLayoutReact.Services
{
    public class DynamicData : IDynamicData
    {
        private string ConnectionString = @"Data Source=(LocalDb)\LocalDBDemo;Initial Catalog=DonationDB;Integrated Security=True";


        public List<Table> GetAllDBTables()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'", con);
                    con.Open();
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    List<Table> tableList = new List<Table>();
                    while (dataReader.Read())
                    {
                        tableList.Add(new Table()
                        {
                            Name = dataReader["TABLE_NAME"] != null ? dataReader["TABLE_NAME"].ToString() : ""
                        });
                    }
                    return tableList;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<TableSchema> GetTableSchema(string tableName)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = @"SELECT T.Name AS TableName,
                                        C.Name AS ColumnName,
                                        Ty.Name AS ColumnDataType,
                                        C.is_nullable AS IsNullAble,
                                        C.is_identity AS IsIdentity ,
										case when Ty.Name = 'nvarchar'
                                        then C.max_length / 2
                                        else
                                        C.max_length end AS MaximumLength
                                        FROM sys.tables T
                                        INNER JOIN sys.columns C
                                        ON T.OBJECT_ID = C.OBJECT_ID
                                        INNER JOIN sys.types Ty
                                        ON C.system_type_id = Ty.system_type_id
                                        WHERE T.is_ms_shipped = 0 AND T.Name = '" + tableName + "' AND Ty.Name != 'sysname' ORDER BY T.name";
                    con.Open();
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    List<TableSchema> tableSchemaList = new List<TableSchema>();
                    while (dataReader.Read())
                    {
                        tableSchemaList.Add(new TableSchema
                        {
                            IsNull = dataReader["IsNullAble"] != DBNull.Value && dataReader["IsNullAble"].ToString() == "1" ? true : false,
                            IsIdentity = dataReader["IsIdentity"] != DBNull.Value && dataReader["IsIdentity"].ToString() == "1" ? true : false,
                            DataType = dataReader["ColumnDataType"] != DBNull.Value ? dataReader["ColumnDataType"].ToString() : string.Empty,
                            MaximumLength = dataReader["MaximumLength"] != DBNull.Value ? Convert.ToInt32(dataReader["MaximumLength"]) : -1,
                            ColumnName = dataReader["ColumnName"] != DBNull.Value ? dataReader["ColumnName"].ToString() : string.Empty,
                            TableName = dataReader["TableName"] != DBNull.Value ? dataReader["TableName"].ToString() : string.Empty
                        }
                        );
                    };
                    return tableSchemaList;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public Response InsertItemInDB(NewRow rowObj)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand();
                    con.Open();
                    cmd.Connection = con;
                    Dictionary<string, string> result = JsonConvert.DeserializeObject<Dictionary<string, string>>(rowObj.JSONData.ToString());
                    string dict_keys = string.Join(", ", result.Select(p => p.Key));
                    var dict_vals = string.Format("'{0}'", string.Join("','", result.Select(i => i.Value.Replace("'", "''"))));
                    cmd.CommandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", rowObj.TableName, dict_keys, dict_vals);
                    if (cmd.ExecuteNonQuery() == 1)
                        return new Response() { IsResponseSuccess = true, Message = "SUCCESS" };
                    else
                        return new Response() { IsResponseSuccess = false, Message = "FAILED" };
                }
            }
            catch (Exception ex)
            {
                return new Response() { IsResponseSuccess = false, Message = ex.Message.ToString() };

            }
        }




        public Response DeleteItemFromDB(DeleteRow delRowObj)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand();
                    con.Open();
                    cmd.Connection = con;
                    string ids = string.Join(", ", delRowObj.Id.Select(id => id));
                    cmd.CommandText = string.Format("DELETE FROM {0} WHERE {1} IN ({2})", delRowObj.TableName, delRowObj.IdentityColumnName, ids);
                    if (cmd.ExecuteNonQuery() == 1)
                        return new Response() { IsResponseSuccess = true, Message = "SUCCESS" };
                    else
                        return new Response() { IsResponseSuccess = false, Message = "FAILED" };
                }
            }
            catch (Exception ex)
            {
                return new Response() { IsResponseSuccess = false, Message = ex.Message.ToString() };

            }
        }

        public dynamic GetTableData(string tableName)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {

                    SqlCommand cmd = new SqlCommand();
                    con.Open();
                    cmd.Connection = con;
                    tableName = "summary_prod_map";
                    cmd.CommandText = string.Format("SELECT * FROM {0}", tableName);
                    var data=cmd.ExecuteReader();

                    DataTable dataTable = new DataTable();
                    dataTable.Load(data);

                    return dataTable;


                }
            }
            catch (Exception ex)
            {
                return new Response() { IsResponseSuccess = false, Message = ex.Message.ToString() };

            }
        }

        public Response UpdateItemInDB(EditRow editRowObj)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand();
                    con.Open();
                    cmd.Connection = con;
                    var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(editRowObj.JSONData.ToString());
                    string keyValuesQueryTxt = string.Empty;
                    int index = 0;
                    foreach (var item in result)
                    {
                        keyValuesQueryTxt += string.Concat(item.Key, " = '", item.Value, "'", (index == result.Count - 1) ? "" : ",");
                        index++;
                    }

                    cmd.CommandText = string.Format("UPDATE {0} SET {1} WHERE {2}={3}", editRowObj.TableName, keyValuesQueryTxt, editRowObj.IdentityColumnName, editRowObj.Id);
                    if (cmd.ExecuteNonQuery() == 1)
                        return new Response() { IsResponseSuccess = true, Message = "SUCCESS" };
                    else
                        return new Response() { IsResponseSuccess = false, Message = "FAILED" };
                }
            }
            catch (Exception ex)
            {
                return new Response() { IsResponseSuccess = false, Message = ex.Message.ToString() };

            }

        }






    }
}
