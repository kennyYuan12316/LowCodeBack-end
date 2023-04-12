using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ZJN.Calb
{
    public class XmlInputFormatterByCallEmpty : TextInputFormatter
    {
        public XmlInputFormatterByCallEmpty()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/xml"));
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        protected override bool CanReadType(Type type)
            => type == typeof(XmlInputModelByCallEmpty);

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(
            InputFormatterContext context, Encoding effectiveEncoding)
        {
            var httpContext = context.HttpContext;
            var serviceProvider = httpContext.RequestServices;

            var logger = serviceProvider.GetRequiredService<ILogger<XmlInputFormatterByCallEmpty>>();
            try
            {
                using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8);
                var text = await reader.ReadToEndAsync();
                Log.Information(text);
                int startIndex = text.IndexOf("<tem:json>");
                int endIndex = text.IndexOf("</tem:json>");

                var json = text.Substring(startIndex + 10, endIndex - startIndex - 10);
                XmlInputModelByCallEmpty xmlInput = new XmlInputModelByCallEmpty();
                if (text.IndexOf("ARTIFICIAL_CALLEMPTYTRAY") > 0)
                {
                    xmlInput.Method = "ARTIFICIAL_CALLEMPTYTRAY";
                }
                else if (text.IndexOf("CONTAINER_INTO_STOP") > 0)
                { 
                    xmlInput.Method = "CONTAINER_INTO_STOP";
                }
                else
                {
                    return await InputFormatterResult.FailureAsync();
                }

                xmlInput.Json = json;
                return await InputFormatterResult.SuccessAsync(xmlInput);
            }
            catch (Exception)
            {
                return await InputFormatterResult.FailureAsync();
            }
        }
    }
}
