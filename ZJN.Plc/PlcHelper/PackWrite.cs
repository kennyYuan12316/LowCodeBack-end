using Newtonsoft.Json;
using S7.Net.Types;
using System.ComponentModel;

namespace ZJN.Plc.PlcHelper
{
    /// <summary>
    /// 物料类型
    /// </summary>
    public enum ProductType : short
    {
        Null = 0,
        A实盘箔材,
        A空箱,
        B实盘箔材,
        B空箱,
        C实盘箔材,
        C空箱,
        D实盘箔材,
        D空箱
    }

    public enum TrayType : short
    {
        Null = 0,
        大,
        小,
    }


    /// <summary>
    /// 请求类型
    /// </summary>
    public enum ResponseWcs : short
    {
        Null = 0,
        /// <summary>
        /// 响应堆垛机
        /// </summary>
        Stacker,
        /// <summary>
        /// 响应目标位置
        /// </summary>
        TargetDevice,
        /// <summary>
        /// 响应判断物料类型
        /// </summary>
        JudeType,
        /// <summary>
        /// 响应校验
        /// </summary>
        Verification,
        /// <summary>
        /// 响应Agv或Rgv
        /// </summary>
        AgvOrRgv,
    }


    /// <summary>
    /// Agv交互
    /// </summary>
    public enum AgvWcs:short
    {
        /// <summary>
        /// 初始化
        /// </summary>
        Init = 0,

        /// <summary>
        /// 允许取料
        /// </summary>
        PullRequest = 1,
        /// <summary>
        /// 响应取料完成
        /// </summary>
        PullComplete,

        /// <summary>
        /// 允许放料
        /// </summary>
        PushRequest,

        /// <summary>
        /// 响应放料完成
        /// </summary>
        PushComplete,

    }



    /// <summary>
    /// PLC常规控制包，写
    /// </summary>
    public class PackWrite : PackBase
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        [DisplayName("设备编号"), PackStart(0), PackLength(8), Category("PLC报文"), Description("12 Byte,类型为S7String")]
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
        [DisplayName("托盘条码"), PackStart(20), PackLength(20), Category("PLC报文"), Description("36 Byte,类型为S7String")]
        public byte[] TrayCode { set; get; } = new byte[20];


        /// <summary>
        /// 空托满托
        /// </summary>
        [DisplayName("空托满托"), PackStart(40), PackLength(2), Category("PLC报文"), Description("2 Byte;随场景定义")]
        public short ProductWcs { set; get; }


        /// <summary>
        /// 握手类型
        /// </summary>
        [DisplayName("握手类型"), PackStart(42), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short ResponseWcs { set; get; }


        /// <summary>
        /// AGV交互 by--xyl 随便给的起始位置
        /// </summary>
        //[DisplayName("AGV交互"), PackStart(30), PackLength(2), Category("PLC报文"), Description("2 Byte")]
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


        ///// <summary>
        ///// AGV交互
        ///// </summary>
        //[JsonIgnore()]
        //public AgvWcs AgvPlcEnum { get { return (AgvWcs)this.AgvWcs; } }

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
        [DisplayName("包体大小"), Description("字节数,48 Byte"), JsonIgnore()]
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
    public class PackWriteList
    {
        public PackWriteList(int count)
        {
            this.List = new PackWrite[count];
        }

        public PackWrite[] List { set; get; }
    }
}

