using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.System.Entitys.Permission;
using HSZ.VisualDev.Entitys;
using HSZ.VisualDev.Entitys.Dto.VisualDev;
using HSZ.VisualDev.Interfaces;
using HSZ.VisualDev.Run.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Threading.Tasks;
using Mapster;
using System.Collections.Generic;
using System.Linq;
using HSZ.Common.Util;
using HSZ.System.Entitys.System;
using HSZ.VisualDev.Entitys.Model.VisualDevModelData;
using HSZ.WorkFlow.Entitys;
using System;
using HSZ.System.Interfaces.System;
using HSZ.VisualDev.Entitys.Dto.VisualDevModelData;
using HSZ.Common.Extension;
using HSZ.Common.Helper;
using HSZ.JsonSerialization;
using HSZ.WorkFlow.Interfaces.FlowTask;
using HSZ.FriendlyException;
using HSZ.Common.Enum;
using HSZ.Common.Filter;
using SqlSugar.IOC;
using System.Diagnostics;
using HSZ.System.Entitys.Model.System.DataBase;
using HSZ.ChangeDataBase;

namespace HSZ.VisualDev
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：可视化开发基础
    /// </summary>
    [ApiDescriptionSettings(Tag = "VisualDev", Name = "Base", Order = 171)]
    [Route("api/visualdev/[controller]")]
    public class VisualDevService : IVisualDevService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<VisualDevEntity> _visualDevRepository;
        private readonly IDictionaryDataService _dictionaryDataService;
        private readonly IChangeDataBase _changeDataBase;
        private readonly IRunService _runService;
        private readonly SqlSugarScope Db;

        /// <summary>
        /// 初始化一个<see cref="VisualDevService"/>类型的新实例
        /// </summary>
        public VisualDevService(ISqlSugarRepository<VisualDevEntity> visualDevRepository,
            IChangeDataBase changeDataBase,
            IRunService runService,
            IDictionaryDataService dictionaryDataService)
        {
            _visualDevRepository = visualDevRepository;
            _dictionaryDataService = dictionaryDataService;
            _runService = runService;
            _changeDataBase = changeDataBase;
            Db = DbScoped.SugarScope;
        }

        #region Get

        /// <summary>
        /// 获取功能列表
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] VisualDevListQueryInput input)
        {
            var data = await _visualDevRepository.AsSugarClient().Queryable<VisualDevEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.keyword), a => a.FullName.Contains(input.keyword) || a.EnCode.Contains(input.keyword))
                .WhereIF(!string.IsNullOrEmpty(input.category), a => a.Category == input.category)
                .Where(a => a.DeleteMark == null && a.Type == input.type)
                .OrderBy(a => a.SortCode, OrderByType.Asc)
                .OrderBy(a => a.CreatorTime, OrderByType.Desc)
                .OrderBy(a => a.LastModifyTime, OrderByType.Desc)
                .Select(a => new VisualDevListOutput
                {
                    id = a.Id,
                    fullName = a.FullName,
                    enCode = a.EnCode,
                    state = a.State,
                    type = a.Type,
                    webType = a.WebType,
                    tables = a.Tables,
                    description = a.Description,
                    creatorTime = a.CreatorTime,
                    lastModifyTime = a.LastModifyTime,
                    deleteMark = a.DeleteMark,
                    sortCode = a.SortCode,
                    parentId = a.Category,
                    category = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(d => d.Id == a.Category).Select(b => b.FullName),
                    creatorUser = SqlFunc.Subqueryable<UserEntity>().Where(u => u.Id == a.CreatorUserId).Select(u => SqlFunc.MergeString(u.RealName, "/", u.Account)),
                    lastModifyUser = SqlFunc.Subqueryable<UserEntity>().Where(u => u.Id == a.LastModifyUserId).Select(u => SqlFunc.MergeString(u.RealName, "/", u.Account))
                })
                .ToPagedListAsync(input.currentPage, input.pageSize);

            return PageResult<VisualDevListOutput>.SqlSugarPageResult(data);

            //var parentIds = data.Select(x => x.parentId).ToList().Distinct();
            //var parentData = await _visualDevRepository.Context.Queryable<DictionaryDataEntity>()
            //    .Where(d => parentIds.Contains(d.Id) && d.DeleteMark == null && d.EnabledMark==1).OrderBy(o => o.SortCode, OrderByType.Asc)
            //    .Select(d => new VisualDevListOutput { parentId = "-1", fullName = d.FullName, id = d.Id, deleteMark = d.DeleteMark, state = SqlFunc.ToInt32(d.EnabledMark) })
            //    .ToListAsync();
            //var treeList = data.Union(parentData).ToList().ToTree("-1");
            //return new { list = treeList };
        }

        /// <summary>
        /// 获取功能列表下拉框
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("Selector")]
        public async Task<dynamic> GetSelector([FromQuery] VisualDevSelectorInput input)
        {
            var data = await _visualDevRepository.AsQueryable().Where(v => v.Type == input.type && v.State == 1 && v.DeleteMark == null).
                OrderBy(a => a.Category).OrderBy(a => a.SortCode).ToListAsync();
            var output = data.Adapt<List<VisualDevSelectorOutput>>();
            var parentIds = output.Select(x => x.parentId).ToList().Distinct();
            var pList = new List<VisualDevSelectorOutput>();
            var parentData = await _visualDevRepository.AsSugarClient().Queryable<DictionaryDataEntity>().Where(d => parentIds.Contains(d.Id) && d.DeleteMark == null).OrderBy(x=>x.SortCode).ToListAsync();
            foreach (var item in parentData)
            {
                var pData = item.Adapt<VisualDevSelectorOutput>();
                pData.parentId = "-1";
                pList.Add(pData);
            }
            var treeList = output.Union(pList).ToList().ToTree("-1");
            return new { list = treeList };
        }

        /// <summary>
        /// 获取功能信息
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var data = await _visualDevRepository.GetFirstAsync(v => v.Id == id && v.DeleteMark == null);
            var output = data.Adapt<VisualDevInfoOutput>();
            return output;
        }

        /// <summary>
        /// 获取表单主表属性下拉框
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/FormDataFields")]
        public async Task<dynamic> GetFormDataFields(string id)
        {
            var templateEntity = await _visualDevRepository.GetFirstAsync(v => v.Id == id && v.DeleteMark == null);
            var formData = TemplateKeywordsHelper.ReplaceKeywords(templateEntity.FormData).Deserialize<FormDataModel>();
            //剔除多虑多余控件
            var newFields = _runService.TemplateDataConversion(formData.fields);
            var fieldsModels = newFields.FindAll(x => !"".Equals(x.__vModel__) && !"table".Equals(x.__config__.hszKey) && !"relationForm".Equals(x.__config__.hszKey));
            var output = fieldsModels.Select(x => new VisualDevFormDataFieldsOutput()
            {
                label = x.__config__.label,
                vmodel = x.__vModel__
            }).ToList();
            return new { list = output };
        }

        /// <summary>
        /// 获取表单主表属性列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/FieldDataSelect")]
        public async Task<dynamic> GetFieldDataSelect(string id, [FromQuery] VisualDevDataFieldDataListInput input)
        {
            Dictionary<string, object> queryDic = new Dictionary<string, object>();

            if (!string.IsNullOrWhiteSpace(input.relationField) && !string.IsNullOrWhiteSpace(input.keyword))
                queryDic.Add(input.relationField, input.keyword);
              
            var templateEntity = await _visualDevRepository.GetFirstAsync(v => v.Id == id);
            var formData = TemplateKeywordsHelper.ReplaceKeywords(templateEntity.FormData).Deserialize<FormDataModel>();
            var columnData = TemplateKeywordsHelper.ReplaceKeywords(templateEntity.ColumnData).Deserialize<ColumnDesignModel>();
            formData.fields = _runService.TemplateDataConversion(formData.fields);//解开子表无限children，模板控件 可数据缓存

            if (input.IsNotEmptyOrNull() && input.columnOptions.IsNotEmptyOrNull())//指定查询字段
            {
                //显示的所有 字段
                var showFieldList = input.columnOptions.Split(',').ToList();

                var flist = new List<FieldsModel>();
                var clist = new List<IndexGridFieldModel>();

                //获取 调用 该功能表单 的功能模板
                var smodel = formData.fields.Where(x => x.__vModel__ == input.relationField).First();
                smodel.searchType = 2;
                flist.Add(smodel);//添加 关联查询字段
                if(columnData==null)
                {
                    columnData = new ColumnDesignModel()
                    {
                        columnList = new List<IndexGridFieldModel>() { new IndexGridFieldModel() { prop = input.relationField, label = input.relationField } },
                        searchList = new List<FieldsModel>() { smodel }
                    };
                }
                if(!columnData.columnList.Where(x => x.prop == input.relationField).Any())
                {
                    columnData.columnList.Add(new IndexGridFieldModel() { prop = input.relationField, label = input.relationField });
                }
                if (columnData.defaultSidx.IsNotEmptyOrNull() && formData.fields.Any(x => x.__vModel__ == columnData?.defaultSidx))
                    flist.Add(formData.fields.Where(x => x.__vModel__ == columnData?.defaultSidx).FirstOrDefault());//添加 关联排序字段
                formData.fields.ForEach(item =>
                {
                    if (showFieldList.Find(x => x == item.__vModel__) != null) flist.Add(item);
                });
                clist.Add(columnData.columnList.Where(x => x.prop == input.relationField).FirstOrDefault());//添加 关联查询字段
                if (columnData.defaultSidx.IsNotEmptyOrNull() && formData.fields.Any(x => x.__vModel__ == columnData?.defaultSidx))
                    clist.Add(columnData.columnList.Where(x => x.prop == columnData?.defaultSidx).FirstOrDefault());//添加 关联排序字段
                showFieldList.ForEach(item =>
                {
                    if (!columnData.columnList.Where(x => x.prop == item).Any())
                        clist.Add(new IndexGridFieldModel() { prop = item, label = item });
                    else
                        clist.Add(columnData.columnList.Find(x => x.prop == item));
                });

                if (flist.Count > 0)
                {
                    formData.fields = flist.Distinct().ToList();
                    templateEntity.FormData = formData.Serialize();
                }
                if (clist.Count > 0)
                {
                    columnData.columnList = clist.Distinct().ToList();
                    templateEntity.ColumnData = columnData.Serialize();
                }
            }

            //获取值 无分页
            VisualDevModelListQueryInput listQueryInput = new VisualDevModelListQueryInput
            {
                json = queryDic.Serialize(),
                currentPage = input.currentPage > 0 ? input.currentPage : 1,
                pageSize = input.pageSize > 0 ? input.pageSize : 20,
                dataType = "1",
                sidx = columnData?.defaultSidx,
                sort = columnData?.sort
            };
            var realList = (await _runService.GetRelationFormList(templateEntity, listQueryInput, "List"));
            return realList;
        }

        #endregion

        #region Post

        /// <summary>
        /// 新建功能信息
        /// </summary>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] VisualDevCrInput input)
        {
            var entity = input.Adapt<VisualDevEntity>();
            try
            {
                #region 验证模板
                var fieldsModelList = TemplateKeywordsHelper.ReplaceKeywords(entity.FormData).Deserialize<FormDataModel>().fields;
                //剔除布局控件
                fieldsModelList = _runService.TemplateDataConversion(fieldsModelList);
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

                //开启事务
                Db.BeginTran();

                if (input.webType == "3")
                {
                    var categoryData = await _dictionaryDataService.GetInfo(entity.Category);
                    var flowEngine = new FlowEngineEntity();
                    flowEngine.FlowTemplateJson = entity.FlowTemplateJson;
                    flowEngine.EnCode = "#visualDev" + entity.EnCode;
                    flowEngine.Type = 1;
                    flowEngine.FormType = 2;
                    flowEngine.FullName = entity.FullName;
                    flowEngine.Category = categoryData.EnCode;
                    flowEngine.VisibleType = 0;
                    flowEngine.Icon = "icon-sz icon-sz-node";
                    flowEngine.IconBackground = "#008cff";
                    flowEngine.Tables = entity.Tables;
                    flowEngine.DbLinkId = entity.DbLinkId;
                    flowEngine.FormTemplateJson = entity.FormData;
                    //添加流程引擎
                    var engineEntity = await _visualDevRepository.AsSugarClient().Insertable<FlowEngineEntity>(flowEngine).IgnoreColumns(ignoreNullColumn: true).CallEntityMethod(m => m.Creator()).ExecuteReturnEntityAsync();
                    entity.FlowId = engineEntity.Id;
                    entity.Id = engineEntity.Id;
                }

                var visualDev = await _visualDevRepository.AsSugarClient().Insertable(entity).IgnoreColumns(ignoreNullColumn: true).CallEntityMethod(m => m.Create()).ExecuteReturnEntityAsync();

                //关闭事务
                Db.CommitTran();
            }
            catch (Exception)
            {
                Db.RollbackTran();
                throw;
            }
        }

        /// <summary>
        /// 修改接口
        /// </summary>
        /// <param name="id">主键id</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] VisualDevUpInput input)
        {
            var entity = input.Adapt<VisualDevEntity>();
            entity.FlowId = entity.Id;
            try
            {
                #region 验证模板
                var fieldsModelList = TemplateKeywordsHelper.ReplaceKeywords(entity.FormData).Deserialize<FormDataModel>().fields;
                //剔除布局控件
                fieldsModelList = _runService.TemplateDataConversion(fieldsModelList);
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

                //开启事务
                Db.BeginTran();

                if (input.webType == "3")
                {
                    #region 表单或列表 转 流程
                    var oldEntity = await _visualDevRepository.GetByIdAsync(id);

                    if (oldEntity.WebType < 3)
                    {
                        var link = await _visualDevRepository.AsSugarClient().Queryable<DbLinkEntity>().FirstAsync(m => m.Id == oldEntity.DbLinkId && m.DeleteMark == null);
                        if (link == null) link = _runService.GetTenantDbLink();//当前数据库连接
                        var DbType = link?.DbType != null ? link.DbType : Db.CurrentConnectionConfig.DbType.ToString();//当前数据库类型
                        var MainTable = entity.Tables.ToString().Deserialize<List<TableModel>>().Find(m => m.relationTable == "")?.table;//主表名称
                        var fieldList = _changeDataBase.GetFieldList(link, MainTable);//获取主表所有列

                        if (!fieldList.Any(x => SqlFunc.ToLower(x.field) == "f_flowid"))
                        {
                            var pFieldList = new List<DbTableFieldModel>() { new DbTableFieldModel() { field = "F_FlowId" } };
                            _changeDataBase.AddTableColumn(MainTable, pFieldList);
                        }
                    }
                    #endregion

                    var categoryData = await _dictionaryDataService.GetInfo(entity.Category);
                    var engineEntity = await _visualDevRepository.AsSugarClient().Queryable<FlowEngineEntity>().FirstAsync(f => f.Id == entity.FlowId);
                    if (await _visualDevRepository.AsSugarClient().Queryable<FlowTaskEntity>().AnyAsync(x => x.DeleteMark == null && x.FlowId == id))
                        throw HSZException.Oh(ErrorCode.WF0024);
                    engineEntity.FlowTemplateJson = input.flowTemplateJson;
                    engineEntity.EnCode = "#visualDev" + entity.EnCode;
                    engineEntity.Type = 1;
                    engineEntity.FormType = 2;
                    engineEntity.FullName = entity.FullName;
                    engineEntity.Category = categoryData.EnCode;
                    engineEntity.VisibleType = 0;
                    engineEntity.Icon = "icon-sz icon-sz-node";
                    engineEntity.IconBackground = "#008cff";
                    engineEntity.Tables = entity.Tables;
                    engineEntity.DbLinkId = entity.DbLinkId;
                    engineEntity.FormTemplateJson = entity.FormData;
                    await _visualDevRepository.AsSugarClient().Updateable<FlowEngineEntity>(engineEntity).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
                }

                await _visualDevRepository.AsSugarClient().Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();

                //关闭事务
                Db.CommitTran();
            }
            catch (Exception)
            {
                Db.RollbackTran();
                throw;
            }
        }

        /// <summary>
        /// 删除接口
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _visualDevRepository.GetFirstAsync(v => v.Id == id && v.DeleteMark == null);
            await _visualDevRepository.AsSugarClient().Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpPost("{id}/Actions/Copy")]
        public async Task ActionsCopy(string id)
        {
            var random = new Random().NextLetterAndNumberString(5);
            var entity = await _visualDevRepository.GetFirstAsync(v => v.Id == id && v.DeleteMark == null);
            entity.FullName = entity.FullName + "副本" + random;
            entity.EnCode = entity.EnCode + random;
            entity.State = 0;
            entity.Id = null;//复制的数据需要把Id清空，否则会主键冲突错误
            if (entity.WebType == 3)
            {
                var categoryData = await _dictionaryDataService.GetInfo(entity.Category);
                var flowEngine = new FlowEngineEntity();
                flowEngine.FlowTemplateJson = entity.FlowTemplateJson;
                flowEngine.EnCode = "#visualDev" + entity.EnCode;
                flowEngine.Type = 1;
                flowEngine.FormType = 2;
                flowEngine.FullName = entity.FullName;
                flowEngine.Category = categoryData.EnCode;
                flowEngine.VisibleType = 0;
                flowEngine.Icon = "icon-sz icon-sz-node";
                flowEngine.IconBackground = "#008cff";
                flowEngine.Tables = entity.Tables;
                flowEngine.DbLinkId = entity.DbLinkId;
                flowEngine.FormTemplateJson = entity.FormData;

                try
                {
                    //添加流程引擎
                    var engineEntity = await _visualDevRepository.AsSugarClient().Insertable<FlowEngineEntity>(flowEngine).IgnoreColumns(ignoreNullColumn: true).CallEntityMethod(m => m.Creator()).ExecuteReturnEntityAsync();
                    entity.FlowId = engineEntity.Id;
                    entity.Id = engineEntity.Id;
                    await _visualDevRepository.AsSugarClient().Insertable(entity).IgnoreColumns(ignoreNullColumn: true).CallEntityMethod(m => m.Create()).ExecuteCommandAsync();
                }
                catch 
                {
                    if (entity.FullName.Length >= 100 || entity.EnCode.Length >= 50)//数据长度超过 字段设定长度
                        throw HSZException.Oh(ErrorCode.D1403);
                    else
                        throw;
                }
            }
            else
            {
                try
                {
                    await _visualDevRepository.AsSugarClient().Insertable(entity).IgnoreColumns(ignoreNullColumn: true).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
                }
                catch
                {
                    if (entity.FullName.Length >= 100 || entity.EnCode.Length >= 50)//数据长度超过 字段设定长度
                        throw HSZException.Oh(ErrorCode.D1403);
                    else
                        throw;
                }
            }
        }

        #endregion

        #region PublicMethod

        /// <summary>
        /// 获取功能信息
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        [NonAction]
        public async Task<VisualDevEntity> GetInfoById(string id)
        {
            return await _visualDevRepository.GetFirstAsync(v => v.Id == id && v.DeleteMark == null);
        }

        /// <summary>
        /// 判断功能ID是否存在
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<bool> GetDataExists(string id)
        {
            return await _visualDevRepository.IsAnyAsync(it => it.Id == id && it.DeleteMark == null);
        }

        /// <summary>
        /// 判断是否存在编码、名称相同的数据
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<bool> GetDataExists(string enCode, string fullName)
        {
            return await _visualDevRepository.IsAnyAsync(it => it.EnCode == enCode && it.FullName == fullName && it.DeleteMark == null);
        }

        /// <summary>
        /// 新增导入数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [NonAction]
        public async Task CreateImportData(VisualDevEntity input)
        {
            try
            {
                //开启事务
                Db.BeginTran();

                if (input.WebType == 3)
                {
                    var categoryData = await _dictionaryDataService.GetInfo(input.Category);
                    var flowEngine = new FlowEngineEntity();
                    flowEngine.FlowTemplateJson = input.FlowTemplateJson;
                    flowEngine.EnCode = "#visualDev" + input.EnCode;
                    flowEngine.Type = 1;
                    flowEngine.FormType = 2;
                    flowEngine.FullName = input.FullName;
                    flowEngine.Category = categoryData.EnCode;
                    flowEngine.VisibleType = 0;
                    flowEngine.Icon = "icon-sz icon-sz-node";
                    flowEngine.IconBackground = "#008cff";
                    flowEngine.Tables = input.Tables;
                    flowEngine.DbLinkId = input.DbLinkId;
                    flowEngine.FormTemplateJson = input.FormData;
                    //添加流程引擎
                    var engineEntity = await _visualDevRepository.AsSugarClient().Insertable<FlowEngineEntity>(flowEngine).IgnoreColumns(ignoreNullColumn: true).CallEntityMethod(m => m.Creator()).ExecuteReturnEntityAsync();
                    input.FlowId = engineEntity.Id;
                    input.Id = engineEntity.Id;
                }

                //var visualDev = await _visualDevRepository.AsSugarClient().Insertable(input).IgnoreColumns(ignoreNullColumn: true).CallEntityMethod(m => m.Create()).ExecuteReturnEntityAsync();

                var stor = _visualDevRepository.AsSugarClient().Storageable(input).Saveable().ToStorage(); //存在更新不存在插入 根据主键

                await stor.AsInsertable.ExecuteCommandAsync(); //执行插入
                                                               //await stor.AsUpdateable.ExecuteCommandAsync(); //执行更新　
                await _visualDevRepository.AsSugarClient().Updateable(input).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();


                //关闭事务
                Db.CommitTran();
            }
            catch (Exception)
            {
                Db.RollbackTran();
                throw;
            }
        }

        #endregion
    }
}