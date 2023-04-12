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
    [WareDI(WareType.RawMaterial)]
    public class RawMaterialLocationGenerator : ILocationGenerator, IDynamicApiController, IScoped
    {
        private readonly ISqlSugarRepository<ZjnWmsLocationEntity> _zjnWmsLocationRepository;
        private readonly SqlSugarScope _db;

        public RawMaterialLocationGenerator(ISqlSugarRepository<ZjnWmsLocationEntity> zjnWmsLocationRepository)
        {
            _zjnWmsLocationRepository = zjnWmsLocationRepository;
            _db = DbScoped.SugarScope;
        }

        public int MaxPerLayer
        {
            get
            {
                return 1980;
            }
        }

        public ZjnWmsLocationDefineOutput ReferInfo
        {
            get
            {
                return new ZjnWmsLocationDefineOutput()
                {
                    WarehouseNo = "ZHWH-W1",
                    AisleNo = "A1",
                    AisleNos = new string[] { "B3", "B2", "B1", "A3", "A2", "A1" },
                    Row = 16,
                    Cell = 66,
                    Layer = 10,
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
                };
            }
        }

        [HttpPost("GenerateRawMaterialLocation")]
        public async Task GenerateLocation()
        {
            try
            {
                if (!await RedisHelper.HExistsAsync("Location", "ZHWH-W1"))
                {
                    await RedisHelper.HSetAsync("Location", "ZHWH-W1", ReferInfo);
                    _db.BeginTran();
                    await _zjnWmsLocationRepository.AsDeleteable().ExecuteCommandAsync();
                    List<ZjnWmsLocationEntity> list = new List<ZjnWmsLocationEntity>();
                    for (int row = 1; row <= 16; row++)
                    {
                        for (int col = 1; col <= 66; col++)
                        {
                            for (int layer = 1; layer <= 10; layer++)
                            {
                                string symbol;
                                int asileNo;
                                int depth = 0;
                                int rowNum;
                                int priority;
                                if (row <= 8)
                                {
                                    symbol = "B";
                                    if (row < 4)
                                    {
                                        asileNo = 3;
                                        depth = row == 3 ? 1 : 0;
                                        rowNum = row + 1;
                                        priority = row == 1 ? 3 : row == 2 ? 1 : 2;
                                    }
                                    else if (row > 5)
                                    {
                                        asileNo = 1;
                                        depth = row == 6 ? 1 : 0;
                                        rowNum = row - 5;
                                        priority = row == 6 ? 2 : row == 7 ? 1 : 3;
                                    }
                                    else
                                    {
                                        asileNo = 2;
                                        rowNum = row - 2;
                                        priority = row == 4 ? 2 : 1;
                                    }
                                }
                                else
                                {
                                    symbol = "A";
                                    if (row < 12)
                                    {
                                        asileNo = 3;
                                        depth = row == 11 ? 1 : 0;
                                        rowNum = row - 7;
                                        priority = row == 9 ? 3 : row == 10 ? 1 : 2;
                                    }
                                    else if (row > 13)
                                    {
                                        asileNo = 1;
                                        depth = row == 14 ? 1 : 0;
                                        rowNum = row - 13;
                                        priority = row == 14 ? 2 : row == 15 ? 1 : 3;
                                    }
                                    else
                                    {
                                        asileNo = 2;
                                        rowNum = row - 10;
                                        priority = row == 12 ? 2 : 1;
                                    }
                                }

                                string locationNo = $"ZHWH-W1-{symbol}{asileNo}-{ThreeCode(rowNum)}-{ThreeCode(col)}-{ThreeCode(layer)}";
                                ZjnWmsLocationEntity info = new ZjnWmsLocationEntity()
                                {
                                    Id = YitIdHelper.NextId().ToString(),
                                    LocationNo = locationNo,
                                    AisleNo = $"{symbol}{asileNo}",
                                    ByWarehouse = "ZHWH-W1",
                                    Row = rowNum,
                                    Cell = col,
                                    Layer = layer,
                                    Depth = depth,
                                    Priority = priority,
                                    RealRow = row,
                                    Description = $"原材料库{(symbol == "A" ? "正" : "负")}极库位",
                                    TrayNo = "",
                                    LastStatus = 0,
                                    LocationStatus = "0",
                                    EnabledMark = 1,
                                    IsDelete = col > 33 && layer > 0 && layer < 4 ? 1 : 0,
                                    CreateUser = "admin",
                                    CreateTime = DateTime.Now,
                                };
                                list.Add(info);
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

        [HttpPost("GenerateRawMaterialTrayCode")]
        [AllowAnonymous]
        public async Task GenerateTrayCode()
        {
            List<ZjnWmsTrayEntity> trays = new List<ZjnWmsTrayEntity>();
            for (int i = 1; i < 100000; i++)
            {
                string trayCode = $"W1A{i.ToString("d12")}";
                trays.Add(new ZjnWmsTrayEntity()
                {
                    CreateTime = DateTime.Now,
                    CreateUser = "system",
                    EnabledMark = 1,
                    TrayStates = 1,
                    Id = YitIdHelper.NextId().ToString(),
                    IsDelete = 0,
                    TrayName = $"粉料托盘{i}",
                    TrayNo = trayCode,
                    Type = 1
                });
            }
            for (int i = 1; i < 100000; i++)
            {
                string trayCode = $"W1B{i.ToString("d12")}";
                trays.Add(new ZjnWmsTrayEntity()
                {
                    CreateTime = DateTime.Now,
                    CreateUser = "system",
                    EnabledMark = 1,
                    TrayStates = 1,
                    Id = YitIdHelper.NextId().ToString(),
                    IsDelete = 0,
                    TrayName = $"箔材托盘{i}",
                    TrayNo = trayCode,
                    Type = 2
                });
            }
            await _db.Insertable(trays).ExecuteCommandAsync();
        }
    }
}
