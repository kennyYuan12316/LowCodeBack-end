using HSZ.Dependency;
using System.Collections.Generic;

namespace HSZ.WorkFlow.Entitys.Model
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：引擎关联表单
    /// </summary>
    [SuppressSniffer]
    public class FlowEngineTablesModel
    {
        /// <summary>
        /// 主表关联字段
        /// </summary>
        public string relationField { get; set; }
        /// <summary>
        /// 关联主表
        /// </summary>
        public string relationTable { get; set; }
        /// <summary>
        /// 关联表
        /// </summary>
        public string table { get; set; }
        /// <summary>
        /// 关联表名称
        /// </summary>
        public string tableName { get; set; }
        /// <summary>
        /// 关联表关联主表字段
        /// </summary>
        public string tableField { get; set; }
        /// <summary>
        /// 是否主表（0否，1是）
        /// </summary>
        public string typeId { get; set; }
        /// <summary>
        /// 关联表字段信息
        /// </summary>
        public List<FieldsItem> fields { get; set; }
    }

    public class FieldsItem
    {
        /// <summary>
        /// 字段key
        /// </summary>
        public string field { get; set; }
        /// <summary>
        /// 字段说明
        /// </summary>
        public string fieldName { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public string dataType { get; set; }
    }
}
