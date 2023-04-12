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
    public class XmlOutputFormatterByCallEmpty : TextOutputFormatter
    {
        public XmlOutputFormatterByCallEmpty()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/xml"));
            SupportedEncodings.Add(Encoding.UTF8);
        }

        protected override bool CanWriteType(Type type)
            => typeof(XmlOutputModelByCallEmpty).IsAssignableFrom(type)
                || typeof(IEnumerable<XmlOutputModelByCallEmpty>).IsAssignableFrom(type);


        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var httpContext = context.HttpContext;
            var xml = context.Object as XmlOutputModelByCallEmpty;
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">");
            builder.AppendLine("   <s:Body>");
            builder.AppendLine($"      <{xml.Method}Response xmlns=\"http://tempuri.org/\">");
            builder.AppendLine($"            <{xml.Method}Result>{xml.Json}</{xml.Method}Result>");
            builder.AppendLine($"      </{xml.Method}Response>");
            builder.AppendLine("   </s:Body>");
            builder.AppendLine("</s:Envelope>");

            await httpContext.Response.WriteAsync(builder.ToString(), selectedEncoding);
        }
    }
}
