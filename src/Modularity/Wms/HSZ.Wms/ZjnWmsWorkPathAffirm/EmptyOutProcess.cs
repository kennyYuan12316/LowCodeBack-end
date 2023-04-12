using HSZ.Dependency;
using HSZ.Entitys.wms;
using HSZ.FriendlyException;
using SqlSugar;
using System;
using System.Threading.Tasks;

namespace HSZ.Wms.ZjnWmsWorkPathAffirm
{
    /// <summary>
    /// 空托出库业务流程
    /// </summary>
    public class EmptyOutProcess: ITransient
    {
        private readonly ISqlSugarRepository<ZjnWcsProcessConfigEntity> _zjnWcsProcessConfigRepository;


        public EmptyOutProcess(
            ISqlSugarRepository<ZjnWcsProcessConfigEntity> zjnWcsProcessConfigRepository
            )
        {
            _zjnWcsProcessConfigRepository = zjnWcsProcessConfigRepository;
        }


        public async Task<string> GetOutProcess(int workType, string deviceId)
        {
            try
            { 
                var outProcess = await _zjnWcsProcessConfigRepository.GetFirstAsync(p=>p.WorkType== workType && (p.GoodsType==null || p.GoodsType=="" || p.GoodsType == "4") && p.WorkEnd.Contains(deviceId));
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
