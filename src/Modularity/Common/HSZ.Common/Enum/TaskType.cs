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
    /// Name 子任务  出入库状态
    /// 使用场景：
    /// </summary>
    [SuppressSniffer]
    public enum OperationType
    {

        /// <summary>
        /// 入库
        /// </summary>
        [Description("入库")]
        Into = 0,

        /// <summary>
        /// 出库
        /// </summary>
        [Description("出库")]
        Out = 1,

        /// <summary>
        /// 搬运
        /// </summary>
        [Description("搬运")]
        Move = 2,

        
    }


    /// <summary>
    /// CreateTime 2022年10月8日14:09:44
    /// 任务流程类型
    /// 使用场景：
    /// </summary>
    [SuppressSniffer]
    public enum TaskProcessType
    {

        /// <summary>
        /// 出库
        /// </summary>
        [Description("出库")]
        Out = 1,

        /// <summary>
        /// 入库
        /// </summary>
        [Description("入库")]
        Into = 2,

        /// <summary>
        /// 移库
        /// </summary>
        [Description("移库")]
        Move = 3,

        /// <summary>
        /// 其他
        /// </summary>
        [Description("其他")]
        Other = 4,

        /// <summary>
        /// 空托入库
        /// </summary>
        [Description("空托入库")]
        EmptyTrayInto = 5,

        /// <summary>
        /// 空托出库
        /// </summary>
        [Description("空托出库")]
        EmptyTrayOut = 6,
        /// <summary>
        /// 入线体存储
        /// </summary>
        [Description("线体入存")]
        LineInto = 7,
        /// <summary>
        /// 出线体存储
        /// </summary>
        [Description("线体出存")]
        LineOut = 8,
    }



}
