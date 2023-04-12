using HSZ.Common.Const;
using HSZ.Common.Core.Manager;
using HSZ.Common.TaskResultPubilcParms;
using HSZ.Entitys.wms;
using HSZ.UnifyResult;
using HSZ.wms.Entitys.Dto.ZjnWmsTaskDetails;
using HSZ.wms.Interfaces.ZjnServicePathConfig;
using HSZ.wms.ZjnWmsTask;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSZ.FriendlyException;
using HSZ.Dependency;
using HSZ.wms.Interfaces.ZjnWmsTask;
using HSZ.Wms.Interfaces.ZjnWorkProsess;
using HSZ.Common.DI;

namespace HSZ.Wms.ZjnWorkProsess
{
    /// <summary>
    /// 子任务业务处理 --- 称重业务
    /// </summary>
    [WareDI(WareType.Production)]
    public class ProductionWeighDeviceProcess : IWeighDeviceProcess, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWmsTaskDetailsEntity> _zjnTaskListDetailsRepository;
        private readonly ISqlSugarRepository<ZjnWmsGoodsWeightEntity> _zjnWmsGoodsWeightRepository;
        private readonly ISqlSugarRepository<ZjnWmsTrayGoodsEntity> _zjnWmsTrayGoodsRepository;
        private readonly ISqlSugarRepository<ZjnWmsTrayEntity> _zjnWmsTrayRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        private readonly IZjnWmsTaskService _ZjnTaskService;

        /// <summary>
        /// 初始化
        /// </summary>
        public ProductionWeighDeviceProcess(
            IUserManager userManager,
            IZjnWmsTaskService zjnTaskService,
            ISqlSugarRepository<ZjnWmsTaskDetailsEntity> zjnTaskListDetailsRepository,
            ISqlSugarRepository<ZjnWmsTrayEntity> zjnWmsTrayRepository,
            ISqlSugarRepository<ZjnWmsTrayGoodsEntity> zjnWmsTrayGoodsRepository,
            ISqlSugarRepository<ZjnWmsGoodsWeightEntity> zjnWmsGoodsWeightRepository)
        {
            //只能作为事务处理
            _db = DbScoped.SugarScope;
            _zjnTaskListDetailsRepository = zjnTaskListDetailsRepository;
            _zjnWmsGoodsWeightRepository = zjnWmsGoodsWeightRepository;
            _zjnWmsTrayGoodsRepository = zjnWmsTrayGoodsRepository;
            _zjnWmsTrayRepository = zjnWmsTrayRepository;
            _ZjnTaskService = zjnTaskService;
        }


        /// <summary>
        /// 称重机业务处理  LES信息暂未上传
        /// </summary>
        /// <param name="WmsTaskData">子任务数据</param>
        /// <param name="TaskState">状态</param>
        /// <param name="parameter">重量</param>
        /// <returns></returns>
        public async Task<ZjnWmsTaskDetailsInfoOutput> TaskDetailsStart(ZjnWmsTaskDetailsEntity WmsTaskData, int TaskState, TaskResultPubilcParameter parameter)
        {
            //称重机方法有点特殊，相当于任务还未开始， 需要通过设备号作为条件，确定下一条子任务路径
            //1.称重校验，若不合格，解绑托盘，更改托盘状态、删除当前子任务缓存，取消主任务、子任务。
            //2.若合格，更改数据表里的子任务返回值字段（填写传过来的设备号）、完成状态。删除当前子任务缓存,再调用获取下一子任务接口,把下一子任务写入缓存、并回传子任务DTO

            //暂时调用WMS自己写的称重，后面再改成LES的
            if (await this.WmsPutinStorage(WmsTaskData, parameter))
            {
                //await _ZjnTaskService.ResetTask(WmsTaskData.TaskDetailsId.ToString(), TaskState);
                //return await _ZjnTaskService.GetNextTaskDetails(WmsTaskData.TaskDetailsId);
                return await _ZjnTaskService.FinishTask(WmsTaskData, TaskState);

            }
            await RedisHelper.HDelAsync(CommonConst.CACHE_KEY_TASK_LIST, WmsTaskData.TaskDetailsId.ToString());
            await _ZjnTaskService.CancelTask(WmsTaskData.TaskDetailsId);
            throw HSZException.Oh("称重不合格，任务取消");
        }



        /// <summary>
        /// 称重是否合格（不合格解绑托盘）
        /// </summary>
        /// <param name="WmsTaskData">子任务实体</param>
        /// <param name="parameter">公共参数获取重量</param>
        /// <returns></returns>
        public async Task<bool> WmsPutinStorage(ZjnWmsTaskDetailsEntity WmsTaskData, TaskResultPubilcParameter parameter)
        {
            try
            {
                var TrayGoods = await _zjnWmsTrayGoodsRepository.GetFirstAsync(x => x.TrayNo == WmsTaskData.TrayNo && x.EnabledMark == 1);
                if (TrayGoods == null)
                {
                    throw HSZException.Oh("托盘未与物料绑定!");
                }
                var Serial = await _zjnWmsGoodsWeightRepository.GetFirstAsync(x => x.GoodsCode == TrayGoods.GoodsCode && x.Max <= parameter.weigh);
                if (Serial == null)
                {
                    return true;
                }
                else
                {
                    var Tray = await _zjnWmsTrayRepository.GetFirstAsync(x => x.TrayNo == WmsTaskData.TrayNo);
                    if (Tray == null)
                    {
                        throw HSZException.Oh("托盘编号不存在!");
                    }
                    else
                    {
                        Tray.TrayStates = 1;


                        if (TrayGoods != null)
                        {

                            TrayGoods.IsDeleted = 1;
                            TrayGoods.EnabledMark = 1;
                            //开启事务
                            _db.BeginTran();
                            await _zjnWmsTrayRepository.AsUpdateable(Tray).ExecuteCommandAsync();
                            await _zjnWmsTrayGoodsRepository.AsUpdateable(TrayGoods).ExecuteCommandAsync();
                            _db.CommitTran();
                            throw HSZException.Oh("称重不合格!托盘解绑成功。").StatusCode(300);
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                _db.RollbackTran();
                throw HSZException.Oh(ex.Message);
            }

        }

    }

}
