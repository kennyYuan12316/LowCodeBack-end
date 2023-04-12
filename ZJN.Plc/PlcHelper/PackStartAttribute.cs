using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ZJN.Plc.PlcHelper
{
    /// <summary>
    /// 点位地址
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class PackAddAttribute : Attribute
    {

        public PackAddAttribute(string add)
        {
            this.mPackAdd = add;
        }


        private string mPackAdd = "";

        /// <summary>
        /// 点位地址
        /// </summary>
        public string PackAdd
        {
            get
            {
                return mPackAdd;
            }
            set
            {
                mPackAdd = value;
            }
        }
    }

    /// <summary>
    /// 起始位置
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class PackStartAttribute : Attribute
    {

        public PackStartAttribute(int start)
        {
            this.mPackStart = start;
        }


        private int mPackStart = 0;

        /// <summary>
        /// 是否写日志
        /// </summary>
        public int PackStart
        {
            get
            {
                return mPackStart;
            }
            set
            {
                mPackStart = value;
            }
        }
    }


    /// <summary>
    /// 包体长度
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class PackLengthAttribute : Attribute
    {

        public PackLengthAttribute(double length)
        {
            this.mPackLength = length;
        }


        private double mPackLength = 0;

        /// <summary>
        /// 是否写日志
        /// </summary>
        public double PackStart
        {
            get
            {
                return mPackLength;
            }
            set
            {
                mPackLength = value;
            }
        }
    }

}
