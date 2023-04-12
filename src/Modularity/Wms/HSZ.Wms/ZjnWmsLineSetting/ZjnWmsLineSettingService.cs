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
using HSZ.System.Entitys.System;
using HSZ.wms.Entitys.Dto.ZjnWmsLineSetting;
using HSZ.wms.Interfaces.ZjnWmsLineSetting;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnWmsLineSetting
{
    /// <summary>
    /// 线体信息配置服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms", Name = "ZjnWmsLineSetting", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnWmsLineSettingService : IZjnWmsLineSettingService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWmsLineSettingEntity> _zjnLineSettingRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnWmsLineSettingService"/>类型的新实例
        /// </summary>
        public ZjnWmsLineSettingService(ISqlSugarRepository<ZjnWmsLineSettingEntity> zjnLineSettingRepository,
            IUserManager userManager)
        {
            _zjnLineSettingRepository = zjnLineSettingRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 线体编号是否存在
        /// </summary>
        /// <param name="lineNo"></param>
        /// <returns></returns>
        [HttpGet("existLineNo")]
        public async Task<bool> ExistLineNo(string lineNo)
        {
            var output = await _zjnLineSettingRepository.IsAnyAsync(p => p.LineNo == lineNo);
            return output;
        }

        /// <summary>
        /// 获取线体信息配置
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnLineSettingRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnWmsLineSettingInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取线体信息配置列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnWmsLineSettingListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnLineSettingRepository.AsSugarClient().Queryable<ZjnWmsLineSettingEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_LineNo), a => a.LineNo.Contains(input.F_LineNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_LineName), a => a.LineName.Contains(input.F_LineName))
                .WhereIF(input.F_EnabledMark.HasValue, a => a.EnabledMark == input.F_EnabledMark)
                .Select((a
) => new ZjnWmsLineSettingListOutput
{
    F_Id = a.Id,
    F_LineNo = a.LineNo,
    F_LineName = a.LineName,
    F_GoodsType=a.GoodsType,
    F_GoodsTypeName = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.GoodsType && s.DictionaryTypeId == "325449144728552709" && SqlFunc.IsNull(s.DeleteMark, 0) == 0).Select(s => s.FullName),
    F_LineStart =a.LineStart,
    F_LineEnd = a.LineEnd,
    F_LineLayer=a.LineLayer,
    F_LineMaxWork = a.LineMaxWork,
    F_LineNowWork = a.LineNowWork,
    F_Description = a.Description,
    F_EnabledMark = a.EnabledMark,
}).OrderBy(sidx + " " + input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<ZjnWmsLineSettingListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建线体信息配置
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnWmsLineSettingCrInput input)
        {
            if (_zjnLineSettingRepository.IsAny(a => a.LineNo == input.lineNo))
            {
                throw HSZException.Oh(ErrorCode.COM1004);
            }
            var entity = input.Adapt<ZjnWmsLineSettingEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateUser = _userManager.UserId;
            entity.CreateTime = DateTime.Now;
            entity.LineNowWork = 0;
            entity.Expand = string.Empty;
            var isOk = await _zjnLineSettingRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 更新线体信息配置
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnWmsLineSettingUpInput input)
        {
            var entity = input.Adapt<ZjnWmsLineSettingEntity>();
            var isOk = await _zjnLineSettingRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除线体信息配置
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnLineSettingRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnLineSettingRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


