using HSZ.Common.Enum;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsTaskDetails
{
    /// <summary>
    /// 子任务修改输出参数
    /// </summary>
    public class ZjnWmsTaskDetailsInfoOutput
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 子任务id
        /// </summary>
        public string taskDetailsId { get; set; }
        
        /// <summary>
        /// 子任务名称
        /// </summary>
        public string taskDetailsName { get; set; }
        
        /// <summary>
        /// 主任务id
        /// </summary>
        public string taskId { get; set; }
        
        /// <summary>
        /// 起点工位
        /// </summary>
        public string taskDetailsStart { get; set; }

        /// <summary>
        /// 起点工位名称
        /// </summary>
        public string taskDetailsStartName { get; set; }

        /// <summary>
        /// 终点工位
        /// </summary>
        public string taskDetailsEnd { get; set; }

        /// <summary>
        /// 终点工位名称
        /// </summary>
        public string taskDetailsEndName { get; set; }

        /// <summary>
        /// 子任务状态
        /// </summary>
        public int? taskDetailsStates { get; set; }
        
        /// <summary>
        /// 子任务状态描述
        /// </summary>
        public string taskDetailsDescribe { get; set; }
        
        /// <summary>
        /// 创建用户
        /// </summary>
        public string createUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createTime { get; set; }
        
        /// <summary>
        /// 修改用户
        /// </summary>
        public string modifiedUser { get; set; }
        
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? modifiedTime { get; set; }

        /// <summary>
        /// 托盘号
        /// </summary>
        public string trayNo { get; set; }


        /// <summary>
        /// 下一个节点名称
        /// </summary>
        public string taskDetailsNextName { get; set; }

        /// <summary>
        /// 路径id
        /// </summary>
        public string workPathId { get; set; }

        /// <summary>
        /// 路径名称
        /// </summary>
        public string workPathname { get; set; }

        /// <summary>
        /// 开始行
        /// </summary>
        public int? rowStart { get; set; }

        /// <summary>
        /// 开始列
        /// </summary>
        public int? cellStart { get; set; }

        /// <summary>
        /// 开始层
        /// </summary>
        public int? layerStart { get; set; }

        /// <summary>
        /// 结束行
        /// </summary>
        public int? rowEnd { get; set; }

        /// <summary>
        /// 结束列
        /// </summary>
        public int? cellEnd { get; set; }

        /// <summary>
        /// 结束层
        /// </summary>
        public int? layerEnd { get; set; }

        /// <summary>
        /// 当前节点id
        /// </summary>
        public string nodeCode { get; set; }

        /// <summary>
        /// 上一个节点id
        /// </summary>
        public string nodeUp { get; set; }

        /// <summary>
        /// 下一个节点id
        /// </summary>
        public string nodeNext { get; set; }

        /// <summary>
        /// 节点类型
        /// </summary>
        public string nodeType { get; set; }

        /// <summary>
        /// json
        public string nodePropertyJson { get; set; }


        /// <summary>
        /// 返回判断值
        /// </summary>
        public string resultValue { get; set; }

        /// <summary>
        /// 子任务类型
        /// </summary>
        public int? taskType { get; set; }



        /// <summary>
        /// 优先级
        /// </summary>
        public int? priority { get; set; }

        /// <summary>
        /// 任务流程类型
        /// </summary>
        public TaskProcessType taskProcessType { get; set; }

        /// <summary>
        /// 输送设备工位
        /// </summary>
        public string taskDetailsMove { get; set; }

        /// <summary>
        /// 输送设备名称
        /// </summary>
        public string taskDetailsMoveName { get; set; }

        /// <summary>
        /// 产品等级
        /// </summary>
        public string productLevel { get; set; }

        /// <summary>
        /// 主任务的出库口，出库最后一个任务绑定出库口
        /// </summary>
        public string positionTo { get; set; }

        public static explicit operator ZjnWmsTaskDetailsInfoOutput(bool v)
        {
            throw new NotImplementedException();
        }
    }
}