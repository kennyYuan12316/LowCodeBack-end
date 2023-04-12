using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HSZ.System.Interfaces.Common
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：通用控制器
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="type"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        Task<dynamic> Uploader(string type, IFormFile file);

        /// <summary>
        /// 根据类型获取文件存储路径
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string GetPathByType(string type);

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        dynamic Export(string jsonStr,string name);

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        string Import(IFormFile file);

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="type"></param>
        /// <param name="file"></param>
        void UploadFile(string type, IFormFile file);
    }
}
