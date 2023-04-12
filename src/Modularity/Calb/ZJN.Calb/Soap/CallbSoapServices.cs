using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Logging.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZJN.Calb.Soap
{
    [AllowAnonymous,IgnoreLog]
    [NonUnify]
    [Route("LESService")]
    public class CallbSoapServices: IDynamicApiController, ITransient
    {
        private readonly ILesService _LesService;

        public CallbSoapServices(ILesService service)
        {
            _LesService = service;
        }

        [HttpPost("")]
        public XmlOutputModel Post([FromBody] XmlInputModel location)
        {
            var input = new XmlOutputModel();
            var obj=  _LesService.GetType().GetMethod(location.Method)?.Invoke(_LesService, new object[] { location.Json });
            XmlOutputModel xmlOutput=new XmlOutputModel();
            xmlOutput.Method= location.Method;
            xmlOutput.Json = obj?.ToString();
            return xmlOutput;
        }
    }
}
