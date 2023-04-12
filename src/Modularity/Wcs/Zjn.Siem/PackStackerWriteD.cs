using Newtonsoft.Json;
using S7.Net.Types;
using System.ComponentModel;

namespace Zjn.Siem
{
    /// <summary>
    /// PLC堆垛机控制包（双），读，86 Byte
    /// </summary>
    public class PackStackerWriteD:PackStackerWrite 
    {
        /// <summary>
        /// 任务序号
        /// </summary>
        [DisplayName("任务序号2"), PackStart(110), PackLength(4), Category("PLC报文"), Description("2 Byte;")]
        public int JobID2 { set; get; }

        /// <summary>
        /// 起点行
        /// </summary>
        [DisplayName("起点行2"), PackStart(114), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short F_Row2 { set; get; }

        /// <summary>
        /// 起点列
        /// </summary>
        [DisplayName("起点列2"), PackStart(116), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short F_Col2 { set; get; }

        /// <summary>
        /// 起点层
        /// </summary>
        [DisplayName("起点层2"), PackStart(118), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short F_Lay2 { set; get; }

        /// <summary>
        /// 终点行
        /// </summary>
        [DisplayName("终点行2"), PackStart(120), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short T_Row2 { set; get; }

        /// <summary>
        /// 终点列
        /// </summary>
        [DisplayName("终点列2"), PackStart(122), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short T_Col2 { set; get; }

        /// <summary>
        /// 终点层
        /// </summary>
        [DisplayName("终点层2"), PackStart(124), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short T_Lay2 { set; get; }

        /// <summary>
        /// 
        /// </summary>
        [DisplayName("起点货叉1"), PackStart(126), PackLength(2), Category("PLC报文"), Description("2 Byte;0复位；1叉1；2叉2；3双叉")]
        public short F_ForkChoice { set; get; }

        /// <summary>
        /// 
        /// </summary>
        [DisplayName("起点货叉2"), PackStart(128), PackLength(2), Category("PLC报文"), Description("2 Byte;0复位；1叉1；2叉2；3双叉")]
        public short F_ForkChoice2 { set; get; }


        /// <summary>
        /// 
        /// </summary>
        [DisplayName("终点货叉1"), PackStart(130), PackLength(2), Category("PLC报文"), Description("2 Byte;0复位；1叉1；2叉2；3双叉")]
        public short T_ForkChoice { set; get; }

        /// <summary>
        /// 
        /// </summary>
        [DisplayName("终点货叉2"), PackStart(132), PackLength(2), Category("PLC报文"), Description("2 Byte;0复位；1叉1；2叉2；3双叉")]
        public short T_ForkChoice2 { set; get; }

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
        [DisplayName("包体大小"), Description("字节数,62+24 Byte"), JsonIgnore()]
        public override int PackSize
        {
            get
            {
                return (int)Class.GetClassSize(this);
            }
        }
    }
}
