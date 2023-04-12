using HSZ.Apps.Entitys;
using HSZ.Apps.Entitys.Dto;
using HSZ.Apps.Interfaces;
using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Util;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.System.Entitys.Permission;
using HSZ.System.Entitys.System;
using HSZ.System.Interfaces.System;
using HSZ.WorkFlow.Interfaces.FlowEngine;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSZ.Apps
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：App常用数据
    /// </summary>
    [ApiDescriptionSettings(Tag = "App", Name = "Data", Order = 800)]
    [Route("api/App/[controller]")]
    public class AppDataService :IAppDataService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<AppDataEntity> _appDataRepository;
        private readonly IUserManager _userManager;
        private readonly IDictionaryDataService _dictionaryDataService;
        private readonly IFlowEngineService _flowEngineService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appDataRepository"></param>
        /// <param name="userManager"></param>
        /// <param name="dictionaryDataService"></param>
        /// <param name="flowEngineService"></param>
        public AppDataService(ISqlSugarRepository<AppDataEntity> appDataRepository, IUserManager userManager, IDictionaryDataService dictionaryDataService, IFlowEngineService flowEngineService)
        {
            _appDataRepository = appDataRepository;
            _userManager = userManager;
            _dictionaryDataService = dictionaryDataService;
            _flowEngineService = flowEngineService;
        }

        #region Get
        /// <summary>
        /// 常用数据
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery]string type)
        {
            var list = await GetListByType(type);
            var output = list.Adapt<List<AppDataListOutput>>();
            return new { list = output };
        }

        /// <summary>
        /// 所有流程
        /// </summary>
        /// <returns></returns>
        [HttpGet("getFlowList")]
        public async Task<dynamic> GetFlowList()
        {
            var list1 = await _flowEngineService.GetFlowFormList();
            var dicDataInfo = await _dictionaryDataService.GetInfo(list1.First().parentId);
            var dicDataList = (await _dictionaryDataService.GetList(dicDataInfo.DictionaryTypeId)).FindAll(x=>x.EnabledMark==1);
            var list2 = new List<AppFlowListAllOutput>();
            foreach (var item in dicDataList)
            {
                list2.Add(new AppFlowListAllOutput()
                {
                    fullName = item.FullName,
                    parentId = "0",
                    id = item.Id,
                    num = list1.FindAll(x => x.category == item.EnCode).Count
                });
            }
            var appList = list1.Adapt<List<AppFlowListAllOutput>>();
            foreach (var item in appList)
            {
                item.isData = _appDataRepository.IsAny(x =>x.ObjectType=="1" && x.CreatorUserId == _userManager.UserId && x.ObjectId == item.id && x.DeleteMark == null);
            }
            var output = appList.Union(list2).ToList().ToTree();
            return new { list = output };
        }

        /// <summary>
        /// 所有流程
        /// </summary>
        /// <returns></returns>
        [HttpGet("getDataList")]
        public async Task<dynamic> GetDataList()
        {
            var list = (await GetAppMenuList()).Adapt<List<AppDataListAllOutput>>();
            foreach (var item in list)
            {
                item.isData = _appDataRepository.IsAny(x => x.ObjectType == "2" && x.CreatorUserId == _userManager.UserId && x.ObjectId == item.id && x.DeleteMark == null);
            }
            var output = list.ToTree("-1");
            return new { list = output };
        }
        #endregion

        #region Post
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody]AppDataCrInput input)
        {
            var entity = input.Adapt<AppDataEntity>();
            var isOk= await _appDataRepository.AsInsertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        [HttpDelete("{objectId}")]
        public async Task Delete(string objectId)
        {
            var entity = await _appDataRepository.GetFirstAsync(x => x.ObjectId == objectId && x.CreatorUserId == _userManager.UserId && x.DeleteMark == null);
            if (entity == null)
                throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _appDataRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1000);
        }
        #endregion

        #region PrivateMethod
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private async Task<List<AppDataEntity>> GetListByType(string type)
        {
            return await _appDataRepository.AsQueryable().Where(x => x.ObjectType == type && x.CreatorUserId == _userManager.UserId && x.DeleteMark == null).ToListAsync();
        }

        /// <summary>
        /// 菜单列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<ModuleEntity>> GetAppMenuList()
        {
            var menuList = new List<ModuleEntity>();
            if (_userManager.IsAdministrator)
            {
                menuList = await _appDataRepository.AsSugarClient().Queryable<ModuleEntity>().Where(x => x.EnabledMark == 1 && x.Category == "App"&&x.DeleteMark==null).ToListAsync();
            }
            else
            {
                var objectIds = (await _userManager.GetUserInfo()).roleIds;
                if (objectIds.Length==0)
                    return menuList;
                var ids = await _appDataRepository.AsSugarClient().Queryable<AuthorizeEntity>().In(x => x.ObjectId, objectIds).Where(x => x.ObjectType == "Role"&&x.ItemType== "module").Select(x => x.ItemId).ToListAsync();
                if (ids.Count == 0)
                    return menuList;
                menuList = await _appDataRepository.AsSugarClient().Queryable<ModuleEntity>().In(x=>x.Id,ids.ToArray()).Where(x => x.EnabledMark == 1 && x.Category == "App" && x.DeleteMark == null).ToListAsync();
            }
            return menuList;
        } 
        #endregion
    }
}
