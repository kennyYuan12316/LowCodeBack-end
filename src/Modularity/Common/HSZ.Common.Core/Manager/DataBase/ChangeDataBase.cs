using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Data.SqlSugar.Extensions;
using HSZ.Dependency;
using HSZ.FriendlyException;
using HSZ.JsonSerialization;
using HSZ.System.Entitys.Dto.System.Database;
using HSZ.System.Entitys.Model.System.DataBase;
using HSZ.System.Entitys.System;
using HSZ.VisualDev.Entitys.Dto.VisualDevModelData;
using HSZ.VisualDev.Entitys.Model.VisualDevModelData;
using Mapster;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSZ.ChangeDataBase
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：实现切换数据库后操作
    /// </summary>
    public class ChangeDataBase : IChangeDataBase, IScoped
    {
        /// <summary>
        /// SqlSugarScope操作数据库是线程安的可以单例
        /// </summary>
        public static SqlSugarScope Db = new SqlSugarScope(new ConnectionConfig()
        {
            ConfigId = App.Configuration["ConnectionStrings:ConfigId"],
            DbType = (SqlSugar.DbType)Enum.Parse(typeof(SqlSugar.DbType), App.Configuration["ConnectionStrings:DBType"]),
            ConnectionString = string.Format($"{App.Configuration["ConnectionStrings:DefaultConnection"]}", App.Configuration["ConnectionStrings:DBName"]),
            IsAutoCloseConnection = true,
            ConfigureExternalServices = new ConfigureExternalServicesExtenisons()
            {
                EntityNameServiceType = typeof(SugarTable)//这个不管是不是自定义都要写，主要是用来获取所有实体
            }
        }, db =>
        {
            //如果用单例配置要统一写在这儿
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                if (sql.StartsWith("SELECT"))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                if (sql.StartsWith("UPDATE") || sql.StartsWith("INSERT"))
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                if (sql.StartsWith("DELETE"))
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                //在控制台输出sql语句
                Console.WriteLine(SqlProfiler.ParameterFormat(sql, pars));
                //App.PrintToMiniProfiler("SqlSugar", "Info", SqlProfiler.ParameterFormat(sql, pars));
            };
        });

        /// <summary>
        /// 执行Sql(查询)
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="strSql">sql语句</param>
        /// <returns></returns>
        public async Task<int> ExecuteSql(DbLinkEntity link, string strSql)
        {
            if (link != null)
            {
                Db.AddConnection(new ConnectionConfig()
                {
                    ConfigId = link.Id,
                    DbType = ToDbType(link.DbType),
                    ConnectionString = ToConnectionString(link),
                    InitKeyType = InitKeyType.Attribute,
                    IsAutoCloseConnection = true
                });

                Db.ChangeDatabase(link.Id);
            }

            try
            {
                Db.BeginTran();
                var flag = 0;
                if (Db.CurrentConnectionConfig.DbType == SqlSugar.DbType.Oracle)
                {
                    await Db.Ado.ExecuteCommandAsync(strSql.TrimEnd(';'));
                }
                else
                {
                    flag = await Db.Ado.ExecuteCommandAsync(strSql);
                }
                Db.CommitTran();
                return flag;
            }
            catch (Exception)
            {
                Db.RollbackTran();
                throw;
            }
        }

        /// <summary>
        /// 执行Sql(查询)
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="strSql">sql语句</param>
        /// <returns></returns>
        public bool WhereDynamicFilter(DbLinkEntity link, string strSql)
        {
            if (link != null)
            {
                Db.AddConnection(new ConnectionConfig()
                {
                    ConfigId = link.Id,
                    DbType = ToDbType(link.DbType),
                    ConnectionString = ToConnectionString(link),
                    InitKeyType = InitKeyType.Attribute,
                    IsAutoCloseConnection = true
                });

                Db.ChangeDatabase(link.Id);
            }

            var flag = Db.Ado.SqlQuery<dynamic>(strSql).Count > 0;
            return flag;
        }

        /// <summary>
        /// 执行Sql(新增、修改)
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="table">表名</param>
        /// <param name="dicList">数据</param>
        /// <param name="primaryField">主键字段</param>
        /// <returns></returns>
        public async Task<int> ExecuteSql(DbLinkEntity link, string table, List<Dictionary<string, object>> dicList, string primaryField = "")
        {
            if (link != null)
            {
                Db.AddConnection(new ConnectionConfig()
                {
                    ConfigId = link.Id,
                    DbType = ToDbType(link.DbType),
                    ConnectionString = ToConnectionString(link),
                    InitKeyType = InitKeyType.Attribute,
                    IsAutoCloseConnection = true
                });

                Db.ChangeDatabase(link.Id);
            }

            try
            {
                Db.BeginTran();

                var flag = 0;
                if (primaryField == "")
                {
                    flag = await Db.Insertable(dicList).AS(table).ExecuteCommandAsync();
                }
                else
                {
                    flag = await Db.Updateable(dicList).AS(table).WhereColumns(primaryField).ExecuteCommandAsync();
                }
                Db.CommitTran();
                return flag;
            }
            catch (Exception)
            {
                Db.RollbackTran();
                throw;
            }
        }

        /// <summary>
        /// 根据链接获取分页数据
        /// </summary>
        /// <returns></returns>
        public PageResult<Dictionary<string, object>> GetInterFaceData(DbLinkEntity link, string strSql, VisualDevModelListQueryInput pageInput, ColumnDesignModel columnDesign, List<IConditionalModel> dataPermissions, Dictionary<string, string> outColumnName = null)
        {
            if (link != null)
            {
                Db.AddConnection(new ConnectionConfig()
                {
                    ConfigId = link.Id,
                    DbType = ToDbType(link.DbType),
                    ConnectionString = ToConnectionString(link),
                    InitKeyType = InitKeyType.Attribute,
                    IsAutoCloseConnection = true
                });

                Db.ChangeDatabase(link.Id);
            }

            try
            {
                int total = 0;
                //将查询的关键字json转成Dictionary
                Dictionary<string, object> keywordJsonDic = string.IsNullOrEmpty(pageInput.json) ? null : pageInput.json.Deserialize<Dictionary<string, object>>();
                var conModels = new List<IConditionalModel>();
                if (keywordJsonDic != null)
                {
                    foreach (KeyValuePair<string, object> item in keywordJsonDic)
                    {
                        var model = columnDesign.searchList.Find(it => it.__vModel__.Equals(item.Key));
                        var type = model.__config__.hszKey;
                        switch (type)
                        {
                            case "date":
                                {
                                    var timeRange = item.Value.ToObject<List<string>>();
                                    var startTime = Ext.GetDateTime(timeRange.First());
                                    var endTime = Ext.GetDateTime(timeRange.Last());
                                    if (model.format == "yyyy-MM-dd HH:mm:ss")
                                    {
                                        conModels.Add(new ConditionalModel
                                        {
                                            FieldName = item.Key,
                                            ConditionalType = ConditionalType.GreaterThanOrEqual,
                                            FieldValue = startTime.ToDateTimeString(),
                                            FieldValueConvertFunc = it => Convert.ToDateTime(it)
                                        });
                                        conModels.Add(new ConditionalModel
                                        {
                                            FieldName = item.Key,
                                            ConditionalType = ConditionalType.LessThanOrEqual,
                                            FieldValue = endTime.ToDateTimeString(),
                                            FieldValueConvertFunc = it => Convert.ToDateTime(it)
                                        });
                                    }
                                    else
                                    {
                                        conModels.Add(new ConditionalModel
                                        {
                                            FieldName = item.Key,
                                            ConditionalType = ConditionalType.GreaterThanOrEqual,
                                            FieldValue = new DateTime(startTime.ToDate().Year, startTime.ToDate().Month, startTime.ToDate().Day, 0, 0, 0, 0).ToString(),
                                            FieldValueConvertFunc = it => Convert.ToDateTime(it)
                                        });
                                        conModels.Add(new ConditionalModel
                                        {
                                            FieldName = item.Key,
                                            ConditionalType = ConditionalType.LessThanOrEqual,
                                            FieldValue = new DateTime(endTime.ToDate().Year, endTime.ToDate().Month, endTime.ToDate().Day, 23, 59, 59, 999).ToString(),
                                            FieldValueConvertFunc = it => Convert.ToDateTime(it)
                                        });
                                    }
                                }
                                break;
                            case "time":
                                {
                                    var timeRange = item.Value.ToString().Deserialize<List<string>>();
                                    var startTime = Ext.GetDateTime(timeRange.First());
                                    var endTime = Ext.GetDateTime(timeRange.Last());
                                    conModels.Add(new ConditionalModel
                                    {
                                        FieldName = item.Key,
                                        ConditionalType = ConditionalType.GreaterThanOrEqual,
                                        FieldValue = new DateTime(startTime.ToDate().Year, startTime.ToDate().Month, startTime.ToDate().Day, startTime.ToDate().Hour, startTime.ToDate().Minute, startTime.ToDate().Second, 0).ToString(),
                                        FieldValueConvertFunc = it => Convert.ToDateTime(it)
                                    });
                                    conModels.Add(new ConditionalModel
                                    {
                                        FieldName = item.Key,
                                        ConditionalType = ConditionalType.LessThanOrEqual,
                                        FieldValue = new DateTime(endTime.ToDate().Year, endTime.ToDate().Month, endTime.ToDate().Day, endTime.ToDate().Hour, endTime.ToDate().Minute, endTime.ToDate().Second, 0).ToString(),
                                        FieldValueConvertFunc = it => Convert.ToDateTime(it)
                                    });
                                }
                                break;
                            case "createTime":
                                {
                                    var timeRange = item.Value.ToObject<List<string>>();
                                    var startTime = Ext.GetDateTime(timeRange.First());
                                    var endTime = Ext.GetDateTime(timeRange.Last());
                                    conModels.Add(new ConditionalModel
                                    {
                                        FieldName = item.Key,
                                        ConditionalType = ConditionalType.GreaterThanOrEqual,
                                        FieldValue = new DateTime(startTime.ToDate().Year, startTime.ToDate().Month, startTime.ToDate().Day, 0, 0, 0, 0).ToString(),
                                        FieldValueConvertFunc = it => Convert.ToDateTime(it)
                                    });
                                    conModels.Add(new ConditionalModel
                                    {
                                        FieldName = item.Key,
                                        ConditionalType = ConditionalType.LessThanOrEqual,
                                        FieldValue = new DateTime(endTime.ToDate().Year, endTime.ToDate().Month, endTime.ToDate().Day, 23, 59, 59, 999).ToString(),
                                        FieldValueConvertFunc = it => Convert.ToDateTime(it)
                                    });
                                }
                                break;
                            case "modifyTime":
                                {
                                    var timeRange = item.Value.ToObject<List<string>>();
                                    var startTime = Ext.GetDateTime(timeRange.First());
                                    var endTime = Ext.GetDateTime(timeRange.Last());
                                    conModels.Add(new ConditionalModel
                                    {
                                        FieldName = item.Key,
                                        ConditionalType = ConditionalType.GreaterThanOrEqual,
                                        FieldValue = new DateTime(startTime.ToDate().Year, startTime.ToDate().Month, startTime.ToDate().Day, 0, 0, 0, 0).ToString(),
                                        FieldValueConvertFunc = it => Convert.ToDateTime(it)
                                    });
                                    conModels.Add(new ConditionalModel
                                    {
                                        FieldName = item.Key,
                                        ConditionalType = ConditionalType.LessThanOrEqual,
                                        FieldValue = new DateTime(endTime.ToDate().Year, endTime.ToDate().Month, endTime.ToDate().Day, 23, 59, 59, 999).ToString(),
                                        FieldValueConvertFunc = it => Convert.ToDateTime(it)
                                    });
                                }
                                break;
                            case "numInput":
                                {
                                    List<string> numArray = item.Value.ToObject<List<string>>();
                                    var startNum = numArray.First().ToInt();
                                    var endNum = numArray.Last() == null ? Int64.MaxValue : numArray.Last().ToInt();
                                    conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = startNum.ToString() });
                                    conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = endNum.ToString() });
                                }
                                break;
                            case "calculate":
                                {
                                    List<string> numArray = item.Value.ToObject<List<string>>();
                                    var startNum = numArray.First().ToInt();
                                    var endNum = numArray.Last() == null ? Int64.MaxValue : numArray.Last().ToInt();
                                    conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = startNum.ToString() });
                                    conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = endNum.ToString() });
                                }
                                break;
                            case "checkbox":
                                {
                                    conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.Like, FieldValue = item.Value.ToString() });
                                }
                                break;
                            case "posSelect":
                                {
                                    if (model.multiple)
                                    {
                                        conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.Like, FieldValue = item.Value.ToString() });
                                    }
                                    else
                                    {
                                        conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.Equal, FieldValue = item.Value.ToString().Replace("\r\n ", "").Replace("\r\n", "").Replace(" ", "") });
                                    }
                                }
                                break;
                            case "userSelect":
                                {
                                    if (model.multiple)
                                    {
                                        conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.Like, FieldValue = item.Value.ToString() });
                                    }
                                    else
                                    {
                                        conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.Equal, FieldValue = item.Value.ToString().Replace("\r\n ", "").Replace("\r\n", "").Replace(" ", "") });
                                    }
                                }
                                break;
                            case "depSelect":
                                {
                                    if (model.multiple)
                                    {
                                        conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.Like, FieldValue = item.Value.ToString() });
                                    }
                                    else
                                    {
                                        conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.Equal, FieldValue = item.Value.ToString().Replace("\r\n ", "").Replace("\r\n", "").Replace(" ", "") });
                                    }
                                }
                                break;
                            case "cascader":
                                {
                                    conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.Like, FieldValue = item.Value.ToString().Replace("\r\n ", "").Replace("\r\n", "").Replace(" ", "") });
                                }
                                break;
                            case "treeSelect":
                                {
                                    if (item.Value.IsNotEmptyOrNull() && item.Value.ToString().Contains("["))
                                    {
                                        var value = item.Value?.ToString().Deserialize<List<string>>();
                                        if (value.Any()) conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.Like, FieldValue = value.LastOrDefault() });
                                    }
                                    else conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.Like, FieldValue = item.Value.ToString() });
                                }
                                break;
                            case "address":
                                {
                                    //多选时为模糊查询
                                    if (model.multiple)
                                    {
                                        var value = item.Value?.ToString().Deserialize<List<string>>();
                                        if (value.Any()) conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.Like, FieldValue = value.LastOrDefault() });
                                    }
                                    else
                                    {
                                        conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.Equal, FieldValue = item.Value.ToString().Replace("\r\n ", "").Replace("\r\n", "").Replace(" ", "") });
                                    }
                                }
                                break;
                            case "currOrganize":
                                {
                                    var value = item.Value?.ToString().Deserialize<List<string>>();
                                    if (value.Any()) conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.InLike, FieldValue = value.LastOrDefault() });
                                }
                                break;
                            case "comSelect":
                                {
                                    var value = item.Value?.ToString().Deserialize<List<string>>();
                                    if (value.Any()) conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.InLike, FieldValue = value.LastOrDefault() });
                                }
                                break;
                            case "select":
                                {
                                    //多选时为模糊查询
                                    if (model.multiple)
                                    {
                                        conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.Like, FieldValue = item.Value.ToString() });
                                    }
                                    else
                                    {
                                        conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.Equal, FieldValue = item.Value.ToString().Replace("\r\n ", "").Replace("\r\n", "").Replace(" ", "") });
                                    }
                                }
                                break;
                            default:
                                {
                                    if (model.searchType == 2)
                                    {
                                        conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.Like, FieldValue = item.Value.ToString() });
                                    }
                                    else if (model.searchType == 1)
                                    {
                                        conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.Equal, FieldValue = item.Value.ToString().Replace("\r\n ", "").Replace("\r\n", "").Replace(" ", "") });
                                    }
                                }
                                break;
                        }
                    }
                }

                if (Db.CurrentConnectionConfig.DbType == SqlSugar.DbType.Oracle)
                {
                    strSql = strSql.Replace(";", "");
                }

                var sidx = pageInput.sidx.IsNotEmptyOrNull() && pageInput.sort.IsNotEmptyOrNull();//按前端参数排序
                var defaultSidx = columnDesign.defaultSidx.IsNotEmptyOrNull() && columnDesign.sort.IsNotEmptyOrNull();//按模板默认排序

                var dt = Db.SqlQueryable<object>(strSql).Where(conModels).Where(dataPermissions)
                    .OrderByIF(sidx, pageInput.sidx + " " + pageInput.sort)
                    .OrderByIF(!sidx && defaultSidx, columnDesign.defaultSidx + " " + columnDesign.sort)
                    .ToDataTablePage(pageInput.currentPage, pageInput.pageSize, ref total);

                //如果有字段别名 替换 ColumnName
                if (outColumnName != null && outColumnName.Count > 0)
                {
                    var resultKey = string.Empty;
                    for (var i = 0; i < dt.Columns.Count; i++)
                        dt.Columns[i].ColumnName = outColumnName.TryGetValue(dt.Columns[i].ColumnName.ToUpper(), out resultKey) == true ? outColumnName[dt.Columns[i].ColumnName.ToUpper()] : dt.Columns[i].ColumnName.ToUpper();
                }

                return new PageResult<Dictionary<string, object>>()
                {
                    pagination = new PageResult()
                    {
                        pageIndex = pageInput.currentPage,
                        pageSize = pageInput.pageSize,
                        total = total
                    },
                    list = dt.ToObject<List<Dictionary<string, object>>>()
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        /// <summary>
        /// 删除表
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="table">表名</param>
        /// <returns></returns>
        public bool Delete(DbLinkEntity link, string table)
        {
            if (link != null)
            {
                Db.AddConnection(new ConnectionConfig()
                {
                    ConfigId = link.Id,
                    DbType = ToDbType(link.DbType),
                    ConnectionString = ToConnectionString(link),
                    InitKeyType = InitKeyType.Attribute,
                    IsAutoCloseConnection = true
                });

                Db.ChangeDatabase(link.Id);
            }

            try
            {
                Db.BeginTran();

                Db.DbMaintenance.DropTable(table);

                Db.CommitTran();
                return true;
            }
            catch (Exception)
            {
                Db.RollbackTran();
                throw;
            }
        }

        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="tableModel">表对象</param>
        /// <param name="tableFieldList">字段对象</param>
        /// <returns></returns>
        public async Task<bool> Create(DbLinkEntity link, DbTableModel tableModel, List<DbTableFieldModel> tableFieldList)
        {
            if (link != null)
            {
                Db.AddConnection(new ConnectionConfig()
                {
                    ConfigId = link.Id,
                    DbType = ToDbType(link.DbType),
                    ConnectionString = ToConnectionString(link),
                    InitKeyType = InitKeyType.Attribute,
                    IsAutoCloseConnection = true
                });

                Db.ChangeDatabase(link.Id);
            }

            if (Db.CurrentConnectionConfig.DbType == SqlSugar.DbType.MySql)
            {
                await CreateTableMySql(tableModel, tableFieldList);
            }
            else
            {
                CreateTable(tableModel, tableFieldList);
            }

            return true;
        }

        /// <summary>
        /// 修改表
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="oldTable">原数据</param>
        /// <param name="tableModel">表对象</param>
        /// <param name="tableFieldList">字段对象</param>
        /// <returns></returns>
        public async Task<bool> Update(DbLinkEntity link, string oldTable, DbTableModel tableModel, List<DbTableFieldModel> tableFieldList)
        {
            if (link != null)
            {
                Db.AddConnection(new ConnectionConfig()
                {
                    ConfigId = link.Id,
                    DbType = ToDbType(link.DbType),
                    ConnectionString = ToConnectionString(link),
                    InitKeyType = InitKeyType.Attribute,
                    IsAutoCloseConnection = true
                });

                Db.ChangeDatabase(link.Id);
            }

            try
            {
                Db.BeginTran();


                Db.DbMaintenance.DropTable(oldTable);

                if (Db.CurrentConnectionConfig.DbType == SqlSugar.DbType.MySql)
                {
                    await CreateTableMySql(tableModel, tableFieldList);
                }
                else
                {
                    CreateTable(tableModel, tableFieldList);
                }

                Db.CommitTran();

                return true;
            }
            catch (Exception)
            {
                Db.RollbackTran();
                throw;
            }
        }

        /// <summary>
        /// 表是否存在
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="table">表名</param>
        /// <returns></returns>
        public bool IsAnyTable(DbLinkEntity link, string table)
        {
            if (link != null)
            {
                Db.AddConnection(new ConnectionConfig()
                {
                    ConfigId = link.Id,
                    DbType = ToDbType(link.DbType),
                    ConnectionString = ToConnectionString(link),
                    InitKeyType = InitKeyType.Attribute,
                    IsAutoCloseConnection = true
                });

                Db.ChangeDatabase(link.Id);
            }

            return Db.DbMaintenance.IsAnyTable(table, false);
        }

        /// <summary>
        /// 获取表字段列表
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>TableFieldListModel
        public List<DbTableFieldModel> GetFieldList(DbLinkEntity link, string tableName)
        {
            if (link != null)
            {
                Db.AddConnection(new ConnectionConfig()
                {
                    ConfigId = link.Id,
                    DbType = ToDbType(link.DbType),
                    ConnectionString = ToConnectionString(link),
                    InitKeyType = InitKeyType.Attribute,
                    IsAutoCloseConnection = true
                });

                Db.ChangeDatabase(link.Id);
            }

            var list = Db.DbMaintenance.GetColumnInfosByTableName(tableName, false);
            return list.Adapt<List<DbTableFieldModel>>();
        }

        /// <summary>
        /// 获取表数据
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public DataTable GetData(DbLinkEntity link, string tableName)
        {
            if (link != null)
            {
                Db.AddConnection(new ConnectionConfig()
                {
                    ConfigId = link.Id,
                    DbType = ToDbType(link.DbType),
                    ConnectionString = ToConnectionString(link),
                    InitKeyType = InitKeyType.Attribute,
                    IsAutoCloseConnection = true
                });

                Db.ChangeDatabase(link.Id);
            }

            var data = Db.Queryable<dynamic>().AS(tableName).ToDataTable();
            return data;
        }

        /// <summary>
        /// 根据链接获取数据
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="strSql">sqlyuju </param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public DataTable GetInterFaceData(DbLinkEntity link, string strSql, params SugarParameter[] parameters)
        {
            if (link != null)
            {
                Db.AddConnection(new ConnectionConfig()
                {
                    ConfigId = link.Id,
                    DbType = ToDbType(link.DbType),
                    ConnectionString = ToConnectionString(link),
                    InitKeyType = InitKeyType.Attribute,
                    IsAutoCloseConnection = true
                });

                Db.ChangeDatabase(link.Id);
            }

            if (Db.CurrentConnectionConfig.DbType == SqlSugar.DbType.Oracle)
            {
                strSql = strSql.Replace(";", "");
            }

            var dt = Db.Ado.GetDataTable(strSql, parameters);
            return dt;
        }

        /// <summary>
        /// 执行增删改sql
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="strSql">sqlyuju </param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public void ExecuteCommand(DbLinkEntity link, string strSql, params SugarParameter[] parameters)
        {
            if (link != null)
            {
                Db.AddConnection(new ConnectionConfig()
                {
                    ConfigId = link.Id,
                    DbType = ToDbType(link.DbType),
                    ConnectionString = ToConnectionString(link),
                    InitKeyType = InitKeyType.Attribute,
                    IsAutoCloseConnection = true
                });

                Db.ChangeDatabase(link.Id);
            }

            if (Db.CurrentConnectionConfig.DbType == SqlSugar.DbType.Oracle)
            {
                strSql = strSql.Replace(";", "");
            }

            Db.Ado.ExecuteCommand(strSql, parameters);
        }

        /// <summary>
        /// 获取表信息
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public DatabaseTableInfoOutput GetDataBaseTableInfo(DbLinkEntity link, string tableName)
        {
            if (link != null)
            {
                Db.AddConnection(new ConnectionConfig()
                {
                    ConfigId = link.Id,
                    DbType = ToDbType(link.DbType),
                    ConnectionString = ToConnectionString(link),
                    InitKeyType = InitKeyType.Attribute,
                    IsAutoCloseConnection = true
                });

                Db.ChangeDatabase(link.Id);
            }

            var data = new DatabaseTableInfoOutput()
            {
                tableInfo = Db.DbMaintenance.GetTableInfoList(false).Find(m => m.Name == tableName).Adapt<TableInfo>(),
                tableFieldList = Db.DbMaintenance.GetColumnInfosByTableName(tableName, false).Adapt<List<TableFieldList>>()
            };

            data.tableFieldList = ViewDataTypeConversion(data.tableFieldList, Db.CurrentConnectionConfig.DbType);

            return data;
        }

        /// <summary>
        /// 获取数据库表信息
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <returns></returns>
        public List<DbTableModel> GetDBTableList(DbLinkEntity link)
        {
            if (link != null)
            {
                Db.AddConnection(new ConnectionConfig()
                {
                    ConfigId = link.Id,
                    DbType = ToDbType(link.DbType),
                    ConnectionString = ToConnectionString(link),
                    InitKeyType = InitKeyType.Attribute,
                    IsAutoCloseConnection = true
                });

                Db.ChangeDatabase(link.Id);
            }

            var dbType = link == null ? "SQLServer" : link.DbType;
            var sql = DBTableSql(dbType);
            var modelList = Db.Ado.SqlQuery<DynamicDbTableModel>(sql);
            var list = modelList.Adapt<List<DbTableModel>>();
            return list;
        }

        /// <summary>
        /// 获取数据库表信息
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <returns></returns>
        public List<DbTableInfo> GetTableInfos(DbLinkEntity link)
        {
            if (link != null)
            {
                Db.AddConnection(new ConnectionConfig()
                {
                    ConfigId = link.Id,
                    DbType = ToDbType(link.DbType),
                    ConnectionString = ToConnectionString(link),
                    InitKeyType = InitKeyType.Attribute,
                    IsAutoCloseConnection = true
                });

                Db.ChangeDatabase(link.Id);
            }

            return Db.DbMaintenance.GetTableInfoList(false);
        }

        /// <summary>
        /// 获取数据表分页(SQL语句)
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="dbSql">数据SQL</param>
        /// <param name="pageIndex">页数</param>
        /// <param name="pageSize">条数</param>
        /// <returns></returns>
        public async Task<dynamic> GetDataTablePage(DbLinkEntity link, string dbSql, int pageIndex, int pageSize)
        {
            if (link != null)
            {
                Db.AddConnection(new ConnectionConfig()
                {
                    ConfigId = link.Id,
                    DbType = ToDbType(link.DbType),
                    ConnectionString = ToConnectionString(link),
                    InitKeyType = InitKeyType.Attribute,
                    IsAutoCloseConnection = true
                });

                Db.ChangeDatabase(link.Id);
            }

            RefAsync<int> totalNumber = 0;
            var list = await Db.SqlQueryable<object>(dbSql).ToDataTablePageAsync(pageIndex, pageSize, totalNumber);
            var pageList = new SqlSugarPagedList<dynamic>()
            {
                list = ToDynamicList(list),
                pagination = new PagedModel()
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    Total = totalNumber
                }
            };

            return PageResult<dynamic>.SqlSugarPageResult(pageList);
        }

        /// <summary>
        /// 获取数据表分页(实体)
        /// </summary>
        /// <typeparam name="TEntity">T</typeparam>
        /// <param name="link">数据连接</param>
        /// <param name="pageIndex">页数</param>
        /// <param name="pageSize">条数</param>
        /// <returns></returns>
        public async Task<List<TEntity>> GetDataTablePage<TEntity>(DbLinkEntity link, int pageIndex, int pageSize)
        {
            if (link != null)
            {
                Db.AddConnection(new ConnectionConfig()
                {
                    ConfigId = link.Id,
                    DbType = ToDbType(link.DbType),
                    ConnectionString = ToConnectionString(link),
                    InitKeyType = InitKeyType.Attribute,
                    IsAutoCloseConnection = true
                });

                Db.ChangeDatabase(link.Id);
            }

            var data = await Db.Queryable<TEntity>().ToPageListAsync(pageIndex, pageSize);
            return data;
        }

        /// <summary>
        /// 同步数据
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="dt">同步数据</param>
        /// <param name="table">表</param>
        /// <returns></returns>
        public bool SyncData(DbLinkEntity link, DataTable dt, string table)
        {
            if (link != null)
            {
                Db.AddConnection(new ConnectionConfig()
                {
                    ConfigId = link.Id,
                    DbType = ToDbType(link.DbType),
                    ConnectionString = ToConnectionString(link),
                    InitKeyType = InitKeyType.Attribute,
                    IsAutoCloseConnection = true
                });

                Db.ChangeDatabase(link.Id);
            }

            var str = dt.Serialize();
            List<Dictionary<string, object>> dc = Db.Utilities.DataTableToDictionaryList(dt);//5.0.23版本支持
            var isOk = Db.Insertable(dc).AS(table).ExecuteCommand();
            return isOk > 0;
        }

        /// <summary>
        /// 同步表操作
        /// </summary>
        /// <param name="linkFrom">原数据库</param>
        /// <param name="linkTo">目前数据库</param>
        /// <param name="table">表名称</param>
        /// <param name="type"></param>
        public void SyncTable(DbLinkEntity linkFrom, DbLinkEntity linkTo, string table, int type)
        {
            try
            {
                if (type == 2)
                {
                    if (linkFrom != null)
                    {
                        Db.AddConnection(new ConnectionConfig()
                        {
                            ConfigId = linkFrom.Id,
                            DbType = ToDbType(linkFrom.DbType),
                            ConnectionString = ToConnectionString(linkFrom),
                            InitKeyType = InitKeyType.Attribute,
                            IsAutoCloseConnection = true
                        });

                        Db.ChangeDatabase(linkFrom.Id);
                    }
                    var columns = Db.DbMaintenance.GetColumnInfosByTableName(table, false);

                    if (linkTo != null)
                    {
                        Db.AddConnection(new ConnectionConfig()
                        {
                            ConfigId = linkTo.Id,
                            DbType = ToDbType(linkTo.DbType),
                            ConnectionString = ToConnectionString(linkTo),
                            InitKeyType = InitKeyType.Attribute,
                            IsAutoCloseConnection = true
                        });

                        Db.ChangeDatabase(linkTo.Id);
                    }

                    DelDataLength(columns);

                    Db.DbMaintenance.CreateTable(table, columns);
                }
                else if (type == 3)
                {
                    if (linkTo != null)
                    {
                        Db.AddConnection(new ConnectionConfig()
                        {
                            ConfigId = linkTo.Id,
                            DbType = ToDbType(linkTo.DbType),
                            ConnectionString = ToConnectionString(linkTo),
                            InitKeyType = InitKeyType.Attribute,
                            IsAutoCloseConnection = true
                        });

                        Db.ChangeDatabase(linkTo.Id);
                    }

                    Db.DbMaintenance.TruncateTable(table);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 使用存储过程
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="stored">存储过程名称</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public void UseStoredProcedure(DbLinkEntity link, string stored, List<SugarParameter> parameters)
        {
            if (link != null)
            {
                Db.AddConnection(new ConnectionConfig()
                {
                    ConfigId = link.Id,
                    DbType = ToDbType(link.DbType),
                    ConnectionString = ToConnectionString(link),
                    InitKeyType = InitKeyType.Attribute,
                    IsAutoCloseConnection = true
                });

                Db.ChangeDatabase(link.Id);
            }

            Db.Ado.UseStoredProcedure().GetDataTable(stored, parameters);
        }

        /// <summary>
        /// 测试数据库连接
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <returns></returns>
        public bool IsConnection(DbLinkEntity link)
        {
            try
            {
                if (link != null)
                {
                    Db.AddConnection(new ConnectionConfig()
                    {
                        ConfigId = link.Id,
                        DbType = ToDbType(link.DbType),
                        ConnectionString = ToConnectionString(link),
                        InitKeyType = InitKeyType.Attribute,
                        IsAutoCloseConnection = true
                    });

                    Db.ChangeDatabase(link.Id);
                }
                Db.Open();
                Db.Close();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="databaseType"></param>
        public List<TableFieldList> ViewDataTypeConversion(List<TableFieldList> fields, SqlSugar.DbType databaseType)
        {
            foreach (var item in fields)
            {
                item.dataType = item.dataType.ToLower();
                if (item.dataType.Equals("string"))
                {
                    item.dataType = "varchar";
                    if (item.dataLength.ToInt() > 2000)
                    {
                        item.dataType = "text";
                        item.dataLength = "50";
                    }
                }
                else if (item.dataType.Equals("single"))
                {
                    item.dataType = "decimal";
                }
            }
            return fields;
        }

        /// <summary>
        /// 转换数据库类型
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public SqlSugar.DbType ToDbType(string dbType)
        {
            switch (dbType.ToLower())
            {
                case "sqlserver":
                    return SqlSugar.DbType.SqlServer;
                case "mysql":
                    return SqlSugar.DbType.MySql;
                case "oracle":
                    return SqlSugar.DbType.Oracle;
                case "dm8":
                    return SqlSugar.DbType.Dm;
                case "dm":
                    return SqlSugar.DbType.Dm;
                case "kdbndp":
                    return SqlSugar.DbType.Kdbndp;
                case "kingbasees":
                    return SqlSugar.DbType.Kdbndp;
                case "postgresql":
                    return SqlSugar.DbType.PostgreSQL;
                default:
                    throw HSZException.Oh(ErrorCode.D1505);
            }
        }

        /// <summary>
        /// 转换连接字符串
        /// </summary>
        /// <param name="dbLinkEntity"></param>
        /// <returns></returns>
        public string ToConnectionString(DbLinkEntity dbLinkEntity)
        {
            switch (dbLinkEntity.DbType.ToLower())
            {
                case "sqlserver":
                    return string.Format("Data Source={0},{4};Initial Catalog={1};User ID={2};Password={3};MultipleActiveResultSets=true", dbLinkEntity.Host, dbLinkEntity.ServiceName, dbLinkEntity.UserName, dbLinkEntity.Password, dbLinkEntity.Port);
                case "oracle":
                    return string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVER = DEDICATED)(SERVICE_NAME={2})));User Id={3};Password={4}", dbLinkEntity.Host, dbLinkEntity.Port.ToString(), dbLinkEntity.TableSpace, dbLinkEntity.UserName, dbLinkEntity.Password);
                case "mysql":
                    return string.Format("server={0};port={1};database={2};user={3};password={4};AllowLoadLocalInfile=true", dbLinkEntity.Host, dbLinkEntity.Port.ToString(), dbLinkEntity.ServiceName, dbLinkEntity.UserName, dbLinkEntity.Password);
                case "dm8":
                    return string.Format("server={0};port={1};database={2};User Id={3};PWD={4}", dbLinkEntity.Host, dbLinkEntity.Port.ToString(), dbLinkEntity.ServiceName, dbLinkEntity.UserName, dbLinkEntity.Password);
                case "dm":
                    return string.Format("server={0};port={1};database={2};User Id={3};PWD={4}", dbLinkEntity.Host, dbLinkEntity.Port.ToString(), dbLinkEntity.ServiceName, dbLinkEntity.UserName, dbLinkEntity.Password);
                case "kdbndp":
                    return string.Format("server={0};port={1};database={2};UID={3};PWD={4}", dbLinkEntity.Host, dbLinkEntity.Port.ToString(), dbLinkEntity.ServiceName, dbLinkEntity.UserName, dbLinkEntity.Password);
                case "kingbasees":
                    return string.Format("server={0};port={1};database={2};UID={3};PWD={4}", dbLinkEntity.Host, dbLinkEntity.Port.ToString(), dbLinkEntity.ServiceName, dbLinkEntity.UserName, dbLinkEntity.Password);
                case "postgresql":
                    return string.Format("server={0};port={1};Database={2};User Id={3};Password={4}", dbLinkEntity.Host, dbLinkEntity.Port.ToString(), dbLinkEntity.ServiceName, dbLinkEntity.UserName, dbLinkEntity.Password);
                default:
                    throw HSZException.Oh(ErrorCode.D1505);
            }
        }

        /// <summary>
        /// DataTable转DicList
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private List<Dictionary<string, object>> DataTableToDicList(DataTable dt)
        {
            return dt.AsEnumerable().Select(
                    row => dt.Columns.Cast<DataColumn>().ToDictionary(
                    column => column.ColumnName.ToLower(),
                    column => row[column]
                    )).ToList();
        }

        /// <summary>
        /// 将DataTable 转换成 List<dynamic>
        /// reverse 反转：控制返回结果中是只存在 FilterField 指定的字段,还是排除.
        /// [flase 返回FilterField 指定的字段]|[true 返回结果剔除 FilterField 指定的字段]
        /// FilterField  字段过滤，FilterField 为空 忽略 reverse 参数；返回DataTable中的全部数
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <param name="reverse">
        /// 反转：控制返回结果中是只存在 FilterField 指定的字段,还是排除.
        /// [flase 返回FilterField 指定的字段]|[true 返回结果剔除 FilterField 指定的字段]
        ///</param>
        /// <param name="FilterField">字段过滤，FilterField 为空 忽略 reverse 参数；返回DataTable中的全部数据</param>
        /// <returns>List<dynamic></returns>
        public static List<dynamic> ToDynamicList(DataTable table, bool reverse = true, params string[] FilterField)
        {
            var modelList = new List<dynamic>();
            foreach (DataRow row in table.Rows)
            {
                dynamic model = new ExpandoObject();
                var dict = (IDictionary<string, object>)model;
                foreach (DataColumn column in table.Columns)
                {
                    if (FilterField.Length != 0)
                    {
                        if (reverse == true)
                        {
                            if (!FilterField.Contains(column.ColumnName))
                            {
                                dict[column.ColumnName] = row[column];
                            }
                        }
                        else
                        {
                            if (FilterField.Contains(column.ColumnName))
                            {
                                dict[column.ColumnName] = row[column];
                            }
                        }
                    }
                    else
                    {
                        dict[column.ColumnName.ToLower()] = row[column];
                    }
                }
                modelList.Add(model);
            }
            return modelList;
        }

        /// <summary>
        /// 数据库表SQL
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        private string DBTableSql(string dbType)
        {
            StringBuilder sb = new StringBuilder();
            switch (dbType.ToLower())
            {
                case "sqlserver":
                    sb.Append(@"DECLARE @TABLEINFO TABLE ( NAME VARCHAR(50) , SUMROWS VARCHAR(11) , RESERVED VARCHAR(50) , DATA VARCHAR(50) , INDEX_SIZE VARCHAR(50) , UNUSED VARCHAR(50) , PK VARCHAR(50) ) DECLARE @TABLENAME TABLE ( NAME VARCHAR(50) ) DECLARE @NAME VARCHAR(50) DECLARE @PK VARCHAR(50) INSERT INTO @TABLENAME ( NAME ) SELECT O.NAME FROM SYSOBJECTS O , SYSINDEXES I WHERE O.ID = I.ID AND O.XTYPE = 'U' AND I.INDID < 2 ORDER BY I.ROWS DESC , O.NAME WHILE EXISTS ( SELECT 1 FROM @TABLENAME ) BEGIN SELECT TOP 1 @NAME = NAME FROM @TABLENAME DELETE @TABLENAME WHERE NAME = @NAME DECLARE @OBJECTID INT SET @OBJECTID = OBJECT_ID(@NAME) SELECT @PK = COL_NAME(@OBJECTID, COLID) FROM SYSOBJECTS AS O INNER JOIN SYSINDEXES AS I ON I.NAME = O.NAME INNER JOIN SYSINDEXKEYS AS K ON K.INDID = I.INDID WHERE O.XTYPE = 'PK' AND PARENT_OBJ = @OBJECTID AND K.ID = @OBJECTID INSERT INTO @TABLEINFO ( NAME , SUMROWS , RESERVED , DATA , INDEX_SIZE , UNUSED ) EXEC SYS.SP_SPACEUSED @NAME UPDATE @TABLEINFO SET PK = @PK WHERE NAME = @NAME END SELECT F.NAME AS F_TABLE,ISNULL(P.TDESCRIPTION,F.NAME) AS F_TABLENAME, F.RESERVED AS F_SIZE, RTRIM(F.SUMROWS) AS F_SUM, F.PK AS F_PRIMARYKEY FROM @TABLEINFO F LEFT JOIN ( SELECT NAME = CASE WHEN A.COLORDER = 1 THEN D.NAME ELSE '' END , TDESCRIPTION = CASE WHEN A.COLORDER = 1 THEN ISNULL(F.VALUE, '') ELSE '' END FROM SYSCOLUMNS A LEFT JOIN SYSTYPES B ON A.XUSERTYPE = B.XUSERTYPE INNER JOIN SYSOBJECTS D ON A.ID = D.ID AND D.XTYPE = 'U' AND D.NAME <> 'DTPROPERTIES' LEFT JOIN SYS.EXTENDED_PROPERTIES F ON D.ID = F.MAJOR_ID WHERE A.COLORDER = 1 AND F.MINOR_ID = 0 ) P ON F.NAME = P.NAME WHERE 1 = 1 ORDER BY F_TABLE");
                    break;
                case "oracle":
                    sb.Append(@"SELECT DISTINCT COL.TABLE_NAME AS F_TABLE,TAB.COMMENTS AS F_TABLENAME,0 AS F_SIZE,NVL(T.NUM_ROWS,0)AS F_SUM,COLUMN_NAME AS F_PRIMARYKEY FROM USER_CONS_COLUMNS COL INNER JOIN USER_CONSTRAINTS CON ON CON.CONSTRAINT_NAME=COL.CONSTRAINT_NAME INNER JOIN USER_TAB_COMMENTS TAB ON TAB.TABLE_NAME=COL.TABLE_NAME INNER JOIN USER_TABLES T ON T.TABLE_NAME=COL.TABLE_NAME WHERE CON.CONSTRAINT_TYPE NOT IN('C','R')ORDER BY COL.TABLE_NAME");
                    break;
                case "mysql":
                    sb.Append(@"SELECT T1.*,(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.`COLUMNS`WHERE TABLE_SCHEMA=DATABASE()AND TABLE_NAME=T1.F_TABLE AND COLUMN_KEY='PRI')F_PRIMARYKEY FROM(SELECT TABLE_NAME F_TABLE,0 F_SIZE,TABLE_ROWS F_SUM,(SELECT IF(LENGTH(TRIM(TABLE_COMMENT))<1,TABLE_NAME,TABLE_COMMENT))F_TABLENAME FROM INFORMATION_SCHEMA.`TABLES`WHERE TABLE_SCHEMA=DATABASE())T1 ORDER BY T1.F_TABLE");
                    break;
                default:
                    throw new Exception("不支持");
            }
            return sb.ToString();
        }

        /// <summary>
        /// MySql创建表单+注释
        /// </summary>
        /// <param name="db">连接Db</param>
        /// <param name="tableModel">表</param>
        /// <param name="tableFieldList">字段</param>
        private async Task CreateTableMySql(DbTableModel tableModel, List<DbTableFieldModel> tableFieldList)
        {
            try
            {
                Db.BeginTran();

                StringBuilder strSql = new StringBuilder();
                strSql.Append("CREATE TABLE `" + tableModel.table + "` (\r\n");
                foreach (var item in tableFieldList)
                {
                    if (item.primaryKey == 1 && item.allowNull == 1)
                        throw HSZException.Oh(ErrorCode.D1509);
                    strSql.Append(" `" + item.field + "` " + item.dataType.ToUpper() + "");
                    if (item.dataType == "varchar" || item.dataType == "nvarchar" || item.dataType == "decimal")
                        strSql.Append(" (" + item.dataLength + ") ");
                    if (item.primaryKey == 1)
                    {
                        strSql.Append(" primary key ");
                    }
                    if (item.allowNull == 0)
                        strSql.Append(" NOT NULL ");
                    else
                        strSql.Append(" NULL ");
                    strSql.Append("COMMENT '" + item.fieldName + "'");
                    strSql.Append(",");
                }
                strSql.Remove(strSql.Length - 1, 1);
                strSql.Append("\r\n");
                strSql.Append(") COMMENT = '" + tableModel.tableName + "';");
                await Db.Ado.ExecuteCommandAsync(strSql.ToString());

                Db.CommitTran();
            }
            catch (Exception ex)
            {
                Db.RollbackTran();
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// sqlsugar建表
        /// </summary>
        /// <param name="tableModel"></param>
        /// <param name="tableFieldList"></param>
        /// <returns></returns>
        private void CreateTable(DbTableModel tableModel, List<DbTableFieldModel> tableFieldList)
        {
            try
            {
                var cloumnList = tableFieldList.Adapt<List<DbColumnInfo>>();
                DelDataLength(cloumnList);
                var isOk = Db.DbMaintenance.CreateTable(tableModel.table, cloumnList);
                Db.DbMaintenance.AddTableRemark(tableModel.table, tableModel.tableName);
                foreach (var item in cloumnList)
                {
                    Db.DbMaintenance.AddColumnRemark(item.DbColumnName, tableModel.table, item.ColumnDescription);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// sqlsugar添加表字段
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="tableFieldList">表字段集合</param>
        /// <returns></returns>
        public void AddTableColumn(string tableName, List<DbTableFieldModel> tableFieldList)
        {
            try
            {
                var cloumnList = tableFieldList.Adapt<List<DbColumnInfo>>();
                DelDataLength(cloumnList);
                foreach (var item in cloumnList)
                {
                    Db.DbMaintenance.AddColumn(tableName, item);
                    Db.DbMaintenance.AddColumnRemark(item.DbColumnName, tableName, item.ColumnDescription);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 删除列长度（SqlSugar除了字符串其他不需要类型长度）
        /// </summary>
        /// <param name="dbColumnInfos"></param>
        private void DelDataLength(List<DbColumnInfo> dbColumnInfos)
        {
            foreach (var item in dbColumnInfos)
            {
                if (item.DataType != "varchar")
                {
                    item.Length = 0;
                }
                item.DataType = DataTypeConversion(item.DataType, Db.CurrentConnectionConfig.DbType);
            }
        }

        /// <summary>
        /// 数据库数据类型转换
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="databaseType"></param>
        /// <returns></returns>
        private string DataTypeConversion(string dataType, SqlSugar.DbType databaseType)
        {
            if (databaseType.Equals(SqlSugar.DbType.Oracle))
            {
                switch (dataType)
                {
                    case "text":
                        return "CLOB";
                    case "decimal":
                        return "DECIMAL(38,38)";
                    case "datetime":
                        return "DATE";
                    case "bigint":
                        return "NUMBER";
                    default:
                        return dataType.ToUpper();
                }
            }
            else if (databaseType.Equals(SqlSugar.DbType.Dm))
            {
                return dataType.ToUpper();
            }
            else if (databaseType.Equals(SqlSugar.DbType.Kdbndp))
            {
                switch (dataType)
                {
                    case "int":
                        return "NUMBER";
                    case "datetime":
                        return "DATE";
                    case "bigint":
                        return "INT8";
                    default:
                        return dataType.ToUpper();
                }
            }
            else if (databaseType.Equals(SqlSugar.DbType.PostgreSQL))
            {
                switch (dataType)
                {
                    case "varchar":
                        return "varchar";
                    case "int":
                        return "NUMBER";
                    case "datetime":
                        return "DATE";
                    case "decimal":
                        return "DECIMAL";
                    case "bigint":
                        return "INT8";
                    case "text":
                        return "TEXT";
                    default:
                        return dataType;
                }
            }
            else
            {
                return dataType;
            }
        }
    }
}
