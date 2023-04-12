using HSZ.Entitys.wms;
using Microsoft.AspNetCore.Http;
using Serilog;
using SoapCore.Extensibility;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Yitter.IdGenerator;

namespace ZJN.Calb
{
    public class LESSoapMessageProcessor : ISoapMessageProcessor
    {
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="LESSoapMessageProcessor"/>类型的新实例
        /// </summary>
        public LESSoapMessageProcessor()
        {
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        public async Task<Message> ProcessMessage(Message message, HttpContext context, Func<Message, Task<Message>> next)
        {
            ZjnTaskInterfaceApplyLogEntity zjnTaskInterfaceApplyLogEntity = new ZjnTaskInterfaceApplyLogEntity();
            var buff = message.GetReaderAtBodyContents();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(buff);
            var nodeFirst = xmlDocument.ChildNodes[0];

            if (nodeFirst.FirstChild == null || nodeFirst.FirstChild.NodeType != XmlNodeType.Element)
            {
                nodeFirst.InnerXml = "<json>" + nodeFirst.InnerXml + "</json>";
            }
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(xmlDocument.OuterXml));
            var xmlReader = XmlReader.Create(ms);
            var mes = Message.CreateMessage(message.Version, null, xmlReader);

            zjnTaskInterfaceApplyLogEntity.Id = YitIdHelper.NextId().ToString();
            zjnTaskInterfaceApplyLogEntity.Address = context.Connection.RemoteIpAddress.ToString() + ":" + context.Connection.RemotePort.ToString();
            zjnTaskInterfaceApplyLogEntity.InterfaceName = nodeFirst.LocalName;//buff.LocalName.ToString();
            zjnTaskInterfaceApplyLogEntity.EnterParameter = nodeFirst.InnerXml;
            zjnTaskInterfaceApplyLogEntity.OutParameter = "";
            zjnTaskInterfaceApplyLogEntity.CreateTime = DateTime.Now;
            zjnTaskInterfaceApplyLogEntity.Msg = "";
            _db.Insertable(zjnTaskInterfaceApplyLogEntity).ExecuteCommand();

            return await next(mes);
        }
    }
}
