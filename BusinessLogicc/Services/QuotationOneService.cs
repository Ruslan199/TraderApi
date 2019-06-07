using BusinessLogic.Interfaces;
using Domain;
using Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Services
{
    public class QuotationOneService : BaseCrudService<QuotationOne>, IQuotationOneService
    {
        public QuotationOneService(IRepository<QuotationOne> repository) : base(repository)
        {
        }
    }
}
