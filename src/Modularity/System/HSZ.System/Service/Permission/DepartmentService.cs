using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Filter;
using HSZ.Common.Util;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.System.Entitys.Dto.Permission.Department;
using HSZ.System.Entitys.Dto.Permission.Organize;
using HSZ.System.Entitys.Model.Permission.User;
using HSZ.System.Entitys.Permission;
using HSZ.System.Interfaces.Permission;
using HSZ.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
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
    /// 描 述：业务实现：部门管理
    /// </summary>
    [ApiDescriptionSettings(Tag = "Permission", Name = "Organize", Order = 166)]
    [Route("api/permission/[controller]")]
    public class DepartmentService : IDepartmentService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<OrganizeEntity> _departmentRepository;
        private readonly ISqlSugarRepository<PositionEntity> _positionRepository;
        private readonly IOrganizeService _organizeService;
        private readonly ISqlSugarRepository<UserEntity> _userRepository;
        private readonly ISysConfigService _sysConfigService;
        private readonly ISynThirdInfoService _synThirdInfoService;
        private readonly IUserManager _userManager;
        /// <summary>
        /// 初始化一个<see cref="DepartmentService"/>类型的新实例
        /// </summary>
        /// <param name="departmentRepository"></param>
        /// <param name="positionRepository"></param>
        /// <param name="userRepository"></param>
        /// <param name="organizeService"></param>
        /// <param name="sysConfigService"></param>
        /// <param name="synThirdInfoService"></param>
        public DepartmentService(ISqlSugarRepository<OrganizeEntity> departmentRepository, ISqlSugarRepository<PositionEntity> positionRepository,
            ISqlSugarRepository<UserEntity> userRepository, IOrganizeService organizeService, ISysConfigService sysConfigService,
            ISynThirdInfoService synThirdInfoService, IUserManager userManager)
        {
            _departmentRepository = departmentRepository;
            _positionRepository = positionRepository;
            _userRepository = userRepository;
            _organizeService = organizeService;
            _sysConfigService = sysConfigService;
            _synThirdInfoService = synThirdInfoService;
            _userManager = userManager;
        }

        #region GET

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="companyId">公司主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpGet("{companyId}/Department")]
        public async Task<dynamic> GetList(string companyId, [FromQuery] KeywordInput input)
        {
            var data = new List<DepartmentListOutput>();

            //全部部门数据
            var departmentAllList = await _departmentRepository.AsSugarClient().Queryable<OrganizeEntity, UserEntity>((a, b) => new JoinQueryInfos(JoinType.Left, b.Id == a.ManagerId))
                .Select((a, b) => new { Id = a.Id, ParentId = a.ParentId, FullName = a.FullName, EnCode = a.EnCode, Description = a.Description, EnabledMark = a.EnabledMark, CreatorTime = a.CreatorTime, Manager = SqlFunc.MergeString(b.RealName, "/", b.Account), SortCode = a.SortCode, Category = a.Category, DeleteMark = a.DeleteMark })
                .MergeTable().Where(t => t.ParentId == companyId && t.Category.Equals("department") && t.DeleteMark == null).OrderBy(a => a.SortCode)
                .OrderBy(a => a.CreatorTime, OrderByType.Desc).Select<DepartmentListOutput>().ToListAsync();

            //当前公司部门
            var departmentList = await _departmentRepository.AsQueryable()
                .WhereIF(!string.IsNullOrEmpty(input.keyword), d => d.FullName.Contains(input.keyword) || d.EnCode.Contains(input.keyword))
                .Where(t => t.ParentId == companyId && t.Category.Equals("department") && t.DeleteMark == null)
                .OrderBy(a => a.SortCode)
                .OrderBy(a => a.CreatorTime, OrderByType.Desc)
                .ToListAsync();
            departmentList.ForEach(item =>
            {
                item.ParentId = "0";
                data.AddRange(departmentAllList.TreeChildNode(item.Id, t => t.id, t => t.parentId));
            });
            return new { list = data.OrderBy(x => x.sortCode).ToList() };
        }

        /// <summary>
        /// 获取下拉框
        /// </summary>
        /// <returns></returns>
        [HttpGet("Department/Selector/{id}")]
        public async Task<dynamic> GetSelector(string id)
        {
            var data = await _departmentRepository.AsQueryable().Where(t => t.DeleteMark == null).OrderBy(o => o.SortCode).OrderBy(a => a.CreatorTime, OrderByType.Desc).ToListAsync();
            if (!"0".Equals(id))
            {
                var info = data.Find(it => it.Id == id);
                data.Remove(info);
            }
            var treeList = data.Adapt<List<DepartmentSelectorOutput>>();
            treeList.ForEach(item =>
            {
                if (item.type != null && item.type.Equals("company"))
                {
                    item.icon = "icon-sz icon-sz-tree-organization3";
                }
            });
            return new { list = treeList.OrderBy(x => x.sortCode).ToList().ToTree("-1") };
        }

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpGet("Department/{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var entity = await _departmentRepository.GetSingleAsync(d => d.Id == id);
            var output = entity.Adapt<DepartmentInfoOutput>();
            return output;
        }

        #endregion

        #region POST

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("Department")]
        public async Task Create([FromBody] DepartmentCrInput input)
        {
            var user = await _userManager.GetUserInfo();
            if (!user.dataScope.Any(it => it.organizeId == input.parentId && it.Add == true) && !user.isAdministrator)
            {
                throw HSZException.Oh(ErrorCode.D1013);
            }
            if (await _departmentRepository.IsAnyAsync(o => o.EnCode == input.enCode && o.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.D2014);
            if (await _departmentRepository.IsAnyAsync(o => o.ParentId == input.parentId && o.FullName == input.fullName && o.Category == "department" && o.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.D2019);
            var entity = input.Adapt<OrganizeEntity>();
            entity.Category = "department";
            entity.Id = YitIdHelper.NextId().ToString();

            #region 处理 上级ID列表 存储
            var idList = new List<string>();
            idList.Add(entity.Id);
            if (entity.ParentId != "-1")
            {
                var ids = _departmentRepository.AsSugarClient().Queryable<OrganizeEntity>().ToParentList(it => it.ParentId, entity.ParentId).Select(x => x.Id).ToList();
                idList.AddRange(ids);
            }
            idList.Reverse();
            entity.OrganizeIdTree = string.Join(",", idList);
            #endregion

            var newEntity = await _departmentRepository.AsInsertable(entity).CallEntityMethod(m => m.Create()).ExecuteReturnEntityAsync();
            _ = newEntity ?? throw HSZException.Oh(ErrorCode.D2015);

            #region 第三方同步
            try
            {
                var sysConfig = await _sysConfigService.GetInfo();
                var orgList = new List<OrganizeListOutput>();
                orgList.Add(entity.Adapt<OrganizeListOutput>());
                if (sysConfig.dingSynIsSynOrg == 1)
                {
                    await _synThirdInfoService.SynDep(2, 2, sysConfig, orgList);
                }
                if (sysConfig.qyhIsSynOrg == 1)
                {
                    await _synThirdInfoService.SynDep(1, 2, sysConfig, orgList);
                }
            }
            catch (Exception)
            {
            }
            #endregion
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("Department/{id}")]
        public async Task Delete(string id)
        {
            var user = new UserInfo();
            await Scoped.Create(async (_, scope) =>
            {
                var services = scope.ServiceProvider;

                var _userManager = App.GetService<IUserManager>(services);

                user = await _userManager.GetUserInfo();
            });
            if (!user.dataScope.Any(it => it.organizeId == id && it.Delete == true) && !user.isAdministrator)
            {
                throw HSZException.Oh(ErrorCode.D1013);
            }
            //该机构下有机构，则不能删
            if (await _departmentRepository.IsAnyAsync(o => o.ParentId.Equals(id) && o.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.D2005);
            //该机构下有岗位，则不能删
            if (await _positionRepository.IsAnyAsync(p => p.OrganizeId.Equals(id) && p.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.D2006);
            //该机构下有用户，则不能删
            if (await _userRepository.AsSugarClient().Queryable<UserRelationEntity>().Where(x => x.ObjectType == "Organize" && x.ObjectId == id).AnyAsync())
                throw HSZException.Oh(ErrorCode.D2004);
            //该机构下有角色，则不能删
            if (await _userRepository.AsSugarClient().Queryable<OrganizeRelationEntity>().Where(x => x.OrganizeId == id && x.ObjectType == "Role").AnyAsync())
                throw HSZException.Oh(ErrorCode.D2020);
            var entity = await _departmentRepository.GetSingleAsync(o => o.Id == id && o.DeleteMark == null);
            _ = entity ?? throw HSZException.Oh(ErrorCode.D2002);
            var isOK = await _departmentRepository.AsUpdateable(entity).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
            if (!(isOK > 0))
                throw HSZException.Oh(ErrorCode.D2017);
            else
            {
                //删除该组织和角色关联数据
                await _departmentRepository.AsSugarClient().Deleteable<OrganizeRelationEntity>().Where(x => x.OrganizeId == id && x.ObjectType == "Role").ExecuteCommandAsync();
            }

            #region 第三方数据删除
            try
            {
                var sysConfig = await _sysConfigService.GetInfo();
                if (sysConfig.dingSynIsSynOrg == 1)
                {
                    await _synThirdInfoService.DelSynData(2, 2, sysConfig, id);
                }
                if (sysConfig.qyhIsSynOrg == 1)
                {
                    await _synThirdInfoService.DelSynData(1, 2, sysConfig, id);
                }
            }
            catch (Exception)
            {
            }
            #endregion
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("Department/{id}")]
        public async Task Update(string id, [FromBody] DepartmentUpInput input)
        {
            var user = new UserInfo();
            await Scoped.Create(async (_, scope) =>
            {
                var services = scope.ServiceProvider;

                var _userManager = App.GetService<IUserManager>(services);

                user = await _userManager.GetUserInfo();
            });
            var oldEntity = await _departmentRepository.GetSingleAsync(it => it.Id == id);
            if (oldEntity.ParentId != input.parentId && !user.dataScope.Any(it => it.organizeId == oldEntity.ParentId && it.Edit == true) && !user.isAdministrator)
                throw HSZException.Oh(ErrorCode.D1013);

            if (oldEntity.ParentId != input.parentId && !user.dataScope.Any(it => (it.organizeId == input.parentId || it.organizeId == input.id) && it.Edit == true) && !user.isAdministrator)
                throw HSZException.Oh(ErrorCode.D1013);

            if (!user.dataScope.Any(it => it.organizeId == id && it.Edit == true) && !user.isAdministrator)
                throw HSZException.Oh(ErrorCode.D1013);
            if (input.parentId.Equals(id))
                throw HSZException.Oh(ErrorCode.D2001);
            //父id不能为自己的子节点
            var childIdListById = await _organizeService.GetChildIdListWithSelfById(id);
            if (childIdListById.Contains(input.parentId))
                throw HSZException.Oh(ErrorCode.D2001);
            if (await _departmentRepository.IsAnyAsync(o => o.EnCode == input.enCode && o.Id != id && o.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.D2014);
            if (await _departmentRepository.IsAnyAsync(o => o.ParentId == input.parentId && o.FullName == input.fullName && o.Id != id && o.Category == "department" && o.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.D2019);
            var entity = input.Adapt<OrganizeEntity>();

            #region 处理 上级ID列表 存储
            if (string.IsNullOrWhiteSpace(oldEntity.OrganizeIdTree) || entity.ParentId != oldEntity.ParentId)
            {
                var idList = new List<string>();
                idList.Add(entity.Id);
                if (entity.ParentId != "-1")
                {
                    var ids = _departmentRepository.AsSugarClient().Queryable<OrganizeEntity>().ToParentList(it => it.ParentId, entity.ParentId).Select(x => x.Id).ToList();
                    idList.AddRange(ids);
                }
                idList.Reverse();
                entity.OrganizeIdTree = string.Join(",", idList);

                //如果上级结构 变动 ，需要更改所有包含 该组织的id 的结构
                if (entity.OrganizeIdTree != oldEntity.OrganizeIdTree)
                {
                    var oldEntityList = await _departmentRepository.GetListAsync(x => x.OrganizeIdTree.Contains(oldEntity.Id) && x.Id != oldEntity.Id);
                    oldEntityList.ForEach(item =>
                    {
                        var childList = item.OrganizeIdTree.Split(oldEntity.Id).LastOrDefault();
                        item.OrganizeIdTree = entity.OrganizeIdTree + childList;
                    });

                    await _departmentRepository.AsUpdateable(oldEntityList).UpdateColumns(x => x.OrganizeIdTree).ExecuteCommandAsync();//批量修改 父级组织
                }
            }
            #endregion

            var isOK = await _departmentRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (!(isOK > 0))
                throw HSZException.Oh(ErrorCode.D2018);

            #region 第三方同步
            try
            {
                var sysConfig = await _sysConfigService.GetInfo();
                var orgList = new List<OrganizeListOutput>();
                var synEntity = _departmentRepository.GetFirst(x => x.Id == id);
                orgList.Add(synEntity.Adapt<OrganizeListOutput>());
                if (sysConfig.dingSynIsSynOrg == 1)
                {
                    await _synThirdInfoService.SynDep(2, 2, sysConfig, orgList);
                }
                if (sysConfig.qyhIsSynOrg == 1)
                {
                    await _synThirdInfoService.SynDep(1, 2, sysConfig, orgList);
                }
            }
            catch (Exception)
            {
            }
            #endregion
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpPut("Department/{id}/Actions/State")]
        public async Task UpdateState(string id)
        {
            var user = new UserInfo();
            await Scoped.Create(async (_, scope) =>
            {
                var services = scope.ServiceProvider;

                var _userManager = App.GetService<IUserManager>(services);

                user = await _userManager.GetUserInfo();
            });
            if (!user.dataScope.Any(it => it.organizeId == id && it.Edit == true) && !user.isAdministrator)
            {
                throw HSZException.Oh(ErrorCode.D1013);
            }
            var entity = await _departmentRepository.GetFirstAsync(o => o.Id == id);
            _ = entity.EnabledMark == 1 ? 0 : 1;
            var isOk = await _departmentRepository.AsUpdateable(entity).UpdateColumns(o => new { o.EnabledMark }).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (!(isOk > 0))
                throw HSZException.Oh(ErrorCode.D2016);
        }

        #endregion

        #region PublicMethod

        /// <summary>
        /// 获取部门列表(其他服务使用)
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<OrganizeEntity>> GetListAsync()
        {
            return await _departmentRepository.AsQueryable().Where(t => t.Category.Equals("department") && t.EnabledMark == 1 && t.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
        }

        /// <summary>
        /// 部门名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public string GetDepName(string id)
        {
            var entity = _departmentRepository.GetFirst(x => x.Id == id && x.Category == "department" && x.EnabledMark == 1 && x.DeleteMark == null);
            var name = entity == null ? "" : entity.FullName;
            return name;
        }

        /// <summary>
        /// 公司名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public string GetComName(string id)
        {
            var name = "";
            var entity = _departmentRepository.GetFirst(x => x.Id == id && x.EnabledMark == 1 && x.DeleteMark == null);
            if (entity == null)
            {
                return name;
            }
            else
            {
                if (entity.Category == "company")
                {
                    return entity.FullName;
                }
                else
                {
                    var pEntity = _departmentRepository.GetFirst(x => x.Id == entity.ParentId && x.EnabledMark == 1 && x.DeleteMark == null);
                    return GetComName(pEntity.Id);
                }
            }
        }

        /// <summary>
        /// 公司结构名称树
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public string GetOrganizeNameTree(string id)
        {
            var names = "";

            //组织结构
            var olist = _departmentRepository.AsSugarClient().Queryable<OrganizeEntity>().ToParentList(it => it.ParentId, id).Select(x => x.FullName).ToList();
            olist.Reverse();
            names = string.Join(" / ", olist);

            return names;
        }

        /// <summary>
        /// 公司id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public string GetCompanyId(string id)
        {
            var entity = _departmentRepository.GetFirst(x => x.Id == id && x.EnabledMark == 1 && x.DeleteMark == null);
            if (entity == null)
            {
                return "";
            }
            else
            {
                if (entity.Category == "company")
                {
                    return entity.Id;
                }
                else
                {
                    var pEntity = _departmentRepository.GetFirst(x => x.Id == entity.ParentId && x.EnabledMark == 1 && x.DeleteMark == null);
                    return GetCompanyId(pEntity.Id);
                }
            }
        }

        #endregion

    }
}
