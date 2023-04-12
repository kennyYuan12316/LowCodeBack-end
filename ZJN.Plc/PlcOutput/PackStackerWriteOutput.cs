using Newtonsoft.Json;
using S7.Net.Types;
using System;
using System.ComponentModel;

namespace ZJN.Plc.PlcHelper
{

    /// <summary>
    /// PLC堆垛机控制包，读，62 Byte
    /// </summary>
    public class PackStackerWriteOutput 
    {
        /// <summary>
        /// 堆垛机编码
        /// </summary>
        [DisplayName("设备编码"), Description("堆垛机编码,指定")]
        public string DeviceCode { set; get; }

        /// <summary>
        /// 启动
        /// </summary>
        [DisplayName("启动"), PackStart(0), PackLength(0.125), Category("PLC报文"), Description("1 bit")]
        public bool IsStart { set; get; }

        /// <summary>
        /// 报警复位
        /// </summary>
        [DisplayName("报警复位"), PackStart(0), PackLength(0.125), Category("PLC报文"), Description("1 bit")]
        public bool IsResetAlarm { set; get; }

        /// <summary>
        /// 自动复位
        /// </summary>
        [DisplayName("自动复位"), PackStart(0), PackLength(0.125), Category("PLC报文"), Description("1 bit")]
        public bool IsResetAuto { set; get; }

        /// <summary>
        /// 消防命令
        /// </summary>
        [DisplayName("消防命令"), PackStart(0), PackLength(0.125), Category("PLC报文"), Description("1 bit")]
        public bool IsFireCommand { set; get; }

        /// <summary>
        /// 温度正常
        /// </summary>
        [DisplayName("温度正常"), PackStart(0), PackLength(0.125), Category("PLC报文"), Description("1 bit")]
        public bool IsTpNormal { set; get; }

        /// <summary>
        /// 温度异常
        /// </summary>
        [DisplayName("温度异常"), PackStart(0), PackLength(0.125), Category("PLC报文"), Description("1 bit")]
        public bool IsTpAbnormal { set; get; }

        /// <summary>
        /// DTS异常
        /// </summary>
        [DisplayName("DTS异常"), PackStart(0), PackLength(0.125), Category("PLC报文"), Description("1 bit")]
        public bool IsDtsAbnormal { set; get; }

        /// <summary>
        /// 手动复位
        /// </summary>
        [DisplayName("手动复位"), PackStart(0), PackLength(0.125), Category("PLC报文"), Description("1 bit")]
        public bool IsResetManual { set; get; }

        //[DisplayName("预留1"), PackStart(1), PackLength(8), Category("PLC报文"), Description("8 bit")]
        //public bool[] Reserve1 { set; get; } = new bool[8];

        public bool IsReserve1 { set; get; }
        public bool IsReserve2 { set; get; }

        public bool IsReserve3 { set; get; }
        public bool IsReserve4 { set; get; }
        public bool IsReserve5 { set; get; }
        public bool IsReserve6 { set; get; }
        public bool IsReserve7 { set; get; }
        public bool IsReserve8 { set; get; }
        /// <summary>
        /// 任务序号
        /// </summary>
        [DisplayName("任务序号"), PackStart(2), PackLength(4), Category("PLC报文"), Description("2 Byte;")]
        public int JobID { set; get; }

        /// <summary>
        /// 任务命令
        /// </summary>
        [DisplayName("任务命令"), PackStart(6), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short JobCommand { set; get; }


        /// <summary>
        /// 起点行
        /// </summary>
        [DisplayName("起点行"), PackStart(8), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short F_Row { set; get; }

        /// <summary>
        /// 起点列
        /// </summary>
        [DisplayName("起点列"), PackStart(10), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short F_Col { set; get; }

        /// <summary>
        /// 起点层
        /// </summary>
        [DisplayName("起点层"), PackStart(12), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short F_Lay { set; get; }

        /// <summary>
        /// 终点行
        /// </summary>
        [DisplayName("终点行"), PackStart(14), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short T_Row { set; get; }

        /// <summary>
        /// 终点列
        /// </summary>
        [DisplayName("终点列"), PackStart(16), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short T_Col { set; get; }

        /// <summary>
        /// 终点层
        /// </summary>
        [DisplayName("终点层"), PackStart(18), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short T_Lay { set; get; }

        /// <summary>
        /// 当前层
        /// </summary>
        [DisplayName("提交完成"), PackStart(20), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short IsPost { set; get; }

        /// <summary>
        /// 预留8
        /// </summary>
        [DisplayName("预留8"), PackStart(22), PackLength(2), Category("PLC报文"), Description("2 byte")]
        public short Reserve8 { set; get; }


        /// <summary>
        /// 预留9
        /// </summary>
        [DisplayName("预留9"), PackStart(24), PackLength(2), Category("PLC报文"), Description("2 byte")]
        public short Reserve9 { set; get; }


        /// <summary>
        /// 预留10
        /// </summary>
        [DisplayName("预留10"), PackStart(26), PackLength(2), Category("PLC报文"), Description("2 byte")]
        public short Reserve10 { set; get; }


        /// <summary>
        /// 预留11
        /// </summary>
        [DisplayName("预留11"), PackStart(28), PackLength(2), Category("PLC报文"), Description("2 byte")]
        public short Reserve1 { set; get; }


        /// <summary>
        /// 预留12
        /// </summary>
        [DisplayName("预留12"), PackStart(30), PackLength(2), Category("PLC报文"), Description("2 byte")]
        public short Reserve2 { set; get; }

        public string ip { get; set; }
    }
}
