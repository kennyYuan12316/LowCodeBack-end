using Newtonsoft.Json;
using S7.Net.Types;
using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace ZJN.Plc.PlcHelper
{





    /// <summary>
    /// PLC常规控制包，读
    /// </summary>
    public class PackStatusOutput
    {
        //设备编号 String[6]	2.0
        //任务号 DInt	10.0
        //目标位置编号 String[6]	14.0
        //托盘条码 String[32]	22.0  
        //物料类型 Int	56.0
        //握手类型 Int	58.0
        //AGV交互 Int	60.0
        //预留_1 Int	62.0
        //预留_2 Int	64.0

        /// <summary>
        /// 设备编号S
        /// </summary>
        [DisplayName("设备S"), Description("字符串")]
        public string DeviceCode_S { get; set; }

        /// <summary>
        /// 任务编号
        /// </summary>
        [DisplayName("任务"), PackStart(8), PackLength(4), Category("PLC报文"), Description("4 Byte")]
        public int TaskCode { set; get; }

        /// <summary>
        /// 目标设备S
        /// </summary>
        [DisplayName("目标S"), Description("字符串")]
        public string TargetDevice_S { get; set; }

        /// <summary>
        /// 托盘编码S
        /// </summary>
        [DisplayName("托盘S"), Description("字符串")]
        public string TrayCode_S { get; set; }


        /// <summary>
        /// 占位状态
        /// </summary>
        [DisplayName("占位"), PackStart(54), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short ExistTray { set; get; }


        /// <summary>
        /// 报警类型
        /// </summary>
        [DisplayName("报警"), PackStart(56), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short Alarm { set; get; }


        /// <summary>
        /// 动作类型
        /// </summary>
        [DisplayName("状态"), PackStart(58), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short Status { set; get; }


        /// <summary>
        /// 动作类型
        /// </summary>
        [DisplayName("预留1"), PackStart(60), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short Reserve1 { set; get; }

        /// <summary>
        /// 动作类型
        /// </summary>
        [DisplayName("预留2"), PackStart(62), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short Reserve2 { set; get; }



        public string ip { get; set; }
    }


    /// <summary>
    /// DB包列表
    /// </summary>
  
}

