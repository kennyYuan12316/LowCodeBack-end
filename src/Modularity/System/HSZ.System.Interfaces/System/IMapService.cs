using HSZ.Common.Filter;
using HSZ.System.Entitys.Dto.System.Map;
using System.Threading.Tasks;

namespace HSZ.System.Interfaces.System
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：地图管理
    /// </summary>
    public interface IMapService
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns>返回列表</returns>
        Task<dynamic> GetList(PageInputBase input);
        /// <summary>
        /// 下拉列表
        /// </summary>
        Task<dynamic> GetSelector();
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns>返回对象</returns>
        Task<dynamic> GetInfo(string id);
        /// <summary>
        /// 获取地图数据
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>返回列表</returns>
        Task<object> GetMapData(string id);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        Task Delete(string id);
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        Task Create(MapCrInput input);
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        Task Update(string id,MapUpInput input);
    }
}
