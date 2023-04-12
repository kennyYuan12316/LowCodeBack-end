using HSZ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SoapCore;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ZJN.Calb.Client;

namespace ZJN.Calb
{
    /// <summary>
    ///
    /// </summary>
    [AppStartup(9)]
    public class Startup : AppStartup
    {

        public void ConfigureServices(IServiceCollection services)
        {
            var configurationBuilder = new ConfigurationBuilder().AddJsonFile("CalbConfig.json");
            IConfiguration config = configurationBuilder.Build();
            services.AddOptions().Configure<WebSerivcesConfig>(x=>config.GetSection("WebSerivces").Bind(x));
            services.AddOptions().Configure<LESLoginConfig>(x => config.GetSection("LESLoagin").Bind(x));
            services.AddScoped<LESServerClient>();
            services.AddScoped<MESServerClient>();
            services.AddSoapCore();
            services.AddSoapMessageProcessor(new LESSoapMessageProcessor());
            services.ConfigureAll<MvcOptions>(options =>
            {
                options.InputFormatters.Insert(0, new XmlInputFormatter());
                options.OutputFormatters.Insert(0, new XmlOutputFormatter());
                options.InputFormatters.Insert(1, new XmlInputFormatterByCallEmpty());
                options.OutputFormatters.Insert(1, new XmlOutputFormatterByCallEmpty());
            });

            
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.UseSoapEndpoint<ILesService>((opt) =>
                {
                    opt.Path = "/LESService.asmx";
                    opt.SoapSerializer = SoapSerializer.XmlSerializer;
                    opt.CaseInsensitivePath = true;
                    opt.EncoderOptions = new[] {
                    new SoapEncoderOptions() { MessageVersion = MessageVersion.Soap11, WriteEncoding = Encoding.UTF8,},
                };
                });
            });


        }
    }
}
