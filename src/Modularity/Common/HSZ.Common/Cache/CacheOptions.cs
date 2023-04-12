using HSZ.ConfigurableOptions;

namespace HSZ.Common.Cache
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：缓存配置
    /// </summary>
    public class CacheOptions : IConfigurableOptions
    {
        /// <summary>
        /// 缓存类型
        /// </summary>
        public CacheType CacheType { get; set; }

        /// <summary>
        /// Redis配置
        /// </summary>
        public string RedisConnectionString { get; set; }
    }

    public enum CacheType
    {
        /// <summary>
        /// 内存缓存
        /// </summary>
        MemoryCache,

        /// <summary>
        /// Redis缓存
        /// </summary>
        RedisCache
    }
}
