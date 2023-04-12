using HSZ.Dependency;
using HSZ.Entitys.wms;
using HSZ.FriendlyException;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSZ.Wms.ZjnWmsWorkPathAffirm
{
    public class LineIntoProcess : ITransient
    {
        private readonly ISqlSugarRepository<ZjnWcsProcessConfigEntity> _zjnWcsProcessConfigRepository;

        public LineIntoProcess(ISqlSugarRepository<ZjnWcsProcessConfigEntity> zjnWcsProcessConfigRepository)
        {
            _zjnWcsProcessConfigRepository = zjnWcsProcessConfigRepository;
        }
        /// <summary>
        /// 组盘入线体存储获取业务路径
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="workType"></param>
        /// <returns></returns>
        public async Task<string> GetLineIntoProcess(string deviceId, int workType)
        {
            try
            {
                var lineIntoProcess = await _zjnWcsProcessConfigRepository.AsQueryable().Where(p => p.WorkType == workType)
                    .WhereIF(!string.IsNullOrEmpty(deviceId), p => p.WorkStart.Contains(deviceId)).FirstAsync();

                if (lineIntoProcess == null)
                {
                    throw HSZException.Oh("业务路径数据不存在");
                }
                return lineIntoProcess.Id;
            }
            catch (Exception ex)
            {

            }

            return null;

        }
    }
}
