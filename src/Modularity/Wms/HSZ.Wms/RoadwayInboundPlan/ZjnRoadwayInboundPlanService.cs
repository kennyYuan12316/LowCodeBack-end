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
using HSZ.wms.Entitys.Dto.ZjnRoadwayInboundPlan;
using HSZ.wms.Entitys.Dto.ZjnWmsRoadwayInboundPlan;
using HSZ.wms.Interfaces.ZjnRoadwayInboundPlan;
using HSZ.wms.Interfaces.ZjnWmsRoadwayInboundPlan;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnRoadwayInboundPlan
{
    /// <summary>
    /// 巷道入库策略均衡服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnRoadwayInboundPlan", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnWmsRoadwayInboundPlanService : IZjnWmsRoadwayInboundPlanService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWmsRoadwayInboundPlanEntity> _zjnRoadwayInboundPlanRepository;
        private readonly ISqlSugarRepository<ZjnRoadwayGroupEntity> _zjnRoadwayGroupEntity;
        private readonly ISqlSugarRepository<ZjnRoadwayGroupDetailsEntity> _zjnRoadwayGroupDetailsEntity;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnWmsRoadwayInboundPlanService"/>类型的新实例
        /// </summary>
        public ZjnWmsRoadwayInboundPlanService(ISqlSugarRepository<ZjnWmsRoadwayInboundPlanEntity> zjnRoadwayInboundPlanRepository, ISqlSugarRepository<ZjnRoadwayGroupEntity> zjnRoadwayGroupEntity,
            ISqlSugarRepository<ZjnRoadwayGroupDetailsEntity> zjnRoadwayGroupDetailsEntity, IUserManager userManager)
        {
            _zjnRoadwayGroupDetailsEntity = zjnRoadwayGroupDetailsEntity;
            _zjnRoadwayGroupEntity = zjnRoadwayGroupEntity;
            _zjnRoadwayInboundPlanRepository = zjnRoadwayInboundPlanRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取巷道入库策略均衡
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnRoadwayInboundPlanRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnWmsRoadwayInboundPlanInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取巷道入库策略均衡列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnWmsRoadwayInboundPlanListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnRoadwayInboundPlanRepository.AsSugarClient().Queryable<ZjnWmsRoadwayInboundPlanEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_roadwayGroupNo), a => a.RoadwayGroupNo.Contains(input.F_roadwayGroupNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_roadwayGroupName), a => a.RoadwayGroupName.Contains(input.F_roadwayGroupName))
                .Select((a
)=> new ZjnWmsRoadwayInboundPlanListOutput
                {
                    F_Id = a.Id,
                    F_roadwayGroupNo = a.RoadwayGroupNo,
                    F_roadwayGroupName = a.RoadwayGroupName,
                    F_NowroadwayGroup = a.NowroadwayGroup,
                    F_NowroadwayGroupCode = a.NowroadwayGroupCode,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnWmsRoadwayInboundPlanListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建巷道入库策略均衡
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnWmsRoadwayInboundPlanCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnWmsRoadwayInboundPlanEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            
            var isOk = await _zjnRoadwayInboundPlanRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 更新巷道入库策略均衡
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnWmsRoadwayInboundPlanUpInput input)
        {
            var entity = input.Adapt<ZjnWmsRoadwayInboundPlanEntity>();
            var isOk = await _zjnRoadwayInboundPlanRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除巷道入库策略均衡
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnRoadwayInboundPlanRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnRoadwayInboundPlanRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        [HttpGet("initialization")]
        public async Task initialization(string id)
        {
            List<ZjnWmsRoadwayInboundPlanEntity> listRoadwayInbound = new List<ZjnWmsRoadwayInboundPlanEntity>();
            _db.BeginTran();
            try
            {
                if (id == null)
                {
                    //清空原有数据
                    await _zjnRoadwayInboundPlanRepository.AsDeleteable().ExecuteCommandAsync();
                   
                    var list =  _zjnRoadwayGroupEntity.AsSugarClient().Queryable<ZjnWmsRoadwayGroupEntity>().Where(p => p.EnabledMark==1).ToList();
                    foreach (var item in list)
                    {
                        var deis = _zjnRoadwayGroupDetailsEntity.AsSugarClient().Queryable<ZjnWmsRoadwayGroupDetailsEntity>().Where(x => x.EnabledMark == 1&& x.RoadwayCode==item.RoadwayNo).OrderBy(x=> x.RoadwayDetailsGrade).ToList();
                        if (deis.Count()>0)
                        {
                            ZjnWmsRoadwayInboundPlanEntity inboundPlanEntity = new ZjnWmsRoadwayInboundPlanEntity();
                            inboundPlanEntity.Id = YitIdHelper.NextId().ToString();
                            inboundPlanEntity.NowroadwayGroup = deis[0].RoadwayDetailsNo;
                            inboundPlanEntity.NowroadwayGroupCode = deis[0].RoadwayDetailsGrade;
                            inboundPlanEntity.RoadwayGroupName = item.RoadwayName;
                            inboundPlanEntity.RoadwayGroupNo = item.RoadwayNo;
                            listRoadwayInbound.Add(inboundPlanEntity);
                        }
                    }
                    await _zjnRoadwayInboundPlanRepository.AsInsertable(listRoadwayInbound).ExecuteCommandAsync();


                }

                _db.CommitTran();
            }
            catch (Exception)
            {
                _db.RollbackTran();
                throw HSZException.Oh(ErrorCode.COM1000);
            }

        }


    }
}


