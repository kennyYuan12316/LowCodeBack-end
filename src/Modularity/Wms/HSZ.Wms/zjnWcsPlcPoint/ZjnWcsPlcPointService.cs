using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Common.Helper;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Entitys.wms;
using HSZ.FriendlyException;
using HSZ.JsonSerialization;
using HSZ.wms.Entitys.Dto.ZjnWcsPlcPoint;
using HSZ.wms.Interfaces.ZjnWcsPlcPoint;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnWcsPlcPoint
{
    /// <summary>
    /// PLC点位表服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnWcsPlcPoint", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnWcsPlcPointService : IZjnWcsPlcPointService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWcsPlcPointEntity> _zjnWcsPlcPointRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnWcsPlcPointService"/>类型的新实例
        /// </summary>
        public ZjnWcsPlcPointService(ISqlSugarRepository<ZjnWcsPlcPointEntity> zjnWcsPlcPointRepository,
            IUserManager userManager)
        {
            _zjnWcsPlcPointRepository = zjnWcsPlcPointRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取PLC点位表
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnWcsPlcPointRepository.GetFirstAsync(p => p.PlcPointId.Equals(id))).Adapt<ZjnWcsPlcPointInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取PLC点位表列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnWcsPlcPointListQueryInput input)
        {
            var sidx = input.sidx == null ? "PlcPointID" : input.sidx;
            List<object> queryDb = input.Db != null ? input.Db.Split(',').ToObject<List<object>>() : null;
            var startDb = input.Db != null && !string.IsNullOrEmpty(queryDb.First().ToString()) ? queryDb.First() : decimal.MinValue;
            var endDb = input.Db != null && !string.IsNullOrEmpty(queryDb.Last().ToString()) ? queryDb.Last() : decimal.MaxValue;
            var data = await _zjnWcsPlcPointRepository.AsSugarClient().Queryable<ZjnWcsPlcPointEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.Caption), a => a.Caption.Contains(input.Caption))
                .WhereIF(!string.IsNullOrEmpty(input.PlcID), a => a.PlcId.Contains(input.PlcID))
                .WhereIF(!string.IsNullOrEmpty(input.ObjType), a => a.ObjType.Contains(input.ObjType))
                .WhereIF(!string.IsNullOrEmpty(input.Region), a => a.Region.Equals(input.Region))
                .WhereIF(input.Start!=null, a => a.Start.Equals(input.Start))
                .WhereIF(queryDb != null, a => SqlFunc.Between(a.Db, startDb, endDb))
                .Select((a
)=> new ZjnWcsPlcPointListOutput
                {
                    IsActive = SqlFunc.IIF(a.IsActive == false, "关", "开"),
                    Caption = a.Caption,
                    Region = a.Region,
                    PlcID = a.PlcId,
                    Db = a.Db,
                    Start = a.Start,
                    Length = a.Length,
                    ObjType = a.ObjType,
                    IsList = SqlFunc.IIF(a.IsList == false, "关", "开"),
                    ListCount = a.ListCount,
                    ObjValue = a.ObjValue,
                    PackType = a.PackType,
                    PackSize = a.PackSize,
                    Descrip = a.Descrip,
                    PlcPointID = a.PlcPointId,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnWcsPlcPointListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建PLC点位表
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnWcsPlcPointCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnWcsPlcPointEntity>();
            entity.PlcPointId = YitIdHelper.NextId().ToString();
            
            var isOk = await _zjnWcsPlcPointRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 更新PLC点位表
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnWcsPlcPointUpInput input)
        {
            var entity = input.Adapt<ZjnWcsPlcPointEntity>();
            var isOk = await _zjnWcsPlcPointRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除PLC点位表
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnWcsPlcPointRepository.GetFirstAsync(p => p.PlcPointId.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnWcsPlcPointRepository.AsDeleteable().Where(d => d.PlcPointId.Equals(id)).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


