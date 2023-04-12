using HSZ.Common.Filter;
using HSZ.Common.Util;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.System.Entitys.Permission;
using HSZ.System.Entitys.System;
using HSZ.VisualDev.Entitys;
using HSZ.VisualDev.Entitys.Dto.Portal;
using HSZ.VisualDev.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Mapster;
using SqlSugar;
using System.Linq;
using System.Threading.Tasks;
using HSZ.Common.Core.Manager;
using HSZ.FriendlyException;
using HSZ.Common.Enum;
using System.Collections.Generic;
using HSZ.Common.Extension;
using HSZ.JsonSerialization;
using HSZ.System.Interfaces.Common;
using Microsoft.AspNetCore.Http;
using System;
using HSZ.VisualDev.Entitys.Dto.VisualDev;
using System.IO;

namespace HSZ.VisualDev
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：业务实现：门户设计
    /// </summary>
    [ApiDescriptionSettings(Tag = "VisualDev", Name = "Portal", Order = 173)]
    [Route("api/visualdev/[controller]")]
    public class PortalService : IPortalService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<PortalEntity> _portalRepository;
        private readonly ISqlSugarRepository<DictionaryDataEntity> _dictionaryDataRepository;// 数据字典表仓储
        private readonly ISqlSugarRepository<AuthorizeEntity> _authorizeRepository; //权限操作表仓储
        private readonly ISqlSugarRepository<RoleEntity> _roleRepository;
        private readonly IUserManager _userManager;
        private readonly IFileService _fileService;

        /// <summary>
        /// 初始化一个<see cref="PortalService"/>类型的新实例
        /// </summary>
        public PortalService(ISqlSugarRepository<PortalEntity> portalRepository, ISqlSugarRepository<RoleEntity> roleRepository, ISqlSugarRepository<DictionaryDataEntity> dictionaryDataRepository, IUserManager userManager, ISqlSugarRepository<AuthorizeEntity> authorizeRepository, IFileService fileService)
        {
            _portalRepository = portalRepository;
            _dictionaryDataRepository = dictionaryDataRepository;
            _userManager = userManager;
            _authorizeRepository = authorizeRepository;
            _roleRepository = roleRepository;
            _fileService = fileService;
        }

        #region Get

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns>返回列表</returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] VisualDevListQueryInput input)
        {
            var portalList = await _portalRepository.AsSugarClient().Queryable<PortalEntity, UserEntity, UserEntity, DictionaryDataEntity>((a, b, c, d) => new JoinQueryInfos(JoinType.Left, b.Id == a.CreatorUserId, JoinType.Left, c.Id == a.LastModifyUserId, JoinType.Left, d.Id == a.Category))
               .WhereIF(!string.IsNullOrEmpty(input.keyword), a => a.FullName.Contains(input.keyword) || a.EnCode.Contains(input.keyword))
               .WhereIF(!string.IsNullOrEmpty(input.category), a => a.Category == input.category).Where(a => a.DeleteMark == null)
               .OrderBy(a => a.SortCode, OrderByType.Asc)
               .OrderBy(a => a.CreatorTime, OrderByType.Desc)
               .OrderBy(a => a.LastModifyTime, OrderByType.Desc)
               .Select((a, b, c, d) => new PortalListOutput
               {
                   id = a.Id,
                   fullName = a.FullName,
                   enCode =
                   a.EnCode,
                   deleteMark =
                   SqlFunc.ToString(a.DeleteMark),
                   description = a.Description,
                   category = d.FullName,
                   creatorTime = a.CreatorTime,
                   creatorUser = SqlFunc.MergeString(b.RealName, "/", b.Account),
                   parentId = a.Category,
                   lastModifyUser = SqlFunc.MergeString(c.RealName, SqlFunc.IIF(c.RealName == null, "", "/"), c.Account),
                   lastModifyTime = SqlFunc.ToDate(a.LastModifyTime),
                   enabledMark = a.EnabledMark,
                   type=a.Type,
                   sortCode = SqlFunc.ToString(a.SortCode)
               })
               .ToPagedListAsync(input.currentPage, input.pageSize);

            return PageResult<PortalListOutput>.SqlSugarPageResult(portalList);
            //var parentIds = portalList.Select(x => x.parentId).ToList().Distinct();
            //var treeList = await _dictionaryDataRepository.Where(d => parentIds.Contains(d.Id) && d.DeleteMark == null && d.EnabledMark==1).OrderBy(x=>x.SortCode,OrderByType.Asc)
            //    .Select(d => new PortalListOutput { id = d.Id, parentId = "0", enCode = "", fullName = d.FullName }).ToListAsync();
            //return new { list = treeList.Union(portalList).ToList().ToTree("0") };

        }

        /// <summary>
        /// 获取门户侧边框列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("Selector")]
        public async Task<dynamic> GetSelector([FromQuery] string type)
        {
            var userInfo = await _userManager.GetUserInfo();
            var data = new List<PortalSelectOutput>();
            if ("1".Equals(type) && !userInfo.isAdministrator)
            {
                var roleId = await _roleRepository.AsQueryable().In(r => r.Id, userInfo.roleIds).Where(r => r.EnabledMark == 1 && r.DeleteMark == null).Select(r => r.Id).ToListAsync();
                var items = await _authorizeRepository.AsQueryable().In(a => a.ObjectId, roleId).Where(a => a.ItemType == "portal").GroupBy(it => new { it.ItemId }).Select(it => new { it.ItemId }).ToListAsync();
                if (items.Count != 0)
                    data = await _portalRepository.AsQueryable().In(p => p.Id, items.Select(it => it.ItemId).ToArray())
                        .Where(p => p.EnabledMark == 1 && p.DeleteMark == null).OrderBy(p => p.SortCode)
                        .Select(s => new PortalSelectOutput
                        {
                            id = s.Id,
                            fullName = s.FullName,
                            parentId = s.Category
                        }).ToListAsync();
            }
            else
            {
                data = await _portalRepository.AsQueryable()
                    .Where(p => p.EnabledMark == 1 && p.DeleteMark == null).OrderBy(o => o.SortCode)
                    .Select(s => new PortalSelectOutput
                    {
                        id = s.Id,
                        fullName = s.FullName,
                        parentId = s.Category,
                    }).ToListAsync();
            }
            var parentIds = data.Select(it => it.parentId).Distinct().ToList();
            var treeList = new List<PortalSelectOutput>();
            if (parentIds.Count() != 0)
                treeList = await _dictionaryDataRepository.AsQueryable().In(it => it.Id, parentIds.ToArray())
                    .Where(d => d.DeleteMark == null && d.EnabledMark == 1).OrderBy(o => o.SortCode)
                    .Select(d => new PortalSelectOutput
                    {
                        id = d.Id,
                        parentId = "0",
                        fullName = d.FullName
                    }).ToListAsync();
            return new { list = treeList.Union(data).ToList().ToTree("0") };
        }

        /// <summary>
        /// 获取门户信息
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var data = await _portalRepository.GetSingleAsync(p => p.Id == id);
            var output = data.Adapt<PortalInfoOutput>();
            return output;
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/auth")]
        public async Task<dynamic> GetInfoAuth(string id)
        {
            var userInfo = await _userManager.GetUserInfo();
            if (userInfo.roleIds != null && !userInfo.isAdministrator)
            {
                var roleId = await _roleRepository.AsQueryable().In(r => r.Id, userInfo.roleIds).Where(r => r.EnabledMark==1 && r.DeleteMark == null).Select(r => r.Id).ToListAsync();
                var items = await _authorizeRepository.AsQueryable().In(a => a.ObjectId, roleId).Where(a => a.ItemType == "portal").GroupBy(it => new { it.ItemId }).Select(it => new { it.ItemId }).ToListAsync();
                if (items.Count == 0) return null;
                var entity = await _portalRepository.AsQueryable().In(p => p.Id, items.Select(it => it.ItemId).ToArray()).SingleAsync(p => p.Id == id && p.EnabledMark==1 && p.DeleteMark == null);
                _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
                var res = entity.Adapt<PortalInfoAuthOutput>();
                return res;
            }
            if (userInfo.isAdministrator)
            {
                var entity = await _portalRepository.GetSingleAsync(p => p.Id == id && p.EnabledMark==1 && p.DeleteMark == null);
                _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
                var res = entity.Adapt<PortalInfoAuthOutput>();
                return res;
            }
            throw HSZException.Oh(ErrorCode.D1900);
        }

        #endregion

        #region Post

        /// <summary>
        /// 门户导出
        /// </summary>
        /// <param name="modelId"></param>
        /// <returns></returns>
        [HttpPost("{modelId}/Actions/ExportData")]
        public async Task<dynamic> ActionsExportData(string modelId)
        {
            //模板实体
            var templateEntity = await _portalRepository.GetSingleAsync(p => p.Id == modelId);
            var jsonStr = templateEntity.Serialize();
            return _fileService.Export(jsonStr, templateEntity.FullName);
        }

        /// <summary>
        /// 门户导入
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("Model/Actions/ImportData")]
        public async Task ActionsImportData(IFormFile file)
        {
            var fileType = Path.GetExtension(file.FileName).Replace(".", "");
            if (!fileType.ToLower().Equals("json"))
                throw HSZException.Oh(ErrorCode.D3006);
            var josn = _fileService.Import(file);
            PortalEntity templateEntity = null;
            try
            {
                templateEntity = josn.Deserialize<PortalEntity>();
            }
            catch
            {
                throw HSZException.Oh(ErrorCode.D3006);
            }
            if (templateEntity == null)
                throw HSZException.Oh(ErrorCode.D3006);
            if (templateEntity != null && templateEntity.FormData.IsNotEmptyOrNull() && templateEntity.FormData.IndexOf("layouyId") <= 0)
                throw HSZException.Oh(ErrorCode.D3006);
            if (!string.IsNullOrEmpty(templateEntity.Id) && await _portalRepository.IsAnyAsync(it => it.Id == templateEntity.Id && it.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.D1400);
            if (await _portalRepository.IsAnyAsync(it => it.EnCode == templateEntity.EnCode && it.FullName == templateEntity.FullName && it.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.D1400);
            var stor = _portalRepository.AsSugarClient().Storageable(templateEntity).Saveable().ToStorage(); //存在更新不存在插入 根据主键

            await stor.AsInsertable.ExecuteCommandAsync(); //执行插入
            //await stor.AsUpdateable.ExecuteCommandAsync(); //执行更新　
            await _portalRepository.AsUpdateable(templateEntity).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 新建门户信息
        /// </summary>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] PortalCrInput input)
        {
            var entity = input.Adapt<PortalEntity>();
            if (string.IsNullOrEmpty(entity.Category))
                throw HSZException.Oh(ErrorCode.D1901);
            else if (string.IsNullOrEmpty(entity.FullName))
                throw HSZException.Oh(ErrorCode.D1902);
            else if (string.IsNullOrEmpty(entity.EnCode))
                throw HSZException.Oh(ErrorCode.D1903);
            else
                await _portalRepository.AsInsertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 修改接口
        /// </summary>
        /// <param name="id">主键id</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] PortalUpInput input)
        {
            var entity = input.Adapt<PortalEntity>();
            var isOk = await _portalRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除接口
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _portalRepository.GetSingleAsync(p => p.Id == id && p.DeleteMark == null);
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _portalRepository.AsUpdateable(entity).UpdateColumns(it => new { it.DeleteMark, it.DeleteUserId, it.DeleteTime }).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpPost("{id}/Actions/Copy")]
        public async Task ActionsCopy(string id)
        {
            var newEntity = new PortalEntity();
            var random = new Random().NextLetterAndNumberString(5);
            var entity = await _portalRepository.GetSingleAsync(p => p.Id == id && p.DeleteMark == null);
            newEntity.FullName = entity.FullName + "副本" + random;
            newEntity.EnCode = entity.EnCode + random;
            newEntity.Category = entity.Category;
            newEntity.FormData = entity.FormData;
            newEntity.Description = entity.Description;
            newEntity.EnabledMark = 0;
            newEntity.SortCode = entity.SortCode;
            newEntity.Type = entity.Type;
            newEntity.LinkType = entity.LinkType;
            newEntity.CustomUrl = entity.CustomUrl;

            try
            {
                var isOk = await _portalRepository.AsInsertable(newEntity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
            }
            catch
            {
                if (entity.FullName.Length >= 100 || entity.EnCode.Length >= 50)//数据长度超过 字段设定长度
                    throw HSZException.Oh(ErrorCode.D1403);
                else
                    throw;
            }
        }

        /// <summary>
        /// 设置默认门户
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}/Actions/SetDefault")]
        public async Task SetDefault(string id)
        {
            var userEntity = _userManager.User;
            _ = userEntity ?? throw HSZException.Oh(ErrorCode.D5002);
            if (userEntity != null)
            {
                userEntity.PortalId = id;
                var isOk = await _portalRepository.AsSugarClient().Updateable<UserEntity>().SetColumns(it => new UserEntity()
                {
                    PortalId = id,
                    LastModifyTime = SqlFunc.GetDate(),
                    LastModifyUserId = _userManager.UserId
                }).Where(it => it.Id == userEntity.Id).ExecuteCommandAsync();
                if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.D5014);
            }
        }

        #endregion

        #region PublicMethod

        /// <summary>
        /// 获取默认
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<string> GetDefault()
        {
            var user = _userManager.User;
            if (!user.IsAdministrator.ToBool())
            {
                if (!string.IsNullOrEmpty(user.RoleId))
                {
                    var roleIds = user.RoleId.Split(',');
                    var roleId = await _roleRepository.AsQueryable().In(r => r.Id, roleIds).Where(r => r.EnabledMark==1 && r.DeleteMark == null).Select(r => r.Id).ToListAsync();
                    var items = await _authorizeRepository.AsQueryable().In(a => a.ObjectId, roleId).Where(a => a.ItemType == "portal").GroupBy(it => new { it.ItemId }).Select(it => new { it.ItemId }).ToListAsync();
                    if (items.Count == 0) return null;
                    var portalList = await _portalRepository.AsQueryable().In(p => p.Id, items.Select(it => it.ItemId).ToArray()).Where(p => p.EnabledMark==1 && p.DeleteMark == null).OrderBy(o => o.SortCode).Select(s => s.Id).ToListAsync();
                    return portalList.FirstOrDefault();
                }
                return null;
            }
            else
            {
                var portalList = await _portalRepository.AsQueryable().Where(p => p.EnabledMark==1 && p.DeleteMark == null).OrderBy(o => o.SortCode).Select(s => s.Id).ToListAsync();
                return portalList.FirstOrDefault();
            }
        }

        #endregion
    }
}