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
using HSZ.wms.Entitys.Dto.ZjnWcsPlc;
using HSZ.wms.Interfaces.ZjnWcsPlc;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnWcsPlc
{
    /// <summary>
    /// PLC连接表服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnWcsPlc", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnWcsPlcService : IZjnWcsPlcService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWcsPlcEntity> _zjnWcsPlcRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnWcsPlcService"/>类型的新实例
        /// </summary>
        public ZjnWcsPlcService(ISqlSugarRepository<ZjnWcsPlcEntity> zjnWcsPlcRepository,
            IUserManager userManager)
        {
            _zjnWcsPlcRepository = zjnWcsPlcRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 判断plc编号是否存在
        /// </summary>
        /// <param name="PlcId"></param>
        /// <returns></returns>
        [HttpGet("ExistPlcId")]
        public async Task<bool> ExistPlcId(string PlcId)
        {
            var output = await _zjnWcsPlcRepository.IsAnyAsync(p => p.PlcId == PlcId);
            return output;
        }

        /// <summary>
        /// 获取PLC连接表
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnWcsPlcRepository.GetFirstAsync(p => p.PlcId == id)).Adapt<ZjnWcsPlcInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取PLC连接表列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnWcsPlcListQueryInput input)
        {
            var sidx = input.sidx == null ? "PlcID" : input.sidx;
            var data = await _zjnWcsPlcRepository.AsSugarClient().Queryable<ZjnWcsPlcEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.Caption), a => a.Caption.Equals(input.Caption))
                .WhereIF(!string.IsNullOrEmpty(input.Region), a => a.Region.Equals(input.Region))
                .WhereIF(!string.IsNullOrEmpty(input.IP), a => a.Ip.Contains(input.IP))
                .WhereIF(!string.IsNullOrEmpty(input.plcId), a => a.Ip.Contains(input.plcId))

                .Select((a
)=> new ZjnWcsPlcListOutput
                {
                    PlcID = a.PlcId,
                    IsActive = SqlFunc.IIF(a.IsActive == false, "关", "开"),
                    Caption = a.Caption,
                    Region = a.Region,
                    IsConnected = SqlFunc.IIF(a.IsConnected == false, "关", "开"),
                    CpuType = a.CpuType,
                    IP = a.Ip,
                    Port = a.Port,
                    Rack = a.Rack,
                    Sock = a.Sock,
                    TimeOut = a.TimeOut,
                    StackerID = a.StackerId,
                    IsStacker = SqlFunc.IIF(a.IsStacker == false, "关", "开"),
                    Error = a.Error,
                    Descrip = a.Descrip,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnWcsPlcListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建PLC连接表
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnWcsPlcCrInput input)
        {
            if (await this.ExistPlcId(input.plcId)) throw HSZException.Oh(ErrorCode.COM1004);

            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnWcsPlcEntity>();
            //entity.PlcId = YitIdHelper.NextId().ToString();
            
            var isOk = await _zjnWcsPlcRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 更新PLC连接表
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnWcsPlcUpInput input)
        {
            var entity = input.Adapt<ZjnWcsPlcEntity>();
            var isOk = await _zjnWcsPlcRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除PLC连接表
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnWcsPlcRepository.GetFirstAsync(p => p.PlcId.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnWcsPlcRepository.AsDeleteable().Where(d => d.PlcId == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


