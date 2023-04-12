
using HSZ.WorkFlow.Entitys.Model.Properties;
using HSZ.WorkFlow.Entitys.Model;
using Mapster;
using NPOI.POIFS.Crypt.Dsig;
using System.Text;

namespace ZJN.Plc.PlcHelper
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：智佳能-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    class Mapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<PackReadOutput, PackRead>()
.Map(dest => dest.DeviceCode, src => GetS7StringToBytes(src.DeviceCode_S, 6))
.Map(dest => dest.TrayCode, src => GetS7StringToBytes(src.TrayCode_S, 32))
.Map(dest => dest.TargetDevice, src => GetS7StringToBytes(src.TargetPosition_S, 6));

            config.ForType<PackWriteOutput, PackWrite>()
.Map(dest => dest.DeviceCode, src => GetS7StringToBytes(src.DeviceCode_S, 6))
.Map(dest => dest.TrayCode, src => GetS7StringToBytes(src.TrayCode_S, 18))
.Map(dest => dest.TargetDevice, src => GetS7StringToBytes(src.TargetDevice_S, 6));

            config.ForType<PackStatusOutput, PackStatus>()
.Map(dest => dest.DeviceCode, src => GetS7StringToBytes(src.DeviceCode_S, 6))
.Map(dest => dest.TrayCode, src => GetS7StringToBytes(src.TrayCode_S, 32))
.Map(dest => dest.TargetDevice, src => GetS7StringToBytes(src.TargetDevice_S, 6));

            config.ForType<PackReadDOutput, PackReadD>()
.Map(dest => dest.DeviceCode, src => GetS7StringToBytes(src.DeviceCode_S, 6))
.Map(dest => dest.TrayCode1, src => GetS7StringToBytes(src.TrayCode1_S, 32))
.Map(dest => dest.TrayCode2, src => GetS7StringToBytes(src.TrayCode2_S, 32))
.Map(dest => dest.TargetDevice1, src => GetS7StringToBytes(src.TargetDevice1_S, 6))
.Map(dest => dest.TargetDevice2, src => GetS7StringToBytes(src.TargetDevice2_S, 6));

            config.ForType<PackWriteDOutput, PackWriteD>()
.Map(dest => dest.DeviceCode, src => GetS7StringToBytes(src.DeviceCode_S, 6))
.Map(dest => dest.TrayCode1, src => GetS7StringToBytes(src.TrayCode1_S, 32))
.Map(dest => dest.TrayCode2, src => GetS7StringToBytes(src.TrayCode2_S, 32))
.Map(dest => dest.TargetDevice1, src => GetS7StringToBytes(src.TargetDevice1_S, 6))
.Map(dest => dest.TargetDevice2, src => GetS7StringToBytes(src.TargetDevice2_S, 6));

            config.ForType<PackStatusDOutput, PackStatusD>()
.Map(dest => dest.DeviceCode, src => GetS7StringToBytes(src.DeviceCode_S, 6))
.Map(dest => dest.TrayCode1, src => GetS7StringToBytes(src.TrayCode1_S, 32))
.Map(dest => dest.TrayCode2, src => GetS7StringToBytes(src.TrayCode2_S, 32))
.Map(dest => dest.TargetDevice1, src => GetS7StringToBytes(src.TargetPosition1_S, 6))
.Map(dest => dest.TargetDevice2, src => GetS7StringToBytes(src.TargetPosition2_S, 6));

        }
        private byte[] GetS7StringToBytes(string data,int max=0)
        {
            var length = Encoding.ASCII.GetBytes(data).Length;
            if (length >= max && max > 0)
            {
                length = max;
                data = data.Substring(0, max);
            }
            var bytes = S7.Net.Types.S7String.ToByteArray(data, max);
            return bytes;
        }
    }
}
