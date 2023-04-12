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
using HSZ.wms.Entitys.Dto.ZjnPlaneTray;
using HSZ.wms.Interfaces.ZjnPlaneTray;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;
using HSZ.System.Entitys.System;
using System.Drawing;
using HSZ.Wms.Entitys.Dto.zjnPlaneGoods;
using HSZ.Wms.Entitys.Dto.zjnPlaneTray;

namespace HSZ.wms.ZjnPlaneTray
{
    /// <summary>
    /// 平面库托盘信息维护服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnPlaneTray", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnPlaneTrayService : IZjnPlaneTrayService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnPlaneTrayEntity> _zjnPlaneTrayRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;
        private readonly ISqlSugarRepository<DictionaryDataEntity> _dictionaryDataRepository;// 数据字典表仓储
        private List<ZjnPlaneTrayListOutput> GoodsState;
        private readonly ISqlSugarRepository<ZjnWmsOperationLogEntity> _zjnPlaneOperationLogRepository;

        /// <summary>
        /// 初始化一个<see cref="ZjnPlaneTrayService"/>类型的新实例
        /// </summary>
        public ZjnPlaneTrayService(ISqlSugarRepository<ZjnPlaneTrayEntity> zjnPlaneTrayRepository, ISqlSugarRepository<DictionaryDataEntity> dictionaryDataRepository,
            ISqlSugarRepository<ZjnWmsOperationLogEntity> zjnPlaneOperationLogRepository, IUserManager userManager)
        {
            _zjnPlaneOperationLogRepository = zjnPlaneOperationLogRepository;
            _dictionaryDataRepository = dictionaryDataRepository;
            _zjnPlaneTrayRepository = zjnPlaneTrayRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取平面库托盘信息维护
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnPlaneTrayRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnPlaneTrayInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取平面库托盘信息维护列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnPlaneTrayListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnPlaneTrayRepository.AsSugarClient().Queryable<ZjnPlaneTrayEntity>().Where(x=> x.IsDelete==0)
                .WhereIF(!string.IsNullOrEmpty(input.F_TrayNo), a => a.TrayNo.Contains(input.F_TrayNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_Type), a => a.Type.Equals(input.F_Type))
                .WhereIF(!string.IsNullOrEmpty(input.F_TrayState), a => a.TrayState.Equals(input.F_TrayState))
                .Select((a
)=> new ZjnPlaneTrayListOutput
                {
                    F_Id = a.Id,
                    F_TrayNo = a.TrayNo,
                    F_Type = a.Type,
                    F_TrayState = a.TrayState,
                    F_IsDelete = a.IsDelete,
                    F_DisableMark = a.DisableMark,
                    F_CreateUser = a.CreateUser,
                    F_CreateTime = a.CreateTime,
                    F_LastModifyUserId = a.LastModifyUserId,
                    F_LastModifyTime = a.LastModifyTime,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnPlaneTrayListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建平面库托盘信息维护
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnPlaneTrayCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnPlaneTrayEntity>();
            if (entity.TrayState==2)
            {
                throw HSZException.Oh("新增不能直接选择使用中状态");
            }

            entity.Id = YitIdHelper.NextId().ToString();
            entity.IsDelete = 0;
            entity.CreateTime = DateTime.Now;
            entity.CreateUser = userInfo.userId;
            var isOk = await _zjnPlaneTrayRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取平面库托盘信息维护无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] ZjnPlaneTrayListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnPlaneTrayRepository.AsSugarClient().Queryable<ZjnPlaneTrayEntity>().Where(x=> x.IsDelete==0)
                .WhereIF(!string.IsNullOrEmpty(input.F_TrayNo), a => a.TrayNo.Contains(input.F_TrayNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_Type), a => a.Type.Equals(input.F_Type))
                .WhereIF(!string.IsNullOrEmpty(input.F_TrayState), a => a.TrayState.Equals(input.F_TrayState))
                .Select((a
)=> new ZjnPlaneTrayListOutput
                {
                    F_Id = a.Id,
                    F_TrayNo = a.TrayNo,
                    F_Type = a.Type,
                    F_TrayState = a.TrayState,
                    F_IsDelete = a.IsDelete,
                    F_DisableMark = a.DisableMark,
                    F_CreateUser = a.CreateUser,
                    F_CreateTime = a.CreateTime,
                    F_LastModifyUserId = a.LastModifyUserId,
                    F_LastModifyTime = a.LastModifyTime,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出平面库托盘信息维护
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] ZjnPlaneTrayListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<ZjnPlaneTrayListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<ZjnPlaneTrayListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            exportData = exportData.Where(x=> x.F_IsDelete==0).ToList();
            List<ZjnPlaneTrayListOutput> list = new List<ZjnPlaneTrayListOutput>();
            foreach (var item in exportData)
            {
                var typeName = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "332411089230759173" && s.EnCode == item.F_Type.ToString()).Select(s => s.FullName).ToList();
                var TrayStateName = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "332412309932606725" && s.EnCode == item.F_TrayState.ToString()).Select(s => s.FullName).ToList();
                ZjnPlaneTrayListOutput planeTrayListOutput = new ZjnPlaneTrayListOutput();
                planeTrayListOutput.F_TrayNo = item.F_TrayNo;
                planeTrayListOutput.TypeNmae = typeName[0];
                planeTrayListOutput.TrayStateName = TrayStateName[0];
                planeTrayListOutput.F_CreateTime = item.F_CreateTime;
                planeTrayListOutput.F_CreateUser = item.F_CreateUser;
                planeTrayListOutput.F_DisableMark = item.F_DisableMark;
                planeTrayListOutput.F_LastModifyTime = item.F_LastModifyTime;
                planeTrayListOutput.F_LastModifyUserId = item.F_LastModifyUserId;
                list.Add(planeTrayListOutput);
            }
            List<ParamsModel> paramList = "[{\"value\":\"托盘编号\",\"field\":\"F_TrayNo\"},{\"value\":\"托盘类型\",\"field\":\"TypeNmae\"},{\"value\":\"托盘状态\",\"field\":\"TrayStateName\"},{\"value\":\"禁用原因\",\"field\":\"F_DisableMark\"},{\"value\":\"创建时间\",\"field\":\"F_CreateTime\"},{\"value\":\"创建人\",\"field\":\"F_CreateUser\"},{\"value\":\"修改时间\",\"field\":\"F_LastModifyTime\"},{\"value\":\"修改人\",\"field\":\"F_LastModifyUserId\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "托盘信息列表导出.xls";
            excelconfig.HeadFont = "微软雅黑";
            excelconfig.HeadPoint = 10;
            excelconfig.IsAllSizeColumn = true;
            excelconfig.ColumnModel = new List<ExcelColumnModel>();
            List<string> selectKeyList = input.selectKey.Split(',').ToList();
            foreach (var item in selectKeyList)
            {
                var value = "";
                if (item == "F_Type")
                {
                    value = "TypeNmae";
                }
                else if (item == "F_TrayState")
                {
                    value = "TrayStateName";
                }
                else {
                    value = item;
                
                }
                var isExist = paramList.Find(p => p.field == value);
                if (isExist != null)
                {
                    excelconfig.ColumnModel.Add(new ExcelColumnModel() { Column = isExist.field, ExcelColumn = isExist.value });
                }
            }
            var addPath = FileVariable.TemporaryFilePath + excelconfig.FileName;
            ExcelExportHelper<ZjnPlaneTrayListOutput>.Export(list, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 更新平面库托盘信息维护
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnPlaneTrayUpInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entityl = await _zjnPlaneTrayRepository.GetFirstAsync(p => p.Id.Equals(input.id));
            if (entityl.TrayState == 2)
            {
                throw HSZException.Oh("应该托盘在使用中，不可以修改!");
            }
            var entity = input.Adapt<ZjnPlaneTrayEntity>();
            if (entity.TrayNo!= entityl.TrayNo)
            {
                throw HSZException.Oh("托盘号不可以修改!");
            }
            entity.LastModifyUserId = userInfo.userId;
            entity.LastModifyTime = DateTime.Now;
            //插入日志
            ZjnWmsOperationLogEntity operationLogEntity = new ZjnWmsOperationLogEntity();
            operationLogEntity.CreateUser = userInfo.userId;
            operationLogEntity.CreateTime = DateTime.Now;
            operationLogEntity.Describe = "平面库托盘容器信息修改";
            operationLogEntity.AfterDate = entity.ToJson();//修改后的数据
            operationLogEntity.BeforeDate = entityl.ToJson();//修改前的数据
            operationLogEntity.Id = YitIdHelper.NextId().ToString();
            operationLogEntity.Type = 1;
            operationLogEntity.WorkPath = 2;
            try
            {
                //开启事务
                _db.BeginTran();
                //修改数据
                var isOk  = await _zjnPlaneTrayRepository.AsUpdateable(entity).ExecuteCommandAsync();
                //新增日子记录
                var isOk1 = await _zjnPlaneOperationLogRepository.AsInsertable(operationLogEntity).ExecuteReturnEntityAsync();

                _db.CommitTran();
            }
            catch (Exception e)
            {
                string es = e.Message;
                _db.RollbackTran();
                throw HSZException.Oh(ErrorCode.COM1001);

            }
            //var isOk = await _zjnPlaneTrayRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            //if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除平面库托盘信息维护
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = await _zjnPlaneTrayRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            if (entity.TrayState == 2)
            {
                throw HSZException.Oh("应该托盘在使用中，不可以删除!");
            }
            entity.LastModifyUserId = userInfo.userId;
            entity.LastModifyTime = DateTime.Now;
            entity.IsDelete = 1;


            // 插入日志
            ZjnWmsOperationLogEntity operationLogEntity = new ZjnWmsOperationLogEntity();
            operationLogEntity.CreateUser = userInfo.userId;
            operationLogEntity.CreateTime = DateTime.Now;
            operationLogEntity.Describe = "平面库托盘容器信息删除";
           // operationLogEntity.AfterDate = entity.ToJson();//修改后的数据
            operationLogEntity.BeforeDate = entity.ToJson();//修改前的数据
            operationLogEntity.Id = YitIdHelper.NextId().ToString();
            operationLogEntity.Type = 2;
            operationLogEntity.WorkPath = 2;
            try
            {
                //开启事务
                _db.BeginTran();
                //修改数据
                var isOk = await _zjnPlaneTrayRepository.AsUpdateable(entity).ExecuteCommandAsync();
                //新增日子记录
                var isOk1 = await _zjnPlaneOperationLogRepository.AsInsertable(operationLogEntity).ExecuteReturnEntityAsync();

                _db.CommitTran();
            }
            catch (Exception e)
            {
                string es = e.Message;
                _db.RollbackTran();
                throw HSZException.Oh(ErrorCode.COM1002);

            }


            
        }

        /// <summary>
        /// 模板下载
        /// </summary>
        /// <returns></returns>
        [HttpGet("TheRawMaterial")]
        public dynamic TheRawMaterial(string fileName)
        {

            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = fileName + ".xls";
            excelconfig.HeadFont = "微软雅黑";
            excelconfig.HeadPoint = 10;
            excelconfig.IsAllSizeColumn = true;
            excelconfig.ColumnModel = new List<ExcelColumnModel>();
            var filedList = GetTheRawMaterial();
            foreach (var item in filedList)
            {
                var column = item.Key;
                var excelColumn = item.Value;
               
                    excelconfig.ColumnModel.Add(new ExcelColumnModel() { Column = column, ExcelColumn = excelColumn });
                
            }
            //插入案例
            var typeName = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "332411089230759173").Select(s => new ZjnPlaneTrayListOutput { TypeNmae = s.FullName + ":" + s.EnCode }).ToList();
            var TrayStateName = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "332412309932606725").Select(s => new ZjnPlaneTrayListOutput { TypeNmae = s.FullName + ":" + s.EnCode }).ToList();

            var dataList = new List<ZjnPlaneTrayListOutput>() { new ZjnPlaneTrayListOutput() {

               TypeNmae="字段说明："+GettName(typeName),
               F_TrayNo="5454",
               TrayStateName=""+GettName(TrayStateName),
               F_DisableMark="案例不会导入"
            } };//初始化 一条空数据

            var addPath = FileVariable.TemporaryFilePath + excelconfig.FileName;
            ExcelExportHelper<ZjnPlaneTrayListOutput>.Export(dataList, excelconfig, addPath);
            return new { name = excelconfig.FileName, url = "/api/file/Download?encryption=" + DESCEncryption.Encrypt(_userManager.UserId + "|" + excelconfig.FileName + "|" + addPath, "HSZ") };

        }

        public string GettName(List<ZjnPlaneTrayListOutput> lists)
        {
            string Nmae = "";
            foreach (var item in lists)
            {
                Nmae += "" + item.TypeNmae;
            }
            return Nmae;
        }
        private Dictionary<string, string> GetTheRawMaterial(List<string> fields = null)
        {

            var res = new Dictionary<string, string>();
            res.Add("F_TrayNo", "**托盘编号**");
            res.Add("TypeNmae", "**托盘类型**");
            res.Add("TrayStateName", "**托盘状态**");
            res.Add("F_DisableMark", "禁用原因");      

            if (fields == null || !fields.Any()) return res;

            var result = new Dictionary<string, string>();

            foreach (var item in res)
            {
                if (fields.Contains(item.Key))
                    result.Add(item.Key, item.Value);
            }

            return result;

        }


        /// <summary>
        /// 导入预览
        /// </summary>
        /// <returns></returns>
        [HttpGet("ImportPreview")]
        public dynamic ImportPreview(string fileName)
        {
            try
            {
                var FileEncode = GetTheRawMaterial();

                var filePath = FileVariable.TemporaryFilePath;
                var savePath = filePath + fileName;
                //得到数据
                var excelData = ExcelImportHelper.ToDataTable(savePath);
                foreach (var item in excelData.Columns)
                {
                    excelData.Columns[item.ToString()].ColumnName = FileEncode.Where(x => x.Value == item.ToString()).FirstOrDefault().Key;
                }

                //返回结果
                return new { dataRow = excelData };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw HSZException.Oh(ErrorCode.D1801);
            }
        }


        /// <summary>
        /// 入库导入数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost("RawMaterialWarehousingData")]
        public async Task<dynamic> RawMaterialWarehousingData(List<ZjnPlaneTrayListOutput> list)
        {
            var res = await ImportTheRawMaterialDatas(list);
            var addlist = res.First() as List<ZjnPlaneTrayListOutput>;
            var errorlist = res.Last() as List<ZjnPlaneTrayListOutput>;
            return new ZjnPlaneTrayImportResultOutput() { snum = addlist.Count, fnum = errorlist.Count, failResult = errorlist, resultType = errorlist.Count < 1 ? 0 : 1 };
        }



        /// <summary>
        /// 导入数据函数
        /// </summary>
        /// <param name="list">list</param>
        /// <returns>[成功列表,失败列表]</returns>
        private async Task<object[]> ImportTheRawMaterialDatas(List<ZjnPlaneTrayListOutput> list)
        {
            List<ZjnPlaneTrayListOutput> PlaneGoodsList = list;

            #region 排除错误数据
            if (PlaneGoodsList == null || PlaneGoodsList.Count() < 1)
                throw HSZException.Oh(ErrorCode.D5019);

            //必填字段验证 
            var errorList = PlaneGoodsList.Where(x => string.IsNullOrEmpty(x.TrayStateName) || string.IsNullOrEmpty(x.TypeNmae.ToString()) || string.IsNullOrEmpty(x.F_TrayNo)).ToList();
            var _zjnBillsHistoryRepositoryList = await _zjnPlaneTrayRepository.GetListAsync();//数据集
            var repeat = _zjnBillsHistoryRepositoryList.Where(u => PlaneGoodsList.Select(x => x.F_TrayNo).Contains(u.TrayNo)).ToList();//已存在的编号           
            if (repeat.Any())
                errorList.AddRange(PlaneGoodsList.Where(u => repeat.Select(x => x.TrayNo).Contains(u.F_TrayNo) && !errorList.Select(x => x.F_TrayNo).Contains(u.F_TrayNo)));//已存在的编号 列入 错误列表

            PlaneGoodsList = PlaneGoodsList.Except(errorList).ToList();
            #endregion

            var PlaneGoods = new List<ZjnPlaneTrayEntity>();
            var userInfo = await _userManager.GetUserInfo();

            foreach (var item in PlaneGoodsList)
            {
                var uentity = new ZjnPlaneTrayEntity();
                uentity.Id = YitIdHelper.NextId().ToString();
                uentity.CreateTime = DateTime.Now;
                uentity.CreateUser = userInfo.userId;
                uentity.TrayNo = item.F_TrayNo;                
                uentity.Type = Convert.ToInt32(item.TypeNmae);
                uentity.TrayState = Convert.ToInt32(item.TrayStateName);                
                uentity.DisableMark = item.F_DisableMark;
                uentity.IsDelete = 0;
                PlaneGoods.Add(uentity);
            }

            if (PlaneGoods.Any())
            {
                try
                {
                    //开启事务
                    _db.BeginTran();

                    //新增记录
                    var newEntity = await _zjnPlaneTrayRepository.AsInsertable(PlaneGoods).ExecuteReturnEntityAsync();

                    _db.CommitTran();
                }
                catch (Exception e)
                {
                    string es = e.Message;
                    _db.RollbackTran();
                    errorList.AddRange(PlaneGoodsList);
                }
            }
            return new object[] { PlaneGoodsList, errorList };
        }


        /// <summary>
        /// 导出错误报告
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost("ExportExceptionData")]
        public async Task<dynamic> ExportExceptionData(List<ZjnPlaneTrayListOutput> list)
        {
            var res = await ImportTheRawMaterialDatas(list);
            var errorlist = res.Last() as List<ZjnPlaneTrayListOutput>;//错误数据

            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "托盘容器信息导入错误报告.xls";
            excelconfig.HeadFont = "微软雅黑";
            excelconfig.HeadPoint = 10;
            excelconfig.IsAllSizeColumn = true;
            excelconfig.ColumnModel = new List<ExcelColumnModel>();
            var filedList = GetTheRawMaterial();
            foreach (var item in filedList)
            {
                var column = item.Key;
                var excelColumn = item.Value;
                excelconfig.ColumnModel.Add(new ExcelColumnModel() { Column = column, ExcelColumn = excelColumn });
            }
            var addPath = FileVariable.TemporaryFilePath + excelconfig.FileName;
            ExcelExportHelper<ZjnPlaneTrayListOutput>.Export(errorlist, excelconfig, addPath);

            return new { name = excelconfig.FileName, url = "/api/file/Download?encryption=" + DESCEncryption.Encrypt(_userManager.UserId + "|" + excelconfig.FileName + "|" + addPath, "HSZ") };
        }

    }
}


