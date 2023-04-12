using HSZ.Dependency;
using System;
using System.Text.Json.Serialization;

namespace HSZ.System.Entitys.Dto.System.DataInterfaceLog
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class DataInterfaceLogListOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 调用时间
        /// </summary>
        public DateTime? invokTime { get; set; }
        /// <summary>
        /// 调用者
        /// </summary>
        public string userId { get; set; }
        /// <summary>
        /// ip
        /// </summary>
        public string invokIp { get; set; }
        /// <summary>
        /// 设备
        /// </summary>
        public string invokDevice { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string invokType { get; set; }
        /// <summary>
        /// 耗时
        /// </summary>
        public int? invokWasteTime { get; set; }
        [JsonIgnore]
        public string invokeId { get; set; }
    }
}
