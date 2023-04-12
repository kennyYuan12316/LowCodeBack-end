using Newtonsoft.Json;
using S7.Net.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZJN.Plc.PlcHelper
{
    /// <summary>
    /// PLC堆垛机控制包，读，48 Byte
    /// </summary>
    public class PackStackerReadD : PackBase
    {
        /// <summary>
        /// 堆垛机编码
        /// </summary>
        [DisplayName("设备编码"), Description("堆垛机编码,指定")]
        public string DeviceCode
        {
            get { return this.DeviceNick; }
        }

        /// <summary>
        /// 自动
        /// </summary>
        [DisplayName("自动"), PackStart(0), PackLength(0.125), Category("PLC报文"), Description("1 bit")]
        public bool IsAuto { set; get; }

        /// <summary>
        /// 就位
        /// </summary>
        [DisplayName("就位"), PackStart(0), PackLength(0.125), Category("PLC报文"), Description("1 bit")]
        public bool IsReady { set; get; }

        /// <summary>
        /// 货叉左伸
        /// </summary>
        [DisplayName("货叉伸左"), PackStart(0), PackLength(0.125), Category("PLC报文"), Description("1 bit")]
        public bool IsForkL { set; get; }

        /// <summary>
        /// 货叉右伸
        /// </summary>
        [DisplayName("货叉伸右"), PackStart(0), PackLength(0.125), Category("PLC报文"), Description("1 bit")]
        public bool IsForkR { set; get; }

        /// <summary>
        /// 货叉有料
        /// </summary>
        [DisplayName("货叉有料"), PackStart(0), PackLength(0.125), Category("PLC报文"), Description("1 bit")]
        public bool IsForkTray { set; get; }

        /// <summary>
        /// 报警
        /// </summary>
        [DisplayName("报警"), PackStart(0), PackLength(0.125), Category("PLC报文"), Description("1 bit")]
        public bool IsAlarm { set; get; }

        /// <summary>
        /// 运行
        /// </summary>
        [DisplayName("运行"), PackStart(0), PackLength(0.125), Category("PLC报文"), Description("1 bit")]
        public bool IsRunning { set; get; }

        /// <summary>
        /// 完成
        /// </summary>
        [DisplayName("完成"), PackStart(0), PackLength(0.125), Category("PLC报文"), Description("1 bit")]
        public bool IsComplete { set; get; }

        /// <summary>
        /// 关门
        /// </summary>
        [DisplayName("关门"), PackStart(1), PackLength(0.125), Category("PLC报文"), Description("1 bit")]
        public bool IsCloseDoor { set; get; }

        /// <summary>
        /// 预留1,7 bit
        /// </summary>
        [DisplayName("bool预留1"), PackStart(1), PackLength(0.125), Category("PLC报文"), Description("7 bit")]
        public bool IsReserve1 { set; get; }
        public bool IsReserve2 { set; get; }
        public bool IsReserve3 { set; get; }
        public bool IsReserve4 { set; get; }
        public bool IsReserve5 { set; get; }
        public bool IsReserve6 { set; get; }
        public bool IsReserve7 { set; get; }

        /// <summary>
        /// 报警代码
        /// </summary>
        [DisplayName("报警代码"), PackStart(2), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short AlarmCode { set; get; }

        /// <summary>
        /// X轴
        /// </summary>
        [DisplayName("X轴"), PackStart(4), PackLength(4), Category("PLC报文"), Description("2 Byte;")]
        public int AxisX { set; get; }

        /// <summary>
        /// Y轴
        /// </summary>
        [DisplayName("Y轴"), PackStart(8), PackLength(4), Category("PLC报文"), Description("2 Byte;")]
        public int AxisY { set; get; }

        /// <summary>
        /// 当前行
        /// </summary>
        [DisplayName("当前行"), PackStart(12), PackLength(2), Category("PLC报文"), Description("4 Byte;")]
        public short C_Row { set; get; }

        /// <summary>
        /// 当前列
        /// </summary>
        [DisplayName("当前列"), PackStart(14), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short C_Col { set; get; }

        /// <summary>
        /// 当前层
        /// </summary>
        [DisplayName("当前层"), PackStart(16), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short C_Lay { set; get; }

        /// <summary>
        /// 任务序号
        /// </summary>
        [DisplayName("任务序号"), PackStart(18), PackLength(4), Category("PLC报文"), Description("2 Byte;")]
        public int JobID { set; get; }

        /// <summary>
        /// 任务命令
        /// </summary>
        [DisplayName("任务命令"), PackStart(22), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short JobCommand { set; get; }

        /// <summary>
        /// 起点行
        /// </summary>
        [DisplayName("起点行"), PackStart(24), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short F_Row { set; get; }

        /// <summary>
        /// 起点列
        /// </summary>
        [DisplayName("起点列"), PackStart(26), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short F_Col { set; get; }

        /// <summary>
        /// 起点层
        /// </summary>
        [DisplayName("起点层"), PackStart(28), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short F_Lay { set; get; }

        /// <summary>
        /// 终点行
        /// </summary>
        [DisplayName("终点行"), PackStart(30), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short T_Row { set; get; }

        /// <summary>
        /// 终点列
        /// </summary>
        [DisplayName("终点列"), PackStart(32), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short T_Col { set; get; }

        /// <summary>
        /// 终点层
        /// </summary>
        [DisplayName("终点层"), PackStart(34), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short T_Lay { set; get; }

        /// <summary>
        /// 预留1short
        /// </summary>
        [DisplayName("short预留1"), PackStart(36), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short Reserve1 { set; get; }

        /// <summary>
        /// 预留2short
        /// </summary>
        [DisplayName("short预留2"), PackStart(38), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short Reserve2 { set; get; }
        /// <summary>
        /// 预留3short
        /// </summary>
        [DisplayName("short预留3"), PackStart(40), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short Reserve3 { set; get; }
        /// <summary>
        /// 预留4short
        /// </summary>
        [DisplayName("short预留4"), PackStart(42), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short Reserve4 { set; get; }
        /// <summary>
        /// 预留5short
        /// </summary>
        [DisplayName("short预留5"), PackStart(44), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short Reserve5 { set; get; }
        /// <summary>
        /// 预留16short
        /// </summary>
        [DisplayName("short预留6"), PackStart(46), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short Reserve6 { set; get; }

        /// <summary>
        /// 包对象名称
        /// </summary>
        [JsonIgnore()]
        public override string PackType
        {
            get { return "READ"; }
        }

        /// <summary>
        /// 对象大小
        /// </summary>
        [DisplayName("包体大小"), Description("字节数,48 Byte"), JsonIgnore()]
        public override int PackSize
        {
            get
            {
                return (int)Class.GetClassSize(this);
            }
        }

    }
}
