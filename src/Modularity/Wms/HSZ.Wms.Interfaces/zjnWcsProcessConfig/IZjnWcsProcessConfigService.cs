using HSZ.wms.Entitys.Dto.zjnWcsProcessConfig;
using System.Threading.Tasks;

namespace HSZ.wms.Interfaces.ZjnServicePathConfig
{
    public interface IZjnWcsProcessConfigService
    {
        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ZjnWcsProcessConfigJsonOutput> PathData(string id);
    }
}