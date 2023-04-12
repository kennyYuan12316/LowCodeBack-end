using System;
using System.Collections.Generic;

namespace ZJN.Agv.Entitys.Dto.AgvPdaTask
{
    /// <summary>
    /// Agv上传PDA任务更新输入参数
    /// </summary>
    public class ZjnBaseStdPdataskUpInput : ZjnBaseStdPdataskCrInput
    {
        /// <summary>
        /// F_Id
        /// </summary>
        public string id { get; set; }
        
    }
}
