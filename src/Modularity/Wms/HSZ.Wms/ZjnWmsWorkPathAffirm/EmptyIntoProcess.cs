using HSZ.Dependency;
using HSZ.Entitys.wms;
using HSZ.FriendlyException;
using SqlSugar;
using System;
using System.Threading.Tasks;

namespace HSZ.Wms.ZjnWmsWorkPathAffirm
{
    /// <summary>
    /// 空托入库业务流程
    /// </summary>
    public class EmptyIntoProcess: ITransient
    {
        private readonly ISqlSugarRepository<ZjnWcsProcessConfigEntity> _zjnWcsProcessConfigRepository;


        public EmptyIntoProcess(
            ISqlSugarRepository<ZjnWcsProcessConfigEntity> zjnWcsProcessConfigRepository
            )
        {
            _zjnWcsProcessConfigRepository = zjnWcsProcessConfigRepository;
        }


        public async Task<string> GetIntoProcess(int workType, string deviceId)
        {
            try
            {
                var intoProcess = await _zjnWcsProcessConfigRepository.GetFirstAsync(p => p.WorkType == workType && p.WorkStart.Contains(deviceId));
                if (intoProcess == null)
                {
                    throw HSZException.Oh("业务路径数据不存在");
                }

                return intoProcess.Id;
            }
            catch (Exception ex)
            {

            }

            return null;

        }


    }
}
