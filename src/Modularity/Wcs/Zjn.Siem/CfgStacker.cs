using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zjn.Siem
{
    public class CfgStacker : CfgBase
    {
        public CfgStacker(string device) : base(device)
        {
            this.DeviceID = device;
            this.Caption = "堆垛机";
        }

        /// <summary>
        /// 保存锁
        /// </summary>
        private object lock_save = new object();

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="WriteDB">是否写库</param>
        /// <param name="insert">是否插入</param>
        /// <param name="copy">是否拷贝</param>
        public void Save()
        {
            try
            {
                lock (lock_save)
                {
                    var json = Common.Serializer<CfgStacker>(this);
                    //保存deviceid json
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <returns></returns>
        public static CfgStacker Load(string device, string json)
        {
            try
            {
                var obj = Common.Deserialize<CfgStacker>(json);
                if (obj == null) obj = new CfgStacker(device);
                obj.DeviceID = device;
                return obj;
            }
            catch (Exception ex)
            {
                throw new LoadException("配置表加载", ex.Message);
            }
        }
    }
}
