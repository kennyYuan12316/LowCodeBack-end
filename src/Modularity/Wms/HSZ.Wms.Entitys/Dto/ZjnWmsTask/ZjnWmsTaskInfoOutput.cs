using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Dto.ZjnWmsTask
{
    /// <summary>
    /// 主任务输出参数
    /// </summary>
    public class ZjnWmsTaskInfoOutput
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 任务号
        /// </summary>
        public string taskNo { get; set; }
        
        /// <summary>
        /// 任务名称
        /// </summary>
        public string taskName { get; set; }
        
        /// <summary>
        /// 起点工位
        /// </summary>
        public string positionFrom { get; set; }

        /// <summary>
        /// 起点工位名称
        /// </summary>
        public string positionFromName { get; set; }
        /// <summary>
        /// 终点工位
        /// </summary>
        public string positionTo { get; set; }

        /// <summary>
        /// 终点工位名称
        /// </summary>
        public string positionToName { get; set; }
        /// <summary>
        /// 当前工位
        /// </summary>
        public string positionCurrent { get; set; }
        /// <summary>
        /// 当前工位名称
        /// </summary>
        public string positionCurrentName { get; set; }
        /// <summary>
        /// 路径编号
        /// </summary>
        public string routeNo { get; set; }
        
        /// <summary>
        /// 创建者
        /// </summary>
        public string createUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createTime { get; set; }
        
        /// <summary>
        /// 修改用户
        /// </summary>
        public string lastModifyUserId { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public DateTime? lastModifyTime { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        public int? enabledMark { get; set; }
        
        /// <summary>
        /// 任务来源
        /// </summary>
        public string taskFrom { get; set; }
        
        /// <summary>
        /// 任务状态
        /// </summary>
        public int? taskState { get; set; }
        
        /// <summary>
        /// 业务类型编号
        /// </summary>
        public string taskTypeNum { get; set; }

        /// <summary>
        /// 业务类型名称
        /// </summary>
        public string taskTypeName{ get; set; }

        /// <summary>
        /// 任务状态描述
        /// </summary>
        public string taskDescribe { get; set; }


        /// <summary>
        /// 业务Json数据
        /// </summary>
        public string workPath { get; set; }


        /// <summary>
        /// 单据指令
        /// </summary>
        public string orderNo { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        public string materialCode { get; set; }


        /// <summary>
        /// 数量
        /// </summary>
        public decimal? quantity { get; set; }


        /// <summary>
        /// 单据号
        /// </summary>
        public string billNo { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public int? priority { get; set; }

        /// <summary>
        /// 托盘编号
        /// </summary>
        public string trayNo { get; set; }

    }
}