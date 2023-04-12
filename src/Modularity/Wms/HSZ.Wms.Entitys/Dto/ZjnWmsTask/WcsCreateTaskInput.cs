using HSZ.Common.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsTask
{
    /// <summary>
    /// Wcs创建任务参数
    /// </summary>
    public class WcsCreateTaskInput
    {
        /// <summary>
        /// 创建任务类型
        /// </summary>
        public TaskProcessType taskProcessType { get; set; }

        /// <summary>
        /// 设备Id
        /// </summary>
        public string deviceId { get; set; }


        /// <summary>
        /// 托盘号
        /// </summary>
        public string trayNo { get; set; }


        /// <summary>
        /// 托盘类型 1.小托盘 2.大托盘
        /// </summary>
        public int trayType { get; set; }


        /// <summary>
        /// 托盘数量
        /// </summary>
        public int trayNum { get; set; }
        /// <summary>
        /// PLC任务号
        /// </summary>
        public int plcTaskNo { get; set; }




    }
}