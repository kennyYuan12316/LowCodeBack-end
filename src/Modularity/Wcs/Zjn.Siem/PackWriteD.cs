using Newtonsoft.Json;
using S7.Net.Types;
using System.ComponentModel;

namespace Zjn.Siem
{


    /// <summary>
    /// PLC常规控制包，写,双工位
    /// </summary>
    public class PackWriteD : PackBase
    {

        /// <summary>
        /// 设备编号
        /// </summary>
        [DisplayName("设备编号"), PackStart(0), PackLength(8), Category("PLC报文"), Description("8 Byte,类型为S7String")]
        public byte[] DeviceCode
        {
            set
            {
                this.DeviceCodeM = value;
                this.device = Sharp7.S7.GetStringAt(this.DeviceCode, 0);
            }
            get
            { return this.DeviceCodeM; }
        }
        private byte[] DeviceCodeM = new byte[8];

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
        /// 托盘条码
        /// </summary>
        [DisplayName("托盘条码1"), PackStart(16), PackLength(34), Category("PLC报文"), Description("34 Byte,类型为S7String")]
        public byte[] TrayCode1 { set; get; } = new byte[34];

        /// <summary>
        /// 托盘条码
        /// </summary>
        [DisplayName("托盘条码2"), PackStart(50), PackLength(34), Category("PLC报文"), Description("34 Byte,类型为S7String")]
        public byte[] TrayCode2 { set; get; } = new byte[34];

        /// <summary>
        /// 目标设备
        /// </summary>
        [DisplayName("目标设备1"), PackStart(84), PackLength(8), Category("PLC报文"), Description("12 Byte,类型为S7String")]
        public byte[] TargetDevice1 { set; get; } = new byte[8];

        /// <summary>
        /// 目标设备
        /// </summary>
        [DisplayName("目标设备2"), PackStart(92), PackLength(8), Category("PLC报文"), Description("12 Byte,类型为S7String")]
        public byte[] TargetDevice2 { set; get; } = new byte[8];

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
        /// AGV交互
        /// </summary>
        [DisplayName("AGV交互"), PackStart(106), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short AgvWcs { set; get; }

        /// <summary>
        /// 单双托
        /// </summary>
        [DisplayName("托盘类型"), PackStart(108), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short TrayType { set; get; }


        /// <summary>
        /// 预留1
        /// </summary>
        [DisplayName("预留1"), PackStart(110), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short Reserve1 { set; get; }

        /// <summary>
        /// 预留2
        /// </summary>
        [DisplayName("预留2"), PackStart(112), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short Reserve2 { set; get; }

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
        /// 目标设备1S
        /// </summary>
        [DisplayName("目标设备1S"), Description("字符串"), JsonIgnore()]
        public string TargetDevice1_S
        {
            get
            {
                return Sharp7.S7.GetStringAt(this.TargetDevice1, 0);
            }
        }

        /// <summary>
        /// 目标设备2S
        /// </summary>
        [DisplayName("目标设备2S"), Description("字符串"), JsonIgnore()]
        public string TargetDevice2_S
        {
            get
            {
                return Sharp7.S7.GetStringAt(this.TargetDevice2, 0);
            }
        }

        /// <summary>
        /// 托盘编码1S
        /// </summary>
        [DisplayName("托盘编码1S"), Description("字符串"), JsonIgnore()]
        public string TrayCode1_S
        {
            get
            {
                return Sharp7.S7.GetStringAt(this.TrayCode1, 0);
            }
        }

        /// <summary>
        /// 托盘编码2S
        /// </summary>
        [DisplayName("托盘编码2S"), Description("字符串"), JsonIgnore()]
        public string TrayCode2_S
        {
            get
            {
                return Sharp7.S7.GetStringAt(this.TrayCode2, 0);
            }
        }

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
        [DisplayName("包体大小"), Description("字节数,114 Byte"), JsonIgnore()]
        public override int PackSize
        {
            get
            {
                return (int)Class.GetClassSize(this);
            }
        }


        /// <summary>
        /// DB包列表
        /// </summary>


    }
    public class PackWriteDList
    {
        public PackWriteDList(int count)
        {
            this.List = new PackWriteD[count];
        }

        public PackWriteD[] List { set; get; }
    }
}

