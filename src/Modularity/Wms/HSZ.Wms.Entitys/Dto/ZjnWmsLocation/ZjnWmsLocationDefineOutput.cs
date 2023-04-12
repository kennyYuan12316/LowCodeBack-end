using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSZ.Wms.Entitys.Dto.ZjnWmsLocation
{
    /// <summary>
    /// 货位定义
    /// </summary>
    public class ZjnWmsLocationDefineOutput
    {
        /// <summary>
        /// 仓库编号
        /// </summary>
        public string WarehouseNo { get; set; }
        /// <summary>
        /// 初始监控巷道
        /// </summary>
        public string AisleNo { get; set; }
        /// <summary>
        /// 巷道号
        /// </summary>
        public string[] AisleNos { get; set; }
        /// <summary>
        /// 初始行
        /// </summary>
        public int Row { get; set; }
        /// <summary>
        /// 初始列
        /// </summary>
        public int Cell { get; set; }
        /// <summary>
        /// 初始层
        /// </summary>
        public int Layer { get; set; }
        /// <summary>
        /// 每巷道最大数量
        /// </summary>
        public int MaxPerAisle { get; set; }
        /// <summary>
        /// 巷道是否相同行(无须重新加载监控)
        /// </summary>
        public bool IsStatic { get; set; }
        /// <summary>
        /// 两边排布
        /// </summary>
        public StaticSide Side { get; set; }
        /// <summary>
        /// 无规律巷道两边排布
        /// </summary>
        public Dictionary<string, StaticSide> Sides { get; set; }
        /// <summary>
        /// 货位状态字典
        /// </summary>
        public Dictionary<string, object> StatusList { get; set; }
    }

    public class StaticSide
    {
        /// <summary>
        /// 左边行
        /// </summary>
        public int LeftSide { get; set; }
        /// <summary>
        /// 右边行
        /// </summary>
        public int RightSide { get; set; }
    }
}
