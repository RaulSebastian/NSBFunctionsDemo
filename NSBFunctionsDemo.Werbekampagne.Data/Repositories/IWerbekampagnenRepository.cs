using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSBFunctionsDemo.Werbekampagne.Data.Repositories
{
    public interface IWerbekampagnenRepository
    {
        Task Create(Domain.Model.Werbekampagne werbekampagne);
        Task Update(Domain.Model.Werbekampagne werbekampagne);
        Task<IEnumerable<Domain.Model.Werbekampagne>> GetAll();
        Task<Domain.Model.Werbekampagne> GetById(Guid id);
        Task Delete(Guid id);
    }
}