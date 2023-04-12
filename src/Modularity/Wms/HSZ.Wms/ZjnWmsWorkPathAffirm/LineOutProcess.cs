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
    public class LineOutProcess : ITransient
    {
        private readonly ISqlSugarRepository<ZjnWcsProcessConfigEntity> _zjnWcsProcessConfigRepository;

        public LineOutProcess(ISqlSugarRepository<ZjnWcsProcessConfigEntity> zjnWcsProcessConfigRepository)
        {
            _zjnWcsProcessConfigRepository = zjnWcsProcessConfigRepository;
        }
        /// <summary>
        /// 产线叫料出线体存储获取业务路径
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="workType"></param>
        /// <returns></returns>
        public async Task<string> GetLineOutProcess(string deviceId, int workType)
        {
            try
            {
                var lineOutProcess = await _zjnWcsProcessConfigRepository.AsQueryable().Where(p => p.WorkType == workType)
                    .WhereIF(!string.IsNullOrEmpty(deviceId), p => p.WorkEnd.Contains(deviceId)).FirstAsync();

                if (lineOutProcess == null)
                {
                    throw HSZException.Oh("业务路径数据不存在");
                }
                return lineOutProcess.Id;
            }
            catch (Exception ex)
            {

            }

            return null;

        }
    }
}
