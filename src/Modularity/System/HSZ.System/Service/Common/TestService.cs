using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Logging.Attributes;
using HSZ.Expand.Thirdparty;
using HSZ.Expand.Thirdparty.DingDing;
using HSZ.System.Entitys.Permission;
using HSZ.System.Entitys.System;
using HSZ.System.Interfaces.Common;
using HSZ.System.Interfaces.Permission;
using HSZ.System.Interfaces.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;
using HSZ.TaskScheduler.Interfaces.TaskScheduler;
using HSZ.TaskScheduler.Entitys.Entity;
using HSZ.Common.Util;
using HSZ.Common.Cache;
using System;
using HSZ.Common.Const;
using HSZ.Entitys.wms;
using HSZ.FriendlyException;
using HSZ.Common.Enum;
using HSZ.UnifyResult;
using HSZ.wms.Interfaces.ZjnWmsTask;
using HSZ.wms.Entitys.Dto.ZjnWmsTaskDetails;
using static COSXML.Model.Tag.ListBucketVersions;
using ZJN.Plc.PlcHelper;
using Senparc.Weixin.Work.Entities.Request.KF;
using S7.Net;
using ErrorCode = HSZ.Common.Enum.ErrorCode;
using MySqlX.XDevAPI.Common;
using HSZ.Common.TaskResultPubilcParms;
using HSZ.RemoteRequest.Extensions;
using HSZ.JsonSerialization;

namespace HSZ.System.Service.Common
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：测试1接口
    /// </summary>
    [ApiDescriptionSettings(Name = "Test", Order = 306)]
    [Route("api/[controller]")]
    public class TestService : IDynamicApiController, ITransient
    {
        private readonly SqlSugarScope db;
        private readonly ISqlSugarRepository<UserEntity> _sqlSugarRepository;
        private readonly ISqlSugarRepository<ZjnWmsLocationEntity> _zjnWmsLocationRepository;
        private readonly ISqlSugarRepository<RoleEntity> _sqlSugarRoleRepository;
        private readonly ITimeTaskService _ITimeTaskService;
        private readonly RedisCache _redisCache;
        private readonly IZjnWmsTaskService _zjnWmsTaskService;

        public TestService(ISqlSugarRepository<UserEntity> sqlSugarRepository, ISqlSugarRepository<RoleEntity> sqlSugarRoleRepository,
            ITimeTaskService timeTaskService, RedisCache redisCache, IZjnWmsTaskService zjnWmsTaskService, ISqlSugarRepository<ZjnWmsLocationEntity> zjnWmsLocationRepository)
        {
            _sqlSugarRepository = sqlSugarRepository;
            _sqlSugarRoleRepository = sqlSugarRoleRepository;
            db = DbScoped.SugarScope;
            _ITimeTaskService = timeTaskService;
            _redisCache = redisCache;
            _zjnWmsTaskService = zjnWmsTaskService;
            _zjnWmsLocationRepository = zjnWmsLocationRepository;
        }

        [HttpGet("")]
        [AllowAnonymous]
        public async Task<dynamic> test()
        {

            try
            {
                var str = GetCreateTableSql();
                return "";
            }
            catch (global::System.Exception ex)
            {
                db.RollbackTran();
                throw;
            }

        }

        [HttpGet("add")]
        [AllowAnonymous]
        public async Task<dynamic> add(string name = "test")
        {

            try
            {
                string ip = NetUtil.Ip;
                string GetLanIp = NetUtil.GetLanIp();
                string Url = NetUtil.Url;


                //db.Ado.ExecuteCommand($"INSERT INTO [dbo].[test]( [name], [createTime], [handshake], [states], [ip]) VALUES ( N'{name}', GETDATE(), 0, '添加', '{GetLanIp}')");
                return "";
            }
            catch (global::System.Exception ex)
            {
                db.RollbackTran();
                throw;
            }

        }

        [HttpGet("dev1")]
        [AllowAnonymous]
        public async Task<dynamic> dev1(string name = "dev1")
        {

            try
            {
                dynamic data = db.Ado.SqlQuery<dynamic>($"select * from test where name='{name}'").First();

                if (data == null) return null;

                int hPlc = data.hPlc;
                if (hPlc == 0)
                {

                    while (true)

                    {

                    }
                }

                return "";
            }
            catch (global::System.Exception ex)
            {
                throw;
            }

        }
        [HttpGet("dev2")]
        [AllowAnonymous]
        public async Task<dynamic> dev2(string name = "dev2")
        {

            try
            {
                dynamic data = db.Ado.SqlQuery<dynamic>($"select * from test where name='{name}'").First();

                if (data == null) return null;

                int hPlc = data.hPlc;
                if (hPlc == 0)
                {

                    while (true)
                    {

                    }
                }

                return "";
            }
            catch (global::System.Exception ex)
            {
                throw;
            }

        }
        [HttpGet("dev3")]
        [AllowAnonymous]
        public async Task<dynamic> dev3(string name = "dev3")
        {

            try
            {
                dynamic data = db.Ado.SqlQuery<dynamic>($"select * from test where name='{name}'").First();

                if (data == null) return null;

                int hPlc = data.hPlc;
                if (hPlc == 0)
                {

                    while (true)
                    {

                    }
                }

                return "";
            }
            catch (global::System.Exception ex)
            {
                throw;
            }

        }
        [HttpGet("dev4")]
        [AllowAnonymous]
        public async Task<dynamic> dev4(string name = "dev4")
        {

            try
            {
                dynamic data = db.Ado.SqlQuery<dynamic>($"select * from test where name='{name}'").First();

                if (data == null) return null;

                int hPlc = data.hPlc;
                if (hPlc == 0)
                {

                    while (true)
                    {

                    }
                }

                return "";
            }
            catch (global::System.Exception ex)
            {
                throw;
            }

        }
        [HttpGet("dev5")]
        [AllowAnonymous]
        public async Task<dynamic> dev5(string name = "dev5")
        {

            try
            {
                dynamic data = db.Ado.SqlQuery<dynamic>($"select * from test where name='{name}'").First();
                if (data == null) return null;

                int hPlc = data.hPlc;
                if (hPlc == 0)
                {

                    while (true)
                    {

                    }
                }

                return "";
            }
            catch (global::System.Exception ex)
            {
                throw;
            }

        }


        [HttpGet("addtest")]
        [AllowAnonymous]
        public async Task<dynamic> addtest()
        {

            try
            {
                db.BeginTran();

                TimeTaskEntity timeTaskEntity = new TimeTaskEntity();
                timeTaskEntity.Id = YitIdHelper.NextId().ToString();
                timeTaskEntity.EnCode = "0002";
                timeTaskEntity.FullName = "测试自动添加，能否执行";



                await _ITimeTaskService.Create(timeTaskEntity);
                //var str = GetCreateTableSql();
                return "";
            }
            catch (global::System.Exception ex)
            {
                db.RollbackTran();
                throw;
            }

        }

        [HttpGet("stoptest")]
        [AllowAnonymous]
        public async Task<dynamic> stoptest()
        {

            try
            {
                //var str = GetCreateTableSql();
                return "";
            }
            catch (global::System.Exception ex)
            {
                db.RollbackTran();
                throw;
            }

        }

        [HttpGet("testredis")]
        [AllowAnonymous, IgnoreLog]
        public int testredis()
        {

            RedisHelper.SetNx("taskId", 1000000000);
            long TaskId = RedisHelper.IncrBy("taskId");
            if (TaskId == Int32.MaxValue) RedisHelper.Del("taskId");
            return Convert.ToInt32(TaskId);
            //int newTaskId = 0;
            try
            {
                //    var bl =_redisCache.Exists("taskId");
                //    if (bl)
                //    {
                //        int taskID = Convert.ToInt32(_redisCache.Get("taskId"));
                //        //newTaskId = taskID==Int32.MaxValue? initTaskId: taskID + 1;
                //        //_redisCache.Set("taskId", newTaskId);
                //        if (taskID == Int32.MaxValue)
                //        {
                //            newTaskId = initTaskId;

                //            _redisCache.Set("taskId", newTaskId);
                //        }
                //        else { 
                //        RedisHelper.IncrBy("taskId");
                //        }
                //    }
                //    else {
                //        newTaskId = initTaskId;
                //        _redisCache.Set("taskId", newTaskId);
                //    }
                return 0;
            }
            catch (global::System.Exception ex)
            {
                throw;
            }

        }


        [HttpGet("readRedis")]
        [AllowAnonymous, IgnoreLog]
        public async Task<dynamic> readRedis()
        {

            try
            {

                //var tasklist = RedisHelper.LRange<List<ZjnWmsTaskDetailsEntity>>(CommonConst.CACHE_KEY_TASK_LIST,0,-1);
                //var j = tasklist[0];
                var tasklist = RedisHelper.HGetAll<ZjnWmsTaskDetailsInfoOutput>(CommonConst.CACHE_KEY_TASK_LIST);

                List<ZjnWmsTaskDetailsInfoOutput> pList = new List<ZjnWmsTaskDetailsInfoOutput>(tasklist.Values);

                //var one = RedisHelper.HGet<List<ZjnWmsTaskDetailsInfoOutput>>(CommonConst.CACHE_KEY_TASK_LIST, "1000000008");

                //RedisHelper.HDel(CommonConst.CACHE_KEY_TASK_LIST, "1000000008");  

                return "";
            }
            catch (global::System.Exception ex)
            {
                db.RollbackTran();
                throw;
            }

        }

        [HttpGet("SetTaskRedis")]
        [AllowAnonymous, IgnoreLog]
        public async Task<dynamic> SetNextTaskRedis(string id)
        {

            try
            {
                var task = await _zjnWmsTaskService.GetNextTaskDetails(id);
                _ = RedisHelper.HSetAsync(CommonConst.CACHE_KEY_TASK_LIST, task.taskDetailsId, task);
                return "";
            }
            catch (global::System.Exception ex)
            {
                db.RollbackTran();
                throw;
            }

        }
        [HttpGet("resultTest")]
        [AllowAnonymous, IgnoreLog]
        public dynamic resultTest(string type = "1")
        {

            switch (type)
            {
                case "1":
                    return "1";
                case "2":
                    return 1;
                case "3":
                    return false;
                case "4":
                    throw HSZException.Oh(ErrorCode.COM1000);
                case "5":
                    throw HSZException.Bah("sd", ErrorCode.COM1000);
                case "6":
                    UnifyContext.Fill("8888");
                    return new { name = "6666" };
                default:

                    return null;
            }

        }
        [HttpGet("NextNodeTest")]
        [AllowAnonymous, IgnoreLog]
        public async Task<dynamic> NextNodeTest(string taskListId, [FromQuery] List<ZjnWmsTaskDetailsInfoOutput> list = null)
        {

            var task = await _zjnWmsTaskService.GetNextTaskDetails(taskListId);


            if (task == null)
            {
                return list;

            }
            else
            {
                list.Add(task);
                return await this.NextNodeTest(task.taskDetailsId, list);
            }
        }

        [HttpGet("NextNodeTestOne")]
        [AllowAnonymous, IgnoreLog]
        public async Task<dynamic> NextNodeTestOne(string taskListId)
        {

            return await _zjnWmsTaskService.GetNextTaskDetails(taskListId);
        }
        [HttpGet("Testlocation")]
        [AllowAnonymous, IgnoreLog]
        public async Task<dynamic> Testlocation()
        {

            var a= await _zjnWmsLocationRepository.AsQueryable()
                .OrderBy(x => x.Cell, OrderByType.Desc)
                .OrderBy(x => x.Row, OrderByType.Desc)
                .OrderBy(x => x.Layer)

                .ToListAsync();
            return a;
        }
        [HttpGet("TestSum")]
        [AllowAnonymous, IgnoreLog]
        public async Task<dynamic> TestSum()
        {

            var a = await _zjnWmsLocationRepository.AsSugarClient().Queryable<ZjnWmsMaterialInventoryEntity>().SumAsync(x => x.ProductsQuantity);
            return a;
        }
        [HttpGet("Testapi")]
        [AllowAnonymous, IgnoreLog]
        [NonUnify]
        public async Task<RESTfulResult<ZjnWmsTaskDetailsInfoOutput>> Testapi()
        {
            RESTfulResult<ZjnWmsTaskDetailsInfoOutput> res = new();
            string path = "http://localhost:58504/api/wms/zjnwmstask/TaskProcesByWcs";
            var dicHerader = new Dictionary<string, object>();
            dicHerader.Add("HSZ_API", true);

            var dic = new Dictionary<string, object>();
            dic.Add("TaskNo", 1000006083);
            dic.Add("TaskState", 3);
            var parameter = new TaskResultPubilcParameter { targetType = 2 };

            var result = await path.SetHeaders(dicHerader).SetQueries(dic).SetBody(parameter).PostAsStringAsync();
            res = result.Deserialize<RESTfulResult<ZjnWmsTaskDetailsInfoOutput>>();
            return res;
        }
        [HttpGet("GetInfplco")]
        [AllowAnonymous, IgnoreLog]
        public async Task GetInfplco(string ip = "192.168.1.17",int dbplc=25100)
        {

            var a = db.Ado.SqlQuery<dynamic>("select * from atest");
            var device = new Device(S7.Net.CpuType.S71500, ip, dbplc, 0, 1, "172.17.26.25");

            try
            {
                var bl = await device.StartAsync();
                if (device.Plc.IsConnected)
                {
                    foreach (var item in a)
                    {

                        var s = item.start;
                        var devid = Convert.ToString(item.id);

                        PackStatusD obj = device.ReadDbClass<PackStatusD>("PackStatusD", dbplc, (int)item.start, 0);
                        obj.DeviceCode = obj.GetS7StringToBytes(devid, 0);
                        var flag=await obj.WriteAsync();
                    }
                }
            }
            catch (Exception)
            {
                device.Close();
                throw;
            }
           


            //var ip = "192.168.1.35";
            //创建plc连接
            //var device = new Device(S7.Net.CpuType.S71500, ip, 102, 0, 1, "003");
            //try
            //{
            //    var bl = await device.StartAsync();
            //    var a = device.Plc.IsConnected;
            //    var lista = device.ReadDbClass("PackRead", 101, 0, 0);


            //    var list = device.ReadDbClass("PackRead", 201, 0, 0);

            //    var lists = device.ReadDbClass("PackReadD", 201, 768, 0);

            //    int c = (int)OperationType.Into;

            //}
            //catch (Exception ex)
            //{

            //    throw;
            //}
            //if (plc.Plc.IsConnected)
            //{
            //    var obj1 = plc.Plc.Read(DataType.DataBlock, 103, 0, VarType.Byte, 8);
            //}
            //var kk = plc.ReadDbBytes(0, 1101, 00);
            //var obj = plc.ReadDbClass("PackTestList", 101, 4, 4);
            //return device;
        }
        /// <summary>
        /// 创建表脚本勿删
        /// </summary>
        /// <returns></returns>
        private string GetCreateTableSql()
        {
            db.AddConnection(new ConnectionConfig()
            {
                ConfigId = "777",
                DbType = DbType.Dm,
                //ConnectionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.0.103)(PORT=1521))(CONNECT_DATA=(SERVER = DEDICATED)(SERVICE_NAME=orcl11g)));User Id=HSZ_INIT_33;Password=ltVSPQcrMbI6U8aJ",//oracle
                ConnectionString = "Server=192.168.0.50:5232;User Id=HSZ_INIT_33;PWD=Eg3osp1RkpOSiSeK;DATABASE=HSZ_INIT_33",//dm
                //ConnectionString = "Data Source=192.168.0.52;Initial Catalog=hsz_init_333;User ID=sa;Password=7V!FJsYI*0ZNqi$l;MultipleActiveResultSets=true",
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });
            db.ChangeDatabase("777");
            var sb = new StringBuilder();
            var tables = db.DbMaintenance.GetTableInfoList(false).OrderBy(x => x.Name).ToList();
            foreach (var item in tables)
            {
                //sb.AppendFormat("insert into {0} select * from {1}.{0};", item.Name, "${dbName}").AppendLine();//oracle
                //sb.AppendFormat("insert into {0} select * from {1}.dbo.{0};", item.Name, "${dbName}").AppendLine();
                sb.AppendFormat("insert into HSZ_INIT_33.{0} select * from {1}.{0};", item.Name, "${dbName}").AppendLine();//dm
            }
            var sql = sb.ToString();
            return sql;
        }

        /// <summary>
        /// 模拟操作plc
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="arrived"></param>
        /// <param name="traycode1"></param>
        /// <param name="traycode2"></param>
        /// <param name="taskId1"></param>
        /// <param name="taskId2"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        [HttpPut("writedb")]
        [AllowAnonymous, IgnoreLog]
        public async Task WriteDbClassTest(string deviceId, bool arrived, string traycode1, string traycode2, int taskId1, int taskId2, short weight = 1000)
        {
            var plcObj = await db.Queryable<ZjnWcsPlcObjectEntity>().FirstAsync(w => w.DeviceId == deviceId && w.PackType == "READ");
            if (plcObj == null) return;
            var plc = await db.Queryable<ZjnWcsPlcEntity>().FirstAsync(w => w.PlcId == plcObj.PlcId);
            if (plc == null) return;
            try
            {
                Device device = new Device((CpuType)plc.CpuType, plc.Ip, plc.Port ?? 102,
                    Convert.ToInt16(plc.Rack), Convert.ToInt16(plc.Sock), plc.PlcId, plc.StackerId);
                await device.StartAsync();
                if (device.IsConnected)
                {
                    switch (plcObj.ObjType)
                    {
                        case "PackRead":
                            {
                                var packRead = device.ReadDbClass<PackRead>(plcObj.ObjType, plcObj.Db.Value, plcObj.Start.Value);
                                if (!arrived)
                                {
                                    packRead.RequestPlc = 2;
                                    packRead.TrayCode = packRead.GetS7StringToBytes(traycode1, traycode1.Length);
                                    packRead.TaskCode = taskId1;
                                    packRead.Reserve1 = weight;
                                }
                                else
                                {
                                    packRead.RequestPlc = 0;
                                    packRead.TrayCode = packRead.GetS7StringToBytes(string.Empty, 0);
                                    packRead.TaskCode = 0;
                                    packRead.Reserve1 = 0;
                                }
                                await packRead.WriteAsync();
                            }
                            break;
                        case "PackReadD":
                            {
                                var packRead = device.ReadDbClass<PackReadD>(plcObj.ObjType, plcObj.Db.Value, plcObj.Start.Value);
                                if (!arrived)
                                {
                                    packRead.RequestPlc = 2;
                                    packRead.TrayCode1 = packRead.GetS7StringToBytes(traycode1, traycode1.Length);
                                    packRead.TrayCode2 = packRead.GetS7StringToBytes(traycode2, traycode2.Length);
                                    packRead.TaskCode1 = taskId1;
                                    packRead.TaskCode2 = taskId2;
                                    packRead.Reserve1 = weight;
                                }
                                else
                                {
                                    packRead.RequestPlc = 0;
                                    packRead.TrayCode1 = packRead.GetS7StringToBytes(string.Empty, 0);
                                    packRead.TrayCode2 = packRead.GetS7StringToBytes(string.Empty, 0);
                                    packRead.TaskCode1 = 0;
                                    packRead.TaskCode2 = 0;
                                    packRead.Reserve1 = 0;
                                }
                                await packRead.WriteAsync();
                            }
                            break;
                        case "PackStackerRead":
                            {
                                var packRead = device.ReadDbClass<PackStackerRead>(plcObj.ObjType, plcObj.Db.Value, plcObj.Start.Value);
                                var plcWriteObj = await db.Queryable<ZjnWcsPlcObjectEntity>().FirstAsync(w => w.DeviceId == deviceId && w.PackType == "WRITE");
                                if (plcWriteObj == null) return;
                                var packWrite = device.ReadDbClass<PackStackerWrite>(plcWriteObj.ObjType, plcWriteObj.Db.Value, plcWriteObj.Start.Value);
                                if (!arrived)
                                {
                                    packRead.IsAuto = true;
                                    packRead.IsReady = true;
                                    packRead.IsAlarm = false;
                                    packRead.IsCloseDoor = true;
                                    packRead.IsForkTray = false;
                                    packRead.IsRunning = false;
                                    packRead.JobID = taskId1;
                                    packWrite.IsStart = false;
                                }
                                else
                                {
                                    packRead.IsAuto = true;
                                    packRead.IsAlarm = false;
                                    packRead.IsRunning = true;
                                    packRead.IsCloseDoor = true;
                                    packRead.IsComplete = true;
                                    packRead.JobID = taskId1;
                                    packWrite.IsResetAuto = false;
                                }
                                await packRead.WriteAsync();
                                await packWrite.WriteAsync();
                            }
                            break;
                        case "PackStackerReadD":
                            {
                                var packRead = device.ReadDbClass<PackStackerReadD>(plcObj.ObjType, plcObj.Db.Value, plcObj.Start.Value);
                                var plcWriteObj = await db.Queryable<ZjnWcsPlcObjectEntity>().FirstAsync(w => w.DeviceId == deviceId && w.PackType == "WRITE");
                                if (plcWriteObj == null) return;
                                var packWrite = device.ReadDbClass<PackStackerWriteD>(plcWriteObj.ObjType, plcWriteObj.Db.Value, plcWriteObj.Start.Value);
                                if (!arrived)
                                {
                                    packRead.IsAuto = true;
                                    packRead.IsReady = true;
                                    packRead.IsAlarm = false;
                                    packRead.IsCloseDoor = true;
                                    packRead.IsForkTray = false;
                                    packRead.IsRunning = false;
                                    packRead.JobID = taskId1;
                                    packWrite.IsStart = false;
                                }
                                else
                                {
                                    packRead.IsAuto = true;
                                    packRead.IsAlarm = false;
                                    packRead.IsRunning = true;
                                    packRead.IsCloseDoor = true;
                                    packRead.IsComplete = true;
                                    packRead.JobID = taskId1;
                                    packWrite.IsResetAuto = false;
                                }
                                await packRead.WriteAsync();
                                await packWrite.WriteAsync();
                            }
                            break;
                    }

                }
                device.Close();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
