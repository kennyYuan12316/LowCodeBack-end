using HSZ.Common.DI;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Entitys.wms;
using HSZ.Wms.Entitys.Dto.ZjnWmsLocation;
using HSZ.Wms.Interfaces.zjnLocationGenerator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.Wms.zjnLocationGenerator
{
    /// <summary>
    /// 生成货位服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "LocationGenerator", Name = "Gen", Order = 200)]
    [Route("api/wms/[controller]")]
    [WareDI(WareType.FrameMember)]
    public class FrameMemberLocationGenerator : ILocationGenerator, IDynamicApiController, IScoped
    {
        private readonly ISqlSugarRepository<ZjnWmsLocationEntity> _zjnWmsLocationRepository;
        private readonly SqlSugarScope _db;

        public FrameMemberLocationGenerator(ISqlSugarRepository<ZjnWmsLocationEntity> zjnWmsLocationRepository)
        {
            _zjnWmsLocationRepository = zjnWmsLocationRepository;
            _db = DbScoped.SugarScope;
        }

        public int MaxPerLayer
        {
            get
            {
                return 1656;
            }
        }

        public ZjnWmsLocationDefineOutput ReferInfo
        {
            get
            {
                return new ZjnWmsLocationDefineOutput()
                {
                    WarehouseNo = "ZHWH-W2",
                    AisleNo = "A1",
                    AisleNos = new string[] { "A5", "A4", "A3", "A2", "A1" },
                    Row = 4,
                    Cell = 46,
                    Layer = 9,
                    MaxPerAisle = 1656,
                    IsStatic = true,
                    Side = new StaticSide()
                    {
                        LeftSide = 2,
                        RightSide = 2
                    }
                };
            }
        }
        /// <summary>
        /// 生成结构件库位
        /// </summary>
        /// <returns></returns>
        [HttpPost("GenerateFrameMemberLocation")]
        [AllowAnonymous]
        public async Task GenerateLocation()
        {
            try
            {
                if (!await RedisHelper.HExistsAsync("Location", "ZHWH-W2"))
                {
                    RedisHelper.HSet("Location", "ZHWH-W2", ReferInfo);
                    _db.BeginTran();
                    await _zjnWmsLocationRepository.AsDeleteable().ExecuteCommandAsync();

                    List<ZjnWmsLocationEntity> list = new List<ZjnWmsLocationEntity>();

                    for (int asile = 1; asile <= 5; asile++)
                    {
                        for (int row = 1; row <= 4; row++)
                        {
                            for (int col = 1; col <= 46; col++)
                            {
                                for (int layer = 1; layer <= 9; layer++)
                                {
                                    string asileNo = "A" + asile;
                                    int priority = row % 2; 
                                    int[] depthCol = { 1, 4 };
                                    string locationNo = $"ZHWH-W2-{asileNo}-{ThreeCode(row)}-{ThreeCode(col)}-{ThreeCode(layer)}";
                                    ZjnWmsLocationEntity info = new ZjnWmsLocationEntity()
                                    {
                                        Id = YitIdHelper.NextId().ToString(),
                                        LocationNo = locationNo,
                                        AisleNo = asileNo,
                                        ByWarehouse = "ZHWH-W2",
                                        Row = row,
                                        Cell = col,
                                        Layer = layer,
                                        Depth = depthCol.Contains(row) ? 1 : 0,
                                        Priority = priority,
                                        RealRow = row,
                                        Warning = 0,
                                        Description = "结构件库位",
                                        TrayNo = "",
                                        LastStatus = 0,
                                        LocationStatus = "0",
                                        EnabledMark = 1,
                                        IsDelete = 0,
                                        CreateUser = "admin",
                                        CreateTime = DateTime.Now,
                                    };
                                    list.Add(info);
                                }
                            }
                        }
                    }
                    await _zjnWmsLocationRepository.AsInsertable(list).ExecuteCommandAsync();
                    _db.CommitTran();
                }
            }
            catch (Exception ex)
            {
                 
                _db.RollbackTran();
            }
        }

        private string ThreeCode(int code)
        {
            return code.ToString("D3");
        }


    }
}
