using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Filter;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.System.Entitys.Dto.System.ModuleColumn;
using HSZ.System.Entitys.Dto.System.ModuleForm;
using HSZ.System.Entitys.System;
using HSZ.System.Entitys.Permission;
using HSZ.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using HSZ.System.Service.Permission;

namespace HSZ.System.Service.System
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：功能按钮
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "ModuleForm", Order = 212)]
    [Route("api/system/[controller]")]
    public class ModuleFormService : IModuleFormService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ModuleFormEntity> _moduleFormRepository; //系统功能按钮表仓储
        private readonly ISqlSugarRepository<AuthorizeEntity> _authorizeRepository; //权限操作表仓储
        private readonly ISqlSugarRepository<UserEntity> _userRepository; //用户仓储
        private readonly ISqlSugarRepository<RoleEntity> _roleRepository;
        private readonly IUserManager _userManager;

        /// <summary>
        /// 初始化一个<see cref="ModuleFormService"/>类型的新实例
        /// </summary>
        public ModuleFormService(ISqlSugarRepository<ModuleFormEntity> moduleFormRepository, ISqlSugarRepository<AuthorizeEntity> authorizeRepository, ISqlSugarRepository<UserEntity> userRepository, ISqlSugarRepository<RoleEntity> roleRepository, IUserManager userManager)
        {
            _moduleFormRepository = moduleFormRepository;
            _authorizeRepository = authorizeRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userManager = userManager;
        }

        #region Get

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="moduleId">功能主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpGet("{moduleId}/Fields")]
        public async Task<dynamic> GetList(string moduleId, [FromQuery] KeywordInput input)
        {
            var list = await _moduleFormRepository.AsQueryable().Where(x => x.ModuleId == moduleId && x.DeleteMark == null)
                .WhereIF(!string.IsNullOrEmpty(input.keyword), t => t.EnCode.Contains(input.keyword) || t.FullName.Contains(input.keyword))
                .OrderBy(x=>x.SortCode).OrderBy(x => x.CreatorTime, OrderByType.Desc).OrderByIF(!string.IsNullOrEmpty(input.keyword), t => t.LastModifyTime, OrderByType.Desc).ToListAsync();
            return new { list = list.Adapt<List<ModuleFormListOutput>>() };
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo_Api(string id)
        {
            var data = await _moduleFormRepository.GetFirstAsync(x => x.Id == id && x.DeleteMark == null);
            var output = data.Adapt<ModuleFormListOutput>();
            return output;
        }
        #endregion

        #region Post
        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ModuleFormCrInput input)
        {
            var entity = input.Adapt<ModuleFormEntity>();
            var isOk = await _moduleFormRepository.AsInsertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync(); ;
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ModuleFormUpInput input)
        {
            var entity = input.Adapt<ModuleFormEntity>();
            var isOk = await _moduleFormRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            if (await _moduleFormRepository.IsAnyAsync(x => x.ParentId == id && x.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.D1007);
            var entity = await _moduleFormRepository.GetFirstAsync(x => x.Id == id && x.DeleteMark == null);
            if (entity == null)
                throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _moduleFormRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1002);
        }

        /// <summary>
        /// 批量新建
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpPost("Actions/Batch")]
        public async Task BatchCreate([FromBody] ModuleFormActionsBatchInput input)
        {
            var entitys = new List<ModuleFormEntity>();
            foreach (var item in input.formJson)
            {
                var entity = input.Adapt<ModuleFormEntity>();
                entity.EnCode = item.enCode;
                entity.FullName = item.fullName;
                entitys.Add(entity);
            }
            var newDic = await _moduleFormRepository.AsInsertable(entitys).CallEntityMethod(m => m.Creator()).ExecuteReturnEntityAsync();
            _ = newDic ?? throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 更新字段状态
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/State")]
        public async Task ActionsState(string id)
        {
            var entity = await _moduleFormRepository.GetFirstAsync(x => x.Id == id && x.DeleteMark == null);
            entity.EnabledMark = entity.EnabledMark == 0 ? 1 : 0;
            var isOk = await _moduleFormRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync(); ;
            if (isOk < 0)
                throw HSZException.Oh(ErrorCode.COM1003);
        }
        #endregion

        #region PublicMethod
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public async Task<List<ModuleFormEntity>> GetList(string moduleId)
        {
            return await _moduleFormRepository.AsQueryable().Where(x => x.ModuleId == moduleId && x.DeleteMark == null).ToListAsync();
        }



        /// <summary>
        /// 获取用户表单列表
        /// </summary>
        /// <param name="isAdmin"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetUserModuleFormList(bool isAdmin, string userId)
        {
            var output = new List<ModuleColumnOutput>();
            if (!isAdmin)
            {
                var userInfo = _userRepository.GetFirst(u => u.Id == userId);
                if (!string.IsNullOrEmpty(userInfo.RoleId))
                {
                    //从用户角色关联表 获取 当前组织角色集合
                    var roleId = await _userManager.GetUserOrgRoleIds(userInfo.RoleId, userInfo.OrganizeId);
                    var items = await _authorizeRepository.AsQueryable().In(a => a.ObjectId, roleId).Where(a => a.ItemType == "form").GroupBy(it => new { it.ItemId }).Select(a => a.ItemId).ToListAsync();
                    var forms = await _moduleFormRepository.AsQueryable().In(a => a.Id, items).Where(a => a.EnabledMark == 1 && a.DeleteMark == null).Select<ModuleFormEntity>().OrderBy(q => q.SortCode).ToListAsync();
                    output = forms.Adapt<List<ModuleColumnOutput>>();
                }
            }
            else
            {
                var forms = await _moduleFormRepository.AsQueryable().Where(a => a.EnabledMark == 1 && a.DeleteMark == null).Select<ModuleFormEntity>().OrderBy(q => q.SortCode).ToListAsync();
                output = forms.Adapt<List<ModuleColumnOutput>>();
            }
            return output;
        }
        #endregion
    }
}
