using System;
using System.Collections.Generic;
using System.Text;
using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using Domain;
using Storage.Interfaces;

namespace BusinessLogic.Services
{
    public class QuotationFiveService : BaseCrudService<Quotation>, IQuotationFiveService
    {
        public QuotationFiveService(IRepository<Quotation> repository) : base(repository)
        {
        }
    }
}
