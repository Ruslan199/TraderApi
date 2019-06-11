using BusinessLogic.Interfaces;
using Domain;
using Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Services
{
    public class ActivationKeyService : BaseCrudService<ActivationKey>, IActivationKeyService
    {
        public ActivationKeyService(IRepository<ActivationKey> repository) : base(repository)
        {
        }
    }
}
