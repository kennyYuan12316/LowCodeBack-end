using HSZ.ChangeDataBase;
using HSZ.Common.Configuration;
using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Helper;
using HSZ.Common.Util;
using HSZ.DataEncryption;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.System.Entitys.System;
using HSZ.System.Interfaces.System;
using HSZ.ViewEngine;
using HSZ.VisualDev.Entitys;
using HSZ.VisualDev.Entitys.Dto.VisualDev;
using HSZ.VisualDev.Entitys.Model.CodeGen;
using HSZ.VisualDev.Entitys.Model.VisualDevModelData;
using HSZ.VisualDev.Run.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.VisualDev
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：可视化开发基础
    /// </summary>
    [ApiDescriptionSettings(Tag = "VisualDev", Name = "Generater", Order = 175)]
    [Route("api/visualdev/[controller]")]
    public class CodeGenService : IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<VisualDevEntity> _visualDevRepository;
        private readonly IViewEngine _viewEngine;
        private readonly IUserManager _userManager;
        private readonly IDbLinkService _dbLinkService;
        private readonly IChangeDataBase _databaseService;
        private readonly IRunService _runService;
        private readonly IDictionaryDataService _dictionaryDataService;
        private int active = 1;

        /// <summary>
        /// 初始化一个<see cref="CodeGenService"/>类型的新实例
        /// </summary>
        public CodeGenService(ISqlSugarRepository<VisualDevEntity> visualDevRepository, IChangeDataBase databaseService, IDbLinkService dbLinkService, IRunService runService, IViewEngine viewEngine, IUserManager userManager, IDictionaryDataService dictionaryDataService)
        {
            _visualDevRepository = visualDevRepository;
            _viewEngine = viewEngine;
            _userManager = userManager;
            _databaseService = databaseService;
            _dbLinkService = dbLinkService;
            _runService = runService;
            _dictionaryDataService = dictionaryDataService;
        }

        #region Get

        /// <summary>
        /// 获取命名空间
        /// </summary>
        [HttpGet("AreasName")]
        public dynamic GetAreasName()
        {
            List<string> areasName = new List<string>();
            if (KeyVariable.AreasName.Count > 0)
            {
                areasName = KeyVariable.AreasName;
            }
            return areasName;
        }

        #endregion

        #region Post

        /// <summary>
        /// 下载代码
        /// </summary>
        [HttpPost("{id}/Actions/DownloadCode")]
        public async Task<dynamic> DownloadCode(string id, [FromBody] DownloadCodeFormInput downloadCodeForm)
        {
            var userInfo = _userManager.GetUserInfo();
            var templateEntity = await _visualDevRepository.GetFirstAsync(v => v.Id == id && v.DeleteMark == null);
            _ = templateEntity ?? throw HSZException.Oh(ErrorCode.COM1005);
            _ = templateEntity.Tables ?? throw HSZException.Oh(ErrorCode.D2100);
            var model = TemplateKeywordsHelper.ReplaceKeywords(templateEntity.FormData).ToObject<FormDataModel>();
            if (templateEntity.Type == 3)
            {
                downloadCodeForm.module = "WorkFlowForm";
            }
            model.className = downloadCodeForm.className.ToPascalCase();
            model.areasName = downloadCodeForm.module;
            string fileName = templateEntity.FullName;
            //判断子表名称
            var childTb = new List<string>();
            if (!downloadCodeForm.subClassName.IsNullOrEmpty())
            {
                childTb = new List<string>(downloadCodeForm.subClassName.Split(','));
            }
            //子表名称去重
            HashSet<string> set = new HashSet<string>(childTb);
            templateEntity.FormData = model.ToJson();
            bool result = childTb.Count == set.Count ? true : false;
            if (!result)
            {
                throw HSZException.Oh(ErrorCode.D2101);
            }
            await this.TemplatesDataAggregation(fileName, templateEntity);
            string randPath = Path.Combine(FileVariable.GenerateCodePath, fileName);
            string downloadPath = randPath + ".zip";
            //判断是否存在同名称文件
            if (File.Exists(downloadPath))
            {
                File.Delete(downloadPath);
            }
            ZipFile.CreateFromDirectory(randPath, downloadPath);
            var downloadFileName = userInfo.Id + "|" + fileName + ".zip|codeGenerator";
            return new { name = fileName, url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(downloadFileName, "HSZ") };
        }

        /// <summary>
        /// 预览代码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="downloadCodeForm"></param>
        /// <returns></returns>
        [HttpPost("{id}/Actions/CodePreview")]
        public async Task<dynamic> CodePreview(string id, [FromBody] DownloadCodeFormInput downloadCodeForm)
        {
            var userInfo = _userManager.GetUserInfo();
            var templateEntity = await _visualDevRepository.GetFirstAsync(v => v.Id == id && v.DeleteMark == null);
            _ = templateEntity ?? throw HSZException.Oh(ErrorCode.COM1005);
            _ = templateEntity.Tables ?? throw HSZException.Oh(ErrorCode.D2100);
            var model = TemplateKeywordsHelper.ReplaceKeywords(templateEntity.FormData).ToObject<FormDataModel>();
            model.className = downloadCodeForm.className.ToPascalCase();
            model.areasName = downloadCodeForm.module;
            string fileName = YitIdHelper.NextId().ToString();
            //判断子表名称
            var childTb = new List<string>();
            //子表名称去重
            HashSet<string> set = new HashSet<string>(childTb);
            templateEntity.FormData = model.ToJson();
            bool result = childTb.Count == set.Count ? true : false;
            if (!result)
            {
                throw HSZException.Oh(ErrorCode.D2101);
            }
            await this.TemplatesDataAggregation(fileName, templateEntity);
            string randPath = Path.Combine(FileVariable.GenerateCodePath, fileName);
            var dataList = this.PriviewCode(randPath, templateEntity.Type.ToInt(), templateEntity.WebType.ToInt());
            if (dataList == null && dataList.Count == 0)
                throw HSZException.Oh(ErrorCode.D2102);
            return new { list = dataList };
        }

        #endregion

        #region PublicMethod

        /// <summary>
        /// 模板数据聚合
        /// </summary>
        /// <param name="fileName">生成ZIP文件名</param>
        /// <param name="templateEntity">模板实体</param>
        /// <returns></returns>
        public async Task TemplatesDataAggregation(string fileName, VisualDevEntity templateEntity)
        {
            var categoryName = (await _dictionaryDataService.GetInfo(templateEntity.Category)).EnCode;
            var funcTablePrimaryKey = string.Empty;
            var tableRelation = templateEntity.Tables.ToObject<List<DbTableRelationModel>>();
            var columnDesignModel = templateEntity.ColumnData.ToObject<ColumnDesignModel>();
            var formDataModel = templateEntity.FormData.ToObject<FormDataModel>();
            var controls = _runService.TemplateDataConversion(formDataModel.fields);

            var targetPathList = new List<string>();
            var templatePathList = new List<string>();
            //主表名称
            var mianTable = tableRelation.Find(it => it.relationTable == "");

            //主表列配置
            var mainTableColumnConfigList = new List<TableColumnConfigModel>();

            //找副表后移除副表
            var auxiliaryTableList = new List<DbTableRelationModel>();

            //找副表控件
            if (controls.Any(x => x.__vModel__.Contains("_hsz_")))
            {
                var tableName = new List<string>();
                foreach (var field in controls.FindAll(it => it.__vModel__.Contains("_hsz_")).Select(it => it.__vModel__))
                {
                    string str = field;
                    Regex regex = new Regex(@"hsz_(?<table>[\s\S]*?)_hsz_");
                    Match match = regex.Match(str);
                    tableName.Add(match.Groups["table"].Value);
                };
                //为副表列表添加主表信息
                auxiliaryTableList.Add(tableRelation.Find(it => it.relationTable == ""));
                tableName = tableName.Distinct().ToList();
                foreach (var item in tableName)
                {
                    var tableInfo = tableRelation.Find(it => it.table.Equals(item));
                    auxiliaryTableList.Add(tableInfo);
                    tableRelation.Remove(tableInfo);
                }
            }
            //后端默认排序
            var webDefaultSidx = string.Empty;
            var apiDefaultSidx = string.Empty;
            var defaultFlowId = string.Empty;

            #region 后端

            //后端文件生成
            foreach (var item in tableRelation)
            {
                int tableNum = 0;
                if (item.relationTable != "")
                {
                    //获取出子表控件值
                    var children = controls.Find(f => f.__vModel__.Contains("Field") && f.__config__.tableName.Equals(item.table));
                    if (children != null) controls = children.__config__.children;
                    tableNum++;
                }
                var tableName = item.table;
                var link = await _dbLinkService.GetInfo(templateEntity.DbLinkId);
                if (link == null)
                    link = _runService.GetTenantDbLink();
                var table = _databaseService.GetFieldList(link, tableName);
                var tableColumnList = new List<TableColumnConfigModel>();
                foreach (var column in table)
                {
                    var field = GetReplaceDefault(column.field).ToPascalCase().LowerFirstChar();
                    var columnControlKey = controls.Find(c => c.__vModel__ == field);
                    tableColumnList.Add(new TableColumnConfigModel()
                    {
                        ColumnName = GetReplaceDefault(column.field).ToPascalCase(),
                        Alias = column.field,
                        OriginalColumnName = column.field,
                        ColumnComment = column.fieldName,
                        DataType = column.dataType,
                        NetType = CodeGenUtil.ConvertDataType(column.dataType),
                        PrimaryKey = column.primaryKey.ToBool(),
                        QueryWhether = columnDesignModel != null && columnDesignModel.searchList != null ? GetIsColumnQueryWhether(columnDesignModel.searchList, field) : false,
                        QueryType = columnDesignModel != null && columnDesignModel.searchList != null ? GetColumnQueryType(columnDesignModel.searchList, field) : 0,
                        IsShow = columnDesignModel != null && columnDesignModel.columnList != null ? GetIsShowColumn(columnDesignModel.columnList, field) : false,
                        IsMultiple = GetIsMultipleColumn(controls, field),
                        hszKey = columnControlKey != null ? columnControlKey.__config__.hszKey : null,
                        Rule = columnControlKey != null ? columnControlKey.__config__.rule : null,
                        IsAuxiliary = false,
                        TableName = tableName,
                        IsDateTime = GetIsDateTime(columnControlKey),
                        ActiveTxt = columnControlKey != null ? columnControlKey.activeTxt : null,
                        InactiveTxt = columnControlKey != null ? columnControlKey.inactiveTxt : null,
                    });
                }
                if (!tableColumnList.Any(t => t.PrimaryKey == true))
                {
                    throw HSZException.Oh(ErrorCode.D2104);
                }
                //有副表的情况且主表
                if (auxiliaryTableList.Count > 0 && item.relationTable == "")
                {
                    var tableNo = 1;
                    foreach (var auxiliay in auxiliaryTableList.FindAll(it => it.relationTable != ""))
                    {
                        var auxiliaryTable = _databaseService.GetFieldList(link, auxiliay.table);
                        var auxiliaryTableColumnList = new List<TableColumnConfigModel>();
                        foreach (var column in auxiliaryTable)
                        {
                            var field = GetReplaceDefault(column.field).ToPascalCase().LowerFirstChar();
                            var columnControlKey = controls.Find(c => c.__vModel__.Equals(String.Format("hsz_{0}_hsz_{1}", auxiliay.table, field)));
                            auxiliaryTableColumnList.Add(new TableColumnConfigModel()
                            {
                                ColumnName = GetReplaceDefault(column.field).ToPascalCase(),
                                Alias = column.field,
                                OriginalColumnName = column.field,
                                ColumnComment = column.fieldName,
                                DataType = column.dataType,
                                NetType = CodeGenUtil.ConvertDataType(column.dataType),
                                PrimaryKey = column.primaryKey.ToBool(),
                                QueryWhether = columnDesignModel != null && columnDesignModel.searchList != null ? GetIsColumnQueryWhether(columnDesignModel.searchList, String.Format("hsz_{0}_hsz_{1}", auxiliay.table, field)) : false,
                                QueryType = columnDesignModel != null && columnDesignModel.searchList != null ? GetColumnQueryType(columnDesignModel.searchList, String.Format("hsz_{0}_hsz_{1}", auxiliay.table, field)) : 0,
                                IsShow = columnDesignModel != null && columnDesignModel.columnList != null ? GetIsShowColumn(columnDesignModel.columnList, String.Format("hsz_{0}_hsz_{1}", auxiliay.table, field)) : false,
                                IsMultiple = GetIsMultipleColumn(controls, String.Format("hsz_{0}_hsz_{1}", auxiliay.table, field)),
                                hszKey = columnControlKey != null ? columnControlKey.__config__.hszKey : null,
                                Rule = columnControlKey != null ? columnControlKey.__config__.rule : null,
                                IsAuxiliary = true,
                                TableName = auxiliay.table,
                                TableNo = tableNo,
                                IsDateTime = GetIsDateTime(columnControlKey),
                                ActiveTxt = columnControlKey != null ? columnControlKey.activeTxt : null,
                                InactiveTxt = columnControlKey != null ? columnControlKey.inactiveTxt : null,
                            });
                        }
                        if (!auxiliaryTableColumnList.Any(t => t.PrimaryKey == true))
                        {
                            throw HSZException.Oh(ErrorCode.D2104);
                        }
                        tableNo++;

                        var IsAuxiliaryUplpad = auxiliaryTableColumnList.Find(it => it.hszKey != null && (it.hszKey.Equals("uploadImg") || it.hszKey.Equals("uploadFz")));
                        //var IsAuxiliaryMapper = auxiliaryTableColumnList.Find(it => it.hszKey != null && (it.hszKey.Equals("checkbox") || it.hszKey.Equals("cascader") || it.hszKey.Equals("address") || it.hszKey.Equals("uploadImg") || it.hszKey.Equals("uploadFz") || (it.hszKey.Equals("select") && it.IsMultiple == true)));

                        targetPathList = GetAuxiliaryTableBackendTargetPathList(auxiliay, fileName);
                        templatePathList = GetAuxiliaryTableBackendTemplatePathList();

                        //生成副表相关文件
                        for (var i = 0; i < templatePathList.Count; i++)
                        {
                            var tContent = File.ReadAllText(templatePathList[i]);
                            var tResult = _viewEngine.RunCompileFromCached(tContent, new
                            {
                                BusName = templateEntity.FullName,
                                ClassName = formDataModel.className,
                                NameSpace = formDataModel.areasName,
                                PrimaryKey = auxiliaryTableColumnList.Find(it => it.PrimaryKey == true).ColumnName,
                                AuxiliaryTable = auxiliay.table.ToPascalCase(),
                                MainTable = item.table.ToPascalCase(),
                                OriginalMainTableName = auxiliay.table,
                                TableField = auxiliaryTableColumnList,
                                IsUplpad = IsAuxiliaryUplpad == null ? false : true,
                                IsMapper = true,
                                WebType = templateEntity.WebType,
                                Type = templateEntity.Type,
                            });
                            var dirPath = new DirectoryInfo(targetPathList[i]).Parent.FullName;
                            if (!Directory.Exists(dirPath))
                                Directory.CreateDirectory(dirPath);
                            File.WriteAllText(targetPathList[i], tResult, Encoding.UTF8);
                        }
                        tableColumnList.AddRange(auxiliaryTableColumnList.FindAll(it => it.hszKey != null));
                    }
                }
                if (item.relationTable == "")
                {
                    if (templateEntity.Type == 3 && !tableColumnList.Any(it => it.ColumnName.ToLower().Equals("flowid")))
                    {
                        throw HSZException.Oh(ErrorCode.D2105);
                    }
                    if (templateEntity.Type == 3)
                    {
                        defaultFlowId = tableColumnList.Find(it => it.ColumnName.ToLower().Equals("flowid")).ColumnName;
                    }
                    mainTableColumnConfigList = tableColumnList;
                    //默认排序
                    if (columnDesignModel != null && !string.IsNullOrEmpty(columnDesignModel.defaultSidx))
                    {
                        if (columnDesignModel.defaultSidx.Contains("_hsz_"))
                        {
                            Regex regex = new Regex(@"hsz_(?<table>[\s\S]*?)_hsz_");
                            Match match = regex.Match(columnDesignModel.defaultSidx);
                            var auxiliaryTableName = match.Groups["table"].Value;
                            var column = columnDesignModel.defaultSidx.Replace(match.Value, "");
                            var tableColumn = tableColumnList.Find(it => it.LowerColumnName.Equals(column) && it.TableName.Equals(auxiliaryTableName));
                            if (tableColumn != null)
                            {
                                apiDefaultSidx = String.Format("a{0}.{1}", tableColumn.TableNo, tableColumn.Alias);
                                webDefaultSidx = String.Format("a{0}_{1}", tableColumn.TableNo, tableColumn.Alias);
                            }
                        }
                        else if (controls.Any(it => it.__vModel__.Contains("_hsz_")))
                        {
                            var tableColumn = tableColumnList.Find(it => it.LowerColumnName.Equals(columnDesignModel.defaultSidx));
                            if (tableColumn != null)
                            {
                                apiDefaultSidx = String.Format("a.{0}", tableColumn.Alias);
                                webDefaultSidx = String.Format("a_{0}", tableColumn.Alias);
                            }
                        }
                        else
                        {
                            var tableColumn = tableColumnList.Find(it => it.LowerColumnName.Equals(columnDesignModel.defaultSidx));
                            if (tableColumn != null)
                            {
                                if ((templateEntity.Type == 4 && templateEntity.WebType == 3) || templateEntity.Type == 3)
                                {
                                    apiDefaultSidx = String.Format("a.{0}", tableColumn.Alias);
                                    webDefaultSidx = String.Format("a_{0}", tableColumn.Alias);
                                }
                                else
                                {
                                    apiDefaultSidx = tableColumn.Alias;
                                    webDefaultSidx = tableColumn.Alias;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (controls.Any(it => it.__vModel__.Contains("_hsz_")))
                        {
                            apiDefaultSidx = String.Format("a.{0}", tableColumnList.Find(it => it.PrimaryKey == true).Alias);
                            webDefaultSidx = String.Format("a_{0}", tableColumnList.Find(it => it.PrimaryKey == true).Alias);
                        }
                        else
                        {
                            if (templateEntity.Type == 3 || templateEntity.WebType == 3)
                            {
                                apiDefaultSidx = String.Format("a.{0}", tableColumnList.Find(it => it.PrimaryKey == true).Alias);
                                webDefaultSidx = String.Format("a_{0}", tableColumnList.Find(it => it.PrimaryKey == true).Alias);
                            }
                            else
                            {
                                apiDefaultSidx = String.Format("{0}", tableColumnList.Find(it => it.PrimaryKey == true).Alias);
                                webDefaultSidx = String.Format("{0}", tableColumnList.Find(it => it.PrimaryKey == true).Alias);
                            }
                        }
                    }
                }

                var IsUplpad = tableColumnList.Find(it => it.hszKey != null && (it.hszKey.Equals("uploadImg") || it.hszKey.Equals("uploadFz")));
                var IsExport = columnDesignModel != null && columnDesignModel.btnsList != null && templateEntity.WebType != 1 ? columnDesignModel.btnsList.Find(it => it.value == "download") : null;
                var IsMapper = tableColumnList.Find(it => it.hszKey != null && (it.hszKey.Equals("checkbox") || it.hszKey.Equals("cascader") || it.hszKey.Equals("address") || it.hszKey.Equals("comSelect") || it.hszKey.Equals("uploadImg") || it.hszKey.Equals("uploadFz") || (it.hszKey.Equals("select") && it.IsMultiple == true)));
                var isBillRule = templateEntity.FormData.Contains("billRule");
                var isFlowId = tableColumnList.Find(it => it.ColumnName.ToLower().Equals("flowid"));
                var configModel = new CodeGenConfigModel();
                if (templateEntity.Type == 4 || templateEntity.Type == 5)
                {
                    //为生成功能方法模块添加数据
                    switch (templateEntity.Type)
                    {
                        case 4:
                            switch (templateEntity.WebType)
                            {
                                case 1:
                                    columnDesignModel = new ColumnDesignModel();
                                    columnDesignModel.btnsList = new List<ButtonConfigModel>();
                                    columnDesignModel.btnsList.Add(new ButtonConfigModel()
                                    {
                                        value = "add",
                                        icon = "el-icon-plus",
                                        label = "新增",
                                    });
                                    break;
                                case 2:
                                    break;
                                case 3:
                                    break;
                            }
                            break;
                        case 5:
                            switch (templateEntity.WebType)
                            {
                                case 1:
                                    columnDesignModel = new ColumnDesignModel();
                                    columnDesignModel.btnsList = new List<ButtonConfigModel>();
                                    columnDesignModel.btnsList.Add(new ButtonConfigModel()
                                    {
                                        value = "add",
                                        icon = "el-icon-plus",
                                        label = "新增",
                                    });
                                    break;
                                case 2:
                                case 3:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    configModel = new CodeGenConfigModel()
                    {
                        NameSpace = formDataModel.areasName,
                        OriginalMainTableName = item.table,
                        PrimaryKey = tableColumnList.Find(t => t.PrimaryKey == true).ColumnName,
                        MainTable = item.table.ToPascalCase(),
                        BusName = templateEntity.FullName,
                        ClassName = formDataModel.className,
                        hasPage = columnDesignModel != null ? columnDesignModel.hasPage : false,
                        Function = GetCodeGenFunctionList(columnDesignModel.hasPage, columnDesignModel.btnsList, columnDesignModel.columnBtnsList, templateEntity.WebType.ToInt()),
                        TableField = tableColumnList,
                        TableRelations = GetCodeGenTableRelationList(tableRelation, item.table, link, controls.FindAll(it => it.__vModel__.Contains("tableField"))),
                    };
                    //为后续程序能正常运行还原数据
                    switch (templateEntity.Type)
                    {
                        case 4:
                            switch (templateEntity.WebType)
                            {
                                case 1:
                                    columnDesignModel = null;
                                    break;
                            }
                            break;
                        case 5:
                            switch (templateEntity.WebType)
                            {
                                case 1:
                                    columnDesignModel.btnsList = null;
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
                else if (templateEntity.Type == 3)
                {
                    configModel = new CodeGenConfigModel()
                    {
                        NameSpace = formDataModel.areasName,
                        OriginalMainTableName = item.table,
                        PrimaryKey = tableColumnList.Find(t => t.PrimaryKey == true).ColumnName,
                        MainTable = item.table.ToPascalCase(),
                        BusName = templateEntity.FullName,
                        ClassName = formDataModel.className,
                        hasPage = columnDesignModel != null ? columnDesignModel.hasPage : false,
                        Function = GetCodeGenFlowFunctionList(),
                        TableField = tableColumnList,
                        TableRelations = GetCodeGenTableRelationList(tableRelation, item.table, link, controls.FindAll(it => it.__vModel__.Contains("tableField"))),
                    };
                }
                funcTablePrimaryKey = tableColumnList.Find(t => t.PrimaryKey == true).Alias;
                var isChildTable = item.relationTable.IsNullOrEmpty() ? false : true;
                //Web表单&||app代码生成器
                if (templateEntity.Type == 4 || templateEntity.Type == 5)
                {
                    if (!isChildTable)
                    {
                        targetPathList = GetMainTableBackendTargetPathList(item, fileName, templateEntity.WebType.ToInt());
                        templatePathList = GetMainTableBackendTemplatePathList(templateEntity.WebType.ToInt());
                    }
                    else
                    {
                        targetPathList = GetRelationTableBackendTargetPathList(item, fileName, templateEntity.WebType.ToInt());
                        templatePathList = GetRelationTableBackendTemplatePathList(templateEntity.WebType.ToInt());
                    }
                }
                //流程表单
                else if (templateEntity.Type == 3)
                {
                    if (!isChildTable)
                    {
                        targetPathList = GetFlowMainTableBackendTargetPathList(item, fileName);
                        templatePathList = GetFlowMainTableBackendTemplatePathList();
                    }
                    else
                    {
                        targetPathList = GetFlowRelationTableBackendTargetPathList(item, fileName);
                        templatePathList = GetFlowRelationTableBackendTemplatePathList();
                    }
                }

                //生成后端文件
                for (var i = 0; i < templatePathList.Count; i++)
                {
                    var tContent = File.ReadAllText(templatePathList[i]);
                    var tResult = _viewEngine.RunCompileFromCached(tContent, new
                    {
                        BusName = configModel.BusName,
                        NameSpace = configModel.NameSpace,
                        PrimaryKey = configModel.PrimaryKey,
                        PrimaryKeyAlias = configModel.TableField.Find(it => it.PrimaryKey == true).Alias,
                        MainTable = configModel.MainTable,
                        OriginalMainTableName = configModel.OriginalMainTableName,
                        LowerMainTable = configModel.LowerMainTable,
                        ClassName = configModel.ClassName,
                        hasPage = configModel.hasPage,
                        Function = configModel.Function,
                        TableField = configModel.TableField,
                        TableRelations = configModel.TableRelations,
                        DefaultSidx = apiDefaultSidx,
                        IsExport = IsExport == null ? false : true,
                        IsUplpad = IsUplpad == null ? false : true,
                        IsTableRelations = tableNum > 0 ? true : false,
                        ColumnField = IsExport == null ? null : GetMainTableColumnField(columnDesignModel.columnList),
                        IsMapper = IsMapper == null ? false : true,
                        IsBillRule = isBillRule,
                        DbLinkId = templateEntity.DbLinkId,
                        FlowId = templateEntity.Id,
                        WebType = templateEntity.WebType,
                        Type = templateEntity.Type,
                        IsMainTable = item.relationTable == "" ? true : false,
                        isFlowId = isFlowId != null ? true : false,
                        EnCode = templateEntity.EnCode,
                        UseDataPermission = columnDesignModel != null ? columnDesignModel.useDataPermission : false,
                        SearchList = tableColumnList.FindAll(it => it.QueryType.Equals(1)).Count(),
                        IsAuxiliaryTable = auxiliaryTableList.Count > 0 ? true : false,
                        AuxiliaryTable = GetCodeGenTableRelationList(auxiliaryTableList, mianTable.table, link, controls.FindAll(it => it.__vModel__.Contains("tableField"))),
                        DefaultFlowId = defaultFlowId
                    });
                    var dirPath = new DirectoryInfo(targetPathList[i]).Parent.FullName;
                    if (!Directory.Exists(dirPath))
                        Directory.CreateDirectory(dirPath);
                    File.WriteAllText(targetPathList[i], tResult, Encoding.UTF8);
                }
                controls = _runService.TemplateDataConversion(formDataModel.fields);
            }

            #endregion

            #region 前端

            //前端文件生成
            //表单列
            Dictionary<string, List<string>> ListQueryControlsType = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> ListColumnControlsType = new Dictionary<string, List<string>>();

            //左侧树
            var treeRelation = string.Empty;

            #region 查询控件归类

            if (templateEntity.Type == 4)
            {
                var useInputList = new List<string>();
                useInputList.Add("comInput");
                useInputList.Add("textarea");
                useInputList.Add("HSZText");
                useInputList.Add("billRule");
                ListQueryControlsType["inputList"] = useInputList;

                var useDateList = new List<string>();
                useDateList.Add("createTime");
                useDateList.Add("modifyTime");
                ListQueryControlsType["dateList"] = useDateList;

                var useSelectList = new List<string>();
                useSelectList.Add("select");
                useSelectList.Add("radio");
                useSelectList.Add("checkbox");
                ListQueryControlsType["selectList"] = useSelectList;

                var timePickerList = new List<string>();
                timePickerList.Add("time");
                ListQueryControlsType["timePickerList"] = timePickerList;

                var numRangeList = new List<string>();
                numRangeList.Add("numInput");
                numRangeList.Add("calculate");
                ListQueryControlsType["numRangeList"] = numRangeList;

                var datePickerList = new List<string>();
                datePickerList.Add("date");
                ListQueryControlsType["datePickerList"] = datePickerList;

                var userSelectList = new List<string>();
                userSelectList.Add("createUser");
                userSelectList.Add("modifyUser");
                userSelectList.Add("userSelect");
                ListQueryControlsType["userSelectList"] = userSelectList;

                var organizeList = new List<string>();
                ListQueryControlsType["organizeList"] = organizeList;

                var comSelectList = new List<string>();
                comSelectList.Add("comSelect");
                comSelectList.Add("currOrganize");
                ListQueryControlsType["comSelectList"] = comSelectList;

                var depSelectList = new List<string>();
                depSelectList.Add("currDept");
                depSelectList.Add("depSelect");
                ListQueryControlsType["depSelectList"] = depSelectList;

                var posSelectList = new List<string>();
                posSelectList.Add("currPosition");
                posSelectList.Add("posSelect");
                ListQueryControlsType["posSelectList"] = posSelectList;

                var useCascaderList = new List<string>();
                useCascaderList.Add("cascader");
                ListQueryControlsType["useCascaderList"] = useCascaderList;

                var HSZAddressList = new List<string>();
                HSZAddressList.Add("address");
                ListQueryControlsType["HSZAddressList"] = HSZAddressList;

                var treeSelectList = new List<string>();
                treeSelectList.Add("treeSelect");
                ListQueryControlsType["treeSelectList"] = treeSelectList;
            }
            else if (templateEntity.Type == 5)
            {
                var inputList = new List<string>();
                inputList.Add("comInput");
                inputList.Add("textarea");
                inputList.Add("HSZText");
                inputList.Add("billRule");
                inputList.Add("calculate");
                ListQueryControlsType["input"] = inputList;

                var numRangeList = new List<string>();
                numRangeList.Add("numInput");
                ListQueryControlsType["numRange"] = numRangeList;

                var switchList = new List<string>();
                switchList.Add("switch");
                ListQueryControlsType["switch"] = switchList;

                var selectList = new List<string>();
                selectList.Add("radio");
                selectList.Add("checkbox");
                selectList.Add("select");
                ListQueryControlsType["select"] = selectList;

                var cascaderList = new List<string>();
                cascaderList.Add("cascader");
                ListQueryControlsType["cascader"] = cascaderList;

                var timeList = new List<string>();
                timeList.Add("time");
                ListQueryControlsType["time"] = timeList;

                var dateList = new List<string>();
                dateList.Add("date");
                dateList.Add("createTime");
                dateList.Add("modifyTime");
                ListQueryControlsType["date"] = dateList;

                var comSelectList = new List<string>();
                comSelectList.Add("comSelect");
                ListQueryControlsType["comSelect"] = comSelectList;

                var depSelectList = new List<string>();
                depSelectList.Add("depSelect");
                depSelectList.Add("currDept");
                depSelectList.Add("currOrganize");
                ListQueryControlsType["depSelect"] = depSelectList;

                var posSelectList = new List<string>();
                posSelectList.Add("posSelect");
                posSelectList.Add("currPosition");
                ListQueryControlsType["posSelect"] = posSelectList;

                var userSelectList = new List<string>();
                userSelectList.Add("userSelect");
                userSelectList.Add("createUser");
                userSelectList.Add("modifyUser");
                ListQueryControlsType["userSelect"] = userSelectList;

                var treeSelectList = new List<string>();
                treeSelectList.Add("treeSelect");
                ListQueryControlsType["treeSelect"] = treeSelectList;

                var addressList = new List<string>();
                addressList.Add("address");
                ListQueryControlsType["address"] = addressList;
            }

            #endregion

            #region 列控件归类

            var columnList = new List<string>();
            columnList.Add("select");
            columnList.Add("radio");
            columnList.Add("checkbox");
            columnList.Add("treeSelect");
            columnList.Add("cascader");
            ListColumnControlsType["columnList"] = columnList;

            #endregion

            //表单全控件
            var formAllControlsList = GetFormAllControlsList(formDataModel.fields, formDataModel.gutter, formDataModel.labelWidth, true);

            //表单控件
            var formColimnList = new List<CodeGenFormColumnModel>();
            //列表主表控件Option
            var indnxListMainTableControlOption = GetFormAllControlsProps(formDataModel.fields, templateEntity.Type.ToInt(), true);
            foreach (var item in controls)
            {
                var config = item.__config__;
                switch (config.hszKey)
                {
                    case "table":
                        {
                            var childrenFormColimnList = new List<CodeGenFormColumnModel>();
                            foreach (var children in config.children)
                            {
                                var childrenConfig = children.__config__;
                                switch (childrenConfig.hszKey)
                                {
                                    case "relationFormAttr":
                                    case "popupAttr":
                                        break;
                                    case "switch":
                                        {
                                            childrenFormColimnList.Add(new CodeGenFormColumnModel()
                                            {
                                                Name = children.__vModel__,
                                                OriginalName = children.__vModel__,
                                                hszKey = childrenConfig.hszKey,
                                                DataType = childrenConfig.dataType,
                                                DictionaryType = childrenConfig.dataType == "dictionary" ? childrenConfig.dictionaryType : (childrenConfig.dataType == "dynamic" ? childrenConfig.propsUrl : null),
                                                Format = children.format,
                                                Multiple = childrenConfig.hszKey == "cascader" ? children.props.props.multiple : item.multiple,
                                                BillRule = childrenConfig.rule,
                                                Required = childrenConfig.required,
                                                Placeholder = childrenConfig.label,
                                                Range = children.range,
                                                RegList = childrenConfig.regList,
                                                DefaultValue = childrenConfig.defaultValue.ToBool(),
                                                Trigger = childrenConfig.trigger.ToJson() == "null" ? "blur" : (childrenConfig.trigger.ToJson().Contains("[") ? childrenConfig.trigger.ToJson() : childrenConfig.trigger.ToString()),
                                                ChildrenList = null,
                                                IsSummary = item.showSummary && item.summaryField.Find(it => it.Equals(children.__vModel__)) != null ? true : false,
                                            });
                                        }
                                        break;
                                    default:
                                        {
                                            childrenFormColimnList.Add(new CodeGenFormColumnModel()
                                            {
                                                Name = children.__vModel__,
                                                OriginalName = children.__vModel__,
                                                hszKey = childrenConfig.hszKey,
                                                DataType = childrenConfig.dataType,
                                                DictionaryType = childrenConfig.dataType == "dictionary" ? childrenConfig.dictionaryType : (childrenConfig.dataType == "dynamic" ? childrenConfig.propsUrl : null),
                                                Format = children.format,
                                                Multiple = childrenConfig.hszKey == "cascader" ? children.props.props.multiple : item.multiple,
                                                BillRule = childrenConfig.rule,
                                                Required = childrenConfig.required,
                                                Placeholder = childrenConfig.label,
                                                Range = children.range,
                                                RegList = childrenConfig.regList,
                                                DefaultValue = childrenConfig.defaultValue != null && childrenConfig.defaultValue.ToString() != "[]" && !string.IsNullOrEmpty(childrenConfig.defaultValue.ToString()) ? childrenConfig.defaultValue : null,
                                                Trigger = childrenConfig.trigger.ToJson() == "null" ? "blur" : (childrenConfig.trigger.ToJson().Contains("[") ? childrenConfig.trigger.ToJson() : childrenConfig.trigger.ToString()),
                                                ChildrenList = null,
                                                IsSummary = item.showSummary && item.summaryField.Find(it => it.Equals(children.__vModel__)) != null ? true : false,
                                            });
                                        }
                                        break;
                                }
                            }
                            formColimnList.Add(new CodeGenFormColumnModel()
                            {
                                Name = config.tableName.ToPascalCase(),
                                Placeholder = config.label,
                                OriginalName = config.tableName,
                                hszKey = config.hszKey,
                                ChildrenList = childrenFormColimnList,
                                ShowSummary = item.showSummary,
                                SummaryField = item.summaryField.ToJson()
                            });
                        }
                        break;
                    case "relationFormAttr":
                    case "popupAttr":
                        break;
                    case "switch":
                        {
                            var originalName = string.Empty;
                            if (item.__vModel__.Contains("_hsz_"))
                            {
                                Regex regex = new Regex(@"hsz_(?<table>[\s\S]*?)_hsz_");
                                Match match = regex.Match(item.__vModel__);
                                var auxiliaryTableName = match.Groups["table"].Value;
                                var column = item.__vModel__.Replace(match.Value, "");
                                var columns = mainTableColumnConfigList.Find(it => it.LowerColumnName.Equals(column) && it.TableName.Equals(auxiliaryTableName));
                                if (columns != null)
                                    originalName = columns.Alias;
                            }
                            else
                            {
                                var columns = mainTableColumnConfigList.Find(it => it.LowerColumnName.Equals(item.__vModel__));
                                if (columns != null)
                                    originalName = columns.Alias;
                            }

                            formColimnList.Add(new CodeGenFormColumnModel()
                            {
                                Name = item.__vModel__,
                                OriginalName = originalName,
                                hszKey = config.hszKey,
                                DataType = config.dataType,
                                DictionaryType = config.dataType == "dictionary" ? config.dictionaryType : (config.dataType == "dynamic" ? config.propsUrl : null),
                                Format = item.format,
                                Multiple = item.multiple,
                                BillRule = config.rule,
                                Required = config.required,
                                Placeholder = config.label,
                                Range = item.range,
                                RegList = config.regList,
                                DefaultValue = config.defaultValue.ToBool(),
                                Trigger = config.trigger.ToJson() == "null" ? "blur" : (config.trigger.ToJson().Contains("[") ? config.trigger.ToJson() : config.trigger.ToString()),
                                ChildrenList = null
                            });
                        }
                        break;
                    default:
                        {
                            var originalName = string.Empty;
                            if (item.__vModel__.Contains("_hsz_"))
                            {
                                Regex regex = new Regex(@"hsz_(?<table>[\s\S]*?)_hsz_");
                                Match match = regex.Match(item.__vModel__);
                                var auxiliaryTableName = match.Groups["table"].Value;
                                var column = item.__vModel__.Replace(match.Value, "");
                                var columns = mainTableColumnConfigList.Find(it => it.LowerColumnName.Equals(column) && it.TableName.Equals(auxiliaryTableName));
                                if (columns != null)
                                    originalName = columns.Alias;
                            }
                            else
                            {
                                var columns = mainTableColumnConfigList.Find(it => it.LowerColumnName.Equals(item.__vModel__));
                                if (columns != null)
                                    originalName = columns.Alias;
                            }

                            formColimnList.Add(new CodeGenFormColumnModel()
                            {
                                Name = item.__vModel__,
                                OriginalName = originalName,
                                hszKey = config.hszKey,
                                DataType = config.dataType,
                                DictionaryType = config.dataType == "dictionary" ? config.dictionaryType : (config.dataType == "dynamic" ? config.propsUrl : null),
                                Format = item.format,
                                Multiple = config.hszKey == "cascader" ? item.props.props.multiple : item.multiple,
                                BillRule = config.rule,
                                Required = config.required,
                                Placeholder = config.label,
                                Range = item.range,
                                RegList = config.regList,
                                DefaultValue = config.defaultValue != null && config.defaultValue.ToString() != "[]" && !string.IsNullOrEmpty(config.defaultValue.ToString()) ? config.defaultValue : null,
                                Trigger = config.trigger.ToJson() == "null" ? "blur" : (config.trigger.ToJson().Contains("[") ? config.trigger.ToJson() : config.trigger.ToString()),
                                ChildrenList = null
                            });
                        }
                        break;
                }
            }

            //列表页查询
            var indexSearchColumnDesignList = new List<CodeGenConvIndexSearchColumnDesign>();
            //列表页按钮
            var indexListTopButtonList = new List<CodeGenConvIndexListTopButtonDesign>();
            //列表列按钮
            var indexListColumnButtonDesignList = new List<CodeGenConvIndexListTopButtonDesign>();
            //列表页列表
            var indexListColumnDesignList = new List<CodeGenConvIndexListColumnDesign>();
            if (templateEntity.WebType != 1 && templateEntity.Type != 3)
            {
                foreach (var item in columnDesignModel.searchList)
                {
                    var column = controls.Find(c => c.__vModel__ == item.__vModel__);
                    var queryControls = ListQueryControlsType.Where(q => q.Value.Contains(column.__config__.hszKey)).FirstOrDefault();
                    var queryColumn = new TableColumnConfigModel();
                    if (item.__vModel__.Contains("_hsz_"))
                    {
                        Regex regex = new Regex(@"hsz_(?<table>[\s\S]*?)_hsz_");
                        Match match = regex.Match(item.__vModel__);
                        var auxiliaryTableName = match.Groups["table"].Value;
                        var columnName = item.__vModel__.Replace(match.Value, "");
                        queryColumn = mainTableColumnConfigList.Find(it => it.LowerColumnName.Equals(columnName) && it.TableName.Equals(auxiliaryTableName));
                    }
                    else
                    {
                        queryColumn = mainTableColumnConfigList.Find(it => it.LowerColumnName.Equals(item.__vModel__));
                    }
                    var columnDesignName = string.Empty;
                    if (templateEntity.WebType == 3)
                    {
                        columnDesignName = String.Format("{0}_{1}", queryColumn.IsAuxiliary ? $"a{queryColumn.TableNo}" : "a", queryColumn.Alias);
                    }
                    else
                    {
                        columnDesignName = auxiliaryTableList.Count == 0 ? queryColumn.Alias : String.Format("{0}_{1}", queryColumn.IsAuxiliary ? $"a{queryColumn.TableNo}" : "a", queryColumn.Alias);
                    }
                    indexSearchColumnDesignList.Add(new CodeGenConvIndexSearchColumnDesign()
                    {
                        OriginalName = item.__vModel__,
                        Name = columnDesignName,
                        LowerName = GetReplaceDefault(columnDesignName).LowerFirstChar(),
                        Tag = item.__config__.tag,
                        Clearable = item.clearable ? "clearable " : "",
                        Format = column.format,
                        ValueFormat = column.valueformat,
                        Label = item.__config__.label,
                        QueryControlsKey = queryControls.Key != null ? queryControls.Key : null,
                        Props = column.__config__.props,
                        Index = columnDesignModel.searchList.IndexOf(item),
                        Type = column.type,
                        ShowAllLevels = column.showalllevels ? "true" : "false",
                        Level = column.level
                    });
                }
                //为生成功能方法模块添加数据
                switch (templateEntity.Type)
                {
                    case 5:
                        switch (templateEntity.WebType)
                        {
                            case 1:
                                columnDesignModel.btnsList = new List<ButtonConfigModel>();
                                columnDesignModel.columnBtnsList = new List<ButtonConfigModel>();
                                break;
                        }
                        break;
                }
                foreach (var item in columnDesignModel.btnsList)
                {
                    indexListTopButtonList.Add(new CodeGenConvIndexListTopButtonDesign()
                    {
                        Type = columnDesignModel.btnsList.IndexOf(item) == 0 ? "primary" : "text",
                        Icon = item.icon,
                        Method = GetCodeGenConvIndexListTopButtonMethod(item.value),
                        Value = item.value,
                        Label = item.label
                    });
                }

                foreach (var item in columnDesignModel.columnBtnsList)
                {
                    indexListColumnButtonDesignList.Add(new CodeGenConvIndexListTopButtonDesign()
                    {
                        Type = item.value == "remove" ? "class='HSZ-table-delBtn' " : "",
                        Icon = item.icon,
                        Method = GetCodeGenConvIndexListColumnButtonMethod(item.value, funcTablePrimaryKey, auxiliaryTableList.Count),
                        Value = item.value,
                        Label = item.label,
                        Disabled = GetCodeGenWorkflowIndexListColumnButtonDisabled(item.value),
                        IsDetail = item.value == "detail" ? true : false
                    });
                }
                switch (templateEntity.Type)
                {
                    case 5:
                        switch (templateEntity.WebType)
                        {
                            case 1:
                                columnDesignModel.btnsList = null;
                                columnDesignModel.columnBtnsList = null;
                                break;
                        }
                        break;
                }

                foreach (var item in columnDesignModel.columnList)
                {
                    var conversion = ListColumnControlsType.Where(q => q.Value.Contains(item.hszKey)).FirstOrDefault();
                    var tableColumn = new TableColumnConfigModel();
                    if (item.prop.Contains("_hsz_"))
                    {
                        Regex regex = new Regex(@"hsz_(?<table>[\s\S]*?)_hsz_");
                        Match match = regex.Match(item.prop);
                        var auxiliaryTableName = match.Groups["table"].Value;
                        var column = item.prop.Replace(match.Value, "");
                        tableColumn = mainTableColumnConfigList.Find(it => it.LowerColumnName.Equals(column) && it.TableName.Equals(auxiliaryTableName));
                    }
                    else
                    {
                        tableColumn = mainTableColumnConfigList.Find(it => it.LowerColumnName.Equals(item.prop));
                    }
                    var columnDesignName = auxiliaryTableList.Count > 0 ? tableColumn.IsAuxiliary ? string.Format("a{0}_{1}", tableColumn.TableNo, tableColumn.Alias) : String.Format("a_{0}", tableColumn.Alias) : tableColumn.Alias;
                    indexListColumnDesignList.Add(new CodeGenConvIndexListColumnDesign()
                    {
                        Name = columnDesignName,
                        OptionsName = item.prop,
                        LowerName = GetReplaceDefault(columnDesignName).LowerFirstChar(),
                        hszKey = item.hszKey,
                        Label = item.label,
                        Width = item.width == "0" ? "" : "width='" + item.width + "' ",
                        Align = item.align,
                        IsAutomatic = conversion.Key == null ? false : true,
                        IsSort = item.sortable ? "sortable='custom' " : ""
                    });
                }
            }

            var isBatchRemoveDel = indexListTopButtonList.Any(it => it.Value == "batchRemove");
            var isDownload = indexListTopButtonList.Any(it => it.Value == "download");
            var isRemoveDel = indexListColumnButtonDesignList.Any(it => it.Value == "remove");
            var isEdit = indexListColumnButtonDesignList.Any(it => it.Value == "edit");
            var isDetail = indexListColumnButtonDesignList.Any(it => it.IsDetail.Equals(true));
            var isSort = columnDesignModel != null && columnDesignModel.columnList != null ? columnDesignModel.columnList.Find(it => it.sortable == true) : null;
            var isSummary = formColimnList.Any(it => it.hszKey.Equals("table") && it.ShowSummary.Equals(true));
            var isAdd = indexListTopButtonList.Any(it => it.Value == "add");
            if (columnDesignModel != null && columnDesignModel.treeRelation != null)
            {
                var treeRelationName = mainTableColumnConfigList.Find(it => it.LowerColumnName.Equals(columnDesignModel.treeRelation));
                if (treeRelationName != null)
                {
                    treeRelation = auxiliaryTableList.Count == 0 ? templateEntity.WebType == 3 ? string.Format("a_{0}", treeRelationName.Alias) : treeRelationName.Alias : (!treeRelationName.IsAuxiliary ? string.Format("a_{0}", treeRelationName.Alias) : string.Format("a{0}_{1}", treeRelationName.TableNo, treeRelationName.Alias));
                }
            }

            if (mainTableColumnConfigList.Any(it => it.IsAuxiliary == true))
            {
                funcTablePrimaryKey = string.Format("a_{0}", funcTablePrimaryKey);
            }

            var convFromConfigModel = new CodeGenConvFormConfigModel()
            {
                NameSpace = formDataModel.areasName,
                ClassName = formDataModel.className,
                PrimaryKey = funcTablePrimaryKey,
                FormPrimaryKey = GetReplaceDefault(mainTableColumnConfigList.Find(t => t.PrimaryKey == true).Alias).LowerFirstChar(),
                BusName = tableRelation.FirstOrDefault().tableName,
                FormList = formColimnList,
                PopupType = formDataModel.popupType,
                SearchColumnDesign = indexSearchColumnDesignList,
                AllColumnDesign = columnDesignModel,
                TopButtonDesign = indexListTopButtonList,
                ColumnDesign = indexListColumnDesignList,
                ColumnButtonDesign = indexListColumnButtonDesignList,
                TreeRelation = treeRelation,
                IsExistQuery = columnDesignModel != null && columnDesignModel.searchList != null ? columnDesignModel.searchList.Any(it => it.__vModel__ == columnDesignModel.treeRelation) : false,
                OptionsList = indnxListMainTableControlOption,
                IsBatchRemoveDel = isBatchRemoveDel,
                IsDownload = isDownload,
                IsRemoveDel = isRemoveDel,
                IsDetail = isDetail,
                FormAllContols = formAllControlsList,
                Type = columnDesignModel != null ? columnDesignModel.type : 0
            };
            targetPathList = new List<string>();
            templatePathList = new List<string>();

            //Web表单
            if (templateEntity.Type == 4)
            {
                targetPathList = GetFrontendTargetPathList(mianTable, fileName, templateEntity.WebType.ToInt(), convFromConfigModel.IsDetail);
                templatePathList = GetFrontendTemplatePathList(templateEntity.WebType.ToInt(), convFromConfigModel.IsDetail);
            }
            //app代码生成器
            else if (templateEntity.Type == 5)
            {
                targetPathList = GetAppFrontendTargetPathList(mianTable, fileName, templateEntity.WebType.ToInt());
                templatePathList = GetAppFrontendTemplatePathList(templateEntity.WebType.ToInt());
            }
            //流程表单
            else if (templateEntity.Type == 3)
            {
                targetPathList = GetFlowFrontendTargetPathList(mianTable, fileName);
                templatePathList = GetFlowFrontendTemplatePathList();
            }
            var flowTemplateJson = templateEntity.FlowTemplateJson != null ? templateEntity.FlowTemplateJson.ToJson() : null;
            if (tableRelation.Count > 1)
            {
                foreach (var item in tableRelation)
                {
                    if (item.relationTable != "")
                    {
                        var tableField = controls.Find(it => it.__config__.hszKey.Equals("table") && it.__config__.tableName.Equals(item.table));
                        //替换工作流模板内子表命名
                        if (flowTemplateJson != null && tableField != null)
                            flowTemplateJson = flowTemplateJson.Replace(tableField.__vModel__, item.table.ToPascalCase().LowerFirstChar());
                    }
                }
            }
            for (var i = 0; i < templatePathList.Count; i++)
            {
                var tContent = File.ReadAllText(templatePathList[i]);
                var tResult = _viewEngine.RunCompileFromCached(tContent, new
                {
                    BusName = convFromConfigModel.BusName,
                    ClassName = formDataModel.className,
                    NameSpace = convFromConfigModel.NameSpace,
                    PrimaryKey = convFromConfigModel.PrimaryKey,
                    LowerPrimaryKey = convFromConfigModel.PrimaryKey.LowerFirstChar(),
                    FormPrimaryKey = convFromConfigModel.FormPrimaryKey,
                    FormDataModel = formDataModel,
                    FormList = formColimnList,
                    PopupType = formDataModel.popupType == null ? "" : formDataModel.popupType,
                    SearchColumnDesign = convFromConfigModel.SearchColumnDesign,
                    AllColumnDesign = convFromConfigModel.AllColumnDesign,
                    TopButtonDesign = convFromConfigModel.TopButtonDesign,
                    ColumnDesign = convFromConfigModel.ColumnDesign,
                    ColumnButtonDesign = convFromConfigModel.ColumnButtonDesign,
                    TreeRelation = convFromConfigModel.TreeRelation,
                    IsExistQuery = convFromConfigModel.IsExistQuery,
                    Type = convFromConfigModel.Type,
                    OptionsList = convFromConfigModel.OptionsList,
                    IsBatchRemoveDel = convFromConfigModel.IsBatchRemoveDel,
                    IsDownload = convFromConfigModel.IsDownload,
                    IsRemoveDel = convFromConfigModel.IsRemoveDel,
                    IsDetail = convFromConfigModel.IsDetail,
                    IsEdit = isEdit,
                    IsAdd = isAdd,
                    IsSort = isSort == null ? false : true,
                    FormAllContols = convFromConfigModel.FormAllContols,
                    DefaultSidx = webDefaultSidx,
                    EnCode = templateEntity.EnCode,
                    FlowId = templateEntity.Id,
                    FullName = templateEntity.FullName,
                    DbLinkId = templateEntity.DbLinkId,
                    FlowTemplateJson = flowTemplateJson,
                    Tables = templateEntity.Tables.ToJson(),
                    Category = categoryName,
                    MianTable = mianTable.table.ToPascalCase().LowerFirstChar(),
                    WebType = templateEntity.WebType,
                    HasPage = columnDesignModel != null ? columnDesignModel.hasPage : false,
                    UseFormPermission = columnDesignModel != null ? columnDesignModel.useFormPermission : false,
                    CreatorTime = Ext.ConvertToTimeStamp(DateTime.Now),
                    CreatorUserId = _userManager.UserId,
                    IsSummary = isSummary,
                    AddTitleName = isAdd ? convFromConfigModel.TopButtonDesign.Find(it => it.Value.Equals("add")).Label : "",
                    EditTitleName = isEdit ? indexListColumnButtonDesignList.Find(it => it.Value.Equals("edit")).Label : "",
                    DetailTitleName = isDetail ? indexListColumnButtonDesignList.Find(it => it.IsDetail.Equals(true)).Label : "",
                }, builderAction: builder =>
                {
                    builder.AddUsing("HSZ.JsonSerialization");
                    builder.AddUsing("HSZ.VisualDev.Entitys.Model.CodeGen");
                    builder.AddAssemblyReferenceByName("HSZ.VisualDev.Entitys");
                });
                var dirPath = new DirectoryInfo(targetPathList[i]).Parent.FullName;
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);
                File.WriteAllText(targetPathList[i], tResult, Encoding.UTF8);
            }

            #endregion

        }

        /// <summary>
        /// 预览代码
        /// </summary>
        /// <param name="codePath"></param>
        /// <param name="type">1-Web设计,2-App设计,3-流程表单,4-Web表单,5-App表单</param>
        /// <param name="webType">页面类型（1、纯表单，2、表单加列表，3、表单列表工作流）</param>
        /// <returns></returns>
        public List<Dictionary<string, object>> PriviewCode(string codePath, int type, int webType)
        {
            var dataList = FileHelper.GetAllFiles(codePath);
            List<Dictionary<string, string>> datas = new List<Dictionary<string, string>>();
            List<Dictionary<string, object>> allDatas = new List<Dictionary<string, object>>();
            foreach (var item in dataList)
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                FileStream fileStream = new FileStream(item.FullName, FileMode.Open);
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    var buffer = new Char[(int)reader.BaseStream.Length];
                    reader.Read(buffer, 0, (int)reader.BaseStream.Length);
                    var content = new string(buffer);
                    if ("cs".Equals(item.Extension.Replace(".", "")))
                    {
                        string fileName = item.FullName.ToLower();
                        if (fileName.Contains("listqueryinput") || fileName.Contains("crinput") || fileName.Contains("upinput") || fileName.Contains("upoutput") || fileName.Contains("listoutput") || fileName.Contains("infooutput"))
                        {
                            data.Add("folderName", "dto");
                        }
                        else if (fileName.Contains("mapper"))
                        {
                            data.Add("folderName", "mapper");
                        }
                        else if (fileName.Contains("entity"))
                        {
                            data.Add("folderName", "entity");
                        }
                        else
                        {
                            data.Add("folderName", "dotnet");
                        }
                        data.Add("fileName", item.Name);
                        //剔除"\0"特殊符号
                        data.Add("fileContent", content.Replace("\0", ""));
                        data.Add("fileType", item.Extension.Replace(".", ""));
                        datas.Add(data);
                    }
                    else if (".json".Equals(item.Extension))
                    {
                        data.Add("folderName", "json");
                        data.Add("id", YitIdHelper.NextId().ToString());
                        data.Add("fileName", item.Name);
                        //剔除"\0"特殊符号
                        data.Add("fileContent", content.Replace("\0", ""));
                        data.Add("fileType", item.Extension.Replace(".", ""));
                        datas.Add(data);

                    }
                    else if (".vue".Equals(item.Extension))
                    {
                        switch (type)
                        {
                            case 5:
                                data.Add("folderName", "app");
                                break;
                            case 3:
                                if (item.FullName.ToLower().Contains("app"))
                                {
                                    data.Add("folderName", "app");
                                }
                                else if (item.FullName.ToLower().Contains("pc"))
                                {
                                    data.Add("folderName", "web");
                                }
                                break;
                            default:
                                data.Add("folderName", "web");
                                break;
                        }
                        data.Add("id", YitIdHelper.NextId().ToString());
                        data.Add("fileName", item.Name);
                        //剔除"\0"特殊符号
                        data.Add("fileContent", content.Replace("\0", ""));
                        data.Add("fileType", item.Extension.Replace(".", ""));
                        datas.Add(data);
                    }
                }
            }
            //datas 集合去重
            var parent = datas.GroupBy(d => d["folderName"]).Select(d => d.First()).OrderBy(d => d["folderName"]).ToList();
            foreach (var item in parent)
            {
                Dictionary<string, object> dataMap = new Dictionary<string, object>();
                dataMap["fileName"] = item["folderName"];
                dataMap["id"] = YitIdHelper.NextId().ToString();
                dataMap["children"] = datas.FindAll(d => d["folderName"] == item["folderName"]);
                allDatas.Add(dataMap);
            }
            return allDatas;
        }

        #endregion

        #region PrivateMethod

        #region 后端主表

        /// <summary>
        /// 获取后端主表模板文件路径集合
        /// </summary>
        /// <param name="webType">页面类型（1、纯表单，2、表单加列表，3、表单列表工作流）</param>
        /// <returns></returns>
        private List<string> GetMainTableBackendTemplatePathList(int webType)
        {
            var templatePath = Path.Combine(App.WebHostEnvironment.WebRootPath, "Template");
            switch (webType)
            {
                case 1:
                    return new List<string>()
                    {
                        Path.Combine(templatePath , "Service.cs.vm"),
                        Path.Combine(templatePath , "IService.cs.vm"),
                        Path.Combine(templatePath , "Entity.cs.vm"),
                        Path.Combine(templatePath , "CrInput.cs.vm"),
                        Path.Combine(templatePath , "Mapper.cs.vm"),
                    };
                case 2:
                    return new List<string>()
                    {
                        Path.Combine(templatePath , "Service.cs.vm"),
                        Path.Combine(templatePath , "IService.cs.vm"),
                        Path.Combine(templatePath , "Entity.cs.vm"),
                        Path.Combine(templatePath , "Mapper.cs.vm"),
                        Path.Combine(templatePath , "CrInput.cs.vm"),
                        Path.Combine(templatePath , "UpInput.cs.vm"),
                        Path.Combine(templatePath , "ListQueryInput.cs.vm"),
                        Path.Combine(templatePath , "InfoOutput.cs.vm"),
                        Path.Combine(templatePath , "ListOutput.cs.vm")
                    };
                case 3:
                    return new List<string>()
                    {
                        Path.Combine(templatePath , "WorkflowService.cs.vm"),
                        Path.Combine(templatePath , "IService.cs.vm"),
                        Path.Combine(templatePath , "Entity.cs.vm"),
                        Path.Combine(templatePath , "Mapper.cs.vm"),
                        Path.Combine(templatePath , "CrInput.cs.vm"),
                        Path.Combine(templatePath , "UpInput.cs.vm"),
                        Path.Combine(templatePath , "ListQueryInput.cs.vm"),
                        Path.Combine(templatePath , "InfoOutput.cs.vm"),
                        Path.Combine(templatePath , "ListOutput.cs.vm")
                    };
            }
            return null;
        }

        /// <summary>
        /// 获取后端主表生成文件路径
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="fileName"></param>
        /// <param name="webType">页面类型（1、纯表单，2、表单加列表，3、表单列表工作流）</param>
        /// <returns></returns>
        private List<string> GetMainTableBackendTargetPathList(DbTableRelationModel tableInfo, string fileName, int webType)
        {
            var backendPath = Path.Combine(FileVariable.GenerateCodePath, fileName, "Net");
            var tableName = tableInfo.table.ToPascalCase();
            var servicePath = Path.Combine(backendPath, "Controller", tableName + "Service.cs");
            var iservicePath = Path.Combine(backendPath, "Controller", "I" + tableName + "Service.cs");
            var entityPath = Path.Combine(backendPath, "Models", "Entity", tableName + "Entity.cs");
            var mapperPath = Path.Combine(backendPath, "Models", "Mapper", tableName + "Mapper.cs");
            var inputCrPath = Path.Combine(backendPath, "Models", "Dto", tableName + "CrInput.cs");
            var inputUpPath = Path.Combine(backendPath, "Models", "Dto", tableName + "UpInput.cs");
            var inputListQueryPath = Path.Combine(backendPath, "Models", "Dto", tableName + "ListQueryInput.cs");
            var outputInfoPath = Path.Combine(backendPath, "Models", "Dto", tableName + "InfoOutput.cs");
            var outputListPath = Path.Combine(backendPath, "Models", "Dto", tableName + "ListOutput.cs");
            switch (webType)
            {
                case 1:
                    return new List<string>()
                    {
                        servicePath,
                        iservicePath,
                        entityPath,
                        inputCrPath,
                        mapperPath
                    };
                case 2:
                    return new List<string>()
                    {
                        servicePath,
                        iservicePath,
                        entityPath,
                        mapperPath,
                        inputCrPath,
                        inputUpPath,
                        inputListQueryPath,
                        outputInfoPath,
                        outputListPath
                    };
                case 3:
                    return new List<string>()
                    {
                        servicePath,
                        iservicePath,
                        entityPath,
                        mapperPath,
                        inputCrPath,
                        inputUpPath,
                        inputListQueryPath,
                        outputInfoPath,
                        outputListPath
                    };
            }
            return null;
        }

        /// <summary>
        /// 获取流程后端主表模板文件路径集合
        /// </summary>
        /// <returns></returns>
        private List<string> GetFlowMainTableBackendTemplatePathList()
        {
            var templatePath = Path.Combine(App.WebHostEnvironment.WebRootPath, "Template");
            return new List<string>()
            {
                Path.Combine(templatePath , "WorkFlowFormService.cs.vm"),
                Path.Combine(templatePath , "IService.cs.vm"),
                Path.Combine(templatePath , "Entity.cs.vm"),
                Path.Combine(templatePath , "Mapper.cs.vm"),
                Path.Combine(templatePath , "CrInput.cs.vm"),
                Path.Combine(templatePath , "UpInput.cs.vm"),
                Path.Combine(templatePath , "InfoOutput.cs.vm")
            };
        }

        /// <summary>
        /// 获取流程后端主表生成文件路径
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private List<string> GetFlowMainTableBackendTargetPathList(DbTableRelationModel tableInfo, string fileName)
        {
            var backendPath = Path.Combine(FileVariable.GenerateCodePath, fileName, "Net");
            var tableName = tableInfo.table.ToPascalCase();
            var servicePath = Path.Combine(backendPath, "Controller", tableName + "Service.cs");
            var iservicePath = Path.Combine(backendPath, "Controller", "I" + tableName + "Service.cs");
            var entityPath = Path.Combine(backendPath, "Models", "Entity", tableName + "Entity.cs");
            var mapperPath = Path.Combine(backendPath, "Models", "Mapper", tableName + "Mapper.cs");
            var inputCrPath = Path.Combine(backendPath, "Models", "Dto", tableName + "CrInput.cs");
            var inputUpPath = Path.Combine(backendPath, "Models", "Dto", tableName + "UpInput.cs");
            var outputInfoPath = Path.Combine(backendPath, "Models", "Dto", tableName + "InfoOutput.cs");
            return new List<string>()
            {
                servicePath,
                iservicePath,
                entityPath,
                mapperPath,
                inputCrPath,
                inputUpPath,
                outputInfoPath
            };
        }

        #endregion

        #region 后端副表

        /// <summary>
        /// 获取后端副表模板文件路径集合
        /// </summary>
        /// <returns></returns>
        private List<string> GetAuxiliaryTableBackendTemplatePathList()
        {
            var templatePath = Path.Combine(App.WebHostEnvironment.WebRootPath, "Template");
            return new List<string>()
                    {
                        Path.Combine(templatePath , "AuxiliaryEntity.cs.vm"),
                        Path.Combine(templatePath , "AuxiliaryMapper.cs.vm")
                    };
        }

        /// <summary>
        /// 获取后端主表生成文件路径
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private List<string> GetAuxiliaryTableBackendTargetPathList(DbTableRelationModel tableInfo, string fileName)
        {
            var backendPath = Path.Combine(FileVariable.GenerateCodePath, fileName, "Net");
            var tableName = tableInfo.table.ToPascalCase();
            var entityPath = Path.Combine(backendPath, "Models", "Entity", tableName + "Entity.cs");
            var mapperPath = Path.Combine(backendPath, "Models", "Mapper", tableName + "Mapper.cs");
            return new List<string>()
                    {
                        entityPath,
                        mapperPath
                    };
        }

        #endregion

        #region 后端关系表

        /// <summary>
        /// 获取后端主表模板文件路径集合
        /// </summary>
        /// <param name="webType">页面类型（1、纯表单，2、表单加列表，3、表单列表工作流）</param>
        /// <returns></returns>
        private List<string> GetRelationTableBackendTemplatePathList(int webType)
        {
            var templatePath = Path.Combine(App.WebHostEnvironment.WebRootPath, "Template");
            switch (webType)
            {
                case 1:
                    return new List<string>()
                    {
                        Path.Combine(templatePath , "Entity.cs.vm"),
                        Path.Combine(templatePath , "Mapper.cs.vm"),
                        Path.Combine(templatePath , "CrInput.cs.vm")
                    };
                case 2:
                case 3:
                    return new List<string>()
                    {
                        Path.Combine(templatePath , "Entity.cs.vm"),
                        Path.Combine(templatePath , "Mapper.cs.vm"),
                        Path.Combine(templatePath , "CrInput.cs.vm"),
                        Path.Combine(templatePath , "UpInput.cs.vm"),
                        Path.Combine(templatePath , "InfoOutput.cs.vm")
                    };
            }
            return null;
        }

        /// <summary>
        /// 获取后端关系表生成文件路径
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="fileName"></param>
        /// <param name="webType">页面类型（1、纯表单，2、表单加列表，3、表单列表工作流）</param>
        /// <returns></returns>
        private List<string> GetRelationTableBackendTargetPathList(DbTableRelationModel tableInfo, string fileName, int webType)
        {
            var backendPath = Path.Combine(FileVariable.GenerateCodePath, fileName, "Net");
            var tableName = tableInfo.table.ToPascalCase();
            var entityPath = Path.Combine(backendPath, "Models", "Entity", tableName + "Entity.cs");
            var mapperPath = Path.Combine(backendPath, "Models", "Mapper", tableName + "Mapper.cs");
            var inputCrPath = Path.Combine(backendPath, "Models", "Dto", tableName + "CrInput.cs");
            var inputUpPath = Path.Combine(backendPath, "Models", "Dto", tableName + "UpInput.cs");
            var outputInfoPath = Path.Combine(backendPath, "Models", "Dto", tableName + "InfoOutput.cs");

            switch (webType)
            {
                case 1:
                    return new List<string>()
                    {
                        entityPath,
                        mapperPath,
                        inputCrPath
                    };
                case 2:
                    return new List<string>()
                    {
                        entityPath,
                        mapperPath,
                        inputCrPath,
                        inputUpPath,
                        outputInfoPath
                    };
                case 3:
                    return new List<string>()
                    {
                        entityPath,
                        mapperPath,
                        inputCrPath,
                        inputUpPath,
                        outputInfoPath
                    };
            }
            return null;
        }

        /// <summary>
        /// 获取流程主表模板文件路径集合
        /// </summary>
        /// <returns></returns>
        private List<string> GetFlowRelationTableBackendTemplatePathList()
        {
            var templatePath = Path.Combine(App.WebHostEnvironment.WebRootPath, "Template");
            return new List<string>()
            {
                Path.Combine(templatePath , "Entity.cs.vm"),
                Path.Combine(templatePath , "Mapper.cs.vm"),
                Path.Combine(templatePath , "CrInput.cs.vm"),
                Path.Combine(templatePath , "UpInput.cs.vm"),
                Path.Combine(templatePath , "InfoOutput.cs.vm")
            };
        }

        /// <summary>
        /// 获取流程后端关系表生成文件路径
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private List<string> GetFlowRelationTableBackendTargetPathList(DbTableRelationModel tableInfo, string fileName)
        {
            var backendPath = Path.Combine(FileVariable.GenerateCodePath, fileName, "Net");
            var tableName = tableInfo.table.ToPascalCase();
            var entityPath = Path.Combine(backendPath, "Models", "Entity", tableName + "Entity.cs");
            var mapperPath = Path.Combine(backendPath, "Models", "Mapper", tableName + "Mapper.cs");
            var inputCrPath = Path.Combine(backendPath, "Models", "Dto", tableName + "CrInput.cs");
            var inputUpPath = Path.Combine(backendPath, "Models", "Dto", tableName + "UpInput.cs");
            var outputInfoPath = Path.Combine(backendPath, "Models", "Dto", tableName + "InfoOutput.cs");
            return new List<string>()
            {
                entityPath,
                mapperPath,
                inputCrPath,
                inputUpPath,
                outputInfoPath
            };
        }

        #endregion

        #region 前端页面

        /// <summary>
        /// 获取前端页面模板文件路径集合
        /// </summary>
        /// <param name="webType">页面类型（1、纯表单，2、表单加列表，3、表单列表工作流）</param>
        /// <param name="isDetail">是否有详情</param>
        /// <returns></returns>
        private List<string> GetFrontendTemplatePathList(int webType, bool isDetail = false)
        {
            var templatePath = Path.Combine(App.WebHostEnvironment.WebRootPath, "Template");
            var pathList = new List<string>();
            switch (webType)
            {
                case 1:
                    {
                        pathList.Add(Path.Combine(templatePath, "Form.vue.vm"));
                    }
                    break;
                case 2:
                    {
                        pathList.Add(Path.Combine(templatePath, "index.vue.vm"));
                        pathList.Add(Path.Combine(templatePath, "Form.vue.vm"));
                        if (isDetail)
                        {
                            pathList.Add(Path.Combine(templatePath, "Detail.vue.vm"));
                        }
                        pathList.Add(Path.Combine(templatePath, "ExportBox.vue.vm"));
                    }
                    break;
                case 3:
                    {
                        pathList.Add(Path.Combine(templatePath, "WorkflowIndex.vue.vm"));
                        pathList.Add(Path.Combine(templatePath, "WorkflowForm.vue.vm"));
                        if (isDetail)
                        {
                            pathList.Add(Path.Combine(templatePath, "Detail.vue.vm"));
                        }
                        pathList.Add(Path.Combine(templatePath, "ExportBox.vue.vm"));
                        pathList.Add(Path.Combine(templatePath, "ExportJson.json.vm"));
                    }
                    break;
                default:
                    break;
            }
            return pathList;
        }

        /// <summary>
        /// 设置前端页面生成文件路径
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="fileName"></param>
        /// <param name="webType">页面类型（1、纯表单，2、表单加列表，3、表单列表工作流）</param>
        /// <param name="isDetail">是否有详情</param>
        /// <returns></returns>
        private List<string> GetFrontendTargetPathList(DbTableRelationModel tableInfo, string fileName, int webType, bool isDetail = false)
        {
            var frontendPath = Path.Combine(FileVariable.GenerateCodePath, fileName, "Net");
            var tableName = tableInfo.table.ToPascalCase().LowerFirstChar();
            var indexPath = Path.Combine(frontendPath, "html", tableName, "index.vue");
            var formPath = Path.Combine(frontendPath, "html", tableName, "Form.vue");
            var detailPath = Path.Combine(frontendPath, "html", tableName, "Detail.vue");
            var exportBoxPath = Path.Combine(frontendPath, "html", tableName, "ExportBox.vue");
            var exportJsonPath = Path.Combine(frontendPath, "json", "ExportJson.json");
            var pathList = new List<string>();
            switch (webType)
            {
                case 1:
                    pathList.Add(indexPath);
                    break;
                case 2:
                    pathList.Add(indexPath);
                    pathList.Add(formPath);
                    if (isDetail)
                    {
                        pathList.Add(detailPath);
                    }
                    pathList.Add(exportBoxPath);
                    break;
                case 3:
                    pathList.Add(indexPath);
                    pathList.Add(formPath);
                    if (isDetail)
                    {
                        pathList.Add(detailPath);
                    }
                    pathList.Add(exportBoxPath);
                    pathList.Add(exportJsonPath);
                    break;
            }
            return pathList;
        }

        /// <summary>
        /// 获取App前端页面模板文件路径集合
        /// </summary>
        /// <param name="webType"></param>
        /// <returns></returns>
        private List<string> GetAppFrontendTemplatePathList(int webType)
        {
            var templatePath = Path.Combine(App.WebHostEnvironment.WebRootPath, "Template");
            switch (webType)
            {
                case 1:
                    return new List<string>()
                    {
                        Path.Combine(templatePath , "appForm.vue.vm")
                    };
                case 2:
                    return new List<string>()
                    {
                        Path.Combine(templatePath , "appIndex.vue.vm"),
                        Path.Combine(templatePath , "appForm.vue.vm")
                    };
                case 3:
                    return new List<string>()
                    {
                        Path.Combine(templatePath , "appWorkflowIndex.vue.vm"),
                        Path.Combine(templatePath , "appWorkflowForm.vue.vm"),
                        Path.Combine(templatePath , "ExportJson.json.vm")
                    };
                default:
                    break;
            }
            return null;
        }

        /// <summary>
        /// 设置App前端页面生成文件路径
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="fileName"></param>
        /// <param name="webType">页面类型（1、纯表单，2、表单加列表，3、表单列表工作流）</param>
        /// <returns></returns>
        private List<string> GetAppFrontendTargetPathList(DbTableRelationModel tableInfo, string fileName, int webType)
        {
            var frontendPath = Path.Combine(FileVariable.GenerateCodePath, fileName, "Net");
            var tableName = tableInfo.table.ToPascalCase().LowerFirstChar();
            var indexPath = Path.Combine(frontendPath, "html", tableName, "index.vue");
            var formPath = Path.Combine(frontendPath, "html", tableName, "form.vue");
            var exportJsonPath = Path.Combine(frontendPath, "json", "ExportJson.json");
            switch (webType)
            {
                case 1:
                    return new List<string>()
                    {
                        indexPath
                    };
                case 2:
                    return new List<string>()
                    {
                        indexPath,
                        formPath,
                    };
                case 3:
                    return new List<string>()
                    {
                        Path.Combine(frontendPath, "html", "app","index", "index.vue"),
                        Path.Combine(frontendPath, "html", "app","form", "index.vue"),
                        exportJsonPath
                    };
            }
            return null;
        }

        /// <summary>
        /// 获取流程前端页面模板文件路径集合
        /// </summary>
        /// <returns></returns>
        private List<string> GetFlowFrontendTemplatePathList()
        {
            var templatePath = Path.Combine(App.WebHostEnvironment.WebRootPath, "Template");
            return new List<string>()
            {
                Path.Combine(templatePath , "WorkflowForm.vue.vm"),
                Path.Combine(templatePath , "appWorkflowForm.vue.vm")
            };
        }

        /// <summary>
        /// 设置流程前端页面生成文件路径
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private List<string> GetFlowFrontendTargetPathList(DbTableRelationModel tableInfo, string fileName)
        {
            var frontendPath = Path.Combine(FileVariable.GenerateCodePath, fileName, "Net");
            var tableName = tableInfo.table.ToPascalCase().LowerFirstChar();
            var indexPath = Path.Combine(frontendPath, "html", "PC", tableName, "index.vue");
            var indexAppPath = Path.Combine(frontendPath, "html", "APP", tableName, "index.vue");
            return new List<string>()
            {
                indexPath,
                indexAppPath
            };
        }

        #endregion

        /// <summary>
        /// 获取主表字段名
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private string GetMainTableColumnField(List<IndexGridFieldModel> list)
        {
            StringBuilder columnSb = new StringBuilder();
            foreach (var item in list)
            {
                columnSb.AppendFormat("{{\\\"value\\\":\\\"{0}\\\",\\\"field\\\":\\\"{1}\\\"}},", item.label, item.prop);
            }
            return columnSb.ToString();
        }

        /// <summary>
        /// 获取是否查询列
        /// </summary>
        /// <param name="searchList"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        private bool GetIsColumnQueryWhether(List<FieldsModel> searchList, string alias)
        {
            if (searchList != null)
            {
                var column = searchList.Find(s => s.__vModel__ == alias);
                return column == null ? false : true;
            }
            return false;
        }

        /// <summary>
        /// 获取列查询类型
        /// </summary>
        /// <param name="searchList"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        private int GetColumnQueryType(List<FieldsModel> searchList, string alias)
        {
            if (searchList != null)
            {
                var column = searchList.Find(s => s.__vModel__ == alias);
                return column == null ? 0 : column.searchType;
            }
            return 0;
        }

        /// <summary>
        /// 获取是否展示列
        /// </summary>
        /// <param name="columnList"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        private bool GetIsShowColumn(List<IndexGridFieldModel> columnList, string alias)
        {
            if (columnList != null)
            {
                var column = columnList.Find(s => s.prop == alias);
                return column == null ? false : true;
            }
            return false;
        }

        /// <summary>
        /// 获取是否多选
        /// </summary>
        /// <param name="columnList"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        public bool GetIsMultipleColumn(List<FieldsModel> columnList, string alias)
        {
            bool isMultiple = false;
            var column = columnList.Find(s => s.__vModel__ == alias);
            if (column != null)
            {
                switch (column.__config__.hszKey)
                {
                    case "cascader":
                        isMultiple = column.props.props.multiple;
                        break;
                    default:
                        isMultiple = column.multiple;
                        break;
                }
            }
            return isMultiple;
        }

        /// <summary>
        /// 获取代码生成方法列表
        /// </summary>
        /// <param name="hasPage">是否分页</param>
        /// <param name="btnsList">头部按钮</param>
        /// <param name="columnBtnsList">列表按钮</param>
        /// <param name="webType">页面类型（1、纯表单，2、表单加列表，3、表单列表工作流）</param>
        /// <returns></returns>
        private List<CodeGenFunctionModel> GetCodeGenFunctionList(bool hasPage, List<ButtonConfigModel> btnsList, List<ButtonConfigModel> columnBtnsList, int webType)
        {
            List<CodeGenFunctionModel> functionList = new List<CodeGenFunctionModel>();
            switch (webType)
            {
                case 1:
                    functionList.Add(new CodeGenFunctionModel()
                    {
                        FullName = "add",
                        IsInterface = true
                    });
                    break;
                default:
                    //信息
                    functionList.Add(new CodeGenFunctionModel()
                    {
                        FullName = "info",
                        IsInterface = true
                    });
                    if (!hasPage)
                        functionList.Add(new CodeGenFunctionModel()
                        {
                            FullName = "noPage",
                            IsInterface = true
                        });
                    else
                        functionList.Add(new CodeGenFunctionModel()
                        {
                            FullName = "page",
                            IsInterface = true
                        });

                    btnsList.ForEach(b =>
                    {
                        if (b.value == "download" && !hasPage)
                            functionList.Add(new CodeGenFunctionModel()
                            {
                                FullName = "page",
                                IsInterface = false
                            });
                        else if (b.value == "download" && hasPage)
                            functionList.Add(new CodeGenFunctionModel()
                            {
                                FullName = "noPage",
                                IsInterface = false
                            });
                        functionList.Add(new CodeGenFunctionModel()
                        {
                            FullName = b.value,
                            IsInterface = true
                        });
                    });
                    columnBtnsList.ForEach(c =>
                    {
                        functionList.Add(new CodeGenFunctionModel()
                        {
                            FullName = c.value,
                            IsInterface = true
                        });
                    });
                    break;
            }
            return functionList;
        }

        /// <summary>
        /// 获取代码生成流程方法列表
        /// </summary>
        /// <returns></returns>
        private List<CodeGenFunctionModel> GetCodeGenFlowFunctionList()
        {
            List<CodeGenFunctionModel> functionList = new List<CodeGenFunctionModel>();
            //信息
            functionList.Add(new CodeGenFunctionModel()
            {
                FullName = "info",
                IsInterface = true
            });
            functionList.Add(new CodeGenFunctionModel()
            {
                FullName = "add",
                IsInterface = true
            });
            functionList.Add(new CodeGenFunctionModel()
            {
                FullName = "edit",
                IsInterface = true
            });
            functionList.Add(new CodeGenFunctionModel()
            {
                FullName = "remove",
                IsInterface = true
            });
            return functionList;
        }

        /// <summary>
        /// 获取代码生成表关系列表
        /// </summary>
        /// <param name="tableRelation">全部表列表</param>
        /// <param name="currentTable">当前表</param>
        /// <param name="link">连接ID</param>
        /// <param name="fieldList">子表控件</param>
        /// <returns></returns>
        private List<CodeGenTableRelationsModel> GetCodeGenTableRelationList(List<DbTableRelationModel> tableRelation, string currentTable, DbLinkEntity link, List<FieldsModel> fieldList)
        {
            List<CodeGenTableRelationsModel> tableRelationsList = new List<CodeGenTableRelationsModel>();
            if (tableRelation.Count > 0)
            {
                var relationTable = tableRelation.Find(t => t.table.Equals(currentTable)).relationTable;
                if (!relationTable.IsNotEmptyOrNull())
                {
                    var tableNo = 1;
                    tableRelation.ForEach(t =>
                    {
                        List<TableColumnConfigModel> tableColumnConfigList = new List<TableColumnConfigModel>();
                        if (t.relationTable.IsNotEmptyOrNull())
                        {
                            var table = _databaseService.GetFieldList(link, t.table);
                            var field = fieldList.Find(it => it.__config__.tableName.Equals(t.table));
                            if (field != null)
                            {
                                foreach (var column in table)
                                {
                                    var columnName = GetReplaceDefault(column.field).ToPascalCase();
                                    var control = field.__config__.children.Find(it => it.__vModel__.Equals(columnName.LowerFirstChar()));
                                    tableColumnConfigList.Add(new TableColumnConfigModel()
                                    {
                                        ColumnName = columnName,
                                        Alias = column.field,
                                        OriginalColumnName = column.field,
                                        ColumnComment = column.fieldName,
                                        DataType = column.dataType,
                                        NetType = CodeGenUtil.ConvertDataType(column.dataType),
                                        PrimaryKey = column.primaryKey.ToBool(),
                                        IsMultiple = GetIsMultipleColumn(fieldList, column.field),
                                        hszKey = control != null ? control.__config__.hszKey : null,
                                        Rule = control != null ? control.__config__.rule : null
                                    });
                                }
                            }
                            tableRelationsList.Add(new CodeGenTableRelationsModel()
                            {
                                TableName = t.table.ToPascalCase(),
                                PrimaryKey = GetReplaceDefault(table.Find(t => t.primaryKey == 1).field).ToPascalCase(),
                                TableField = GetReplaceDefault(t.tableField).ToPascalCase(),
                                RelationField = GetReplaceDefault(t.relationField).ToPascalCase(),
                                TableComment = t.tableName,
                                ChilderColumnConfigList = tableColumnConfigList,
                                TableNo = tableNo,
                            });
                            tableNo++;
                        }
                    });
                }
            }
            return tableRelationsList;
        }

        /// <summary>
        /// 获取代码生成常规Index列表头部按钮方法
        /// </summary>
        /// <returns></returns>
        private string GetCodeGenConvIndexListTopButtonMethod(string value)
        {
            var method = string.Empty;
            switch (value)
            {
                case "add":
                    method = "addOrUpdateHandle()";
                    break;
                case "download":
                    method = "exportData()";
                    break;
                case "batchRemove":
                    method = "handleBatchRemoveDel()";
                    break;
                default:
                    break;
            }
            return method;
        }

        /// <summary>
        /// 获取代码生成常规Index列表列按钮方法
        /// </summary>
        /// <returns></returns>
        private string GetCodeGenConvIndexListColumnButtonMethod(string value, string primaryKey, int auxiliaryTableNum)
        {
            var method = string.Empty;
            switch (value)
            {
                case "edit":
                    method = String.Format("addOrUpdateHandle(scope.row.{0})", auxiliaryTableNum == 0 ? primaryKey : string.Format("a_{0}", primaryKey));
                    break;
                case "remove":
                    method = String.Format("handleDel(scope.row.{0})", auxiliaryTableNum == 0 ? primaryKey : string.Format("a_{0}", primaryKey));
                    break;
                case "detail":
                    method = String.Format("goDetail(scope.row.{0})", auxiliaryTableNum == 0 ? primaryKey : string.Format("a_{0}", primaryKey));
                    break;
                default:
                    break;
            }
            return method;
        }

        /// <summary>
        /// 获取代码生成流程Index列表列按钮是否禁用
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetCodeGenWorkflowIndexListColumnButtonDisabled(string value)
        {
            var disabled = string.Empty;
            switch (value)
            {
                case "edit":
                    disabled = ":disabled = '[1, 2, 5].indexOf(scope.row.flowState) > -1' ";
                    break;
                case "remove":
                    disabled = ":disabled = '[1, 2, 3, 5].indexOf(scope.row.flowState) > -1' ";
                    break;
            }
            return disabled;
        }

        /// <summary>
        /// 获取常规index列表控件Option
        /// </summary>
        /// <param name="name"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private string GetCodeGenConvIndexListControlOption(string name, List<Dictionary<string, object>> options)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"{name}Options:");
            sb.Append("[");
            var list = options.ToObject<List<Dictionary<string, object>>>();
            foreach (var valueItem in list)
            {
                sb.Append("{");
                foreach (var items in valueItem)
                {
                    sb.Append($"'{items.Key}':{items.Value.ToJson()},");
                }
                sb = new StringBuilder(sb.ToString().TrimEnd(','));
                sb.Append("},");
            }
            sb = new StringBuilder(sb.ToString().TrimEnd(','));
            sb.Append("],");

            return sb.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private string GetCodeGenConvIndexListControlOption(string name, List<object> options)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"{name}Options:{options.ToJson()},");

            return sb.ToString();
        }

        /// <summary>
        /// 获取表单全部控件列表
        /// </summary>
        /// <param name="fieldList"></param>
        /// <param name="gutter"></param>
        /// <param name="isMain">是否主循环</param>
        /// <param name="labelWidth">是否主循环</param>
        /// <returns></returns>
        private List<CodeGenFormAllControlsDesign> GetFormAllControlsList(List<FieldsModel> fieldList, int gutter, int labelWidth, bool isMain = false)
        {
            List<CodeGenFormAllControlsDesign> list = new List<CodeGenFormAllControlsDesign>();
            foreach (var item in fieldList)
            {
                var config = item.__config__;
                switch (config.hszKey)
                {
                    //栅格布局
                    case "row":
                        {
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                hszKey = config.hszKey,
                                Span = config.span,
                                Gutter = gutter,
                                Children = GetFormAllControlsList(config.children, gutter, labelWidth)
                            });
                        }
                        break;
                    //表格
                    case "table":
                        {
                            List<CodeGenFormAllControlsDesign> childrenTableList = new List<CodeGenFormAllControlsDesign>();
                            foreach (var children in config.children)
                            {
                                var childrenConfig = children.__config__;
                                switch (childrenConfig.hszKey)
                                {
                                    //关联表单属性
                                    //弹窗选择属性
                                    case "relationFormAttr":
                                    case "popupAttr":
                                        {
                                            var relationField = Regex.Match(children.relationField, @"^(.+)_hszTable_").Groups[1].Value;
                                            childrenTableList.Add(new CodeGenFormAllControlsDesign()
                                            {
                                                hszKey = childrenConfig.hszKey,
                                                RelationField = relationField,
                                                ShowField = children.showField,
                                                Tag = childrenConfig.tag,
                                                Label = childrenConfig.label,
                                                Span = childrenConfig.span,
                                                LabelWidth = string.IsNullOrEmpty(childrenConfig.labelWidth) ? labelWidth.ToString() : childrenConfig.labelWidth,
                                                ColumnWidth = childrenConfig.columnWidth != null ? $"width='{childrenConfig.columnWidth}' " : null,
                                            });
                                        }
                                        break;
                                    default:
                                        {
                                            childrenTableList.Add(new CodeGenFormAllControlsDesign()
                                            {
                                                hszKey = childrenConfig.hszKey,
                                                Name = children.__vModel__,
                                                OriginalName = children.__vModel__,
                                                Style = !string.IsNullOrEmpty(children.type) ? $":style='{children.style.ToJson()}' " : "",
                                                Span = childrenConfig.span,
                                                Placeholder = children.placeholder != null ? $"placeholder='{children.placeholder}' " : "",
                                                Clearable = children.clearable ? "clearable " : "",
                                                Readonly = children.@readonly ? "readonly " : "",
                                                Disabled = children.disabled ? "disabled " : "",
                                                IsDisabled = item.disabled ? "disabled " : $":disabled=\"judgeWrite('{config.tableName.ToPascalCase().LowerFirstChar()}')\" ",
                                                ShowWordLimit = children.showwordlimit ? "show-word-limit " : "",
                                                Type = children.type != null ? $"type='{children.type}' " : "",
                                                Format = children.format != null ? $"format='{children.format}' " : "",
                                                ValueFormat = children.valueformat != null ? $"value-format='{children.valueformat}' " : "",
                                                AutoSize = children.autosize != null ? $":autosize='{children.autosize.ToJson()}' " : "",
                                                Multiple = children.multiple ? $"multiple " : "",
                                                Size = childrenConfig.optionType != null ? (childrenConfig.optionType == "default" ? "" : $"size='{children.size}' ") : "",
                                                PrefixIcon = !string.IsNullOrEmpty(children.prefixicon) ? $"prefix-icon='{children.prefixicon}' " : "",
                                                SuffixIcon = !string.IsNullOrEmpty(children.suffixicon) ? $"suffix-icon='{children.suffixicon}' " : "",
                                                MaxLength = !string.IsNullOrEmpty(children.maxlength) ? $"maxlength='{children.maxlength}' " : "",
                                                ShowPassword = children.showPassword ? "show-password " : "",
                                                Filterable = children.filterable ? "filterable " : "",
                                                Label = childrenConfig.label,
                                                Props = childrenConfig.props,
                                                MainProps = children.props != null ? $":props='{children.__vModel__}Props' " : "",
                                                Tag = childrenConfig.tag,
                                                Options = children.options != null ? $":options='{children.__vModel__}Options' " : "",
                                                ShowAllLevels = children.showalllevels ? "show-all-levels " : "",
                                                Separator = !string.IsNullOrEmpty(children.separator) ? $"separator='{children.separator}' " : "",
                                                RangeSeparator = !string.IsNullOrEmpty(children.rangeseparator) ? $"range-separator='{children.rangeseparator}' " : "",
                                                StartPlaceholder = !string.IsNullOrEmpty(children.startplaceholder) ? $"start-placeholder='{children.startplaceholder}' " : "",
                                                EndPlaceholder = !string.IsNullOrEmpty(children.endplaceholder) ? $"end-placeholder='{children.endplaceholder}' " : "",
                                                PickerOptions = children.pickeroptions != null && children.pickeroptions.ToJson() != "null" ? $":picker-options='{children.pickeroptions.ToJson()}' " : "",
                                                Required = childrenConfig.required ? "required " : "",
                                                Step = children.step != 0 ? $":step='{children.step}' " : "",
                                                StepStrictly = children.stepstrictly ? "step-strictly " : "",
                                                Max = children.max != 0 ? $":max='{children.max}' " : "",
                                                Min = children.min != 0 ? $":min='{children.min}' " : "",
                                                ColumnWidth = childrenConfig.columnWidth != null ? $"width='{childrenConfig.columnWidth}' " : null,
                                                ModelId = children.modelId != null ? $"modelId='{children.modelId}' " : "",
                                                RelationField = children.relationField != null ? $"relationField='{children.relationField}' " : "",
                                                ColumnOptions = children.columnOptions != null ? $":columnOptions='{children.__vModel__}Options' " : "",
                                                HasPage = children.hasPage ? "hasPage " : "",
                                                PageSize = children.pageSize != 0 ? $":pageSize='{children.pageSize}' " : "",
                                                PropsValue = children.propsValue != null ? $"propsValue='{children.propsValue}' " : "",
                                                InterfaceId = children.interfaceId != null ? $"interfaceId='{children.interfaceId}' " : "",
                                                Precision = children.precision != 0 ? $":precision='{children.precision}' " : "",
                                                ActiveText = !string.IsNullOrEmpty(children.activetext) ? $"active-text='{children.activetext}' " : "",
                                                InactiveText = !string.IsNullOrEmpty(children.inactivetext) ? $"inactive-text='{children.inactivetext}' " : "",
                                                ActiveColor = !string.IsNullOrEmpty(children.activecolor) ? $"active-color='{children.activecolor}' " : "",
                                                InactiveColor = !string.IsNullOrEmpty(children.inactivecolor) ? $"inactive-color='{children.inactivecolor}' " : "",
                                                IsSwitch = childrenConfig.hszKey == "switch" ? $":active-value='{children.activevalue}' :inactive-value='{children.inactivevalue}' " : "",
                                                ShowStops = children.showstops ? $"show-stops " : "",
                                                Accept = !string.IsNullOrEmpty(children.accept) ? $"accept='{children.accept}' " : "",
                                                ShowTip = children.showTip ? $"showTip " : "",
                                                FileSize = children.fileSize != 0 && !string.IsNullOrEmpty(children.fileSize.ToString()) ? $":fileSize='{children.fileSize}' " : "",
                                                SizeUnit = !string.IsNullOrEmpty(children.sizeUnit) ? $"sizeUnit='{children.sizeUnit}' " : "",
                                                Limit = children.limit != 0 ? $":limit='{children.limit}' " : "",
                                                ButtonText = !string.IsNullOrEmpty(children.buttonText) ? $"buttonText='{children.buttonText}' " : "",
                                                Level = childrenConfig.hszKey == "address" ? $":level='{children.level}' " : "",
                                                NoShow = childrenConfig.noShow ? "v-if='false' " : "",
                                                Prepend = children.__slot__ != null && !string.IsNullOrEmpty(children.__slot__.prepend) ? children.__slot__.prepend : null,
                                                Append = children.__slot__ != null && !string.IsNullOrEmpty(children.__slot__.append) ? children.__slot__.append : null,
                                                ShowLevel = !string.IsNullOrEmpty(children.showLevel) ? "" : "",
                                                LabelWidth = string.IsNullOrEmpty(childrenConfig.labelWidth) ? labelWidth.ToString() : childrenConfig.labelWidth,
                                                PopupType = !string.IsNullOrEmpty(children.popupType) ? $"popupType='{children.popupType}' " : "",
                                                PopupTitle = !string.IsNullOrEmpty(children.popupTitle) ? $"popupTitle='{children.popupTitle}' " : "",
                                                PopupWidth = !string.IsNullOrEmpty(children.popupWidth) ? $"popupWidth='{children.popupWidth}' " : "",
                                                Field = childrenConfig.hszKey.Equals("relationForm") || childrenConfig.hszKey.Equals("popupSelect") ? $":field=\"'{children.__vModel__}'+scope.$index\" " : "",
                                            });
                                        }
                                        break;
                                }
                            }
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                hszKey = config.hszKey,
                                Name = item.__vModel__,
                                OriginalName = config.tableName,
                                Span = config.span,
                                ShowText = config.showTitle,
                                Label = config.label,
                                ChildTableName = config.tableName.ToPascalCase(),
                                Children = childrenTableList,
                                LabelWidth = string.IsNullOrEmpty(config.labelWidth) ? labelWidth.ToString() : config.labelWidth,
                                ShowSummary = item.showSummary
                            });
                        }
                        break;
                    //卡片
                    case "card":
                        {
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                hszKey = config.hszKey,
                                OriginalName = item.__vModel__,
                                Shadow = item.shadow,
                                Children = GetFormAllControlsList(config.children, gutter, labelWidth),
                                Span = config.span,
                                Content = item.header,
                                LabelWidth = string.IsNullOrEmpty(config.labelWidth) ? labelWidth.ToString() : config.labelWidth,
                            });
                        }
                        break;
                    //分割线
                    case "divider":
                        {
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                hszKey = config.hszKey,
                                OriginalName = item.__vModel__,
                                Span = config.span,
                                Contentposition = item.contentposition,
                                Default = item.__slot__.@default,
                                LabelWidth = string.IsNullOrEmpty(config.labelWidth) ? labelWidth.ToString() : config.labelWidth,
                            });
                        }
                        break;
                    //折叠面板
                    case "collapse":
                        {
                            //先加为了防止 children下 还有折叠面板
                            List<CodeGenFormAllControlsDesign> childrenCollapseList = new List<CodeGenFormAllControlsDesign>();
                            foreach (var children in config.children)
                            {
                                childrenCollapseList.Add(new CodeGenFormAllControlsDesign()
                                {
                                    Title = children.title,
                                    Name = children.name,
                                    Gutter = gutter,
                                    Children = GetFormAllControlsList(children.__config__.children, gutter, labelWidth)
                                });
                            }
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                hszKey = config.hszKey,
                                Accordion = item.accordion ? "true" : "false",
                                Name = "active" + active++,
                                Active = childrenCollapseList.Select(it => it.Name).ToJson(),
                                Children = childrenCollapseList,
                                Span = config.span,
                                LabelWidth = string.IsNullOrEmpty(config.labelWidth) ? labelWidth.ToString() : config.labelWidth,
                            });
                        }
                        break;
                    //tab标签
                    case "tab":
                        {
                            //先加为了防止 children下 还有折叠面板
                            List<CodeGenFormAllControlsDesign> childrenCollapseList = new List<CodeGenFormAllControlsDesign>();
                            foreach (var children in config.children)
                            {
                                childrenCollapseList.Add(new CodeGenFormAllControlsDesign()
                                {
                                    Title = children.title,
                                    Gutter = gutter,
                                    Children = GetFormAllControlsList(children.__config__.children, gutter, labelWidth)
                                });
                            }
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                hszKey = config.hszKey,
                                Type = item.type,
                                TabPosition = item.tabPosition,
                                Name = "active" + active++,
                                Active = config.active.ToString(),
                                Children = childrenCollapseList,
                                Span = config.span,
                                LabelWidth = string.IsNullOrEmpty(config.labelWidth) ? labelWidth.ToString() : config.labelWidth,
                            });
                        }
                        break;
                    //分组标题
                    case "groupTitle":
                        {
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                hszKey = config.hszKey,
                                Span = config.span,
                                Contentposition = item.contentposition,
                                Content = item.content,
                                LabelWidth = string.IsNullOrEmpty(config.labelWidth) ? labelWidth.ToString() : config.labelWidth,
                            });
                        }
                        break;
                    //文本
                    case "HSZText":
                        {
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                hszKey = config.hszKey,
                                Span = config.span,
                                DefaultValue = config.defaultValue,
                                TextStyle = item.textStyle != null ? item.textStyle.ToJson() : "",
                                Style = item.style.ToJson(),
                                LabelWidth = string.IsNullOrEmpty(config.labelWidth) ? labelWidth.ToString() : config.labelWidth,
                            });
                        }
                        break;
                    //按钮
                    case "button":
                        {
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                hszKey = config.hszKey,
                                Span = config.span,
                                Align = item.align,
                                ButtonText = item.buttonText,
                                Type = item.type,
                                Disabled = item.disabled ? "disabled " : "",
                                LabelWidth = string.IsNullOrEmpty(config.labelWidth) ? labelWidth.ToString() : config.labelWidth,
                            });
                        }
                        break;
                    //关联表单属性
                    //弹窗选择属性
                    case "relationFormAttr":
                    case "popupAttr":
                        {
                            var relationField = Regex.Match(item.relationField, @"^(.+)_hszTable_").Groups[1].Value;
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                hszKey = config.hszKey,
                                RelationField = relationField,
                                ShowField = item.showField,
                                Tag = config.tag,
                                Label = config.label,
                                Span = config.span,
                                LabelWidth = string.IsNullOrEmpty(config.labelWidth) ? labelWidth.ToString() : config.labelWidth,
                            });
                        }
                        break;
                    //常规
                    default:
                        {
                            string vModel = string.Empty;
                            string name = item.__vModel__;
                            var Model = name;
                            switch (config.hszKey)
                            {
                                default:
                                    vModel = $"v-model='dataForm.{Model}' ";
                                    break;
                            }
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                Name = item.__vModel__,
                                OriginalName = item.__vModel__,
                                hszKey = config.hszKey,
                                Border = config.border ? "border " : "",
                                Style = item.style != null ? $":style='{item.style.ToJson()}' " : "",
                                Type = !string.IsNullOrEmpty(item.type) ? $"type='{item.type}' " : "",
                                Span = config.span,
                                Clearable = item.clearable ? "clearable " : "",
                                Readonly = item.@readonly ? "readonly " : "",
                                Required = config.required ? "required " : "",
                                Placeholder = !string.IsNullOrEmpty(item.placeholder) ? $"placeholder='{item.placeholder}' " : "",
                                Disabled = item.disabled ? "disabled " : "",
                                IsDisabled = item.disabled ? "disabled " : $":disabled='judgeWrite(\"{ item.__vModel__}\")' ",
                                ShowWordLimit = item.showwordlimit ? "show-word-limit " : "",
                                Format = !string.IsNullOrEmpty(item.format) ? $"format='{item.format}' " : "",
                                ValueFormat = !string.IsNullOrEmpty(item.valueformat) ? $"value-format='{item.valueformat}' " : "",
                                AutoSize = item.autosize != null && item.autosize.ToJson() != "null" ? $":autosize='{item.autosize.ToJson()}' " : "",
                                Multiple = item.multiple ? $"multiple " : "",
                                IsRange = item.isrange ? "is-range " : "",
                                Props = config.props,
                                MainProps = item.props != null ? $":props='{Model}Props' " : "",
                                OptionType = config.optionType == "default" ? "" : "-button",
                                Size = !string.IsNullOrEmpty(config.optionType) ? (config.optionType == "default" ? "" : $"size='{item.size}' ") : "",
                                PrefixIcon = !string.IsNullOrEmpty(item.prefixicon) ? $"prefix-icon='{item.prefixicon}' " : "",
                                SuffixIcon = !string.IsNullOrEmpty(item.suffixicon) ? $"suffix-icon='{item.suffixicon}' " : "",
                                MaxLength = !string.IsNullOrEmpty(item.maxlength) ? $"maxlength='{item.maxlength}' " : "",
                                Step = item.step != 0 ? $":step='{item.step}' " : "",
                                StepStrictly = item.stepstrictly ? "step-strictly " : "",
                                ControlsPosition = !string.IsNullOrEmpty(item.controlsposition) ? $"controls-position='{item.controlsposition}' " : "",
                                ShowChinese = item.showChinese ? "showChinese " : "",
                                ShowPassword = item.showPassword ? "show-password " : "",
                                Filterable = item.filterable ? "filterable " : "",
                                ShowAllLevels = item.showalllevels ? "show-all-levels " : "",
                                Separator = !string.IsNullOrEmpty(item.separator) ? $"separator='{item.separator}' " : "",
                                RangeSeparator = !string.IsNullOrEmpty(item.rangeseparator) ? $"range-separator='{item.rangeseparator}' " : "",
                                StartPlaceholder = !string.IsNullOrEmpty(item.startplaceholder) ? $"start-placeholder='{item.startplaceholder}' " : "",
                                EndPlaceholder = !string.IsNullOrEmpty(item.endplaceholder) ? $"end-placeholder='{item.endplaceholder}' " : "",
                                PickerOptions = item.pickeroptions != null && item.pickeroptions.ToJson() != "null" ? $":picker-options='{item.pickeroptions.ToJson()}' " : "",
                                Options = item.options != null ? $":options='{item.__vModel__}Options' " : "",
                                Max = item.max != 0 ? $":max='{item.max}' " : "",
                                AllowHalf = item.allowhalf ? "allow-half " : "",
                                ShowTexts = item.showtext ? $"show-text " : "",
                                ShowScore = item.showScore ? $"show-score " : "",
                                ShowAlpha = item.showalpha ? $"show-alpha " : "",
                                ColorFormat = !string.IsNullOrEmpty(item.colorformat) ? $"color-format='{item.colorformat}' " : "",
                                ActiveText = !string.IsNullOrEmpty(item.activetext) ? $"active-text='{item.activetext}' " : "",
                                InactiveText = !string.IsNullOrEmpty(item.inactivetext) ? $"inactive-text='{item.inactivetext}' " : "",
                                ActiveColor = !string.IsNullOrEmpty(item.activecolor) ? $"active-color='{item.activecolor}' " : "",
                                InactiveColor = !string.IsNullOrEmpty(item.inactivecolor) ? $"inactive-color='{item.inactivecolor}' " : "",
                                IsSwitch = config.hszKey == "switch" ? $":active-value='{item.activevalue}' :inactive-value='{item.inactivevalue}' " : "",
                                Min = item.min != 0 ? $":min='{item.min}' " : "",
                                ShowStops = item.showstops ? $"show-stops " : "",
                                Range = item.range ? $"range " : "",
                                Accept = !string.IsNullOrEmpty(item.accept) ? $"accept='{item.accept}' " : "",
                                ShowTip = item.showTip ? $"showTip " : "",
                                FileSize = item.fileSize != 0 && !string.IsNullOrEmpty(item.fileSize.ToString()) ? $":fileSize='{item.fileSize}' " : "",
                                SizeUnit = !string.IsNullOrEmpty(item.sizeUnit) ? $"sizeUnit='{item.sizeUnit}' " : "",
                                Limit = item.limit != 0 ? $":limit='{item.limit}' " : "",
                                Contentposition = !string.IsNullOrEmpty(item.contentposition) ? $"content-position='{item.contentposition}' " : "",
                                ButtonText = !string.IsNullOrEmpty(item.buttonText) ? $"buttonText='{item.buttonText}' " : "",
                                Level = config.hszKey == "address" ? $":level='{item.level}' " : "",
                                ActionText = !string.IsNullOrEmpty(item.actionText) ? $"actionText='{item.actionText}' " : "",
                                Shadow = !string.IsNullOrEmpty(item.shadow) ? $"shadow='{item.shadow}' " : "",
                                Content = !string.IsNullOrEmpty(item.content) ? $"content='{item.content}' " : "",
                                NoShow = config.noShow ? "v-if='false' " : "",
                                Label = config.label,
                                vModel = vModel,
                                Prepend = item.__slot__ != null && !string.IsNullOrEmpty(item.__slot__.prepend) ? item.__slot__.prepend : null,
                                Append = item.__slot__ != null && !string.IsNullOrEmpty(item.__slot__.append) ? item.__slot__.append : null,
                                Tag = config.tag,
                                Count = item.max,
                                ModelId = item.modelId != null ? $"modelId='{item.modelId}' " : "",
                                RelationField = item.relationField != null ? $"relationField='{item.relationField}' " : "",
                                ColumnOptions = item.columnOptions != null ? $":columnOptions='{item.__vModel__}Options' " : "",
                                HasPage = item.hasPage ? "hasPage " : "",
                                PageSize = item.pageSize != 0 ? $":pageSize='{item.pageSize}' " : "",
                                PropsValue = item.propsValue != null ? $"propsValue='{item.propsValue}' " : "",
                                InterfaceId = item.interfaceId != null ? $"interfaceId='{item.interfaceId}' " : "",
                                Precision = item.precision != 0 ? $":precision='{item.precision}' " : "",
                                ShowLevel = !string.IsNullOrEmpty(item.showLevel) ? "" : "",
                                LabelWidth = string.IsNullOrEmpty(config.labelWidth) ? labelWidth.ToString() : config.labelWidth,
                                PopupType = !string.IsNullOrEmpty(item.popupType) ? $"popupType='{item.popupType}' " : "",
                                PopupTitle = !string.IsNullOrEmpty(item.popupTitle) ? $"popupTitle='{item.popupTitle}' " : "",
                                PopupWidth = !string.IsNullOrEmpty(item.popupWidth) ? $"popupWidth='{item.popupWidth}' " : "",
                                Field = config.hszKey.Equals("relationForm") || config.hszKey.Equals("popupSelect") ? $"field='{item.__vModel__}' " : "",
                            });
                        }
                        break;
                }
            }
            //还原为后面方法做准备
            if (isMain)
                active = 1;
            return list;
        }

        /// <summary>
        /// 生成表单Children无限级
        /// </summary>
        /// <param name="childrenList"></param>
        /// <param name="gutter"></param>
        /// <returns></returns>
        private List<CodeGenFormAllControlsDesign> GetFormChildrenControlsList(List<FieldsModel> childrenList, int gutter)
        {
            List<CodeGenFormAllControlsDesign> list = new List<CodeGenFormAllControlsDesign>();
            foreach (var item in childrenList)
            {
                var config = item.__config__;
                switch (config.hszKey)
                {
                    //栅格布局
                    case "row":
                        {
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                hszKey = config.hszKey,
                                Span = config.span,
                                Gutter = gutter,
                                Children = GetFormChildrenControlsList(config.children, gutter)
                            });
                        }
                        break;
                    //表格
                    case "table":
                        {
                            List<CodeGenFormAllControlsDesign> childrenTableList = new List<CodeGenFormAllControlsDesign>();
                            foreach (var children in config.children)
                            {
                                var childrenConfig = children.__config__;
                                childrenTableList.Add(new CodeGenFormAllControlsDesign()
                                {
                                    hszKey = childrenConfig.hszKey,
                                    Name = children.__vModel__,
                                    Style = !string.IsNullOrEmpty(children.type) ? $":style='{children.style.ToJson()}' " : "",
                                    Placeholder = children.placeholder != null ? $"placeholder='{children.placeholder}' " : "",
                                    Clearable = children.clearable ? "clearable " : "",
                                    Readonly = children.@readonly ? "readonly " : "",
                                    Disabled = children.disabled ? "disabled " : "",
                                    ShowWordLimit = children.showwordlimit ? "show-word-limit " : "",
                                    Type = children.type != null ? $"type='{children.type}' " : "",
                                    Format = children.format != null ? $"format='{children.format}' " : "",
                                    ValueFormat = children.valueformat != null ? $"value-format='{children.valueformat}' " : "",
                                    AutoSize = children.autosize != null ? $":autosize='{children.autosize.ToJson()}' " : "",
                                    Multiple = children.multiple ? $"multiple " : "",
                                    Size = childrenConfig.optionType != null ? (childrenConfig.optionType == "default" ? "" : $"size='{children.size}' ") : "",
                                    Label = childrenConfig.label,
                                    Props = childrenConfig.props,
                                    Tag = childrenConfig.tag,
                                    MainProps = children.props != null ? $":props='{children.__vModel__}Props'" : "",
                                    Options = children.options != null ? $":options='{children.__vModel__}Options' " : "",
                                    ShowAllLevels = children.showalllevels ? "show-all-levels " : "",
                                    Separator = !string.IsNullOrEmpty(children.separator) ? $"separator='{children.separator}' " : "",
                                    Required = childrenConfig.required ? "required " : "",
                                    Step = children.step != 0 ? $":step='{children.step}' " : "",
                                });
                            }
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                hszKey = config.hszKey,
                                Span = config.span,
                                ShowText = config.showTitle,
                                Label = config.label,
                                ChildTableName = item.__config__.tableName.ToPascalCase(),
                                Children = childrenTableList
                            });
                        }
                        break;
                    //卡片
                    case "card":
                        {
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                hszKey = config.hszKey,
                                Shadow = item.shadow,
                                Children = GetFormChildrenControlsList(config.children, gutter),
                                Span = config.span
                            });
                        }
                        break;
                    //分割线
                    case "divider":
                        {
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                hszKey = config.hszKey,
                                Span = config.span,
                                Contentposition = item.contentposition,
                                Default = item.__slot__.@default
                            });
                        }
                        break;
                    //折叠面板
                    case "collapse":
                        {
                            //先加为了防止 children下 还有折叠面板
                            List<CodeGenFormAllControlsDesign> childrenCollapseList = new List<CodeGenFormAllControlsDesign>();
                            foreach (var children in config.children)
                            {
                                childrenCollapseList.Add(new CodeGenFormAllControlsDesign()
                                {
                                    Title = children.title,
                                    Name = children.name,
                                    Gutter = gutter,
                                    Children = GetFormChildrenControlsList(children.__config__.children, gutter)
                                });
                            }
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                hszKey = config.hszKey,
                                Name = "active" + active++,
                                Accordion = item.accordion ? "true" : "false",
                                Active = config.active.ToObject<List<string>>().ToJson(),
                                Children = childrenCollapseList,
                                Span = config.span,
                            });
                        }
                        break;
                    //tab标签
                    case "tab":
                        {
                            //先加为了防止 children下 还有折叠面板
                            List<CodeGenFormAllControlsDesign> childrenCollapseList = new List<CodeGenFormAllControlsDesign>();
                            foreach (var children in config.children)
                            {
                                childrenCollapseList.Add(new CodeGenFormAllControlsDesign()
                                {
                                    Title = children.title,
                                    Gutter = gutter,
                                    Children = GetFormChildrenControlsList(children.__config__.children, gutter)
                                });
                            }
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                hszKey = config.hszKey,
                                Type = item.type,
                                TabPosition = item.tabPosition,
                                Name = "active" + active++,
                                Active = config.active.ToString(),
                                Children = childrenCollapseList,
                                Span = config.span
                            });
                        }
                        break;
                    //分组标题
                    case "groupTitle":
                        {
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                hszKey = config.hszKey,
                                Span = config.span,
                                Contentposition = item.contentposition,
                                Content = item.content
                            });
                        }
                        break;
                    //文本
                    case "HSZText":
                        {
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                hszKey = config.hszKey,
                                Span = config.span,
                                DefaultValue = config.defaultValue,
                                TextStyle = item.textStyle != null ? item.textStyle.ToJson() : "",
                                Style = item.style.ToJson()
                            });
                        }
                        break;
                    //常规
                    default:
                        {
                            string vModel = string.Empty;
                            string name = item.__vModel__.Replace("F_", "").Replace("f_", "").ToPascalCase();
                            var Model = name.LowerFirstChar();
                            switch (config.hszKey)
                            {
                                default:
                                    vModel = $"v-model='dataForm.{Model}' ";
                                    break;
                            }
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                Name = item.__vModel__.Replace("F_", "").Replace("f_", "").ToPascalCase(),
                                hszKey = config.hszKey,
                                Style = item.style != null ? $":style='{item.style.ToJson()}' " : "",
                                Type = !string.IsNullOrEmpty(item.type) ? $"type='{item.type}' " : "",
                                Span = config.span,
                                Clearable = item.clearable ? "clearable " : "",
                                Readonly = item.@readonly ? "readonly " : "",
                                Required = config.required ? "required " : "",
                                Placeholder = !string.IsNullOrEmpty(item.placeholder) ? $"placeholder='{item.placeholder}' " : "",
                                Disabled = item.disabled ? "disabled " : "",
                                ShowWordLimit = item.showwordlimit ? "show-word-limit " : "",
                                Format = !string.IsNullOrEmpty(item.format) ? $"format='{item.format}' " : "",
                                ValueFormat = !string.IsNullOrEmpty(item.valueformat) ? $"value-format='{item.valueformat}' " : "",
                                AutoSize = item.autosize != null && item.autosize.ToJson() != "null" ? $":autosize='{item.autosize.ToJson()}' " : "",
                                Multiple = item.multiple ? $"multiple " : "",
                                IsRange = item.isrange ? "is-range " : "",
                                Props = config.props,
                                MainProps = item.props != null ? $":props='{Model}Props' " : "",
                                OptionType = config.optionType == "default" ? "" : "-button",
                                Size = !string.IsNullOrEmpty(config.optionType) ? (config.optionType == "default" ? "" : $"size='{item.size}' ") : "",
                                PrefixIcon = !string.IsNullOrEmpty(item.prefixicon) ? $"prefix-icon='{item.prefixicon}' " : "",
                                SuffixIcon = !string.IsNullOrEmpty(item.suffixicon) ? $"suffix-icon='{item.suffixicon}' " : "",
                                MaxLength = !string.IsNullOrEmpty(item.maxlength) ? $"maxlength='{item.maxlength}' " : "",
                                Step = item.step != 0 ? $":step='{item.step}' " : "",
                                StepStrictly = item.stepstrictly ? "step-strictly " : "",
                                ControlsPosition = !string.IsNullOrEmpty(item.controlsposition) ? $"controls-position='{item.controlsposition}' " : "",
                                ShowChinese = item.showChinese ? "showChinese " : "",
                                ShowPassword = item.showPassword ? "show-password " : "",
                                Filterable = item.filterable ? "filterable " : "",
                                ShowAllLevels = item.showalllevels ? "show-all-levels " : "",
                                Separator = !string.IsNullOrEmpty(item.separator) ? $"separator='{item.separator}' " : "",
                                RangeSeparator = !string.IsNullOrEmpty(item.rangeseparator) ? $"range-separator='{item.rangeseparator}' " : "",
                                StartPlaceholder = !string.IsNullOrEmpty(item.startplaceholder) ? $"start-placeholder='{item.startplaceholder}' " : "",
                                EndPlaceholder = !string.IsNullOrEmpty(item.endplaceholder) ? $"end-placeholder='{item.endplaceholder}' " : "",
                                PickerOptions = item.pickeroptions != null && item.pickeroptions.ToJson() != "null" ? $":picker-options='{item.pickeroptions.ToJson()}' " : "",
                                Options = item.options != null ? $":options='{item.__vModel__}Options' " : "",
                                Max = item.max != 0 ? $":max='{item.max}' " : "",
                                AllowHalf = item.allowhalf ? "allow-half " : "",
                                ShowTexts = item.showtext ? $"show-text " : "",
                                ShowScore = item.showScore ? $"show-score " : "",
                                ShowAlpha = item.showalpha ? $"show-alpha " : "",
                                ColorFormat = !string.IsNullOrEmpty(item.colorformat) ? $"color-format='{item.colorformat}' " : "",
                                ActiveText = !string.IsNullOrEmpty(item.activetext) ? $"active-text='{item.activetext}' " : "",
                                InactiveText = !string.IsNullOrEmpty(item.inactivetext) ? $"inactive-text='{item.inactivetext}' " : "",
                                ActiveColor = !string.IsNullOrEmpty(item.activecolor) ? $"active-color='{item.activecolor}' " : "",
                                InactiveColor = !string.IsNullOrEmpty(item.inactivecolor) ? $"inactive-color='{item.inactivecolor}' " : "",
                                IsSwitch = config.hszKey == "switch" ? $":active-value='{item.activevalue}' :inactive-value='{item.inactivevalue}' " : "",
                                Min = item.min != 0 ? $":min='{item.min}' " : "",
                                ShowStops = item.showstops ? $"show-stops " : "",
                                Range = item.range ? $"range " : "",
                                Accept = !string.IsNullOrEmpty(item.accept) ? $":accept='{item.accept}' " : "",
                                ShowTip = item.showTip ? $"showTip " : "",
                                FileSize = item.fileSize != 0 ? $":fileSize='{item.fileSize}' " : "",
                                SizeUnit = !string.IsNullOrEmpty(item.sizeUnit) ? $"sizeUnit='{item.sizeUnit}' " : "",
                                Limit = item.limit != 0 ? $":limit='{item.limit}' " : "",
                                Contentposition = !string.IsNullOrEmpty(item.contentposition) ? $"content-position='{item.contentposition}' " : "",
                                ButtonText = !string.IsNullOrEmpty(item.buttonText) ? $"buttonText='{item.buttonText}' " : "",
                                Level = item.level != 0 ? $":level='{item.level}' " : "",
                                ActionText = !string.IsNullOrEmpty(item.actionText) ? $"actionText='{item.actionText}' " : "",
                                Shadow = !string.IsNullOrEmpty(item.shadow) ? $"shadow='{item.shadow}' " : "",
                                Content = !string.IsNullOrEmpty(item.content) ? $"content='{item.content}' " : "",
                                NoShow = config.noShow ? "v-if='false' " : "",
                                Label = config.label,
                                vModel = vModel,
                                Prepend = item.__slot__ != null && !string.IsNullOrEmpty(item.__slot__.prepend) ? item.__slot__.prepend : null,
                                Append = item.__slot__ != null && !string.IsNullOrEmpty(item.__slot__.append) ? item.__slot__.append : null,
                                Tag = config.tag,
                                Count = item.max
                            });
                        }
                        break;
                }
            }
            return list;
        }

        /// <summary>
        /// 获取表单全部控件选项配置
        /// </summary>
        /// <param name="fieldList"></param>
        /// <param name="type">1-Web设计,2-App设计,3-流程表单,4-Web表单,5-App表单</param>
        /// <param name="isMain">是否主循环</param>
        /// <returns></returns>
        private List<CodeGenConvIndexListControlOptionDesign> GetFormAllControlsProps(List<FieldsModel> fieldList, int type, bool isMain = false)
        {
            List<CodeGenConvIndexListControlOptionDesign> list = new List<CodeGenConvIndexListControlOptionDesign>();
            foreach (var item in fieldList)
            {
                var config = item.__config__;
                switch (config.hszKey)
                {
                    //卡片
                    case "card":
                    //栅格布局
                    case "row":
                        {
                            list.AddRange(GetFormAllControlsProps(config.children, type));
                        }
                        break;
                    //表格
                    case "table":
                        {
                            for (int i = 0; i < config.children.Count; i++)
                            {
                                var childrenConfig = config.children[i].__config__;
                                switch (childrenConfig.hszKey)
                                {
                                    case "select":
                                        {
                                            switch (childrenConfig.dataType)
                                            {
                                                //静态数据
                                                case "static":
                                                    list.Add(new CodeGenConvIndexListControlOptionDesign()
                                                    {
                                                        hszKey = childrenConfig.hszKey,
                                                        Name = config.children[i].__vModel__,
                                                        DictionaryType = childrenConfig.dataType == "dictionary" ? childrenConfig.dictionaryType : (childrenConfig.dataType == "dynamic" ? childrenConfig.propsUrl : null),
                                                        DataType = childrenConfig.dataType,
                                                        IsStatic = true,
                                                        IsIndex = false,
                                                        IsProps = type == 5 || type == 3 ? true : false,
                                                        Props = $"{{'label':'{childrenConfig.props.label}','value':'{childrenConfig.props.value}'}}",
                                                        IsChildren = true,
                                                        Content = GetCodeGenConvIndexListControlOption(config.children[i].__vModel__, config.children[i].__slot__.options)
                                                    });
                                                    break;
                                                default:
                                                    list.Add(new CodeGenConvIndexListControlOptionDesign()
                                                    {
                                                        hszKey = childrenConfig.hszKey,
                                                        Name = config.children[i].__vModel__,
                                                        DictionaryType = childrenConfig.dataType == "dictionary" ? childrenConfig.dictionaryType : (childrenConfig.dataType == "dynamic" ? childrenConfig.propsUrl : null),
                                                        DataType = childrenConfig.dataType,
                                                        IsStatic = false,
                                                        IsIndex = false,
                                                        IsProps = type == 5 || type == 3 ? true : false,
                                                        Props = $"{{'label':'{childrenConfig.props.label}','value':'{childrenConfig.props.value}'}}",
                                                        IsChildren = true,
                                                        Content = $"{config.children[i].__vModel__}Options : [],"
                                                    });
                                                    break;
                                            }
                                        }
                                        break;
                                    case "treeSelect":
                                    case "cascader":
                                        {
                                            switch (childrenConfig.dataType)
                                            {
                                                case "static":
                                                    list.Add(new CodeGenConvIndexListControlOptionDesign()
                                                    {
                                                        hszKey = childrenConfig.hszKey,
                                                        Name = config.children[i].__vModel__,
                                                        DictionaryType = childrenConfig.dataType == "dictionary" ? childrenConfig.dictionaryType : (childrenConfig.dataType == "dynamic" ? childrenConfig.propsUrl : null),
                                                        DataType = childrenConfig.dataType,
                                                        IsStatic = true,
                                                        IsIndex = false,
                                                        IsProps = true,
                                                        IsChildren = true,
                                                        Props = config.children[i].props.props.ToJson(),
                                                        QueryProps = GetQueryPropsModel(config.children[i].props.props).ToJson(),
                                                        Content = GetCodeGenConvIndexListControlOption(config.children[i].__vModel__, config.children[i].options)
                                                    });
                                                    break;
                                                default:
                                                    list.Add(new CodeGenConvIndexListControlOptionDesign()
                                                    {
                                                        hszKey = childrenConfig.hszKey,
                                                        Name = config.children[i].__vModel__,
                                                        DictionaryType = childrenConfig.dataType == "dictionary" ? childrenConfig.dictionaryType : (childrenConfig.dataType == "dynamic" ? childrenConfig.propsUrl : null),
                                                        DataType = childrenConfig.dataType,
                                                        IsStatic = false,
                                                        IsIndex = false,
                                                        IsProps = true,
                                                        IsChildren = true,
                                                        Props = config.children[i].props.props.ToJson(),
                                                        QueryProps = GetQueryPropsModel(config.children[i].props.props).ToJson(),
                                                        Content = $"{config.children[i].__vModel__}Options : [],"
                                                    });
                                                    break;
                                            }
                                        }
                                        break;
                                    //弹窗选择
                                    case "popupSelect":
                                        {
                                            list.Add(new CodeGenConvIndexListControlOptionDesign()
                                            {
                                                hszKey = childrenConfig.hszKey,
                                                Name = config.children[i].__vModel__,
                                                DictionaryType = null,
                                                DataType = null,
                                                IsStatic = true,
                                                IsIndex = false,
                                                IsProps = false,
                                                Props = null,
                                                IsChildren = false,
                                                Content = $"{config.children[i].__vModel__}Options : {config.children[i].columnOptions.ToJson()},"
                                            });
                                        }
                                        break;
                                    //关联表单
                                    case "relationForm":
                                        {
                                            list.Add(new CodeGenConvIndexListControlOptionDesign()
                                            {
                                                hszKey = childrenConfig.hszKey,
                                                Name = config.children[i].__vModel__,
                                                DictionaryType = null,
                                                DataType = null,
                                                IsStatic = true,
                                                IsIndex = false,
                                                IsProps = false,
                                                Props = null,
                                                IsChildren = false,
                                                Content = $"{config.children[i].__vModel__}Options : {config.children[i].columnOptions.ToJson()},"
                                            });
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        break;
                    //折叠面板
                    case "collapse":
                        {
                            StringBuilder title = new StringBuilder("[");
                            StringBuilder activeList = new StringBuilder("[");
                            foreach (var children in config.children)
                            {
                                title.Append($"{{title:'{children.title}'}},");
                                activeList.Append($"'{children.name}',");
                                list.AddRange(GetFormAllControlsProps(children.__config__.children, type));
                            }
                            title.Remove(title.Length - 1, 1);
                            activeList.Remove(activeList.Length - 1, 1);
                            title.Append("]");
                            activeList.Append("]");
                            list.Add(new CodeGenConvIndexListControlOptionDesign()
                            {
                                hszKey = config.hszKey,
                                Name = "active" + active++,
                                IsStatic = true,
                                IsIndex = false,
                                IsProps = false,
                                IsChildren = false,
                                Content = activeList.ToString(),
                                Title = title.ToString()
                            });
                        }
                        break;
                    //tab标签
                    case "tab":
                        {
                            StringBuilder title = new StringBuilder("[");
                            foreach (var children in config.children)
                            {
                                title.Append($"{{title:'{children.title}'}},");
                                list.AddRange(GetFormAllControlsProps(children.__config__.children, type));
                            }
                            title.Remove(title.Length - 1, 1);
                            title.Append("]");
                            list.Add(new CodeGenConvIndexListControlOptionDesign()
                            {
                                hszKey = config.hszKey,
                                Name = "active" + active++,
                                IsStatic = true,
                                IsIndex = false,
                                IsProps = false,
                                IsChildren = false,
                                Content = config.active.ToString(),
                                Title = title.ToString()
                            });
                        }
                        break;
                    //分组标题
                    case "groupTitle":
                    //分割线
                    case "divider":
                    //文本
                    case "HSZText":
                        break;
                    //常规
                    default:
                        {
                            switch (config.hszKey)
                            {
                                //弹窗选择
                                case "popupSelect":
                                    {
                                        list.Add(new CodeGenConvIndexListControlOptionDesign()
                                        {
                                            hszKey = config.hszKey,
                                            Name = item.__vModel__,
                                            DictionaryType = null,
                                            DataType = null,
                                            IsStatic = true,
                                            IsIndex = false,
                                            IsProps = false,
                                            Props = null,
                                            IsChildren = false,
                                            Content = $"{item.__vModel__}Options : {item.columnOptions.ToJson()},"
                                        });
                                    }
                                    break;
                                //关联表单
                                case "relationForm":
                                    {
                                        list.Add(new CodeGenConvIndexListControlOptionDesign()
                                        {
                                            hszKey = config.hszKey,
                                            Name = item.__vModel__,
                                            DictionaryType = null,
                                            DataType = null,
                                            IsStatic = true,
                                            IsIndex = false,
                                            IsProps = false,
                                            Props = null,
                                            IsChildren = false,
                                            Content = $"{item.__vModel__}Options : {item.columnOptions.ToJson()},"
                                        });
                                    }
                                    break;
                                //复选框
                                case "checkbox":
                                //下拉框多选
                                case "select":
                                //单选框
                                case "radio":
                                    {
                                        switch (config.dataType)
                                        {
                                            case "static":
                                                list.Add(new CodeGenConvIndexListControlOptionDesign()
                                                {
                                                    hszKey = config.hszKey,
                                                    Name = item.__vModel__,
                                                    DictionaryType = config.dataType == "dictionary" ? config.dictionaryType : (config.dataType == "dynamic" ? config.propsUrl : null),
                                                    DataType = config.dataType,
                                                    IsStatic = true,
                                                    IsIndex = true,
                                                    IsProps = type == 5 || type == 3 ? true : false,
                                                    Props = $"{{'label':'{config.props.label}','value':'{config.props.value}'}}",
                                                    QueryProps = GetQueryPropsModel(config.props).ToJson(),
                                                    IsChildren = false,
                                                    Content = GetCodeGenConvIndexListControlOption(item.__vModel__, item.__slot__.options)
                                                });
                                                break;
                                            default:
                                                list.Add(new CodeGenConvIndexListControlOptionDesign()
                                                {
                                                    hszKey = config.hszKey,
                                                    Name = item.__vModel__,
                                                    DictionaryType = config.dataType == "dictionary" ? config.dictionaryType : (config.dataType == "dynamic" ? config.propsUrl : null),
                                                    DataType = config.dataType,
                                                    IsStatic = false,
                                                    IsIndex = true,
                                                    IsProps = type == 5 || type == 3 ? true : false,
                                                    QueryProps = GetQueryPropsModel(config.props).ToJson(),
                                                    Props = $"{{'label':'{config.props.label}','value':'{config.props.value}'}}",
                                                    IsChildren = false,
                                                    Content = $"{item.__vModel__}Options : [],"
                                                });
                                                break;
                                        }
                                    }
                                    break;
                                case "treeSelect":
                                case "cascader":
                                    {
                                        switch (config.dataType)
                                        {
                                            case "static":
                                                list.Add(new CodeGenConvIndexListControlOptionDesign()
                                                {
                                                    hszKey = config.hszKey,
                                                    Name = item.__vModel__,
                                                    DictionaryType = config.dataType == "dictionary" ? config.dictionaryType : (config.dataType == "dynamic" ? config.propsUrl : null),
                                                    DataType = config.dataType,
                                                    IsStatic = true,
                                                    IsIndex = true,
                                                    IsProps = true,
                                                    IsChildren = false,
                                                    Props = item.props.props.ToJson(),
                                                    QueryProps = GetQueryPropsModel(item.props.props).ToJson(),
                                                    Content = GetCodeGenConvIndexListControlOption(item.__vModel__, item.options)
                                                });
                                                break;
                                            default:
                                                list.Add(new CodeGenConvIndexListControlOptionDesign()
                                                {
                                                    hszKey = config.hszKey,
                                                    Name = item.__vModel__,
                                                    DictionaryType = config.dataType == "dictionary" ? config.dictionaryType : (config.dataType == "dynamic" ? config.propsUrl : null),
                                                    DataType = config.dataType,
                                                    IsStatic = false,
                                                    IsIndex = true,
                                                    IsProps = true,
                                                    IsChildren = false,
                                                    Props = item.props.props.ToJson(),
                                                    QueryProps = GetQueryPropsModel(item.props.props).ToJson(),
                                                    Content = $"{item.__vModel__}Options : [],"
                                                });
                                                break;
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                }
            }
            //还原为后面方法做准备
            if (isMain)
                active = 1;
            return list;
        }

        /// <summary>
        /// 表单子表控件选项配置
        /// </summary>
        /// <param name="childrenFormData"></param>
        /// <returns></returns>
        private List<CodeGenConvIndexListControlOptionDesign> GetFormChildrenControlsProps(List<FieldsModel> childrenFormData)
        {
            List<CodeGenConvIndexListControlOptionDesign> list = new List<CodeGenConvIndexListControlOptionDesign>();
            foreach (var item in childrenFormData)
            {
                var config = item.__config__;
                switch (config.hszKey)
                {
                    //栅格布局
                    case "row":
                        {
                            list.AddRange(GetFormChildrenControlsProps(config.children));
                        }
                        break;
                    //表格
                    case "table":
                        {
                            foreach (var children in config.children)
                            {
                                var childrenConfig = children.__config__;
                                if (childrenConfig.hszKey == "select")
                                {
                                    switch (config.dataType)
                                    {
                                        //静态数据
                                        case "static":
                                            list.Add(new CodeGenConvIndexListControlOptionDesign()
                                            {
                                                hszKey = config.hszKey,
                                                Name = children.__vModel__.Replace("F_", "").Replace("f_", "").ToPascalCase(),
                                                DictionaryType = childrenConfig.dataType == "dictionary" ? childrenConfig.dictionaryType : (childrenConfig.dataType == "dynamic" ? childrenConfig.propsUrl : null),
                                                DataType = childrenConfig.dataType,
                                                IsStatic = true,
                                                IsIndex = false,
                                                IsProps = false,
                                                IsChildren = true,
                                                Content = GetCodeGenConvIndexListControlOption(children.__vModel__, children.__slot__.options)
                                            });
                                            break;
                                        default:
                                            list.Add(new CodeGenConvIndexListControlOptionDesign()
                                            {
                                                hszKey = config.hszKey,
                                                Name = children.__vModel__.Replace("F_", "").Replace("f_", "").ToPascalCase(),
                                                DictionaryType = childrenConfig.dataType == "dictionary" ? childrenConfig.dictionaryType : (childrenConfig.dataType == "dynamic" ? childrenConfig.propsUrl : null),
                                                DataType = childrenConfig.dataType,
                                                IsStatic = false,
                                                IsIndex = false,
                                                IsProps = false,
                                                IsChildren = true,
                                                Content = $"{children.__vModel__}Options : [],"
                                            });
                                            break;
                                    }
                                    break;

                                }
                            }
                        }
                        break;
                    //卡片
                    case "card":
                        {
                            list.AddRange(GetFormChildrenControlsProps(config.children));
                        }
                        break;
                    //折贴面板
                    case "collapse":
                        {
                            foreach (var children in config.children)
                            {
                                list.AddRange(GetFormChildrenControlsProps(children.__config__.children));
                            }
                            list.Add(new CodeGenConvIndexListControlOptionDesign()
                            {
                                hszKey = config.hszKey,
                                Name = "active" + active++,
                                IsStatic = true,
                                IsIndex = false,
                                IsProps = false,
                                IsChildren = false,
                                Content = config.active.ToObject<List<string>>().ToJson()
                            });
                        }
                        break;
                    //tab标签
                    case "tab":
                        {
                            foreach (var children in config.children)
                            {
                                list.AddRange(GetFormChildrenControlsProps(children.__config__.children));
                            }
                            list.Add(new CodeGenConvIndexListControlOptionDesign()
                            {
                                hszKey = config.hszKey,
                                Name = "active" + active++,
                                IsStatic = true,
                                IsIndex = false,
                                IsProps = false,
                                IsChildren = false,
                                Content = config.active.ToString()
                            });
                        }
                        break;
                    //文本
                    case "HSZText":
                        {

                        }
                        break;
                    //分割线
                    case "divider":
                        {

                        }
                        break;
                    //分组标题
                    case "groupTitle":
                        {

                        }
                        break;
                    default:
                        {
                            switch (config.hszKey)
                            {
                                //复选框
                                case "checkbox":
                                //下拉框多选
                                case "select":
                                //单选框
                                case "radio":
                                    {
                                        switch (config.dataType)
                                        {
                                            case "static":
                                                {
                                                    list.Add(new CodeGenConvIndexListControlOptionDesign()
                                                    {
                                                        hszKey = config.hszKey,
                                                        Name = item.__vModel__.Replace("F_", "").Replace("f_", "").ToPascalCase(),
                                                        DictionaryType = config.dataType == "dictionary" ? config.dictionaryType : (config.dataType == "dynamic" ? config.propsUrl : null),
                                                        DataType = config.dataType,
                                                        IsStatic = true,
                                                        IsIndex = true,
                                                        IsProps = false,
                                                        IsChildren = false,
                                                        Content = GetCodeGenConvIndexListControlOption(item.__vModel__, item.__slot__.options)
                                                    });
                                                }
                                                break;
                                            default:
                                                list.Add(new CodeGenConvIndexListControlOptionDesign()
                                                {
                                                    hszKey = config.hszKey,
                                                    Name = item.__vModel__.Replace("F_", "").Replace("f_", "").ToPascalCase(),
                                                    DictionaryType = config.dataType == "dictionary" ? config.dictionaryType : (config.dataType == "dynamic" ? config.propsUrl : null),
                                                    DataType = config.dataType,
                                                    IsStatic = false,
                                                    IsProps = false,
                                                    IsChildren = false,
                                                    IsIndex = true,
                                                    Content = $"{item.__vModel__}Options : [],"
                                                });
                                                break;
                                        }
                                    }
                                    break;
                                case "treeSelect":
                                case "cascader":
                                    {
                                        switch (config.dataType)
                                        {
                                            case "static":
                                                {
                                                    list.Add(new CodeGenConvIndexListControlOptionDesign()
                                                    {
                                                        hszKey = config.hszKey,
                                                        Name = item.__vModel__.Replace("F_", "").Replace("f_", "").ToPascalCase(),
                                                        DictionaryType = config.dataType == "dictionary" ? config.dictionaryType : (config.dataType == "dynamic" ? config.propsUrl : null),
                                                        DataType = config.dataType,
                                                        IsStatic = true,
                                                        IsIndex = true,
                                                        IsProps = true,
                                                        IsChildren = false,
                                                        Props = item.props.props.ToJson(),
                                                        Content = GetCodeGenConvIndexListControlOption(item.__vModel__, item.options)
                                                    });
                                                }
                                                break;
                                            default:
                                                list.Add(new CodeGenConvIndexListControlOptionDesign()
                                                {
                                                    hszKey = config.hszKey,
                                                    Name = item.__vModel__.Replace("F_", "").Replace("f_", "").ToPascalCase(),
                                                    DictionaryType = config.dataType == "dictionary" ? config.dictionaryType : (config.dataType == "dynamic" ? config.propsUrl : null),
                                                    DataType = config.dataType,
                                                    IsStatic = false,
                                                    IsProps = true,
                                                    IsChildren = false,
                                                    IsIndex = true,
                                                    Props = item.props.props.ToJson(),
                                                    Content = $"{item.__vModel__}Options : [],"
                                                });
                                                break;
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                }
            }
            return list;
        }

        /// <summary>
        /// 获取替换默认
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetReplaceDefault(string name)
        {
            var model = string.Empty;
            Regex myRegex = new Regex("^f_", RegexOptions.IgnoreCase);
            model = myRegex.Replace(name, "");
            return model;
        }

        /// <summary>
        /// 查询时将多选关闭
        /// </summary>
        /// <param name="propsModel"></param>
        /// <returns></returns>
        public PropsBeanModel GetQueryPropsModel(PropsBeanModel propsModel)
        {
            var model = new PropsBeanModel();
            if (propsModel != null && propsModel.multiple == true)
            {
                model = propsModel;
                model.multiple = false;
            }
            return model;
        }

        /// <summary>
        /// 是否datetime
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public bool GetIsDateTime(FieldsModel fields)
        {
            var isDateTime = false;
            if (fields != null && fields.__config__.hszKey == "date" && fields.type == "datetime")
            {
                isDateTime = true;
            }
            return isDateTime;
        }

        #endregion
    }
}
