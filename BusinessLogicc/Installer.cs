using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic
{
    public static class Installer
    {
        public static void AddBuisnessServices(this IServiceCollection container)
        {
            container.AddScoped<IQuotationFiveService, QuotationFiveService>();
            // container.AddScoped<IQuotationOneService, QuotationOneService>();


        }
    }
}
