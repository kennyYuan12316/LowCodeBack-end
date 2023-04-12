using Newtonsoft.Json;
using S7.Net.Types;
using System.ComponentModel;

namespace ZJN.Plc.PlcHelper
{
    /// <summary>
    /// PLC常规控制包，状态，双工位
    /// </summary>
    public class PackStatusD:PackBase
    {

        /// <summary>
        /// 设备编号
        /// </summary>
        [DisplayName("设备"), PackStart(0), PackLength(8), Category("PLC报文"), Description("8 Byte,类型为S7String")]
        public byte[] DeviceCode
        {
            set { this.mDeviceCode = value; this.device = Sharp7.S7.GetStringAt(this.mDeviceCode, 0); }
            get { return this.mDeviceCode; }
        }
        private byte[] mDeviceCode = new byte[8];

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
        /// 目标设备
        /// </summary>
        [DisplayName("目标1"), PackStart(16), PackLength(8), Category("PLC报文"), Description("12 Byte,类型为S7String")]
        public byte[] TargetDevice1 { set; get; } = new byte[8];

        /// <summary>
        /// 目标设备
        /// </summary>
        [DisplayName("目标2"), PackStart(24), PackLength(8), Category("PLC报文"), Description("12 Byte,类型为S7String")]
        public byte[] TargetDevice2 { set; get; } = new byte[8];


        /// <summary>
        /// 托盘条码
        /// </summary>
        [DisplayName("托盘1"), PackStart(32), PackLength(34), Category("PLC报文"), Description("34 Byte,类型为S7String")]
        public byte[] TrayCode1 { set; get; } = new byte[34];

        /// <summary>
        /// 托盘条码
        /// </summary>
        [DisplayName("托盘2"), PackStart(66), PackLength(34), Category("PLC报文"), Description("34 Byte,类型为S7String")]
        public byte[] TrayCode2 { set; get; } = new byte[34];


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

        /// <summary>
        /// 设备编号S
        /// </summary>
        [DisplayName("设备S"),  Description("字符串")]
        public string DeviceCode_S
        {
            get
            {
                return Sharp7.S7.GetStringAt(this.DeviceCode, 0).Trim();
            }
        }

        /// <summary>
        /// 目标设备S
        /// </summary>
        [DisplayName("目标1S"), Description("字符串")]
        public string TargetPosition1_S
        {
            get
            {
                return Sharp7.S7.GetStringAt(this.TargetDevice1, 0).Trim();
            }
        }

        /// <summary>
        /// 目标设备S
        /// </summary>
        [DisplayName("目标2S"), Description("字符串")]
        public string TargetPosition2_S
        {
            get
            {
                return Sharp7.S7.GetStringAt(this.TargetDevice1, 0).Trim();
            }
        }

        /// <summary>
        /// 托盘编码S
        /// </summary>
        [DisplayName("托盘1S"), Description("字符串")]
        public string TrayCode1_S
        {
            get
            {
                return Sharp7.S7.GetStringAt(this.TrayCode1, 0).Trim();
            }
        }

        /// <summary>
        /// 托盘编码S
        /// </summary>
        [DisplayName("托盘2S"), Description("字符串")]
        public string TrayCode2_S
        {
            get
            {
                return Sharp7.S7.GetStringAt(this.TrayCode2, 0).Trim();
            }
        }

        /// <summary>
        /// 包对象名称
        /// </summary>
        [JsonIgnore()]
        public override string PackType
        {
            get { return "STATUS"; }
        }

        /// <summary>
        /// 对象大小
        /// </summary>
        [DisplayName("包体大小"), Description("字节数,110 Byte"), JsonIgnore()]
        public override int PackSize
        {
            get
            {
                return (int)Class.GetClassSize(this);
            }
        }

    }

    /// <summary>
    /// DB包列表
    /// </summary>
    public class PackStatusDList
    {
        public PackStatusDList(int count)
        {
            this.List = new PackStatusD[count];
        }

        public PackStatusD[] List { set; get; }
    }
}
