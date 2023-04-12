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
    public class PackWriteOutput
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
        [DisplayName("设备编号S"), Description("字符串")]
        public string DeviceCode_S { get; set; }

        /// <summary>
        /// 任务编号
        /// </summary>
        [DisplayName("任务编号"), PackStart(8), PackLength(4), Category("PLC报文"), Description("4 Byte")]
        public int TaskCode { set; get; }

        /// <summary>
        /// 目标设备S
        /// </summary>
        [DisplayName("目标设备S"), Description("字符串")]
        public string TargetDevice_S { get; set; }

        /// <summary>
        /// 托盘编码S
        /// </summary>
        [DisplayName("托盘编码S"), Description("字符串")]
        public string TrayCode_S { get; set; }




        /// <summary>
        /// 物料类型
        /// </summary>
        [DisplayName("物料类型"), PackStart(40), PackLength(2), Category("PLC报文"), Description("2 Byte;随场景定义")]
        public short ProductWcs { set; get; }


        /// <summary>
        /// 握手类型
        /// </summary>
        [DisplayName("握手类型"), PackStart(42), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short ResponseWcs { set; get; }


        /// <summary>
        /// AGV交互
        /// </summary>
        //[DisplayName("AGV交互"), PackStart(58), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        //public short AgvWcs { set; get; }


        /// <summary>
        /// 预留1
        /// </summary>
        [DisplayName("预留1"), PackStart(44), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short Reserve1 { set; get; }

        /// <summary>
        /// 预留2
        /// </summary>
        [DisplayName("预留2"), PackStart(46), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short Reserve2 { set; get; }


        public string ip { get; set; }
    }



}

