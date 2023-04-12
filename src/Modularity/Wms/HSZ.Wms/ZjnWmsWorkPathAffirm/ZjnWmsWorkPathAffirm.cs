using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Entitys.wms;
using HSZ.FriendlyException;
using HSZ.Wms.Interfaces.ZjnWmsWorkPathAffirm;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Threading.Tasks;

namespace HSZ.Wms.ZjnWmsWorkPathAffirm
{



    [ApiDescriptionSettings(Tag = "wms", Name = "ZjnWmsWorkPathAffirm", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnWmsWorkPathAffirm : IZjnWmsWorkPathAffirm, IDynamicApiController, ITransient
    {

        private readonly ISqlSugarRepository<ZjnWmsTrayEntity> _zjnWmsTrayRepository;
        private readonly ISqlSugarRepository<ZjnWmsTrayGoodsEntity> _zjnWmsTrayGoodsRepository;
        private readonly ISqlSugarRepository<ZjnWcsWorkDeviceEntity> _zjnWmsWorkDeviceRepository;
        private readonly ISqlSugarRepository<ZjnWmsGoodsEntity> _zjnWmsGoodsEntityRepository;
        private readonly ISqlSugarRepository<ZjnWcsProcessConfigEntity> _zjnWcsProcessConfigRepository;
        private readonly SqlSugarScope _db;

        //按任务类型区分处理业务
        private readonly OutProcess _OutProcess;
        private readonly IntoProcess _IntoProcess;
        private readonly EmptyIntoProcess _EmptyIntoProcess;
        private readonly EmptyOutProcess _EmptyOutProcess;
        private readonly LineIntoProcess _lineIntoProcess;
        private readonly LineOutProcess _lineOutProcess;

        public ZjnWmsWorkPathAffirm(

           ISqlSugarRepository<ZjnWmsTrayEntity> zjnWmsTrayRepository,
           ISqlSugarRepository<ZjnWmsTrayGoodsEntity> zjnWmsTrayGoodsRepository,
           ISqlSugarRepository<ZjnWmsGoodsEntity> zjnWmsGoodsEntityRepository,
           ISqlSugarRepository<ZjnWcsWorkDeviceEntity> zjnWmsWorkDeviceRepository,
           ISqlSugarRepository<ZjnWcsProcessConfigEntity> zjnWcsProcessConfigRepository,
           OutProcess OutProcess,
           IntoProcess IntoProcess,
           EmptyIntoProcess EmptyIntoProcess,
           EmptyOutProcess EmptyOutProcess,
           LineIntoProcess lineIntoProcess,
           LineOutProcess lineOutProcess
            )
        {
            _EmptyOutProcess = EmptyOutProcess;
            _EmptyIntoProcess = EmptyIntoProcess;
            _OutProcess = OutProcess;
            _IntoProcess = IntoProcess;
            _db = DbScoped.SugarScope;
            _zjnWmsTrayRepository = zjnWmsTrayRepository;
            _zjnWmsTrayGoodsRepository = zjnWmsTrayGoodsRepository;
            _zjnWmsWorkDeviceRepository = zjnWmsWorkDeviceRepository;
            _zjnWmsGoodsEntityRepository = zjnWmsGoodsEntityRepository;
            _zjnWcsProcessConfigRepository = zjnWcsProcessConfigRepository;
            _lineIntoProcess = lineIntoProcess;
            _lineOutProcess = lineOutProcess;
        }

        /// <summary>
        /// 获取业务流程ID
        /// </summary>
        /// <param name="trayNo"></param>托盘码
        /// <param name="deviceId"></param>设备ID
        /// <param name="goodsCode"></param>物料编码
        /// <param name="workType"></param>任务类型-出入库
        /// <returns></returns>
        [HttpGet("GetWorkPathId/{trayNo}/{deviceId}/{goodsCode}/{workType}")]
        public async Task<string> GetWorkPathId(string trayNo, string deviceId, string goodsCode, int workType)
        {
            try
            {

                //获取托盘数据--托盘类型
                var tray = await _zjnWmsTrayRepository.GetFirstAsync(w => w.TrayNo == trayNo && w.IsDelete == 0 && w.EnabledMark == 1);
                if (tray == null)
                {
                    throw HSZException.Oh("未找到托盘数据");
                }
                //获取设备数据
                if (workType == 2)
                {
                    var workDevice = await _zjnWmsWorkDeviceRepository.GetFirstAsync(d => d.DeviceId == deviceId && d.IsDelete == 0 && d.IsActive == 1);
                    if (workDevice == null)
                    {
                        throw HSZException.Oh("未找到设备数据");
                    }
                    if (tray.TrayStates != 1)
                    {
                        throw HSZException.Oh("当前托盘已占用，请确认后解绑或更换条码");
                    }
                }
                ZjnWmsGoodsEntity goods = null;
                //获取托盘物料绑定数据
                // var trayGoods = await _zjnWmsTrayGoodsRepository.GetFirstAsync(g => g.TrayNo == trayNo && g.GoodsCode == goodsCode);               
                if (workType != 6 && workType != 5)
                {
                    //获取物料信息
                    goods = await _zjnWmsGoodsEntityRepository.GetFirstAsync(g => g.GoodsCode == goodsCode && g.IsDelete == 0);
                    if (goods == null)
                    {
                        throw HSZException.Oh("未找到该物料数据");
                    }
                }

                var process = "";
                switch (workType)
                {
                    case 1://出库
                        process = await _OutProcess.GetOutProcess(1, goods.GoodsType, deviceId);
                        break;
                    case 2://入库
                        process = await _IntoProcess.GetIntoProcess(2, goods.GoodsType, deviceId);
                        break;
                    case 3://移库

                        break;
                    case 4://其他

                        break;
                    case 5://空托入库
                        process = await _EmptyIntoProcess.GetIntoProcess(5, deviceId);
                        break;
                    case 6://空托出库
                        process = await _EmptyOutProcess.GetOutProcess(6, deviceId);
                        break;
                    case 7://组盘入线体存储
                        process = await _lineIntoProcess.GetLineIntoProcess(deviceId, workType);
                        break;
                    case 8://投产叫料出线体存储
                        process = await _lineOutProcess.GetLineOutProcess(deviceId, workType);
                        break;
                    default:

                        break;
                }

                return process;
            }
            catch (Exception ex)
            {
                throw HSZException.Oh(ex.Message);
            }

        }
        /// <summary>
        /// 线体存储业务单独获取业务唯一ID
        /// </summary>
        /// <param name="goodsType"></param>物料类型
        /// <param name="deviceId"></param>设备ID
        /// <param name="workType"></param>任务类型-出入库
        /// <returns></returns>
        [HttpGet("GetWorkPathIdByLine/{goodsType}/{deviceId}/{workType}")]
        public async Task<string> GetWorkPathIdByLine(string deviceId, int workType)
        {
            try
            {
                var process = "";
                switch (workType)
                {
                    case 7://组盘入线体存储
                        process = await _lineIntoProcess.GetLineIntoProcess(deviceId, workType);
                        break;
                    case 8://投产叫料出线体存储
                        process = await _lineOutProcess.GetLineOutProcess(deviceId, workType);
                        break;
                    default:

                        break;
                }

                return process;
            }
            catch (Exception ex)
            {
                throw HSZException.Oh(ex.Message);
            }

        }


    }
}
