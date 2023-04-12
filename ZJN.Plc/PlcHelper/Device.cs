using HSZ.Common.Extension;
using HSZ.Dependency;
using NPOI.SS.Formula.Functions;
using S7.Net;
using S7.Net.Types;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static ZJN.Plc.PlcHelper.PackWriteD;

namespace ZJN.Plc.PlcHelper
{
    public class Device
    {
        public S7.Net.Plc Plc { set; get; } = null;
        public string Ip { get; set; }

        /// <summary>
        /// Plc Key;连接关键字
        /// </summary>
        public string PlcKey { set; get; } = "";

        /// <summary>
        /// 设备
        /// </summary>
        public string PlcDevice { set; get; } = "";

        private bool mIsConnected;
        public bool IsConnected
        {
            get
            {
                if (!this.Plc.IsConnected) mIsConnected = false;
                return mIsConnected;
            }
        }
        /// <summary>
        /// 创建PLC连接对象
        /// </summary>
        /// <param name="cpu">CPU型号</param>
        /// <param name="ip"></param>
        /// <param name="plcConnectId"></param>
        /// <param name="port"></param>
        /// <param name="rack"></param>
        /// <param name="sock"></param>
        /// <param name="key">PLC ID</param>
        /// <param name="device">指定设备,如堆垛机</param>
        /// <param name="timeout"></param>
        public Device(CpuType cpu, string ip, int port, short rack, short slot, string key, string device = "", int timeout = 2000)
        {
            this.Plc = new S7.Net.Plc(cpu, ip, rack, slot);
            this.Plc.ReadTimeout = timeout;
            this.Plc.WriteTimeout = timeout;
            this.Ip = ip;
            this.PlcKey = key;
            this.PlcDevice = device;
        }



        /// <summary>
        /// 内部循环用，关闭了设false
        /// </summary>
        private bool isLoop = false;


        /// <summary>
        /// 启动
        /// </summary>
        public async Task<bool> StartAsync()
        {
            try
            {
                mIsConnected = await PingAsync(this.Plc.IP, 300);
                if (mIsConnected)
                    await this.Plc.OpenAsync();
            }
            catch (
            Exception ex
            ) { /*throw new Exception($"PLC连接失败{ex}");*/ }
            mIsConnected = this.Plc.IsConnected;
            return mIsConnected;
        }


        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            try
            {
                this.isLoop = false;
                this.Plc.Close();
            }
            catch
            { }
            finally
            {
                this.mIsConnected = false;
            }
        }


        /// <summary>
        /// DB105.DBX10.1;DB105.DBW10.0;DB105.DBD10.0;MB28,T45
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public async Task<object> ReadObject(string point)
        {
            try
            {
                if (!this.IsConnected) await this.StartAsync();
                return this.Plc.Read(point);
            }
            catch
            {
                this.Close();
                return null;
            }
        }



        /// <summary>
        /// DB105.DBX10.1;DB105.DBW10.0;DB105.DBD10.0;MB28,T45
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="obj"></param>
        public async Task WriteObject(string point, object obj)
        {
            try
            {
                if (!this.IsConnected) await this.StartAsync();
                this.Plc.Write(point, obj);
            }
            catch
            {
                this.Close();
            }
        }


        /// <summary>
        /// 写DbClass
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="db"></param>
        /// <param name="start"></param>
        public async Task<bool> WriteDbClass(object obj, int db, int start = 0)
        {
            try
            {
                mIsConnected = await PingAsync(this.Plc.IP, 300);
                if (!mIsConnected)
                {
                    return false;
                }//await this.StartAsync();
                else
                {

                    this.Plc.WriteClass(obj, db, start);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 读DbClass（T中不能含不定长数组）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public async Task<T> ReadDbClass<T>(int db, int start = 0)
        {
            var t = Activator.CreateInstance<T>();
            try
            {
                if (!this.IsConnected) await this.StartAsync();
                this.Plc.ReadClass(t, db, start);

                //设置DB信息
                this.SetPackOfDbInfo(t, db, start);
            }
            catch
            {
            }
            return t;
        }


        /// <summary>
        /// 读取单个DbClass
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="db"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public object ReadDbClass(object obj, int db, int start = 0)
        {
            try
            {
                if (!this.IsConnected) _= this.StartAsync();
                this.Plc.ReadClass(obj, db, start);

                //设置DB信息
                this.SetPackOfDbInfo(obj, db, start);

                return obj;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        /// <summary>
        /// 读取DbClass
        /// </summary>
        /// <param name="classname"></param>
        /// <param name="db"></param>
        /// <param name="start"></param>
        /// <param name="listcount"></param>
        /// <returns></returns>
        public object ReadDbClass(string classname, int db, int start = 0, int listcount = 0)
        {
            var name = $"ZJN.Plc.PlcHelper.{classname}";
            Type type = Type.GetType(name);
            if (listcount == 0)
            {
                if (classname.Substring(0, 4) != "Pack")
                    throw new Exception("类名不匹配。务必:[Pack]开头");

                var obj = Activator.CreateInstance(type);
                return  this.ReadDbClass(obj, db, start);
            }
            else
            {
                if (classname.Substring(0, 4) != "Pack" && classname.Substring(classname.Length - 1, 4) != "List")
                    throw new Exception("类名不匹配。务必:[Pack]开头，[List]结尾");

                var obj = Activator.CreateInstance(type, listcount);
                return  this.ReadDbClass(obj, db, start);
            }
        }


        /// <summary>
        /// 读取DbClass
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="classname"></param>
        /// <param name="db"></param>
        /// <param name="start"></param>
        /// <param name="listcount"></param>
        /// <returns></returns>
        public  T ReadDbClass<T>(string classname, int db, int start = 0, int listcount = 0)
        {
            return (T)ReadDbClass(classname, db, start, listcount);
        }

        /// <summary>
        /// 对数据包设置DB信息
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="db"></param>
        /// <param name="start"></param>
        private void SetPackOfDbInfo(object obj, int db, int start = 0)
        {
            try
            {
                var i = 0;
                var type = obj.GetType().Name;

                switch (type)
                {
                    case "PackReadList":
                        var obj2 = obj as PackReadList;
                        obj2.List.ToList().ForEach(x => { x.SetCfg(this, db, start + x.PackSize * i); i++; });
                        break;
                    case "PackReadDList":
                        var obj3 = obj as PackReadDList;
                        obj3.List.ToList().ForEach(x => { x.SetCfg(this, db, start + x.PackSize * i); i++; });
                        break;
                    case "PackWriteList":
                        var obj4 = obj as PackWriteList;
                        obj4.List.ToList().ForEach(x => { x.SetCfg(this, db, start + x.PackSize * i); i++; });
                        break;
                    case "PackWriteDList":
                        var obj5 = obj as PackWriteDList;
                        obj5.List.ToList().ForEach(x => { x.SetCfg(this, db, start + x.PackSize * i); i++; });
                        break;
                    case "PackStatusList":
                        var obj6 = obj as PackStatusList;
                        obj6.List.ToList().ForEach(x => { x.SetCfg(this, db, start + x.PackSize * i); i++; });
                        break;
                    case "PackStatusDList":
                        var obj7 = obj as PackStatusDList;
                        obj7.List.ToList().ForEach(x => { x.SetCfg(this, db, start + x.PackSize * i); i++; });
                        break;
                    default:
                        var obj1 = obj as PackBase;
                        obj1.SetCfg(this, db, start);
                        break;
                }
            }
            catch 
            { 
            }
        }


        /// <summary>
        /// 读取Bb的bytes
        /// </summary>
        /// <param name="db"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public async Task<byte[]> ReadDbBytes(int db, int start, int length)
        {

            try
            {
                if (!this.IsConnected) await this.StartAsync();
                byte[] bytes = this.Plc.ReadBytes(DataType.DataBlock, db, start, length);
                return bytes;
            }
            catch
            {
                this.Close();
                return null;
            }
        }


        /// <summary>
        /// 写取Bb的bytes
        /// </summary>
        /// <param name="db"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="bytes"></param>
        public async Task WriteDbBytes(int db, int start, byte[] bytes)
        {
            try
            {
                if (!this.IsConnected) await this.StartAsync();

               await this.Plc.WriteBytesAsync(DataType.DataBlock, db, start, bytes);
            }
            catch
            {
                this.Close();
            }
        }

        /// <summary>
        /// 读S7String(包头byte+长度byte)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public async Task<string> ReadDbS7String(int db, int start)
        {
            try
            {
                if (!this.IsConnected) await this.StartAsync();

                var reservedLength = (byte)this.Plc.Read(DataType.DataBlock, db, start, VarType.Byte, 1);
                var obj = (string)this.Plc.Read(DataType.DataBlock, db, start, VarType.S7String, reservedLength);

                return obj.ToString();
            }
            catch
            {
                this.Close();
                return null;
            }
        }


        /// <summary>
        /// 写S7String(包头byte+长度byte)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="start"></param>
        /// <param name="data"></param>
        /// <param name="max"></param>

        public async Task WriteDbS7String(int db, int start, string data, int max = 0)
        {
            try
            {
                if (!this.IsConnected) await this.StartAsync();

                var bytes = GetS7StringToBytes(data, max);
                this.Plc.WriteBytes(DataType.DataBlock, db, start, bytes);
            }
            catch
            {
                this.Close();
            }
        }

        /// <summary>
        /// 从bytes获取对象 务必是对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public T GetClassFromBytes<T>(byte[] bytes)
        {
            var t = Activator.CreateInstance<T>();
            S7.Net.Types.Class.FromBytes(t, bytes);
            return t;
        }



        /// <summary>
        /// 把对象转成bytes ，务必是对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public byte[] GetClassToBytes(object obj)
        {
            byte[] array = new byte[(int)Class.GetClassSize(obj)];
            S7.Net.Types.Class.ToBytes(obj, array);

            return array;
        }


        /// <summary>
        /// S7String,string转成bytes
        /// </summary>
        /// <param name="data"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public byte[] GetS7StringToBytes(string data, int max = 0)
        {
            var length = Encoding.ASCII.GetBytes(data).Length;
            byte[] bytedb = new byte[max];
            if (length >= max && max > 0)
            {
                length = max;
                data = data.Substring(0, max);
            }
            //更新
            var bytes = S7.Net.Types.S7String.ToByteArray(data, length);
            for (int i = 0; i < length+2; i++)
            {
                bytedb[i] = bytes[i];
            }
            return bytedb;
        }


        /// <summary>
        /// S7String,byte[]转成string,长度自动识别
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public string GetS7StringToString(byte[] data, int start = 0)
        {
            return Sharp7.S7.GetStringAt(data, start);
        }

        /// <summary>  
        /// Ping主机  
        /// </summary>  
        /// <param name="ip">ip 地址或主机名或域名</param>  
        /// <returns>true 通，false 不通</returns>  
        public static async Task<bool> PingAsync(string ip, int timeout = 200)
        {
            System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
            System.Net.NetworkInformation.PingOptions options = new System.Net.NetworkInformation.PingOptions();
            options.DontFragment = true;
            string data = "Test Data!";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            // int timeout = 2000; // Timeout 时间，单位：毫秒  
            System.Net.NetworkInformation.PingReply reply =await p.SendPingAsync(ip, timeout, buffer, options);
            if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                return true;
            else
                return false;
        }
    }
}
