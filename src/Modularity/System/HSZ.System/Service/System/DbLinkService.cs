using HSZ.Basics.Models.PlatForm.Dtos.DbLink;
using HSZ.ChangeDataBase;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Common.Util;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.System.Entitys.Dto.System.DbLink;
using HSZ.System.Entitys.Entity.System;
using HSZ.System.Entitys.Permission;
using HSZ.System.Entitys.System;
using HSZ.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.System.Core.Service.DbLink
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：数据连接
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "DataSource", Order = 205)]
    [Route("api/system/[controller]")]
    public class DbLinkService : IDbLinkService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<DbLinkEntity> _dbLinkRepository;
        private readonly IDictionaryDataService _dictionaryDataService;
        private readonly IChangeDataBase _changeDataBase;

        /// <summary>
        /// 
        /// </summary>
        public DbLinkService(ISqlSugarRepository<DbLinkEntity> dbLinkRepository, IDictionaryDataService dictionaryDataService,
            IChangeDataBase changeDataBase)
        {
            _dbLinkRepository = dbLinkRepository;
            _dictionaryDataService = dictionaryDataService;
            _changeDataBase = changeDataBase;
        }

        #region GET
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList_Api([FromQuery] DbLinkListInput input)
        {
            var data = await GetPageList(input);
            return data;
        }

        /// <summary>
        /// 下拉框列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("Selector")]
        public async Task<dynamic> GetSelector()
        {
            var data = (await GetList()).Adapt<List<DbLinkSelectorOutput>>();
            //数据库分类
            var dbTypeList = (await _dictionaryDataService.GetList("dbType")).FindAll(x => x.EnabledMark == 1);
            var output = new List<DbLinkSelectorOutput>();
            output.Add(new DbLinkSelectorOutput()
            {
                id = "-1",
                parentId = "0",
                fullName = "未分类",
                num = data.FindAll(x => x.parentId == null).Count
            });
            foreach (var item in dbTypeList)
            {
                var index = data.FindAll(x => x.dbType.Equals(item.EnCode)).Count;
                if (index > 0)
                {
                    output.Add(new DbLinkSelectorOutput()
                    {
                        id = item.Id,
                        fullName = item.FullName
                    });
                }
            }
            var treeList = output.Union(data).ToList().ToTree();
            return new { list = treeList };
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo_Api(string id)
        {
            var data = await GetInfo(id);
            var output = data.Adapt<DbLinkInfoOutput>();
            return output;
        }
        #endregion

        #region POST
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete_Api(string id)
        {
            var entity = await GetInfo(id);
            if (entity == null)
                throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await Delete(entity);
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1002);
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create_Api([FromBody] DbLinkCrInput input)
        {
            if (await _dbLinkRepository.IsAnyAsync(x => x.FullName == input.fullName && x.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.COM1004);
            var entity = input.Adapt<DbLinkEntity>();
            var isOk = await Create(entity);
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update_Api(string id, [FromBody] DbLinkUpInput input)
        {
            if (await _dbLinkRepository.IsAnyAsync(x => x.Id != id && x.FullName == input.fullName && x.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.COM1004);
            var entity = input.Adapt<DbLinkEntity>();
            var isOk = await Update(entity);
            if (isOk < 0)
                throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 测试连接
        /// </summary>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPost("Actions/Test")]
        public void TestDbConnection([FromBody] DbLinkActionsTestInput input)
        {
            var entity = input.Adapt<DbLinkEntity>();
            entity.Id = input.id.Equals("0") ? YitIdHelper.NextId().ToString() : input.id;
            var flag = _changeDataBase.IsConnection(entity);
            if (!flag)
                throw HSZException.Oh(ErrorCode.D1507);
        }

        #endregion

        #region PublicMethod
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<DbLinkListOutput>> GetList()
        {
            var list = await _dbLinkRepository.AsSugarClient().Queryable<DbLinkEntity, UserEntity, UserEntity, DictionaryDataEntity, DictionaryTypeEntity>(
                (a, b, c, d, e) => new JoinQueryInfos(
                    JoinType.Left, a.CreatorUserId == b.Id,
                    JoinType.Left, a.LastModifyUserId == c.Id,
                    JoinType.Left, a.DbType == d.EnCode && d.DeleteMark == null,
                    JoinType.Left, d.DictionaryTypeId == e.Id && e.EnCode == "dbType"))
                    .Where((a, b, c) => a.DeleteMark == null).
                    Select((a, b, c, d) => new DbLinkListOutput()
                    {
                        id = a.Id,
                        parentId = d.Id == null ? "-1" : d.Id,
                        creatorTime = a.CreatorTime,
                        creatorUser = SqlFunc.MergeString(b.RealName, "/", b.Account),
                        dbType = a.DbType,
                        enabledMark = a.EnabledMark,
                        fullName = a.FullName,
                        host = a.Host,
                        lastModifyTime = a.LastModifyTime,
                        lastModifyUser = SqlFunc.MergeString(c.RealName, "/", c.Account),
                        port = a.Port.ToString(),
                        sortCode = a.SortCode
                    }).MergeTable().Distinct().OrderBy(o => o.sortCode).OrderBy(o => o.creatorTime, OrderByType.Desc).ToListAsync();
            return list;
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [NonAction]
        public async Task<DbLinkEntity> GetInfo(string id)
        {
            return await _dbLinkRepository.GetFirstAsync(m => m.Id == id && m.DeleteMark == null);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Delete(DbLinkEntity entity)
        {
            return await _dbLinkRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Create(DbLinkEntity entity)
        {
            return await _dbLinkRepository.AsInsertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Update(DbLinkEntity entity)
        {
            return await _dbLinkRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }
        #endregion

        #region PrivateMethod

        /// <summary>
        /// 列表(分页)
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetPageList(DbLinkListInput input)
        {
            var list = await _dbLinkRepository.AsSugarClient().Queryable<DbLinkEntity, UserEntity, UserEntity>(
                (a, b, c) => new JoinQueryInfos(
                    JoinType.Left, a.CreatorUserId == b.Id,
                    JoinType.Left, a.LastModifyUserId == c.Id))
                    .Where((a, b, c) => a.DeleteMark == null).WhereIF(input.dbType.IsNotEmptyOrNull(), a => a.DbType == input.dbType).
                    WhereIF(input.keyword.IsNotEmptyOrNull(), a => a.FullName.Contains(input.keyword)).
                    Select((a, b, c) => new DbLinkListOutput()
                    {
                        id = a.Id,
                        creatorTime = a.CreatorTime,
                        creatorUser = SqlFunc.MergeString(b.RealName, "/", b.Account),
                        dbType = a.DbType,
                        enabledMark = a.EnabledMark,
                        fullName = a.FullName,
                        host = a.Host,
                        lastModifyTime = a.LastModifyTime,
                        lastModifyUser = SqlFunc.MergeString(c.RealName, "/", c.Account),
                        port = a.Port.ToString(),
                        sortCode = a.SortCode
                    }).MergeTable()
                    .Distinct().OrderBy(o => o.sortCode).OrderBy(o => o.creatorTime, OrderByType.Desc).OrderByIF(!string.IsNullOrEmpty(input.keyword), t => t.lastModifyTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<DbLinkListOutput>.SqlSugarPageResult(list);
        }
        #endregion

    }
}
