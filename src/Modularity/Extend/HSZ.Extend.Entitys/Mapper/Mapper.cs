using HSZ.Expand.Thirdparty.Email.Model;
using HSZ.Extend.Entitys.Dto.Email;
using HSZ.Extend.Entitys.Dto.Order;
using Mapster;

namespace HSZ.Extend.Entitys.Mapper
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
            config.ForType<EmailReceiveEntity, EmailHomeOutput>()
                .Map(dest => dest.fullName, src => src.Subject);
            config.ForType<EmailReceiveEntity, EmailListOutput>()
                .Map(dest => dest.fdate, src => src.Date)
                .Map(dest => dest.sender, src => src.SenderName)
                .Map(dest => dest.isRead, src => src.Read);
            config.ForType<EmailSendEntity, EmailListOutput>()
                .Map(dest => dest.recipient, src => src.To);
            config.ForType<EmailConfigEntity, EmailConfigInfoOutput>()
                .Map(dest => dest.emailSsl, src => src.Ssl);
            config.ForType<EmailReceiveEntity, EmailInfoOutput>()
                .Map(dest => dest.recipient, src => src.MAccount)
                .Map(dest => dest.fdate, src => src.Date);
            config.ForType<EmailSendEntity, EmailInfoOutput>()
                .Map(dest => dest.recipient, src => src.To);
            config.ForType<OrderReceivableEntity, OrderCollectionPlanOutput>()
                .Map(dest => dest.fabstract, src => src.Abstract)
                .Map(dest => dest.receivableMoney, src => src.ReceivableMoney.ToString());
            config.ForType<EmailConfigUpInput,EmailConfigEntity>()
               .Map(dest => dest.Ssl, src => src.emailSsl);
            config.ForType<EmailConfigActionsCheckMailInput, EmailConfigEntity>()
               .Map(dest => dest.Ssl, src => src.emailSsl);
            config.ForType<EmailConfigEntity, MailAccount>()
               .Map(dest => dest.Ssl, src => src.Ssl!=null&& src.Ssl==1?true:false);
        }
    }
}
