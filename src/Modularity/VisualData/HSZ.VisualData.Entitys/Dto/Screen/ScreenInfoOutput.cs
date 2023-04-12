using System;

namespace HSZ.VisualData.Entitys.Dto.Screen
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：大屏信息输出
    /// </summary>
    public class ScreenInfoOutput
    {
        /// <summary>
        /// 大屏背景
        /// </summary>
        public string backgroundUrl { get; set; }

        /// <summary>
        /// 	大屏类型
        /// </summary>
        public int category { get; set; }

        /// <summary>
        /// 创建部门
        /// </summary>
        public string createDept { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string createUser { get; set; }

        /// <summary>
        /// 主键ID
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 是否已删除
        /// </summary>
        public int isDeleted { get; set; }

        /// <summary>
        /// 发布密码
        /// </summary>
        public string password { get; set; }

        /// <summary>
        /// 业务状态
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 大屏标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? updateTime { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public string updateUser { get; set; }
    }
}
