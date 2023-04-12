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
    /// 入库业务流程
    /// </summary>
    public class IntoProcess : ITransient
    {
        private readonly ISqlSugarRepository<ZjnWcsProcessConfigEntity> _zjnWcsProcessConfigRepository;


        public IntoProcess(
            ISqlSugarRepository<ZjnWcsProcessConfigEntity> zjnWcsProcessConfigRepository
            )
        {
            _zjnWcsProcessConfigRepository = zjnWcsProcessConfigRepository;
        }


        public async Task<string> GetIntoProcess(int workType, string goodsType, string deviceId)
        {
            try
            {
                var outProcess = await _zjnWcsProcessConfigRepository.AsQueryable().Where(p => p.WorkType == workType)
                    .WhereIF(string.IsNullOrEmpty(deviceId), p => p.GoodsType == goodsType)
                    .WhereIF(!string.IsNullOrEmpty(deviceId), p => p.WorkStart.Contains(deviceId)).FirstAsync();

                if (outProcess == null)
                {
                    throw HSZException.Oh("业务路径数据不存在");
                }

                return outProcess.Id;
            }
            catch (Exception ex)
            {

            }

            return null;

        }


    }
}
