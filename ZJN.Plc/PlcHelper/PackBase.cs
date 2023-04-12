using Newtonsoft.Json;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Text;

namespace ZJN.Plc.PlcHelper
{
    /// <summary>
    /// DB包基类,按bytes顺序定义，不可跳格
    /// </summary>
    public class PackBase
    {

        private Device plc = null;

        private string ip = "";

        private string plcid = "";

        private int db = 0;

        private int start = 0;

        protected string device = "";


        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="plc"></param>
        /// <param name="db"></param>
        /// <param name="start"></param>
        public void SetCfg(Device plc, int db, int start)
        {
            this.plc = plc;
            this.plcid = plc != null ? plc.PlcKey : "";
            this.ip = plc != null ? plc.Ip : "";
            this.db = db;
            this.start = start;
            this.device = plc.PlcDevice;
        }

        /// <summary>
        /// 读方法
        /// </summary>
        /// <returns></returns>
        public object Read()
        {
            return this.plc.ReadDbClass(this, this.db, this.start);
        }


        /// <summary>
        /// 写方法
        /// </summary>
        public async Task<bool> WriteAsync()
        {
            try
            {
                await this.plc.WriteDbClass(this, this.db, this.start);
                return true;
            }
            catch { return false; }
        }


        /// <summary>
        /// PLc连接对象
        /// </summary>
        [JsonIgnore()]
        public Device Plc
        {
            get { return this.plc; }
        }

        public string Ip
        {
            get { return this.ip; }
        }


        [JsonIgnore()]
        public string PlcID
        {
            get { return this.plcid; }
        }

        /// <summary>
        /// db
        /// </summary>
        [JsonIgnore()]
        public string PlcKey
        {
            get { return this.plc == null ? "" : this.plc.PlcKey; }
        }

        /// <summary>
        /// db
        /// </summary>
        [JsonIgnore()]
        public int Db
        {
            get { return this.db; }
        }

        /// <summary>
        /// 起始位
        /// </summary>
        [JsonIgnore()]
        public int Start
        {
            get { return this.start; }
        }

        /// <summary>
        /// 设备名
        /// </summary>
        [JsonIgnore()]
        public string DeviceNick
        {
            get { return this.device; }
        }

        /// <summary>
        /// 包对象名称
        /// </summary>
        [JsonIgnore()]
        public string ObjectType
        {
            get { return this.GetType().Name; }
        }

        /// <summary>
        /// 包对象名称
        /// </summary>
        [JsonIgnore()]
        public virtual string PackType
        {
            get { return "READ"; }
        }


        /// <summary>
        /// 包体大小
        /// </summary>
        [JsonIgnore()]
        public virtual int PackSize { get; }

        /// <summary>
        /// S7String,string转成bytes
        /// </summary>
        /// <param name="data"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public byte[] GetS7StringToBytes(string data, int max = 0)
        {
            var bytes = this.plc.GetS7StringToBytes(data, max);

            return bytes;
        }

    }
}

