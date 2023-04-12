using HSZ.Common.Util;
using HSZ.Dependency;
using System;

namespace HSZ.System.Entitys.Dto.Permission.Department
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：部门树列表输出
    /// </summary>
    [SuppressSniffer]
    public class DepartmentListOutput : TreeModel
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 部门编码
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 部门经理（姓名/账号）
        /// </summary>
        public string manager { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? creatorTime { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public long? sortCode { get; set; }
    }
}
