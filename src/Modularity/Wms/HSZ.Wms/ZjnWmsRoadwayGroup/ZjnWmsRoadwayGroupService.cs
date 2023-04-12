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
using HSZ.wms.Entitys.Dto.ZjnRoadwayGroup;
using HSZ.wms.Entitys.Dto.ZjnWmsRoadwayGroup;
using HSZ.wms.Interfaces.ZjnRoadwayGroup;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnWmsRoadwayGroup
{
    /// <summary>
    /// 巷道分组信息服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnWmsRoadwayGroup", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnWmsRoadwayGroupService : IZjnRoadwayGroupService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWmsRoadwayGroupEntity> _zjnRoadwayGroupRepository;
        private readonly ISqlSugarRepository<ZjnWmsRoadwayGroupDetailsEntity> _zjnRoadwayGroupDetailsRepository;
        private readonly ISqlSugarRepository<ZjnWmsRoadwayInboundPlanEntity> _zjnRoadwayInboundPlanRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnWmsRoadwayGroup"/>类型的新实例
        /// </summary>
        public ZjnWmsRoadwayGroupService(ISqlSugarRepository<ZjnWmsRoadwayGroupEntity> zjnRoadwayGroupRepository, ISqlSugarRepository<ZjnWmsRoadwayInboundPlanEntity> zjnRoadwayInboundPlanRepository
            ,ISqlSugarRepository<ZjnWmsRoadwayGroupDetailsEntity> zjnRoadwayGroupDetailsRepository,
            IUserManager userManager)
        {
            _zjnRoadwayInboundPlanRepository = zjnRoadwayInboundPlanRepository;
            _zjnRoadwayGroupRepository = zjnRoadwayGroupRepository;
            _zjnRoadwayGroupDetailsRepository = zjnRoadwayGroupDetailsRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取巷道分组信息
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnRoadwayGroupRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnWmsRoadwayGroupInfoOutput>();            
            var zjnRoadwayGroupDetailsList = await _zjnRoadwayGroupDetailsRepository.GetListAsync(w => w.RoadwayCode == output.roadwayNo);
            output.zjnRoadwayGroupDetailsList = zjnRoadwayGroupDetailsList.Adapt<List<ZjnWmsRoadwayGroupDetailsInfoOutput>>();
            return output;
        }

        /// <summary>
		/// 获取巷道分组信息列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnWmsRoadwayGroupListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnRoadwayGroupRepository.AsSugarClient().Queryable<ZjnWmsRoadwayGroupEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_roadwayNo), a => a.RoadwayNo.Contains(input.F_roadwayNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_roadwayName), a => a.RoadwayName.Contains(input.F_roadwayName))
                .Select((a
)=> new ZjnWmsRoadwayGroupListOutput
                {
                    F_Id = a.Id,
                    F_roadwayNo = a.RoadwayNo,
                    F_roadwayName = a.RoadwayName,
                    F_Description = a.Description,
                    F_EnabledMark = SqlFunc.IIF(a.EnabledMark == 0, "禁用", "启用"),
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnWmsRoadwayGroupListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建巷道分组信息
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnWmsRoadwayGroupCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnWmsRoadwayGroupEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateTime = DateTime.Now;
            entity.CreateUser = userInfo.userId;
            try
            {

                _db.BeginTran();
                var newEntity = await _zjnRoadwayGroupRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteReturnEntityAsync();
                
                var zjnRoadwayGroupDetailsEntityList = input.zjnRoadwayGroupDetailsList.Adapt<List<ZjnWmsRoadwayGroupDetailsEntity>>();
                if(zjnRoadwayGroupDetailsEntityList != null)
                {
                    int i = 1;
                    foreach (var item in zjnRoadwayGroupDetailsEntityList)
                    {
                        item.RoadwayDetailsGrade = i;
                        item.Id = YitIdHelper.NextId().ToString();
                        item.RoadwayCode = newEntity.RoadwayNo;
                        item.CreateUser = userInfo.userId;
                        item.CreateTime = DateTime.Now;
                        i++;
                    }
                    await _zjnRoadwayGroupDetailsRepository.AsInsertable(zjnRoadwayGroupDetailsEntityList).ExecuteCommandAsync();
                    if (entity.EnabledMark==1)
                    {
                        ZjnWmsRoadwayGroupDetailsEntity detailsEntity = zjnRoadwayGroupDetailsEntityList.Where(x => x.EnabledMark == 1).OrderBy(x => x.RoadwayDetailsGrade).ToList()[0];
                        ZjnWmsRoadwayInboundPlanEntity inboundPlanEntity = new ZjnWmsRoadwayInboundPlanEntity();
                        inboundPlanEntity.Id = YitIdHelper.NextId().ToString();
                        inboundPlanEntity.NowroadwayGroup = detailsEntity.RoadwayDetailsNo;
                        inboundPlanEntity.NowroadwayGroupCode = detailsEntity.RoadwayDetailsGrade;
                        inboundPlanEntity.RoadwayGroupName = entity.RoadwayName;
                        inboundPlanEntity.RoadwayGroupNo = entity.RoadwayNo;
                        await _zjnRoadwayInboundPlanRepository.AsInsertable(inboundPlanEntity).ExecuteCommandAsync();
                    }
                    
                }
                
                _db.CommitTran();
            }
            catch (Exception)
            {
                _db.RollbackTran();
                throw HSZException.Oh(ErrorCode.COM1000);
            }
        }

        /// <summary>
        /// 更新巷道分组信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnWmsRoadwayGroupUpInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnWmsRoadwayGroupEntity>();
            try
            {
                //开启事务
                _db.BeginTran();
                
                await _zjnRoadwayGroupRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
                
                //清空原有数据
                await _zjnRoadwayGroupDetailsRepository.AsDeleteable().Where(it => it.RoadwayCode == entity.RoadwayNo).ExecuteCommandAsync();
                
                //新增新数据
                var zjnRoadwayGroupDetailsEntityList = input.zjnRoadwayGroupDetailsList.Adapt<List<ZjnWmsRoadwayGroupDetailsEntity>>();
                if(zjnRoadwayGroupDetailsEntityList != null)
                {
                    foreach (var item in zjnRoadwayGroupDetailsEntityList)
                    {
                        item.CreateTime = DateTime.Now;
                        item.CreateUser = userInfo.userId;
                        item.Id = YitIdHelper.NextId().ToString();
                        item.RoadwayCode = entity.RoadwayNo;
                    }
                    await _zjnRoadwayGroupDetailsRepository.AsInsertable(zjnRoadwayGroupDetailsEntityList).ExecuteCommandAsync();
                }
                
                //关闭事务
                _db.CommitTran();
            }
            catch (Exception)
            {
                //回滚事务
                _db.RollbackTran();
                throw HSZException.Oh(ErrorCode.COM1001);
            }
        }

        /// <summary>
        /// 删除巷道分组信息
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnRoadwayGroupRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            try
            {
                //开启事务
                _db.BeginTran();
                
                await _zjnRoadwayGroupRepository.AsDeleteable().Where(it => it.Id == id).ExecuteCommandAsync();
                
                await _zjnRoadwayGroupDetailsRepository.AsDeleteable().Where(it => it.RoadwayCode.Equals(entity.RoadwayNo)).ExecuteCommandAsync();
                
                //关闭事务
                _db.CommitTran();
            }
            catch (Exception)
            {
                //回滚事务
                _db.RollbackTran();
                throw HSZException.Oh(ErrorCode.COM1001);
            }
        }
        /// <summary>
        /// 判断编号是否存在
        /// </summary>
        /// <param name="LocationNo"></param>
        /// <returns></returns>
        [HttpGet("ExistLocationNo")]
        public async Task<bool> ExistLocationNo(string LocationNo)
        {
            var output = await _zjnRoadwayGroupRepository.IsAnyAsync(p => p.RoadwayNo == LocationNo);
            return output;
        }


    }
}


