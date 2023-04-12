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
    public class PackStatusDOutput
    {
        //设备编号 String[6]	
        //任务号1 DInt	10.0
        //任务号2 DInt	10.0
        //托盘条码1 String[32]
        //托盘条码2 String[32]
        //目标位置编号1 String[6]
        //目标位置编号2 String[6]
        //物料类型1 Int	56.0
        //物料类型2 Int	56.0
        //握手类型 Int	58.0
        //单双托 Int	60.0
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
        [DisplayName("任务1"), PackStart(8), PackLength(4), Category("PLC报文"), Description("4 Byte")]
        public int TaskCode1 { set; get; }

        /// <summary>
        /// 任务编号
        /// </summary>
        [DisplayName("任务2"), PackStart(12), PackLength(4), Category("PLC报文"), Description("4 Byte")]
        public int TaskCode2 { set; get; }

        /// <summary>
        /// 目标设备S
        /// </summary>
        [DisplayName("目标1S"), Description("字符串")]
        public string TargetPosition1_S { get; set; }

        /// <summary>
        /// 目标设备S
        /// </summary>
        [DisplayName("目标2S"), Description("字符串")]
        public string TargetPosition2_S { get; set; }

        /// <summary>
        /// 托盘编码S
        /// </summary>
        [DisplayName("托盘1S"), Description("字符串")]
        public string TrayCode1_S { get; set; }

        /// <summary>
        /// 托盘编码S
        /// </summary>
        [DisplayName("托盘2S"), Description("字符串")]
        public string TrayCode2_S { get; set; }


        /// <summary>
        /// 占位状态
        /// </summary>
        [DisplayName("占位"), PackStart(100), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short ExistTray { set; get; }


        /// <summary>
        /// 报警类型
        /// </summary>
        [DisplayName("报警"), PackStart(102), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short Alarm { set; get; }


        /// <summary>
        /// 动作类型
        /// </summary>
        [DisplayName("状态"), PackStart(104), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short Status { set; get; }


        /// <summary>
        /// 动作类型
        /// </summary>
        [DisplayName("预留1"), PackStart(106), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short Reserve1 { set; get; }

        /// <summary>
        /// 动作类型
        /// </summary>
        [DisplayName("预留2"), PackStart(108), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short Reserve2 { set; get; }

        

        public string ip { get; set; }
    }


}

