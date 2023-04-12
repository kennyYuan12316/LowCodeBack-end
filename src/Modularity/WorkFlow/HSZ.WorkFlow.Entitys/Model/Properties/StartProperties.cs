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
    public class StartProperties
    {
        /// <summary>
        /// 发起节点标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 指定发起人（为空则是所有人）
        /// </summary>
        public List<string> initiator { get; set; }
        /// <summary>
        /// 指定发起岗位（为空则是所有人）
        /// </summary>
        public List<string> initiatePos { get; set; }
        /// <summary>
        /// 指定发起角色
        /// </summary>
        public List<string> initiateRole { get; set; }
        /// <summary>
        /// 表单权限
        /// </summary>
        public List<FormOperatesModels> formOperates { get; set; }
        /// <summary>
        /// 打印id
        /// </summary>
        public string printId { get; set; }
        /// <summary>
        /// 是否评论
        /// </summary>
        public bool isComment { get; set; }
        /// <summary>
        /// 是否批量
        /// </summary>
        public bool isBatchApproval { get; set; }

        #region 按钮

        /// <summary>
        /// 
        /// </summary>
        public string revokeBtnText { get; set; } = "撤 回";
        /// <summary>
        /// 
        /// </summary>
        public bool hasTransferBtn { get; set; } = true;
        /// <summary>
        /// 
        /// </summary>
        public string submitBtnText { get; set; } = "提交审核";
        /// <summary>
        /// 
        /// </summary>
        public bool hasSubmitBtn { get; set; } = true;
        /// <summary>
        /// 
        /// </summary>
        public string saveBtnText { get; set; } = "保存草稿";
        /// <summary>
        /// 
        /// </summary>
        public bool hasSaveBtn { get; set; } = true;
        /// <summary>
        /// 
        /// </summary>
        public string pressBtnText { get; set; } = "催 办";
        /// <summary>
        /// 
        /// </summary>
        public bool hasPressBtn { get; set; } = true;
        /// <summary>
        /// 
        /// </summary>
        public string printBtnText { get; set; } = "打 印";
        /// <summary>
        /// 
        /// </summary>
        public bool hasPrintBtn { get; set; } = true;
        #endregion

        #region 节点事件
        /// <summary>
        /// 流程发起事件
        /// </summary>
        public FuncConfig initFuncConfig { get; set; }
        /// <summary>
        /// 流程结束事件
        /// </summary>
        public FuncConfig endFuncConfig { get; set; }
        /// <summary>
        /// 流程撤回事件
        /// </summary>
        public FuncConfig flowRecallFuncConfig { get; set; }
        #endregion

        #region 消息
        /// <summary>
        /// 审核
        /// </summary>
        public MsgConfig waitApproveMsgConfig { get; set; }
        /// <summary>
        /// 结束
        /// </summary>
        public MsgConfig endMsgConfig { get; set; }
        /// <summary>
        /// 同意
        /// </summary>
        public MsgConfig approveMsgConfig { get; set; }
        /// <summary>
        /// 拒绝
        /// </summary>
        public MsgConfig rejectMsgConfig { get; set; }
        /// <summary>
        /// 抄送
        /// </summary>
        public MsgConfig copyMsgConfig { get; set; }
        #endregion
    }
}
