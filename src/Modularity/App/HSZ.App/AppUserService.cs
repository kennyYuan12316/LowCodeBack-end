using HSZ.Apps.Entitys.Dto;
using HSZ.Common.Core.Manager;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.System.Interfaces.Permission;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HSZ.Apps
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：App用户信息
    /// </summary>
    [ApiDescriptionSettings(Tag = "App", Name = "User", Order = 800)]
    [Route("api/App/[controller]")]
    public class AppUserService : IDynamicApiController, ITransient
    {
        private readonly IUsersService _usersService;
        private readonly IDepartmentService _departmentService;
        private readonly IPositionService _positionService;
        private readonly IUserManager _userManager;
        /// <summary>
        ///
        /// </summary>
        /// <param name="usersService"></param>
        /// <param name="departmentService"></param>
        /// <param name="roleService"></param>
        /// <param name="positionService"></param>
        /// <param name="userManager"></param>
        public AppUserService(IUsersService usersService, IDepartmentService departmentService, IPositionService positionService, IUserManager userManager)
        {
            _usersService = usersService;
            _departmentService = departmentService;
            _positionService = positionService;
            _userManager = userManager;
        }

        #region Get
        /// <summary>
        /// 用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetInfo()
        {
            var userEntity = _usersService.GetInfoByUserId(_userManager.UserId);
            var appUserInfo = userEntity.Adapt<AppUserOutput>();
            appUserInfo.positionIds = userEntity.PositionId == null ? null : await _usersService.GetPosition(userEntity.PositionId);
            appUserInfo.departmentName = _departmentService.GetOrganizeNameTree(userEntity.OrganizeId);
            appUserInfo.organizeId = _departmentService.GetCompanyId(userEntity.OrganizeId);
            appUserInfo.organizeName = appUserInfo.departmentName;
            //获取当前组织角色和全局角色
            var roleList= await _userManager.GetUserOrgRoleIds(userEntity.RoleId,userEntity.OrganizeId);
            appUserInfo.roleName = await _userManager.GetRoleNameByIds(string.Join(",", roleList));
            appUserInfo.manager = await _usersService.GetUserName(userEntity.ManagerId);
            return appUserInfo;
        }

        /// <summary>
        /// 用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public dynamic GetInfo(string id)
        {
            var userEntity = _usersService.GetInfoByUserId(id);
            var appUserInfo = userEntity.Adapt<AppUserInfoOutput>();
            appUserInfo.organizeName = _departmentService.GetDepName(userEntity.OrganizeId);
            appUserInfo.positionName = _positionService.GetName(userEntity.PositionId);
            return appUserInfo;
        }
        #endregion

    }
}
