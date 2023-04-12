using Aspose.Cells;
using HSZ.Common.DI;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Entitys.wms;
using HSZ.Wms.Entitys.Dto.ZjnWmsLocation;
using HSZ.Wms.Interfaces.zjnLocationGenerator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spire.Presentation;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.Wms.zjnLocationGenerator
{
    /// <summary>
    /// 生成货位服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "LocationGenerator", Name = "Gen", Order = 200)]
    [Route("api/wms/[controller]")]
    [WareDI(WareType.Production)]
    public class ProductionLocationGenerator : ILocationGenerator, IDynamicApiController, IScoped
    {
        private readonly ISqlSugarRepository<ZjnWmsLocationEntity> _zjnWmsLocationRepository;
        private readonly SqlSugarScope _db;

        public ProductionLocationGenerator(ISqlSugarRepository<ZjnWmsLocationEntity> zjnWmsLocationRepository)
        {
            _zjnWmsLocationRepository = zjnWmsLocationRepository;
            _db = DbScoped.SugarScope;
        }

        public int MaxPerLayer
        {
            get
            {
                return 2352;
            }
        }

        public ZjnWmsLocationDefineOutput ReferInfo
        {
            get
            {
                return new ZjnWmsLocationDefineOutput()
                {
                    WarehouseNo = "ZHWH-W3",
                    AisleNo = "001",
                    AisleNos = Enumerable.Range(1, 12).OrderByDescending(a => a).Select(s => s.ToString("d3")).ToArray(),
                    Row = 24,
                    Cell = 98,
                    Layer = 17,
                    MaxPerAisle = 2352,
                    IsStatic = true,
                    Side = new StaticSide()
                    {
                        LeftSide = 1,
                        RightSide = 1
                    }
                };
            }
        }

        [HttpPost("GenerateProductionLocation")]
        [AllowAnonymous]
        public async Task GenerateLocation()
        {
            try
            {
                if (!await RedisHelper.HExistsAsync("Location", "ZHWH-W3"))
                {
                    RedisHelper.HSet("Location", "ZHWH-W3", ReferInfo);
                    _db.BeginTran();
                    await _zjnWmsLocationRepository.AsDeleteable().ExecuteCommandAsync();
                    List<ZjnWmsLocationEntity> list = new List<ZjnWmsLocationEntity>();
                    for (int aisleNo = 1; aisleNo <= 12; aisleNo++)
                    {
                        for (int row = 1; row <= 2; row++)
                        {
                            for (int col = 1; col <= 98; col++)
                            {
                                for (int layer = 1; layer <= 17; layer++)
                                {
                                    int priority = row % 2 + 1;
                                    int[] warningCol = { 1, 2, 47, 48, 97, 98 };
                                    int[] disableCol = { 49, 50 };
                                    string locationNo = $"ZHWH-W3-{ThreeCode(aisleNo)}-{ThreeCode(row)}-{ThreeCode(col)}-{ThreeCode(layer)}";
                                    ZjnWmsLocationEntity info = new ZjnWmsLocationEntity()
                                    {
                                        Id = YitIdHelper.NextId().ToString(),
                                        LocationNo = locationNo,
                                        AisleNo = ThreeCode(aisleNo),
                                        ByWarehouse = "ZHWH-W3",
                                        Row = row,
                                        Cell = col,
                                        Layer = layer,
                                        Depth = 0,
                                        Priority = priority,
                                        RealRow = row * aisleNo,
                                        Warning = warningCol.Contains(col) && row == (aisleNo - 1) % 2 + 1 ? 1 : 0,
                                        Description = "成品库库位",
                                        TrayNo = "",
                                        LastStatus = 0,
                                        LocationStatus = "0",
                                        EnabledMark = disableCol.Contains(col) ? 0 : 1,
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
            catch (Exception)
            {
                _db.RollbackTran();
            }
        }

        private string ThreeCode(int code)
        {
            return code.ToString("D3");
        }

        [HttpPost("Cache")]
        [AllowAnonymous]
        public void Cache()
        {
            RedisHelper.HSet("Location", "ZHWH-W1", new ZjnWmsLocationDefineOutput()
            {
                WarehouseNo = "ZHWH-W1",
                AisleNo = "A1",
                AisleNos = new string[] { "B3", "B2", "B1", "A3", "A2", "A1" },
                Row = 16,
                Cell = 66,
                Layer = 10,
                MaxPerAisle = 1980,
                IsStatic = false,
                Sides = new Dictionary<string, StaticSide>()
                {
                    {"B3",new StaticSide(){LeftSide=1,RightSide=2} },
                    {"A3",new StaticSide(){LeftSide=1,RightSide=2} },
                    {"B2",new StaticSide(){LeftSide=1,RightSide=1} },
                    {"A2",new StaticSide(){LeftSide=1,RightSide=1} },
                    {"B1",new StaticSide(){LeftSide=2,RightSide=1} },
                    {"A1",new StaticSide(){LeftSide=2,RightSide=1} }
                }
            });
            RedisHelper.HSet("Location", "ZHWH-W3", new ZjnWmsLocationDefineOutput()
            {
                WarehouseNo = "ZHWH-W3",
                AisleNo = "001",
                AisleNos = Enumerable.Range(1, 12).OrderByDescending(a => a).Select(s => s.ToString("d3")).ToArray(),
                Row = 24,
                Cell = 98,
                Layer = 17,
                MaxPerAisle = 2352,
                IsStatic = true,
                Side = new StaticSide()
                {
                    LeftSide = 1,
                    RightSide = 1
                }
            });
        }
    }
}
