using HSZ.System.Entitys.Dto.Permission.Department;
using HSZ.System.Entitys.Dto.Permission.Organize;
using HSZ.System.Entitys.Dto.Permission.OrganizeAdministrator;
using HSZ.System.Entitys.Dto.Permission.User;
using HSZ.System.Entitys.Model.Permission.Organize;
using HSZ.System.Entitys.Model.Permission.User;
using HSZ.System.Entitys.Permission;
using Mapster;

namespace HSZ.System.Entitys.Mapper
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    public class PermissionMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<UserEntity, UserInfo>()
                .Map(dest => dest.userId, src => src.Id)
                .Map(dest => dest.userAccount, src => src.Account)
                .Map(dest => dest.userName, src => src.RealName)
                .Map(dest => dest.headIcon, src => "/api/File/Image/userAvatar/" + src.HeadIcon)
                .Map(dest => dest.prevLoginTime, src => src.PrevLogTime)
                .Map(dest => dest.prevLoginIPAddress, src => src.PrevLogIP);
            config.ForType<UserEntity, UserInfoOutput>()
                 .Map(dest => dest.headIcon, src => "/api/File/Image/userAvatar/" + src.HeadIcon);
            config.ForType<UserEntity, UserSelectorOutput>()
                .Map(dest => dest.fullName, src => src.RealName + "/" + src.Account)
                .Map(dest => dest.type, src => "user")
                .Map(dest => dest.parentId, src => src.OrganizeId);
            config.ForType<OrganizeEntity, UserSelectorOutput>()
                .Map(dest => dest.type, src => src.Category)
                .Map(dest => dest.icon, src => "icon-sz icon-sz-tree-organization3");
            config.ForType<OrganizeEntity, DepartmentSelectorOutput>()
                 .Map(dest => dest.type, src => src.Category);
            config.ForType<OrganizeAdminIsTratorCrInput, OrganizeAdministratorEntity>()
                .Ignore(dest => dest.UserId);
        }
    }
}
