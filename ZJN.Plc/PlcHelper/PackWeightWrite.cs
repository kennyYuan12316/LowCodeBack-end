using Newtonsoft.Json;
using S7.Net.Types;
using System.ComponentModel;

namespace ZJN.Plc.PlcHelper
{
    /// <summary>
    /// PLC常规控制包，写
    /// </summary>
    public class PackWeightWrite : PackBase
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        [DisplayName("设备编号"), PackStart(0), PackLength(8), Category("PLC报文"), Description("8 Byte,类型为S7String")]
        public byte[] DeviceCode
        {
            set { this.mDeviceCode = value; this.device = Sharp7.S7.GetStringAt(this.mDeviceCode, 0); }
            get { return this.mDeviceCode; }
        }
        private byte[] mDeviceCode = new byte[8];

        /// <summary>
        /// 任务编号
        /// </summary>
        [DisplayName("任务编号"), PackStart(8), PackLength(4), Category("PLC报文"), Description("4 Byte")]
        public int TaskCode { set; get; }

        /// <summary>
        /// 目标设备
        /// </summary>
        [DisplayName("目标设备"), PackStart(12), PackLength(8), Category("PLC报文"), Description("12 Byte,类型为S7String")]
        public byte[] TargetDevice { set; get; } = new byte[8];


        /// <summary>
        /// 托盘条码
        /// </summary>
        [DisplayName("托盘条码"), PackStart(20), PackLength(34), Category("PLC报文"), Description("34 Byte,类型为S7String")]
        public byte[] TrayCode { set; get; } = new byte[34];


        /// <summary>
        /// 物料类型
        /// </summary>
        [DisplayName("物料类型"), PackStart(54), PackLength(2), Category("PLC报文"), Description("2 Byte;随场景定义")]
        public short ProductWcs { set; get; }


        /// <summary>
        /// 握手类型
        /// </summary>
        [DisplayName("握手类型"), PackStart(56), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short ResponseWcs { set; get; }


        /// <summary>
        /// AGV交互
        /// </summary>
        [DisplayName("AGV交互"), PackStart(58), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short AgvWcs { set; get; }


        /// <summary>
        /// 预留1
        /// </summary>
        [DisplayName("预留1"), PackStart(60), PackLength(4), Category("PLC报文"), Description("4 Byte")]
        public float Reserve1 { set; get; }

        /// <summary>
        /// 预留2
        /// </summary>
        //[DisplayName("预留2"), PackStart(62), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        //public short Reserve2 { set; get; }

        /// <summary>
        /// 设备编号S
        /// </summary>
        [DisplayName("设备编号S"), Description("字符串"), JsonIgnore()]
        public string DeviceCode_S
        {
            get
            {
                return Sharp7.S7.GetStringAt(this.DeviceCode, 0);
            }
        }

        /// <summary>
        /// 目标设备S
        /// </summary>
        [DisplayName("目标设备S"), Description("字符串"), JsonIgnore()]
        public string TargetDevice_S
        {
            get
            {
                return Sharp7.S7.GetStringAt(this.TargetDevice, 0);
            }
        }

        /// <summary>
        /// 托盘编码S
        /// </summary>
        [DisplayName("托盘编码S"), Description("字符串"), JsonIgnore()]
        public string TrayCode_S
        {
            get
            {
                return Sharp7.S7.GetStringAt(this.TrayCode, 0);
            }
        }

        /// <summary>
        /// PLC请求
        /// </summary>
        [JsonIgnore()]
        public ResponseWcs RequestPlcEnum { get { return (ResponseWcs)this.ResponseWcs; } }


        /// <summary>
        /// AGV交互
        /// </summary>
        [JsonIgnore()]
        public AgvWcs AgvPlcEnum { get { return (AgvWcs)this.AgvWcs; } }

        /// <summary>
        /// 包对象名称
        /// </summary>
        [JsonIgnore()]
        public override string PackType
        {
            get { return "WRITE"; }
        }

        /// <summary>
        /// 对象大小
        /// </summary>
        [DisplayName("包体大小"), Description("字节数,64 Byte"), JsonIgnore()]
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
    public class PackWeightWriteList
    {
        public PackWeightWriteList(int count)
        {
            this.List = new PackWeightWrite[count];
        }

        public PackWeightWrite[] List { set; get; }
    }
}

