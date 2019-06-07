using BusinessLogic.Interfaces;
using Domain;
using Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Services
{
    public class PairService : BaseCrudService<Pair>, IPairService
    {
        public PairService(IRepository<Pair> repository) : base(repository)
        {
        }
    }
}
