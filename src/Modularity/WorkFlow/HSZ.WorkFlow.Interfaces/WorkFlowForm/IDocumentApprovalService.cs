using System.Threading.Tasks;

namespace HSZ.WorkFlow.Interfaces.WorkFlowForm
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：文件签批意见表
    /// </summary>
    public interface IDocumentApprovalService
    {
        /// <summary>
        /// 工作流表单操作
        /// </summary>
        /// <param name="id"></param>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task Save(string id,object obj,int type);
    }
}
