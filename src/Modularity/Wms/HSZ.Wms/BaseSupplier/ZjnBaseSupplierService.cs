using HSZ.Common.Configuration;
using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Common.Helper;
using HSZ.Common.Model.NPOI;
using HSZ.DataEncryption;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Entitys.wms;
using HSZ.FriendlyException;
using HSZ.JsonSerialization;
using HSZ.System.Entitys.Permission;
using HSZ.wms.Entitys.Dto.BaseSupplier;
using HSZ.wms.Interfaces.BaseSupplier;
using HSZ.Wms.Entitys.Dto.BaseSupplier;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.BaseSupplier
{
    /// <summary>
    /// 供应商管理服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms", Name = "BaseSupplier", Order = 200)]
    [Route("api/wms/[controller]")]
    public class BaseSupplierService : IBaseSupplierService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnBaseSupplierEntity> _zjnBaseSupplierRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;
        private readonly ISqlSugarRepository<ZjnWmsOperationLogEntity> _zjnPlaneOperationLogRepository;

        /// <summary>
        /// 初始化一个<see cref="BaseSupplierService"/>类型的新实例
        /// </summary>
        public BaseSupplierService(ISqlSugarRepository<ZjnBaseSupplierEntity> zjnBaseSupplierRepository, ISqlSugarRepository<ZjnWmsOperationLogEntity> zjnPlaneOperationLogRepository,
            IUserManager userManager)
        {
            _zjnPlaneOperationLogRepository = zjnPlaneOperationLogRepository;
            _zjnBaseSupplierRepository = zjnBaseSupplierRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取供应商管理
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnBaseSupplierRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnBaseSupplierInfoOutput>();
            return output;
        }

        /// <summary>
        /// 判断供应商编号是否存在
        /// </summary>
        /// <param name="SupplierNo"></param>
        /// <returns></returns>
        [HttpGet("ExistSupplierNo")]
        public async Task<bool> ExistSupplierNo(string SupplierNo)
        {
            var output = await _zjnBaseSupplierRepository.IsAnyAsync(p => p.SupplierNo == SupplierNo && p.IsDelete==0);
            return output;
        }

        /// <summary>
        /// 模板下载
        /// </summary>
        /// <returns></returns>
        [HttpGet("CreateModel")]
        public dynamic CreateModel()
        {
            var dataList = new List<ZjnBaseSupplierCrInput>() { new ZjnBaseSupplierCrInput() { } };//初始化 一条空数据

            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "供应商信息导入模板.xls";
            excelconfig.HeadFont = "微软雅黑";
            excelconfig.HeadPoint = 10;
            excelconfig.IsAllSizeColumn = true;
            excelconfig.ColumnModel = new List<ExcelColumnModel>();
            var filedList = GetUserInfoFieldToTitle();
            foreach (var item in filedList)
            {
                var column = item.Key;
                var excelColumn = item.Value;
                excelconfig.ColumnModel.Add(new ExcelColumnModel() { Column = column, ExcelColumn = excelColumn });
            }
            var addPath = FileVariable.TemporaryFilePath + excelconfig.FileName;
            ExcelExportHelper<ZjnBaseSupplierCrInput>.Export(dataList, excelconfig, addPath);

            return new { name = excelconfig.FileName, url = "/api/file/Download?encryption=" + DESCEncryption.Encrypt(_userManager.UserId + "|" + excelconfig.FileName + "|" + addPath, "HSZ") };
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
                var FileEncode = GetUserInfoFieldToTitle();

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
        /// 导入数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost("ImportData")]
        public async Task<dynamic> ImportData(List<ZjnBaseSupplierCrInput> list)
        {
            var res = await ImportSupplierDatas(list);
            var addlist = res.First() as List<ZjnBaseSupplierCrInput>;
            var errorlist = res.Last() as List<ZjnBaseSupplierCrInput>;
            return new SupplierImportResultOutput() { snum = addlist.Count, fnum = errorlist.Count, failResult = errorlist, resultType = errorlist.Count < 1 ? 0 : 1 };
        }

        /// <summary>
        /// 导入供应商数据函数
        /// </summary>
        /// <param name="list">list</param>
        /// <returns>[成功列表,失败列表]</returns>
        private async Task<object[]> ImportSupplierDatas(List<ZjnBaseSupplierCrInput> list)
        {
            List<ZjnBaseSupplierCrInput> supplierInputList = list;

            #region 排除错误数据
            if (supplierInputList == null || supplierInputList.Count() < 1)
                throw HSZException.Oh(ErrorCode.D5019);

            //必填字段验证 (供应商编号，供应商名称)
            var errorList = supplierInputList.Where(x => !x.supplierNo.IsNotEmptyOrNull() || !x.supplierName.IsNotEmptyOrNull()).ToList();
            var _zjnBaseSupplierRepositoryList = await _zjnBaseSupplierRepository.GetListAsync();//供应商数据集

            var repeat = _zjnBaseSupplierRepositoryList.Where(u => supplierInputList.Select(x => x.supplierNo).Contains(u.SupplierNo)).ToList();//已存在的编号

            if (repeat.Any())
                errorList.AddRange(supplierInputList.Where(u => repeat.Select(x => x.SupplierNo).Contains(u.supplierNo)));//已存在的编号 列入 错误列表

            supplierInputList = supplierInputList.Except(errorList).ToList();
            #endregion

            var zjnBaseSuppList = new List<ZjnBaseSupplierEntity>();
            var userInfo = await _userManager.GetUserInfo();

            foreach (var item in supplierInputList)
            {
                var uentity = new ZjnBaseSupplierEntity();
                uentity.Id = YitIdHelper.NextId().ToString();
                uentity.CreateTime = DateTime.Now;
                uentity.CreateUser = userInfo.userId;
                uentity.EnabledMark = 1;
                uentity.SupplierNo = item.supplierNo;
                uentity.SupplierName = item.supplierName;
                uentity.ModifiedUser = userInfo.userId;
                uentity.ModifiedTime = DateTime.Now;
                uentity.IsDelete = 0;
                zjnBaseSuppList.Add(uentity);
            }

            if (zjnBaseSuppList.Any())
            {
                try
                {
                    //开启事务
                    _db.BeginTran();

                    //新增供应商记录
                    var newEntity = await _zjnBaseSupplierRepository.AsInsertable(zjnBaseSuppList).ExecuteReturnEntityAsync();

                    _db.CommitTran();
                }
                catch (Exception e)
                {
                    string es = e.Message;
                    _db.RollbackTran();
                    errorList.AddRange(supplierInputList);
                }
            }
            return new object[] { supplierInputList, errorList };
        }

        /// <summary>
        /// 导出错误报告
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost("ExportExceptionData")]
        public async Task<dynamic> ExportExceptionData(List<ZjnBaseSupplierCrInput> list)
        {
            var res = await ImportSupplierDatas(list);
            var errorlist = res.Last() as List<ZjnBaseSupplierCrInput>;//错误数据

            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "供应商导入错误报告.xls";
            excelconfig.HeadFont = "微软雅黑";
            excelconfig.HeadPoint = 10;
            excelconfig.IsAllSizeColumn = true;
            excelconfig.ColumnModel = new List<ExcelColumnModel>();
            var filedList = GetUserInfoFieldToTitle();
            foreach (var item in filedList)
            {
                var column = item.Key;
                var excelColumn = item.Value;
                excelconfig.ColumnModel.Add(new ExcelColumnModel() { Column = column, ExcelColumn = excelColumn });
            }
            var addPath = FileVariable.TemporaryFilePath + excelconfig.FileName;
            ExcelExportHelper<ZjnBaseSupplierCrInput>.Export(errorlist, excelconfig, addPath);

            return new { name = excelconfig.FileName, url = "/api/file/Download?encryption=" + DESCEncryption.Encrypt(_userManager.UserId + "|" + excelconfig.FileName + "|" + addPath, "HSZ") };
        }



        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("ExportExcel")]
        public async Task<dynamic> ExportExcel([FromQuery] ZjnBaseSupplierListQueryInput input)
        {
            //供应商信息列表
            var userList = new List<ZjnBaseSupplierInfoOutput>();
            var sidx = input.sidx == null ? "F_Id" : input.sidx;

            if (input.dataType == "0")
            {
                userList = await _zjnBaseSupplierRepository.AsSugarClient().Queryable<ZjnBaseSupplierEntity>()
                .Where(a=>a.IsDelete==0)
                .WhereIF(!string.IsNullOrEmpty(input.F_SupplierNo), a => a.SupplierNo.Contains(input.F_SupplierNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_SupplierName), a => a.SupplierName.Contains(input.F_SupplierName))
                .Select((a) => new ZjnBaseSupplierInfoOutput
                {
                    id = a.Id,
                    supplierNo = a.SupplierNo,
                    supplierName = a.SupplierName,
                    createUser = a.CreateUser,
                    createTime = a.CreateTime,
                    modifiedUser = a.ModifiedUser,
                    modifiedTime = a.ModifiedTime,
                }).OrderBy(sidx + " " + input.sort).ToPageListAsync(input.currentPage, input.pageSize);
            }
            else
            {
                userList = await _zjnBaseSupplierRepository.AsSugarClient().Queryable<ZjnBaseSupplierEntity>()
                    .Where(a => a.IsDelete == 0)
                    .WhereIF(!string.IsNullOrEmpty(input.F_SupplierNo), a => a.SupplierNo.Contains(input.F_SupplierNo))
                    .WhereIF(!string.IsNullOrEmpty(input.F_SupplierName), a => a.SupplierName.Contains(input.F_SupplierName))
                    .Select((a) => new ZjnBaseSupplierInfoOutput
                    {
                        id = a.Id,
                        supplierNo = a.SupplierNo,
                        supplierName = a.SupplierName,
                        createUser = a.CreateUser,
                        createTime = a.CreateTime,
                        modifiedUser = a.ModifiedUser,
                        modifiedTime = a.ModifiedTime,
                    }).OrderBy(sidx + " " + input.sort).ToListAsync();
            }

            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = DateTime.Now.ToDateString() + "供应商信息导出数据.xls";
            excelconfig.HeadFont = "微软雅黑";
            excelconfig.HeadPoint = 10;
            excelconfig.IsAllSizeColumn = true;
            excelconfig.ColumnModel = new List<ExcelColumnModel>();
            var filedList = GetUserInfoFieldToTitle(input.selectKey.Split(',').ToList());
            foreach (var item in filedList)
            {
                var column = item.Key;
                var excelColumn = item.Value;
                excelconfig.ColumnModel.Add(new ExcelColumnModel() { Column = column, ExcelColumn = excelColumn });
            }
            var addPath = FileVariable.TemporaryFilePath + excelconfig.FileName;
            ExcelExportHelper<ZjnBaseSupplierInfoOutput>.Export(userList, excelconfig, addPath);

            return new { name = excelconfig.FileName, url = "/api/file/Download?encryption=" + DESCEncryption.Encrypt(_userManager.UserId + "|" + excelconfig.FileName + "|" + addPath, "HSZ") };
        }


        /// <summary>
        /// 供应商信息 字段对应 列名称
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetUserInfoFieldToTitle(List<string> fields = null)
        {
            var res = new Dictionary<string, string>();
            res.Add("supplierNo", "供应商编号");
            res.Add("supplierName", "供应商名称");

            if (fields == null || !fields.Any()) return res;

            var result = new Dictionary<string, string>();

            res.Add("createUser", "创建者");
            res.Add("createTime", "创建时间");
            res.Add("modifiedUser", "修改者");
            res.Add("modifiedTime", "修改时间");

            foreach (var item in res)
            {
                if (fields.Contains(item.Key))
                    result.Add(item.Key, item.Value);
            }

            return result;
        }


       
        /// <summary>
		/// 获取供应商管理列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnBaseSupplierListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnBaseSupplierRepository.AsSugarClient().Queryable<ZjnBaseSupplierEntity>()
                .Where(x=> x.IsDelete==0)
                .WhereIF(!string.IsNullOrEmpty(input.F_SupplierNo), a => a.SupplierNo.Contains(input.F_SupplierNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_SupplierName), a => a.SupplierName.Contains(input.F_SupplierName))
                .Select((a) => new ZjnBaseSupplierListOutput{
                F_Id = a.Id,
                F_SupplierNo = a.SupplierNo,
    F_SupplierName = a.SupplierName,
    F_CreateUser = SqlFunc.Subqueryable<UserEntity>().Where(s => s.Id == a.CreateUser).Select(s => s.RealName) ,
    F_CreateTime = a.CreateTime,
    F_ModifiedUser = SqlFunc.Subqueryable<UserEntity>().Where(s => s.Id == a.ModifiedUser).Select(s => s.RealName),
    F_ModifiedTime = a.ModifiedTime,
}).OrderBy(sidx + " " + input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<ZjnBaseSupplierListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建供应商管理
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnBaseSupplierCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnBaseSupplierEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateUser = _userManager.UserId;
            entity.CreateTime = DateTime.Now;
            entity.IsDelete = 0;
            var isOk = await _zjnBaseSupplierRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 更新供应商管理
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnBaseSupplierUpInput input)
        {
            var entityl = await _zjnBaseSupplierRepository.GetFirstAsync(p => p.Id.Equals(input.id));
            var entity = input.Adapt<ZjnBaseSupplierEntity>();
            entity.ModifiedUser = _userManager.UserId;
            entity.ModifiedTime = DateTime.Now;
            entity.EnabledMark = entityl.EnabledMark;
            entity.CreateTime = entityl.CreateTime;
            entity.CreateUser = entityl.CreateUser;
            //插入日志
            ZjnWmsOperationLogEntity operationLogEntity = new ZjnWmsOperationLogEntity();
            operationLogEntity.CreateUser = _userManager.UserId;
            operationLogEntity.CreateTime = DateTime.Now;
            operationLogEntity.Describe = "平面库供应商管理信息修改";
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
                var isOk = await _zjnBaseSupplierRepository.AsUpdateable(entity).ExecuteCommandAsync();
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



            //var isOk = await _zjnBaseSupplierRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            //if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除供应商管理
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            ZjnWmsOperationLogEntity operationLogEntity = new ZjnWmsOperationLogEntity();
            var entity = await _zjnBaseSupplierRepository.GetFirstAsync(p => p.Id.Equals(id));

            operationLogEntity.BeforeDate = entity.ToJson();//修改前的数据
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            //取消数据状态
            entity.IsDelete = 1;
            //添加日志
            operationLogEntity.CreateUser = _userManager.UserId;
            operationLogEntity.CreateTime = DateTime.Now;
            operationLogEntity.Describe = "平面库供应商管理信息删除";
            operationLogEntity.AfterDate = entity.ToJson();//修改后的数据            
            operationLogEntity.Id = YitIdHelper.NextId().ToString();
            operationLogEntity.Type = 2;
            operationLogEntity.WorkPath = 2;
            try
            {
                //开启事务
                _db.BeginTran();
                //修改数据
                var isOk = await _zjnBaseSupplierRepository.AsUpdateable(entity).ExecuteCommandAsync();
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

            //var isOk = await _zjnBaseSupplierRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            //if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


