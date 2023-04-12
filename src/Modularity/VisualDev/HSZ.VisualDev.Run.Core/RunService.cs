using HSZ.Dependency;
using HSZ.VisualDev.Entitys;
using HSZ.VisualDev.Entitys.Dto.VisualDevModelData;
using HSZ.VisualDev.Run.Interfaces;
using HSZ.Common.Helper;
using HSZ.VisualDev.Entitys.Model.VisualDevModelData;
using HSZ.Common.Core.Manager;
using HSZ.System.Interfaces.System;
using HSZ.System.Interfaces.Permission;
using HSZ.JsonSerialization;
using HSZ.VisualDev.Entitys.Entity;
using HSZ.System.Entitys.Model.System.DataBase;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Common.Const;
using HSZ.VisualDev.Entitys.Enum.VisualDevModelData;
using HSZ.System.Entitys.System;
using HSZ.FriendlyException;
using HSZ.Common.Enum;
using Newtonsoft.Json.Linq;
using HSZ.WorkFlow.Interfaces.FlowTask;
using HSZ.WorkFlow.Interfaces.FlowTask.Repository;
using HSZ.Common.Model;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using SqlSugar;
using Yitter.IdGenerator;
using HSZ.ClayObject.Extensions;
using HSZ.RemoteRequest.Extensions;
using HSZ.UnifyResult;
using SqlSugar.IOC;
using HSZ.ChangeDataBase;
using HSZ.WorkFlow.Interfaces.FlowEngine;
using System.Diagnostics;

namespace HSZ.VisualDev.Core
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：在线开发运行服务
    /// </summary>
    public class RunService : IRunService, ITransient
    {
        private readonly ISqlSugarRepository<VisualDevModelDataEntity> _visualDevModelDataRepository;
        private readonly ISqlSugarRepository<VisualDevEntity> _visualDevRepository;
        private readonly IBillRullService _billRuleService;
        private readonly IOrganizeService _organizeService;
        private readonly IDepartmentService _departmentService;
        private readonly IUsersService _userService;
        private readonly IPositionService _positionService;
        private readonly IChangeDataBase _databaseService;
        private readonly IDbLinkService _dbLinkService;
        private readonly IDictionaryDataService _dictionaryDataService;
        private readonly IDataInterfaceService _dataInterfaceService;
        private readonly IDictionaryTypeService _dictionaryTypeService;
        private readonly IProvinceService _provinceService;
        private readonly IUserManager _userManager; // 用户管理
        private readonly ICacheManager _cacheManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="RunService"/>类型的新实例
        /// </summary>
        public RunService(ISqlSugarRepository<VisualDevModelDataEntity> visualDevModelDataRepository,
            ISqlSugarRepository<VisualDevEntity> visualDevRepository,
            ICacheManager cacheManager,
            IUserManager userManager,
            IBillRullService billRuleService,
            IOrganizeService organizeService,
            IUsersService userService,
            IPositionService positionService,
            IChangeDataBase databaseService,
            IDbLinkService dbLinkService,
            IDictionaryDataService dictionaryDataService,
            IDataInterfaceService dataInterfaceService,
            IDictionaryTypeService dictionaryTypeService,
            IProvinceService provinceService,
            IDepartmentService departmentService)
        {
            _visualDevModelDataRepository = visualDevModelDataRepository;
            _visualDevRepository = visualDevRepository;
            _userManager = userManager;
            _cacheManager = cacheManager;
            _billRuleService = billRuleService;
            _organizeService = organizeService;
            _userService = userService;
            _positionService = positionService;
            _databaseService = databaseService;
            _dbLinkService = dbLinkService;
            _dictionaryDataService = dictionaryDataService;
            _dataInterfaceService = dataInterfaceService;
            _dictionaryTypeService = dictionaryTypeService;
            _provinceService = provinceService;
            _departmentService = departmentService;
            _db = DbScoped.SugarScope;
        }

        #region Get

        /// <summary>
        /// 列表数据处理
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PageResult<Dictionary<string, object>>> GetListResult(VisualDevEntity entity, VisualDevModelListQueryInput input, string actionType = "List")
        {
            var realList = new PageResult<Dictionary<string, object>>();
            realList.list = new List<Dictionary<string, object>>();
            var list = new List<VisualDevModelDataEntity>();

            var templateInfo = new TemplateParsingHelp(entity);//解析模板控件
            input.json = templateInfo.TimeControlQueryConvert(input.json);//时间控件 查询 处理

            //解开子表无限children，模板控件 可数据缓存
            Dictionary<string, object> templateData = await GetVisualDevTemplateData(entity.Id, TemplateCacheDataConversion(templateInfo.AllFieldsModel));

            //列表主键
            var primaryKey = "F_Id";

            if (templateInfo.IsHasTable)
            {
                var link = await _dbLinkService.GetInfo(entity.DbLinkId);
                if (link == null) link = GetTenantDbLink();//当前数据库连接
                var DbType = link?.DbType != null ? link.DbType : _db.CurrentConnectionConfig.DbType.ToString();//当前数据库类型
                var tableList = _databaseService.GetFieldList(link, templateInfo.MainTableName);//获取主表所有列
                var mainPrimary = tableList.Find(t => t.primaryKey == 1);//主表主键
                if (mainPrimary.IsNullOrEmpty()) throw HSZException.Oh(ErrorCode.D1402);//主表未设置主键
                primaryKey = mainPrimary.field;
                StringBuilder feilds = new StringBuilder();
                var sql = string.Empty;//查询sql

                //联表查询 表字段名称 对应 前端字段名称 (应对oracle 查询字段长度不能超过30个)
                var tableFieldKeyValue = new Dictionary<string, string>();

                //是否存在副表
                if (templateInfo.AuxiliaryTableFieldsModelList.Count < 1)//没有副表 只查询主表
                {
                    feilds.AppendFormat("{0},", primaryKey);
                    if (templateInfo.SingleFormData.Count > 0 || templateInfo.ColumnData.columnList.Count > 0)//只查询 要显示的列
                    {
                        templateInfo.ColumnData.columnList.ForEach(item =>
                        {
                            feilds.AppendFormat("{0},", item.prop);
                        });
                        feilds = new StringBuilder(feilds.ToString().TrimEnd(','));
                    }
                    else
                    {
                        tableList.ForEach(item =>
                        {
                            //主表列名
                            if (templateInfo.MainTableFieldsModelList.Find(x => x.__vModel__ == item.field) != null) feilds.AppendFormat("{0},", item.field);
                        });

                        feilds = new StringBuilder(feilds.ToString().TrimEnd(','));
                    }
                    sql = string.Format("select {0} from {1}", feilds, templateInfo.MainTableName);
                }
                else
                {
                    #region 所有主、副表 字段名 和 处理查询、排序字段
                    //所有主、副表 字段名
                    var cFieldNameList = new List<string>();
                    cFieldNameList.Add(templateInfo.MainTableName + "." + primaryKey);
                    tableFieldKeyValue.Add(primaryKey.ToUpper(), primaryKey);
                    var inputJson = input.json?.Deserialize<Dictionary<string, object>>();
                    for (var i = 0; i < templateInfo.SingleFormData.Count; i++)
                    {
                        if (templateInfo.ColumnData.columnList.Any(x => x.prop == templateInfo.SingleFormData[i].__vModel__))//只显示要显示的列
                        {
                            var vmodel = templateInfo.SingleFormData[i].__vModel__.ReplaceRegex(@"(\w+)_hsz_", "");//Field

                            if (vmodel.IsNotEmptyOrNull())
                            {
                                cFieldNameList.Add(templateInfo.SingleFormData[i].__config__.tableName + "." + vmodel + " FIELD_" + i);//TableName.Field_0
                                tableFieldKeyValue.Add("FIELD_" + i, templateInfo.SingleFormData[i].__vModel__);

                                //查询字段替换
                                if (inputJson!=null && inputJson.Count>0 && inputJson.ContainsKey(templateInfo.SingleFormData[i].__vModel__))
                                    input.json = input.json.Replace(templateInfo.SingleFormData[i].__vModel__ + "\":", "FIELD_" + i + "\":");

                                templateInfo.ColumnData.searchList.Where(x => x.__vModel__ == templateInfo.SingleFormData[i].__vModel__).ToList().ForEach(item =>
                                {
                                    item.__vModel__ = item.__vModel__.Replace(templateInfo.SingleFormData[i].__vModel__, "FIELD_" + i);
                                });

                                //排序字段替换
                                if (templateInfo.ColumnData.defaultSidx.IsNotEmptyOrNull() && templateInfo.ColumnData.defaultSidx == vmodel)
                                    templateInfo.ColumnData.defaultSidx = "FIELD_" + i;

                                if (input.sidx.IsNotEmptyOrNull() && input.sidx == vmodel)
                                    input.sidx = "FIELD_" + i;
                            }
                        }
                    }
                    feilds.Append(string.Join(",", cFieldNameList));
                    #endregion

                    #region 关联字段
                    var relationKey = new List<string>();
                    var auxiliaryFieldList = templateInfo.AuxiliaryTableFieldsModelList.Select(x => x.__config__.tableName).Distinct().ToList();
                    auxiliaryFieldList.ForEach(tName =>
                    {
                        var tableField = templateInfo.AllTable.Find(tf => tf.table == tName)?.tableField;
                        relationKey.Add(templateInfo.MainTableName + "." + primaryKey + "=" + tName + "." + tableField);
                    });
                    var whereStr = string.Join(" and ", relationKey);
                    #endregion

                    sql = string.Format("select {0} from {1} where {2}", feilds, templateInfo.MainTableName + "," + string.Join(",", auxiliaryFieldList), whereStr);//多表， 联合查询
                }

                var pvalue = _userManager.GetCondition<Dictionary<string, object>>(primaryKey,input.menuId, templateInfo.ColumnData.useDataPermission);
                realList = _databaseService.GetInterFaceData(link, sql, input, templateInfo.ColumnData, pvalue, tableFieldKeyValue);

                list = GetTableDataList(realList.list.ToList(), primaryKey);
            }
            else
            {
                list = await GetList(entity.Id);
            }

            input.sidx = string.IsNullOrEmpty(input.sidx) ? (templateInfo.ColumnData.defaultSidx == "" ? primaryKey : templateInfo.ColumnData.defaultSidx) : input.sidx;
            //关键字过滤
            if (list.Count > 0)
            {
                //将查询的关键字json转成Dictionary
                Dictionary<string, object> keywordJsonDic = string.IsNullOrEmpty(input.json) ? null : input.json.Deserialize<Dictionary<string, object>>();
                //将关键字查询传输的id转换成名称
                Dictionary<string, object> keyAndList = new Dictionary<string, object>();
                if ((!string.IsNullOrEmpty(entity.Tables) && "[]".Equals(entity.Tables)) || string.IsNullOrEmpty(entity.Tables))
                {
                    list = GetNoTableFilteringData(list, keywordJsonDic, templateInfo.FieldsModelList);
                    keyAndList = await GetKeyData(templateInfo.FieldsModelList, templateData, null, list, templateInfo.ColumnData, actionType, entity.WebType.ToInt(), primaryKey);
                    realList.list = GetQueryDataConversion(keyAndList["list"].Serialize().Deserialize<List<VisualDevModelDataEntity>>());
                    //关键字过滤
                    //realList.list = GetQueryFilteringData(keyAndList["keyJsonMap"].Serialize().Deserialize<Dictionary<string, object>>(), keyAndList["list"].Serialize().Deserialize<List<VisualDevModelDataEntity>>(), columnData);
                }
                else
                {
                    keyAndList = await GetKeyData(templateInfo.FieldsModelList, templateData, null, list, templateInfo.ColumnData, actionType, entity.WebType.ToInt(), primaryKey);
                    realList.list = GetQueryDataConversion(keyAndList["list"].Serialize().Deserialize<List<VisualDevModelDataEntity>>());
                }
                if (!templateInfo.IsHasTable && input.sidx.IsNotEmptyOrNull())//如果是无表数据并且排序字段不为空，再进行数据排序
                {
                    if (input.sort == "desc")
                    {
                        realList.list = realList.list.OrderByDescending(x =>
                        {
                            var dic = x as IDictionary<string, object>;
                            dic.GetOrAdd(input.sidx, () => null);
                            return dic[input.sidx];
                        }).ToList();
                    }
                    else
                    {
                        realList.list = realList.list.OrderBy(x =>
                        {
                            var dic = x as IDictionary<string, object>;
                            dic.GetOrAdd(input.sidx, () => null);
                            return dic[input.sidx];
                        }).ToList();
                    }
                }
            }
            if (input.dataType == "0")
            {
                if (!string.IsNullOrEmpty(entity.Tables) && !"[]".Equals(entity.Tables))
                {
                }
                else
                {
                    realList.pagination = new PageResult();
                    realList.pagination.total = realList.list.Count;
                    realList.pagination.pageSize = input.pageSize;
                    realList.pagination.pageIndex = input.currentPage;
                    realList.list = realList.list.Skip(input.pageSize * (input.currentPage - 1)).Take(input.pageSize).ToList();
                }
                //分组表格
                if (templateInfo.ColumnData.type == 3)
                {
                    var groupList = templateInfo.ColumnData.columnList.Where(p => p.prop == templateInfo.ColumnData.groupField).ToList();
                    var exceptList = templateInfo.ColumnData.columnList.Except(groupList).FirstOrDefault();
                    //分组数据
                    Dictionary<string, List<Dictionary<string, object>>> groupDic = new Dictionary<string, List<Dictionary<string, object>>>();
                    foreach (var item in realList.list)
                    {
                        if (item.ContainsKey(templateInfo.ColumnData.groupField))
                        {
                            var groupDicKey = item[templateInfo.ColumnData.groupField] is null ? "" : item[templateInfo.ColumnData.groupField].ToString();
                            if (!groupDic.ContainsKey(groupDicKey))
                            {
                                groupDic.Add(groupDicKey, new List<Dictionary<string, object>>()); //初始化
                            }
                            item.Remove(templateInfo.ColumnData.groupField);
                            groupDic[groupDicKey].Add(item);
                        }
                        else
                        {
                            var groupDicKey = "null";
                            if (!groupDic.ContainsKey(groupDicKey))
                            {
                                groupDic.Add(groupDicKey, new List<Dictionary<string, object>>()); //初始化
                            }
                            groupDic[groupDicKey].Add(item);
                        }

                    }
                    List<Dictionary<string, object>> realGroupDic = new List<Dictionary<string, object>>();
                    foreach (var item in groupDic)
                    {
                        Dictionary<string, object> dataMap = new Dictionary<string, object>();
                        dataMap.Add("top", true);
                        dataMap.Add("id", YitIdHelper.NextId().ToString());
                        dataMap.Add("children", DateConver(item.Value));
                        if (!exceptList.IsEmpty() && !string.IsNullOrWhiteSpace(exceptList.prop)) dataMap.Add(exceptList.prop, item.Key);
                        else dataMap.Add(templateInfo.ColumnData.groupField, item.Key);
                        realGroupDic.Add(dataMap);
                    }
                    realList.list = realGroupDic;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(entity.Tables) && !"[]".Equals(entity.Tables))
                {
                }
                else
                {
                    realList.pagination = new PageResult();
                    realList.pagination.total = realList.list.Count;
                    realList.pagination.pageSize = input.pageSize;
                    realList.pagination.pageIndex = input.currentPage;
                    realList.list = realList.list.ToList();
                }
            }
            realList.list = DateConver(realList.list);
            return realList;
        }

        /// <summary>
        /// 关联表单列表数据处理
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PageResult<Dictionary<string, object>>> GetRelationFormList(VisualDevEntity entity, VisualDevModelListQueryInput input, string actionType = "List")
        {
            var realList = new PageResult<Dictionary<string, object>>();
            realList.list = new List<Dictionary<string, object>>();
            var list = new List<VisualDevModelDataEntity>();

            var templateInfo = new TemplateParsingHelp(entity);//解析模板控件
            input.json = templateInfo.TimeControlQueryConvert(input.json);//时间控件 查询 处理

            //解开子表无限children，模板控件 可数据缓存
            Dictionary<string, object> templateData = await GetVisualDevTemplateData(entity.Id, TemplateCacheDataConversion(templateInfo.AllFieldsModel));

            //列表主键
            var primaryKey = "F_Id";

            if (templateInfo.IsHasTable)
            {
                var link = await _dbLinkService.GetInfo(entity.DbLinkId);
                if (link == null) link = GetTenantDbLink();//当前数据库连接
                var DbType = link?.DbType != null ? link.DbType : _db.CurrentConnectionConfig.DbType.ToString();//当前数据库类型
                var tableList = _databaseService.GetFieldList(link, templateInfo.MainTableName);//获取主表所有列
                var mainPrimary = tableList.Find(t => t.primaryKey == 1);//主表主键
                if (mainPrimary.IsNullOrEmpty()) throw HSZException.Oh(ErrorCode.D1402);//主表未设置主键
                primaryKey = mainPrimary.field;
                StringBuilder feilds = new StringBuilder();
                var sql = string.Empty;//查询sql

                //联表查询 表字段名称 对应 前端字段名称 (应对oracle 查询字段长度不能超过30个)
                var tableFieldKeyValue = new Dictionary<string, string>();

                //是否存在副表
                if (templateInfo.AuxiliaryTableFieldsModelList.Count < 1)//没有副表 只查询主表
                {
                    feilds.AppendFormat("{0},", primaryKey);
                    if (templateInfo.SingleFormData.Count > 0 || templateInfo.ColumnData.columnList.Count > 0)//只查询 要显示的列
                    {
                        templateInfo.ColumnData.columnList.ForEach(item =>
                        {
                            feilds.AppendFormat("{0},", item.prop);
                        });
                        feilds = new StringBuilder(feilds.ToString().TrimEnd(','));
                    }
                    else
                    {
                        tableList.ForEach(item =>
                        {
                            //主表列名
                            if (templateInfo.MainTableFieldsModelList.Find(x => x.__vModel__ == item.field) != null) feilds.AppendFormat("{0},", item.field);
                        });

                        feilds = new StringBuilder(feilds.ToString().TrimEnd(','));
                    }
                    sql = string.Format("select {0} from {1}", feilds, templateInfo.MainTableName);
                }
                else
                {
                    #region 所有主、副表 字段名 和 处理查询、排序字段
                    //所有主、副表 字段名
                    var cFieldNameList = new List<string>();
                    cFieldNameList.Add(templateInfo.MainTableName + "." + primaryKey);
                    tableFieldKeyValue.Add(primaryKey.ToUpper(), primaryKey);
                    var inputJson = input.json?.Deserialize<Dictionary<string, object>>();
                    for (var i = 0; i < templateInfo.SingleFormData.Count; i++)
                    {
                        if (templateInfo.ColumnData.columnList.Any(x => x.prop == templateInfo.SingleFormData[i].__vModel__))//只显示要显示的列
                        {
                            var vmodel = templateInfo.SingleFormData[i].__vModel__.ReplaceRegex(@"(\w+)_hsz_", "");//Field

                            if (vmodel.IsNotEmptyOrNull())
                            {
                                cFieldNameList.Add(templateInfo.SingleFormData[i].__config__.tableName + "." + vmodel + " FIELD_" + i);//TableName.Field_0
                                tableFieldKeyValue.Add("FIELD_" + i, templateInfo.SingleFormData[i].__vModel__);

                                //查询字段替换
                                if (inputJson != null && inputJson.Count > 0 && inputJson.ContainsKey(templateInfo.SingleFormData[i].__vModel__))
                                    input.json = input.json.Replace(templateInfo.SingleFormData[i].__vModel__ + "\":", "FIELD_" + i + "\":");

                                templateInfo.ColumnData.searchList.Where(x => x.__vModel__ == templateInfo.SingleFormData[i].__vModel__).ToList().ForEach(item =>
                                {
                                    item.__vModel__ = item.__vModel__.Replace(templateInfo.SingleFormData[i].__vModel__, "FIELD_" + i);
                                });

                                //排序字段替换
                                if (!string.IsNullOrWhiteSpace(templateInfo.ColumnData.defaultSidx) && templateInfo.ColumnData.defaultSidx == vmodel)
                                    templateInfo.ColumnData.defaultSidx = "FIELD_" + i;

                                if (input.sidx.IsNotEmptyOrNull() && input.sidx == vmodel)
                                    input.sidx = "FIELD_" + i;
                            }
                        }
                    }
                    feilds.Append(string.Join(",", cFieldNameList));
                    #endregion

                    #region 关联字段
                    var relationKey = new List<string>();
                    var auxiliaryFieldList = templateInfo.AuxiliaryTableFieldsModelList.Select(x => x.__config__.tableName).Distinct().ToList();
                    auxiliaryFieldList.ForEach(tName =>
                    {
                        var tableField = templateInfo.AllTable.Find(tf => tf.table == tName)?.tableField;
                        relationKey.Add(templateInfo.MainTableName + "." + primaryKey + "=" + tName + "." + tableField);
                    });
                    var whereStr = string.Join(" and ", relationKey);
                    #endregion

                    sql = string.Format("select {0} from {1} where {2}", feilds, templateInfo.MainTableName + "," + string.Join(",", auxiliaryFieldList), whereStr);//多表， 联合查询
                }

                var pvalue = new List<IConditionalModel>();//关联表单调用 数据全部放开
                //var pageSize = input.pageSize;//存储设定的分页，实现伪分页
                var queryJson = input.json;
                //input.pageSize = 1000000;//伪分页, 关联表单需要查询全部数据并转换后再模糊查询
                input.json = "";
                realList = _databaseService.GetInterFaceData(link, sql, input, templateInfo.ColumnData, pvalue, tableFieldKeyValue);

                list = GetTableDataList(realList.list.ToList(), primaryKey);
                //input.pageSize = pageSize;
                input.json = queryJson;
            }
            else
            {
                list = await GetList(entity.Id);
            }

            input.sidx = string.IsNullOrEmpty(input.sidx) ? (templateInfo.ColumnData.defaultSidx == "" ? primaryKey : templateInfo.ColumnData.defaultSidx) : input.sidx;
            //关键字过滤
            if (list.Count > 0)
            {
                //将查询的关键字json转成Dictionary
                Dictionary<string, object> keywordJsonDic = string.IsNullOrEmpty(input.json) ? null : input.json.Deserialize<Dictionary<string, object>>();
                //将关键字查询传输的id转换成名称
                Dictionary<string, object> keyAndList = new Dictionary<string, object>();
                if ((!string.IsNullOrEmpty(entity.Tables) && "[]".Equals(entity.Tables)) || string.IsNullOrEmpty(entity.Tables))
                {
                    list = GetNoTableFilteringData(list, keywordJsonDic, templateInfo.FieldsModelList);
                    keyAndList = await GetKeyData(templateInfo.FieldsModelList, templateData, null, list, templateInfo.ColumnData, actionType, entity.WebType.ToInt(), primaryKey);
                    realList.list = GetQueryDataConversion(keyAndList["list"].Serialize().Deserialize<List<VisualDevModelDataEntity>>());
                    //关键字过滤
                    //realList.list = GetQueryFilteringData(keyAndList["keyJsonMap"].Serialize().Deserialize<Dictionary<string, object>>(), keyAndList["list"].Serialize().Deserialize<List<VisualDevModelDataEntity>>(), columnData);
                }
                else
                {
                    keyAndList = await GetKeyData(templateInfo.FieldsModelList, templateData, null, list, templateInfo.ColumnData, actionType, entity.WebType.ToInt(), primaryKey);
                    realList.list = GetQueryDataConversion(keyAndList["list"].Serialize().Deserialize<List<VisualDevModelDataEntity>>());

                    if (input.json.IsNotEmptyOrNull())
                    {
                        var search = input.json.Deserialize<Dictionary<string, string>>();
                        if (search.FirstOrDefault().Value.IsNotEmptyOrNull())
                        {
                            var keword = search.FirstOrDefault().Value;

                            var newList = new List<Dictionary<string, object>>();
                            var columnName = templateInfo.ColumnData.columnList.Select(x=>x.prop).ToList();
                            realList.list.ForEach(item =>
                            {
                                if (item.Where(x => x.Value != null && columnName.Contains(x.Key)).Where(x => x.Value.ToString().Contains(keword)).Any())
                                    newList.Add(item);
                            });

                            realList.list = newList;
                        }
                    }
                }

                if (input.sort == "desc")
                {
                    realList.list = realList.list.OrderByDescending(x =>
                    {
                        var dic = x as IDictionary<string, object>;
                        dic.GetOrAdd(input.sidx, () => null);
                        return dic[input.sidx];
                    }).ToList();
                }
                else
                {
                    realList.list = realList.list.OrderBy(x =>
                    {
                        var dic = x as IDictionary<string, object>;
                        dic.GetOrAdd(input.sidx, () => null);
                        return dic[input.sidx];
                    }).ToList();
                }
            }

            if (!string.IsNullOrEmpty(entity.Tables) && !"[]".Equals(entity.Tables))
            {
            }
            else
            {
                realList.pagination = new PageResult();
                realList.pagination.total = realList.list.Count;
                realList.pagination.pageSize = input.pageSize;
                realList.pagination.pageIndex = input.currentPage;
                realList.list = realList.list.ToList();
            }
            realList.list = DateConver(realList.list);
            return realList;
        }

        /// <summary>
        /// 获取模型数据信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<VisualDevModelDataEntity> GetInfo(string id)
        {
            return await _visualDevModelDataRepository.GetSingleAsync(m => m.Id == id);
        }

        /// <summary>
        /// 获取无表详情转换
        /// </summary>
        /// <param name="templateEntity">模板实体</param>
        /// <param name="data">真实数据</param>
        /// <returns></returns>
        public async Task<string> GetIsNoTableInfo(VisualDevEntity templateEntity, string data)
        {
            var formData = TemplateKeywordsHelper.ReplaceKeywords(templateEntity.FormData).ToObject<FormDataModel>();
            var modelList = formData.fields;
            modelList = TemplateCacheDataConversion(modelList);
            //获取Redis缓存模板数据
            var templateData = await GetVisualDevTemplateData(templateEntity.Id, modelList);
            data = await GetSystemComponentsData(modelList, templateData, data);
            return data;
        }

        /// <summary>
        /// 获取有表详情转换
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">模板实体</param>
        /// <returns></returns>
        public async Task<string> GetHaveTableInfo(string id, VisualDevEntity templateEntity)
        {
            var templateInfo = new TemplateParsingHelp(templateEntity);//解析模板控件
            var sql = string.Empty;//联表查询sql
            var feildList = new List<string>();//存放所有查询列名

            var link = await _dbLinkService.GetInfo(templateEntity.DbLinkId);
            if (link == null) link = GetTenantDbLink();//存储当前数据连接
            var tableList = _databaseService.GetFieldList(link, templateInfo.MainTableName);//获取主表 所有字段名
            var mainPrimary = tableList.Find(t => t.primaryKey == 1)?.field;//主表主键
            if (mainPrimary.IsNullOrEmpty()) throw HSZException.Oh(ErrorCode.D1402);//主表未设置主键

            //联表查询 表字段 别名
            var tableFieldKeyValue = new Dictionary<string, string>();
            //是否存在副表
            if (templateInfo.AuxiliaryTableFieldsModelList.Count<1)//没有副表,只查询主表
            {
                feildList.Add(mainPrimary);//主表主键
                for (var i = 0; i < tableList.Count; i++)
                {
                    if (templateInfo.MainTableFieldsModelList.Any(x => x.__vModel__ == tableList[i].field))
                        feildList.Add(tableList[i].field);//主表列名
                }
                sql = string.Format("select {0} from {1} where {2}='{3}'", string.Join(",", feildList), templateInfo.MainTableName, mainPrimary, id);
            }
            else
            {
                #region 所有主表、副表 字段名
                feildList.Add(templateInfo.MainTableName + "." + mainPrimary);//主表主键
                tableFieldKeyValue.Add(mainPrimary.ToUpper(), mainPrimary);
                for (var i = 0; i < templateInfo.SingleFormData.Count ; i++)
                {
                    var vmodel = templateInfo.SingleFormData[i].__vModel__.ReplaceRegex(@"(\w+)_hsz_", "");//Field
                    if (vmodel.IsNotEmptyOrNull())
                    {
                        feildList.Add(templateInfo.SingleFormData[i].__config__.tableName + "." + vmodel + " FIELD" + i);//TableName.Field_0
                        tableFieldKeyValue.Add("FIELD" + i, templateInfo.SingleFormData[i].__vModel__);
                    }
                }
                #endregion

                #region 所有副表 关联字段
                var ctNameList = templateInfo.AuxiliaryTableFieldsModelList.Select(x => x.__config__.tableName).Distinct().ToList();
                var relationKey = new List<string>();
                relationKey.Add(string.Format(" {0}.{1}='{2}' ", templateInfo.MainTableName, mainPrimary, id));//主表ID
                ctNameList.ForEach(tName =>
                {
                    var tableField = templateInfo.AllTable.Find(tf => tf.table == tName)?.tableField;
                    relationKey.Add(string.Format(" {0}.{1}={2}.{3} ", templateInfo.MainTableName, mainPrimary , tName , tableField));
                });
                var whereStr = string.Join(" and ", relationKey);
                #endregion

                sql=string.Format("select {0} from {1} where {2}", string.Join(",", feildList), templateInfo.MainTableName + "," + string.Join(",", ctNameList), whereStr);//多表， 联合查询
            }

            var Data = _databaseService.GetInterFaceData(link, sql.ToString()).Serialize().Deserialize<List<Dictionary<string, object>>>();
            if (Data.Count < 1) return id;
            var mainData= new List<Dictionary<string, object>>();
            #region 查询别名转换
            if (templateInfo.AuxiliaryTableFieldsModelList.Count > 0)
            {
                foreach (var dicItem in Data)
                {
                    var temp = new Dictionary<string, object>();
                    foreach (var item in dicItem)
                    {
                        temp.Add(tableFieldKeyValue[item.Key.ToUpper()], item.Value);
                    }
                    mainData.Add(temp);
                }
            }
            else
            {
                mainData = Data;
            }
            #endregion

            List<Dictionary<string, object>> mainMap = GetTableDataInfoByTimeStamp(mainData, templateInfo.FieldsModelList, "detail");
            //记录全部数据
            Dictionary<string, object> dataMap = mainMap.FirstOrDefault();
            Dictionary<string, object> newDataMap = new Dictionary<string, object>();

            #region 处理子表数据
            foreach (var model in templateInfo.ChildTableFieldsModelList)
            {
                if (!string.IsNullOrEmpty(model.__vModel__))
                {
                    if ("table".Equals(model.__config__.hszKey))
                    {
                        StringBuilder feilds = new StringBuilder();
                        List<FieldsModel> childModelList = model.__config__.children;
                        foreach (var childModel in childModelList)
                        {
                            if (!string.IsNullOrEmpty(childModel.__vModel__)) feilds.Append(childModel.__vModel__ + ",");
                        }
                        if (childModelList.Count > 0)
                        {
                            feilds = new StringBuilder(feilds.ToString().TrimEnd(','));
                        }
                        //子表字段
                        string relationFeild = "";
                        //主表字段
                        string relationMainFeild = "";
                        string relationMainFeildValue = "";
                        //查询子表数据
                        StringBuilder childSql = new StringBuilder();
                        childSql.Append("select " + feilds + " from " + model.__config__.tableName + " where 1=1 ");
                        foreach (var tableMap in templateInfo.AllTable)
                        {
                            if (tableMap.table.Equals(model.__config__.tableName))
                            {
                                relationFeild = tableMap.tableField;
                                relationMainFeild = tableMap.relationField;
                                if (dataMap.ContainsKey(relationMainFeild))
                                {
                                    relationMainFeildValue = dataMap[relationMainFeild].ToString();
                                    childSql.Append(@" And " + relationFeild + "='" + relationMainFeildValue + "'");
                                }
                                var childData = _databaseService.GetInterFaceData(link, childSql.ToString()).Serialize().Deserialize<List<Dictionary<string, object>>>();
                                var childTableData = GetTableDataInfoByTimeStamp(childData, model.__config__.children, "detail");

                                #region 获取关联表单属性 和 弹窗选择属性
                                var rFormList = model.__config__.children.Where(x => x.__config__.hszKey == "relationForm").ToList();
                                foreach (var item in rFormList)
                                {
                                    foreach (var dataItem in childTableData)
                                    {
                                        if (!string.IsNullOrWhiteSpace(item.__vModel__) && dataItem.ContainsKey(item.__vModel__) && dataItem[item.__vModel__] != null)
                                        {
                                            //获取关联表单id
                                            var relationValueId = dataItem[item.__vModel__].ToString();

                                            //获取 关联表单 转换后的数据
                                            var relationInfo = await _visualDevRepository.GetByIdAsync(item.modelId);
                                            var relationValueStr = "";
                                            if (relationInfo.Tables.IsNullOrWhiteSpace() || relationInfo.Tables.Equals("[]"))
                                            {
                                                var noTableData = await GetInfo(relationValueId);
                                                relationValueStr = await GetIsNoTableInfo(relationInfo, noTableData.Data);
                                            }
                                            else
                                                relationValueStr = await GetHaveTableInfo(relationValueId, relationInfo);

                                            var relationValue = relationValueStr.ToObject<Dictionary<string, object>>();

                                            //添加到 子表 列
                                            model.__config__.children.Where(x => x.relationField.ReplaceRegex(@"_hszTable_(\w+)", "") == item.__vModel__).ToList().ForEach(citem =>
                                            {
                                                citem.__vModel__ = item.__vModel__ + "_" + citem.showField;
                                                dataItem.Add(item.__vModel__ + "_" + citem.showField, relationValue[citem.showField]);
                                            });
                                        }
                                    }
                                }

                                if (model.__config__.children.Where(x => x.__config__.hszKey == "popupAttr").Any())
                                {
                                    var pFormList = model.__config__.children.Where(x => x.__config__.hszKey == "popupSelect").ToList();
                                    foreach (var item in pFormList)
                                    {
                                        var pDataList = await GetPopupSelectDataList(item.interfaceId, model);//获取接口数据列表
                                        foreach (var dataItem in childTableData)
                                        {
                                            if (!string.IsNullOrWhiteSpace(item.__vModel__) && dataItem.ContainsKey(item.__vModel__) && dataItem[item.__vModel__] != null)
                                            {
                                                //获取关联表单id
                                                var relationValueId = dataItem[item.__vModel__].ToString();

                                                //添加到 子表 列
                                                model.__config__.children.Where(x => x.relationField.ReplaceRegex(@"_hszTable_(\w+)", "") == item.__vModel__).ToList().ForEach(citem =>
                                                {
                                                    var id = dataItem[item.__vModel__].ToString();
                                                    citem.__vModel__ = item.__vModel__ + "_" + citem.showField;
                                                    var value = pDataList.Where(x => x.Values.Contains(id)).FirstOrDefault();
                                                    if (value.Keys.IsNotEmptyOrNull()) dataItem.Add(item.__vModel__ + "_" + citem.showField, value[citem.showField]);
                                                });
                                            }
                                        }
                                    }
                                }
                                #endregion

                                if (childTableData.Count > 0)
                                {
                                    newDataMap[model.__vModel__] = childTableData;
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            int dicCount = newDataMap.Keys.Count;
            string[] strKey = new string[dicCount];
            newDataMap.Keys.CopyTo(strKey, 0);
            for (int i = 0; i < strKey.Length; i++)
            {
                var model = templateInfo.FieldsModelList.Where(m => m.__vModel__ == strKey[i]).FirstOrDefault();
                if (model != null)
                {
                    List<Dictionary<string, object>> tables = newDataMap[strKey[i]].ToObject<List<Dictionary<string, object>>>();
                    List<Dictionary<string, object>> newTables = new List<Dictionary<string, object>>();
                    foreach (var item in tables)
                    {
                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        foreach (var value in item)
                        {
                            var child = model.__config__.children.Find(c => c.__vModel__ == value.Key);
                            if (child != null)
                            {
                                dic.Add(value.Key, value.Value);
                            }
                        }
                        newTables.Add(dic);
                    }
                    if (newTables.Count > 0)
                    {
                        newDataMap[strKey[i]] = newTables;
                    }
                }
            }

            foreach (var entryMap in dataMap)
            {
                if (entryMap.Value != null)
                {
                    var model = templateInfo.FieldsModelList.Where(m => m.__vModel__ == entryMap.Key.ToString()).FirstOrDefault();
                    if (model != null)
                    {
                        if (entryMap.Key.Equals(model.__vModel__))
                        {
                            newDataMap[entryMap.Key] = entryMap.Value;
                        }
                    }
                }
            }

            //处理 条形码 、 二维码 控件
            GetBARAndQR(templateInfo.FieldsModelList, newDataMap, dataMap);

            return await GetTableInfoBySystemComponentsData(templateEntity.Id, templateInfo.FieldsModelList, newDataMap.Serialize());
        }

        /// <summary>
        /// 获取有表详情转换
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">模板实体</param>
        /// <returns></returns>
        public async Task<string> GetHaveTableInfoDetails(string id, VisualDevEntity templateEntity, bool isFlowTask = false)
        {
            var templateInfo = new TemplateParsingHelp(templateEntity, isFlowTask);//解析模板控件

            //解开子表无限children，模板控件 可数据缓存
            Dictionary<string, object> templateData = await GetVisualDevTemplateData(templateEntity.Id, TemplateCacheDataConversion(templateInfo.AllFieldsModel));

            List<TableModel> tableMapList = templateEntity.Tables.ToObject<List<TableModel>>();
            StringBuilder mainSql = new StringBuilder();

            var link = await _dbLinkService.GetInfo(templateEntity.DbLinkId);
            if (link == null) link = GetTenantDbLink();//存储当前数据连接
            var tableList = _databaseService.GetFieldList(link, templateInfo.MainTableName);//获取主表 所有字段名
            var mainPrimary = tableList.Find(t => t.primaryKey == 1)?.field;//主表主键
            if (mainPrimary.IsNullOrEmpty()) throw HSZException.Oh(ErrorCode.D1402);//主表未设置主键

            var sql = string.Empty;//联表查询sql
            var feildList = new List<string>();//存放所有查询列名

            //联表查询 表字段 别名
            var tableFieldKeyValue = new Dictionary<string, string>();

            //是否存在副表
            if (templateInfo.AuxiliaryTableFieldsModelList.Count < 1)//没有副表,只查询主表
            {
                feildList.Add(mainPrimary);//主表主键
                for (var i = 0; i < tableList.Count; i++)
                {
                    if (templateInfo.MainTableFieldsModelList.Any(x => x.__vModel__ == tableList[i].field))
                        feildList.Add(tableList[i].field);//主表列名
                }
                sql = string.Format("select {0} from {1} where {2}='{3}'", string.Join(",", feildList), templateInfo.MainTableName, mainPrimary, id);
            }
            else
            {
                #region 所有主表、副表 字段名
                feildList.Add(templateInfo.MainTableName + "." + mainPrimary);//主表主键
                tableFieldKeyValue.Add(mainPrimary.ToUpper(), mainPrimary);
                for (var i = 0; i < templateInfo.SingleFormData.Count; i++)
                {
                    var vmodel = templateInfo.SingleFormData[i].__vModel__.ReplaceRegex(@"(\w+)_hsz_", "");//Field
                    if (vmodel.IsNotEmptyOrNull())
                    {
                        feildList.Add(templateInfo.SingleFormData[i].__config__.tableName + "." + vmodel + " FIELD" + i);//TableName.Field_0
                        tableFieldKeyValue.Add("FIELD" + i, templateInfo.SingleFormData[i].__vModel__);
                    }
                }
                #endregion

                #region 所有副表 关联字段
                var ctNameList = templateInfo.AuxiliaryTableFieldsModelList.Select(x => x.__config__.tableName).Distinct().ToList();
                var relationKey = new List<string>();
                relationKey.Add(string.Format(" {0}.{1}='{2}' ", templateInfo.MainTableName, mainPrimary, id));//主表ID
                ctNameList.ForEach(tName =>
                {
                    var tableField = templateInfo.AllTable.Find(tf => tf.table == tName)?.tableField;
                    relationKey.Add(string.Format(" {0}.{1}={2}.{3} ", templateInfo.MainTableName, mainPrimary, tName, tableField));
                });
                var whereStr = string.Join(" and ", relationKey);
                #endregion

                sql = string.Format("select {0} from {1} where {2}", string.Join(",", feildList), templateInfo.MainTableName + "," + string.Join(",", ctNameList), whereStr);//多表， 联合查询
            }

            var Data = _databaseService.GetInterFaceData(link, sql.ToString()).Serialize().Deserialize<List<Dictionary<string, object>>>();
            if (Data.Count < 1) return id;

            var mainData = new List<Dictionary<string, object>>();
            #region 查询别名转换
            if (templateInfo.AuxiliaryTableFieldsModelList.Count > 0)
            {
                foreach (var dicItem in Data)
                {
                    var temp = new Dictionary<string, object>();
                    foreach (var item in dicItem)
                    {
                        temp.Add(tableFieldKeyValue[item.Key.ToUpper()], item.Value);
                    }
                    mainData.Add(temp);
                }
            }
            else
            {
                mainData = Data;
            }
            #endregion

            List<Dictionary<string, object>> mainMap = GetTableDataInfo(mainData, templateInfo.FieldsModelList, "detail");
            //记录全部主表数据
            Dictionary<string, object> dataMap = mainMap.FirstOrDefault();
            Dictionary<string, object> newDataMap = new Dictionary<string, object>();

            #region 处理子表数据
            foreach (var model in templateInfo.ChildTableFieldsModelList)
            {
                if (!string.IsNullOrEmpty(model.__vModel__))
                {
                    if ("table".Equals(model.__config__.hszKey))
                    {
                        StringBuilder feilds = new StringBuilder();
                        List<FieldsModel> childModelList = model.__config__.children;
                        foreach (var childModel in childModelList)
                        {
                            if (!string.IsNullOrEmpty(childModel.__vModel__)) feilds.Append(childModel.__vModel__ + ",");
                        }
                        if (childModelList.Count > 0)
                        {
                            feilds = new StringBuilder(feilds.ToString().TrimEnd(','));
                        }
                        //子表字段
                        string relationFeild = "";
                        //主表字段
                        string relationMainFeild = "";
                        string relationMainFeildValue = "";
                        //查询子表数据
                        StringBuilder childSql = new StringBuilder();
                        childSql.Append("select " + feilds + " from " + model.__config__.tableName + " where 1=1 ");
                        foreach (var tableMap in tableMapList)
                        {
                            if (tableMap.table.Equals(model.__config__.tableName))
                            {
                                relationFeild = tableMap.tableField;
                                relationMainFeild = tableMap.relationField;
                                if (dataMap != null && dataMap.ContainsKey(relationMainFeild))
                                {
                                    relationMainFeildValue = dataMap[relationMainFeild].ToString();
                                    childSql.Append(@" And " + relationFeild + "='" + relationMainFeildValue + "'");
                                }
                                var childData = _databaseService.GetInterFaceData(link, childSql.ToString()).Serialize().Deserialize<List<Dictionary<string, object>>>();
                                var childTableData = GetTableDataInfo(childData, model.__config__.children, "transition");

                                #region 获取关联表单属性
                                var rFormList = model.__config__.children.Where(x => x.__config__.hszKey == "relationForm").ToList();
                                foreach (var item in rFormList)
                                {
                                    foreach(var dataItem in childTableData)
                                    {
                                        if (!string.IsNullOrWhiteSpace(item.__vModel__) && dataItem.ContainsKey(item.__vModel__) && dataItem[item.__vModel__]!=null)
                                        {
                                            //获取关联表单id
                                            var relationValueId = dataItem[item.__vModel__].ToString();

                                            //获取 关联表单 转换后的数据
                                            var relationInfo = await _visualDevRepository.GetByIdAsync(item.modelId);
                                            var relationValueStr = "";
                                            if (relationInfo.Tables.IsNullOrWhiteSpace() || relationInfo.Tables.Equals("[]"))
                                            {
                                                var noTableData = await GetInfo(relationValueId);
                                                relationValueStr = await GetIsNoTableInfo(relationInfo, noTableData.Data);
                                            }
                                            else
                                                relationValueStr = await GetHaveTableInfoDetails(relationValueId, relationInfo, isFlowTask);

                                            var relationValue = relationValueStr.ToObject<Dictionary<string, object>>();

                                            //添加到 子表 列
                                            model.__config__.children.Where(x => x.relationField.ReplaceRegex(@"_hszTable_(\w+)", "") == item.__vModel__).ToList().ForEach(citem =>
                                            {
                                                citem.__vModel__ = item.__vModel__ + "_" + citem.showField;
                                                dataItem.Add(item.__vModel__ + "_" + citem.showField, relationValue[citem.showField]);
                                            });
                                        }
                                    }
                                }

                                if(model.__config__.children.Where(x => x.__config__.hszKey == "popupAttr").Any())
                                {
                                    var pFormList = model.__config__.children.Where(x => x.__config__.hszKey == "popupSelect").ToList();
                                    foreach (var item in pFormList)
                                    {
                                        var pDataList = await GetPopupSelectDataList(item.interfaceId, model);//获取接口数据列表
                                        foreach (var dataItem in childTableData)
                                        {
                                            if (!string.IsNullOrWhiteSpace(item.__vModel__) && dataItem.ContainsKey(item.__vModel__) && dataItem[item.__vModel__] != null)
                                            {
                                                //获取关联表单id
                                                var relationValueId = dataItem[item.__vModel__].ToString();

                                                //添加到 子表 列
                                                model.__config__.children.Where(x => x.relationField.ReplaceRegex(@"_hszTable_(\w+)", "") == item.__vModel__).ToList().ForEach(citem =>
                                                {
                                                    var id = dataItem[item.__vModel__].ToString();
                                                    var value = pDataList.Where(x => x.Values.Contains(id)).FirstOrDefault();
                                                    citem.__vModel__ = item.__vModel__ + "_" + citem.showField;
                                                    if (value.Keys.IsNotEmptyOrNull()) dataItem.Add(item.__vModel__ + "_" + citem.showField, value[citem.showField]);
                                                });
                                            }
                                        }
                                    }
                                }
                                #endregion

                                if (childTableData.Count > 0)
                                {
                                    newDataMap[model.__vModel__] = childTableData;
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            int dicCount = newDataMap.Keys.Count;
            string[] strKey = new string[dicCount];
            newDataMap.Keys.CopyTo(strKey, 0);
            for (int i = 0; i < strKey.Length; i++)
            {
                var model = templateInfo.FieldsModelList.Find(m => m.__vModel__ == strKey[i]);
                if (model != null)
                {
                    List<VisualDevModelDataEntity> childModelData = new List<VisualDevModelDataEntity>();
                    List<Dictionary<string, object>> tables = newDataMap[strKey[i]].ToObject<List<Dictionary<string, object>>>();
                    List<Dictionary<string, object>> newTables = new List<Dictionary<string, object>>();
                    foreach (var item in tables)
                    {
                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        foreach (KeyValuePair<string, object> value in item)
                        {
                            var child = model.__config__.children.Find(c => c.__vModel__ == value.Key);
                            if (child != null && value.Value != null)
                            {
                                var hszKey = child.__config__.hszKey;
                                switch (hszKey)
                                {
                                    case "time":
                                        dic.Add(value.Key, value.Value);
                                        break;
                                    case "date":
                                        dic.Add(value.Key, Ext.ConvertToTimeStamp(value.Value.ToDate()));
                                        break;
                                    default:
                                        dic.Add(value.Key, value.Value);
                                        break;
                                }

                            }
                        }
                        childModelData.Add(new VisualDevModelDataEntity()
                        {
                            Id = YitIdHelper.NextId().ToString(),
                            Data = dic.ToJson()
                        });
                    }
                    if (childModelData.Count > 0)
                    {
                        //将关键字查询传输的id转换成名称
                        Dictionary<string, object> childKeyAndList = await GetKeyData(model.__config__.children, templateData, null, childModelData, templateInfo.ColumnData);

                        foreach (var item in childKeyAndList["list"].ToObject<List<VisualDevModelDataEntity>>())
                        {
                            newTables.Add(item.Data.ToObject<Dictionary<string, object>>());
                        }

                        newDataMap[strKey[i]] = newTables;
                    }
                }
            }

            var entity = new VisualDevModelDataEntity()
            {
                Id = dataMap[mainPrimary].ToString(),
                Data = dataMap.ToJson()
            };
            List<VisualDevModelDataEntity> listEntity = new List<VisualDevModelDataEntity>();
            listEntity.Add(entity);
            //将关键字查询传输的id转换成名称
            Dictionary<string, object> keyAndList = await GetKeyData(templateInfo.FieldsModelList, templateData, null, listEntity, templateInfo.ColumnData, "detail");
            var convData = keyAndList["list"].ToObject<List<VisualDevModelDataEntity>>().FirstOrDefault().Data.ToObject<Dictionary<string, object>>();

            foreach (var entryMap in convData)
            {
                if (entryMap.Value != null)
                {
                    var model = templateInfo.FieldsModelList.Where(m => m.__vModel__.Contains(entryMap.Key.ToString())).FirstOrDefault();
                    if (model != null)
                    {
                        if (entryMap.Key.Equals(model.__vModel__))
                        {
                            newDataMap[entryMap.Key] = entryMap.Value;
                        }
                    }
                    else
                    {
                        model = templateInfo.FieldsModelList.Where(m => m.__vModel__ == entryMap.Key.ToString().Replace("_id", "")).FirstOrDefault();
                        if (model != null)
                        {
                            newDataMap[entryMap.Key] = entryMap.Value;
                        }
                    }
                }
            }

            //处理 条形码 、 二维码 控件
            GetBARAndQR(templateInfo.FieldsModelList, newDataMap, dataMap);

            return newDataMap.ToJson();
        }

        /// <summary>
        /// 获取无表信息详情
        /// </summary>
        /// <param name="templateEntity">模板实体</param>
        /// <param name="data">真实数据</param>
        /// <returns></returns>
        public async Task<string> GetIsNoTableInfoDetails(VisualDevEntity templateEntity, VisualDevModelDataEntity data)
        {
            var formData = TemplateKeywordsHelper.ReplaceKeywords(templateEntity.FormData).ToObject<FormDataModel>();
            List<FieldsModel> modelList = formData.fields;

            //先模板数据缓存 解开子表无限children
            modelList = TemplateCacheDataConversion(modelList);
            Dictionary<string, object> templateData = await GetVisualDevTemplateData(templateEntity.Id, modelList);

            modelList = formData.fields;
            //剔除布局控件
            modelList = TemplateDataConversion(modelList);
            //模板分离子表模板
            var childTemplateList = modelList.FindAll(m => m.__config__.hszKey == "table");
            //删除模板内子表模板
            modelList.RemoveAll(x => x.__config__.hszKey == "table");

            var columnData = TemplateKeywordsHelper.ReplaceKeywords(templateEntity.ColumnData).Deserialize<ColumnDesignModel>();

            //获取真实数据
            var realData = data.Data.ToObject<Dictionary<string, object>>();
            //数据分离子表数据
            var childRealData = realData.Where(p => p.Key.Contains("tableField"));
            //获取关联表单
            var relationFormList = modelList.FindAll(m => m.__config__.hszKey == "relationForm");
            if (relationFormList.Count > 0)
            {
                foreach (var item in relationFormList)
                {
                    realData.Add(item.__vModel__ + "_id", null);
                }
            }
            Dictionary<string, object> newDataMap = new Dictionary<string, object>();
            //循环子表数据
            foreach (var item in childRealData)
            {
                realData.Remove(item.Key);
                List<VisualDevModelDataEntity> childModelData = new List<VisualDevModelDataEntity>();
                var childTemplate = childTemplateList.Find(c => c.__vModel__ == item.Key);
                List<Dictionary<string, object>> tables = item.Value.ToObject<List<Dictionary<string, object>>>();
                List<Dictionary<string, object>> newTables = new List<Dictionary<string, object>>();
                if (childTemplate != null)
                {
                    foreach (var childColumn in tables)
                    {
                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        foreach (KeyValuePair<string, object> column in childColumn)
                        {
                            var child = childTemplate.__config__.children.Find(c => c.__vModel__ == column.Key);
                            if (child != null && column.Value != null)
                            {
                                dic.Add(column.Key, column.Value);
                            }
                        }
                        childModelData.Add(new VisualDevModelDataEntity()
                        {
                            Id = YitIdHelper.NextId().ToString(),
                            Data = dic.ToJson()
                        });
                    }
                    if (childModelData.Count > 0)
                    {
                        //将关键字查询传输的id转换成名称
                        Dictionary<string, object> childKeyAndList = await GetKeyData(childTemplate.__config__.children, templateData, null, childModelData, columnData);

                        foreach (var items in childKeyAndList["list"].ToObject<List<VisualDevModelDataEntity>>())
                        {
                            newTables.Add(items.Data.ToObject<Dictionary<string, object>>());
                        }

                        #region 获取关联表单属性
                        var rFormList = childTemplate.__config__.children.Where(x => x.__config__.hszKey == "relationForm").ToList();
                        foreach (var itemForm in rFormList)
                        {
                            foreach (var dataItem in newTables)
                            {
                                if (!string.IsNullOrWhiteSpace(itemForm.__vModel__) && dataItem.ContainsKey(itemForm.__vModel__) && dataItem[itemForm.__vModel__] != null)
                                {
                                    //获取关联表单id
                                    var relationValueId = dataItem[itemForm.__vModel__ + "_id"].ToString();

                                    //获取 关联表单 转换后的数据
                                    var relationInfo = await _visualDevRepository.GetByIdAsync(itemForm.modelId);
                                    var relationValueStr = "";
                                    if (relationInfo.Tables.IsNullOrWhiteSpace() || relationInfo.Tables.Equals("[]"))
                                    {
                                        var noTableData = await GetInfo(relationValueId);
                                        relationValueStr = await GetIsNoTableInfo(relationInfo, noTableData.Data);
                                    }
                                    else
                                        relationValueStr = await GetHaveTableInfo(relationValueId, relationInfo);

                                    var relationValue = relationValueStr.ToObject<Dictionary<string, object>>();

                                    //添加到 子表 列
                                    childTemplate.__config__.children.Where(x => x.relationField.ReplaceRegex(@"_hszTable_(\w+)", "") == itemForm.__vModel__).ToList().ForEach(citem =>
                                    {
                                        citem.__vModel__ = itemForm.__vModel__ + "_" + citem.showField;
                                        dataItem.Add(itemForm.__vModel__ + "_" + citem.showField, relationValue[citem.showField]);
                                    });
                                }
                            }
                        }

                        if (childTemplate.__config__.children.Where(x => x.__config__.hszKey == "popupAttr").Any())
                        {
                            var pFormList = childTemplate.__config__.children.Where(x => x.__config__.hszKey == "popupSelect").ToList();
                            foreach (var itemForm in pFormList)
                            {
                                var pDataList = await GetPopupSelectDataList(itemForm.interfaceId, childTemplate);//获取接口数据列表
                                foreach (var dataItem in newTables)
                                {
                                    if (!string.IsNullOrWhiteSpace(itemForm.__vModel__) && dataItem.ContainsKey(itemForm.__vModel__) && dataItem[itemForm.__vModel__] != null)
                                    {
                                        //获取关联表单id
                                        var relationValueId = dataItem[itemForm.__vModel__].ToString();

                                        //添加到 子表 列
                                        childTemplate.__config__.children.Where(x => x.relationField.ReplaceRegex(@"_hszTable_(\w+)", "") == itemForm.__vModel__).ToList().ForEach(citem =>
                                        {
                                            var id = dataItem[itemForm.__vModel__].ToString();
                                            citem.__vModel__ = itemForm.__vModel__ + "_" + citem.showField;
                                            var value = pDataList.Where(x => x.Values.Contains(id)).FirstOrDefault();
                                            if (value.Keys.IsNotEmptyOrNull()) dataItem.Add(itemForm.__vModel__ + "_" + citem.showField, value[citem.showField]);
                                        });
                                    }
                                }
                            }
                        }
                        #endregion

                        newDataMap[item.Key] = newTables;
                    }
                }
            }
            var modelData = new VisualDevModelDataEntity()
            {
                Id = data.Id.ToString(),
                Data = realData.ToJson()
            };
            List<VisualDevModelDataEntity> listEntity = new List<VisualDevModelDataEntity>();
            listEntity.Add(modelData);

            //将关键字查询传输的id转换成名称
            Dictionary<string, object> keyAndList = await GetKeyData(modelList, templateData, null, listEntity, columnData);
            var convData = keyAndList["list"].ToObject<List<VisualDevModelDataEntity>>().FirstOrDefault().Data.ToObject<Dictionary<string, object>>();

            foreach (var entryMap in convData)
            {
                if (entryMap.Value != null)
                {
                    var model = modelList.Where(m => m.__vModel__ == entryMap.Key.ToString()).FirstOrDefault();
                    if (model != null)
                    {
                        newDataMap[entryMap.Key] = entryMap.Value;
                    }
                    else
                    {
                        model = modelList.Where(m => m.__vModel__ == entryMap.Key.ToString().Replace("_id", "")).FirstOrDefault();
                        if (model != null)
                        {
                            newDataMap[entryMap.Key] = entryMap.Value;
                        }
                    }
                }
            }

            //处理 条形码 、 二维码 控件
            GetBARAndQR(modelList, newDataMap, realData);

            return newDataMap.ToJson();
        }

        #endregion

        #region Post

        /// <summary>
        /// 创建在线开发功能
        /// </summary>
        /// <param name="templateEntity">功能模板实体</param>
        /// <param name="dataInput">数据输入</param>
        /// <param name="isNewId">是否创建新ID</param>
        /// <returns></returns>
        public async Task Create(VisualDevEntity templateEntity, VisualDevModelDataCrInput dataInput)
        {
            if (!string.IsNullOrEmpty(templateEntity.Tables) && !"[]".Equals(templateEntity.Tables))
            {
                var mainId = YitIdHelper.NextId().ToString();
                var link = await _dbLinkService.GetInfo(templateEntity.DbLinkId);
                if (link == null)
                    link = GetTenantDbLink();
                var haveTableSql = await CreateHaveTableSql(templateEntity, dataInput, mainId);

                try
                {
                    //开启事务
                    _db.BeginTran();

                    //新增功能数据
                    foreach (var item in haveTableSql)
                    {
                        await _databaseService.ExecuteSql(link, item);
                    }

                    if (templateEntity.WebType == 3 && dataInput.status == 0)
                    {
                        var _flowTaskService = App.GetService<IFlowTaskService>();
                        var _flowEngineService = App.GetService<IFlowEngineService>();
                        var FlowTitle = _userManager.User.RealName + "的" + templateEntity.FullName;//流程标题
                        var IsSysTable = false;//流程是否系统表单
                        var eModel = await _flowEngineService.GetInfo(templateEntity.FlowId);
                        if (eModel?.FormType == 1) IsSysTable = true;
                        await _flowTaskService.Submit(null, templateEntity.FlowId, mainId, FlowTitle, 1, null, dataInput.data.Deserialize<JObject>(), 0, 0, IsSysTable, true, dataInput.candidateList);
                    }

                    //关闭事务
                    _db.CommitTran();
                }
                catch (Exception)
                {
                    _db.RollbackTran();
                    throw;
                }
            }
            else
            {
                var allDataMap = dataInput.data.Deserialize<Dictionary<string, object>>();
                var formData = TemplateKeywordsHelper.ReplaceKeywords(templateEntity.FormData).Deserialize<FormDataModel>();
                var fieldsModelList = formData.fields;
                //剔除布局控件
                fieldsModelList = TemplateDataConversion(fieldsModelList);
                // 生成系统自动生成字段
                allDataMap = await GenerateFeilds(fieldsModelList, allDataMap, true);

                VisualDevModelDataEntity entity = new VisualDevModelDataEntity();
                entity.Data = allDataMap.Serialize();
                entity.VisualDevId = templateEntity.Id;
                try
                {
                    //开启事务
                    _db.BeginTran();

                    //新增功能数据
                    var visualDevModelData = await _visualDevModelDataRepository.AsInsertable(entity).CallEntityMethod(m => m.Creator()).ExecuteReturnEntityAsync();

                    if (templateEntity.WebType == 3 && dataInput.status == 0)
                    {
                        var _flowTaskService = App.GetService<IFlowTaskService>();
                        var _flowEngineService = App.GetService<IFlowEngineService>();
                        var FlowTitle = _userManager.User.RealName + "的" + templateEntity.FullName;//流程标题
                        var IsSysTable = false;//流程是否系统表单
                        var eModel = await _flowEngineService.GetInfo(templateEntity.FlowId);
                        if (eModel?.FormType == 1) IsSysTable = true;
                        await _flowTaskService.Submit(null, templateEntity.FlowId, visualDevModelData.Id, FlowTitle, 1, null, dataInput.data.Deserialize<JObject>(), 0, 0, IsSysTable, true, dataInput.candidateList);
                    }

                    //关闭事务
                    _db.CommitTran();
                }
                catch (Exception)
                {
                    _db.RollbackTran();
                    throw;
                }
            }
        }

        /// <summary>
        /// 创建有表SQL
        /// </summary>
        /// <param name="templateEntity"></param>
        /// <param name="dataInput"></param>
        /// <param name="mainId"></param>
        /// <returns></returns>
        public async Task<List<string>> CreateHaveTableSql(VisualDevEntity templateEntity, VisualDevModelDataCrInput dataInput, string mainId)
        {
            var allDataMap = dataInput.data.Deserialize<Dictionary<string, object>>();
            var formData = TemplateKeywordsHelper.ReplaceKeywords(templateEntity.FormData).Deserialize<FormDataModel>();
            var fieldsModelList = formData.fields;
            //剔除布局控件
            fieldsModelList = TemplateDataConversion(fieldsModelList);
            // 生成系统自动生成字段
            allDataMap = await GenerateFeilds(fieldsModelList, allDataMap, true);

            #region 验证模板
            if (fieldsModelList.Count(x => x.__config__.hszKey == "table") > 0)
            {
                var tlist = fieldsModelList.Where(x => x.__config__.hszKey == "table").ToList();//设计子表集合
                var flist = fieldsModelList.Where(x => x.__vModel__.Contains("_hsz_")).ToList();//单控件副表集合

                //处理旧控件 部分没有 tableName
                tlist.Where(x => string.IsNullOrWhiteSpace(x.__config__.tableName)).ToList().ForEach(item =>
                {
                    if (item.__vModel__.Contains("_hsz_")) item.__config__.tableName = item.__vModel__.ReplaceRegex(@"_hsz_(\w+)", "").Replace("hsz_", "");//副表
                });
                flist.Where(x => string.IsNullOrWhiteSpace(x.__config__.tableName)).ToList().ForEach(item =>
                {
                    if (item.__vModel__.Contains("_hsz_")) item.__config__.tableName = item.__vModel__.ReplaceRegex(@"_hsz_(\w+)", "").Replace("hsz_", "");//副表
                });

                tlist.ForEach(item =>
                {
                    var tc = flist.Find(x => x.__vModel__.Contains(item.__config__.tableName + "_hsz_"));

                    if (tc != null) throw HSZException.Oh(ErrorCode.D1401);
                });
            }

            #endregion

            //实例化模板信息
            var tableMapList = templateEntity.Tables.Deserialize<List<TableModel>>();
            //获取主表
            var mainTable = tableMapList.Find(t => t.relationTable == "");
            var tableList = new List<DbTableFieldModel>();
            var link = await _dbLinkService.GetInfo(templateEntity.DbLinkId);
            if (link == null)
                link = GetTenantDbLink();
            var DbType = link?.DbType != null ? link.DbType : _db.CurrentConnectionConfig.DbType.ToString();

            tableList = _databaseService.GetFieldList(link, mainTable.table);

            //辅助表数据名
            var auxiliaryTableDataKey = fieldsModelList.Where(x => x.__vModel__.Contains("_hsz_")).Select(x => x.__vModel__).ToList();

            //先记录子表数据
            var childTableDataKey = allDataMap.Where(d => d.Key.Contains("tableField")).Select(d => d.Key).ToList();
            //记录主表数据
            var mainPrimary = tableList.Find(t => t.primaryKey == 1);
            //记录主表数据
            var mainTableDataKey = allDataMap.Keys.Except(childTableDataKey).Except(auxiliaryTableDataKey).ToList();
            //主表字段集合
            StringBuilder mainFelid = new StringBuilder();
            List<string> mainFelidList = new List<string>();
            //主表查询语句
            List<string> mainSql = new List<string>();
            StringBuilder mainColumn = new StringBuilder();
            var mainValues = new List<string>();

            //拼接主表查询语句
            var mainFields = mainTable.fields;
            foreach (var item in mainTableDataKey)
            {
                var mainField = mainFields.Where(f => f.field == item).FirstOrDefault();
                if (mainField != null)
                {
                    var itemData = allDataMap[item];
                    if (itemData != null && !string.IsNullOrEmpty(itemData.ToString()) && itemData.ToString() != "[]")
                    {
                        //Column部分
                        mainColumn.AppendFormat("{0},", item);
                        //Values部分
                        mainValues.Add(InsertValueHandle(DbType, tableList, item, itemData, fieldsModelList));
                    }
                }
            }
            //去除多余的,
            if (mainTableDataKey.Count > 0)
            {
                mainSql.Add(string.Format(
                    "insert into {0} ({3}{1}) values('{4}'{2});",
                    mainTable.table,
                    (mainColumn.Length > 0 ? "," : "") + mainColumn.ToString().Trim(','),
                    (mainValues.Count > 0 ? "," : "") + string.Join(",", mainValues),
                    mainPrimary.field,
                    mainId));
            }
            else
            {
                mainSql.Add(string.Format("insert into {0} ({1}) values('{2}');", mainTable.table, mainPrimary.field, mainId));
            }

            #region 子表 单控件 集合
            if (auxiliaryTableDataKey.Count > 0)
            {
                var cTableList = fieldsModelList.Where(x => x.__vModel__.Contains("_hsz_")).Select(x => x.__config__.tableName).Distinct().ToList();//获取所有子表

                cTableList.ForEach(tbname => {
                    var tableAllField = _databaseService.GetFieldList(link, tbname);//数据库里获取表的所有字段

                    var tableFieldList = new List<string>();

                    /////剔除空值控件
                    auxiliaryTableDataKey.Where(x => x.Contains("hsz_" + tbname + "_hsz_")).ToList().ForEach(item =>
                    {
                        var itemData = allDataMap.Where(x => x.Key == item).Count() > 0 ? allDataMap[item] : null;
                        if (itemData != null && itemData.ToString() != "") tableFieldList.Add(item);
                    });

                    var fieldList = string.Join(",", tableFieldList.Select(x => x.ReplaceRegex(@"(\w+)_hsz_", "")).ToList());//插入的字段名

                    var valueList = new List<string>();//对应的插入值

                    tableFieldList.ForEach(item => {
                        //前端未填写数据的字段，默认会找不到字段名，需要验证
                        var itemData = allDataMap.Where(x => x.Key == item).Count() > 0 ? allDataMap[item] : null;
                        if (itemData != null)
                            valueList.Add(InsertValueHandle(DbType, tableList, item, itemData, fieldsModelList));//Values部分
                    });

                    if (fieldList.Length > 1)//没有插入数据，只插入主键和外键数据
                    {
                        mainSql.Add(string.Format("insert into {0}({1},{2},{3}) values('{4}','{5}',{6});",
                            tbname,
                            _databaseService.GetFieldList(link, tbname)?.Find(x => x.primaryKey == 1).field,//主键字段名,
                            tableMapList.Find(t => t.table == tbname).tableField,//外键字段名,
                            fieldList,
                            YitIdHelper.NextId().ToString(),
                            mainId,
                            string.Join(",", valueList)));
                    }
                    else
                    {
                        mainSql.Add(string.Format("insert into {0}({1},{2}) values('{3}','{4}');",
                            tbname,
                            _databaseService.GetFieldList(link, tbname)?.Find(x => x.primaryKey == 1).field,//主键字段名,
                            tableMapList.Find(t => t.table == tbname).tableField,//外键字段名,
                            YitIdHelper.NextId().ToString(),
                            mainId));
                    }
                });
            }

            #endregion

            //拼接子表sql
            foreach (var item in childTableDataKey)
            {
                //查找到该控件数据
                var objectData = allDataMap[item];
                var model = objectData.Serialize().Deserialize<List<Dictionary<string, object>>>();
                if (model != null && model.Count > 0)
                {
                    //利用key去找模板
                    var fieldsModel = fieldsModelList.Find(f => f.__vModel__ == item);
                    var fieldsConfig = fieldsModel.__config__;
                    model = GetTableDataListByDic(model, fieldsConfig.children);
                    StringBuilder childColumn = new StringBuilder();
                    var childValues = new List<string>();
                    var childTable = tableMapList.Find(t => t.table == fieldsModel.__config__.tableName);
                    tableList = new List<DbTableFieldModel>();
                    tableList = _databaseService.GetFieldList(link, childTable.table);
                    var childPrimary = tableList.Find(t => t.primaryKey == 1);
                    foreach (var data in model)
                    {
                        if (data.Count > 0)
                        {
                            foreach (KeyValuePair<string, object> child in data)
                            {
                                if (child.Value != null && child.Value.ToString() != "[]" && child.Value.ToString() != "")
                                {
                                    //Column部分
                                    childColumn.AppendFormat("{0},", child.Key);
                                    //Values部分
                                    childValues.Add(InsertValueHandle(DbType, tableList, child.Key, child.Value, fieldsConfig.children));
                                }
                            }
                            if (!string.IsNullOrEmpty(childColumn.ToString()))
                            {
                                mainSql.Add(string.Format(
                                    "insert into {0}({6},{4},{1}) values('{3}','{5}',{2});",
                                    fieldsModel.__config__.tableName,
                                    childColumn.ToString().Trim(','),
                                    string.Join(",", childValues),
                                    YitIdHelper.NextId().ToString(),
                                    childTable.tableField,
                                    mainId,
                                    childPrimary.field));
                            }
                            childColumn = new StringBuilder();
                            childValues = new List<string>();
                        }
                    }
                }
            }

            return mainSql;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="id">修改ID</param>
        /// <param name="visualdevEntity"></param>
        /// <param name="visualdevModelDataUpForm"></param>
        /// <returns></returns>
        public async Task Update(string id, VisualDevEntity templateEntity, VisualDevModelDataUpInput visualdevModelDataUpForm)
        {
            if (!string.IsNullOrEmpty(templateEntity.Tables) && !"[]".Equals(templateEntity.Tables))
            {
                var link = await _dbLinkService.GetInfo(templateEntity.DbLinkId);
                if (link == null)
                    link = GetTenantDbLink();
                var haveTableSql = await UpdateHaveTableSql(templateEntity, visualdevModelDataUpForm, id);

                try
                {
                    //开启事务
                    _db.BeginTran();

                    //修改功能数据
                    foreach (var item in haveTableSql)
                    {
                        await _databaseService.ExecuteSql(link, item);
                    }

                    if (templateEntity.WebType == 3 && visualdevModelDataUpForm.status == 0)
                    {
                        var _flowTaskRepository = App.GetService<IFlowTaskRepository>();
                        var _flowTaskService = App.GetService<IFlowTaskService>();
                        var _flowEngineService = App.GetService<IFlowEngineService>();
                        var taskEntity = await _flowTaskRepository.GetTaskInfo(id);
                        var FlowTitle = _userManager.User.RealName + "的" + templateEntity.FullName;//流程标题
                        var IsSysTable = false;//流程是否系统表单
                        var eModel = await _flowEngineService.GetInfo(templateEntity.FlowId);
                        if (eModel?.FormType == 1) IsSysTable = true;
                        if (taskEntity == null)
                            await _flowTaskService.Submit(null, templateEntity.FlowId, id, FlowTitle, 1, null, visualdevModelDataUpForm.data.Deserialize<JObject>(), 0, 0, IsSysTable, true, visualdevModelDataUpForm.candidateList);
                        else
                            await _flowTaskService.Submit(id, templateEntity.FlowId, id, FlowTitle, 1, null, visualdevModelDataUpForm.data.Deserialize<JObject>(), 0, 0, IsSysTable, true, visualdevModelDataUpForm.candidateList);
                    }

                    //关闭事务
                    _db.CommitTran();
                }
                catch (Exception ex)
                {
                    _db.RollbackTran();
                    throw;
                }
            }
            else
            {
                Dictionary<string, object> allDataMap = visualdevModelDataUpForm.data.ToObject<Dictionary<string, object>>();
                FormDataModel formData = TemplateKeywordsHelper.ReplaceKeywords(templateEntity.FormData).ToObject<FormDataModel>();
                List<FieldsModel> fieldsModelList = formData.fields.ToObject<List<FieldsModel>>();
                fieldsModelList = TemplateDataConversion(fieldsModelList);
                //生成系统自动生成字段
                allDataMap = await GenerateFeilds(fieldsModelList, allDataMap, false);

                //获取旧数据
                var oldEntity = await _visualDevModelDataRepository.GetByIdAsync(id);
                var oldAllDataMap = oldEntity.Data.Deserialize<Dictionary<string, object>>();
                var curr = fieldsModelList.Where(x => x.__config__.hszKey == "currOrganize" || x.__config__.hszKey == "currPosition").Select(x => x.__vModel__).ToList();
                foreach(var item in curr) allDataMap[item] = oldAllDataMap[item];//当前组织和当前岗位不做修改

                VisualDevModelDataEntity entity = new VisualDevModelDataEntity();
                entity.Data = allDataMap.Serialize();
                entity.VisualDevId = templateEntity.Id;
                entity.Id = id;
                try
                {
                    //开启事务
                    _db.BeginTran();

                    //修改功能数据
                    await _visualDevModelDataRepository.AsUpdateable(entity).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();

                    if (templateEntity.WebType == 3 && visualdevModelDataUpForm.status == 0)
                    {
                        var _flowTaskRepository = App.GetService<IFlowTaskRepository>();
                        var _flowTaskService = App.GetService<IFlowTaskService>();
                        var _flowEngineService = App.GetService<IFlowEngineService>();
                        var taskEntity = await _flowTaskRepository.GetTaskInfo(id);
                        var FlowTitle = _userManager.User.RealName + "的" + templateEntity.FullName;//流程标题
                        var IsSysTable = false;//流程是否系统表单
                        var eModel = await _flowEngineService.GetInfo(templateEntity.FlowId);
                        if (eModel?.FormType == 1) IsSysTable = true;
                        if (taskEntity == null)
                            await _flowTaskService.Submit(null, templateEntity.FlowId, id, FlowTitle, 1, null, visualdevModelDataUpForm.data.Deserialize<JObject>(), 0, 0, IsSysTable, true, visualdevModelDataUpForm.candidateList);
                        else
                            await _flowTaskService.Submit(id, templateEntity.FlowId, id, FlowTitle, 1, null, visualdevModelDataUpForm.data.Deserialize<JObject>(), 0, 0, IsSysTable, true, visualdevModelDataUpForm.candidateList);
                    }

                    //关闭事务
                    _db.CommitTran();
                }
                catch (Exception)
                {
                    _db.RollbackTran();
                    throw;
                }
            }
        }

        /// <summary>
        /// 修改有表SQL
        /// </summary>
        /// <param name="templateEntity"></param>
        /// <param name="visualdevModelDataUpForm"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<string>> UpdateHaveTableSql(VisualDevEntity templateEntity, VisualDevModelDataUpInput visualdevModelDataUpForm, string id)
        {
            Dictionary<string, object> allDataMap = visualdevModelDataUpForm.data.ToObject<Dictionary<string, object>>();
            FormDataModel formData = TemplateKeywordsHelper.ReplaceKeywords(templateEntity.FormData).ToObject<FormDataModel>();
            List<FieldsModel> fieldsModelList = formData.fields.ToObject<List<FieldsModel>>();
            fieldsModelList = TemplateDataConversion(fieldsModelList);
            //生成系统自动生成字段
            allDataMap = await GenerateFeilds(fieldsModelList, allDataMap, false);

            #region 验证模板
            if (fieldsModelList.Count(x => x.__config__.hszKey == "table") > 0)
            {
                var tlist = fieldsModelList.Where(x => x.__config__.hszKey == "table").ToList();//设计子表集合
                var flist = fieldsModelList.Where(x => x.__vModel__.Contains("_hsz_")).ToList();//单控件副表集合

                //处理旧控件 部分没有 tableName
                tlist.Where(x => string.IsNullOrWhiteSpace(x.__config__.tableName)).ToList().ForEach(item =>
                {
                    if (item.__vModel__.Contains("_hsz_")) item.__config__.tableName = item.__vModel__.ReplaceRegex(@"_hsz_(\w+)", "").Replace("hsz_", "");//副表
                });
                flist.Where(x => string.IsNullOrWhiteSpace(x.__config__.tableName)).ToList().ForEach(item =>
                {
                    if (item.__vModel__.Contains("_hsz_")) item.__config__.tableName = item.__vModel__.ReplaceRegex(@"_hsz_(\w+)", "").Replace("hsz_", "");//副表
                });

                tlist.ForEach(item =>
                {
                    var tc = flist.Find(x => x.__vModel__.Contains(item.__config__.tableName + "_hsz_"));

                    if (tc != null) throw HSZException.Oh(ErrorCode.D1401);
                });
            }

            #endregion

            List<TableModel> tableMapList = templateEntity.Tables.ToObject<List<TableModel>>();
            //循环表
            var mainTable = tableMapList.Where(t => t.relationTable == "").FirstOrDefault();
            var tableList = new List<DbTableFieldModel>();
            var link = await _dbLinkService.GetInfo(templateEntity.DbLinkId);
            if (link == null)
                link = GetTenantDbLink();
            var DbType = link?.DbType != null ? link.DbType : _db.CurrentConnectionConfig.DbType.ToString();
            tableList = _databaseService.GetFieldList(link, mainTable.table);
            var mainPrimary = tableList.Find(t => t.primaryKey == 1);
            //执行sql语句
            List<string> mainSql = new List<string>();
            mainSql.Add(string.Format("delete from {0} where {2}='{1}';", mainTable.table, id, mainPrimary.field));
            StringBuilder queryMain = new StringBuilder();
            queryMain.AppendFormat("select * from {0} where {2}='{1}';", mainTable.table, id, mainPrimary.field);
            var mainData = _databaseService.GetInterFaceData(link, queryMain.ToString()).Serialize().Deserialize<List<Dictionary<string, object>>>();
            Dictionary<string, object> mainMap = GetTableDataInfo(mainData, fieldsModelList, "update").FirstOrDefault();
            if (tableMapList.Count > 1)
            {
                //去除主表,剩余的为子表，再进行子表删除语句生成
                tableMapList.RemoveAt(0);
                foreach (var tableMap in tableMapList)
                {
                    //主表字段
                    //与主表关联字段
                    string relationField = tableMap.relationField;
                    var relationFieldValue = mainMap.Where(f => f.Key == relationField).FirstOrDefault();
                    //子表字段
                    string tableField = tableMap.tableField;
                    mainSql.Add(string.Format("delete from {0} where {1}='{2}';", tableMap.table, tableField, relationFieldValue.Value));
                }
            }
            string mainId = id;
            //分离主子表数据

            //单控件 数据名称列表
            var auxiliaryTableDataKey = fieldsModelList.Where(x => x.__vModel__.Contains("_hsz_")).Select(x => x.__vModel__).ToList();

            //先记录子表数据
            var childTableDataKey = allDataMap.Where(d => d.Key.Contains("tableField")).Select(d => d.Key).ToList();
            //记录主表数据
            var mainTableDataKey = allDataMap.Keys.Except(childTableDataKey).Except(auxiliaryTableDataKey).ToList();
            //实例化模板信息
            //主表字段集合
            StringBuilder mainFelid = new StringBuilder();
            List<string> mainFelidList = new List<string>();
            StringBuilder mainColumn = new StringBuilder();
            var mainValues = new List<string>();

            #region 处理当前组织和当前岗位 (不做修改)
            //获取当前组织和当前岗位控件
            var curr = fieldsModelList.Where(x => x.__config__.hszKey == "currOrganize" || x.__config__.hszKey == "currPosition").Select(x => x.__vModel__).ToList();
            foreach(var item in curr)
            {
                if (mainMap.ContainsKey(item)) allDataMap[item] = mainMap[item];//当前组织和当前岗位不做修改
            }
            #endregion

            //拼接主表插入语句
            var mainFields = mainTable.fields;
            foreach (var item in mainTableDataKey)
            {
                var mainField = mainFields.Where(f => f.field == item).FirstOrDefault();
                if (mainField != null)
                {
                    var itemData = allDataMap[item];
                    if (itemData != null && !string.IsNullOrEmpty(itemData.ToString()) && !itemData.ToString().Contains("[]"))
                    {
                        //Column部分
                        mainColumn.AppendFormat("{0},", item);
                        //Values部分
                        mainValues.Add(InsertValueHandle(DbType, tableList, item, itemData, fieldsModelList));
                    }
                }
            }
            //去除多余的,
            if (mainTableDataKey.Count > 0)
            {
                mainSql.Add(string.Format(
                    "insert into {0} ({3}{1}) values('{4}'{2});",
                    mainTable.table,
                    (mainColumn.Length > 0 ? "," : "") + mainColumn.ToString().Trim(','),
                    (mainValues.Count > 0 ? "," : "") + string.Join(",", mainValues),
                    mainPrimary.field,
                    mainId));
            }
            else
            {
                mainSql.Add(string.Format("insert into {0} ({1}) values('{2}');", mainTable.table, mainPrimary.field, mainId));
            }

            #region 子表 单控件 集合
            if (auxiliaryTableDataKey.Count > 0)
            {
                var cTableList = fieldsModelList.Where(x => x.__vModel__.Contains("_hsz_")).Select(x => x.__config__.tableName).Distinct().ToList();//获取所有子表

                cTableList.ForEach(tbname => {
                    var tableAllField = _databaseService.GetFieldList(link, tbname);//数据库里获取表的所有字段

                    #region 处理当前组织和当前岗位 (不做修改)
                    if (curr.Any(x => x.Contains("_hsz_")))
                    {
                        var querTableSql = string.Format("select * from {0} where {1}='{2}';", tbname, tableMapList.Find(t => t.table == tbname).tableField, mainId);

                        //获取表数据
                        var tableData = _databaseService.GetInterFaceData(link, querTableSql).Serialize().Deserialize<List<Dictionary<string, object>>>();
                        foreach (var item in curr)
                        {
                            var itemKey = item.ReplaceRegex(@"(\w+)_hsz_", "");
                            if (tableData.Any() && tableData.FirstOrDefault().ContainsKey(itemKey)) allDataMap[item] = tableData.FirstOrDefault()[itemKey];//当前组织和当前岗位不做修改
                        }
                    }
                    #endregion

                    var tableFieldList = new List<string>();

                    /////剔除空值控件
                    auxiliaryTableDataKey.Where(x => x.Contains("hsz_" + tbname + "_hsz_")).ToList().ForEach(item =>
                    {
                        var itemData = allDataMap.Where(x => x.Key == item).Count() > 0 ? allDataMap[item] : null;
                        if (itemData != null && itemData.ToString() != "") tableFieldList.Add(item);
                    });

                    var fieldList = string.Join(",", tableFieldList.Select(x => x.ReplaceRegex(@"(\w+)_hsz_", "")).ToList());//插入的字段名

                    var valueList = new List<string>();//对应的插入值

                    tableFieldList.ForEach(item => {
                        //前端未填写数据的字段，默认会找不到字段名，需要验证
                        var itemData = allDataMap.Where(x => x.Key == item).Count() > 0 ? allDataMap[item] : null;
                        if (itemData != null && itemData.ToString() != "")
                            valueList.Add(InsertValueHandle(DbType, tableList, item, itemData, fieldsModelList));//Values部分
                    });

                    if (fieldList.Length > 1)//没有插入数据，只插入主键和外键数据
                    {
                        mainSql.Add(string.Format("insert into {0}({1},{2},{3}) values('{4}','{5}',{6});",
                            tbname,
                            _databaseService.GetFieldList(link, tbname)?.Find(x => x.primaryKey == 1).field,//主键字段名,
                            tableMapList.Find(t => t.table == tbname).tableField,//外键字段名,
                            fieldList,
                            YitIdHelper.NextId().ToString(),
                            mainId,
                            string.Join(",", valueList)));
                    }
                    else
                    {
                        mainSql.Add(string.Format("insert into {0}({1},{2}) values('{3}','{4}');",
                            tbname,
                            _databaseService.GetFieldList(link, tbname)?.Find(x => x.primaryKey == 1).field,//主键字段名,
                            tableMapList.Find(t => t.table == tbname).tableField,//外键字段名,
                            YitIdHelper.NextId().ToString(),
                            mainId));
                    }
                });
            }

            #endregion

            //拼接子表sql
            foreach (var item in childTableDataKey)
            {
                //查找到该控件数据
                var model = allDataMap[item].ToJson().ToList<Dictionary<string, object>>();
                if (model != null && model.Count > 0)
                {
                    //利用key去找模板
                    var fieldsModel = fieldsModelList.Find(f => f.__vModel__ == item);
                    var fieldsConfig = fieldsModel.__config__;
                    model = GetTableDataListByDic(model, fieldsConfig.children);
                    StringBuilder childColumn = new StringBuilder();
                    var childValues = new List<string>();
                    var childTable = tableMapList.Where(t => t.table == fieldsConfig.tableName).FirstOrDefault();
                    tableList = new List<DbTableFieldModel>();
                    tableList = _databaseService.GetFieldList(link, childTable.table);
                    var childPrimary = tableList.Find(t => t.primaryKey == 1);
                    foreach (var data in model)
                    {
                        if (data.Count > 0)
                        {
                            #region 处理当前组织和当前岗位 (不做修改)
                            //获取当前组织和当前岗位控件
                            curr = fieldsConfig.children.Where(x => x.__config__.hszKey == "currOrganize" || x.__config__.hszKey == "currPosition").Select(x => x.__vModel__).ToList();
                            if (curr.Any())
                            {
                                var querTableSql = string.Format("select * from {0} where {1}='{2}';", fieldsModel.__config__.tableName, childTable.tableField, mainId);

                                //获取表数据
                                var tableData = _databaseService.GetInterFaceData(link, querTableSql).Serialize().Deserialize<List<Dictionary<string, object>>>();
                                foreach (var it in curr)
                                {
                                    if (tableData.Any() && tableData.FirstOrDefault().ContainsKey(it)) data[it] = tableData.FirstOrDefault()[it];//当前组织和当前岗位不做修改
                                }
                            }
                            #endregion

                            foreach (KeyValuePair<string, object> child in data)
                            {
                                if (!string.IsNullOrWhiteSpace(child.Key) && child.Value != null)
                                {
                                    //Column部分
                                    childColumn.AppendFormat("{0},", child.Key);
                                    //Values部分
                                    childValues.Add(InsertValueHandle(DbType, tableList, child.Key, child.Value, fieldsConfig.children));
                                }
                            }
                            if (!string.IsNullOrEmpty(childColumn.ToString()))
                            {
                                mainSql.Add(string.Format(
                                    "insert into {0}({6},{4},{1}) values('{3}','{5}',{2});",
                                    fieldsModel.__config__.tableName,
                                    childColumn.ToString().Trim(','),
                                    string.Join(",", childValues),
                                    YitIdHelper.NextId().ToString(),
                                    childTable.tableField,
                                    mainId,
                                    childPrimary.field));
                            }
                            childColumn = new StringBuilder();
                            childValues = new List<string>();
                        }
                    }
                }
            }
            return mainSql;
        }

        #endregion

        #region PrivateMethod

        /// <summary>
        /// 处理 Sql 插入数据的 value
        /// </summary>
        /// <param name="dbType">数据库 类型</param>
        /// <param name="tableList">表数据</param>
        /// <param name="field">前端字段名</param>
        /// <param name="data">插入的数据</param>
        /// <param name="_fieldsModelList"></param>
        /// <returns></returns>
        private string InsertValueHandle(string dbType, List<DbTableFieldModel> _tableList, string field, object data, List<FieldsModel> _fieldsModelList)
        {
            //单独处理 Oracle,Kdbndp 日期格式转换
            //if ((dbType == "Oracle" || dbType == "Kdbndp") && _tableList.Find(x => x.dataType == "DateTime" && x.field == field.ReplaceRegex(@"(\w+)_hsz_", "")) != null)
            if (dbType == "Oracle" && _tableList.Find(x => x.dataType == "DateTime" && x.field == field.ReplaceRegex(@"(\w+)_hsz_", "")) != null)
                return string.Format("to_date('{0}','yyyy-mm-dd HH24/MI/SS')", TransformDataObject(data, field, _fieldsModelList, "create"));
            else
                return string.Format("'{0}'", TransformDataObject(data, field, _fieldsModelList, "create"));
        }

        /// <summary>
        /// 获取多租户Link
        /// </summary>
        /// <returns></returns>
        public DbLinkEntity GetTenantDbLink()
        {
            return new DbLinkEntity
            {
                Id = _userManager.TenantId,
                ServiceName = _userManager.TenantDbName,
                DbType = App.Configuration["ConnectionStrings:DBType"],
                Host = App.Configuration["ConnectionStrings:Host"],
                Port = App.Configuration["ConnectionStrings:Port"].ToInt(),
                UserName = App.Configuration["ConnectionStrings:UserName"],
                Password = App.Configuration["ConnectionStrings:Password"]
            };
        }

        #region 拆解模板

        /// <summary>
        /// 模板缓存数据转换
        /// 专门为模板缓存数据，会将子表内的控件全部获取出来
        /// 适用场景缓存模板数据
        /// </summary>
        /// <returns></returns>
        private List<FieldsModel> TemplateCacheDataConversion(List<FieldsModel> fieldsModelList)
        {
            var template = new List<FieldsModel>();
            //将模板内的无限children解析出来
            //包含子表children
            foreach (var item in fieldsModelList)
            {
                var config = item.__config__;
                switch (config.hszKey)
                {
                    //栅格布局
                    case "row":
                        {
                            template.AddRange(TemplateCacheDataConversion(config.children));
                        }
                        break;
                    //表格
                    case "table":
                        {
                            template.AddRange(TemplateCacheDataConversion(config.children));
                        }
                        break;
                    //卡片
                    case "card":
                        {
                            template.AddRange(TemplateCacheDataConversion(config.children));
                        }
                        break;
                    //折叠面板
                    case "collapse":
                        {
                            foreach (var collapse in config.children)
                            {
                                template.AddRange(TemplateCacheDataConversion(collapse.__config__.children));
                            }
                        }
                        break;
                    //tab标签
                    case "tab":
                        {
                            foreach (var collapse in config.children)
                            {
                                template.AddRange(TemplateCacheDataConversion(collapse.__config__.children));
                            }
                        }
                        break;
                    //文本
                    case "HSZText":
                        break;
                    //分割线
                    case "divider":
                        break;
                    //分组标题
                    case "groupTitle":
                        break;
                    //按钮
                    case "button":
                        break;
                    default:
                        {
                            template.Add(item);
                        }
                        break;
                }
            }
            return template;
        }

        #endregion

        #region 解析模板数据

        /// <summary>
        /// 控制模板数据转换
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="fieldsModel">数据模板</param>
        /// <param name="actionType">操作类型(List-列表值,create-创建值,update-更新值,detail-详情值,transition-过渡值,query-查询)</param>
        /// <returns></returns>
        private object TemplateControlsDataConversion(object data, FieldsModel fieldsModel, string actionType = null)
        {
            if (data == null || string.IsNullOrEmpty(data.ToString())) return "";

            try
            {
                object conversionData = new object();
                switch (fieldsModel.__config__.hszKey)
                {
                    #region 基础控件

                    //单行输入
                    case "comInput":
                        {
                            conversionData = string.IsNullOrEmpty(data.ToString()) ? null : data.ToString();
                        }
                        break;
                    //多行输入
                    case "textarea":
                        {
                            conversionData = data.ToString();
                        }
                        break;
                    //数字输入
                    case "numInputc":
                        {
                            conversionData = data.ToInt();
                        }
                        break;
                    //金额输入
                    case "HSZAmount":
                        {
                            conversionData = data.ToDecimal();
                        }
                        break;
                    //单选框组
                    case "radio":
                        {
                            conversionData = string.IsNullOrEmpty(data.ToString()) ? null : data.ToString();
                        }
                        break;
                    //多选框组
                    case "checkbox":
                        {
                            if (data.ToString().Contains("["))
                                conversionData = data.ToString().ToObject<List<string>>();
                            else
                                conversionData = data.ToString();
                        }
                        break;
                    //下拉选择
                    case "select":
                        {
                            switch (actionType)
                            {
                                case "transition":
                                    {
                                        conversionData = data;
                                    }
                                    break;
                                case "List":
                                    {
                                        if (data.ToString().Contains("["))
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else if (data.ToString().Contains(","))
                                        {
                                            conversionData = string.Join(",", data.ToString().Split(',').ToArray());
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                default:
                                    {
                                        if (fieldsModel.multiple && actionType != "query")
                                        {
                                            if (data.ToString().Contains("["))
                                            {
                                                conversionData = data.ToString().ToObject<List<string>>();
                                            }
                                            else if (data.ToString().Contains(","))
                                            {
                                                conversionData = string.Join(",", data.ToString().Split(',').ToArray());
                                            }
                                            else
                                            {
                                                conversionData = data.ToString();
                                            }
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    //时间选择
                    case "time":
                        {
                            switch (actionType)
                            {
                                case "List":
                                    DateTime dtDate;
                                    if (DateTime.TryParse(data.ToString(), out dtDate))
                                        conversionData = string.Format("{0:HH:mm:ss} ", data.ToString());
                                    else
                                        conversionData = string.Format("{0:HH:mm:ss} ", Ext.GetDateTime(data.ToString()));
                                    break;
                                case "create":
                                    var date = string.Format("{0:yyyy-MM-dd}", DateTime.MaxValue);
                                    conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss}", date + " " + data);
                                    break;
                                case "update":
                                    var udate = string.Format("{0:yyyy-MM-dd}", DateTime.MaxValue);
                                    conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss}", udate + " " + data);
                                    try
                                    {
                                        DateTime.Parse(conversionData.ToString());
                                    }
                                    catch
                                    {
                                        conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss} ", data.ToString()).ToDate().ToString("yyyy-MM-dd HH:mm:ss");
                                    }
                                    break;
                                case "detail":
                                    try
                                    {
                                        DateTime.Parse(data.ToString());
                                        conversionData = string.Format("{0:HH:mm:ss} ", data.ToString());
                                    }
                                    catch
                                    {
                                        conversionData = string.Format("{0:HH:mm:ss} ", Ext.GetDateTime(data.ToString()));
                                    }
                                    break;
                                default:
                                    conversionData = data;
                                    break;
                            }
                        }
                        break;
                    //时间范围
                    case "timeRange":
                        {
                            switch (actionType)
                            {
                                case "List":
                                    conversionData = data.ToString().ToObject<List<string>>();
                                    break;
                                case "create":
                                    conversionData = data.ToString().ToObject<List<string>>();
                                    break;
                                case "transition":
                                    conversionData = data;
                                    break;
                                case "update":
                                    conversionData = data.ToString().ToObject<List<string>>();
                                    break;
                            }
                        }
                        break;
                    //日期选择
                    case "date":
                        {
                            switch (actionType)
                            {
                                case "List":
                                    DateTime dtDate;
                                    if (DateTime.TryParse(data.ToString(), out dtDate))
                                        conversionData = data.ToString();
                                    else
                                        conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss}", Ext.GetDateTime(data.ToString()));
                                    break;
                                case "create":
                                    conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss}", Ext.GetDateTime(data.ToString()));
                                    break;
                                case "detail":
                                    conversionData = data;
                                    break;
                                case "update":
                                    try
                                    {
                                        DateTime.Parse(data.ToString());
                                        conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss} ", data.ToString());
                                    }
                                    catch
                                    {
                                        conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss} ", Ext.GetDateTime(data.ToString()));
                                    }
                                    //conversionData = data;
                                    break;
                                default:
                                    try
                                    {
                                        DateTime.Parse(data.ToString());
                                        conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss} ", data.ToString());
                                    }
                                    catch
                                    {
                                        conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss} ", Ext.GetDateTime(data.ToString()));
                                    }
                                    break;
                            }
                        }
                        break;
                    //日期范围
                    case "dateRange":
                        {
                            switch (actionType)
                            {
                                case "List":
                                    conversionData = data.ToString().ToObject<List<string>>();
                                    break;
                                case "transition":
                                    conversionData = data;
                                    break;
                                case "create":
                                    conversionData = data.ToString().ToObject<List<string>>();
                                    break;
                                case "update":
                                    conversionData = data.ToString().ToObject<List<string>>();
                                    break;
                            }
                        }
                        break;
                    //创建时间
                    case "createTime":
                        {
                            switch (actionType)
                            {
                                case "create":
                                    conversionData = data;
                                    break;
                                default:
                                    DateTime dtDate;
                                    if (DateTime.TryParse(data.ToString(), out dtDate))
                                        conversionData = data.ToString();
                                    else
                                        conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss}", Ext.GetDateTime(data.ToString()));
                                    break;
                            }
                        }
                        break;
                    //修改时间
                    case "modifyTime":
                        {
                            switch (actionType)
                            {
                                case "create":
                                    conversionData = data.ToString();
                                    break;
                                default:
                                    DateTime dtDate;
                                    if (DateTime.TryParse(data.ToString(), out dtDate))
                                        conversionData = data.ToString();
                                    else
                                        conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss}", Ext.GetDateTime(data.ToString()));
                                    break;
                            }
                        }
                        break;
                    //文件上传
                    case "uploadFz":
                        {
                            if (data.ToString() != "[]")
                            {
                                if (data is List<FileControlsModel>)
                                    conversionData = data.ToJson().ToObject<List<FileControlsModel>>();
                                else
                                    conversionData = data.ToString().ToObject<List<FileControlsModel>>();
                            }
                            else
                            {
                                conversionData = null;
                            }
                        }
                        break;
                    //图片上传
                    case "uploadImg":
                        {
                            if (data.ToString() != "[]")
                            {
                                conversionData = data.ToString().ToObject<List<FileControlsModel>>();
                            }
                            else
                            {
                                conversionData = null;
                            }
                        }
                        break;
                    //颜色选择
                    case "colorPicker":
                        {
                            conversionData = string.IsNullOrEmpty(data.ToString()) ? null : data.ToString();
                        }
                        break;
                    //评分
                    case "rate":
                        {
                            conversionData = data.ToInt();
                        }
                        break;
                    //开关
                    case "switch":
                        {
                            conversionData = data.ToInt();
                        }
                        break;
                    //滑块
                    case "slider":
                        {
                            if (fieldsModel.range)
                            {
                                conversionData = data.ToString().ToObject<List<int>>();
                            }
                            else
                            {
                                conversionData = data.ToInt();
                            };
                        }
                        break;
                    ////文本
                    //case "HSZText":
                    //    break;
                    //富文本
                    case "editor":
                        {
                            conversionData = string.IsNullOrEmpty(data.ToString()) ? null : data.ToString();
                        }
                        break;
                    //单据组件
                    case "billRule":
                        {
                            conversionData = data.ToString();
                        }
                        break;
                    //省市区联动
                    case "address":
                        {
                            switch (actionType)
                            {
                                case "transition":
                                    {
                                        conversionData = data;
                                    }
                                    break;
                                default:
                                    conversionData = data.ToString().ToObject<List<object>>();
                                    break;
                            }
                        }
                        break;
                    //创建人员
                    case "createUser":
                        {
                            conversionData = data.ToString();
                        }
                        break;
                    //修改人员
                    case "modifyUser":
                        {
                            conversionData = string.IsNullOrEmpty(data.ToString()) ? null : data.ToString();
                        }
                        break;
                    //所属组织
                    case "currOrganize":
                        {
                            conversionData = data.ToString();
                        }
                        break;
                    //所属部门
                    case "currDept":
                        {
                            conversionData = data.ToString();
                        }
                        break;
                    //所属岗位
                    case "currPosition":
                        {
                            conversionData = data.ToString();
                        }
                        break;
                    case "table":
                        {

                        }
                        break;
                    //级联
                    case "cascader":
                        switch (actionType)
                        {
                            case "transition":
                                {
                                    conversionData = data;
                                }
                                break;
                            default:
                                {
                                    if (fieldsModel.props.props.multiple)
                                        conversionData = data.ToString().ToObject<List<object>>();
                                    else if (data.ToString().Contains("["))
                                        conversionData = data.ToString().ToObject<List<object>>();
                                    else
                                        conversionData = data;
                                }
                                break;
                        }
                        break;
                    default:
                        conversionData = data.ToString();
                        break;

                    #endregion

                    #region 高级控件

                    //公司组件
                    case "comSelect":
                        {
                            switch (actionType)
                            {
                                case "List":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<List<object>>>();
                                        }
                                        else
                                        {
                                            conversionData = data;
                                        }
                                    }
                                    break;
                                case "create":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            //conversionData = data.CastTo("").Split(",", true).ToList();
                                            conversionData = data.ToString().ToObject<List<List<object>>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                    }
                                    break;
                                case "update":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            //var list = data.ToString().ToObject<List<string>>();
                                            conversionData = data.ToString().ToObject<List<List<object>>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                    }
                                    break;
                                case "detail":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            //conversionData = data.ToString().ToObject<List<string>>();
                                            conversionData = data.ToString().ToObject<List<List<string>>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                    }
                                    break;
                                case "transition":
                                    {
                                        conversionData = data;
                                    }
                                    break;
                                default:
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<List<string>>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    //部门组件
                    case "depSelect":
                        {
                            switch (actionType)
                            {
                                case "List":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                            //conversionData = data.CastTo("").Split(",", true).ToList();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "create":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            //conversionData = data.CastTo("").Split(",", true).ToList();
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "update":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            //var list = data.ToString().ToObject<List<string>>();
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "detail":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            //conversionData = data.ToString().ToObject<List<string>>();
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "transition":
                                    {
                                        conversionData = data;
                                    }
                                    break;
                                default:
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    //岗位组件
                    case "posSelect":
                        {
                            switch (actionType)
                            {
                                case "List":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                            //conversionData = data.CastTo("").Split(",", true).ToList();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "create":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            //conversionData = data.CastTo("").Split(",", true).ToList();
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "update":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            //var list = data.ToString().ToObject<List<string>>();
                                            //conversionData = string.Join(",", list.ToArray());
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "detail":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            //conversionData = data.ToString().ToObject<List<string>>();
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "transition":
                                    {
                                        conversionData = data;
                                    }
                                    break;
                                default:
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    //用户组件
                    case "userSelect":
                        {
                            switch (actionType)
                            {
                                case "List":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                            //conversionData = data.CastTo("").Split(",", true).ToList();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "create":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            //conversionData = data.CastTo("").Split(",", true).ToList();
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "update":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            //var list = data.ToString().ToObject<List<string>>();
                                            //conversionData = string.Join(",", list.ToArray());
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "detail":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                            //conversionData = data.ToString();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "transition":
                                    {
                                        conversionData = data;
                                    }
                                    break;
                                default:
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    //树形选择
                    case "treeSelect":
                        {
                            switch (actionType)
                            {
                                case "List":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "create":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            //conversionData = data.CastTo("").Split(",", true).ToList();
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "update":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            //var list = data.ToString().ToObject<List<string>>();
                                            //conversionData = string.Join(",", list.ToArray());
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "detail":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "transition":
                                    {
                                        conversionData = data;
                                    }
                                    break;
                                default:
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    //弹窗选择
                    case "popupSelect":
                        {
                            conversionData = data.ToString();
                        }
                        break;
                        #endregion

                }
                return conversionData;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 转换对应数据Object
        /// </summary>
        /// <param name="dataValue">数据值</param>
        /// <param name="dataKey">数据Key</param>
        /// <param name="fieldsModelList">模板</param>
        /// <param name="actionType">操作类型(List-列表值,create-创建值,detail-详情值,transition-过渡值)</param>
        /// <returns></returns>
        private string TransformDataObject(object dataValue, string dataKey, List<FieldsModel> fieldsModelList, string actionType = null)
        {
            StringBuilder sb = new StringBuilder();
            //根据KEY查找模板
            var model = fieldsModelList.Find(f => f.__vModel__ == dataKey);
            if (model != null)
            {
                switch (model.__config__.hszKey)
                {
                    //文件上传
                    case "uploadFz":
                        {
                            var fileList = TemplateControlsDataConversion(dataValue, model);
                            if (fileList != null)
                            {
                                sb.AppendFormat("{0}", fileList.Serialize());
                            }
                        }
                        break;
                    //图片上传
                    case "uploadImg":
                        {
                            var fileList = TemplateControlsDataConversion(dataValue, model);
                            if (fileList != null)
                            {
                                sb.AppendFormat("{0}", fileList.Serialize());
                            }
                        }
                        break;
                    //省市区联动
                    case "address":
                        {
                            sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model).Serialize());
                        }
                        break;
                    //树形选择
                    case "treeSelect":
                        {
                            if (model.multiple)
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model, actionType).Serialize());
                            }
                            else
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model));
                            }
                        }
                        break;
                    //公司组件
                    case "comSelect":
                        {
                            if (model.multiple)
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model, actionType).Serialize());
                            }
                            else
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model).Serialize());
                            }
                        }
                        break;
                    //部门组件
                    case "depSelect":
                        {
                            if (model.multiple)
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model, actionType).Serialize());
                            }
                            else
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model));
                            }
                        }
                        break;
                    //岗位组件
                    case "posSelect":
                        {
                            if (model.multiple)
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model, actionType).Serialize());
                            }
                            else
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model));
                            }
                        }
                        break;
                    //用户组件
                    case "userSelect":
                        {
                            if (model.multiple)
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model, actionType).Serialize());
                            }
                            else
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model));
                            }
                        }
                        break;
                    //滑块
                    case "slider":
                        {
                            if (model.multiple)
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model, actionType).Serialize());
                            }
                            else
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model));
                            }
                        }
                        break;
                    //时间范围
                    case "timeRange":
                        {
                            sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model, actionType).Serialize());
                        }
                        break;
                    //日期范围
                    case "dateRange":
                        {
                            sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model, actionType).Serialize());
                        }
                        break;
                    //下拉选择
                    case "select":
                        {
                            if (model.multiple)
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model, actionType).Serialize());
                            }
                            else
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model));
                            }
                        }
                        break;
                    //复选框
                    case "checkbox":
                        {
                            sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model).Serialize());
                        }
                        break;
                    //级联
                    case "cascader":
                        {
                            sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model).Serialize());
                        }
                        break;
                    //日期选择
                    case "date":
                        {
                            try
                            {
                                DateTime.Parse(dataValue.ToString());
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(Ext.ConvertToTimeStamp(Convert.ToDateTime(dataValue)), model, actionType));
                            }
                            catch
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model, actionType));
                            }
                        }
                        break;
                    default:
                        sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model, actionType));
                        break;
                }
            }
            if (dataKey == "F_Id")
            {
                sb.AppendFormat("{0}", dataValue.ToString());
            }
            return sb.ToString().TrimStart('"').TrimEnd('"');
        }

        #endregion

        #region 缓存模板数据

        /// <summary>
        /// 获取可视化开发模板可缓存数据
        /// </summary>
        /// <param name="moldelId">模型id</param>
        /// <param name="formData">模板数据结构</param>
        /// <returns></returns>
        private async Task<Dictionary<string, object>> GetVisualDevTemplateData(string moldelId, List<FieldsModel> formData)
        {
            Dictionary<string, object> templateData = new Dictionary<string, object>();
            var cacheKey = CommonConst.VISUALDEV + _userManager.TenantId + "_" + moldelId;
            if (_cacheManager.Exists(cacheKey))
            {
                templateData = _cacheManager.Get(cacheKey).Deserialize<Dictionary<string, object>>();
            }
            else
            {
                #region 远端数据 配置参数
                var parameter = new List<SugarParameter>();
                if (_userManager.ToKen != null)
                {
                    parameter.Add(new SugarParameter("@user", _userManager.UserId));
                    parameter.Add(new SugarParameter("@organize", _userManager.User.OrganizeId));
                    parameter.Add(new SugarParameter("@department", _userManager.User.OrganizeId));
                    parameter.Add(new SugarParameter("@postion", _userManager.User.PositionId));
                }
                #endregion
                foreach (var model in formData)
                {
                    if (model != null && model.__vModel__ != null)
                    {
                        ConfigModel configModel = model.__config__;
                        string fieldName1 = configModel.renderKey + "_" + model.__vModel__;
                        string type = configModel.hszKey;
                        switch (type)
                        {
                            //单选框
                            case HszKeyConst.RADIO:
                                {
                                    if (vModelType.DICTIONARY.GetDescription() == configModel.dataType)
                                    {
                                        var dictionaryDataEntityList = string.IsNullOrEmpty(configModel.dictionaryType) ? null : await _dictionaryDataService.GetList(configModel.dictionaryType);
                                        List<Dictionary<string, string>> dictionaryDataList = new List<Dictionary<string, string>>();
                                        foreach (var item in dictionaryDataEntityList)
                                        {
                                            Dictionary<string, string> dictionary = new Dictionary<string, string>();
                                            dictionary.Add(item.Id, item.FullName);
                                            dictionary.Add(item.EnCode, item.FullName);
                                            dictionaryDataList.Add(dictionary);
                                        }
                                        templateData.Add(fieldName1, dictionaryDataList);
                                    }
                                    if (vModelType.STATIC.GetDescription() == configModel.dataType)
                                    {
                                        var optionList = model.__slot__.options;
                                        List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                                        foreach (var item in optionList)
                                        {
                                            Dictionary<string, string> option = new Dictionary<string, string>();
                                            option.Add(item[model.__config__.props.value].ToString(), item[model.__config__.props.label].ToString());
                                            list.Add(option);
                                        }
                                        templateData.Add(fieldName1, list);
                                    }
                                    if (vModelType.DYNAMIC.GetDescription() == configModel.dataType)
                                    {
                                        //获取远端数据
                                        var dynamic = await _dataInterfaceService.GetInfo(model.__config__.propsUrl);
                                        if (dynamic != null && 1.Equals(dynamic.DataType))
                                        {
                                            var linkEntity = await _dbLinkService.GetInfo(dynamic.DBLinkId);
                                            if (linkEntity == null) linkEntity = GetTenantDbLink();
                                            _dataInterfaceService.ReplaceParameterValue(dynamic, new Dictionary<string, string>());
                                            var dt = _databaseService.GetInterFaceData(linkEntity, dynamic.Query, parameter.ToArray());
                                            List<Dictionary<string, object>> dynamicDataList = dt.Serialize().Deserialize<List<Dictionary<string, object>>>();
                                            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                                            foreach (var item in dynamicDataList)
                                            {
                                                Dictionary<string, string> dynamicDic = new Dictionary<string, string>();
                                                dynamicDic.Add(item[model.__config__.props.value].ToString(), item[model.__config__.props.label].ToString());
                                                list.Add(dynamicDic);
                                            }
                                            templateData.Add(fieldName1, list);
                                        }
                                    }
                                }
                                break;
                            //下拉框
                            case HszKeyConst.SELECT:
                                {
                                    if (vModelType.DICTIONARY.GetDescription() == configModel.dataType)
                                    {
                                        List<DictionaryDataEntity> dictionaryDataEntityList = string.IsNullOrEmpty(configModel.dictionaryType) ? null : await _dictionaryDataService.GetList(configModel.dictionaryType);
                                        List<Dictionary<string, string>> dictionaryDataList = new List<Dictionary<string, string>>();
                                        foreach (var item in dictionaryDataEntityList)
                                        {
                                            Dictionary<string, string> dictionary = new Dictionary<string, string>();
                                            dictionary.Add(item.Id, item.FullName);
                                            dictionary.Add(item.EnCode, item.FullName);
                                            dictionaryDataList.Add(dictionary);
                                        }
                                        templateData.Add(fieldName1, dictionaryDataList);
                                    }
                                    else if (vModelType.STATIC.GetDescription() == configModel.dataType)
                                    {
                                        var optionList = model.__slot__.options;
                                        List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                                        foreach (var item in optionList)
                                        {
                                            Dictionary<string, string> option = new Dictionary<string, string>();
                                            option.Add(item[model.__config__.props.value].ToString(), item[model.__config__.props.label].ToString());
                                            list.Add(option);
                                        }
                                        templateData.Add(fieldName1, list);
                                    }
                                    else if (vModelType.DYNAMIC.GetDescription() == configModel.dataType)
                                    {
                                        //获取远端数据
                                        DataInterfaceEntity dynamic = await _dataInterfaceService.GetInfo(model.__config__.propsUrl);

                                        if (dynamic != null && 1.Equals(dynamic.DataType))
                                        {
                                            var linkEntity = await _dbLinkService.GetInfo(dynamic.DBLinkId);
                                            if (linkEntity == null) linkEntity = GetTenantDbLink();
                                            _dataInterfaceService.ReplaceParameterValue(dynamic, new Dictionary<string, string>());
                                            var dt = _databaseService.GetInterFaceData(linkEntity, dynamic.Query, parameter.ToArray());
                                            List<Dictionary<string, object>> dynamicDataList = dt.Serialize().Deserialize<List<Dictionary<string, object>>>();
                                            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                                            foreach (var item in dynamicDataList)
                                            {
                                                Dictionary<string, string> dynamicDic = new Dictionary<string, string>();
                                                dynamicDic.Add(item[model.__config__.props.value].ToString(), item[model.__config__.props.label].ToString());
                                                list.Add(dynamicDic);
                                            }
                                            templateData.Add(fieldName1, list);
                                        }
                                    }
                                }
                                break;
                            //复选框
                            case HszKeyConst.CHECKBOX:
                                {
                                    if (vModelType.DICTIONARY.GetDescription() == configModel.dataType)
                                    {
                                        List<DictionaryDataEntity> dictionaryDataEntityList = string.IsNullOrEmpty(configModel.dictionaryType) ? null : await _dictionaryDataService.GetList(configModel.dictionaryType);
                                        List<Dictionary<string, string>> dictionaryDataList = new List<Dictionary<string, string>>();
                                        foreach (var item in dictionaryDataEntityList)
                                        {
                                            Dictionary<string, string> dictionary = new Dictionary<string, string>();
                                            dictionary.Add(item.Id, item.FullName);
                                            dictionary.Add(item.EnCode, item.FullName);
                                            dictionaryDataList.Add(dictionary);
                                        }
                                        templateData.Add(fieldName1, dictionaryDataList);
                                    }
                                    if (vModelType.STATIC.GetDescription() == configModel.dataType)
                                    {
                                        var optionList = model.__slot__.options;
                                        List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                                        foreach (var item in optionList)
                                        {
                                            Dictionary<string, string> option = new Dictionary<string, string>();
                                            option.Add(item[model.__config__.props.value].ToString(), item[model.__config__.props.label].ToString());
                                            list.Add(option);
                                        }
                                        templateData.Add(fieldName1, list);
                                    }
                                    if (vModelType.DYNAMIC.GetDescription() == configModel.dataType)
                                    {
                                        //获取远端数据
                                        DataInterfaceEntity dynamic = await _dataInterfaceService.GetInfo(model.__config__.propsUrl);
                                        if (dynamic != null && 1.Equals(dynamic.DataType))
                                        {
                                            var linkEntity = await _dbLinkService.GetInfo(dynamic.DBLinkId);
                                            if (linkEntity == null) linkEntity = GetTenantDbLink();
                                            _dataInterfaceService.ReplaceParameterValue(dynamic, new Dictionary<string, string>());
                                            var dt = _databaseService.GetInterFaceData(linkEntity, dynamic.Query, parameter.ToArray());
                                            List<Dictionary<string, object>> dynamicDataList = dt.Serialize().Deserialize<List<Dictionary<string, object>>>();
                                            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                                            foreach (var item in dynamicDataList)
                                            {
                                                Dictionary<string, string> dynamicDic = new Dictionary<string, string>();
                                                dynamicDic.Add(item[model.__config__.props.value].ToString(), item[model.__config__.props.label].ToString());
                                                list.Add(dynamicDic);
                                            }
                                            templateData.Add(fieldName1, list);
                                        }
                                    }
                                }
                                break;
                            //树形选择
                            case HszKeyConst.TREESELECT:
                                {
                                    if (vModelType.DICTIONARY.GetDescription() == configModel.dataType)
                                    {
                                        List<DictionaryDataEntity> dictionaryDataEntityList = await _dictionaryDataService.GetList();
                                        List<Dictionary<string, string>> dictionaryDataList = new List<Dictionary<string, string>>();
                                        foreach (var item in dictionaryDataEntityList)
                                        {
                                            Dictionary<string, string> dictionary = new Dictionary<string, string>();
                                            dictionary.Add(item.Id, item.FullName);
                                            dictionary.Add(item.EnCode, item.FullName);
                                            dictionaryDataList.Add(dictionary);
                                        }
                                        templateData.Add(fieldName1, dictionaryDataList);
                                    }
                                    else if (vModelType.STATIC.GetDescription() == configModel.dataType)
                                    {
                                        var props = model.props.props;
                                        var optionList = GetTreeOptions(model.options, props);
                                        List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                                        foreach (var item in optionList)
                                        {
                                            Dictionary<string, string> option = new Dictionary<string, string>();
                                            option.Add(item.value, item.label);
                                            list.Add(option);
                                        }
                                        templateData.Add(fieldName1, list);
                                    }
                                    else if (vModelType.DYNAMIC.GetDescription() == configModel.dataType)
                                    {
                                        //获取远端数据
                                        DataInterfaceEntity dynamic = await _dataInterfaceService.GetInfo(model.__config__.propsUrl);
                                        if (dynamic != null && 1.Equals(dynamic.DataType))
                                        {
                                            var linkEntity = await _dbLinkService.GetInfo(dynamic.DBLinkId);
                                            if (linkEntity == null) linkEntity = GetTenantDbLink();
                                            _dataInterfaceService.ReplaceParameterValue(dynamic, new Dictionary<string, string>());
                                            var dt = _databaseService.GetInterFaceData(linkEntity, dynamic.Query, parameter.ToArray());
                                            List<Dictionary<string, object>> dynamicDataList = dt.Serialize().Deserialize<List<Dictionary<string, object>>>();
                                            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                                            foreach (var item in dynamicDataList)
                                            {
                                                Dictionary<string, string> dynamicDic = new Dictionary<string, string>();
                                                dynamicDic.Add(item[model.__config__.props.value].ToString(), item[model.__config__.props.label].ToString());
                                                list.Add(dynamicDic);
                                            }
                                            templateData.Add(fieldName1, list);
                                        }
                                    }
                                }
                                break;
                            //公司
                            case HszKeyConst.COMSELECT:
                                {
                                    var com_organizeEntityList = await _organizeService.GetListAsync();
                                    List<Dictionary<string, string[]>> com_organizeList = new List<Dictionary<string, string[]>>();
                                    foreach (var item in com_organizeEntityList)
                                    {
                                        Dictionary<string, string[]> com_organize = new Dictionary<string, string[]>();
                                        com_organize.Add(item.Id, new string[] { item.OrganizeIdTree, item.FullName });
                                        com_organizeList.Add(com_organize);
                                    }
                                    templateData.Add(fieldName1, com_organizeList);
                                }
                                break;
                            //部门
                            case HszKeyConst.DEPSELECT:
                                {
                                    var dep_organizeEntityList = await _departmentService.GetListAsync();
                                    List<Dictionary<string, string>> dep_organizeList = new List<Dictionary<string, string>>();
                                    foreach (var item in dep_organizeEntityList)
                                    {
                                        Dictionary<string, string> dep_organize = new Dictionary<string, string>();
                                        dep_organize.Add(item.Id, item.FullName);
                                        dep_organizeList.Add(dep_organize);
                                    }
                                    templateData.Add(fieldName1, dep_organizeList);
                                }
                                break;
                            //岗位
                            case HszKeyConst.POSSELECT:
                                {
                                    var positionEntityList = await _positionService.GetListAsync();
                                    List<Dictionary<string, string>> positionList = new List<Dictionary<string, string>>();
                                    foreach (var item in positionEntityList)
                                    {
                                        Dictionary<string, string> position = new Dictionary<string, string>();
                                        position.Add(item.Id, item.FullName);
                                        positionList.Add(position);
                                    }
                                    templateData.Add(fieldName1, positionList);
                                }
                                break;
                            //数据字典
                            case HszKeyConst.DICTIONARY:
                                {
                                    var dictionaryTypeEntityLists = await _dictionaryTypeService.GetList();
                                    List<Dictionary<string, string>> dictionaryTypeList = new List<Dictionary<string, string>>();
                                    foreach (var item in dictionaryTypeEntityLists)
                                    {
                                        Dictionary<string, string> dictionaryType = new Dictionary<string, string>();
                                        dictionaryType.Add(item.Id, item.FullName);
                                        dictionaryTypeList.Add(dictionaryType);
                                    }
                                    templateData.Add(fieldName1, dictionaryTypeList);
                                }
                                break;
                            //级联选择
                            case HszKeyConst.CASCADER:
                                {
                                    if (vModelType.DICTIONARY.GetDescription() == configModel.dataType)
                                    {
                                        List<DictionaryDataEntity> dictionaryDataEntityList = string.IsNullOrEmpty(configModel.dictionaryType) ? null : await _dictionaryDataService.GetList(configModel.dictionaryType);
                                        List<Dictionary<string, string>> dictionaryDataList = new List<Dictionary<string, string>>();
                                        foreach (var item in dictionaryDataEntityList)
                                        {
                                            Dictionary<string, string> dictionary = new Dictionary<string, string>();
                                            dictionary.Add(item.Id, item.FullName);
                                            dictionary.Add(item.EnCode, item.FullName);
                                            dictionaryDataList.Add(dictionary);
                                        }
                                        templateData.Add(fieldName1, dictionaryDataList);
                                    }
                                    if (vModelType.STATIC.GetDescription() == configModel.dataType)
                                    {
                                        var props = model.props.props;
                                        var optionList = GetTreeOptions(model.options, props);
                                        List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                                        foreach (var item in optionList)
                                        {
                                            Dictionary<string, string> option = new Dictionary<string, string>();
                                            option.Add(item.value, item.label);
                                            list.Add(option);
                                        }
                                        templateData.Add(fieldName1, list);
                                    }
                                }
                                break;
                            //所属组织
                            case HszKeyConst.CURRORGANIZE:
                                {
                                    var orgMapList = await _organizeService.GetListAsync();
                                    List<Dictionary<string, string[]>> addList = new List<Dictionary<string, string[]>>();
                                    foreach (var item in orgMapList)
                                    {
                                        Dictionary<string, string[]> address = new Dictionary<string, string[]>();
                                        address.Add(item.Id, new string[] { item.OrganizeIdTree, item.Category, item.FullName });
                                        addList.Add(address);
                                    }
                                    templateData.Add(fieldName1, addList);
                                }
                                break;
                        }
                    }
                }

                //缓存2分钟
                _cacheManager.Set(cacheKey, templateData, TimeSpan.FromMinutes(2));
            }

            #region 省市区 单独处理
            if (formData.Where(x => x.__config__.hszKey == "address").Any())
            {
                var level = formData.Where(x => x.__config__.hszKey == "address" && x.level != 3).Any();
                var level3 = formData.Where(x => x.__config__.hszKey == "address" && x.level == 3).Any();

                var addCacheKey = CommonConst.VISUALDEV + "_address1";
                var addCacheKey2 = CommonConst.VISUALDEV + "_address2";
                if (level3)
                {
                    if (_cacheManager.Exists(addCacheKey2))
                    {
                        templateData.Add(addCacheKey2, _cacheManager.Get(addCacheKey2));
                    }
                    else
                    {
                        var addressEntityList = await _provinceService.GetList();

                        #region 处理省市区树
                        addressEntityList.Where(x => x.Type == "1").ToList().ForEach(item =>
                        {
                            item.QuickQuery = item.FullName;
                        });
                        addressEntityList.Where(x => x.Type == "2").ToList().ForEach(item =>
                        {
                            item.QuickQuery = addressEntityList.Find(x => x.Id == item.ParentId).QuickQuery + "/" + item.FullName;
                        });
                        addressEntityList.Where(x => x.Type == "3").ToList().ForEach(item =>
                        {
                            item.QuickQuery = addressEntityList.Find(x => x.Id == item.ParentId).QuickQuery + "/" + item.FullName;
                        });
                        addressEntityList.Where(x => x.Type == "4").ToList().ForEach(item =>
                        {
                            var it = addressEntityList.Find(x => x.Id == item.ParentId);
                            if (it != null) item.QuickQuery = it.QuickQuery + "/" + item.FullName;
                        });
                        #endregion

                        #region 分开 省市区街道 数据
                        List<Dictionary<string, string>> addressList = new List<Dictionary<string, string>>();
                        foreach (var item in addressEntityList.Where(x => x.Type == "4").ToList())
                        {
                            Dictionary<string, string> address = new Dictionary<string, string>();
                            address.Add(item.Id, item.QuickQuery);
                            addressList.Add(address);
                        }

                        //缓存七天
                        _cacheManager.Set(addCacheKey2, addressList, TimeSpan.FromDays(7));
                        templateData.Add(addCacheKey2, addressList.Serialize());
                        #endregion
                    }
                }
                if (level)
                {
                    if (_cacheManager.Exists(addCacheKey))
                    {
                        templateData.Add(addCacheKey, _cacheManager.Get(addCacheKey));
                    }
                    else
                    {
                        var addressEntityList = await _provinceService.GetList();

                        #region 处理省市区树
                        addressEntityList.Where(x => x.Type == "1").ToList().ForEach(item =>
                        {
                            item.QuickQuery = item.FullName;
                        });
                        addressEntityList.Where(x => x.Type == "2").ToList().ForEach(item =>
                        {
                            item.QuickQuery = addressEntityList.Find(x => x.Id == item.ParentId).QuickQuery + "/" + item.FullName;
                        });
                        addressEntityList.Where(x => x.Type == "3").ToList().ForEach(item =>
                        {
                            item.QuickQuery = addressEntityList.Find(x => x.Id == item.ParentId).QuickQuery + "/" + item.FullName;
                        });
                        #endregion

                        #region 分开 省市区街道 数据
                        List<Dictionary<string, string>> addressList = new List<Dictionary<string, string>>();
                        foreach (var item in addressEntityList.Where(x => x.Type == "1").ToList())
                        {
                            Dictionary<string, string> address = new Dictionary<string, string>();
                            address.Add(item.Id, item.QuickQuery);
                            addressList.Add(address);
                        }
                        foreach (var item in addressEntityList.Where(x => x.Type == "2").ToList())
                        {
                            Dictionary<string, string> address = new Dictionary<string, string>();
                            address.Add(item.Id, item.QuickQuery);
                            addressList.Add(address);
                        }
                        foreach (var item in addressEntityList.Where(x => x.Type == "3").ToList())
                        {
                            Dictionary<string, string> address = new Dictionary<string, string>();
                            address.Add(item.Id, item.QuickQuery);
                            addressList.Add(address);
                        }
                        #endregion

                        //缓存七天
                        _cacheManager.Set(addCacheKey, addressList, TimeSpan.FromDays(7));
                        templateData.Add(addCacheKey, addressList.Serialize());
                    }
                }
            }
            #endregion

            #region 用户单独处理
            if (formData.Where(x => x.__config__.hszKey == "userSelect").Any())
            {
                var userCacheKey = CommonConst.VISUALDEV + "_userSelect";
                if (_cacheManager.Exists(userCacheKey))
                {
                    templateData.Add(userCacheKey, _cacheManager.Get(userCacheKey));
                }
                else
                {
                    var userEntityList = await _userService.GetUserListByExp(x => x.DeleteMark == null
                    , x => new System.Entitys.Permission.UserEntity() { Id = x.Id, RealName = x.RealName, Account = x.Account });

                    List<Dictionary<string, string>> userList = new List<Dictionary<string, string>>();
                    foreach (var item in userEntityList)
                    {
                        Dictionary<string, string> user = new Dictionary<string, string>();
                        user.Add(item.Id, item.RealName + "/" + item.Account);
                        userList.Add(user);
                    }
                    //缓存0.5小时
                    _cacheManager.Set(userCacheKey, userList, TimeSpan.FromMinutes(30));
                    templateData.Add(userCacheKey, userList.Serialize());
                }
            }
            #endregion

            return templateData;
        }

        #endregion

        #region 系统组件生成与解析

        /// <summary>
        /// 生成系统自动生成字段
        /// </summary>
        /// <param name="fieldsModelList">模板数据</param>
        /// <param name="allDataMap">真实数据</param>
        /// <param name="IsCreate">创建与修改标识 true创建 false 修改</param>
        /// <returns></returns>
        public async Task<Dictionary<string, object>> GenerateFeilds(List<FieldsModel> fieldsModelList, Dictionary<string, object> allDataMap, bool IsCreate)
        {
            var userInfo = await _userManager.GetUserInfo();
            int dicCount = allDataMap.Keys.Count;
            string[] strKey = new string[dicCount];
            allDataMap.Keys.CopyTo(strKey, 0);
            for (int i = 0; i < strKey.Length; i++)
            {
                //根据KEY查找模板
                var model = fieldsModelList.Find(f => f.__vModel__ == strKey[i]);
                if (model != null)
                {
                    //如果模板hszKey为table为子表数据
                    if ("table".Equals(model.__config__.hszKey) && allDataMap[strKey[i]] != null)
                    {
                        List<FieldsModel> childFieldsModelList = model.__config__.children;
                        var objectData = allDataMap[strKey[i]];
                        List<Dictionary<string, object>> childAllDataMapList = objectData.Serialize().Deserialize<List<Dictionary<string, object>>>();
                        if (childAllDataMapList != null && childAllDataMapList.Count > 0)
                        {
                            List<Dictionary<string, object>> newChildAllDataMapList = new List<Dictionary<string, object>>();
                            foreach (var childmap in childAllDataMapList)
                            {
                                var newChildData = new Dictionary<string, object>();
                                foreach (KeyValuePair<string, object> item in childmap)
                                {
                                    var childFieldsModel = childFieldsModelList.Where(c => c.__vModel__ == item.Key).FirstOrDefault();
                                    if (childFieldsModel != null)
                                    {
                                        if (childFieldsModel.__vModel__.Equals(item.Key))
                                        {
                                            string hszKeyType = childFieldsModel.__config__.hszKey;
                                            switch (hszKeyType)
                                            {
                                                case "billRule":
                                                    if (IsCreate)
                                                    {
                                                        string billNumber = await _billRuleService.GetBillNumber(childFieldsModel.__config__.rule);
                                                        if (!"单据规则不存在".Equals(billNumber))
                                                        {
                                                            newChildData[item.Key] = billNumber;
                                                        }
                                                        else
                                                        {
                                                            newChildData[item.Key] = "";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        newChildData[item.Key] = childmap[item.Key];
                                                    }
                                                    break;
                                                case "createUser":
                                                    if (IsCreate)
                                                    {
                                                        newChildData[item.Key] = userInfo.userId;
                                                    }
                                                    break;
                                                case "createTime":
                                                    if (IsCreate)
                                                    {
                                                        newChildData[item.Key] = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                                                    }
                                                    break;
                                                case "modifyUser":
                                                    if (!IsCreate)
                                                    {
                                                        newChildData[item.Key] = userInfo.userId;
                                                    }
                                                    break;
                                                case "modifyTime":
                                                    if (!IsCreate)
                                                    {
                                                        newChildData[item.Key] = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                                                    }
                                                    break;
                                                case "currPosition":
                                                    if (IsCreate)
                                                    {
                                                        var userEntity = _userService.GetInfoByUserId(userInfo.userId);
                                                        if (!string.IsNullOrEmpty(userEntity.PositionId))
                                                        {
                                                            var positionEntity = await _positionService.GetInfoById(userEntity.PositionId.Split(",").FirstOrDefault());
                                                            if (positionEntity != null)
                                                            {
                                                                newChildData[item.Key] = positionEntity.Id;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            newChildData[item.Key] = "";
                                                        }
                                                    }
                                                    break;
                                                case "currOrganize":
                                                    {
                                                        if (IsCreate)
                                                        {
                                                            if (userInfo.organizeId != null)
                                                            {
                                                                newChildData[item.Key] = userInfo.organizeId;
                                                            }
                                                            else
                                                            {
                                                                newChildData[item.Key] = "";
                                                            }
                                                        }
                                                    }
                                                    break;
                                                //文件上传
                                                case "uploadFz":
                                                    {
                                                        if (allDataMap[strKey[i]].IsNullOrEmpty())
                                                        {
                                                            allDataMap[strKey[i]] = new string[] { };
                                                        }
                                                    }
                                                    break;
                                                default:
                                                    newChildData[item.Key] = childmap[item.Key];
                                                    break;
                                            }
                                        }
                                    }
                                }
                                newChildAllDataMapList.Add(newChildData);
                                allDataMap[strKey[i]] = newChildAllDataMapList;
                            }
                        }
                    }
                    else
                    {
                        if (model.__vModel__.Equals(strKey[i]))
                        {
                            string hszKeyType = model.__config__.hszKey;
                            switch (hszKeyType)
                            {
                                case "billRule":
                                    if (IsCreate)
                                    {
                                        string billNumber = await _billRuleService.GetBillNumber(model.__config__.rule);
                                        if (!"单据规则不存在".Equals(billNumber))
                                        {
                                            allDataMap[strKey[i]] = billNumber;
                                        }
                                        else
                                        {
                                            allDataMap[strKey[i]] = "";
                                        }
                                    }
                                    break;
                                case "createUser":
                                    {
                                        if (IsCreate)
                                        {
                                            allDataMap[strKey[i]] = userInfo.userId;
                                        }
                                        else
                                        {
                                            allDataMap[strKey[i]] = await _userService.GetUserIdByRealName(allDataMap[strKey[i]].ToString());
                                        }
                                    }
                                    break;
                                case "createTime":
                                    if (IsCreate)
                                    {
                                        allDataMap[strKey[i]] = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                                    }
                                    break;
                                case "modifyUser":
                                    if (!IsCreate)
                                    {
                                        allDataMap[strKey[i]] = userInfo.userId;
                                    }
                                    break;
                                case "modifyTime":
                                    if (!IsCreate)
                                    {
                                        allDataMap[strKey[i]] = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                                    }
                                    break;
                                case "currPosition":
                                    if (IsCreate)
                                    {
                                        var userEntity = _userService.GetInfoByUserId(userInfo.userId);
                                        if (!string.IsNullOrEmpty(userEntity.PositionId))
                                        {
                                            var positionEntity = await _positionService.GetInfoById(userEntity.PositionId.Split(",").FirstOrDefault());
                                            if (positionEntity != null)
                                            {
                                                allDataMap[strKey[i]] = positionEntity.Id;
                                            }
                                        }
                                        else
                                        {
                                            allDataMap[strKey[i]] = "";
                                        }
                                    }
                                    break;
                                case "currOrganize":
                                    if (IsCreate)
                                    {
                                        if (userInfo.organizeId != null)
                                        {
                                            allDataMap[strKey[i]] = userInfo.organizeId;
                                        }
                                        else
                                        {
                                            allDataMap[strKey[i]] = "";
                                        }
                                    }
                                    break;
                                //文件上传
                                case "uploadFz":
                                    {
                                        if (allDataMap[strKey[i]].IsNullOrEmpty())
                                        {
                                            allDataMap[strKey[i]] = new string[] { };
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            return allDataMap;
        }

        /// <summary>
        /// 将系统组件生成的数据转换为数据
        /// </summary>
        /// <param name="formData">数据库模板数据</param>
        /// <param name="templateData">模板真实数据</param>
        /// <param name="entity">真实数据</param>
        /// <returns></returns>
        private async Task<string> GetSystemComponentsData(List<FieldsModel> formData, Dictionary<string, object> templateData, string modelData)
        {
            //剔除无限极
            formData = TemplateDataConversion(formData);
            //数据库保存的F_Data
            Dictionary<string, object> dataMap = modelData.ToObject<Dictionary<string, object>>();

            #region 序列化后时间戳转换处理
            var timeList = formData.Where(x => x.__config__.hszKey == "createTime" || x.__config__.hszKey == "modifyTime").ToList();

            if (timeList.Count > 0)
            {
                timeList.ForEach(item =>
                {
                    if (dataMap.ContainsKey(item.__vModel__))
                    {
                        var value = dataMap[item.__vModel__].ToString();
                        if (value.IsNotEmptyOrNull())
                        {
                            dataMap.Remove(item.__vModel__);
                            DateTime dtDate;
                            if (!DateTime.TryParse(value, out dtDate)) value = string.Format("{0:yyyy-MM-dd HH:mm:ss}", Ext.GetDateTime(value));
                            dataMap.Add(item.__vModel__, value);
                        }
                    }
                });
            }
            #endregion

            int dicCount = dataMap.Keys.Count;
            string[] strKey = new string[dicCount];
            dataMap.Keys.CopyTo(strKey, 0);
            //自动生成的数据不在模板数据内
            //var record = dataMap.Keys.Except(templateData.Keys).ToList();
            var record = dataMap.Keys.ToList();

            foreach (var key in record)
            {
                if (dataMap[key] != null)
                {
                    var dataValue = dataMap[key].ToString();
                    if (!string.IsNullOrEmpty(dataValue))
                    {
                        var model = formData.Where(f => f.__vModel__ == key).FirstOrDefault();
                        if (model != null)
                        {
                            ConfigModel configModel = model.__config__;
                            string type = configModel.hszKey;
                            if (string.IsNullOrWhiteSpace(model.separator)) model.separator = ";";
                            switch (type)
                            {
                                //case "currDept":
                                //    {
                                //        var deptMapList = await _departmentService.GetListAsync();
                                //        dataMap[key] = deptMapList.Find(o => o.Id.Equals(dataMap[key].ToString())).FullName;
                                //    }
                                //    break;
                                //case "switch"://开关
                                //    {
                                //        dataMap[key] = dataMap[key].ToInt() == 0 ? model.inactiveTxt : model.activeTxt;
                                //    }
                                //    break;
                                case "createUser":
                                    {
                                        var createUser = await _userService.GetInfoByUserIdAsync(dataMap[key].ToString());
                                        if (createUser != null)
                                            dataMap[key] = createUser.RealName;
                                    }
                                    break;
                                case "modifyUser":
                                    {
                                        var modifyUser = await _userService.GetInfoByUserIdAsync(dataMap[key].ToString());
                                        if (modifyUser != null)
                                            dataMap[key] = modifyUser.RealName;
                                    }
                                    break;
                                case "currPosition":
                                    {
                                        var mapList = await _positionService.GetListAsync();
                                        dataMap[key] = mapList.Where(p => p.Id == dataMap[key].ToString()).FirstOrDefault().FullName;
                                    }
                                    break;
                                case "currOrganize":
                                    {
                                        var currOrganizeTemplateValue = templateData.Where(t => t.Key.Equals(configModel.renderKey + "_" + key)).FirstOrDefault().Value?.Serialize().Deserialize<List<Dictionary<string, string[]>>>();
                                        var valueId = currOrganizeTemplateValue.Where(x => x.Keys.Contains(dataMap[key].ToString())).FirstOrDefault();

                                        if (valueId != null)
                                        {
                                            if (model.showLevel == "all")
                                            {
                                                var cascaderData = valueId[dataMap[key].ToString()];
                                                if (cascaderData != null && !string.IsNullOrWhiteSpace(cascaderData.FirstOrDefault()))
                                                {
                                                    var treeFullName = new List<string>();
                                                    cascaderData.FirstOrDefault()?.Split(',').ToList().ForEach(item =>
                                                    {
                                                        treeFullName.Add(currOrganizeTemplateValue.Find(x => x.Keys.Contains(item)).FirstOrDefault().Value?.LastOrDefault());
                                                    });
                                                    dataMap[key] = string.Join("/", treeFullName);
                                                }
                                                else
                                                {
                                                    dataMap[key] = "";
                                                    //dataMap[key] = cascaderData?.LastOrDefault();
                                                }
                                            }
                                            else
                                            {
                                                var cascaderData = valueId[dataMap[key].ToString()];
                                                if (cascaderData != null && cascaderData[1] == "department")
                                                    dataMap[key] = valueId[dataMap[key].ToString()]?.LastOrDefault();
                                                else
                                                    dataMap[key] = "";
                                            }
                                        }
                                        else
                                        {
                                            dataMap[key] = "";
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
            }

            return dataMap.Serialize();
        }

        #endregion

        #region 转换数据

        /// <summary>
        /// 将关键字key查询传输的id转换成名称，还有动态数据id成名称
        /// </summary>
        /// <param name="formData">数据库模板数据</param>
        /// <param name="templateData">模板真实数据</param>
        /// <param name="keywordJsonDic">查询数据</param>
        /// <param name="list">真实数据</param>
        /// <param name="webType">表单类型1-纯表单、2-普通表单、3-工作流表单</param>
        /// <param name="primaryKey">数据主键</param>
        /// <returns></returns>
        private async Task<Dictionary<string, object>> GetKeyData(List<FieldsModel> formData, Dictionary<string, object> templateData, Dictionary<string, object> keywordJsonDic, List<VisualDevModelDataEntity> list, ColumnDesignModel columnDesign = null, string actionType = "List", int webType = 2, string primaryKey = "F_Id")
        {
            List<string> redisList = new List<string>();

            //转换数据
            var convData = new Dictionary<string, object>();

            #region 存放 预缓存数据的控件的缓存数据 ， 避免循环时重复序列化 耗资源
            var currOrganizeTemplateValue = new List<Dictionary<string, string[]>>();//组织
            var addressTemplateValue = new List<Dictionary<string, string>>();//地址 省市区 缓存
            var streetTemplateValue = new List<Dictionary<string, string>>();//地址 街道 缓存
            var COMSELECTTemplateValue = new List<Dictionary<string, string[]>>();//公司
            var DEPSELECTTemplateValue = new List<Dictionary<string, string>>();//部门
            var USERSELECTTemplateValue = new List<Dictionary<string, string>>();//用户
            var POSSELECTTemplateValue = new List<Dictionary<string, string>>();//岗位
            var TemplateValues = new Dictionary<string, List<Dictionary<string, string>>>();//其他
            #endregion

            //转换列表数据
            foreach (var entity in list)
            {
                //数据库保存的F_Data
                Dictionary<string, object> dataMap = entity.Data.Deserialize<Dictionary<string, object>>();
                if (webType == 3)
                {
                    IFlowTaskRepository _flowTaskService = null;
                    if (_flowTaskService == null)
                    {
                        _flowTaskService = App.GetService<IFlowTaskRepository>();
                    }
                    var flowTask = await _flowTaskService.GetTaskInfo(entity.Id);
                    if (flowTask != null)
                    {
                        dataMap["flowState"] = flowTask.Status;
                    }
                    else
                    {
                        dataMap["flowState"] = 0;
                    }
                }
                int dicCount = dataMap.Keys.Count;
                string[] strKey = new string[dicCount];
                dataMap.Keys.CopyTo(strKey, 0);
                for (int i = 0; i < strKey.Length; i++)
                {
                    if (!(dataMap[strKey[i]] is null))
                    {
                        var form = formData.Where(f => f.__vModel__ == strKey[i]).FirstOrDefault();
                        if (form != null)
                        {
                            if (form.__vModel__.Contains(form.__config__.hszKey + "Field"))
                            {
                                dataMap[strKey[i]] = TemplateControlsDataConversion(dataMap[strKey[i]], form);
                            }
                            else
                            {
                                dataMap[strKey[i]] = TemplateControlsDataConversion(dataMap[strKey[i]], form, actionType);
                            }

                            var hszKey = form.__config__.hszKey;
                            var templateValue = templateData.Where(t => t.Key.Equals(form.__config__.renderKey + "_" + strKey[i])).FirstOrDefault();

                            #region 处理 单独存储缓存
                            if(hszKey== HszKeyConst.USERSELECT)
                            {
                                templateValue = templateData.Where(t => t.Key.Equals(CommonConst.VISUALDEV + "_userSelect")).FirstOrDefault();
                                if (!templateData.ContainsKey(form.__config__.renderKey + "_" + strKey[i])) templateData.Add(form.__config__.renderKey + "_" + strKey[i], "");
                            }
                            if (hszKey == HszKeyConst.ADDRESS)
                            {
                                if (form.level == 3) templateValue = templateData.Where(t => t.Key.Equals(CommonConst.VISUALDEV + "_address2")).FirstOrDefault();
                                else templateValue = templateData.Where(t => t.Key.Equals(CommonConst.VISUALDEV + "_address1")).FirstOrDefault();
                                if (!templateData.ContainsKey(form.__config__.renderKey + "_" + strKey[i])) templateData.Add(form.__config__.renderKey + "_" + strKey[i], "");
                            }
                            #endregion

                            //转换后的数据值
                            var dataDicValue = dataMap[strKey[i]];
                            if (templateValue.Key != null && !(dataDicValue is null) && dataDicValue.ToString() != "[]")
                            {
                                var moreValue = dataDicValue as IEnumerable<object>;
                                if (dataDicValue.IsNullOrEmpty()) hszKey = "";//空数据直接赋值
                                //不是List数据直接赋值 用逗号切割成 List
                                if (moreValue == null && !dataDicValue.ToString().Contains("[")) moreValue = dataDicValue.ToString().Split(",");

                                if (string.IsNullOrWhiteSpace(form.separator)) form.separator = ";";

                                switch (hszKey)
                                {
                                    case HszKeyConst.COMSELECT:
                                        {
                                            var currOrganizeValues = new List<List<object>>();
                                            if (form.multiple) currOrganizeValues = moreValue != null ? moreValue.ToJson().ToObject<List<List<object>>>() : dataDicValue.ToString().Deserialize<List<List<object>>>();
                                            else currOrganizeValues.Add(moreValue != null ? moreValue.ToJson().ToObject<List<object>>() : dataDicValue.ToString().Deserialize<List<object>>());

                                            var addNames = new List<string>();

                                            if (COMSELECTTemplateValue.Count < 1) COMSELECTTemplateValue = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string[]>>>();

                                            foreach (var item in currOrganizeValues)
                                            {
                                                var addName = new List<string>();
                                                var itlist = item as List<object>;
                                                foreach (var it in itlist)
                                                {
                                                    var currOrganizeData = COMSELECTTemplateValue.Where(a => a.ContainsKey(it.ToString())).FirstOrDefault();
                                                    if (currOrganizeData != null) addName.Add(currOrganizeData[it.ToString()].LastOrDefault());
                                                }

                                                addNames.Add(string.Join("/", addName));
                                            }
                                            if (addNames.Count != 0)
                                            {
                                                dataMap[strKey[i]] = string.Join(form.separator, addNames);
                                                //dataMap[strKey[i]] = addName.ToString();
                                            }
                                        }
                                        break;
                                    case HszKeyConst.DEPSELECT:
                                        {
                                            //StringBuilder addName = new StringBuilder();
                                            var addName = new List<string>();
                                            if (DEPSELECTTemplateValue.Count < 1) DEPSELECTTemplateValue = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>();
                                            foreach (var item in moreValue)
                                            {
                                                var comData = DEPSELECTTemplateValue.Where(a => a.ContainsKey(item.ToString())).FirstOrDefault();
                                                if (comData != null)
                                                {
                                                    addName.Add(comData[item.ToString()]);
                                                }
                                            }
                                            if (addName.Count != 0)
                                            {
                                                dataMap[strKey[i]] = string.Join(form.separator, addName);
                                            }
                                        }
                                        break;
                                    case HszKeyConst.ADDRESS:
                                        {
                                            var addressValues = new List<List<object>>();
                                            if (form.multiple) addressValues = dataDicValue.ToJson().Deserialize<List<List<object>>>();
                                            else addressValues.Add(dataDicValue.ToJson().Deserialize<List<object>>());

                                            var addNames = new List<string>();

                                            //StringBuilder addName = new StringBuilder();
                                            if (addressTemplateValue.Count < 1) addressTemplateValue = templateValue.Value.ToString().Deserialize<List<Dictionary<string, string>>>();
                                            if (form.level == 3 && streetTemplateValue.Count < 1) streetTemplateValue = templateValue.Value.ToString().Deserialize<List<Dictionary<string, string>>>();

                                            foreach (var item in addressValues)
                                            {
                                                var itlist = item as List<object>;
                                                if (itlist.Any())
                                                {
                                                    var value = addressTemplateValue.Where(a => a.ContainsKey(itlist.LastOrDefault().ToString())).FirstOrDefault();
                                                    if (form.level == 3) value = streetTemplateValue.Where(a => a.ContainsKey(itlist.LastOrDefault().ToString())).FirstOrDefault();
                                                    addNames.Add(value[itlist.LastOrDefault().ToString()]);
                                                }
                                            }
                                            if (addNames.Count != 0)
                                            {
                                                dataMap[strKey[i]] = string.Join(form.separator, addNames);
                                                //dataMap[strKey[i]] = addName.ToString();
                                            }
                                        }
                                        break;
                                    case HszKeyConst.USERSELECT:
                                        {
                                            //StringBuilder userName = new StringBuilder();
                                            var userName = new List<string>();
                                            if (USERSELECTTemplateValue.Count < 1) USERSELECTTemplateValue = templateValue.Value.ToString().Deserialize<List<Dictionary<string, string>>>();
                                            foreach (var item in moreValue)
                                            {
                                                var cascaderData = USERSELECTTemplateValue.Where(c => c.ContainsKey(item.ToString())).FirstOrDefault();
                                                if (cascaderData != null)
                                                {
                                                    userName.Add(cascaderData[item.ToString()]);
                                                }
                                            }
                                            if (userName.Count != 0)
                                            {
                                                dataMap[strKey[i]] = string.Join(form.separator, userName);
                                                //dataMap[strKey[i]] = userName.ToString().TrimEnd(',');
                                            }
                                        }
                                        break;
                                    case HszKeyConst.POSSELECT:
                                        {
                                            //StringBuilder posName = new StringBuilder();
                                            var posName = new List<string>();
                                            if (POSSELECTTemplateValue.Count < 1) POSSELECTTemplateValue = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>();
                                            foreach (var item in moreValue)
                                            {
                                                var cascaderData = POSSELECTTemplateValue.Where(c => c.ContainsKey(item.ToString())).FirstOrDefault();
                                                if (cascaderData != null)
                                                {
                                                    posName.Add(cascaderData[item.ToString()]);
                                                }
                                            }
                                            if (posName.Count != 0)
                                            {
                                                dataMap[strKey[i]] = string.Join(form.separator, posName);
                                                //dataMap[strKey[i]] = posName.ToString().TrimEnd(',');
                                            }
                                        }
                                        break;
                                    case HszKeyConst.CURRORGANIZE:
                                        {
                                            if (currOrganizeTemplateValue.Count < 1) currOrganizeTemplateValue = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string[]>>>();

                                            var valueId = currOrganizeTemplateValue.Where(x => x.Keys.Contains(dataDicValue.ToString())).FirstOrDefault();

                                            if (valueId != null)
                                            {
                                                if (form.showLevel == "all")
                                                {
                                                    var cascaderData = valueId[dataDicValue.ToString()];
                                                    if (cascaderData != null && !string.IsNullOrWhiteSpace(cascaderData.FirstOrDefault()))
                                                    {
                                                        var treeFullName = new List<string>();
                                                        cascaderData.FirstOrDefault()?.Split(',').ToList().ForEach(item =>
                                                        {
                                                            treeFullName.Add(currOrganizeTemplateValue.Find(x => x.Keys.Contains(item)).FirstOrDefault().Value?.LastOrDefault());
                                                        });
                                                        dataMap[strKey[i]] = string.Join("/", treeFullName);
                                                    }
                                                    else
                                                    {
                                                        dataMap[strKey[i]] = "";
                                                        //dataMap[strKey[i]] = cascaderData?.LastOrDefault();
                                                    }
                                                }
                                                else
                                                {
                                                    var cascaderData = valueId[dataDicValue.ToString()];
                                                    if (cascaderData != null && cascaderData[1] == "department")
                                                        dataMap[strKey[i]] = valueId[dataDicValue.ToString()]?.LastOrDefault();
                                                    else
                                                        dataMap[strKey[i]] = "";
                                                }
                                            }
                                            else
                                            {
                                                dataMap[strKey[i]] = "";
                                            }
                                        }
                                        break;
                                    case HszKeyConst.CASCADER:
                                        {
                                            //StringBuilder addName = new StringBuilder();
                                            var addName = new List<string>();
                                            if (!TemplateValues.ContainsKey(strKey[i]))
                                                TemplateValues.Add(strKey[i], templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>());

                                            if (form.props.props.multiple)
                                            {
                                                foreach (var item in moreValue)
                                                {
                                                    var sb = new List<string>();
                                                    item.Serialize().Deserialize<List<string>>().ForEach(items => {
                                                        var cascaderData = TemplateValues[strKey[i]].Where(c => c.ContainsKey(items.ToString())).FirstOrDefault();
                                                        if (cascaderData != null)
                                                        {
                                                            sb.Add(cascaderData[items.ToString()]);
                                                        }
                                                    });
                                                    if (sb.Count != 0)
                                                    {
                                                        addName.Add(string.Join("/", sb));
                                                        //addName.AppendFormat("{0}", sb.ToString().TrimEnd('/') + ",");
                                                    }
                                                }
                                                if (addName.Count != 0)
                                                {
                                                    dataMap[strKey[i]] = string.Join(form.separator, addName);
                                                    //dataMap[strKey[i]] = addName.ToString().TrimEnd(',');
                                                }
                                            }
                                            else
                                            {
                                                foreach (var item in moreValue)
                                                {
                                                    var cascaderData = TemplateValues[strKey[i]].Where(c => c.ContainsKey(item.ToString())).FirstOrDefault();
                                                    if (cascaderData != null)
                                                    {
                                                        addName.Add(cascaderData[item.ToString()]);
                                                        //addName.Append(cascaderData[item.ToString()] + "/");
                                                    }
                                                }
                                                if (addName.Count != 0)
                                                {
                                                    dataMap[strKey[i]] = string.Join(form.separator, addName);
                                                    //dataMap[strKey[i]] = addName.ToString().TrimEnd('/');
                                                }
                                            }
                                        }
                                        break;
                                    case HszKeyConst.CHECKBOX:
                                        {
                                            //StringBuilder addName = new StringBuilder();
                                            var addName = new List<string>();
                                            if (!TemplateValues.ContainsKey(strKey[i]))
                                                TemplateValues.Add(strKey[i], templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>());

                                            foreach (var item in moreValue)
                                            {
                                                var cascaderData = TemplateValues[strKey[i]].Where(c => c.ContainsKey(item.ToString())).FirstOrDefault();
                                                if (cascaderData != null)
                                                {
                                                    addName.Add(cascaderData[item.ToString()]);
                                                }
                                            }
                                            if (addName.Count != 0)
                                            {
                                                dataMap[strKey[i]] = string.Join(form.separator, addName);
                                                //dataMap[strKey[i]] = addName.ToString().TrimEnd(',');
                                            }
                                        }
                                        break;
                                    case HszKeyConst.SELECT:
                                        {
                                            //StringBuilder selectName = new StringBuilder();
                                            var selectName = new List<string>();
                                            if (!TemplateValues.ContainsKey(strKey[i]))
                                                TemplateValues.Add(strKey[i], templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>());

                                            foreach (var item in moreValue)
                                            {
                                                var cascaderData = TemplateValues[strKey[i]].Where(c => c.ContainsKey(item.ToString())).FirstOrDefault();
                                                if (cascaderData != null)
                                                {
                                                    selectName.Add(cascaderData[item.ToString()]);
                                                }
                                            }
                                            if (selectName.Count != 0)
                                            {
                                                dataMap[strKey[i]] = string.Join(form.separator, selectName);
                                            }
                                        }
                                        break;
                                    case HszKeyConst.TREESELECT:
                                        {
                                            //StringBuilder treeSelectName = new StringBuilder();
                                            var treeSelectName = new List<string>();
                                            if (!TemplateValues.ContainsKey(strKey[i]))
                                                TemplateValues.Add(strKey[i], templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>());

                                            foreach (var item in moreValue)
                                            {
                                                var cascaderData = TemplateValues[strKey[i]].Where(c => c.ContainsKey(item.ToString())).FirstOrDefault();
                                                if (cascaderData != null)
                                                {
                                                    treeSelectName.Add(cascaderData[item.ToString()]);
                                                }
                                            }
                                            if (treeSelectName.Count != 0)
                                            {
                                                dataMap[strKey[i]] = string.Join(form.separator, treeSelectName);
                                            }
                                        }
                                        break;
                                    default:
                                        {
                                            if (hszKey.IsNotEmptyOrNull())
                                            {
                                                if (!TemplateValues.ContainsKey(strKey[i]))
                                                    TemplateValues.Add(strKey[i], templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>());

                                                var convertData = TemplateValues[strKey[i]].Where(t => t.ContainsKey(dataMap[strKey[i]].ToString())).FirstOrDefault();
                                                if (convertData != null)
                                                {
                                                    dataMap[strKey[i]] = convertData.Values.FirstOrDefault().ToString();
                                                }
                                            }
                                            else dataMap[strKey[i]] = "";
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
                //转换模板没记录的数据
                var record = dataMap.Keys.Except(templateData.Keys.Select(x => x = x.Substring(14, x.Length - 14))).ToList();
                //针对有表的模板去除"rownum,主键"
                record.RemoveAll(r => { return r == "ROWNUM"; });
                record.RemoveAll(r => { return r == primaryKey; });

                foreach (var key in record)
                {
                    if (!(dataMap[key] is null) && dataMap[key].ToString() != "")
                    {
                        var dataValue = dataMap[key];
                        var model = formData.Where(f => f.__vModel__ == key).FirstOrDefault();
                        if (model != null)
                        {
                            ConfigModel configModel = model.__config__;
                            string type = configModel.hszKey;
                            if (string.IsNullOrWhiteSpace(model.separator)) model.separator = ";";
                            switch (type)
                            {
                                //switch开关
                                case "switch":
                                    {
                                        dataMap[key] = dataMap[key].ToInt() == 0 ? model.inactiveTxt : model.activeTxt;
                                    }
                                    break;
                                //时间范围
                                case HszKeyConst.TIMERANGE:
                                    {
                                        List<string> jsonArray = new List<string>();
                                        jsonArray = dataValue.Serialize().Deserialize<List<string>>();
                                        string value1 = string.Format("{0:" + model.format + "}", Convert.ToDateTime(jsonArray.First()));
                                        string value2 = string.Format("{0:" + model.format + "}", Convert.ToDateTime(jsonArray.Last()));
                                        jsonArray.Clear();
                                        jsonArray.Add(value1 + "至");
                                        jsonArray.Add(value2);
                                        dataMap[key] = jsonArray;
                                    }
                                    break;
                                //日期选择
                                case HszKeyConst.DATE:
                                    {
                                        string value = string.Empty;
                                        var keyValue = dataMap[key].ToString();
                                        DateTime dtDate;
                                        if (DateTime.TryParse(keyValue, out dtDate)) value = string.Format("{0:yyyy-MM-dd HH:mm:ss} ", keyValue);
                                        else value = string.Format("{0:yyyy-MM-dd HH:mm:ss} ", Ext.GetDateTime(keyValue));

                                        if (!string.IsNullOrEmpty(model.format))
                                        {
                                            value = string.Format("{0:" + model.format + "}", Convert.ToDateTime(value));
                                        }
                                        else
                                        {
                                            switch (model.type)
                                            {
                                                case "date":
                                                    value = string.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(value));
                                                    break;
                                                default:
                                                    value = string.Format("{0:yyyy-MM-dd HH:mm:ss}", Convert.ToDateTime(value));
                                                    break;
                                            }
                                        }
                                        dataMap[key] = value;
                                    }
                                    break;
                                //日期选择
                                case HszKeyConst.TIME:
                                    {
                                        string value = string.Empty;
                                        if (!string.IsNullOrEmpty(model.format))
                                        {
                                            value = string.Format("{0:" + model.format + "}", Convert.ToDateTime(dataMap[key].ToString()));
                                        }
                                        else
                                        {
                                            value = dataMap[key].ToString();
                                        }
                                        dataMap[key] = value;
                                    }
                                    break;
                                //日期范围
                                case HszKeyConst.DATERANGE:
                                    {
                                        List<string> jsonArray = new List<string>();
                                        jsonArray = dataValue.Serialize().Deserialize<List<string>>();
                                        string value1 = string.Format("{0:" + model.format + "}", Ext.GetDateTime(jsonArray.First()));
                                        string value2 = string.Format("{0:" + model.format + "}", Ext.GetDateTime(jsonArray.Last()));
                                        jsonArray.Clear();
                                        jsonArray.Add(value1 + "至");
                                        jsonArray.Add(value2);
                                        dataMap[key] = jsonArray;
                                    }
                                    break;
                                case "currDept":
                                    {
                                        var orgMapList = await _organizeService.GetListAsync();
                                        foreach (var organizeEntity in orgMapList)
                                        {
                                            if (dataMap[key].ToString().Equals(organizeEntity.Id))
                                            {
                                                dataMap[key] = organizeEntity.FullName;
                                            }
                                        }
                                    }
                                    break;
                                case "createUser":
                                    {
                                        var userCreEntity = await _userService.GetInfoByUserIdAsync(dataMap[key].ToString());
                                        if (userCreEntity != null)
                                        {
                                            dataMap[key] = userCreEntity.RealName;
                                        }
                                        else
                                        {
                                            dataMap[key] = dataMap[key];
                                        }
                                    }
                                    break;
                                case "createTime":
                                    {
                                        string value = string.Format("{0:yyyy-MM-dd HH:mm:ss}", Convert.ToDateTime(dataMap[key].ToString()));
                                        dataMap[key] = value;
                                    }
                                    break;
                                case "modifyUser":
                                    {
                                        var userModEntity = await _userService.GetInfoByUserIdAsync(dataMap[key].ToString());
                                        if (userModEntity != null)
                                        {
                                            dataMap[key] = userModEntity.RealName;
                                        }
                                        else
                                        {
                                            dataMap[key] = dataMap[key];
                                        }
                                    }
                                    break;
                                case "modifyTime":
                                    {
                                        string value = string.Format("{0:yyyy-MM-dd HH:mm:ss}", Convert.ToDateTime(dataMap[key].ToString()));
                                        dataMap[key] = value;
                                    }
                                    break;
                                case "currPosition":
                                    {
                                        var mapList = await _positionService.GetListAsync();
                                        var position = mapList.Where(p => p.Id == dataMap[key].ToString()).FirstOrDefault();
                                        if (position != null)
                                        {
                                            dataMap[key] = position.FullName;
                                        }
                                    }
                                    break;
                                case "currOrganize":
                                    {
                                        var orgMapList = await _organizeService.GetListAsync();
                                        foreach (var organizeEntity in orgMapList)
                                        {
                                            if (dataMap[key].ToString().Equals(organizeEntity.Id))
                                            {
                                                dataMap[key] = organizeEntity.FullName;
                                            }
                                        }
                                    }
                                    break;
                                //级联
                                case "cascader":
                                    {
                                        if (dataValue.ToJson() == "[]")//空数组的数据处理
                                        {
                                            dataMap[key] = "";
                                            break;
                                        }
                                        switch (configModel.dataType)
                                        {
                                            case "dictionary":
                                                {
                                                    List<DictionaryDataEntity> data = new List<DictionaryDataEntity>();
                                                    var cacheKey = CommonConst.VISUALDEV + _userManager.TenantId + "_" + configModel.hszKey + "_" + configModel.dictionaryType;
                                                    if (_cacheManager.Exists(cacheKey))
                                                    {
                                                        data = _cacheManager.Get(cacheKey).Deserialize<List<DictionaryDataEntity>>();
                                                    }
                                                    else
                                                    {
                                                        data = await _dictionaryDataService.GetList(configModel.dictionaryType);
                                                        _cacheManager.Set(cacheKey, data);
                                                        redisList.Add(cacheKey);
                                                    }

                                                    List<object> dataList = dataMap[key].Serialize().Deserialize<List<object>>();
                                                    List<string> cascaderList = new List<string>();
                                                    foreach (var items in dataList)
                                                    {
                                                        if (items.ToString().IndexOf("[") == 0)
                                                        {
                                                            var its = items.Serialize().Deserialize<List<string>>();
                                                            its.ForEach(it => {
                                                                var cascader = data.Find(d => d.Id == it);
                                                                if (cascader != null)
                                                                {
                                                                    cascaderList.Add(cascader.FullName);
                                                                }
                                                            });
                                                        }
                                                        else
                                                        {
                                                            var cascader = data.Find(d => d.Id == items.ToString());
                                                            if (cascader != null)
                                                            {
                                                                cascaderList.Add(cascader.FullName);
                                                            }
                                                        }
                                                    }
                                                    if (actionType == "List")//列表直接输出字符串 按 , 号 隔开
                                                        dataMap[key] = string.Join(model.separator, cascaderList);
                                                    else
                                                        dataMap[key] = cascaderList;
                                                }
                                                break;
                                            case "dynamic":
                                                {
                                                    //获取远端数据
                                                    DataInterfaceEntity dataEntity = await _dataInterfaceService.GetInfo(configModel.propsUrl);
                                                    if (dataEntity == null)
                                                        break;
                                                    switch (dataEntity.DataType)
                                                    {
                                                        //SQL数据
                                                        case 1:
                                                            {
                                                                var linkEntity = await _dbLinkService.GetInfo(dataEntity.DBLinkId);
                                                                if (linkEntity == null) linkEntity = GetTenantDbLink();
                                                                _dataInterfaceService.ReplaceParameterValue(dataEntity, new Dictionary<string, string>());
                                                                #region 远端数据 配置参数
                                                                var parameter = new List<SugarParameter>();
                                                                if (_userManager.ToKen != null)
                                                                {
                                                                    parameter.Add(new SugarParameter("@user", _userManager.UserId));
                                                                    parameter.Add(new SugarParameter("@organize", _userManager.User.OrganizeId));
                                                                    parameter.Add(new SugarParameter("@department", _userManager.User.OrganizeId));
                                                                    parameter.Add(new SugarParameter("@postion", _userManager.User.PositionId));
                                                                }
                                                                #endregion
                                                                _dataInterfaceService.ReplaceParameterValue(dataEntity, new Dictionary<string, string>());
                                                                var popupsList = _databaseService.GetInterFaceData(linkEntity, dataEntity.Query, parameter.ToArray())
                                                                    .Serialize().Deserialize<List<Dictionary<string, string>>>();

                                                                List<string> dataList = dataMap[key].Serialize().Deserialize<List<string>>();
                                                                List<string> cascaderList = new List<string>();
                                                                foreach (var items in dataList)
                                                                {
                                                                    var vara = popupsList.Where(a => a.ContainsValue(items)).FirstOrDefault();
                                                                    if (vara != null)
                                                                    {
                                                                        cascaderList.Add(vara[model.props.props.label]);
                                                                    }
                                                                }

                                                                if (actionType == "List")//列表直接输出字符串 按 , 号 隔开
                                                                    dataMap[key] = string.Join(model.separator, cascaderList);
                                                                else
                                                                    dataMap[key] = cascaderList;
                                                            }
                                                            break;
                                                        //静态数据
                                                        case 2:
                                                            {
                                                                List<Dictionary<string, string>> dynamicList = new List<Dictionary<string, string>>();
                                                                var value = model.props.props.value;
                                                                var label = model.props.props.label;
                                                                var children = model.props.props.children;
                                                                JToken dynamicDataList = JValue.Parse(dataEntity.Query);
                                                                foreach (var data in dynamicDataList)
                                                                {
                                                                    Dictionary<string, string> dic = new Dictionary<string, string>();
                                                                    dic[value] = data.Value<string>(value);
                                                                    dic[label] = data.Value<string>(label);
                                                                    dynamicList.Add(dic);
                                                                    if (data.Value<object>(children) != null && data.Value<object>(children).ToString() != "")
                                                                    {
                                                                        dynamicList.AddRange(GetDynamicInfiniteData(data.Value<object>(children).ToString(), model.props.props));
                                                                    }
                                                                }

                                                                List<List<string>> dataList = new List<List<string>>();
                                                                var mapValue = dataMap[key].Serialize();
                                                                if (mapValue.Contains("[["))
                                                                    dataList = mapValue.Deserialize<List<List<string>>>();
                                                                else
                                                                    dataList.Add(mapValue.Deserialize<List<string>>());

                                                                List<string> cascaderList = new List<string>();
                                                                foreach (var items in dataList)
                                                                {
                                                                    foreach(var item in items)
                                                                    {
                                                                        var vara = dynamicList.Where(a => a.ContainsValue(item)).FirstOrDefault();
                                                                        if (vara != null) cascaderList.Add(vara[label]);
                                                                    }
                                                                }
                                                                if (actionType == "List")//列表直接输出字符串 按 , 号 隔开
                                                                    dataMap[key] = string.Join(model.separator, cascaderList);
                                                                else
                                                                    dataMap[key] = cascaderList;
                                                            }
                                                            break;
                                                        //Api数据
                                                        case 3:
                                                            {
                                                                var interFace = dataEntity.Path;
                                                                var response = await interFace.GetAsStringAsync();//请求接口
                                                                var result = JSON.Deserialize<RESTfulResult<object>>(response);
                                                                var data = result.data?.ToString().Deserialize<Dictionary<string, List<dynamic>>>().FirstOrDefault().Value;

                                                                List<object> cascaderList = new List<object>();

                                                                if (data != null)
                                                                {
                                                                    //model.propsValue 存储数据ID
                                                                    //model.relationField 存储显示数据
                                                                    data.ForEach(obj =>
                                                                    {
                                                                        if (obj[model.propsValue] == dataValue.ToString())
                                                                            cascaderList.Add(obj[model.relationField]);
                                                                    });
                                                                }

                                                                dataMap[key] = string.Join(model.separator, cascaderList);
                                                            }
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                }
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    break;
                                //关联表单
                                case HszKeyConst.RELATIONFORM:
                                    {
                                        List<Dictionary<string, object>> relationFormDataList = new List<Dictionary<string, object>>();

                                        var redisName = CommonConst.VISUALDEV + _userManager.TenantId + "_" + configModel.hszKey + "_" + model.modelId;
                                        if (_cacheManager.Exists(redisName))
                                        {
                                            relationFormDataList = _cacheManager.Get(redisName).Deserialize<List<Dictionary<string, object>>>();
                                        }
                                        else
                                        {
                                            //根据可视化功能ID获取该模板全部数据
                                            var relationFormModel = await _visualDevRepository.GetFirstAsync(v => v.Id == model.modelId);
                                            FormDataModel relationFormData = relationFormModel.FormData.Deserialize<FormDataModel>();
                                            //多虑多余控件
                                            var newFields = this.TemplateDataConversion(relationFormData.fields);
                                            var newFieLdsModelList = newFields.FindAll(x => model.relationField.Equals(x.__vModel__));
                                            VisualDevModelListQueryInput listQueryInput = new VisualDevModelListQueryInput
                                            {
                                                dataType = "1",
                                                sidx = columnDesign.defaultSidx,
                                                sort = columnDesign.sort
                                            };
                                            relationFormDataList = (await GetListResult(relationFormModel, listQueryInput)).list.ToList();
                                            _cacheManager.Set(redisName, relationFormDataList);
                                            redisList.Add(redisName);
                                        }
                                        var relationFormRealData = relationFormDataList.Where(it => it["id"].Equals(dataMap[key])).FirstOrDefault();
                                        if (relationFormRealData != null && relationFormRealData.Count > 0)
                                        {
                                            dataMap[key + "_id"] = relationFormRealData["id"];
                                            dataMap[key] = relationFormRealData[model.relationField];
                                        }
                                    }
                                    break;
                                case HszKeyConst.POPUPSELECT:
                                    {
                                        //获取远端数据
                                        var dynamic = await _dataInterfaceService.GetInfo(model.interfaceId);
                                        if (dynamic == null)
                                            break;
                                        switch (dynamic.DataType)
                                        {
                                            //SQL数据
                                            case 1:
                                                {
                                                    _dataInterfaceService.ReplaceParameterValue(dynamic, new Dictionary<string, string>());
                                                    var pObj = await _dataInterfaceService.GetData(dynamic);
                                                    var popupsList = pObj.Serialize().Deserialize<List<Dictionary<string, string>>>();

                                                    var specificData = popupsList.Where(it => it.ContainsKey(model.propsValue) && it.ContainsValue(dataMap[key].ToString())).FirstOrDefault();
                                                    if (specificData != null)
                                                    {
                                                        if (model.relationField.IsNullOrEmpty()) //要用模板的 “显示字段 - relationField”来展示数据
                                                        {
                                                            var showField = model.columnOptions.First();
                                                            dataMap[key] = specificData[showField.value];
                                                        }
                                                        else
                                                        {
                                                            dataMap[key + "_id"] = dataValue;
                                                            dataMap[key] = specificData[model.relationField];
                                                        }
                                                    }
                                                }
                                                break;
                                            //静态数据
                                            case 2:
                                                {
                                                    List<Dictionary<string, string>> dynamicList = new List<Dictionary<string, string>>();
                                                    var value = model.props.props.value;
                                                    var label = model.props.props.label;
                                                    var children = model.props.props.children;
                                                    JToken dynamicDataList = JValue.Parse(dynamic.Query);
                                                    foreach (var data in dynamicDataList)
                                                    {
                                                        Dictionary<string, string> dic = new Dictionary<string, string>();
                                                        dic[value] = data.Value<string>(value);
                                                        dic[label] = data.Value<string>(label);
                                                        dynamicList.Add(dic);
                                                        if (data.Value<object>(children) != null && data.Value<object>(children).ToString() != "")
                                                        {
                                                            dynamicList.AddRange(GetDynamicInfiniteData(data.Value<object>(children).ToString(), model.props.props));
                                                        }
                                                    }
                                                    List<string> dataList = dataMap[key].Serialize().Deserialize<List<string>>();
                                                    List<string> cascaderList = new List<string>();
                                                    foreach (var items in dataList)
                                                    {
                                                        var vara = dynamicList.Where(a => a.ContainsValue(items)).FirstOrDefault();
                                                        if (vara != null)
                                                        {
                                                            cascaderList.Add(vara[label]);
                                                        }
                                                    }

                                                    dataMap[key + "_id"] = dataValue;
                                                    if (actionType == "List")//列表直接输出字符串 按 , 号 隔开
                                                        dataMap[key] = string.Join(model.separator, cascaderList);
                                                    else
                                                        dataMap[key] = cascaderList;
                                                }
                                                break;
                                            //Api数据
                                            case 3:
                                                {
                                                    var interFace = dynamic.Path;
                                                    var response = await interFace.GetAsStringAsync();//请求接口
                                                    var result = JSON.Deserialize<RESTfulResult<object>>(response);
                                                    var data = result.data?.ToString().Deserialize<Dictionary<string, List<dynamic>>>().FirstOrDefault().Value;

                                                    List<object> cascaderList = new List<object>();

                                                    if (data != null)
                                                    {
                                                        //model.propsValue 存储数据ID
                                                        //model.relationField 存储显示数据
                                                        data.ForEach(obj =>
                                                        {
                                                            if (obj[model.propsValue] == dataValue.ToString())
                                                                cascaderList.Add(obj[model.relationField]);
                                                        });
                                                    }

                                                    dataMap[key + "_id"] = dataValue;
                                                    dataMap[key] = string.Join(model.separator, cascaderList);
                                                }
                                                break;
                                            default:
                                                break;
                                        }

                                        //var popupsList = (await _dataInterfaceService.GetData(model.interfaceId)).Serialize().Deserialize<List<Dictionary<string, string>>>();
                                        //var specificData = popupsList.Where(it => it.ContainsKey(model.propsValue) && it.ContainsValue(dataMap[key].ToString())).FirstOrDefault();

                                        //if (specificData != null)
                                        //{
                                        //    if (model.relationField.IsNullOrEmpty()) //要用模板的 “显示字段 - relationField”来展示数据
                                        //    {
                                        //        var showField = model.columnOptions.First();
                                        //        dataMap[key] = specificData[showField.value];
                                        //    }
                                        //    else
                                        //    {
                                        //        dataMap[key] = specificData[model.relationField];
                                        //    }
                                        //}
                                    }
                                    break;
                            }
                        }
                    }
                }
                entity.Data = dataMap.Serialize();
            }
            //为数据实时性，接口请求完后删除Redis缓存
            if (redisList != null)
            {
                foreach (var items in redisList)
                {
                    _cacheManager.Del(items);
                }
            }
            //转换查询数据
            if (keywordJsonDic != null)
            {
                int dicCount = keywordJsonDic.Keys.Count;
                string[] strKey = new string[dicCount];
                keywordJsonDic.Keys.CopyTo(strKey, 0);
                for (int i = 0; i < strKey.Length; i++)
                {
                    var form = formData.Where(f => f.__vModel__ == strKey[i]).FirstOrDefault();
                    //范围查询跳过数据转换
                    var scopeList = new List<string>();
                    scopeList.Add(HszKeyConst.TIMERANGE);
                    scopeList.Add(HszKeyConst.DATE);
                    scopeList.Add(HszKeyConst.TIME);
                    scopeList.Add(HszKeyConst.DATERANGE);
                    scopeList.Add("createTime");
                    scopeList.Add("modifyTime");
                    scopeList.Add("numInput");
                    var hszKey = form.__config__.hszKey;
                    var isScope = scopeList.Find(s => s.Equals(hszKey));
                    if (form != null && isScope == null)
                    {
                        if (form.__vModel__.Contains(form.__config__.hszKey + "Field"))
                        {
                            keywordJsonDic[strKey[i]] = TemplateControlsDataConversion(keywordJsonDic[strKey[i]], form);
                        }
                        else
                        {
                            keywordJsonDic[strKey[i]] = TemplateControlsDataConversion(keywordJsonDic[strKey[i]], form, "query");
                        }
                        var templateValue = templateData.Where(t => t.Key.Equals(form.__config__.renderKey + "_" + strKey[i])).FirstOrDefault();
                        //转换后的数据值
                        var dataDicValue = keywordJsonDic[strKey[i]];
                        if (templateValue.Key != null && !(dataDicValue is null) && dataDicValue.ToString() != "[]")
                        {
                            var moreValue = dataDicValue as IEnumerable<object>;
                            //不是List数据直接赋值
                            if (moreValue == null) hszKey = "";
                            if (string.IsNullOrWhiteSpace(form.separator)) form.separator = ";";

                            switch (hszKey)
                            {
                                case HszKeyConst.COMSELECT:
                                    {
                                        //StringBuilder addName = new StringBuilder();
                                        var addName = new List<string>();
                                        List<Dictionary<string, string[]>> dicAdd = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string[]>>>();
                                        foreach (var item in moreValue)
                                        {
                                            var comData = dicAdd.Where(a => a.ContainsKey(item.ToString())).FirstOrDefault();
                                            if (comData != null) addName.Add(comData[item.ToString()]?.LastOrDefault());
                                        }
                                        if (addName.Count != 0)
                                        {
                                            keywordJsonDic[strKey[i]] = string.Join(form.separator, addName);
                                            //keywordJsonDic[strKey[i]] = addName.ToString().TrimEnd(',');
                                        }
                                    }
                                    break;
                                case HszKeyConst.DEPSELECT:
                                    {
                                        //StringBuilder addName = new StringBuilder();
                                        var addName = new List<string>();
                                        List<Dictionary<string, string>> dicAdd = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>();
                                        foreach (var item in moreValue)
                                        {
                                            var comData = dicAdd.Where(a => a.ContainsKey(item.ToString())).FirstOrDefault();
                                            if (comData != null)
                                            {
                                                addName.Add(comData[item.ToString()]);
                                            }
                                        }
                                        if (addName.Count != 0)
                                        {
                                            keywordJsonDic[strKey[i]] = string.Join(form.separator, addName);
                                            //keywordJsonDic[strKey[i]] = addName.ToString().TrimEnd(',');
                                        }
                                    }
                                    break;
                                case HszKeyConst.ADDRESS:
                                    {
                                        var addressValues = new List<List<object>>();
                                        if (dataDicValue.ToString().Contains("[[")) addressValues = dataDicValue.ToString().Deserialize<List<List<object>>>();
                                        else addressValues = new List<List<object>>() { moreValue.ToList() };

                                        var addNames = new List<string>();

                                        //StringBuilder addName = new StringBuilder();
                                        if (addressTemplateValue.Count < 1) addressTemplateValue = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>();

                                        foreach (var items in addressValues)
                                        {
                                            var addName = new List<string>();
                                            foreach (var item in items)
                                            {
                                                var addressData = addressTemplateValue.Where(a => a.ContainsKey(item.ToString())).FirstOrDefault();
                                                if (addressData != null) addName.Add(addressData[item.ToString()]);
                                            }
                                            addNames.Add(string.Join("/", addName));
                                        }
                                        if (addNames.Count != 0)
                                        {
                                            keywordJsonDic[strKey[i]] = string.Join(form.separator, addNames);
                                            //dataMap[strKey[i]] = addName.ToString();
                                        }
                                    }
                                    break;
                                case HszKeyConst.CASCADER:
                                    {
                                        //StringBuilder addName = new StringBuilder();
                                        var addName = new List<string>();
                                        List<Dictionary<string, string>> dicAdd = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>();
                                        if (form.props.props.multiple)
                                        {
                                            foreach (var item in moreValue)
                                            {
                                                var sb = new List<string>();
                                                foreach (var items in item.Serialize().Deserialize<List<string>>())
                                                {
                                                    var cascaderData = dicAdd.Where(c => c.ContainsKey(items.ToString())).FirstOrDefault();
                                                    if (cascaderData != null)
                                                    {
                                                        sb.Add(cascaderData[items.ToString()]);
                                                    }
                                                }
                                                if (sb.Count != 0)
                                                {
                                                    addName.Add(string.Join("/", sb));
                                                }
                                            }
                                            if (addName.Count != 0)
                                            {
                                                keywordJsonDic[strKey[i]] = string.Join(form.separator, addName);
                                                //keywordJsonDic[strKey[i]] = addName.ToString().TrimEnd(',');
                                            }
                                        }
                                        else
                                        {
                                            foreach (var item in moreValue)
                                            {
                                                var cascaderData = dicAdd.Where(c => c.ContainsKey(item.ToString())).FirstOrDefault();
                                                if (cascaderData != null)
                                                {
                                                    addName.Add(cascaderData[item.ToString()]);
                                                }
                                            }
                                            if (addName.Count != 0)
                                            {
                                                keywordJsonDic[strKey[i]] = string.Join(form.separator, addName);
                                                //keywordJsonDic[strKey[i]] = addName.ToString().TrimEnd('/');
                                            }
                                        }
                                    }
                                    break;
                                case HszKeyConst.CHECKBOX:
                                    {
                                        //StringBuilder addName = new StringBuilder();
                                        var addName = new List<string>();
                                        List<Dictionary<string, string>> dicAdd = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>();
                                        foreach (var item in moreValue)
                                        {
                                            var cascaderData = dicAdd.Where(c => c.ContainsKey(item.ToString())).FirstOrDefault();
                                            if (cascaderData != null)
                                            {
                                                addName.Add(cascaderData[item.ToString()]);
                                            }
                                        }
                                        if (addName.Count != 0)
                                        {
                                            keywordJsonDic[strKey[i]] = string.Join(form.separator, addName);
                                            //keywordJsonDic[strKey[i]] = addName.ToString().TrimEnd(',');
                                        }
                                    }
                                    break;
                                case HszKeyConst.USERSELECT:
                                    {
                                        //StringBuilder userName = new StringBuilder();
                                        var userName = new List<string>();
                                        List<Dictionary<string, string>> dicUser = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>();
                                        foreach (var item in moreValue)
                                        {
                                            var cascaderData = dicUser.Where(c => c.ContainsKey(item.ToString())).FirstOrDefault();
                                            if (cascaderData != null)
                                            {
                                                userName.Add(cascaderData[item.ToString()]);
                                            }
                                        }
                                        if (userName.Count != 0)
                                        {
                                            keywordJsonDic[strKey[i]] = string.Join(form.separator, userName);
                                            //keywordJsonDic[strKey[i]] = userName.ToString().TrimEnd(',');
                                        }
                                    }
                                    break;
                                case HszKeyConst.POSSELECT:
                                    {
                                        //StringBuilder posName = new StringBuilder();
                                        var posName = new List<string>();
                                        List<Dictionary<string, string>> dicPos = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>();
                                        foreach (var item in moreValue)
                                        {
                                            var cascaderData = dicPos.Where(c => c.ContainsKey(item.ToString())).FirstOrDefault();
                                            if (cascaderData != null)
                                            {
                                                posName.Add(cascaderData[item.ToString()]);
                                            }
                                        }
                                        if (posName.Count != 0)
                                        {
                                            keywordJsonDic[strKey[i]] = string.Join(form.separator, posName);
                                            //keywordJsonDic[strKey[i]] = posName.ToString().TrimEnd(',');
                                        }
                                    }
                                    break;
                                case HszKeyConst.SELECT:
                                    {
                                        //StringBuilder selectName = new StringBuilder();
                                        var selectName = new List<string>();
                                        List<Dictionary<string, string>> dicSelect = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>();
                                        foreach (var item in moreValue)
                                        {
                                            var cascaderData = dicSelect.Where(c => c.ContainsKey(item.ToString())).FirstOrDefault();
                                            if (cascaderData != null)
                                            {
                                                selectName.Add(cascaderData[item.ToString()]);
                                            }
                                        }
                                        if (selectName.Count != 0)
                                        {
                                            keywordJsonDic[strKey[i]] = string.Join(form.separator, selectName);
                                            //keywordJsonDic[strKey[i]] = selectName.ToString().TrimEnd(',');
                                        }
                                    }
                                    break;
                                case HszKeyConst.TREESELECT:
                                    {
                                        //StringBuilder treeSelectName = new StringBuilder();
                                        var treeSelectName = new List<string>();
                                        List<Dictionary<string, string>> treeSelectDic = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>();
                                        foreach (var item in moreValue)
                                        {
                                            var cascaderData = treeSelectDic.Where(c => c.ContainsKey(item.ToString())).FirstOrDefault();
                                            if (cascaderData != null)
                                            {
                                                treeSelectName.Add(cascaderData[item.ToString()]);
                                            }
                                        }
                                        if (treeSelectName.Count != 0)
                                        {
                                            keywordJsonDic[strKey[i]] = string.Join(form.separator, treeSelectName);
                                            //keywordJsonDic[strKey[i]] = treeSelectName.ToString().TrimEnd(',');
                                        }
                                    }
                                    break;
                                default:
                                    {
                                        var convertList = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>();
                                        var convertData = convertList.Where(t => t.ContainsKey(keywordJsonDic[strKey[i]].ToString())).FirstOrDefault();
                                        if (convertData != null)
                                        {
                                            keywordJsonDic[strKey[i]] = convertData.Values.FirstOrDefault().ToString();
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
                //转换查询条件没记录的数据
                var record = keywordJsonDic.Keys.Except(templateData.Keys.Select(x => x = x.Substring(14, x.Length - 14))).ToList();
                foreach (var key in record)
                {
                    var model = formData.Where(f => f.__vModel__ == key).FirstOrDefault();
                    ConfigModel configModel = model.__config__;
                    string type = configModel.hszKey;
                    switch (type)
                    {
                        //时间范围
                        case HszKeyConst.TIMERANGE:
                            {
                                List<string> jsonArray = keywordJsonDic[key].Serialize().Deserialize<List<string>>();
                                string value1 = string.Format("{0:HH:mm:ss}", jsonArray[0].ToDate());
                                string value2 = string.Format("{0:HH:mm:ss}", jsonArray[1].ToDate());
                                jsonArray.Clear();
                                jsonArray.Add(value1 + "至");
                                jsonArray.Add(value2);
                                keywordJsonDic[key] = jsonArray;
                            }
                            break;
                        //日期选择
                        case HszKeyConst.DATE:
                            {
                                var jsonArray = keywordJsonDic[key].Serialize().Deserialize<List<string>>();
                                string value1 = string.Format("{0:" + model.format + "}", Ext.GetDateTime(jsonArray[0]));
                                string value2 = string.Format("{0:" + model.format + "}", Ext.GetDateTime(jsonArray[1]));
                                jsonArray.Clear();
                                jsonArray.Add(value1 + "至");
                                jsonArray.Add(value2 + "至");
                                keywordJsonDic[key] = jsonArray;
                            }
                            break;
                        //日期范围
                        case HszKeyConst.DATERANGE:
                            {
                                List<string> jsonArray = keywordJsonDic[key].Serialize().Deserialize<List<string>>();
                                string value1 = string.Format("{0:yyyy-MM-dd}", Ext.GetDateTime(jsonArray[0]));
                                string value2 = string.Format("{0:yyyy-MM-dd}", Ext.GetDateTime(jsonArray[1]));
                                jsonArray.Clear();
                                jsonArray.Add(value1 + "至");
                                jsonArray.Add(value2);
                                keywordJsonDic[key] = jsonArray;
                            }
                            break;
                        case "createUser":
                            {
                                var userEntity = _userService.GetInfoByUserId(keywordJsonDic[key].ToString());
                                keywordJsonDic[key] = userEntity.RealName;
                            }
                            break;
                        case "createTime":
                            {
                                var jsonArray = keywordJsonDic[key].ToObject<List<string>>();
                                string value1 = string.Format("{0:yyyy-MM-dd 00:00:00}", Ext.GetDateTime(jsonArray[0]));
                                string value2 = string.Format("{0:yyyy-MM-dd 23:59:59}", Ext.GetDateTime(jsonArray[1]));
                                jsonArray.Clear();
                                jsonArray.Add(value1 + "至");
                                jsonArray.Add(value2 + "至");
                                keywordJsonDic[key] = jsonArray;
                            }
                            break;
                        case "modifyUser":
                            {
                                var userEntity = _userService.GetInfoByUserId(keywordJsonDic[key].ToString());
                                keywordJsonDic[key] = userEntity.RealName;
                            }
                            break;
                        case "modifyTime":
                            {
                                var jsonArray = keywordJsonDic[key].ToObject<List<string>>();
                                string value1 = string.Format("{0:yyyy-MM-dd 00:00:00}", Ext.GetDateTime(jsonArray[0]));
                                string value2 = string.Format("{0:yyyy-MM-dd 23:59:59}", Ext.GetDateTime(jsonArray[1]));
                                jsonArray.Clear();
                                jsonArray.Add(value1 + "至");
                                jsonArray.Add(value2 + "至");
                                keywordJsonDic[key] = jsonArray;
                            }
                            break;
                        case "currDept":
                            {
                                var currDept = _departmentService.GetDepName(keywordJsonDic[key].ToString());
                                keywordJsonDic[key] = currDept;
                            }
                            break;
                        case "currPosition":
                            {
                                var currPosition = _positionService.GetName(keywordJsonDic[key].ToString());
                                keywordJsonDic[key] = currPosition;
                            }
                            break;
                        case "cascader":
                            {
                                switch (configModel.dataType)
                                {
                                    case "dictionary":
                                        {
                                            List<DictionaryDataEntity> data = new List<DictionaryDataEntity>();
                                            var cacheKey = CommonConst.VISUALDEV + _userManager.TenantId + "_" + configModel.dictionaryType;
                                            if (_cacheManager.Exists(cacheKey))
                                            {
                                                data = _cacheManager.Get(cacheKey).Deserialize<List<DictionaryDataEntity>>();
                                            }
                                            else
                                            {
                                                data = await _dictionaryDataService.GetList(configModel.dictionaryType);
                                                _cacheManager.Set(cacheKey, data);
                                                redisList.Add(cacheKey);
                                            }
                                            List<string> dataList = keywordJsonDic[key].Serialize().Deserialize<List<string>>();
                                            List<string> cascaderList = new List<string>();
                                            foreach (var items in dataList)
                                            {
                                                var cascader = data.Find(d => d.Id == items);
                                                if (cascader != null)
                                                {
                                                    cascaderList.Add(cascader.FullName);
                                                }
                                            }
                                            keywordJsonDic[key] = cascaderList;
                                        }
                                        break;
                                    case "dynamic":
                                        {
                                            //获取远端数据
                                            //DataInterfaceEntity dynamic = await dataInterfaceService.GetInfo(configModel.propsUrl);
                                            //if (1.Equals(dynamic.DataType))
                                            //{
                                            //    var linkEntity = await dbLinkservice.GetInfo(dynamic.DBLinkId);
                                            //    var dt = await databaseService.GetInterFaceData(linkEntity, dynamic.Query);
                                            //    List<Dictionary<string, object>> dynamicDataList = UtilConvert.DataTableToDicList(dt);
                                            //    List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                                            //    foreach (var item in dynamicDataList)
                                            //    {
                                            //        Dictionary<string, string> dynamicDic = new Dictionary<string, string>();
                                            //        dynamicDic.Add(item[model.__config__.props.value].ToString(), item[model.__config__.props.label].ToString());
                                            //        list.Add(dynamicDic);
                                            //    }
                                            //    templateData.Add(fieldName1, list);
                                            //}
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }

                            break;
                    }
                }
            }
            Dictionary<string, object> map = new Dictionary<string, object>();
            map[vModelType.LIST.GetDescription()] = list;
            map[vModelType.KEYJSONMAP.GetDescription()] = keywordJsonDic;
            return map;
        }

        #endregion

        #region 数据转换与筛选

        /// <summary>
        /// 无表的数据筛选
        /// </summary>
        /// <param name="list"></param>
        /// <param name="keyJsonMap"></param>
        /// <param name="formData"></param>
        /// <returns></returns>
        private List<VisualDevModelDataEntity> GetNoTableFilteringData(List<VisualDevModelDataEntity> list, Dictionary<string, object> keyJsonMap, List<FieldsModel> formData)
        {
            List<VisualDevModelDataEntity> realList = new List<VisualDevModelDataEntity>();
            foreach (var entity in list)
            {
                Dictionary<String, object> query = keyJsonMap;
                Dictionary<String, object> realEntity = entity.Data.Deserialize<Dictionary<string, object>>();
                if (query != null && query.Count != 0)
                {
                    int m = 0;
                    int dicCount = query.Keys.Count;
                    string[] strKey = new string[dicCount];
                    query.Keys.CopyTo(strKey, 0);
                    for (int i = 0; i < strKey.Length; i++)
                    {
                        var keyValue = keyJsonMap[strKey[i]];
                        var queryEntity = realEntity.Where(e => e.Key == strKey[i]).FirstOrDefault();
                        var model = formData.Where(f => f.__vModel__ == strKey[i]).FirstOrDefault();
                        if (queryEntity.Value != null && !string.IsNullOrWhiteSpace(keyValue.ToString()))
                        {
                            var realValue = queryEntity.Value;
                            var type = model.__config__.hszKey;
                            switch (type)
                            {
                                case HszKeyConst.DATE:
                                    {
                                        List<String> queryTime = keyValue.ToObject<List<string>>();
                                        int formatType = 0;
                                        if (model.format == "yyyy-MM")
                                        {
                                            formatType = 1;
                                        }
                                        else if (model.format == "yyyy")
                                        {
                                            formatType = 2;
                                        }
                                        string value1 = string.Format("{0:yyyy-MM-dd}", Ext.GetDateTime(queryTime.First()));
                                        string value2 = string.Format("{0:yyyy-MM-dd}", Ext.GetDateTime(queryTime.Last()));
                                        if (Ext.IsInTimeRange(Ext.GetDateTime(realValue.ToString()), value1, value2, formatType))
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                case HszKeyConst.TIME:
                                    {
                                        var queryTime = new List<string>();
                                        keyValue.ToObject<List<string>>().ForEach(item => {
                                            if (!string.IsNullOrWhiteSpace(item)) queryTime.Add(Ext.GetDateTime(item).ToLongTimeString());
                                        });

                                        if (Ext.IsInTimeRange(realValue.ToDate(), queryTime.First(), queryTime.Last(), 3))
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                case "createTime":
                                    {
                                        List<String> dayTime1 = keyValue.ToObject<List<string>>();
                                        string value1 = string.Format("{0:yyyy-MM-dd 00:00:00}", Ext.GetDateTime(dayTime1.First()));
                                        string value2 = string.Format("{0:yyyy-MM-dd 23:59:59}", Ext.GetDateTime(dayTime1.Last()));
                                        if (Ext.IsInTimeRange(Convert.ToDateTime(realValue), value1, value2))
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                case "modifyTime":
                                    {
                                        List<String> dayTime1 = keyValue.ToObject<List<string>>();
                                        string value1 = string.Format("{0:yyyy-MM-dd 00:00:00}", Ext.GetDateTime(dayTime1.First()));
                                        string value2 = string.Format("{0:yyyy-MM-dd 23:59:59}", Ext.GetDateTime(dayTime1.Last()));
                                        if (!string.IsNullOrEmpty(realValue.ToString()))
                                        {
                                            if (Ext.IsInTimeRange(Convert.ToDateTime(realValue), value1, value2))
                                            {
                                                realEntity["id"] = entity.Id;
                                                m++;
                                            }
                                        }
                                    }
                                    break;
                                case "numInput":
                                    {
                                        List<string> numArray = keyValue.ToObject<List<string>>();
                                        var numA = numArray.First().ToInt();
                                        var numB = numArray.Last() == null ? Int64.MaxValue : numArray.Last().ToInt();
                                        var numC = realValue.ToInt();
                                        if (numC >= numA && numC <= numB)
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                case "calculate":
                                    {
                                        List<string> numArray = keyValue.ToObject<List<string>>();
                                        var numA = numArray.First().ToInt();
                                        var numB = numArray.Last() == null ? Int64.MaxValue : numArray.Last().ToInt();
                                        var numC = realValue.ToInt();
                                        if (numC >= numA && numC <= numB)
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                default:
                                    {
                                        if (model.searchType == 2)
                                        {
                                            if (realValue.ToString().Contains(keyValue.ToString()))
                                            {
                                                realEntity["id"] = entity.Id;
                                                m++;
                                            }
                                        }
                                        else if (model.searchType == 1)
                                        {
                                            //多选时为模糊查询
                                            if (model.multiple || type == "checkbox")
                                            {
                                                if (realValue.ToString().Contains(keyValue.ToString()))
                                                {
                                                    realEntity["id"] = entity.Id;
                                                    m++;
                                                }
                                            }
                                            else
                                            {
                                                if (realValue.ToString().Equals(keyValue.ToString()))
                                                {
                                                    realEntity["id"] = entity.Id;
                                                    m++;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (realValue.ToString().Replace(" ", "").Contains(keyValue.ToString().Replace(" ", "")))
                                            {
                                                realEntity["id"] = entity.Id;
                                                m++;
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                        if (m == dicCount)
                        {
                            realList.Add(entity);
                        }
                    }
                }
                else
                {
                    realList.Add(entity);
                }
            }
            return realList;
        }

        /// <summary>
        /// 查询过滤数据
        /// </summary>
        /// <param name="keyJsonMap"></param>
        /// <param name="list"></param>
        /// <param name="formData"></param>
        /// <returns></returns>
        private List<Dictionary<string, object>> GetQueryFilteringData(Dictionary<string, object> keyJsonMap, List<VisualDevModelDataEntity> list, ColumnDesignModel columnDesignModel)
        {
            List<Dictionary<string, object>> realList = new List<Dictionary<string, object>>();
            foreach (var entity in list)
            {
                Dictionary<String, object> query = keyJsonMap;
                Dictionary<String, object> realEntity = entity.Data.Deserialize<Dictionary<String, object>>();
                var formData = columnDesignModel.searchList;
                if (query != null && query.Count != 0)
                {
                    //添加关键词全匹配计数，全符合条件则添加
                    int m = 0;
                    int dicCount = query.Keys.Count;
                    string[] strKey = new string[dicCount];
                    query.Keys.CopyTo(strKey, 0);
                    for (int i = 0; i < strKey.Length; i++)
                    {
                        var keyValue = keyJsonMap[strKey[i]];
                        var queryEntity = realEntity.Where(e => e.Key == strKey[i]).FirstOrDefault();
                        var model = formData.Where(f => f.__vModel__ == strKey[i]).FirstOrDefault();
                        if (queryEntity.Value != null && !string.IsNullOrWhiteSpace(keyValue.ToString()))
                        {
                            var realValue = queryEntity.Value;
                            var type = model.__config__.hszKey;
                            switch (type)
                            {
                                case HszKeyConst.DATERANGE:
                                    {
                                        List<String> dayTime1 = keyValue.Serialize().Deserialize<List<string>>();
                                        List<String> dayTime2 = realValue.Serialize().Deserialize<List<string>>();
                                        var newList1 = new List<string> { dayTime1[0].TrimEnd('至') };
                                        var newList2 = new List<string> { dayTime2[0].TrimEnd('至') };
                                        dayTime1.RemoveRange(0, 1);
                                        dayTime2.RemoveRange(0, 1);
                                        dayTime1.InsertRange(0, newList1);
                                        dayTime2.InsertRange(0, newList2);
                                        bool cont1 = Ext.timeCalendar(dayTime2[0], dayTime1[0], dayTime1[1]);
                                        bool cont2 = Ext.timeCalendar(dayTime2[1], dayTime1[0], dayTime1[1]);
                                        if (cont1 || cont2)
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                case HszKeyConst.DATE:
                                    {
                                        List<String> queryTime = keyValue.ToObject<List<string>>();
                                        if (Ext.IsInTimeRange(realValue.ToDate(), queryTime[0].TrimEnd('至'), queryTime[1].TrimEnd('至')))
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                case HszKeyConst.TIME:
                                    {
                                        List<String> queryTime = keyValue.ToObject<List<string>>();
                                        if (Ext.IsInTimeRange(realValue.ToDate(), queryTime[0], queryTime[1]))
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                case HszKeyConst.TIMERANGE:
                                    {
                                        List<String> dayTime1 = keyValue.Serialize().Deserialize<List<string>>();
                                        List<String> dayTime2 = realValue.Serialize().Deserialize<List<string>>();
                                        if (dayTime2 != null)
                                        {
                                            var newList1 = new List<string> { dayTime1[0].TrimEnd('至') };
                                            var newList2 = new List<string> { dayTime2[0].TrimEnd('至') };
                                            dayTime1.RemoveRange(0, 1);
                                            dayTime2.RemoveRange(0, 1);
                                            dayTime1.InsertRange(0, newList1);
                                            dayTime2.InsertRange(0, newList2);
                                            bool cont1 = Ext.timeCalendar(dayTime2[0], dayTime1[0], dayTime1[1]);
                                            bool cont2 = Ext.timeCalendar(dayTime2[1], dayTime1[0], dayTime1[1]);
                                            if (cont1 || cont2)
                                            {
                                                realEntity["id"] = entity.Id;
                                                m++;
                                            }
                                        }
                                    }
                                    break;
                                case "createTime":
                                    {
                                        List<String> dayTime1 = keyValue.ToObject<List<string>>();
                                        if (Ext.IsInTimeRange(realValue.ToDate(), dayTime1[0].TrimEnd('至'), dayTime1.Last().TrimEnd('至')))
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                case "modifyTime":
                                    {
                                        List<String> dayTime1 = keyValue.ToObject<List<string>>();
                                        if (Ext.IsInTimeRange(realValue.ToDate(), dayTime1[0].TrimEnd('至'), dayTime1.Last().TrimEnd('至')))
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                case "numInput":
                                    {
                                        List<string> numArray = keyValue.ToObject<List<string>>();

                                        var numA = numArray.First().ToInt();
                                        var numB = numArray.Last() == null ? Int64.MaxValue : numArray.Last().ToInt();
                                        var numC = realValue.ToInt();
                                        if (numC >= numA && numC <= numB)
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                default:
                                    {
                                        if (model.searchType == 2)
                                        {
                                            if (realValue.ToString().Contains(keyValue.ToString()))
                                            {
                                                realEntity["id"] = entity.Id;
                                                m++;
                                            }
                                        }
                                        else if (model.searchType == 1)
                                        {
                                            //多选时为模糊查询
                                            if (model.multiple || type == "checkbox")
                                            {
                                                if (realValue.ToString().Contains(keyValue.ToString()))
                                                {
                                                    realEntity["id"] = entity.Id;
                                                    m++;
                                                }
                                            }
                                            else
                                            {
                                                if (realValue.ToString().Equals(keyValue.ToString()))
                                                {
                                                    realEntity["id"] = entity.Id;
                                                    m++;
                                                }
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                        if (m == dicCount)
                        {
                            realList.Add(realEntity);
                        }
                    }
                }
                else
                {
                    realEntity["id"] = entity.Id;
                    realList.Add(realEntity);
                }
            }
            return realList;
        }

        /// <summary>
        /// 查询转换格式
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<Dictionary<string, object>> GetQueryDataConversion(List<VisualDevModelDataEntity> list)
        {
            List<Dictionary<string, object>> realList = new List<Dictionary<string, object>>();
            foreach (var entity in list)
            {
                Dictionary<String, object> realEntity = entity.Data.Deserialize<Dictionary<String, object>>();
                realEntity["id"] = entity.Id;
                realList.Add(realEntity);
            }
            return realList;
        }

        #endregion

        /// <summary>
        /// 获取无表列表数据
        /// </summary>
        /// <param name="modelId">模板ID</param>
        /// <returns></returns>
        private async Task<List<VisualDevModelDataEntity>> GetList(string modelId)
        {
            return await _visualDevModelDataRepository.AsQueryable().Where(m => m.VisualDevId == modelId && m.DeleteMark == null).ToListAsync();
        }

        /// <summary>
        /// options无限级
        /// </summary>
        /// <returns></returns>
        private List<OptionsModel> GetTreeOptions(List<object> model, PropsBeanModel props)
        {
            List<OptionsModel> options = new List<OptionsModel>();
            foreach (var item in model)
            {
                OptionsModel option = new OptionsModel();
                var dicObject = item.Serialize().Deserialize<Dictionary<string, object>>();
                option.label = dicObject[props.label].ToString();
                option.value = dicObject[props.value].ToString();
                if (dicObject.ContainsKey(props.children))
                {
                    var children = dicObject[props.children].Serialize().Deserialize<List<object>>();
                    options.AddRange(GetTreeOptions(children, props));
                }
                options.Add(option);
            }
            return options;
        }

        /// <summary>
        /// 获取动态无限级数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="props"></param>
        /// <returns></returns>
        private List<Dictionary<string, string>> GetDynamicInfiniteData(string data, PropsBeanModel props)
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            var value = props.value;
            var label = props.label;
            var children = props.children;
            JToken dynamicDataList = JValue.Parse(data);
            foreach (var info in dynamicDataList)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic[value] = info.Value<string>(value);
                dic[label] = info.Value<string>(label);
                list.Add(dic);
                if (info.Value<object>(children) != null && info.Value<object>(children).ToString() != "")
                {
                    list.AddRange(GetDynamicInfiniteData(info.Value<object>(children).ToString(), props));
                }
            }
            return list;
        }

        /// <summary>
        /// 将有表转无表
        /// </summary>
        /// <param name="dicList"></param>
        /// <param name="mainPrimary"></param>
        /// <returns></returns>
        private List<VisualDevModelDataEntity> GetTableDataList(List<Dictionary<string, object>> dicList, string mainPrimary)
        {
            List<VisualDevModelDataEntity> list = new List<VisualDevModelDataEntity>();
            foreach (var dataMap in dicList)
            {
                VisualDevModelDataEntity entity = new VisualDevModelDataEntity();
                //需要将小写的转为大写
                entity.Data = dataMap.ToJson();
                entity.Id = dataMap[mainPrimary].ToString();
                list.Add(entity);
            }
            return list;
        }

        /// <summary>
        /// 获取有表单条数据
        /// </summary>
        /// <param name="main"></param>
        /// <returns></returns>
        private List<Dictionary<string, object>> GetTableDataInfo(List<Dictionary<string, object>> dataList, List<FieldsModel> fieldsModels, string actionType)
        {
            //转换表字符串成数组
            foreach (var dataMap in dataList)
            {
                int dicCount = dataMap.Keys.Count;
                string[] strKey = new string[dicCount];
                dataMap.Keys.CopyTo(strKey, 0);
                for (int i = 0; i < strKey.Length; i++)
                {
                    var dataValue = dataMap[strKey[i]];
                    if (dataValue != null && !string.IsNullOrEmpty(dataValue.ToString()) && !dataValue.ToString().Contains("[]"))
                    {
                        var model = fieldsModels.Find(f => f.__vModel__ == strKey[i]);
                        if (model != null)
                        {
                            switch (model.__config__.hszKey)
                            {
                                //日期选择
                                case "date":
                                    {
                                        dataMap[strKey[i]] = TemplateControlsDataConversion(dataMap[strKey[i]], model, actionType);
                                    }
                                    break;
                                //创建时间
                                case "createTime":
                                    {
                                        dataMap[strKey[i]] = TemplateControlsDataConversion(dataMap[strKey[i]], model, actionType);
                                    }
                                    break;
                                //修改选择
                                case "modifyTime":
                                    {
                                        dataMap[strKey[i]] = TemplateControlsDataConversion(dataMap[strKey[i]], model, actionType);
                                    }
                                    break;
                                default:
                                    dataMap[strKey[i]] = TemplateControlsDataConversion(dataMap[strKey[i]], model, actionType);
                                    break;
                            }

                        }
                    }
                }
            }
            return dataList;
        }

        /// <summary>
        /// 转换字典列表内是否有时间戳
        /// </summary>
        /// <param name="dicList"></param>
        /// <returns></returns>
        private List<Dictionary<string, object>> GetTableDataListByDic(List<Dictionary<string, object>> dicList, List<FieldsModel> fieldsModels)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            foreach (var dataMap in dicList)
            {
                int dicCount = dataMap.Keys.Count;
                string[] strKey = new string[dicCount];
                dataMap.Keys.CopyTo(strKey, 0);
                for (int i = 0; i < strKey.Length; i++)
                {
                    var dataValue = dataMap[strKey[i]];
                    if (dataValue != null && !string.IsNullOrEmpty(dataValue.ToString()))
                    {
                        var model = fieldsModels.Find(f => f.__vModel__ == strKey[i]);
                        if (model != null)
                        {
                            dataMap[strKey[i]] = TemplateControlsDataConversion(dataMap[strKey[i]], model, "transition");
                        }
                    }
                }
                list.Add(dataMap);
            }
            return list;
        }

        /// <summary>
        /// 获取有表单条数据 转时间戳
        /// </summary>
        /// <param name="main"></param>
        /// <param name="fieldsModels"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        private List<Dictionary<string, object>> GetTableDataInfoByTimeStamp(List<Dictionary<string, object>> dataList, List<FieldsModel> fieldsModels, string actionType)
        {
            //转换表字符串成数组
            foreach (var dataMap in dataList)
            {
                int dicCount = dataMap.Keys.Count;
                string[] strKey = new string[dicCount];
                dataMap.Keys.CopyTo(strKey, 0);
                for (int i = 0; i < strKey.Length; i++)
                {
                    var dataValue = dataMap[strKey[i]];
                    if (dataValue != null && !string.IsNullOrEmpty(dataValue.ToString()))
                    {
                        var model = fieldsModels.Find(f => f.__vModel__.Equals(strKey[i]));
                        if (model != null)
                        {
                            dataMap[strKey[i]] = TemplateControlsDataConversion(dataMap[strKey[i]], model, actionType);
                        }
                    }
                }
            }
            return dataList;
        }

        /// <summary>
        /// 解析 处理 条形码和二维码
        /// </summary>
        /// <param name="fieldsModels"></param>
        private void GetBARAndQR(List<FieldsModel> fieldsModels, Dictionary<string, object> _newDataMap, Dictionary<string, object> _dataMap)
        {
            //处理 条形码 、 二维码 控件
            fieldsModels.Where(x => x.__config__.hszKey == "barcode" || x.__config__.hszKey == "qrcode").Where(x => !string.IsNullOrWhiteSpace(x.relationField)).ToList().ForEach(item =>
            {
                if (!_newDataMap.ContainsKey(item.relationField + "_id") && _dataMap.ContainsKey(item.relationField))
                    _newDataMap.Add(item.relationField + "_id", _dataMap[item.relationField]);
            });
        }
        #endregion

        #region PublicMethod

        /// <summary>
        /// 模板数据转换
        /// </summary>
        /// <param name="fieldsModelList"></param>
        /// <returns></returns>
        public List<FieldsModel> TemplateDataConversion(List<FieldsModel> fieldsModelList)
        {
            var template = new List<FieldsModel>();
            //将模板内的无限children解析出来
            //不包含子表children
            foreach (var item in fieldsModelList)
            {
                var config = item.__config__;
                switch (config.hszKey)
                {
                    //栅格布局
                    case "row":
                        {
                            template.AddRange(TemplateDataConversion(config.children));
                        }
                        break;
                    //表格
                    case "table":
                        {
                            template.Add(item);
                        }
                        break;
                    //卡片
                    case "card":
                        {
                            template.AddRange(TemplateDataConversion(config.children));
                        }
                        break;
                    //折叠面板
                    case "collapse":
                        {
                            foreach (var collapse in config.children)
                            {
                                template.AddRange(TemplateDataConversion(collapse.__config__.children));
                            }
                        }
                        break;
                    case "tab":
                        foreach (var collapse in config.children)
                        {
                            template.AddRange(TemplateDataConversion(collapse.__config__.children));
                        }
                        break;
                    //文本
                    case "HSZText":
                        break;
                    //分割线
                    case "divider":
                        break;
                    //分组标题
                    case "groupTitle":
                        break;
                    //按钮
                    case "button":
                        break;
                    default:
                        {
                            template.Add(item);
                        }
                        break;
                }
            }
            return template;
        }

        /// <summary>
        /// 获取有表详情系统组件数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modelList"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> GetTableInfoBySystemComponentsData(string id, List<FieldsModel> modelList, string data)
        {
            //获取Redis缓存模板数据
            var templateData = await GetVisualDevTemplateData(id, modelList);
            data = await GetSystemComponentsData(modelList, templateData, data);
            return data;
        }

        /// <summary>
        /// 删除无表数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DelIsNoTableInfo(string id, VisualDevEntity templateEntity)
        {
            var ids = new List<string>();
            ids.Add(id);
            if (templateEntity.WebType == 3)
            {
                Scoped.Create((_, scope) =>
                {
                    var services = scope.ServiceProvider;

                    var _flowTaskRepository = App.GetService<IFlowTaskRepository>(services);
                    var notAllow = _flowTaskRepository.GetAllowDeleteFlowTaskList(ids);
                    ids = ids.Except(notAllow).ToList();
                });
            }
            id = ids.FirstOrDefault();
            if (id != null)
            {
                var entity = await _visualDevModelDataRepository.GetFirstAsync(v => v.Id == id && v.DeleteMark == null);
                _ = entity ?? throw HSZException.Oh(ErrorCode.COM1007);

                try
                {
                    //开启事务
                    _db.BeginTran();

                    //删除无表表数据
                    await _visualDevModelDataRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).UpdateColumns(it => new { it.DeleteMark, it.DeleteTime, it.DeleteUserId }).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();

                    if (templateEntity.WebType == 3)
                    {
                        Scoped.Create((_, scope) =>
                        {
                            var services = scope.ServiceProvider;
                            var _flowTaskRepository = App.GetService<IFlowTaskRepository>(services);
                            var entity = _flowTaskRepository.GetTaskInfoNoAsync(id);
                            if (entity != null)
                                _flowTaskRepository.DeleteTaskNoAsync(entity);
                        });
                    }

                    //关闭事务
                    _db.CommitTran();
                }
                catch (Exception)
                {
                    _db.RollbackTran();
                    throw;
                }
            }
        }

        /// <summary>
        /// 批量删除无表数据
        /// </summary>
        /// <param name="ids">ID数组</param>
        /// <returns></returns>
        public async Task BatchDelIsNoTableData(List<string> ids, VisualDevEntity templateEntity)
        {
            ids = ids.Where(i => i != null).ToList();
            if (templateEntity.WebType == 3)
            {
                Scoped.Create((_, scope) =>
                {
                    var services = scope.ServiceProvider;

                    var _flowTaskRepository = App.GetService<IFlowTaskRepository>(services);
                    var notAllow = _flowTaskRepository.GetAllowDeleteFlowTaskList(ids);
                    ids = ids.Except(notAllow).ToList();
                });
            }
            if (ids.Count > 0)
            {
                var dataList = await _visualDevModelDataRepository.AsQueryable().In(r => r.Id, ids.Where(i => i != null).ToArray()).Where(v => v.DeleteMark == null).ToListAsync();

                try
                {
                    //开启事务
                    _db.BeginTran();

                    //删除有表数据
                    await _visualDevModelDataRepository.AsUpdateable(dataList).UpdateColumns(it => new { it.DeleteMark, it.DeleteTime, it.DeleteUserId }).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();

                    if (templateEntity.WebType == 3)
                    {
                        Scoped.Create((_, scope) =>
                        {
                            var services = scope.ServiceProvider;

                            var _flowTaskRepository = App.GetService<IFlowTaskRepository>(services);
                            ids.ForEach(it =>
                            {
                                var entity = _flowTaskRepository.GetTaskInfoNoAsync(it);
                                if (entity != null)
                                    _flowTaskRepository.DeleteTaskNoAsync(entity);
                            });
                        });
                    }

                    //关闭事务
                    _db.CommitTran();
                }
                catch (Exception)
                {
                    _db.RollbackTran();
                    throw;
                }

            }
        }

        /// <summary>
        /// 删除有表信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="templateEntity">模板实体</param>
        /// <returns></returns>
        public async Task DelHaveTableInfo(string id, VisualDevEntity templateEntity)
        {
            var ids = new List<string>();
            ids.Add(id);
            if (templateEntity.WebType == 3)
            {
                Scoped.Create((_, scope) =>
                {
                    var services = scope.ServiceProvider;

                    var _flowTaskRepository = App.GetService<IFlowTaskRepository>(services);
                    var notAllow = _flowTaskRepository.GetAllowDeleteFlowTaskList(ids);
                    ids = ids.Except(notAllow).ToList();
                });
            }
            id = ids.FirstOrDefault();
            if (ids.Count > 0)
            {
                List<TableModel> tableMapList = templateEntity.Tables.ToObject<List<TableModel>>();
                //获取详情模板
                FormDataModel formDataModel = TemplateKeywordsHelper.ReplaceKeywords(templateEntity.FormData).ToObject<FormDataModel>();
                List<FieldsModel> formData = formDataModel.fields;

                var tableList = new List<DbTableFieldModel>();
                //获取主表
                var mainTable = tableMapList.Find(t => t.relationTable == "");

                var link = await _dbLinkService.GetInfo(templateEntity.DbLinkId);
                if (link == null)
                    link = GetTenantDbLink();
                tableList = _databaseService.GetFieldList(link, mainTable.table);
                //获取主键
                var mainPrimary = tableList.Find(t => t.primaryKey == 1);
                //拼接语句
                var allDelSql = new List<string>();
                //查询主表信息语句
                StringBuilder queryMain = new StringBuilder();
                allDelSql.Add(string.Format("delete from {0} where {1} = '{2}';", mainTable.table, mainPrimary.field, id));
                queryMain.AppendFormat("select * from {0} where {1}='{2}';", tableMapList.FirstOrDefault().table, mainPrimary.field, id);
                var mainData = _databaseService.GetInterFaceData(link, queryMain.ToString()).Serialize().Deserialize<List<Dictionary<string, object>>>();
                List<Dictionary<string, object>> mainMapList = GetTableDataInfo(mainData, formData, null);
                if (mainMapList.Count > 0)
                {
                    if (tableMapList.Count > 1)
                    {
                        //去除主表
                        tableMapList.Remove(mainTable);
                        foreach (var tableMap in tableMapList)
                        {
                            //主表字段
                            string relationField = tableMap.relationField;
                            string relationFieldValue = mainMapList.First()[relationField].ToString();
                            //子表字段
                            string tableField = tableMap.tableField;
                            allDelSql.Add(string.Format("delete from {0} where {1} = '{2}';", tableMap.table, tableField, relationFieldValue));
                        }
                    }

                    try
                    {
                        //开启事务
                        _db.BeginTran();

                        //删除有表数据
                        foreach (var item in allDelSql)
                        {
                            await _databaseService.ExecuteSql(link, item);
                        }

                        if (templateEntity.WebType == 3)
                        {
                            Scoped.Create((_, scope) =>
                            {
                                var services = scope.ServiceProvider;
                                var _flowTaskRepository = App.GetService<IFlowTaskRepository>(services);
                                var entity = _flowTaskRepository.GetTaskInfoNoAsync(id);
                                if (entity != null)
                                    _flowTaskRepository.DeleteTaskNoAsync(entity);
                            });
                        }

                        //关闭事务
                        _db.CommitTran();
                    }
                    catch (Exception)
                    {
                        _db.RollbackTran();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 批量删除有表数据
        /// </summary>
        /// <param name="ids">id数组</param>
        /// <param name="templateEntity">模板实体</param>
        /// <returns></returns>
        public async Task BatchDelHaveTableData(List<string> ids, VisualDevEntity templateEntity)
        {
            if (templateEntity.WebType == 3)
            {
                Scoped.Create((_, scope) =>
                {
                    var services = scope.ServiceProvider;

                    var _flowTaskRepository = App.GetService<IFlowTaskRepository>(services);
                    var notAllow = _flowTaskRepository.GetAllowDeleteFlowTaskList(ids);
                    ids = ids.Except(notAllow).ToList();
                });
            }
            if (ids.Count > 0)
            {
                List<TableModel> tableMapList = templateEntity.Tables.ToObject<List<TableModel>>();
                //获取详情模板
                FormDataModel formDataModel = TemplateKeywordsHelper.ReplaceKeywords(templateEntity.FormData).ToObject<FormDataModel>();
                List<FieldsModel> formData = formDataModel.fields;

                var tableList = new List<DbTableFieldModel>();
                //获取主表
                var mainTable = tableMapList.Find(t => t.relationTable == "");

                var link = await _dbLinkService.GetInfo(templateEntity.DbLinkId);
                if (link == null)
                    link = GetTenantDbLink();
                tableList = _databaseService.GetFieldList(link, mainTable.table);
                //获取主键
                var mainPrimary = tableList.Find(t => t.primaryKey == 1);
                //拼接语句
                //主表删除语句
                StringBuilder delMain = new StringBuilder();
                //总删除语句
                StringBuilder allDelSql = new StringBuilder();
                //查询主表信息语句
                StringBuilder queryMain = new StringBuilder();
                //子表删除语句
                StringBuilder childSql = new StringBuilder();
                delMain.AppendFormat("delete from {0} where {1} in ('{2}');", mainTable.table, mainPrimary.field, string.Join("','", ids.ToArray()));
                allDelSql.Append(delMain);
                queryMain.AppendFormat("select * from {0} where {1} in ('{2}');", tableMapList.FirstOrDefault().table, mainPrimary.field, string.Join("','", ids.ToArray()));
                var mainData = _databaseService.GetInterFaceData(link, queryMain.ToString()).Serialize().Deserialize<List<Dictionary<string, object>>>();
                List<Dictionary<string, object>> mainMapList = GetTableDataInfo(mainData, formData, null);
                if (mainMapList.Count > 0)
                {
                    if (tableMapList.Count > 1)
                    {
                        //去除主表
                        tableMapList.Remove(mainTable);
                        foreach (var tableMap in tableMapList)
                        {
                            //主表字段
                            string relationField = tableMap.relationField;
                            string relationFieldValue = mainMapList.First()[relationField].ToString();
                            //子表字段
                            string tableField = tableMap.tableField;
                            childSql.AppendFormat("delete from {0} where {1} = '{2}';", tableMap.table, tableField, relationFieldValue);
                            allDelSql.Append(childSql);
                        }
                    }

                    try
                    {
                        //开启事务
                        _db.BeginTran();

                        //删除有表数据
                        await _databaseService.ExecuteSql(link, allDelSql.ToString());

                        if (templateEntity.WebType == 3)
                        {
                            Scoped.Create((_, scope) =>
                            {
                                var services = scope.ServiceProvider;

                                var _flowTaskRepository = App.GetService<IFlowTaskRepository>(services);
                                ids.ForEach(it =>
                                {
                                    var entity = _flowTaskRepository.GetTaskInfoNoAsync(it);
                                    if (entity != null)
                                        _flowTaskRepository.DeleteTaskNoAsync(entity);
                                });
                            });
                        }

                        //关闭事务
                        _db.CommitTran();
                    }
                    catch (Exception)
                    {
                        _db.RollbackTran();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 动态表单时间格式转换
        /// </summary>
        /// <param name="diclist"></param>
        /// <returns></returns>
        private List<Dictionary<string, object>> DateConver(List<Dictionary<string, object>> diclist)
        {
            foreach (var item in diclist)
            {
                foreach (var dic in item.Keys)
                {
                    if (item[dic] is DateTime)
                    {
                        item[dic] = item[dic].ToString() + " ";
                    }
                }
            }
            return diclist;
        }

        /// <summary>
        /// 获取弹窗选择 数据列表
        /// </summary>
        /// <param name="interfaceId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task <List<Dictionary<string, string>>> GetPopupSelectDataList(string interfaceId, FieldsModel model )
        {
            var result = new List<Dictionary<string, string>>();

            //获取远端数据
            var dynamic = await _dataInterfaceService.GetInfo(interfaceId);
            if (dynamic == null) return null;
            switch (dynamic.DataType)
            {
                //SQL数据
                case 1:
                    {
                        _dataInterfaceService.ReplaceParameterValue(dynamic, new Dictionary<string, string>());
                        var pObj = await _dataInterfaceService.GetData(dynamic);
                        result = pObj.Serialize().Deserialize<List<Dictionary<string, string>>>();
                    }
                    break;
                //静态数据
                case 2:
                    {
                        List<Dictionary<string, string>> dynamicList = new List<Dictionary<string, string>>();
                        var value = model.props.props.value;
                        var label = model.props.props.label;
                        var children = model.props.props.children;
                        JToken dynamicDataList = JValue.Parse(dynamic.Query);
                        foreach (var data in dynamicDataList)
                        {
                            Dictionary<string, string> dic = new Dictionary<string, string>();
                            dic[value] = data.Value<string>(value);
                            dic[label] = data.Value<string>(label);
                            dynamicList.Add(dic);
                            if (data.Value<object>(children) != null && data.Value<object>(children).ToString() != "")
                            {
                                dynamicList.AddRange(GetDynamicInfiniteData(data.Value<object>(children).ToString(), model.props.props));
                            }
                        }

                        result = dynamicList;
                    }
                    break;
                //Api数据
                case 3:
                    {
                        var interFace = dynamic.Path;
                        var response = await interFace.GetAsStringAsync();//请求接口
                        var res = JSON.Deserialize<RESTfulResult<object>>(response);
                        var data = res.data?.ToString().Deserialize<Dictionary<string, List<Dictionary<string, string>>>>().FirstOrDefault().Value;
                        result = data;
                    }
                    break;
                default:
                    break;
            }

            return result;
        }
        #endregion
    }


    /// <summary>
    /// 在线开发 模板解析帮助 类
    /// </summary>
    public class TemplateParsingHelp
    {
        /// <summary>
        /// 是否有表 (true 有表, false 无表)
        /// </summary>
        public bool IsHasTable { get; set; }

        /// <summary>
        /// 表单配置JSON模型
        /// </summary>
        public FormDataModel FormModel { get; set; }

        /// <summary>
        /// 列配置JSON模型
        /// </summary>
        public ColumnDesignModel ColumnData { get; set; }

        /// <summary>
        /// 所有控件集合
        /// </summary>
        public List<FieldsModel> AllFieldsModel { get; set; }

        /// <summary>
        /// 所有控件集合(已剔除布局控件)
        /// </summary>
        public List<FieldsModel> FieldsModelList { get; set; }

        /// <summary>
        /// 主表控件集合
        /// </summary>
        public List<FieldsModel> MainTableFieldsModelList { get; set; }

        /// <summary>
        /// 副表控件集合
        /// </summary>
        public List<FieldsModel> AuxiliaryTableFieldsModelList { get; set; }

        /// <summary>
        /// 子表控件集合
        /// </summary>
        public List<FieldsModel> ChildTableFieldsModelList { get; set; }

        /// <summary>
        /// 所有非子表控件集合
        /// </summary>
        public List<FieldsModel> SingleFormData { get; set; }

        /// <summary>
        /// 所有表
        /// </summary>
        public List<TableModel> AllTable { get; set; }

        /// <summary>
        /// 主表 表名
        /// </summary>
        public string MainTableName { get; set; }

        /// <summary>
        /// 在线开发 模板解析帮助 构造
        /// </summary>
        /// <param name="entity">模板实体</param>
        public TemplateParsingHelp(VisualDevEntity entity, bool isFlowTask = false)
        {
            FormDataModel FormModel = !isFlowTask ? TemplateKeywordsHelper.ReplaceKeywords(entity.FormData).ToObject<FormDataModel>() : entity.FormData.ToObject<FormDataModel>();
            IsHasTable = !string.IsNullOrEmpty(entity.Tables) && !"[]".Equals(entity.Tables);//是否有表
            AllFieldsModel = FormModel.fields;//所有控件集合
            FieldsModelList = TemplateDataConvert(FormModel.fields);//已剔除布局控件集合
            MainTableName = entity.Tables.ToString().Deserialize<List<TableModel>>().Find(m => m.relationTable == "")?.table;//主表名称
            //处理旧控件 部分没有 tableName
            FieldsModelList.Where(x => string.IsNullOrWhiteSpace(x.__config__.tableName)).ToList().ForEach(item =>
            {
                if (item.__vModel__.Contains("_hsz_")) item.__config__.tableName = item.__vModel__.ReplaceRegex(@"_hsz_(\w+)", "").Replace("hsz_", "");//副表
                else item.__config__.tableName = MainTableName;//主表
            });
            AllTable= entity.Tables.ToObject<List<TableModel>>();//所有表
            AuxiliaryTableFieldsModelList = FieldsModelList.Where(x => x.__vModel__.Contains("_hsz_")).ToList();//单控件副表集合
            ChildTableFieldsModelList = FieldsModelList.Where(x => x.__config__.hszKey == "table").ToList();//子表集合
            MainTableFieldsModelList = FieldsModelList.Except(AuxiliaryTableFieldsModelList).Except(ChildTableFieldsModelList).ToList();//主表控件集合
            SingleFormData= FieldsModelList.Where(x => x.__config__.hszKey != "table").ToList();//非子表集合

            if (!string.IsNullOrWhiteSpace(entity.ColumnData)) ColumnData = TemplateKeywordsHelper.ReplaceKeywords(entity.ColumnData).Deserialize<ColumnDesignModel>();//列配置模型
            else ColumnData = new ColumnDesignModel();
        }

        /// <summary>
        /// 验证模板
        /// </summary>
        /// <returns>true 通过</returns>
        public bool VerifyTemplate()
        {
            if (FieldsModelList.Count(x => x.__config__.hszKey == "table") > 0)
            {
                foreach (var item in ChildTableFieldsModelList)
                {
                    var tc = AuxiliaryTableFieldsModelList.Find(x => x.__vModel__.Contains(item.__config__.tableName + "_hsz_"));
                    if (tc != null) return false;
                };
            }

            return true;
        }

        /// <summary>
        /// 时间控件 查询时转换：DateTime.MaxValue + 查询时间
        /// </summary>
        /// <param name="queryJson"></param>
        /// <returns>queryJson</returns>
        public string TimeControlQueryConvert(string queryJson)
        {
            if (!string.IsNullOrWhiteSpace(queryJson))
            {
                var mainFormData = FieldsModelList.Where(x => x.__config__.hszKey == "time").ToList();//获取所有 时间 控件
                var newJson = new Dictionary<string, object>();

                var sjson = queryJson.ToObject<Dictionary<string, object>>().ToList();
                sjson.ForEach(item =>
                {
                    var vlist = new List<object>();
                    var vmodel = mainFormData.Find(x => x.__vModel__ == item.Key);
                    if (vmodel != null)
                    {
                        item.Value.ToJson().Deserialize<List<string>>().ForEach(it =>
                        {
                            vlist.Add(Ext.ConvertToTimeStamp((DateTime.MaxValue.ToDateString() + " " + it).ToDate()));
                        });
                        newJson.Add(item.Key, vlist);
                    }
                    else
                    {
                        newJson.Add(item.Key, item.Value);
                    }
                });

                return newJson.Serialize();
            }

            return queryJson;
        }


        /// <summary>
        /// 模板数据转换
        /// </summary>
        /// <param name="fieldsModelList"></param>
        /// <returns></returns>
        private List<FieldsModel> TemplateDataConvert(List<FieldsModel> fieldsModelList)
        {
            var template = new List<FieldsModel>();
            //将模板内的无限children解析出来
            //不包含子表children
            foreach (var item in fieldsModelList)
            {
                var config = item.__config__;
                switch (config.hszKey)
                {
                    //栅格布局
                    case "row":
                        {
                            template.AddRange(TemplateDataConvert(config.children));
                        }
                        break;
                    //表格
                    case "table":
                        {
                            template.Add(item);
                        }
                        break;
                    //卡片
                    case "card":
                        {
                            template.AddRange(TemplateDataConvert(config.children));
                        }
                        break;
                    //折叠面板
                    case "collapse":
                        {
                            foreach (var collapse in config.children)
                            {
                                template.AddRange(TemplateDataConvert(collapse.__config__.children));
                            }
                        }
                        break;
                    case "tab":
                        foreach (var collapse in config.children)
                        {
                            template.AddRange(TemplateDataConvert(collapse.__config__.children));
                        }
                        break;
                    //文本
                    case "HSZText":
                        break;
                    //分割线
                    case "divider":
                        break;
                    //分组标题
                    case "groupTitle":
                        break;
                    default:
                        {
                            template.Add(item);
                        }
                        break;
                }
            }
            return template;
        }

    }
}
