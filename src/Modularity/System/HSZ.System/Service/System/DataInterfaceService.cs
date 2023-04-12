using HSZ.ChangeDataBase;
using HSZ.Common.Configuration;
using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Common.Helper;
using HSZ.Common.Model;
using HSZ.DataEncryption;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.JsonSerialization;
using HSZ.LinqBuilder;
using HSZ.Logging.Attributes;
using HSZ.RemoteRequest.Extensions;
using HSZ.SensitiveDetection;
using HSZ.System.Entitys.Dto.System.DataInterFace;
using HSZ.System.Entitys.Model.System.DataInterFace;
using HSZ.System.Entitys.Permission;
using HSZ.System.Entitys.System;
using HSZ.System.Interfaces.Common;
using HSZ.System.Interfaces.System;
using HSZ.UnifyResult;
using HSZ.VisualDev.Entitys.Dto.VisualDev;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UAParser;
using Yitter.IdGenerator;

namespace HSZ.System.Core.Service.DataInterFace
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：数据接口
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "DataInterface", Order = 204)]
    [Route("api/system/[controller]")]
    public class DataInterfaceService : IDataInterfaceService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<DataInterfaceEntity> _dataInterfaceRepository;
        private readonly IDictionaryDataService _dictionaryDataService;
        private readonly IChangeDataBase _changeDataBase;
        private readonly IUserManager _userManager;
        private readonly IFileService _fileService;
        private readonly SqlSugarScope Db;
        private readonly ISensitiveDetectionProvider _sensitiveDetectionProvider;
        private string configId = App.Configuration["ConnectionStrings:ConfigId"];
        private string dbName = App.Configuration["ConnectionStrings:netcore_test"];

        /// <summary>
        /// 初始化一个<see cref="DataInterfaceService"/>类型的新实例
        /// </summary>
        public DataInterfaceService(ISqlSugarRepository<DataInterfaceEntity> dataInterfaceRepository,
            IDictionaryDataService dictionaryDataService,
            IChangeDataBase changeDataBase,
            IUserManager userManager,
            IFileService fileService,
            ISensitiveDetectionProvider sensitiveDetectionProvider)
        {
            _dataInterfaceRepository = dataInterfaceRepository;
            _dictionaryDataService = dictionaryDataService;
            _changeDataBase = changeDataBase;
            _userManager = userManager;
            _fileService = fileService;
            Db = DbScoped.SugarScope;
            _sensitiveDetectionProvider = sensitiveDetectionProvider;
        }

        #region Get

        /// <summary>
        /// 获取接口列表(分页)
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList_Api([FromQuery] DataInterfaceListQuery input)
        {
            var pageInput = input.Adapt<PageInputBase>();
            var queryWhere = LinqExpression.And<DataInterfaceListOutput>();
            if (!string.IsNullOrEmpty(input.categoryId))
                queryWhere = queryWhere.And(m => m.categoryId == input.categoryId);
            //关键字（名称、编码）
            if (!string.IsNullOrEmpty(input.keyword))
                queryWhere = queryWhere.And(m => m.fullName.Contains(input.keyword) || m.enCode.Contains(input.keyword));
            var list = await _dataInterfaceRepository.AsSugarClient().Queryable<DataInterfaceEntity, UserEntity>((a, b) =>
            new JoinQueryInfos(JoinType.Left, b.Id == a.CreatorUserId)).Where(a => a.DeleteMark == null).Select((a, b) =>
                new DataInterfaceListOutput
                {
                    id = a.Id,
                    categoryId = a.CategoryId,
                    creatorTime = a.CreatorTime,
                    creatorUser = SqlFunc.MergeString(b.RealName, "/", b.Account),
                    dataType = a.DataType,
                    dbLinkId = a.DBLinkId,
                    description = a.Description,
                    enCode = a.EnCode,
                    fullName = a.FullName,
                    enabledMark = a.EnabledMark,
                    path = a.Path,
                    query = a.Query,
                    requestMethod = SqlFunc.IF(a.RequestMethod.Equals("1")).Return("新增").ElseIF(a.RequestMethod.Equals("2")).Return("修改")
                    .ElseIF(a.RequestMethod.Equals("3")).Return("查询").ElseIF(a.RequestMethod.Equals("4")).Return("删除")
                    .ElseIF(a.RequestMethod.Equals("5")).Return("存储过程").ElseIF(a.RequestMethod.Equals("6")).Return("Get请求")
                    .End("Post请求"),
                    requestParameters = a.RequestParameters,
                    responseType = a.ResponseType,
                    sortCode = a.SortCode,
                    checkType = a.CheckType,
                    tenantId = _userManager.TenantId
                }).MergeTable().Where(queryWhere).OrderBy(x => x.sortCode)
            .OrderBy(t => t.creatorTime, OrderByType.Desc).OrderByIF(!string.IsNullOrEmpty(input.keyword), t => t.lastModifyTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<DataInterfaceListOutput>.SqlSugarPageResult(list);
        }

        /// <summary>
        /// 获取接口列表(分页)
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpGet("getList")]
        public async Task<dynamic> getList([FromQuery] DataInterfaceListQuery input)
        {
            var pageInput = input.Adapt<PageInputBase>();
            var queryWhere = LinqExpression.And<DateInterfaceGetListOutput>();
            if (!string.IsNullOrEmpty(input.categoryId))
                queryWhere = queryWhere.And(m => m.categoryId == input.categoryId);
            if (!string.IsNullOrEmpty(input.dataType))
                queryWhere = queryWhere.And(m => m._dataType.ToString() == input.dataType);
            //关键字（名称、编码）
            if (!string.IsNullOrEmpty(input.keyword))
                queryWhere = queryWhere.And(m => m.fullName.Contains(input.keyword) || m.enCode.Contains(input.keyword));
            var list = await _dataInterfaceRepository.AsSugarClient().Queryable<DataInterfaceEntity, UserEntity>((a, b) =>
            new JoinQueryInfos(JoinType.Left, b.Id == a.CreatorUserId)).Where(a => a.DeleteMark == null).Select((a, b) =>
                new DateInterfaceGetListOutput
                {
                    id = a.Id,
                    categoryId = a.CategoryId,
                    creatorTime = a.CreatorTime,
                    creatorUser = SqlFunc.MergeString(b.RealName, "/", b.Account),
                    _dataType = a.DataType,
                    dbLinkId = a.DBLinkId,
                    description = a.Description,
                    enCode = a.EnCode,
                    fullName = a.FullName,
                    enabledMark = a.EnabledMark,
                    path = a.Path,
                    query = a.Query,
                    requestMethod = SqlFunc.IF(a.RequestMethod.Equals("1")).Return("新增").ElseIF(a.RequestMethod.Equals("2")).Return("修改")
                    .ElseIF(a.RequestMethod.Equals("3")).Return("查询").ElseIF(a.RequestMethod.Equals("4")).Return("删除")
                    .ElseIF(a.RequestMethod.Equals("5")).Return("存储过程").ElseIF(a.RequestMethod.Equals("6")).Return("Get请求")
                    .End("Post请求"),
                    requestParameters = a.RequestParameters,
                    responseType = a.ResponseType,
                    sortCode = a.SortCode,
                    checkType = a.CheckType,
                    tenantId = _userManager.TenantId
                }).MergeTable().Where(queryWhere).OrderBy(x => x.sortCode)
            .OrderBy(t => t.creatorTime, OrderByType.Desc).OrderByIF(!string.IsNullOrEmpty(input.keyword), t => t.lastModifyTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<DateInterfaceGetListOutput>.SqlSugarPageResult(list);
        }

        /// <summary>
        /// 获取接口列表下拉框
        /// </summary>
        /// <returns></returns>
        [HttpGet("Selector")]
        public async Task<dynamic> GetSelector()
        {
            List<DataInterfaceSelectorOutput> tree = new List<DataInterfaceSelectorOutput>();
            var data = (await GetList()).FindAll(x => x.EnabledMark == 1).OrderBy(x => x.SortCode).ToList();
            foreach (var entity in data)
            {
                var dictionaryDataEntity = await _dictionaryDataService.GetInfo(entity.CategoryId);
                if (dictionaryDataEntity != null && tree.Where(t => t.id == entity.CategoryId).Count() == 0)
                {
                    DataInterfaceSelectorOutput firstModel = dictionaryDataEntity.Adapt<DataInterfaceSelectorOutput>();
                    firstModel.categoryId = "0";
                    DataInterfaceSelectorOutput treeModel = entity.Adapt<DataInterfaceSelectorOutput>();
                    treeModel.categoryId = "1";
                    treeModel.parentId = dictionaryDataEntity.Id;
                    firstModel.children.Add(treeModel);
                    tree.Add(firstModel);
                }
                else
                {
                    DataInterfaceSelectorOutput treeModel = entity.Adapt<DataInterfaceSelectorOutput>();
                    treeModel.categoryId = "1";
                    treeModel.parentId = entity.CategoryId;
                    var parent = tree.Where(t => t.id == entity.CategoryId).FirstOrDefault();
                    if (parent != null)
                    {
                        parent.children.Add(treeModel);
                    }
                }
            }
            return tree.OrderBy(x => x.sortCode).ToList();
        }

        /// <summary>
        /// 获取接口数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo_Api(string id)
        {
            var data = (await GetInfo(id)).Adapt<DataInterfaceInfoOutput>();
            return data;
        }

        /// <summary>
        /// 预览接口
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/Preview")]
        public async Task<dynamic> Preview(string id)
        {
            object output = null;
            var info = await GetInfo(id);
            ReplaceParameterValue(info, new Dictionary<string, string>());
            if (info.DataType == 1)
            {
                if (await _sensitiveDetectionProvider.VaildedAsync(info.Query.ToUpper()))
                    throw HSZException.Oh(ErrorCode.xg1005);
                output = await GetData(info);
            }
            else if (info.DataType == 2)
            {
                output = JSON.Deserialize<object>(info.Query);
            }
            else
            {
                output = await GetApiDataByTypePreview(info);
            }
            return new { data = output, dataProcessing = info.DataProcessing };
        }

        /// <summary>
        /// 访问接口
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tenantId">有值则为地址请求，没有则是内部请求</param>
        /// <returns></returns>
        [AllowAnonymous]
        [IgnoreLog]
        [HttpGet("{id}/Actions/Response")]
        public async Task<dynamic> ActionsResponse(string id, [FromQuery] string tenantId)
        {
            return await GetResponseByType(id, 2, tenantId);
        }

        /// <summary>
        /// 访问接口 分页
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [IgnoreLog]
        [HttpGet("{id}/Action/List")]
        public async Task<dynamic> ActionsResponseList(string id, [FromQuery] string tenantId, [FromQuery] VisualDevDataFieldDataListInput input)
        {
            return await GetResponseByType(id, 0, tenantId, input);
        }


        /// <summary>
        /// 访问接口 选中 回写
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [IgnoreLog]
        [HttpGet("{id}/Action/Info")]
        public async Task<dynamic> ActionsResponseInfo(string id, [FromQuery] string tenantId, [FromQuery] VisualDevDataFieldDataListInput input)
        {
            return await GetResponseByType(id, 1, tenantId, input);
        }


        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/Action/Export")]
        public async Task<dynamic> ActionsExport(string id)
        {
            var data = await GetInfo(id);
            var jsonStr = data.Serialize();
            return _fileService.Export(jsonStr, data.FullName);
        }

        #endregion

        #region Post

        /// <summary>
        /// 添加接口
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create_Api([FromBody] DataInterfaceCrInput input)
        {
            var entity = input.Adapt<DataInterfaceEntity>();
            var isOk = await Create(entity);
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 修改接口
        /// </summary>
        /// <param name="id">主键id</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update_Api(string id, [FromBody] DataInterfaceUpInput input)
        {
            var entity = input.Adapt<DataInterfaceEntity>();
            var isOk = await Update(entity);
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除接口
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete_Api(string id)
        {
            var entity = await GetInfo(id);
            var isOk = await Delete(entity);
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1002);
        }

        /// <summary>
        /// 更新接口状态
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/State")]
        public async Task UpdateState_Api(string id)
        {
            var entity = await GetInfo(id);
            entity.EnabledMark = entity.EnabledMark == 1 ? 0 : 1;
            var isOk = await Update(entity);
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1003);
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("Action/Import")]
        public async Task ActionsImport(IFormFile file)
        {
            var fileType = Path.GetExtension(file.FileName).Replace(".", "");
            if (!fileType.ToLower().Equals("json"))
                throw HSZException.Oh(ErrorCode.D3006);
            var josn = _fileService.Import(file);
            var data = josn.Deserialize<DataInterfaceEntity>();
            if (data == null)
                throw HSZException.Oh(ErrorCode.D3006);
            await ImportData(data);
        }

        #endregion

        #region PublicMethod

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<DataInterfaceEntity>> GetList()
        {
            return await _dataInterfaceRepository.AsQueryable().Where(x => x.DeleteMark == null).OrderBy(x => x.SortCode).ToListAsync();
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        [NonAction]
        public async Task<DataInterfaceEntity> GetInfo(string id)
        {
            return await _dataInterfaceRepository.GetFirstAsync(x => x.Id == id && x.DeleteMark == null);
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Create(DataInterfaceEntity entity)
        {
            return await _dataInterfaceRepository.AsInsertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Delete(DataInterfaceEntity entity)
        {
            return await _dataInterfaceRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Update(DataInterfaceEntity entity)
        {
            return await _dataInterfaceRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<DataTable> GetData(DataInterfaceEntity entity)
        {
            var result = await connection(entity.DBLinkId, entity.Query, entity.RequestMethod);
            return result;
        }

        /// <summary>
        /// 查询(工作流)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<DataTable> GetData(string id)
        {
            var data = await _dataInterfaceRepository.GetFirstAsync(x => x.Id == id && x.DeleteMark == null);
            var result = await connection(data.DBLinkId, data.Query, data.RequestMethod);
            return result;
        }

        /// <summary>
        /// 根据不同类型请求接口
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type">0 ： 分页 1 ：详情 ，其他 原始</param>
        /// <param name="tenantId"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<object> GetResponseByType(string id, int type, string tenantId, VisualDevDataFieldDataListInput input = null, Dictionary<string, string> dicParameters = null)
        {
            try
            {
                var sw = new Stopwatch();
                sw.Start();
                if (KeyVariable.MultiTenancy)
                {
                    tenantId = tenantId.IsNullOrEmpty() ? _userManager.TenantId : tenantId;
                    var interFace = App.Configuration["HSZ_App:MultiTenancyDBInterFace"] + tenantId;
                    var response = await interFace.GetAsStringAsync();
                    var result = JSON.Deserialize<RESTfulResult<TenantInterFaceOutput>>(response);
                    if (result.code != 200)
                        throw HSZException.Oh(result.msg);
                    else if (result.data.dbName == null)
                        throw HSZException.Oh(ErrorCode.D1025);
                    if (!Db.IsAnyConnection(tenantId))
                    {
                        Db.AddConnection(new ConnectionConfig()
                        {
                            DbType = (SqlSugar.DbType)Enum.Parse(typeof(SqlSugar.DbType), App.Configuration["ConnectionStrings:DBType"]),
                            ConfigId = tenantId,//设置库的唯一标识
                            IsAutoCloseConnection = true,
                            ConnectionString = string.Format($"{App.Configuration["ConnectionStrings:DefaultConnection"]}", result.data.dbName)
                        });
                    }
                    Db.ChangeDatabase(tenantId);
                    configId = tenantId;
                    dbName = result.data.dbName;
                }
                var data = await Db.Queryable<DataInterfaceEntity>().FirstAsync(x => x.Id == id && x.DeleteMark == null);
                if (input.IsNotEmptyOrNull())
                {

                    //重构框架源码，新增多个关键字模糊查询
                    if (!string.IsNullOrWhiteSpace(input.relationField) && !string.IsNullOrWhiteSpace(input.keyword))
                    {
                        string[] temp = input.relationField.Split('#');
                        if (temp.Length >= 1)
                        {
                            data.Query = string.Format("select * from ({0}) t where 1=1 and( ", data.Query.TrimEnd(';'));

                            for (int i = 0; i < temp.Length; i++)
                            {
                                var item = temp[i];
                                if (temp.Length - 1 == i)
                                {
                                    data.Query += string.Format(" {0} like '%{1}%'", item, input.keyword) + ")";
                                }
                                else
                                {
                                    data.Query += string.Format(" {0} like '%{1}%' or", item, input.keyword);
                                }
                            }
                        }
                        else
                        {
                            data.Query = string.Format("select * from ({0}) t where {1} like '%{2}%' ", data.Query.TrimEnd(';'), input.relationField, input.keyword);
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(input.propsValue) && !string.IsNullOrWhiteSpace(input.id))
                        data.Query = string.Format("select * from ({0}) t where {1} like '%{2}%' ", data.Query.TrimEnd(';'), input.propsValue, input.id);


                    if (!input.queryJson.IsNullOrEmpty())
                    {
                        dicParameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(input.queryJson);
                    }
                }
               

                if (dicParameters==null)
                    dicParameters = new Dictionary<string, string>();
                ReplaceParameterValue(data, dicParameters);
                object output = null;

                #region 授权判断
                if (data == null)
                {
                    throw HSZException.Oh(ErrorCode.COM1005);
                }
                else if (data.CheckType == 1)
                {
                    var tokenStr = App.HttpContext.Request.Headers["Authorization"].ToString();
                    if (tokenStr.IsNullOrEmpty())
                        throw HSZException.Oh(ErrorCode.D9007);
                    var token = new JsonWebToken(tokenStr.Replace("Bearer ", ""));
                    var flag = JWTEncryption.ValidateJwtBearerToken((DefaultHttpContext)App.HttpContext, out token);
                    if (!flag)
                        throw HSZException.Oh(ErrorCode.D9007);
                }
                else if (data.CheckType == 2)
                {
                    var ipList = data.IpAddress.Split(",").ToList();
                    if (!ipList.Contains(App.HttpContext.GetLocalIpAddressToIPv4()))
                        throw HSZException.Oh(ErrorCode.D9002);
                }
                #endregion

                #region 调用接口

                if (1.Equals(data.DataType))
                {
                    var resTable = await GetData(data);
                    if (type == 0)
                    {
                        //分页
                        var dt = GetPageToDataTable(resTable, input.currentPage, input.pageSize);
                        var res = new
                        {
                            pagination = new PageResult()
                            {
                                pageIndex = input.currentPage,
                                pageSize = input.pageSize,
                                total = resTable.Rows.Count
                            },
                            list = dt.ToObject<List<Dictionary<string, object>>>(),
                            dataProcessing = data.DataProcessing
                        };

                        output = res;
                    }
                    else if (type == 1)
                    {
                        output = resTable.ToObject<List<Dictionary<string, object>>>().FirstOrDefault();
                    }
                    else
                    {
                        output = new { data = resTable, dataProcessing = data.DataProcessing };
                    }

                }
                else if (2.Equals(data.DataType))
                {

                    output = new { data = JSON.Deserialize<object>(data.Query), dataProcessing = data.DataProcessing };
                }
                else
                {
                    if (type == 0)
                    {
                        output = await GetApiDataPagination(data);
                    }
                    else if (type == 1)
                    {
                        output = await GetApiDataByType(data);
                        var resObj = JSON.Deserialize<object>(output.ToString()).ToObject<JObject>();
                        if (resObj.ContainsKey("list"))
                        {
                            var resList = resObj["list"].Serialize().ToList<Dictionary<string, object>>();
                            var resItem = resList.Find(x => x.ContainsKey(input.propsValue) && x.ContainsValue(input.id));
                            return resItem;
                        }
                    }
                    else
                    {
                        output = new { data = await GetApiDataByType(data), dataProcessing = data.DataProcessing };
                    }
                }
                #endregion
                sw.Stop();

                #region 插入日志
                if (App.HttpContext.IsNotEmptyOrNull())
                {
                    var httpContext = App.HttpContext;
                    var headers = httpContext.Request.Headers;
                    var clientInfo = Parser.GetDefault().Parse(headers["User-Agent"]);
                    var log = new DataInterfaceLogEntity()
                    {
                        Id = YitIdHelper.NextId().ToString(),
                        InvokId = id,
                        InvokTime = DateTime.Now,
                        UserId = _userManager.UserId,
                        InvokIp = httpContext.GetLocalIpAddressToIPv4(),
                        InvokDevice = String.Format("{0}-{1}", clientInfo.OS.ToString(), clientInfo.UA.ToString()),
                        InvokWasteTime = (int)sw.ElapsedMilliseconds,
                        InvokType = httpContext.Request.Method
                    };
                    await Db.Insertable(log).ExecuteCommandAsync();
                }
                #endregion
                return output;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        #endregion

        #region PrivateMethod

        /// <summary>
        /// 通过连接执行sql
        /// </summary>
        /// <returns></returns>
        private async Task<DataTable> connection(string dbLinkId, string sql, string reqMethod)
        {
            var linkEntity = await Db.Queryable<DbLinkEntity>().FirstAsync(x => x.Id == dbLinkId && x.DeleteMark == null);
            if (linkEntity == null)
                linkEntity = await GetTenantDbLink();
            var parameter = new List<SugarParameter>();
            if (_userManager.ToKen != null)
            {
                parameter.Add(new SugarParameter("@user", _userManager.UserId));
                parameter.Add(new SugarParameter("@organize", _userManager.User.OrganizeId));
                parameter.Add(new SugarParameter("@department", _userManager.User.OrganizeId));
                parameter.Add(new SugarParameter("@postion", _userManager.User.PositionId));
            }
            if (reqMethod.Equals("3"))
            {
                var dt = _changeDataBase.GetInterFaceData(linkEntity, sql, parameter.ToArray());
                return dt;
            }
            else
            {
                _changeDataBase.ExecuteCommand(linkEntity, sql, parameter.ToArray());
                return new DataTable();
            }
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task ImportData(DataInterfaceEntity data)
        {
            try
            {
                Db.BeginTran();
                var stor = _dataInterfaceRepository.AsSugarClient().Storageable(data).Saveable().ToStorage(); //存在更新不存在插入 根据主键
                await stor.AsInsertable.ExecuteCommandAsync(); //执行插入
                //await stor.AsUpdateable.ExecuteCommandAsync(); //执行更新，停用原因：Oracle 数据库环境会抛异常：ora-01704: 字符串文字太长
                await _dataInterfaceRepository.AsSugarClient().Updateable(data).ExecuteCommandAsync();//执行更新
                Db.CommitTran();
            }
            catch (Exception ex)
            {
                Db.RollbackTran();
                throw HSZException.Oh(ErrorCode.D3006);
            }
        }

        /// <summary>
        /// 根据不同规则请求接口
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private async Task<object> GetApiDataByType(DataInterfaceEntity entity)
        
        {
            var result = string.Empty;
            var parameters = JSON.Deserialize<List<DataInterfaceReqParameter>>(entity.RequestParameters);
            var parametersHerader = JSON.Deserialize<List<DataInterfaceReqParameter>>(entity.RequestHeaders);
            var dic = new Dictionary<string, object>();
            var dicHerader = new Dictionary<string, object>();
            dicHerader.Add("HSZ_API", true);
            if (entity.CheckType != 0) {
            if (_userManager?.ToKen != null)
            {
                dicHerader.Add("Authorization", _userManager.ToKen);
            }
            }
            foreach (var key in parameters)
            {
                dic.Add(key.field, key.defaultValue);
            }
            foreach (var key in parametersHerader)
            {
                dicHerader[key.field] = key.defaultValue;
            }
            //判断接口是否有域名，没有域名则自动选用当前api接口地址
            try
            {
               
                if (!RegularHelper.Check(entity.Path, RegularHelper.Type.url))
                {
                    entity.Path = KeyVariable.DataInterfaceUrl + entity.Path;
                }
            }
            catch (Exception)
            {

                entity.Path = KeyVariable.DataInterfaceUrl + entity.Path;
            }

            
            switch (entity.RequestMethod)
            {
                case "6":
                    result = await entity.Path.SetHeaders(dicHerader).SetQueries(dic).GetAsStringAsync();
                    break;
                case "7":
                    //result = await entity.Path.SetHeaders(dicHerader).SetBody(dic).PostAsStringAsync();//SetBody 改为 SetQueries
                    result = await entity.Path.SetHeaders(dicHerader).SetQueries(dic).PostAsStringAsync();
                    break;
            }
            return result;
        }


        /// <summary>
        /// 根据不同规则请求接口(预览)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private async Task<object> GetApiDataByTypePreview(DataInterfaceEntity entity)
        {
            var result = new object();
            var parameters = JSON.Deserialize<List<DataInterfaceReqParameter>>(entity.RequestParameters);
            var parametersHerader = JSON.Deserialize<List<DataInterfaceReqParameter>>(entity.RequestHeaders);
            var dic = new Dictionary<string, object>();
            var dicHerader = new Dictionary<string, object>();
            dicHerader.Add("HSZ_API", true);
            if (_userManager.ToKen != null)
            {
                dicHerader.Add("Authorization", _userManager.ToKen);
            }
            foreach (var key in parameters)
            {
                dic.Add(key.field, key.defaultValue);
            }
            foreach (var key in parametersHerader)
            {
                dicHerader[key.field] = key.defaultValue;
            }
            switch (entity.RequestMethod)
            {
                case "6":
                    {
                        result = await entity.Path.SetHeaders(dicHerader).SetQueries(dic).GetAsStringAsync();
                    }
                    break;
                case "7":
                    {
                        result = await entity.Path.SetHeaders(dicHerader).SetBody(dic).PostAsStringAsync();
                    }
                    break;
            }
            return result;
        }

        /// <summary>
        /// 根据不同规则请求接口(分页)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private async Task<object> GetApiDataPagination(DataInterfaceEntity entity)
        {
            var parameters = JSON.Deserialize<List<DataInterfaceReqParameter>>(entity.RequestParameters);
            var parametersHerader = JSON.Deserialize<List<DataInterfaceReqParameter>>(entity.RequestHeaders);
            var dic = new Dictionary<string, object>();
            var dicHerader = new Dictionary<string, object>();
            dicHerader.Add("HSZ_API", true);
            if (_userManager.ToKen != null)
            {
                dicHerader.Add("Authorization", _userManager.ToKen);
            }
            foreach (var key in parameters)
            {
                dic.Add(key.field, key.defaultValue);
            }
            foreach (var key in parametersHerader)
            {
                dicHerader[key.field] = key.defaultValue;
            }

            switch (entity.RequestMethod)
            {
                case "6":
                    {
                        var result = await entity.Path.SetHeaders(dicHerader).SetQueries(dic).GetAsStringAsync();
                        var jobj = JSON.Deserialize<object>(result).ToObject<JObject>();
                        var value = jobj.ContainsKey("list") ? jobj["list"] : jobj;
                        return new { list = value, dataProcessing = entity.DataProcessing };
                    }
                case "7":
                    {
                        var result = await entity.Path.SetHeaders(dicHerader).SetBody(dic).PostAsStringAsync();
                        var jobj = JSON.Deserialize<object>(result).ToObject<JObject>();
                        var value = jobj.ContainsKey("list") ? jobj["list"] : jobj;
                        return new { list = value, dataProcessing = entity.DataProcessing };
                    }
            }
            return null;
        }


        /// <summary>
        /// DataTable 数据分页
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="PageIndex">第几页</param>
        /// <param name="PageSize">每页多少条</param>
        /// <returns></returns>
        public static DataTable GetPageToDataTable(DataTable dt, int PageIndex, int PageSize)
        {
            if (PageIndex == 0)
                return dt;//0页代表每页数据，直接返回

            if (dt == null)
            {
                DataTable table = new DataTable();
                return table;
            }

            DataTable newdt = dt.Copy();
            newdt.Clear();//copy dt的框架

            int rowbegin = (PageIndex - 1) * PageSize;
            int rowend = PageIndex * PageSize;//要展示的数据条数

            if (rowbegin >= dt.Rows.Count)
                return dt;//源数据记录数小于等于要显示的记录，直接返回dt

            if (rowend > dt.Rows.Count)
                rowend = dt.Rows.Count;
            for (int i = rowbegin; i <= rowend - 1; i++)
            {
                DataRow newdr = newdt.NewRow();
                DataRow dr = dt.Rows[i];
                foreach (DataColumn column in dt.Columns)
                {
                    newdr[column.ColumnName] = dr[column.ColumnName];
                }
                newdt.Rows.Add(newdr);
            }
            return newdt;
        }

        /// <summary>
        /// 获取多租户Link
        /// </summary>
        /// <returns></returns>
        public async Task<DbLinkEntity> GetTenantDbLink()
        {
            return new DbLinkEntity
            {
                Id = configId,
                ServiceName = dbName,
                DbType = App.Configuration["ConnectionStrings:DBType"],
                Host = App.Configuration["ConnectionStrings:Host"],
                Port = App.Configuration["ConnectionStrings:Port"].ToInt(),
                UserName = App.Configuration["ConnectionStrings:UserName"],
                Password = App.Configuration["ConnectionStrings:Password"]
            };
        }

        /// <summary>
        /// 替换参数默认值
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        public void ReplaceParameterValue(DataInterfaceEntity entity, Dictionary<string, string> dic)
        {
            if (dic.IsNotEmptyOrNull() && entity.IsNotEmptyOrNull() && entity.RequestParameters.IsNotEmptyOrNull())
            {
                var parameterList = entity.RequestParameters.ToList<DataInterfaceReqParameter>();
                foreach (var item in parameterList)
                {
                    if (dic.Keys.Contains(item.field))
                    {
                        item.defaultValue = dic[item.field].ToString();
                    }
                    if (entity.DataType == 1)
                    {
                        entity.Query = entity.Query.Replace("{" + item.field + "}", "'" + item.defaultValue + "'");
                    }
                    else
                    {
                        entity.Query = entity.Query.Replace("{" + item.field + "}", item.defaultValue);
                    }

                }
                entity.RequestParameters = parameterList.Serialize();
            }
        }
        #endregion
    }
}
