using HSZ.Dependency;

namespace HSZ.System.Entitys.Dto.System.DataInterFace
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class DataInterfaceCrInput
    {
        /// <summary>
        /// 接口名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 分类id
        /// </summary>
        public string categoryId { get; set; }

        /// <summary>
        /// 数据源id
        /// </summary>
        public string dbLinkId { get; set; }

        /// <summary>
        /// 请求方式
        /// </summary>
        public string requestMethod { get; set; }

        /// <summary>
        /// 返回类型
        /// </summary>
        public string responseType { get; set; }

        /// <summary>
        ///排序
        /// </summary>
        public long? sortCode { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 查询语句
        /// </summary>
        public string query { get; set; }

        /// <summary>
        /// 数据类型(1-SQL数据，2-静态数据，3-Api数据)
        /// </summary>
        public int? dataType { get; set; }

        /// <summary>
        /// 请求参数JSON
        /// </summary>
        public string requestParameters { get; set; }

        /// <summary>
        /// 接口路径
        /// </summary>
        public string path { get; set; }

        /// <summary>
        /// 规则
        /// </summary>
        public int? checkType { get; set; }

        /// <summary>
        /// 请求头
        /// </summary>
        public string requestHeaders { get; set; }
        /// <summary>
        /// 处理数据
        /// </summary>
        public string dataProcessing { get; set; }

        /// <summary>
        /// 跨域ip
        /// </summary>
        public string ipAddress { get; set; }
    }
}
