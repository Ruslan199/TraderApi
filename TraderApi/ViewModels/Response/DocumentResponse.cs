using Binance.Net.Objects;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TraderApi.Models.Response;

namespace TraderApi.ViewModels.Response
{
    public class DocumentResponse : ResponseModel
    {
        public Quotation[] DataPrice { get; set; }
    }
}

