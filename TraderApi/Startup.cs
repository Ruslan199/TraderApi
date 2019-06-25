using System;
using BusinessLogic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
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
            

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            // укзывает, будет ли валидироваться издатель при валидации токена
                            ValidateIssuer = true,
                            // строка, представляющая издателя
                            ValidIssuer = AuthOptions.ISSUER,

                            // будет ли валидироваться потребитель токена
                            ValidateAudience = true,
                            // установка потребителя токена
                            ValidAudience = AuthOptions.AUDIENCE,
                            // будет ли валидироваться время существования
                            ValidateLifetime = true,

                            // установка ключа безопасности
                            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                            // валидация ключа безопасности
                            ValidateIssuerSigningKey = true,
                        };
                    });

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
            app.UseAuthentication();
            app.UseMvc();
            app.UseWebSockets();
            
            var notifMessageHandler = serviceProvider.GetService<NotificationsMessageHandler>();
            app.MapWebSocketManager("/notifications", notifMessageHandler);
        }
    }
}
