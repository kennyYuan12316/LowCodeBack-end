using HSZ.Dependency;

namespace HSZ.WorkFlow.Entitys.Enum
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：流程状态
    /// </summary>
    [SuppressSniffer]
    public class FlowTaskStatusEnum
    {
        /// <summary>
        /// 等待提交
        /// </summary>
        public static int Draft
        {
            get
            {
                return 0;
            }
        }
        /// <summary>
        /// 等待审核
        /// </summary>
        public static int Handle
        {
            get
            {
                return 1;
            }
        }
        /// <summary>
        /// 审核通过
        /// </summary>
        public static int Adopt
        {
            get
            {
                return 2;
            }
        }
        /// <summary>
        /// 审核驳回
        /// </summary>
        public static int Reject
        {
            get
            {
                return 3;
            }
        }
        /// <summary>
        /// 审核撤销
        /// </summary>
        public static int Revoke
        {
            get
            {
                return 4;
            }
        }
        /// <summary>
        /// 审核作废
        /// </summary>
        public static int Cancel
        {
            get
            {
                return 5;
            }
        }
    }
}