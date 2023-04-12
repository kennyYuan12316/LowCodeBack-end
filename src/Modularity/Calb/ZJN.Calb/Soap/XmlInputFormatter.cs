using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ZJN.Calb
{
    public class XmlInputFormatter : TextInputFormatter
    {
        public XmlInputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/xml"));
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        protected override bool CanReadType(Type type)
            => type == typeof(XmlInputModel);

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(
            InputFormatterContext context, Encoding effectiveEncoding)
        {
            var httpContext = context.HttpContext;
            var serviceProvider = httpContext.RequestServices;

            var logger = serviceProvider.GetRequiredService<ILogger<XmlInputFormatter>>();
          
            try
            {
                using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8);
                var xmlText = await reader.ReadToEndAsync();
                XElement node = XElement.Parse(xmlText);
               
                var val=
                from body in node.Descendants()
                where body.Name.LocalName=="Body"
                select new XmlInputModel 
                { Method = ((XElement)body.FirstNode).Name.LocalName, Json = body.Value };
                
                return await InputFormatterResult.SuccessAsync(val?.FirstOrDefault());
            }
            catch
            {
                return await InputFormatterResult.FailureAsync();
            }
        }
    }
}
