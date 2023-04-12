using HSZ.Common.Const;
using HSZ.Common.Extension;
using HSZ.Common.Helper;
using HSZ.Common.Util;
using HSZ.Dependency;
using HSZ.System.Entitys.Enum;
using HSZ.System.Entitys.Model.Permission.Authorize;
using HSZ.System.Entitys.Model.Permission.User;
using HSZ.System.Entitys.Permission;
using HSZ.System.Entitys.System;
using Microsoft.AspNetCore.Http;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UAParser;
using Enums = System.Enum;

namespace HSZ.Common.Core.Manager
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：用户管理
    /// </summary>
    public class UserManager : IUserManager, IScoped
    {
        private readonly ISqlSugarRepository<UserEntity> _userRepository;  // 用户表仓储
        private readonly ISqlSugarRepository<OrganizeEntity> _organizeRepository;
        private readonly ISqlSugarRepository<RoleEntity> _roleRepository;  // 角色表仓储
        private readonly ISqlSugarRepository<PositionEntity> _positionRepository;
        private readonly ISqlSugarRepository<SysConfigEntity> _sysConfigRepository;
        private readonly ISqlSugarRepository<AuthorizeEntity> _authorizeRepository; //权限操作表仓储
        private readonly ISqlSugarRepository<ModuleDataAuthorizeSchemeEntity> _moduleDataAuthorizeSchemeRepository;
        private readonly ISqlSugarRepository<OrganizeAdministratorEntity> _organizeAdministratorRepository;
        private readonly ICacheManager _cacheManager;
        private readonly HttpContext _httpContext;

        /// <summary>
        /// 初始化一个<see cref="UserManager"/>类型的新实例
        /// </summary>
        public UserManager(ISqlSugarRepository<UserEntity> userRepository,
            ISqlSugarRepository<RoleEntity> roleRepository,
            ISqlSugarRepository<SysConfigEntity> sysConfigRepository,
            ISqlSugarRepository<OrganizeEntity> organizeRepository,
            ISqlSugarRepository<PositionEntity> positionRepository,
            ISqlSugarRepository<AuthorizeEntity> authorizeRepository,
            ISqlSugarRepository<ModuleDataAuthorizeSchemeEntity> moduleDataAuthorizeSchemeRepository,
            ISqlSugarRepository<OrganizeAdministratorEntity> organizeAdministratorRepository,
            ICacheManager cacheManager)
        {
            _roleRepository = roleRepository;
            _authorizeRepository = authorizeRepository;
            _userRepository = userRepository;
            _sysConfigRepository = sysConfigRepository;
            _organizeRepository = organizeRepository;
            _positionRepository = positionRepository;
            _moduleDataAuthorizeSchemeRepository = moduleDataAuthorizeSchemeRepository;
            _organizeAdministratorRepository = organizeAdministratorRepository;
            _cacheManager = cacheManager;
            _httpContext = App.HttpContext;
        }

        /// <summary>
        /// 用户信息
        /// </summary>
        public UserEntity User
        {
            get => _userRepository.GetSingle(u => u.Id == UserId);
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId
        {
            get => _httpContext.User.FindFirst(ClaimConst.CLAINM_USERID)?.Value;
        }

        /// <summary>
        /// 用户账号
        /// </summary>
        public string Account
        {
            get => _httpContext.User.FindFirst(ClaimConst.CLAINM_ACCOUNT)?.Value;
        }

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string RealName
        {
            get => _httpContext.User.FindFirst(ClaimConst.CLAINM_REALNAME)?.Value;
        }

        /// <summary>
        /// 当前用户 token
        /// </summary>
        public string ToKen
        {
            get => _httpContext?.Request.Headers["Authorization"];
        }

        /// <summary>
        /// 租户ID
        /// </summary>
        public string TenantId
        {
            get => _httpContext.User.FindFirst(ClaimConst.TENANT_ID)?.Value;
        }

        /// <summary>
        /// 租户数据库名称
        /// </summary>
        public string TenantDbName
        {
            get => _httpContext.User.FindFirst(ClaimConst.TENANT_DB_NAME)?.Value;
        }

        /// <summary>
        /// 是否是管理员
        /// </summary>
        public bool IsAdministrator
        {
            get => _httpContext.User.FindFirst(ClaimConst.CLAINM_ADMINISTRATOR)?.Value == ((int)Enum.AccountType.Administrator).ToString();
        }

        /// <summary>
        /// 获取用户登录信息
        /// </summary>
        /// <returns></returns>
        public async Task<UserInfo> GetUserInfo()
        {
            var data = new UserInfo();
            var clent = Parser.GetDefault().Parse(_httpContext.Request.Headers["User-Agent"]);
            var ipAddress = NetUtil.Ip;
            var ipAddressName = await NetUtil.GetLocation(ipAddress);
            var defaultPortalId = string.Empty;
            var userDataScope = await GetUserDataScope(UserId);
            var sysConfigInfo = await _sysConfigRepository.GetFirstAsync(s => s.Category.Equals("SysConfig") && s.Key.ToLower().Equals("tokentimeout"));
            data = await _userRepository.AsSugarClient().Queryable<UserEntity>().Where(a => a.Id == UserId)
               .Select(a => new UserInfo
               {
                   userId = a.Id,
                   headIcon = SqlFunc.MergeString("/api/File/Image/userAvatar/", a.HeadIcon),
                   userAccount = a.Account,
                   userName = a.RealName,
                   gender = a.Gender,
                   organizeId = a.OrganizeId,
                   departmentId=a.OrganizeId,
                   departmentName = SqlFunc.Subqueryable<OrganizeEntity>().Where(o => o.Id == SqlFunc.ToString(a.OrganizeId)).Select(o => o.FullName),
                   organizeName = SqlFunc.Subqueryable<OrganizeEntity>().Where(o => o.Id == SqlFunc.ToString(a.OrganizeId)).Select(o => o.OrganizeIdTree),
                   managerId = a.ManagerId,
                   isAdministrator = SqlFunc.IIF(a.IsAdministrator == 1, true, false),
                   portalId = SqlFunc.IIF(a.PortalId == null, defaultPortalId, a.PortalId),
                   positionId = a.PositionId,
                   roleId = a.RoleId,
                   prevLoginTime = a.PrevLogTime,
                   prevLoginIPAddress = a.PrevLogIP,
                   landline = a.Landline,
                   telePhone = a.TelePhone,
                   manager= SqlFunc.Subqueryable<UserEntity>().Where(u => u.Id == a.ManagerId).Select(u => SqlFunc.MergeString(u.RealName , "/" , u.Account)),
                   mobilePhone =a.MobilePhone,
                   email=a.Email,
                   birthday=a.Birthday
               }).FirstAsync();
            if(data!=null && data.organizeName.IsNotEmptyOrNull())
            {

                var orgIdTree = data?.organizeName?.Split(',');
                var organizeName = await _organizeRepository.AsQueryable().Where(x => orgIdTree.Contains(x.Id)).OrderBy(x => x.SortCode).OrderBy(x => x.CreatorTime).Select(x => x.FullName).ToListAsync();
                data.departmentName = string.Join("/", organizeName);
                data.organizeName = data.departmentName;
            }
            data.organizeName = data.departmentName;
            data.loginTime = DateTime.Now;
            data.prevLogin = (await _sysConfigRepository.GetFirstAsync(x => x.Category.Equals("SysConfig") && x.Key.ToLower().Equals("lastlogintimeswitch"))).Value.ToInt();
            data.loginIPAddress = ipAddress;
            data.loginIPAddressName = ipAddressName;
            data.prevLoginIPAddressName = await NetUtil.GetLocation(data.prevLoginIPAddress);
            data.loginPlatForm = clent.String;
            data.subsidiary = await this.GetSubsidiary(data.organizeId, data.isAdministrator);
            data.subordinates = await this.GetSubordinates(UserId);
            data.positionIds = data.positionId == null ? null : await GetPosition(data.positionId);
            data.positionName = data.positionIds == null ? null : string.Join(",", data.positionIds.Select(it => it.name));
            var roleList = await GetUserOrgRoleIds(data.roleId, data.organizeId);
            data.roleName = await GetRoleNameByIds(string.Join(",", roleList));
            data.roleIds = roleList.ToArray();
            if (!data.isAdministrator && data.roleIds.Any())
            {
                var portalIds = await _authorizeRepository.AsQueryable().In(a => a.ObjectId, data.roleIds).Where(a => a.ItemType == "portal").GroupBy(it => new { it.ItemId }).Select(it => it.ItemId).ToListAsync();
                if (portalIds.Any())
                {
                    if (!portalIds.Any(x => x == data.portalId)) data.portalId = portalIds.FirstOrDefault()?.ToString();
                }
                else data.portalId = string.Empty;
            }
            data.overdueTime = TimeSpan.FromMinutes(sysConfigInfo.Value.ToDouble());
            data.dataScope = userDataScope;
            data.tenantId = TenantId;
            data.tenantDbName = _httpContext.User.FindFirst(ClaimConst.TENANT_DB_NAME)?.Value;
            //根据系统配置过期时间自动过期
            await _cacheManager.SetUserInfo(TenantId + "_" + UserId, data, TimeSpan.FromMinutes(sysConfigInfo.Value.ToDouble()));
            return data;
        }

        /// <summary>
        /// 获取用户数据范围
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task<List<UserDataScope>> GetUserDataScope(string userId)
        {
            List<UserDataScope> data = new List<UserDataScope>();
            List<UserDataScope> subData = new List<UserDataScope>();
            List<UserDataScope> inteList = new List<UserDataScope>();
            var list = await _organizeAdministratorRepository.GetListAsync(it => SqlFunc.ToString(it.UserId) == userId && it.DeleteMark == null);
            //填充数据
            foreach (var item in list)
            {
                if (item.SubLayerAdd.ToBool() || item.SubLayerEdit.ToBool() || item.SubLayerDelete.ToBool())
                {
                    var subsidiary = (await GetSubsidiary(item.OrganizeId, false)).ToList();
                    subsidiary.Remove(item.OrganizeId);
                    subsidiary.ToList().ForEach(it =>
                    {
                        subData.Add(new UserDataScope()
                        {
                            organizeId = it,
                            Add = item.SubLayerAdd.ToBool(),
                            Edit = item.SubLayerEdit.ToBool(),
                            Delete = item.SubLayerDelete.ToBool()
                        });
                    });
                }
                if (item.ThisLayerAdd.ToBool() || item.ThisLayerEdit.ToBool() || item.ThisLayerDelete.ToBool())
                {
                    data.Add(new UserDataScope()
                    {
                        organizeId = item.OrganizeId,
                        Add = item.ThisLayerAdd.ToBool(),
                        Edit = item.ThisLayerEdit.ToBool(),
                        Delete = item.ThisLayerDelete.ToBool()
                    });
                }
            }
            //比较数据
            //所有分级数据权限以本级权限为主 子级为辅
            //将本级数据与子级数据对比 对比出子级数据内组织ID存在本级数据的组织ID
            var intersection = data.Select(it => it.organizeId).Intersect(subData.Select(it => it.organizeId)).ToList();
            intersection.ForEach(it =>
            {
                var parent = data.Find(item => item.organizeId == it);
                var child = subData.Find(item => item.organizeId == it);
                var add = false;
                var edit = false;
                var delete = false;
                if (parent.Add || child.Add)
                {
                    add = true;
                }
                if (parent.Edit || child.Edit)
                {
                    edit = true;
                }
                if (parent.Delete || child.Delete)
                {
                    delete = true;
                }
                inteList.Add(new UserDataScope()
                {
                    organizeId = it,
                    Add = add,
                    Edit = edit,
                    Delete = delete
                });
                data.Remove(parent);
                subData.Remove(child);
            });
            return data.Union(subData).Union(inteList).ToList();
        }

        /// <summary>
        /// 获取数据条件
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="moduleId">模块ID</param>
        /// <returns></returns>
        public async Task<List<IConditionalModel>> GetConditionAsync<T>(string moduleId, string primaryKey = "F_Id", bool isDataPermissions = true) where T : new()
        {
            var userInfo = await GetUserInfo();
            var conModels = new List<IConditionalModel>();
            if (this.IsAdministrator)
                return conModels;
            var items = await _authorizeRepository.AsSugarClient().Queryable<AuthorizeEntity, RoleEntity>((a, b) => new JoinQueryInfos(JoinType.Left, b.Id == a.ObjectId && b.EnabledMark == 1 && b.DeleteMark == null))
                       .In((a, b) => b.Id, userInfo.roleIds)
                       .Where(a => a.ItemType == "resource")
                       .GroupBy(a => new { a.ItemId }).Select(a => a.ItemId).ToListAsync();
            if (items.Count == 0 && isDataPermissions == true)
            {
                conModels.Add(new ConditionalCollections()
                {
                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                    {
                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.And, new ConditionalModel() { FieldName = primaryKey, ConditionalType = ConditionalType.Equal, FieldValue = "0", FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(string)) })
                    }
                });
                return conModels;
            }
            var resourceList = _moduleDataAuthorizeSchemeRepository.AsQueryable().In(it => it.Id, items).Where(it => it.ModuleId == moduleId && it.DeleteMark == null).ToList();
            foreach (var item in resourceList)
            {
                var conditionModelList = JsonHelper.ToList<AuthorizeModuleResourceConditionModel>(item.ConditionJson);
                foreach (var conditionItem in conditionModelList)
                {
                    foreach (var fieldItem in conditionItem.Groups)
                    {
                        var itemField = fieldItem.Field.Replace("F_", "").Replace("f_", "").LowerFirstChar();
                        var itemValue = fieldItem.Value;
                        var itemMethod = (SearchMethod)Enums.Parse(typeof(SearchMethod), fieldItem.Op);
                        switch (itemValue.ToString())
                        {
                            //当前用户
                            case "@userId":
                                {
                                    switch (conditionItem.Logic)
                                    {
                                        case "and":
                                            conModels.Add(new ConditionalCollections()
                                            {
                                                ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>() {
                                                    new KeyValuePair<WhereType, ConditionalModel>(WhereType.And, GetConditionalModel(itemMethod, itemField, userInfo.userId))
                                                }
                                            });
                                            break;
                                        case "or":
                                            conModels.Add(new ConditionalCollections()
                                            {
                                                ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>() {
                                                    new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, GetConditionalModel(itemMethod, itemField, userInfo.userId))
                                                }
                                            });
                                            break;
                                    }
                                }
                                break;
                            //当前用户集下属
                            case "@userAraSubordinates":
                                {
                                    switch (conditionItem.Logic)
                                    {
                                        case "and":
                                            conModels.Add(new ConditionalCollections()
                                            {
                                                ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>() {
                                                   new KeyValuePair<WhereType, ConditionalModel>(WhereType.And, GetConditionalModel(itemMethod, itemField, userInfo.userId)),
                                                   new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, GetConditionalModel(SearchMethod.In, itemField, string.Join(",", userInfo.subordinates)))
                                                }
                                            });
                                            break;
                                        case "or":
                                            conModels.Add(new ConditionalCollections()
                                            {
                                                ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>() {
                                                   new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, GetConditionalModel(itemMethod, itemField, userInfo.userId)),
                                                   new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, GetConditionalModel(SearchMethod.In, itemField, string.Join(",", userInfo.subordinates)))
                                                }
                                            });
                                            break;
                                    }
                                }
                                break;
                            //当前组织
                            case "@organizeId":
                                {
                                    if (!string.IsNullOrEmpty(userInfo.organizeId))
                                        switch (conditionItem.Logic)
                                        {
                                            case "and":
                                                conModels.Add(new ConditionalCollections()
                                                {
                                                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                                                    {
                                                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.And, GetConditionalModel(itemMethod, itemField, userInfo.organizeId))
                                                    }
                                                });
                                                break;
                                            case "or":
                                                conModels.Add(new ConditionalCollections()
                                                {
                                                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                                                    {
                                                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, GetConditionalModel(itemMethod, itemField, userInfo.organizeId))
                                                    }
                                                });
                                                break;
                                        }
                                }
                                break;
                            //当前组织及子组织
                            case "@organizationAndSuborganization":
                                {
                                    if (!string.IsNullOrEmpty(userInfo.organizeId))
                                        switch (conditionItem.Logic)
                                        {
                                            case "and":
                                                conModels.Add(new ConditionalCollections()
                                                {
                                                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                                                    {
                                                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.And, GetConditionalModel(itemMethod, itemField, userInfo.organizeId)),
                                                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, GetConditionalModel(SearchMethod.In, itemField, string.Join(",", userInfo.subsidiary)))
                                                    }
                                                });
                                                break;
                                            case "or":
                                                conModels.Add(new ConditionalCollections()
                                                {
                                                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                                                    {
                                                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, GetConditionalModel(itemMethod, itemField, userInfo.organizeId)),
                                                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, GetConditionalModel(SearchMethod.In, itemField, string.Join(",", userInfo.subsidiary)))
                                                    }
                                                });
                                                break;
                                        }
                                }
                                break;
                            default:
                                {
                                    if (!string.IsNullOrEmpty(itemValue))
                                        switch (conditionItem.Logic)
                                        {
                                            case "and":
                                                conModels.Add(new ConditionalCollections()
                                                {
                                                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                                                    {
                                                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.And, GetConditionalModel(itemMethod, itemField, itemValue, fieldItem.Type))
                                                    }
                                                });
                                                break;
                                            case "or":
                                                conModels.Add(new ConditionalCollections()
                                                {
                                                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                                                    {
                                                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, GetConditionalModel(itemMethod, itemField, itemValue, fieldItem.Type))
                                                    }
                                                });
                                                break;
                                        }
                                }
                                break;
                        }
                    }
                }
            }
            if (resourceList.Count == 0)
            {
                conModels.Add(new ConditionalCollections()
                {
                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                    {
                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.And, new ConditionalModel() { FieldName = primaryKey, ConditionalType = ConditionalType.Equal, FieldValue = "0", FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(string)) })
                    }
                });
            }
            return conModels;
        }

        /// <summary>
        /// 获取数据条件(在线开发专用)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public List<IConditionalModel> GetCondition<T>(string primaryKey, string moduleId, bool isDataPermissions = true) where T : new()
        {
            var userInfo = GetUserInfo().Result;
            var conModels = new List<IConditionalModel>();
            if (this.IsAdministrator)
                return conModels;

            var items = _authorizeRepository.AsSugarClient().Queryable<AuthorizeEntity, RoleEntity>((a, b) => new JoinQueryInfos(JoinType.Left, b.Id == a.ObjectId && b.EnabledMark == 1 && b.DeleteMark == null))
                       .In((a, b) => b.Id, userInfo.roleIds)
                       .Where(a => a.ItemType == "resource")
                       .GroupBy(a => new { a.ItemId }).Select(a => a.ItemId).ToList();

            if (isDataPermissions == false)
            {
                conModels.Add(new ConditionalCollections()
                {
                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                    {
                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.And, new ConditionalModel() { FieldName = primaryKey, ConditionalType = ConditionalType.NoEqual, FieldValue = "0", FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(string)) })
                    }
                });
                return conModels;
            }
            else if (items.Count == 0 && isDataPermissions == true)
            {
                conModels.Add(new ConditionalCollections()
                {
                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                    {
                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.And, new ConditionalModel() { FieldName = primaryKey, ConditionalType = ConditionalType.Equal, FieldValue = "0", FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(string)) })
                    }
                });
                return conModels;
            }

            var resourceList = _moduleDataAuthorizeSchemeRepository.AsQueryable().In(it => it.Id, items).Where(it => it.ModuleId == moduleId && it.DeleteMark == null).ToList();

            //方案和方案，分组和分组 之间 必须要 用 And 条件 拼接
            var isAnd = false;

            foreach (var item in resourceList)
            {
                var conditionModelList = item.ConditionJson.ToList<AuthorizeModuleResourceConditionModel>();
                foreach (var conditionItem in conditionModelList)
                {
                    foreach (var fieldItem in conditionItem.Groups)
                    {
                        var itemField = fieldItem.Field;
                        var itemValue = fieldItem.Value;
                        var itemMethod = (SearchMethod)Enums.Parse(typeof(SearchMethod), fieldItem.Op);

                        switch (itemValue.ToString())
                        {
                            //当前用户
                            case "@userId":
                                {
                                    switch (conditionItem.Logic)
                                    {
                                        case "and":
                                            conModels.Add(new ConditionalCollections()
                                            {
                                                ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>() {
                                                    new KeyValuePair<WhereType, ConditionalModel>(WhereType.And, GetConditionalModel(itemMethod, itemField, userInfo.userId))
                                                }
                                            });
                                            break;
                                        case "or":
                                            conModels.Add(new ConditionalCollections()
                                            {
                                                ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>() {
                                                    new KeyValuePair<WhereType, ConditionalModel>(isAnd ? WhereType.And : WhereType.Or, GetConditionalModel(itemMethod, itemField, userInfo.userId))
                                                }
                                            });
                                            break;
                                    }
                                }
                                break;
                            //当前用户集下属
                            case "@userAraSubordinates":
                                {
                                    switch (conditionItem.Logic)
                                    {
                                        case "and":
                                            conModels.Add(new ConditionalCollections()
                                            {
                                                ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>() {
                                                   new KeyValuePair<WhereType, ConditionalModel>(WhereType.And, GetConditionalModel(itemMethod, itemField, userInfo.userId)),
                                                   new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, GetConditionalModel(SearchMethod.In, itemField, string.Join(",", userInfo.subordinates)))
                                                }
                                            });
                                            break;
                                        case "or":
                                            conModels.Add(new ConditionalCollections()
                                            {
                                                ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>() {
                                                   new KeyValuePair<WhereType, ConditionalModel>(isAnd ? WhereType.And : WhereType.Or, GetConditionalModel(itemMethod, itemField, userInfo.userId)),
                                                   new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, GetConditionalModel(SearchMethod.In, itemField, string.Join(",", userInfo.subordinates)))
                                                }
                                            });
                                            break;
                                    }
                                }
                                break;
                            //当前组织
                            case "@organizeId":
                                {
                                    if (!string.IsNullOrEmpty(userInfo.organizeId))
                                        switch (conditionItem.Logic)
                                        {
                                            case "and":
                                                conModels.Add(new ConditionalCollections()
                                                {
                                                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                                                    {
                                                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.And, GetConditionalModel(itemMethod, itemField, userInfo.organizeId))
                                                    }
                                                });
                                                break;
                                            case "or":
                                                conModels.Add(new ConditionalCollections()
                                                {
                                                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                                                    {
                                                        new KeyValuePair<WhereType, ConditionalModel>(isAnd ? WhereType.And : WhereType.Or, GetConditionalModel(itemMethod, itemField, userInfo.organizeId))
                                                    }
                                                });
                                                break;
                                        }
                                }
                                break;
                            //当前组织及子组织
                            case "@organizationAndSuborganization":
                                {
                                    if (!string.IsNullOrEmpty(userInfo.organizeId))
                                        switch (conditionItem.Logic)
                                        {
                                            case "and":
                                                conModels.Add(new ConditionalCollections()
                                                {
                                                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                                                    {
                                                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.And, GetConditionalModel(itemMethod, itemField, userInfo.organizeId)),
                                                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, GetConditionalModel(SearchMethod.In, itemField, string.Join(",", userInfo.subsidiary)))
                                                    }
                                                });
                                                break;
                                            case "or":
                                                conModels.Add(new ConditionalCollections()
                                                {
                                                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                                                    {
                                                        new KeyValuePair<WhereType, ConditionalModel>(isAnd ? WhereType.And : WhereType.Or, GetConditionalModel(itemMethod, itemField, userInfo.organizeId)),
                                                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, GetConditionalModel(SearchMethod.In, itemField, string.Join(",", userInfo.subsidiary)))
                                                    }
                                                });
                                                break;
                                        }
                                }
                                break;
                            default:
                                {
                                    if (!string.IsNullOrEmpty(itemValue))
                                        switch (conditionItem.Logic)
                                        {
                                            case "and":
                                                conModels.Add(new ConditionalCollections()
                                                {
                                                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                                                    {
                                                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.And, GetConditionalModel(itemMethod, itemField, itemValue, fieldItem.Type))
                                                    }
                                                });
                                                break;
                                            case "or":
                                                conModels.Add(new ConditionalCollections()
                                                {
                                                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                                                    {
                                                        new KeyValuePair<WhereType, ConditionalModel>(isAnd ? WhereType.And : WhereType.Or, GetConditionalModel(itemMethod, itemField, itemValue, fieldItem.Type))
                                                    }
                                                });
                                                break;
                                        }
                                }
                                break;
                        }
                        isAnd = false;
                    }
                    isAnd = true;//分组和分组
                }
                isAnd = true;//方案和方案
            }
            if (resourceList.Count == 0)
            {
                conModels.Add(new ConditionalCollections()
                {
                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                    {
                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.And, new ConditionalModel() { FieldName = primaryKey, ConditionalType = ConditionalType.Equal, FieldValue = "0", FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(string)) })
                    }
                });
            }
            return conModels;
        }

        /// <summary>
        /// 下属机构
        /// </summary>
        /// <param name="organizeId">机构ID</param>
        /// <param name="isAdmin">是否管理员</param>
        /// <returns></returns>
        private async Task<string[]> GetSubsidiary(string organizeId, bool isAdmin)
        {
            var data = await _organizeRepository.GetListAsync(o => o.DeleteMark == null && o.EnabledMark == 1);
            if (!isAdmin)
            {
                data = data.TreeChildNode(organizeId, t => t.Id, t => t.ParentId);
            }
            return data.Select(m => m.Id).ToArray();
        }

        /// <summary>
        /// 获取下属
        /// </summary>
        /// <param name="managerId">主管Id</param>
        /// <returns></returns>
        private async Task<string[]> GetSubordinates(string managerId)
        {
            List<string> data = new List<string>();
            var userIds = await _userRepository.AsSugarClient().Queryable<UserEntity>().Where(m => m.ManagerId == managerId && m.DeleteMark == null).Select(m => m.Id).ToListAsync();
            data.AddRange(userIds);
            //关闭无限级我的下属
            //data.AddRange(await GetInfiniteSubordinats(userIds.ToArray()));
            return data.ToArray();
        }

        /// <summary>
        /// 获取下属无限极
        /// </summary>
        /// <param name="parentIds"></param>
        /// <returns></returns>
        private async Task<List<string>> GetInfiniteSubordinats(string[] parentIds)
        {
            List<string> data = new List<string>();
            if (parentIds.ToList().Count > 0)
            {
                var userIds = await _userRepository.AsSugarClient().Queryable<UserEntity>().In(it => it.ManagerId, parentIds).Where(it => it.DeleteMark == null).OrderBy(it => it.SortCode).Select(it => it.Id).ToListAsync();
                data.AddRange(userIds);
                data.AddRange(await GetInfiniteSubordinats(userIds.ToArray()));
            }
            return data;
        }

        /// <summary>
        /// 获取当前用户岗位信息
        /// </summary>
        /// <param name="PositionIds"></param>
        /// <returns></returns>
        private async Task<List<PositionInfo>> GetPosition(string PositionIds)
        {
            var ids = PositionIds.Split(",");
            return await _positionRepository.AsSugarClient().Queryable<PositionEntity>().In(it => it.Id, ids).Select(it => new PositionInfo { id = it.Id, name = it.FullName }).ToListAsync();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private ConditionalModel GetConditionalModel(SearchMethod expressType, string fieldName, string fieldValue, string dataType = "string")
        {
            switch (expressType)
            {
                //like
                case SearchMethod.Contains:
                    return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.Like, FieldValue = fieldValue };
                //等于
                case SearchMethod.Equal:
                    switch (dataType)
                    {
                        case "Double":
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.Equal, FieldValue = fieldValue, FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(double)) };
                        case "Int32":
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.Equal, FieldValue = fieldValue, FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(int)) };
                        default:
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.Equal, FieldValue = fieldValue };
                    }
                //不等于
                case SearchMethod.NotEqual:
                    switch (dataType)
                    {
                        case "Double":
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.NoEqual, FieldValue = fieldValue, FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(double)) };
                        case "Int32":
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.NoEqual, FieldValue = fieldValue, FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(int)) };
                        default:
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.NoEqual, FieldValue = fieldValue };
                    }
                //小于
                case SearchMethod.LessThan:
                    switch (dataType)
                    {
                        case "Double":
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.LessThan, FieldValue = fieldValue, FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(double)) };
                        case "Int32":
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.LessThan, FieldValue = fieldValue, FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(int)) };
                        default:
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.LessThan, FieldValue = fieldValue };
                    }
                //小于等于
                case SearchMethod.LessThanOrEqual:
                    switch (dataType)
                    {
                        case "Double":
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = fieldValue, FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(double)) };
                        case "Int32":
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = fieldValue, FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(int)) };
                        default:
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = fieldValue };
                    }
                //大于
                case SearchMethod.GreaterThan:
                    switch (dataType)
                    {
                        case "Double":
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.GreaterThan, FieldValue = fieldValue, FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(double)) };
                        case "Int32":
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.GreaterThan, FieldValue = fieldValue, FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(int)) };
                        default:
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.GreaterThan, FieldValue = fieldValue };
                    }
                //大于等于
                case SearchMethod.GreaterThanOrEqual:
                    switch (dataType)
                    {
                        case "Double":
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = fieldValue, FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(double)) };
                        case "Int32":
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = fieldValue, FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(int)) };
                        default:
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = fieldValue };
                    }
                //包含
                case SearchMethod.In:
                    return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.In, FieldValue = fieldValue };
            }
            return new ConditionalModel();
        }

        /// <summary>
        /// 获取角色名称 根据 角色Ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<string> GetRoleNameByIds(string ids)
        {
            if (ids.IsNullOrEmpty())
            {
                return "";
            }
            var idList = ids.Split(",").ToList();
            var nameList = new List<string>();
            var roleList = await _roleRepository.AsQueryable().Where(x => x.DeleteMark == null && x.EnabledMark == 1).ToListAsync();
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

        /// <summary>
        /// 根据角色Ids和组织Id 获取组织下的角色以及全局角色
        /// </summary>
        /// <param name="roleIds">角色Id集合</param>
        /// <param name="organizeId">组织Id</param>
        /// <returns></returns>
        public async Task<List<string>> GetUserOrgRoleIds(string roleIds, string organizeId)
        {
            if (roleIds.IsNotEmptyOrNull())
            {
                var userRoleIds = roleIds.Split(",");

                //当前组织下的角色Id 集合
                var roleList = await _roleRepository.AsSugarClient().Queryable<OrganizeRelationEntity>()
                    .Where(x => x.OrganizeId == organizeId && x.ObjectType == "Role" && userRoleIds.Contains(x.ObjectId)).Select(x => x.ObjectId).ToListAsync();

                //全局角色Id 集合
                var gRoleList = await _roleRepository.AsQueryable().Where(x => userRoleIds.Contains(x.Id) && x.GlobalMark == 1)
                    .Where(r => r.EnabledMark == 1 && r.DeleteMark == null).Select(x => x.Id).ToListAsync();

                roleList.AddRange(gRoleList);//组织角色 + 全局角色

                return roleList;
            }
            else return new List<string>();
        }
    }
}
