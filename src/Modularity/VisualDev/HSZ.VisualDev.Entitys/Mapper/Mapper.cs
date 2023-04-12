using HSZ.VisualDev.Entitys.Dto.VisualDev;
using Mapster;

namespace HSZ.VisualDev.Entitys.Mapper
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
            config.ForType<VisualDevEntity, VisualDevSelectorOutput>()
                .Map(dest => dest.parentId, src => src.Category);
        }
    }
}
