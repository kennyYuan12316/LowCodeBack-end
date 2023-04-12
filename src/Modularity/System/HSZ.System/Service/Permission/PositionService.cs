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
    /// 描 述：业务实现：岗位管理
    /// </summary>
    [ApiDescriptionSettings(Tag = "Permission", Name = "Position", Order = 162)]
    [Route("api/Permission/[controller]")]
    public class PositionService : IPositionService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<PositionEntity> _positionRepository;
        private readonly ISqlSugarRepository<UserRelationEntity> _userRelationRepository;
        private readonly ISqlSugarRepository<OrganizeEntity> _organizeRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IOrganizeService _organizeService;
        private readonly IUserManager _userManager;

        /// <summary>
        /// 初始化一个<see cref="PositionService"/>类型的新实例
        /// </summary>
        public PositionService(ISqlSugarRepository<PositionEntity> positionRepository, ISqlSugarRepository<UserRelationEntity> userRelationRepository, ISqlSugarRepository<OrganizeEntity> organizeRepository, IOrganizeService organizeService, ICacheManager cacheManager, IUserManager userManager)
        {
            _organizeRepository = organizeRepository;
            _organizeService = organizeService;
            _userRelationRepository = userRelationRepository;
            _positionRepository = positionRepository;
            _cacheManager = cacheManager;
            _userManager = userManager;
        }

        #region GET

        /// <summary>
        /// 获取列表 根据organizeId
        /// </summary>
        /// <param name="organizeId">参数</param>
        /// <returns></returns>
        [HttpGet("getList/{organizeId}")]
        public async Task<dynamic> GetListByOrganizeId(string organizeId)
        {
            var oid = new List<string>();
            if (!string.IsNullOrWhiteSpace(organizeId))
            {
                //获取组织下的所有组织 id 集合
                var oentity = await _positionRepository.AsSugarClient().Queryable<OrganizeEntity>().ToChildListAsync(x => x.ParentId, organizeId);
                oid = oentity.Select(x=>x.Id).ToList();
            }

            var data = await _positionRepository.AsSugarClient().Queryable<PositionEntity, OrganizeEntity, DictionaryDataEntity>(
                (a, b, c) => new JoinQueryInfos(JoinType.Left, b.Id == a.OrganizeId, JoinType.Left, a.Type == c.EnCode && c.DictionaryTypeId == "dae93f2fd7cd4df999d32f8750fa6a1e"))
                //组织机构
                .WhereIF(!string.IsNullOrWhiteSpace(organizeId), a => oid.Contains(a.OrganizeId))
                .Where(a => a.DeleteMark == null).OrderBy(a => a.SortCode)
                .Select((a, b, c) => new PositionListOutput
                {
                    id = a.Id,
                    fullName = a.FullName,
                    enCode = a.EnCode,
                    type = c.FullName,
                    department = b.FullName,
                    enabledMark = a.EnabledMark,
                    creatorTime = a.CreatorTime,
                    description = a.Description,
                    sortCode = a.SortCode
                }).ToListAsync();
            return data.OrderBy(x => x.sortCode).ToList();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] PositionListQuery input)
        {
            if (input.organizeId == "0")
            {
                var user = await _userManager.GetUserInfo();
                input.organizeId = user.organizeId;
            }

            var data = await _positionRepository.AsSugarClient().Queryable<PositionEntity, OrganizeEntity, DictionaryDataEntity>(
                (a, b, c) => new JoinQueryInfos(JoinType.Left, b.Id == a.OrganizeId, JoinType.Left, a.Type == c.EnCode && c.DictionaryTypeId == "dae93f2fd7cd4df999d32f8750fa6a1e"))
                //组织机构
                .WhereIF(!string.IsNullOrWhiteSpace(input.organizeId), a => a.OrganizeId == input.organizeId)
                //关键字（名称、编码）
                .WhereIF(!input.keyword.IsNullOrEmpty(), a => a.FullName.Contains(input.keyword) || a.EnCode.Contains(input.keyword))
                .Where(a => a.DeleteMark == null).OrderBy(a => a.SortCode)
                .Select((a, b, c) => new PositionListOutput
                {
                    id = a.Id,
                    fullName = a.FullName,
                    enCode = a.EnCode,
                    type = c.FullName,
                    department = b.FullName,
                    enabledMark = a.EnabledMark,
                    creatorTime = a.CreatorTime,
                    description = a.Description,
                    organizeId=b.OrganizeIdTree,
                    sortCode = a.SortCode
                }).ToPagedListAsync(input.currentPage, input.pageSize);

            #region 处理岗位所属组织树
            var orgList = await _positionRepository.AsSugarClient().Queryable<OrganizeEntity>().Where(x => x.DeleteMark == null).OrderBy(x => x.CreatorTime).ToListAsync();

            foreach (var item in data.list)
            {
                if (item.organizeId.IsNotEmptyOrNull())
                {
                    var tree = orgList.Where(x => item.organizeId.Split(",").Contains(x.Id)).Select(x => x.FullName).ToList();
                    item.department = string.Join("/", tree);
                }
            }

            #endregion

            return PageResult<PositionListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("All")]
        public async Task<dynamic> GetList()
        {
            var data = await _positionRepository.AsSugarClient().Queryable<PositionEntity, OrganizeEntity, DictionaryDataEntity>((a, b, c) => new JoinQueryInfos(JoinType.Left, b.Id == a.OrganizeId, JoinType.Left, a.Type == c.EnCode && c.DictionaryTypeId == "dae93f2fd7cd4df999d32f8750fa6a1e"))
                .Where(a => a.DeleteMark == null && a.EnabledMark == 1).OrderBy(a => a.SortCode)
                .Select((a, b, c) => new PositionListOutput
                {
                    id = a.Id,
                    fullName = a.FullName,
                    enCode = a.EnCode,
                    type = c.FullName,
                    department = b.FullName,
                    enabledMark = a.EnabledMark,
                    creatorTime = a.CreatorTime,
                    description = a.Description,
                    sortCode = a.SortCode
                }).ToListAsync();
            return new { list = data.OrderBy(x => x.sortCode).ToList() };
        }

        /// <summary>
        /// 获取下拉框（公司+部门+岗位）
        /// </summary>
        /// <returns></returns>
        [HttpGet("Selector")]
        public async Task<dynamic> GetSelector()
        {
            var organizeList = await _organizeService.GetListAsync();
            var positionList = await _positionRepository.AsQueryable().Where(t => t.EnabledMark == 1 && t.DeleteMark == null).OrderBy(o => o.SortCode).OrderBy(a => a.CreatorTime, OrderByType.Desc).ToListAsync();
            var treeList = new List<PositionSelectorOutput>();
            organizeList.ForEach(item =>
            {
                var icon = "";
                if (item.Category.Equals("department"))
                {
                    icon = "icon-sz icon-sz-tree-department1";
                }
                else
                {
                    icon = "icon-sz icon-sz-tree-organization3";
                }
                treeList.Add(
                    new PositionSelectorOutput
                    {
                        id = item.Id,
                        parentId = item.ParentId,
                        fullName = item.FullName,
                        enabledMark = item.EnabledMark,
                        icon = icon,
                        type = item.Category,
                        sortCode = item.SortCode
                    });
            });
            positionList.ForEach(item =>
            {
                treeList.Add(
                    new PositionSelectorOutput
                    {
                        id = item.Id,
                        parentId = item.OrganizeId,
                        fullName = item.FullName,
                        enabledMark = item.EnabledMark,
                        icon = "icon-sz icon-sz-tree-position1",
                        type = "position",
                        sortCode = item.SortCode
                    });
            });
            return new { list = treeList.OrderBy(x => x.sortCode).ToList().ToTree("-1") };
        }

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var entity = await _positionRepository.GetSingleAsync(p => p.Id == id);
            var output = entity.Adapt<PositionInfoOutput>();
            return output;
        }

        #endregion

        #region POST

        /// <summary>
        /// 获取岗位列表 根据组织Id集合
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("getListByOrgIds")]
        public async Task<dynamic> GetListByOrgIds([FromBody] PositionListQuery input)
        {
            var data = await _positionRepository.AsSugarClient().Queryable<PositionEntity, OrganizeEntity, DictionaryDataEntity>(
                (a, b, c) => new JoinQueryInfos(JoinType.Left, b.Id == a.OrganizeId, JoinType.Left, a.Type == c.EnCode && c.DictionaryTypeId == "dae93f2fd7cd4df999d32f8750fa6a1e"))
                .Where(a => input.organizeIds.Contains(a.OrganizeId) && a.DeleteMark==null && a.EnabledMark==1).OrderBy(a => a.SortCode)
                .Select((a, b, c) => new PositionListOutput
                {
                    id = a.Id,
                    parentId = b.Id,
                    fullName = a.FullName,
                    enabledMark = a.EnabledMark,
                    creatorTime = a.CreatorTime,
                    sortCode = a.SortCode,
                    isLeaf=true
                }).ToListAsync();

            //获取所有组织
            var allOrgList = await _organizeRepository.AsSugarClient().Queryable<OrganizeEntity>().OrderBy(x => x.CreatorTime, OrderByType.Asc).ToListAsync();
            allOrgList.ForEach(item =>
            {
                item.ParentId = "0";
                if (item.OrganizeIdTree == null || item.OrganizeIdTree == "") item.OrganizeIdTree = item.Id;
            });

            var organizeList = allOrgList.Where(x => input.organizeIds.Contains(x.Id)).Select(x => new PositionListOutput()
            {
                id = x.Id,
                parentId = "0",
                fullName = string.Join("/", allOrgList.Where(all => x.OrganizeIdTree.Split(",").Contains(all.Id)).Select(x => x.FullName)),
                num = data.Count(x => x.parentId == x.id)
            }).ToList();

            return new { list = organizeList.Union(data).OrderBy(x => x.sortCode).ToList().ToTree("0") };
        }

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] PositionCrInput input)
        {
            var user = await _userManager.GetUserInfo();
            if (!user.dataScope.Any(it => it.organizeId == input.organizeId && it.Add == true) && !user.isAdministrator)
            {
                throw HSZException.Oh(ErrorCode.D1013);
            }
            if (await _positionRepository.IsAnyAsync(p => p.OrganizeId == input.organizeId && p.FullName == input.fullName && p.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.D6005);
            if (await _positionRepository.IsAnyAsync(p => p.OrganizeId == input.organizeId && p.EnCode == input.enCode && p.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.D6000);
            var entity = input.Adapt<PositionEntity>();
            var isOk = await _positionRepository.AsInsertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.D6001);
            await _cacheManager.DelPosition(_userManager.TenantId + "_" + _userManager.UserId);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var user = await _userManager.GetUserInfo();
            var entity = await _positionRepository.GetSingleAsync(p => p.Id == id && p.DeleteMark == null);
            if (!user.dataScope.Any(it => it.organizeId == entity.OrganizeId && it.Delete == true) && !user.isAdministrator)
            {
                throw HSZException.Oh(ErrorCode.D1013);
            }
            // 岗位下有用户不能删
            if (await _userRelationRepository.IsAnyAsync(u => u.ObjectType == "Position" && u.ObjectId == id))
                throw HSZException.Oh(ErrorCode.D6007);

            var isOk = await _positionRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
            //var isOk = await _positionRepository.AsSugarClient().Updateable<PositionEntity>().UpdateColumns(it => new PositionEntity()
            //{
            //    DeleteMark = 1,
            //    DeleteTime = SqlFunc.GetDate(),
            //    DeleteUserId = user.userId
            //}).Where(it => it.Id == id && it.DeleteMark == null).ExecuteCommandAsync();
            if (!(isOk > 0))
                throw HSZException.Oh(ErrorCode.D6002);
            await _cacheManager.DelPosition(_userManager.TenantId + "_" + _userManager.UserId);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] PositionUpInput input)
        {
            var user = await _userManager.GetUserInfo();
            var oldEntity = await _positionRepository.GetSingleAsync(it => it.Id == id);
            if (oldEntity.OrganizeId != input.organizeId && !user.dataScope.Any(it => it.organizeId == oldEntity.OrganizeId && it.Edit == true) && !user.isAdministrator)
                throw HSZException.Oh(ErrorCode.D1013);
            if (!user.dataScope.Any(it => it.organizeId == input.organizeId && it.Edit == true) && !user.isAdministrator)
                throw HSZException.Oh(ErrorCode.D1013);
            if (await _positionRepository.IsAnyAsync(p => p.OrganizeId == input.organizeId && p.FullName == input.fullName && p.DeleteMark == null && p.Id != id))
                throw HSZException.Oh(ErrorCode.D6005);
            if (await _positionRepository.IsAnyAsync(p => p.OrganizeId == input.organizeId && p.EnCode == input.enCode && p.DeleteMark == null && p.Id != id))
                throw HSZException.Oh(ErrorCode.D6000);

            //如果变更组织，该岗位下已存在成员，则不允许修改
            if (input.organizeId != oldEntity.OrganizeId)
            {
                if (await _userRelationRepository.IsAnyAsync(u => u.ObjectType == "Position" && u.ObjectId == id))
                    throw HSZException.Oh(ErrorCode.D6008);
            }

            var entity = input.Adapt<PositionEntity>();
            var isOk = await _positionRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (!(isOk > 0))
                throw HSZException.Oh(ErrorCode.D6003);
            await _cacheManager.DelPosition(_userManager.TenantId + "_" + _userManager.UserId);
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/State")]
        public async Task UpdateState(string id)
        {
            var user = await _userManager.GetUserInfo();
            if (!user.dataScope.Any(it => it.organizeId == id && it.Add == true) && !user.isAdministrator)
            {
                throw HSZException.Oh(ErrorCode.D1013);
            }
            if (!await _positionRepository.IsAnyAsync(r => r.Id == id && r.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.D6006);

            var isOk = await _positionRepository.AsSugarClient().Updateable<PositionEntity>().UpdateColumns(it => new PositionEntity()
            {
                EnabledMark = SqlFunc.IIF(it.EnabledMark == 1, 0, 1),
                LastModifyUserId = _userManager.UserId,
                LastModifyTime = SqlFunc.GetDate()
            }).Where(it => it.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0))
                throw HSZException.Oh(ErrorCode.D6004);
            await _cacheManager.DelPosition(_userManager.TenantId + "_" + _userManager.UserId);
        }

        #endregion

        #region PublicMethod

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="id">获取信息</param>
        /// <returns></returns>
        [NonAction]
        public async Task<PositionEntity> GetInfoById(string id)
        {
            return await _positionRepository.GetSingleAsync(p => p.Id == id);
        }

        /// <summary>
        /// 获取岗位列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<PositionEntity>> GetListAsync()
        {
            return await _positionRepository.AsQueryable().Where(u => u.DeleteMark == null).ToListAsync();
        }

        /// <summary>
        /// 名称
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [NonAction]
        public string GetName(string ids)
        {
            if (ids.IsNullOrEmpty())
            {
                return "";
            }
            var idList = ids.Split(",").ToList();
            var nameList = new List<string>();
            var roleList = _positionRepository.AsQueryable().Where(x => x.DeleteMark == null && x.EnabledMark == 1).ToList();
            foreach (var item in idList)
            {
                var info = roleList.Find(x => x.Id == item);
                if (info != null && info.FullName.IsNotEmptyOrNull())
                {
                    nameList.Add(info.FullName);
                }
            }
            var name = string.Join(",", nameList);
            return name;
        }
        #endregion
    }
}
