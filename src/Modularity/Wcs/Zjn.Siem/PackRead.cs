using Newtonsoft.Json;
using S7.Net.Types;
using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Zjn.Siem
{
    /// <summary>
    /// 请求类型
    /// </summary>
    public enum RequestPlc : short
    {
        Null=0,
        /// <summary>
        /// 请求堆垛机
        /// </summary>
        Stacker ,
        /// <summary>
        /// 请求目标位置
        /// </summary>
        TargetDevice,
        /// <summary>
        /// 请求判断物料类型
        /// </summary>
        JudeType,
        /// <summary>
        /// 请求校验
        /// </summary>
        Verification,
        /// <summary>
        /// 请求Agv或Rgv
        /// </summary>
        AgvOrRgv,
    }


    /// <summary>
    /// Agv交互
    /// </summary>
    public enum AgvPlc:short
    {
        /// <summary>
        /// 初始化
        /// </summary>
        Init=0,

        /// <summary>
        /// 允许取料
        /// </summary>
        PullAllow = 1,
        /// <summary>
        /// 响应取料完成
        /// </summary>
        PullResponse,

        /// <summary>
        /// 允许放料
        /// </summary>
        PushAllow,

        /// <summary>
        /// 响应放料完成
        /// </summary>
        PushResponse,

    }



    /// <summary>
    /// PLC常规控制包，读
    /// </summary>
    public class PackRead : PackBase
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
        public short ProductPlc { set; get; }


        /// <summary>
        /// 握手类型
        /// </summary>
        [DisplayName("握手类型"), PackStart(56), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short RequestPlc { set; get; }





        /// <summary>
        /// Agv交互
        /// </summary>
        [DisplayName("AGV交互"), PackStart(58), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short AgvPlc { set; get; }


        /// <summary>
        /// 预留1
        /// </summary>
        [DisplayName("预留1"), PackStart(60), PackLength(2), Category("PLC报文"), Description("2 Byte")]
        public short Reserve1 { set; get; }

        /// <summary>
        /// 预留2
        /// </summary>
        [DisplayName("预留2"), PackStart(62), PackLength(2), Category("PLC报文"), Description("2 Byte")]
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
        public string TargetPosition_S
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
        public RequestPlc RequestPlcEnum { get { return (RequestPlc)this.RequestPlc; } }


        /// <summary>
        /// AGV交互
        /// </summary>
        [JsonIgnore()]
        public AgvPlc AgvPlcEnum { get { return (AgvPlc)this.AgvPlc; } }

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
    public class PackReadList
    {
        public PackReadList(int count)
        {
            this.List = new PackRead[count];
        }

        public PackRead[] List { set; get; }
    }
}

