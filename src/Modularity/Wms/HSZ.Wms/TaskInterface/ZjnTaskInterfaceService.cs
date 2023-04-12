using HSZ.Common.Core.Manager;
using HSZ.ClayObject;
using HSZ.Common.Configuration;
using HSZ.Common.Model.NPOI;
using HSZ.DataEncryption;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Common.Helper;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Entitys.wms;
using HSZ.FriendlyException;
using HSZ.JsonSerialization;
using HSZ.wms.Entitys.Dto.ZjnTaskInterface;
using HSZ.wms.Interfaces.ZjnTaskInterface;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnTaskInterface
{
    /// <summary>
    /// 接口配置服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnTaskInterface", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnTaskInterfaceService : IZjnTaskInterfaceService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnTaskInterfaceEntity> _zjnTaskInterfaceRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnTaskInterfaceService"/>类型的新实例
        /// </summary>
        public ZjnTaskInterfaceService(ISqlSugarRepository<ZjnTaskInterfaceEntity> zjnTaskInterfaceRepository,
            IUserManager userManager)
        {
            _zjnTaskInterfaceRepository = zjnTaskInterfaceRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取接口配置
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnTaskInterfaceRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnTaskInterfaceInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取接口配置列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnTaskInterfaceListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnTaskInterfaceRepository.AsSugarClient().Queryable<ZjnTaskInterfaceEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_NameInterface), a => a.NameInterface.Contains(input.F_NameInterface))
                .WhereIF(!string.IsNullOrEmpty(input.F_Communication), a => a.Communication.Equals(input.F_Communication))
                .WhereIF(!string.IsNullOrEmpty(input.F_InterfaceProvide), a => a.InterfaceProvide.Contains(input.F_InterfaceProvide))
                .Select((a
)=> new ZjnTaskInterfaceListOutput
                {
                    F_Id = a.Id,
                    F_NameInterface = a.NameInterface,
                    F_CnInterface = a.CnInterface,
                    F_EnterParameter = a.EnterParameter,
                    F_OutParameter = a.OutParameter,
                    F_Communication = a.Communication,
                    F_Introduction = a.Introduction,
                    F_InterfaceProvide = a.InterfaceProvide,
                    F_CreateUser = a.CreateUser,
                    F_CreateTime = a.CreateTime,
                    F_EnabledMark = SqlFunc.IIF(a.EnabledMark == 0, "关", "开"),
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnTaskInterfaceListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建接口配置
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnTaskInterfaceCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnTaskInterfaceEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateUser = _userManager.UserId;
            entity.CreateTime = DateTime.Now;
            
            var isOk = await _zjnTaskInterfaceRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取接口配置无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] ZjnTaskInterfaceListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnTaskInterfaceRepository.AsSugarClient().Queryable<ZjnTaskInterfaceEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_NameInterface), a => a.NameInterface.Contains(input.F_NameInterface))
                .WhereIF(!string.IsNullOrEmpty(input.F_Communication), a => a.Communication.Equals(input.F_Communication))
                .WhereIF(!string.IsNullOrEmpty(input.F_InterfaceProvide), a => a.InterfaceProvide.Contains(input.F_InterfaceProvide))
                .Select((a
)=> new ZjnTaskInterfaceListOutput
                {
                    F_Id = a.Id,
                    F_NameInterface = a.NameInterface,
                    F_CnInterface = a.CnInterface,
                    F_EnterParameter = a.EnterParameter,
                    F_OutParameter = a.OutParameter,
                    F_Communication = a.Communication,
                    F_Introduction = a.Introduction,
                    F_InterfaceProvide = a.InterfaceProvide,
                    F_CreateUser = a.CreateUser,
                    F_CreateTime = a.CreateTime,
                    F_EnabledMark = SqlFunc.IIF(a.EnabledMark == 0, "关", "开"),
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出接口配置
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] ZjnTaskInterfaceListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<ZjnTaskInterfaceListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<ZjnTaskInterfaceListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"接口名称\",\"field\":\"nameInterface\"},{\"value\":\"中文\",\"field\":\"cnInterface\"},{\"value\":\"入参\",\"field\":\"enterParameter\"},{\"value\":\"出参\",\"field\":\"outParameter\"},{\"value\":\"通讯协议\",\"field\":\"communication\"},{\"value\":\"说明\",\"field\":\"introduction\"},{\"value\":\"接口提供者\",\"field\":\"interfaceProvide\"},{\"value\":\"有效标志\",\"field\":\"enabledMark\"},{\"value\":\"创建者\",\"field\":\"createUser\"},{\"value\":\"创建时间\",\"field\":\"createTime\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "接口配置.xls";
            excelconfig.HeadFont = "微软雅黑";
            excelconfig.HeadPoint = 10;
            excelconfig.IsAllSizeColumn = true;
            excelconfig.ColumnModel = new List<ExcelColumnModel>();
            List<string> selectKeyList = input.selectKey.Split(',').ToList();
            foreach (var item in selectKeyList)
            {
                var isExist = paramList.Find(p => p.field == item);
                if (isExist != null)
                {
                    excelconfig.ColumnModel.Add(new ExcelColumnModel() { Column = isExist.field, ExcelColumn = isExist.value });
                }
            }
            var addPath = FileVariable.TemporaryFilePath + excelconfig.FileName;
            ExcelExportHelper<ZjnTaskInterfaceListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 更新接口配置
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnTaskInterfaceUpInput input)
        {
            var entity = input.Adapt<ZjnTaskInterfaceEntity>();
            var isOk = await _zjnTaskInterfaceRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除接口配置
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnTaskInterfaceRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnTaskInterfaceRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


