using System;
using System.Collections.Generic;

namespace ZJN.Agv.Entitys.Dto.AgvTaskStatus
{
    /// <summary>
    /// Agv上传任务状态更新输入参数
    /// </summary>
    public class ZjnBaseStdTaskstatusUpInput : ZjnBaseStdTaskstatusCrInput
    {
        /// <summary>
        /// F_Id
        /// </summary>
        public string id { get; set; }
        
    }
}
