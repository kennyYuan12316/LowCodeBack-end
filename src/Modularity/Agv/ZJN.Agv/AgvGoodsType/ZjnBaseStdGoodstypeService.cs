using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Common.Helper;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.JsonSerialization;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;
using ZJN.Agv.Entitys.Dto.AgvGoodsType;
using ZJN.Agv.Entitys.Entity;
using ZJN.Agv.Interfaces;
using HSZ.wms.Entitys;
using Microsoft.AspNetCore.Authorization;
using HSZ.Entitys.wms;

namespace ZJN.Agv.AgvGoodsType
{
    /// <summary>
    /// Agv请求物料类型服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "Agv",Name = "ZjnBaseStdGoodstype", Order = 200)]
    [Route("api/agv/[controller]")]
    public class ZjnBaseStdGoodstypeService : IZjnBaseStdGoodstypeService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnBaseStdGoodstypeEntity> _zjnBaseStdGoodstypeRepository;

        private readonly ISqlSugarRepository<ZjnWmsGoodsEntity> _goodsInfo;

        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnBaseStdGoodstypeService"/>类型的新实例
        /// </summary>
        public ZjnBaseStdGoodstypeService(ISqlSugarRepository<ZjnBaseStdGoodstypeEntity> zjnBaseStdGoodstypeRepository,
            ISqlSugarRepository<ZjnWmsGoodsEntity> goodsInfo,
            IUserManager userManager)
        {
            _zjnBaseStdGoodstypeRepository = zjnBaseStdGoodstypeRepository;
            _goodsInfo = goodsInfo;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取Agv请求物料类型
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnBaseStdGoodstypeRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnBaseStdGoodstypeInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取Agv请求物料类型列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnBaseStdGoodstypeListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            List<string> queryRequestTime = input.F_RequestTime != null ? input.F_RequestTime.Split(',').ToObject<List<string>>() : null;
            DateTime? startRequestTime = queryRequestTime != null ? Ext.GetDateTime(queryRequestTime.First()) : null;
            DateTime? endRequestTime = queryRequestTime != null ? Ext.GetDateTime(queryRequestTime.Last()) : null;
            var data = await _zjnBaseStdGoodstypeRepository.AsSugarClient().Queryable<ZjnBaseStdGoodstypeEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_RequestId), a => a.RequestId.Contains(input.F_RequestId))
                .WhereIF(queryRequestTime != null, a => a.RequestTime >= new DateTime(startRequestTime.ToDate().Year, startRequestTime.ToDate().Month, startRequestTime.ToDate().Day, startRequestTime.ToDate().Hour, startRequestTime.ToDate().Minute, startRequestTime.ToDate().Second))
                .WhereIF(queryRequestTime != null, a => a.RequestTime <= new DateTime(endRequestTime.ToDate().Year, endRequestTime.ToDate().Month, endRequestTime.ToDate().Day, endRequestTime.ToDate().Hour, endRequestTime.ToDate().Minute, endRequestTime.ToDate().Second))
                .WhereIF(!string.IsNullOrEmpty(input.F_GoodsCode), a => a.GoodsCode.Contains(input.F_GoodsCode))
                .Select((a
)=> new ZjnBaseStdGoodstypeListOutput
                {
                    F_Id = a.Id,
                    F_RequestId = a.RequestId,
                    F_ClientCode = a.ClientCode,
                    F_ChannelId = a.ChannelId,
                    F_RequestTime = a.RequestTime,
                    F_GoodsCode = a.GoodsCode,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnBaseStdGoodstypeListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建Agv请求物料类型
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        [AllowAnonymous]
        public async Task<dynamic> Create([FromBody] ZjnBaseStdGoodstypeCrInput input)
        {
            //var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnBaseStdGoodstypeEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            
            var isOk = await _zjnBaseStdGoodstypeRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            RESTfulResult_AgvGoodsType result_AgvGoodsType = new RESTfulResult_AgvGoodsType();
            if (!(isOk > 0))
            {
                //throw HSZException.Oh(ErrorCode.COM1000);
                result_AgvGoodsType.Code = "404";
                result_AgvGoodsType.Msg = ErrorCode.COM1000.ToString();
            }
            else
            {
                //获取货物信息
                var goodsLists = _goodsInfo.AsSugarClient().Queryable<ZjnWmsGoodsEntity>().
                    WhereIF(!string.IsNullOrEmpty(input.goodsCode), a => a.GoodsCode.Contains(input.goodsCode)).Select((a) =>new GoodsList
                    {
                        GoodsId=a.GoodsCode,
                        GoodsName=a.GoodsName,
                    }
                    ).ToArray();

                result_AgvGoodsType.Code = "200";
                result_AgvGoodsType.Msg = "Success";
                result_AgvGoodsType.GoodsLists = goodsLists;

            }
            return result_AgvGoodsType;
        }

        /// <summary>
        /// 更新Agv请求物料类型
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnBaseStdGoodstypeUpInput input)
        {
            var entity = input.Adapt<ZjnBaseStdGoodstypeEntity>();
            var isOk = await _zjnBaseStdGoodstypeRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除Agv请求物料类型
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnBaseStdGoodstypeRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnBaseStdGoodstypeRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


