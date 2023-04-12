using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSZ.Common.DI
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class WareDIAttribute:Attribute
    {
        public WareDIAttribute(DIType dIType)
        {
            this.DIType = dIType;
            this.WareType = WareType.Default;
        }
        public WareDIAttribute()
        {
            DIType = DIType.OTHER;
            this.WareType = WareType.Default;
        }

        /// <summary>
        /// 约定
        /// </summary>
        /// <param name="contract"></param>
        public WareDIAttribute(WareType wareType)
        {
            this.WareType = wareType;
            this.DIType = DIType.OTHER;
        }

        public WareDIAttribute(DIType dIType, WareType wareType)
        {
            this.WareType = wareType;
            this.DIType = dIType;
        }

        /// <summary>
        /// DI生命周期
        /// </summary>
        public DIType DIType { get; }

        /// <summary>
        /// 约定库
        /// </summary>
        public WareType WareType { get; }

    }

    /// <summary>
    /// 注入类型选择
    /// </summary>
    public enum DIType
    {
        /// <summary>
        /// 单例模式
        /// </summary>
        SINGLETON,


        /// <summary>
        /// 其他模式
        /// </summary>
        OTHER
    }


    public enum WareType
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default,

        /// <summary>
        /// 原材料库
        /// </summary>
        RawMaterial,

        /// <summary>
        /// 小极卷库
        /// </summary>
        SmallCoil,

        /// <summary>
        /// 大极卷库
        /// </summary>
        LargeCoil,

        /// <summary>
        /// 结构件库
        /// </summary>
        FrameMember,

        /// <summary>
        /// 成品库
        /// </summary>
        Production
    }
}
