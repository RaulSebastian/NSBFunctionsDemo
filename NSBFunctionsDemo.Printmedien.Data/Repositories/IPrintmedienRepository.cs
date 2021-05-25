using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSBFunctionsDemo.Printmedien.Domain.Model;

namespace NSBFunctionsDemo.Printmedien.Data.Repositories
{
    public interface IPrintmedienRepository
    {
        Task Create(Druckauftrag druckauftrag);
        Task Update(Druckauftrag druckauftrag);
        Task<IEnumerable<Domain.Model.Druckauftrag>> GetAll();
        Task<Druckauftrag> GetById(Guid id);
        Task<Druckauftrag> GetForWerbekampagneId(Guid id);
        Task Delete(Guid id);
    }
}