using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Common.Helper;
using HSZ.Common.Util;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.JsonSerialization;
using HSZ.LinqBuilder;
using HSZ.System.Entitys.Permission;
using HSZ.System.Entitys.System;
using HSZ.System.Interfaces.Common;
using HSZ.System.Interfaces.System;
using HSZ.VisualDev.Entitys.Model.VisualDevModelData;
using HSZ.VisualDev.Run.Interfaces;
using HSZ.WorkFlow.Entitys;
using HSZ.WorkFlow.Entitys.Dto.FlowEngine;
using HSZ.WorkFlow.Entitys.Model;
using HSZ.WorkFlow.Interfaces.FlowEngine;
using HSZ.WorkFlow.Interfaces.FlowTask.Repository;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HSZ.WorkFlow.Core.Service.FlowEngine
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：流程引擎
    /// </summary>
    [ApiDescriptionSettings(Tag = "WorkflowEngine", Name = "FlowEngine", Order = 301)]
    [Route("api/workflow/Engine/[controller]")]
    public class FlowEngineService : IFlowEngineService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<FlowEngineEntity> _flowEngineRepository;
        private readonly ISqlSugarRepository<FlowEngineVisibleEntity> _flowEngineVisibleRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope Db;// 核心对象：拥有完整的SqlSugar全部功能
        private readonly IFlowTaskRepository _flowTaskRepository;
        private readonly IDictionaryDataService _dictionaryDataService;
        private readonly IFileService _fileService;
        private readonly IRunService _runService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="flowEngineRepository"></param>
        /// <param name="flowEngineVisibleRepository"></param>
        /// <param name="userManager"></param>
        /// <param name="flowTaskRepository"></param>
        /// <param name="dictionaryDataService"></param>
        /// <param name="fileService"></param>
        public FlowEngineService(ISqlSugarRepository<FlowEngineEntity> flowEngineRepository,
            ISqlSugarRepository<FlowEngineVisibleEntity> flowEngineVisibleRepository,
            IUserManager userManager, IFlowTaskRepository flowTaskRepository,
            IDictionaryDataService dictionaryDataService, IFileService fileService, IRunService runService)
        {
            _flowEngineRepository = flowEngineRepository;
            _flowEngineVisibleRepository = flowEngineVisibleRepository;
            _userManager = userManager;
            Db = DbScoped.SugarScope;
            _flowTaskRepository = flowTaskRepository;
            _dictionaryDataService = dictionaryDataService;
            _fileService = fileService;
            _runService = runService;
        }

        #region GET
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] FlowEngineListInput input)
        {
            return await GetOutPageList(input);
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("ListAll")]
        public async Task<dynamic> GetListAll()
        {
            var list1 = await GetFlowFormList();
            var dicDataInfo = await _dictionaryDataService.GetInfo(list1.First().parentId);
            var dicDataList = (await _dictionaryDataService.GetList(dicDataInfo.DictionaryTypeId)).FindAll(x => x.EnabledMark == 1);
            var list2 = new List<FlowEngineListOutput>();
            foreach (var item in dicDataList)
            {
                list2.Add(new FlowEngineListOutput()
                {
                    fullName = item.FullName,
                    parentId = "0",
                    id = item.Id,
                    num = list1.FindAll(x => x.category == item.EnCode).Count
                });
            }
            var output = list1.Union(list2).ToList().ToTree();
            return new { list = output };
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("PageListAll")]
        public async Task<dynamic> GetListPageAll([FromQuery] FlowEngineListInput input)
        {
            var data = await GetFlowFormList();
            if (input.category.IsNotEmptyOrNull())
            {
                data = data.FindAll(x => x.category == input.category);
            }
            if (input.keyword.IsNotEmptyOrNull())
            {
                data = data.FindAll(o => o.fullName.Contains(input.keyword) || o.enCode.Contains(input.keyword));
            }
            var pageList = new SqlSugarPagedList<FlowEngineListOutput>()
            {
                list = data,
                pagination = new PagedModel()
                {
                    PageIndex = input.currentPage,
                    PageSize = input.pageSize,
                    Total = data.Count
                }
            };
            return PageResult<FlowEngineListOutput>.SqlSugarPageResult(pageList);
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo_Api(string id)
        {
            var flowEntity = await GetInfo(id);
            var output = flowEntity.Adapt<FlowEngineInfoOutput>();
            return output;
        }

        /// <summary>
        /// 获取流程设计列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet("Selector")]
        public async Task<dynamic> ListSelect(int type)
        {
            var list1 = (await GetOutList(1)).FindAll(x => x.enabledMark == 1);
            if (type.IsEmpty())
            {
                list1 = list1.FindAll(x => x.formType == type);
            }
            var dicDataInfo = await _dictionaryDataService.GetInfo(list1.First().parentId);
            var dicDataList = (await _dictionaryDataService.GetList(dicDataInfo.DictionaryTypeId)).FindAll(x => x.EnabledMark == 1);
            var list2 = new List<FlowEngineListOutput>();
            foreach (var item in dicDataList)
            {
                var index = list1.FindAll(x => x.category == item.EnCode).Count;
                if (index > 0)
                {
                    list2.Add(new FlowEngineListOutput()
                    {
                        fullName = item.FullName,
                        parentId = "0",
                        id = item.Id,
                        num = index
                    });
                }
            }
            var output = list1.Union(list2).ToList().ToTree();
            return new { list = output };
        }

        /// <summary>
        /// 表单主表属性
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/FormDataFields")]
        public async Task<dynamic> getFormDataField(string id)
        {
            var entity = await GetInfo(id);
            List<FormDataField> formDataFieldList = new List<FormDataField>();
            if (entity.FormType == 1)
            {
                var filedList = entity.FormTemplateJson.ToList<Field>();
                foreach (var item in filedList)
                {
                    formDataFieldList.Add(new FormDataField()
                    {
                        vmodel = item.filedId,
                        label = item.filedName
                    });
                }
            }
            else
            {
                FormDataModel formData = entity.FormTemplateJson.ToObject<FormDataModel>();
                List<FieldsModel> list = formData.fields;
                foreach (var item in list)
                {
                    if (item.__vModel__.IsNotEmptyOrNull() && !"table".Equals(item.__config__.hszKey) && !"relationForm".Equals(item.__config__.hszKey) && !"relationFlow".Equals(item.__config__.hszKey))
                    {
                        formDataFieldList.Add(new FormDataField()
                        {
                            vmodel = item.__vModel__,
                            label = item.__config__.label
                        });
                    }
                }
            }
            return new { list = formDataFieldList };
        }

        /// <summary>
        /// 表单列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/FieldDataSelect")]
        public async Task<dynamic> getFormData(string id)
        {
            var flowTaskList = await _flowTaskRepository.GetTaskList(id);
            var output = flowTaskList.Select(x => new FlowEngineListSelectOutput()
            {
                id = x.Id,
                fullName = SqlFunc.MergeString(x.FullName , "/" , x.EnCode)
            }).ToList();
            return flowTaskList;
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/Actions/ExportData")]
        public async Task<dynamic> ActionsExport(string id)
        {
            var importModel = new FlowEngineImportModel();
            importModel.flowEngine = await _flowEngineRepository.GetFirstAsync(x => x.Id == id && x.DeleteMark == null);
            importModel.visibleList = await _flowEngineVisibleRepository.AsQueryable().Where(x => x.FlowId == id).ToListAsync();
            var jsonStr = importModel.Serialize();
            return _fileService.Export(jsonStr, importModel.flowEngine.FullName);
        }
        #endregion

        #region POST
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var flowEngineEntity = await GetInfo(id);
            if (flowEngineEntity == null)
                throw HSZException.Oh(ErrorCode.COM1005); ;
            if (await _flowTaskRepository.AnyFlowTask(x => x.DeleteMark == null && x.FlowId == id))
                throw HSZException.Oh(ErrorCode.WF0024);
            var isOk = await Delete(flowEngineEntity);
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1002);
        }

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] FlowEngineCrInput input)
        {
            if (await _flowEngineRepository.IsAnyAsync(x => x.EnCode == input.enCode && x.DeleteMark == null) || await _flowEngineRepository.IsAnyAsync(x => x.FullName == input.fullName && x.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.COM1004);
            if (input.formType == 2)
            {
                #region 验证模板
                var fieldsModelList = TemplateKeywordsHelper.ReplaceKeywords(input.formData).Deserialize<FormDataModel>().fields;
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
            }
            var flowEngineEntity = input.Adapt<FlowEngineEntity>();
            var flowVisibleList = GetFlowEngineVisibleList(input.flowTemplateJson);
            var result = await Create(flowEngineEntity, flowVisibleList);
            _ = result ?? throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] FlowEngineUpInput input)
        {
            if (await _flowEngineRepository.IsAnyAsync(x => x.Id != id && x.EnCode == input.enCode && x.DeleteMark == null) || await _flowEngineRepository.IsAnyAsync(x => x.Id != id && x.FullName == input.fullName && x.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.COM1004);
            if (await _flowTaskRepository.AnyFlowTask(x => x.DeleteMark == null && x.FlowId == id && x.Status != 2))
                throw HSZException.Oh(ErrorCode.WF0024);
            if (input.formType == 2)
            {
                #region 验证模板
                var fieldsModelList = TemplateKeywordsHelper.ReplaceKeywords(input.formData).Deserialize<FormDataModel>().fields;
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
            }
            var flowEngineEntity = input.Adapt<FlowEngineEntity>();
            var flowVisibleList = GetFlowEngineVisibleList(input.flowTemplateJson);
            var isOk = await Update(id, flowEngineEntity, flowVisibleList);
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpPost("{id}/Actions/Copy")]
        public async Task ActionsCopy(string id)
        {
            var entity = await GetInfo(id);
            var random = RandomExtensions.NextLetterAndNumberString(new Random(), 5).ToLower();
            entity.FullName = entity.FullName + "副本" + random;
            entity.EnCode = entity.EnCode + random;
            if (entity.FullName.Length >= 50 || entity.EnCode.Length >= 50)
                throw HSZException.Oh(ErrorCode.COM1009);
            var flowVisibleList = GetFlowEngineVisibleList(entity.FlowTemplateJson);
            var result = await Create(entity, flowVisibleList);
            _ = result ?? throw HSZException.Oh(ErrorCode.WF0002);
        }

        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpPost("Release/{id}")]
        public async Task Release(string id)
        {
            var entity = await GetInfo(id);
            if (entity == null)
                throw HSZException.Oh(ErrorCode.COM1005);
            entity.EnabledMark = 1;
            var isOk = await Update(id, entity);
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1003);
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpPost("Stop/{id}")]
        public async Task Stop(string id)
        {
            var entity = await GetInfo(id);
            if (entity == null)
                throw HSZException.Oh(ErrorCode.COM1005);
            entity.EnabledMark = 0;
            var isOk = await Update(id, entity);
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1003);
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("Actions/ImportData")]
        public async Task ActionsImport(IFormFile file)
        {
            var fileType = Path.GetExtension(file.FileName).Replace(".", "");
            if (!fileType.ToLower().Equals("json"))
                throw HSZException.Oh(ErrorCode.D3006);
            var josn = _fileService.Import(file);
            FlowEngineImportModel model = null;
            try
            {
                model = josn.Deserialize<FlowEngineImportModel>();
            }
            catch
            {
                throw HSZException.Oh(ErrorCode.D3006);
            }
            if (model == null)
                throw HSZException.Oh(ErrorCode.D3006);
            await ImportData(model);
        }
        #endregion

        #region PublicMethod
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="visibleList"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<FlowEngineEntity> Create(FlowEngineEntity entity, List<FlowEngineVisibleEntity> visibleList)
        {
            try
            {
                Db.BeginTran();
                entity.VisibleType = visibleList.Count == 0 ? 0 : 1;
                var result = await _flowEngineRepository.AsInsertable(entity).CallEntityMethod(m => m.Creator()).ExecuteReturnEntityAsync();
                if (result == null)
                    throw HSZException.Oh(ErrorCode.COM1005);
                foreach (var item in visibleList)
                {
                    item.FlowId = entity.Id;
                    item.SortCode = visibleList.IndexOf(item);
                }
                if (visibleList.Count > 0)
                    await _flowEngineVisibleRepository.AsInsertable(visibleList).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
                Db.CommitTran();
                return result;
            }
            catch (Exception ex)
            {
                Db.RollbackTran();
                return null;
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Delete(FlowEngineEntity entity)
        {
            try
            {
                Db.BeginTran();
                await _flowEngineVisibleRepository.DeleteAsync(a => a.FlowId == entity.Id);
                var isOk = await _flowEngineRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
                Db.CommitTran();
                return isOk;
            }
            catch (Exception ex)
            {
                Db.RollbackTran();
                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<FlowEngineEntity> GetInfo(string id)
        {
            return await _flowEngineRepository.GetFirstAsync(a => a.Id == id && a.DeleteMark == null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enCode"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<FlowEngineEntity> GetInfoByEnCode(string enCode)
        {
            return await _flowEngineRepository.GetFirstAsync(a => a.EnCode == enCode && a.EnabledMark == 1 && a.DeleteMark == null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<FlowEngineEntity>> GetList()
        {
            return await _flowEngineRepository.AsQueryable().Where(a => a.DeleteMark == null).OrderBy(a => a.SortCode).OrderBy(o => o.CreatorTime).ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<FlowEngineVisibleEntity>> GetVisibleFlowList(string userId)
        {
            return await _flowEngineRepository.AsSugarClient().Queryable<FlowEngineVisibleEntity, UserRelationEntity>((a, b) => new JoinQueryInfos(JoinType.Left, a.OperatorId == b.ObjectId)).Select((a, b) => new { Id = a.Id, FlowId = a.FlowId, OperatorType = a.OperatorType, OperatorId = a.OperatorId, SortCode = a.SortCode, CreatorTime = a.CreatorTime, CreatorUserId = a.CreatorUserId, UserId = b.UserId }).MergeTable().Where(a => a.OperatorId == _userManager.UserId || a.UserId == _userManager.UserId).Select<FlowEngineVisibleEntity>().ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <param name="visibleList"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Update(string id, FlowEngineEntity entity, List<FlowEngineVisibleEntity> visibleList)
        {
            try
            {
                Db.BeginTran();
                entity.VisibleType = visibleList.Count == 0 ? 0 : 1;
                await _flowEngineVisibleRepository.DeleteAsync(a => a.FlowId == entity.Id);
                foreach (var item in visibleList)
                {
                    item.FlowId = entity.Id;
                    item.SortCode = visibleList.IndexOf(item);
                }
                if (visibleList.Count > 0)
                    await _flowEngineVisibleRepository.AsInsertable(visibleList).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
                var isOk = await _flowEngineRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
                Db.CommitTran();
                return isOk;
            }
            catch (Exception ex)
            {
                Db.RollbackTran();
                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Update(string id, FlowEngineEntity entity)
        {
            return await _flowEngineRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<FlowEngineListOutput>> GetFlowFormList()
        {
            var list = (await GetOutList()).FindAll(x => x.enabledMark == 1 && x.type == 0);
            if (_userManager.User.IsAdministrator == 0)
            {
                var data = new List<FlowEngineListOutput>();
                //部分看见
                var flowVisibleData = await GetVisibleFlowList(_userManager.UserId);
                //去重
                var ids = new List<string>();
                foreach (var item in flowVisibleData)
                {
                    FlowEngineListOutput flowEngineEntity = list.Find(m => m.id == item.FlowId);
                    if (flowEngineEntity != null && !ids.Contains(flowEngineEntity.id))
                    {
                        data.Add(flowEngineEntity);
                        ids.Add(flowEngineEntity.id);
                    }
                }
                ////全部看见
                foreach (FlowEngineListOutput flowEngineEntity in list.FindAll(m => m.visibleType == 0))
                {
                    data.Add(flowEngineEntity);
                }
                return data;
            }
            else
            {
                return list;
            }
        }
        #endregion

        #region PrivateMethod
        /// <summary>
        /// 解析流程可见参数
        /// </summary>
        /// <param name="josnStr"></param>
        /// <returns></returns>
        private List<FlowEngineVisibleEntity> GetFlowEngineVisibleList(string josnStr)
        {
            var output = new List<FlowEngineVisibleEntity>();
            var jobj = JSON.Deserialize<FlowTemplateJsonModel>(josnStr).properties;
            var initiator = jobj["initiator"] as JArray;
            var initiatePos = jobj["initiatePos"] as JArray;
            var initiateRole = jobj["initiateRole"] as JArray;
            if (initiator != null && initiator.Count != 0)
            {
                foreach (var item in initiator)
                {
                    var entity = new FlowEngineVisibleEntity();
                    entity.OperatorId = item.ToString();
                    entity.OperatorType = "user";
                    output.Add(entity);
                }
            }
            if (initiatePos != null && initiatePos.Count != 0)
            {
                foreach (var item in initiatePos)
                {
                    var entity = new FlowEngineVisibleEntity();
                    entity.OperatorId = item.ToString();
                    entity.OperatorType = "Position";
                    output.Add(entity);
                }
            }
            if (initiateRole != null && initiateRole.Count != 0)
            {
                foreach (var item in initiateRole)
                {
                    var entity = new FlowEngineVisibleEntity();
                    entity.OperatorId = item.ToString();
                    entity.OperatorType = "Role";
                    output.Add(entity);
                }
            }
            return output;
        }

        /// <summary>
        /// 流程列表(功能流程不显示)
        /// </summary>
        /// <param name="type">0:流程设计，1：下拉列表</param>
        /// <returns></returns>
        private async Task<List<FlowEngineListOutput>> GetOutList(int type = 0)
        {
            return await _flowEngineRepository.AsSugarClient().Queryable<FlowEngineEntity, UserEntity, UserEntity, DictionaryDataEntity>((a, b, c, d) => new JoinQueryInfos(JoinType.Left, b.Id == a.CreatorUserId, JoinType.Left, c.Id == a.LastModifyUserId, JoinType.Left, a.Category == d.EnCode))
                 .Where((a, b, c, d) => a.DeleteMark == null && d.DictionaryTypeId == "507f4f5df86b47588138f321e0b0dac7")
                 .Where(a => !(a.FormType == 2 && a.Type == 1)).WhereIF(type != 0, a => a.Type != 1)
                 .Select((a, b, c, d) => new FlowEngineListOutput
                 {
                     category = a.Category,
                     id = a.Id,
                     description = a.Description,
                     creatorTime = a.CreatorTime,
                     creatorUser = SqlFunc.MergeString(b.RealName, "/", b.Account),
                     enCode = a.EnCode,
                     enabledMark = a.EnabledMark,
                     flowTemplateJson = a.FlowTemplateJson,
                     formData = a.FormTemplateJson,
                     fullName = a.FullName,
                     formType = a.FormType,
                     icon = a.Icon,
                     iconBackground = a.IconBackground,
                     lastModifyTime = a.LastModifyTime,
                     lastModifyUser = SqlFunc.MergeString(c.RealName, "/", c.Account),
                     sortCode = a.SortCode,
                     type = a.Type,
                     visibleType = a.VisibleType,
                     parentId = d.Id
                 }).MergeTable().OrderBy(a => a.sortCode).OrderBy(a => a.creatorTime, OrderByType.Desc)
                 .OrderBy(a => a.lastModifyTime, OrderByType.Desc).ToListAsync();
        }

        private async Task<dynamic> GetOutPageList(FlowEngineListInput input, int type = 0)
        {
            var list = await _flowEngineRepository.AsSugarClient().Queryable<FlowEngineEntity, UserEntity, UserEntity, DictionaryDataEntity>((a, b, c, d) => new JoinQueryInfos(JoinType.Left, b.Id == a.CreatorUserId, JoinType.Left, c.Id == a.LastModifyUserId, JoinType.Left, a.Category == d.EnCode))
                .Where((a, b, c, d) => a.DeleteMark == null && d.DictionaryTypeId == "507f4f5df86b47588138f321e0b0dac7")
                .Where(a => !(a.FormType == 2 && a.Type == 1)).WhereIF(type != 0, a => a.Type != 1)
                .WhereIF(input.category.IsNotEmptyOrNull(), a => a.Category == input.category)
                .WhereIF(input.keyword.IsNotEmptyOrNull(), a => a.FullName.Contains(input.keyword) || a.EnCode.Contains(input.keyword))
                .Select((a, b, c, d) => new FlowEngineListOutput
                {
                    category = d.FullName,
                    id = a.Id,
                    description = a.Description,
                    creatorTime = a.CreatorTime,
                    creatorUser = SqlFunc.MergeString(b.RealName, "/", b.Account),
                    enCode = a.EnCode,
                    enabledMark = a.EnabledMark,
                    flowTemplateJson = a.FlowTemplateJson,
                    formData = a.FormTemplateJson,
                    fullName = a.FullName,
                    formType = a.FormType,
                    icon = a.Icon,
                    iconBackground = a.IconBackground,
                    lastModifyTime = a.LastModifyTime,
                    lastModifyUser = SqlFunc.MergeString(c.RealName, "/", c.Account),
                    sortCode = a.SortCode,
                    type = a.Type,
                    visibleType = a.VisibleType,
                    parentId = d.Id
                }).MergeTable().OrderBy(a => a.sortCode).OrderBy(a => a.creatorTime, OrderByType.Desc)
                .OrderByIF(!string.IsNullOrEmpty(input.keyword), t => t.lastModifyTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<FlowEngineListOutput>.SqlSugarPageResult(list);
        }


        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task ImportData(FlowEngineImportModel model)
        {
            try
            {
                Db.BeginTran();
                var stor = _flowEngineRepository.AsSugarClient().Storageable(model.flowEngine).Saveable().ToStorage(); //存在更新不存在插入 根据主键
                await stor.AsInsertable.ExecuteCommandAsync(); //执行插入
                //await stor.AsUpdateable.ExecuteCommandAsync(); //执行更新，停用原因：Oracle 数据库环境会抛异常：ora-01704: 字符串文字太长
                await _flowEngineRepository.AsSugarClient().Updateable(model.flowEngine).ExecuteCommandAsync();//执行更新

                var stor1 = _flowEngineRepository.AsSugarClient().Storageable(model.visibleList).Saveable().ToStorage(); //存在更新不存在插入 根据主键
                await stor1.AsInsertable.ExecuteCommandAsync(); //执行插入
                await stor1.AsUpdateable.ExecuteCommandAsync(); //执行更新
                Db.CommitTran();
            }
            catch (Exception ex)
            {
                Db.RollbackTran();
                throw HSZException.Oh(ErrorCode.D3006);
            }
        }
        #endregion
    }
}
