using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZJN.Plc.PlcHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using S7.Net;
using HSZ.Common.Extension;
using NPOI.SS.Formula.Functions;

namespace ZJN.Plc.PlcHelper.Tests
{
    [TestClass()]
    public class DeviceTests
    {
        /// <summary>
        /// 代码修改read db模拟plc完成
        /// </summary>
        /// <returns></returns>
        [TestMethod()]
        public async Task WriteDbClassTest()
        {
            try
            {
                Device device = new Device(S7.Net.CpuType.S71500, "192.168.1.101", 102, 0, 0, "003");
                await device.StartAsync();
                if (device.IsConnected)
                {
                    var packRead = device.ReadDbClass<PackRead>("PackRead", 101, 0);
                    packRead.RequestPlc = 2;
                    packRead.Reserve1 = 1000;
                    string traycode = "888";
                    packRead.TrayCode = packRead.GetS7StringToBytes(traycode, traycode.Length + 2);
                    await packRead.WriteAsync();
                }
                device.Close();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}