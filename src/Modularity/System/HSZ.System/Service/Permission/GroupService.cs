using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Common.Helper;
using HSZ.Common.Util;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.System.Entitys.Dto.Permission.Position;
using HSZ.System.Entitys.Dto.Permission.Group;
using HSZ.System.Entitys.Permission;
using HSZ.System.Entitys.System;
using HSZ.System.Interfaces.Permission;
using HSZ.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSZ.System.Service.Permission
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：业务实现：分组管理
    /// </summary>
    [ApiDescriptionSettings(Tag = "Permission", Name = "Group", Order = 162)]
    [Route("api/Permission/[controller]")]
    public class GroupService : IUserGroupService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<GroupEntity> _userGroupRepository;
        private readonly ISqlSugarRepository<UserRelationEntity> _userRelationRepository;
        private readonly IUserManager _userManager;

        /// <summary>
        /// 初始化一个<see cref="GroupService"/>类型的新实例
        /// </summary>
        public GroupService(
            ISqlSugarRepository<GroupEntity> userGroupRepository,
            ISqlSugarRepository<UserRelationEntity> userRelationRepository,
            IUserManager userManager)
        {
            _userGroupRepository = userGroupRepository;
            _userRelationRepository = userRelationRepository;
            _userManager = userManager;
        }

        #region GET

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] PageInputBase input)
        {
            var data = await _userGroupRepository.AsSugarClient().Queryable<GroupEntity, DictionaryDataEntity>(
                (a, b) => new JoinQueryInfos(JoinType.Left, a.Type == b.Id && b.DictionaryTypeId == "271905527003350725"))
                //关键字（名称、编码）
                .WhereIF(!input.keyword.IsNullOrEmpty(), a => a.FullName.Contains(input.keyword) || a.EnCode.Contains(input.keyword))
                .Where(a => a.DeleteMark == null).OrderBy(a => a.SortCode)
                .Select((a, b) => new GroupListOutput
                {
                    id = a.Id,
                    fullName = a.FullName,
                    enCode = a.EnCode,
                    type = b.FullName,
                    enabledMark = a.EnabledMark,
                    creatorTime = a.CreatorTime,
                    description = a.Description,
                    sortCode = a.SortCode
                }).ToPagedListAsync(input.currentPage, input.pageSize);

            return PageResult<GroupListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var entity = await _userGroupRepository.GetSingleAsync(p => p.Id == id);
            var output = entity.Adapt<GroupUpInput>();
            return output;
        }

        /// <summary>
        /// 获取下拉框
        /// </summary>
        /// <returns></returns>
        [HttpGet("Selector")]
        public async Task<dynamic> GetSelector()
        {
            //获取所有分组数据
            var groupList = await _userGroupRepository.AsSugarClient().Queryable<GroupEntity>()
                .Where(t => t.EnabledMark == 1 && t.DeleteMark == null)
                .OrderBy(o => o.SortCode).OrderBy(a => a.CreatorTime, OrderByType.Desc).ToListAsync();

            //获取所有分组类型(字典)
            var typeList = await _userGroupRepository.AsSugarClient().Queryable<DictionaryDataEntity>()
                .Where(x => x.DictionaryTypeId == "271905527003350725" && x.DeleteMark == null && x.EnabledMark == 1).ToListAsync();

            var treeList = new List<GroupSelectorOutput>();
            typeList.ForEach(item =>
            {
                if(groupList.Count(x => x.Type == item.Id) > 0)
                {
                    treeList.Add(new GroupSelectorOutput()
                    {
                        id = item.Id,
                        parentId = "0",
                        num = groupList.Count(x => x.Type == item.Id),
                        fullName = item.FullName
                    });
                }
            });

            groupList.ForEach(item =>
            {
                treeList.Add(
                    new GroupSelectorOutput
                    {
                        id = item.Id,
                        parentId = item.Type,
                        fullName = item.FullName,
                        sortCode = item.SortCode
                    });
            });

            return treeList.OrderBy(x => x.sortCode).ToList().ToTree("0");
        }

        #endregion

        #region POST

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] GroupCrInput input)
        {
            if (await _userGroupRepository.IsAnyAsync(p => p.FullName == input.fullName && p.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.D2402);
            if (await _userGroupRepository.IsAnyAsync(p => p.EnCode == input.enCode && p.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.D2401);
            var entity = input.Adapt<GroupEntity>();
            var isOk = await _userGroupRepository.AsInsertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.D2400);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            // 岗位下有用户不能删
            if (await _userRelationRepository.IsAnyAsync(u => u.ObjectType == "Group" && u.ObjectId == id))
                throw HSZException.Oh(ErrorCode.D2406);

            var entity = await _userGroupRepository.GetSingleAsync(p => p.Id == id && p.DeleteMark == null);
            var isOk = await _userGroupRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
            if (!(isOk > 0))
                throw HSZException.Oh(ErrorCode.D2403);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] GroupUpInput input)
        {
            var oldEntity = await _userGroupRepository.GetSingleAsync(it => it.Id == id);
            if (await _userGroupRepository.IsAnyAsync(p => p.FullName == input.fullName && p.DeleteMark == null && p.Id != id))
                throw HSZException.Oh(ErrorCode.D2402);
            if (await _userGroupRepository.IsAnyAsync(p => p.EnCode == input.enCode && p.DeleteMark == null && p.Id != id))
                throw HSZException.Oh(ErrorCode.D2401);

            var entity = input.Adapt<GroupEntity>();
            var isOk = await _userGroupRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (!(isOk > 0))
                throw HSZException.Oh(ErrorCode.D2404);
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/State")]
        public async Task UpdateState(string id)
        {
            if (!await _userGroupRepository.IsAnyAsync(r => r.Id == id && r.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.D2405);

            var isOk = await _userGroupRepository.AsSugarClient().Updateable<GroupEntity>().UpdateColumns(it => new GroupEntity()
            {
                EnabledMark = SqlFunc.IIF(it.EnabledMark == 1, 0, 1),
                LastModifyUserId = _userManager.UserId,
                LastModifyTime = SqlFunc.GetDate()
            }).Where(it => it.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0))
                throw HSZException.Oh(ErrorCode.D6004);
        }

        #endregion

        #region Public

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="id">获取信息</param>
        /// <returns></returns>
        [NonAction]
        public async Task<GroupEntity> GetInfoById(string id)
        {
            return await _userGroupRepository.GetSingleAsync(p => p.Id == id);
        }

        #endregion
    }
}
