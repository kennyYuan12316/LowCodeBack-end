using HSZ.Tenant.Entitys.Model;
using Mapster;

namespace HSZ.Tenant.Entitys.Mapper
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
            config.ForType<DynamicDbTableModel, DbTableModel>()
                .Map(dest => dest.table, src => src.F_TABLE)
                .Map(dest => dest.tableName, src => src.F_TABLENAME)
                .Map(dest => dest.size, src => src.F_SIZE)
                .Map(dest => dest.sum, src => int.Parse(src.F_SUM))
                .Map(dest => dest.primaryKey, src => src.F_PRIMARYKEY);
        }
    }
}
