using HSZ.Common.Configuration;
using HSZ.Common.Core.Captcha.General;
using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Helper;
using HSZ.DataEncryption;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.JsonSerialization;
using HSZ.Logging.Attributes;
using HSZ.RemoteRequest.Extensions;
using HSZ.System.Interfaces.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnceMi.AspNetCore.OSS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.System.Service.Common
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：业务实现：通用控制器 
    /// </summary>
    [ApiDescriptionSettings(Tag = "Common", Name = "File", Order = 161)]
    [Route("api/[controller]")]
    [IgnoreLog]
    public class FileService : IFileService, IDynamicApiController, ITransient
    {
        private readonly IGeneralCaptcha _captchaHandle;// 验证码服务
        private readonly IUserManager _userManager;
        private readonly IOSSServiceFactory _oSSServiceFactory;

        /// <summary>
        /// 初始化一个<see cref="FileService"/>类型的新实例
        /// </summary>
        public FileService(IGeneralCaptcha captchaHandle, 
            IUserManager userManager, 
            IOSSServiceFactory oSSServiceFactory)
        {
            _captchaHandle = captchaHandle;
            _userManager = userManager;
            _oSSServiceFactory = oSSServiceFactory;
        }

        /// <summary>
        /// 上传文件/图片
        /// </summary>
        /// <returns></returns>
        [HttpPost("Uploader/{type}")]
        [AllowAnonymous, IgnoreLog]
        public async Task<dynamic> Uploader(string type, IFormFile file)
        {
            var fileType = Path.GetExtension(file.FileName).Replace(".", "");
            if (!this.AllowFileType(fileType, type))
                throw HSZException.Oh(ErrorCode.D1800);
            var _filePath = GetPathByType(type);
            var _fileName = DateTime.Now.ToString("yyyyMMdd") + "_" + YitIdHelper.NextId().ToString() + Path.GetExtension(file.FileName);
            await UploadFileByType(file, _filePath, _fileName);
            return new { name = _fileName, url = string.Format("/api/File/Image/{0}/{1}", type, _fileName) };
        }

        /// <summary>
        /// 上传文件预览 (doc/docx/xls/xlsx/ppt/pptx/pdf)
        /// </summary>
        /// <returns></returns>
        [HttpGet("Uploader/Preview")]
        [AllowAnonymous, IgnoreLog]
        public async Task<dynamic> Preview(string fileName)
        {
            var typeList = new string[] { "doc", "docx", "xls", "xlsx", "ppt", "pptx", "pdf" };
            var type = fileName.Split('.').LastOrDefault();
            if (typeList.Contains(type))
            {
                if (fileName.IsNotEmptyOrNull())
                {
                    var previewType = App.Configuration["HSZ_APP:PreviewType"];

                    if (previewType.Equals("localPreview")) return KKFileUploaderPreview(fileName);
                    else return await YoZoUploaderPreview(fileName, 5, 1);
                }
                else throw HSZException.Oh(ErrorCode.D8000);
            }
            else throw HSZException.Oh(ErrorCode.D1802);
        }

        /// <summary>
        /// 生成图片链接
        /// </summary>
        /// <param name="type">图片类型 </param>
        /// <param name="fileName">注意 后缀名前端故意把 .替换@ </param>
        /// <returns></returns>
        [HttpGet("Image/{type}/{fileName}")]
        [AllowAnonymous, IgnoreLog]
        public async Task<IActionResult> GetImg(string type, string fileName)
        {
            var filePath = Path.Combine(GetPathByType(type), fileName.Replace("@", "."));
            return await DownloadFileByType(filePath, fileName);
        }

        /// <summary>
        /// 生成大屏图片链接
        /// </summary>
        /// <param name="type">图片类型 </param>
        /// <param name="fileName">注意 后缀名前端故意把 .替换@ </param>
        /// <returns></returns>
        [HttpGet("VisusalImg/{type}/{fileName}")]
        [AllowAnonymous, IgnoreLog]
        public async Task<IActionResult> GetScreenImg(string type, string fileName)
        {
            var filePath = Path.Combine(GetPathByType(type), type, fileName.Replace("@", "."));
            return await DownloadFileByType(filePath, fileName);
        }

        /// <summary>
        /// 获取图形验证码
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <returns></returns>
        [HttpGet("ImageCode/{timestamp}")]
        [AllowAnonymous, IgnoreLog]
        [NonUnify]
        public IActionResult GetCode(string timestamp)
        {
            return new FileContentResult(_captchaHandle.CreateCaptchaImage(timestamp, 114, 32), "image/jpeg");
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        [HttpPost("Uploader/userAvatar")]
        [AllowAnonymous, IgnoreLog]
        public async Task<dynamic> UploadImage(IFormFile file)
        {
            var ImgType = Path.GetExtension(file.FileName).Replace(".", "");
            if (!this.AllowImageType(ImgType))
                throw HSZException.Oh(ErrorCode.D5013);
            var filePath = FileVariable.UserAvatarFilePath;
            var fileName = DateTime.Now.ToString("yyyyMMdd") + "_" + YitIdHelper.NextId().ToString() + Path.GetExtension(file.FileName);
            await UploadFileByType(file, filePath, fileName);
            return new { name = fileName, url = "/api/file/Image/userAvatar/" + fileName };
        }

        #region 下载附件

        /// <summary>
        /// 获取下载文件链接
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet("Download/{type}/{fileName}")]
        [AllowAnonymous, IgnoreLog]
        public dynamic DownloadUrl(string type, string fileName)
        {
            var url = _userManager.UserId + "|" + fileName + "|" + type;
            var encryptStr = DESCEncryption.Encrypt(url, "HSZ");
            return new { name = fileName, url = "/api/file/Download?encryption=" + encryptStr };
        }

        /// <summary>
        /// 下载文件链接
        /// </summary>
        [HttpGet("Download")]
        [AllowAnonymous, IgnoreLog]
        public async Task<dynamic> DownloadFile([FromQuery] string encryption)
        {
            var decryptStr = DESCEncryption.Decrypt(encryption, "HSZ");
            var paramsList = decryptStr.Split("|").ToList();
            if (paramsList.Count > 0)
            {
                var fileName = paramsList.Count > 1 ? paramsList[1] : "";
                string type = paramsList.Count > 2 ? paramsList[2] : "";
                var filePath = Path.Combine(GetPathByType(type), fileName.Replace("@", "."));
                var fileDownloadName = fileName.Replace(GetPathByType(type), "");
                return await DownloadFileByType(filePath, fileDownloadName);
            }
            else
            {
                throw HSZException.Oh(ErrorCode.D8000);
            }
        }

        /// <summary>
        /// App启动信息
        /// </summary>
        [HttpGet("AppStartInfo/{appName}")]
        [AllowAnonymous, IgnoreLog]
        public async Task<dynamic> AppStartInfo(string appName)
        {
            return new { appVersion = KeyVariable.AppVersion, appUpdateContent = KeyVariable.AppUpdateContent };
        }

        #endregion

        #region 多种存储文件
        /// <summary>
        /// 根据存储类型上传文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [NonAction]
        public async Task UploadFileByType(IFormFile file, string filePath, string fileName)
        {
            try
            {
                var bucketName = KeyVariable.BucketName;//桶名
                var fileStoreType = KeyVariable.FileStoreType;//文件存储类型
                var uploadPath = Path.Combine(filePath, fileName);//上传路径
                var stream = file.OpenReadStream();//文件流
                switch (fileStoreType)
                {
                    case "Invalid":
                        if (!Directory.Exists(filePath))
                            Directory.CreateDirectory(filePath);
                        using (var stream4 = File.Create(uploadPath))
                        {
                            await file.CopyToAsync(stream4);
                        }
                        break;
                    default:
                        await _oSSServiceFactory.Create(fileStoreType).PutObjectAsync(bucketName, uploadPath, stream);
                        break;
                }
            }
            catch (Exception)
            {
                throw HSZException.Oh(ErrorCode.D8003);
            }
        }

        /// <summary>
        /// 根据存储类型下载文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileDownLoadName"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<FileStreamResult> DownloadFileByType(string filePath, string fileDownLoadName)
        {
            try
            {
                var bucketName = KeyVariable.BucketName;
                var fileStoreType = KeyVariable.FileStoreType;
                switch (fileStoreType)
                {
                    case "Invalid":
                        return new FileStreamResult(new FileStream(filePath, FileMode.Open), "application/octet-stream") { FileDownloadName = fileDownLoadName };
                    default:
                        var url = await _oSSServiceFactory.Create().PresignedGetObjectAsync(bucketName, filePath, 86400);
                        var stream = await url.GetAsStreamAsync();
                        return new FileStreamResult(stream, "application/octet-stream") { FileDownloadName = fileDownLoadName };
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw HSZException.Oh(ErrorCode.D8003);
            }
        }
        #endregion

        /// <summary>
        /// 根据类型获取文件存储路径
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [NonAction]
        public string GetPathByType(string type)
        {
            switch (type)
            {
                case "userAvatar":
                    return FileVariable.UserAvatarFilePath;
                case "mail":
                    return FileVariable.EmailFilePath;
                case "IM":
                    return FileVariable.IMContentFilePath;
                case "weixin":
                    return FileVariable.MPMaterialFilePath;
                case "workFlow":
                    return FileVariable.SystemFilePath;
                case "annex":
                    return FileVariable.SystemFilePath;
                case "annexpic":
                    return FileVariable.SystemFilePath;
                case "document":
                    return FileVariable.DocumentFilePath;
                //case "dataBackup":
                //    return ConfigurationFileConst.DataBackupFilePath;
                case "preview":
                    return FileVariable.DocumentPreviewFilePath;
                case "screenShot":
                case "banner":
                case "bg":
                case "border":
                case "source":
                    return FileVariable.BiVisualPath;
                case "template":
                    return FileVariable.TemplateFilePath;
                case "codeGenerator":
                    return FileVariable.GenerateCodePath;
                default:
                    return FileVariable.TemporaryFilePath;
            }
        }

        #region kkfile 文件预览

        /// <summary>
        /// KKFile 文件预览
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string KKFileUploaderPreview(string fileName)
        {
            var domain = App.Configuration["HSZ_APP:Domain"];

            var filePath = domain + "/api/File/Image/annex/" + fileName;
            return filePath;
        }

        #endregion

        #region YoZo 生成 sign 方法
        /// <summary>
        /// 调用YoZo 文件预览 
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="maxNumber">最多请求次数</param>
        /// <param name="number">当前请求次数</param>
        /// <returns></returns>
        public async Task<string> YoZoUploaderPreview(string fileName, int maxNumber, int number)
        {
            var domain = App.Configuration["HSZ_APP:Domain"];
            var UploadAPI = App.Configuration["HSZ_APP:YOZO:UploadAPI"];
            var DownloadAPI = App.Configuration["HSZ_APP:YOZO:DownloadAPI"];
            var yozoAppId = App.Configuration["HSZ_APP:YOZO:AppId"];
            var yozoAppKey = App.Configuration["HSZ_APP:YOZO:AppKey"];
            var outputFilePath = string.Format("{0}/api/File/Image/annex/{1}", domain, fileName);
            //outputFilePath = "https://java.hszsoft.com/api/extend/DocumentPreview/down/报价单.xls";//测试文件

            #region 生成签名sign
            Dictionary<string, string[]> dic = new Dictionary<string, string[]>();
            dic.Add("fileUrl", new string[] { outputFilePath });
            dic.Add("appId", new string[] { yozoAppId });
            var sign = generateSign(yozoAppKey, dic);
            #endregion

            UploadAPI = string.Format(UploadAPI, outputFilePath, yozoAppId, sign);
            var resStr = await UploadAPI.PostAsStringAsync();
            if (resStr.IsNotEmptyOrNull())
            {
                var result = resStr.Deserialize<Dictionary<string, object>>();
                if (result.ContainsKey("data"))
                {
                    var data = result["data"].ToString().Deserialize<Dictionary<string, object>>();
                    if (data != null)
                    {
                        var fileVersionId = data.ContainsKey("fileVersionId") ? data["fileVersionId"].ToString() : "";

                        #region 生成签名sign
                        dic = new Dictionary<string, string[]>();
                        dic.Add("fileVersionId", new string[] { fileVersionId });
                        dic.Add("appId", new string[] { yozoAppId });
                        sign = generateSign(yozoAppKey, dic);
                        #endregion

                        var url = string.Format(DownloadAPI, fileVersionId, yozoAppId, sign);
                        return url;
                    }
                    else return await YoZoUploaderPreview(fileName, maxNumber, number + 1);
                }
                else
                {
                    if (number >= maxNumber) return "";
                    else return await YoZoUploaderPreview(fileName, maxNumber, number + 1);
                }
            }
            else
            {
                if (number >= maxNumber) return "";
                else return await YoZoUploaderPreview(fileName, maxNumber, number + 1);
            }
        }

        private string generateSign(string secret, Dictionary<string, string[]> paramMap)
        {
            string fullParamStr = uniqSortParams(paramMap);
            return HmacSHA256(fullParamStr, secret);
        }

        private string uniqSortParams(Dictionary<string, string[]> paramMap)
        {
            paramMap.Remove("sign");
            paramMap = paramMap.OrderBy(o => o.Key).ToDictionary(o => o.Key.ToString(), p => p.Value);
            StringBuilder strB = new StringBuilder();
            foreach (KeyValuePair<string, string[]> kvp in paramMap)
            {
                string key = kvp.Key;
                string[] value = kvp.Value;
                if (value.Length > 0)
                {
                    Array.Sort(value);
                    foreach (string temp in value)
                    {
                        strB.Append(key).Append("=").Append(temp);
                    }
                }
                else
                {
                    strB.Append(key).Append("=");
                }

            }
            return strB.ToString();
        }

        private string HmacSHA256(string data, string key)
        {
            string signRet = string.Empty;
            using (HMACSHA256 mac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hash = mac.ComputeHash(Encoding.UTF8.GetBytes(data));
                signRet = ToHexString(hash); ;
            }
            return signRet;
        }

        private string ToHexString(byte[] bytes)
        {
            string hexString = string.Empty;
            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();
                foreach (byte b in bytes)
                {
                    strB.AppendFormat("{0:X2}", b);
                }
                hexString = strB.ToString();
            }
            return hexString;
        }

        #endregion

        /// <summary>
        /// 允许文件类型
        /// </summary>
        /// <param name="fileExtension">文件后缀名</param>
        /// <param name="type">文件类型</param>
        /// <returns></returns>
        private bool AllowFileType(string fileExtension, string type)
        {
            var allowExtension = KeyVariable.AllowUploadFileType;
            if (type.Equals("weixin"))
            {
                allowExtension = KeyVariable.WeChatUploadFileType;
            }
            var isExist = allowExtension.Find(a => a == fileExtension.ToLower());
            if (!string.IsNullOrEmpty(isExist))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 允许文件类型
        /// </summary>
        /// <param name="fileExtension">文件后缀名</param>
        /// <returns></returns>
        private bool AllowImageType(string fileExtension)
        {
            var allowExtension = KeyVariable.AllowImageType;
            var isExist = allowExtension.Find(a => a == fileExtension.ToLower());
            if (!string.IsNullOrEmpty(isExist))
                return true;
            else
                return false;
        }

        #region 导入导出

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [NonAction]
        public dynamic Export(string jsonStr, string name)
        {
            var _filePath = GetPathByType("");
            var _fileName = name + Ext.GetTimeStamp + ".Json";
            if (!Directory.Exists(_filePath))
                Directory.CreateDirectory(_filePath);
            var byteList = new UTF8Encoding(true).GetBytes(jsonStr.ToCharArray());
            FileHelper.CreateFile(_filePath + _fileName, byteList);
            var fileName = _userManager.UserId + "|" + _filePath + _fileName + "|json";
            var output = new
            {
                name = _fileName,
                url = "/api/file/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [NonAction]
        public string Import(IFormFile file)
        {
            var stream = file.OpenReadStream();
            var byteList = new byte[file.Length];
            stream.Read(byteList, 0, (int)file.Length);
            stream.Position = 0;
            var sr = new StreamReader(stream, Encoding.Default);
            var json = sr.ReadToEnd();
            sr.Close();
            stream.Close();
            return json;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="file"></param>
        [NonAction]
        public void UploadFile(string type, IFormFile file)
        {
            var fileType = Path.GetExtension(file.FileName).Replace(".", "");
            if (!this.AllowFileType(fileType, type))
                throw HSZException.Oh(ErrorCode.D1800);
            var _filePath = GetPathByType(type);
            var _fileName = file.FileName;
            if (!Directory.Exists(_filePath))
                Directory.CreateDirectory(_filePath);
            using (var stream = File.Create(Path.Combine(_filePath, _fileName)))
            {
                file.CopyTo(stream);
            }
        }
        #endregion
    }
}
