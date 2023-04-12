using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZJN.Calb
{
    public class XmlOutputFormatter : TextOutputFormatter
    {
        public XmlOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/json"));
            SupportedEncodings.Add(Encoding.UTF8);
        }

        protected override bool CanWriteType(Type? type)
            => typeof(XmlOutputModel).IsAssignableFrom(type)
                || typeof(IEnumerable<XmlOutputModel>).IsAssignableFrom(type);


        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var httpContext = context.HttpContext;
            var xml = context.Object as XmlOutputModel;
            await httpContext.Response.WriteAsync(xml.Json, selectedEncoding);
        }
    }
}
