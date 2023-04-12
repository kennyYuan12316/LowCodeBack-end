using Google.Protobuf.WellKnownTypes;
using HSZ.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSZ.Common.Enum
{
    /// <summary>
    /// 货位状态
    /// </summary>
    [SuppressSniffer]
    public enum LocationStatus
    {
        /// <summary>
        /// 空
        /// </summary>
        Empty = 0,
        /// <summary>
        /// 已满
        /// </summary>
        Full = 1,
        /// <summary>
        /// 未满
        /// </summary>
        NotFull = 2,
        /// <summary>
        /// 故障
        /// </summary>
        Trouble = 3,
        /// <summary>
        /// 火警
        /// </summary>
        Fire = 4,
        /// <summary>
        /// 静置中
        /// </summary>
        Standing = 5,
        /// <summary>
        /// 静置完成
        /// </summary>
        StandingOver = 6,
        /// <summary>
        /// 预约
        /// </summary>
        Order = 7,
        /// <summary>
        /// 锁定
        /// </summary>
        Lock = 8
    }
}
