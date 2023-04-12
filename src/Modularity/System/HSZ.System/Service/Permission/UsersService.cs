using HSZ.Common.Configuration;
using HSZ.Common.Const;
using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Common.Helper;
using HSZ.Common.Model.NPOI;
using HSZ.Common.Util;
using HSZ.DataEncryption;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.JsonSerialization;
using HSZ.System.Entitys.Dto.Permission.Organize;
using HSZ.System.Entitys.Dto.Permission.Role;
using HSZ.System.Entitys.Dto.Permission.User;
using HSZ.System.Entitys.Dto.Permission.UserRelation;
using HSZ.System.Entitys.Entity.System;
using HSZ.System.Entitys.Model.Permission.User;
using HSZ.System.Entitys.Permission;
using HSZ.System.Entitys.System;
using HSZ.System.Interfaces.Common;
using HSZ.System.Interfaces.Permission;
using HSZ.System.Interfaces.System;
using HSZ.VisualDev.Entitys;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.System.Service.Permission
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：业务实现：用户信息
    /// </summary>
    [ApiDescriptionSettings(Tag = "Permission", Name = "Users", Order = 163)]
    [Route("api/permission/[controller]")]
    public class UsersService : IUsersService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<UserRelationEntity> _userRelationRepository;
        private readonly ISqlSugarRepository<OrganizeEntity> _organizeRepository;
        private readonly ISqlSugarRepository<UserEntity> _userRepository;  // 用户表仓储
        private readonly ISqlSugarRepository<PositionEntity> _positionRepository;
        private readonly ISqlSugarRepository<RoleEntity> _roleRepository;
        private readonly IOrganizeService _organizeService; // 机构表仓储
        private readonly IUserRelationService _userRelationService; // 用户关系表服务
        private readonly ISysConfigService _sysConfigService; //系统配置仓储
        private readonly ISynThirdInfoService _synThirdInfoService;
        private readonly IUserManager _userManager;
        private readonly IFileService _fileService;
        private readonly SqlSugarScope Db;

        /// <summary>
        /// 初始化一个<see cref="UsersService"/>类型的新实例
        /// </summary>
        public UsersService(ISqlSugarRepository<UserEntity> userRepository,
            ISqlSugarRepository<OrganizeEntity> organizeRepository,
            ISqlSugarRepository<UserRelationEntity> userRelationRepository,
            ISqlSugarRepository<PositionEntity> positionRepository,
            ISqlSugarRepository<RoleEntity> roleRepository,
            IOrganizeService organizeService,
            IUserRelationService userRelationService,
            ISysConfigService sysConfigService,
            ISynThirdInfoService synThirdInfoService,
            IFileService fileService,
            IUserManager userManager)
        {
            _userRepository = userRepository;
            _userRelationRepository = userRelationRepository;
            _organizeRepository = organizeRepository;
            _positionRepository = positionRepository;
            _roleRepository = roleRepository;
            _organizeService = organizeService;
            _userRelationService = userRelationService;
            _sysConfigService = sysConfigService;
            _userManager = userManager;
            _synThirdInfoService = synThirdInfoService;
            _fileService = fileService;
            Db = DbScoped.SugarScope;
        }

        #region GET

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] UserListQuery input)
        {
            //当前请求用户ID
            var userId = _userManager.UserId;
            var tenantId = _userManager.TenantId;
            var pageInput = input.Adapt<PageInputBase>();
            var dbType = _organizeRepository.AsSugarClient().CurrentConnectionConfig.DbType;

            #region 处理组织树 名称
            var orgTreeNameList = new List<OrganizeEntity>();
            var orgList = await _organizeRepository.AsQueryable().Where(x => x.DeleteMark == null && x.EnabledMark == 1).OrderBy(x => x.SortCode).OrderBy(x => x.CreatorTime).ToListAsync();
            orgList.ForEach(item =>
            {
                if (item.OrganizeIdTree.IsNullOrEmpty()) item.OrganizeIdTree = item.Id;
                var newItem = new OrganizeEntity();
                newItem.Id = item.Id;
                newItem.FullName = string.Join("/", orgList.Where(x => item.OrganizeIdTree.Split(",").Contains(x.Id)).Select(x => x.FullName).ToList());
                orgTreeNameList.Add(newItem);
            });
            #endregion

            #region 获取组织层级
            var childOrgIds = new List<string>();
            if (input.organizeId.IsNotEmptyOrNull())
            {
                childOrgIds.Add(input.organizeId);
                //根据组织Id 获取所有子组织Id集合
                childOrgIds.AddRange(_organizeRepository.AsQueryable().Where(x => x.DeleteMark == null && x.EnabledMark == 1)
                    .ToChildList(x => x.ParentId, input.organizeId).Select(x => x.Id).ToList());
                childOrgIds = childOrgIds.Distinct().ToList();
            }
            #endregion

            //获取配置文件 账号锁定类型
            var config = await _userRepository.AsSugarClient().Queryable<SysConfigEntity>().Where(x => x.Key.Equals("lockType") && x.Category.Equals("SysConfig")).FirstAsync();
            var configLockType = config?.Value;

            var data = new SqlSugarPagedList<UserListOutput>();
            if (childOrgIds.Any())
            {
                //拼接查询
                var listQuery = new List<ISugarQueryable<UserListOutput>>();
                foreach (var item in childOrgIds)
                {
                    var quer = _userRepository.AsSugarClient().Queryable<UserRelationEntity, UserEntity>((a, b) => new JoinQueryInfos(JoinType.Left, b.Id == a.UserId))
                    .Where((a, b) => item == a.ObjectId)
                    .WhereIF(!pageInput.keyword.IsNullOrEmpty(), (a, b) => b.Account.Contains(pageInput.keyword) || b.RealName.Contains(pageInput.keyword))
                    .Where((a, b) => b.DeleteMark == null)
                    .Select((a, b) => new UserListOutput
                    {
                        id = b.Id,
                        account = b.Account,
                        realName = b.RealName,
                        creatorTime = b.CreatorTime,
                        gender = b.Gender,
                        mobilePhone = b.MobilePhone,
                        sortCode = b.SortCode,
                        unLockTime = b.UnLockTime,
                        enabledMark = b.EnabledMark
                    });
                    listQuery.Add(quer);
                }
                data = await _userRepository.AsSugarClient().UnionAll(listQuery)
                    .Select(a => new UserListOutput
                    {
                        id = a.id,
                        account = a.account,
                        realName = a.realName,
                        creatorTime = a.creatorTime,
                        gender = a.gender,
                        mobilePhone = a.mobilePhone,
                        sortCode = a.sortCode,
                        enabledMark = SqlFunc.IIF(!SqlFunc.IsNullOrEmpty(configLockType) && (configLockType == "2" && a.enabledMark == 2 && a.unLockTime < DateTime.Now), 1, a.enabledMark)
                    }).ToPagedListAsync(input.currentPage, input.pageSize);
            }
            else
            {
                data = await _userRepository.AsSugarClient().Queryable<UserEntity>()
                    .WhereIF(!pageInput.keyword.IsNullOrEmpty(), a => a.Account.Contains(pageInput.keyword) || a.RealName.Contains(pageInput.keyword))
                    .Where(a => a.DeleteMark == null)
                    .OrderBy(a => a.SortCode).OrderBy(a => a.CreatorTime, OrderByType.Desc).OrderBy(a => a.LastModifyTime, OrderByType.Desc)
                    .Select(a => new UserListOutput
                    {
                        id = a.Id,
                        account = a.Account,
                        realName = a.RealName,
                        creatorTime = a.CreatorTime,
                        gender = a.Gender,
                        mobilePhone = a.MobilePhone,
                        sortCode = a.SortCode,
                        enabledMark = SqlFunc.IIF(!SqlFunc.IsNullOrEmpty(configLockType) && (configLockType == "2" && a.EnabledMark == 2 && a.UnLockTime < DateTime.Now), 1, a.EnabledMark)
                    }).ToPagedListAsync(input.currentPage, input.pageSize); ;
            }

            #region 处理 用户 多组织
            var orgUserIdAll = await _organizeRepository.AsSugarClient().Queryable<UserRelationEntity>()
                .Where(x => data.list.Select(u => u.id).Contains(x.UserId)).ToListAsync();
            foreach (var item in data.list)
            {
                var roleOrgList = orgUserIdAll.Where(x => x.UserId == item.id).Select(x => x.ObjectId).ToList();//获取用户组织集合
                item.organize = string.Join(" ; ", orgTreeNameList.Where(x => roleOrgList.Contains(x.Id)).Select(x => x.FullName));
            }
            #endregion

            return PageResult<UserListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 获取全部用户
        /// </summary>
        /// <returns></returns>
        [HttpGet("All")]
        public async Task<dynamic> GetUserAllList()
        {
            var list = await _userRepository.AsSugarClient().Queryable<UserEntity, OrganizeEntity>((a, b) => new JoinQueryInfos(JoinType.Left, b.Id == SqlFunc.ToString(a.OrganizeId)))
                .Select((a, b) => new UserListAllOutput
                {
                    id = a.Id,
                    account = a.Account,
                    realName = a.RealName,
                    headIcon = SqlFunc.MergeString("/api/File/Image/userAvatar/", a.HeadIcon),
                    gender = a.Gender,
                    department = b.FullName,
                    sortCode = a.SortCode,
                    quickQuery = a.QuickQuery,
                    enabledMark = a.EnabledMark,
                    deleteMark = a.DeleteMark
                })
                .MergeTable().Where(p => p.enabledMark == 1 && p.deleteMark == null).OrderBy(p => p.sortCode).ToListAsync();
            return list;
        }

        /// <summary>
        /// 获取用户数据分页 根据角色Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("getUsersByRoleId")]
        public async Task<dynamic> GetUsersByRoleId([FromQuery] RoleListInput input)
        {
            var roleInfo = await _roleRepository.AsQueryable().Where(x => x.Id == input.roleId).FirstAsync();

            if (roleInfo.GlobalMark == 1)//查询全部用户 (全局角色)
            {
                var list = await _userRepository.AsSugarClient().Queryable<UserEntity>()
                    .WhereIF(!input.keyword.IsNullOrEmpty(), a => a.Account.Contains(input.keyword) || a.RealName.Contains(input.keyword))
                    .Select(a => new UserListAllOutput
                    {
                        id = a.Id,
                        account = a.Account,
                        realName = a.RealName,
                        gender = a.Gender,
                        sortCode = a.SortCode,
                        quickQuery = a.QuickQuery,
                        enabledMark = a.EnabledMark,
                        deleteMark = a.DeleteMark
                    })
                    .MergeTable().Where(p => p.enabledMark == 1 && p.deleteMark == null).OrderBy(p => p.sortCode).ToPagedListAsync(input.currentPage, input.pageSize);

                return PageResult<UserListAllOutput>.SqlSugarPageResult(list);
            }
            else//查询角色 所属 所有组织 用户
            {
                //查询角色 所有所属组织
                var orgList = await _userRepository.AsSugarClient().Queryable<OrganizeRelationEntity>().Where(x => x.ObjectType == "Role" && x.ObjectId == roleInfo.Id).Select(x => x.OrganizeId).ToListAsync();

                var userIdList = await _userRepository.AsSugarClient().Queryable<UserRelationEntity>().Where(x => x.ObjectType == "Organize" && orgList.Contains(x.ObjectId)).Select(x => x.UserId).Distinct().ToListAsync();

                var list = await _userRepository.AsSugarClient().Queryable<UserEntity>()
                    .Where(a => userIdList.Contains(a.Id))
                    .WhereIF(!input.keyword.IsNullOrEmpty(), a => a.Account.Contains(input.keyword) || a.RealName.Contains(input.keyword))
                    .Select(a => new UserListAllOutput
                    {
                        id = a.Id,
                        account = a.Account,
                        realName = a.RealName,
                        gender = a.Gender,
                        sortCode = a.SortCode,
                        quickQuery = a.QuickQuery,
                        enabledMark = a.EnabledMark,
                        deleteMark = a.DeleteMark
                    })
                    .MergeTable().Where(p => p.enabledMark == 1 && p.deleteMark == null).OrderBy(p => p.sortCode).ToPagedListAsync(input.currentPage, input.pageSize);

                return PageResult<UserListAllOutput>.SqlSugarPageResult(list);
            }
        }

        /// <summary>
        /// 获取IM用户列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("ImUser")]
        public async Task<dynamic> GetImUserList([FromQuery] PageInputBase input)
        {
            var list = await _userRepository.AsSugarClient().Queryable<UserEntity, OrganizeEntity>((a, b) => new JoinQueryInfos(JoinType.Left, b.Id == SqlFunc.ToString(a.OrganizeId)))
                .Select((a, b) => new IMUserListOutput { id = a.Id, account = a.Account, realName = a.RealName, headIcon = SqlFunc.MergeString("/api/File/Image/userAvatar/", a.HeadIcon), department = b.FullName, sortCode = a.SortCode, enabledMark = a.EnabledMark, deleteMark = a.DeleteMark })
                .MergeTable().WhereIF(!input.keyword.IsNullOrEmpty(), u => u.account.Contains(input.keyword) || u.realName.Contains(input.keyword)).Where(p => p.id != _userManager.UserId && p.enabledMark == 1 && p.deleteMark == null).OrderBy(p => p.sortCode).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<IMUserListOutput>.SqlSugarPageResult(list);
        }

        /// <summary>
        /// 获取下拉框（公司+部门+用户）
        /// </summary>
        /// <returns></returns>
        [HttpGet("Selector")]
        public async Task<dynamic> GetSelector()
        {
            var organizeList = await _organizeService.GetListAsync();
            var userList = await _userRepository.AsQueryable().Where(t => t.EnabledMark == 1 && t.DeleteMark == null).OrderBy(u => u.SortCode).OrderBy(a => a.CreatorTime, OrderByType.Desc).ToListAsync();
            var organizeTreeList = organizeList.Adapt<List<UserSelectorOutput>>();
            var treeList = userList.Adapt<List<UserSelectorOutput>>();
            treeList = treeList.Concat(organizeTreeList).ToList();
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
            var entity = await _userRepository.GetFirstAsync(u => u.Id == id);
            var config = await _userRepository.AsSugarClient().Queryable<SysConfigEntity>().Where(x => x.Key.Equals("lockType") && x.Category.Equals("SysConfig")).FirstAsync();
            var configLockType = config?.Value;
            entity.EnabledMark = configLockType.IsNotEmptyOrNull() && configLockType == "2" && entity.EnabledMark == 2 && entity.UnLockTime < DateTime.Now ? 1 : entity.EnabledMark;
            var output = entity.Adapt<UserInfoOutput>();
            if (output.headIcon == "/api/File/Image/userAvatar/") output.headIcon = "";
            if (entity != null)
            {
                var allRelationList = await _userRelationService.GetListByUserId(id);
                var relationIds = allRelationList.Where(x => x.ObjectType == "Organize" || x.ObjectType == "Position").Select(x => new { x.ObjectId, x.ObjectType }).ToList();
                var oList = await _organizeRepository.AsQueryable().Where(x => relationIds.Where(x => x.ObjectType == "Organize").Select(x => x.ObjectId).Contains(x.Id)).ToListAsync();
                output.organizeIdTree = new List<List<string>>();
                oList.ForEach(item =>
                {
                    if (item.OrganizeIdTree.IsNotEmptyOrNull()) output.organizeIdTree.Add(item.OrganizeIdTree.Split(",").ToList());
                });
                output.organizeId = string.Join(",", relationIds.Where(x => x.ObjectType == "Organize").Select(x => x.ObjectId));
                output.positionId = string.Join(",", relationIds.Where(x => x.ObjectType == "Position").Select(x => x.ObjectId));
            }
            return output;
        }

        #endregion

        #region POST

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetUserList")]
        public async Task<dynamic> GetUserList([FromBody] UserRelationInput input)
        {
            var data = await _userRepository.AsSugarClient()
                .Queryable<UserEntity>().Where(it => it.EnabledMark == 1 && it.DeleteMark == null)
                .In(it => it.Id, input.userId.ToArray())
                .Select(it => new { id = it.Id, fullName = SqlFunc.MergeString(it.RealName, "/", it.Account), enabledMark = it.EnabledMark, deleteMark = it.DeleteMark, sortCode = it.SortCode })
                .OrderBy(it => it.sortCode).ToListAsync();
            return new { list = data };
        }

        /// <summary>
        /// 获取机构成员列表
        /// </summary>
        /// <param name="organizeId">机构ID</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("ImUser/Selector/{organizeId}")]
        public async Task<dynamic> GetOrganizeMemberList(string organizeId, [FromBody] KeywordInput input)
        {
            var output = new List<OrganizeMemberListOutput>();
            if (!input.keyword.IsNullOrEmpty())
            {
                output = await _userRepository.AsQueryable()
                    .WhereIF(!input.keyword.IsNullOrEmpty(), u => u.Account.Contains(input.keyword) || u.RealName.Contains(input.keyword))
                    .Where(u => u.EnabledMark == 1 && u.DeleteMark == null).OrderBy(o => o.SortCode)
                    .Select(u => new OrganizeMemberListOutput
                    {
                        id = u.Id,
                        Account = u.Account,
                        RealName = u.RealName,
                        fullName = SqlFunc.MergeString(u.RealName, "/", u.Account),
                        enabledMark = u.EnabledMark,
                        icon = "icon-sz icon-sz-tree-user2",
                        isLeaf = true,
                        hasChildren = false,
                        type = "user",
                        DeleteMark = u.DeleteMark,
                        SortCode = u.SortCode
                    }).Take(50).ToListAsync();
            }
            else
            {
                output = await _organizeService.GetOrganizeMemberList(organizeId);
            }
            return new { list = output };
        }

        ///<summary>
        ///获取当前用户所属机构下属成员
        ///</summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpGet("getOrganization")]
        public async Task<dynamic> GetOrganizeMember([FromQuery] UserListQuery input)
        {
            var user = await _userManager.GetUserInfo();
            if (input.organizeId.IsNotEmptyOrNull() && input.organizeId != "0") input.organizeId = input.organizeId.Split(",").LastOrDefault();
            else input.organizeId = user.organizeId;

            //获取岗位所属组织的所有成员
            var userList = await _userRelationRepository.AsQueryable().Where(x => x.ObjectType == "Organize" && x.ObjectId == input.organizeId).Select(x => x.UserId).Distinct().ToListAsync();

            return await _userRepository.AsQueryable()
                    .WhereIF(!input.keyword.IsNullOrEmpty(), u => u.Account.Contains(input.keyword) || u.RealName.Contains(input.keyword))
                    .Where(u => u.EnabledMark == 1 && u.DeleteMark == null && userList.Contains(u.Id)).OrderBy(o => o.SortCode)
                    .Select(u => new OrganizeMemberListOutput
                    {
                        id = u.Id,
                        Account = u.Account,
                        RealName = u.RealName,
                        fullName = SqlFunc.MergeString(u.RealName, "/", u.Account),
                        enabledMark = u.EnabledMark,
                        icon = "icon-sz icon-sz-tree-user2",
                        isLeaf = true,
                        hasChildren = false,
                        type = "user",
                        DeleteMark = u.DeleteMark,
                        SortCode = u.SortCode
                    }).ToListAsync();
        }


        ///<summary>
        ///获取当前用户下属成员
        ///</summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("getSubordinates")]
        public async Task<dynamic> GetSubordinate([FromBody] KeywordInput input)
        {
            var userId = _userManager.UserId;

            return await _userRepository.AsQueryable()
                    .WhereIF(!input.keyword.IsNullOrEmpty(), u => u.Account.Contains(input.keyword) || u.RealName.Contains(input.keyword))
                    .Where(u => u.EnabledMark == 1 && u.DeleteMark == null && u.ManagerId == userId).OrderBy(o => o.SortCode)
                    .Select(u => new OrganizeMemberListOutput
                    {
                        id = u.Id,
                        Account = u.Account,
                        RealName = u.RealName,
                        fullName = SqlFunc.MergeString(u.RealName, "/", u.Account),
                        enabledMark = u.EnabledMark,
                        icon = "icon-sz icon-sz-tree-user2",
                        isLeaf = true,
                        hasChildren = false,
                        type = "user",
                        DeleteMark = u.DeleteMark,
                        SortCode = u.SortCode
                    }).ToListAsync();
        }


        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] UserCrInput input)
        {
            var user = await _userManager.GetUserInfo();
            if (!user.dataScope.Any(it => input.organizeId.Contains(it.organizeId) && it.Add == true) && !user.isAdministrator)
            {
                throw HSZException.Oh(ErrorCode.D1013);
            }

            var isExist = await _userRepository.IsAnyAsync(u => u.Account == input.account && u.DeleteMark == null);
            if (isExist) throw HSZException.Oh(ErrorCode.D1003);
            var entity = input.Adapt<UserEntity>();

            #region 用户表单

            entity.IsAdministrator = 0;
            entity.EntryDate = input.entryDate.IsNullOrEmpty() ? DateTime.Now : Ext.GetDateTime(input.entryDate.ToString());
            entity.Birthday = input.birthday.IsNullOrEmpty() ? DateTime.Now : Ext.GetDateTime(input.birthday.ToString());
            entity.QuickQuery = PinyinUtil.PinyinString(input.realName);
            entity.Secretkey = Guid.NewGuid().ToString();
            entity.Password = MD5Encryption.Encrypt(MD5Encryption.Encrypt(CommonConst.DEFAULT_PASSWORD) + entity.Secretkey);
            var headIcon = input.headIcon.Split('/').ToList().Last();
            if (string.IsNullOrEmpty(headIcon))
                headIcon = "001.png";
            entity.HeadIcon = headIcon;

            #region 多组织 优先选择有权限组织
            var orgList = entity.OrganizeId.Split(",");//多 组织
            entity.OrganizeId = "";

            foreach (var item in orgList)
            {
                var roleList = await _userManager.GetUserOrgRoleIds(entity.RoleId, item);

                //如果该组织下有角色并且有角色权限 则为默认组织
                if (roleList.Any() && _userRepository.AsSugarClient().Queryable<AuthorizeEntity>().Where(x => x.ObjectType == "Role" && x.ItemType == "module" && roleList.Contains(x.ObjectId)).Any())
                {
                    entity.OrganizeId = item;//多 组织 默认
                    break;
                }
            }
            if (entity.OrganizeId.IsNullOrEmpty())//如果所选组织下都没有角色或者没有角色权限 默认取第一个
                entity.OrganizeId = input.organizeId.Split(",").FirstOrDefault();

            #endregion

            var PositionIds = entity.PositionId?.Split(",");
            var pIdList = await _positionRepository.AsQueryable().Where(x => x.OrganizeId == entity.OrganizeId && PositionIds.Contains(x.Id)).Select(x => x.Id).ToListAsync();
            entity.PositionId = pIdList.FirstOrDefault();//多 岗位 默认取当前组织第一个
            #endregion

            try
            {
                //开启事务
                Db.BeginTran();

                //新增用户记录
                var newEntity = await _userRepository.AsInsertable(entity).CallEntityMethod(m => m.Creator()).ExecuteReturnEntityAsync();

                //将临时文件迁移至正式文件
                FileHelper.MoveFile(FileVariable.TemporaryFilePath + headIcon, FileVariable.UserAvatarFilePath + headIcon);

                var userRelationList = new List<UserRelationEntity>();
                var roleList = _userRelationService.CreateByRole(newEntity.Id, newEntity.RoleId);
                var positionList = _userRelationService.CreateByPosition(newEntity.Id, input.positionId);
                var organizeList = _userRelationService.CreateByOrganize(newEntity.Id, input.organizeId);
                var groupList = _userRelationService.CreateByGroup(newEntity.Id, input.groupId);
                userRelationList.AddRange(positionList);
                userRelationList.AddRange(roleList);
                userRelationList.AddRange(organizeList);
                userRelationList.AddRange(groupList);

                if (userRelationList.Count > 0)
                {
                    //批量新增用户关系
                    await _userRelationService.Create(userRelationList);
                }

                Db.CommitTran();
            }
            catch (Exception)
            {
                Db.RollbackTran();
                throw HSZException.Oh(ErrorCode.D5001);
            }

            #region 第三方同步
            try
            {
                var sysConfig = await _sysConfigService.GetInfo();
                var userList = new List<UserEntity>();
                userList.Add(entity);
                if (sysConfig.dingSynIsSynUser == 1)
                {
                    await _synThirdInfoService.SynUser(2, 3, sysConfig, userList);
                }
                if (sysConfig.qyhIsSynUser == 1)
                {
                    await _synThirdInfoService.SynUser(1, 3, sysConfig, userList);
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
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var user = await _userManager.GetUserInfo();
            var entity = await _userRepository.GetFirstAsync(u => u.Id == id && u.DeleteMark == null);
            if (!user.dataScope.Any(it => it.organizeId == entity.OrganizeId && it.Delete == true) && !user.isAdministrator)
            {
                throw HSZException.Oh(ErrorCode.D1013);
            }
            var depManagerId = await _organizeService.GetIsManagerByUserId(id);
            if (depManagerId)
                throw HSZException.Oh(ErrorCode.D2003);
            _ = entity ?? throw HSZException.Oh(ErrorCode.D5002);
            if (entity.IsAdministrator == (int)AccountType.Administrator)
                throw HSZException.Oh(ErrorCode.D1014);
            if (entity.Id == user.userId)
                throw HSZException.Oh(ErrorCode.D1001);
            var userId = user.userId;
            try
            {
                //开启事务
                Db.BeginTran();

                //用户软删除
                await _userRepository.AsUpdateable(entity).IgnoreColumns(it => new { it.CreatorTime, it.CreatorUserId, it.LastModifyTime, it.LastModifyUserId }).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();

                //直接删除用户关系表相关相关数据
                await _userRelationService.Delete(id);

                Db.CommitTran();
            }
            catch (Exception)
            {
                Db.RollbackTran();
                throw;
            }
            #region 第三方同步
            try
            {
                var sysConfig = await _sysConfigService.GetInfo();
                if (sysConfig.dingSynIsSynUser == 1)
                {
                    await _synThirdInfoService.DelSynData(2, 3, sysConfig, id);
                }
                if (sysConfig.qyhIsSynUser == 1)
                {
                    await _synThirdInfoService.DelSynData(1, 3, sysConfig, id);
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
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] UserUpInput input)
        {
            var user = await _userManager.GetUserInfo();
            var oldUserEntity = await _userRepository.GetSingleAsync(it => it.Id == id);

            if (user.userId != oldUserEntity.Id && oldUserEntity.IsAdministrator == 1 && user.userAccount != "admin")//超级管理员 只有 admin 账号才有变更权限
                throw HSZException.Oh(ErrorCode.D1013);

            //旧数据
            var orgIdList = await _organizeRepository.AsSugarClient().Queryable<UserRelationEntity>().Where(x => x.UserId == id && x.ObjectType == "Organize").Select(x => x.ObjectId).ToListAsync();
            if (!user.dataScope.Any(it => orgIdList.Contains(it.organizeId) && it.Edit == true) && !user.isAdministrator)
            {
                throw HSZException.Oh(ErrorCode.D1013);
            }
            //新数据
            if (!user.dataScope.Any(it => input.organizeId.Contains(it.organizeId) && it.Edit == true) && !user.isAdministrator)
            {
                throw HSZException.Oh(ErrorCode.D1013);
            }

            // 排除自己并且判断与其他是否相同
            var isExist = await _userRepository.IsAnyAsync(u => u.Account == input.account && u.DeleteMark == null && u.Id != id);
            if (isExist) throw HSZException.Oh(ErrorCode.D1003);
            if (id == input.managerId) throw HSZException.Oh(ErrorCode.D1021);
            //直属主管的上级不能为自己的下属
            if (await GetIsMyStaff(id, input.managerId, 10)) throw HSZException.Oh(ErrorCode.D1026);
            var entity = input.Adapt<UserEntity>();
            entity.QuickQuery = PinyinUtil.PinyinString(input.realName);
            var headIcon = input.headIcon.Split('/').ToList().Last();
            entity.HeadIcon = headIcon;
            entity.LastModifyTime = DateTime.Now;
            entity.LastModifyUserId = user.userId;
            if (entity.EnabledMark == 2) entity.UnLockTime = null;

            #region 多组织 优先选择有权限组织
            var orgList = entity.OrganizeId.Split(",");//多 组织
            entity.OrganizeId = "";

            if (orgList.Contains(oldUserEntity.OrganizeId))
            {
                var roleList = await _userManager.GetUserOrgRoleIds(entity.RoleId, oldUserEntity.OrganizeId);

                //如果该组织下有角色并且有角色权限 则为默认组织
                if (roleList.Any() && _userRepository.AsSugarClient().Queryable<AuthorizeEntity>().Where(x => x.ObjectType == "Role" && x.ItemType == "module" && roleList.Contains(x.ObjectId)).Any())
                    entity.OrganizeId = oldUserEntity.OrganizeId;//多 组织 默认
            }

            if (entity.OrganizeId.IsNullOrEmpty())
            {
                foreach (var item in orgList)
                {
                    var roleList = await _userManager.GetUserOrgRoleIds(entity.RoleId, item);

                    //如果该组织下有角色并且有角色权限 则为默认组织
                    if (roleList.Any() && _userRepository.AsSugarClient().Queryable<AuthorizeEntity>().Where(x => x.ObjectType == "Role" && x.ItemType == "module" && roleList.Contains(x.ObjectId)).Any())
                    {
                        entity.OrganizeId = item;//多 组织 默认
                        break;
                    }
                }
            }
            if (entity.OrganizeId.IsNullOrEmpty())//如果所选组织下都没有角色或者没有角色权限 默认取第一个
                entity.OrganizeId = input.organizeId.Split(",").FirstOrDefault();
            #endregion

            //获取默认组织下的岗位
            var PositionIds = entity.PositionId?.Split(",");
            var pIdList = await _positionRepository.AsQueryable().Where(x => x.OrganizeId == entity.OrganizeId && PositionIds.Contains(x.Id)).Select(x => x.Id).ToListAsync();

            if (entity.PositionId.IsNotEmptyOrNull() && pIdList.Contains(oldUserEntity.PositionId))
                entity.PositionId = oldUserEntity.PositionId;
            else entity.PositionId = pIdList.FirstOrDefault();//多 岗位 默认取第一个

            try
            {
                //开启事务
                Db.BeginTran();

                //更新用户记录
                var newEntity = await _userRepository.AsUpdateable(entity).UpdateColumns(it => new
                {
                    it.Account,
                    it.RealName,
                    it.QuickQuery,
                    it.Gender,
                    it.Email,
                    it.OrganizeId,
                    it.ManagerId,
                    it.PositionId,
                    it.RoleId,
                    it.SortCode,
                    it.EnabledMark,
                    it.Description,
                    it.HeadIcon,
                    it.Nation,
                    it.NativePlace,
                    it.EntryDate,
                    it.CertificatesType,
                    it.CertificatesNumber,
                    it.Education,
                    it.UrgentContacts,
                    it.UrgentTelePhone,
                    it.PostalAddress,
                    it.MobilePhone,
                    it.Birthday,
                    it.TelePhone,
                    it.Landline,
                    it.UnLockTime,
                    it.LastModifyTime,
                    it.GroupId,
                    it.LastModifyUserId
                }).ExecuteCommandAsync();

                //将临时文件迁移至正式文件
                FileHelper.MoveFile(FileVariable.TemporaryFilePath + headIcon, FileVariable.UserAvatarFilePath + headIcon);

                //直接删除用户关系表相关相关数据
                await _userRelationService.Delete(id);

                var userRelationList = new List<UserRelationEntity>();
                var roleList = _userRelationService.CreateByRole(id, entity.RoleId);
                var positionList = _userRelationService.CreateByPosition(id, input.positionId);
                var organizeList = _userRelationService.CreateByOrganize(id, input.organizeId);
                var groupList = _userRelationService.CreateByGroup(id, input.groupId);
                userRelationList.AddRange(positionList);
                userRelationList.AddRange(roleList);
                userRelationList.AddRange(organizeList);
                userRelationList.AddRange(groupList);
                if (userRelationList.Count > 0)
                {
                    //批量新增用户关系
                    await _userRelationService.Create(userRelationList);
                }

                Db.CommitTran();
            }
            catch (Exception)
            {
                FileHelper.MoveFile(FileVariable.UserAvatarFilePath + headIcon, FileVariable.TemporaryFilePath + headIcon);
                Db.RollbackTran();
                throw HSZException.Oh(ErrorCode.D5004);
            }

            #region 第三方同步
            try
            {
                var sysConfig = await _sysConfigService.GetInfo();
                var userList = new List<UserEntity>();
                userList.Add(entity);
                if (sysConfig.dingSynIsSynUser == 1)
                {
                    await _synThirdInfoService.SynUser(2, 3, sysConfig, userList);
                }
                if (sysConfig.qyhIsSynUser == 1)
                {
                    await _synThirdInfoService.SynUser(1, 3, sysConfig, userList);
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
        [HttpPut("{id}/Actions/State")]
        public async Task UpdateState(string id)
        {
            var user = new UserInfo();
            await Scoped.Create(async (_, scope) =>
            {
                var services = scope.ServiceProvider;

                var _userManager = App.GetService<IUserManager>(services);

                user = await _userManager.GetUserInfo();
            });
            var entity = await _userRepository.GetSingleAsync(it => it.Id == id);
            if (!user.dataScope.Any(it => it.organizeId == entity.OrganizeId && it.Edit == true) && !user.isAdministrator)
            {
                throw HSZException.Oh(ErrorCode.D1013);
            }

            if (!await _userRepository.IsAnyAsync(u => u.Id == id && u.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.D1002);

            var isOk = await _userRepository.AsSugarClient().Updateable<UserEntity>().SetColumns(it => new UserEntity()
            {
                EnabledMark = SqlFunc.IIF(it.EnabledMark == 1, 0, 1),
                LastModifyUserId = user.userId,
                LastModifyTime = SqlFunc.GetDate()
            }).Where(it => it.Id == id).ExecuteCommandAsync();

            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.D5005);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("{id}/Actions/ResetPassword")]
        public async Task ResetPassword(string id, [FromBody] UserResetPasswordInput input)
        {
            var user = new UserInfo();
            await Scoped.Create(async (_, scope) =>
            {
                var services = scope.ServiceProvider;

                var _userManager = App.GetService<IUserManager>(services);

                user = await _userManager.GetUserInfo();
            });
            var entity = await _userRepository.GetFirstAsync(u => u.Id == id && u.DeleteMark == null);
            if (!user.dataScope.Any(it => it.organizeId == entity.OrganizeId && it.Edit == true) && !user.isAdministrator)
            {
                throw HSZException.Oh(ErrorCode.D1013);
            }

            if (!input.userPassword.Equals(input.validatePassword))
                throw HSZException.Oh(ErrorCode.D5006);

            _ = entity ?? throw HSZException.Oh(ErrorCode.D1002);

            var password = MD5Encryption.Encrypt(input.userPassword + entity.Secretkey);

            var isOk = await _userRepository.AsSugarClient().Updateable<UserEntity>().SetColumns(it => new UserEntity()
            {
                Password = password,
                ChangePasswordDate = SqlFunc.GetDate(),
                LastModifyUserId = user.userId,
                LastModifyTime = SqlFunc.GetDate()
            }).Where(it => it.Id == id).ExecuteCommandAsync();

            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.D5005);

            //强制将用户提掉线
        }

        /// <summary>
        /// 解除锁定
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/Unlock")]
        public async Task Unlock(string id)
        {
            var user = new UserInfo();
            await Scoped.Create(async (_, scope) =>
            {
                var services = scope.ServiceProvider;

                var _userManager = App.GetService<IUserManager>(services);

                user = await _userManager.GetUserInfo();
            });
            var entity = await _userRepository.GetFirstAsync(u => u.Id == id && u.DeleteMark == null);
            if (!user.dataScope.Any(it => it.organizeId == entity.OrganizeId && it.Edit == true) && !user.isAdministrator)
            {
                throw HSZException.Oh(ErrorCode.D1013);
            }

            var isOk = await _userRepository.AsSugarClient().Updateable<UserEntity>().SetColumns(it => new UserEntity()
            {
                LockMark = 0,//解锁
                LogErrorCount = 0,//解锁
                EnabledMark = 1,//解锁
                UnLockTime = DateTime.Now,//取消解锁时间
                LastModifyUserId = user.userId,
                LastModifyTime = SqlFunc.GetDate()
            }).Where(it => it.Id == id).ExecuteCommandAsync();

            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.D5005);

            //强制将用户提掉线
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("ExportExcel")]
        public async Task<dynamic> ExportExcel([FromQuery] UserExportDataInput input)
        {
            //用户信息列表
            var userList = new List<UserListImportDataInput>();

            if (input.dataType == "0")
            {
                userList = await _userRepository.AsSugarClient().Queryable<UserEntity, OrganizeEntity>((a, b) => new JoinQueryInfos(JoinType.Left, b.Id == SqlFunc.ToString(a.OrganizeId)))
                //组织机构
                .WhereIF(!string.IsNullOrWhiteSpace(input.organizeId), a => a.OrganizeId == input.organizeId)
                .WhereIF(!input.keyword.IsNullOrEmpty(), a => a.Account.Contains(input.keyword) || a.RealName.Contains(input.keyword))
                .Where(a => a.DeleteMark == null)
                .OrderBy(a => a.SortCode).OrderBy(a => a.CreatorTime, OrderByType.Desc)
                .Select(a => new UserListImportDataInput()
                {
                    id = a.Id,
                    account = a.Account,
                    realName = a.RealName,
                    birthday = a.Birthday,
                    certificatesNumber = a.CertificatesNumber,
                    managerId = SqlFunc.Subqueryable<UserEntity>().Where(e => e.Id == a.ManagerId).Select(u => SqlFunc.MergeString(u.RealName, "/", u.Account)),
                    organizeId = a.OrganizeId,//组织结构
                    positionId = a.PositionId,//岗位
                    roleId = a.RoleId,//多角色
                    certificatesType = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(d => d.DictionaryTypeId == "7866376d5f694d4d851c7164bd00ebfc" && d.Id == a.CertificatesType).Select(b => b.FullName),
                    education = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(d => d.DictionaryTypeId == "6a6d6fb541b742fbae7e8888528baa16" && d.Id == a.Education).Select(b => b.FullName),
                    gender = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(d => d.DictionaryTypeId == "963255a34ea64a2584c5d1ba269c1fe6" && d.EnCode == SqlFunc.ToString(a.Gender)).Select(b => b.FullName),
                    nation = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(d => d.DictionaryTypeId == "b6cd65a763fa45eb9fe98e5057693e40" && d.Id == a.Nation).Select(b => b.FullName),
                    description = a.Description,
                    entryDate = a.EntryDate,
                    email = a.Email,
                    enabledMark = SqlFunc.IF(a.EnabledMark.Equals(0)).Return("禁用").ElseIF(a.EnabledMark.Equals(1)).Return("正常").End("锁定"),
                    mobilePhone = a.MobilePhone,
                    nativePlace = a.NativePlace,
                    postalAddress = a.PostalAddress,
                    telePhone = a.TelePhone,
                    urgentContacts = a.UrgentContacts,
                    urgentTelePhone = a.UrgentTelePhone,
                    landline = a.Landline,
                    sortCode = a.SortCode.ToString()
                }).ToPageListAsync(input.currentPage, input.pageSize);
            }
            else
            {
                userList = await _userRepository.AsSugarClient().Queryable<UserEntity, OrganizeEntity>((a, b) => new JoinQueryInfos(JoinType.Left, b.Id == SqlFunc.ToString(a.OrganizeId)))
                //组织机构
                .WhereIF(!string.IsNullOrWhiteSpace(input.organizeId), a => a.OrganizeId == input.organizeId)
                .WhereIF(!input.keyword.IsNullOrEmpty(), a => a.Account.Contains(input.keyword) || a.RealName.Contains(input.keyword))
                .Where(a => a.DeleteMark == null)
                .OrderBy(a => a.SortCode).OrderBy(a => a.CreatorTime, OrderByType.Desc)
                .Select(a => new UserListImportDataInput()
                {
                    id = a.Id,
                    account = a.Account,
                    realName = a.RealName,
                    birthday = a.Birthday,
                    certificatesNumber = a.CertificatesNumber,
                    managerId = SqlFunc.Subqueryable<UserEntity>().Where(e => e.Id == a.ManagerId).Select(u => SqlFunc.MergeString(u.RealName, "/", u.Account)),
                    organizeId = a.OrganizeId,//组织结构
                    positionId = a.PositionId,//岗位
                    roleId = a.RoleId,//多角色
                    certificatesType = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(d => d.DictionaryTypeId == "7866376d5f694d4d851c7164bd00ebfc" && d.Id == a.CertificatesType).Select(b => b.FullName),
                    education = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(d => d.DictionaryTypeId == "6a6d6fb541b742fbae7e8888528baa16" && d.Id == a.Education).Select(b => b.FullName),
                    gender = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(d => d.DictionaryTypeId == "963255a34ea64a2584c5d1ba269c1fe6" && d.EnCode == SqlFunc.ToString(a.Gender)).Select(b => b.FullName),
                    nation = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(d => d.DictionaryTypeId == "b6cd65a763fa45eb9fe98e5057693e40" && d.Id == a.Nation).Select(b => b.FullName),
                    description = a.Description,
                    entryDate = a.EntryDate,
                    email = a.Email,
                    enabledMark = SqlFunc.IF(a.EnabledMark.Equals(0)).Return("禁用").ElseIF(a.EnabledMark.Equals(1)).Return("正常").End("锁定"),
                    mobilePhone = a.MobilePhone,
                    nativePlace = a.NativePlace,
                    postalAddress = a.PostalAddress,
                    telePhone = a.TelePhone,
                    urgentContacts = a.UrgentContacts,
                    urgentTelePhone = a.UrgentTelePhone,
                    landline = a.Landline,
                    sortCode = a.SortCode.ToString()
                }).ToListAsync();
            }

            var olist = await _organizeRepository.AsQueryable().Where(t => t.EnabledMark == 1 && t.DeleteMark == null).OrderBy(a => a.CreatorTime, OrderByType.Asc).ToListAsync(); ;//获取所有组织
            var plist = await _positionRepository.GetListAsync();//获取所有岗位
            var rlist = await _roleRepository.GetListAsync();//获取所有角色

            //转换 组织结构 和 岗位(多岗位)
            foreach (var item in userList)
            {
                //获取用户组织关联数据
                var orgRelList = await _organizeRepository.AsSugarClient().Queryable<UserRelationEntity>().Where(x => x.ObjectType == "Organize" && x.UserId == item.id).Select(x => x.ObjectId).ToListAsync();

                if (orgRelList.Any())
                {
                    var oentityList = olist.Where(x => orgRelList.Contains(x.Id)).ToList();
                    if (oentityList.Any())
                    {
                        var userOrgList = new List<string>();
                        oentityList.ForEach(oentity =>
                        {
                            var oidList = oentity.OrganizeIdTree?.Split(',').ToList();
                            if (oidList != null)
                            {
                                var oNameList = olist.Where(x => oidList.Contains(x.Id)).Select(x => x.FullName).ToList();
                                userOrgList.Add(string.Join("/", oNameList));
                            }
                            else
                            {
                                var oNameList = new List<string>();
                                oNameList.Add(oentity.FullName);
                                //递归获取上级组织
                                GetOrganizeParentName(olist, oentity.ParentId, oNameList);
                                userOrgList.Add(string.Join("/", oNameList));
                            }
                        });
                        item.organizeId = string.Join(";", userOrgList);
                    }
                }

                //获取用户岗位关联
                var posRelList = await _organizeRepository.AsSugarClient().Queryable<UserRelationEntity>().Where(x => x.ObjectType == "Position" && x.UserId == item.id).Select(x => x.ObjectId).ToListAsync();
                if (posRelList.Any())
                {
                    item.positionId = string.Join(";", plist.Where(x => posRelList.Contains(x.Id)).Select(x => x.FullName + "/" + x.EnCode).ToList());
                }
                else item.positionId = "";

                //角色
                if (item.roleId.IsNotEmptyOrNull())
                {
                    var ridList = item.roleId.Split(',').ToList();
                    item.roleId = string.Join(";", rlist.Where(x => ridList.Contains(x.Id)).Select(x => x.FullName).ToList());
                }
            }

            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = DateTime.Now.ToDateString() + "_用户信息.xls";
            excelconfig.HeadFont = "微软雅黑";
            excelconfig.HeadPoint = 10;
            excelconfig.IsAllSizeColumn = true;
            excelconfig.ColumnModel = new List<ExcelColumnModel>();
            var filedList = GetUserInfoFieldToTitle(input.selectKey.Split(',').ToList());
            foreach (var item in filedList)
            {
                var column = item.Key;
                var excelColumn = item.Value;
                excelconfig.ColumnModel.Add(new ExcelColumnModel() { Column = column, ExcelColumn = excelColumn });
            }
            var addPath = FileVariable.TemporaryFilePath + excelconfig.FileName;
            ExcelExportHelper<UserListImportDataInput>.Export(userList, excelconfig, addPath);

            return new { name = excelconfig.FileName, url = "/api/file/Download?encryption=" + DESCEncryption.Encrypt(_userManager.UserId + "|" + excelconfig.FileName + "|" + addPath, "HSZ") };
        }

        /// <summary>
        /// 模板下载
        /// </summary>
        /// <returns></returns>
        [HttpGet("TemplateDownload")]
        public dynamic TemplateDownload()
        {
            var dataList = new List<UserListImportDataInput>() { new UserListImportDataInput() { } };//初始化 一条空数据

            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "用户信息导入模板.xls";
            excelconfig.HeadFont = "微软雅黑";
            excelconfig.HeadPoint = 10;
            excelconfig.IsAllSizeColumn = true;
            excelconfig.ColumnModel = new List<ExcelColumnModel>();
            var filedList = GetUserInfoFieldToTitle();
            foreach (var item in filedList)
            {
                var column = item.Key;
                var excelColumn = item.Value;
                excelconfig.ColumnModel.Add(new ExcelColumnModel() { Column = column, ExcelColumn = excelColumn });
            }
            var addPath = FileVariable.TemporaryFilePath + excelconfig.FileName;
            ExcelExportHelper<UserListImportDataInput>.Export(dataList, excelconfig, addPath);

            return new { name = excelconfig.FileName, url = "/api/file/Download?encryption=" + DESCEncryption.Encrypt(_userManager.UserId + "|" + excelconfig.FileName + "|" + addPath, "HSZ") };
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("Uploader")]
        public async Task<dynamic> Uploader(IFormFile file)
        {
            return await _fileService.Uploader("", file);
        }

        /// <summary>
        /// 导入预览
        /// </summary>
        /// <returns></returns>
        [HttpGet("ImportPreview")]
        public dynamic ImportPreview(string fileName)
        {
            try
            {
                var FileEncode = GetUserInfoFieldToTitle();

                var filePath = FileVariable.TemporaryFilePath;
                var savePath = filePath + fileName;
                //得到数据
                var excelData = ExcelImportHelper.ToDataTable(savePath);
                foreach (var item in excelData.Columns)
                {
                    excelData.Columns[item.ToString()].ColumnName = FileEncode.Where(x => x.Value == item.ToString()).FirstOrDefault().Key;
                }

                //返回结果
                return new { dataRow = excelData };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw HSZException.Oh(ErrorCode.D1801);
            }
        }

        /// <summary>
        /// 导出错误报告
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost("ExportExceptionData")]
        public async Task<dynamic> ExportExceptionData([FromBody] UserImportDataInput list)
        {
            var res = await ImportUserData(list.list);
            var errorlist = res.Last() as List<UserListImportDataInput>;//错误数据

            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "用户导入错误报告.xls";
            excelconfig.HeadFont = "微软雅黑";
            excelconfig.HeadPoint = 10;
            excelconfig.IsAllSizeColumn = true;
            excelconfig.ColumnModel = new List<ExcelColumnModel>();
            var filedList = GetUserInfoFieldToTitle();
            foreach (var item in filedList)
            {
                var column = item.Key;
                var excelColumn = item.Value;
                excelconfig.ColumnModel.Add(new ExcelColumnModel() { Column = column, ExcelColumn = excelColumn });
            }
            var addPath = FileVariable.TemporaryFilePath + excelconfig.FileName;
            ExcelExportHelper<UserListImportDataInput>.Export(errorlist, excelconfig, addPath);

            return new { name = excelconfig.FileName, url = "/api/file/Download?encryption=" + DESCEncryption.Encrypt(_userManager.UserId + "|" + excelconfig.FileName + "|" + addPath, "HSZ") };
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost("ImportData")]
        public async Task<dynamic> ImportData([FromBody] UserImportDataInput list)
        {
            var res = await ImportUserData(list.list);
            var addlist = res.First() as List<UserEntity>;
            var errorlist = res.Last() as List<UserListImportDataInput>;
            return new UserImportResultOutput() { snum = addlist.Count, fnum = errorlist.Count, failResult = errorlist, resultType = errorlist.Count < 1 ? 0 : 1 };
        }

        #endregion

        #region PublicMethod

        /// <summary>
        /// 获取用户信息 根据用户ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        [NonAction]
        public UserEntity GetInfoByUserId(string userId)
        {
            return _userRepository.GetFirst(u => u.Id == userId && u.DeleteMark == null);
        }

        /// <summary>
        /// 获取用户信息 根据用户ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        [NonAction]
        public async Task<UserEntity> GetInfoByUserIdAsync(string userId)
        {
            return await _userRepository.GetFirstAsync(u => u.Id == userId && u.DeleteMark == null);
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<UserEntity>> GetList()
        {
            return await _userRepository.AsQueryable().Where(u => u.DeleteMark == null).ToListAsync();
        }

        /// <summary>
        /// 获取用户信息 根据用户账户
        /// </summary>
        /// <param name="account">用户账户</param>
        /// <returns></returns>
        [NonAction]
        public async Task<UserEntity> GetInfoByAccount(string account)
        {
            try
            {
                return await _userRepository.GetFirstAsync(u => u.Account == account && u.DeleteMark == null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        /// <summary>
        /// 获取用户信息 根据登录信息
        /// </summary>
        /// <param name="account">用户账户</param>
        /// <param name="password">用户密码</param>
        /// <returns></returns>
        [NonAction]
        public async Task<UserEntity> GetInfoByLogin(string account, string password)
        {
            return await _userRepository.GetFirstAsync(u => u.Account == account && u.Password == password && u.DeleteMark == null);
        }

        /// <summary>
        /// 根据用户姓名获取用户ID
        /// </summary>
        /// <param name="realName">用户姓名</param>
        /// <returns></returns>
        [NonAction]
        public async Task<string> GetUserIdByRealName(string realName)
        {
            return (await _userRepository.GetFirstAsync(u => u.RealName == realName && u.DeleteMark == null)).Id;
        }

        /// <summary>
        /// 下属机构
        /// </summary>
        /// <param name="organizeId">机构ID</param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<string>> GetSubOrganizeIds(string organizeId)
        {
            var data = await _organizeService.GetListAsync();
            data = data.TreeChildNode(organizeId, t => t.Id, t => t.ParentId);
            return data.Select(m => m.Id).ToList();
        }

        /// <summary>
        /// 获取用户名
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<string> GetUserName(string userId)
        {
            var entity = await _userRepository.GetFirstAsync(x => x.Id == userId && x.DeleteMark == null);
            if (entity.IsNullOrEmpty())
                return "";
            return entity.RealName + "/" + entity.Account;
        }

        /// <summary>
        /// 获取当前用户岗位信息
        /// </summary>
        /// <param name="PositionIds"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<PositionInfo>> GetPosition(string PositionIds)
        {
            var ids = PositionIds.Split(",");
            return await _positionRepository.AsQueryable().In(it => it.Id, ids).Select(it => new PositionInfo { id = it.Id, name = it.FullName }).ToListAsync();
        }

        /// <summary>
        /// 根据id数组获取用户（分页）
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="pageInputBase"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetUserPageList(List<string> userIds, PageInputBase pageInputBase)
        {
            var data = await _userRepository.AsSugarClient().Queryable<UserEntity>().In(it => it.Id, userIds.ToArray()).
                WhereIF(pageInputBase.keyword.IsNotEmptyOrNull(), it => it.RealName.Contains(pageInputBase.keyword) || it.Account.Contains(pageInputBase.keyword)).Select(it => new { userId = it.Id, userName = SqlFunc.MergeString(it.RealName, "/", it.Account) }).MergeTable().ToPagedListAsync(pageInputBase.currentPage, pageInputBase.pageSize);
            var pageList = new SqlSugarPagedList<object>()
            {
                list = data.list,
                pagination = data.pagination
            };
            return PageResult<object>.SqlSugarPageResult(pageList);
        }

        /// <summary>
        /// 表达式获取用户
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<UserEntity> GetUserByExp(Expression<Func<UserEntity, bool>> expression)
        {
            var entity = await _userRepository.GetFirstAsync(expression);
            return entity;
        }

        /// <summary>
        /// 表达式获取用户列表
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<UserEntity>> GetUserListByExp(Expression<Func<UserEntity, bool>> expression)
        {
            var entityList = await _userRepository.AsQueryable().Where(expression).ToListAsync();
            return entityList;
        }

        /// <summary>
        /// 表达式获取指定字段的用户列表
        /// </summary>
        /// <param name="expression">where 条件表达式</param>
        /// <param name="select">select 选择字段表达式</param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<UserEntity>> GetUserListByExp(Expression<Func<UserEntity, bool>> expression, Expression<Func<UserEntity, UserEntity>> select)
        {
            var entityList = await _userRepository.AsQueryable().Where(expression).Select(select).ToListAsync();
            return entityList;
        }
        #endregion

        /// <summary>
        /// 获取集合中的组织 树 ， 根据上级ID
        /// </summary>
        /// <param name="list">组织 集合</param>
        /// <param name="parentId">上级ID</param>
        /// <param name="addList">返回</param>
        /// <returns></returns>
        private List<string> GetOrganizeParentName(List<OrganizeEntity> list, string parentId, List<string> addList)
        {
            var entity = list.Find(x => x.Id == parentId);

            if (entity.ParentId != "-1") GetOrganizeParentName(list, entity.ParentId, addList);
            else addList.Add(entity.FullName);

            return addList;
        }

        /// <summary>
        /// 是否我的下属
        /// </summary>
        /// <param name="userId">当前用户</param>
        /// <param name="managerId">主管ID</param>
        /// <param name="tier">层级</param>
        /// <returns></returns>
        private async Task<bool> GetIsMyStaff(string userId, string managerId, int tier)
        {
            var isMyStaff = false;
            if (tier <= 0)
            {
                return true;
            }
            var superiorUserId = (await _userRepository.GetFirstAsync(it => it.Id.Equals(managerId) && it.DeleteMark == null))?.ManagerId;
            if (superiorUserId == null)
            {
                isMyStaff = false;
            }
            else if (userId == superiorUserId)
            {
                isMyStaff = true;
            }
            else
            {
                tier--;
                isMyStaff = await GetIsMyStaff(userId, superiorUserId, tier);
            }
            return isMyStaff;
        }

        /// <summary>
        /// 用户信息 字段对应 列名称
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetUserInfoFieldToTitle(List<string> fields = null)
        {
            var res = new Dictionary<string, string>();
            res.Add("account", "账户");
            //res.Add("Password", "密码");
            res.Add("realName", "姓名");
            res.Add("gender", "性别");
            //res.Add("NickName", "昵称");
            res.Add("email", "电子邮箱");
            res.Add("organizeId", "所属组织");
            res.Add("managerId", "直属主管");
            res.Add("positionId", "岗位");
            res.Add("roleId", "角色");
            res.Add("sortCode", "排序");
            res.Add("enabledMark", "状态");
            //res.Add("_DeleteMark", "删除标志");
            res.Add("description", "说明");
            res.Add("nation", "民族");
            res.Add("nativePlace", "籍贯");
            res.Add("entryDate", "入职时间");
            res.Add("certificatesType", "证件类型");
            res.Add("certificatesNumber", "证件号码");
            res.Add("education", "文化程度");
            res.Add("birthday", "出生年月");
            res.Add("telePhone", "办公电话");
            res.Add("landline", "办公座机");
            res.Add("mobilePhone", "手机号码");
            res.Add("urgentContacts", "紧急联系");
            res.Add("urgentTelePhone", "紧急电话");
            res.Add("postalAddress", "通讯地址");
            //res.Add("ChangePasswordDate", "最后修改密码时间");
            //res.Add("CreatorTime", "创建时间");
            //res.Add("CreatorUserId", "创建用户");
            //res.Add("LastModifyUserId", "修改用户");
            //res.Add("DeleteTime", "删除时间");
            //res.Add("DeleteUserId", "删除用户");
            //res.Add("PortalId", "门户");
            //res.Add("FirstLogIP", "首次登录IP");
            //res.Add("FirstLogTime", "首次登录时间");
            //res.Add("_IsAdministrator", "是否管理员");
            //res.Add("CommonMenu", "常用菜单");
            //res.Add("PrevLogIP", "前次登录IP");
            //res.Add("PrevLogTime", "前次登录时间");
            //res.Add("PropertyJson", "扩展属性");
            //res.Add("QuickQuery", "快速查询");
            //res.Add("Secretkey", "密钥");
            //res.Add("Signature", "自我介绍");
            //res.Add("Theme", "系统样式");
            //res.Add("UnLockTime", "解锁时间");
            //res.Add("LogErrorCount", "登录错误次数");
            //res.Add("Language", "系统语言");
            //res.Add("LastLogIP", "最后登录IP");
            //res.Add("LastLogTime", "最后登录时间");
            //res.Add("LastModifyTime", "修改时间");
            //res.Add("_LockMark", "是否锁定");
            //res.Add("LogSuccessCount", "登录成功次数");

            if (fields == null || !fields.Any()) return res;

            var result = new Dictionary<string, string>();

            foreach (var item in res)
            {
                if (fields.Contains(item.Key))
                    result.Add(item.Key, item.Value);
            }

            return result;
        }

        /// <summary>
        /// 导入用户数据函数
        /// </summary>
        /// <param name="list">list</param>
        /// <returns>[成功列表,失败列表]</returns>
        private async Task<object[]> ImportUserData(List<UserListImportDataInput> list)
        {
            List<UserListImportDataInput> userInputList = list;

            #region 初步排除错误数据
            if (userInputList == null || userInputList.Count() < 1)
                throw HSZException.Oh(ErrorCode.D5019);

            //必填字段验证 (账号，姓名，所属组织)
            var errorList = userInputList.Where(x => !x.account.IsNotEmptyOrNull() || !x.realName.IsNotEmptyOrNull() || !x.organizeId.IsNotEmptyOrNull()).ToList();
            var _userRepositoryList = await _userRepository.GetListAsync();//用户账号 (匹配直属主管 和 验证重复账号)

            var repeat = _userRepositoryList.Where(u => userInputList.Select(x => x.account).Contains(u.Account) && u.DeleteMark == null).ToList();//已存在的账号

            if (repeat.Any())
                errorList.AddRange(userInputList.Where(u => repeat.Select(x => x.Account).Contains(u.account)));//已存在的账号 列入 错误列表

            userInputList = userInputList.Except(errorList).ToList();
            #endregion

            var userList = new List<UserEntity>();

            #region 预处理关联表数据
            var _organizeServiceList = await _organizeService.GetListAsync();//组织机构
            _organizeServiceList = _organizeServiceList.Where(x => x.DeleteMark == null).OrderBy(x => x.CreatorTime).ToList();
            var organizeDic = new Dictionary<string, string>();

            //组织 结构树 处理
            foreach (var item in _organizeServiceList)
            {
                var tree = item.OrganizeIdTree?.Split(',');
                if (tree != null)
                {
                    var nameList = _organizeServiceList.Where(x => tree.Contains(x.Id)).Select(x => x.FullName).ToList();
                    organizeDic.Add(item.Id, string.Join("/", nameList));
                }
                else
                {
                    organizeDic.Add(item.Id, string.Join("/", ""));
                }
            }

            var _positionRepositoryList = await _positionRepository.AsQueryable().Where(x => x.DeleteMark == null).ToListAsync();//岗位
            var _roleRepositoryList = await _roleRepository.AsQueryable().Where(x => x.DeleteMark == null).ToListAsync();//角色

            var typeEntity = await Db.Queryable<DictionaryTypeEntity>().Where(x => (x.Id == "963255a34ea64a2584c5d1ba269c1fe6" || x.EnCode == "963255a34ea64a2584c5d1ba269c1fe6") && x.DeleteMark == null).FirstAsync();
            var _genderList = await Db.Queryable<DictionaryDataEntity>().Where(d => d.DictionaryTypeId == typeEntity.Id && d.DeleteMark == null).ToListAsync();//性别

            typeEntity = await Db.Queryable<DictionaryTypeEntity>().Where(x => (x.Id == "b6cd65a763fa45eb9fe98e5057693e40" || x.EnCode == "b6cd65a763fa45eb9fe98e5057693e40") && x.DeleteMark == null).FirstAsync();
            var _nationList = await Db.Queryable<DictionaryDataEntity>().Where(d => d.DictionaryTypeId == typeEntity.Id && d.DeleteMark == null).ToListAsync();//民族

            typeEntity = await Db.Queryable<DictionaryTypeEntity>().Where(x => (x.Id == "7866376d5f694d4d851c7164bd00ebfc" || x.EnCode == "7866376d5f694d4d851c7164bd00ebfc") && x.DeleteMark == null).FirstAsync();
            var _certificateTypeList = await Db.Queryable<DictionaryDataEntity>().Where(d => d.DictionaryTypeId == typeEntity.Id && d.DeleteMark == null).ToListAsync();//证件类型

            typeEntity = await Db.Queryable<DictionaryTypeEntity>().Where(x => (x.Id == "6a6d6fb541b742fbae7e8888528baa16" || x.EnCode == "6a6d6fb541b742fbae7e8888528baa16") && x.DeleteMark == null).FirstAsync();
            var _educationList = await Db.Queryable<DictionaryDataEntity>().Where(d => d.DictionaryTypeId == typeEntity.Id && d.DeleteMark == null).ToListAsync();//文化程度

            #endregion

            var userRelationList = new List<UserRelationEntity>();//用户关系数据
            foreach (var item in userInputList)
            {
                var orgIds = new List<string>();//多组织 , 号隔开
                var posIds = new List<string>();//多岗位 , 号隔开

                var uentity = new UserEntity();
                uentity.Id = YitIdHelper.NextId().ToString();
                if (string.IsNullOrEmpty(uentity.HeadIcon)) uentity.HeadIcon = "001.png";
                uentity.Secretkey = Guid.NewGuid().ToString();
                uentity.Password = MD5Encryption.Encrypt(MD5Encryption.Encrypt(CommonConst.DEFAULT_PASSWORD) + uentity.Secretkey);//初始化密码
                uentity.ManagerId = _userRepositoryList.Find(x => x.Account == item.managerId?.Split('/').LastOrDefault())?.Id;//寻找主管

                //寻找角色
                if (item.roleId.IsNotEmptyOrNull() && item.roleId.Split(";").Any())
                    uentity.RoleId = string.Join(",", _roleRepositoryList.Where(r => item.roleId.Split(";").Contains(r.FullName)).Select(x => x.Id).ToList());

                //寻找组织
                var userOidList = item.organizeId.Split(";");
                if (userOidList.Any())
                {
                    foreach (var oinfo in userOidList)
                    {
                        if (organizeDic.ContainsValue(oinfo)) orgIds.Add(organizeDic.Where(x => x.Value == oinfo).FirstOrDefault().Key);
                    }
                }
                else errorList.Add(item);//如果未找到组织，列入错误列表
                if (!orgIds.Any()) errorList.Add(item);//如果未找到组织，列入错误列表

                //寻找岗位
                item.positionId?.Split(';').ToList().ForEach(it =>
                {
                    var pinfo = it.Split("/");
                    var pid = _positionRepositoryList.Find(x => x.FullName == pinfo.FirstOrDefault() && x.EnCode == pinfo.LastOrDefault())?.Id;
                    if (pid.IsNotEmptyOrNull()) posIds.Add(pid);//多岗位
                });

                //性别
                if (_genderList.Find(x => x.FullName == item.gender) != null) uentity.Gender = _genderList.Find(x => x.FullName == item.gender).EnCode.ToInt();
                else uentity.Gender = _genderList.Find(x => x.FullName == "保密").EnCode.ToInt();

                uentity.Nation = _nationList.Find(x => x.FullName == item.nation)?.Id;//民族
                uentity.Education = _educationList.Find(x => x.FullName == item.education)?.Id;//文化程度
                uentity.CertificatesType = _certificateTypeList.Find(x => x.FullName == item.certificatesType)?.Id;//证件类型
                uentity.Account = item.account;
                uentity.Birthday = item.birthday;
                uentity.CertificatesNumber = item.certificatesNumber;
                uentity.CreatorUserId = _userManager.UserId;
                uentity.CreatorTime = DateTime.Now;
                uentity.Description = item.description;
                uentity.Email = item.email;
                switch (item.enabledMark)
                {
                    case "禁用":
                        uentity.EnabledMark = 0;
                        break;
                    case "正常":
                        uentity.EnabledMark = 1;
                        break;
                    case "锁定":
                        uentity.EnabledMark = 2;
                        break;
                    default:
                        uentity.EnabledMark = 2;
                        break;
                }
                uentity.EntryDate = item.entryDate;
                uentity.Landline = item.landline;
                uentity.MobilePhone = item.mobilePhone;
                uentity.NativePlace = item.nativePlace;
                uentity.PostalAddress = item.postalAddress;
                uentity.RealName = item.realName;
                uentity.SortCode = item.sortCode.ToInt();
                uentity.TelePhone = item.telePhone;
                uentity.UrgentContacts = item.urgentContacts;
                uentity.UrgentTelePhone = item.urgentTelePhone;
                userList.Add(uentity);

                #region 多组织 优先选择有权限组织
                uentity.OrganizeId = "";

                foreach (var it in orgIds)
                {
                    var UserRoleList = await _userManager.GetUserOrgRoleIds(uentity.RoleId, it);

                    //如果该组织下有角色并且有角色权限 则为默认组织
                    if (UserRoleList.Any() && _userRepository.AsSugarClient().Queryable<AuthorizeEntity>().Where(x => x.ObjectType == "Role" && x.ItemType == "module" && UserRoleList.Contains(x.ObjectId)).Any())
                    {
                        uentity.OrganizeId = it;//多 组织 默认
                        break;
                    }
                }
                if (uentity.OrganizeId.IsNullOrEmpty())//如果所选组织下都没有角色或者没有角色权限 默认取第一个
                    uentity.OrganizeId = orgIds.FirstOrDefault();

                #endregion

                if (uentity.OrganizeId.IsNotEmptyOrNull())
                {
                    var roleList = _userRelationService.CreateByRole(uentity.Id, uentity.RoleId);//角色关系
                    var positionList = _userRelationService.CreateByPosition(uentity.Id, string.Join(",", posIds));//岗位关系
                    var organizeList = _userRelationService.CreateByOrganize(uentity.Id, string.Join(",", orgIds));//组织关系
                    userRelationList.AddRange(positionList);
                    userRelationList.AddRange(roleList);
                    userRelationList.AddRange(organizeList);
                }
            }

            var addList = userList.Where(x => !errorList.Select(e => e.account).Contains(x.Account)).ToList();

            if (addList.Any())
            {
                try
                {
                    //开启事务
                    Db.BeginTran();

                    //新增用户记录
                    var newEntity = await _userRepository.AsInsertable(addList).CallEntityMethod(m => m.Create()).ExecuteReturnEntityAsync();

                    //批量新增用户关系
                    if (userRelationList.Count > 0) await _userRelationService.Create(userRelationList);

                    Db.CommitTran();
                }
                catch (Exception)
                {
                    Db.RollbackTran();
                    errorList.AddRange(userInputList);
                    userInputList = new List<UserListImportDataInput>();
                }
            }

            return new object[] { addList, errorList };
        }
    }
}
