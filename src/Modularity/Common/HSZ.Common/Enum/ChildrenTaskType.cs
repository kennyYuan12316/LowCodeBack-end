using HSZ.Dependency;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSZ.Common.Enum
{

    /// <summary>
    /// CreateTime 2022年10月8日14:09:44
    /// Name 子任务执行类型  枚举
    /// 使用场景：1.由WCS上报子任务完成状态接口时使用 2.WMS判断业务场景、分发任务处理接口时使用
    /// </summary>
    [SuppressSniffer]
    public enum ChildrenTaskType
    {

        /// <summary>
        /// 平面设备调度任务
        /// </summary>
        [Description("平面设备调度任务")]
        PlaneTask = 1,

        /// <summary>
        /// 称重校验任务
        /// </summary>
        [Description("称重校验任务")]
        WeighTask = 2,

        /// <summary>
        /// Rgv调度任务
        /// </summary>
        [Description("Rgv调度任务")]
        RgvTask = 3,

        /// <summary>
        /// 堆垛机出库任务
        /// </summary>
        [Description("堆垛机出库任务")]
        StockerOutTask = 4,

        /// <summary>
        /// 堆垛机入库任务
        /// </summary>
        [Description("堆垛机入库任务")]
        StockerInTask = 5,

        /// <summary>
        /// 堆垛机移库任务
        /// </summary>
        [Description("堆垛机移库任务")]
        StockerMoveTask = 6,

        /// <summary>
        /// 入库台任务
        /// </summary>
        [Description("入库台任务")]
        DeskInTask = 7,

        /// <summary>
        /// 出库台任务
        /// </summary>
        [Description("出库台任务")]
        DeskOutTask = 8,

        /// <summary>
        /// 提升机调度任务
        /// </summary>
        [Description("提升机调度任务")]
        HoistTask = 9,

        /// <summary>
        /// 堆垛机消防任务
        /// </summary>
        [Description("堆垛机消防任务")]
        FireControlTask = 10,

        /// <summary>
        /// Agv调度任务
        /// </summary>
        [Description("Agv调度任务")]
        AgvTask = 11


    }
}
