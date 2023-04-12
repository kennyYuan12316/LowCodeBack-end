using HSZ.Dependency;
using HSZ.Entitys.wms;
using HSZ.FriendlyException;
using SqlSugar;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HSZ.Wms.ZjnWmsWorkPathAffirm
{
    /// <summary>
    /// 出库业务流程
    /// </summary>
    public class OutProcess : ITransient
    {
        private readonly ISqlSugarRepository<ZjnWcsProcessConfigEntity> _zjnWcsProcessConfigRepository;


        public OutProcess(
            ISqlSugarRepository<ZjnWcsProcessConfigEntity> zjnWcsProcessConfigRepository
            )
        {
            _zjnWcsProcessConfigRepository = zjnWcsProcessConfigRepository;
        }

        /// <summary>
        /// 出库不指定终点设备就是分流，指定则找设备路径
        /// </summary>
        /// <param name="workType"></param>
        /// <param name="goodsType"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task<string> GetOutProcess(int workType, string goodsType, string deviceId)
        {
            var outProcess = await _zjnWcsProcessConfigRepository.AsQueryable().Where(p => p.WorkType == workType)
                .WhereIF(string.IsNullOrEmpty(deviceId), p => p.GoodsType == goodsType)
                .WhereIF(!string.IsNullOrEmpty(deviceId), p => p.WorkEnd.Contains(deviceId)).FirstAsync();

            if (outProcess == null)
            {
                throw HSZException.Oh("业务路径数据不存在");
            }

            return outProcess.Id;

        }


    }
}
