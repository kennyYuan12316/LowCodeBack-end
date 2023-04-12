using HSZ.Common.Enum;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.VisualData.Entity;
using HSZ.VisualData.Entitys.Dto.ScreenMap;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.VisualData
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：业务实现：大屏地图
    /// </summary>
    [ApiDescriptionSettings(Tag = "BladeVisual", Name = "Map", Order = 160)]
    [Route("api/blade-visual/[controller]")]
    public class ScreenMapConfigService : IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<VisualMapEntity> _visualMapRepository;

        /// <summary>
        /// 初始化一个<see cref="ScreenMapConfigService"/>类型的新实例
        /// </summary>
        public ScreenMapConfigService(ISqlSugarRepository<VisualMapEntity> visualMapRepository)
        {
            _visualMapRepository = visualMapRepository;
        }

        #region Get

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("list")]
        public async Task<dynamic> GetList([FromQuery] ScreenMapListQueryInput input)
        {
            var data = await _visualMapRepository.AsQueryable().Select(v => new ScreenMapListOutput { id = v.Id, name = v.Name }).ToPagedListAsync(input.current, input.size);
            return new { current = data.pagination.PageIndex, pages = data.pagination.PageIndex, records = data.list, size = data.pagination.PageSize, total = data.pagination.Total };
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("detail")]
        public async Task<dynamic> GetInfo([FromQuery] string id)
        {
            var entity = await _visualMapRepository.GetSingleAsync(v => v.Id == id);
            var data = entity.Adapt<ScreenMapInfoOutput>();
            return data;
        }

        /// <summary>
        /// 数据详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonUnify]
        [HttpGet("data")]
        public dynamic GetDataInfo(string id)
        {
            var entity = _visualMapRepository.GetSingle(v => v.Id == id);
            return entity.data;
        }

        #endregion

        #region Post

        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        [HttpPost("save")]
        public async Task Create([FromBody] ScreenMapCrInput input)
        {
            var entity = input.Adapt<VisualMapEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            var isOk = await _visualMapRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        [HttpPost("update")]
        public async Task Update([FromBody] ScreenMapUpInput input)
        {
            var entity = input.Adapt<VisualMapEntity>();
            var isOk = await _visualMapRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [HttpPost("remove")]
        public async Task Delete(string ids)
        {
            var entity = _visualMapRepository.GetSingleAsync(v => v.Id == ids);
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _visualMapRepository.AsSugarClient().Deleteable<VisualMapEntity>(it => ids.Contains(it.Id)).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }

        #endregion
    }
}