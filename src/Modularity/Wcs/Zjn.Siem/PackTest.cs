using S7.Net.Types;

namespace Zjn.Siem
{

    /// <summary>
    /// DB包列表
    /// </summary>
    public class PackTestList
    {
        public PackTestList(int count)
        {
            this.List = new PackTest[count];
        }

        public PackTest[] List { set; get; }
    }

    public class PackTest : PackBase
    {
        public byte[] 设备编号 { set; get; } = new byte[8];
        public int 任务号 { set; get; }

        public byte[] 目标位置编号 { set; get; } = new byte[8];

        public byte[] 托盘条码 { set; get; } = new byte[20];

        public short 空托满托 { set; get; }

        public short 握手类型 { set; get; }

        public short 预留_1 { set; get; }
        public short 预留_2 { set; get; }

        public string 设备编号_Str
        {
            get
            {
                return Sharp7.S7.GetStringAt(this.设备编号, 0);
            }
        }

        public string 目标位置编号_Str
        {
            get
            {
                return Sharp7.S7.GetStringAt(this.目标位置编号, 0);
            }
        }


        public string 托盘条码_Str
        {
            get
            {
                return Sharp7.S7.GetStringAt(this.托盘条码, 0);
            }
        }

        /// <summary>
        /// 大小
        /// </summary>
        public override int PackSize
        {
            get
            {
                return (int)Class.GetClassSize(this);
            }
        }
    }
}
