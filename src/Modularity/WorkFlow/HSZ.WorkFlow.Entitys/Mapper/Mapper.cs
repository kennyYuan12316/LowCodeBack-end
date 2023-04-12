using Aspose.Cells.Charts;
using HSZ.Common.Extension;
using HSZ.WorkFlow.Entitys.Dto.FlowBefore;
using HSZ.WorkFlow.Entitys.Dto.FlowEngine;
using HSZ.WorkFlow.Entitys.Dto.FlowLaunch;
using HSZ.WorkFlow.Entitys.Model;
using HSZ.WorkFlow.Entitys.Model.Properties;
using Mapster;
using Newtonsoft.Json;
using System;
using Yitter.IdGenerator;

namespace HSZ.WorkFlow.Entitys.Mapper
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    class Mapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<FlowEngineEntity, FlowEngineListAllOutput>()
                .Map(dest => dest.formData, src => src.FormTemplateJson);
            config.ForType<FlowEngineEntity, FlowEngineInfoOutput>()
                .Map(dest => dest.formData, src => src.FormTemplateJson)
                .Map(dest => dest.dbLinkId, src => src.DbLinkId.IsEmpty() ? "0" : src.DbLinkId);
            config.ForType<FlowEngineCrInput, FlowEngineEntity>()
                .Map(dest => dest.FormTemplateJson, src => src.formData);
            config.ForType<FlowEngineUpInput, FlowEngineEntity>()
                .Map(dest => dest.FormTemplateJson, src => src.formData);
            config.ForType<FlowEngineEntity, FlowLaunchListOutput>()
               .Map(dest => dest.formData, src => src.FormTemplateJson);
            config.ForType<FlowEngineEntity, FlowBeforeListOutput>()
                .Map(dest => dest.formData, src => src.FlowTemplateJson);
            config.ForType<FlowTemplateJsonModel, TaskNodeModel>()
                .Map(dest => dest.upNodeId, src => src.prevId);
            config.ForType<TaskNodeModel,FlowTaskNodeEntity>()
                .Map(dest => dest.Id, src => YitIdHelper.NextId().ToString())
                .Map(dest => dest.CreatorTime, src => DateTime.Now)
                .Map(dest => dest.TaskId, src => src.taskId)
                .Map(dest => dest.NodeCode, src => src.nodeId)
                .Map(dest => dest.NodeType, src => src.type)
                .Map(dest => dest.Completion, src => src.type == "start" ? 1 : 0)
                .Map(dest => dest.NodeName, src => NodeName(src))
                .Map(dest => dest.NodeUp, src => src.type != "approver" ? null : (src.propertyJson as ApproversProperties).rejectStep)
                .Map(dest => dest.NodeNext, src => src.nextNodeId)
                .Map(dest => dest.NodePropertyJson, src => NodePropertyJson(src))
                .Map(dest => dest.State, src => "-2");
        }

        private string NodeName(TaskNodeModel src) {
            switch (src.type) {
                case "start": return "开始";
                case "approver":return (src.propertyJson as ApproversProperties).title;
                case "condition": return (src.propertyJson as ConditionProperties).title;
                case "dynamic": return (src.propertyJson as DynamicProperties).title;
                default: return "";
            }
        }
        private string NodePropertyJson(TaskNodeModel src)
        {
            switch (src.type)
            {
                case "start": return JsonConvert.SerializeObject(src.propertyJson as StartProperties);
                case "approver": return JsonConvert.SerializeObject(src.propertyJson as ApproversProperties);
                case "condition": return JsonConvert.SerializeObject(src.propertyJson as ConditionProperties);
                case "dynamic": return JsonConvert.SerializeObject(src.propertyJson as DynamicProperties);
                default: return "";
            }
        }
    }
}
