using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSZ.Common.TaskResultPubilcParms
{
    /// <summary>
    /// 任务反馈公共参数   后续可继续追加
    /// </summary>
    public class TaskResultPubilcParameter
    {
        /// <summary>
        /// 称重机称重参数
        /// </summary>
        public float? weigh { get; set; }

        /// <summary>
        /// 设备号
        /// </summary>
        public string deviceNo { get; set; }

        /// <summary>
        /// 通过单/双托，判断目标位置  单托：1，双托1:2，双托2:3
        /// </summary>
        public int targetType { get; set; }

    }


}
