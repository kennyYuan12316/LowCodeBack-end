using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace Zjn.Siem
{
	/// <summary>
	/// 配置接口
	/// </summary>
	public interface IConfiguration 
    {

    }
    /// <summary>
    /// Configuration 的摘要说明。
    /// </summary>
    public class CfgBase : IConfiguration, INotifyPropertyChanged, ICloneable
    {
        public CfgBase(string deviceid)
        {
            this.DeviceID = deviceid;
        }

        public virtual event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] String propertyname = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceID { set; get; }

        /// <summary>
        /// 设备号
        /// </summary>
        public string Caption { set; get; }

        [XmlIgnore]
        [Browsable(true), ReadOnly(true), Category("控制开关"), DisplayName("循环"), Description("程序退出时置false")]
        public bool IsLoop { set; get; } = true;

    }

    /// <summary>
    /// 加载异常类
    /// </summary>
    public class LoadException : Exception
    {
        private string configName;
        private string reason;

        public LoadException(string configName, string reason)
            : base(reason)
        {
            this.configName = configName;
            this.reason = reason;
        }

        public override string ToString()
        {
            return String.Format("配置[{0}]加载错误:{1}", this.configName, this.reason);
        }
    }


}
