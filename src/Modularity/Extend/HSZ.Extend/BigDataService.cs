using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Extend.Entitys;
using HSZ.Extend.Entitys.Dto.BigData;
using HSZ.FriendlyException;
using HSZ.LinqBuilder;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.Extend
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：大数据测试
    /// </summary>
    [ApiDescriptionSettings(Tag = "Extend", Name = "BigData", Order = 600)]
    [Route("api/extend/[controller]")]
    public class BigDataService : IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<BigDataEntity> _bigDataRepository;
        private readonly SqlSugarScope db;// 核心对象：拥有完整的SqlSugar全部功能

        /// <summary>
        /// 初始化一个<see cref="BigDataService"/>类型的新实例
        /// </summary>
        public BigDataService(ISqlSugarRepository<BigDataEntity> bigDataRepository)
        {
            _bigDataRepository = bigDataRepository;
            db = DbScoped.SugarScope;
        }

        #region GET
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] PageInputBase input)
        {
            var queryWhere = LinqExpression.And<BigDataEntity>();
            if (!string.IsNullOrEmpty(input.keyword))
                queryWhere = queryWhere.And(m => m.FullName.Contains(input.keyword) || m.EnCode.Contains(input.keyword));
            var list = await _bigDataRepository.AsQueryable().Where(queryWhere).OrderBy(x => x.CreatorTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            var pageList = new SqlSugarPagedList<BigDataListOutput>()
            {
                list = list.list.Adapt<List<BigDataListOutput>>(),
                pagination = list.pagination
            };
            return PageResult<BigDataListOutput>.SqlSugarPageResult(pageList);
        }
        #endregion

        #region POST

        /// <summary>
        /// 新建
        /// </summary>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create()
        {
            var list = await _bigDataRepository.GetListAsync();
            var code = 0;
            if (list.Count > 0)
            {
                code = list.Select(x => x.EnCode).ToList().Max().ToInt();
            }
            var index = code == 0 ? 10000001 : code;
            if (index > 11500001)
                throw HSZException.Oh(ErrorCode.Ex0001);
            List<BigDataEntity> entityList = new List<BigDataEntity>();
            for (int i = 0; i < 10000; i++)
            {
                entityList.Add(new BigDataEntity
                {
                    Id = YitIdHelper.NextId().ToString(),
                    EnCode = index.ToString(),
                    FullName = "测试大数据" + index,
                    CreatorTime = DateTime.Now,
                });
                index++;
            }
            Blukcopy(entityList);
        }

        #endregion

        #region PrivateMethod

        /// <summary>
        /// 大数据批量插入
        /// </summary>
        /// <param name="entityList"></param>
        private void Blukcopy(List<BigDataEntity> entityList)
        {
            try
            {
                var storageable = _bigDataRepository.AsSugarClient().Storageable(entityList).SplitInsert(x => true).ToStorage();
                switch (_bigDataRepository.AsSugarClient().CurrentConnectionConfig.DbType)
                {
                    case DbType.Dm:
                        storageable.AsInsertable.ExecuteCommand();
                        break;
                    case DbType.Kdbndp:
                        storageable.AsInsertable.ExecuteCommand();
                        break;
                    case DbType.Oracle:
                        _bigDataRepository.AsSugarClient().Storageable(entityList).ToStorage().BulkCopy();
                        break;
                    default:
                        _bigDataRepository.AsSugarClient().Fastest<BigDataEntity>().BulkCopy(entityList);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        #endregion
    }
}
