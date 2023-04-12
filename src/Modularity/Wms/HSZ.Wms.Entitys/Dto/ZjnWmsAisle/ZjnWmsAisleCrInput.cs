using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsAisle
{
    /// <summary>
    /// 巷道信息修改输入参数
    /// </summary>
    public class ZjnWmsAisleCrInput
    {
        /// <summary>
        /// 巷道编号
        /// </summary>
        public string aisleNo { get; set; }

        /// <summary>
        /// 巷道名称
        /// </summary>
        public string aisleName { get; set; }

        /// <summary>
        /// 区域编号
        /// </summary>
        public string regionNo { get; set; }

        /// <summary>
        /// 仓库编号
        /// </summary>
        public string warehouseNo { get; set; }

        /// <summary>
        /// 堆垛机编号
        /// </summary>
        public string stackerNo { get; set; }

        /// <summary>
        /// 堆垛机作业模式
        /// </summary>
        public string stackerWorkType { get; set; }

        /// <summary>
        /// 堆垛机当前作业
        /// </summary>
        public string stackerWorkNo { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public string createUser { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createTime { get; set; }

        /// <summary>
        /// 有效标志
        /// </summary>
        public string enabledMark { get; set; }

    }
}