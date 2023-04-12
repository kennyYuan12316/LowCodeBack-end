using HSZ.Common.Configuration;
using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Common.Util;
using HSZ.DataEncryption;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.System.Entitys.Dto.Permission.UsersCurrent;
using HSZ.System.Entitys.Model.Permission.UsersCurrent;
using HSZ.System.Entitys.Permission;
using HSZ.System.Entitys.System;
using HSZ.System.Interfaces.Permission;
using HSZ.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSZ.System.Service.Permission
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：业务实现:个人资料
    /// </summary>
    [ApiDescriptionSettings(Tag = "Permission", Name = "Current", Order = 168)]
    [Route("api/permission/Users/[controller]")]
    public class UsersCurrentService : IUsersCurrentService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<OrganizeEntity> _organizeRepository;
        private readonly ISqlSugarRepository<UserEntity> _userRepository;
        private readonly ISqlSugarRepository<PositionEntity> _positionRepository;
        private readonly ISqlSugarRepository<SysLogEntity> _sysLogRepository;
        private readonly IAuthorizeService _authorizeService;
        private readonly ICacheManager _cacheManager;
        private readonly IUserManager _userManager; // 用户管理

        /// <summary>
        /// 初始化一个<see cref="UsersCurrentService"/>类型的新实例
        /// </summary>
        public UsersCurrentService(ISqlSugarRepository<OrganizeEntity> organizeRepository,
            ISqlSugarRepository<UserEntity> userRepository,
            ISqlSugarRepository<PositionEntity> positionRepository,
            ISqlSugarRepository<SysLogEntity> sysLogRepository,
            IAuthorizeService authorizeService,
            ICacheManager cacheManager, IUserManager userManager)
        {
            _organizeRepository = organizeRepository;
            _userRepository = userRepository;
            _positionRepository = positionRepository;
            _sysLogRepository = sysLogRepository;
            _authorizeService = authorizeService;
            _cacheManager = cacheManager;
            _userManager = userManager;
        }

        #region GET
        /// <summary>
        /// 获取我的下属
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <returns></returns>
        [HttpGet("Subordinate/{id}")]
        public async Task<dynamic> GetSubordinate(string id)
        {
            var userInfo = await _userManager.GetUserInfo();

            //获取用户Id 下属 ,顶级节点为 自己
            var userIds = new List<string>();
            if (id == "0") userIds.Add(userInfo.userId);
            else userIds = await _userRepository.AsSugarClient().Queryable<UserEntity>().Where(m => m.ManagerId == id && m.DeleteMark == null).Select(m => m.Id).ToListAsync();

            if (userIds.Any())
            {
                var data = await _userRepository.AsSugarClient().Queryable<UserEntity, OrganizeEntity, PositionEntity>
                    ((a, b, c) => new JoinQueryInfos(JoinType.Left, b.Id == SqlFunc.ToString(a.OrganizeId), JoinType.Left, c.Id == SqlFunc.ToString(a.PositionId)))
                    .Select((a, b, c) => new
                    {
                        Id = a.Id,
                        Avatar = SqlFunc.MergeString("/api/File/Image/userAvatar/", a.HeadIcon),
                        UserName = SqlFunc.MergeString(a.RealName, "/", a.Account),
                        IsLeaf = false,
                        Department = b.FullName,
                        Position = c.FullName,
                        DeleteMark = a.DeleteMark,
                        EnabledMark = a.EnabledMark,
                        SortCode = a.SortCode
                    }).MergeTable()
                    .WhereIF(userIds.Any(), u => userIds.Contains(u.Id))
                    .Where(u => u.DeleteMark == null && u.EnabledMark == 1)
                    .OrderBy(o => o.SortCode).Select<UsersCurrentSubordinateOutput>()
                    .ToListAsync();
                return data;
            }
            else return new List<UsersCurrentSubordinateOutput>();
        }

        /// <summary>
        /// 获取个人资料
        /// </summary>
        /// <returns></returns>
        [HttpGet("BaseInfo")]
        public async Task<dynamic> GetBaseInfo()
        {
            var userId = _userManager.UserId;
            var tenantId = _userManager.TenantId;

            var data = await _userRepository.AsSugarClient().Queryable<UserEntity, UserEntity>((a, d) => new JoinQueryInfos(JoinType.Left, d.Id == a.ManagerId))
                .Select((a, d) => new UsersCurrentInfoOutput
                {
                    id = a.Id,
                    account = SqlFunc.IIF(KeyVariable.MultiTenancy == true,
                    SqlFunc.MergeString(_userManager.TenantId, "@", a.Account), a.Account),
                    realName = a.RealName,
                    position = "",
                    positionId = a.PositionId,
                    organizeId = a.OrganizeId,
                    manager = SqlFunc.IIF(d.Account == null, null,
                    SqlFunc.MergeString(d.RealName, "/", d.Account)),
                    roleId = "",
                    roleIds = a.RoleId,
                    creatorTime = a.CreatorTime,
                    prevLogTime = a.PrevLogTime,
                    signature = a.Signature,
                    gender = a.Gender.ToString(),
                    nation = a.Nation,
                    nativePlace = a.NativePlace,
                    entryDate = a.EntryDate,
                    certificatesType = a.CertificatesType,
                    certificatesNumber = a.CertificatesNumber,
                    education = a.Education,
                    birthday = a.Birthday,
                    telePhone = a.TelePhone,
                    landline = a.Landline,
                    mobilePhone = a.MobilePhone,
                    email = a.Email,
                    urgentContacts = a.UrgentContacts,
                    urgentTelePhone = a.UrgentTelePhone,
                    postalAddress = a.PostalAddress,
                    theme = a.Theme,
                    language = a.Language,
                    avatar = SqlFunc.IIF(SqlFunc.IsNullOrEmpty(SqlFunc.ToString(a.HeadIcon)), "", SqlFunc.MergeString("/api/File/Image/userAvatar/", SqlFunc.ToString(a.HeadIcon)))
                }).MergeTable()
                .Where(a => a.id == userId).FirstAsync();

            //组织结构
            var olist = _userRepository.AsSugarClient().Queryable<OrganizeEntity>().ToParentList(it => it.ParentId, data.organizeId).OrderBy(x => x.CreatorTime).Select(x => x.FullName).ToList();
            data.organize = string.Join(" / ", olist);

            //获取当前用户、当前组织下的所有岗位
            var pNameList = await _positionRepository.AsSugarClient().Queryable<PositionEntity,UserRelationEntity>((a,b)=>new JoinQueryInfos(JoinType.Left,a.Id==b.ObjectId))
                .Where((a, b) => b.ObjectType == "Position" && b.UserId == userId && a.OrganizeId==data.organizeId).Select(a => a.FullName).ToListAsync();
            data.position = string.Join(",", pNameList);

            //获取当前用户、全局角色 和当前组织下的所有角色
            var roleList = await _userManager.GetUserOrgRoleIds(data.roleIds, data.organizeId);
            data.roleId = await _userManager.GetRoleNameByIds(string.Join(",", roleList));

            return data;
        }

        /// <summary>
        /// 获取系统权限
        /// </summary>
        /// <returns></returns>
        [HttpGet("Authorize")]
        public async Task<dynamic> GetAuthorize()
        {
            var user = await _userManager.GetUserInfo();
            var userId = _userManager.UserId;
            var isAdmin = _userManager.IsAdministrator;
            var output = new UsersCurrentAuthorizeOutput();
            var moduleList = await _authorizeService.GetCurrentUserModuleAuthorize(userId, isAdmin, user.roleIds);
            var buttonList = await _authorizeService.GetCurrentUserButtonAuthorize(userId, isAdmin, user.roleIds);
            var columnList = await _authorizeService.GetCurrentUserColumnAuthorize(userId, isAdmin, user.roleIds);
            var resourceList = await _authorizeService.GetCurrentUserResourceAuthorize(userId, isAdmin, user.roleIds);
            var formList = await _authorizeService.GetCurrentUserFormAuthorize(userId, isAdmin, user.roleIds);
            if (moduleList.Count != 0)
                output.module = moduleList.Adapt<List<UsersCurrentAuthorizeMoldel>>().ToTree("-1");
            if (buttonList.Count != 0)
            {
                var menuAuthorizeData = new List<UsersCurrentAuthorizeMoldel>();
                var pids = buttonList.Select(m => m.ModuleId).ToList();
                this.GetParentsModuleList(pids, moduleList, ref menuAuthorizeData);
                output.button = menuAuthorizeData.Union(buttonList.Adapt<List<UsersCurrentAuthorizeMoldel>>()).ToList().ToTree("-1");
            }
            if (columnList.Count != 0)
            {
                var menuAuthorizeData = new List<UsersCurrentAuthorizeMoldel>();
                var pids = columnList.Select(m => m.ModuleId).ToList();
                this.GetParentsModuleList(pids, moduleList, ref menuAuthorizeData);
                output.column = menuAuthorizeData.Union(columnList.Adapt<List<UsersCurrentAuthorizeMoldel>>()).ToList().ToTree("-1");
            }
            if (resourceList.Count != 0)
            {
                var resourceData = resourceList.Select(r => new UsersCurrentAuthorizeMoldel
                {
                    id = r.Id,
                    parentId = r.ModuleId,
                    fullName = r.FullName,
                    icon = "icon-sz icon-sz-extend"
                }).ToList();
                var menuAuthorizeData = new List<UsersCurrentAuthorizeMoldel>();
                var pids = resourceList.Select(bt => bt.ModuleId).ToList();
                this.GetParentsModuleList(pids, moduleList, ref menuAuthorizeData);
                output.resource = menuAuthorizeData.Union(resourceData.Adapt<List<UsersCurrentAuthorizeMoldel>>()).ToList().ToTree("-1");
            }

            if (formList.Count != 0)
            {
                var formData = formList.Select(r => new UsersCurrentAuthorizeMoldel
                {
                    id = r.Id,
                    parentId = r.ModuleId,
                    fullName = r.FullName,
                    icon = "icon-sz icon-sz-extend"
                }).ToList();
                var menuAuthorizeData = new List<UsersCurrentAuthorizeMoldel>();
                var pids = formList.Select(bt => bt.ModuleId).ToList();
                this.GetParentsModuleList(pids, moduleList, ref menuAuthorizeData);
                output.form = menuAuthorizeData.Union(formData.Adapt<List<UsersCurrentAuthorizeMoldel>>()).ToList().ToTree("-1");
            }

            return output;
        }

        /// <summary>
        /// 获取系统日志
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpGet("SystemLog")]
        public async Task<dynamic> GetSystemLog([FromQuery] UsersCurrentSystemLogQuery input)
        {
            DateTime? startTime = input.startTime != null ? Ext.GetDateTime(input.startTime.ToString()) : null;
            DateTime? endTime = input.endTime != null ? Ext.GetDateTime(input.endTime.ToString()) : null;
            var userId = _userManager.UserId;
            var requestParam = input.Adapt<PageInputBase>();
            var data = await _sysLogRepository.AsSugarClient().Queryable<SysLogEntity>().Select(a => new UsersCurrentSystemLogOutput { creatorTime = a.CreatorTime, userName = a.UserName, ipaddress = a.IPAddress, moduleName = a.ModuleName, category = a.Category, userId = a.UserId, platForm = a.PlatForm, requestURL = a.RequestURL, requestMethod = a.RequestMethod, requestDuration = a.RequestDuration }).MergeTable()
                .WhereIF(!startTime.IsNullOrEmpty(), s => s.creatorTime >= new DateTime(startTime.ToDate().Year, startTime.ToDate().Month, startTime.ToDate().Day, 0, 0, 0, 0))
                .WhereIF(!endTime.IsNullOrEmpty(), s => s.creatorTime <= new DateTime(endTime.ToDate().Year, endTime.ToDate().Month, endTime.ToDate().Day, 23, 59, 59, 999))
                .WhereIF(!input.keyword.IsNullOrEmpty(), s => s.userName.Contains(input.keyword) || s.ipaddress.Contains(input.keyword) || s.moduleName.Contains(input.keyword))
                .Where(s => s.category == input.category && s.userId == userId).OrderBy(o => o.creatorTime, OrderByType.Desc).ToPagedListAsync(requestParam.currentPage, requestParam.pageSize);
            return PageResult<UsersCurrentSystemLogOutput>.SqlSugarPageResult(data);
        }

        #endregion

        #region Post

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        [HttpPost("Actions/ModifyPassword")]
        public async Task ModifyPassword([FromBody] UsersCurrentActionsModifyPasswordInput input)
        {
            var user = _userManager.User;
            if (MD5Encryption.Encrypt(input.oldPassword + user.Secretkey) != user.Password.ToLower())
                throw HSZException.Oh(ErrorCode.D5007);
            var imageCode = _cacheManager.GetCode(input.timestamp);
            if (!input.code.ToLower().Equals(imageCode.Result.ToString().ToLower()))
            {
                throw HSZException.Oh(ErrorCode.D5015);
            }
            else
            {
                await _cacheManager.DelCode(input.timestamp);
                await _cacheManager.DelUserInfo(_userManager.TenantId + "_" + user.Id);
            }
            user.Password = MD5Encryption.Encrypt(input.password + user.Secretkey);
            user.ChangePasswordDate = DateTime.Now;
            var isOk = await _userRepository.AsUpdateable(user).UpdateColumns(it => new
            {
                it.Password,
                it.ChangePasswordDate,
                it.LastModifyUserId,
                it.LastModifyTime
            }).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.D5008);
        }

        /// <summary>
        /// 修改个人资料
        /// </summary>
        /// <returns></returns>
        [HttpPut("BaseInfo")]
        public async Task UpdateBaseInfo([FromBody] UsersCurrentInfoUpInput input)
        {
            var user = _userManager.User;
            var userInfo = input.Adapt<UserEntity>();
            userInfo.Id = user.Id;
            userInfo.IsAdministrator = user.IsAdministrator;
            var isOk = await _userRepository.AsUpdateable(userInfo).UpdateColumns(it => new
            {
                it.RealName,
                it.Signature,
                it.Gender,
                it.Nation,
                it.NativePlace,
                it.CertificatesType,
                it.CertificatesNumber,
                it.Education,
                it.Birthday,
                it.TelePhone,
                it.Landline,
                it.MobilePhone,
                it.Email,
                it.UrgentContacts,
                it.UrgentTelePhone,
                it.PostalAddress,
                it.LastModifyUserId,
                it.LastModifyTime
            }).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.D5009);
        }

        /// <summary>
        /// 修改主题
        /// </summary>
        /// <returns></returns>
        [HttpPut("SystemTheme")]
        public async Task UpdateBaseInfo([FromBody] UsersCurrentSysTheme input)
        {
            var user = _userManager.User;
            user.Theme = input.theme;
            var isOk = await _userRepository.AsUpdateable(user).UpdateColumns(it => new
            {
                it.Theme,
                it.LastModifyUserId,
                it.LastModifyTime
            }).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.D5010);
        }

        /// <summary>
        /// 修改语言
        /// </summary>
        /// <returns></returns>
        [HttpPut("SystemLanguage")]
        public async Task UpdateLanguage([FromBody] UsersCurrentSysLanguage input)
        {
            var user = _userManager.User;
            user.Language = input.language;
            var isOk = await _userRepository.AsUpdateable(user).UpdateColumns(it => new
            {
                it.Language,
                it.LastModifyUserId,
                it.LastModifyTime
            }).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.D5011);
        }

        /// <summary>
        /// 修改头像
        /// </summary>
        /// <returns></returns>
        [HttpPut("Avatar/{name}")]
        public async Task UpdateAvatar(string name)
        {
            var user = _userManager.User;
            user.HeadIcon = name;
            var isOk = await _userRepository.AsUpdateable(user).UpdateColumns(it => new
            {
                it.HeadIcon,
                it.LastModifyUserId,
                it.LastModifyTime
            }).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.D5012);
        }

        /// <summary>
        /// 切换 默认 ： 组织、岗位、角色
        /// </summary>
        /// <returns></returns>
        [HttpPut("major")]
        public async Task DefaultOrganize([FromBody] UsersCurrentDefaultOrganizeInput input)
        {
            var userInfo = _userManager.User;

            switch (input.majorType)
            {
                case "Organize"://组织
                    {
                        userInfo.OrganizeId = input.majorId;

                        var roleList = await _userManager.GetUserOrgRoleIds(userInfo.RoleId, userInfo.OrganizeId);

                        //如果该组织下没有角色 则 切换组织失败
                        if (!roleList.Any())
                            throw HSZException.Oh(ErrorCode.D5023);
                        //该组织下没有任何权限 则 切换组织失败
                        if (!_userRepository.AsSugarClient().Queryable<AuthorizeEntity>().Where(x => x.ObjectType == "Role" && x.ItemType == "module" && roleList.Contains(x.ObjectId)).Any())
                            throw HSZException.Oh(ErrorCode.D5023);

                        //获取切换组织 Id 下的所有岗位
                        var pList = await _positionRepository.AsSugarClient().Queryable<PositionEntity>().Where(x => x.OrganizeId == input.majorId).Select(x => x.Id).ToListAsync();

                        //获取切换组织的 岗位，如果该组织没有岗位则为空
                        var idList = await _userRepository.AsSugarClient().Queryable<UserRelationEntity>()
                            .Where(x => x.UserId == userInfo.Id && pList.Contains(x.ObjectId) && x.ObjectType == "Position").Select(x => x.ObjectId).ToListAsync();
                        userInfo.PositionId = idList.FirstOrDefault() == null ? "" : idList.FirstOrDefault();

                        var isOk = await _userRepository.AsUpdateable(userInfo).UpdateColumns(it => new
                        {
                            it.OrganizeId,
                            it.PositionId,
                            it.LastModifyUserId,
                            it.LastModifyTime
                        }).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
                        if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.D5020);
                    }
                    break;
                case "Position"://岗位
                    {
                        userInfo.PositionId = input.majorId;
                        var isOk = await _userRepository.AsUpdateable(userInfo).UpdateColumns(it => new
                        {
                            it.PositionId,
                            it.LastModifyUserId,
                            it.LastModifyTime
                        }).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
                        if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.D5020);
                        break;
                    }
            }
        }

        /// <summary>
        /// 获取当前用户所有组织
        /// </summary>
        /// <returns></returns>
        [HttpGet("getUserOrganizes")]
        public async Task<dynamic> GetUserOrganizes()
        {
            var userInfo = await _userManager.GetUserInfo();

            //获取当前用户所有关联 组织ID 集合
            var idList = await _userRepository.AsSugarClient().Queryable<UserRelationEntity>()
                .Where(x => x.UserId == userInfo.userId && x.ObjectType == "Organize")
                .Select(x=>x.ObjectId).ToListAsync();

            //获取所有组织
            var allOranizeList = await _organizeRepository.AsSugarClient().Queryable<OrganizeEntity>().Where(x => x.DeleteMark == null).OrderBy(x=>x.CreatorTime).ToListAsync();
            allOranizeList.Where(x => x.OrganizeIdTree == null || x.OrganizeIdTree == "").ToList().ForEach(item => { item.OrganizeIdTree = item.Id; });

            //根据关联组织ID 查询组织信息
            var oList = allOranizeList.Where(x => idList.Contains(x.Id))
                .Select(x => new CurrentUserOrganizesOutput
                {
                    id = x.Id,
                    fullName = string.Join("/", allOranizeList.Where(all => x.OrganizeIdTree.Split(",").Contains(all.Id)).Select(s=>s.FullName))
                }).ToList();

            var def = oList.Where(x => x.id == userInfo.organizeId).FirstOrDefault();
            if (def != null) def.isDefault = true;

            return oList;
        }

        /// <summary>
        /// 获取当前用户所有岗位
        /// </summary>
        /// <returns></returns>
        [HttpGet("getUserPositions")]
        public async Task<dynamic> GetUserPositions()
        {
            var userInfo = await _userManager.GetUserInfo();

            //获取当前用户所有关联 岗位ID 集合
            var idList = await _userRepository.AsSugarClient().Queryable<UserRelationEntity>()
                .Where(x => x.UserId == userInfo.userId && x.ObjectType == "Position")
                .Select(x => x.ObjectId).ToListAsync();

            //根据关联 岗位ID 查询岗位信息
            var oList = await _positionRepository.AsSugarClient().Queryable<PositionEntity>()
                .Where(x => x.OrganizeId == userInfo.organizeId).Where(x => idList.Contains(x.Id))
                .Select(x => new CurrentUserOrganizesOutput
                {
                    id = x.Id,
                    fullName = x.FullName
                }).ToListAsync();

            var def = oList.Where(x => x.id == userInfo.positionId).FirstOrDefault();
            if (def != null) def.isDefault = true;

            return oList;
        }

        #endregion

        #region PrivateMethod

        /// <summary>
        /// 过滤菜单权限数据
        /// </summary>
        /// <param name="pids">其他权限数据</param>
        /// <param name="moduleList">勾选菜单权限数据</param>
        /// <param name="output">返回值</param>
        private void GetParentsModuleList(List<string> pids, List<ModuleEntity> moduleList, ref List<UsersCurrentAuthorizeMoldel> output)
        {
            var authorizeModuleData = moduleList.Adapt<List<UsersCurrentAuthorizeMoldel>>();
            foreach (var item in pids)
            {
                this.GteModuleListById(item, authorizeModuleData, output);
            }
            output = output.Distinct().ToList();
        }

        /// <summary>
        /// 根据菜单id递归获取authorizeDataOutputModel的父级菜单
        /// </summary>
        /// <param name="id">菜单id</param>
        /// <param name="authorizeModuleData">选中菜单集合</param>
        /// <param name="output">返回数据</param>
        private void GteModuleListById(string id, List<UsersCurrentAuthorizeMoldel> authorizeModuleData, List<UsersCurrentAuthorizeMoldel> output)
        {
            var data = authorizeModuleData.Find(l => l.id == id);
            if (data != null)
            {
                if (!data.parentId.Equals("-1"))
                {
                    if (!output.Contains(data))
                    {
                        output.Add(data);
                    }
                    GteModuleListById(data.parentId, authorizeModuleData, output);
                }
                else
                {
                    if (!output.Contains(data))
                    {
                        output.Add(data);
                    }
                }
            }
        }

        #endregion
    }
}
