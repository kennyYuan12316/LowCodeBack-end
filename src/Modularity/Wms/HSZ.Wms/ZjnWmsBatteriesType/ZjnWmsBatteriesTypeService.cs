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
using HSZ.wms.Entitys.Dto.ZjnWmsBatteriesType;
using HSZ.wms.Interfaces.ZjnWmsBatteriesType;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnWmsBatteriesType
{
    /// <summary>
    /// 电芯种类静置配置服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnWmsBatteriesType", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnWmsBatteriesTypeService : IZjnWmsBatteriesTypeService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWmsBatteriesTypeEntity> _zjnWmsBatteriesTypeRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnWmsBatteriesTypeService"/>类型的新实例
        /// </summary>
        public ZjnWmsBatteriesTypeService(ISqlSugarRepository<ZjnWmsBatteriesTypeEntity> zjnWmsBatteriesTypeRepository,
            IUserManager userManager)
        {
            _zjnWmsBatteriesTypeRepository = zjnWmsBatteriesTypeRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取电芯种类静置配置
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnWmsBatteriesTypeRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnWmsBatteriesTypeInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取电芯种类静置配置列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnWmsBatteriesTypeListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnWmsBatteriesTypeRepository.AsSugarClient().Queryable<ZjnWmsBatteriesTypeEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_BatteriesType), a => a.BatteriesType.Equals(input.F_BatteriesType))
                .WhereIF(!string.IsNullOrEmpty(input.F_StandingTime), a => a.StandingTime.Equals(input.F_StandingTime))
                .Select((a
)=> new ZjnWmsBatteriesTypeListOutput
                {
                    F_Id = a.Id,
                    F_BatteriesType = a.BatteriesType,
                    F_StandingTime = a.StandingTime,
                    F_EnabledMark = SqlFunc.IIF(a.EnabledMark == 0, "关", "开"),
                    F_Description = a.Description,
                    F_CreateTime = a.CreateTime,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnWmsBatteriesTypeListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建电芯种类静置配置
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnWmsBatteriesTypeCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnWmsBatteriesTypeEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            
            var isOk = await _zjnWmsBatteriesTypeRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 更新电芯种类静置配置
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnWmsBatteriesTypeUpInput input)
        {
            var entity = input.Adapt<ZjnWmsBatteriesTypeEntity>();
            var isOk = await _zjnWmsBatteriesTypeRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除电芯种类静置配置
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnWmsBatteriesTypeRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnWmsBatteriesTypeRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


