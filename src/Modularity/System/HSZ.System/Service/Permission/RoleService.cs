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
using HSZ.System.Entitys.Dto.Permission.Role;
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
using Yitter.IdGenerator;

namespace HSZ.System.Service.Permission
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：业务实现：角色信息
    /// </summary>
    [ApiDescriptionSettings(Tag = "Permission", Name = "Role", Order = 167)]
    [Route("api/permission/[controller]")]
    public class RoleService : IRoleService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<OrganizeEntity> _organizeRepository;
        private readonly ISqlSugarRepository<RoleEntity> _roleRepository;  // 角色表仓储
        private readonly ISqlSugarRepository<UserRelationEntity> _userRelationRepository;
        private readonly ISqlSugarRepository<OrganizeRelationEntity> _organizeRelationRepository;
        private readonly IAuthorizeService _authorizeService;
        private readonly ISqlSugarRepository<DictionaryDataEntity> _dictionaryDataRepository;// 数据字典表仓储
        private readonly ICacheManager _cacheManager;
        private readonly IUserManager _userManager;

        /// <summary>
        /// 初始化一个<see cref="RoleService"/>类型的新实例
        /// </summary>
        public RoleService(ISqlSugarRepository<RoleEntity> roleRepository, ISqlSugarRepository<DictionaryDataEntity> dictionaryDataRepository, ISqlSugarRepository<OrganizeEntity> organizeRepository, ISqlSugarRepository<OrganizeRelationEntity> organizeRelationRepository, ISqlSugarRepository<UserRelationEntity> userRelationRepository, IAuthorizeService authorizeService, ICacheManager cacheManager, IUserManager userManager)
        {
            _organizeRepository = organizeRepository;
            _roleRepository = roleRepository;
            _authorizeService = authorizeService;
            _organizeRelationRepository = organizeRelationRepository;
            _userRelationRepository = userRelationRepository;
            _dictionaryDataRepository = dictionaryDataRepository;
            _cacheManager = cacheManager;
            _userManager = userManager;
        }

        #region GET

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] RoleListInput input)
        {
            var oIdList = new List<OrganizeRelationEntity>();

            //获取组织角色关联 列表
            if (input.organizeId != "0")
            {
                oIdList = await _organizeRelationRepository.AsSugarClient().Queryable<OrganizeRelationEntity>()
                    .WhereIF(input.organizeId != "0", x => x.OrganizeId == input.organizeId && x.ObjectType== "Role")
                    .Select(x => new OrganizeRelationEntity { ObjectId = x.ObjectId, OrganizeId = x.OrganizeId }).ToListAsync();
            }

            var list = await _roleRepository.AsSugarClient().Queryable<RoleEntity>()
                .WhereIF(input.organizeId == "0", a => a.GlobalMark == 1)
                .WhereIF(input.organizeId.IsNotEmptyOrNull() && input.organizeId != "0", a=>oIdList.Select(x=>x.ObjectId).Contains(a.Id) )
                .WhereIF(!string.IsNullOrEmpty(input.keyword), a => a.FullName.Contains(input.keyword) || a.EnCode.Contains(input.keyword))
                .Where(a => a.DeleteMark == null)
                .Select((a) => new RoleListOutput
                {
                    id = a.Id,
                    parentId = a.Type,
                    type = SqlFunc.IIF(a.GlobalMark==1,"全局","组织"),
                    enCode = a.EnCode,
                    fullName = a.FullName,
                    description = a.Description,
                    enabledMark = a.EnabledMark,
                    creatorTime = a.CreatorTime,
                    sortCode = a.SortCode
                }).MergeTable().OrderBy(a => a.sortCode).OrderBy(a => a.creatorTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);

            #region 处理 角色 多组织
            //获取组织角色关联 列表
            if (input.organizeId != "0")
            {
                oIdList = await _organizeRelationRepository.AsSugarClient().Queryable<OrganizeRelationEntity>().Select(x => new OrganizeRelationEntity { ObjectId = x.ObjectId, OrganizeId = x.OrganizeId }).ToListAsync();
                var orgList = _organizeRepository.AsQueryable().Where(x => x.DeleteMark == null).OrderBy(x => x.CreatorTime).ToList();
                foreach (var item in list.list)
                {
                    var roleOrgList = oIdList.Where(x => x.ObjectId == item.id).Select(x => x.OrganizeId).ToList();//获取角色组织集合
                    var tree = orgList.Where(x => roleOrgList.Contains(x.Id)).Select(x => x.OrganizeIdTree).ToList();//获取组织树

                    var infoList = new List<string>();

                    tree.ForEach(treeItem =>
                    {
                        if (treeItem.IsNotEmptyOrNull())
                            infoList.Add(string.Join("/", orgList.Where(x => treeItem.Split(",").Contains(x.Id)).Select(x => x.FullName).ToList()));
                    });

                    item.organizeInfo = string.Join(" ; ", infoList);
                }
            }

            #endregion
            return PageResult<RoleListOutput>.SqlSugarPageResult(list);
        }

        /// <summary>
        /// 获取下拉框(类型+角色)
        /// </summary>
        /// <returns></returns>
        [HttpGet("Selector")]
        public async Task<dynamic> GetSelector()
        {
            //获取所有组织 对应 的 角色id集合
            var ridList = await _roleRepository.AsSugarClient().Queryable<OrganizeRelationEntity>().Where(x => x.ObjectType == "Role")
                .Select(x => new { x.ObjectId, x.OrganizeId }).ToListAsync();

            //获取 全局角色 和 组织角色
            var roleList = await _roleRepository.AsSugarClient().Queryable<RoleEntity>()
                .Where(a => a.DeleteMark == null).Where(a => a.GlobalMark == 1 || ridList.Select(x => x.ObjectId).Contains(a.Id))
                .Select(a => new RoleListOutput
                {
                    id = a.Id,
                    parentId = a.GlobalMark.ToString(),
                    fullName = a.FullName,
                    enabledMark = a.EnabledMark,
                    creatorTime = a.CreatorTime,
                    type = "role",
                    icon = "icon-sz icon-sz-global-role",
                    sortCode = a.SortCode
                }).MergeTable().OrderBy(a => a.sortCode).OrderBy(a => a.creatorTime, OrderByType.Desc).ToListAsync();

            for(var i=0;i< roleList.Count; i++)
            {
                roleList[i].onlyId = "role_" + i;
            }

            //处理 组织角色
            roleList.Where(x => ridList.Select(x => x.ObjectId).Contains(x.id)).ToList().ForEach(item =>
            {
                var oolist = ridList.Where(x => x.ObjectId == item.id).ToList();

                for (var i = 0; i < oolist.Count; i++)
                {
                    if (i == 0) item.parentId = oolist.FirstOrDefault().OrganizeId;
                    else//该角色属于多个组织
                    {
                        var newItemStr = item.Serialize();
                        var newItem = newItemStr.Deserialize<RoleListOutput>();
                        newItem.parentId = oolist[i].OrganizeId;
                        roleList.Add(newItem);
                    }
                }
            });

            //设置 全局 根目录
            var treeList = new List<RoleListOutput>() { new RoleListOutput() { id = "1", type = "", parentId = "-1", enCode = "", fullName = "全局", num = roleList.Count(x => x.parentId == "1") } };

            //获取所有组织
            var allOrgList = await _organizeRepository.AsSugarClient().Queryable<OrganizeEntity>().ToListAsync();

            var organizeList = allOrgList.Select(x => new RoleListOutput()
            {
                id = x.Id,
                type = "",
                parentId = x.ParentId,
                icon = x.Category == "company" ? "icon-sz icon-sz-tree-organization3" : "icon-sz icon-sz-tree-department1",
                fullName = x.FullName
            }).ToList();
            treeList.AddRange(organizeList);

            for (var i = 0; i < treeList.Count; i++)
            {
                treeList[i].onlyId = "organizeList_" + i;
            }

            return new { list = treeList.Union(roleList).OrderBy(x => x.sortCode).ToList().ToTree("-1") };
        }

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var entity = await _roleRepository.GetFirstAsync(r => r.Id == id);
            var output = entity.Adapt<RoleInfoOutput>();
            output.organizeIdsTree = new List<List<string>>();

            var oIds = await _organizeRelationRepository.GetListAsync(x => x.ObjectId == id);
            var oList = await _organizeRepository.GetListAsync(x => oIds.Select(o=>o.OrganizeId).Contains(x.Id));

            oList.ForEach(item =>
            {
                var idList = item.OrganizeIdTree?.Split(",").ToList();
                output.organizeIdsTree.Add(idList);
            });

            return output;
        }

        #endregion

        #region POST

        /// <summary>
        /// 获取角色列表 根据组织Id集合
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("getListByOrgIds")]
        public async Task<dynamic> GetListByOrgIds([FromBody] RoleListInput input)
        {
            //获取所有组织 对应 的 角色id集合
            var ridList = await _roleRepository.AsSugarClient().Queryable<OrganizeRelationEntity>()
                .Where(x => x.ObjectType == "Role" && input.organizeIds.Contains(x.OrganizeId))
                .Select(x => new { x.ObjectId ,x.OrganizeId}).ToListAsync();

            //获取 全局角色 和 组织角色
            var roleList = await _roleRepository.AsSugarClient().Queryable<RoleEntity>()
                .Where(a=>a.DeleteMark==null && a.EnabledMark==1).Where(a => a.GlobalMark == 1 || ridList.Select(x => x.ObjectId).Contains(a.Id))
                .Select(a => new RoleListOutput
                {
                    id = a.Id,
                    parentId = a.GlobalMark.ToString(),
                    fullName = a.FullName,
                    enabledMark = a.EnabledMark,
                    creatorTime = a.CreatorTime,
                    sortCode = a.SortCode
                }).MergeTable().OrderBy(a => a.sortCode).OrderBy(a => a.creatorTime, OrderByType.Desc).ToListAsync();

            for (var i = 0; i < roleList.Count; i++)
            {
                roleList[i].onlyId = "role_" + i;
            }

            //处理 组织角色
            roleList.Where(x => ridList.Select(x => x.ObjectId).Contains(x.id)).ToList().ForEach(item =>
            {
                var oolist = ridList.Where(x => x.ObjectId == item.id).ToList();

                for(var i = 0; i < oolist.Count; i++)
                {
                    if (i == 0) item.parentId = oolist.FirstOrDefault().OrganizeId;
                    else//该角色属于多个组织
                    {
                        var newItemStr = item.Serialize();
                        var newItem = newItemStr.Deserialize<RoleListOutput>();
                        newItem.parentId = oolist[i].OrganizeId;
                        roleList.Add(newItem);
                    }
                }
            });

            var treeList = new List<RoleListOutput>() { new RoleListOutput() { id = "1", type = "", parentId = "0", enCode = "", fullName = "全局", num = roleList.Count(x => x.parentId == "1") } };

            //获取所有组织
            var allOrgList = await _organizeRepository.AsSugarClient().Queryable<OrganizeEntity>().OrderBy(x => x.CreatorTime, OrderByType.Asc).ToListAsync();
            allOrgList.ForEach(item =>
            {
                if (item.OrganizeIdTree == null || item.OrganizeIdTree == "") item.OrganizeIdTree = item.Id;
            });

            var organizeList = allOrgList.Where(x => input.organizeIds.Contains(x.Id)).Select(x => new RoleListOutput()
            {
                id = x.Id,
                type = "",
                parentId = "0",
                enCode = "",
                fullName = string.Join("/", allOrgList.Where(all=> x.OrganizeIdTree.Split(",").Contains(all.Id)).Select(x => x.FullName)),
                num = roleList.Count(x => x.parentId == x.id)
            }).ToList();
            treeList.AddRange(organizeList);

            for (var i = 0; i < treeList.Count; i++)
            {
                treeList[i].onlyId = "organizeList_" + i;
            }

            return new { list = treeList.Union(roleList).OrderBy(x => x.sortCode).ToList().ToTree("0") };
        }

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        public async Task Create([FromBody] RoleCrInput input)
        {
            //全局角色 只能超管才能变更
            if (input.globalMark == 1 && !_userManager.IsAdministrator)
                throw HSZException.Oh(ErrorCode.D1612);

            #region 分级权限验证
            var orgIdList = input.organizeIdsTree.Select(x => x.LastOrDefault()).ToList();
            var user = await _userManager.GetUserInfo();
            if (!user.dataScope.Any(it => orgIdList.Contains(it.organizeId) && it.Edit == true) && !user.isAdministrator)
                throw HSZException.Oh(ErrorCode.D1013);
            #endregion

            if (await _roleRepository.IsAnyAsync(r => r.EnCode == input.enCode && r.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.D1600);
            if (await _roleRepository.IsAnyAsync(r => r.FullName == input.fullName && r.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.D1601);

            var entity = input.Adapt<RoleEntity>();
            var isOk = await _roleRepository.AsInsertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.D1602);

            #region 组织角色关系
            if (input.globalMark == 0)
            {
                var oreList = new List<OrganizeRelationEntity>();
                input.organizeIdsTree.ForEach(item =>
                {
                    var id = item.LastOrDefault();
                    if (id.IsNotEmptyOrNull())
                    {
                        var oreEntity = new OrganizeRelationEntity();
                        oreEntity.ObjectType = "Role";
                        oreEntity.CreatorUserId = _userManager.UserId;
                        oreEntity.ObjectId = entity.Id;
                        oreEntity.OrganizeId = id;
                        oreList.Add(oreEntity);
                    }
                });

                isOk = await _organizeRelationRepository.AsInsertable(oreList).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();//插入关系数据
                if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.D1602);
            }
            #endregion

            await _cacheManager.DelRole(_userManager.TenantId + "_" + _userManager.UserId);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _roleRepository.GetFirstAsync(r => r.Id == id && r.DeleteMark == null);
            _ = entity ?? throw HSZException.Oh(ErrorCode.D1608);

            //全局角色 只能超管才能变更
            if (entity.GlobalMark == 1 && !_userManager.IsAdministrator)
                throw HSZException.Oh(ErrorCode.D1612);

            #region 分级权限验证
            //旧数据
            var orgIdList = await _organizeRepository.AsSugarClient().Queryable<OrganizeRelationEntity>().Where(x => x.ObjectId == id && x.ObjectType == "Role").Select(x => x.OrganizeId).ToListAsync();
            var user = await _userManager.GetUserInfo();
            if (!user.dataScope.Any(it => orgIdList.Contains(it.organizeId) && it.Edit == true) && !user.isAdministrator)
                throw HSZException.Oh(ErrorCode.D1013);
            #endregion

            //角色下有数据权限不能删
            var items = await _authorizeService.GetAuthorizeItemIds(entity.Id, "resource");
            if (items.Count > 0)
                throw HSZException.Oh(ErrorCode.D1603);
            //角色下有表单不能删
            items = await _authorizeService.GetAuthorizeItemIds(entity.Id, "form");
            if (items.Count > 0)
                throw HSZException.Oh(ErrorCode.D1606);
            //角色下有列不能删除
            items = await _authorizeService.GetAuthorizeItemIds(entity.Id, "column");
            if (items.Count > 0)
                throw HSZException.Oh(ErrorCode.D1605);
            //角色下有按钮不能删除
            items = await _authorizeService.GetAuthorizeItemIds(entity.Id, "button");
            if (items.Count > 0)
                throw HSZException.Oh(ErrorCode.D1604);
            //角色下有菜单不能删
            items = await _authorizeService.GetAuthorizeItemIds(entity.Id, "module");
            if (items.Count > 0)
                throw HSZException.Oh(ErrorCode.D1606);
            //角色下有用户不能删
            if (await _userRelationRepository.IsAnyAsync(u => u.ObjectType == "Role" && u.ObjectId == id))
                throw HSZException.Oh(ErrorCode.D1607);
            var isOk = await _roleRepository.AsSugarClient().Updateable<RoleEntity>().SetColumns(it => new RoleEntity()
            {
                DeleteMark = 1,
                DeleteTime = SqlFunc.GetDate(),
                DeleteUserId = _userManager.UserId
            }).Where(it => it.Id == id && it.DeleteMark == null).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.D1609);
            //删除角色和组织关联数据
            await _organizeRelationRepository.DeleteAsync(x => x.ObjectType == "Role" && x.ObjectId == id);

            await _cacheManager.DelRole(_userManager.TenantId + "_" + _userManager.UserId);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] RoleUpInput input)
        {
            var oldRole = await _roleRepository.GetByIdAsync(input.id);

            //全局角色 只能超管才能变更
            if (oldRole.GlobalMark == 1 && !_userManager.IsAdministrator)
                throw HSZException.Oh(ErrorCode.D1612);

            #region 分级权限验证
            var user = await _userManager.GetUserInfo();
            //旧数据
            var orgIdList = await _organizeRepository.AsSugarClient().Queryable<OrganizeRelationEntity>().Where(x => x.ObjectId == id && x.ObjectType == "Role").Select(x => x.OrganizeId).ToListAsync();
            if (!user.dataScope.Any(it => orgIdList.Contains(it.organizeId) && it.Edit == true) && !user.isAdministrator)
                throw HSZException.Oh(ErrorCode.D1013);
            //新数据
            orgIdList = input.organizeIdsTree.Select(x => x.LastOrDefault()).ToList();
            if (!user.dataScope.Any(it => orgIdList.Contains(it.organizeId) && it.Edit == true) && !user.isAdministrator)
                throw HSZException.Oh(ErrorCode.D1013);
            #endregion

            if (await _roleRepository.IsAnyAsync(r => r.EnCode == input.enCode && r.DeleteMark == null && r.Id != id))
                throw HSZException.Oh(ErrorCode.D1600);
            if (await _roleRepository.IsAnyAsync(r => r.FullName == input.fullName && r.DeleteMark == null && r.Id != id))
                throw HSZException.Oh(ErrorCode.D1601);

            #region 如果变更组织，该角色下已存在成员，则不允许修改
            if (oldRole.GlobalMark == 0)
            {
                //查找该角色下的所有所属组织id
                var orgRoleList = await _organizeRelationRepository.AsQueryable().Where(x => x.ObjectType == "Role" && x.ObjectId == id).Select(x => x.OrganizeId).ToListAsync();
                //查找该角色下的所有成员id
                var roleUserList = await _userRelationRepository.AsQueryable().Where(x => x.ObjectType == "Role" && x.ObjectId == id).Select(x => x.UserId).ToListAsync();
                //获取带有角色成员的组织集合
                var orgUserCountList = await _userRelationRepository.AsQueryable().Where(x => x.ObjectType == "Organize" && roleUserList.Contains(x.UserId))
                    .GroupBy(it => new { it.ObjectId })
                    .Having(x => SqlFunc.AggregateCount(x.UserId) > 0)
                    .Select(x => new { x.ObjectId, UCount = SqlFunc.AggregateCount(x.UserId) })
                    .ToListAsync();
                var oldList = orgRoleList.Intersect(orgUserCountList.Select(x => x.ObjectId)).ToList();//将两个组织List交集
                var newList = input.organizeIdsTree.Select(x => x.LastOrDefault()).ToList();

                if (oldList.Except(newList).Any())
                    throw HSZException.Oh(ErrorCode.D1613);
            }
            if(oldRole.GlobalMark==1 && input.globalMark==0)//全局改成组织
            {
                if (_userRelationRepository.AsQueryable().Where(x => x.ObjectType == "Role" && x.ObjectId == id).Any())
                    throw HSZException.Oh(ErrorCode.D1615);
            }
            #endregion

            var entity = input.Adapt<RoleEntity>();
            var isOk = await _roleRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.D1610);

            #region 组织角色关系
            await _organizeRelationRepository.DeleteAsync(x => x.ObjectType == "Role" && x.ObjectId == entity.Id);//删除原数据
            if (input.globalMark == 0)
            {
                var oreList = new List<OrganizeRelationEntity>();
                input.organizeIdsTree.ForEach(item =>
                {
                    var id = item.LastOrDefault();
                    if (id.IsNotEmptyOrNull())
                    {
                        var oreEntity = new OrganizeRelationEntity();
                        oreEntity.ObjectType = "Role";
                        oreEntity.CreatorUserId = _userManager.UserId;
                        oreEntity.ObjectId = entity.Id;
                        oreEntity.OrganizeId = id;
                        oreList.Add(oreEntity);
                    }
                });

                //获取所有相关角色用户列表
                var userList = await _roleRepository.AsSugarClient().Queryable<UserEntity>().Where(x => x.RoleId.Contains(entity.Id)).ToListAsync();



                isOk = await _organizeRelationRepository.AsInsertable(oreList).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();//插入关系数据
                if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.D1602);
            }
            #endregion
            await _cacheManager.DelRole(_userManager.TenantId + "_" + _userManager.UserId);
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/State")]
        public async Task UpdateState(string id)
        {
            if (!await _roleRepository.IsAnyAsync(r => r.Id == id && r.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.D1608);

            //只能超管才能变更
            if (!_userManager.IsAdministrator)
                throw HSZException.Oh(ErrorCode.D1612);

            var isOk = await _roleRepository.AsSugarClient().Updateable<RoleEntity>().SetColumns(it => new RoleEntity()
            {
                EnabledMark = SqlFunc.IIF(it.EnabledMark == 1, 0, 1),
                LastModifyUserId = _userManager.UserId,
                LastModifyTime = SqlFunc.GetDate()
            }).Where(it => it.Id == id).ExecuteCommandAsync();

            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.D1610);
            await _cacheManager.DelRole(_userManager.TenantId + "_" + _userManager.UserId);
        }

        #endregion

        #region PublicMethod
        /// <summary>
        /// 名称
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<string> GetName(string ids)
        {
            if (ids.IsNullOrEmpty())
            {
                return "";
            }
            var idList = ids.Split(",").ToList();
            var nameList = new List<string>();
            var roleList =await _roleRepository.AsQueryable().Where(x => x.DeleteMark == null && x.EnabledMark == 1).ToListAsync();
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
