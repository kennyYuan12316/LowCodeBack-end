using HSZ.Dependency;
using System;
using System.Collections.Generic;

namespace HSZ.System.Entitys.Model.Permission.User
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：登录者信息
    /// </summary>
    [SuppressSniffer]
    public class UserInfo
    {
        /// <summary>
        /// 用户主键
        /// </summary>
        public string userId { get; set; }

        /// <summary>
        /// 用户账户
        /// </summary>
        public string userAccount { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string headIcon { get; set; }

        /// <summary>
        /// 用户性别
        /// </summary>
        public int gender { get; set; }

        /// <summary>
        /// 座机号
        /// </summary>
        public string landline { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string telePhone { get; set; }

        ///// <summary>
        ///// 公司ID
        ///// </summary>
        //public string companyId { get; set; }

        ///// <summary>
        ///// 公司名称
        ///// </summary>
        //public string companyName { get; set; }

        /// <summary>
        /// 所属组织
        /// </summary>
        public string organizeId { get; set; }

        /// <summary>
        /// 组织名称
        /// </summary>
        public string organizeName { get; set; }

        /// <summary>
        /// 我的主管
        /// </summary>
        public string managerId { get; set; }

        /// <summary>
        /// 下属机构
        /// </summary>
        public string[] subsidiary { get; set; }

        /// <summary>
        /// 我的下属
        /// </summary>
        public string[] subordinates { get; set; }

        /// <summary>
        /// 岗位信息
        /// </summary>
        public List<PositionInfo> positionIds { get; set; }

        /// <summary>
        /// 岗位名称
        /// </summary>
        public string positionName { get; set; }

        /// <summary>
        /// 岗位主键
        /// </summary>
        public string positionId { get; set; }

        /// <summary>
        /// 角色主键
        /// </summary>
        public string roleId { get; set; }

        /// <summary>
        /// 角色主键名称
        /// </summary>
        public string roleName { get; set; }

        /// <summary>
        /// 角色数组
        /// </summary>
        public string[] roleIds { get; set; }

        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime? loginTime { get; set; }

        /// <summary>
        /// 登录IP地址
        /// </summary>
        public string loginIPAddress { get; set; }

        /// <summary>
        /// 登录IP地址所在城市
        /// </summary>
        public string loginIPAddressName { get; set; }

        /// <summary>
        /// 登录MAC地址
        /// </summary>		
        public string MACAddress { get; set; }

        /// <summary>
        /// 登录平台设备
        /// </summary>
        public string loginPlatForm { get; set; }

        /// <summary>
        /// 上次登录开启（0:未开启,1:已开启）
        /// </summary>
        /// <returns></returns>
        public int? prevLogin { get; set; }

        /// <summary>
        /// 上次登录时间
        /// </summary>
        /// <returns></returns>
        public DateTime? prevLoginTime { get; set; }

        /// <summary>
        /// 上次登录IP地址
        /// </summary>
        /// <returns></returns>
        public string prevLoginIPAddress { get; set; }

        /// <summary>
        /// 上次登录IP地址所在城市
        /// </summary>
        /// <returns></returns>
        public string prevLoginIPAddressName { get; set; }

        /// <summary>
        /// 是否超级管理员
        /// </summary>
        public bool isAdministrator { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public TimeSpan? overdueTime { get; set; }

        /// <summary>
        /// 租户编码
        /// </summary>
        public string tenantId { get; set; }

        /// <summary>
        /// 租户数据库名称
        /// </summary>
        public string tenantDbName { get; set; }

        /// <summary>
        /// 租户数据库类型
        /// </summary>
        public string tenantDbType { get; set; }

        /// <summary>
        /// 门户id
        /// </summary>
        public string portalId { get; set; }

        /// <summary>
        /// 数据范围
        /// </summary>
        public List<UserDataScope> dataScope { get; set; }

        /// <summary>
        /// 直属主管
        /// </summary>
        public string manager { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string mobilePhone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? birthday { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        public string departmentId { get; set; }

        /// <summary>
        /// 部门名称 结构树 
        /// </summary>
        public string departmentName { get; set; }
    }
}
