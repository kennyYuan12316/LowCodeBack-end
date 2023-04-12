using HSZ.Common.Configuration;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Helper;
using HSZ.Common.Util;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.Logging.Attributes;
using HSZ.VisualData.Entity;
using HSZ.VisualData.Entitys.Dto.Screen;
using HSZ.VisualData.Entitys.Dto.ScreenCategory;
using HSZ.VisualData.Entitys.Dto.ScreenConfig;
using HSZ.VisualData.Entitys.Enum;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.VisualData
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：业务实现：大屏
    /// </summary>
    [ApiDescriptionSettings(Tag = "BladeVisual", Name = "Visual", Order = 160)]
    [Route("api/blade-visual/[controller]")]
    public class ScreenService : IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<VisualEntity> _visualRepository;
        private readonly ISqlSugarRepository<VisualConfigEntity> _visualConfigRepository;
        private readonly ISqlSugarRepository<VisualCategoryEntity> _visualCategoryRepository;
        private readonly SqlSugarScope Db;

        /// <summary>
        /// 初始化一个<see cref="ScreenService"/>类型的新实例
        /// </summary>
        public ScreenService(ISqlSugarRepository<VisualEntity> visualRepository,
            ISqlSugarRepository<VisualConfigEntity> visualConfigRepository,
            ISqlSugarRepository<VisualCategoryEntity> visualCategoryRepository)
        {
            _visualRepository = visualRepository;
            _visualConfigRepository = visualConfigRepository;
            _visualCategoryRepository = visualCategoryRepository;
            Db = DbScoped.SugarScope;
        }

        #region Get

        /// <summary>
        /// 分页
        /// </summary>
        /// <returns></returns>
        [HttpGet("list")]
        public async Task<dynamic> GetList([FromQuery] ScreenListQueryInput input)
        {
            var data = await _visualRepository.AsQueryable().Where(v => v.IsDeleted == 0).Where(v => v.Category == input.category)
                .Select(v => new ScreenListOutput
                {
                    id = v.Id,
                    backgroundUrl = v.BackgroundUrl,
                    category = v.Category,
                    createDept = v.CreateDept,
                    createTime = SqlFunc.ToString(v.CreateTime),
                    createUser = v.CreateUser,
                    isDeleted = v.IsDeleted,
                    password = v.Password,
                    status = v.Status,
                    title = v.Title,
                    updateTime = SqlFunc.ToString(v.UpdateTime),
                    updateUser = v.UpdateUser
                }).ToPagedListAsync(input.current, input.size);
            return new { current = data.pagination.PageIndex, pages = data.pagination.Total / data.pagination.PageSize, records = data.list, size = data.pagination.PageSize, total = data.pagination.Total };
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <returns></returns>
        [HttpGet("detail")]
        public async Task<dynamic> GetInfo([FromQuery] string id)
        {
            var entity = await _visualRepository.GetFirstAsync(v => v.Id == id);
            var configEntity = await _visualConfigRepository.GetFirstAsync(v => v.VisualId == id);
            return new { config = configEntity.Adapt<ScreenConfigInfoOutput>(), visual = entity.Adapt<ScreenInfoOutput>() };
        }

        /// <summary>
        /// 获取类型
        /// </summary>
        /// <returns></returns>
        [HttpGet("category")]
        public async Task<dynamic> GetCategoryList()
        {
            var entity = await _visualCategoryRepository.GetListAsync(v => v.IsDeleted==0);
            return entity.Adapt<ScreenCategoryListOutput>();
        }

        /// <summary>
        /// 获取图片列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("{type}")]
        public dynamic GetImgFileList(string type)
        {
            var list = new List<ScreenImgFileOutput>();
            var typeEnum = EnumExtensions.GetEnumDescDictionary(typeof(ScreenImgEnum));
            var imgEnum = typeEnum.Where(t => t.Value.Equals(type)).FirstOrDefault();
            if (imgEnum.Value != null)
            {
                var path = Path.Combine(FileVariable.BiVisualPath, imgEnum.Value);
                var fileinfos = FileHelper.GetAllFiles(path);
                foreach (var item in fileinfos)
                {
                    list.Add(new ScreenImgFileOutput()
                    {
                        link = string.Format(@"/api/file/VisusalImg/{0}/{1}", type, item.Name),
                        originalName = item.Name
                    });
                }
            }
            return list;
        }

        /// <summary>
        /// 查看图片
        /// </summary>
        /// <returns></returns>
        [HttpGet("{type}/{fileName}"), AllowAnonymous, IgnoreLog]
        public dynamic GetImgFile(string type, string fileName)
        {
            var typeEnum = EnumExtensions.GetEnumDescDictionary(typeof(ScreenImgEnum));
            var imgEnum = typeEnum.Where(t => t.Value.Equals(type)).FirstOrDefault();
            if (imgEnum.Value != null)
            {
                var path = Path.Combine(FileVariable.BiVisualPath, imgEnum.Value, fileName);
                return new FileStreamResult(new FileStream(path, FileMode.Open), "application/octet-stream") { FileDownloadName = fileName };
            }
            return Task.FromResult(false);
        }

        /// <summary>
        /// 大屏下拉框
        /// </summary>
        /// <returns></returns>
        [HttpGet("selector")]
        public async Task<dynamic> GetSelector()
        {
            var screenList = await _visualRepository.AsQueryable().Where(v => v.IsDeleted==0)
                .Select(v => new ScreenSelectorOuput
                {
                    id = v.Id,
                    parentId =SqlFunc.ToString(v.Category),
                    fullName = v.Title,
                    isDeleted = v.IsDeleted
                }).ToListAsync();
            var categortList = await _visualCategoryRepository.AsQueryable().Where(v => v.IsDeleted == 0)
                .Select(v => new ScreenSelectorOuput
                {
                    id = v.CategoryValue,
                    parentId = "0",
                    fullName = v.CategoryKey,
                    isDeleted = v.IsDeleted
                }).ToListAsync();
            return new { list = categortList.Union(screenList).ToList().ToTree("0") };
        }

        #endregion

        #region Post

        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        [HttpPost("save")]
        public async Task<dynamic> Save([FromBody] ScreenCrInput input)
        {
            var entity = input.visual.Adapt<VisualEntity>();
            var configEntity = input.config.Adapt<VisualConfigEntity>();
            try
            {
                //开启事务
                Db.BeginTran();

                var newEntity = await _visualRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).CallEntityMethod(m => m.Create()).ExecuteReturnEntityAsync();
                configEntity.VisualId = newEntity.Id;
                await _visualConfigRepository.AsInsertable(configEntity).IgnoreColumns(ignoreNullColumn: true).CallEntityMethod(m => m.Create()).ExecuteReturnEntityAsync();

                Db.CommitTran();

                return new { id = newEntity.Id };
            }
            catch (Exception)
            {
                Db.RollbackTran();
                throw HSZException.Oh(ErrorCode.COM1000);
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        [HttpPost("update")]
        public async Task Update([FromBody] ScreenUpInput input)
        {
            var entity = new VisualEntity();
            if (input.visual.backgroundUrl != null)
            {
                entity = await _visualRepository.GetFirstAsync(v => v.Id == input.visual.id);
                entity.BackgroundUrl = input.visual.backgroundUrl;
            }
            else
            {
                entity = input.visual.Adapt<VisualEntity>();
            }
            var configEntity = input.config.Adapt<VisualConfigEntity>();
            try
            {
                //开启事务
                Db.BeginTran();

                await _visualRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
                if (configEntity != null)
                    await _visualConfigRepository.AsUpdateable(configEntity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();

                Db.CommitTran();
            }
            catch (Exception)
            {
                Db.RollbackTran();
                throw HSZException.Oh(ErrorCode.COM1000);
            }
        }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        /// <returns></returns>
        [HttpPost("remove")]
        public async Task Remove(string ids)
        {
            var entity = await _visualRepository.GetFirstAsync(v => v.Id == ids);
            await _visualRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        [HttpPost("copy")]
        public async Task<dynamic> Copy(string id)
        {
            var entity = await _visualRepository.GetFirstAsync(v => v.Id == id);
            var configEntity = await _visualConfigRepository.GetFirstAsync(v => v.VisualId == id);
            try
            {
                //开启事务
                Db.BeginTran();

                var newEntity = await _visualRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).CallEntityMethod(m => m.Create()).ExecuteReturnEntityAsync();
                configEntity.VisualId = newEntity.Id;
                await _visualConfigRepository.AsInsertable(configEntity).IgnoreColumns(ignoreNullColumn: true).CallEntityMethod(m => m.Create()).ExecuteReturnEntityAsync();

                Db.CommitTran();

                return new { id = newEntity.Id };
            }
            catch (Exception)
            {
                Db.RollbackTran();
                throw HSZException.Oh(ErrorCode.COM1000);
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        [HttpPost("put-file/{type}"), AllowAnonymous, IgnoreLog]
        public async Task<dynamic> SaveFile(string type, IFormFile file)
        {
            var typeEnum = EnumExtensions.GetEnumDescDictionary(typeof(ScreenImgEnum));
            var imgEnum = typeEnum.Where(t => t.Value.Equals(type)).FirstOrDefault();
            if (imgEnum.Value != null)
            {
                var ImgType = Path.GetExtension(file.FileName).Replace(".", "");
                if (!this.AllowImageType(ImgType))
                    throw HSZException.Oh(ErrorCode.D5013);
                var path = imgEnum.Value;
                var filePath = Path.Combine(FileVariable.BiVisualPath, path);
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);
                var fileName = YitIdHelper.NextId().ToString() + "." + ImgType;
                using (var stream = File.Create(Path.Combine(filePath, fileName)))
                {
                    await file.CopyToAsync(stream);
                }
                return new { name = "/" + Path.Combine("api", "file", "VisusalImg", path, fileName), link = "/" + Path.Combine("api", "file", "VisusalImg", path, fileName), originalName = file.FileName };
            }
            return Task.FromResult(false);
        }

        #endregion

        #region PrivateMethod

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

        #endregion
    }
}