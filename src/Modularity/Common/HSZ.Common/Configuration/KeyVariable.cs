using HSZ.Common.Extension;
using HSZ.Dependency;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HSZ.Common.Configuration
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：Key常量
    /// </summary>
    [SuppressSniffer]
    public class KeyVariable
    {
        /// <summary>
        /// 多租户模式
        /// </summary>
        public static bool MultiTenancy
        {
            get
            {
                var flag = App.Configuration["HSZ_App:MultiTenancy"];
                return flag.ToBool();
            }
        }

        /// <summary>
        /// 系统文件路径
        /// </summary>
        public static string SystemPath
        {
            get
            {
                return string.IsNullOrEmpty(App.Configuration["HSZ_App:SystemPath"]) ? Directory.GetCurrentDirectory() : App.Configuration["HSZ_App:SystemPath"];
            }
        }

        /// <summary>
        /// 命名空间
        /// </summary>
        public static List<string> AreasName
        {
            get
            {
                return string.IsNullOrEmpty(App.Configuration["HSZ_APP:CodeAreasName"]) ? new List<string>() : App.Configuration["HSZ_APP:CodeAreasName"].Split(',').ToList();
            }
        }

        /// <summary>
        /// 允许上传图片类型
        /// </summary>
        public static List<string> AllowImageType
        {
            get
            {
                return string.IsNullOrEmpty(App.Configuration["HSZ_APP:AllowUploadImageType"]) ? new List<string>() : App.Configuration["HSZ_APP:AllowUploadImageType"].Split(',').ToList();
            }
        }

        /// <summary>
        /// 允许上传文件类型
        /// </summary>
        public static List<string> AllowUploadFileType
        {
            get
            {
                return string.IsNullOrEmpty(App.Configuration["HSZ_APP:AllowUploadFileType"]) ? new List<string>() : App.Configuration["HSZ_APP:AllowUploadFileType"].Split(',').ToList();
            }
        }

        /// <summary>
        /// 微信允许上传文件类型
        /// </summary>
        public static List<string> WeChatUploadFileType
        {
            get
            {
                return string.IsNullOrEmpty(App.Configuration["HSZ_APP:WeChatUploadFileType"]) ? new List<string>() : App.Configuration["HSZ_APP:WeChatUploadFileType"].Split(',').ToList();
            }
        }

        /// <summary>
        /// MinIO桶
        /// </summary>
        public static string BucketName
        {
            get
            {
                return string.IsNullOrEmpty(App.Configuration["HSZ_APP:SSO:BucketName"]) ? "" : App.Configuration["HSZ_APP:SSO:BucketName"];
            }
        }

        /// <summary>
        /// 文件储存类型
        /// </summary>
        public static string FileStoreType
        {
            get
            {
                return string.IsNullOrEmpty(App.Configuration["HSZ_APP:SSO:Provider"]) ? "Invalid" : App.Configuration["HSZ_APP:SSO:Provider"];
            }
        }

        /// <summary>
        /// App版本
        /// </summary>
        public static string AppVersion
        {
            get
            {
                return string.IsNullOrEmpty(App.Configuration["HSZ_APP:AppVersion"]) ? "" : App.Configuration["HSZ_APP:AppVersion"];
            }
        }

        /// <summary>
        /// 文件储存类型
        /// </summary>
        public static string AppUpdateContent
        {
            get
            {
                return string.IsNullOrEmpty(App.Configuration["HSZ_APP:AppUpdateContent"]) ? "" : App.Configuration["HSZ_APP:AppUpdateContent"];
            }
        }

        /// <summary>
        /// 数据接口api，域名配置
        /// </summary>
        public static string DataInterfaceUrl
        {
            get
            {
                return string.IsNullOrEmpty(App.Configuration["HSZ_APP:DataInterfaceUrl"]) ? "" : App.Configuration["HSZ_APP:DataInterfaceUrl"];
            }
        }
    }
}
