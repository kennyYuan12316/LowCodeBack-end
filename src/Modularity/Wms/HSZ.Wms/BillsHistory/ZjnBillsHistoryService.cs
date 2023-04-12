using HSZ.ClayObject;
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
using HSZ.System.Entitys.System;
using HSZ.wms.Entitys.Dto.ZjnBillsHistory;
using HSZ.wms.Entitys.Dto.ZjnPlaneGoods;
using HSZ.wms.Entitys.Dto.ZjnPlaneMaterialInventory;
using HSZ.wms.Interfaces.ZjnBillsHistory;
using HSZ.Wms.Entitys.Dto.BillsHistory;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnBillsHistory
{
    /// <summary>
    /// 入库明细-APP端服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnBillsHistory", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnBillsHistoryService : IZjnBillsHistoryService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnBillsHistoryEntity> _zjnBillsHistoryRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;
        private readonly ISqlSugarRepository<ZjnPlaneGoodsEntity> _zjnPlaneGoodsRepository;
        private readonly ISqlSugarRepository<ZjnWmsSupplierEntity> _zjnBaseSupplierRepository;
        private readonly ISqlSugarRepository<DictionaryDataEntity> _dictionaryDataRepository;// 数据字典表仓储
        private readonly ISqlSugarRepository<ZjnPlaneMaterialInventoryEntity> _zjnPlaneMaterialInventoryRepository;
        private readonly ISqlSugarRepository<ZjnPlaneTrayEntity> _zjnPlaneTrayrRepository;
        private readonly ISqlSugarRepository<ZjnWmsOperationLogEntity> _zjnPlaneOperationLogRepository;
        private readonly ISqlSugarRepository<ZjnPlaneLocationEntity> _zjnBaseLocationRepository;
        /// <summary>
        /// 初始化一个<see cref="ZjnBillsHistoryService"/>类型的新实例
        /// </summary>
        public ZjnBillsHistoryService(ISqlSugarRepository<ZjnBillsHistoryEntity> zjnBillsHistoryRepository, ISqlSugarRepository<ZjnPlaneGoodsEntity> zjnPlaneGoodsRepository, ISqlSugarRepository<ZjnWmsSupplierEntity> zjnWmsSupplierRepository,
         ISqlSugarRepository<ZjnPlaneTrayEntity> zjnPlaneTrayrRepository, ISqlSugarRepository<ZjnPlaneLocationEntity> zjnBaseLocationRepository, ISqlSugarRepository<ZjnWmsOperationLogEntity> zjnPlaneOperationLogRepository, ISqlSugarRepository<DictionaryDataEntity> dictionaryDataRepository, ISqlSugarRepository<ZjnPlaneMaterialInventoryEntity> zjnPlaneMaterialInventoryRepository,IUserManager userManager)
        {
            _zjnBaseLocationRepository = zjnBaseLocationRepository;
            _zjnPlaneOperationLogRepository = zjnPlaneOperationLogRepository;
            _zjnPlaneTrayrRepository = zjnPlaneTrayrRepository;
            _zjnPlaneMaterialInventoryRepository = zjnPlaneMaterialInventoryRepository;
            _zjnBillsHistoryRepository = zjnBillsHistoryRepository;
            _userManager = userManager;
            _zjnPlaneGoodsRepository = zjnPlaneGoodsRepository;
            _zjnBaseSupplierRepository = zjnWmsSupplierRepository;
            _dictionaryDataRepository = dictionaryDataRepository;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取入库明细-APP端
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {

            var output = (await _zjnBillsHistoryRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnBillsHistoryInfoOutput>();
            return output;
        }

        /// <summary>
        /// 查询是否出过库
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("GetChuKuInfo")]
        public async Task<dynamic> GetChuKuInfo(string id)
        {

            var output = (await _zjnBillsHistoryRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnBillsHistoryInfoOutput>();
           
            //查询是否出库
            var chukJluList = await _zjnBillsHistoryRepository.GetFirstAsync(x => x.OrderNo == output.orderNo && x.ProductsNo == output.productsNo && x.ProductsLocation == output.productsLocation && x.OrderType == "004");
            if (chukJluList!=null)
            {
                throw HSZException.Oh("出库不可以修改");
            }
            return chukJluList;
        }


        /// <summary>
		/// 获取入库明细-APP端列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnBillsHistoryListQueryInput input)
        {
          List<string> list = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "337742110771905797").Select(s => s.EnCode).ToList();
            var sidx = input.sidx == "a.Id";
            var data = await _zjnBillsHistoryRepository.AsSugarClient().Queryable<ZjnBillsHistoryEntity>()
                .LeftJoin<ZjnBaseCustomerEntity>((a, b) => a.ProductsUser == b.CustomerNo)
                .LeftJoin<ZjnWmsSupplierEntity>((a, b,c) => a.ProductsSupplier == c.SupplierNo)
                .Where(a =>  list.Contains(a.OrderType) && a.IsDelete == 0)
                .WhereIF(!string.IsNullOrEmpty(input.F_OrderNo), a => a.OrderNo.Contains(input.F_OrderNo))
                .WhereIF(!string.IsNullOrEmpty(input.OrderTypes), a => a.OrderType.Contains(input.OrderTypes))
                .WhereIF(!string.IsNullOrEmpty(input.F_PurchaseOrder), a => a.PurchaseOrder.Contains(input.F_PurchaseOrder))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsName.Contains(input.F_ProductsName))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsNo), a => a.ProductsNo.Contains(input.F_ProductsNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsUser), a => a.ProductsUser.Contains(input.F_ProductsUser))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsSupplier), a => a.ProductsSupplier.Contains(input.F_ProductsSupplier))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsLocation), a => a.ProductsLocation.Contains(input.F_ProductsLocation))
                .Select((a,b,c)=> new ZjnBillsHistoryListOutput
                 {
                    F_Id = a.Id,
                    F_OrderNo = a.OrderNo,
                    F_OrderType = a.OrderType,
                    F_ProductsName = a.ProductsName,
                    F_ProductsType = a.ProductsType,
                    F_ProductsStyle = a.ProductsStyle,
                    F_ProductsTotal = a.ProductsTotal,
                    F_ProductsUnit = a.ProductsUnit,
                    F_ProductsGrade = a.ProductsGrade,
                    F_ProductsBach = a.ProductsBach,
                    F_ProductsUser = b.CustomerName,
                    F_ProductsSupplier = c.SupplierName,
                    F_ProductsLocation = a.ProductsLocation,
                    F_LastModifyUserId = a.LastModifyUserId,
                    F_LastModifyTime = a.LastModifyTime,
                    F_TheContainer = a.TheContainer,
                    F_TheTray = a.TheTray,
                    F_CreateUser = a.CreateUser,
                    F_CreateTime = a.CreateTime,
                    F_InspectionStatus=a.InspectionStatus,
                    F_ExpiryDate=a.ExpiryDate,
                    F_PurchaseOrder=a.PurchaseOrder,
                    F_TestType=a.TestType,
                    F_IsDelete=a.IsDelete
                }).OrderBy(" a.F_Id  desc ").ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnBillsHistoryListOutput>.SqlSugarPageResult(data);
        }


        /// <summary>
        /// 获取出库列表
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("GetListChuk")]
        public async Task<dynamic> GetListChuk([FromQuery] ZjnBillsHistoryListQueryInput input)
        {
            List<string> list = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "337742224898917637").Select(s => s.EnCode).ToList();
            var sidx = input.sidx == "a.Id";
            var data = await _zjnBillsHistoryRepository.AsSugarClient().Queryable<ZjnBillsHistoryEntity>()
                .LeftJoin<ZjnBaseCustomerEntity>((a, b) => a.ProductsUser == b.CustomerNo)
                .LeftJoin<ZjnWmsSupplierEntity>((a, b, c) => a.ProductsSupplier == c.SupplierNo)
                .Where(a => list.Contains(a.OrderType) && a.IsDelete == 0)
                .WhereIF(!string.IsNullOrEmpty(input.F_OrderNo), a => a.OrderNo.Contains(input.F_OrderNo))
                .WhereIF(!string.IsNullOrEmpty(input.OrderTypes), a => a.OrderType.Contains(input.OrderTypes))
                .WhereIF(!string.IsNullOrEmpty(input.F_PurchaseOrder), a => a.PurchaseOrder.Contains(input.F_PurchaseOrder))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsName.Contains(input.F_ProductsName))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsNo), a => a.ProductsNo.Contains(input.F_ProductsNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsUser), a => a.ProductsUser.Contains(input.F_ProductsUser))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsSupplier), a => a.ProductsSupplier.Contains(input.F_ProductsSupplier))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsLocation), a => a.ProductsLocation.Contains(input.F_ProductsLocation))
                .Select((a, b, c) => new ZjnBillsHistoryListOutput
                {
                    F_Id = a.Id,
                    F_OrderNo = a.OrderNo,
                    F_OrderType = a.OrderType,
                    F_ProductsName = a.ProductsName,
                    F_ProductsType = a.ProductsType,
                    F_ProductsStyle = a.ProductsStyle,
                    F_ProductsTotal = a.ProductsTotal,
                    F_ProductsUnit = a.ProductsUnit,
                    F_ProductsGrade = a.ProductsGrade,
                    F_ProductsBach = a.ProductsBach,
                    F_ProductsUser = b.CustomerName,
                    F_ProductsSupplier = c.SupplierName,
                    F_ProductsLocation = a.ProductsLocation,
                    F_LastModifyUserId = a.LastModifyUserId,
                    F_LastModifyTime = a.LastModifyTime,
                    F_TheContainer = a.TheContainer,
                    F_TheTray = a.TheTray,
                    F_CreateUser = a.CreateUser,
                    F_CreateTime = a.CreateTime,
                    F_InspectionStatus = a.InspectionStatus,
                    F_ExpiryDate = a.ExpiryDate,
                    F_PurchaseOrder = a.PurchaseOrder,
                    F_TestType = a.TestType,
                    F_IsDelete = a.IsDelete
                }).OrderBy(" a.F_Id  desc ").ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<ZjnBillsHistoryListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建入库明细-APP端
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnBillsHistoryCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            
            //出入库记录表
            List<ZjnBillsHistoryEntity> listBillsHistory = new List<ZjnBillsHistoryEntity>();
            //新增库存记录
            List<ZjnPlaneMaterialInventoryEntity> zjnPlaneMaterialInventory = new List<ZjnPlaneMaterialInventoryEntity>();
            int updateNum = 0;
            #region 添加入库记录

            var buwzhi = input.orderNo.Substring(0, 2);
            string OrderNo = null;
            string productsSupplier = null;
            string productsSupplierDate = null;
            if (buwzhi == "00")
            {
                OrderNo = input.orderNo.Substring(4, 10);
                productsSupplier = input.orderNo.Substring(14, 7);
                productsSupplierDate = "20" + input.orderNo.Substring(21, 6);
            }
            else
            {
                OrderNo = input.orderNo.Substring(0, 10);
                productsSupplier = input.orderNo.Substring(10, 7);
                productsSupplierDate = "20" + input.orderNo.Substring(17, 6);
            }
            //查询物料
            var listplaneGoods = await _zjnPlaneGoodsRepository.GetFirstAsync(x => x.GoodsCode == OrderNo);
            //查询供应商
            var productsSupplierList = await _zjnBaseSupplierRepository.GetFirstAsync(x => x.SupplierNo == productsSupplier);
            if (listplaneGoods == null || productsSupplierList == null)
            {
                throw HSZException.Oh("32位流水码错误");
            }
            if (input.TestType == 0)
            {

                input.InspectionStatus = 0;
            }
            else
            {
                input.InspectionStatus = 2;
            }
            
            //验证/托盘/货位/容器
            if (input.TheTray != null || input.TheContainer != null)
            {
                var entityType = await _zjnPlaneTrayrRepository.GetFirstAsync(p => p.TrayNo.Equals(input.TheTray));
                var entityTheContainer = await _zjnPlaneTrayrRepository.GetFirstAsync(p => p.TrayNo.Equals(input.TheContainer));
                if (entityType != null)
                {
                    if (entityType.TrayState != 1)
                    {
                        throw HSZException.Oh("托盘在使用或者禁止状态，请重新选择");
                    }
                }
                if (entityTheContainer != null)
                {
                    if (entityTheContainer.TrayState != 1)
                    {
                        throw HSZException.Oh("容器在使用或者禁止状态，请重新选择");
                    }
                }
            }
            var BaseLocation=await  _zjnBaseLocationRepository.GetFirstAsync(x=> x.LocationNo.Equals(input.productsLocation));
            if (BaseLocation == null)
            {
                throw HSZException.Oh("托盘/容器在使用或者禁止状态，请重新选择");
            }
            else {
                if (BaseLocation.LocationStatus!="0"||BaseLocation.EnabledMark!=1)
                {
                    throw HSZException.Oh("货位在使用或者禁止状态，请重新选择");
                }
            }

            ZjnBillsHistoryEntity zjnBillsHistory = new ZjnBillsHistoryEntity()
            {

                Id = YitIdHelper.NextId().ToString(),
                OrderType = input.orderType,
                ProductsName = listplaneGoods.GoodsName,
                ProductsLocation = input.productsLocation,
                ProductsBach = input.orderNo,
                OrderNo = input.orderNo,
                PurchaseOrder = input.PurchaseOrder,
                ProductsGrade = input.productsGrade,
                ProductsNo = listplaneGoods.GoodsCode,
                ProductsStyle = listplaneGoods.Specifications,
                ProductsSupplier = productsSupplierList.SupplierNo,
                ProductsTotal = input.productsTotal,
                ProductsType = listplaneGoods.GoodsType.ToString(),
                ProductsUnit = listplaneGoods.Unit.ToString(),
                ProductsUser = input.productsUser,
                TheDateOfProduction = DateTime.ParseExact(productsSupplierDate, "yyyyMMdd", CultureInfo.CurrentCulture),
                TestType = listplaneGoods.CheckType,
                TheContainer = input.TheContainer,
                TheTray = input.TheTray,
                CreateTime = DateTime.Now,
                CreateUser = userInfo.userId,
                InspectionStatus = input.InspectionStatus,
                ExpiryDate = input.ExpiryDate,
                LastModifyUserId = userInfo.userId,
                LastModifyTime = DateTime.Now,
                BatchDeliveryQuantity = "0",
                IsDelete =0
            };
            listBillsHistory.Add(zjnBillsHistory);
            #endregion

            #region 添加库存或者修改库存数量

            var inventoryEntity = (await _zjnPlaneMaterialInventoryRepository.GetFirstAsync(x => x.ProductsCode == OrderNo && x.ProductsBatch == input.orderNo && x.ProductsLocation == input.productsLocation)).Adapt<ZjnPlaneMaterialInventoryEntity>();
            if (inventoryEntity == null)
            {
                //插入库存
                ZjnPlaneMaterialInventoryEntity zjnPlane = new ZjnPlaneMaterialInventoryEntity()
                {
                    Id = YitIdHelper.NextId().ToString(),
                    ProductsBatch = input.orderNo,
                    ProductsCode = OrderNo,
                    CreateUser = userInfo.userId,
                    ProductsCustomer = input.productsUser,
                    ProductsSupplier = productsSupplierList.SupplierNo,
                    ProductsName = listplaneGoods.GoodsName,
                    ProductsQuantity = input.productsTotal,
                    ProductsUnit = input.productsUnit,
                    ProductsType = listplaneGoods.GoodsType.ToString(),
                    ProductsGrade = input.productsBach,
                    ProductsLocation = input.productsLocation,
                    ProductsStyle = input.productsStyle,
                    ProductsState = listplaneGoods.GoodsState.ToString(),
                    ExpiryDate= DateTime.ParseExact(productsSupplierDate, "yyyyMMdd", CultureInfo.CurrentCulture),
                    InspectionStatus = input.InspectionStatus.ToString(),
                    CreateTime = DateTime.Now,
                    LastModifyUserId = userInfo.userId,
                    LastModifyTime = DateTime.Now
                };
                zjnPlaneMaterialInventory.Add(zjnPlane);

            }
            else
            {
                updateNum = 1;
                ZjnPlaneMaterialInventoryEntity inventoryEntity1 = new ZjnPlaneMaterialInventoryEntity();
                inventoryEntity.ProductsQuantity = inventoryEntity.ProductsQuantity + input.productsTotal;
                inventoryEntity1 = inventoryEntity;
                zjnPlaneMaterialInventory.Add(inventoryEntity1);
            }



            #endregion

            if (listBillsHistory.Any())
            {
                try
                {
                    //开启事务
                    _db.BeginTran();

                    //新增记录
                    var newEntity = await _zjnBillsHistoryRepository.AsInsertable(listBillsHistory).ExecuteReturnEntityAsync();
                    if (updateNum == 0)
                    {
                        //库存
                        await _zjnPlaneMaterialInventoryRepository.AsInsertable(zjnPlaneMaterialInventory).ExecuteReturnEntityAsync();
                    }
                    else {
                        //库存
                        await _zjnPlaneMaterialInventoryRepository.AsUpdateable(zjnPlaneMaterialInventory).ExecuteCommandAsync(); //(zjnPlaneMaterialInventory).ExecuteReturnEntityAsync();
                    }
                   
                   
                    _db.CommitTran();
                }
                catch (Exception e)
                {
                    string es = e.Message;
                    _db.RollbackTran();
                    throw HSZException.Oh(ErrorCode.COM1000);

                }
            }

        }

       

        /// <summary>
        /// 更新入库明细-APP端
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnBillsHistoryUpInput input)
        {
            if (input.TheTray!=null||input.TheContainer!=null)
            {
                var entityType = await _zjnPlaneTrayrRepository.GetFirstAsync(p => p.TrayNo.Equals(input.TheTray));
                var entityTheContainer = await _zjnPlaneTrayrRepository.GetFirstAsync(p => p.TrayNo.Equals(input.TheContainer));
                if (entityType != null) {
                    if (entityType.TrayState != 1)
                    {
                        throw HSZException.Oh("托盘在使用或者禁止状态，请重新选择");
                    }
                }
                if (entityTheContainer != null)
                {
                    if (entityTheContainer.TrayState != 1)
                    {
                        throw HSZException.Oh("容器在使用或者禁止状态，请重新选择");
                    }
                }

            }
            var BaseLocation = await _zjnBaseLocationRepository.GetFirstAsync(x => x.LocationNo.Equals(input.productsLocation));
            if (BaseLocation == null)
            {
                throw HSZException.Oh("货位在使用或者禁止状态，请重新选择");
            }
            else
            {
                if (BaseLocation.LocationStatus != "0" || BaseLocation.EnabledMark != 1)
                {
                    throw HSZException.Oh("货位在使用或者禁止状态，请重新选择");
                }
            }
            //当前用户
            var userInfo = await _userManager.GetUserInfo();
            //查询当前入库信息
            var entityss = await _zjnBillsHistoryRepository.GetFirstAsync(p => p.Id.Equals(id));
            //修改的入库信息
            var entity = input.Adapt<ZjnBillsHistoryEntity>();
            //查询库存信息   
            var inventoryEntityList =await _zjnPlaneMaterialInventoryRepository.GetFirstAsync(x => x.ProductsCode == input.productsNo && x.ProductsLocation == input.productsLocation &&  x.ProductsBatch == input.orderNo);//根据物料号和货位查询出所有的库存信息
            bool Tatal = true;
            decimal? sumCode = 0;
            if (entityss.ProductsTotal == entity.ProductsTotal)
            {
                Tatal = false;
            }
            else {
                if (entityss.ProductsTotal > entity.ProductsTotal)
                {
                    sumCode = entityss.ProductsTotal - entity.ProductsTotal;
                    inventoryEntityList.ProductsQuantity = inventoryEntityList.ProductsQuantity - sumCode;
                }
                else {
                    sumCode = entity.ProductsTotal - entityss.ProductsTotal;
                    inventoryEntityList.ProductsQuantity = inventoryEntityList.ProductsQuantity +sumCode;
                }
            }
            entity.LastModifyUserId = userInfo.userId;
            entity.LastModifyTime = DateTime.Now;
            entity.CreateTime = entityss.CreateTime;
            entity.CreateUser = entityss.CreateUser;
            entity.IsDelete = 0;
            // 插入日志
            ZjnWmsOperationLogEntity operationLogEntity = new ZjnWmsOperationLogEntity();
            operationLogEntity.CreateUser = userInfo.userId;
            operationLogEntity.CreateTime = DateTime.Now;
            operationLogEntity.Describe = "原材料信息修改";            
            operationLogEntity.BeforeDate = entity.ToJson();//修改前的数据
            operationLogEntity.Id = YitIdHelper.NextId().ToString();
            operationLogEntity.Type = 1;
            operationLogEntity.WorkPath = 2;
            operationLogEntity.TheBusinessArea = "原材料库";
            try
            {
                //开启事务
                _db.BeginTran();
                //修改数据
                var isOk = await _zjnBillsHistoryRepository.AsUpdateable(entity).ExecuteCommandAsync();
                //新增日子记录
                var isOk1 = await _zjnPlaneOperationLogRepository.AsInsertable(operationLogEntity).ExecuteReturnEntityAsync();
                if (Tatal==true)
                {
                    //修改库存
                    await _zjnPlaneMaterialInventoryRepository.AsUpdateable(inventoryEntityList).ExecuteCommandAsync();
                }
                

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
        /// 更新入库状态
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("Updatesta")]
        public async Task Updatesta(string id, [FromBody] ZjnBillsHistoryUpInput input)
        {
           
            //当前用户
            var userInfo = await _userManager.GetUserInfo();
            List<ZjnBillsHistoryEntity> entityList = new List<ZjnBillsHistoryEntity>();
            List<string> list = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "337742110771905797").Select(s => s.EnCode).ToList();
            //查询当前入库信息
            var inventoryEntityList = _zjnBillsHistoryRepository.AsSugarClient().Queryable<ZjnBillsHistoryEntity>().Where(p => p.OrderNo.Equals(input.orderNo) && p.ProductsLocation.Equals(input.productsLocation)&& p.PurchaseOrder.Equals(input.PurchaseOrder)&& list.Contains(p.OrderType)).ToList();
           
            var EntityList = await _zjnPlaneMaterialInventoryRepository.GetFirstAsync(p => p.ProductsBatch.Equals(input.orderNo)&& p.ProductsLocation.Equals(input.productsLocation));
            foreach (var item in inventoryEntityList)
            {
                ZjnBillsHistoryEntity entity = new ZjnBillsHistoryEntity();
                entity = item;
                entity.LastModifyUserId = userInfo.userId;
                entity.LastModifyTime = DateTime.Now;
                entity.InspectionStatus = input.InspectionStatus;
                entity.IsDelete = 0;
                entityList.Add(entity);
            }
            EntityList.Case1 = input.productsStyle;
            //EntityList.InspectionStatus = input.InspectionStatus.ToString();

            // 插入日志
            ZjnWmsOperationLogEntity operationLogEntity = new ZjnWmsOperationLogEntity();
            operationLogEntity.CreateUser = userInfo.userId;
            operationLogEntity.CreateTime = DateTime.Now;
            operationLogEntity.Describe = "原材料状态修改";
            operationLogEntity.BeforeDate = entityList.ToJson();//修改前的数据
            operationLogEntity.Id = YitIdHelper.NextId().ToString();
            operationLogEntity.Type = 1;
            operationLogEntity.WorkPath = 2;
            operationLogEntity.TheBusinessArea = "原材料库";
            try
            {
                //开启事务
                _db.BeginTran();
                //修改数据
                 var isOk = await _zjnBillsHistoryRepository.AsUpdateable(entityList).ExecuteCommandAsync();
                 await _zjnPlaneMaterialInventoryRepository.AsUpdateable(EntityList).ExecuteCommandAsync();
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
        /// 删除入库明细-APP端
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = await _zjnBillsHistoryRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            entity.IsDelete = 1;
            entity.LastModifyUserId = userInfo.userId;
            entity.LastModifyTime = DateTime.Now;
            //查询是否出库
            var chukJluList = await _zjnBillsHistoryRepository.GetFirstAsync(x => x.OrderNo == entity.OrderNo && x.ProductsNo == entity.ProductsNo && x.ProductsLocation == entity.ProductsLocation && x.OrderType == "004");
            if (chukJluList != null)
            {
                throw HSZException.Oh("出库不可以删除");
            }
            // 插入日志
            ZjnWmsOperationLogEntity operationLogEntity = new ZjnWmsOperationLogEntity();
            operationLogEntity.CreateUser = userInfo.userId;
            operationLogEntity.CreateTime = DateTime.Now;
            operationLogEntity.Describe = "原材料信息删除";
            // operationLogEntity.AfterDate = entity.ToJson();//修改后的数据
            operationLogEntity.BeforeDate = entity.ToJson();//修改前的数据
            operationLogEntity.Id = YitIdHelper.NextId().ToString();
            operationLogEntity.Type = 2;
            operationLogEntity.WorkPath = 2;
            operationLogEntity.TheBusinessArea = "原材料库";
            try
            {
                //开启事务
                _db.BeginTran();
                //修改数据
                var isOk = await _zjnBillsHistoryRepository.AsUpdateable(entity).ExecuteCommandAsync();
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
        /// 出库列表
        /// </summary>
        /// <param name="OrderType"></param>
        /// <returns></returns>
        [HttpGet("PlaneOutboundList")]
        public async Task<dynamic> PlaneOutboundList([FromQuery] ZjnBillsHistoryListQueryInput input) {

            var sidx = input.sidx == "a.F_Id";
            var data = await _zjnBillsHistoryRepository.AsSugarClient().Queryable<ZjnBillsHistoryEntity>().Where(q=> q.OrderType=="004")
                .LeftJoin<ZjnBaseCustomerEntity>((a, b) => a.ProductsUser == b.CustomerNo)
                .LeftJoin<ZjnWmsSupplierEntity>((a, b, c) => a.ProductsSupplier == c.SupplierNo)
                .WhereIF(!string.IsNullOrEmpty(input.F_OrderType), a => a.OrderType.Contains(input.F_OrderType))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsName.Contains(input.F_ProductsName))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsUser), a => a.ProductsUser.Contains(input.F_ProductsUser))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsSupplier), a => a.ProductsSupplier.Contains(input.F_ProductsSupplier))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsLocation), a => a.ProductsLocation.Contains(input.F_ProductsLocation))
                .Select((a, b, c) => new ZjnBillsHistoryListOutput
                {
                    F_Id = a.Id,
                    F_OrderNo = a.OrderNo,
                    F_OrderType = a.OrderType,
                    F_ProductsName = a.ProductsName,
                    F_ProductsType = a.ProductsType,
                    F_ProductsStyle = a.ProductsStyle,
                    F_ProductsTotal = a.ProductsTotal,
                    F_ProductsUnit = a.ProductsUnit,
                    F_ProductsGrade = a.ProductsGrade,
                    F_ProductsBach = a.ProductsBach,
                    F_ProductsUser = b.CustomerName,
                    F_ProductsSupplier = c.SupplierName,
                    F_ProductsLocation = a.ProductsLocation,
                    F_LastModifyUserId=a.LastModifyUserId,
                    F_LastModifyTime = a.LastModifyTime,
                    F_TheContainer = a.TheContainer,
                    F_TheTray = a.TheTray,
                    F_CreateUser = a.CreateUser,
                    F_CreateTime = a.CreateTime,
                }).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<ZjnBillsHistoryListOutput>.SqlSugarPageResult(data);
        }


        /// <summary>
        /// 新建出库明细
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("NewOutboundRecord")]
        public async Task NewOutboundRecord([FromBody] ZjnBillsHistoryEntity input)
        {
            if (input.ProductsTotal==null)
            {
                throw HSZException.Oh("出库数量不能为空");
            }

            if (input.OrderNo == null)
            {
                throw HSZException.Oh("流水码不能为空");
            }
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnBillsHistoryEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateUser = userInfo.userId;
            entity.CreateTime = DateTime.Now;
            
            //新增记录出库
            List<ZjnBillsHistoryEntity> listBillsHistory = new List<ZjnBillsHistoryEntity>();
            //修改记录入库库
            List<ZjnBillsHistoryEntity> listBillsHistoryRku = new List<ZjnBillsHistoryEntity>();
            //修改库存修改数量
            List<ZjnPlaneMaterialInventoryEntity> zjnPlaneMaterial = new List<ZjnPlaneMaterialInventoryEntity>();

            #region 添加入库记录


            var buwzhi = input.OrderNo.Substring(0, 2);
            string OrderNo = null;
            string productsSupplier = null;
            string productsDate = null;
            if (buwzhi == "00")
            {
                OrderNo = input.OrderNo.Substring(4, 10);
                productsSupplier = input.OrderNo.Substring(14, 7);
                productsDate = "20" + input.OrderNo.Substring(21, 6);
            }
            else
            {
                OrderNo = input.OrderNo.Substring(0, 10);
                productsSupplier = input.OrderNo.Substring(10, 7);
                productsDate = "20" + input.OrderNo.Substring(17, 6);
            }
            //string OrderNo = input.OrderNo.Substring(0, 10);
            //string productsSupplier = input.OrderNo.Substring(10, 7);
            //string productsDate = "20"+input.OrderNo.Substring(17, 6);
            //查询物料
            var listplaneGoods = await _zjnPlaneGoodsRepository.GetFirstAsync(x => x.GoodsCode == OrderNo);
            string Type = null;
            //if (input.OrderType == "010")
            //{
            //    Type = "009";
            //}
            //else {
            //    Type = "001";
            //}
            //查询入库记录信息
            //var BillsHistory = (await _zjnBillsHistoryRepository.GetListAsync(x => x.OrderNo == input.OrderNo && x.ProductsLocation == input.ProductsLocation)).Adapt<ZjnBillsHistoryEntity>();
            var BillsHistory = (await _zjnBillsHistoryRepository.GetFirstAsync(p => p.OrderNo == input.OrderNo &&  p.ProductsLocation == input.ProductsLocation && p.IsDelete == 0)).Adapt<ZjnBillsHistoryInfoOutput>();
            
            //查询供应商
            var productsSupplierList = await _zjnBaseSupplierRepository.GetFirstAsync(x => x.SupplierNo == productsSupplier);
            if (listplaneGoods.Id==null || productsSupplierList.Id == null)
            {
                throw HSZException.Oh(ErrorCode.COM1000);
            }

            if (input.OrderType != "C004" && input.OrderType != "C003")
            {
                if (BillsHistory.InspectionStatus != "0")
                {
                    throw HSZException.Oh("流水号:" + input.OrderNo + " ,物料不是检验合格状态，不能出库");
                }
            }
            
            //查询库存信息   //状态 //先入先出
            var inventoryEntityList = _zjnPlaneMaterialInventoryRepository.AsSugarClient().Queryable<ZjnPlaneMaterialInventoryEntity>().Where(x => x.ProductsCode == OrderNo && x.ProductsLocation == input.ProductsLocation && x.ProductsQuantity > 0&& x.ProductsBatch==input.OrderNo&& x.ProductsState=="1").OrderBy("F_CreateTime").ToList();//根据物料号和货位查询出所有的库存信息
            var sQuantity = inventoryEntityList.Sum(x => x.ProductsQuantity);
            //入库记录和库存表不可以为空
            if (BillsHistory.id== null || inventoryEntityList.Count() == 0)
            {
                throw HSZException.Oh(ErrorCode.COM1000);
            }
            //出库数量是否大于当前库存
            if (sQuantity < input.ProductsTotal)
            {
                throw HSZException.Oh(ErrorCode.COM1000);

            }
            //先进先出
            decimal? Quantity = -1;

            foreach (var inventory in inventoryEntityList)
            {
               

                ZjnPlaneMaterialInventoryEntity inventoryEntity1 = new ZjnPlaneMaterialInventoryEntity();
                if (inventory.ProductsQuantity > input.ProductsTotal)
                {
                    #region 修改库存数量  
                    inventory.ProductsQuantity = inventory.ProductsQuantity - input.ProductsTotal;
                    inventoryEntity1 = inventory;
                    zjnPlaneMaterial.Add(inventoryEntity1);
                    Quantity = 0;
                    var listHistory = _zjnBillsHistoryRepository.AsSugarClient().Queryable<ZjnBillsHistoryEntity>().Where(p => p.Id== BillsHistory.id).ToList();
                    ZjnBillsHistoryEntity zjnBills = new ZjnBillsHistoryEntity();
                    zjnBills = listHistory[0];
                    zjnBills.BatchDeliveryQuantity = input.ProductsTotal.ToString();
                    listBillsHistoryRku.Add(zjnBills);
                    //listBillsHistoryRku.AddRange(listHR(inventory, input.ProductsTotal));
                    #endregion
                }
                else
                {
                    #region 修改库存数量     
                    if (Quantity == -1)
                    {

                        var sum = input.ProductsTotal - inventory.ProductsQuantity;
                        Quantity = sum;
                        inventory.ProductsQuantity = 0;
                        var listHistory = _zjnBillsHistoryRepository.AsSugarClient().Queryable<ZjnBillsHistoryEntity>().Where(p => p.Id == BillsHistory.id).ToList();
                        ZjnBillsHistoryEntity zjnBills = new ZjnBillsHistoryEntity();
                        zjnBills = listHistory[0];
                        zjnBills.BatchDeliveryQuantity = inventory.ProductsQuantity.ToString();
                        listBillsHistoryRku.Add(zjnBills);

                        // listBillsHistoryRku.AddRange(listHR(inventory, Quantity));
                    }
                    else
                    {
                        var sum = Quantity - inventory.ProductsQuantity;
                        if (sum < 0)
                        {
                            inventory.ProductsQuantity = inventory.ProductsQuantity - sum;
                            Quantity = 0;

                            var listHistory = _zjnBillsHistoryRepository.AsSugarClient().Queryable<ZjnBillsHistoryEntity>().Where(p => p.Id == BillsHistory.id).ToList();
                            ZjnBillsHistoryEntity zjnBills = new ZjnBillsHistoryEntity();
                            zjnBills = listHistory[0];
                            zjnBills.BatchDeliveryQuantity = sum.ToString();
                            listBillsHistoryRku.Add(zjnBills);
                            // listBillsHistoryRku.AddRange(listHR(inventory, Quantity));
                        }
                        else
                        {
                            inventory.ProductsQuantity = 0;
                            Quantity = sum;
                            var listHistory = _zjnBillsHistoryRepository.AsSugarClient().Queryable<ZjnBillsHistoryEntity>().Where(p => p.Id == BillsHistory.id).ToList();
                            ZjnBillsHistoryEntity zjnBills = new ZjnBillsHistoryEntity();
                            zjnBills = listHistory[0];
                            zjnBills.BatchDeliveryQuantity = inventory.ProductsQuantity.ToString();
                            listBillsHistoryRku.Add(zjnBills);
                            //listBillsHistoryRku.AddRange(listHR(inventory, Quantity));
                        }

                    }
                    inventoryEntity1 = inventory;
                    zjnPlaneMaterial.Add(inventoryEntity1);
                    #endregion
                }
                if (Quantity == 0) break;
            }

            //插入出库明细
            ZjnBillsHistoryEntity zjnBillsHistory = new ZjnBillsHistoryEntity()
            {

                Id = YitIdHelper.NextId().ToString(),
              
                OrderType = input.OrderType,
                ProductsName = listplaneGoods.GoodsName,
                ProductsLocation = BillsHistory.ProductsLocation,
                ProductsBach = BillsHistory.productsBach,
                OrderNo = BillsHistory.orderNo,
                PurchaseOrder = BillsHistory.PurchaseOrder,
                ProductsGrade = BillsHistory.productsGrade,
                ProductsNo = listplaneGoods.GoodsCode,
                ProductsStyle = listplaneGoods.Specifications,
                ProductsSupplier = productsSupplierList.SupplierNo,
                ProductsTotal = input.ProductsTotal,
                ProductsType = listplaneGoods.GoodsType.ToString(),
                ProductsUnit = listplaneGoods.Unit.ToString(),
                ProductsUser = BillsHistory.productsUser,
                TheDateOfProduction = DateTime.ParseExact(productsDate, "yyyyMMdd", CultureInfo.CurrentCulture),
                TestType = listplaneGoods.CheckType,
                TheContainer = BillsHistory.TheContainer,
                TheTray = BillsHistory.TheTray,
                CreateTime = DateTime.Now,
                CreateUser = userInfo.userId,
                InspectionStatus =Convert.ToInt32(BillsHistory.InspectionStatus),
                ExpiryDate = BillsHistory.ExpiryDate,
                LastModifyUserId = userInfo.userId,
                LastModifyTime = DateTime.Now,
                IsDelete=0
            };
            listBillsHistory.Add(zjnBillsHistory);
            #endregion



            if (listBillsHistory.Any())
            {
                try
                {
                    //开启事务
                    _db.BeginTran();

                    //新增记录
                    var newEntity = await _zjnBillsHistoryRepository.AsInsertable(listBillsHistory).ExecuteReturnEntityAsync();
                    //修改库存
                    await _zjnPlaneMaterialInventoryRepository.AsUpdateable(zjnPlaneMaterial).ExecuteCommandAsync();
                    //修改出库记录
                    await _zjnBillsHistoryRepository.AsUpdateable(listBillsHistoryRku).ExecuteCommandAsync();
                    _db.CommitTran();
                }
                catch (Exception e)
                {
                    string es = e.Message;
                    _db.RollbackTran();
                    
                }
            }




        }

        /// <summary>
        /// 先进先出
        /// </summary>
        /// <param name="inventory"></param>
        /// <returns></returns>
        public List<ZjnBillsHistoryEntity> listHR(ZjnPlaneMaterialInventoryEntity inventory,decimal? sumProductsTotal) {

            List<ZjnBillsHistoryEntity> zjnBillsHistorieslist = new List<ZjnBillsHistoryEntity>();
            var listHistory =  _zjnBillsHistoryRepository.AsSugarClient().Queryable<ZjnBillsHistoryEntity>().Where(p => p.OrderNo == inventory.ProductsBatch && p.ProductsLocation == inventory.ProductsLocation && p.IsDelete == 0 && Convert.ToDecimal(p.BatchDeliveryQuantity) < p.ProductsTotal).ToList();
            decimal? Quantity2 = -1;
            decimal? sunsd = -1;
            foreach (var itemhistor in listHistory)
            {
                ZjnBillsHistoryEntity historyEntity = new ZjnBillsHistoryEntity();
                var bunSum = itemhistor.ProductsTotal - Convert.ToDecimal(itemhistor.BatchDeliveryQuantity);
                if (sunsd==-1)
                {
                    sunsd = sumProductsTotal;
                }
                
                if (bunSum > sunsd)
                {
                    if (Quantity2 == -1)
                    {
                        historyEntity = itemhistor;
                        historyEntity.BatchDeliveryQuantity = (Convert.ToDecimal(itemhistor.BatchDeliveryQuantity) + sumProductsTotal).ToString();
                        zjnBillsHistorieslist.Add(historyEntity);
                        Quantity2 = 0;
                    }
                    else
                    {
                        historyEntity = itemhistor;
                        historyEntity.BatchDeliveryQuantity = (Convert.ToDecimal(itemhistor.BatchDeliveryQuantity) + Quantity2).ToString();
                        zjnBillsHistorieslist.Add(historyEntity);
                        Quantity2 = 0;
                    }

                }
                else
                {
                    historyEntity = itemhistor;
                    historyEntity.BatchDeliveryQuantity = (Convert.ToDecimal(itemhistor.BatchDeliveryQuantity) + bunSum).ToString();
                    zjnBillsHistorieslist.Add(historyEntity);
                    Quantity2 = sumProductsTotal - bunSum;
                    sunsd = Quantity2;
                }
                if (Quantity2 == 0) break;
            }

            return zjnBillsHistorieslist;
        }
        /// <summary>
        /// 根据物料号查询库存信息
        /// </summary>
        /// <param name="ProductsCode"></param>
        /// <returns></returns>
        [HttpGet("EntiyList")]
        public List<ZjnPlaneMaterialInventoryEntity> EntiyList(string ProductsCode) {
            //查询库存信息
            var inventoryEntityList = _zjnPlaneMaterialInventoryRepository.AsSugarClient().Queryable<ZjnPlaneMaterialInventoryEntity>().Where(x => x.ProductsCode == ProductsCode && x.ProductsQuantity > 0 && x.ProductsState == "1").OrderBy("F_CreateTime").ToList();//根据物料号查询出所有的库存信息
            return inventoryEntityList;


        }

        /// <summary>
        /// 入库模板下载
        /// </summary>
        /// <returns></returns>
        [HttpGet("TheRawMaterial")]
        public dynamic TheRawMaterial(string fileName)
        {
            
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = fileName+".xls";
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
            var UnitName = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "337742224898917637").Select(s => new ZjnPlaneGoodsListOutput { UnitName = s.FullName + ":" + s.EnCode }).ToList();


            var UnitName2 = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "337742110771905797").Select(s => new ZjnPlaneGoodsListOutput { UnitName = s.FullName + ":" + s.EnCode }).ToList();


            var Unittype = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "325448475967751429").Select(s => new ZjnPlaneGoodsListOutput { UnitName = s.FullName + ":" + s.EnCode }).ToList();
            var dataList = new List<ZjnBillsHistoryEntity>() { new ZjnBillsHistoryEntity() {
                PurchaseOrder= "单据号",
                OrderNo="xxxxx-0",
                OrderType="字段说明:"+GettName(UnitName2),
                ProductsTotal=500,
                ProductsLocation="xxxxxx",              
                CreateUser="字段说明:待检验：1 ,已验合格：2",
                ProductsGrade="字段说明:"+GettName(UnitName),
                ProductsUser="客户编号",
                TheTray ="xxxxx",
                TheContainer="xxxx",
            } };//初始化 一条空数据
            var addPath = FileVariable.TemporaryFilePath + excelconfig.FileName;
            ExcelExportHelper<ZjnBillsHistoryEntity>.Export(dataList, excelconfig, addPath);
            return new { name = excelconfig.FileName, url = "/api/file/Download?encryption=" + DESCEncryption.Encrypt(_userManager.UserId + "|" + excelconfig.FileName + "|" + addPath, "HSZ") };

        }

        /// <summary>
        /// 出库模板下载
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet("TheRawMateri")]
        public dynamic TheRawMateri(string fileName) {

            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = fileName + ".xls";
            excelconfig.HeadFont = "微软雅黑";
            excelconfig.HeadPoint = 10;
            excelconfig.IsAllSizeColumn = true;
            excelconfig.ColumnModel = new List<ExcelColumnModel>();
            var filedList = GetTTheRawMaterialChuK();
            foreach (var item in filedList)
            {
                var column = item.Key;
                var excelColumn = item.Value;
                excelconfig.ColumnModel.Add(new ExcelColumnModel() { Column = column, ExcelColumn = excelColumn });
            }
            var UnitName = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "337742224898917637").Select(s => new ZjnPlaneGoodsListOutput { UnitName = s.FullName + ":" + s.EnCode }).ToList();
            var dataList = new List<ZjnBillsHistoryEntity>() { new ZjnBillsHistoryEntity() {
                PurchaseOrder= "单据号",
                OrderNo="xxxxx-0",
                ProductsTotal=500,
                ProductsLocation="xxxxxx",
                OrderType="字段说明:"+GettName(UnitName),
            } };//初始化 一条空数据
            var addPath = FileVariable.TemporaryFilePath + excelconfig.FileName;
            ExcelExportHelper<ZjnBillsHistoryEntity>.Export(dataList, excelconfig, addPath);
            return new { name = excelconfig.FileName, url = "/api/file/Download?encryption=" + DESCEncryption.Encrypt(_userManager.UserId + "|" + excelconfig.FileName + "|" + addPath, "HSZ") };

        }

        public string GettName(List<ZjnPlaneGoodsListOutput> lists)
        {
            string Nmae = "";
            foreach (var item in lists)
            {
                Nmae += "" + item.UnitName;
            }
            return Nmae;
        }


        public Dictionary<string, string> GetTTheRawMaterialChuK(List<string> fields = null) {
            var res = new Dictionary<string, string>();
            res.Add("PurchaseOrder","单据编号");
            res.Add("OrderNo", "**32位流水号**");           
            res.Add("ProductsLocation", "**物料货位**");
            res.Add("ProductsTotal", "**出库数量**");
            res.Add("OrderType", "**单据类型**");
            if (fields == null || !fields.Any()) return res;
            var result = new Dictionary<string, string>();
            foreach (var item in res)
            {
                if (fields.Contains(item.Key))
                    result.Add(item.Key, item.Value);
            }

            return result;

        }



        private Dictionary<string, string> GetTheRawMaterial(List<string> fields = null)
        {
            var res = new Dictionary<string, string>();            
            res.Add("PurchaseOrder", "**单据号**");
            res.Add("OrderNo", "**32位流水号**");
            res.Add("OrderType", "**单据类型**");
            res.Add("ProductsTotal", "**物料数量**");
            res.Add("ProductsLocation", "**物料货位**");           
            res.Add("CreateUser", "**检验状态**"); 
            res.Add("ProductsGrade", "物料等级");
            res.Add("ProductsUser", "客户编号");                     
            res.Add("TheTray", "普通托盘号");
            res.Add("TheContainer", "容器托盘号");

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
        /// 小极卷出库导入数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost("ImportData")]
        public async Task<dynamic> ImportData(List<ZjnBillsHistoryEntity> list)
        {
            var res = await ImportTheRawMaterialDatas(list,"006");
            var addlist = res.First() as List<ZjnBillsHistoryEntity>;
            var errorlist = res.Last() as List<ZjnBillsHistoryEntity>;
            return new BillsHistoryImportResultOutput() { snum = addlist.Count, fnum = errorlist.Count, failResult = errorlist, resultType = errorlist.Count < 1 ? 0 : 1 };
        }

        /// <summary>
        /// 小极卷入库导入数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost("SmallpolarvolumeImportData")]
        public async Task<dynamic> SmallpolarvolumeImportData(List<ZjnBillsHistoryEntity> list)
        {
            var res = await ImportTheRawMaterialDatas(list, "005");
            var addlist = res.First() as List<ZjnBillsHistoryEntity>;
            var errorlist = res.Last() as List<ZjnBillsHistoryEntity>;
            return new BillsHistoryImportResultOutput() { snum = addlist.Count, fnum = errorlist.Count, failResult = errorlist, resultType = errorlist.Count < 1 ? 0 : 1 };
        }

        /// <summary>
        /// 大极卷出库导入数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost("TheBigPoleRollsOutOfstorageData")] 
        public async Task<dynamic> TheBigPoleRollsOutOfstorageData(List<ZjnBillsHistoryEntity> list)
        {
            var res = await ImportTheRawMaterialDatas(list, "008");
            var addlist = res.First() as List<ZjnBillsHistoryEntity>;
            var errorlist = res.Last() as List<ZjnBillsHistoryEntity>;
            return new BillsHistoryImportResultOutput() { snum = addlist.Count, fnum = errorlist.Count, failResult = errorlist, resultType = errorlist.Count < 1 ? 0 : 1 };
        }

        /// <summary>
        /// 大极卷入库导入数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost("TheLargePoleIsRolledIntoStorageeData")]  
        public async Task<dynamic> TheLargePoleIsRolledIntoStorageeData(List<ZjnBillsHistoryEntity> list)
        {
            var res = await ImportTheRawMaterialDatas(list, "007");
            var addlist = res.First() as List<ZjnBillsHistoryEntity>;
            var errorlist = res.Last() as List<ZjnBillsHistoryEntity>;
            return new BillsHistoryImportResultOutput() { snum = addlist.Count, fnum = errorlist.Count, failResult = errorlist, resultType = errorlist.Count < 1 ? 0 : 1 };
        }

        /// <summary>
        /// 原材料出库导入数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost("RawMaterialOutData")]
        public async Task<dynamic> RawMaterialOutData(List<ZjnBillsHistoryEntity> list)
        {
            var res = await ImportTheRawMaterialDatasChuKu(list, "004");
            var addlist = res.First() as List<ZjnBillsHistoryEntity>;
            var errorlist = res.Last() as List<ZjnBillsHistoryEntity>;
            return new BillsHistoryImportResultOutput() { snum = addlist.Count, fnum = errorlist.Count, failResult = errorlist, resultType = errorlist.Count < 1 ? 0 : 1 };
        }

        /// <summary>
        /// 原材料入库导入数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost("RawMaterialWarehousingData")]
        public async Task<dynamic> RawMaterialWarehousingData(List<ZjnBillsHistoryEntity> list)
        {
            var res = await ImportTheRawMaterialDatas(list, "001");
            var addlist = res.First() as List<ZjnBillsHistoryEntity>;
            var errorlist = res.Last() as List<ZjnBillsHistoryEntity>;
            return new BillsHistoryImportResultOutput() { snum = addlist.Count, fnum = errorlist.Count, failResult = errorlist, resultType = errorlist.Count < 1 ? 0 : 1 };
        }


        /// <summary>
        /// 结构件入库导入数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost("RawMaterialWJgJingData")]
        public async Task<dynamic> RawMaterialWJgJingData(List<ZjnBillsHistoryEntity> list)
        {
            var res = await ImportTheRawMaterialDatas(list, "009");
            var addlist = res.First() as List<ZjnBillsHistoryEntity>;
            var errorlist = res.Last() as List<ZjnBillsHistoryEntity>;
            return new BillsHistoryImportResultOutput() { snum = addlist.Count, fnum = errorlist.Count, failResult = errorlist, resultType = errorlist.Count < 1 ? 0 : 1 };
        }
        /// <summary>
        /// 结构件出库导入数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost("RawMaterialOutjgjData")]
        public async Task<dynamic> RawMaterialOutjgjData(List<ZjnBillsHistoryEntity> list)
        {
            var res = await ImportTheRawMaterialDatasChuKu(list, "010");
            var addlist = res.First() as List<ZjnBillsHistoryEntity>;
            var errorlist = res.Last() as List<ZjnBillsHistoryEntity>;
            return new BillsHistoryImportResultOutput() { snum = addlist.Count, fnum = errorlist.Count, failResult = errorlist, resultType = errorlist.Count < 1 ? 0 : 1 };
        }

        /// <summary>
        /// 入库导入数据函数
        /// </summary>
        /// <param name="list">list</param>
        /// <returns>[成功列表,失败列表]</returns>
        private async Task<object[]> ImportTheRawMaterialDatas(List<ZjnBillsHistoryEntity> list,string type)
        {
            List<ZjnBillsHistoryEntity> billsHistoryList = list;
            var userInfo = await _userManager.GetUserInfo();
            #region 排除错误数据
            if (billsHistoryList == null || billsHistoryList.Count() < 1)
                throw HSZException.Oh(ErrorCode.D5019);
            //必填字段验证 (32位流水号,物料id)
            var errorList = billsHistoryList.Where(x => string.IsNullOrEmpty(x.OrderNo) || string.IsNullOrEmpty(x.PurchaseOrder) || x.ProductsTotal < 0 || string.IsNullOrEmpty(x.ProductsLocation) || x.CreateUser == null).ToList();
            
            billsHistoryList = billsHistoryList.Except(errorList).ToList();
            //出入库记录表
            List<ZjnBillsHistoryEntity> listBillsHistory = new List<ZjnBillsHistoryEntity>();
            //新增库存记录
            List<ZjnPlaneMaterialInventoryEntity> zjnPlaneMaterialInventory = new List<ZjnPlaneMaterialInventoryEntity>();
            //修改库存修改数量
            List<ZjnPlaneMaterialInventoryEntity> zjnPlaneMaterial = new List<ZjnPlaneMaterialInventoryEntity>();
            //分析32流水码
            foreach (var item in billsHistoryList)
            {
                #region 添加入库记录
                var buwzhi = item.OrderNo.Substring(0, 2);
                string OrderNo = null;
                string productsSupplier = null;
                string productsSupplierDate = null;
                if (buwzhi == "00")
                {
                    //老的32位流水
                    OrderNo = item.OrderNo.Substring(4, 10);
                    productsSupplier = item.OrderNo.Substring(14, 7);
                    productsSupplierDate = "20" + item.OrderNo.Substring(21, 6);
                }
                else
                {
                    //新的32位流水
                    OrderNo = item.OrderNo.Substring(0, 10);
                    productsSupplier = item.OrderNo.Substring(10, 7);
                    productsSupplierDate = "20" + item.OrderNo.Substring(17, 6);
                }

               
                //查询物料
                var listplaneGoods = await _zjnPlaneGoodsRepository.GetFirstAsync(x => x.GoodsCode == OrderNo);
                //查询供应商
                var productsSupplierList = await _zjnBaseSupplierRepository.GetFirstAsync(x => x.SupplierNo == productsSupplier);
                if (listplaneGoods == null || productsSupplierList == null)
                {
                    //添加没有供应商的的数据
                    errorList.AddRange(billsHistoryList.Where(u => u.OrderNo == item.OrderNo && !errorList.Select(x => x.OrderNo).Contains(u.OrderNo)));//已存在的编号 列入 错误列表
                    continue;
                }

                //验证/托盘/货位/容器
                if (item.TheTray != null || item.TheContainer != null)
                {
                    //查询托盘
                    var entityType = await _zjnPlaneTrayrRepository.GetFirstAsync(p => p.TrayNo.Equals(item.TheTray));
                    //查询容器
                    var entityTheContainer = await _zjnPlaneTrayrRepository.GetFirstAsync(p => p.TrayNo.Equals(item.TheContainer));
                    if (entityType!=null)
                    {
                        //判断是否空闲
                        if (entityType.TrayState != 1)
                        {
                            errorList.AddRange(billsHistoryList.Where(u => u.OrderNo == item.OrderNo && !errorList.Select(x => x.OrderNo).Contains(u.OrderNo)));//已存在的编号 列入 错误列表
                            continue;
                        }
                    }
                    if (entityTheContainer!=null)
                    {
                        //判断是否空闲
                        if (entityTheContainer.TrayState != 1)
                        {
                            //插入错误列表
                            errorList.AddRange(billsHistoryList.Where(u => u.OrderNo == item.OrderNo && !errorList.Select(x => x.OrderNo).Contains(u.OrderNo)));
                            continue;
                        }
                    }
                    
                }
                //查询货位信息
                var BaseLocation = await _zjnBaseLocationRepository.GetFirstAsync(x => x.LocationNo.Equals(item.ProductsLocation));
                if (BaseLocation == null)
                {
                    //插入错误列表
                    errorList.AddRange(billsHistoryList.Where(u => u.OrderNo == item.OrderNo && !errorList.Select(x => x.OrderNo).Contains(u.OrderNo)));
                    continue;
                }
                else
                {
                    //查询货位信息
                    if (BaseLocation.LocationStatus != "0" || BaseLocation.EnabledMark != 1)
                    {
                        errorList.AddRange(billsHistoryList.Where(u => u.OrderNo == item.OrderNo && !errorList.Select(x => x.OrderNo).Contains(u.OrderNo)));
                        continue;
                    }
                }

                //1:是待检验；2：已检验合格
                int InspectionStatus = 2;
                if (item.CreateUser =="1")
                {
                     InspectionStatus = 2;
                }
                else {
                    InspectionStatus = 0;
                }
                //插入入库记录
                ZjnBillsHistoryEntity zjnBillsHistory = new ZjnBillsHistoryEntity()
                {

                    Id = YitIdHelper.NextId().ToString(),
                    OrderType = item.OrderType,
                    ProductsName = listplaneGoods.GoodsName,
                    ProductsLocation = item.ProductsLocation,
                    ProductsBach = item.OrderNo,
                    OrderNo = item.OrderNo,
                    PurchaseOrder = item.PurchaseOrder,
                    ProductsGrade = item.ProductsGrade,
                    ProductsNo = listplaneGoods.GoodsCode,
                    ProductsStyle = listplaneGoods.Specifications,
                    ProductsSupplier = productsSupplierList.SupplierNo,
                    ProductsTotal = item.ProductsTotal,
                    ProductsType = listplaneGoods.GoodsType.ToString(),
                    ProductsUnit = listplaneGoods.Unit.ToString(),
                    ProductsUser = item.ProductsUser,
                    TheDateOfProduction = DateTime.ParseExact(productsSupplierDate, "yyyyMMdd", CultureInfo.CurrentCulture),
                    TestType = listplaneGoods.CheckType,
                    TheContainer = item.TheContainer,
                    TheTray = item.TheTray,
                    CreateTime = DateTime.Now,
                    CreateUser = userInfo.userId,
                    InspectionStatus = InspectionStatus,
                    ExpiryDate = DateTime.ParseExact(productsSupplierDate, "yyyyMMdd", CultureInfo.CurrentCulture).AddDays(Convert.ToDouble(listplaneGoods.ShelfLife)), //item.ExpiryDate,
                    LastModifyUserId =userInfo.userId,
                    LastModifyTime=DateTime.Now,
                    IsDelete=0,
                    BatchDeliveryQuantity = "0"

                };
                listBillsHistory.Add(zjnBillsHistory);
                #endregion

                #region 添加库存或者修改库存数量
                
                var inventoryEntity=(await _zjnPlaneMaterialInventoryRepository.GetFirstAsync(x=> x.ProductsCode== OrderNo && x.ProductsBatch==item.OrderNo && x.ProductsLocation==item.ProductsLocation)).Adapt<ZjnPlaneMaterialInventoryEntity>();
                if (inventoryEntity == null)
                {
                    //插入库存
                    ZjnPlaneMaterialInventoryEntity zjnPlane = new ZjnPlaneMaterialInventoryEntity() {
                        Id = YitIdHelper.NextId().ToString(),
                        ProductsBatch = item.OrderNo,
                        ProductsCode = OrderNo,
                        CreateUser = userInfo.userId,
                        ProductsCustomer = item.ProductsUser,
                        ProductsSupplier = productsSupplierList.SupplierNo,
                        ProductsName = listplaneGoods.GoodsName,
                        ProductsQuantity = item.ProductsTotal,
                        ProductsUnit = listplaneGoods.Unit.ToString(),
                        ProductsType = listplaneGoods.GoodsType.ToString(),
                        ProductsGrade = item.ProductsBach,
                        ProductsLocation = item.ProductsLocation,
                        ProductsStyle = item.ProductsStyle,
                        ProductsState = listplaneGoods.GoodsState.ToString(),
                        ExpiryDate = DateTime.ParseExact(productsSupplierDate, "yyyyMMdd", CultureInfo.CurrentCulture),
                        InspectionStatus = InspectionStatus.ToString(),
                        CreateTime=DateTime.Now,
                        LastModifyUserId = userInfo.userId,
                        LastModifyTime = DateTime.Now
                    };
                    zjnPlaneMaterialInventory.Add(zjnPlane);

                }
                else {
                    //修改库存数量
                    ZjnPlaneMaterialInventoryEntity inventoryEntity1 = new ZjnPlaneMaterialInventoryEntity();
                    inventoryEntity.ProductsQuantity = inventoryEntity.ProductsQuantity+ item.ProductsTotal;
                    inventoryEntity1 = inventoryEntity;
                    zjnPlaneMaterial.Add(inventoryEntity1);
                }



                #endregion
            }
            #endregion
            //去掉错误的数据
            listBillsHistory=listBillsHistory.Except(errorList).ToList();


            if (listBillsHistory.Any())
            {
                try
                {
                    //开启事务
                    _db.BeginTran();

                    //新增记录
                    var newEntity = await _zjnBillsHistoryRepository.AsInsertable(listBillsHistory).ExecuteReturnEntityAsync();
                    //新增库存
                    await _zjnPlaneMaterialInventoryRepository.AsInsertable(zjnPlaneMaterialInventory).ExecuteReturnEntityAsync();
                    //修改库存
                    await _zjnPlaneMaterialInventoryRepository.AsUpdateable(zjnPlaneMaterial).ExecuteCommandAsync();
                    _db.CommitTran();
                }
                catch (Exception e)
                {
                    string es = e.Message;
                    _db.RollbackTran();
                    errorList.AddRange(billsHistoryList);
                }
            }
            return new object[] { listBillsHistory, errorList };
        }

        /// <summary>
        /// 出库导入数据函数
        /// </summary>
        /// <param name="list"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private async Task<object[]> ImportTheRawMaterialDatasChuKu(List<ZjnBillsHistoryEntity> list, string type) {
            List<ZjnBillsHistoryEntity> billsHistoryList = list;
            //获取当前用户信息
            var userInfo = await _userManager.GetUserInfo();
            #region 排除错误数据
            if (billsHistoryList == null || billsHistoryList.Count() < 1)
                throw HSZException.Oh(ErrorCode.D5019);
            //必填字段验证 
            var errorList = billsHistoryList.Where(x => string.IsNullOrEmpty(x.OrderNo) || x.ProductsTotal < 0 || string.IsNullOrEmpty(x.ProductsLocation)).ToList();
            
            billsHistoryList = billsHistoryList.Except(errorList).ToList();
            //出入库记录表
            List<ZjnBillsHistoryEntity> listBillsHistory = new List<ZjnBillsHistoryEntity>();           
            //修改库存修改数量
            List<ZjnPlaneMaterialInventoryEntity> zjnPlaneMaterial = new List<ZjnPlaneMaterialInventoryEntity>();
            List<ZjnBillsHistoryEntity> listBillsHistoryRku = new List<ZjnBillsHistoryEntity>();
            //分析32流水码
            foreach (var item in billsHistoryList)
            {
                #region 添加入库记录

                var buwzhi = item.OrderNo.Substring(0, 2);
                string OrderNo = null;
                string productsSupplier = null;
                string productsDate = null;
                if (buwzhi == "00")
                {
                    OrderNo = item.OrderNo.Substring(4, 10);
                    productsSupplier = item.OrderNo.Substring(14, 7);
                    productsDate = "20" + item.OrderNo.Substring(21, 6);
                }
                else
                {
                    OrderNo = item.OrderNo.Substring(0, 10);
                    productsSupplier = item.OrderNo.Substring(10, 7);
                    productsDate = "20" + item.OrderNo.Substring(17, 6);
                }
                //查询入库信息              
                var BillsHistory = (await _zjnBillsHistoryRepository.GetFirstAsync(p => p.OrderNo == item.OrderNo && p.ProductsLocation == item.ProductsLocation && p.IsDelete == 0)).Adapt<ZjnBillsHistoryInfoOutput>();

                if (BillsHistory == null)
                {
                    errorList.AddRange(billsHistoryList.Where(u => u.OrderNo == item.OrderNo && !errorList.Select(x => x.OrderNo).Contains(u.OrderNo)));//错误列表
                    continue;

                }
                //查询物料
                var listplaneGoods = await _zjnPlaneGoodsRepository.GetFirstAsync(x => x.GoodsCode == OrderNo);
                //查询供应商
                var productsSupplierList = await _zjnBaseSupplierRepository.GetFirstAsync(x => x.SupplierNo == productsSupplier);
                if (listplaneGoods == null || productsSupplierList== null)
                {
                    errorList.AddRange(billsHistoryList.Where(u => u.OrderNo == item.OrderNo && !errorList.Select(x => x.OrderNo).Contains(u.OrderNo)));//错误列表
                    continue;
                }
                if (item.OrderType != "C004" && item.OrderType != "C003")
                {
                    if (BillsHistory.InspectionStatus != "0")
                    {
                        errorList.AddRange(billsHistoryList.Where(u => u.OrderNo == item.OrderNo && !errorList.Select(x => x.OrderNo).Contains(u.OrderNo)));//错误列表
                        continue;
                    }
                }
                //查询库存信息
                var inventoryEntityList = _zjnPlaneMaterialInventoryRepository.AsSugarClient().Queryable<ZjnPlaneMaterialInventoryEntity>().Where(x => x.ProductsCode == OrderNo&& x.ProductsBatch==item.OrderNo && x.ProductsLocation == item.ProductsLocation&& x.ProductsQuantity>0&& x.ProductsBatch == item.OrderNo&& x.ProductsState == "1").OrderBy("F_CreateTime").ToList();//根据物料号和货位查询出所有的库存信息
               //获取库存总数
                var sQuantity=inventoryEntityList.Sum(x => x.ProductsQuantity);
                //入库记录和库存表不可以为空
                if (BillsHistory== null || inventoryEntityList.Count()==0)
                {
                    errorList.AddRange(billsHistoryList.Where(u => u.OrderNo == item.OrderNo && !errorList.Select(x => x.OrderNo).Contains(u.OrderNo)));//错误列表                  
                    continue;
                }
                //出库数量是否大于当前库存
                if (sQuantity< item.ProductsTotal) {
                    errorList.AddRange(billsHistoryList.Where(u => u.OrderNo == item.OrderNo && !errorList.Select(x => x.OrderNo).Contains(u.OrderNo)));//错误列表
                    continue;

                }
               
                
                    //先进先出
                    decimal? Quantity = -1;
                    foreach (var inventory in inventoryEntityList)
                    {

                        ZjnPlaneMaterialInventoryEntity inventoryEntity1 = new ZjnPlaneMaterialInventoryEntity();
                        if (inventory.ProductsQuantity > item.ProductsTotal)
                        {
                            #region 修改库存数量  
                            inventory.ProductsQuantity = inventory.ProductsQuantity - item.ProductsTotal;
                            inventoryEntity1 = inventory;
                            zjnPlaneMaterial.Add(inventoryEntity1);
                            Quantity = 0;
                        var listHistory = _zjnBillsHistoryRepository.AsSugarClient().Queryable<ZjnBillsHistoryEntity>().Where(p => p.Id == BillsHistory.id).ToList();
                        ZjnBillsHistoryEntity zjnBills = new ZjnBillsHistoryEntity();
                        zjnBills = listHistory[0];
                        zjnBills.BatchDeliveryQuantity = item.ProductsTotal.ToString();
                        listBillsHistoryRku.Add(zjnBills);
                        //listBillsHistoryRku.AddRange(listHR(inventory, item.ProductsTotal));
                        #endregion
                    }
                        else
                        {
                            #region 修改库存数量     
                            if (Quantity == -1)
                            {

                                var sum = item.ProductsTotal - inventory.ProductsQuantity;
                                Quantity = sum;
                                inventory.ProductsQuantity = 0;
                            var listHistory = _zjnBillsHistoryRepository.AsSugarClient().Queryable<ZjnBillsHistoryEntity>().Where(p => p.Id == BillsHistory.id).ToList();
                            ZjnBillsHistoryEntity zjnBills = new ZjnBillsHistoryEntity();
                            zjnBills = listHistory[0];
                            zjnBills.BatchDeliveryQuantity = inventory.ProductsQuantity.ToString();
                            listBillsHistoryRku.Add(zjnBills);
                            //listBillsHistoryRku.AddRange(listHR(inventory, item.ProductsTotal));
                        }
                            else
                            {
                                var sum = Quantity - inventory.ProductsQuantity;
                                if (sum < 0)
                                {
                                    inventory.ProductsQuantity = inventory.ProductsQuantity - sum;
                                    Quantity = 0;
                                    listBillsHistoryRku.AddRange(listHR(inventory, item.ProductsTotal));
                                var listHistory = _zjnBillsHistoryRepository.AsSugarClient().Queryable<ZjnBillsHistoryEntity>().Where(p => p.Id == BillsHistory.id).ToList();
                                ZjnBillsHistoryEntity zjnBills = new ZjnBillsHistoryEntity();
                                zjnBills = listHistory[0];
                                zjnBills.BatchDeliveryQuantity = inventory.ProductsQuantity.ToString();
                                listBillsHistoryRku.Add(zjnBills);
                                }
                                else
                                {
                                    inventory.ProductsQuantity = 0;
                                    Quantity = sum;
                                    listBillsHistoryRku.AddRange(listHR(inventory, item.ProductsTotal));
                                var listHistory = _zjnBillsHistoryRepository.AsSugarClient().Queryable<ZjnBillsHistoryEntity>().Where(p => p.Id == BillsHistory.id).ToList();
                                ZjnBillsHistoryEntity zjnBills = new ZjnBillsHistoryEntity();
                                zjnBills = listHistory[0];
                                zjnBills.BatchDeliveryQuantity = inventory.ProductsQuantity.ToString();
                                listBillsHistoryRku.Add(zjnBills);
                            }

                            }
                            inventoryEntity1 = inventory;
                            zjnPlaneMaterial.Add(inventoryEntity1);
                            #endregion
                        }
                        if (Quantity == 0) break;
                    }

                    //插入出库明细
                    ZjnBillsHistoryEntity zjnBillsHistory = new ZjnBillsHistoryEntity()
                    {

                        Id = YitIdHelper.NextId().ToString(),
                        OrderType = item.OrderType==null?"C001": item.OrderType,
                        ProductsName = listplaneGoods.GoodsName,
                        ProductsLocation = BillsHistory.ProductsLocation,
                        ProductsBach = BillsHistory.orderNo,
                        OrderNo = BillsHistory.orderNo,
                        PurchaseOrder = BillsHistory.PurchaseOrder,
                        ProductsGrade = BillsHistory.productsGrade,
                        ProductsNo = listplaneGoods.GoodsCode,
                        ProductsStyle = listplaneGoods.Specifications,
                        ProductsSupplier = productsSupplierList.SupplierNo,
                        ProductsTotal = item.ProductsTotal,
                        ProductsType = listplaneGoods.GoodsType.ToString(),
                        ProductsUnit = listplaneGoods.Unit.ToString(),
                        ProductsUser = BillsHistory.productsUser,
                        TheDateOfProduction = DateTime.ParseExact(productsDate, "yyyyMMdd", CultureInfo.CurrentCulture),
                        TestType = listplaneGoods.CheckType,
                        TheContainer = BillsHistory.TheContainer,
                        TheTray = BillsHistory.TheTray,
                        CreateTime = DateTime.Now,
                        CreateUser = userInfo.userId,
                        InspectionStatus = Convert.ToInt32(BillsHistory.InspectionStatus),
                        ExpiryDate = BillsHistory.ExpiryDate,
                        LastModifyUserId = userInfo.userId,
                        LastModifyTime = DateTime.Now,
                        IsDelete = 0
                    };
                    listBillsHistory.Add(zjnBillsHistory);
                
                
                #endregion

                
            }
            #endregion

            listBillsHistory=listBillsHistory.Except(errorList).ToList();
            if (listBillsHistory.Any())
            {
                try
                {
                    //开启事务
                    _db.BeginTran();

                    //新增记录
                    var newEntity = await _zjnBillsHistoryRepository.AsInsertable(listBillsHistory).ExecuteReturnEntityAsync();                   
                    //修改库存
                    await _zjnPlaneMaterialInventoryRepository.AsUpdateable(zjnPlaneMaterial).ExecuteCommandAsync();
                    //修改批次的批次库存
                    await _zjnBillsHistoryRepository.AsUpdateable(listBillsHistoryRku).ExecuteCommandAsync();
                    _db.CommitTran();
                }
                catch (Exception e)
                {
                    string es = e.Message;
                    _db.RollbackTran();
                    errorList.AddRange(billsHistoryList);
                }
            }
            return new object[] { listBillsHistory, errorList };

        }


        /// <summary>
        /// 导出错误报告
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost("ExportExceptionData")]
        public async Task<dynamic> ExportExceptionData(List<ZjnBillsHistoryEntity> list,string fileName,string type)
        {
            var res = await ImportTheRawMaterialDatas(list, type);
            var errorlist = res.Last() as List<ZjnBillsHistoryEntity>;//错误数据

            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName =  "入库信息导入错误报告.xls";
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
            ExcelExportHelper<ZjnBillsHistoryEntity>.Export(errorlist, excelconfig, addPath);

            return new { name = excelconfig.FileName, url = "/api/file/Download?encryption=" + DESCEncryption.Encrypt(_userManager.UserId + "|" + excelconfig.FileName + "|" + addPath, "HSZ") };
        }


        /// <summary>
        /// 出库导出错误报告
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost("ExportExceptionwuhData")]
        public async Task<dynamic> ExportExceptionwuhData(List<ZjnBillsHistoryEntity> list, string fileName, string type)
        {
            var res = await ImportTheRawMaterialDatas(list, type);
            var errorlist = res.Last() as List<ZjnBillsHistoryEntity>;//错误数据

            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "出库信息导入错误报告.xls";
            excelconfig.HeadFont = "微软雅黑";
            excelconfig.HeadPoint = 10;
            excelconfig.IsAllSizeColumn = true;
            excelconfig.ColumnModel = new List<ExcelColumnModel>();
            var filedList = GetTTheRawMaterialChuK();
            foreach (var item in filedList)
            {
                var column = item.Key;
                var excelColumn = item.Value;
                excelconfig.ColumnModel.Add(new ExcelColumnModel() { Column = column, ExcelColumn = excelColumn });
            }
            var addPath = FileVariable.TemporaryFilePath + excelconfig.FileName;
            ExcelExportHelper<ZjnBillsHistoryEntity>.Export(errorlist, excelconfig, addPath);

            return new { name = excelconfig.FileName, url = "/api/file/Download?encryption=" + DESCEncryption.Encrypt(_userManager.UserId + "|" + excelconfig.FileName + "|" + addPath, "HSZ") };
        }



        /// <summary>
        /// 入库导入预览
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
        /// 出库导入预览
        /// </summary>
        /// <returns></returns>
        [HttpGet("ImportPreviewYulan")]
        public dynamic ImportPreviewYulan(string fileName)
        {
            try
            {
                var FileEncode = GetTTheRawMaterialChuK();
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
        /// 入库导出Excel
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("ExportExcel")]
        public async Task<dynamic> ExportExcel([FromQuery]  ZjnBillsHistoryListQueryInput input)
        {
            //供应商信息列表
            var userList = new List<ZjnBillsHistoryListQueryInput>();
           
                var list = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "337742110771905797").Select(s => s.EnCode).ToList();
           
           
            if (input.dataType == 0)
            {
                userList = await _zjnBillsHistoryRepository.AsSugarClient().Queryable<ZjnBillsHistoryEntity>()
                .LeftJoin<ZjnBaseCustomerEntity>((a, b) => a.ProductsUser == b.CustomerNo)
                .LeftJoin<ZjnWmsSupplierEntity>((a, b, c) => a.ProductsSupplier == c.SupplierNo)
                .Where(a => list.Contains(a.OrderType) && a.IsDelete == 0)                
                .WhereIF(!string.IsNullOrEmpty(input.F_OrderNo), a => a.OrderNo.Contains(input.F_OrderNo))
                .WhereIF(!string.IsNullOrEmpty(input.OrderTypes), a => a.OrderType.Contains(input.OrderTypes))
                .WhereIF(!string.IsNullOrEmpty(input.F_PurchaseOrder), a => a.PurchaseOrder.Contains(input.F_PurchaseOrder))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsName.Contains(input.F_ProductsName))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsNo), a => a.ProductsNo.Contains(input.F_ProductsNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsUser), a => a.ProductsUser.Contains(input.F_ProductsUser))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsSupplier), a => a.ProductsSupplier.Contains(input.F_ProductsSupplier))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsLocation), a => a.ProductsLocation.Contains(input.F_ProductsLocation))
                .Select((a, b, c) => new ZjnBillsHistoryListQueryInput
                {
                    F_OrderNo = a.OrderNo,
                    F_OrderType = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.OrderType && s.DictionaryTypeId == "337742110771905797").Select(s => s.FullName),
                    F_ProductsNo = a.ProductsNo,
                    F_ProductsName = a.ProductsName,
                    F_ProductsType = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.ProductsType && s.DictionaryTypeId == "325449144728552709").Select(s => s.FullName),
                    F_ProductsStyle = a.ProductsStyle,
                    F_ProductsTotal = a.ProductsTotal,
                    F_ProductsUnit = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.ProductsUnit && s.DictionaryTypeId == "326384591566800133").Select(s => s.FullName),
                    F_ProductsGrade = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.ProductsGrade && s.DictionaryTypeId == "326588657760732421").Select(s => s.FullName),
                    F_ProductsBach = a.ProductsBach,
                    F_ProductsUser = b.CustomerName,
                    F_ProductsSupplier = c.SupplierName,
                    F_ProductsLocation = a.ProductsLocation,
                    F_LastModifyTime = a.LastModifyTime,
                    F_TheContainer = a.TheContainer,
                    F_TheTray = a.TheTray,
                    F_CreateUser = a.CreateUser,
                    F_CreateTime = a.CreateTime,
                    TestTypeName = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.TestType.ToString() && s.DictionaryTypeId == "325448312364729605").Select(s => s.FullName),
                    F_PurchaseOrder=a.PurchaseOrder,
                    F_ExpiryDate=a.ExpiryDate,
                    F_InspectionStatus = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.InspectionStatus.ToString() && s.DictionaryTypeId == "325448475967751429").Select(s => s.FullName),
                    F_LastModifyUserId = a.LastModifyUserId,
                }).ToPageListAsync(input.currentPage, input.pageSize);

            }
            else
            {
                userList = await _zjnBillsHistoryRepository.AsSugarClient().Queryable<ZjnBillsHistoryEntity>()
                .LeftJoin<ZjnBaseCustomerEntity>((a, b) => a.ProductsUser == b.CustomerNo)
                .LeftJoin<ZjnWmsSupplierEntity>((a, b, c) => a.ProductsSupplier == c.SupplierNo)
                .Where(a => list.Contains(a.OrderType) && a.IsDelete == 0)
                .WhereIF(!string.IsNullOrEmpty(input.F_OrderNo), a => a.OrderNo.Contains(input.F_OrderNo))
                .WhereIF(!string.IsNullOrEmpty(input.OrderTypes), a => a.OrderType.Contains(input.OrderTypes))
                .WhereIF(!string.IsNullOrEmpty(input.F_PurchaseOrder), a => a.PurchaseOrder.Contains(input.F_PurchaseOrder))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsName.Contains(input.F_ProductsName))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsNo), a => a.ProductsNo.Contains(input.F_ProductsNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsUser), a => a.ProductsUser.Contains(input.F_ProductsUser))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsSupplier), a => a.ProductsSupplier.Contains(input.F_ProductsSupplier))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsLocation), a => a.ProductsLocation.Contains(input.F_ProductsLocation))
                 .Select((a, b, c) => new ZjnBillsHistoryListQueryInput
                 {
                     F_OrderNo = a.OrderNo,
                     F_OrderType = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.OrderType && s.DictionaryTypeId == "337742110771905797").Select(s => s.FullName),
                     F_ProductsName = a.ProductsName,
                     F_ProductsType = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.ProductsType && s.DictionaryTypeId == "325449144728552709").Select(s => s.FullName),
                     F_ProductsStyle = a.ProductsStyle,
                     F_ProductsTotal = a.ProductsTotal,
                     F_ProductsUnit = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.ProductsUnit && s.DictionaryTypeId == "326384591566800133").Select(s => s.FullName),
                     F_ProductsGrade = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.ProductsGrade && s.DictionaryTypeId == "326588657760732421").Select(s => s.FullName),
                     F_ProductsBach = a.ProductsBach,
                     F_ProductsUser = b.CustomerName,
                     F_ProductsSupplier = c.SupplierName,
                     F_ProductsLocation = a.ProductsLocation,
                     F_LastModifyTime = a.LastModifyTime,
                     F_TheContainer = a.TheContainer,
                     F_ProductsNo=a.ProductsNo,
                     F_TheTray = a.TheTray,
                     F_CreateUser = a.CreateUser,
                     F_CreateTime = a.CreateTime,
                     TestTypeName = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.TestType.ToString() && s.DictionaryTypeId == "325448312364729605").Select(s => s.FullName),
                     F_PurchaseOrder = a.PurchaseOrder,
                     F_ExpiryDate = a.ExpiryDate,
                     F_InspectionStatus = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.InspectionStatus.ToString() && s.DictionaryTypeId == "325448475967751429").Select(s => s.FullName),
                     F_LastModifyUserId=a.LastModifyUserId,
                    
                 }).ToListAsync();
            }

            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = DateTime.Now.ToDateString() + input.OrderTypeName + ".xls";
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
            ExcelExportHelper<ZjnBillsHistoryListQueryInput>.Export(userList, excelconfig, addPath);

            return new { name = excelconfig.FileName, url = "/api/file/Download?encryption=" + DESCEncryption.Encrypt(_userManager.UserId + "|" + excelconfig.FileName + "|" + addPath, "HSZ") };
        }


        /// <summary>
        /// 出库导出Excel
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("ExportExcelChuK")]
        public async Task<dynamic> ExportExcelChuK([FromQuery] ZjnBillsHistoryListQueryInput input)
        {
            //供应商信息列表
            var userList = new List<ZjnBillsHistoryListQueryInput>();
            List<string> list = new List<string>();
            list = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "337742224898917637").Select(s => s.EnCode).ToList();
            if (input.dataType == 0)
            {
                userList = await _zjnBillsHistoryRepository.AsSugarClient().Queryable<ZjnBillsHistoryEntity>()
                .LeftJoin<ZjnBaseCustomerEntity>((a, b) => a.ProductsUser == b.CustomerNo)
                .LeftJoin<ZjnWmsSupplierEntity>((a, b, c) => a.ProductsSupplier == c.SupplierNo)
                .Where(a => list.Contains(a.OrderType) && a.IsDelete == 0)
                .WhereIF(!string.IsNullOrEmpty(input.F_OrderNo), a => a.OrderNo.Contains(input.F_OrderNo))
                .WhereIF(!string.IsNullOrEmpty(input.OrderTypes), a => a.OrderType.Contains(input.OrderTypes))
                .WhereIF(!string.IsNullOrEmpty(input.F_PurchaseOrder), a => a.PurchaseOrder.Contains(input.F_PurchaseOrder))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsName.Contains(input.F_ProductsName))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsNo), a => a.ProductsNo.Contains(input.F_ProductsNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsUser), a => a.ProductsUser.Contains(input.F_ProductsUser))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsSupplier), a => a.ProductsSupplier.Contains(input.F_ProductsSupplier))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsLocation), a => a.ProductsLocation.Contains(input.F_ProductsLocation))
                .Select((a, b, c) => new ZjnBillsHistoryListQueryInput
                {
                    F_OrderNo = a.OrderNo,
                    F_OrderType = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.OrderType && s.DictionaryTypeId == "337742224898917637").Select(s => s.FullName),
                    F_ProductsNo = a.ProductsNo,
                    F_ProductsName = a.ProductsName,
                    F_ProductsType = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.ProductsType && s.DictionaryTypeId == "325449144728552709").Select(s => s.FullName),
                    F_ProductsStyle = a.ProductsStyle,
                    F_ProductsTotal = a.ProductsTotal,
                    F_ProductsUnit = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.ProductsUnit && s.DictionaryTypeId == "326384591566800133").Select(s => s.FullName),
                    F_ProductsGrade = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.ProductsGrade && s.DictionaryTypeId == "326588657760732421").Select(s => s.FullName),
                    F_ProductsBach = a.ProductsBach,
                    F_ProductsUser = b.CustomerName,
                    F_ProductsSupplier = c.SupplierName,
                    F_ProductsLocation = a.ProductsLocation,
                    F_LastModifyTime = a.LastModifyTime,
                    F_TheContainer = a.TheContainer,
                    F_TheTray = a.TheTray,
                    F_CreateUser = a.CreateUser,
                    F_CreateTime = a.CreateTime,
                    TestTypeName = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.TestType.ToString() && s.DictionaryTypeId == "325448312364729605").Select(s => s.FullName),
                    F_PurchaseOrder = a.PurchaseOrder,
                    F_ExpiryDate = a.ExpiryDate,
                    F_InspectionStatus = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.InspectionStatus.ToString() && s.DictionaryTypeId == "325448475967751429").Select(s => s.FullName),
                    F_LastModifyUserId = a.LastModifyUserId,
                }).ToPageListAsync(input.currentPage, input.pageSize);

            }
            else
            {
                userList = await _zjnBillsHistoryRepository.AsSugarClient().Queryable<ZjnBillsHistoryEntity>()
                .LeftJoin<ZjnBaseCustomerEntity>((a, b) => a.ProductsUser == b.CustomerNo)
                .LeftJoin<ZjnWmsSupplierEntity>((a, b, c) => a.ProductsSupplier == c.SupplierNo)
                .Where(a => list.Contains(a.OrderType) && a.IsDelete == 0)
                .WhereIF(!string.IsNullOrEmpty(input.F_OrderNo), a => a.OrderNo.Contains(input.F_OrderNo))
                .WhereIF(!string.IsNullOrEmpty(input.OrderTypes), a => a.OrderType.Contains(input.OrderTypes))
                .WhereIF(!string.IsNullOrEmpty(input.F_PurchaseOrder), a => a.PurchaseOrder.Contains(input.F_PurchaseOrder))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsName.Contains(input.F_ProductsName))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsNo), a => a.ProductsNo.Contains(input.F_ProductsNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsUser), a => a.ProductsUser.Contains(input.F_ProductsUser))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsSupplier), a => a.ProductsSupplier.Contains(input.F_ProductsSupplier))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsLocation), a => a.ProductsLocation.Contains(input.F_ProductsLocation))
                 .Select((a, b, c) => new ZjnBillsHistoryListQueryInput
                 {
                     F_OrderNo = a.OrderNo,
                     F_OrderType = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.OrderType && s.DictionaryTypeId == "337742224898917637").Select(s => s.FullName),
                     F_ProductsName = a.ProductsName,
                     F_ProductsType = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.ProductsType && s.DictionaryTypeId == "325449144728552709").Select(s => s.FullName),
                     F_ProductsStyle = a.ProductsStyle,
                     F_ProductsTotal = a.ProductsTotal,
                     F_ProductsUnit = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.ProductsUnit && s.DictionaryTypeId == "326384591566800133").Select(s => s.FullName),
                     F_ProductsGrade = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.ProductsGrade && s.DictionaryTypeId == "326588657760732421").Select(s => s.FullName),
                     F_ProductsBach = a.ProductsBach,
                     F_ProductsUser = b.CustomerName,
                     F_ProductsSupplier = c.SupplierName,
                     F_ProductsLocation = a.ProductsLocation,
                     F_LastModifyTime = a.LastModifyTime,
                     F_TheContainer = a.TheContainer,
                     F_ProductsNo = a.ProductsNo,
                     F_TheTray = a.TheTray,
                     F_CreateUser = a.CreateUser,
                     F_CreateTime = a.CreateTime,
                     TestTypeName = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.TestType.ToString() && s.DictionaryTypeId == "325448312364729605").Select(s => s.FullName),
                     F_PurchaseOrder = a.PurchaseOrder,
                     F_ExpiryDate = a.ExpiryDate,
                     F_InspectionStatus = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.InspectionStatus.ToString() && s.DictionaryTypeId == "325448475967751429").Select(s => s.FullName),
                     F_LastModifyUserId = a.LastModifyUserId,

                 }).ToListAsync();
            }

            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = DateTime.Now.ToDateString() + input.OrderTypeName + ".xls";
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
            ExcelExportHelper<ZjnBillsHistoryListQueryInput>.Export(userList, excelconfig, addPath);

            return new { name = excelconfig.FileName, url = "/api/file/Download?encryption=" + DESCEncryption.Encrypt(_userManager.UserId + "|" + excelconfig.FileName + "|" + addPath, "HSZ") };
        }


        /// <summary>
        ///  字段对应 列名称
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetUserInfoFieldToTitle(List<string> fields = null)
        {
            var res = new Dictionary<string, string>();            
            res.Add("F_OrderNo", "32位流水号");
            res.Add("F_PurchaseOrder", "单据号");
            res.Add("F_OrderType", "单据类型");
            res.Add("F_ProductsNo", "物料编号");
            res.Add("F_ProductsName", "物料名称");           
            res.Add("F_ProductsStyle", "物料规格");
            res.Add("F_ProductsType", "物料类型");
            res.Add("F_ProductsLocation", "物料货位");
            res.Add("F_ProductsTotal", "物料数量");
            res.Add("F_ProductsUnit", "物料单位");
            res.Add("F_ProductsGrade", "物料等级");
            res.Add("F_ProductsBach", "物料批次");
            res.Add("F_ExpiryDate", "失效日期");
            res.Add("TestTypeName", "检验类型");
            res.Add("F_InspectionStatus", "检验状态");
            res.Add("F_ProductsUser", "客户编号");
            res.Add("F_ProductsSupplier", "供应商编号");
            res.Add("F_TheContainer", "容器托盘号");
            res.Add("F_TheTray", "普通托盘号");
           
            if (fields == null || !fields.Any()) return res;

            var result = new Dictionary<string, string>();

            res.Add("F_CreateUser", "创建人员");
            res.Add("F_CreateTime", "创建时间");
            res.Add("F_LastModifyUserId", "修改人员");
            res.Add("F_LastModifyTime", "修改时间");

            foreach (var item in res)
            {
                if (fields.Contains(item.Key))
                    result.Add(item.Key, item.Value);
            }

            return result;
        }
        /// <summary>
        /// 入库分析32位流水码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("GeAnalysisOfThe")]
        public async Task<dynamic> GeAnalysisOfThe(string id)
        {
            var buwzhi = id.Substring(0, 2);
            string OrderNo = null;
            string productsSupplier = null;
            string productsSupplierDate = null;
            if (buwzhi == "00")
            {
                OrderNo = id.Substring(4, 10);
                productsSupplier = id.Substring(14, 7);
                productsSupplierDate = "20" + id.Substring(21, 6);
            }
            else {
                OrderNo = id.Substring(0, 10);
                productsSupplier = id.Substring(10, 7);
                productsSupplierDate = "20" + id.Substring(17, 6);
            }

           
            //查询物料
            var listplaneGoods= await _zjnPlaneGoodsRepository.GetFirstAsync(x=> x.GoodsCode== OrderNo);
            //查询供应商
            var productsSupplierList= await _zjnBaseSupplierRepository.GetFirstAsync(x=> x.SupplierNo== productsSupplier);
            if (listplaneGoods==null|| productsSupplierList==null)
            {                
                throw HSZException.Oh("32位流水码有误");
            }
            ZjnBillsHistoryInfoOutput zjnBillsHistoryInfo = new ZjnBillsHistoryInfoOutput();
            zjnBillsHistoryInfo.orderNo = id;
            zjnBillsHistoryInfo.orderType = "001";
            zjnBillsHistoryInfo.productsBach = id;
            zjnBillsHistoryInfo.productsName = listplaneGoods.GoodsName;
            zjnBillsHistoryInfo.productsNo = listplaneGoods.GoodsCode;
            zjnBillsHistoryInfo.productsStyle = listplaneGoods.Specifications;
            zjnBillsHistoryInfo.productsSupplier = productsSupplierList.SupplierNo;
            zjnBillsHistoryInfo.productsType = listplaneGoods.GoodsType.ToString();
            zjnBillsHistoryInfo.productsUnit = listplaneGoods.Unit.ToString();
            zjnBillsHistoryInfo.ExpiryDate = DateTime.ParseExact(productsSupplierDate, "yyyyMMdd", CultureInfo.CurrentCulture).AddDays(Convert.ToDouble(listplaneGoods.ShelfLife));
            zjnBillsHistoryInfo.TheDateOfProduction = DateTime.ParseExact(productsSupplierDate, "yyyyMMdd",CultureInfo.CurrentCulture);
            //zjnBillsHistoryInfo.TestType = listplaneGoods.CheckType.ToString();
            return zjnBillsHistoryInfo;
        }

        /// <summary>
        /// 根据类型获取数据
        /// </summary>55
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("GetHistoryDataByType")]
        public async Task<dynamic> GetHistoryDataByType(ZjnBillsHistoryListQueryInput input)
        {
            string code = input.F_ProductsBach;
            if (code.Length == 10)
            {
                //根据物料号  查找入库明细表，过滤同批次同货位数据
                var data2 = await _zjnBillsHistoryRepository.AsSugarClient().Queryable<ZjnBillsHistoryEntity>()
                .Where(a => a.ProductsNo == code && a.IsDelete == 0 && a.ProductsTotal > 0 && a.InspectionStatus == 0)
                .WhereIF(!string.IsNullOrEmpty(input.F_OrderType), a => a.OrderType.Contains(input.F_OrderType))
                .GroupBy(a => new { a.ProductsBach, a.ProductsLocation })
                .Select(a => new ZjnBillsHistoryListOutput
                {
                    F_ProductsBach = a.ProductsBach,
                    F_ProductsTotal = SqlFunc.AggregateSum(a.ProductsTotal),
                    F_ProductsLocation = a.ProductsLocation
                }).ToListAsync();
                return null;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 出库分析32位流水码
        /// </summary>55
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("GeAnalysisOfTheChuKu")]
        public async Task<dynamic> GeAnalysisOfTheChuKu(string id)
        {
            var buwzhi = id.Substring(0, 2);
            string OrderNo = null;
            string productsSupplier = null;
            string productsSupplierDate = null;
            if (buwzhi == "00")
            {
                OrderNo = id.Substring(4, 10);
                productsSupplier = id.Substring(14, 7);
                productsSupplierDate = "20" + id.Substring(21, 6);
            }
            else
            {
                OrderNo = id.Substring(0, 10);
                productsSupplier = id.Substring(10, 7);
                productsSupplierDate = "20" + id.Substring(17, 6);
            }
            // var BillsHistoryRepository =await _zjnBillsHistoryRepository.AsSugarClient().Queryable<ZjnBillsHistoryEntity>().Where(x => x.OrderNo == id);
            //查询物料
            var listplaneGoods = await _zjnPlaneGoodsRepository.GetFirstAsync(x => x.GoodsCode == OrderNo);
            //查询供应商
            var productsSupplierList = await _zjnBaseSupplierRepository.GetFirstAsync(x => x.SupplierNo == productsSupplier);
            if (listplaneGoods == null || productsSupplierList == null)
            {

                throw HSZException.Oh("32位流水码有误");
            }
            
            var BillsHistoryRepository=(await _zjnBillsHistoryRepository.GetFirstAsync(p => p.OrderNo == id&& p.IsDelete==0)).Adapt<ZjnBillsHistoryInfoOutput>();
            if (BillsHistoryRepository == null) {
                throw HSZException.Oh("32位流水码有误");
            }
            var inventoryEntityList = _zjnPlaneMaterialInventoryRepository.AsSugarClient().Queryable<ZjnPlaneMaterialInventoryEntity>()
                .Where(x => x.ProductsCode == BillsHistoryRepository.productsNo && x.ProductsBatch == BillsHistoryRepository.orderNo && x.ProductsLocation == BillsHistoryRepository.productsLocation
                 && x.ProductsState == "1").ToList();
            ZjnBillsHistoryInfoOutput zjnBillsHistoryInfo = new ZjnBillsHistoryInfoOutput();
            zjnBillsHistoryInfo = BillsHistoryRepository;

            zjnBillsHistoryInfo.orderNo = id;
            zjnBillsHistoryInfo.orderType = "C001";
            zjnBillsHistoryInfo.productsBach = id;
            zjnBillsHistoryInfo.productsName = listplaneGoods.GoodsName;
            zjnBillsHistoryInfo.productsNo = listplaneGoods.GoodsCode;
            zjnBillsHistoryInfo.productsStyle = listplaneGoods.Specifications;
            zjnBillsHistoryInfo.productsSupplier = productsSupplierList.SupplierNo;
            zjnBillsHistoryInfo.productsType = listplaneGoods.GoodsType.ToString();
            zjnBillsHistoryInfo.productsUnit = listplaneGoods.Unit.ToString();
            zjnBillsHistoryInfo.TheDateOfProduction = DateTime.ParseExact(productsSupplierDate, "yyyyMMdd", CultureInfo.CurrentCulture);
            zjnBillsHistoryInfo.PurchaseOrder = BillsHistoryRepository.PurchaseOrder;
            zjnBillsHistoryInfo.id = null;
            zjnBillsHistoryInfo.TestType = listplaneGoods.CheckType.ToString();
            zjnBillsHistoryInfo.productsTotal =Convert.ToDecimal( Convert.ToDecimal(inventoryEntityList.Sum(x=> x.ProductsQuantity)).ToString());

            return zjnBillsHistoryInfo;
        }

        #region 报表
        /// <summary>
        /// 库存明细
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("TheReportDetailsList")]
        public async Task<dynamic> TheReportDetailsList([FromQuery] ZjnBillsHistoryListQueryInput input)
        {
            List<string> list = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "337742110771905797").Select(s => s.EnCode).ToList();
            var data = await _zjnBillsHistoryRepository.AsSugarClient().Queryable<ZjnBillsHistoryEntity>()
                  .InnerJoin<ZjnPlaneMaterialInventoryEntity>((a, b) => a.ProductsNo == b.ProductsCode&&a.OrderNo==b.ProductsBatch&&a.ProductsLocation==b.ProductsLocation)                
                  .Where((a,b) => a.IsDelete == 0 && list.Contains(a.OrderType.ToString())&& b.ProductsQuantity>0)
                  //.Where(!string.IsNullOrEmpty(input.F_OrderType), a => a.OrderType.Contains(input.F_OrderType))
                  .WhereIF(!string.IsNullOrEmpty(input.F_OrderNo), a => a.OrderNo.Contains(input.F_OrderNo))
                  .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsName.Contains(input.F_ProductsName))
                  .WhereIF(!string.IsNullOrEmpty(input.F_ProductsLocation), a => a.ProductsLocation.Contains(input.F_ProductsLocation))
                  .GroupBy((a,b) => new { a.OrderNo, a.ProductsNo, a.ProductsName, a.ProductsUnit,a.ProductsBach,a.ProductsLocation,
                           a.TheContainer,a.InspectionStatus,a.TheDateOfProduction,b.ProductsState, a.TheTray, a.PurchaseOrder , b.ProductsQuantity,b.Case1
                  })
                  .Select((a,b) => new ZjnBillsHistoryListOutput
                  {
                      F_OrderNo=a.OrderNo,
                      F_ProductsNo=a.ProductsNo,
                      F_ProductsName = a.ProductsName,                     
                      F_ProductsTotal =Convert.ToDecimal(Convert.ToDecimal(SqlFunc.AggregateSum(a.ProductsTotal)- SqlFunc.AggregateSum(Convert.ToDecimal(a.BatchDeliveryQuantity))).ToString()),//a.ProductsTotal,
                     
                      F_ProductsUnit = a.ProductsUnit,
                      F_ProductsBach = a.ProductsBach,
                      F_ProductsLocation = a.ProductsLocation,
                      F_TheContainer=a.TheContainer,
                      F_InspectionStatus=a.InspectionStatus,
                      F_TheDateOfProduction=a.TheDateOfProduction,
                    
                      F_ProductsStyle = b.ProductsQuantity.ToString(),
                      //F_CreateTime=a.CreateTime,
                      F_ProductsUser= b.Case1,
                      F_TheTray = a.TheTray,
                      F_PurchaseOrder=a.PurchaseOrder,
                      F_BatchDeliveryQuantity = SqlFunc.AggregateSum(Convert.ToDecimal(a.BatchDeliveryQuantity)).ToString(),//a.BatchDeliveryQuantity,
                  })
                  .ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<ZjnBillsHistoryListOutput>.SqlSugarPageResult(data);
        }
        /// <summary>
        /// 实时库存
        /// </summary>
        /// <returns></returns>
        [HttpGet("JustinTimeInventory")]
        public async Task<dynamic> JustinTimeInventory([FromQuery] ZjnPlaneMaterialInventoryListQueryInput input) {
            List<string> list = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "337742110771905797").Select(s => s.EnCode).ToList();
            var data = await _zjnPlaneMaterialInventoryRepository.AsSugarClient().Queryable<ZjnPlaneMaterialInventoryEntity>()
                .LeftJoin<ZjnBillsHistoryEntity>((a, b) => a.ProductsBatch == b.OrderNo && a.ProductsLocation == b.ProductsLocation && b.InspectionStatus == 1 && list.Contains(b.OrderType))//不合格
                .LeftJoin<ZjnBillsHistoryEntity>((a, b, c) => a.ProductsBatch == c.OrderNo && a.ProductsLocation == c.ProductsLocation && c.InspectionStatus == 0 && list.Contains(c.OrderType))//合格
                .LeftJoin<ZjnBillsHistoryEntity>((a, b, c, e) => a.ProductsBatch == e.OrderNo && a.ProductsLocation == e.ProductsLocation && e.InspectionStatus == 2 && list.Contains(e.OrderType))//待检验
                .LeftJoin<ZjnBillsHistoryEntity>((a, b, c, e, s) => a.ProductsBatch == s.OrderNo && a.ProductsLocation == s.ProductsLocation && s.InspectionStatus == 3 && list.Contains(s.OrderType))//冻结
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsCode), a => a.ProductsCode.Contains(input.F_ProductsCode))                
                .WhereIF(!string.IsNullOrEmpty(input.F_TheContainer), a => a.ProductsBatch.Contains(input.F_TheContainer))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsName.Contains(input.F_ProductsName))
                .GroupBy((a, b, c, e, s) => new { a.ProductsCode, a.ProductsName, a.ProductsBatch, a.ProductsState, a.ProductsUnit  })
                .Select((a, b, c, e, s) => new ZjnPlaneMaterialInventoryInfoOutput
                  {
                    productsGrade = Convert.ToDouble(SqlFunc.AggregateSum(b.ProductsTotal??0) - SqlFunc.AggregateSum(Convert.ToDecimal(b.BatchDeliveryQuantity ?? "0"))).ToString(),//不合格
                    JyanHege = Convert.ToDouble(SqlFunc.AggregateSum(c.ProductsTotal??0) - SqlFunc.AggregateSum(Convert.ToDecimal(c.BatchDeliveryQuantity ?? "0"))).ToString(),//合格
                    dongjieState = Convert.ToDouble(SqlFunc.AggregateSum(e.ProductsTotal??0)- SqlFunc.AggregateSum(Convert.ToDecimal(e.BatchDeliveryQuantity ?? "0"))).ToString() ,//待检验
                    dongjieStates = Convert.ToDouble(SqlFunc.AggregateSum(s.ProductsTotal??0) - SqlFunc.AggregateSum(Convert.ToDecimal(s.BatchDeliveryQuantity ?? "0"))).ToString(),//冻结
                    productsCode =a.ProductsCode,
                     productsName=a.ProductsName,
                     productsBatch=a.ProductsBatch,
                    productsTakeCount = Convert.ToInt32(a.ProductsState),
                     productsIsLock =Convert.ToInt32(a.ProductsUnit),
                     productsQuantity = Convert.ToDouble((SqlFunc.AggregateSum(b.ProductsTotal??0)- SqlFunc.AggregateSum(Convert.ToDecimal(b.BatchDeliveryQuantity??"0")))+
                                       (SqlFunc.AggregateSum(c.ProductsTotal??0) - SqlFunc.AggregateSum(Convert.ToDecimal(c.BatchDeliveryQuantity ?? "0")))+
                                       (SqlFunc.AggregateSum(e.ProductsTotal??0) - SqlFunc.AggregateSum(Convert.ToDecimal(e.BatchDeliveryQuantity ?? "0"))) +
                                       (SqlFunc.AggregateSum(s.ProductsTotal??0) - SqlFunc.AggregateSum(Convert.ToDecimal(s.BatchDeliveryQuantity ?? "0")))).ToString()

                }).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<ZjnPlaneMaterialInventoryInfoOutput>.SqlSugarPageResult(data);

        }

       



        /// <summary>
        /// 实时库存导出
        /// </summary>
        /// <returns></returns>

        public async Task<dynamic> JustinTimeInventory11([FromQuery] ZjnPlaneMaterialInventoryListQueryInput input)
        {
            List<string> list = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "337742110771905797").Select(s => s.EnCode).ToList();
            var data = await _zjnPlaneMaterialInventoryRepository.AsSugarClient().Queryable<ZjnPlaneMaterialInventoryEntity>()
                .LeftJoin<ZjnBillsHistoryEntity>((a, b) => a.ProductsBatch == b.OrderNo && a.ProductsLocation == b.ProductsLocation && b.InspectionStatus == 1 && list.Contains(b.OrderType))//不合格
                .LeftJoin<ZjnBillsHistoryEntity>((a, b, c) => a.ProductsBatch == c.OrderNo && a.ProductsLocation == c.ProductsLocation && c.InspectionStatus == 0 && list.Contains(c.OrderType))//合格
                .LeftJoin<ZjnBillsHistoryEntity>((a, b, c, e) => a.ProductsBatch == e.OrderNo && a.ProductsLocation == e.ProductsLocation && e.InspectionStatus == 2 && list.Contains(e.OrderType))//待检验
                .LeftJoin<ZjnBillsHistoryEntity>((a, b, c, e, s) => a.ProductsBatch == s.OrderNo && a.ProductsLocation == s.ProductsLocation && s.InspectionStatus == 3 && list.Contains(s.OrderType))//冻结
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsCode), a => a.ProductsName.Contains(input.F_ProductsCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_TheContainer), a => a.ProductsBatch.Contains(input.F_TheContainer))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsLocation.Contains(input.F_ProductsName))
                .GroupBy((a, b, c, e, s) => new { a.ProductsCode, a.ProductsName, a.ProductsBatch, a.ProductsState, a.ProductsUnit, a.ProductsQuantity })
                .Select((a, b, c, e, s) => new ZjnPlaneMaterialInventoryInfoOutput
                {
                    productsGrade = SqlFunc.AggregateSum(b.ProductsTotal).ToString() ?? "0",//不合格
                    JyanHege = SqlFunc.AggregateSum(c.ProductsTotal).ToString() ?? "0",//合格
                    dongjieState = SqlFunc.AggregateSum(e.ProductsTotal).ToString() ?? "0",//待检验
                    dongjieStates = SqlFunc.AggregateSum(s.ProductsTotal).ToString() ?? "0",//冻结
                    productsCode = a.ProductsCode,
                    productsName = a.ProductsName,
                    productsBatch = a.ProductsBatch,
                    productsTakeCount = Convert.ToInt32(a.ProductsState),
                    productsIsLock = Convert.ToInt32(a.ProductsUnit),
                    productsQuantity = SqlFunc.AggregateSum(Convert.ToDecimal(b.BatchDeliveryQuantity ?? "0")).ToString(),
                    //((SqlFunc.AggregateSum(b.ProductsTotal ?? 0) - SqlFunc.AggregateSum(Convert.ToDecimal(b.BatchDeliveryQuantity ?? "0"))) +
                    //                   (SqlFunc.AggregateSum(c.ProductsTotal ?? 0) - SqlFunc.AggregateSum(Convert.ToDecimal(c.BatchDeliveryQuantity ?? "0"))) +
                    //                   (SqlFunc.AggregateSum(e.ProductsTotal ?? 0) - SqlFunc.AggregateSum(Convert.ToDecimal(e.BatchDeliveryQuantity ?? "0"))) +
                    //                   (SqlFunc.AggregateSum(s.ProductsTotal ?? 0) - SqlFunc.AggregateSum(Convert.ToDecimal(s.BatchDeliveryQuantity ?? "0")))).ToString()

                }).ToListAsync();


            return data;
        }
        /// <summary>
        /// 全部导出
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<dynamic> TheReportDetailsListdynamic([FromQuery] ZjnBillsHistoryListQueryInput input)
        {

            List<string> list = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "337742110771905797").Select(s => s.EnCode).ToList();
            var data = await _zjnBillsHistoryRepository.AsSugarClient().Queryable<ZjnBillsHistoryEntity>()
                  .LeftJoin<ZjnPlaneMaterialInventoryEntity>((a, b) => a.ProductsNo == b.ProductsCode && a.ProductsBach == b.ProductsBatch && a.ProductsLocation == b.ProductsLocation)
                  .Where((a, b) => a.IsDelete == 0 && list.Contains(a.OrderType.ToString()) && b.ProductsQuantity > 0)
                  //.Where(!string.IsNullOrEmpty(input.F_OrderType), a => a.OrderType.Contains(input.F_OrderType))
                  .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsName.Contains(input.F_ProductsName))
                  .WhereIF(!string.IsNullOrEmpty(input.F_ProductsLocation), a => a.ProductsLocation.Contains(input.F_ProductsLocation))
                  .GroupBy((a, b) => new {
                      a.OrderNo,
                      a.OrderType,
                      a.ProductsNo,
                      a.ProductsName,
                      a.ProductsUnit,
                      a.ProductsBach,
                      a.ProductsLocation,
                      a.TheContainer,
                      a.InspectionStatus,
                      a.TheDateOfProduction,
                      b.ProductsState,
                      a.TheTray,
                      a.PurchaseOrder,
                      b.ProductsQuantity,
                      b.Case1
                  }).Select((a, b) => new ZjnBillsHistoryListOutput
                  {
                      F_OrderNo = a.OrderNo,
                      F_ProductsNo = a.ProductsNo,
                      F_ProductsName = a.ProductsName,
                      F_ProductsTotal = SqlFunc.AggregateSum(a.ProductsTotal) - SqlFunc.AggregateSum(Convert.ToDecimal(a.BatchDeliveryQuantity)),//a.ProductsTotal,
                      F_ProductsUnit = a.ProductsUnit,
                      F_ProductsBach = a.ProductsBach,
                      F_ProductsLocation = a.ProductsLocation,
                      F_TheContainer = a.TheContainer,
                      F_InspectionStatus = a.InspectionStatus,
                      F_TheDateOfProduction = a.TheDateOfProduction,
                      F_TestType = Convert.ToInt32(b.ProductsState),
                      F_ProductsStyle = b.ProductsQuantity.ToString(),
                      //F_CreateTime=a.CreateTime,
                      F_ProductsUser = b.Case1,
                      F_TheTray = a.TheTray,
                      F_PurchaseOrder = a.PurchaseOrder,
                      F_BatchDeliveryQuantity = SqlFunc.AggregateSum(Convert.ToDecimal(a.BatchDeliveryQuantity)).ToString(),//a.BatchDeliveryQuantity,
                  }).ToListAsync();
            return data;
        }


        /// <summary>
        /// 导出库存明细信息
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] ZjnBillsHistoryListQueryInput input)
        {
            
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<ZjnBillsHistoryListOutput>();
            var ssd =new  List<ZjnBillsHistoryListQueryInput>();
            if (input.dataType == 0)
            {
                //分页导出
                var data = Clay.Object(await this.TheReportDetailsList(input));
                exportData = data.Solidify<PageResult<ZjnBillsHistoryListOutput>>().list;
            }
            else
            {
                //全部导出
                 exportData = await this.TheReportDetailsListdynamic(input);
            }
           
            List<ZjnBillsHistoryListQueryInput> list = new List<ZjnBillsHistoryListQueryInput>();
            foreach (var item in exportData)
            {
                var typeStatus = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "325448475967751429" && s.EnCode == item.F_InspectionStatus.ToString()).Select(s => s.FullName).ToList();
                var TraytypeName = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "326590282281780485" && s.EnCode == item.F_TestType.ToString()).Select(s => s.FullName).ToList();
                var ProductsTotal = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "326384591566800133" && s.EnCode == item.F_ProductsUnit.ToString()).Select(s => s.FullName).ToList();
                ZjnBillsHistoryListQueryInput planeTrayListOutput = new ZjnBillsHistoryListQueryInput();
                planeTrayListOutput.F_OrderNo = item.F_OrderNo;
                planeTrayListOutput.F_OrderType = item.F_OrderType;
                planeTrayListOutput.F_ProductsNo = item.F_ProductsNo;
                planeTrayListOutput.F_ProductsName = item.F_ProductsName;
                planeTrayListOutput.F_ProductsType = item.F_ProductsType;
                planeTrayListOutput.F_ProductsStyle = item.F_ProductsStyle;
                planeTrayListOutput.F_BatchDeliveryQuantity = item.F_BatchDeliveryQuantity;
                planeTrayListOutput.F_ProductsUnit = ProductsTotal[0];
                planeTrayListOutput.F_ProductsLocation = item.F_ProductsLocation;
                planeTrayListOutput.F_TheContainer = item.F_TheContainer;
                planeTrayListOutput.F_InspectionStatus = typeStatus[0];
                planeTrayListOutput.F_TheDateOfProduction = item.F_TheDateOfProduction;
                planeTrayListOutput.TestTypeName = TraytypeName[0];
                planeTrayListOutput.F_CreateTime = item.F_CreateTime;
                planeTrayListOutput.F_ProductsUser = item.F_ProductsUser;
                //planeTrayListOutput.F_ProductsUser = Products;
                planeTrayListOutput.F_TheTray = item.F_TheTray;
                planeTrayListOutput.F_PurchaseOrder = item.F_PurchaseOrder;
                list.Add(planeTrayListOutput);
            }
            List<ParamsModel> paramList = "[{\"value\":\"单据号\",\"field\":\"F_PurchaseOrder\"},{\"value\":\"32位流水号\",\"field\":\"F_OrderNo\"},{\"value\":\"物料编号\",\"field\":\"F_ProductsNo\"},{\"value\":\"物料名称\",\"field\":\"F_ProductsName\"},{\"value\":\"物料货位置\",\"field\":\"F_ProductsLocation\"},{\"value\":\"剩余数量\",\"field\":\"F_ProductsStyle\"},{\"value\":\"出库数量\",\"field\":\"F_BatchDeliveryQuantity\"},{\"value\":\"物料单位\",\"field\":\"F_ProductsUnit\"},{\"value\":\"物料状态\",\"field\":\"F_InspectionStatus\"},{\"value\":\"生产时间\",\"field\":\"F_TheDateOfProduction\"},{\"value\":\"容器编号\",\"field\":\"F_TheContainer\"},{\"value\":\"托盘编号\",\"field\":\"F_TheTray\"},{\"value\":\"入库时间\",\"field\":\"F_CreateTime\"},{\"value\":\"物料货位置\",\"field\":\"F_ProductsLocation\"},{\"value\":\"物料货位中文名称\",\"field\":\"F_ProductsLocation\"},{\"value\":\"备注\",\"field\":\"F_ProductsUser\"}]".ToList<ParamsModel>(); ;
          
            //"[{\"value\":\"单据号\",\"field\":\"F_PurchaseOrder\"},{\"value\":\"32位流水号\",\"field\":\"F_OrderNo\"},{\"value\":\"物料编号\",\"field\":\"F_ProductsNo\"},{\"value\":\"物料名称\",\"field\":\"F_ProductsName\"},{\"value\":\"是否冻结\",\"field\":\"TestTypeName\"},{\"value\":\"物料状态\",\"field\":\"F_InspectionStatus\"},{\"value\":\"生产时间\",\"field\":\"F_TheDateOfProduction\"},{\"value\":\"物料数量\",\"field\":\"F_ProductsTotal\"},{\"value\":\"物料单位\",\"field\":\"F_ProductsUnit\"},{\"value\":\"容器编号\",\"field\":\"F_TheContainer\"},{\"value\":\"托盘编号\",\"field\":\"F_TheTray\"},{\"value\":\"入库时间\",\"field\":\"F_CreateTime\"},{\"value\":\"物料货位置\",\"field\":\"F_ProductsLocation\"},{\"value\":\"物料货位中文名称\",\"field\":\"F_ProductsLocation\"},{\"value\":\"备注\",\"field\":\"F_ProductsUser\"}]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "库存明细报表导出.xls";
            excelconfig.HeadFont = "微软雅黑";
            excelconfig.HeadPoint = 10;
            excelconfig.IsAllSizeColumn = true;
            excelconfig.ColumnModel = new List<ExcelColumnModel>();
            List<string> selectKeyList = input.selectKey.Split(',').ToList();
            //循环选择导出的表头
            foreach (var item in selectKeyList)
            {
                var isExist = paramList.Find(p => p.field == item);
                if (isExist != null)
                {
                    excelconfig.ColumnModel.Add(new ExcelColumnModel() { Column = isExist.field, ExcelColumn = isExist.value });
                }
            }
            //路径
            var addPath = FileVariable.TemporaryFilePath + excelconfig.FileName;
            ExcelExportHelper<ZjnBillsHistoryListQueryInput>.Export(list, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }
        /// <summary>
        /// 导出实时库存信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("Actions/ExportSzai")]
        public async Task<dynamic> ExportSzai([FromQuery] ZjnPlaneMaterialInventoryListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<ZjnPlaneMaterialInventoryInfoOutput>();
           
            if (input.dataType == 0)
            {
                //分页导出
               var data = Clay.Object(await this.JustinTimeInventory(input));
                exportData = data.Solidify<PageResult<ZjnPlaneMaterialInventoryInfoOutput>>().list;

            }
            else
            {
                //全部导出
                exportData = await this.JustinTimeInventory11(input);
            }

            List<ZjnPlaneMaterialInventoryInfoOutput> list = new List<ZjnPlaneMaterialInventoryInfoOutput>();
            foreach (var item in exportData)
            {
                string productsUnit = item.productsUnit == null ? "999" : item.productsUnit.ToString();
                string productsBatch = item.productsBatch == null ? "无" : item.productsBatch.ToString();
                var typeStatus = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "326590282281780485" && s.EnCode == item.productsTakeCount.ToString()).Select(s => s.FullName).ToList();               
                var ProductsTotal = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "326384591566800133" && s.EnCode == productsUnit).Select(s => s.FullName).ToList();
                ZjnPlaneMaterialInventoryInfoOutput planeTrayListOutput = new ZjnPlaneMaterialInventoryInfoOutput();
                planeTrayListOutput.productsCode = item.productsCode;
                planeTrayListOutput.productsName = item.productsName;
                planeTrayListOutput.productsBatch = productsBatch;
                planeTrayListOutput.productsState = typeStatus[0];
                planeTrayListOutput.productsUnit = ProductsTotal[0];
                planeTrayListOutput.productsQuantity = item.productsQuantity;
                planeTrayListOutput.productsGrade = item.productsGrade;
                planeTrayListOutput.JyanHege = item.JyanHege;
                planeTrayListOutput.dongjieState = item.dongjieState;
                planeTrayListOutput.dongjieStates = item.dongjieStates;
                list.Add(planeTrayListOutput);
            }
            List<ParamsModel> paramList = "[{\"value\":\"物料编号\",\"field\":\"productsCode\"},{\"value\":\"物料名称\",\"field\":\"productsName\"},{\"value\":\"物料批次\",\"field\":\"productsBatch\"},{\"value\":\"物料状态\",\"field\":\"productsState\"},{\"value\":\"物料单位\",\"field\":\"productsIsLock\"},{\"value\":\"当前库存数量\",\"field\":\"productsQuantity\"},{\"value\":\"不合格汇总\",\"field\":\"productsGrade\"},{\"value\":\"合格汇总\",\"field\":\"JyanHege\"},{\"value\":\"待检汇总\",\"field\":\"dongjieState\"},{\"value\":\"冻结汇总\",\"field\":\"dongjieStates\"}]".ToList<ParamsModel>();
            //"[{\"value\":\"物料编号\",\"field\":\"productsCode\"},{\"value\":\"物料名称\",\"field\":\"productsName\"},{\"value\":\"物料状态\",\"field\":\"productsState\"},{\"value\":\"物料单位\",\"field\":\"productsUnit\"},{\"value\":\"库存总数\",\"field\":\"productsQuantity\"},{\"value\":\"库内托数\",\"field\":\"purchaseOrder7\"},{\"value\":\"库外总数\",\"field\":\"purchaseOrder6\"},{\"value\":\"库外托数\",\"field\":\"purchaseOrder5\"},{\"value\":\"正在移动总数\",\"field\":\"purchaseOrder4\"},{\"value\":\"正在移动托数\",\"field\":\"purchaseOrder3\"},{\"value\":\"全部总数\",\"field\":\"productsSupplier2\"},{\"value\":\"全部托数\",\"field\":\"purchaseOrder1\"},{\"value\":\"工厂\",\"field\":\"gongc\"}]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "库存明细报表导出.xls";
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
            ExcelExportHelper<ZjnPlaneMaterialInventoryInfoOutput>.Export(list, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;

        }
        /// <summary>
        /// 预警报表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("WarningStatements")]
        public async Task<dynamic> WarningStatements([FromQuery] ZjnPlaneGoodsListQueryInput input) {
            
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnPlaneGoodsRepository.AsSugarClient().Queryable<ZjnPlaneMaterialInventoryEntity>()
                .InnerJoin<ZjnPlaneGoodsEntity>((a, b) => a.ProductsCode == b.GoodsCode)
                .InnerJoin<ZjnBillsHistoryEntity>((a, b, c) => a.ProductsBatch == c.OrderNo &&  a.ProductsLocation==c.ProductsLocation&& c.ProductsNo==a.ProductsCode)
                .Where((a, b, c) => b.IsDelete == 0)
                .WhereIF(!string.IsNullOrEmpty(input.F_GoodsCode), a => a.ProductsCode.Contains(input.F_GoodsCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_GoodsName), a => a.ProductsName.Contains(input.F_GoodsName))
               // .WhereIF(!string.IsNullOrEmpty(input.F_GoodsType), b => b..Equals(input.F_GoodsType))
               .GroupBy((a, b, c) => new
               {
                   b.GoodsCode,
                   b.GoodsName,
                   b.GoodsType,
                   c.ProductsSupplier,
                   b.Unit,
                   b.TellDate,
                   b.DisableMark,
                   b.Ceiling,
                   b.TheLowerLimit,
                   //Convert.ToDateTime(c.CreateTime),                  
                   a.ProductsBatch,
                   c.ExpiryDate,
                   c.TheDateOfProduction,
               })
                .Select((a, b, c) => new ZjnPlaneGoodsListOutput
                {
                    F_GoodsCode = b.GoodsCode,
                    F_GoodsName = b.GoodsName,
                    F_Unit = b.Unit,
                    CreateUser = a.ProductsBatch,//批次
                    F_GoodsType = b.GoodsType,
                    ProductsQuantity =(SqlFunc.AggregateSum(c.ProductsTotal) - SqlFunc.AggregateSum(Convert.ToDecimal(c.BatchDeliveryQuantity))).ToString(),
                    F_VendorId = c.ProductsSupplier,
                    F_TellDate = b.TellDate,
                    F_DisableMark = b.DisableMark,
                    F_Ceiling = b.Ceiling,
                    F_TheLowerLimit = b.TheLowerLimit,
                    // CreateTime = c.CreateTime,
                    ExpiryDate = c.ExpiryDate,
                    TheDateOfProduction = c.TheDateOfProduction,
                    F_Specifications = SqlFunc.DateAdd(c.ExpiryDate, -SqlFunc.SqlServer_DateDiff("MM", c.ExpiryDate, c.TheDateOfProduction.AddDays(1)) - Convert.ToInt32(b.TellDate)).ToString("yyyy-MM-dd hh:mm:ss"),//Convert.ToString(c.TheDateOfProduction.AddDays(-Convert.ToDouble((c.ExpiryDate-c.TheDateOfProduction).Days- a.TellDate))),
                }).ToPagedListAsync(input.currentPage, input.pageSize);

            var dataObject = Clay.Object(data);
            List<ZjnPlaneGoodsListOutput> exportData = dataObject.Solidify<PageResult<ZjnPlaneGoodsListOutput>>().list;
            SqlSugarPagedList<ZjnPlaneGoodsListOutput> list = new SqlSugarPagedList<ZjnPlaneGoodsListOutput>();
            List<ZjnPlaneGoodsListOutput> ZjnPlaneGoodsList = new List<ZjnPlaneGoodsListOutput>();
            foreach (var item in exportData)
            {
                if (Convert.ToDateTime(item.F_Specifications)<=DateTime.Now)
                {
                    ZjnPlaneGoodsListOutput zjnPlaneGoods = new ZjnPlaneGoodsListOutput();
                    zjnPlaneGoods = item;
                    ZjnPlaneGoodsList.Add(zjnPlaneGoods);
                }
            }
            list.list = ZjnPlaneGoodsList;
            list.pagination= data.pagination;           
            return PageResult<ZjnPlaneGoodsListOutput>.SqlSugarPageResult(list);
        }
        /// <summary>
        /// 库存上下线
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("UpperAndLowerLimitOfInventory")]
        public async Task<dynamic> UpperAndLowerLimitOfInventory([FromQuery] ZjnPlaneGoodsListQueryInput input) {

            var Products = "";
            if (input.F_OrderType == "001")
            {
                Products = "原材料库";
            }
            else
            {
                Products = "结构件库";
            }
            var sidx = input.sidx == "a.Id";
            var data = await _zjnPlaneGoodsRepository.AsSugarClient().Queryable<ZjnPlaneGoodsEntity>()
                .InnerJoin<ZjnPlaneMaterialInventoryEntity>((a, b) => a.GoodsCode == b.ProductsCode)
                .WhereIF(!string.IsNullOrEmpty(input.F_GoodsCode), a => a.GoodsCode.Contains(input.F_GoodsCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_GoodsName), a => a.GoodsName.Contains(input.F_GoodsName))
                .WhereIF(!string.IsNullOrEmpty(input.F_GoodsType), a => a.GoodsType.Equals(input.F_GoodsType))
                .Select((a) => new ZjnPlaneGoodsListOutput
                {
                    F_GoodsCode = a.GoodsCode,
                    F_GoodsName = a.GoodsName,
                    F_Unit = a.Unit,
                    F_Ceiling=a.Ceiling,
                    F_TheLowerLimit=a.TheLowerLimit,
                    F_DisableMark= Products,
                    IsFirstOutName= input.F_OrderType
                }).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<ZjnPlaneGoodsListOutput>.SqlSugarPageResult(data);

        }

        /// <summary>
        /// 单据流水报表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("WaterDocuments")]
        public async Task<dynamic> WaterDocuments([FromQuery] ZjnBillsHistoryListQueryInput input) {
            //选择库名称
            var Products = "";
            if (input.F_OrderType == "001")
            {
                Products = "原材料库";
            }
            else
            {
                Products = "结构件库";
            }
            var data = await _zjnBillsHistoryRepository.AsSugarClient().Queryable<ZjnBillsHistoryEntity>()
                    .Where(a => a.IsDelete == 0)
                     .WhereIF(!string.IsNullOrEmpty(input.F_ProductsBach), a => a.ProductsBach.Contains(input.F_ProductsBach))
                    .WhereIF(!string.IsNullOrEmpty(input.F_ProductsNo), a => a.ProductsNo.Contains(input.F_ProductsNo))
                    .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsName.Contains(input.F_ProductsName))
                    .WhereIF(!string.IsNullOrEmpty(input.F_ProductsLocation), a => a.ProductsLocation.Contains(input.F_ProductsLocation))
                    .Select((a) => new ZjnBillsHistoryListOutput
                    {
                        F_Id=a.Id,
                        F_OrderNo = a.OrderNo,
                        F_OrderType = a.OrderType,
                        F_ProductsNo = a.ProductsNo,
                        F_ProductsName = a.ProductsName,
                        F_ProductsType = a.ProductsType,
                        F_ProductsStyle = a.ProductsStyle,
                        F_ProductsTotal = a.ProductsTotal,
                        F_ProductsUnit = a.ProductsUnit,
                        F_ProductsGrade = a.ProductsGrade,
                        F_ProductsBach = a.ProductsBach,
                        F_ProductsLocation = a.ProductsLocation,
                        F_TheContainer = a.TheContainer,
                        F_InspectionStatus = a.InspectionStatus,
                        F_TheDateOfProduction = a.TheDateOfProduction,                        
                        F_TheTray = a.TheTray,
                        F_PurchaseOrder = a.PurchaseOrder,
                        F_CreateTime=a.CreateTime,
                        F_LastModifyUserId= Products
                    }).OrderBy(" F_Id  desc ").ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<ZjnBillsHistoryListOutput>.SqlSugarPageResult(data);

        }

        [HttpGet("GetAccordingToSerialNumber")]
        public async Task<dynamic> GetAccordingToSerialNumber(string id,string ProductsLocation,string ProductsCode) {

            if (id==null)
            {
                throw HSZException.Oh("流水号不能为空!");
            }
            ZjnPlaneMaterialInventoryEntity zjnPlane = new ZjnPlaneMaterialInventoryEntity();
           var zjnPlanelist = await _zjnPlaneMaterialInventoryRepository.GetFirstAsync(x => x.ProductsCode == ProductsCode && x.ProductsBatch == id && x.ProductsLocation == ProductsLocation
                 && x.ProductsState == "1");

            if (zjnPlanelist != null)
            {
                zjnPlane= zjnPlanelist;
            }

            return zjnPlane;
        }

        /// <summary>
        /// 单据流水报表导出
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[HttpGet("WaterDocumentsExportSzai")]
        //public async Task<dynamic> WaterDocumentsExportSzai([FromQuery] ZjnBillsHistoryListQueryInput input)
        //{


        //}


        #endregion

    }
}


