using Newtonsoft.Json;
using S7.Net.Types;
using System.ComponentModel;

namespace ZJN.Plc.PlcHelper
{
    /// <summary>
    /// 双工位Rgv的写DB
    /// </summary>
    public class PackStackerWriteD : PackBase
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
        /// 启动命令
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
        public int Job_One_ID { set; get; }

        /// <summary>
        /// 工作命令代码fromPC（1入库，2出库，3移库，4移动到，5消防）
        /// </summary>
        [DisplayName("任务命令"), PackStart(6), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short JobCommand { set; get; }


        /// <summary>
        /// 起点行
        /// </summary>
        [DisplayName("起点行"), PackStart(8), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short F_One_Row { set; get; }

        /// <summary>
        /// 起点列
        /// </summary>
        [DisplayName("起点列"), PackStart(10), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short F_One_Col { set; get; }

        /// <summary>
        /// 起点层
        /// </summary>
        [DisplayName("起点层"), PackStart(12), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short F_One_Lay { set; get; }

        /// <summary>
        /// 终点行
        /// </summary>
        [DisplayName("终点行"), PackStart(14), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short T_One_Row { set; get; }

        /// <summary>
        /// 终点列
        /// </summary>
        [DisplayName("终点列"), PackStart(16), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short T_One_Col { set; get; }

        /// <summary>
        /// 终点层
        /// </summary>
        [DisplayName("终点层"), PackStart(18), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short T_One_Lay { set; get; }

        /// <summary>
        /// 提交完成
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
        public short Reserve11 { set; get; }

        /// <summary>
        /// 预留12
        /// </summary>
        [DisplayName("预留12"), PackStart(30), PackLength(2), Category("PLC报文"), Description("2 byte")]
        public short Reserve12 { set; get; }

        /// <summary>
        /// RGV-PC消防水箱处请求放料
        /// </summary>
        [DisplayName("RGV-PC消防水箱处请求放料"), PackStart(32), PackLength(2), Category("PLC报文"), Description("1 bit")]
        public bool FireRequestDown { get; set; }


        /// <summary>
        /// RGV-PC消防水箱处放料完成
        /// </summary>
        [DisplayName("RGV-PC消防水箱处放料完成"), PackStart(32), PackLength(2), Category("PLC报文"), Description("1 bit")]
        public bool FireDownOver { get; set; }

        /// <summary>
        /// PC-RGV消防水箱处允许放料
        /// </summary>
        [DisplayName("PC-RGV消防水箱处允许放料"), PackStart(32), PackLength(2), Category("PLC报文"), Description("1 bit")]
        public bool FireAllowDown { get; set; }

        public bool IsReserve9 { set; get; }
        public bool IsReserve10 { set; get; }
        public bool IsReserve11 { set; get; }
        public bool IsReserve12 { set; get; }
        public bool IsReserve13 { set; get; }


        /// <summary>
        /// 开机时间M
        /// </summary>
        [DisplayName("开机时间M"), PackStart(34), PackLength(4), Category("PLC报文"), Description("1 bit")]
        public int OpenMachineTime { get; set; }



        /// <summary>
        /// 手动时间M
        /// </summary>
        [DisplayName("手动时间M"), PackStart(38), PackLength(4), Category("PLC报文"), Description("1 bit")]
        public int ManualMachineTime { get; set; }


        /// <summary>
        /// 手动故障时间M
        /// </summary>
        [DisplayName("手动故障时间M"), PackStart(42), PackLength(4), Category("PLC报文"), Description("1 bit")]
        public int ManualAlarmTime { get; set; }


        /// <summary>
        /// 自动运行时间M
        /// </summary>
        [DisplayName("自动运行时间M"), PackStart(46), PackLength(4), Category("PLC报文"), Description("1 bit")]
        public int AutoRunTime { get; set; }


        /// <summary>
        /// 自动待机时间M
        /// </summary>
        [DisplayName("自动待机时间M"), PackStart(50), PackLength(4), Category("PLC报文"), Description("1 bit")]
        public int AutoStandbyTime { get; set; }


        /// <summary>
        /// 自动故障时间M
        /// </summary>
        [DisplayName("自动故障时间M"), PackStart(54), PackLength(4), Category("PLC报文"), Description("1 bit")]
        public int AutoAlarmTime { get; set; }


        /// <summary>
        /// 任务运行时间M
        /// </summary>
        [DisplayName("任务运行时间M"), PackStart(58), PackLength(4), Category("PLC报文"), Description("1 bit")]
        public int TaskRunTime { get; set; }


        /// <summary>
        /// 任务序号
        /// </summary>
        [DisplayName("任务序号"), PackStart(62), PackLength(4), Category("PLC报文"), Description("2 Byte;")]
        public int Job_Two_ID { set; get; }


        /// <summary>
        /// 起点行
        /// </summary>
        [DisplayName("起点行"), PackStart(66), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short F_Two_Row { set; get; }

        /// <summary>
        /// 起点列
        /// </summary>
        [DisplayName("起点列"), PackStart(68), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short F_Two_Col { set; get; }

        /// <summary>
        /// 起点层
        /// </summary>
        [DisplayName("起点层"), PackStart(70), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short F_Two_Lay { set; get; }

        /// <summary>
        /// 终点行
        /// </summary>
        [DisplayName("终点行"), PackStart(72), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short T_Two_Row { set; get; }

        /// <summary>
        /// 终点列
        /// </summary>
        [DisplayName("终点列"), PackStart(74), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short T_Two_Col { set; get; }

        /// <summary>
        /// 终点层
        /// </summary>
        [DisplayName("终点层"), PackStart(76), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short T_Two_Lay { set; get; }


        /// <summary>
        /// 1次起始货叉选择 0复位  1 1号货叉 2 2号货叉  3  双货叉
        /// </summary>
        [DisplayName("1次起始货叉选择 0复位  1 1号货叉 2 2号货叉  3  双货叉"), PackStart(78), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short Start_one_fork { set; get; }

        /// <summary>
        /// 2次起始货叉选择 0复位  1 1号货叉 2 2号货叉  3  双货叉
        /// </summary>
        [DisplayName("2次起始货叉选择 0复位  1 1号货叉 2 2号货叉  3  双货叉"), PackStart(80), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short Start_two_fork { set; get; }



        /// <summary>
        /// 1次目标货叉选择 0复位  1 1号货叉 2 2号货叉  3  双货叉
        /// </summary>
        [DisplayName("1次目标货叉选择 0复位  1 1号货叉 2 2号货叉  3  双货叉"), PackStart(82), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short Target_one_fork { set; get; }

        /// <summary>
        /// 2次目标货叉选择 0复位  1 1号货叉 2 2号货叉  3  双货叉
        /// </summary>
        [DisplayName("2次目标货叉选择 0复位  1 1号货叉 2 2号货叉  3  双货叉"), PackStart(84), PackLength(2), Category("PLC报文"), Description("2 Byte;")]
        public short Target_two_fork { set; get; }

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
        [DisplayName("包体大小"), Description("字节数,86 Byte"), JsonIgnore()]
        public override int PackSize
        {
            get
            {
                return (int)Class.GetClassSize(this);
            }
        }
    }
}
