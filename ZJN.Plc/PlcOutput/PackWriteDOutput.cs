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
    public class PackWriteDOutput
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
        [DisplayName("设备编号S"), Description("字符串")]
        public string DeviceCode_S { get; set; }

        /// <summary>
        /// 任务编号
        /// </summary>
        [DisplayName("任务编号"), PackStart(8), PackLength(4), Category("PLC报文"), Description("4 Byte")]
        public int TaskCode1 { set; get; }


        /// <summary>
        /// 任务编号
        /// </summary>
        [DisplayName("任务编号"), PackStart(12), PackLength(4), Category("PLC报文"), Description("4 Byte")]
        public int TaskCode2 { set; get; }

        /// <summary>
        /// 目标设备1S
        /// </summary>
        [DisplayName("目标设备1S"), Description("字符串")]
        public string TargetDevice1_S { get; set; }

        /// <summary>
        /// 目标设备2S
        /// </summary>
        [DisplayName("目标设备2S"), Description("字符串")]
        public string TargetDevice2_S { get; set; }

        /// <summary>
        /// 托盘编码1S
        /// </summary>
        [DisplayName("托盘编码1S"), Description("字符串")]
        public string TrayCode1_S { get; set; }

        /// <summary>
        /// 托盘编码2S
        /// </summary>
        [DisplayName("托盘编码2S"), Description("字符串")]
        public string TrayCode2_S { get; set; }



        /// <summary>
        /// 物料类型
        /// </summary>
        [DisplayName("物料类型1"), PackStart(100), PackLength(2), Category("PLC报文"), Description("2 Byte;随场景定义")]
        public short ProductPlc1 { set; get; }


        /// <summary>
        /// 物料类型
        /// </summary>
        [DisplayName("物料类型2"), PackStart(102), PackLength(2), Category("PLC报文"), Description("2 Byte;随场景定义")]
        public short ProductPlc2 { set; get; }


        /// <summary>
        /// 握手类型
        /// </summary>
        [DisplayName("握手类型"), PackStart(104), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short ResponseWcs { set; get; }


        /// <summary>
        /// 单双托
        /// </summary>
        [DisplayName("单双托"), PackStart(106), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short TrayCount { set; get; }



        /// <summary>
        /// 预留1
        /// </summary>
        [DisplayName("预留1"), PackStart(108), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short Reserve1 { set; get; }

        /// <summary>
        /// 预留2
        /// </summary>
        [DisplayName("预留2"), PackStart(110), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short Reserve2 { set; get; }



        public string ip { get; set; }
    }


}

