using HSZ.Apps.Entitys.Dto;
using HSZ.System.Entitys.Permission;
using Mapster;

namespace HSZ.Apps.Entitys.Mapper
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    class Mapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<UserEntity, AppUserOutput>()
                .Map(dest => dest.userId, src => src.Id)
                .Map(dest => dest.userAccount, src => src.Account)
                .Map(dest => dest.userName, src => src.RealName)
                .Map(dest => dest.headIcon, src => "/api/File/Image/userAvatar/" + src.HeadIcon);
            config.ForType<UserEntity, AppUserInfoOutput>()
                .Map(dest => dest.headIcon, src => "/api/File/Image/userAvatar/" + src.HeadIcon);
        }
    }
}
