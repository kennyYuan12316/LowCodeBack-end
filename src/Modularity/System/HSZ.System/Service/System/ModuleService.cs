using HSZ.Apps.Interfaces;
using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Util;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.JsonSerialization;
using HSZ.System.Entitys.Dto.System.Module;
using HSZ.System.Entitys.Permission;
using HSZ.System.Entitys.System;
using HSZ.System.Interfaces.Common;
using HSZ.System.Interfaces.System;
using HSZ.System.Service.Permission;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HSZ.System.Service.System
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：菜单管理
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "Menu", Order = 212)]
    [Route("api/system/[controller]")]
    public class ModuleService : IModuleService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ModuleEntity> _moduleRepository; //系统功能表仓储
        private readonly ISqlSugarRepository<AuthorizeEntity> _authorizeRepository; //权限操作表仓储
        private readonly ISqlSugarRepository<UserEntity> _userRepository; //用户仓储
        private readonly IModuleButtonService _moduleButtonService;
        private readonly IModuleColumnService _moduleColumnService;
        private readonly IModuleDataAuthorizeSchemeService _moduleDataAuthorizeSchemeService;
        private readonly IModuleDataAuthorizeSerive _moduleDataAuthorizeSerive;
        private readonly IModuleFormService _moduleFormSerive;
        private readonly IFileService _fileService;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope Db;

        /// <summary>
        /// 初始化一个<see cref="ModuleService"/>类型的新实例
        /// </summary>
        public ModuleService(ISqlSugarRepository<ModuleEntity> moduleRepository,
            ISqlSugarRepository<AuthorizeEntity> authorizeRepository,
            ISqlSugarRepository<UserEntity> userRepository,
            IModuleButtonService moduleButtonService,
            IModuleColumnService moduleColumnService,
            IModuleDataAuthorizeSchemeService moduleDataAuthorizeSchemeService,
            IModuleDataAuthorizeSerive moduleDataAuthorizeSerive, IUserManager userManager,
            IFileService fileService, IModuleFormService moduleFormSerive)
        {
            _moduleRepository = moduleRepository;
            _authorizeRepository = authorizeRepository;
            _userRepository = userRepository;
            _moduleButtonService = moduleButtonService;
            _moduleColumnService = moduleColumnService;
            _moduleDataAuthorizeSchemeService = moduleDataAuthorizeSchemeService;
            _moduleDataAuthorizeSerive = moduleDataAuthorizeSerive;
            _fileService = fileService;
            _userManager = userManager;
            _moduleFormSerive = moduleFormSerive;
            _userManager = userManager;
            Db = DbScoped.SugarScope;
        }

        #region GET

        /// <summary>
        /// 获取菜单列表
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ModuleListQuery input)
        {
            try
            {
                var data = await GetList();
                if (!string.IsNullOrEmpty(input.category))
                    data = data.FindAll(x => x.Category == input.category);
                if (!string.IsNullOrEmpty(input.keyword))
                    data = data.TreeWhere(t => t.FullName.Contains(input.keyword) || t.EnCode.Contains(input.keyword) || (t.UrlAddress.IsNotEmptyOrNull() && t.UrlAddress.Contains(input.keyword)), t => t.Id, t => t.ParentId);
                var treeList = data.Adapt<List<ModuleListOutput>>();
                return new { list = treeList.ToTree("-1") };
            }
            catch (Exception e)
            {

                throw;
            }
        }

        /// <summary>
        /// 获取菜单下拉框
        /// </summary>
        /// <param name="category">菜单分类（参数有Web,App），默认显示所有分类</param>
        /// <returns></returns>
        [HttpGet("Selector/{id}")]
        public async Task<dynamic> GetSelector(string id, string category)
        {
            var data = await GetList();
            if (!string.IsNullOrEmpty(category))
                data = data.FindAll(x => x.Category == category && x.Type == 1);
            if (!id.Equals("0"))
            {
                data.RemoveAll(x => x.Id == id);
            }
            var treeList = data.Adapt<List<ModuleSelectorOutput>>();
            return new { list = treeList.ToTree("-1") };
        }

        /// <summary>
        /// 获取菜单列表（下拉框）
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpGet("Selector/All")]
        public async Task<dynamic> GetSelectorAll(string category)
        {
            var data = await GetList();
            if (!string.IsNullOrEmpty(category))
                data = data.FindAll(x => x.Category == category);
            var treeList = data.Adapt<List<ModuleSelectorAllOutput>>();
            return new { list = treeList.ToTree("-1") };
        }

        /// <summary>
        /// 获取菜单信息
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo_Api(string id)
        {
            var data = await GetInfo(id);
            var output = data.Adapt<ModuleInfoOutput>();
            return output;
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/Action/Export")]
        public async Task<dynamic> ActionsExport(string id)
        {
            var data = (await GetInfo(id)).Adapt<ModuleExportInput>();
            data.buttonEntityList = (await _moduleButtonService.GetList(id)).Adapt<List<ButtonEntityListItem>>();
            data.columnEntityList = (await _moduleColumnService.GetList(id)).Adapt<List<ColumnEntityListItem>>();
            data.authorizeEntityList = (await _moduleDataAuthorizeSerive.GetList(id)).Adapt<List<AuthorizeEntityListItem>>();
            data.schemeEntityList = (await _moduleDataAuthorizeSchemeService.GetList(id)).Adapt<List<SchemeEntityListItem>>();
            data.formEntityList = await _moduleFormSerive.GetList(id);
            var jsonStr = data.Serialize();
            return _fileService.Export(jsonStr, data.fullName);
        }
        #endregion

        #region Post

        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Creater([FromBody] ModuleCrInput input)
        {
            if (await _moduleRepository.IsAnyAsync(x => x.EnCode == input.enCode && x.DeleteMark == null && x.Category == input.category) || await _moduleRepository.IsAnyAsync(x => x.FullName == input.fullName && x.DeleteMark == null && x.Category == input.category && input.parentId == x.ParentId))
                throw HSZException.Oh(ErrorCode.COM1004);
            var entity = input.Adapt<ModuleEntity>();
            //添加字典菜单按钮
            if (entity.Type == 4)
            {
                var btnEntityList = (await _moduleButtonService.GetList()).FindAll(x => x.ModuleId == "-1");
                foreach (var item in btnEntityList)
                {
                    item.ModuleId = entity.Id;
                    await _moduleButtonService.Create(item);
                }
            }
            var isOk = await Create(entity);
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 修改菜单
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ModuleUpInput input)
        {
            if (await _moduleRepository.IsAnyAsync(x => x.Id != id && x.EnCode == input.enCode && x.DeleteMark == null && x.Category == input.category)
                || await _moduleRepository.IsAnyAsync(x => x.Id != id && x.FullName == input.fullName && x.DeleteMark == null && x.Category == input.category && input.parentId == x.ParentId))
                throw HSZException.Oh(ErrorCode.COM1004);
            var info = await _moduleRepository.GetFirstAsync(x => x.Id == id && x.DeleteMark == null);
            if (info.Type == 1 && info.Type != input.type && await _moduleRepository.IsAnyAsync(x => x.ParentId == id && x.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.D4008);
            var entity = input.Adapt<ModuleEntity>();
            entity.Id = id;
            var isOk = await Update(entity);
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            try
            {
                var entity = await _moduleRepository.GetFirstAsync(x => x.Id == id && x.DeleteMark == null);
                if (entity == null || await _moduleRepository.IsAnyAsync(x => x.ParentId == id && x.DeleteMark == null))
                    throw HSZException.Oh(ErrorCode.D1007);
                Db.BeginTran();
                if (entity.Category.Equals("App"))
                {
                    var appDataService = App.GetService<IAppDataService>();
                    await appDataService.Delete(entity.Id);
                }
                var isOk = await Delete(entity);
                if (isOk < 1)
                    throw HSZException.Oh(ErrorCode.COM1002);
                Db.CommitTran();
            }
            catch (Exception)
            {
                Db.RollbackTran();
                throw;
            }
        }

        /// <summary>
        /// 更新菜单状态
        /// </summary>
        /// <param name="id">菜单id</param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/State")]
        public async Task ActionsState(string id)
        {
            var entity = await _moduleRepository.GetFirstAsync(x => x.Id == id && x.DeleteMark == null);
            entity.EnabledMark = entity.EnabledMark == 0 ? 1 : 0;
            var isOk = await Update(entity);
            if (isOk < 0)
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
            var moduleModel = josn.Deserialize<ModuleExportInput>();
            if (moduleModel == null || moduleModel.linkTarget.IsNullOrEmpty())
                throw HSZException.Oh(ErrorCode.D3006);
            if (moduleModel.parentId != "-1" && !_moduleRepository.IsAny(x => x.Id == moduleModel.parentId && x.DeleteMark == null))
            {
                throw HSZException.Oh(ErrorCode.D3007);
            }
            if (await _moduleRepository.IsAnyAsync(x => x.EnCode == moduleModel.enCode && x.DeleteMark == null) || await _moduleRepository.IsAnyAsync(x => x.FullName == moduleModel.fullName && x.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.D4000);
            await ImportData(moduleModel);
        }
        #endregion

        #region PublicMethod

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<ModuleEntity>> GetList()
        {
            return await _moduleRepository.AsQueryable().Where(x => x.DeleteMark == null).OrderBy(o => o.SortCode).OrderBy(t => t.CreatorTime, OrderByType.Desc).OrderBy(t => t.LastModifyTime, OrderByType.Desc).ToListAsync();
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [NonAction]
        public async Task<ModuleEntity> GetInfo(string id)
        {
            return await _moduleRepository.GetFirstAsync(x => x.Id == id && x.DeleteMark == null);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Delete(ModuleEntity entity)
        {
            return await _moduleRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Create(ModuleEntity entity)
        {
            return await _moduleRepository.AsInsertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Update(ModuleEntity entity)
        {
            return await _moduleRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 获取用户树形模块功能列表
        /// </summary>
        /// <param name="isAdmin"></param>
        /// <param name="userId"></param>
        /// <param name="type">Web、App</param>
        /// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetUserTreeModuleList(bool isAdmin, string userId, string type)
        {
            var output = new List<ModuleNodeOutput>();
            if (!isAdmin)
            {
                var userInfo = _userRepository.GetFirst(u => u.Id == userId);
                if (!string.IsNullOrEmpty(userInfo.RoleId))
                {
                    //获取当前用户、全局角色 和当前组织下的所有角色
                    var roleId = await _userManager.GetUserOrgRoleIds(userInfo.RoleId, userInfo.OrganizeId);
                    var items = await _authorizeRepository.AsQueryable().In(a => a.ObjectId, roleId).Where(a => a.ItemType == "module").Select(a => a.ItemId).ToListAsync();
                    var menus = await _moduleRepository.AsQueryable().In(a => a.Id, items).Where(a => a.EnabledMark == 1 && a.Category.Equals(type) && a.DeleteMark == null).Select<ModuleEntity>().OrderBy(q => q.ParentId).OrderBy(q => q.SortCode).ToListAsync();
                    output = menus.Adapt<List<ModuleNodeOutput>>();
                }
            }
            else
            {
                var menus = await _moduleRepository.AsQueryable().Where(a => a.EnabledMark==1 && a.Category.Equals(type) && a.DeleteMark == null).Select<ModuleEntity>().OrderBy(q => q.ParentId).OrderBy(q => q.SortCode).ToListAsync();
                output = menus.Adapt<List<ModuleNodeOutput>>();
            }
            return output.ToTree("-1");
        }

        /// <summary>
        /// 获取用户菜单模块功能列表
        /// </summary>
        /// <param name="isAdmin"></param>
        /// <param name="userId"></param>
        /// <param name="type">Web、App</param>
        /// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetUserModueList(bool isAdmin, string userId, string type)
        {
            var output = new List<ModuleOutput>();
            if (!isAdmin)
            {
                var userInfo = _userRepository.GetFirst(u => u.Id == userId);
                if (!string.IsNullOrEmpty(userInfo.RoleId))
                {
                    //获取当前用户、全局角色 和当前组织下的所有角色
                    var roleId = await _userManager.GetUserOrgRoleIds(userInfo.RoleId, userInfo.OrganizeId);
                    var items = await _authorizeRepository.AsQueryable().In(a => a.ObjectId, roleId).Where(a => a.ItemType == "module").GroupBy(it => new { it.ItemId }).Select(it => new { it.ItemId }).ToListAsync();
                    if (items.Count == 0) return output;
                    output = await _moduleRepository.AsQueryable().In(a => a.Id, items.Select(it => it.ItemId).ToArray()).Where(a => a.EnabledMark == 1 && a.Category.Equals(type) && a.DeleteMark == null).Select(a => new { Id = a.Id, FullName = a.FullName, SortCode = a.SortCode }).MergeTable().OrderBy(o => o.SortCode).Select<ModuleOutput>().ToListAsync();
                }
            }
            else
            {
                output = await _moduleRepository.AsQueryable().Where(a => a.EnabledMark == 1 && a.Category.Equals(type) && a.DeleteMark == null).Select(a => new { Id = a.Id, FullName = a.FullName, SortCode = a.SortCode }).MergeTable().OrderBy(o => o.SortCode).Select<ModuleOutput>().ToListAsync();
            }

            return output;
        }

        #endregion

        #region PrivateMethod
        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task ImportData(ModuleExportInput data)
        {
            try
            {
                var module = data.Adapt<ModuleEntity>();
                var button = data.buttonEntityList.Adapt<List<ModuleButtonEntity>>();
                var colum = data.buttonEntityList.Adapt<List<ModuleColumnEntity>>();
                var dataAuthorize = data.buttonEntityList.Adapt<List<ModuleDataAuthorizeEntity>>();
                var dataAuthorizeScheme = data.buttonEntityList.Adapt<List<ModuleDataAuthorizeSchemeEntity>>();

                Db.BeginTran();
                var storBtn = _moduleRepository.AsSugarClient().Storageable(button).Saveable().ToStorage(); //存在更新不存在插入 根据主键
                await storBtn.AsInsertable.ExecuteCommandAsync(); //执行插入
                await storBtn.AsUpdateable.ExecuteCommandAsync(); //执行更新　

                var storcolum = _moduleRepository.AsSugarClient().Storageable(colum).Saveable().ToStorage(); //存在更新不存在插入 根据主键
                await storcolum.AsInsertable.ExecuteCommandAsync(); //执行插入
                await storcolum.AsUpdateable.ExecuteCommandAsync(); //执行更新

                var storAuthorize = _moduleRepository.AsSugarClient().Storageable(dataAuthorize).Saveable().ToStorage(); //存在更新不存在插入 根据主键
                await storAuthorize.AsInsertable.ExecuteCommandAsync(); //执行插入
                await storAuthorize.AsUpdateable.ExecuteCommandAsync(); //执行更新

                var storAuthorizeScheme = _moduleRepository.AsSugarClient().Storageable(dataAuthorizeScheme).Saveable().ToStorage();
                await storAuthorizeScheme.AsInsertable.ExecuteCommandAsync(); //执行插入
                await storAuthorizeScheme.AsUpdateable.ExecuteCommandAsync(); //执行更新

                var stroForm = _moduleRepository.AsSugarClient().Storageable(data.formEntityList).Saveable().ToStorage(); //存在更新不存在插入 根据主键
                await stroForm.AsInsertable.ExecuteCommandAsync(); //执行插入
                await stroForm.AsUpdateable.ExecuteCommandAsync(); //执行更新

                var stroModule = _moduleRepository.AsSugarClient().Storageable(module).Saveable().ToStorage(); //存在更新不存在插入 根据主键
                await stroModule.AsInsertable.ExecuteCommandAsync(); //执行插入
                await stroModule.AsUpdateable.ExecuteCommandAsync(); //执行更新
                Db.CommitTran();
            }
            catch (Exception ex)
            {
                Db.RollbackTran();
                throw HSZException.Oh(ErrorCode.D3008);
            }
        }

        #endregion
    }
}
