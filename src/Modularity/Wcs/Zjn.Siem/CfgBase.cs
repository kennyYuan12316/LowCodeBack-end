using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace Zjn.Siem
{
	/// <summary>
	/// ���ýӿ�
	/// </summary>
	public interface IConfiguration 
    {

    }
    /// <summary>
    /// Configuration ��ժҪ˵����
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
        /// �豸ID
        /// </summary>
        public string DeviceID { set; get; }

        /// <summary>
        /// �豸��
        /// </summary>
        public string Caption { set; get; }

        [XmlIgnore]
        [Browsable(true), ReadOnly(true), Category("���ƿ���"), DisplayName("ѭ��"), Description("�����˳�ʱ��false")]
        public bool IsLoop { set; get; } = true;

    }

    /// <summary>
    /// �����쳣��
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
            return String.Format("����[{0}]���ش���:{1}", this.configName, this.reason);
        }
    }


}
