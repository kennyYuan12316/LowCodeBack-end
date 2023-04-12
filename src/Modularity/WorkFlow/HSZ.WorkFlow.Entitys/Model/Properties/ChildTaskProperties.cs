using HSZ.Dependency;
using System.Collections.Generic;

namespace HSZ.WorkFlow.Entitys.Model.Properties
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class ChildTaskProperties
    {
        /// <summary>
        /// 子流程
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 子流程发起人(1:自定义，2：部门主管，3：发起者主管，4：发起者本人)
        /// </summary>
        public int initiateType { get; set; }
        /// <summary>
        /// 主管级别
        /// </summary>
        public int managerLevel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> initiator { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> initiatePos { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> initiateRole { get; set; }
        /// <summary>
        /// 子流程引擎
        /// </summary>
        public string flowId { get; set; }
        /// <summary>
        /// 继承父流程字段数据
        /// </summary>
        public List<Assign> assignList { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public List<string> messageType { get; set; }
        /// <summary>
        /// 子流程id
        /// </summary>
        public List<string> childTaskId { get; set; } = new List<string>();
        /// <summary>
        /// 子流程数据
        /// </summary>
        public string formData { get; set; }
        /// <summary>
        /// 同步异步(异步:true)
        /// </summary>
        public bool isAsync { get; set; }
        /// <summary>
        /// 表单字段
        /// </summary>
        public string formField { get; set; }
        /// <summary>
        /// 指定复审审批节点
        /// </summary>
        public string nodeId { get; set; }
        /// <summary>
        /// 服务 请求路径
        /// </summary>
        public string getUserUrl { get; set; }
        /// <summary>
        /// 发起通知
        /// </summary>
        public MsgConfig launchMsgConfig { get; set; }

    }

    public class Assign
    {
        /// <summary>
        /// 父流程字段
        /// </summary>
        public string parentField { get; set; }
        /// <summary>
        /// 子流程字段
        /// </summary>
        public string childField { get; set; }
    }
}
