using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic;
using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHibernate;
using Storage;
using TraderApi.BinanceTradeManager;
using TraderApi.Interface;
using TraderApi.WebSocketManager;

namespace TraderApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddBuisnessServices();
            services.AddNHibernate("Server=localhost;Port=3306;Uid=root;Pwd=qwerty27;Database=example;");
            services.AddCors();
            services.AddSingleton<ITimerService, TimerService>();
            // services.AddScoped<BinanceTradeManager.BinanceTradeManager>();
            services.AddHostedService<TimedHostedService>();
            services.AddWebSocketManager();
            // services.AddScoped<TimerRealAlgoritm>();
            // services.AddScoped<ITimerService, TimerService>();
            //services.AddScoped<TimerService>();
            // services.AddScoped<IQuotationFiveService, QuotationFiveService>();

            //services.AddScoped<BinanceTradeManager.BinanceTradeManagerOne>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            //var binanceTM2 = serviceProvider.GetService<BinanceTradeManager.BinanceTradeManagerOne>();
            //binanceTM2.SetServices(serviceProvider.GetService<IQuotationOneService>());
            app.UseCors(builder =>
               builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod());
            //app.UseHttpsRedirecxtion();
            app.UseMvc();
            app.UseWebSockets();
            var notifMessageHandler = serviceProvider.GetService<NotificationsMessageHandler>();
            app.MapWebSocketManager("/notifications", notifMessageHandler);
        }
    }
}
