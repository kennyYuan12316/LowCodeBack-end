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
    [Route("LESServiceByCallEmpty")]
    public class CallbSoapServicesByCallEmpty : IDynamicApiController, ITransient
    {
        private readonly ILesService _LesService;

        public CallbSoapServicesByCallEmpty(ILesService service)
        {
            _LesService = service;
        }

        [HttpPost("")]
        public XmlOutputModelByCallEmpty Post([FromBody] XmlInputModelByCallEmpty location)
        {
            var input = new XmlOutputModel();
            var obj=  _LesService.GetType().GetMethod(location.Method)?.Invoke(_LesService, new object[] { location.Json });
            XmlOutputModelByCallEmpty xmlOutput =new XmlOutputModelByCallEmpty();
            xmlOutput.Method= location.Method;
            xmlOutput.Json = obj?.ToString();
            return xmlOutput;
        }
    }
}
