using HSZ.Dependency;

namespace HSZ.Common.Const
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：公共常量
    /// </summary>
    [SuppressSniffer]
    public class CommonConst
    {
        /// <summary>
        /// 默认密码
        /// </summary>
        public const string DEFAULT_PASSWORD = "123456";

        /// <summary>
        /// 用户缓存
        /// </summary>
        public const string CACHE_KEY_USER = "user_";

        /// <summary>
        /// 菜单缓存
        /// </summary>
        public const string CACHE_KEY_MENU = "menu_";

        /// <summary>
        /// 权限缓存
        /// </summary>
        public const string CACHE_KEY_PERMISSION = "permission_";

        /// <summary>
        /// 数据范围缓存
        /// </summary>
        public const string CACHE_KEY_DATASCOPE = "datascope_";

        /// <summary>
        /// 验证码缓存
        /// </summary>
        public const string CACHE_KEY_CODE = "vercode_";

        /// <summary>
        /// 单据编码缓存
        /// </summary>
        public const string CACHE_KEY_BILLRULE = "billrule_";

        /// <summary>
        /// 在线用户缓存
        /// </summary>
        public const string CACHE_KEY_ONLINE_USER = "onlineuser_";

        /// <summary>
        /// 岗位缓存
        /// </summary>
        public const string CACHE_KEY_POSITION = "position_";

        /// <summary>
        /// 角色缓存
        /// </summary>
        public const string CACHE_KEY_ROLE = "role_";

        /// <summary>
        /// 在线开发缓存
        /// </summary>
        public const string VISUALDEV = "visualdev_";

        /// <summary>
        /// 定时任务缓存
        /// </summary>
        public const string CACHE_KEY_TIMER_JOB = "timerjob";


        /// <summary>
        /// 任务号缓存  int32 唯一id
        /// </summary>
        public const string CACHE_KEY_TASKID = "taskId";

        /// <summary>
        /// 任务数据缓存
        /// </summary>
        public const string CACHE_KEY_TASK_LIST = "taskList";

        /// <summary>
        /// 监控数据缓存
        /// </summary>
        public const string CACHE_KEY_Monitor_Data = "monitorData";


        /// <summary>
        /// PLC连接状态
        /// </summary>
        public const string CACHE_KEY_Plc_Connect_State = "plcConnectState";

        /// <summary>
        /// 货位数据缓存
        /// </summary>
        public const string CACHE_KEY_LOCATION_LIST = "locationList";
        /// <summary>
        /// 线体存储队列Key
        /// </summary>
        public const string cache_queue_linelist = "LineListQueue_";
    }
}
