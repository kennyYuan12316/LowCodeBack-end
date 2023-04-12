using HSZ.Common.Const;
using HSZ.Common.Entity;
using SqlSugar;

namespace HSZ.System.Entitys.System
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：数据接口
    /// </summary>
    [SugarTable("ZJN_BASE_DATA_INTERFACE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class DataInterfaceEntity : CLDEntityBase
    {
        /// <summary>
        /// 分类ID
        /// </summary>
        [SugarColumn(ColumnName = "F_CATEGORYID")]
        public string CategoryId { get; set; }

        /// <summary>
        /// 接口名
        /// </summary>
        [SugarColumn(ColumnName = "F_FULLNAME")]
        public string FullName { get; set; }

        /// <summary>
        /// 接口链接
        /// </summary>
        [SugarColumn(ColumnName = "F_PATH")]
        public string Path { get; set; }

        /// <summary>
        /// 请求方式
        /// </summary>
        [SugarColumn(ColumnName = "F_REQUESTMETHOD")]
        public string RequestMethod { get; set; }

        /// <summary>
        /// 返回类型
        /// </summary>
        [SugarColumn(ColumnName = "F_RESPONSETYPE")]
        public string ResponseType { get; set; }

        /// <summary>
        /// 查询语句
        /// </summary>
        [SugarColumn(ColumnName = "F_QUERY")]
        public string Query { get; set; }

        /// <summary>
        /// 接口入参
        /// </summary>
        [SugarColumn(ColumnName = "F_REQUESTPARAMETERS")]
        public string RequestParameters { get; set; }

        /// <summary>
        /// 接口数据处理
        /// </summary>
        [SugarColumn(ColumnName = "F_DATAPROCESSING")]
        public string DataProcessing { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [SugarColumn(ColumnName = "F_ENCODE")]
        public string EnCode { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        [SugarColumn(ColumnName = "F_SORTCODE")]
        public long? SortCode { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }

        /// <summary>
        /// 数据源id
        /// </summary>
        [SugarColumn(ColumnName = "F_DBLINKID")]
        public string DBLinkId { get; set; }

        /// <summary>
        /// 数据类型(1-SQL数据，2-静态数据，3-Api数据)
        /// </summary>
        [SugarColumn(ColumnName = "F_DATATYPE")]
        public int? DataType { get; set; }

        /// <summary>
        /// 验证规则(0-不验证，1-授权，2-域名)
        /// </summary>
        [SugarColumn(ColumnName = "F_CHECKTYPE")]
        public int? CheckType { get; set; }

        /// <summary>
        /// 请求头
        /// </summary>
        [SugarColumn(ColumnName = "F_REQUESTHEADERS")]
        public string RequestHeaders { get; set; }

        /// <summary>
        /// 跨域鉴权ip
        /// </summary>
        [SugarColumn(ColumnName = "F_IPADDRESS")]
        public string IpAddress { get; set; }
    }
}
