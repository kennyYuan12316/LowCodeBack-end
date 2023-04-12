using HSZ.Message.Entitys.Dto.IM;
using HSZ.Message.Entitys.Model.IM;
using Mapster;

namespace HSZ.Message.Entitys.Mapper
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    public class Mapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<IMContentEntity, IMUnreadNumModel>()
                .Map(dest => dest.unreadNum, src => src.State);
            config.ForType<UserOnlineModel, OnlineUserListOutput>()
              .Map(dest => dest.userId, src => src.userId)
              .Map(dest => dest.userAccount, src => src.account)
              .Map(dest => dest.userName, src => src.userName)
              .Map(dest => dest.loginTime, src => src.lastTime)
              .Map(dest => dest.loginIPAddress, src => src.lastLoginIp)
              .Map(dest => dest.loginPlatForm, src => src.lastLoginPlatForm);
        }
    }
}
