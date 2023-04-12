using HSZ.Dependency;

namespace HSZ.Common.Enum
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：数据操作类型
    /// </summary>
    [SuppressSniffer]
    public enum DataOpType
    {
        /// <summary>
        /// 其它
        /// </summary>
        OTHER,

        /// <summary>
        /// 增加
        /// </summary>
        ADD,

        /// <summary>
        /// 删除
        /// </summary>
        DELETE,

        /// <summary>
        /// 编辑
        /// </summary>
        EDIT,

        /// <summary>
        /// 更新
        /// </summary>
        UPDATE,

        /// <summary>
        /// 查询
        /// </summary>
        QUERY,

        /// <summary>
        /// 详情
        /// </summary>
        DETAIL,

        /// <summary>
        /// 树
        /// </summary>
        TREE,

        /// <summary>
        /// 导入
        /// </summary>
        IMPORT,

        /// <summary>
        /// 导出
        /// </summary>
        EXPORT,

        /// <summary>
        /// 授权
        /// </summary>
        GRANT,

        /// <summary>
        /// 强退
        /// </summary>
        FORCE,

        /// <summary>
        /// 清空
        /// </summary>
        CLEAN,

        /// <summary>
        /// 修改状态
        /// </summary>
        CHANGE_STATUS
    }
}
